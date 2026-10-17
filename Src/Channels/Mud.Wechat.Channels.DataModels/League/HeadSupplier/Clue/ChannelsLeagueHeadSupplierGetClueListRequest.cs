// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>获取线索订单列表请求（官方 <c>headsupplier/clue/list/get</c> 请求体）。
/// <c>create_time_range</c> 自 2026-05-22 起为<b>必填</b>（此前可缺省）⇒ 建模为非空类型。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetClueListRequest
{
    /// <summary>获取或设置线索订单所属小店 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置推客 appid（官方 <c>sharer_appid</c>，用于查询推客或达人平台产生的订单数据）。</summary>
    [JsonPropertyName("sharer_appid")]
    public string? SharerAppid { get; set; }

    /// <summary>获取或设置创建时间范围（官方 <c>create_time_range</c>，必填，秒级时间戳）。</summary>
    [JsonPropertyName("create_time_range")]
    public ChannelsLeagueHeadSupplierTimeRange CreateTimeRange { get; set; } = new();

    /// <summary>获取或设置单页数量（官方 <c>page_size</c>，必填，不超过 100）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置翻页上下文（官方 <c>next_key</c>，顺序翻页时传上次返回值）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}
