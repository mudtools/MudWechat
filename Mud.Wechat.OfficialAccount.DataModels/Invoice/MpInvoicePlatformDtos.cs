// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 上传发票 PDF（POST /card/invoice/platform/setpdf）

/// <summary>
/// 上传发票 PDF（<c>POST /card/invoice/platform/setpdf</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>POST multipart/form-data</b>（表单文件字段名 <c>pdf</c>），Query 携带 <c>access_token</c>。
/// </para>
/// <para>
/// <b>有效期语义（官方原文，勿弱化）</b>：<c>s_media_id</c> 在「将发票卡券插入用户卡包」时用于关联 PDF 与发票卡券，
/// <b>有效期 3 天</b>；上传成功的 PDF 若 3 天内未被关联到发票卡券并发送到用户卡包，<b>将被清理</b>
/// （3 天后仍要关联须<b>重新上传</b>）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>s_media_id</c> 类型标为 <c>string</c> 但描述写「<b>64 位整数</b>」，
/// 而示例值仅 16 位 —— 三处不一致 ⇒ SDK 以 <b>string</b> 承载（不丢前导零 / 不受整数位宽限制）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceSetPdfResponse : MpResponse
{
    /// <summary>获取或设置 PDF 素材 id（官方 <c>s_media_id</c>；插卡时关联用，<b>有效期 3 天</b>）。</summary>
    [JsonPropertyName("s_media_id")]
    public string? SMediaId { get; set; }
}

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

// ---------------------------------------------------------------- 更新发票卡券状态（POST /card/invoice/platform/updatestatus）

/// <summary>
/// 更新发票卡券状态（<c>POST /card/invoice/platform/updatestatus</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>card_id</c> / <c>code</c> / <c>reimburse_status</c> 均必填。
/// </para>
/// <para>
/// <b><c>reimburse_status</c> 枚举（官方本页仅出现两值，完整表见发票状态一览表——该表不在本页）</b>：
/// 本页示例用 <c>INVOICE_REIMBURSE_INIT</c>；<b>电子发票冲红</b>时置为
/// <c>INVOICE_REIMBURSE_CANCEL</c>（官方描述为「表现为卡券被核销」，与 <c>CANCEL</c> 语义表述冲突，照录）。
/// SDK 的枚举表合并了同族 <c>getinvoiceinfo</c>/<c>updatestatusbatch</c> 页给出的三值（见 <c>MpInvoiceReimburseStatuses</c>）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUpdateStatusRequest
{
    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置发票 code（官方 <c>code</c>，必填）。</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>获取或设置发票报销状态（官方 <c>reimburse_status</c>；见 <c>MpInvoiceReimburseStatuses</c>）。</summary>
    [JsonPropertyName("reimburse_status")]
    public string ReimburseStatus { get; set; } = string.Empty;
}

// ---------------------------------------------------------------- 创建发票卡券模板（POST /card/invoice/platform/createcard）

/// <summary>
/// 创建发票卡券模板（<c>POST /card/invoice/platform/createcard</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>invoice_info</c>（发票模板对象）必填。
/// <b>适用范围为「公众号 / 服务号 —— 需申请」</b>（须提交场景申请并审核通过后方可调用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCreateCardRequest
{
    /// <summary>获取或设置发票模板对象（官方 <c>invoice_info</c>，必填）。</summary>
    [JsonPropertyName("invoice_info")]
    public MpInvoiceTemplateInfo InvoiceInfo { get; set; } = new MpInvoiceTemplateInfo();
}

