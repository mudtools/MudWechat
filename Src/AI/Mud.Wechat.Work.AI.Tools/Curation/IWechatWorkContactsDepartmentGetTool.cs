// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>contacts.department.get</c>：读取单个部门的详情（名称、父部门、负责人）。
/// </summary>
/// <remarks>
/// <para>
/// 声明面只描述「模型可见的 Schema」，不含 HTTP 路由与令牌 —— 二者由 <see cref="AgentToolAttribute.Source"/>
/// 指向的 SDK 接口在编译期交叉校验派生。
/// </para>
/// <para>
/// <b>命名须带 <c>IWechatWork</c> 前缀</b>：剖面的令牌身份标记表按前缀推导，承载接口名与 <c>Source</c> 指向的 SDK 接口名
/// 必须被同一标记命中，否则引擎以「身份与承载接口不符」（<c>MUDWT016</c>）拒绝生成。
/// </para>
/// </remarks>
[AgentTool(
    name: "contacts.department.get",
    Description = "按部门 ID 读取企业微信部门详情。参数 department_id 为整数（根部门为 1）。返回部门名称、父部门 ID 与负责人列表。需要应用对目标部门有查看权限。",
    Source = "IWechatWorkDepartmentsService.GetDepartmentAsync",
    RequiredScopes = ["contacts.department.read"],
    IsWrite = false)]
public interface IWechatWorkContactsDepartmentGetTool
{
    /// <summary>读取部门详情。</summary>
    /// <param name="department_id">部门 ID（整数；根部门为 1）。</param>
    void Department([ToolParameter("department_id", "部门 ID（整数；根部门为 1）。")] int department_id);
}
