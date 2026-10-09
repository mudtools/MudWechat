// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Combine;
using Mud.Wechat.Pay.DataModels.PayScore;

namespace Mud.Wechat.Pay.DataModels.Callback;

/// <summary>
/// 解密后的<b>交易</b>资源载荷（<c>TRANSACTION.*</c> 通知；<c>resource.ciphertext</c> 解密所得的 JSON 明文）。
/// </summary>
/// <remarks>
/// <para>
/// 字段与「查询订单」应答<b>同源</b>（同一交易对象），但<b>有意不复用</b> <c>TransactionQueryResponse</c>：
/// 后者带 <c>code</c>/<c>message</c> 错误信封（用于 4xx 判错），而通知载荷恒为成功数据、无错误信封；
/// 强行复用一个类型会让「通知载荷」误带判错语义（并把回调包绑到查询应答的演进上）。
/// </para>
/// <para><b>字段名照官方原文</b>；<c>trade_state</c> 为 <c>SUCCESS</c> 才是支付成功（须显式判定，勿假定回调即成功）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayTransactionResource
{
    /// <summary>公众账号 ID（<c>appid</c>，string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>，string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，string(32)）；<b>业务幂等键</b>。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>交易类型（<c>trade_type</c>，string(16)）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>交易状态（<c>trade_state</c>，string(32)）；<c>SUCCESS</c> 为支付成功。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>交易状态描述（<c>trade_state_desc</c>，string(256)）。</summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>付款银行（<c>bank_type</c>，string(32)）。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>商户数据包（<c>attach</c>，string(128)），下单时上送、原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，string(64)，rfc3339）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>支付者信息（<c>payer</c>），见 <see cref="WechatPayTransactionPayer"/>。</summary>
    [JsonPropertyName("payer")]
    public WechatPayTransactionPayer? Payer { get; set; }

    /// <summary>订单金额（<c>amount</c>），见 <see cref="WechatPayTransactionAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public WechatPayTransactionAmount? Amount { get; set; }
}

