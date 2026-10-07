// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Invoice;

// ---------------------------------------------------------------- 发票报销（reimburse/*）

/// <summary>
/// 查询报销发票信息（<c>POST /card/invoice/reimburse/getinvoiceinfo</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档缺陷（照录，SDK 按「存在性冲突取并集」建模）</b>：官方「请求体 REQUEST PAYLOAD」栏
/// <b>标注为「无」</b>，但同页<b>代码示例</b>给出 <c>card_id</c> 与 <c>encrypt_code</c> 两个字段
/// （未标类型 / 必填）——若按「无请求体」建模该端点将无法寻址发票 ⇒ SDK 按示例建模，
/// 并由守卫锁定（<c>MpInvoiceReimburseUpdateRequest</c> 同处此缺陷）。
/// </para>
/// <para>
/// 官方发票状态码（原文）：<c>INVOICE_REIMBURSE_INIT</c> 初始（未锁定，可提交报销）/
/// <c>INVOICE_REIMBURSE_LOCK</c> 已锁定（无法重复提交报销）/
/// <c>INVOICE_REIMBURSE_CLOSURE</c> 已核销（从用户卡包中移除）——见 <c>MpInvoiceReimburseStatuses</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseQueryRequest
{
    /// <summary>获取或设置发票卡券 id（官方请求示例字段 <c>card_id</c>；请求体表标注「无」，照录）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code（官方请求示例字段 <c>encrypt_code</c>）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;
}

/// <summary>
/// 报销发票信息（<c>getinvoiceinfo</c> 的<b>响应体</b>与 <c>getinvoicebatch</c> 的
/// <b><c>item_list[]</c> 条目</b>字段集一致 ⇒ <b>共用本类型</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>共用裁决</b>：单张查询的响应为<b>平级</b>业务字段（+ 根 <c>errcode</c>/<c>errmsg</c>），
/// 批量查询的条目为<b>同一组业务字段</b>（无 <c>errcode</c>）——两者字段集<b>完全一致</b>，
/// 故本类型继承 <see cref="MpResponse"/>（条目位置的 <c>errcode</c> 缺省为 0，天然成立）。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：批量返回示例中还出现未在字段表列出的 <c>order_id</c> ⇒ SDK <b>不建模</b>
/// （示例独有且无字段表形态说明）。
/// </para>
/// <para>官方错误码：<c>0</c>（官方把成功码也列入表，照录）/ <c>72015</c> / <c>72017</c> / <c>72023</c> / <c>72024</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseInfo : MpResponse
{
    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>获取或设置开始时间（官方 <c>begin_time</c>；官方未说明单位）。</summary>
    [JsonPropertyName("begin_time")]
    public long? BeginTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>；官方未说明单位）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置用户 openid（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置发票类型（官方 <c>type</c>）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置收款方（官方 <c>payee</c>）。</summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }

    /// <summary>获取或设置发票详情（官方 <c>detail</c>；官方未给出结构说明）。</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>获取或设置用户发票信息（官方 <c>user_info</c>；与插卡请求的 <c>invoice_user_data</c> 同构 ⇒ 共用 <see cref="MpInvoiceUserData"/>）。</summary>
    [JsonPropertyName("user_info")]
    public MpInvoiceUserData? UserInfo { get; set; }
}

/// <summary>
/// 更新报销发票状态（<c>POST /card/invoice/reimburse/updateinvoicestatus</c>）请求体。
/// </summary>
/// <remarks>
/// 官方文档缺陷（照录）：与 <c>getinvoiceinfo</c> 同——官方请求体栏标「无」而示例给出三字段 ⇒ 按示例建模。
/// 官方原文：对某一张发票进行<b>锁定 / 解锁 / 报销</b>操作。<b>报销状态不可逆</b>（报销后发票从用户卡包移除）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseUpdateRequest
{
    /// <summary>获取或设置发票卡券 id（官方请求示例字段 <c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code（官方请求示例字段 <c>encrypt_code</c>）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;

    /// <summary>获取或设置报销状态（官方请求示例字段 <c>reimburse_status</c>；见 <c>MpInvoiceReimburseStatuses</c>）。</summary>
    [JsonPropertyName("reimburse_status")]
    public string ReimburseStatus { get; set; } = string.Empty;
}

