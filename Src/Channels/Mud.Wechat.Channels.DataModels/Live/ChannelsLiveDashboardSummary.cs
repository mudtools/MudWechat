// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>直播基础数据汇总（官方 <c>summary</c> 对象，<c>LiveDashboardData2</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsLiveDashboardSummary
{
    /// <summary>获取或设置观看人数（官方 <c>new_watch_uv</c>）。</summary>
    [JsonPropertyName("new_watch_uv")]
    public long? NewWatchUv { get; set; }

    /// <summary>获取或设置最大在线人数（官方 <c>max_online_watch_uv</c>）。</summary>
    [JsonPropertyName("max_online_watch_uv")]
    public long? MaxOnlineWatchUv { get; set; }

    /// <summary>获取或设置曝光人数（官方 <c>impression_uv</c>）。</summary>
    [JsonPropertyName("impression_uv")]
    public long? ImpressionUv { get; set; }

    /// <summary>获取或设置平均观看时长（官方 <c>average_watch_seconds_per_audience</c>，秒）。</summary>
    [JsonPropertyName("average_watch_seconds_per_audience")]
    public long? AverageWatchSecondsPerAudience { get; set; }

    /// <summary>获取或设置新增关注人数（官方 <c>new_follow_uv</c>）。</summary>
    [JsonPropertyName("new_follow_uv")]
    public long? NewFollowUv { get; set; }

    /// <summary>获取或设置新增粉丝团人数（官方 <c>new_fans_club_uv</c>）。</summary>
    [JsonPropertyName("new_fans_club_uv")]
    public long? NewFansClubUv { get; set; }

    /// <summary>获取或设置评论人数（官方 <c>comment_uv</c>）。</summary>
    [JsonPropertyName("comment_uv")]
    public long? CommentUv { get; set; }

    /// <summary>获取或设置打赏人数（官方 <c>reward_uv</c>）。</summary>
    [JsonPropertyName("reward_uv")]
    public long? RewardUv { get; set; }

    /// <summary>获取或设置分享直播间人数（官方 <c>sharing_uv</c>）。</summary>
    [JsonPropertyName("sharing_uv")]
    public long? SharingUv { get; set; }

    /// <summary>获取或设置热度（官方 <c>hot_quota</c>）。</summary>
    [JsonPropertyName("hot_quota")]
    public long? HotQuota { get; set; }
}
