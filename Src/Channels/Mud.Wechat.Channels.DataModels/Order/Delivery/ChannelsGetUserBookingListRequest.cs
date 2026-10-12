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

/// <summary>获取用户预约发货列表（<c>order/userbooking/list</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetUserBookingListRequest
{
    /// <summary>获取或设置订单 ID 数组（官方 <c>order_id_list</c>）。</summary>
    [JsonPropertyName("order_id_list")]
    public List<long>? OrderIdList { get; set; }

    /// <summary>获取或设置用户预约类型（官方 <c>booking_type</c>，1=指定时间发货，2=暂不发货）。</summary>
    [JsonPropertyName("booking_type")]
    public long? BookingType { get; set; }

    /// <summary>获取或设置下单时间范围（官方 <c>time_range_order_create</c>，间隔不超过 7 天）。</summary>
    [JsonPropertyName("time_range_order_create")]
    public ChannelsOrderTimeRange TimeRangeOrderCreate { get; set; } = new();

    /// <summary>获取或设置预约发货时间范围（官方 <c>time_range_booking_predict</c>，间隔不超过 7 天）。</summary>
    [JsonPropertyName("time_range_booking_predict")]
    public ChannelsOrderTimeRange TimeRangeBookingPredict { get; set; } = new();

    /// <summary>获取或设置页码信息（官方 <c>page_info</c>）。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsOrderPageInfo? PageInfo { get; set; }
}
