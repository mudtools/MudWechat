// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.PayScore;

/// <summary>
/// 创建支付分订单（<c>POST /v3/payscore/serviceorder</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587900"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.06；支持商户：<b>普通商户</b>）。
/// </para>
/// <para>
/// <b>官方要点</b>：① <b>支持原参重入</b> —— 相同参数重复调用可返回成功（商户侧仍应以
/// <c>out_order_no</c> 做幂等）；② 创单成功后 <c>state = CREATED</c>；
/// ③ <c>out_order_no</c> <b>不可</b>用作申请退款接口的 <c>out_trade_no</c>；
/// ④ <b>完结订单与取消订单须与创单传入的 <c>appid</c> 保持一致</b>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreServiceOrderRequest
{
    /// <summary>商户服务订单号（<c>out_order_no</c>，必填 string(32)）：<b>业务幂等键</b>。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）：须与完结 / 取消时一致。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)，32 位数字）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填，<b>最多 100 条</b>），见 <see cref="PayScorePostPayment"/>。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填，<b>最多 30 条</b>），见 <see cref="PayScorePostDiscount"/>。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，<b>必填</b>），见 <see cref="PayScoreTimeRange"/>。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填），见 <see cref="PayScoreLocation"/>。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，<b>必填</b>），见 <see cref="PayScoreRiskFund"/>。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreRiskFund? RiskFund { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(256)）：官方注明<b>需 urlencode</b>，回调原样回传。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，必填 string(255)）：<b>确认订单回调</b>与<b>支付成功回调</b>共用。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>
    /// 是否需要用户确认（<c>need_user_confirm</c>，选填 bool）：官方原文「<b>固定 true 或不传，默认 true</b>」。
    /// </summary>
    [JsonPropertyName("need_user_confirm")]
    public bool? NeedUserConfirm { get; set; }

    /// <summary>设备信息（<c>device</c>，选填），见 <see cref="PayScoreDevice"/>。</summary>
    [JsonPropertyName("device")]
    public PayScoreDevice? Device { get; set; }
}

