// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Subsidy;

/// <summary>国补订单开票信息（官方 <c>data</c> 对象，query_invoicing_info 响应）。</summary>
/// <remarks>
/// <para>金额字段官方未标注单位（官方示例 <c>payment_amount</c> + <c>subsidy_amount</c> = <c>invoice_total_amount</c>）。</para>
/// <para>
/// <b>官方文档矛盾照录</b>：官方示例 JSON 将开票总金额写作 <c>invoice_tatal_amount</c>（拼写瑕疵）
/// ——按参数表字段名 <c>invoice_total_amount</c> 建模。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Subsidy")]
public class ChannelsSubsidyInvoicingInfo
{
    /// <summary>获取或设置发票抬头（官方 <c>name</c>，订单归属人姓名）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置发票税号（官方 <c>tax_no</c>，订单归属人身份证号）。</summary>
    [JsonPropertyName("tax_no")]
    public string? TaxNo { get; set; }

    /// <summary>获取或设置用户实付金额（官方 <c>payment_amount</c>；官方未标注单位）。</summary>
    [JsonPropertyName("payment_amount")]
    public long? PaymentAmount { get; set; }

    /// <summary>获取或设置国家补贴金额（官方 <c>subsidy_amount</c>；官方未标注单位）。</summary>
    [JsonPropertyName("subsidy_amount")]
    public long? SubsidyAmount { get; set; }

    /// <summary>获取或设置开票总金额（官方 <c>invoice_total_amount</c>；官方示例拼写为 invoice_tatal_amount——矛盾照录）。</summary>
    [JsonPropertyName("invoice_total_amount")]
    public long? InvoiceTotalAmount { get; set; }

    /// <summary>获取或设置 69 码（官方 <c>bar_code</c>）。</summary>
    [JsonPropertyName("bar_code")]
    public string? BarCode { get; set; }

    /// <summary>获取或设置 sn 码（官方 <c>sn_code</c>）。</summary>
    [JsonPropertyName("sn_code")]
    public string? SnCode { get; set; }

    /// <summary>获取或设置 imei 码（官方 <c>imei1</c>）。</summary>
    [JsonPropertyName("imei1")]
    public string? Imei1 { get; set; }

    /// <summary>获取或设置 imei 码（官方 <c>imei2</c>）。</summary>
    [JsonPropertyName("imei2")]
    public string? Imei2 { get; set; }

    /// <summary>获取或设置微信支付单号（官方 <c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置是否已上传发票信息（官方 <c>is_invoice_uploaded</c>）。</summary>
    [JsonPropertyName("is_invoice_uploaded")]
    public bool? IsInvoiceUploaded { get; set; }

    /// <summary>获取或设置能效/水效等级（官方 <c>efficiency</c>）。</summary>
    [JsonPropertyName("efficiency")]
    public string? Efficiency { get; set; }

    /// <summary>获取或设置商品型号（官方 <c>goods_model</c>，从广东银联接口获得）。</summary>
    [JsonPropertyName("goods_model")]
    public string? GoodsModel { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>goods_name</c>，从广东银联接口获得）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>获取或设置商品品牌（官方 <c>brand_name</c>，从广东银联接口获得）。</summary>
    [JsonPropertyName("brand_name")]
    public string? BrandName { get; set; }

    /// <summary>获取或设置购买人电话（官方 <c>buyer_phone</c>）。</summary>
    [JsonPropertyName("buyer_phone")]
    public string? BuyerPhone { get; set; }

    /// <summary>获取或设置商家让利金额（官方 <c>give_profit_amount</c>；官方未标注单位）。</summary>
    [JsonPropertyName("give_profit_amount")]
    public long? GiveProfitAmount { get; set; }
}
