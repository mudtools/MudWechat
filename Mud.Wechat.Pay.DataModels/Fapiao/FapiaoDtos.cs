// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Fapiao;

/// <summary>
/// 开具电子发票（<c>POST /v3/new-tax-control-fapiao/fapiao-applications</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26）。<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>⚠️ 更正一处方案记录</b>：设计方案 §2.5 曾记「普通商户文档中心<b>无</b>独立入口」——
/// 实测<b>不成立</b>：普通商户侧有完整电子发票文档（本页即其一）；服务商侧另有
/// 「开具<b>通用行业</b>电子发票」走 <c>/fapiao-applications/<b>issue-general</b></c>（<b>不同路由</b>）。
/// </para>
/// <para>
/// <b>无应答包体</b>：官方标注应答状态 <b>202 Accepted</b> 且<b>无任何应答字段</b> ⇒ 接口方法返回 <c>Task</c>。
/// 官方原文：本接口成功返回<b>仅代表开票请求已被受理</b>，开票完成须经回调通知或「查询电子发票」获取。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoIssueRequest
{
    /// <summary>场景（<c>scene</c>，必填 string）：开票场景标识（值域未在本轮核验）。</summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }

    /// <summary>发票申请单号（<c>fapiao_apply_id</c>，必填 string(32)）：<b>业务幂等键</b>（查询接口即以此为 path）。</summary>
    [JsonPropertyName("fapiao_apply_id")]
    public string? FapiaoApplyId { get; set; }

    /// <summary>购买方信息（<c>buyer_information</c>，必填），见 <see cref="FapiaoBuyerInformation"/>。</summary>
    [JsonPropertyName("buyer_information")]
    public FapiaoBuyerInformation? BuyerInformation { get; set; }

    /// <summary>发票信息（<c>fapiao_information</c>，必填数组），见 <see cref="FapiaoIssueInformation"/>。</summary>
    [JsonPropertyName("fapiao_information")]
    public List<FapiaoIssueInformation>? FapiaoInformation { get; set; }
}

