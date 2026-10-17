// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>推客推广参数（官方 <c>commission_info.promotion_info</c>）。
/// 其内层 <c>finder_info</c> / <c>talent_info</c> 与外层同名字段<b>字段集不同</b>（无 ratio/amount）⇒ 各建一支类型。</summary>
[HttpJsonSerializable(SerializerClassName = "League")]
public class ChannelsLeagueHeadSupplierOrderPromotionInfo
{
    /// <summary>获取或设置推客推广类型（官方 <c>channel_type</c>，见 <see cref="ChannelsLeagueHeadSupplierPromotionChannelTypes"/>）。</summary>
    [JsonPropertyName("channel_type")]
    public int? ChannelType { get; set; }

    /// <summary>获取或设置推广渠道来自视频号时的视频号信息（官方 <c>finder_info</c>）。</summary>
    [JsonPropertyName("finder_info")]
    public ChannelsLeagueHeadSupplierOrderPromotionFinderInfo? FinderInfo { get; set; }

    /// <summary>获取或设置推广渠道来自公众号时的公众号信息（官方 <c>mp_info</c>）。</summary>
    [JsonPropertyName("mp_info")]
    public ChannelsLeagueHeadSupplierOrderPromotionMpInfo? MpInfo { get; set; }

    /// <summary>获取或设置推广渠道来自带货内容时的带货助手信息（官方 <c>talent_info</c>）。</summary>
    [JsonPropertyName("talent_info")]
    public ChannelsLeagueHeadSupplierOrderPromotionTalentInfo? TalentInfo { get; set; }

    /// <summary>获取或设置机构自定义关联参数（官方 <c>custom_token</c>，针对推客带货商品卡分享直接成交场景，2026-03-12 新增）。</summary>
    [JsonPropertyName("custom_token")]
    public string? CustomToken { get; set; }
}
