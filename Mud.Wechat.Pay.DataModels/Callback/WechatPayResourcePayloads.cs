// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

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
