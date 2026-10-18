// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>wedoc.doc.rename</c>：重命名微文档或收集表。
/// </summary>
/// <remarks>
/// <b>docid 与 formid 二选一</b>：这是官方请求体的互斥约束，Schema 无法表达「恰好一个」，
/// 由执行器在调用前校验并回填可读错误（见 <c>WedocToolHandlers</c>）。
/// </remarks>
[AgentTool(
    name: "wedoc.doc.rename",
    Description = "重命名企业微信微文档或收集表。参数 new_name 为新名称（≤255 字符，英文算 1 个、汉字算 2 个，超出被截断，必填）；docid 与 formid 二选一（只能填其中一个）。仅可修改本应用创建的文档/收集表。成功返回 ok=true 与目标标识。该操作为受控写：需宿主启用 + 授权通过 + 审批放行。",
    Source = "IWechatWorkWedocService.RenameDocumentAsync",
    RequiredScopes = ["wedoc.doc.write"],
    IsWrite = true,
    // SDK 返回 `WechatWorkResponse` 是剖面的**信封**类型（OutputWrapperTypeNames）：解包后无载荷，
    // 自动推导得到空 Schema ⇒ 显式声明执行器的实际投影（ok + 目标标识 + 回显的新名称，见 WedocToolHandlers.Ok）。
    OutputSchema = """{"type":"object","properties":{"ok":{"type":"boolean"},"docid":{"type":"string"},"formid":{"type":"string"},"new_name":{"type":"string"}},"required":["ok"]}""")]
public interface IWechatWorkWedocDocRenameTool
{
    /// <summary>重命名文档。</summary>
    /// <returns>白名单投影后的 JSON 文本（ok、docid、formid、new_name）。</returns>
    Task<string> RenameDocumentAsync(
        [ToolParameter("new_name", "新名称。", Required = true)] string new_name,
        [ToolParameter("docid", "文档 ID（与 formid 二选一）。")] string? docid,
        [ToolParameter("formid", "收集表 ID（与 docid 二选一）。")] string? formid,
        CancellationToken cancellationToken = default);
}
