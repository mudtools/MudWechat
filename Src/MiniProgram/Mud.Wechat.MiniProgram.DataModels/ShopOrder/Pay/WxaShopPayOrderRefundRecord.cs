// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 支付订单的退款记录（<c>/shop/pay/getorder</c> 回包 <c>order.refund_list[]</c>）。
/// </summary>

[HttpJsonSerializable(SerializerClassName = "ShopOrder")]
public class WxaShopPayOrderRefundRecord
{
    /// <summary>商家退款单号。</summary>
    [JsonPropertyName("refund_no")]
    public string? RefundNo { get; set; }

    /// <summary>微信支付侧退款单号（敏感，不得进日志）。</summary>
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    /// <summary>退款金额，<b>单位分</b>。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>退款申请时间，秒级时间戳。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>退款完成时间，秒级时间戳。</summary>
    [JsonPropertyName("finish_time")]
    public long? FinishTime { get; set; }

    /// <summary>退款结果（官方未枚举取值）。</summary>
    [JsonPropertyName("result")]
    public string? Result { get; set; }

    /// <summary>失败原因。</summary>
    [JsonPropertyName("fail_reason")]
    public string? FailReason { get; set; }
}
