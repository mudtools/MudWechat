// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>查询橱窗上机构商品详情请求（官方 <c>headsupplier/window/getdetail</c> 请求体）。
/// 商品定位走 <c>product_id</c> 与 <c>product_window_id</c> 二选一（官方推荐后者）。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetWindowProductDetailRequest
{
    /// <summary>获取或设置机构 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>获取或设置视频号 finder_id（官方 <c>finder_id</c>，未填 <c>openfinderid</c> 时必填，官方标注后续废弃）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置商品 id（官方 <c>product_id</c>，与 <c>product_window_id</c> 二选一）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置视频号 openfinderid（官方 <c>openfinderid</c>，与 <c>opentalentid</c> 二选一）。</summary>
    [JsonPropertyName("openfinderid")]
    public string? OpenFinderId { get; set; }

    /// <summary>获取或设置橱窗商品 ID（官方 <c>product_window_id</c>，与 <c>product_id</c> 二选一，官方推荐使用）。</summary>
    [JsonPropertyName("product_window_id")]
    public long? ProductWindowId { get; set; }

    /// <summary>获取或设置带货达人 opentalentid（官方 <c>opentalentid</c>，与 <c>openfinderid</c> 二选一）。</summary>
    [JsonPropertyName("opentalentid")]
    public string? OpenTalentId { get; set; }
}
