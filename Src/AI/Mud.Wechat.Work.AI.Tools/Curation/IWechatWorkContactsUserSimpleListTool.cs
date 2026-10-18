// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.user.simple_list</c>：按部门读取成员摘要（userid / 姓名 / 所属部门）。
/// </summary>
/// <remarks>
/// 摘要端点<b>不含敏感字段</b>，是「先列人再看详情」的第一跳；需要详情时再逐个调 <c>contacts.user.get</c>。
/// </remarks>
[AgentTool(
    name: "contacts.user.simple_list",
    Description = "列出某部门的成员摘要。参数 department_id 为整数部门 ID（必填）；fetch_child 为布尔值（true 递归子部门，默认 false 仅直属成员）。返回 userlist（userid/name/department）。需要应用对该部门有查看权限。",
    Source = "IWechatWorkUsersService.GetUserSimpleListAsync",
    RequiredScopes = ["contacts.user.read"],
    IsWrite = false)]
public interface IWechatWorkContactsUserSimpleListTool
{
    /// <summary>列出部门成员摘要。</summary>
    /// <returns>白名单投影后的 JSON 文本（userlist、userid、name、department）。</returns>
    Task<string> GetUserSimpleListAsync(
        [ToolParameter("department_id", "部门 ID（整数）。")] int department_id,
        [ToolParameter("fetch_child", "是否递归子部门（true 递归）。")] bool? fetch_child,
        CancellationToken cancellationToken = default);
}