/// <summary>交易载荷的支付者（<c>payer</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayTransactionPayer
{
    /// <summary>用户在商户 <c>appid</c> 下的唯一标识（<c>openid</c>，string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>交易载荷的金额（<c>amount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayTransactionAmount
{
    /// <summary>订单总金额（<c>total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>用户支付金额（<c>payer_total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>货币类型（<c>currency</c>，string(16)），固定 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>用户支付币种（<c>payer_currency</c>，string(16)）。</summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }
}

/// <summary>
/// 解密后的<b>退款</b>资源载荷（<c>REFUND.*</c> 通知；<c>resource.ciphertext</c> 解密所得的 JSON 明文）。
/// </summary>
/// <remarks>字段名照官方原文；<c>refund_status</c> 为 <c>SUCCESS</c> 才是退款成功。</remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayRefundResource
{
    /// <summary>商户号（<c>mchid</c>，string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，string(64)）；<b>业务幂等键</b>。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>微信支付退款单号（<c>refund_id</c>，string(32)）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>退款状态（<c>refund_status</c>，string(32)）：<c>SUCCESS</c> / <c>CLOSED</c> / <c>ABNORMAL</c>。</summary>
    [JsonPropertyName("refund_status")]
    public string? RefundStatus { get; set; }

    /// <summary>退款成功时间（<c>success_time</c>，string(64)，rfc3339）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>退款入账账户（<c>user_received_account</c>，string(64)）。</summary>
    [JsonPropertyName("user_received_account")]
    public string? UserReceivedAccount { get; set; }

    /// <summary>退款金额信息（<c>amount</c>），见 <see cref="WechatPayRefundAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public WechatPayRefundAmount? Amount { get; set; }
}

/// <summary>退款载荷的金额（<c>amount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayRefundAmount
{
    /// <summary>订单总金额（<c>total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>退款金额（<c>refund</c>，整型，单位为分）。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>用户支付金额（<c>payer_total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>用户退款金额（<c>payer_refund</c>，整型，单位为分）。</summary>
    [JsonPropertyName("payer_refund")]
    public long? PayerRefund { get; set; }
}

/// <summary>
/// 解密后的<b>分账</b>资源载荷（分账动态通知；<c>resource.ciphertext</c> 解密所得的 JSON 明文）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter8_1_10.shtml"/>
/// （分账动态通知，2026-10-09 逐字段核验；更新时间 2025.02.19）。
/// </para>
/// <para>
/// <b>⚠️ 本通知不能靠 <c>event_type</c> 区分</b>：官方本页把「分账」与「分账回退」两种通知
/// <b>都</b>标为 <c>event_type = TRANSACTION.SUCCESS</c> —— 与<b>支付成功通知同值</b>。
/// 唯一可靠的区分字段是 <c>resource.original_type = profitsharing</c>
/// （官方原文：「加密前的对象类型，分账动账通知的类型为 profitsharing」）。
/// 因此消费侧<b>必须</b>先看 <c>OriginalType</c>，再决定按本类型还是按
/// <see cref="WechatPayTransactionResource"/> 解析 —— 只看 <c>event_type</c> 会把分账通知错解析成交易载荷
/// （字段大面积为空但<b>不报错</b>，最危险的静默形态）。
/// </para>
/// <para>
/// <b>⚠️ 官方文档缺口（照录 + 显式标注）</b>：本页同时列出「分账」与「分账回退」两种通知，
/// 但<b>只给出一张</b>解密字段表（即下表）。回退通知是否另带 <c>out_return_no</c> / <c>return_mchid</c> 等字段
/// 官方未列 ⇒ 本模型照录已列字段，<b>不臆造</b>；若收到回退通知，可用
/// <c>WechatPayCallbackContext.ResourceJson</c> 取明文兜底。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayProfitSharingResource
{
    /// <summary>商户号（<c>mchid</c>，string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>微信分账单号（<c>order_id</c>，string(64)）：微信系统返回的唯一标识。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>商户分账单号（<c>out_order_no</c>，string(64)）；<b>业务幂等键</b>（与请求分账同一单号）。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>本次成功分账的接收方（<c>receiver</c>，对象），见 <see cref="WechatPayProfitSharingReceiver"/>。</summary>
    [JsonPropertyName("receiver")]
    public WechatPayProfitSharingReceiver? Receiver { get; set; }

    /// <summary>分账成功时间（<c>success_time</c>，string(64)，rfc3339）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }
}

/// <summary>分账载荷的接收方（<c>receiver</c>）。</summary>
/// <remarks>官方按<b>单数对象</b>给出（每个接收方一条通知），<b>不是</b> <c>receivers[]</c> 数组 —— 勿按下单侧形态套用。</remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayProfitSharingReceiver
{
    /// <summary>接收方类型（<c>type</c>，string）：<c>MERCHANT_ID</c> / <c>PERSONAL_OPENID</c>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>接收方账号（<c>account</c>，string(64)）。</summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>分账金额（<c>amount</c>，整型，单位为分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>分账描述（<c>description</c>，string(80)）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

/// <summary>
/// 解密后的<b>支付分订单支付成功</b>资源载荷（<c>PAYSCORE.USER_PAID</c> 通知；解密所得的 JSON 明文）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587960"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.15）。官方原文：「支付分订单支付成功通知为
/// <c>PAYSCORE.USER_PAID</c>」。
/// </para>
/// <para>
/// <b>⚠️ 与「查询支付分订单」应答<b>结构不同</b>（第三处形态差异）</b>：本载荷<b>没有顶层</b>
/// <c>promotion_detail</c>，而是把它嵌在 <c>collection.details[]</c> <b>每一项之下</b>。
/// 至此本域已出现三种 <c>promotion_detail</c> 位置：查询页（顶层）、修改页（collection 下）、
/// 本载荷（<b>collection.details[] 项下</b>）—— <b>三者都照官方原文建模，不「统一」</b>。
/// </para>
/// <para>
/// <b>复用既有子类型</b>：<c>post_payments</c> / <c>post_discounts</c> / <c>risk_fund</c> /
/// <c>time_range</c> / <c>location</c> 与支付分域字段表一致 ⇒ 直接复用 <c>Mud.Wechat.Pay.DataModels.PayScore</c>
/// 下的既有类型；仅容器（<c>collection</c> 与 <c>details[]</c>）因嵌套结构不同而另建。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayPayScorePaidResource
{
    /// <summary>公众账号 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户服务订单号（<c>out_order_no</c>）；<b>业务幂等键</b>。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>用户标识（<c>openid</c>，string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>服务订单状态（<c>state</c>）：见 <c>PayScoreServiceOrderStates</c>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>总金额（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>后付费商户优惠（<c>post_discounts</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间段（<c>time_range</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，string(256)）：下单时上送、原样回传。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，31 位数字）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>是否需要收款（<c>need_collection</c>，bool）。</summary>
    [JsonPropertyName("need_collection")]
    public bool? NeedCollection { get; set; }

    /// <summary>商户回调地址（<c>notify_url</c>，string(256)）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>收款信息（<c>collection</c>）：<b>本载荷的嵌套结构与查询应答不同</b>，见 <see cref="WechatPayPayScoreCollection"/>。</summary>
    [JsonPropertyName("collection")]
    public WechatPayPayScoreCollection? Collection { get; set; }
}

/// <summary>支付分支付成功载荷的收款信息（<c>collection</c>）。</summary>
/// <remarks>
/// 与查询应答的 <c>collection</c> 字段集相同，但本类型存在的原因是<b>其 <c>details[]</c> 项内嵌
/// <c>promotion_detail</c></b>（见 <see cref="WechatPayPayScoreCollectionDetail"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayPayScoreCollection
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

    /// <summary>收款明细（<c>details</c>），见 <see cref="WechatPayPayScoreCollectionDetail"/>。</summary>
    [JsonPropertyName("details")]
    public List<WechatPayPayScoreCollectionDetail>? Details { get; set; }
}

/// <summary>
/// 支付分支付成功载荷的收款明细项（<c>collection.details[]</c>）—— <b>内嵌 <c>promotion_detail</c></b>。
/// </summary>
/// <remarks>
/// 官方本载荷把 <c>promotion_detail</c> 放在<b>每个收款明细项之内</b>（查询页在顶层、修改页在
/// <c>collection</c> 之下）—— 三处位置各不相同，均为官方原样。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayPayScoreCollectionDetail
{
    /// <summary>序号（<c>seq</c>）。</summary>
    [JsonPropertyName("seq")]
    public long? Seq { get; set; }

    /// <summary>收款金额（<c>amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>收款类型（<c>paid_type</c>）：<c>NEWTON</c> / <c>ADVANCE</c> / <c>BALANCE</c>。</summary>
    [JsonPropertyName("paid_type")]
    public string? PaidType { get; set; }

    /// <summary>收款时间（<c>paid_time</c>）。</summary>
    [JsonPropertyName("paid_time")]
    public string? PaidTime { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 优惠信息（<c>promotion_detail</c>，<b>嵌在明细项之下</b>）：字段表与支付分域一致 ⇒ 复用
    /// <c>PayScorePromotionDetail</c>（其内已含 <c>goods_detail</c>）。
    /// </summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayScorePromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>
/// 解密后的<b>支付分订单确认成功</b>资源载荷（<c>PAYSCORE.USER_CONFIRM</c> 通知）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012587953"/>
/// （确认订单回调通知，2026-10-09 逐字段核验；更新时间 2024.12.10）。官方原文：
/// 「支付分订单确认成功通知为 <c>PAYSCORE.USER_CONFIRM</c>」。
/// </para>
/// <para>
/// <b>⚠️ 大小写的坑（已实测纠正）</b>：检索摘要里该值一度呈现为全小写
/// <c>payscore.user_confirm</c>，而<b>官方页面逐字为大写 <c>PAYSCORE.USER_CONFIRM</c></b> ——
/// <c>event_type</c> 是<b>大小写敏感</b>的字符串匹配键，按小写实现会让回调永远不命中（静默、无异常）。
/// 本仓取值一律以<b>页面原文</b>为准，不采信二手摘要。
/// </para>
/// <para>
/// <b>与支付成功载荷的形态差异</b>：本载荷<b>无</b> <c>notify_url</c>、<b>无</b> <c>collection</c>，
/// 但<b>有</b> <c>state_description</c>（支付成功载荷反之）⇒ 两个类型分建，不合并。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayPayScoreConfirmResource
{
    /// <summary>公众账号 ID（<c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>商户号（<c>mchid</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户服务订单号（<c>out_order_no</c>）；<b>业务幂等键</b>。</summary>
    [JsonPropertyName("out_order_no")]
    public string? OutOrderNo { get; set; }

    /// <summary>服务 ID（<c>service_id</c>）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>用户标识（<c>openid</c>，string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>服务订单状态（<c>state</c>）：用户确认后为 <c>DOING</c>。</summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>订单状态说明（<c>state_description</c>）：如 <c>USER_CONFIRM</c>。</summary>
    [JsonPropertyName("state_description")]
    public string? StateDescription { get; set; }

    /// <summary>总金额（<c>total_amount</c>，整型，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>服务信息（<c>service_introduction</c>，string(20)）。</summary>
    [JsonPropertyName("service_introduction")]
    public string? ServiceIntroduction { get; set; }

    /// <summary>后付费项目（<c>post_payments</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("post_payments")]
    public List<PayScorePostPayment>? PostPayments { get; set; }

    /// <summary>后付费商户优惠（<c>post_discounts</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("post_discounts")]
    public List<PayScorePostDiscount>? PostDiscounts { get; set; }

    /// <summary>服务风险金（<c>risk_fund</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("risk_fund")]
    public PayScoreRiskFund? RiskFund { get; set; }

    /// <summary>服务时间段（<c>time_range</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("time_range")]
    public PayScoreTimeRange? TimeRange { get; set; }

    /// <summary>服务位置（<c>location</c>）：复用支付分域类型。</summary>
    [JsonPropertyName("location")]
    public PayScoreLocation? Location { get; set; }

    /// <summary>商户数据包（<c>attach</c>，string(256)）。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>微信支付服务订单号（<c>order_id</c>，31 位数字）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>是否需要收款（<c>need_collection</c>，bool）。</summary>
    [JsonPropertyName("need_collection")]
    public bool? NeedCollection { get; set; }
}

/// <summary>
/// 解密后的<b>支付分开启 / 解除授权服务</b>资源载荷
/// （<c>PAYSCORE.USER_OPEN_SERVICE</c> / <c>PAYSCORE.USER_CLOSE_SERVICE</c> 通知）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012086446"/>
/// （开启/解除授权服务回调通知，2026-10-09 逐字段核验；更新时间 2025.04.22）。
/// </para>
/// <para>
/// <b>为何两类事件共用一个类型</b>：官方同一页给出两类通知的<b>同一张</b>字段表，
/// 差别只在 <c>user_service_status</c> / <c>openorclose_time</c> 的取值 ——
/// 字段集合<b>相同</b> ⇒ 共用（本仓纪律：表相同则共用）；分建两个类型只会让同一份事实两处漂移。
/// </para>
/// <para>
/// <b>⚠️ 面（face）</b>：该页是官方的<b>从业机构（支付机构）· 支付分免确认模式</b>页，
/// 含 <c>sub_appid</c> / <c>sub_mchid</c> / <c>channel_id</c> 等<b>从业机构侧</b>字段；
/// 普通商户侧本轮<b>未</b>检索到对应页 ⇒ 普通商户侧字段可能不全，未填字段解析为 <c>null</c> 属正常
/// （本类全部允许为空，<b>不</b>对必填性做运行期强制，以免把「从业机构面」的契约强加给普通商户）。
/// </para>
/// <para>
/// <b>该页未列出 <c>resource.original_type</c></b>（只有 <c>resource_type</c>）⇒
/// 本载荷的判别只能凭 <c>event_type</c>，<b>不得</b>假定 <c>original_type = payscore</c> 一定存在。
/// </para>
/// <para>
/// <b>官方字段说明的一处自相重复（照录，不「纠正」）</b>：<c>sub_mchid</c> 与 <c>channel_id</c>
/// 的中文说明<b>完全相同</b>（均为「【子商户号】」）—— 按其名与「渠道」语义，二者应不同，
/// 但页面原文如此，故本类注释按原文照录。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayPayScoreAuthorizationResource
{
    /// <summary>从业机构公众账号 ID（<c>appid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>服务商商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户公众账号 ID（<c>sub_appid</c>，选填 string(32)）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户号（<c>channel_id</c>，选填 string(32)）—— 官方中文说明与 <c>sub_mchid</c> 重复，照录。</summary>
    [JsonPropertyName("channel_id")]
    public string? ChannelId { get; set; }

    /// <summary>服务 ID（<c>service_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("service_id")]
    public string? ServiceId { get; set; }

    /// <summary>用户标识（<c>openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>用户标识（<c>sub_openid</c>，选填 string(128)）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>
    /// 回调状态（<c>user_service_status</c>，选填 string(32)）——
    /// <b>授权成功与解除授权成功的判别字段</b>。官方该页只写「【回调状态】」，
    /// <b>未列取值</b> ⇒ 本仓<b>不</b>臆造枚举常量（消费侧按官方页面/实际报文取值判断）。
    /// </summary>
    [JsonPropertyName("user_service_status")]
    public string? UserServiceStatus { get; set; }

    /// <summary>服务开启 / 解除授权时间（<c>openorclose_time</c>，选填 string(32)，rfc3339）。</summary>
    [JsonPropertyName("openorclose_time")]
    public string? OpenOrCloseTime { get; set; }

    /// <summary>授权协议号（<c>authorization_code</c>，选填 string(32)）：免确认模式下调用预授权/解冻等接口的钥匙。</summary>
    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }
}

/// <summary>
/// 解密后的<b>电子发票</b>资源载荷（<b>四类事件共用</b>）：<c>FAPIAO.ISSUED</c>（开具成功）、
/// <c>FAPIAO.CARD_INSERTED</c>（插卡成功）、<c>FAPIAO.REVERSED</c>（冲红成功）、
/// <c>FAPIAO.CARD_DISCARDED</c>（卡券作废）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（2026-10-09 逐字段核验，四页<b>字段表逐项一致</b>；更新时间 2025.09.26）：
/// <c>4012286057</c> 开具成功 / <c>4012286082</c> 插卡成功 /
/// <c>…/docs/merchant/apis/fapiao/fapiao-applications/invoice-flush-success-notice.html</c> 冲红成功 /
/// <c>…/docs/merchant/apis/fapiao/fapiao-card-template/invoice-card-cancel-notice.html</c> 卡券作废。
/// </para>
/// <para>
/// <b>为何四类共用一个类型</b>：四页的明文字段表<b>完全相同</b>
/// （<c>mchid</c> / <c>fapiao_apply_id</c> / <c>fapiao_information[]</c>），
/// 事件差别只体现在 <c>fapiao_status</c> / <c>card_status</c> 的<b>取值</b>上 ⇒
/// 按本仓纪律「表相同则共用」合建；分建四个会让同一份事实四处漂移。
/// </para>
/// <para>
/// <b>⚠️ 判别只能靠 <c>event_type</c></b>：这四页的 <c>resource</c> <b>都没有</b>
/// <c>original_type</c> 字段（只有 <c>algorithm</c> / <c>ciphertext</c> / <c>associated_data</c> / <c>nonce</c>）——
/// 与分账通知「必须靠 <c>original_type</c> 区分」的形态<b>正好相反</b>，勿套用。
/// </para>
/// <para>
/// <b>两组状态别混用</b>：<c>fapiao_status</c>（开票/冲红线）与 <c>card_status</c>（卡券线）
/// 是两套枚举，见 <c>FapiaoStatuses</c> / <c>FapiaoCardStatuses</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayFapiaoResource
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>
    /// 发票申请单号（<c>fapiao_apply_id</c>，必填 string(64)）：开票时指定的发票申请单号。
    /// </summary>
    [JsonPropertyName("fapiao_apply_id")]
    public string? FapiaoApplyId { get; set; }

    /// <summary>发票申请单下关联的所有发票信息（<c>fapiao_information</c>，必填 array），见 <see cref="WechatPayFapiaoInformation"/>。</summary>
    [JsonPropertyName("fapiao_information")]
    public List<WechatPayFapiaoInformation>? FapiaoInformation { get; set; }
}

/// <summary>
/// 单张电子发票的状态（<c>fapiao_information</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayFapiaoInformation
{
    /// <summary>商户发票单号（<c>fapiao_id</c>，必填 string(32)）：唯一标识一张发票。</summary>
    [JsonPropertyName("fapiao_id")]
    public string? FapiaoId { get; set; }

    /// <summary>发票状态（<c>fapiao_status</c>，必填）：取值见 <c>FapiaoStatuses</c>（开票 / 冲红维度）。</summary>
    [JsonPropertyName("fapiao_status")]
    public string? FapiaoStatus { get; set; }

    /// <summary>发票卡券状态（<c>card_status</c>，必填）：取值见 <c>FapiaoCardStatuses</c>（卡券维度，与上面那组<b>不是</b>同一套）。</summary>
    [JsonPropertyName("card_status")]
    public string? CardStatus { get; set; }
}

/// <summary>
/// 解密后的<b>用户发票抬头填写完成</b>资源载荷（<c>FAPIAO.USER_APPLIED</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012286009"/>
/// （2026-10-09 逐字段核验；更新时间 2025.09.26）。
/// </para>
/// <para>
/// <b>为何不与其余四类发票通知共用</b>：本载荷只有 <c>mchid</c> / <c>fapiao_apply_id</c> /
/// <c>apply_time</c> <b>三</b>个字段，<b>没有</b> <c>fapiao_information</c>
/// （此刻还没有任何发票可言）⇒ 表不同则分建。
/// </para>
/// <para>
/// <b>同样是「无 <c>original_type</c>」形态</b>：判别只能靠 <c>event_type</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayFapiaoUserAppliedResource
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>
    /// 发票申请单号（<c>fapiao_apply_id</c>，必填）：官方原文「唯一标识一次开票行为；
    /// <b>微信支付场景下为微信支付订单号</b>，非微信支付场景下为调用【获取抬头填写链接】时指定的发票申请单号」。
    /// </summary>
    [JsonPropertyName("fapiao_apply_id")]
    public string? FapiaoApplyId { get; set; }

    /// <summary>用户完成发票抬头填写的时间（<c>apply_time</c>，RFC3339 格式）。</summary>
    [JsonPropertyName("apply_time")]
    public string? ApplyTime { get; set; }
}

