// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「订单主文档」DTO（order/get、order/list/get、order/search、order/price/update、
// order/address/update、order/addressmodify/*、order/merchantnotes/update、order/presentnote/add、
// order/presentsuborder/get、order/preshipmentchangesku/*、order/blindboxitem/set 等核心端点）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>时间范围（官方 <c>order/list/get</c> 的 <c>create_time_range</c>/<c>update_time_range</c> 与
/// <c>userbooking/list</c> 的 <c>time_range_order_create</c>/<c>time_range_booking_predict</c> 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsOrderTimeRange
{
    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，秒级时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，秒级时间戳，与 start_time 间隔不可超过 7 天）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
