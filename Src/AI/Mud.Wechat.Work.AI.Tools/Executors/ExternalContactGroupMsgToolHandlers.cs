// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json.Nodes;
using Mud.AI.Abstractions.Tools.Handlers;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Agent.Results;
using Mud.Wechat.Work.AI.Tools.Curation;
using Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 客户群发域执行器：群发记录 → 成员执行任务 → 逐客户送达结果，外加一个受控写（创建群发）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三段式读面</b>：官方把「发了什么 / 谁发了 / 发给谁成没成」拆成三个端点，钥匙都是 <c>msgid</c>
/// （送达结果再加 <c>userid</c>）。工具面保持同一形状，不拼装「一次拉全」的假能力。
/// </para>
/// <para>
/// <b>投影边界</b>：附件只给 <c>msgtype</c>（<c>media_id</c> / <c>pic_url</c> 是宿主素材资产）；
/// 送达结果给 <c>external_userid</c> / <c>chat_id</c>——与 <c>contact.list</c> / <c>groupchat.list</c>
/// 同级的自有数据，但不含消息内容（官方此端点也不返回内容）。
/// </para>
/// <para>
/// <b>写路径收敛</b>：目标集必须显式（客户模式给 <c>external_userid</c>、群模式给 <c>chat_id_list</c>），
/// 不提供官方支持的「只给 sender = 全部客户/群」；附件不开放（模型产不出 <c>media_id</c>）。
/// </para>
/// </remarks>
/// <param name="groupMsg">客户群发域 SDK。</param>
public sealed class ExternalContactGroupMsgToolHandlers(IWechatWorkExternalContactGroupMsgService groupMsg)
{
    /// <summary>群发记录列表的官方单页上限。</summary>
    private const int MaxListPageSize = 100;

    /// <summary>群发记录列表的官方默认单页条数。</summary>
    private const int DefaultListPageSize = 50;

    /// <summary>执行任务/送达结果的官方单页上限。</summary>
    private const int MaxTaskPageSize = 1000;

    /// <summary>执行任务/送达结果的官方默认单页条数。</summary>
    private const int DefaultTaskPageSize = 500;

    /// <summary>官方允许的时间跨度上限（1 个月按 31 天计）。</summary>
    private const long MaxSpanSeconds = 31L * 24 * 60 * 60;

    /// <summary>客户群发单次最多可指定的客户数。</summary>
    private const int MaxCustomerPerTask = 10000;

    /// <summary>客户群群发单次最多可指定的群数。</summary>
    private const int MaxChatPerTask = 2000;

    /// <summary>官方文本长度上限（字节）。</summary>
    private const int MaxContentBytes = 4000;

    private const string TimeFormatHint =
        "时间格式非法：start_time / end_time 应为 Unix 秒（如 \"1700000000\"）或带时区的 ISO 8601（如 \"2026-10-10T09:00:00+08:00\"），且不接受无时区文本。";

    private readonly IWechatWorkExternalContactGroupMsgService _groupMsg =
        groupMsg ?? throw new ArgumentNullException(nameof(groupMsg));

    /// <summary>执行 <c>externalcontact.groupmsg.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactGroupMsgListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactGroupmsgListArgs.Unpack(args);