/// <summary>发票模板对象（官方 <c>invoice_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceTemplateInfo
{
    /// <summary>获取或设置发票卡券模板基础信息（官方 <c>base_info</c>，必填）。</summary>
    [JsonPropertyName("base_info")]
    public MpInvoiceCardBaseInfo BaseInfo { get; set; } = new MpInvoiceCardBaseInfo();

    /// <summary>获取或设置收款方（开票方）全称（官方 <c>payee</c>，必填）。</summary>
    [JsonPropertyName("payee")]
    public string Payee { get; set; } = string.Empty;

    /// <summary>获取或设置发票类型（官方 <c>type</c>，必填；如「广东省增值税普通发票」）。</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// 发票卡券模板基础信息（官方 <c>invoice_info.base_info</c>）。
/// </summary>
/// <remarks>
/// 官方原文约束：<c>title</c>（收款方列表显示）<b>上限 9 个汉字</b>；<c>custom_url_name</c> /
/// <c>promotion_url_name</c> ≤ 5 汉字且与对应 url 同用；两个 <c>*_sub_title</c> ≤ 6 汉字；
/// <c>logo_url</c> 须走永久素材接口（官方字段说明原文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCardBaseInfo
{
    /// <summary>获取或设置发票商家 LOGO（官方 <c>logo_url</c>，必填；须走永久素材接口）。</summary>
    [JsonPropertyName("logo_url")]
    public string LogoUrl { get; set; } = string.Empty;

    /// <summary>获取或设置收款方（官方 <c>title</c>，必填；列表显示，上限 9 个汉字）。</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>获取或设置自定义入口名称（官方 <c>custom_url_name</c>；≤5 汉字，与 custom_url 同用）。</summary>
    [JsonPropertyName("custom_url_name")]
    public string? CustomUrlName { get; set; }

    /// <summary>获取或设置自定义入口外链（官方 <c>custom_url</c>；会携带发票参数）。</summary>
    [JsonPropertyName("custom_url")]
    public string? CustomUrl { get; set; }

    /// <summary>获取或设置自定义入口右侧 tips（官方 <c>custom_url_sub_title</c>；≤6 汉字）。</summary>
    [JsonPropertyName("custom_url_sub_title")]
    public string? CustomUrlSubTitle { get; set; }

    /// <summary>获取或设置营销场景自定义入口名称（官方 <c>promotion_url_name</c>）。</summary>
    [JsonPropertyName("promotion_url_name")]
    public string? PromotionUrlName { get; set; }

    /// <summary>获取或设置营销场景入口外链（官方 <c>promotion_url</c>；会携带发票参数）。</summary>
    [JsonPropertyName("promotion_url")]
    public string? PromotionUrl { get; set; }

    /// <summary>获取或设置营销入口右侧 tips（官方 <c>promotion_url_sub_title</c>；≤6 汉字）。</summary>
    [JsonPropertyName("promotion_url_sub_title")]
    public string? PromotionUrlSubTitle { get; set; }
}

/// <summary>
/// 创建发票卡券模板（<c>POST /card/invoice/platform/createcard</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<c>card_id</c>（错误码为 0 时返回；后续调用插卡接口时必填）。
/// 官方文档缺陷（照录）：本页错误码表与「创建卡券模板」场景不匹配（大量为插卡 / 票据领取场景码
/// —— 族级表整段复用）；且把 <c>errcode</c> 标为 <c>string</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCreateCardResponse : MpResponse
{
    /// <summary>获取或设置发票卡券模板编号（官方 <c>card_id</c>；后续插卡接口必填）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }
}

// ---------------------------------------------------------------- 将电子发票卡券插入用户卡包（POST /card/invoice/insert）

/// <summary>
/// 将电子发票卡券插入用户卡包（<c>POST /card/invoice/insert</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>order_id</c> / <c>card_id</c> / <c>appid</c> / <c>card_ext</c> 均必填。
/// </para>
/// <para>
/// <b>调用方约束（官方接口描述原文，勿弱化）</b>：本接口由开票平台或自建平台商户调用；
/// <b>「需要使用之前调用获取 <c>s_pappid</c> 接口时的开票平台公众号 appid 调用本接口，否则会造成报错，插卡失败」</b>
/// —— 但官方字段表把 <c>appid</c> 说明为「该订单号授权时使用的 appid，<b>一般为商户 appid</b>」，两处口径不一（照录）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceInsertRequest
{
    /// <summary>获取或设置发票订单号（官方 <c>order_id</c>，必填；商户给用户授权开票的订单号）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>，必填；取自 <c>createcard</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置调用方 appid（官方 <c>appid</c>，必填；口径矛盾见类型级 remarks）。</summary>
    [JsonPropertyName("appid")]
    public string AppId { get; set; } = string.Empty;

    /// <summary>获取或设置发票具体内容（官方 <c>card_ext</c>，必填）。</summary>
    [JsonPropertyName("card_ext")]
    public MpInvoiceCardExt CardExt { get; set; } = new MpInvoiceCardExt();
}

/// <summary>发票具体内容（官方 <c>card_ext</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCardExt
{
    /// <summary>获取或设置随机字符串（官方 <c>nonce_str</c>，必填；防重复）。</summary>
    [JsonPropertyName("nonce_str")]
    public string NonceStr { get; set; } = string.Empty;

    /// <summary>获取或设置用户信息结构体（官方 <c>user_card</c>，必填）。</summary>
    [JsonPropertyName("user_card")]
    public MpInvoiceUserCard UserCard { get; set; } = new MpInvoiceUserCard();
}

/// <summary>用户信息结构体（官方 <c>card_ext.user_card</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUserCard
{
    /// <summary>获取或设置用户信息（官方 <c>invoice_user_data</c>，必填）。</summary>
    [JsonPropertyName("invoice_user_data")]
    public MpInvoiceUserData InvoiceUserData { get; set; } = new MpInvoiceUserData();
}

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

/// <summary>
/// 发票商品明细（官方 <c>info[]</c>；插卡请求与报销查询响应同构 ⇒ 共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceItem
{
    /// <summary>获取或设置项目名称（官方 <c>name</c>，必填）。</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>获取或设置数量（官方 <c>num</c>）。</summary>
    [JsonPropertyName("num")]
    public int? Number { get; set; }

    /// <summary>获取或设置单位（官方 <c>unit</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>获取或设置单价（官方 <c>price</c>，必填；单位与 <c>fee</c> 一致为分）。</summary>
    [JsonPropertyName("price")]
    public int? Price { get; set; }
}

/// <summary>
/// 将电子发票卡券插入用户卡包（<c>POST /card/invoice/insert</c>）响应。
/// </summary>
/// <remarks>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceInsertResponse : MpResponse
{
    /// <summary>获取或设置发票 code（官方 <c>code</c>）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>获取或设置获得发票用户的 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置用户 unionid（官方 <c>unionid</c>；仅用户将公众号绑定到开放平台账号后出现）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
}
