// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 查询与设置授权页与商户信息（POST /card/invoice/setbizattr?action=…）

/// <summary>
/// 查询与设置授权页与商户信息（<c>POST /card/invoice/setbizattr</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>按 Query <c>action</c> 六值分流</b>（见 <c>MpInvoiceBizAttrActions</c>）：
/// <c>set_auth_field</c>/<c>set_pay_mch</c>/<c>set_contact</c> 三者各自携带对应的<b>唯一一个</b>字段；
/// 三个 <c>get_*</c> 的请求体为 <c>{}</c>。
/// </para>
/// <para>
/// <b>官方必填性矛盾（照录，SDK 全部可空、不本地校验）</b>：官方字段表把 <c>auth_field</c>/<c>paymch_info</c>/
/// <c>contact</c> 三者<b>均标必填</b>，但实际按 <c>action</c> 三选一、且 <c>get_*</c> 时请求体为空。
/// </para>
/// <para>
/// 官方一次性设置约束（正文原文，SDK 不编排）：<c>set_auth_field</c>/<c>set_pay_mch</c> 为<b>一次性设置</b>，
/// 除非调整字段 / 识别号变更或更换开票平台；<c>set_contact</c> 需在<b>获取授权链接之前</b>先设置联系方式。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBizAttrRequest
{
    /// <summary>获取或设置授权页字段（官方 <c>auth_field</c>；<c>action = set_auth_field</c> 时使用）。</summary>
    [JsonPropertyName("auth_field")]
    public MpInvoiceAuthField? AuthField { get; set; }

    /// <summary>获取或设置微信商户号与开票平台关系信息（官方 <c>paymch_info</c>；<c>action = set_pay_mch</c> 时使用）。</summary>
    [JsonPropertyName("paymch_info")]
    public MpInvoicePayMchInfo? PayMchInfo { get; set; }

    /// <summary>获取或设置联系方式信息（官方 <c>contact</c>；<c>action = set_contact</c> 时使用）。</summary>
    [JsonPropertyName("contact")]
    public MpInvoiceContact? Contact { get; set; }
}

/// <summary>授权页字段（官方 <c>auth_field</c>；请求与响应同构 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceAuthField
{
    /// <summary>获取或设置个人发票字段（官方 <c>user_field</c>）。</summary>
    [JsonPropertyName("user_field")]
    public MpInvoiceUserField? UserField { get; set; }

    /// <summary>获取或设置单位发票字段（官方 <c>biz_field</c>）。</summary>
    [JsonPropertyName("biz_field")]
    public MpInvoiceBizField? BizField { get; set; }
}

/// <summary>个人发票字段（官方 <c>user_field</c>；各 <c>show_*</c> 为 0 否 / 1 是）。</summary>
/// <remarks>官方文档缺陷（照录）：<c>require_phone</c>/<c>require_email</c> 标注为「仅 get_auth_field 返回」。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUserField
{
    /// <summary>获取或设置是否填写抬头（官方 <c>show_title</c>，0 否 / 1 是）。</summary>
    [JsonPropertyName("show_title")]
    public int? ShowTitle { get; set; }

    /// <summary>获取或设置是否填写电话号码（官方 <c>show_phone</c>）。</summary>
    [JsonPropertyName("show_phone")]
    public int? ShowPhone { get; set; }

    /// <summary>获取或设置是否填写邮箱（官方 <c>show_email</c>）。</summary>
    [JsonPropertyName("show_email")]
    public int? ShowEmail { get; set; }

    /// <summary>获取或设置电话是否必填（官方 <c>require_phone</c>；仅 get_auth_field 返回）。</summary>
    [JsonPropertyName("require_phone")]
    public int? RequirePhone { get; set; }

    /// <summary>获取或设置邮箱是否必填（官方 <c>require_email</c>；仅 get_auth_field 返回）。</summary>
    [JsonPropertyName("require_email")]
    public int? RequireEmail { get; set; }

    /// <summary>获取或设置自定义字段（官方 <c>custom_field</c>）。</summary>
    [JsonPropertyName("custom_field")]
    public List<MpInvoiceCustomField>? CustomField { get; set; }
}

