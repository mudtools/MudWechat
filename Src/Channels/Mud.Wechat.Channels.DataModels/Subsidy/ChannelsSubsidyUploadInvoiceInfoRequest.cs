// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Subsidy;

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
