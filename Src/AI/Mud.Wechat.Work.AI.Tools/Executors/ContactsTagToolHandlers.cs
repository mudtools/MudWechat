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
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.AI.Tools.Executors;

/// <summary>
/// 通讯录「标签」域执行器：<c>contacts.tag.*</c> 两个只读工具落到
/// <see cref="IWechatWorkTagsService"/> 的标签读取端点上。
/// </summary>
/// <remarks>
/// <b>标签是「人群定义」而非「成员快照」</b>：标签成员随时会变，工具结果只代表调用时刻；
/// 因此本执行器不缓存、也不把标签成员展开成逐人详情（那会把一次读取放大成 N 次调用）。
/// </remarks>
/// <param name="tags">标签域 SDK（应用上下文由回合作用域给定）。</param>
public sealed class ContactsTagToolHandlers(IWechatWorkTagsService tags)
{
    private readonly IWechatWorkTagsService _tags = tags ?? throw new ArgumentNullException(nameof(tags));

    /// <summary>执行 <c>contacts.tag.list</c>。</summary>
    /// <param name="args">模型侧入参（零参）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsTagListTool))]
    public async Task<ToolResult> ListTagsAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        // 零参工具仍走生成的解包器：多余/未知键在解包时即被忽略或报错，避免「静默接受拼错的键」。
        _ = ContactsTagListArgs.Unpack(args);

        var response = await _tags.GetTagListAsync(cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data?.TagList is not { } list)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "未返回任何标签（taglist 为空）。");
        }

        var items = new JsonArray();
        foreach (var tag in list)
        {
            items.Add((JsonNode)new JsonObject
            {
                ["tag_id"] = tag.TagId,
                ["name"] = tag.TagName,
            });
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject { ["taglist"] = items, ["count"] = items.Count }));
    }

    /// <summary>执行 <c>contacts.tag.members</c>。</summary>
    /// <param name="args">模型侧入参（<c>tag_id</c>）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果。</returns>
    [ToolHandler(typeof(IWechatWorkContactsTagMembersTool))]
    public async Task<ToolResult> GetTagMembersAsync(
        IReadOnlyDictionary<string, object?> args,
        CancellationToken cancellationToken)
    {
        var request = ContactsTagMembersArgs.Unpack(args);

        var response = await _tags.GetTagMembersAsync(request.TagId, cancellationToken).ConfigureAwait(false);
        var outcome = WechatApiResultReader.Read(response);
        if (!outcome.Ok || outcome.Data is not { } tag)
        {
            return ToolResult.FromError(outcome.ErrorText ?? "标签不存在或不可见。");
        }

        var users = new JsonArray();
        foreach (var member in tag.UserList ?? [])
        {
            users.Add((JsonNode)new JsonObject
            {
                ["userid"] = member.UserId,
                ["name"] = member.Name,
            });
        }

        var departments = new JsonArray();
        foreach (var departmentId in tag.PartyList ?? [])
        {
            departments.Add((JsonNode?)JsonValue.Create(departmentId));
        }

        return ToolResult.FromText(ToolResultJson.ToText(new JsonObject
        {
            ["tagname"] = tag.TagName,
            ["userlist"] = users,
            ["partylist"] = departments,
            ["user_count"] = users.Count,
            ["department_count"] = departments.Count,
        }));
    }
}