/// <summary>
/// 单位发票字段（官方 <c>biz_field</c>；各 <c>show_*</c>/<c>require_*</c> 为 0 否 / 1 是）。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：<c>require_tax_no</c> 在官方字段表的请求体与返回体说明中<b>均重复列出两次</b>；
/// 各 <c>require_*</c> 标注为「仅 get_auth_field 返回」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBizField
{
    /// <summary>获取或设置是否填写抬头（官方 <c>show_title</c>）。</summary>
    [JsonPropertyName("show_title")]
    public int? ShowTitle { get; set; }

    /// <summary>获取或设置是否填写税号（官方 <c>show_tax_no</c>）。</summary>
    [JsonPropertyName("show_tax_no")]
    public int? ShowTaxNumber { get; set; }

    /// <summary>获取或设置是否填写单位地址（官方 <c>show_addr</c>）。</summary>
    [JsonPropertyName("show_addr")]
    public int? ShowAddress { get; set; }

    /// <summary>获取或设置是否填写电话号码（官方 <c>show_phone</c>）。</summary>
    [JsonPropertyName("show_phone")]
    public int? ShowPhone { get; set; }

    /// <summary>获取或设置是否填写开户银行（官方 <c>show_bank_type</c>）。</summary>
    [JsonPropertyName("show_bank_type")]
    public int? ShowBankType { get; set; }

    /// <summary>获取或设置是否填写银行账号（官方 <c>show_bank_no</c>）。</summary>
    [JsonPropertyName("show_bank_no")]
    public int? ShowBankNumber { get; set; }

    /// <summary>获取或设置税号是否必填（官方 <c>require_tax_no</c>；官方字段表重复列出两次，照录）。</summary>
    [JsonPropertyName("require_tax_no")]
    public int? RequireTaxNumber { get; set; }

    /// <summary>获取或设置单位地址是否必填（官方 <c>require_addr</c>）。</summary>
    [JsonPropertyName("require_addr")]
    public int? RequireAddress { get; set; }

    /// <summary>获取或设置电话是否必填（官方 <c>require_phone</c>）。</summary>
    [JsonPropertyName("require_phone")]
    public int? RequirePhone { get; set; }

    /// <summary>获取或设置开户行是否必填（官方 <c>require_bank_type</c>）。</summary>
    [JsonPropertyName("require_bank_type")]
    public int? RequireBankType { get; set; }

    /// <summary>获取或设置银行账号是否必填（官方 <c>require_bank_no</c>）。</summary>
    [JsonPropertyName("require_bank_no")]
    public int? RequireBankNumber { get; set; }

    /// <summary>获取或设置自定义字段（官方 <c>custom_field</c>）。</summary>
    [JsonPropertyName("custom_field")]
    public List<MpInvoiceCustomField>? CustomField { get; set; }
}

/// <summary>自定义字段（官方 <c>custom_field[]</c>；请求与响应同构 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceCustomField
{
    /// <summary>获取或设置字段名（官方 <c>key</c>，必填）。</summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>获取或设置是否必填（官方 <c>is_require</c>，0 否 / 1 是，默认 0）。</summary>
    [JsonPropertyName("is_require")]
    public int? IsRequire { get; set; }

    /// <summary>获取或设置提示文案（官方 <c>notice</c>）。</summary>
    [JsonPropertyName("notice")]
    public string? Notice { get; set; }
}

/// <summary>微信支付商户号与开票平台关系信息（官方 <c>paymch_info</c>；请求与响应同构 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoicePayMchInfo
{
    /// <summary>获取或设置微信支付商户号（官方 <c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>获取或设置开票平台 id（官方 <c>s_pappid</c>；为该商户提供开票服务的开票平台标识）。</summary>
    [JsonPropertyName("s_pappid")]
    public string? SPappId { get; set; }
}

/// <summary>联系方式信息（官方 <c>contact</c>；请求与响应同构 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceContact
{
    /// <summary>获取或设置开票超时时间（官方 <c>time_out</c>；官方未说明单位）。</summary>
    [JsonPropertyName("time_out")]
    public int? TimeOut { get; set; }

    /// <summary>获取或设置联系电话（官方 <c>phone</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
}