/// <summary>
/// 批量更新报销发票状态（<c>POST /card/invoice/reimburse/updatestatusbatch</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：<c>openid</c> / <c>reimburse_status</c> / <c>invoice_list</c> 均必填。
/// </para>
/// <para>
/// <b>事务性语义（官方注意事项原文，勿弱化）</b>：① 「报销方须保证在报销、锁定、解锁后<b>及时将状态同步至微信侧</b>」；
/// ② 「批量更新发票状态接口为<b>事务性操作</b>，如果其中一张发票更新失败，列表中的其它发票状态更新也会无法执行，
/// <b>恢复到接口调用前的状态</b>」；③ 「<b>报销状态为不可逆状态，请开发者慎重调用</b>」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseBatchUpdateRequest
{
    /// <summary>获取或设置用户 openid（官方 <c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置报销状态（官方 <c>reimburse_status</c>，必填；见 <c>MpInvoiceReimburseStatuses</c>）。</summary>
    [JsonPropertyName("reimburse_status")]
    public string ReimburseStatus { get; set; } = string.Empty;

    /// <summary>获取或设置发票列表（官方 <c>invoice_list</c>，必填）。</summary>
    [JsonPropertyName("invoice_list")]
    public List<MpInvoiceRef> InvoiceList { get; set; } = new List<MpInvoiceRef>();
}

/// <summary>
/// 发票定位（官方 <c>card_id</c> + <c>encrypt_code</c> 组合；批量更新与批量查询共用）。
/// </summary>
/// <remarks>官方原文：<c>card_id</c> 与 <c>encrypt_code</c> <b>共同构成一张发票卡券的唯一标识</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceRef
{
    /// <summary>获取或设置发票卡券 id（官方 <c>card_id</c>，必填）。</summary>
    [JsonPropertyName("card_id")]
    public string CardId { get; set; } = string.Empty;

    /// <summary>获取或设置加密 code（官方 <c>encrypt_code</c>，必填）。</summary>
    [JsonPropertyName("encrypt_code")]
    public string EncryptCode { get; set; } = string.Empty;
}

/// <summary>
/// 批量获取报销发票信息（<c>POST /card/invoice/reimburse/getinvoicebatch</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>item_list</c>（发票列表）必填；条目字段 <c>card_id</c>/<c>encrypt_code</c> 仅见代码示例
/// （请求体表未单列）⇒ 按示例建模为 <see cref="MpInvoiceRef"/>。
/// 官方文档缺陷（照录）：<c>item_list</c> <b>未标注最大条数</b>（SDK 不本地拦截）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseBatchGetRequest
{
    /// <summary>获取或设置发票列表（官方 <c>item_list</c>，必填；官方未标注最大条数）。</summary>
    [JsonPropertyName("item_list")]
    public List<MpInvoiceRef> ItemList { get; set; } = new List<MpInvoiceRef>();
}

/// <summary>
/// 批量获取报销发票信息（<c>POST /card/invoice/reimburse/getinvoicebatch</c>）响应。
/// </summary>
/// <remarks>
/// 官方错误码：<c>0</c>（官方描述为「ok 或者 in a normal state」，照录）/ <c>72015</c> / <c>72017</c> / <c>72023</c> / <c>72024</c>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceReimburseBatchGetResponse : MpResponse
{
    /// <summary>获取或设置发票列表（官方 <c>item_list</c>；条目与单张查询响应共用 <see cref="MpInvoiceReimburseInfo"/>）。</summary>
    [JsonPropertyName("item_list")]
    public List<MpInvoiceReimburseInfo>? ItemList { get; set; }
}

// ---------------------------------------------------------------- 极速开发票（biz/* 与 scantitle）

/// <summary>
/// 录入抬头到用户微信（<c>POST /card/invoice/biz/getusertitleurl</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（全部字段标非必填，但语义上条件必填，SDK 不本地校验）：
/// <c>title</c>（<c>user_fill = 0</c> 时必填）/ <c>phone</c>（须数字或「-」）/
/// <c>tax_no</c>（须 15-20 位数字或英文字母）/ <c>addr</c> / <c>bank_type</c> / <c>bank_no</c> /
/// <c>user_fill</c>（0 企业设置的抬头 / 1 用户自己填写抬头）/ <c>out_title_id</c>（开票码）。
/// </para>
/// <para>官方文档缺陷（照录）：返回示例出现重复键 <c>errcode</c>（第二个应为 <c>errmsg</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUserTitleUrlRequest
{
    /// <summary>获取或设置发票抬头（官方 <c>title</c>；<c>user_fill</c> 为 0 时必填）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置联系方式（官方 <c>phone</c>；官方原文「必须是数字或 "-"」）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>获取或设置税号（官方 <c>tax_no</c>；官方原文「必须是 15-20 位数字或英文字母」，SDK 不本地拦截）。</summary>
    [JsonPropertyName("tax_no")]
    public string? TaxNumber { get; set; }

    /// <summary>获取或设置地址（官方 <c>addr</c>）。</summary>
    [JsonPropertyName("addr")]
    public string? Address { get; set; }

    /// <summary>获取或设置银行类型（官方 <c>bank_type</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>获取或设置银行号码（官方 <c>bank_no</c>）。</summary>
    [JsonPropertyName("bank_no")]
    public string? BankNumber { get; set; }

    /// <summary>获取或设置抬头来源（官方 <c>user_fill</c>；0 企业设置的抬头 / 1 用户自己填写抬头）。</summary>
    [JsonPropertyName("user_fill")]
    public int? UserFill { get; set; }

    /// <summary>获取或设置开票码（官方 <c>out_title_id</c>）。</summary>
    [JsonPropertyName("out_title_id")]
    public string? OutTitleId { get; set; }
}

