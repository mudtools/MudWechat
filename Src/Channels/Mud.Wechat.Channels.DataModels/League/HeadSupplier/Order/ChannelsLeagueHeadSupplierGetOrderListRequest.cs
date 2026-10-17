// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取佣金单列表请求（官方 <c>headsupplier/order/list/get</c> 请求体）。</summary>
/// <remarks>
/// 官方约束：<c>create_time_range</c> 与 <c>update_time_range</c> <b>必填一个</b>且互斥（填其一则不填另一）；
/// <c>order_id</c> 填写后其余过滤参数无效。类型照抄官方参数表（<c>order_id</c> 为 string）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetOrderListRequest
{
    /// <summary>获取或设置佣金单所属小店 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置视频号 finder_id（官方 <c>finder_id</c>）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置单页佣金单数（官方 <c>page_size</c>，必填，不超过 30）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置佣金单创建时间范围（官方 <c>create_time_range</c>，与更新时间范围互斥、二者必填一个）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsLeagueHeadSupplierTimeRange? CreateTimeRange { get; set; }

    /// <summary>获取或设置佣金单更新时间范围（官方 <c>update_time_range</c>，与创建时间范围互斥、二者必填一个）。</summary>
    [JsonPropertyName("update_time_range")]
    public ChannelsLeagueHeadSupplierTimeRange? UpdateTimeRange { get; set; }

    /// <summary>获取或设置订单 ID（官方 <c>order_id</c>；填写后其他过滤参数无效）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置视频号 openfinderid（官方 <c>openfinderid</c>）。</summary>
    [JsonPropertyName("openfinderid")]
    public string? OpenFinderId { get; set; }

    /// <summary>获取或设置推客 appid（官方 <c>sharer_appid</c>，用于查询推客或达人平台产生的订单数据）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppid { get; set; }
}
