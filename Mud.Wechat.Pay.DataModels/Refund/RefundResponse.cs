// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Refund;

/// <summary>
/// 退款应答（微信支付 APIv3；退款申请 / 查询单笔退款 / 发起异常退款<b>共用同一应答体</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：退款申请 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791903"/>；
/// 查询单笔退款 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791904"/>；
/// 发起异常退款 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791905"/>（2026-10-09 逐字段核验）。
/// </para>
/// <para>
/// <b>三端点共享本 DTO 的依据</b>：官方对三者给出<b>一致的应答字段表</b>（仅请求参数不同），
/// 复用同一类型可避免三份必然漂移的重复定义。
/// </para>
/// <para>
/// <b>字段名照官方原文</b>（<c>out_refund_no</c> / <c>user_received_account</c> / <c>settlement_refund</c> …）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class RefundResponse : WechatPayResponse
{
    /// <summary>微信支付退款单号（<c>refund_id</c>，string(32)）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，string(64)）。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 退款渠道（<c>channel</c>，string(32)）：<c>ORIGINAL</c>（原路退款）/
    /// <c>BALANCE</c>（退回到余额）/ <c>OTHER_BALANCE</c> / <c>OTHER_BANKCARD</c>。
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    /// <summary>退款入账账户（<c>user_received_account</c>，string(64)），如「招商银行信用卡 1234」「微信零钱」。</summary>
    [JsonPropertyName("user_received_account")]
    public string? UserReceivedAccount { get; set; }

    /// <summary>退款成功时间（<c>success_time</c>，string(64)，rfc3339；未成功时缺省）。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>退款创建时间（<c>create_time</c>，string(64)，rfc3339）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>
    /// 退款状态（<c>status</c>，string(32)）：<c>SUCCESS</c> / <c>CLOSED</c> /
    /// <c>PROCESSING</c>（退款处理中）/ <c>ABNORMAL</c>（退款异常，须调用发起异常退款接口）。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// 退款资金账户（<c>funds_account</c>，string(32)）：<c>UNSETTLED</c> / <c>AVAILABLE</c> /
    /// <c>UNAVAILABLE</c> / <c>OPERATION</c> / <c>BASIC</c> / <c>ECNY_BASIC</c>（数字人民币基本账户）。
    /// </summary>
    [JsonPropertyName("funds_account")]
    public string? FundsAccount { get; set; }

    /// <summary>退款金额信息（<c>amount</c>），见 <see cref="RefundAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public RefundAmount? Amount { get; set; }

    /// <summary>优惠退款信息（<c>promotion_detail</c>，数组），见 <see cref="RefundPromotionDetail"/>。</summary>
    [JsonPropertyName("promotion_detail")]
    public List<RefundPromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>退款应答的金额信息（<c>amount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class RefundAmount
{
    /// <summary>订单总金额（<c>total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>退款金额（<c>refund</c>，整型，单位为分）。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>退款出资的账户类型及金额（<c>from</c>，数组），见 <see cref="RefundFundsFrom"/>。</summary>
    [JsonPropertyName("from")]
    public List<RefundFundsFrom>? From { get; set; }

    /// <summary>用户支付金额（<c>payer_total</c>，整型，单位为分）。</summary>
    [JsonPropertyName("payer_total")]
    public long? PayerTotal { get; set; }

    /// <summary>用户退款金额（<c>payer_refund</c>，整型，单位为分），即用户实际退回的金额。</summary>
    [JsonPropertyName("payer_refund")]
    public long? PayerRefund { get; set; }

    /// <summary>应结退款金额（<c>settlement_refund</c>，整型，单位为分），扣除优惠后退款金额。</summary>
    [JsonPropertyName("settlement_refund")]
    public long? SettlementRefund { get; set; }

    /// <summary>应结订单金额（<c>settlement_total</c>，整型，单位为分），扣除优惠后订单金额。</summary>
    [JsonPropertyName("settlement_total")]
    public long? SettlementTotal { get; set; }

    /// <summary>优惠退款金额（<c>discount_refund</c>，整型，单位为分）。</summary>
    [JsonPropertyName("discount_refund")]
    public long? DiscountRefund { get; set; }

    /// <summary>退款币种（<c>currency</c>，string(16)），固定 <c>CNY</c>。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>手续费退款金额（<c>refund_fee</c>，整型，单位为分）。</summary>
    [JsonPropertyName("refund_fee")]
    public long? RefundFee { get; set; }
}

/// <summary>退款应答的优惠退款明细（<c>promotion_detail[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class RefundPromotionDetail
{
    /// <summary>券 ID（<c>promotion_id</c>，string(32)）。</summary>
    [JsonPropertyName("promotion_id")]
    public string? PromotionId { get; set; }

    /// <summary>优惠范围（<c>scope</c>，string(32)）：<c>GLOBAL</c> / <c>SINGLE</c>。</summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>优惠类型（<c>type</c>，string(32)）：<c>CASH</c> / <c>NOCASH</c>。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>优惠券面额（<c>amount</c>，整型，单位为分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>优惠退款金额（<c>refund_amount</c>，整型，单位为分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }

    /// <summary>商品退款信息（<c>goods_detail</c>，数组），见 <see cref="RefundGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<RefundGoodsDetail>? GoodsDetail { get; set; }
}
