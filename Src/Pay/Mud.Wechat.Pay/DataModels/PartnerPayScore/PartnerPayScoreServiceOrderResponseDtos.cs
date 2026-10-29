// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PartnerPayScore;

/// <summary>
/// 服务商创建支付分订单应答（<b>POST /v3/payscore/partner/serviceorder</b> 的 200 报文）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692136"/>，2026-10-14 逐字段核验。</para>
/// <para><b>与查询应答不是同一形态</b>（官方两页字段表不同 ⇒ 分建两类型）：本应答有 <c>package</c> 与 <c>order_id</c>，
/// 但<b>无</b> <c>total_amount</c> / <c>need_collection</c> / <c>collection</c> / <c>promotion_detail</c> /
/// <c>openid</c> / <c>sub_openid</c>；查询应答反之（无 <c>package</c>）。</para>
/// <para><b>服务商商户号在本域叫 <c>mchid</c></b>：官方本应答表标注「[服务商商户号]」，
/// 与服务商基础支付族的 <c>sp_mchid</c> 拼写不同 —— 勿按那一族改写。</para>
/// <para><b>免确认模式可忽略 <c>package</c></b>：官方原文注明该参数用于拉起微信侧订单页，
/// 本域 <c>need_user_confirm</c> 固定 <c>false</c>，故实际业务无需使用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreCreateOrderResponse : WechatPayResponse
{
    /// <summary>商户服务订单号（<c>out_order_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商 AppID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务商商户号（<c>mchid</c>，必填 string(32)）：官方本域字段名为 <c>mchid</c> 而非 <c>sp_mchid</c>。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填 string(32)）：见 <see cref="PartnerPayScoreServiceOrderStates"/>，须结合流转图判断。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填 string(32)）：官方注明<b>仅 DOING 状态返回</b>，见 <see cref="PartnerPayScoreStateDescriptions"/>。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PartnerPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PartnerPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）：外层选填而其 <c>name</c>/<c>amount</c> 官方仍标必填（照录）。</summary>
    [JsonPropertyName("risk_fund")]
    public PartnerPayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PartnerPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public PartnerPayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(256)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，选填 string(64)）：31 位数字，开头由 <c>1000000000</c> + 年月日组成。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 跳转微信侧数据包（<c>package</c>，选填 string(1024)）：官方注明「数字大小写字母 <c>_-</c> 组成、不超过 300 字符，
    /// <b>免确认模式可忽略</b>」—— 长度上限按官方字段表 <c>string(1024)</c> 记录。
    /// </summary>
    [JsonPropertyName("package")]
    public string? Package { get; set; }
}

