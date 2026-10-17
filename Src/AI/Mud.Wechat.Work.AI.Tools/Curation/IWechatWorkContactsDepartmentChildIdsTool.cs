// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.department.child_ids</c>：读取子部门 ID 列表（2022-08 通讯录安全加固后的官方推荐替代端点）。
/// </summary>
[AgentTool(
    name: "contacts.department.child_ids",
    Description = "列出企业微信子部门 ID。参数 department_id 可选（整数）：给出则返回该部门及其下所有子部门的 ID 与父部门 ID，省略则返回全量组织架构的 ID 列表。轻量、推荐用于逐层遍历组织架构。",
    Source = "IWechatWorkDepartmentsService.GetChildDepartmentIdListAsync",
    RequiredScopes = ["contacts.department.read"],
    IsWrite = false)]
public interface IWechatWorkContactsDepartmentChildIdsTool
{
    /// <summary>读取子部门 ID 列表。</summary>
    /// <param name="department_id">根部门 ID（可选；省略即全量）。</param>
    void ChildIds(int? department_id);
}
