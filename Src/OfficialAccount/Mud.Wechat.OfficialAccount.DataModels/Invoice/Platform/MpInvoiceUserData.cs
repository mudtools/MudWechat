// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 发票用户信息（官方 <c>invoice_user_data</c>；插卡请求与报销查询响应同构 ⇒ 共用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>金额单位统一为「分」</b>：<c>fee</c> / <c>fee_without_tax</c> / <c>tax</c> 均为分。
/// </para>
/// <para>
/// <b>调用示例缺陷（照录）</b>：官方插卡请求示例中 <c>fee = 123</c> 而 <c>fee_without_tax = 2345</c>、
/// <c>tax = 123</c>，<b>不满足 <c>fee = fee_without_tax + tax</c></b>，且示例 JSON 缺逗号。
/// </para>
/// <para>
/// <b>字段名照录</b>：<c>trip_pdf_ur</c> 为官方字段原文（疑为 <c>trip_pdf_url</c> 笔误），<b>不得「规范化」</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUserData
{
    /// <summary>获取或设置发票金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int? Fee { get; set; }

    /// <summary>获取或设置发票抬头（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置开票时间（官方 <c>billing_time</c>；10 位时间戳，UTC+8）。</summary>
    [JsonPropertyName("billing_time")]
    public long? BillingTime { get; set; }

    /// <summary>获取或设置发票号码（官方 <c>billing_no</c>；<b>数电发票传 20 位</b>）。</summary>
    [JsonPropertyName("billing_no")]
    public string? BillingNumber { get; set; }

    /// <summary>获取或设置发票代码（官方 <c>billing_code</c>；<b>数电发票为空</b>）。</summary>
    [JsonPropertyName("billing_code")]
    public string? BillingCode { get; set; }

    /// <summary>获取或设置商品详情（官方 <c>info</c>）。</summary>
    [JsonPropertyName("info")]
    public List<MpInvoiceItem>? Items { get; set; }

    /// <summary>获取或设置不含税金额（官方 <c>fee_without_tax</c>，单位分）。</summary>
    [JsonPropertyName("fee_without_tax")]
    public int? FeeWithoutTax { get; set; }

    /// <summary>获取或设置税额（官方 <c>tax</c>，单位分）。</summary>
    [JsonPropertyName("tax")]
    public int? Tax { get; set; }

    /// <summary>获取或设置 PDF 素材 id（官方 <c>s_pdf_media_id</c>；取自上传发票 PDF 接口）。</summary>
    [JsonPropertyName("s_pdf_media_id")]
    public string? SPdfMediaId { get; set; }

    /// <summary>获取或设置其它消费附件 PDF（官方 <c>s_trip_pdf_media_id</c>；行程单、水单等）。</summary>
    [JsonPropertyName("s_trip_pdf_media_id")]
    public string? STripPdfMediaId { get; set; }

    /// <summary>获取或设置校验码（官方 <c>check_code</c>；PDF 右上角、开票日期下；<b>数电发票为空</b>）。</summary>
    [JsonPropertyName("check_code")]
    public string? CheckCode { get; set; }

    /// <summary>获取或设置购买方纳税人识别号（官方 <c>buyer_number</c>）。</summary>
    [JsonPropertyName("buyer_number")]
    public string? BuyerNumber { get; set; }

    /// <summary>获取或设置购买方地址、电话（官方 <c>buyer_address_and_phone</c>）。</summary>
    [JsonPropertyName("buyer_address_and_phone")]
    public string? BuyerAddressAndPhone { get; set; }

    /// <summary>获取或设置购买方开户行及账号（官方 <c>buyer_bank_account</c>）。</summary>
    [JsonPropertyName("buyer_bank_account")]
    public string? BuyerBankAccount { get; set; }

    /// <summary>获取或设置销售方纳税人识别号（官方 <c>seller_number</c>）。</summary>
    [JsonPropertyName("seller_number")]
    public string? SellerNumber { get; set; }

    /// <summary>获取或设置销售方地址、电话（官方 <c>seller_address_and_phone</c>）。</summary>
    [JsonPropertyName("seller_address_and_phone")]
    public string? SellerAddressAndPhone { get; set; }

    /// <summary>获取或设置销售方开户行及账号（官方 <c>seller_bank_account</c>）。</summary>
    [JsonPropertyName("seller_bank_account")]
    public string? SellerBankAccount { get; set; }

    /// <summary>获取或设置备注（官方 <c>remarks</c>）。</summary>
    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }

    /// <summary>获取或设置收款人（官方 <c>cashier</c>）。</summary>
    [JsonPropertyName("cashier")]
    public string? Cashier { get; set; }

    /// <summary>获取或设置开票人（官方 <c>maker</c>）。</summary>
    [JsonPropertyName("maker")]
    public string? Maker { get; set; }

    /// <summary>获取或设置发票报销状态（官方 <c>reimburse_status</c>；<b>报销查询响应返回</b>，见 <c>MpInvoiceReimburseStatuses</c>）。</summary>
    [JsonPropertyName("reimburse_status")]
    public string? ReimburseStatus { get; set; }

    /// <summary>获取或设置 PDF 的 URL（官方 <c>pdf_url</c>；<b>报销查询响应返回</b>，与 <c>s_pdf_media_id</c> 互为两族引用）。</summary>
    [JsonPropertyName("pdf_url")]
    public string? PdfUrl { get; set; }

    /// <summary>获取或设置行程单 PDF 的 URL（官方字段原文 <c>trip_pdf_ur</c>；<b>疑为 trip_pdf_url 笔误，不得「规范化」</b>）。</summary>
    [JsonPropertyName("trip_pdf_ur")]
    public string? TripPdfUrl { get; set; }
}