/// <summary>
/// 购买方信息（<c>buyer_information</c>）—— 开具请求与查询应答<b>字段表一致 ⇒ 共用</b>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoBuyerInformation
{
    /// <summary>购买方类型（<c>type</c>）：如个人 / 企业（值域未在本轮核验）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>购买方名称（<c>name</c>，string(256)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>纳税人识别号（<c>taxpayer_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>地址（<c>address</c>，选填 string(128)）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>电话（<c>telephone</c>，选填 string(32)）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>开户行（<c>bank_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>bank_account</c>，选填 string(32)）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }

    /// <summary>手机号（<c>phone</c>，选填）：用于发票交付。</summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>邮箱（<c>email</c>，选填）：用于发票交付。</summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}

/// <summary>开具请求的单张发票信息（<c>fapiao_information[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoIssueInformation
{
    /// <summary>商户发票单号（<c>fapiao_id</c>，必填 string(32)）：<b>同一申请单下的发票唯一标识</b>（查询接口可选按它过滤）。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>发票总金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>是否要明细（<c>need_list</c>，选填 bool）。</summary>
    [JsonPropertyName("need_list")]
    public bool? NeedList { get; set; }

    /// <summary>备注（<c>remark</c>，选填 string(200)）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>发票明细（<c>items</c>，必填数组），见 <see cref="FapiaoIssueItem"/>。</summary>
    [JsonPropertyName("items")]
    public List<FapiaoIssueItem>? Items { get; set; }
}

/// <summary>开具请求的发票明细项（<c>fapiao_information[].items[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoIssueItem
{
    /// <summary>税收分类编码（<c>tax_code</c>，必填 string(32)）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>商品分类（<c>goods_category</c>，选填 string(128)）。</summary>
    [JsonPropertyName("goods_category")]
    public string? GoodsCategory { get; set; }

    /// <summary>商品名称（<c>goods_name</c>，选填 string(128)）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品编码（<c>goods_id</c>，选填 integer）。</summary>
    [JsonPropertyName("goods_id")]
    public long? GoodsId { get; set; }

    /// <summary>规格型号（<c>specification</c>，选填 string(20)）。</summary>
    [JsonPropertyName("specification")]
    public string? Specification { get; set; }

    /// <summary>单位（<c>unit</c>，选填 string(20)）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>数量（<c>quantity</c>，必填 integer）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>税率（<c>tax_rate</c>，选填 integer）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>税收优惠标识（<c>tax_prefer_mark</c>，选填 string）。</summary>
    [JsonPropertyName("tax_prefer_mark")]
    public string? TaxPreferMark { get; set; }

    /// <summary>是否优惠（<c>discount</c>，<b>必填</b> bool，官方本页标为必填）。</summary>
    [JsonPropertyName("discount")]
    public bool? Discount { get; set; }
}

/// <summary>
/// 查询电子发票应答（<c>GET /v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}</c>）。
/// </summary>
/// <remarks>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26；支持商户：普通商户）。官方原文建议：
/// 「【将电子发票插入微信用户卡包】接口成功后，应调用本接口查询电子发票开票结果」。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoQueryResponse : WechatPayResponse
{
    /// <summary>发票总数（<c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public long? TotalCount { get; set; }

    /// <summary>发票信息列表（<c>fapiao_information</c>），见 <see cref="FapiaoQueryInformation"/>。</summary>
    [JsonPropertyName("fapiao_information")]
    public List<FapiaoQueryInformation>? FapiaoInformation { get; set; }
}

/// <summary>查询应答的单张发票信息（<c>fapiao_information[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoQueryInformation
{
    /// <summary>商户发票单号（<c>fapiao_id</c>）。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>发票状态（<c>status</c>）：<b>值域未在本轮核验</b>，勿臆造取值判定。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>蓝票信息（<c>blue_fapiao</c>），见 <see cref="FapiaoBlueRedInformation"/>。</summary>
    [JsonPropertyName("blue_fapiao")]
    public FapiaoBlueRedInformation? BlueFapiao { get; set; }

    /// <summary>红票信息（<c>red_fapiao</c>），见 <see cref="FapiaoBlueRedInformation"/>。</summary>
    [JsonPropertyName("red_fapiao")]
    public FapiaoBlueRedInformation? RedFapiao { get; set; }

    /// <summary>卡包信息（<c>card_information</c>），见 <see cref="FapiaoCardInformation"/>。</summary>
    [JsonPropertyName("card_information")]
    public FapiaoCardInformation? CardInformation { get; set; }

    /// <summary>价税合计（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>税额（<c>tax_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("tax_amount")]
    public long? TaxAmount { get; set; }

    /// <summary>金额（不含税，<c>amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>销售方信息（<c>seller_information</c>），见 <see cref="FapiaoSellerInformation"/>。</summary>
    [JsonPropertyName("seller_information")]
    public FapiaoSellerInformation? SellerInformation { get; set; }

    /// <summary>购买方信息（<c>buyer_information</c>）：与开具请求<b>字段表一致</b> ⇒ 复用 <see cref="FapiaoBuyerInformation"/>。</summary>
    [JsonPropertyName("buyer_information")]
    public FapiaoBuyerInformation? BuyerInformation { get; set; }

    /// <summary>附加信息（<c>extra_information</c>），见 <see cref="FapiaoExtraInformation"/>。</summary>
    [JsonPropertyName("extra_information")]
    public FapiaoExtraInformation? ExtraInformation { get; set; }

    /// <summary>发票明细（<c>items</c>），见 <see cref="FapiaoQueryItem"/>。</summary>
    [JsonPropertyName("items")]
    public List<FapiaoQueryItem>? Items { get; set; }

    /// <summary>备注（<c>remark</c>）：官方渲染中位于 <c>items</c> 之后、与本层级同级。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}

/// <summary>蓝票 / 红票信息（<c>blue_fapiao</c> / <c>red_fapiao</c>，官方两处字段表一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoBlueRedInformation
{
    /// <summary>发票代码（<c>fapiao_code</c>）。</summary>
    [JsonPropertyName("fapiao_code")]
    public string? FapiaoCode { get; set; }

    /// <summary>发票号码（<c>fapiao_number</c>）。</summary>
    [JsonPropertyName("fapiao_number")]
    public string? FapiaoNumber { get; set; }

    /// <summary>校验码（<c>check_code</c>）。</summary>
    [JsonPropertyName("check_code")]
    public string? CheckCode { get; set; }

    /// <summary>密码区（<c>password</c>）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>开票时间（<c>fapiao_time</c>）。</summary>
    [JsonPropertyName("fapiao_time")]
    public string? FapiaoTime { get; set; }
}

/// <summary>发票卡包信息（<c>card_information</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoCardInformation
{
    /// <summary>卡券 AppID（<c>card_appid</c>）。</summary>
    [JsonPropertyName("card_appid")]
    public string? CardAppId { get; set; }

    /// <summary>卡券持有者 OpenID（<c>card_openid</c>）。</summary>
    [JsonPropertyName("card_openid")]
    public string? CardOpenId { get; set; }

    /// <summary>卡券 ID（<c>card_id</c>）。</summary>
    [JsonPropertyName("card_id")]
    public string? CardId { get; set; }

    /// <summary>卡券 code（<c>card_code</c>）。</summary>
    [JsonPropertyName("card_code")]
    public string? CardCode { get; set; }

    /// <summary>卡券状态（<c>card_status</c>）：值域未在本轮核验。</summary>
    [JsonPropertyName("card_status")]
    public string? CardStatus { get; set; }
}

/// <summary>销售方信息（<c>seller_information</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoSellerInformation
{
    /// <summary>销售方名称（<c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>纳税人识别号（<c>taxpayer_id</c>）。</summary>
    [JsonPropertyName("taxpayer_id")]
    public string? TaxpayerId { get; set; }

    /// <summary>地址（<c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>电话（<c>telephone</c>）。</summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>开户行（<c>bank_name</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>银行账号（<c>bank_account</c>）。</summary>
    [JsonPropertyName("bank_account")]
    public string? BankAccount { get; set; }
}

/// <summary>附加信息（<c>extra_information</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoExtraInformation
{
    /// <summary>收款人（<c>payee</c>）。</summary>
    [JsonPropertyName("payee")]
    public string? Payee { get; set; }

    /// <summary>复核人（<c>reviewer</c>）。</summary>
    [JsonPropertyName("reviewer")]
    public string? Reviewer { get; set; }

    /// <summary>开票人（<c>drawer</c>）。</summary>
    [JsonPropertyName("drawer")]
    public string? Drawer { get; set; }
}

/// <summary>查询应答的发票明细项（<c>fapiao_information[].items[]</c>）。</summary>
/// <remarks>
/// <b>与开具请求的明细项不是同一张表</b>：应答版多出 <c>unit_price</c> / <c>amount</c> / <c>tax_amount</c>
/// 且 <c>discount</c> 未标必填、<c>goods_category</c>/<c>goods_id</c>/<c>need_list</c> 不在表内
/// ⇒ 独立类型（同一份事实两处类型会漂移）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "NewTaxControlFapiao")]
public class FapiaoQueryItem
{
    /// <summary>税收分类编码（<c>tax_code</c>）。</summary>
    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    /// <summary>商品名称（<c>goods_name</c>）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>规格型号（<c>specification</c>）。</summary>
    [JsonPropertyName("specification")]
    public string? Specification { get; set; }

    /// <summary>单位（<c>unit</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>数量（<c>quantity</c>）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>单价（<c>unit_price</c>，整型，单位分）：<b>仅应答版有</b>。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>金额（<c>amount</c>，整型，单位分）：<b>仅应答版有</b>。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>税额（<c>tax_amount</c>，整型，单位分）：<b>仅应答版有</b>。</summary>
    [JsonPropertyName("tax_amount")]
    public long? TaxAmount { get; set; }

    /// <summary>价税合计（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>税率（<c>tax_rate</c>）。</summary>
    [JsonPropertyName("tax_rate")]
    public long? TaxRate { get; set; }

    /// <summary>税收优惠标识（<c>tax_prefer_mark</c>）。</summary>
    [JsonPropertyName("tax_prefer_mark")]
    public string? TaxPreferMark { get; set; }

    /// <summary>是否优惠（<c>discount</c>，bool）：应答版未标必填。</summary>
    [JsonPropertyName("discount")]
    public bool? Discount { get; set; }
}
