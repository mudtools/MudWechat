// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Invoice;

/// <summary>
/// 电子发票用户信息（<c>user_info</c>，查询电子发票 / 批量查询电子发票响应共用）。
/// </summary>
/// <remarks>
/// 官方约束：金额类字段（fee / tax / fee_without_tax / price）均以<b>分</b>为单位；
/// 全电发票的发票号码在 <see cref="BillingNo"/> 字段（不在 <see cref="BillingCode"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class InvoiceUserInfo
{
    /// <summary>
    /// 获取或设置发票加税合计金额，以分为单位（官方必返回）。
    /// </summary>
    [JsonPropertyName("fee")]
    public long? Fee { get; set; }

    /// <summary>
    /// 获取或设置发票的抬头（官方必返回）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置开票时间（十位 Unix 时间戳，秒；官方必返回）。
    /// </summary>
    [JsonPropertyName("billing_time")]
    public long? BillingTime { get; set; }

    /// <summary>
    /// 获取或设置发票代码（官方必返回；注意：全电发票的发票号码为本字段）。
    /// </summary>
    [JsonPropertyName("billing_no")]
    public string? BillingNo { get; set; }

    /// <summary>
    /// 获取或设置发票号码（官方必返回；全电发票的发票号码不在本字段，见 <see cref="BillingNo"/>）。
    /// </summary>
    [JsonPropertyName("billing_code")]
    public string? BillingCode { get; set; }

    /// <summary>
    /// 获取或设置税额，以分为单位（官方必返回）。
    /// </summary>
    [JsonPropertyName("tax")]
    public long? Tax { get; set; }

    /// <summary>
    /// 获取或设置不含税金额，以分为单位（官方必返回）。
    /// </summary>
    [JsonPropertyName("fee_without_tax")]
    public long? FeeWithoutTax { get; set; }

    /// <summary>
    /// 获取或设置发票详情，一般描述发票使用说明（官方必返回）。
    /// </summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>
    /// 获取或设置发票对应的 PDF URL（官方必返回）。
    /// </summary>
    [JsonPropertyName("pdf_url")]
    public string? PdfUrl { get; set; }

    /// <summary>
    /// 获取或设置其它消费凭证附件 URL，如行程单、水单等（非必返回）。
    /// </summary>
    [JsonPropertyName("trip_pdf_url")]
    public string? TripPdfUrl { get; set; }

    /// <summary>
    /// 获取或设置校验码（官方必返回）。
    /// </summary>
    [JsonPropertyName("check_code")]
    public string? CheckCode { get; set; }

    /// <summary>
    /// 获取或设置购买方纳税人识别号（非必返回）。
    /// </summary>
    [JsonPropertyName("buyer_number")]
    public string? BuyerNumber { get; set; }

    /// <summary>
    /// 获取或设置购买方地址、电话（非必返回）。
    /// </summary>
    [JsonPropertyName("buyer_address_and_phone")]
    public string? BuyerAddressAndPhone { get; set; }

    /// <summary>
    /// 获取或设置购买方开户行及账号（非必返回）。
    /// </summary>
    [JsonPropertyName("buyer_bank_account")]
    public string? BuyerBankAccount { get; set; }

    /// <summary>
    /// 获取或设置销售方纳税人识别号（非必返回）。
    /// </summary>
    [JsonPropertyName("seller_number")]
    public string? SellerNumber { get; set; }

    /// <summary>
    /// 获取或设置销售方地址、电话（非必返回）。
    /// </summary>
    [JsonPropertyName("seller_address_and_phone")]
    public string? SellerAddressAndPhone { get; set; }

    /// <summary>
    /// 获取或设置销售方开户行及账号（非必返回）。
    /// </summary>
    [JsonPropertyName("seller_bank_account")]
    public string? SellerBankAccount { get; set; }

    /// <summary>
    /// 获取或设置备注（非必返回）。
    /// </summary>
    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 获取或设置收款人（发票左下角处，非必返回）。
    /// </summary>
    [JsonPropertyName("cashier")]
    public string? Cashier { get; set; }

    /// <summary>
    /// 获取或设置开票人（发票右下角处，非必返回）。
    /// </summary>
    [JsonPropertyName("maker")]
    public string? Maker { get; set; }

    /// <summary>
    /// 获取或设置报销状态（官方必返回），取值见 <see cref="InvoiceReimburseStatus"/>：
    /// INVOICE_REIMBURSE_INIT - 初始未锁定、INVOICE_REIMBURSE_LOCK - 已锁定、INVOICE_REIMBURSE_CLOSURE - 已核销。
    /// </summary>
    [JsonPropertyName("reimburse_status")]
    public string? ReimburseStatus { get; set; }

    /// <summary>
    /// 获取或设置商品信息数组（非必返回）。
    /// </summary>
    [JsonPropertyName("info")]
    public List<InvoiceItemInfo>? Info { get; set; }

    /// <summary>
    /// 获取或设置报销关联单号。
    /// <para>官方批量查询电子发票的响应 JSON 示例在 user_info 内携带本字段，
    /// 但官方参数表未列出——以官方 JSON 示例为准（对齐 department_id_list / cusor 处置先例）。</para>
    /// </summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}
