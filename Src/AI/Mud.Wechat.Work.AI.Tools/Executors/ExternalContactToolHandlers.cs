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
using Mud.Wechat.Work.DataModels.ExternalContact.Customer;
using Mud.Wechat.Work.DataModels.ExternalContact.Tag;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 客户联系域只读执行器：成员入口 → 客户列表 → 客户详情，外加企业标签库。
/// </summary>
/// <remarks>
/// <para>
/// <b>三跳自举</b>：<c>follow_user.list</c>（谁开了客户联系）→ <c>contact.list</c>（该成员的客户 ID）→
/// <c>contact.get</c>（单个客户详情）。官方没有「按企业一把拉全客户」的公共读端点，工具面也不提供这种越权聚合。
/// </para>
/// <para>
/// <b>投影边界</b>：客户是<b>企业外部自然人</b>，其头像、unionid、备注手机号、视频号昵称一律不外发；
/// 跟进成员的备注描述（自由文本，可能含个人情况）同样不外发。只给业务事实：谁在跟进、什么时候添加、打了什么标签。
/// </para>
/// </remarks>
/// <param name="followUser">客户联系成员域 SDK。</param>
/// <param name="customer">客户管理域 SDK。</param>
/// <param name="tag">客户标签域 SDK。</param>
public sealed class ExternalContactToolHandlers(
    IWechatWorkExternalContactFollowUserService followUser,
    IWechatWorkExternalContactCustomerService customer,
    IWechatWorkExternalContactTagService tag)
{
    private readonly IWechatWorkExternalContactFollowUserService _followUser =
        followUser ?? throw new ArgumentNullException(nameof(followUser));

    private readonly IWechatWorkExternalContactCustomerService _customer =
        customer ?? throw new ArgumentNullException(nameof(customer));

    private readonly IWechatWorkExternalContactTagService _tag = tag ?? throw new ArgumentNullException(nameof(tag));

    /// <summary>执行 <c>externalcontact.follow_user.list</c>。</summary>
    /// <param name="args">模型侧入参（零参数，仍走解包以保持形状一致）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactFollowUserListTool))]
    public async Task<ToolResult> ListFollowUsersAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        ExternalcontactFollowUserListArgs.Unpack(args);

        var response = await _followUser.GetFollowUserListAsync(cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactFollowUserList);
        }

        var users = new JsonArray();
        foreach (var userid in outcome.Data?.FollowUser ?? [])
        {
            users.Add((JsonNode?)JsonValue.Create(userid));
        }

        // 空列表是合法答案（本应用可见范围内没有配置客户联系的成员），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["follow_user"] = users,
            ["count"] = users.Count,
        }));
    }

    /// <summary>执行 <c>externalcontact.contact.list</c>。</summary>
    /// <param name="args">模型侧入参（<c>userid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactContactListTool))]
    public async Task<ToolResult> ListCustomersAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactContactListArgs.Unpack(args);

        var response = await _customer.GetCustomerListAsync(request.Userid, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactContactList);
        }

        var customers = new JsonArray();
        foreach (var externalUserid in outcome.Data?.ExternalUserid ?? [])
        {
            customers.Add((JsonNode?)JsonValue.Create(externalUserid));
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["external_userid_list"] = customers,
            ["count"] = customers.Count,
        }));
    }

    /// <summary>执行 <c>externalcontact.contact.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>external_userid</c> 必填）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactContactGetTool))]
    public async Task<ToolResult> GetCustomerAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ExternalcontactContactGetArgs.Unpack(args);

        var response = await _customer.GetCustomerDetailAsync(request.ExternalUserid, null, cancellationToken)
            .ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactContactGet);
        }

        if (outcome.Data?.ExternalContact is not { } contact)
        {
            return WechatToolResults.NotFound(WechatToolNames.ExternalcontactContactGet);
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["external_userid"] = contact.ExternalUserid,
            ["name"] = contact.Name,
            ["type"] = JsonValue.Create(contact.Type),
            ["gender"] = JsonValue.Create(contact.Gender),
            ["corp_name"] = contact.CorpName,
            ["position"] = contact.Position,
            ["follow_users"] = ToFollowUserArray(outcome.Data.FollowUser),
            ["follow_user_count"] = outcome.Data.FollowUser?.Count ?? 0,
            // 该端点是「单对象 + 内嵌成员列表」而不是分页页：官方可能截断 follow_users，
            // 用 truncated 如实描述这个事实，不借用分页键（page_token 在此无处回传，会误导模型翻页）。
            ["follow_user_truncated"] = !string.IsNullOrEmpty(outcome.Data.NextCursor),
        }));
    }

    /// <summary>执行 <c>externalcontact.tag.list</c>。</summary>
    /// <param name="args">模型侧入参（零参数）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkExternalContactTagListTool))]
    public async Task<ToolResult> ListTagsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        ExternalcontactTagListArgs.Unpack(args);

        var response = await _tag.GetCorpTagListAsync(new GetCorpTagListRequest(), cancellationToken)
            .ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ExternalcontactTagList);
        }

        var groups = new JsonArray();
        var tagCount = 0;
        foreach (var group in outcome.Data?.TagGroup ?? [])
        {
            if (group.Deleted == true)
            {
                continue;
            }

            var tags = new JsonArray();
            foreach (var item in group.Tag ?? [])
            {
                if (item.Deleted == true)
                {
                    continue;
                }

                tags.Add((JsonNode?)new JsonObject
                {
                    ["id"] = item.Id,
                    ["name"] = item.Name,
                });
                tagCount++;
            }

            groups.Add((JsonNode?)new JsonObject
            {
                ["group_id"] = group.GroupId,
                ["group_name"] = group.GroupName,
                ["tags"] = tags,
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["tag_group"] = groups,
            ["tag_count"] = tagCount,
        }));
    }

    /// <summary>跟进关系投影：成员 + 备注名 + 添加时间/方式 + 状态 + 标签名，不含备注描述与手机号。</summary>
    private static JsonArray ToFollowUserArray(List<CustomerFollowUser>? followUsers)
    {
        var users = new JsonArray();
        foreach (var user in followUsers ?? [])
        {
            users.Add((JsonNode?)new JsonObject
            {
                ["userid"] = user.Userid,
                ["remark"] = user.Remark,
                ["createtime"] = JsonValue.Create(user.Createtime),
                ["add_way"] = JsonValue.Create(user.AddWay),
                ["state"] = user.State,
                ["tags"] = ToTagNameArray(user.Tags),
            });
        }

        return users;
    }

    /// <summary>跟进标签投影：只给标签名（客户标签 ID 对企业外部无意义，标签名才是模型要读的语义）。</summary>
    private static JsonArray ToTagNameArray(List<CustomerFollowTag>? tags)
    {
        var names = new JsonArray();
        foreach (var tag in tags ?? [])
        {
            if (!string.IsNullOrWhiteSpace(tag.TagName))
            {
                names.Add((JsonNode?)JsonValue.Create(tag.TagName));
            }
        }

        return names;
    }
}
