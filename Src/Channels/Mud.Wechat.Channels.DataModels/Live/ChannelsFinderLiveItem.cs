// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>留资直播数据（官方 <c>item</c> 数组元素，<c>get_finder_live_data_list</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Live")]
public class ChannelsFinderLiveItem
{
    /// <summary>获取或设置直播唯一 id（官方 <c>export_id</c>）。</summary>
    [JsonPropertyName("export_id")]
    public string? ExportId { get; set; }

    /// <summary>获取或设置开播时间戳（官方 <c>live_start_time</c>，Unix 秒）。</summary>
    [JsonPropertyName("live_start_time")]
    public long? LiveStartTime { get; set; }

    /// <summary>获取或设置直播时长（官方 <c>live_duration_in_seconds</c>，秒）。</summary>
    [JsonPropertyName("live_duration_in_seconds")]
    public long? LiveDurationInSeconds { get; set; }

    /// <summary>获取或设置观看人数（官方 <c>total_audience_count</c>）。</summary>
    [JsonPropertyName("total_audience_count")]
    public int? TotalAudienceCount { get; set; }

    /// <summary>获取或设置喝彩次数（官方 <c>total_cheer_count</c>）。</summary>
    [JsonPropertyName("total_cheer_count")]
    public int? TotalCheerCount { get; set; }

    /// <summary>获取或设置分享次数（官方 <c>forward_count</c>）。</summary>
    [JsonPropertyName("forward_count")]
    public int? ForwardCount { get; set; }

    /// <summary>获取或设置评论条数（官方 <c>total_comment_count</c>）。</summary>
    [JsonPropertyName("total_comment_count")]
    public int? TotalCommentCount { get; set; }

    /// <summary>获取或设置人均观看时长（官方 <c>audiences_avg_seconds</c>，秒）。</summary>
    [JsonPropertyName("audiences_avg_seconds")]
    public long? AudiencesAvgSeconds { get; set; }

    /// <summary>获取或设置最高在线人数（官方 <c>max_online_count</c>）。</summary>
    [JsonPropertyName("max_online_count")]
    public int? MaxOnlineCount { get; set; }

    /// <summary>获取或设置新增粉丝（官方 <c>new_follow_count</c>）。</summary>
    [JsonPropertyName("new_follow_count")]
    public int? NewFollowCount { get; set; }

    /// <summary>获取或设置公众号新增粉丝（官方 <c>new_follow_count_biz</c>）。</summary>
    [JsonPropertyName("new_follow_count_biz")]
    public int? NewFollowCountBiz { get; set; }
}
