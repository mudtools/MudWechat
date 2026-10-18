// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Tools.Curation;

/// <summary>
/// 工具 <c>wedoc.doc.base_info</c>：读取微文档的基础信息（本域唯一的只读工具）。
/// </summary>
/// <remarks>
/// <b>读端点也是 POST</b>：微文档官方面全部以 JSON body 传参，故工具的 HTTP 方法由 SDK 特性派生为 <c>POST</c>，
/// 但风险档仍按「无副作用」判为只读——<b>方法名不代表风险</b>，这是本域最容易被误判的一处。
/// </remarks>
[AgentTool(
    name: "wedoc.doc.base_info",
    Description = "读取企业微信微文档的基础信息。参数 docid 为文档 ID（必填）。返回 docid、文档名、创建时间戳、最后修改时间戳与 doc_type（3 文档 / 4 表格 / 10 智能表格 / 11 智能文档）。仅能读取该应用创建或有权访问的文档。",
    Source = "IWechatWorkWedocService.GetDocumentBaseInfoAsync",
    RequiredScopes = ["wedoc.doc.read"],
    IsWrite = false)]
public interface IWechatWorkWedocDocBaseInfoTool
{
    /// <summary>读取文档基础信息。</summary>
    /// <returns>白名单投影后的 JSON 文本（docid、doc_name、doc_type、create_time、modify_time）。</returns>
    Task<string> GetDocumentBaseInfoAsync(
        [ToolParameter("docid", "文档 ID。", Required = true)] string docid,
        CancellationToken cancellationToken = default);
}