/// <summary>
/// 录入抬头到用户微信（<c>POST /card/invoice/biz/getusertitleurl</c>）响应。
/// </summary>
/// <remarks>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceUserTitleUrlResponse : MpResponse
{
    /// <summary>获取或设置用户确认链接（官方 <c>url</c>；需将链接发送给用户确认）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 获取商户专属抬头链接（<c>POST /card/invoice/biz/getselecttitleurl</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>attach</c>（附加字段，用户提交发票时会发给商户）/ <c>biz_name</c>（将商户名称显示给用户看）；
/// 官方原文「需将链接转为二维码展示」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceSelectTitleUrlRequest
{
    /// <summary>获取或设置附加字段（官方 <c>attach</c>；用户提交发票时会发给商户）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>获取或设置商户名称（官方 <c>biz_name</c>；将商户名称显示给用户看）。</summary>
    [JsonPropertyName("biz_name")]
    public string? BizName { get; set; }
}

/// <summary>
/// 获取商户专属抬头链接（<c>POST /card/invoice/biz/getselecttitleurl</c>）响应。
/// </summary>
/// <remarks>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceSelectTitleUrlResponse : MpResponse
{
    /// <summary>获取或设置专属抬头链接（官方 <c>url</c>；需转为二维码展示）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// 解析扫描的抬头二维码（<c>POST /card/invoice/scantitle</c>）请求体。
/// </summary>
/// <remarks>
/// 官方契约：<c>scan_text</c>（扫码获取的原始数据；示例值为
/// <c>https://mp.weixin.qq.com/intp/invoice/usertitlewxa?…</c> 形态的 URL）。
/// 官方文档缺陷（照录）：代码示例把字段名写成 <c>"scan_ text"</c>（中间多一空格），与参数表不一致。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceScanTitleRequest
{
    /// <summary>获取或设置扫码获取的原始数据（官方 <c>scan_text</c>，必填）。</summary>
    [JsonPropertyName("scan_text")]
    public string ScanText { get; set; } = string.Empty;
}

/// <summary>
/// 解析扫描的抬头二维码（<c>POST /card/invoice/scantitle</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>title_type</c>（0 单位抬头 / 1 个人抬头，见 <c>MpInvoiceTitleTypes</c>）/ <c>title</c> /
/// <c>phone</c> / <c>tax_no</c> / <c>addr</c> / <c>bank_type</c> / <c>bank_no</c>。
/// </para>
/// <para>
/// 官方文档缺陷（照录）：返回示例<b>缺失</b> <c>title_type</c> 与 <c>bank_no</c>（示例不完整）；
/// 示例 <c>tax_no</c> 为 10 位（与税号常见长度不符，疑占位）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Invoice")]
public class MpInvoiceScanTitleResponse : MpResponse
{
    /// <summary>获取或设置抬头类型（官方 <c>title_type</c>；0 单位抬头 / 1 个人抬头）。</summary>
    [JsonPropertyName("title_type")]
    public int? TitleType { get; set; }

    /// <summary>获取或设置发票抬头（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置联系方式（官方 <c>phone</c>）。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>获取或设置税号（官方 <c>tax_no</c>）。</summary>
    [JsonPropertyName("tax_no")]
    public string? TaxNumber { get; set; }

    /// <summary>获取或设置地址（官方 <c>addr</c>）。</summary>
    [JsonPropertyName("addr")]
    public string? Address { get; set; }

    /// <summary>获取或设置银行类型（官方 <c>bank_type</c>）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>获取或设置银行号码（官方 <c>bank_no</c>；官方示例缺失该字段，照录）。</summary>
    [JsonPropertyName("bank_no")]
    public string? BankNumber { get; set; }
}
