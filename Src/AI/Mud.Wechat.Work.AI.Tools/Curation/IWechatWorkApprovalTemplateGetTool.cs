// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>approval.template.get</c>：读取审批模板的结构（模板名 + 控件清单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <c>approval.get</c> 的分工</b>：详情给「这一单填了什么」（且只给字段名），本工具给「这个模板有哪些字段」——
/// 两者合起来才是完整的「先看表单结构、再看单据」读面。
/// </para>
/// <para>
/// <b>只投影控件结构</b>：控件类型 / ID / 标题 / 是否必填；<c>config</c>（选项值、公式等模板配置）不投影——
/// 模板是结构不是数据，与详情域同一条边界。
/// </para>
/// </remarks>
[AgentTool(
    name: "approval.template.get",
    Description = "获取企业微信审批模板的结构。参数 template_id 为审批模板 ID（管理后台「审批 - 模板管理」可查，或从 approval.get 返回的 template_id 取得）。返回 template_id、template_names（模板多语言名称）、controls（每项 control 控件类型如 Text/Number/Date/Contact/Table/File、id 控件 ID、title 控件标题、require 是否必填 1/0）与 control_count。只给模板结构，不给任何已提交单据的取值；控件配置（选项值等）亦不返回。",
    Source = "IWechatWorkApprovalTemplateService.GetTemplateDetailAsync",
    RequiredScopes = ["approval.read"])]
public interface IWechatWorkApprovalTemplateGetTool
{
    /// <summary>读取审批模板结构。</summary>
    /// <returns>白名单投影后的 JSON 文本（template_id、template_names、controls、control_count）。</returns>
    Task<string> GetTemplateDetailAsync(
        [ToolParameter("template_id", "审批模板 ID（管理后台「审批 - 模板管理」可查，或从 approval.get 返回的 template_id 取得）。", Required = true)] string template_id,
        CancellationToken cancellationToken = default);
}
