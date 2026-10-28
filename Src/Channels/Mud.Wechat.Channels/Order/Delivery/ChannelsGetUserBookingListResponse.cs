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

/// <summary>获取用户预约发货列表（<c>order/userbooking/list</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetUserBookingListResponse : ChannelsResponse
{
    /// <summary>获取或设置预约记录列表（官方 <c>record_list</c>）。</summary>
    [JsonPropertyName("record_list")]
    public List<ChannelsUserBookingRecord>? RecordList { get; set; }

    /// <summary>获取或设置下一页信息（官方 <c>next_page</c>）。</summary>
    [JsonPropertyName("next_page")]
    public ChannelsOrderPageInfo? NextPage { get; set; }

    /// <summary>获取或设置记录总数（官方 <c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public long? Total { get; set; }
}