/// <summary>
/// 查询与设置授权页与商户信息（<c>POST /card/invoice/setbizattr</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：报文的三个业务字段（<c>auth_field</c>/<c>paymch_info</c>/<c>contact</c>）**与请求体同构**且
/// <b>各自仅被对应 action 返回</b>（<c>set_*</c> 返回被设置者、<c>get_*</c> 返回被查询者）。
/// 本 DTO 与请求体共用 <see cref="MpInvoiceAuthField"/> / <see cref="MpInvoicePayMchInfo"/> /
/// <see cref="MpInvoiceContact"/> 三个类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceBizAttrResponse : MpResponse
{
    /// <summary>获取或设置授权页字段（官方 <c>auth_field</c>；<c>set_auth_field</c> / <c>get_auth_field</c> 返回）。</summary>
    [JsonPropertyName("auth_field")]
    public MpInvoiceAuthField? AuthField { get; set; }

    /// <summary>获取或设置商户号与开票平台关系（官方 <c>paymch_info</c>；<c>get_pay_mch</c> 返回）。</summary>
    [JsonPropertyName("paymch_info")]
    public MpInvoicePayMchInfo? PayMchInfo { get; set; }

    /// <summary>获取或设置联系方式（官方 <c>contact</c>；<c>get_contact</c> 返回）。</summary>
    [JsonPropertyName("contact")]
    public MpInvoiceContact? Contact { get; set; }
}

// ---------------------------------------------------------------- 查询授权信息（POST /card/invoice/getauthdata）

/// <summary>
/// 查询授权信息（<c>POST /card/invoice/getauthdata</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>order_id</c>（订单号）+ <c>s_pappid</c>（财政局 id）。
/// 官方错误码：<c>40078</c> / <c>72015</c> / <c>72031</c> / <c>72035</c> / <c>72036</c> / <c>72038</c> /
/// <c>72040</c> / <c>72042</c> / <c>72043</c>（本页错误码表含票据 PDF / 插卡等场景码，属族级表复用，照录）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceAuthDataRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置开票平台/财政局标识（官方 <c>s_pappid</c>，必填；本页说明为「财政局 id」）。</summary>
    [JsonPropertyName("s_pappid")]
    public string SPappId { get; set; } = string.Empty;
}

/// <summary>
/// 查询授权信息（<c>POST /card/invoice/getauthdata</c>）响应。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：① 接口名为「查询授权信息」、描述为「查询订单是否有被用户授权」，但返回字段
/// <c>invoice_status</c> 的说明为「发票状态」、示例值语义更像授权状态，字段语义与接口用途不完全一致；
/// ② 请求参数<b>无</b> <c>card_id</c>，但错误码 <c>40078</c> 描述涉及 <c>card_id</c> 未授权（族级表复用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceAuthDataResponse : MpResponse
{
    /// <summary>获取或设置发票状态（官方 <c>invoice_status</c>；官方未给枚举表，字段语义见类型 remarks）。</summary>
    [JsonPropertyName("invoice_status")]
    public string? InvoiceStatus { get; set; }

    /// <summary>获取或设置授权时间戳（官方 <c>auth_time</c>；官方未说明单位）。</summary>
    [JsonPropertyName("auth_time")]
    public long? AuthTime { get; set; }
}

// ---------------------------------------------------------------- 获取授权页链接（POST /card/invoice/getauthurl）

