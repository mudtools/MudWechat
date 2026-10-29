// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>查询橱窗上机构商品列表请求（官方 <c>headsupplier/window/getall</c> 请求体）。
/// 官方按 <c>offset</c> 偏移翻页（非 next_key），与机构侧其余列表接口的翻页形态不同。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierGetWindowListRequest
{
    /// <summary>获取或设置视频号 finder_id（官方 <c>finder_id</c>，未填 <c>openfinderid</c> 时必填，官方标注后续废弃）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置起始位置（官方 <c>offset</c>，必填，从 0 开始）。</summary>
    [JsonPropertyName("offset")]
    public long Offset { get; set; }

    /// <summary>获取或设置每页数量（官方 <c>page_size</c>，默认 100、最大 500）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>获取或设置是否返回橱窗上机构商品总数（官方 <c>need_total_num</c>，默认 <c>false</c>）。</summary>
    [JsonPropertyName("need_total_num")]
    public bool? NeedTotalNum { get; set; }

    /// <summary>获取或设置视频号 openfinderid（官方 <c>openfinderid</c>，与 <c>opentalentid</c> 二选一）。</summary>
    [JsonPropertyName("openfinderid")]
    public string? OpenFinderId { get; set; }

    /// <summary>获取或设置是否查询达人全部橱窗商品（官方 <c>is_get_all</c>，默认 <c>false</c>；为 <c>true</c> 时返回橱窗所有商品而不限本机构商品）。</summary>
    [JsonPropertyName("is_get_all")]
    public bool? IsGetAll { get; set; }

    /// <summary>获取或设置带货达人 opentalentid（官方 <c>opentalentid</c>，与 <c>openfinderid</c> 二选一）。</summary>
    [JsonPropertyName("opentalentid")]
    public string? OpenTalentId { get; set; }
}
