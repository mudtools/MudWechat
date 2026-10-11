// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

/// <summary>
/// 电子发票开票明细项（官方 <c>invoicedetail_list</c> 项；字段名照抄官方拼音缩写）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBillingItem
{
    /// <summary>获取或设置明细行性质（官方 <c>fphxz</c>；0=正常行，1=折扣行，2=被折扣行）。</summary>
    [JsonPropertyName("fphxz")]
    public int Type { get; set; }

    /// <summary>获取或设置商品编码（官方 <c>spbm</c>）。</summary>
    [JsonPropertyName("spbm")]
    public string GoodsCode { get; set; } = string.Empty;

    /// <summary>获取或设置项目名称（官方 <c>xmmc</c>）。</summary>
    [JsonPropertyName("xmmc")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置单位（官方 <c>dw</c>）。</summary>
    [JsonPropertyName("dw")]
    public string Unit { get; set; } = string.Empty;

    /// <summary>获取或设置规格型号（官方 <c>ggxh</c>）。</summary>
    [JsonPropertyName("ggxh")]
    public string Specification { get; set; } = string.Empty;

    /// <summary>获取或设置项目数量（官方 <c>xmsl</c>）。</summary>
    [JsonPropertyName("xmsl")]
    public int Count { get; set; }

    /// <summary>获取或设置项目单价（官方 <c>xmdj</c>，单位元）。</summary>
    [JsonPropertyName("xmdj")]
    public decimal Price { get; set; }

    /// <summary>获取或设置项目金额（官方 <c>xmje</c>，单位元）。</summary>
    [JsonPropertyName("xmje")]
    public decimal Amount { get; set; }

    /// <summary>获取或设置税率（官方 <c>sl</c>）。</summary>
    [JsonPropertyName("sl")]
    public decimal TaxRate { get; set; }

    /// <summary>获取或设置税额（官方 <c>se</c>，单位元）。</summary>
    [JsonPropertyName("se")]
    public decimal Tax { get; set; }
}

/// <summary>
/// 电子发票开票票面信息（官方 <c>invoiceinfo</c>；makeout / clearout 两端点共用承载形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBillingBody
{
    /// <summary>获取或设置用户 OpenId（官方 <c>wxopenid</c>，必填）。</summary>
    [JsonPropertyName("wxopenid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置订单号（官方 <c>ddh</c>；仅开票端点要求）。</summary>
    [JsonPropertyName("ddh")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置发票请求流水号（官方 <c>fpqqlsh</c>，必填；同一商户号下唯一，≤ 32 位）。</summary>
    [JsonPropertyName("fpqqlsh")]
    public string InvoiceSerialNumber { get; set; } = string.Empty;

    /// <summary>获取或设置销售方纳税人识别号（官方 <c>nsrsbh</c>，必填）。</summary>
    [JsonPropertyName("nsrsbh")]
    public string SellerTaxNumber { get; set; } = string.Empty;

    /// <summary>获取或设置销售方名称（官方 <c>nsrmc</c>，必填）。</summary>
    [JsonPropertyName("nsrmc")]
    public string SellerName { get; set; } = string.Empty;

    /// <summary>获取或设置销售方地址（官方 <c>nsrdz</c>，必填）。</summary>
    [JsonPropertyName("nsrdz")]
    public string SellerAddress { get; set; } = string.Empty;

    /// <summary>获取或设置销售方电话（官方 <c>nsrdh</c>，必填）。</summary>
    [JsonPropertyName("nsrdh")]
    public string SellerPhoneNumber { get; set; } = string.Empty;

    /// <summary>获取或设置销售方开户行（官方 <c>nsrbank</c>，必填）。</summary>
    [JsonPropertyName("nsrbank")]
    public string SellerBank { get; set; } = string.Empty;

    /// <summary>获取或设置销售方银行账号（官方 <c>nsrbankid</c>，必填）。</summary>
    [JsonPropertyName("nsrbankid")]
    public string SellerBankNumber { get; set; } = string.Empty;

    /// <summary>获取或设置购买方纳税人识别号（官方 <c>ghfnsrsbh</c>，可选）。</summary>
    [JsonPropertyName("ghfnsrsbh")]
    public string? BuyerTaxNumber { get; set; }

    /// <summary>获取或设置购买方名称（官方 <c>ghfmc</c>，可选）。</summary>
    [JsonPropertyName("ghfmc")]
    public string? BuyerName { get; set; }

    /// <summary>获取或设置购买方地址（官方 <c>ghfdz</c>，可选）。</summary>
    [JsonPropertyName("ghfdz")]
    public string? BuyerAddress { get; set; }

    /// <summary>获取或设置购买方电话（官方 <c>ghfdh</c>，可选）。</summary>
    [JsonPropertyName("ghfdh")]
    public string? BuyerPhoneNumber { get; set; }

    /// <summary>获取或设置购买方开户行（官方 <c>ghfbank</c>，可选）。</summary>
    [JsonPropertyName("ghfbank")]
    public string? BuyerBank { get; set; }

    /// <summary>获取或设置购买方银行账号（官方 <c>ghfbankid</c>，可选）。</summary>
    [JsonPropertyName("ghfbankid")]
    public string? BuyerBankNumber { get; set; }

    /// <summary>获取或设置开票人（官方 <c>kpr</c>，必填）。</summary>
    [JsonPropertyName("kpr")]
    public string Drawer { get; set; } = string.Empty;

    /// <summary>获取或设置收款人（官方 <c>skr</c>，可选）。</summary>
    [JsonPropertyName("skr")]
    public string? Cashier { get; set; }

    /// <summary>获取或设置复核人（官方 <c>fhr</c>，可选）。</summary>
    [JsonPropertyName("fhr")]
    public string? Reviewer { get; set; }

    /// <summary>获取或设置价税合计（官方 <c>jshj</c>，单位元）。</summary>
    [JsonPropertyName("jshj")]
    public decimal Fee { get; set; }

    /// <summary>获取或设置不含税合计金额（官方 <c>hjje</c>，单位元）。</summary>
    [JsonPropertyName("hjje")]
    public decimal FeeWithoutTax { get; set; }

    /// <summary>获取或设置合计税额（官方 <c>hjse</c>，单位元）。</summary>
    [JsonPropertyName("hjse")]
    public decimal Tax { get; set; }

    /// <summary>获取或设置备注（官方 <c>bz</c>，可选）。</summary>
    [JsonPropertyName("bz")]
    public string? Remark { get; set; }

    /// <summary>获取或设置行业类型（官方 <c>hylx</c>，可选；0=商业，1=其它）。</summary>
    [JsonPropertyName("hylx")]
    public int? Type { get; set; }

    /// <summary>获取或设置开票明细列表（官方 <c>invoicedetail_list</c>）。</summary>
    [JsonPropertyName("invoicedetail_list")]
    public List<MpInvoiceBillingItem> InvoiceItemList { get; set; } = new();

    /// <summary>获取或设置原发票代码（官方 <c>yfpdm</c>；仅冲红（clearoutinvoice）端点要求）。</summary>
    [JsonPropertyName("yfpdm")]
    public string OriginalInvoiceCode { get; set; } = string.Empty;

    /// <summary>获取或设置原发票号码（官方 <c>yfphm</c>；仅冲红（clearoutinvoice）端点要求）。</summary>
    [JsonPropertyName("yfphm")]
    public string OriginalInvoiceNumber { get; set; } = string.Empty;
}

/// <summary>
/// 「开具电子发票」请求（官方 <c>card/invoice/makeoutinvoice</c>；官方契约顶层仅 <c>invoiceinfo</c> 一键）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpMakeOutInvoiceRequest
{
    /// <summary>获取或设置票面信息（官方 <c>invoiceinfo</c>，必填）。</summary>
    [JsonPropertyName("invoiceinfo")]
    public MpInvoiceBillingBody InvoiceInfo { get; set; } = new();
}

/// <summary>
/// 「冲红电子发票」请求（官方 <c>card/invoice/clearoutinvoice</c>；官方契约顶层仅 <c>invoiceinfo</c> 一键，
/// 且须填写 <see cref="MpInvoiceBillingBody.OriginalInvoiceCode"/> / <see cref="MpInvoiceBillingBody.OriginalInvoiceNumber"/>）。
/// </summary>
/// <remarks><b>覆盖删除语义</b>：冲红后原发票作废、生成红字发票，不可逆。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpClearOutInvoiceRequest
{
    /// <summary>获取或设置票面信息（官方 <c>invoiceinfo</c>，必填；含原发票代码/号码）。</summary>
    [JsonPropertyName("invoiceinfo")]
    public MpInvoiceBillingBody InvoiceInfo { get; set; } = new();
}

/// <summary>
/// 「查询电子发票开具结果」请求（官方 <c>card/invoice/queryinvoceinfo</c>；官方路由拼写即 queryinvoceinfo，照抄）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpQueryInvoiceInfoRequest
{
    /// <summary>获取或设置发票请求流水号（官方 <c>fpqqlsh</c>，必填；与开票时一致）。</summary>
    [JsonPropertyName("fpqqlsh")]
    public string InvoiceSerialNumber { get; set; } = string.Empty;

    /// <summary>获取或设置销售方纳税人识别号（官方 <c>nsrsbh</c>，必填）。</summary>
    [JsonPropertyName("nsrsbh")]
    public string SellerTaxNumber { get; set; } = string.Empty;
}

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

/// <summary>
/// 「查询电子发票开具结果」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpQueryInvoiceInfoResponse : MpResponse
{
    /// <summary>获取或设置票面详情（官方 <c>invoicedetail</c>；开票仍在处理时可能缺省）。</summary>
    [JsonPropertyName("invoicedetail")]
    public MpInvoiceBillingDetail? InvoiceDetail { get; set; }
}
