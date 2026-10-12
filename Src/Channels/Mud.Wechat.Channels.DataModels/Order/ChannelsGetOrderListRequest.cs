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

/// <summary>获取订单列表（<c>order/list/get</c>）请求体。</summary>
/// <remarks>官方要求 create_time_range / update_time_range 两个时间范围<b>至少填一个</b>（守卫锁定）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Order")]
public class ChannelsGetOrderListRequest
{
    /// <summary>获取或设置订单创建时间范围（官方 <c>create_time_range</c>）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsOrderTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置订单更新时间范围（官方 <c>update_time_range</c>）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsOrderTimeRange? UpdateTimeRange { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>status</c>，枚举值见 <see cref="ChannelsOrderStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置买家身份标识（官方 <c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，上一页请求返回）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
