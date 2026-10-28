// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Transactions;

/// <summary>
/// 付款码支付应答（微信支付 APIv3）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012382150"/>；
/// <b>POST /v3/pay/transactions/codepay</b>。</para>
/// <para><b>业务特征</b>：付款码支付为<b>实时扣款</b>接口，成功即返回完整的交易信息（含 <c>transaction_id</c>），
/// 与 JSAPI/Native 等「先获取预支付 ID、再由用户发起支付」的异步流程不同。</para>
/// <para><b>继承 <see cref="WechatPayResponse"/></b>：失败时官方返回 4xx + <c>{"code","message"}</c>，
/// 继承后错误体可原样落到 <c>Code</c>/<c>Message</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CodepayResponse : WechatPayResponse
{
    /// <summary>微信支付订单号（<c>transaction_id</c>，必填 string(32)）：微信支付的唯一订单标识。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>商户订单号（<c>out_trade_no</c>，必填 string(32)）：商户系统的唯一订单标识。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>交易类型（<c>trade_type</c>，必填 string(16)）：固定为 <c>CODEPAY</c>。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>交易状态（<c>trade_state</c>，必填 string(16)）：
    /// <c>SUCCESS</c>（支付成功）/ <c>REFUND</c>（转入退款）/ <c>NOTPAY</c>（未支付）/
    /// <c>CLOSED</c>（已关闭）/ <c>REVOKED</c>（已撤销）/ <c>PAYERROR</c>（支付失败）。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>交易状态描述（<c>trade_state_desc</c>，选填 string(256)）：对当前交易状态的简要说明。</summary>
    [JsonPropertyName("trade_state_desc")]
    public string? TradeStateDesc { get; set; }

    /// <summary>付款银行（<c>bank_type</c>，选填 string(16)）：银行类型，采用字符串形式的银行标识。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>附加数据（<c>attach</c>，选填 string(128)）：在查询 API 和支付通知中原样返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，选填 string(64)）：rfc3339 格式，如 <c>2018-06-08T10:34:56+08:00</c>。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>支付者信息（<c>payer</c>，选填）。</summary>
    [JsonPropertyName("payer")]
    public CodepayPayer? Payer { get; set; }

    /// <summary>金额信息（<c>amount</c>，选填）。</summary>
    [JsonPropertyName("amount")]
    public CodepayAmount? Amount { get; set; }
}

/// <summary>
/// 付款码支付的支付者信息。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CodepayPayer
{
    /// <summary>用户标识（<c>openid</c>，必填 string(128)）：用户在直连商户 appid 下的唯一标识。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 付款码支付的金额信息。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Transactions")]
public class CodepayAmount
{
    /// <summary>订单总金额（<c>total</c>，必填 int）：单位为分。</summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>用户支付金额（<c>payer_total</c>，选填 int）：单位为分，可能因优惠而小于总金额。</summary>
    [JsonPropertyName("payer_total")]
    public int? PayerTotal { get; set; }

    /// <summary>货币类型（<c>currency</c>，选填 string(16)）：CNY（人民币）。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>用户支付币种（<c>payer_currency</c>，选填 string(16)）：CNY（人民币）。</summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }
}
