// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.PlatformKf;

/// <summary>获取商家工单列表（<c>getmchticketlist</c>）请求体。</summary>
/// <remarks>全字段选填；不传 <c>ticket_state</c> 返回所有状态；默认按创建时间降序；limit 默认 10，最大建议不超过 50。</remarks>
[HttpJsonSerializable(SerializerClassName = "PlatformKf")]
public class ChannelsPlatformKfGetTicketListRequest
{
    /// <summary>获取或设置工单状态（官方 <c>ticket_state</c>，选填，见 <see cref="ChannelsPlatformKfTicketStates"/>；不传则返回所有状态）。</summary>
    [JsonPropertyName("ticket_state")]
    public int? TicketState { get; set; }

    /// <summary>获取或设置工单类型（官方 <c>ticket_class_type</c>，选填；当前仅支持「9:催发货」）。</summary>
    [JsonPropertyName("ticket_class_type")]
    public int? TicketClassType { get; set; }

    /// <summary>获取或设置搜索关键字（官方 <c>search_ticket_id</c>，选填；工单 id / 订单 id 任意其一）。</summary>
    [JsonPropertyName("search_ticket_id")]
    public string? SearchTicketId { get; set; }

    /// <summary>获取或设置查询起始时间（官方 <c>search_start_time</c>，选填，Unix 时间戳，秒——按工单创建时间过滤）。</summary>
    [JsonPropertyName("search_start_time")]
    public long? SearchStartTime { get; set; }

    /// <summary>获取或设置查询结束时间（官方 <c>search_end_time</c>，选填，Unix 时间戳，秒——按工单创建时间过滤）。</summary>
    [JsonPropertyName("search_end_time")]
    public long? SearchEndTime { get; set; }

    /// <summary>获取或设置排序方式（官方 <c>sort_order_type</c>，选填，见 <see cref="ChannelsPlatformKfSortOrderTypes"/>；默认按创建时间降序）。</summary>
    [JsonPropertyName("sort_order_type")]
    public int? SortOrderType { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>limit</c>，选填，默认 10，最大建议不超过 50）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置分页起始偏移量（官方 <c>start</c>，选填，从 0 开始）。</summary>
    [JsonPropertyName("start")]
    public int? Start { get; set; }
}
