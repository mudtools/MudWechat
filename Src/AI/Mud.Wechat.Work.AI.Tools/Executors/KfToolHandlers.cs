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
using Mud.Wechat.Work.DataModels.Kf;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 微信客服执行器：账号入口 → 接待人员 → 会话状态 → 文本消息（受控写）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只承接状态面，不承接消息正文</b>：官方 <c>sync_msg</c> 能拉取聊天内容（含客户原文），属会话内容面；
/// 工具面只回答「谁在接待 / 什么状态」，聊天正文留给宿主的合规通道。
/// </para>
/// <para>
/// <b>发送是受控写</b>：对外可见且不可撤回，走白名单 + 授权 + 审批三件套；执行器只负责参数收口与结果归一，
/// 不放宽任何一档。
/// </para>
/// </remarks>
/// <param name="accounts">客服账号 SDK。</param>
/// <param name="servicers">接待人员 SDK。</param>
/// <param name="sessions">会话与消息 SDK。</param>
/// <param name="statistics">客服数据统计 SDK。</param>
public sealed class KfToolHandlers(
    IWechatWorkKfAccountService accounts,
    IWechatWorkKfServicerService servicers,
    IWechatWorkKfSessionService sessions,
    IWechatWorkKfStatisticsService statistics)
{
    private const int MaxPageSize = 100;

    /// <summary>客服统计的官方时间跨度上限（闭区间，31 天）。</summary>
    private const long MaxStatSpanSeconds = 31L * 24 * 60 * 60;

    private const string StatTimeFormatHint =
        "时间格式非法：start_time / end_time 应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"），且不接受无时区文本。";

    private readonly IWechatWorkKfAccountService _accounts = accounts ?? throw new ArgumentNullException(nameof(accounts));

    private readonly IWechatWorkKfServicerService _servicers =
        servicers ?? throw new ArgumentNullException(nameof(servicers));

    private readonly IWechatWorkKfSessionService _sessions =
        sessions ?? throw new ArgumentNullException(nameof(sessions));

    private readonly IWechatWorkKfStatisticsService _statistics =
        statistics ?? throw new ArgumentNullException(nameof(statistics));

    /// <summary>执行 <c>kf.account.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkKfAccountListTool))]
    public async Task<ToolResult> ListAccountsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = KfAccountListArgs.Unpack(args);

        var limit = request.Limit ?? MaxPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            throw WechatToolArgumentErrors.Invalid($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var response = await _accounts.GetAccountListAsync(
            new GetKfAccountListRequest { Offset = request.Offset, Limit = limit },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.KfAccountList);
        }

        var list = new JsonArray();
        foreach (var account in outcome.Data?.AccountList ?? [])
        {
            list.Add((JsonNode?)new JsonObject
            {
                ["open_kfid"] = account.OpenKfId,
                ["name"] = account.Name,
                ["manage_privilege"] = JsonValue.Create(account.ManagePrivilege),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["account_list"] = list,
            ["count"] = list.Count,
        }));
    }

    /// <summary>执行 <c>kf.servicer.list</c>。</summary>
    /// <param name="args">模型侧入参（<c>open_kfid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkKfServicerListTool))]
    public async Task<ToolResult> ListServicersAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = KfServicerListArgs.Unpack(args);

        var response = await _servicers.GetServicerListAsync(request.OpenKfid, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.KfServicerList);
        }

        var list = new JsonArray();
        foreach (var servicer in outcome.Data?.ServicerList ?? [])
        {
            list.Add((JsonNode?)new JsonObject
            {
                ["userid"] = servicer.UserId,
                ["status"] = JsonValue.Create(servicer.Status),
                ["stop_type"] = JsonValue.Create(servicer.StopType),
                ["department_id"] = JsonValue.Create(servicer.DepartmentId),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["servicer_list"] = list,
            ["count"] = list.Count,
        }));
    }

    /// <summary>执行 <c>kf.session.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>open_kfid</c> / <c>external_userid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkKfSessionGetTool))]
    public async Task<ToolResult> GetSessionAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = KfSessionGetArgs.Unpack(args);

        var response = await _sessions.GetServiceStateAsync(
            new GetKfServiceStateRequest
            {
                OpenKfId = request.OpenKfid,
                ExternalUserId = request.ExternalUserid,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.KfSessionGet);
        }

        if (outcome.Data is not { } state)
        {
            return WechatToolResults.NotFound(WechatToolNames.KfSessionGet);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["service_state"] = JsonValue.Create(state.ServiceState),
            ["service_state_desc"] = DescribeServiceState(state.ServiceState),
            ["servicer_userid"] = state.ServicerUserId,
        }));
    }

    /// <summary>执行 <c>kf.message.send</c>（受控写）。</summary>
    /// <param name="args">模型侧入参（<c>open_kfid</c> / <c>touser</c> / <c>content</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkKfMessageSendTool))]
    public async Task<ToolResult> SendMessageAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = KfMessageSendArgs.Unpack(args);

        // content / touser / open_kfid 的「必填且非空白」由声明面 Required 与生成的解包器收口。
        var response = await _sessions.SendTextMessageAsync(
            new SendKfTextMsgRequest
            {
                OpenKfId = request.OpenKfid,
                ToUser = request.Touser,
                MsgType = "text",
                Text = new KfTextMsgBody { Content = request.Content },
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.KfMessageSend);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["msgid"] = outcome.Data?.MsgId,
        }));
    }

    /// <summary>官方 <c>service_state</c> 取值的中文说明（把语义写死在代码里，避免模型各说各话）。</summary>
    private static string DescribeServiceState(int? state) => state switch
    {
        0 => "未处理",
        1 => "由智能助手接待",
        2 => "待接入池排队中",
        3 => "由人工接待",
        4 => "已结束",
        5 => "待接入",
        _ => "未知状态",
    };

    /// <summary>执行 <c>kf.corp_statistic</c>。</summary>
    /// <param name="args">模型侧入参（<c>open_kfid</c> / 时间范围必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkKfCorpStatisticTool))]
    public async Task<ToolResult> GetCorpStatisticAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = KfCorpStatisticArgs.Unpack(args);

        if (!TryResolveStatRange(request.StartTime, request.EndTime, out var start, out var end, out var error))
        {
            throw WechatToolArgumentErrors.Invalid(error!);
        }

        var response = await _statistics.GetCorpStatisticAsync(
            new GetKfCorpStatisticRequest
            {
                OpenKfId = request.OpenKfid,
                StartTime = start,
                EndTime = end,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.KfCorpStatistic);
        }

        var days = new JsonArray();
        foreach (var item in outcome.Data?.StatisticList ?? [])
        {
            days.Add((JsonNode?)new JsonObject
            {
                ["stat_time"] = JsonValue.Create(item.StatTime),
                ["statistic"] = ToCorpStatisticNode(item.Statistic),
            });
        }

        // 空列表是合法答案（该区间内无统计数据），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["statistic_list"] = days,
            ["count"] = days.Count,
        }));
    }

    /// <summary>执行 <c>kf.servicer_statistic</c>。</summary>
    /// <param name="args">模型侧入参（<c>open_kfid</c> / 时间范围必填，<c>servicer_userid</c> 可选）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkKfServicerStatisticTool))]
    public async Task<ToolResult> GetServicerStatisticAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = KfServicerStatisticArgs.Unpack(args);

        if (!TryResolveStatRange(request.StartTime, request.EndTime, out var start, out var end, out var error))
        {
            throw WechatToolArgumentErrors.Invalid(error!);
        }

        var response = await _statistics.GetServicerStatisticAsync(
            new GetKfServicerStatisticRequest
            {
                OpenKfId = request.OpenKfid,
                ServicerUserId = request.ServicerUserid,
                StartTime = start,
                EndTime = end,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.KfServicerStatistic);
        }

        var days = new JsonArray();
        foreach (var item in outcome.Data?.StatisticList ?? [])
        {
            days.Add((JsonNode?)new JsonObject
            {
                ["stat_time"] = JsonValue.Create(item.StatTime),
                ["statistic"] = ToServicerStatisticNode(item.Statistic),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["statistic_list"] = days,
            ["count"] = days.Count,
        }));
    }

    /// <summary>解析并按官方跨度上限（31 天闭区间）校验统计时间范围。</summary>
    private static bool TryResolveStatRange(
        string? startText,
        string? endText,
        out long start,
        out long end,
        out string? error)
    {
        end = 0;
        error = null;

        if (!ToolTimeText.TryParseUnixSeconds(startText, out start))
        {
            error = StatTimeFormatHint;
            return false;
        }

        if (!ToolTimeText.TryParseUnixSeconds(endText, out end))
        {
            error = StatTimeFormatHint;
            return false;
        }

        if (end <= start)
        {
            error = "参数非法：end_time 必须大于 start_time。";
            return false;
        }

        if (end - start > MaxStatSpanSeconds)
        {
            error = "参数非法：起止时间间隔不得超过 31 天（官方上限），请缩小范围后分批查询。";
            return false;
        }

        return true;
    }

    /// <summary>企业汇总指标投影（无数据的指标官方缺省，此处按 null 投影）。</summary>
    private static JsonNode? ToCorpStatisticNode(KfCorpStatisticData? data)
        => data is null
            ? null
            : new JsonObject
            {
                ["session_cnt"] = JsonValue.Create(data.SessionCount),
                ["customer_cnt"] = JsonValue.Create(data.CustomerCount),
                ["customer_msg_cnt"] = JsonValue.Create(data.CustomerMsgCount),
                ["upgrade_service_customer_cnt"] = JsonValue.Create(data.UpgradeServiceCustomerCount),
                ["ai_session_reply_cnt"] = JsonValue.Create(data.AiSessionReplyCount),
                ["ai_transfer_rate"] = JsonValue.Create(data.AiTransferRate),
                ["ai_knowledge_hit_rate"] = JsonValue.Create(data.AiKnowledgeHitRate),
                ["msg_rejected_customer_cnt"] = JsonValue.Create(data.MsgRejectedCustomerCount),
            };

    /// <summary>接待人员指标投影（含回复率、首次响应时长与满意度三项）。</summary>
    private static JsonNode? ToServicerStatisticNode(KfServicerStatisticData? data)
        => data is null
            ? null
            : new JsonObject
            {
                ["session_cnt"] = JsonValue.Create(data.SessionCount),
                ["customer_cnt"] = JsonValue.Create(data.CustomerCount),
                ["customer_msg_cnt"] = JsonValue.Create(data.CustomerMsgCount),
                ["reply_rate"] = JsonValue.Create(data.ReplyRate),
                ["first_reply_average_sec"] = JsonValue.Create(data.FirstReplyAverageSec),
                ["satisfaction_investgate_count"] = JsonValue.Create(data.SatisfactionInvestgateCount),
                ["satisfaction_participation_rate"] = JsonValue.Create(data.SatisfactionParticipationRate),
                ["satisfied_rate"] = JsonValue.Create(data.SatisfiedRate),
                ["middling_rate"] = JsonValue.Create(data.MiddlingRate),
                ["dissatisfied_rate"] = JsonValue.Create(data.DissatisfiedRate),
                ["upgrade_service_customer_cnt"] = JsonValue.Create(data.UpgradeServiceCustomerCount),
                ["upgrade_service_member_invite_count"] = JsonValue.Create(data.UpgradeServiceMemberInviteCount),
                ["upgrade_service_member_customer_count"] = JsonValue.Create(data.UpgradeServiceMemberCustomerCount),
                ["upgrade_service_groupchat_invite_count"] = JsonValue.Create(data.UpgradeServiceGroupchatInviteCount),
                ["upgrade_service_groupchat_customer_count"] = JsonValue.Create(data.UpgradeServiceGroupchatCustomerCount),
                ["msg_rejected_customer_cnt"] = JsonValue.Create(data.MsgRejectedCustomerCount),
            };
}
