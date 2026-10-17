// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>添加机构商品到橱窗请求（官方 <c>headsupplier/window/add</c> 请求体）。
/// 定位达人走「<c>openfinderid</c> 与 <c>opentalentid</c> 二选一」；<c>finder_id</c> 官方标注后续废弃。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierAddWindowRequest
{
    /// <summary>获取或设置视频号 finder_id（官方 <c>finder_id</c>，未填 <c>openfinderid</c> 时必填，官方标注后续废弃）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置原始商品 ID（官方 <c>product_id</c>，必填）。</summary>
    [JsonPropertyName("product_id")]
    public long ProductId { get; set; }

    /// <summary>获取或设置视频号 openfinderid（官方 <c>openfinderid</c>，操作达人视频号橱窗时必填，与 <c>opentalentid</c> 二选一）。</summary>
    [JsonPropertyName("openfinderid")]
    public string? OpenFinderId { get; set; }

    /// <summary>获取或设置是否在个人橱窗页隐藏（官方 <c>is_hide_for_window</c>，默认 <c>false</c>）。</summary>
    [JsonPropertyName("is_hide_for_window")]
    public bool? IsHideForWindow { get; set; }

    /// <summary>获取或设置联盟商品链接（官方 <c>product_link</c>，机构类商品必填，取合作商品列表 / 机构配置达人佣金计划列表的 <c>head_supplier_item_link</c>）。</summary>
    [JsonPropertyName("product_link")]
    public string? ProductLink { get; set; }

    /// <summary>获取或设置带货达人 opentalentid（官方 <c>opentalentid</c>，操作达人带货助手橱窗时必填，与 <c>openfinderid</c> 二选一）。</summary>
    [JsonPropertyName("opentalentid")]
    public string? OpenTalentId { get; set; }
}
