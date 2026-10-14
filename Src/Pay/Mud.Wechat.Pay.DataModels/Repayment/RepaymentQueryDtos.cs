// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;

namespace Mud.Wechat.Pay.DataModels.Repayment;

/// <summary>
/// 「还款单查询」应答（<b>GET /v3/repayment/combine-transactions/partner/out-trade-no/{combine_out_trade_no}</b>）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015124483"/>，2026-10-14 逐字段核验。</para>
/// <para><b>必填性与下单面镜像反转</b>：本面 <c>sub_orders</c> 与 <c>combine_payer_info</c> 降为<b>选填</b>、
/// 明细内 <c>sub_mchid</c> / <c>attach</c> 亦降为选填，而 <c>trade_state</c> 升为<b>必填</b>
/// ⇒ 与 <see cref="RepaymentPrepayRequest"/> 形态相反，两型不可互为别名。</para>
/// <para><b>本面多出 <c>combine_appid</c> 与 <c>combine_transaction_id</c></b>：
/// 下单请求只有 <c>combine_mchid</c>，AppID 与合单微信单号只在应答里回给。</para>
/// <para><b>支付者信息形态变化</b>：下单面 <c>combine_payer_info.identity</c>（实名三节、密文），
/// 查询面 <c>combine_payer_info.openid</c> ⇒ 同名不同表，分建 <see cref="RepaymentQueryPayerInfo"/>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentQueryResponse : WechatPayResponse
{
    /// <summary>合单 AppID（<c>combine_appid</c>，必填 string(32)）：下单请求体里没有该节，仅应答回给。</summary>
    [JsonPropertyName("combine_appid")]
    public string? CombineAppId { get; set; }

    /// <summary>还款发起方商户号（<c>combine_mchid</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_mchid")]
    public string? CombineMchId { get; set; }

    /// <summary>商户还款单号（<c>combine_out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_out_trade_no")]
    public string? CombineOutTradeNo { get; set; }

    /// <summary>微信支付合单号（<c>combine_transaction_id</c>，必填 string(32)）。</summary>
    [JsonPropertyName("combine_transaction_id")]
    public string? CombineTransactionId { get; set; }

    /// <summary>支付者信息（<c>combine_payer_info</c>，选填 object）：本面只回 <c>openid</c>，见 <see cref="RepaymentQueryPayerInfo"/>。</summary>
    [JsonPropertyName("combine_payer_info")]
    public RepaymentQueryPayerInfo? CombinePayerInfo { get; set; }

    /// <summary>还款明细单列表（<c>sub_orders</c>，<b>本面选填</b> array）：见 <see cref="RepaymentQuerySubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<RepaymentQuerySubOrder>? SubOrders { get; set; }
}

/// <summary>
/// 支付者信息（<c>combine_payer_info</c>，查询面）。
/// </summary>
/// <remarks>与下单面的实名信息对象<b>字段集完全不同</b>（本面仅 <c>openid</c>）⇒ 分建类型。</remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentQueryPayerInfo
{
    /// <summary>用户 OpenID（<c>openid</c>，选填 string(128)）：支付成功后回给。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>
/// 还款明细单（<c>sub_orders[]</c>，查询面）。
/// </summary>
/// <remarks>
/// <para><b>比下单面多六节</b>：<c>transaction_id</c> / <c>trade_type</c> / <c>trade_state</c> /
/// <c>bank_type</c> / <c>success_time</c> 与扩展后的金额对象；<b>少了 <c>description</c> 与
/// <c>settle_info</c> 两节。</b></para>
/// <para><b><c>sub_mchid</c> 本面选填</b>（下单面必填）：与 <c>attach</c> 同为「下单必填、查询选填」的镜像反转样本。</para>
/// <para><c>bank_type</c> 官方注明仅支付成功后返回，非银行卡支付统一回 <c>OTHERS</c>；
/// 本页<b>未附「可选取值」表</b> ⇒ 值集原样透传，不建常量类。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentQuerySubOrder
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）：服务商（还款发起方）商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，<b>本面选填</b> string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }

    /// <summary>商户明细单号（<c>out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>微信支付明细单号（<c>transaction_id</c>，选填 string(32)）：支付成功后返回。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>交易类型（<c>trade_type</c>，选填 string(16)）：本面未附取值表，原样透传。</summary>
    [JsonPropertyName("trade_type")]
    public string? TradeType { get; set; }

    /// <summary>交易状态（<c>trade_state</c>，<b>本面必填</b> string(32)，3 值）：见 <see cref="RepaymentTradeStates"/>。</summary>
    [JsonPropertyName("trade_state")]
    public string? TradeState { get; set; }

    /// <summary>付款银行（<c>bank_type</c>，选填 string(32)）：支付成功后返回，非银行卡支付回 <c>OTHERS</c>。</summary>
    [JsonPropertyName("bank_type")]
    public string? BankType { get; set; }

    /// <summary>商户数据包（<c>attach</c>，<b>本面选填</b> string(128)）：下单未传则不返回。</summary>
    [JsonPropertyName("attach")]
    public string? Attach { get; set; }

    /// <summary>支付完成时间（<c>success_time</c>，选填 string(32)）：rfc3339，保持 <c>string</c>。</summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>订单金额（<c>amount</c>，选填 object）：本面 4 字段，见 <see cref="RepaymentQuerySubOrderAmount"/>。</summary>
    [JsonPropertyName("amount")]
    public RepaymentQuerySubOrderAmount? Amount { get; set; }
}

