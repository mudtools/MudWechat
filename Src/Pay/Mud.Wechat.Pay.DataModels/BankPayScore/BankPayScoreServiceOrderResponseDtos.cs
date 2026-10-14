// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.BankPayScore;

/// <summary>
/// 从业机构创建支付分订单应答（<b>POST /v3/payscore/acquiringbank/serviceorder</b> 的 200 报文）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691326"/>，2026-10-14 逐字段核验。</para>
/// <para><b>与查询应答不是同一形态</b>（官方两页字段表不同 ⇒ 分建两类型）：本应答有 <c>package</c>，
/// 但<b>无</b> <c>total_amount</c> / <c>collection</c> / <c>promotion_detail</c> / <c>openid</c> / <c>sub_openid</c>；
/// 查询应答反之（无 <c>package</c>）。</para>
/// <para><b>免确认模式可忽略 <c>package</c></b>：官方原文注明该参数用于拉起微信侧订单页，
/// 本域 <c>need_user_confirm</c> 固定 <c>false</c> ⇒ 实际业务无需使用。</para>
/// <para><b>本域商户号字段名为 <c>mchid</c></b>（官方标注「[从业机构商户号]」），
/// 与服务商基础支付族的 <c>sp_mchid</c> 拼写不同 —— 勿按那一族改写。
/// <c>channel_id</c>（渠道商商户号）在本应答为<b>必填</b>，是本域区别于服务商支付分域的恒定多出一节。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreCreateOrderResponse : WechatPayResponse
{
    /// <summary>商户服务订单号（<c>out_order_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>从业机构 AppID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>从业机构商户号（<c>mchid</c>，必填 string(32)）：本域字段名为 <c>mchid</c> 而非 <c>sp_mchid</c>。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）：本域独有节，服务商支付分域应答无此字段。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填 string(32)）：见 <see cref="BankPayScoreServiceOrderStates"/>，须结合 <c>collection.state</c> 与 <c>state_description</c> 及官方流转图判断。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填 string(32)）：官方注明<b>仅 DOING 状态返回</b>，见 <see cref="BankPayScoreStateDescriptions"/>。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<BankPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<BankPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）：外层选填而其 <c>name</c>/<c>amount</c> 官方仍标必填（照录）。</summary>
    [JsonPropertyName("risk_fund")]
    public BankPayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public BankPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public BankPayScoreLocation? Location { get; set; }

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
    /// 跳转微信侧小程序订单数据（<c>package</c>，选填 string(1024)）：官方注明「数字大小写字母 <c>_-</c> 组成、
    /// 不超过 300 字符，<b>免确认模式可忽略</b>」—— 长度上限按官方字段表 <c>string(1024)</c> 记录。
    /// </summary>
    [JsonPropertyName("package")]
    public string? Package { get; set; }
}

