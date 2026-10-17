// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Promoter;

/// <summary>短视频推广信息（官方 <c>feed_list</c> 数组项，<c>promoter/get_feed_promotion_info</c> 与 <c>promoter/get_shop_feed_promotion_info</c> 响应共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Promoter")]
public class ChannelsPromoterFeedTokenInfo
{
    /// <summary>获取或设置短视频 id（官方 <c>export_id</c>）。</summary>
    [JsonPropertyName("export_id")]
    public string? ExportId { get; set; }

    /// <summary>获取或设置内嵌短视频的卡片信息（官方 <c>feed_token</c>）。</summary>
    [JsonPropertyName("feed_token")]
    public string? FeedToken { get; set; }

    /// <summary>获取或设置推广推客信息（官方 <c>promoter_share_link</c>）。</summary>
    [JsonPropertyName("promoter_share_link")]
    public string? PromoterShareLink { get; set; }
}
