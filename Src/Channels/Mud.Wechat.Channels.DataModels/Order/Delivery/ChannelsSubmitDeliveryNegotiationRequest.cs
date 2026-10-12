// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「发货协商 / 物流变更 / 生鲜质检 / 预约发货」DTO
// （order/deliveryinfo/update、order/deliverynegotiation/submit、order/deliverynegotiation/result/get、
//  order/freshinspect/submit、order/userbooking/list）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>提交发货协商申请（<c>order/deliverynegotiation/submit</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsSubmitDeliveryNegotiationRequest
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>，必填）。</summary>
    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>获取或设置预计发货时间（官方 <c>predict_delivery_time</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("predict_delivery_time")]
    public long PredictDeliveryTime { get; set; }

    /// <summary>获取或设置协商原因（官方 <c>reason</c>，必填，枚举值以官方文档为准）。</summary>
    [JsonPropertyName("reason")]
    public int Reason { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>，选填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }
}
