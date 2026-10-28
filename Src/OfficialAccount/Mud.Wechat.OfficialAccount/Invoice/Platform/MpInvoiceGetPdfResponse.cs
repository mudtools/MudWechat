// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 查询已上传的 PDF 文件（<c>POST /card/invoice/platform/getpdf</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<c>pdf_url</c>（<b>两个小时有效期</b>）/ <c>pdf_url_expire_time</c>（过期时间，<b>7200 秒</b>）
/// —— 两处为同一事实的重复描述（照录）。官方错误码：<c>40001</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceGetPdfResponse : MpResponse
{
    /// <summary>获取或设置 PDF 的 URL（官方 <c>pdf_url</c>；两个小时有效期）。</summary>
    [JsonPropertyName("pdf_url")]
    public string? PdfUrl { get; set; }

    /// <summary>获取或设置 PDF URL 过期时间（官方 <c>pdf_url_expire_time</c>，7200 秒）。</summary>
    [JsonPropertyName("pdf_url_expire_time")]
    public int? PdfUrlExpireTime { get; set; }
}
