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
using Mud.Wechat.Work.DataModels.ExternalContact.Statistics;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 客户统计域只读执行器：成员联系客户统计 + 客户群统计（按群主 / 按自然日）。
/// </summary>
/// <remarks>
/// <para>
/// <b>投影边界天然干净</b>：官方这一域只回聚合计数与比率，不含任何客户或成员身份，故执行器把字段整块投影，
/// 不需要像客户域那样逐字段剔除——「统计」本身即是脱敏后的形态。
/// </para>
/// <para>
/// <b>官方约束在调用前收口</b>：闭区间、跨度 ≤ 30 天、只保留最近 180 天、非零点取整到当日 0 点、
/// 按群主聚合的 <c>order_by</c> 闭集、<c>limit</c> ≤ 1000。越界时官方只回错误码，模型难以据此纠错。
/// </para>
/// </remarks>
/// <param name="statistics">客户统计域 SDK。</param>
public sealed class ExternalContactStatisticsToolHandlers(IWechatWorkExternalContactStatisticsService statistics)
{
    /// <summary>官方允许的时间跨度上限（30 天）。</summary>
    private const long MaxSpanSeconds = 30L * 24 * 60 * 60;

    /// <summary>行为统计单次可传的成员上限。</summary>
    private const int MaxUserCount = 100;

    /// <summary>群聊统计单页上限。</summary>
    private const int MaxPageSize = 1000;

    /// <summary>群聊统计官方默认单页条数。</summary>
    private const int DefaultPageSize = 500;

    private const string TimeFormatHint =
        "时间格式非法：时间参数应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"），且不接受无时区文本。";

    private readonly IWechatWorkExternalContactStatisticsService _statistics =
        statistics ?? throw new ArgumentNullException(nameof(statistics));

    /// <summary>执行 <c>externalcontact.statistics.user_behavior</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactStatisticsUserBehaviorTool))]
    public async Task<ToolResult> QueryUserBehaviorAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactStatisticsUserBehaviorArgs.Unpack(args);

        if (!ToolTimeText.TryParseUnixSeconds(request.StartTime, out var start))
        {
            throw WechatToolArgumentErrors.Invalid(TimeFormatHint);
        }

        if (!ToolTimeText.TryParseUnixSeconds(request.EndTime, out var end))
        {
            throw WechatToolArgumentErrors.Invalid(TimeFormatHint);
        }

        if (end <= start)
        {
            throw WechatToolArgumentErrors.Invalid("参数非法：end_time 必须大于 start_time。");
        }

        if (end - start > MaxSpanSeconds)
        {
            throw WechatToolArgumentErrors.Invalid("参数非法：起止时间间隔不得超过 30 天（官方按天聚合的上限），请缩小范围后分批查询。");
        }

        if (request.Userid is not { Length: > 0 } && request.DepartmentId is null)
        {
            throw WechatToolArgumentErrors.Invalid("参数非法：userid 与 department_id 至少要给一个（官方要求二者不可同时为空）。");
        }

        if (request.Userid is { Length: > MaxUserCount })
        {
            throw WechatToolArgumentErrors.Invalid($"参数非法：userid 一次最多 {MaxUserCount} 个，请分批查询。");
        }

        var response = await _statistics.GetUserBehaviorDataAsync(
            new GetUserBehaviorDataRequest
            {
                Userid = request.Userid is { Length: > 0 } users ? [.. users] : null,
                Partyid = request.DepartmentId is { } partyId ? [partyId] : null,
                StartTime = start,
                EndTime = end,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactStatisticsUserBehavior);
        }

        var days = new JsonArray();
        foreach (var item in outcome.Data?.BehaviorData ?? [])
        {
            days.Add((JsonNode?)new JsonObject
            {
                ["stat_time"] = JsonValue.Create(item.StatTime),
                ["new_apply_cnt"] = JsonValue.Create(item.NewApplyCnt),
                ["new_contact_cnt"] = JsonValue.Create(item.NewContactCnt),
                ["chat_cnt"] = JsonValue.Create(item.ChatCnt),
                ["message_cnt"] = JsonValue.Create(item.MessageCnt),
                ["reply_percentage"] = JsonValue.Create(item.ReplyPercentage),
                ["avg_reply_time"] = JsonValue.Create(item.AvgReplyTime),
                ["negative_feedback_cnt"] = JsonValue.Create(item.NegativeFeedbackCnt),
            });
        }

