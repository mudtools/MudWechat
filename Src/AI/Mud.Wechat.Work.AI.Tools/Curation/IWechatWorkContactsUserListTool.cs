// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.user.list</c>：按部门读取成员详情列表（字段集同 <c>contacts.user.get</c>）。
/// </summary>
/// <remarks>
/// 官方提示该端点返回量较大：需要「一个部门有哪些人」时优先 <c>contacts.user.simple_list</c>。
/// </remarks>
[AgentTool(
    name: "contacts.user.list",
    Description = "按部门列出成员详情。参数 department_id 为整数部门 ID（必填）；fetch_child 为布尔值（true 递归子部门，默认 false 仅直属成员）。返回 items（userid/name/position/department/status）。仅覆盖应用可见范围；返回量较大，列人优先用 contacts.user.simple_list。",
    Source = "IWechatWorkUsersService.GetUserDetailListAsync",
    RequiredScopes = ["contacts.user.read"],
    IsWrite = false)]
public interface IWechatWorkContactsUserListTool
{
    /// <summary>列出部门成员详情。</summary>
    /// <returns>白名单投影后的 JSON 文本（items、userid、name、position、department、status）。</returns>
    Task<string> GetUserDetailListAsync(
        [ToolParameter("department_id", "部门 ID（整数）。")] int department_id,
        [ToolParameter("fetch_child", "是否递归子部门（true 递归）。")] bool? fetch_child,
        CancellationToken cancellationToken = default);
}
