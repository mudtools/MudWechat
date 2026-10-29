// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>内容力在播指标（官方 <c>on_air</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveComparisonOnAir
{
    /// <summary>获取或设置曝光有效 CTR（万分比）（官方 <c>recommend_effective_new_watch_2_uv_over_impression_uv</c>）。</summary>
    [JsonPropertyName("recommend_effective_new_watch_2_uv_over_impression_uv")]
    public ChannelsLiveOnAirIndexItem? RecommendEffectiveNewWatch2UvOverImpressionUv { get; set; }

    /// <summary>获取或设置人均看播时长（官方 <c>average_watch_seconds</c>）。</summary>
    [JsonPropertyName("average_watch_seconds")]
    public ChannelsLiveOnAirIndexItem? AverageWatchSeconds { get; set; }

    /// <summary>获取或设置评论率（万分比）（官方 <c>comment_uv_over_new_watch_uv</c>）。</summary>
    [JsonPropertyName("comment_uv_over_new_watch_uv")]
    public ChannelsLiveOnAirIndexItem? CommentUvOverNewWatchUv { get; set; }

    /// <summary>获取或设置点赞率（万分比）（官方 <c>like_uv_over_new_watch_uv</c>）。</summary>
    [JsonPropertyName("like_uv_over_new_watch_uv")]
    public ChannelsLiveOnAirIndexItem? LikeUvOverNewWatchUv { get; set; }
}
