// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 「查询电子发票开具结果」响应的票面详情（官方 <c>invoicedetail</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBillingDetail
{
    /// <summary>获取或设置发票请求流水号（官方 <c>fpqqlsh</c>）。</summary>
    [JsonPropertyName("fpqqlsh")]
    public string? InvoiceSerialNumber { get; set; }

    /// <summary>获取或设置发票代码（官方 <c>fpdm</c>）。</summary>
    [JsonPropertyName("fpdm")]
    public string? InvoiceCode { get; set; }

    /// <summary>获取或设置发票号码（官方 <c>fphm</c>）。</summary>
    [JsonPropertyName("fphm")]
    public string? InvoiceNumber { get; set; }

    /// <summary>获取或设置开票日期（官方 <c>kprq</c>，字符串形态照官方）。</summary>
    [JsonPropertyName("kprq")]
    public string? InvoiceDateTimeString { get; set; }

    /// <summary>获取或设置校验码（官方 <c>jym</c>）。</summary>
    [JsonPropertyName("jym")]
    public string? CheckCode { get; set; }

    /// <summary>获取或设置发票 PDF 下载地址（官方 <c>pdfurl</c>）。</summary>
    [JsonPropertyName("pdfurl")]
    public string? PdfUrl { get; set; }
}
