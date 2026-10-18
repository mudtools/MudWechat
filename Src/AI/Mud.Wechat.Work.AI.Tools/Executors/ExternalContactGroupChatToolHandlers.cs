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
using Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 客户群域只读执行器：客户群列表 → 客户群详情。
/// </summary>
/// <remarks>
/// <para>
/// <b>两跳自举</b>：官方 <c>groupchat/list</c> 只回 <c>chat_id</c> 与 <c>status</c>，群名/群主/成员必须再按单个
/// <c>chat_id</c> 调 <c>groupchat/get</c>；工具面保持同一形态，不拼装越权的「批量详情」。
/// </para>
/// <para>
/// <b>投影边界</b>：群成员里混着企业成员与外部客户，<c>unionid</c>（跨应用身份关联键）一律剔除；
/// 姓名只在模型显式要求（<c>need_name = true</c>）时才由官方返回，工具不额外取。
/// </para>
/// </remarks>
/// <param name="groupChat">客户群域 SDK。</param>
public sealed class ExternalContactGroupChatToolHandlers(IWechatWorkExternalContactGroupChatService groupChat)
{
    /// <summary>官方单页上限。</summary>
    private const int MaxPageSize = 1000;

    /// <summary>官方默认单页条数。</summary>
    private const int DefaultPageSize = 500;

    private readonly IWechatWorkExternalContactGroupChatService _groupChat =
        groupChat ?? throw new ArgumentNullException(nameof(groupChat));

    /// <summary>执行 <c>externalcontact.groupchat.list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactGroupChatListTool))]
    public async Task<ToolResult> ListAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactGroupchatListArgs.Unpack(args);

        // status_filter 的取值闭集（0~3）由声明面 EnumType 与生成的解包器收口——越界值到不了这里。
        var limit = request.Limit ?? DefaultPageSize;
        if (limit is < 1 or > MaxPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxPageSize} 之间。");
        }

        var ownerFilter = ToOwnerFilter(request.OwnerUserid, request.OwnerPartyid);

        var response = await _groupChat.GetGroupChatListAsync(
            new GetGroupChatListRequest
            {
                StatusFilter = request.StatusFilter,
                OwnerFilter = ownerFilter,
                Cursor = request.Cursor,
                Limit = limit,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取客户群列表失败。");
        }

        var chats = new JsonArray();
        foreach (var item in outcome.Data?.GroupChatList ?? [])
        {
            chats.Add((JsonNode?)new JsonObject
            {
                ["chat_id"] = item.ChatId,
                ["status"] = JsonValue.Create(item.Status),
            });
        }

        // 空列表是合法答案（该过滤条件下没有客户群），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["group_chat_list"] = chats,
            ["count"] = chats.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    /// <summary>执行 <c>externalcontact.groupchat.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>chat_id</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactGroupChatGetTool))]
    public async Task<ToolResult> GetAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactGroupchatGetArgs.Unpack(args);

        var response = await _groupChat.GetGroupChatDetailAsync(
            new GetGroupChatDetailRequest
            {
                ChatId = request.ChatId,
                NeedName = request.NeedName == true ? 1 : 0,
            },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.GroupChat is not { } chat)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "客户群不存在或无权访问（group_chat 为空）。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["chat_id"] = chat.ChatId,
            ["name"] = chat.Name,
            ["owner"] = chat.Owner,
            ["create_time"] = JsonValue.Create(chat.CreateTime),
            ["notice"] = chat.Notice,
            ["member_count"] = chat.MemberList?.Count ?? 0,
            ["members"] = ToMemberArray(chat.MemberList),
            ["admin_list"] = ToAdminArray(chat.AdminList),
            ["member_version"] = chat.MemberVersion,
        }));
    }

    /// <summary>群主过滤条件：无任何过滤项时返回 <see langword="null"/>（官方语义为「全部群主」）。</summary>
    private static GroupChatOwnerFilter? ToOwnerFilter(string[]? userIds, int? partyId)
    {
        var hasUsers = userIds is { Length: > 0 };
        var hasParty = partyId is not null;
        if (!hasUsers && !hasParty)
        {
            return null;
        }

        return new GroupChatOwnerFilter
        {
            UserIdList = hasUsers ? [.. userIds!] : null,
            PartyIdList = hasParty ? [partyId!.Value] : null,
        };
    }

    /// <summary>群成员投影：身份与入群事实；<c>unionid</c> 不外发。</summary>
    private static JsonArray ToMemberArray(List<GroupChatMember>? members)
    {
        var items = new JsonArray();
        foreach (var member in members ?? [])
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["userid"] = member.UserId,
                ["type"] = JsonValue.Create(member.Type),
                ["join_time"] = JsonValue.Create(member.JoinTime),
                ["join_scene"] = JsonValue.Create(member.JoinScene),
                ["invitor"] = member.Invitor?.UserId,
                ["group_nickname"] = member.GroupNickname,
                ["name"] = member.Name,
            });
        }

        return items;
    }

    /// <summary>管理员投影：只给 userid。</summary>
    private static JsonArray ToAdminArray(List<GroupChatAdmin>? admins)
    {
        var items = new JsonArray();
        foreach (var admin in admins ?? [])
        {
            if (!string.IsNullOrWhiteSpace(admin.UserId))
            {
                items.Add((JsonNode?)JsonValue.Create(admin.UserId));
            }
        }

        return items;
    }
}