/// <summary>
/// 解密后的<b>合单支付成功</b>资源载荷（<c>event_type = TRANSACTION.SUCCESS</c>，<b>合单形态</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4013462574"/>
/// （合单订单支付成功回调通知，2026-10-09 逐字段核验；更新时间 2025.01.16）。
/// </para>
/// <para>
/// <b>🔴 本载荷最重要的一点：通知信封与「普通支付成功通知」<b>完全相同</b></b> ——
/// 二者的 <c>event_type</c> 都是 <c>TRANSACTION.SUCCESS</c>，
/// <c>resource.original_type</c> 也都是 <c>transaction</c>（官方两页各自逐字确认）。
/// 即：<b>无法凭信封区分合单与普通单</b>，唯一判据是<b>解密后的载荷形态</b> ——
/// 合单载荷有 <c>combine_appid</c> / <c>combine_mchid</c> / <c>combine_out_trade_no</c>
/// 与<b>复数</b>的 <c>sub_orders[]</c>；普通支付载荷是单数的
/// <c>appid</c> / <c>mchid</c> / <c>out_trade_no</c> / <c>amount</c>。
/// </para>
/// <para>
/// <b>⚠️ 误用访问器不会报错（典型静默错位）</b>：两个载荷的字段名<b>没有一个重叠</b>，
/// 所以拿 <c>GetTransaction()</c> 去解合单密文（或反之）<b>不会抛异常</b>，只会得到一份
/// 「字段全空」的对象 ⇒ 消费侧<b>必须</b>用 <see cref="CombineOutTradeNo"/> /
/// <see cref="CombineAppId"/> 之类字段做<b>存在性校验</b>，再决定走哪条业务分支。
/// </para>
/// <para>
/// <b>与 <c>CombineQueryResponse</c> 的关系</b>：本载荷是通知形态（字段是查询应答的<b>子集</b>，
/// 无 <c>combine_payer_info.sub_openid</c> 之外的查询附加信息，且 <c>scene_info</c> 只有 <c>device_id</c>）
/// ⇒ 有意不复用查询应答类型（与交易域「查询应答 vs 通知载荷分建」同款理由）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayCombineTransactionResource
{
    /// <summary>合单服务商 APPID（<c>combine_appid</c>，必填）：下单时传入的服务商 APPID。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>合单服务商商户号（<c>combine_mchid</c>，必填）：下单时传入的合单服务商商户号。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>
    /// 合单商户订单号（<c>combine_out_trade_no</c>，必填）：合单下单时传入。
    /// </summary>
    /// <remarks>
    /// <b>本字段是区分「合单通知」与「普通支付通知」的首选判据</b>（见类型 remarks）：
    /// 普通支付载荷没有它 ⇒ 取到非空值即可断定这是合单通知。
    /// </remarks>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>场景信息（<c>scene_info</c>，选填），见 <see cref="WechatPayCombineSceneInfo"/>。</summary>
    [JsonPropertyName("scene_info")]
    public WechatPayCombineSceneInfo? SceneInfo { get; set; }

    /// <summary>商品单列表（<c>sub_orders</c>，必填 array），见 <see cref="WechatPayCombineSubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<WechatPayCombineSubOrder>? SubOrders { get; set; }

    /// <summary>合单支付者信息（<c>combine_payer_info</c>，必填），见 <see cref="WechatPayCombinePayerInfo"/>。</summary>
    [JsonPropertyName("combine_payer_info")]
    public WechatPayCombinePayerInfo? CombinePayerInfo { get; set; }
}

