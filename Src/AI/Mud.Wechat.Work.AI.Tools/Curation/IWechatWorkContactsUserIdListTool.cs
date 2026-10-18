// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.user.id_list</c>：游标分页读取成员 ID 与所属部门的关系（最轻的成员枚举面）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>simple_list</c> / <c>list</c> 的分工</b>：那两个按部门取成员（详情/摘要），本工具是<b>全量游标</b>——
/// 不需要部门即可逐页拿到「userid ↔ 部门」关系，是「这个企业有哪些人」的最轻入口。
/// </para>
/// <para>
/// <b>两种身份二选一投影</b>：自建应用（通讯录同步 secret）返回 <c>userid</c>，第三方返回 <c>open_userid</c>；
/// 两者都是不透明标识符（非个人信息），按官方实际返回哪一个投影哪一个。
/// </para>
/// </remarks>
[AgentTool(
    name: "contacts.user.id_list",
    Description = "以游标分页获取企业成员 ID 与所属部门的关系列表（全量枚举，无需指定部门）。参数 limit 为单页条数（1~10000，默认 1000）；cursor 为分页游标（首页省略，续页传上一页的 page_token）。返回 items（每项 userid 或 open_userid——自建应用（通讯录同步 secret）返回 userid，第三方返回 open_userid——以及 department 所属部门 ID 数组）、count 与 page_token。这是最轻的成员枚举入口：需要姓名/职务等详情时再用 contacts.user.get / contacts.user.simple_list 按 ID 取。",
    Source = "IWechatWorkUsersService.ListUserIdsAsync",
    RequiredScopes = ["contacts.user.read"])]
public interface IWechatWorkContactsUserIdListTool
{
    /// <summary>游标分页读取成员 ID 与部门关系。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、count、page_token）。</returns>
    Task<string> ListUserIdsAsync(
        [ToolParameter("limit", "单页条数（1~10000，可空 ⇒ 1000）。")] int? limit,
        [ToolParameter("cursor", "分页游标（可空 ⇒ 首页）。")] string? cursor,
        CancellationToken cancellationToken = default);
}
