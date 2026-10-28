// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Refund;

/// <summary>
/// 退款申请请求体（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012791903"/>
/// （同内容另有 <c>4013071036</c> 入口；2026-10-09 逐字段核验）。
/// <b>POST</b> <c>/v3/refund/domestic/refunds</c>，支持商户类型：<b>普通商户</b>。
/// </para>
/// <para>
/// <b>字段名照官方原文</b>（<c>out_refund_no</c> / <c>notify_url</c> / <c>funds_account</c> /
/// <c>refund_amount</c> / <c>refund_quantity</c>），契约守卫断言以官方为准，<b>不得「纠正」</b>。
/// </para>
/// <para>
/// <b>必填</b>：<c>out_refund_no</c> / <c>amount</c>，以及 <c>transaction_id</c> 与 <c>out_trade_no</c>
/// <b>二选一</b>（同时传入时官方以 <c>transaction_id</c> 为准）。其余选填一律可空 ——
/// AOT 下源生成按声明类型序列化，可空性即「是否随报文上送」的唯一表达（不做运行时多态）。
/// </para>
/// <para>
/// <b>业务限制（官方原文要点）</b>：① <c>out_refund_no</c> 须 6-32 字符、同一商户号下唯一；
/// ② 单笔订单累计退款金额不得超过订单总额；③ 退款申请受理后官方<b>异步</b>执行，
/// 最终结果经退款结果回调通知或查询单笔退款获取；④ 部分退款场景须上送 <c>amount.refund</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class RefundApplyRequest
{
    /// <summary>微信支付订单号（<c>transaction_id</c>，string(32)；与 <see cref="OutTradeNo"/> 二选一，两者都传时优先本字段）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，string(32)；与 <see cref="TransactionId"/> 二选一）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>商户退款单号（<c>out_refund_no</c>，必填 string(64)）：6-32 字符，同一商户号下唯一。</summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>退款原因（<c>reason</c>，选填 string(80)），用户可见。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>退款结果回调地址（<c>notify_url</c>，选填 string(255)），须为可公网访问的 HTTPS / HTTP 地址。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>
    /// 退款资金来源（<c>funds_account</c>，选填 string(32)）：<c>AVAILABLE</c>（可用余额，默认，实时到账）/
    /// <c>UNSETTLED</c>（未结算资金，仅 T+1 后到账）。
    /// </summary>
    [JsonPropertyName("funds_account")]
    public string? FundsAccount { get; set; }

    /// <summary>退款金额信息（<c>amount</c>，必填），见 <see cref="RefundApplyAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public RefundApplyAmount? Amount { get; set; }

    /// <summary>退款商品信息（<c>goods_detail</c>，选填数组），见 <see cref="RefundGoodsDetail"/>。</summary>
    [JsonPropertyName("goods_detail")]
    public List<RefundGoodsDetail>? GoodsDetail { get; set; }
}

/// <summary>退款申请金额（<c>amount</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class RefundApplyAmount
{
    /// <summary>退款金额（<c>refund</c>，必填整型，单位为分，不能超过订单可退金额）。</summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>退款出资账户及金额（<c>from</c>，选填数组），见 <see cref="RefundFundsFrom"/>。</summary>
    [JsonPropertyName("from")]
    public List<RefundFundsFrom>? From { get; set; }

    /// <summary>原订单金额（<c>total</c>，选填整型，单位为分）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>退款币种（<c>currency</c>，选填 string(16)），固定 <c>CNY</c>（目前仅支持人民币）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>退款出资账户及金额（<c>from[]</c> 元素；申请与应答共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class RefundFundsFrom
{
    /// <summary>
    /// 出资账户类型（<c>account</c>，string(32)）：<c>AVAILABLE</c> / <c>UNSETTLED</c> /
    /// <c>UNAVAILABLE</c> / <c>OPERATION</c> / <c>BASIC</c>。
    /// </summary>
    [JsonPropertyName("account")]
    public string? Account { get; set; }

    /// <summary>对应账户的出资金额（<c>amount</c>，整型，单位为分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}

/// <summary>退款商品信息（<c>goods_detail[]</c> 元素；申请与应答共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Refund")]
public class RefundGoodsDetail
{
    /// <summary>商户侧商品编码（<c>merchant_goods_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("merchant_goods_id")]
    public string? MerchantGoodsId { get; set; }

    /// <summary>微信支付商品编码（<c>wechatpay_goods_id</c>，选填 string(32)）。</summary>
    [JsonPropertyName("wechatpay_goods_id")]
    public string? WechatpayGoodsId { get; set; }

    /// <summary>商品名称（<c>goods_name</c>，选填 string(256)）。</summary>
    [JsonPropertyName("goods_name")]
    public string? GoodsName { get; set; }

    /// <summary>商品单价（<c>unit_price</c>，必填整型，单位为分）。</summary>
    [JsonPropertyName("unit_price")]
    public long? UnitPrice { get; set; }

    /// <summary>商品退款金额（<c>refund_amount</c>，必填整型，单位为分）。</summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }

    /// <summary>商品退款数量（<c>refund_quantity</c>，必填整型）。</summary>
    [JsonPropertyName("refund_quantity")]
    public long? RefundQuantity { get; set; }
}
