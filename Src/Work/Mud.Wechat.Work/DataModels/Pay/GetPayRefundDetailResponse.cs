// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 查询退款响应体（<c>/cgi-bin/miniapppay/get_refund_detail</c>，退款域）。
/// <para>
/// 官方业务限制：退款有一定延时，用零钱支付的退款 20 分钟内到账，
/// 银行卡支付的退款 3 个工作日后重新查询退款状态。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class GetPayRefundDetailResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置微信退款单号（微信支付退款订单号）。
    /// </summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>
    /// 获取或设置商户退款单号（商户系统内部的退款单号，商户系统内部唯一，
    /// 同一退款单号多次请求只退一笔）。
    /// </summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>
    /// 获取或设置微信订单号（微信支付交易订单号）。
    /// </summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>
    /// 获取或设置商户订单号（返回的原交易订单号）。
    /// </summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>
    /// 获取或设置退款渠道（官方可选）：ORIGINAL - 原路退款、BALANCE - 退回到余额、
    /// OTHER_BALANCE - 原账户异常退到其他余额账户、OTHER_BANKCARD - 原银行卡异常退到其他银行卡。
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    /// <summary>
    /// 获取或设置退款入账账户（官方可选，1~64 个字符）：取当前退款单的退款入账方。
    /// <para>
    /// 取值示例：退回银行卡 <c>{银行名称}{卡类型}{卡尾号}</c>、
    /// 退回支付用户零钱 - 支付用户零钱、退还商户 - 商户基本账户/商户结算银行账户、
    /// 退回支付用户零钱通 - 支付用户零钱通。
    /// </para>
    /// </summary>
    [JsonPropertyName("user_received_account")]
    public string? UserReceivedAccount { get; set; }

    /// <summary>
    /// 获取或设置退款成功时间（官方可选）：status 为 SUCCESS 时返回，
    /// rfc3339 标准格式（如 2018-06-08T10:34:56+08:00）。
    /// </summary>
    [JsonPropertyName("success_time")]
    public string? SuccessTime { get; set; }

    /// <summary>
    /// 获取或设置退款创建时间（官方可选）：退款受理时间，rfc3339 标准格式；
    /// 当退款状态为退款成功时返回此字段。
    /// </summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>
    /// 获取或设置退款状态（官方必填）：SUCCESS - 退款成功、CLOSE - 退款关闭、
    /// PROCESSING - 退款处理中、ABNORMAL - 退款异常（用户卡作废或冻结导致原路退款银行卡失败）。
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// 获取或设置退款金额信息（官方必填，见 <see cref="PayRefundAmount"/>）。
    /// </summary>
    [JsonPropertyName("amount")]
    public PayRefundAmount? Amount { get; set; }

    /// <summary>
    /// 获取或设置营销详情列表（官方可选，优惠退款信息，discount_refund &gt; 0 时返回，
    /// 见 <see cref="PayRefundPromotionDetail"/>）。
    /// </summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayRefundPromotionDetail>? PromotionDetail { get; set; }
}
