// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 申请退款请求体（<c>/cgi-bin/miniapppay/refund</c>，退款域）。
/// <para>
/// 官方业务限制：交易时间超过一年的订单无法提交退款；支持单笔交易分多次退款，
/// 总退款金额不能超过订单金额，每个支付订单的部分退款次数不能超过 50 次；
/// 退款失败重试须沿用原退款单号（同一退款单号多次请求只退一笔）。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class ApplyPayRefundRequest
{
    /// <summary>
    /// 获取或设置商户号（官方必填，企业微信分配商户号）。
    /// </summary>
    [JsonPropertyName("mchid")]
    public string? MchId { get; set; }

    /// <summary>
    /// 获取或设置商户 APPID（官方必填，小程序 appid）。
    /// </summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 获取或设置商户订单号（官方必填，6~32 个字符）：原支付交易对应的商户订单号。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 获取或设置商户退款单号（官方必填，1~64 个字符）：
    /// 商户系统内部的退款单号，商户系统内部唯一，只能是数字、大小写字母、<c>_-|*@</c>，
    /// 同一退款单号多次请求只退一笔。
    /// </summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>
    /// 获取或设置退款原因（官方可选，1~80 个字符）：
    /// 若传入，会在下发给用户的退款消息中体现退款原因。
    /// <para>官方业务限制：若订单退款金额 ≤ 1 元且属于部分退款，则不会在退款消息中体现退款原因。</para>
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// 获取或设置订单金额信息（官方必填，见 <see cref="PayRefundApplyAmount"/>）。
    /// </summary>
    [JsonPropertyName("amount")]
    public PayRefundApplyAmount? Amount { get; set; }

    /// <summary>
    /// 获取或设置资金账户（官方可选，1~32 个字符）：
    /// 若订单处于待分账状态，填写该字段后退款时直接从二级商户余额中退款。
    /// 当前枚举：AVAILABLE - 可用余额。
    /// </summary>
    [JsonPropertyName("funds_account")]
    public string? FundsAccount { get; set; }
}

/// <summary>
/// 申请退款的订单金额信息（<see cref="ApplyPayRefundRequest.Amount"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayRefundApplyAmount
{
    /// <summary>
    /// 获取或设置退款金额（官方必填）：币种的最小单位（分），只能为整数，
    /// 不能超过原订单支付金额。
    /// </summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>
    /// 获取或设置原订单金额（官方必填）：原支付交易的订单总金额，
    /// 币种的最小单位（分），只能为整数。
    /// </summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }

    /// <summary>
    /// 获取或设置退款币种（官方必填）：符合 ISO 4217 标准的三位字母代码，
    /// 目前只支持人民币：CNY。
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}
