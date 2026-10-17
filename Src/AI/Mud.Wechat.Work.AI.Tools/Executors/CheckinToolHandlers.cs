// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.Checkin;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 打卡记录域执行器：<c>checkin.record.list</c> 落到
/// <see cref="IWechatWorkInternalCheckinRecordService.GetCheckinDataAsync"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方约束在调用前收口</b>：时间跨度 ≤ 30 天、成员 ≤ 100 人、<c>endtime &gt; starttime</c>——越界时官方只回
/// 一个错误码，模型难以据此纠错；执行器先校验并回填可读文案。
/// </para>
/// <para>
/// <b>位置与设备信息不外发</b>：<c>lat</c>/<c>lng</c>/<c>location_*</c>/<c>wifiname</c>/<c>wifimac</c>/<c>notes</c>/
/// <c>mediaids</c>/<c>deviceid</c> 一律不投影——它们是行踪与设备指纹，一旦进入提示词与日志即不可撤回。
/// 只回填考勤事实：谁、哪一次打卡、类型、异常、标准打卡时间、所属规则/班次。
/// </para>
/// </remarks>
/// <param name="checkin">打卡记录域 SDK（自建口径；应用上下文由回合作用域给定）。</param>
public sealed class CheckinToolHandlers(IWechatWorkInternalCheckinRecordService checkin)
{
    /// <summary>官方单次查询成员上限。</summary>
    private const int MaxUserCount = 100;

    /// <summary>官方允许的查询时间跨度（30 天）。</summary>
    private const long MaxSpanSeconds = 30L * 24 * 60 * 60;

    /// <summary>打卡类型默认值：3 = 全部打卡。</summary>
    private const int AllCheckinTypes = 3;

    private const string TimeFormatHint =
        "时间格式非法：start_time / end_time 应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"）。";

    private readonly IWechatWorkInternalCheckinRecordService _checkin =
        checkin ?? throw new ArgumentNullException(nameof(checkin));

    /// <summary>执行 <c>checkin.record.list</c>。</summary>
    /// <param name="args">模型侧入参（<c>userid_list</c> / <c>start_time</c> / <c>end_time</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkCheckinRecordListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = CheckinRecordListArgs.Unpack(args);

        if (request.UseridList.Length > MaxUserCount)
        {
            return ToolResult.FromError($"参数非法：userid_list 一次最多 {MaxUserCount} 个，请分批查询。");
        }

        if (!TryParseTime(request.StartTime, out var start))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (!TryParseTime(request.EndTime, out var end))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (end <= start)
        {
            return ToolResult.FromError("参数非法：end_time 必须大于 start_time。");
        }

        if (end - start > MaxSpanSeconds)
        {
            return ToolResult.FromError("参数非法：时间跨度不得超过 30 天，请缩小范围后分批查询。");
        }

        var dataType = request.DataType ?? AllCheckinTypes;
        if (dataType is < 1 or > 3)
        {
            return ToolResult.FromError("参数非法：data_type 只能是 1（上下班打卡）、2（外出打卡）或 3（全部打卡）。");
        }

        var response = await _checkin.GetCheckinDataAsync(
            new GetCheckinDataRequest
            {
                Opencheckindatatype = dataType,
                Starttime = start,
                Endtime = end,
                UserIdList = [.. request.UseridList],
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取打卡记录失败。");
        }

        var records = new JsonArray();
        foreach (var record in outcome.Data?.Checkindata ?? [])
        {
            records.Add((JsonNode?)new JsonObject
            {
                ["userid"] = record.Userid,
                ["checkin_type"] = record.CheckinType,
                ["exception_type"] = record.ExceptionType,
                ["checkin_time"] = JsonValue.Create(record.CheckinTime),
                ["sch_checkin_time"] = JsonValue.Create(record.SchCheckinTime),
                ["groupname"] = record.Groupname,
                ["groupid"] = JsonValue.Create(record.Groupid),
                ["schedule_id"] = JsonValue.Create(record.ScheduleId),
            });
        }

        // 空记录是合法答案（该时间段内这些成员没有打卡），不转错误——否则模型会把「没打卡」误读成「查不到」。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["checkindata"] = records,
            ["count"] = records.Count,
        }));
    }

    /// <summary>解析时间入参：Unix 秒，或<b>带时区</b>的 ISO 8601（无时区不猜，直接判非法以免跨主机语义漂移）。</summary>
    private static bool TryParseTime(string? raw, out long seconds)
    {
        seconds = 0;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var text = raw.Trim();
        if (long.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var numeric))
        {
            if (numeric <= 0)
            {
                return false;
            }

            seconds = numeric;
            return true;
        }

        // 日期段的 '-' 在 'T' 之前；'T' 之后出现的 '+/-' 或末尾的 'Z' 才是时区偏移。
        var tIndex = text.IndexOf('T');
        var hasOffset = text.EndsWith("Z", StringComparison.OrdinalIgnoreCase)
            || (tIndex >= 0 && (text.IndexOf('+', tIndex) >= 0 || text.LastIndexOf('-') > tIndex));
        if (!hasOffset
            || !DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var when))
        {
            return false;
        }

        seconds = when.ToUnixTimeSeconds();
        return true;
    }
}