/// <summary>
/// 合单通知的场景信息（<c>scene_info</c>，<b>通知形态</b>）。
/// </summary>
/// <remarks>
/// <b>⚠️ 与本域<b>下单</b>请求的 <c>CombineSceneInfo</c> 不同表</b>：通知里<b>只有 <c>device_id</c></b>
/// （官方原文：下单时传入的支付场景描述），<b>没有</b> <c>payer_client_ip</c> ——
/// 后者是<b>下单入参</b>（服务端采集的客户端 IP），通知不会回吐。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayCombineSceneInfo
{
    /// <summary>商户端设备号（<c>device_id</c>）：终端设备号（门店号或收银设备 ID），下单时传入。</summary>
    [JsonPropertyName("device_id")]
    public string? DeviceId { get; set; }
}

/// <summary>
/// 合单通知里的单个商品单（<c>sub_orders[]</c>，<b>通知形态</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>⚠️ 与本域<b>查询</b>应答的 <c>CombineQuerySubOrder</c> 不是同一张表</b>：本表<b>多</b>
/// <c>trade_type</c> / <c>bank_type</c> / <c>success_time</c> / <c>promotion_detail</c>，
/// 而<b>无</b>查询侧的 <c>description</c> 等字段 ⇒ 分建（本域 CB5 纪律：表相同则共用，表不同则分建）。
/// </para>
/// <para>
/// <b>状态取值</b>见 <see cref="CombineTradeStates"/>（<c>SUCCESS</c> / <c>NOTPAY</c> / <c>CLOSED</c>），
/// <b>交易类型取值</b>见 <see cref="CombineTradeTypes"/>（<c>JSAPI</c> / <c>NATIVE</c> / <c>APP</c> / <c>MWEB</c>）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayCombineSubOrder
{
    /// <summary>商品单商户号（<c>mchid</c>）：合单下单时传入的服务商商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>交易类型（<c>trade_type</c>）：取值见 <see cref="CombineTradeTypes"/>（<b>H5 场景的取值是 <c>MWEB</c></b>）。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>交易状态（<c>trade_state</c>）：取值见 <see cref="CombineTradeStates"/>，<b>须显式判定为 <c>SUCCESS</c></b>。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>
    /// 付款银行（<c>bank_type</c>）：官方原文「银行卡支付返回如 <c>ICBC_DEBIT</c>，
    /// <b>非银行卡统一返回 <c>OTHERS</c></b>」⇒ 勿把 <c>OTHERS</c> 当成"未知/异常"。
    /// </summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>商户数据包（<c>attach</c>，选填）：下单传入的自定义数据包，<b>原样返回</b>。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，rfc3339）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>商品单微信支付订单号（<c>transaction_id</c>）：微信为<b>每个商品单</b>分配的唯一标识（非合单总单）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商品单商户订单号（<c>out_trade_no</c>）：合单下单时传入的商品单商户订单号。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>）：合单下单时传入的特约商户号。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>子商户 APPID（<c>sub_appid</c>，选填）：<c>sub_mchid</c> 绑定的 <c>sub_appid</c>。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppId { get; set; }

    /// <summary>用户子商户标识（<c>sub_openid</c>，选填）：下单传入 <c>sub_appid</c> 后返回。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenId { get; set; }

    /// <summary>商品单金额信息（<c>amount</c>，必填），见 <see cref="WechatPayCombineSubOrderAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public WechatPayCombineSubOrderAmount? Amount { get; set; }

    /// <summary>
    /// 优惠功能（<c>promotion_detail</c>，选填 array）：字段表与支付分域一致 ⇒ 复用
    /// <see cref="PayScorePromotionDetail"/>（其内已含 <c>goods_detail</c> 单品列表）。
    /// </summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayScorePromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>
/// 合单通知的商品单金额信息（<c>sub_orders[].amount</c>）。
/// </summary>
/// <remarks>
/// <b>⚠️ 字段比下单请求的 <c>CombineSubOrderAmount</c> 多</b>：下单只有
/// <c>total_amount</c> / <c>currency</c>，而通知另有<b>实付</b>维度
/// （<c>payer_amount</c> / <c>payer_currency</c> / <c>settlement_rate</c>）⇒ 分建。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayCombineSubOrderAmount
{
    /// <summary>标价金额（<c>total_amount</c>，必填，单位分）。</summary>
    [JsonPropertyName("total_amount")]
    public long? TotalAmount { get; set; }

    /// <summary>标价币种（<c>currency</c>，必填）：下单时传入的标价币种。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>用户支付金额（<c>payer_amount</c>，必填，单位分）：官方原文「= 标价金额 − 代金券金额」。</summary>
    [JsonPropertyName("payer_amount")]
    public long? PayerAmount { get; set; }

    /// <summary>用户支付币种（<c>payer_currency</c>）：ISO 4217 三位字母代码，如 <c>CNY</c>。</summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }

    /// <summary>结算汇率（<c>settlement_rate</c>，选填）：标价币种与结算币种不一致时返回（<b>汇率 × 10^8</b>）。</summary>
    [JsonPropertyName("settlement_rate")]
    public long? SettlementRate { get; set; }
}

/// <summary>
/// 合单通知的支付者信息（<c>combine_payer_info</c>）。
/// </summary>
/// <remarks>
/// <b>通知里只有 <c>openid</c></b>（实际支付的用户在 <c>combine_appid</c> 下对应的标识）——
/// 与下单请求的 <c>CombinePayerInfo</c>（<c>openid</c> / <c>sub_openid</c> 二选一）<b>不是</b>同一张表。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Callback")]
public class WechatPayCombinePayerInfo
{
    /// <summary>用户服务商标识（<c>openid</c>，必填）：实际支付的用户在 <c>combine_appid</c> 下对应的 openid。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
