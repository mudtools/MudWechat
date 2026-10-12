// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>充值订单项（<c>list</c> 元素，<c>getpayorderlist</c> 响应）。</summary>
/// <remarks>
/// 订单状态 <c>order_status</c>：1 待支付、2 支付成功、3 使用中、4 已用完、5 退款中、6 已退款、10 支付超时；
/// 退款状态 <c>refund_status</c>：1 未退款、2 退款中、4 退款成功、5 退款失败。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressPayOrderItem
{
    /// <summary>充值单号（<c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long OrderId { get; set; }

    /// <summary>订单状态（<c>order_status</c>；枚举见类说明）。</summary>
    [JsonPropertyName("order_status")]
    public int OrderStatus { get; set; }

    /// <summary>充值金额（<c>total_price</c>）。</summary>
    [JsonPropertyName("total_price")]
    public long TotalPrice { get; set; }

    /// <summary>订单创建时间（<c>create_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long CreateTime { get; set; }

    /// <summary>支付时间（<c>pay_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("pay_time")]
    public long PayTime { get; set; }

    /// <summary>是否可以退款（<c>can_refund</c>）。</summary>
    /// <remarks>官方参数表标注为 <c>number</c>，但官方返回示例为布尔 <c>true</c>；本 SDK 按示例实际线格式以 <c>bool</c> 承载。</remarks>
    [JsonPropertyName("can_refund")]
    public bool CanRefund { get; set; }

    /// <summary>退款时间（<c>refund_time</c>；秒级时间戳）。</summary>
    [JsonPropertyName("refund_time")]
    public long RefundTime { get; set; }

    /// <summary>退款状态（<c>refund_status</c>；枚举见类说明）。</summary>
    [JsonPropertyName("refund_status")]
    public int RefundStatus { get; set; }

    /// <summary>退款金额（<c>refund_amt</c>）。</summary>
    [JsonPropertyName("refund_amt")]
    public long RefundAmount { get; set; }
}