        if (!IsChatType(request.ChatType))
        {
            return ToolResult.FromError("参数非法：chat_type 只能取 single（发给客户）或 group（发给客户群）。");
        }

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
            return ToolResult.FromError("参数非法：起止时间间隔不得超过 1 个月（官方口径），请缩小范围后分批拉取。");
        }

        if (request.FilterType is { } filterType && filterType is < 0 or > 2)
        {
            return ToolResult.FromError("参数非法：filter_type 只能取 0（企业创建）/ 1（个人创建）/ 2（全部）。");
        }

        var limit = request.Limit ?? DefaultListPageSize;
        if (limit is < 1 or > MaxListPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxListPageSize} 之间。");
        }

        var response = await _groupMsg.GetGroupMsgListAsync(
            new GetGroupMsgListRequest
            {
                ChatType = request.ChatType,
                StartTime = start,
                EndTime = end,
                Creator = request.Creator,
                FilterType = request.FilterType,
                Limit = limit,
                Cursor = request.Cursor,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactGroupmsgList);
        }

        var records = new JsonArray();
        foreach (var item in outcome.Data?.GroupMsgList ?? [])
        {
            records.Add((JsonNode?)new JsonObject
            {
                ["msgid"] = item.MsgId,
                ["creator"] = item.Creator,
                ["create_time"] = JsonValue.Create(item.CreateTime),
                ["create_type"] = JsonValue.Create(item.CreateType),
                ["text"] = item.Text?.Content,
                ["attachment_types"] = ToAttachmentTypeArray(item.Attachments),
            });
        }

        // 空列表是合法答案（该区间内没有群发记录），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["group_msg_list"] = records,
            ["count"] = records.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>externalcontact.groupmsg.task_list</c>。</summary>
    /// <param name="args">模型侧入参（<c>msgid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactGroupMsgTaskListTool))]
    public async Task<ToolResult> ListTasksAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactGroupmsgTaskListArgs.Unpack(args);

        var limit = request.Limit ?? DefaultTaskPageSize;
        if (limit is < 1 or > MaxTaskPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxTaskPageSize} 之间。");
        }

        var response = await _groupMsg.GetGroupMsgTaskListAsync(
            new GetGroupMsgTaskListRequest
            {
                MsgId = request.Msgid,
                Limit = limit,
                Cursor = request.Cursor,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactGroupmsgTaskList);
        }

        var tasks = new JsonArray();
        foreach (var item in outcome.Data?.TaskList ?? [])
        {
            tasks.Add((JsonNode?)new JsonObject
            {
                ["userid"] = item.Userid,
                ["status"] = JsonValue.Create(item.Status),
                ["send_time"] = JsonValue.Create(item.SendTime),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["task_list"] = tasks,
            ["count"] = tasks.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>externalcontact.groupmsg.send_result</c>。</summary>
    /// <param name="args">模型侧入参（<c>msgid</c> / <c>userid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactGroupMsgSendResultTool))]
    public async Task<ToolResult> ListSendResultsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactGroupmsgSendResultArgs.Unpack(args);

        var limit = request.Limit ?? DefaultTaskPageSize;
        if (limit is < 1 or > MaxTaskPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxTaskPageSize} 之间。");
        }

        var response = await _groupMsg.GetGroupMsgSendResultAsync(
            new GetGroupMsgSendResultRequest
            {
                MsgId = request.Msgid,
                Userid = request.Userid,
                Limit = limit,
                Cursor = request.Cursor,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactGroupmsgSendResult);
        }

        var results = new JsonArray();
        foreach (var item in outcome.Data?.SendList ?? [])
        {
            results.Add((JsonNode?)new JsonObject
            {
                ["userid"] = item.Userid,
                ["external_userid"] = item.ExternalUserid,
                ["chat_id"] = item.ChatId,
                ["status"] = JsonValue.Create(item.Status),
                ["send_time"] = JsonValue.Create(item.SendTime),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["send_list"] = results,
            ["count"] = results.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>externalcontact.groupmsg.send</c>（受控写）。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactGroupMsgSendTool))]
    public async Task<ToolResult> SendAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactGroupmsgSendArgs.Unpack(args);

        if (!IsChatType(request.ChatType))
        {
            return ToolResult.FromError("参数非法：chat_type 只能取 single（发给客户）或 group（发给客户群）。");
        }

        var isGroup = string.Equals(request.ChatType, "group", StringComparison.OrdinalIgnoreCase);

        if (isGroup)
        {
            if (request.ChatIdList is not { Length: > 0 })
            {
                return ToolResult.FromError("参数非法：chat_type = group 时必须显式给出 chat_id_list（客户群 ID 数组）。");
            }

            if (request.ChatIdList.Length > MaxChatPerTask)
            {
                return ToolResult.FromError($"参数非法：chat_id_list 单次最多 {MaxChatPerTask} 个群，请分批创建群发。");
            }
        }
        else
        {
            if (request.ExternalUserid is not { Length: > 0 })
            {
                return ToolResult.FromError("参数非法：chat_type = single 时必须显式给出 external_userid（客户 ID 数组）。");
            }

            if (request.ExternalUserid.Length > MaxCustomerPerTask)
            {
                return ToolResult.FromError($"参数非法：external_userid 单次最多 {MaxCustomerPerTask} 个客户，请分批创建群发。");
            }
        }

        var byteCount = Encoding.UTF8.GetByteCount(request.Content);
        if (byteCount > MaxContentBytes)
        {
            return ToolResult.FromError($"参数非法：content 超过官方上限（{byteCount} > {MaxContentBytes} 字节），请精简后重试。");
        }

        var response = await _groupMsg.AddGroupMsgTemplateAsync(
            new AddGroupMsgTemplateRequest
            {
                ChatType = request.ChatType,
                Sender = request.Sender,
                ExternalUserid = isGroup ? null : [.. request.ExternalUserid!],
                ChatIdList = isGroup ? [.. request.ChatIdList!] : null,
                AllowSelect = request.AllowSelect,
                Text = new GroupMsgTextContent { Content = request.Content },
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactGroupmsgSend);
        }

        var failList = new JsonArray();
        foreach (var failed in outcome.Data?.FailList ?? [])
        {
            failList.Add((JsonNode?)JsonValue.Create(failed));
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["ok"] = true,
            ["msgid"] = outcome.Data?.MsgId,
            ["fail_list"] = failList,
            ["fail_count"] = failList.Count,
        }));
    }

    /// <summary>群发任务类型闭集（官方字符串取值，声明面表达不了 ⇒ 执行器收口）。</summary>
    private static bool IsChatType(string? chatType)
        => string.Equals(chatType, "single", StringComparison.OrdinalIgnoreCase)
           || string.Equals(chatType, "group", StringComparison.OrdinalIgnoreCase);

    /// <summary>附件类型投影：只给 <c>msgtype</c>，<c>media_id</c> / <c>pic_url</c> 属宿主素材资产。</summary>
    private static JsonArray ToAttachmentTypeArray(List<GroupMsgAttachment>? attachments)
    {
        var types = new JsonArray();
        foreach (var attachment in attachments ?? [])
        {
            if (!string.IsNullOrWhiteSpace(attachment.MsgType))
            {
                types.Add((JsonNode?)JsonValue.Create(attachment.MsgType));
            }
        }

        return types;
    }
}