/// <summary>
/// 还款明细单金额（<c>sub_orders[].amount</c>，查询面，官方 4 字段）。
/// </summary>
/// <remarks>
/// <b>比下单面多 <c>payer_amount</c> 与 <c>payer_currency</c>，且 <c>currency</c> 长度档为 string(16)</b>
/// （下单面 string(8)）⇒ 两型分建，勿以「订单金额」一个类型覆盖两域。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentQuerySubOrderAmount
{
    /// <summary>商品单总金额（<c>total_amount</c>，必填 integer）：单位为分。</summary>
    [JsonPropertyName("total_amount")]
    public int? TotalAmount { get; set; }

    /// <summary>用户支付金额（<c>payer_amount</c>，必填 integer）：单位为分，本面独有节。</summary>
    [JsonPropertyName("payer_amount")]
    public int? PayerAmount { get; set; }

    /// <summary>货币（<c>currency</c>，必填 string(16)）：本面长度档为 16，下单面为 8。</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>用户支付币种（<c>payer_currency</c>，必填 string(16)）：本面独有节。</summary>
    [JsonPropertyName("payer_currency")]
    public string? PayerCurrency { get; set; }
}

/// <summary>
/// 「商户关单」请求（<b>POST /v3/repayment/combine-transactions/partner/out-trade-no/{combine_out_trade_no}/close</b> 的包体）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015124497"/>，2026-10-14 逐字段核验。</para>
/// <para><b>定位键两处并存</b>：还款单号走 <b>path</b>（<c>combine_out_trade_no</c>），
/// 待关明细走<b>包体</b> <c>sub_orders</c> ⇒ 包体内<b>不再</b>重复还款单号，本 DTO 只有一个字段。</para>
/// <para>官方成功应答为 <b>204 无包体</b> ⇒ 端点返回裸 <see cref="Task"/>，不为其伪造响应类型（守卫 RP-B1）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentCloseRequest
{
    /// <summary>待关闭的还款明细单列表（<c>sub_orders</c>，必填 array）：见 <see cref="RepaymentCloseSubOrder"/>。</summary>
    [JsonPropertyName("sub_orders")]
    public List<RepaymentCloseSubOrder>? SubOrders { get; set; }
}

/// <summary>
/// 还款明细单关单入参（<c>sub_orders[]</c>，关单面，官方 3 字段）。
/// </summary>
/// <remarks><b>本面 <c>sub_mchid</c> 选填</b>（下单面必填），且无金额与描述节。</remarks>
[HttpJsonSerializable(SerializerClassName = "Repayment")]
public class RepaymentCloseSubOrder
{
    /// <summary>商户号（<c>mchid</c>，必填 string(32)）：服务商（还款发起方）商户号。</summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>商户明细单号（<c>out_trade_no</c>，必填 string(32)）。</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>子商户号（<c>sub_mchid</c>，<b>本面选填</b> string(32)）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}
