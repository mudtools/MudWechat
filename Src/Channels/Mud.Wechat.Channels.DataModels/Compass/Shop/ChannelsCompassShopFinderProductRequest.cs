// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>
/// 罗盘带货达人商品维度请求体（<c>getshopfinderproductlist</c> / <c>getshopfinderproductoverall</c>）。
/// </summary>
/// <remarks>共用裁决：两端点请求体字段集完全一致（<c>ds</c> 必填 + 三选一身份 id）⇒ 共用本 DTO。</remarks>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderProductRequest
{
    /// <summary>获取或设置日期（官方 <c>ds</c>，必填，格式 YYYYMMDD）。</summary>
    [JsonPropertyName("ds")]
    public string Ds { get; set; } = string.Empty;

    /// <summary>获取或设置视频号 id（官方 <c>finder_id</c>，与 talent_id、mp_id 三者必填其一）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置达人号 id（官方 <c>talent_id</c>，与 finder_id、mp_id 三者必填其一）。</summary>
    [JsonPropertyName("talent_id")]
    public string? TalentId { get; set; }

    /// <summary>获取或设置公众号 id（官方 <c>mp_id</c>，与 finder_id、talent_id 三者必填其一）。</summary>
    [JsonPropertyName("mp_id")]
    public string? MpId { get; set; }
}