/// <summary>后付费项目（<c>post_payments[]</c>；创建请求与应答共用同一字段集）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScorePostPayment
{
    /// <summary>付费名称（<c>name</c>，必填 string(20)）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>付费金额（<c>amount</c>，选填整型，单位分，<b>≥0</b>；<c>0</c> 表示不扣费）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>付费说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>付费数量（<c>count</c>，选填整型，范围 <b>[1,100]</b>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

/// <summary>
/// 商户优惠（<c>post_discounts[]</c>）。
/// </summary>
/// <remarks>
/// <b>⚠️ 官方字段表自相矛盾（照录 + 超集承载）</b>：<b>请求</b>参数表本子对象<b>只列</b>
/// <c>name</c> / <c>description</c> / <c>count</c>，而<b>应答</b>参数表与官方示例代码中还出现
/// <c>amount</c>（优惠金额）。本模型取<b>超集</b>（含 <c>amount</c>）以免截断官方实际回传的字段 ——
/// 这是本仓对「官方自相矛盾」的既定处置（照录两处、不擅自择一）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScorePostDiscount
{
    /// <summary>优惠名称（<c>name</c>，必填 string(20)）：官方注明<b>同一单多个优惠项目名称不可重复</b>。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠说明（<c>description</c>，必填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>优惠金额（<c>amount</c>，整型，单位分）：官方<b>请求</b>表未列、<b>应答</b>表与示例有 ⇒ 超集保留。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>优惠数量（<c>count</c>，选填整型，范围 <b>[1,100]</b>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }
}

/// <summary>服务时间段（<c>time_range</c>；创建请求必填、应答选填）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreTimeRange
{
    /// <summary>
    /// 服务开始时间（<c>start_time</c>，string(14)）：支持 <c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c> /
    /// <b><c>OnAccept</c></b>（用户确认订单时才开始计时，见 <see cref="PayScoreStartTimeKeywords.OnAccept"/>）。
    /// </summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>服务结束时间（<c>end_time</c>，选填 string(14)）：支持 <c>yyyyMMddHHmmss</c> / <c>yyyyMMdd</c>。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>服务开始时间备注（<c>start_time_remark</c>，选填 string(20)）。</summary>
    [JsonPropertyName("start_time_remark")]
    public string? StartTimeRemark { get; set; }

    /// <summary>服务结束时间备注（<c>end_time_remark</c>，选填 string(20)）。</summary>
    [JsonPropertyName("end_time_remark")]
    public string? EndTimeRemark { get; set; }
}

/// <summary>服务位置（<c>location</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreLocation
{
    /// <summary>服务开始地点（<c>start_location</c>，选填 string(20)）。</summary>
    [JsonPropertyName("start_location")]
    public string? StartLocation { get; set; }

    /// <summary>服务结束地点（<c>end_location</c>，选填 string(20)）。</summary>
    [JsonPropertyName("end_location")]
    public string? EndLocation { get; set; }
}

/// <summary>服务风险金（<c>risk_fund</c>）。</summary>
/// <remarks>官方<b>创建请求</b>把外层标为必填、子字段 <c>name</c>/<c>amount</c> 亦为必填；<b>应答</b>把外层标为选填而子字段仍标必填（照录，不「修平」）。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreRiskFund
{
    /// <summary>风险名称（<c>name</c>，必填 string(30)）：取值见 <see cref="PayScoreRiskFundNames"/>。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>风险金额（<c>amount</c>，必填整型，单位分，官方注明<b>必须 &gt; 0</b>）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>风险说明（<c>description</c>，选填 string(30)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>设备信息（<c>device</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreDevice
{
    /// <summary>服务开始的设备 ID（<c>start_device_id</c>，选填 string(50)）：官方注明<b>无人自助设备行业必传</b>。</summary>
    [JsonPropertyName("start_device_id")]
    public string? StartDeviceId { get; set; }

    /// <summary>服务结束的设备 ID（<c>end_device_id</c>，选填 string(50)）：官方注明<b>无人自助设备行业必传</b>。</summary>
    [JsonPropertyName("end_device_id")]
    public string? EndDeviceId { get; set; }

    /// <summary>物料 URL（<c>materiel_no</c>，选填 string(100)）。</summary>
    [JsonPropertyName("materiel_no")]
    public string? MaterielNo { get; set; }
}

/// <summary>
/// 创建支付分订单应答。
/// </summary>
/// <remarks>
/// <b>与「查询支付分订单」应答<b>不是</b>同一形态</b>（官方两页字段表不同）：本应答含
/// <c>package</c>（拉起支付分小程序确认订单页的凭据）但<b>无</b> <c>total_amount</c> / <c>collection</c> /
/// <c>promotion_detail</c> / <c>openid</c>；查询应答反之。故<b>不</b>合并为同一类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreServiceOrderResponse : WechatPayResponse
{
    /// <summary>公众账号 ID（<c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户服务订单号（<c>out_order_no</c>，必填）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填）：见 <see cref="PayScoreServiceOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填）：官方注明<b>仅 DOING 状态返回</b>，见 <see cref="PayScoreStateDescriptions"/>。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>后付费商户优惠（<c>post_discounts</c>，选填，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填 string(256)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填 string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，必填 string(64)，31 位数字）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 跳转微信侧小程序订单数据（<c>package</c>，必填 string(300)）：用于拉起<b>支付分小程序确认订单页</b>。
    /// </summary>
    [JsonPropertyName("package")]
    public string? Package { get; set; }
}

/// <summary>
/// 查询支付分订单应答。
/// </summary>
/// <remarks>
/// <b>官方要点（原文）</b>：创建成功后可用本接口查订单状态，并<b>参考「支付分订单状态流转图」</b>做业务逻辑处理
/// —— 即状态机须按官方流转图实现，不得凭直觉跳转。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreServiceOrderQueryResponse : WechatPayResponse
{
    /// <summary>商户服务订单号（<c>out_order_no</c>，必填）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填）：见 <see cref="PayScoreServiceOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填 string(32)）。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，选填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>后付费商户优惠（<c>post_discounts</c>，选填）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreRiskFund? RiskFund { get; set; }

    /// <summary>总金额（<c>total_amount</c>，选填整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>是否需要收款（<c>need_collection</c>，选填 bool）：官方原文「<c>true</c> 为需收款，<c>false</c> 为无需收款」。</summary>
    [JsonPropertyName("need_collection")]
    public bool? NeedCollection { get; set; }

    /// <summary>收款信息（<c>collection</c>，选填），见 <see cref="PayScoreCollection"/>。</summary>
    [JsonPropertyName("collection")]
    public PayScoreCollection? Collection { get; set; }

    /// <summary>优惠信息（<c>promotion_detail</c>，选填），见 <see cref="PayScorePromotionDetail"/>。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayScorePromotionDetail>? PromotionDetail { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，必填 string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）：用户在商户 <c>appid</c> 下的唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，选填 string(64)）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>收款信息（<c>collection</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCollection
{
    /// <summary>收款状态（<c>state</c>）：<c>USER_PAYING</c> / <c>USER_PAID</c>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总收款金额（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>待收金额（<c>paying_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("paying_amount")]
    public long? PayingAmount { get; set; }

    /// <summary>已收金额（<c>paid_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("paid_amount")]
    public long? PaidAmount { get; set; }

    /// <summary>收款明细（<c>details</c>），见 <see cref="PayScoreCollectionDetail"/>。</summary>
    [JsonPropertyName("details")]
    public List<PayScoreCollectionDetail>? Details { get; set; }
}

/// <summary>收款明细（<c>collection.details[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCollectionDetail
{
    /// <summary>序号（<c>seq</c>，整型）。</summary>
    [JsonPropertyName("seq")]
    public long? Seq { get; set; }

    /// <summary>收款金额（<c>amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>收款类型（<c>paid_type</c>）：<c>NEWTON</c>（新收款）/ <c>ADVANCE</c>（预授权）/ <c>BALANCE</c>（余额）。</summary>
    [JsonPropertyName("paid_type")]
    public string? PaidType { get; set; }

    /// <summary>收款时间（<c>paid_time</c>，string，rfc3339）。</summary>
    [JsonPropertyName("paid_time")]
    public string? PaidTime { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}

/// <summary>优惠信息（<c>promotion_detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScorePromotionDetail
{
    /// <summary>券 ID（<c>coupon_id</c>，string）。</summary>
    [JsonPropertyName("coupon_id")]
    public string? CouponId { get; set; }

    /// <summary>优惠名称（<c>name</c>，string）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>优惠范围（<c>scope</c>）：<c>GLOBAL</c>（全场）/ <c>SINGLE</c>（单品）。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>优惠类型（<c>type</c>）：<c>CASH</c>（代金券）/ <c>DISCOUNT</c>（折扣券）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>优惠券面额（<c>amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>活动 ID（<c>stock_id</c>，string(32)）。</summary>
    [JsonPropertyName("stock_id")]
    public string? StockId { get; set; }

    /// <summary>微信出资（<c>wechatpay_contribute</c>，整型，单位分）。</summary>
    [JsonPropertyName("wechatpay_contribute")]
    public long? WechatpayContribute { get; set; }

    /// <summary>商户出资（<c>merchant_contribute</c>，整型，单位分）。</summary>
    [JsonPropertyName("merchant_contribute")]
    public long? MerchantContribute { get; set; }

    /// <summary>其他出资（<c>other_contribute</c>，整型，单位分）。</summary>
    [JsonPropertyName("other_contribute")]
    public long? OtherContribute { get; set; }

    /// <summary>优惠币种（<c>currency</c>，string(16)），固定 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>单品列表（<c>goods_detail</c>），见 <see cref="PayScorePromotionGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<PayScorePromotionGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>优惠的单品信息（<c>promotion_detail[].goods_detail[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScorePromotionGoodsDetail
{
    /// <summary>商品编码（<c>goods_id</c>，string(32)）。</summary>
    [JsonPropertyName("goods_id")]
    public string? GoodsId { get; set; }

    /// <summary>商品数量（<c>quantity</c>，整型）。</summary>
    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    /// <summary>商品单价（<c>unit_price</c>，整型，单位分）。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>商品优惠金额（<c>discount_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("discount_amount")]
    public long? DiscountAmount { get; set; }

    /// <summary>商品备注（<c>goods_remark</c>，string(128)）。</summary>
    [JsonPropertyName("goods_remark")]
    public string? GoodsRemark { get; set; }
}

/// <summary>
/// 取消支付分订单（<c>POST /v3/payscore/serviceorder/{out_order_no}/cancel</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_16.shtml"/>
/// （2026-10-09 逐字段核验；更新时间 2025.02.13；支持商户：普通商户）。
/// </para>
/// <para>
/// <b>⚠️ <c>service_id</c> 在本接口是选填</b>（创单接口里它是必填）—— 官方原样，勿「统一」。
/// <c>appid</c> 则<b>必须</b>与创单一致。
/// </para>
/// <para>
/// <b>可取消的状态（官方开发指引）</b>：<c>CREATED</c>（已创单）、<c>DOING</c>（进行中，
/// 含商户已完结但收款状态为待支付 <c>USER_PAYING</c> 的情形）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCancelOrderRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）：<b>须与创单传入的 appid 一致</b>。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，<b>选填</b> string(32)）：32 位数字。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>
    /// 撤销原因（<c>reason</c>，必填 string(50)，不超过 50 字符）：
    /// 官方注明<b>用户微信收到的取消订单消息通知会展示该字段</b> ⇒ 属用户可见文案。
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// 取消支付分订单应答（<b>仅</b> 5 个顶层字段，无嵌套）。
/// </summary>
/// <remarks>
/// <b>为何不复用创建/查询应答</b>：官方本页应答字段表只有 <c>appid</c> / <c>mchid</c> /
/// <c>out_order_no</c> / <c>service_id</c> / <c>order_id</c> —— 与创建应答（含 <c>package</c> 等）
/// 和查询应答（含 <c>collection</c> 等）<b>都不同</b>。三者各按官方表精确建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCancelOrderResponse : WechatPayResponse
{
    /// <summary>公众账号 ID（<c>appid</c>，必填）：与创单时传入的一致。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户订单号（<c>out_order_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，必填 string(128)，31 位数字，开头为 <c>1000000000+年月日</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 完结支付分订单（<c>POST /v3/payscore/serviceorder/{out_order_no}/complete</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587955"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.06；支持商户：普通商户；官方页面 <c>chapter6_1_18</c>）。
/// </para>
/// <para>
/// <b>业务语义</b>：完结即<b>确认最终应收金额</b>（<c>total_amount</c>）并提交最终的后付费项目明细；
/// 完结后应答的 <c>need_collection</c> <b>固定返回 <c>true</c></b>（官方原文）⇒ 表示进入待收款环节，
/// 后续须经「发起催收扣款」或用户自动扣款完成收款。
/// </para>
/// <para><b>appid 一致性</b>：官方再次强调「完结/取消订单需与创单传入的 <c>appid</c> 一致」。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCompleteOrderRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）：<b>须与创单一致</b>。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)，32 位数字）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>
    /// 后付费项目（<c>post_payments</c>，<b>必填</b>，最多 100 条）：
    /// 与创单相比本接口<b>必填</b>（创单为选填）—— 完结必须给出最终的付费明细。
    /// </summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填，最多 30 条）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>
    /// 订单最终收款总金额（<c>total_amount</c>，<b>必填</b> integer，单位分）：
    /// 官方注明<b>受服务 ID 风险金额上限影响</b>（超出会被风控拒绝）。
    /// </summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>实际服务时间段（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>
    /// 实际服务位置（<c>location</c>，选填）。
    /// </summary>
    /// <remarks>
    /// <b>⚠️ 官方本页字段表只列 <c>end_location</c>，而官方请求示例里出现 <c>start_location</c></b>
    /// —— 属官方自相矛盾，本模型沿用 <see cref="PayScoreLocation"/>（<c>start_location</c> + <c>end_location</c>）
    /// 作为<b>超集</b>承载，不擅自删除示例中确实存在的字段。
    /// </remarks>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>
    /// 分账标记（<c>profit_sharing</c>，选填 bool）：<c>true</c> 需分账 / <c>false</c> 不需分账，
    /// 官方原文<b>不传默认 <c>false</c></b>。置 <c>true</c> 后该笔订单方可走分账域。
    /// </summary>
    [JsonPropertyName("profit_sharing")]
    public bool? ProfitSharing { get; set; }

    /// <summary>订单优惠标记（<c>goods_tag</c>，选填 string(32)）。</summary>
    [JsonPropertyName("goods_tag")]
    public string? GoodsTag { get; set; }

    /// <summary>设备信息（<c>device</c>，选填）。</summary>
    [JsonPropertyName("device")]
    public PayScoreDevice? Device { get; set; }
}

/// <summary>
/// 完结支付分订单应答。
/// </summary>
/// <remarks>
/// <b>与查询应答的关系（刻意不合并）</b>：本应答是查询应答的<b>真子集</b>（同名字段类型一致），
/// 但官方两页字段表<b>不同</b> —— 本应答<b>无</b> <c>collection</c> / <c>promotion_detail</c> /
/// <c>attach</c> / <c>notify_url</c> / <c>openid</c>。合并会让调用方以为这些字段在本接口也会返回
/// （永不返回的字段是静默误导），故按官方表精确建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCompleteOrderResponse : WechatPayResponse
{
    /// <summary>公众账号 ID（<c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户服务订单号（<c>out_order_no</c>，必填）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填 string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填）：见 <see cref="PayScoreServiceOrderStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填，仅 <c>DOING</c> 返回）。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>商户收款总金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>后付费项目明细（<c>post_payments</c>，选填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>后付费商户优惠（<c>post_discounts</c>，选填）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填；官方本表只列 <c>end_location</c>）。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，选填 string(64)，31 位数字）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 是否需要收款（<c>need_collection</c>，选填 bool）：官方原文<b>固定返回 <c>true</c></b>。
    /// </summary>
    [JsonPropertyName("need_collection")]
    public bool? NeedCollection { get; set; }
}

/// <summary>
/// 修改订单金额（<c>POST /v3/payscore/serviceorder/{out_order_no}/modify</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_17.shtml"/>
/// （2026-10-09 逐字段核验；更新时间 2025.03.06；支持商户：普通商户）。
/// </para>
/// <para>
/// <b>与创单的两处差异（官方原样）</b>：① <c>post_payments</c> 在本接口是<b>必填</b>（创单为选填）；
/// ② <c>post_discounts[].name</c> 在本接口是<b>选填</b>（创单为必填）—— 必填性不随 DTO 表达，
/// 故在此留档以免调用方按创单的约束照搬。
/// </para>
/// <para><b><c>reason</c> 必填</b>且官方未标注长度以外的语义约束（string(50)）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreModifyOrderRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，<b>必填</b>；子字段 <c>name</c> 必填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>商户优惠（<c>post_discounts</c>，选填；本接口子字段 <c>name</c> 为<b>选填</b>）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>订单最终收款总金额（<c>total_amount</c>，必填 integer，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>修改原因（<c>reason</c>，必填 string(50)）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>设备信息（<c>device</c>，选填）。</summary>
    [JsonPropertyName("device")]
    public PayScoreDevice? Device { get; set; }
}

/// <summary>
/// 修改订单金额应答的收款信息（<c>collection</c>）—— <b>结构与查询应答的 <c>collection</c> 不同</b>。
/// </summary>
/// <remarks>
/// <b>⚠️ 官方两页的 <c>collection</c> 结构不一致（照录，不「统一」）</b>：
/// 本（修改）页把 <c>promotion_detail</c> 与 <c>goods_detail</c> <b>嵌在 collection 之下</b>；
/// 而查询页把 <c>promotion_detail</c> 放在<b>顶层</b>。故二者是两个类型，
/// 不得为「省一个类」把它们并成一个 —— 那会让两个接口各自带上对方才有的嵌套。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreModifyCollection
{
    /// <summary>收款状态（<c>state</c>）：见 <see cref="PayScoreCollectionStates"/>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总收款金额（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>待收金额（<c>paying_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("paying_amount")]
    public long? PayingAmount { get; set; }

    /// <summary>已收金额（<c>paid_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("paid_amount")]
    public long? PaidAmount { get; set; }

    /// <summary>收款明细（<c>details</c>）。</summary>
    [JsonPropertyName("details")]
    public List<PayScoreCollectionDetail>? Details { get; set; }

    /// <summary>优惠信息（<c>promotion_detail</c>，<b>嵌在 collection 下</b>）。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayScorePromotionDetail>? PromotionDetail { get; set; }

    /// <summary>优惠单品（<c>goods_detail</c>，<b>与 promotion_detail 平级嵌在 collection 下</b>）。</summary>
    [JsonPropertyName("goods_detail")]
    public List<PayScorePromotionGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>
/// 修改订单金额应答（顶层 18 字段）。
/// </summary>
/// <remarks>
/// <b>与查询应答的关系</b>：顶层字段集与查询应答接近（本应答<b>无</b> <c>openid</c>），
/// 但 <c>collection</c> 的<b>内部结构不同</b>（见 <see cref="PayScoreModifyCollection"/>）
/// ⇒ 官方两页字段表不同即两个类型，不合并。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreModifyOrderResponse : WechatPayResponse
{
    /// <summary>商户服务订单号（<c>out_order_no</c>，必填）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>公众账号 ID（<c>appid</c>，必填）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，必填）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，必填）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>服务订单状态（<c>state</c>，必填）。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>，选填）。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>，必填）。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>后付费商户优惠（<c>post_discounts</c>，选填）。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>，选填）。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreRiskFund? RiskFund { get; set; }

    /// <summary>订单最终收款总金额（<c>total_amount</c>，选填，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>是否需要收款（<c>need_collection</c>，选填 bool）。</summary>
    [JsonPropertyName("need_collection")]
    public bool? NeedCollection { get; set; }

    /// <summary>收款信息（<c>collection</c>，选填；<b>本页嵌套结构见 <see cref="PayScoreModifyCollection"/></b>）。</summary>
    [JsonPropertyName("collection")]
    public PayScoreModifyCollection? Collection { get; set; }

    /// <summary>服务时间段（<c>time_range</c>，选填）。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>，选填）。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，选填）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，选填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

/// <summary>
/// 发起催收扣款（<c>POST /v3/payscore/serviceorder/{out_order_no}/pay</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter6_1_19.shtml"/>
/// （2026-10-09 逐字段核验；更新时间 2025.01.02；支持商户：普通商户）。
/// </para>
/// <para>
/// <b>⚠️ 官方接口名是「发起催收扣款」，不是「收款」</b> —— 即这是一次<b>主动催收</b>动作
/// （对已完结、待收款 <c>USER_PAYING</c> 的订单发起扣款），勿按「收款」的语义理解成对账或入账。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCollectRequest
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }
}

/// <summary>
/// 发起催收扣款应答（<b>仅 4 个顶层字段</b>，无嵌套）。
/// </summary>
/// <remarks>
/// 官方本页应答字段表为 <c>appid</c> / <c>out_order_no</c> / <c>service_id</c> / <c>order_id</c> ——
/// <b>无 <c>mchid</c></b>，故与取消应答（5 字段含 <c>mchid</c>）也不是同一形态，不合并建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayScore")]
public class PayScoreCollectResponse : WechatPayResponse
{
    /// <summary>公众账号 ID（<c>appid</c>，必填 string）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户订单号（<c>out_order_no</c>，必填 string）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，必填 string(128)）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}