/// <summary>
/// 服务商查询支付分订单应答（<b>GET /v3/payscore/partner/serviceorder</b> 的 200 报文）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012692256"/>，2026-10-14 逐字段核验。
/// 官方原文要求按「支付分订单状态流转图」处理状态，不得凭直觉跳转。</para>
/// <para><b><c>sub_mchid</c> 在本应答降为选填</b>（请求 query 为必填）：本域三处 <c>sub_mchid</c> 的必填性
/// 随页面/位置而变（守卫 PS-B1 逐点锁定）。</para>
/// <para><b><c>openid</c> 与 <c>sub_openid</c> 按是否传 <c>sub_appid</c> 分别返回</b>：官方原文
/// 「不传 sub_appid 的情况下返回 <c>openid</c>」「传了 sub_appid 的情况下返回 <c>sub_openid</c>」。</para>
/// <para><b><c>collection</c> 仅在调完结后返回</b>，且完结时 <c>total_amount</c> 为 0 元时官方<b>不返回</b>该节；
/// <c>need_collection</c> 官方注明<b>固定返回 <c>true</c></b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreQueryOrderResponse : WechatPayResponse
{
    /// <summary>商户服务订单号（<c>out_order_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商 AppID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务商商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，<b>选填</b> string(32)）：请求侧同名字段为必填，本应答为选填。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填 string(32)）：见 <see cref="PartnerPayScoreServiceOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填 string(32)）：仅 DOING 返回，见 <see cref="PartnerPayScoreStateDescriptions"/>。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PartnerPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PartnerPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）。</summary>
    [JsonPropertyName("risk_fund")]
    public PartnerPayScoreRiskFund? RiskFund { get; set; }

    /// <summary>订单最终收款总金额（<c>total_amount</c>，选填整型，单位分）：由完结或修改订单接口传入。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>是否需要收款（<c>need_collection</c>，选填 boolean）：官方注明本域<b>固定返回 <c>true</c></b>。</summary>
    [JsonPropertyName("need_collection")]
    public bool? NeedCollection { get; set; }

    /// <summary>收款信息（<c>collection</c>，选填），见 <see cref="PartnerPayScoreCollection"/>；仅完结后返回，完结金额为 0 时不返回。</summary>
    [JsonPropertyName("collection")]
    public PartnerPayScoreCollection? Collection { get; set; }

    /// <summary>优惠信息（<c>promotion_detail</c>，选填）：官方本页字段表把它列在<b>顶层</b>（订单支付用到代金券时才返回）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<PartnerPayScorePromotionDetail>? PromotionDetail { get; set; }

    /// <summary>服务时间（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PartnerPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public PartnerPayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(256)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>用户在服务商 <c>appid</c> 下的标识（<c>openid</c>，选填 string(128)）：官方注明「不传 <c>sub_appid</c> 时返回」。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户在子商户 <c>sub_appid</c> 下的标识（<c>sub_openid</c>，选填 string(128)）：官方注明「传 <c>sub_appid</c> 时返回」。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，选填 string(64)）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>收款信息（<c>collection</c>，查询应答）。</summary>
/// <remarks>
/// 官方：<b>仅在调用完结订单后返回</b>；若完结时 <c>total_amount</c> 为 0 元则<b>不返回本节</b>。
/// <c>state</c> 须结合订单 <c>state</c> 与 <c>state_description</c> 一起判断（官方流转图）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreCollection
{
    /// <summary>收款状态（<c>state</c>，必填 string(32)）：见 <see cref="PartnerPayScoreCollectionStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总收款金额（<c>total_amount</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>待收金额（<c>paying_amount</c>，选填整型，单位分）：完结成功后、收款成功前等于总额，收款成功后为 0。</summary>
    [JsonPropertyName("paying_amount")]
    public long? PayingAmount { get; set; }

    /// <summary>已收金额（<c>paid_amount</c>，选填整型，单位分）：收款成功后等于总额。</summary>
    [JsonPropertyName("paid_amount")]
    public long? PaidAmount { get; set; }

    /// <summary>收款明细（<c>details</c>，选填），见 <see cref="PartnerPayScoreCollectionDetail"/>。</summary>
    [JsonPropertyName("details")]
    public List<PartnerPayScoreCollectionDetail>? Details { get; set; }
}

/// <summary>收款明细（<c>collection.details[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreCollectionDetail
{
    /// <summary>收款序号（<c>seq</c>，选填整型）：从 1 开始递增。</summary>
    [JsonPropertyName("seq")]
    public long? Seq { get; set; }

    /// <summary>单笔收款金额（<c>amount</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>
    /// 收款渠道（<c>paid_type</c>，选填 string(32)）：本域取值 <c>NEWTON</c>（支付分渠道）/ <c>MCH</c>（商户渠道），
    /// 见 <see cref="PartnerPayScoreCollectionPaidTypes"/> —— <b>与直连支付分域的 <c>ADVANCE</c>/<c>BALANCE</c> 取值集不同</b>。
    /// </summary>
    [JsonPropertyName("paid_type")]
    public string? PaidType { get; set; }

    /// <summary>收款成功时间（<c>paid_time</c>，选填 string(32)，<b><c>yyyyMMddHHmmss</c></b>）：非 rfc3339。</summary>
    [JsonPropertyName("paid_time")]
    public string? PaidTime { get; set; }

    /// <summary>
    /// 微信支付交易单号（<c>transaction_id</c>，选填 string(64)）：仅支付分渠道收款成功才返回，
    /// 等于普通支付接口的 <c>transaction_id</c> —— <b>申请退款用该单号，而非 <c>out_order_no</c></b>。
    /// </summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}

/// <summary>优惠信息（<c>promotion_detail[]</c>，查询应答）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScorePromotionDetail
{
    /// <summary>券 ID（<c>coupon_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>优惠名称（<c>name</c>，选填 string(64)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠范围（<c>scope</c>，选填 string(32)）：<c>GLOBAL</c> 全场 / <c>SINGLE</c> 单品，见 <see cref="PartnerPayScorePromotionScopes"/>。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>优惠类型（<c>type</c>，选填 string(32)）：<c>CASH</c> 预充值 / <c>NOCASH</c> 免充值，见 <see cref="PartnerPayScorePromotionTypes"/>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>优惠券面额（<c>amount</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>活动 ID / 批次号（<c>stock_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>微信出资（<c>wechatpay_contribute</c>，选填整型）：官方注明三个出资字段<b>仅一个</b>会返回值，取决于券批次配置。</summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatpayContribute { get; set; }

    /// <summary>商户出资（<c>merchant_contribute</c>，选填整型）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>其他出资（<c>other_contribute</c>，选填整型）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

    /// <summary>优惠币种（<c>currency</c>，选填 string(32)）：境内商户号仅支持 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>单品列表（<c>goods_detail</c>，选填），见 <see cref="PartnerPayScorePromotionGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<PartnerPayScorePromotionGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>优惠的单品信息（<c>promotion_detail[].goods_detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScorePromotionGoodsDetail
{
    /// <summary>商品编码（<c>goods_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

    /// <summary>商品数量（<c>quantity</c>，选填整型）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>商品价格（<c>unit_price</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>商品优惠金额（<c>discount_amount</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>商品备注（<c>goods_remark</c>，选填 string(128)）。</summary>
    [JsonPropertyName("goods_remark")]
    public string? GoodsRemark { get; set; }
}

/// <summary>
/// 服务商「查询用户分层对应建议先享金额」应答
/// （<b>GET /v3/payscore/partner/servicequotas/service-id/{service_id}</b> 的 200 报文）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013394486"/>，2026-10-14 逐字段核验。</para>
/// <para><b>本端点是本族唯一 path + query 混合的 GET</b>：<c>service_id</c> 在 path，
/// 其余四个（<c>sub_mchid</c>/<c>appid</c>/<c>sub_appid</c>/<c>risk_level_version</c>）全在 query。</para>
/// <para><b><c>risk_level_version</c> 与 <c>risk_level</c> 都是整型</b>：官方本域「分层版本」「用户分层」用
/// <c>integer</c>，而风险值评估应答里的 <c>user_risk_level</c>/<c>risk_level_version</c> 同为整型 —— 勿按字符串枚举处理。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreServiceQuotaResponse : WechatPayResponse
{
    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务商商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>服务商 AppID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>子商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>分层版本（<c>risk_level_version</c>，必填整型）：同请求上送值，表示返回列表归属的数据版本。</summary>
    [JsonPropertyName("risk_level_version")]
    public int? RiskLevelVersion { get; set; }

    /// <summary>先享金额列表（<c>quotas</c>，必填），见 <see cref="PartnerPayScoreServiceQuota"/>。</summary>
    [JsonPropertyName("quotas")]
    public List<PartnerPayScoreServiceQuota>? Quotas { get; set; }
}

/// <summary>建议先享金额（<c>quotas[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PartnerPayScore")]
public class PartnerPayScoreServiceQuota
{
    /// <summary>用户分层（<c>risk_level</c>，必填整型）：与查询用户风险等级应答的 <c>user_risk_level</c> 同版本下对应。</summary>
    [JsonPropertyName("risk_level")]
    public int? RiskLevel { get; set; }

    /// <summary>建议先享金额（<c>suggested_quota</c>，必填整型，单位分）。</summary>
    [JsonPropertyName("suggested_quota")]
    public int? SuggestedQuota { get; set; }
}
