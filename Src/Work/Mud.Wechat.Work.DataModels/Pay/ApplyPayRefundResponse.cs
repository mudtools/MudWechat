// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Pay;

/// <summary>
/// 申请退款响应体（<c>/cgi-bin/miniapppay/refund</c>，退款域）。
/// <para>
/// 官方业务限制：接口返回仅代表受理情况，退款是否成功须通过查询退款接口
/// （<c>get_refund_detail</c>）或退款通知回调确认。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class ApplyPayRefundResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置商户退款单号（商户系统内部的退款单号，商户系统内部唯一，
    /// 同一退款单号多次请求只退一笔）。
    /// </summary>
    [JsonPropertyName("out_refund_no")]
    public string? OutRefundNo { get; set; }

    /// <summary>
    /// 获取或设置订单金额信息（见 <see cref="PayRefundAmount"/>）。
    /// </summary>
    [JsonPropertyName("amount")]
    public PayRefundAmount? Amount { get; set; }

    /// <summary>
    /// 获取或设置优惠退款详情列表（官方可选，优惠退款功能信息，
    /// discount_refund &gt; 0 时返回，见 <see cref="PayRefundPromotionDetail"/>）。
    /// </summary>
    [JsonPropertyName("promotion_detail")]
    public List<PayRefundPromotionDetail>? PromotionDetail { get; set; }
}

/// <summary>
/// 退款响应的订单金额信息（<see cref="ApplyPayRefundResponse.Amount"/> /
/// <see cref="GetPayRefundDetailResponse.Amount"/>，申请退款与查询退款结构一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayRefundAmount
{
    /// <summary>
    /// 获取或设置退款金额：币种的最小单位（分），只能为整数，不能超过原订单支付金额。
    /// </summary>
    [JsonPropertyName("refund")]
    public long? Refund { get; set; }

    /// <summary>
    /// 获取或设置用户退款金额（官方可选）：退给用户的金额，不包含所有优惠券金额。
    /// </summary>
    [JsonPropertyName("payer_refund")]
    public long? PayerRefund { get; set; }

    /// <summary>
    /// 获取或设置优惠退款金额（官方可选）：优惠券的退款金额，原支付单的优惠按比例退款。
    /// </summary>
    [JsonPropertyName("discount_refund")]
    public long? DiscountRefund { get; set; }

    /// <summary>
    /// 获取或设置退款币种：符合 ISO 4217 标准的三位字母代码，目前只支持人民币：CNY。
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}

/// <summary>
/// 退款响应的优惠退款详情（<see cref="ApplyPayRefundResponse.PromotionDetail"/> /
/// <see cref="GetPayRefundDetailResponse.PromotionDetail"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Pay")]
public class PayRefundPromotionDetail
{
    /// <summary>
    /// 获取或设置券 ID（官方可选）：券或者立减优惠 id。
    /// </summary>
    [JsonPropertyName("promotion_id")]
    public string? PromotionId { get; set; }

    /// <summary>
    /// 获取或设置优惠范围（官方可选）：GLOBAL - 全场代金券、SINGLE - 单品优惠。
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    /// <summary>
    /// 获取或设置优惠类型（官方可选）：COUPON - 充值型代金券（商户需预先充值营销经费）、
    /// DISCOUNT - 免充值型优惠券（商户不需要预先充值营销经费）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置优惠券面额（官方可选）：用户享受优惠的金额
    /// （优惠券面额 = 微信出资金额 + 商家出资金额 + 其他出资方金额）。
    /// </summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>
    /// 获取或设置优惠退款金额（官方可选）：代金券退款金额 ≤ 退款金额，
    /// 退款金额 - 代金券或立减优惠退款金额为现金。
    /// </summary>
    [JsonPropertyName("refund_amount")]
    public long? RefundAmount { get; set; }
}
