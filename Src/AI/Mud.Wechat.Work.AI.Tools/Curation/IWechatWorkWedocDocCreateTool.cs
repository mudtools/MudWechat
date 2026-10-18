// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>wedoc.doc.create</c>：新建微文档（文档 / 表格 / 智能表格 / 智能文档）。
/// </summary>
/// <remarks>
/// <para>
/// <b>doc_type 用整数而非枚举</b>：官方取值稀疏（3/4/10/11）且会随新版文档类型扩张，
/// 枚举一旦漏项就变成「模型会用的类型被硬拒」；整数 + 描述里的清单可用性更好，非法值由上游 errcode 回填。
/// </para>
/// <para>
/// <b>docid 只在创建时返回</b>：执行器必须把它回填给模型（错过即永久丢失，官方无「按名查 docid」的读端点）。
/// </para>
/// </remarks>
[AgentTool(
    name: "wedoc.doc.create",
    Description = "新建企业微信微文档。参数 doc_name 为文档名（≤255 字符，必填）；doc_type 为文档类型（Document 文档 / Spreadsheet 表格 / SmartSheet 智能表格 / SmartDoc 智能文档，必填）；spaceid 与 fatherid 可选（指定空间时两者须同时给出）；admin_users 可选（管理员 userid 列表）。返回新建文档的 docid 与访问 url——docid 仅创建时返回，请妥善保存。该操作为受控写：需宿主启用 + 授权通过 + 审批放行。",
    Source = "IWechatWorkWedocService.CreateDocumentAsync",
    RequiredScopes = ["wedoc.doc.write"],
    IsWrite = true)]
public interface IWechatWorkWedocDocCreateTool
{
    /// <summary>新建文档。</summary>
    /// <param name="doc_name">文档名（≤255 字符）。</param>
    /// <param name="doc_type">文档类型（Document 文档 / Spreadsheet 表格 / SmartSheet 智能表格 / SmartDoc 智能文档）。</param>
    /// <param name="spaceid">空间 ID（可空）。</param>
    /// <param name="fatherid">父目录 ID（可空；指定 spaceid 时必填）。</param>
    /// <param name="admin_users">管理员 userid 列表（可空）。</param>
    void Create(
        [ToolParameter("doc_name", "文档名（≤255 字符）。", Required = true)] string doc_name,
        [ToolParameter("doc_type", "文档类型（Document 文档 / Spreadsheet 表格 / SmartSheet 智能表格 / SmartDoc 智能文档）。", EnumType = typeof(WechatWedocDocTypes))] int doc_type,
        [ToolParameter("spaceid", "空间 ID（可空）。")] string? spaceid,
        [ToolParameter("fatherid", "父目录 ID（可空；指定 spaceid 时必填）。")] string? fatherid,
        [ToolParameter("admin_users", "管理员 userid 列表（可空）。")] string[]? admin_users);
}
