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
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 通讯录「成员」域执行器：<c>contacts.user.*</c> 三个只读工具落到
/// <see cref="IWechatWorkUsersService"/> 的强类型端点上。
/// </summary>
/// <remarks>
/// <para>
/// <b>字段投影是白名单而非黑名单</b>：执行器只投影 <c>userid / 姓名 / 别名 / 职务 / 部门 / 主部门 / 状态 / 直属上级</c>，
/// <b>不外发</b>手机号、邮箱、企业邮箱、头像、地址、二维码。理由是「模型上下文的留存期远长于一次请求」——
/// 个人信息一旦进了提示词与日志，删不掉；需要这些字段的场景应由宿主经 oauth2 授权后专线提供。
/// </para>
/// <para>
/// <b>fetch_child 的类型收敛</b>：SDK 以 <c>int?</c>（1/0）表达递归开关，模型侧暴露为 <c>bool</c>
/// （语义清晰、避免模型填 2 或 <c>"1"</c>）；映射在 <see cref="ToFetchChild"/> 一处完成。
/// </para>
/// </remarks>
/// <param name="users">成员域 SDK（应用上下文由回合作用域给定）。</param>
public sealed class ContactsUserToolHandlers(IWechatWorkUsersService users)
{
    /// <summary><c>user/list_id</c> 的官方单页上限。</summary>
    private const int MaxIdPageSize = 10000;

    /// <summary><c>user/list_id</c> 的默认单页条数（官方上限 1 万，工具面取更保守的默认值）。</summary>
    private const int DefaultIdPageSize = 1000;

    private readonly IWechatWorkUsersService _users = users ?? throw new ArgumentNullException(nameof(users));

    /// <summary>执行 <c>contacts.user.get</c>。</summary>
    /// <param name="args">模型侧入参（<c>userid</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsUserGetTool))]
    public async Task<ToolResult> GetUserAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ContactsUserGetArgs.Unpack(args);

        var response = await _users.GetUserAsync(request.Userid, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data is not { } user)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "成员不存在或不在应用可见范围内。");
        }

        return ToolResult.FromText(ToolResultJson.ToText(ProjectUser(user)));
    }

    /// <summary>执行 <c>contacts.user.simple_list</c>。</summary>
    /// <param name="args">模型侧入参（<c>department_id</c> 必填、<c>fetch_child</c> 可选）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsUserSimpleListTool))]
    public async Task<ToolResult> ListUserSummariesAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ContactsUserSimpleListArgs.Unpack(args);

        var response = await _users
            .GetUserSimpleListAsync(request.DepartmentId, ToFetchChild(request.FetchChild), cancellationToken)
            .ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.UserList is not { } list)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "该部门下没有可见成员（userlist 为空）。");
        }

        var items = new JsonArray();
        foreach (var user in list)
        {
            items.Add((JsonNode)new JsonObject
            {
                ["userid"] = user.UserId,
                ["name"] = user.Name,
                ["department"] = ToJsonArray(user.Department),
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject { ["userlist"] = items, ["count"] = items.Count }));
    }

    /// <summary>执行 <c>contacts.user.list</c>。</summary>
    /// <param name="args">模型侧入参（<c>department_id</c> 必填、<c>fetch_child</c> 可选）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsUserListTool))]
    public async Task<ToolResult> ListUsersAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ContactsUserListArgs.Unpack(args);

        var response = await _users
            .GetUserDetailListAsync(request.DepartmentId, ToFetchChild(request.FetchChild), cancellationToken)
            .ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.UserList is not { } list)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "该部门下没有可见成员（userlist 为空）。");
        }

        var items = new JsonArray();
        foreach (var user in list)
        {
            items.Add((JsonNode)ProjectUser(user, includeLeaders: false));
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject { ["userlist"] = items, ["count"] = items.Count }));
    }

    /// <summary>执行 <c>contacts.user.id_list</c>。</summary>
    /// <param name="args">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsUserIdListTool))]
    public async Task<ToolResult> ListIdsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ContactsUserIdListArgs.Unpack(args);

        var limit = request.Limit ?? DefaultIdPageSize;
        if (limit is < 1 or > MaxIdPageSize)
        {
            return ToolResult.FromError($"参数非法：limit 必须在 1~{MaxIdPageSize} 之间。");
        }

        var response = await _users.ListUserIdsAsync(
            new ListUserIdsRequest { Cursor = request.Cursor, Limit = limit },
            cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "拉取成员 ID 列表失败。");
        }

        var items = new JsonArray();
        foreach (var item in outcome.Data?.DeptUser ?? [])
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["userid"] = item.UserId,
                ["open_userid"] = item.OpenUserId,
                ["department"] = JsonValue.Create(item.Department),
            });
        }

        // 空列表是合法答案（该页没有成员或已翻完），不转错误。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["dept_user"] = items,
            ["count"] = items.Count,
            ["next_cursor"] = outcome.Data?.NextCursor,
        }));
    }

    private static JsonObject ProjectUser(UserInfo user, bool includeLeaders = true)
    {
        var projected = new JsonObject
        {
            ["userid"] = user.UserId,
            ["name"] = user.Name,
            ["alias"] = user.Alias,
            ["position"] = user.Position,
            ["department"] = ToJsonArray(user.Department),
            ["main_department"] = user.MainDepartment,
            ["status"] = user.Status,
            ["enable"] = user.Enable,
        };

        if (includeLeaders)
        {
            projected["direct_leader"] = ToJsonArray(user.DirectLeader);
        }

        return projected;
    }

    private static int? ToFetchChild(bool? fetchChild) => fetchChild switch
    {
        null => null,
        true => 1,
        false => 0,
    };

    private static JsonArray ToJsonArray(IEnumerable<string>? values)
    {
        var array = new JsonArray();
        if (values is not null)
        {
            foreach (var value in values)
            {
                array.Add((JsonNode?)JsonValue.Create(value));
            }
        }

        return array;
    }

    private static JsonArray ToJsonArray(IEnumerable<int>? values)
    {
        var array = new JsonArray();
        if (values is not null)
        {
            foreach (var value in values)
            {
                array.Add((JsonNode?)JsonValue.Create(value));
            }
        }

        return array;
    }
}
