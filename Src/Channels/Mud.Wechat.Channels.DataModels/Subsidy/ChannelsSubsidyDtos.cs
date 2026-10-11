// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Subsidy;

/// <summary>获取国补订单开票信息（<c>query_invoicing_info</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Subsidy")]
public class ChannelsSubsidyQueryInvoicingRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填；必须是使用了国补的订单 id）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;
}

/// <summary>获取国补订单开票信息（<c>query_invoicing_info</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Subsidy")]
public class ChannelsSubsidyQueryInvoicingResponse : ChannelsResponse
{
    /// <summary>获取或设置回包信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsSubsidyInvoicingInfo? Data { get; set; }
}

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

/// <summary>上传国补订单发票信息（<c>upload_invoice_info</c>）请求体。</summary>
/// <remarks>
/// <para>
/// 官方契约：金额单位为<b>分</b>；<c>invoice_media_id</c> 需调用「上传国补订单发票文件」接口获取；
/// <c>invoice_code</c> 发票类型为 90 可选填；<c>invoice_check_code</c> 类型 10 或 04 必填、01/09/90 选填。
/// </para>
/// <para>
/// <b>官方文档矛盾照录（invoice_date 类型）</b>：参数表类型列标注 string、说明为「开票时间，秒级时间戳，
/// 要求小于当前时间」、示例为数字 <c>1753186232</c>——按说明与示例的数字时间戳建模。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Subsidy")]
public class ChannelsSubsidyUploadInvoiceInfoRequest
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填；必须是使用了国补的订单 id）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置发票代码（官方 <c>invoice_code</c>，选填；发票类型为 90 可以选填）。</summary>
    [JsonPropertyName("invoice_code")]
    public string? InvoiceCode { get; set; }

    /// <summary>获取或设置发票号码（官方 <c>invoice_no</c>，必填）。</summary>
    [JsonPropertyName("invoice_no")]
    public string InvoiceNo { get; set; } = string.Empty;

    /// <summary>获取或设置开票时间（官方 <c>invoice_date</c>，必填；秒级时间戳，要求小于当前时间；参数表类型列为 string、示例为数字——矛盾照录）。</summary>
    [JsonPropertyName("invoice_date")]
    public long InvoiceDate { get; set; }

    /// <summary>获取或设置发票文件 media_id（官方 <c>invoice_media_id</c>，必填；需调用「上传国补订单发票文件」接口获取）。</summary>
    [JsonPropertyName("invoice_media_id")]
    public string InvoiceMediaId { get; set; } = string.Empty;

    /// <summary>获取或设置发票类型（官方 <c>invoice_type</c>，必填，见 <see cref="ChannelsSubsidyInvoiceTypes"/>）。</summary>
    [JsonPropertyName("invoice_type")]
    public string InvoiceType { get; set; } = string.Empty;

    /// <summary>获取或设置发票校验码（官方 <c>invoice_check_code</c>，选填；类型 10 或 04 必填，01、09、90 选填）。</summary>
    [JsonPropertyName("invoice_check_code")]
    public string? InvoiceCheckCode { get; set; }

    /// <summary>获取或设置税价合计（官方 <c>invoice_total_amount</c>，必填，单位：分）。</summary>
    [JsonPropertyName("invoice_total_amount")]
    public long InvoiceTotalAmount { get; set; }

    /// <summary>获取或设置发票税额（官方 <c>invoice_tax_amount</c>，必填，单位：分）。</summary>
    [JsonPropertyName("invoice_tax_amount")]
    public long InvoiceTaxAmount { get; set; }

    /// <summary>获取或设置销售企业名称（官方 <c>sales_enterprise_name</c>，必填）。</summary>
    [JsonPropertyName("sales_enterprise_name")]
    public string SalesEnterpriseName { get; set; } = string.Empty;

    /// <summary>获取或设置销售方纳税人识别号（官方 <c>tax_payer</c>，必填）。</summary>
    [JsonPropertyName("tax_payer")]
    public string TaxPayer { get; set; } = string.Empty;

    /// <summary>获取或设置数量（官方 <c>num</c>，必填）。</summary>
    [JsonPropertyName("num")]
    public int Num { get; set; }

    /// <summary>获取或设置货物或应税劳务服务名（官方 <c>service_name</c>，必填）。</summary>
    [JsonPropertyName("service_name")]
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>获取或设置备注事项（官方 <c>remark</c>，必填；不得包含 CSV 分隔符，否则报 987100129）。</summary>
    [JsonPropertyName("remark")]
    public string Remark { get; set; } = string.Empty;

    /// <summary>获取或设置规格型号（官方 <c>invoice_goods_model</c>，必填）。</summary>
    [JsonPropertyName("invoice_goods_model")]
    public string InvoiceGoodsModel { get; set; } = string.Empty;
}

/// <summary>上传国补订单发票文件（<c>upload_invoice_file</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Subsidy")]
public class ChannelsSubsidyUploadInvoiceFileRequest
{
    /// <summary>获取或设置文件链接（官方 <c>file_link</c>，必填；需保证公网可访问，仅支持 .jpg/.png，≤ 1MB）。</summary>
    [JsonPropertyName("file_link")]
    public string FileLink { get; set; } = string.Empty;
}

/// <summary>上传国补订单发票文件（<c>upload_invoice_file</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Subsidy")]
public class ChannelsSubsidyUploadInvoiceFileResponse : ChannelsResponse
{
    /// <summary>获取或设置回包信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsSubsidyInvoiceMediaData? Data { get; set; }
}

/// <summary>发票文件回包信息（官方 <c>data</c> 对象，upload_invoice_file 响应）。</summary>
[HttpJsonSerializable(SerializerClassName = "Subsidy")]
public class ChannelsSubsidyInvoiceMediaData
{
    /// <summary>获取或设置发票文件 media_id（官方 <c>media_id</c>，对应一个文件，供「上传国补订单发票信息」使用）。</summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }
}

/// <summary>国补发票类型常量（官方 <c>invoice_type</c> 枚举值，upload_invoice_info）。</summary>
public static class ChannelsSubsidyInvoiceTypes
{
    /// <summary>增值税专用发票。</summary>
    public const string VatSpecial = "01";

    /// <summary>纸质普通发票。</summary>
    public const string PaperGeneral = "04";

    /// <summary>数电专票。</summary>
    public const string DigitalSpecial = "09";

    /// <summary>电子普通发票。</summary>
    public const string ElectronicGeneral = "10";

    /// <summary>数电普票。</summary>
    public const string DigitalGeneral = "90";
}
