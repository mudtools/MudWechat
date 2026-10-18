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
using Mud.Wechat.Work.DataModels.Journal;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 汇报域只读执行器：记录列表 → 记录详情（自建口径，官方明确代开发/第三方暂不支持）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方约束在调用前收口</b>：起止间隔 ≤ 1 个月、单次 ≤ 100 条、<c>cursor</c> 首次为 0；
/// 越界时官方只回 301066，模型难以据此纠错。
/// </para>
/// <para>
/// <b>表单只取文本类取值</b>：成员/部门/文件/位置/表格/打卡/请假等控件一律不投影——它们直接指向个人数据与附件。
/// </para>
/// </remarks>
/// <param name="journal">汇报域 SDK（自建口径）。</param>
public sealed class JournalToolHandlers(IWechatWorkInternalJournalService journal)
{
    /// <summary>官方单次拉取上限。</summary>
    private const int MaxPageSize = 100;

    /// <summary>官方允许的时间跨度上限（一个月按 31 天计）。</summary>
    private const long MaxSpanSeconds = 31L * 24 * 60 * 60;

    private const string TimeFormatHint =
        "时间格式非法：start_time / end_time 应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"）。";

    private readonly IWechatWorkInternalJournalService _journal =
        journal ?? throw new ArgumentNullException(nameof(journal));

    /// <summary>执行 <c>journal.record.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkJournalRecordListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = JournalRecordListArgs.Unpack(args);

        if (!ToolTimeText.TryParseUnixSeconds(request.StartTime, out var start))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (!ToolTimeText.TryParseUnixSeconds(request.EndTime, out var end))
        {
            return ToolResult.FromError(TimeFormatHint);
        }

        if (end <= start)
        {
            return ToolResult.FromError("参数非法：end_time 必须大于 start_time。");
        }

        if (end - start > MaxSpanSeconds)
        {
            return ToolResult.FromError("参数非法：起止时间间隔不得超过 31 天（官方口径为一个月），请缩小范围后分批拉取。");
        }

        var limit = request.Limit ?? MaxPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var filters = new List<JournalRecordFilter>();
        if (!string.IsNullOrWhiteSpace(request.Creator))
        {
            filters.Add(new JournalRecordFilter { Key = "creator", Value = request.Creator });
        }

        if (!string.IsNullOrWhiteSpace(request.TemplateId))
        {
            filters.Add(new JournalRecordFilter { Key = "template_id", Value = request.TemplateId });
        }

        var response = await _journal.GetJournalRecordListAsync(
            new JournalRecordListRequest
            {
                StartTimestamp = start,
                EndTimestamp = end,
                Cursor = request.Cursor ?? 0,
                Limit = limit,
                FilterList = filters.Count == 0 ? null : filters,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取汇报记录列表失败。");
        }

        var uuids = new JsonArray();
        foreach (var uuid in outcome.Data?.JournalIdList ?? [])
        {
            uuids.Add((JsonNode?)JsonValue.Create(uuid));
        }

        // 空列表是合法答案（该时间段内没有汇报），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["journaluuid_list"] = uuids,
            ["count"] = uuids.Count,
            ["ending"] = JsonValue.Create(outcome.Data?.Ending),
            ["next_cursor"] = JsonValue.Create(outcome.Data?.NextCursor),
        }));
    }

    /// <summary>执行 <c>journal.record.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>journaluuid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkJournalRecordGetTool))]
    public async Task<ToolResult> GetAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = JournalRecordGetArgs.Unpack(args);

        var response = await _journal.GetJournalRecordDetailAsync(
            new JournalRecordDetailRequest { JournalId = request.Journaluuid },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.Info is not { } info)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "汇报记录不存在或无权访问（info 为空）。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["journal_uuid"] = info.JournalId,
            ["template_id"] = info.TemplateId,
            ["template_name"] = info.TemplateName,
            ["submitter"] = info.Submitter?.Userid,
            ["report_time"] = JsonValue.Create(info.ReportTimestamp),
            ["receiver_count"] = info.ReceiverList?.Count ?? 0,
            ["readed_count"] = info.ReadedReceiverList?.Count ?? 0,
            ["comment_count"] = info.CommentList?.Count ?? 0,
            ["journal_text"] = info.SysJournalData,
            ["fields"] = ToFieldArray(info.ApplyData),
        }));
    }

    /// <summary>表单字段投影：字段名 + 控件类型 + 文本类取值；成员/部门/文件/位置/表格等不投影。</summary>
    private static JsonArray ToFieldArray(JournalApplyData? applyData)
    {
        var fields = new JsonArray();
        foreach (var item in applyData?.ContentList ?? [])
        {
            var node = new JsonObject
            {
                ["name"] = item.Title?.FirstOrDefault()?.Text,
                ["control"] = item.Control,
            };

            if (item.Value is { } value)
            {
                if (!string.IsNullOrWhiteSpace(value.Text))
                {
                    node["text"] = value.Text;
                }

                if (!string.IsNullOrWhiteSpace(value.NewNumber))
                {
                    node["number"] = value.NewNumber;
                }

                if (!string.IsNullOrWhiteSpace(value.NewMoney))
                {
                    node["money"] = value.NewMoney;
                }

                if (value.Date?.Timestamp is { Length: > 0 } timestamp)
                {
                    node["date"] = timestamp;
                }

                if (value.Selector?.Options is { Count: > 0 } options)
                {
                    var selected = new JsonArray();
                    foreach (var option in options)
                    {
                        var label = option.Value?.FirstOrDefault()?.Text;
                        if (!string.IsNullOrWhiteSpace(label))
                        {
                            selected.Add((JsonNode?)JsonValue.Create(label));
                        }
                    }

                    node["selector"] = selected;
                }
            }

            fields.Add((JsonNode?)node);
        }

        return fields;
    }
}
