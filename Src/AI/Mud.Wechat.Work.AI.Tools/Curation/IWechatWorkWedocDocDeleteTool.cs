// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>wedoc.doc.delete</c>：删除微文档或收集表（<b>L3 高危写</b>的首个样本）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是 L3</b>：删除<b>不可逆</b>（官方无回收站读端点）、且影响面是共享文档（他人可能正在使用）。
/// 风险档由写动词表（<c>delete</c>）派生为 <see cref="Mud.Wechat.Work.AI.AgentTools.WechatToolRisk.HighRiskWrite"/>，
/// 而高危写<b>恒需 HITL 审批</b>（策略层不允许关闭），使其不能仅凭「授权器放行」被执行。
/// </para>
/// <para>
/// <b>docid / formid 二选一</b>：同 <c>wedoc.doc.rename</c>，由执行器前置校验。
/// </para>
/// </remarks>
[AgentTool(
    name: "wedoc.doc.delete",
    Description = "删除企业微信微文档或收集表（不可逆）。参数 docid 与 formid 二选一（只能填其中一个）。仅可删除本应用创建的文档/收集表。成功返回 ok=true 与目标标识。该操作为高危写：需宿主启用 + 授权通过 + 人工审批放行（审批不可关闭）。",
    Source = "IWechatWorkWedocService.DeleteDocumentAsync",
    RequiredScopes = ["wedoc.doc.write"],
    IsWrite = true,
    // SDK 返回 `WechatWorkResponse` 是剖面的**信封**类型（OutputWrapperTypeNames）：解包后无载荷，
    // 自动推导得到空 Schema ⇒ 显式声明执行器的实际投影（ok + 目标标识，见 WedocToolHandlers.Ok）。
    OutputSchema = """{"type":"object","properties":{"ok":{"type":"boolean"},"docid":{"type":"string"},"formid":{"type":"string"}},"required":["ok"]}""")]
public interface IWechatWorkWedocDocDeleteTool
{
    /// <summary>删除文档。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、docid、formid）。</returns>
    Task<string> DeleteDocumentAsync(
        [ToolParameter("docid", "文档 ID（与 formid 二选一）。")] string? docid,
        [ToolParameter("formid", "收集表 ID（与 docid 二选一）。")] string? formid,
        CancellationToken cancellationToken = default);
}
