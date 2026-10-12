// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Compass;

/// <summary>带货达人条目（官方 <c>finder_list</c> 数组元素，<c>getshopfinderlist</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Compass")]
public class ChannelsCompassShopFinderItem
{
    /// <summary>获取或设置视频号 id（官方 <c>finder_id</c>）。</summary>
    [JsonPropertyName("finder_id")]
    public string? FinderId { get; set; }

    /// <summary>获取或设置视频号昵称（官方 <c>finder_nickname</c>）。</summary>
    [JsonPropertyName("finder_nickname")]
    public string? FinderNickname { get; set; }

    /// <summary>获取或设置成交数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public ChannelsCompassDealData? Data { get; set; }

    /// <summary>获取或设置带货者 id（官方 <c>talent_id</c>）。</summary>
    [JsonPropertyName("talent_id")]
    public string? TalentId { get; set; }

    /// <summary>获取或设置带货者昵称（官方 <c>talent_nickname</c>）。</summary>
    [JsonPropertyName("talent_nickname")]
    public string? TalentNickname { get; set; }

    /// <summary>获取或设置公众号 id（官方 <c>mp_id</c>）。</summary>
    [JsonPropertyName("mp_id")]
    public string? MpId { get; set; }

    /// <summary>获取或设置公众号昵称（官方 <c>mp_nickname</c>）。</summary>
    [JsonPropertyName("mp_nickname")]
    public string? MpNickname { get; set; }
}