/// <summary>
/// 从业机构查询支付分订单应答（<b>GET /v3/payscore/acquiringbank/serviceorder</b> 的 200 报文）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012691343"/>，2026-10-14 逐字段核验。
/// 官方原文要求按「支付分订单状态流转图」处理状态，不得凭直觉跳转。</para>
/// <para><b><c>sub_mchid</c> 在本应答降为选填</b>（请求 query 为必填）而 <c>channel_id</c> 恒为必填：
/// 本域必填性随页面/位置而变（守卫 BS-B1 逐点锁定）。</para>
/// <para><b>本域<b>无</b> <c>need_collection</c></b>：服务商支付分域（P0-4）查询应答有该字段且官方注明固定返回
/// <c>true</c>，本页字段表<b>没有</b>该字段 ⇒ 两域查询应答不可共用类型。</para>
/// <para><b><c>openid</c> 与 <c>sub_openid</c> 按是否传 <c>sub_appid</c> 分别返回</b>：官方原文
/// 「不传 sub_appid 的情况下返回 <c>openid</c>」「传了 sub_appid 的情况下返回 <c>sub_openid</c>」。</para>
/// <para><b><c>collection</c> 仅在调完结后返回</b>，且完结时 <c>total_amount</c> 为 0 元时官方<b>不返回</b>该节。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreQueryOrderResponse : WechatPayResponse
{
    /// <summary>商户服务订单号（<c>out_order_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>从业机构 AppID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>从业机构商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户 AppID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，<b>选填</b> string(32)）：请求侧同名字段为必填，本应答为选填。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>渠道商商户号（<c>channel_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填 string(32)）：见 <see cref="BankPayScoreServiceOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填 string(32)）：仅 DOING 返回，见 <see cref="BankPayScoreStateDescriptions"/>。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<BankPayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<BankPayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）。</summary>
    [JsonPropertyName("risk_fund")]
    public BankPayScoreRiskFund? RiskFund { get; set; }

    /// <summary>订单最终收款总金额（<c>total_amount</c>，选填整型，单位分）：由完结或修改订单接口传入。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>收款信息（<c>collection</c>，选填），见 <see cref="BankPayScoreCollection"/>；仅完结后返回，完结金额为 0 时不返回。</summary>
    [JsonPropertyName("collection")]
    public BankPayScoreCollection? Collection { get; set; }

    /// <summary>优惠信息（<c>promotion_detail</c>，选填）：官方本页字段表把它列在<b>顶层</b>（订单支付用到代金券时才返回）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<BankPayScorePromotionDetail>? PromotionDetail { get; set; }

    /// <summary>服务时间（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public BankPayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public BankPayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(256)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>用户在从业机构 <c>appid</c> 下的标识（<c>openid</c>，选填 string(128)）：官方注明「不传 <c>sub_appid</c> 时返回」。</summary>
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
/// 官方：<b>仅在调用完结支付分订单后返回</b>；若完结时 <c>total_amount</c> 为 0 元则<b>不返回本节</b>。
/// <c>state</c> 须结合订单 <c>state</c> 与 <c>state_description</c> 一起判断（官方流转图）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreCollection
{
    /// <summary>收款状态（<c>state</c>，必填 string(32)）：见 <see cref="BankPayScoreCollectionStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总收款金额（<c>total_amount</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>待收金额（<c>paying_amount</c>，选填整型，单位分）：完结成功后、收款成功前等于总额，收款成功后为 0。</summary>
    [JsonPropertyName("paying_amount")]
    public long? PayingAmount { get; set; }

    /// <summary>已收金额（<c>paid_amount</c>，选填整型，单位分）：完结成功后收款成功前为 0，收款成功后等于总额。</summary>
    [JsonPropertyName("paid_amount")]
    public long? PaidAmount { get; set; }

    /// <summary>收款明细（<c>details</c>，选填），见 <see cref="BankPayScoreCollectionDetail"/>。</summary>
    [JsonPropertyName("details")]
    public List<BankPayScoreCollectionDetail>? Details { get; set; }
}

/// <summary>收款明细（<c>collection.details[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreCollectionDetail
{
    /// <summary>收款序号（<c>seq</c>，选填整型）：从 1 开始递增。</summary>
    [JsonPropertyName("seq")]
    public long? Seq { get; set; }

    /// <summary>单笔收款金额（<c>amount</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>
    /// 收款渠道（<c>paid_type</c>，选填 string(32)）：本域取值 <c>NEWTON</c>（支付分渠道）/ <c>MCH</c>
    /// （用户走其它渠道付款、商户调同步状态接口成功后返回），见 <see cref="BankPayScoreCollectionPaidTypes"/>
    /// —— <b>与直连支付分域的 <c>ADVANCE</c>/<c>BALANCE</c> 取值集不同</b>。
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
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScorePromotionDetail
{
    /// <summary>券 ID（<c>coupon_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>优惠名称（<c>name</c>，选填 string(64)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠范围（<c>scope</c>，选填 string(32)）：<c>GLOBAL</c> 全场 / <c>SINGLE</c> 单品，见 <see cref="BankPayScorePromotionScopes"/>。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>优惠类型（<c>type</c>，选填 string(32)）：<c>CASH</c> 预充值 / <c>NOCASH</c> 免充值，见 <see cref="BankPayScorePromotionTypes"/>。</summary>
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

    /// <summary>单品列表（<c>goods_detail</c>，选填），见 <see cref="BankPayScorePromotionGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<BankPayScorePromotionGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>优惠的单品信息（<c>promotion_detail[].goods_detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScorePromotionGoodsDetail
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
/// 登记扣款信息应答（<b>POST …/serviceorder/{out_order_no}/registerdeductinfo</b> 的 200 报文）：
/// <b>只有</b> <c>description</c>。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458414"/>，2026-10-14 逐字段核验。</para>
/// <para>官方原文要求：调用清算机构（银联/网联）委托代扣接口的<b>商品描述字段须与本参数一致</b>，
/// 故本应答是「回显 + 供后续代扣对齐」的凭据，不得改写。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreRegisterDeductResponse : WechatPayResponse
{
    /// <summary>商品描述（<c>description</c>，必填 string(128)）：须与清算机构委托代扣接口的商品描述一致。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 查询扣款信息应答（<b>GET /v3/payscore/acquiringbank/serviceorder/deduction</b> 的 200 报文）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012458665"/>，2026-10-14 逐字段核验。</para>
/// <para>两字段均<b>必填</b>，且官方同样要求 <c>description</c> 与 <c>out_trade_no</c>
/// 与清算机构委托代扣接口的商品描述、商户订单号<b>逐字一致</b>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "BankPayScore")]
public class BankPayScoreDeductionQueryResponse : WechatPayResponse
{
    /// <summary>商品描述（<c>description</c>，必填 string(128)）：须与清算机构委托代扣接口的商品描述一致。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>商户扣款单号（<c>out_trade_no</c>，必填 string(32)）：须与清算机构委托代扣接口的商户订单号一致。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }
}
