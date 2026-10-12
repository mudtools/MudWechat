// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 查询已上传的 PDF（POST /card/invoice/platform/getpdf）

/// <summary>
/// 查询已上传的 PDF 文件（<c>POST /card/invoice/platform/getpdf</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>action</c>（填 <c>get_url</c>）+ <c>s_media_id</c>。
/// 官方文档缺陷（照录）：返回示例中 <c>pdf_url</c> 内含 <c>action=media_pdf</c>，
/// 与请求体要求填 <c>get_url</c> 不一致（示例与参数说明不对应）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceGetPdfRequest
{
    /// <summary>获取或设置动作（官方 <c>action</c>，必填；官方要求填 <c>get_url</c>）。</summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = "get_url";

    /// <summary>获取或设置发票 PDF 素材 id（官方 <c>s_media_id</c>，必填）。</summary>
    [JsonPropertyName("s_media_id")]
    public string SMediaId { get; set; } = string.Empty;
}