/// <summary>
/// 获取授权页链接（<c>POST /card/invoice/getauthurl</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<c>s_pappid</c> / <c>order_id</c> / <c>money</c>（单位<b>分</b>）/
/// <c>timestamp</c> / <c>source</c> / <c>ticket</c> / <c>type</c> 必填；<c>redirect_url</c> 可选。
/// </para>
/// <para>
/// <b><c>ticket</c> 的定位（关键）</b>：官方字段说明为「授权页 ticket」，<b>不是鉴权凭证</b>
/// （本接口鉴权走 Query <c>access_token</c>）；官方文档缺陷（照录）：全文<b>未说明该 ticket 从何获取</b>
/// ——SDK 不代取，归宿主（同族已有「获取 sdk 临时票据」端点可作候选来源，但官方未做关联说明）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：<c>redirect_url</c> 说明为「只有在 <c>source</c> 为 <b>H5</b> 时需要填写」，
/// 但 <c>source</c> 的合法枚举为 <c>app</c>/<c>web</c>/<c>wxa</c>/<c>wap</c>，**不存在 H5 这一取值**
/// （疑指 <c>web</c>）；<c>wxa</c>/<c>wap</c> 是否需该字段未提及。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceAuthUrlRequest
{
    /// <summary>获取或设置开票平台标识号（官方 <c>s_pappid</c>，必填；商户需找开票平台提供）。</summary>
    [JsonPropertyName("s_pappid")]
    public string SPappId { get; set; } = string.Empty;

    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填；商户内单笔开票请求的唯一识别号）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置订单金额（官方 <c>money</c>，必填；以<b>分</b>为单位）。</summary>
    [JsonPropertyName("money")]
    public int Money { get; set; }

    /// <summary>获取或设置时间戳（官方 <c>timestamp</c>，必填；官方未说明单位）。</summary>
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    /// <summary>获取或设置开票来源（官方 <c>source</c>，必填；见 <c>MpInvoiceAuthSources</c>）。</summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>获取或设置授权成功后跳转页面（官方 <c>redirect_url</c>；官方称仅 H5（疑指 web）需要）。</summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    /// <summary>获取或设置授权页 ticket（官方 <c>ticket</c>，必填；<b>非鉴权凭证</b>，官方未说明来源）。</summary>
    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = string.Empty;

    /// <summary>获取或设置授权类型（官方 <c>type</c>，必填；见 <c>MpInvoiceAuthUrlTypes</c>）。</summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }
}

/// <summary>获取授权页链接（<c>POST /card/invoice/getauthurl</c>）响应。</summary>
/// <remarks>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceAuthUrlResponse : MpResponse
{
    /// <summary>获取或设置授权链接（官方 <c>auth_url</c>）。</summary>
    [JsonPropertyName("auth_url")]
    public string? AuthUrl { get; set; }

    /// <summary>获取或设置小程序 appid（官方 <c>appid</c>；<b>仅 source 为 wxa 时才有</b>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

// ---------------------------------------------------------------- 拒绝开票（POST /card/invoice/rejectinsert）

/// <summary>
/// 拒绝开票（<c>POST /card/invoice/rejectinsert</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>s_pappid</c> / <c>order_id</c> / <c>reason</c> 必填；<c>url</c> 可选（跳转链接）。
/// </para>
/// <para>
/// <b>不可逆语义（官方原文，勿弱化）</b>：拒绝开票后该订单<b>无法向用户再次开票</b>；
/// 如需重新开票必须使用<b>新的 <c>order_id</c></b> 并重新获取授权链接让用户再次授权；调用后用户侧会收到通知消息。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceRejectInsertRequest
{
    /// <summary>获取或设置开票平台标识（官方 <c>s_pappid</c>，必填；由开票平台告知商户）。</summary>
    [JsonPropertyName("s_pappid")]
    public string SPappId { get; set; } = string.Empty;

    /// <summary>获取或设置订单 id（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置拒绝原因（官方 <c>reason</c>，必填；如重复开票、抬头无效、已退货无法开票等）。</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>获取或设置跳转链接（官方 <c>url</c>；引导用户重新发起开票 / 重新填写抬头 / 展示订单情况）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

// ---------------------------------------------------------------- 获取开票平台识别码（POST /card/invoice/seturl）

/// <summary>
/// 获取开票平台识别码（<c>POST /card/invoice/seturl</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<b>无请求体</b>；返回 <c>invoice_url</c>（该开票平台专用的授权链接）。
/// </para>
/// <para>
/// <b>官方文档缺陷（照录）</b>：① 接口名「获取开票平台识别码」，但返回字段<b>没有独立的识别码 /
/// <c>s_pappid</c> 字段</b>，仅返回 <c>invoice_url</c> —— <c>s_pappid</c> 须<b>从该 URL 内解析</b>
/// （官方说明反复围绕 <c>s_pappid</c> 却未单列该字段）；② 本页把 <c>errcode</c> 标为 <c>string</c>
/// （官方全站罕见，SDK 仍以 <see cref="MpResponse.ErrorCode"/> 的 <c>int</c> 承载，矛盾照录）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceSetUrlResponse : MpResponse
{
    /// <summary>获取或设置该开票平台专用的授权链接（官方 <c>invoice_url</c>；<c>s_pappid</c> 须从中解析）。</summary>
    [JsonPropertyName("invoice_url")]
    public string? InvoiceUrl { get; set; }
}
