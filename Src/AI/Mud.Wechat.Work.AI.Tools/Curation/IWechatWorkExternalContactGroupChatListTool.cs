// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>externalcontact.groupchat.list</c>：列出配置过客户群管理的客户群（chat_id + 跟进状态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只给 ID 与状态</b>：官方此端点只回 <c>chat_id</c> 与 <c>status</c>；群名、群主、成员需再按单个
/// <c>chat_id</c> 调 <c>externalcontact.groupchat.get</c>（官方无「批量详情」端点）。
/// </para>
/// <para>
/// <b>81017 是入口处的硬约束</b>：不填群主过滤时官方按应用可见范围内全部群主拉取，可见范围超过 1000 人即报错；
/// 群主为离职成员时也必须显式指定群主才能拉到。故本工具把 <c>owner_userid</c> / <c>owner_partyid</c>
/// 作为一等入口参数并在描述里写明该失败形态，避免模型在报错后反复重试。
/// </para>
/// </remarks>
[AgentTool(
    name: "externalcontact.groupchat.list",
    Description = "列出企业微信客户群（配置过客户群管理的群）。参数 status_filter 可选（All 全部 / PendingInherit 离职待继承 / Inheriting 离职继承中 / Inherited 离职继承完成，默认 All）；owner_userid 可选（群主 userid 数组，先用 contacts.user.simple_list 获取）；owner_partyid 可选（整数部门 ID，自动包含其子部门）；cursor 为分页游标（首页省略，续页传上一页的 next_cursor）；limit 为单页条数（1~1000，可空 ⇒ 500）。返回 group_chat_list（每项 chat_id、status）、count 与 next_cursor；拿到 chat_id 后用 externalcontact.groupchat.get 取详情。注意：省略群主过滤时会拉取应用可见范围内全部群主的数据，可见范围人数超过 1000 人官方报错 81017，此时必须先按群主或部门过滤；群主为离职成员时必须显式指定群主才能拉到。",
    Source = "IWechatWorkExternalContactGroupChatService.GetGroupChatListAsync",
    RequiredScopes = ["externalcontact.groupchat.read"])]
public interface IWechatWorkExternalContactGroupChatListTool
{
    /// <summary>列出客户群。</summary>
    /// <param name="status_filter">跟进状态过滤（All 全部 / PendingInherit 离职待继承 / Inheriting 离职继承中 / Inherited 离职继承完成；可空 ⇒ All）。</param>
    /// <param name="owner_userid">群主 userid 列表（可空；省略即拉应用可见范围内全部群主）。</param>
    /// <param name="owner_partyid">群主所属部门 ID（可空；单个部门，含其子部门）。</param>
    /// <param name="cursor">分页游标（可空 ⇒ 首页）。</param>
    /// <param name="limit">单页条数（1~1000，可空 ⇒ 500）。</param>
    void List(
        [ToolParameter("status_filter", "跟进状态过滤（All 全部 / PendingInherit 离职待继承 / Inheriting 离职继承中 / Inherited 离职继承完成；可空 ⇒ All）。", EnumType = typeof(WechatGroupChatStatusFilters))] int? status_filter,
        [ToolParameter("owner_userid", "群主 userid 列表（可空；省略即拉应用可见范围内全部群主）。")] string[]? owner_userid,
        [ToolParameter("owner_partyid", "群主所属部门 ID（可空；单个部门，含其子部门）。")] int? owner_partyid,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        [ToolParameter("limit", "单页条数（1~1000，可空 ⇒ 500）。")] int? limit);
}
