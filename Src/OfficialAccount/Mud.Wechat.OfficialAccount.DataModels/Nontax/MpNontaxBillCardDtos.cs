// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Nontax;

/// <summary>
/// 「创建票据卡券」请求的票面模板（官方 <c>invoice_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxBillTemplate
{
    /// <summary>获取或设置票据卡券基础信息（官方 <c>base_info</c>；卡券面关键字段为 <c>logo_url</c>）。</summary>
    [JsonPropertyName("base_info")]
    public MpNontaxBillTemplateBase? BaseInfo { get; set; }

    /// <summary>获取或设置收款方（官方 <c>payee</c>）。</summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }
}

/// <summary>
/// 票据卡券基础信息（官方 <c>base_info</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxBillTemplateBase
{
    /// <summary>获取或设置卡券封面（官方 <c>logo_url</c>）。</summary>
    [JsonPropertyName("logo_url")]
    public string? LogoUrl { get; set; }
}

/// <summary>
/// 「创建票据卡券」请求（官方 <c>createbillcard</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxCreateBillCardRequest
{
    /// <summary>获取或设置票面模板（官方 <c>invoice_info</c>，必填）。</summary>
    [JsonPropertyName("invoice_info")]
    public MpNontaxBillTemplate InvoiceInfo { get; set; } = new();
}

/// <summary>
/// 「创建票据卡券」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxCreateBillCardResponse : MpResponse
{
    /// <summary>获取或设置票据卡券 ID（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}

/// <summary>
/// 票据用户数据（官方 <c>invoice_user_data</c>；插卡请求的第三层嵌套）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxBillUserData
{
    /// <summary>获取或设置发票抬头（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置实付金额（官方 <c>fee</c>，单位分）。</summary>
    [JsonPropertyName("fee")]
    public int Fee { get; set; }

    /// <summary>获取或设置缴费流水号（官方 <c>billing_no</c>）。</summary>
    [JsonPropertyName("billing_no")]
    public string? BillingNo { get; set; }

    /// <summary>获取或设置缴费编码（官方 <c>billing_code</c>）。</summary>
    [JsonPropertyName("billing_code")]
    public string? BillingCode { get; set; }

    /// <summary>获取或设置缴费时间（官方 <c>billing_time</c>，秒级 Unix 时间戳）。</summary>
    [JsonPropertyName("billing_time")]
    public long BillingTime { get; set; }

    /// <summary>获取或设置票据 PDF 的临时素材 ID（官方 <c>s_pdf_media_id</c>）。</summary>
    [JsonPropertyName("s_pdf_media_id")]
    public string? PdfMediaId { get; set; }
}

/// <summary>
/// 票据卡券用户卡面（官方 <c>user_card</c>；插卡请求的第二层嵌套）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxBillUserCard
{
    /// <summary>获取或设置票据用户数据（官方 <c>invoice_user_data</c>）。</summary>
    [JsonPropertyName("invoice_user_data")]
    public MpNontaxBillUserData? InvoiceUserData { get; set; }
}

/// <summary>
/// 票据卡券扩展信息（官方 <c>card_ext</c>；插卡请求的第二层容器）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxBillCardExtra
{
    /// <summary>获取或设置用户卡面（官方 <c>user_card</c>）。</summary>
    [JsonPropertyName("user_card")]
    public MpNontaxBillUserCard? UserCard { get; set; }
}

/// <summary>
/// 「插入票据」请求（官方 <c>insertbill</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxInsertBillRequest
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置票据卡券 ID（官方 <c>card_id</c>，必填；来自 createbillcard）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置卡券扩展信息（官方 <c>card_ext</c>，必填）。</summary>
    [JsonPropertyName("card_ext")]
    public MpNontaxBillCardExtra CardExt { get; set; } = new();
}

/// <summary>
/// 「插入票据」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxInsertBillResponse : MpResponse
{
    /// <summary>获取或设置卡券 Code（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置用户 OpenId（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 「获取票据授权链接」请求（官方 <c>getbillauthurl</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetBillAuthUrlRequest
{
    /// <summary>获取或设置非税订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置财政局的 <c>appid</c>（官方 <c>s_pappid</c>，必填）。</summary>
    [JsonPropertyName("s_pappid")]
    public string SPAppId { get; set; } = string.Empty;

    /// <summary>获取或设置金额（官方 <c>money</c>，单位分，必填）。</summary>
    [JsonPropertyName("money")]
    public int Money { get; set; }

    /// <summary>获取或设置时间戳（官方 <c>timestamp</c>，秒级 Unix 时间戳，可选；缺省 SDK 以当前时间生成——本 SDK 不代填，由官方或调用方决定）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>获取或设置来源（官方 <c>source</c>，必填）。</summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>获取或设置授权后跳转地址（官方 <c>redirect_url</c>，可选）。</summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    /// <summary>获取或设置票据下载票据（官方 <c>ticket</c>，必填；官方 <c>api_ticket</c>）。</summary>
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;
}

/// <summary>
/// 「获取票据授权链接」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxGetBillAuthUrlResponse : MpResponse
{
    /// <summary>获取或设置授权链接（官方 <c>auth_url</c>）。</summary>
    [JsonPropertyName("auth_url")]
    public string? AuthUrl { get; set; }

    /// <summary>获取或设置链接有效期（官方 <c>expire_time</c>，单位秒）。</summary>
    [JsonPropertyName("expire_time")]
    public int ExpireTime { get; set; }
}

/// <summary>
/// 「下载缴费对账单」请求（官方 <c>downloadbill</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本端点不走声明式管线</b>：成功响应是 <b>CSV 文本</b>（官方 FAQ 原文「以 csv 格式的文本返回」），
/// 失败才是 JSON <c>errcode</c> 形态——经 <c>MpNontaxBillDownloadService</c> 独立下载通道承载
/// （对齐素材下载 I3 裁决），本 DTO 仅承载其 JSON 请求体。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Nontax")]
public class MpNontaxDownloadBillRequest
{
    /// <summary>获取或设置公众号 <c>appid</c>（官方 <c>appid</c>，可选）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>获取或设置商户号（官方 <c>mch_id</c>，可选）。</summary>
    [JsonPropertyName("mch_id")]
    public string? MchId { get; set; }

    /// <summary>获取或设置账单日期（官方 <c>bill_date</c>，yyyyMMdd，必填）。</summary>
    [JsonPropertyName("bill_date")]
    public string BillDate { get; set; } = string.Empty;

    /// <summary>获取或设置账单类型（官方 <c>bill_type</c>，可选）。</summary>
    [JsonPropertyName("bill_type")]
    public string? BillType { get; set; }
}
