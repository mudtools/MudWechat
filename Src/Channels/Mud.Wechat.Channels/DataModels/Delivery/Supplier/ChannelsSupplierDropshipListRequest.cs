// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Delivery;

/// <summary>
/// 供货商视角拉取代发单列表（<c>dropship/supplier/list</c>）请求。
/// </summary>
/// <remarks>
/// 与商家侧 <see cref="ChannelsDropshipListRequest"/> 的官方参数表差异：①本视角过滤商家为 <c>shop_appid</c>
/// （商家侧为 <c>supplier_appid</c>）；②本页 <c>order_id</c> 为 <b>string</b>（商家侧参数表为 number）⇒ 各建一型，禁止互改。
/// 响应与商家侧同形，复用 <see cref="ChannelsDropshipOrderListResponse"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Delivery")]
public class ChannelsSupplierDropshipListRequest
{
    /// <summary>获取或设置订单号（官方 <c>order_id</c>，本视角为 string，非必填，可通过订单号反查代发单）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置商家 appid（官方 <c>shop_appid</c>）。</summary>
    [JsonPropertyName("shop_appid")]
    public string? ShopAppid { get; set; }

    /// <summary>获取或设置代发单状态（官方 <c>status</c>：100 待发货 / 199 部分发货 / 200 已发货 / 300 已取消）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置代发单创建时间范围（官方 <c>create_time_range</c>，秒级时间戳，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsDeliveryTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>page_info</c>，上一页响应返回）。推荐用 <c>next_key</c> 翻页代替 <c>offset</c>。</summary>
    [JsonPropertyName("page_info")]
    public ChannelsDeliveryPageInfo? PageInfo { get; set; }

    /// <summary>获取或设置代发单更新时间范围（官方 <c>update_time_range</c>，秒级时间戳，起止距离不可超过 7 天）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsDeliveryTimeRange? UpdateTimeRange { get; set; }
}