        // 空列表是合法答案（该区间内无数据或成员未开通客户联系），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["behavior_data"] = days,
            ["count"] = days.Count,
        }));
    }

    /// <summary>执行 <c>externalcontact.statistics.groupchat</c>。</summary>
    /// <param name="args">模型侧入参（<c>day_begin_time</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactStatisticsGroupChatTool))]
    public async Task<ToolResult> QueryGroupChatAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactStatisticsGroupchatArgs.Unpack(args);

        if (!TryResolveDayRange(request.DayBeginTime, request.DayEndTime, out var begin, out var end, out var error))
        {
            throw WechatToolArgumentErrors.Invalid(error!);
        }

        // order_by 的取值闭集（1~4）由声明面 EnumType 与生成的解包器收口——越界值到不了这里。
        var offset = request.Offset ?? 0;
        if (offset < 0)
        {
            throw WechatToolArgumentErrors.Invalid("参数非法：offset 不能为负数。");
        }

        var limit = request.Limit ?? DefaultPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            throw WechatToolArgumentErrors.Invalid($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var response = await _statistics.GetGroupChatStatisticAsync(
            new GetGroupChatStatisticRequest
            {
                DayBeginTime = begin,
                DayEndTime = end,
                OwnerFilter = ToOwnerFilter(request.OwnerUserid),
                OrderBy = request.OrderBy,
                OrderAsc = request.Ascending is { } asc ? (asc ? 1 : 0) : null,
                Offset = offset,
                Limit = limit,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactStatisticsGroupchat);
        }

        var items = new JsonArray();
        foreach (var item in outcome.Data?.Items ?? [])
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["owner"] = item.Owner,
                ["data"] = ToDataNode(item.Data),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["total"] = JsonValue.Create(outcome.Data?.Total),
            ["next_offset"] = JsonValue.Create(outcome.Data?.NextOffset),
            ["items"] = items,
            ["count"] = items.Count,
        }));
    }

    /// <summary>执行 <c>externalcontact.statistics.groupchat_by_day</c>。</summary>
    /// <param name="args">模型侧入参（<c>day_begin_time</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactStatisticsGroupChatByDayTool))]
    public async Task<ToolResult> QueryGroupChatByDayAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactStatisticsGroupchatByDayArgs.Unpack(args);

        if (!TryResolveDayRange(request.DayBeginTime, request.DayEndTime, out var begin, out var end, out var error))
        {
            throw WechatToolArgumentErrors.Invalid(error!);
        }

        var response = await _statistics.GetGroupChatStatisticGroupByDayAsync(
            new GetGroupChatStatisticGroupByDayRequest
            {
                DayBeginTime = begin,
                DayEndTime = end,
                OwnerFilter = ToOwnerFilter(request.OwnerUserid),
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactStatisticsGroupchatByDay);
        }

        var items = new JsonArray();
        foreach (var item in outcome.Data?.Items ?? [])
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["stat_time"] = JsonValue.Create(item.StatTime),
                ["data"] = ToDataNode(item.Data),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["items"] = items,
            ["count"] = items.Count,
        }));
    }

    /// <summary>解析并按官方跨度上限校验日期区间（未给结束日期时回落到起始日期）。</summary>
    private static bool TryResolveDayRange(
        string? beginText,
        string? endText,
        out long begin,
        out long? end,
        out string? error)
    {
        end = null;
        error = null;

        if (!ToolTimeText.TryParseUnixSeconds(beginText, out begin))
        {
            error = TimeFormatHint;
            return false;
        }

        if (string.IsNullOrWhiteSpace(endText))
        {
            return true;
        }

        if (!ToolTimeText.TryParseUnixSeconds(endText, out var parsedEnd))
        {
            error = TimeFormatHint;
            return false;
        }

        if (parsedEnd < begin)
        {
            error = "参数非法：day_end_time 不能早于 day_begin_time。";
            return false;
        }

        if (parsedEnd - begin > MaxSpanSeconds)
        {
            error = "参数非法：查询区间跨度不得超过 30 天（官方上限），请缩小范围后分批查询。";
            return false;
        }

        end = parsedEnd;
        return true;
    }

    /// <summary>群主过滤：无过滤项时返回 <see langword="null"/>（官方语义为「应用可见范围内全部群主」）。</summary>
    private static GroupChatStatisticOwnerFilter? ToOwnerFilter(string[]? ownerUserIds)
        => ownerUserIds is { Length: > 0 } ? new GroupChatStatisticOwnerFilter { UseridList = [.. ownerUserIds] } : null;

    /// <summary>群聊统计明细投影（按群主与按自然日两处共用同一字段集）。</summary>
    private static JsonNode? ToDataNode(GroupChatStatisticData? data)
        => data is null
            ? null
            : new JsonObject
            {
                ["new_chat_cnt"] = JsonValue.Create(data.NewChatCnt),
                ["chat_total"] = JsonValue.Create(data.ChatTotal),
                ["chat_has_msg"] = JsonValue.Create(data.ChatHasMsg),
                ["new_member_cnt"] = JsonValue.Create(data.NewMemberCnt),
                ["member_total"] = JsonValue.Create(data.MemberTotal),
                ["member_has_msg"] = JsonValue.Create(data.MemberHasMsg),
                ["msg_total"] = JsonValue.Create(data.MsgTotal),
                ["migrate_trainee_chat_cnt"] = JsonValue.Create(data.MigrateTraineeChatCnt),
            };
}
