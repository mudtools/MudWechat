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

/// <summary>用户预约发货记录（官方 <c>order/userbooking/list</c> 的 <c>record_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsUserBookingRecord
{
    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public long? OrderId { get; set; }

    /// <summary>获取或设置预约类型（官方 <c>booking_type</c>，1=指定时间发货，2=暂不发货）。</summary>
    [JsonPropertyName("booking_type")]
    public long? BookingType { get; set; }

    /// <summary>获取或设置用户预约的发货时间（官方 <c>booking_predict_time</c>）。</summary>
    [JsonPropertyName("booking_predict_time")]
    public long? BookingPredictTime { get; set; }

    /// <summary>获取或设置原承诺发货时间（官方 <c>original_delivery_time</c>）。</summary>
    [JsonPropertyName("original_delivery_time")]
    public long? OriginalDeliveryTime { get; set; }

    /// <summary>获取或设置订单创建时间（官方 <c>order_create_time</c>）。</summary>
    [JsonPropertyName("order_create_time")]
    public long? OrderCreateTime { get; set; }

    /// <summary>获取或设置用户预约时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置记录更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }
}
