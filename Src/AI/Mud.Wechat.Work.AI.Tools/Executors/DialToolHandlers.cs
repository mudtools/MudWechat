// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.Dial;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 公费电话域只读执行器：拨打记录（自建口径，代开发 / 第三方软缺席）。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影边界</b>：主被叫 <c>userid</c>、通话类型与时长进模型可见面；被叫的<b>电话号码不外发</b>
/// （外部号码是个人数据，与成员域手机号/邮箱同一条边界）。
/// </para>
/// <para>
/// <b>起止时间是「同时给或同时不给」</b>：官方对「只给一端」的语义是隐含默认值，工具面把它显式化——
/// 半份时间范围比没有时间范围更容易让模型误判查询窗口。
/// </para>
/// </remarks>
/// <param name="dial">公费电话域 SDK（自建口径）。</param>
public sealed class DialToolHandlers(IWechatWorkInternalDialService dial)
{
    /// <summary>工具侧单页上限（官方未明示，取企微分页的常规上限）。</summary>
    private const int MaxPageSize = 1000;

    /// <summary>工具侧默认单页条数。</summary>
    private const int DefaultPageSize = 100;

    /// <summary>官方允许的时间跨度上限（30 天）。</summary>
    private const long MaxSpanSeconds = 30L * 24 * 60 * 60;

    private const string TimeFormatHint =
        "时间格式非法：start_time / end_time 应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"），且不接受无时区文本。";

    private readonly IWechatWorkInternalDialService _dial = dial ?? throw new ArgumentNullException(nameof(dial));

    /// <summary>执行 <c>dial.record.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkDialRecordListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = DialRecordListArgs.Unpack(args);

        long? start = null;
        long? end = null;
        if (!string.IsNullOrWhiteSpace(request.StartTime) || !string.IsNullOrWhiteSpace(request.EndTime))
        {
            if (string.IsNullOrWhiteSpace(request.StartTime) || string.IsNullOrWhiteSpace(request.EndTime))
            {
                return ToolResult.FromError("参数非法：start_time 与 end_time 需同时给出（都省略时官方默认查最近 30 天）。");
            }

            if (!ToolTimeText.TryParseUnixSeconds(request.StartTime, out var parsedStart))
            {
                return ToolResult.FromError(TimeFormatHint);
            }

            if (!ToolTimeText.TryParseUnixSeconds(request.EndTime, out var parsedEnd))
            {
                return ToolResult.FromError(TimeFormatHint);
            }

            if (parsedEnd < parsedStart)
            {
                return ToolResult.FromError("参数非法：end_time 不得小于 start_time（官方双闭区间）。");
            }

            if (parsedEnd - parsedStart > MaxSpanSeconds)
            {
                return ToolResult.FromError("参数非法：起止时间间隔不得超过 30 天（官方上限），请缩小范围后分批查询。");
            }

            start = parsedStart;
            end = parsedEnd;
        }

        var offset = request.Offset ?? 0;
        if (offset < 0)
        {
            return ToolResult.FromError("参数非法：offset 不能为负数。");
        }

        var limit = request.Limit ?? DefaultPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var response = await _dial.GetDialRecordAsync(
            new GetDialRecordRequest
            {
                StartTime = start,
                EndTime = end,
                Offset = offset,
                Limit = limit,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取公费电话拨打记录失败。");
        }

        var records = new JsonArray();
        foreach (var item in outcome.Data?.Record ?? [])
        {
            records.Add((JsonNode?)new JsonObject
            {
                ["call_time"] = JsonValue.Create(item.CallTime),
                ["total_duration"] = JsonValue.Create(item.TotalDuration),
                ["call_type"] = JsonValue.Create(item.CallType),
                ["caller"] = ToPartyNode(item.Caller),
                ["callee"] = ToCalleeArray(item.Callee),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["record"] = records,
            ["count"] = records.Count,
            ["has_more"] = records.Count >= limit,
        }));
    }

    /// <summary>主叫投影：userid + 时长。</summary>
    private static JsonNode? ToPartyNode(DialCaller? caller)
        => caller is null
            ? null
            : new JsonObject
            {
                ["userid"] = caller.Userid,
                ["duration"] = JsonValue.Create(caller.Duration),
            };

    /// <summary>被叫投影：userid + 时长；<b>电话号码不外发</b>（外部号码是个人数据）。</summary>
    private static JsonArray ToCalleeArray(List<DialCallee>? callees)
    {
        var items = new JsonArray();
        foreach (var callee in callees ?? [])
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["userid"] = callee.Userid,
                ["duration"] = JsonValue.Create(callee.Duration),
            });
        }

        return items;
    }
}
