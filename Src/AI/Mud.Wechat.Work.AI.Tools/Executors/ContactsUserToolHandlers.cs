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
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsUserGet);
        }

        if (outcome.Data is not { } user)
        {
            return WechatToolResults.NotFound(WechatToolNames.ContactsUserGet);
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
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsUserSimpleList);
        }

        if (outcome.Data?.UserList is not { } list)
        {
            return WechatToolResults.EmptyList(WechatToolNames.ContactsUserSimpleList);
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
        if (!outcome.Ok)
        {
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsUserList);
        }

        if (outcome.Data?.UserList is not { } list)
        {
            return WechatToolResults.EmptyList(WechatToolNames.ContactsUserList);
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
            return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsUserIdList);
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

    /// <summary>执行 <c>contacts.user.resolve</c>（手机号 / 邮箱唯一索引，或部门内姓名匹配）。</summary>
    /// <param name="args">模型侧入参（<c>mobile</c> / <c>email</c> / <c>name</c> + <c>department_id</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果（matches、count、hint）。</returns>
    [ToolHandler(typeof(IWechatWorkContactsUserResolveTool))]
    public async Task<ToolResult> ResolveUserAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ContactsUserResolveArgs.Unpack(args);
        var mobile = Clean(request.Mobile);
        var email = Clean(request.Email);
        var name = Clean(request.Name);
        var given = (mobile is null ? 0 : 1) + (email is null ? 0 : 1) + (name is null ? 0 : 1);

        if (given == 0)
        {
            return ToolResult.FromError(
                "请提供 mobile、email 或 name 之一（按姓名解析还需 department_id）；三者都是空值，无法定位成员。");
        }

        if (given > 1)
        {
            return ToolResult.FromError(
                "一次只按一种标识解析（mobile / email / name 三选一）：多种标识可能指向不同成员，混用会让结果不可判定。");
        }

        if (mobile is not null)
        {
            var response = await _users.GetUserIdByMobileAsync(
                new GetUserIdByMobileRequest { Mobile = mobile },
                cancellationToken).ConfigureAwait(false);
            var outcome = WechatApiResultReader.Read(response);
            if (!outcome.Ok)
            {
                return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsUserResolve);
            }

            return SingleMatch(outcome.Data?.UserId, "该手机号未匹配到应用可见范围内的成员");
        }

        if (email is not null)
        {
            var response = await _users.GetUserIdByEmailAsync(
                new GetUserIdByEmailRequest { Email = email },
                cancellationToken).ConfigureAwait(false);
            var outcome = WechatApiResultReader.Read(response);
            if (!outcome.Ok)
            {
                return WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsUserResolve);
            }

            return SingleMatch(outcome.Data?.UserId, "该邮箱未匹配到应用可见范围内的成员");
        }

        if (request.DepartmentId is not { } departmentId)
        {
            return ToolResult.FromError(
                "按姓名解析需要 department_id：企微没有「按姓名搜全企业」的端点。"
                + "先用 contacts.department.list 找到部门 ID，或让用户提供手机号 / 邮箱（更快且唯一）。");
        }

        var listResponse = await _users.GetUserSimpleListAsync(departmentId, 1, cancellationToken).ConfigureAwait(false);
        var listOutcome = WechatApiResultReader.Read(listResponse);
        if (!listOutcome.Ok)
        {
            return WechatToolOutcomeErrors.From(listOutcome, WechatToolNames.ContactsUserResolve);
        }

        var candidates = listOutcome.Data?.UserList ?? [];
        var exact = candidates
            .Where(user => string.Equals(user.Name, name, StringComparison.Ordinal))
            .ToList();
        var matched = exact.Count > 0
            ? exact
            : candidates
                .Where(user => user.Name?.Contains(name!, StringComparison.Ordinal) == true)
                .ToList();

        var items = new JsonArray();
        foreach (var user in matched.Take(MaxResolveMatches))
        {
            items.Add((JsonNode?)new JsonObject
            {
                ["userid"] = user.UserId,
                ["open_userid"] = user.OpenUserId,
                ["name"] = user.Name,
                ["department"] = ToJsonArray(user.Department),
            });
        }

        string? hint = null;
        if (matched.Count == 0)
        {
            hint = "该部门（含子部门）内没有同名成员。可换部门，或改用手机号 / 邮箱（更快且唯一）。";
        }
        else if (matched.Count > MaxResolveMatches)
        {
            hint = $"匹配到 {matched.Count} 人，仅列出前 {MaxResolveMatches} 个；请缩小部门范围，或改用手机号 / 邮箱。";
        }
        else if (exact.Count == 0 && matched.Count > 1)
        {
            hint = "没有精确同名，以下是近似匹配；请让用户确认是哪一位，不要直接用于写入。";
        }

        // 空匹配是合法答案（该部门没有此人），不转错误：模型据此换路。
        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["matches"] = items,
            ["count"] = items.Count,
            ["hint"] = hint,
        }));
    }

    /// <summary>按姓名/标识匹配的结果条数上限（超过即提示收窄范围，避免把整部门送进上下文）。</summary>
    private const int MaxResolveMatches = 20;

    private static ToolResult SingleMatch(string? userId, string notFoundHint)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
            {
                ["matches"] = new JsonArray(),
                ["count"] = 0,
                ["hint"] = notFoundHint + "（该成员可能不存在、已离职，或不在应用可见范围内）。",
            }));
        }

        var matches = new JsonArray { (JsonNode?)new JsonObject { ["userid"] = userId } };

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["matches"] = matches,
            ["count"] = 1,
            ["hint"] = (string?)null,
        }));
    }

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
