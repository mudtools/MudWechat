// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.Living;

/// <summary>
/// 直播信息（<c>get_living_info</c> 响应 <c>living_info</c> 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class LivingInfo
{
    /// <summary>
    /// 获取或设置直播主题。
    /// </summary>
    [JsonPropertyName("theme")]
    public string? Theme { get; set; }

    /// <summary>
    /// 获取或设置直播开始时间戳。
    /// </summary>
    [JsonPropertyName("living_start")]
    public long? LivingStart { get; set; }

    /// <summary>
    /// 获取或设置直播时长（单位为秒）。
    /// </summary>
    [JsonPropertyName("living_duration")]
    public long? LivingDuration { get; set; }

    /// <summary>
    /// 获取或设置主播的 userid。
    /// </summary>
    [JsonPropertyName("anchor_userid")]
    public string? AnchorUserId { get; set; }

    /// <summary>
    /// 获取或设置直播范围（若包含班级群则优先返回班级对应班级的 partyid，否则直接返回群名称）。
    /// </summary>
    [JsonPropertyName("living_range")]
    public LivingRange? LivingRange { get; set; }

    /// <summary>
    /// 获取或设置观看直播总人数。
    /// </summary>
    [JsonPropertyName("viewer_num")]
    public long? ViewerNum { get; set; }

    /// <summary>
    /// 获取或设置评论数。
    /// </summary>
    [JsonPropertyName("comment_num")]
    public long? CommentNum { get; set; }

    /// <summary>
    /// 获取或设置是否开启回放（1：表示开启；0：表示关闭）。
    /// </summary>
    [JsonPropertyName("open_replay")]
    public int? OpenReplay { get; set; }

    /// <summary>
    /// 获取或设置推流地址（仅直播类型为活动直播且状态是待开播时返回该字段）。
    /// </summary>
    [JsonPropertyName("push_stream_url")]
    public string? PushStreamUrl { get; set; }

    /// <summary>
    /// 获取或设置当前在线观看人数。
    /// </summary>
    [JsonPropertyName("online_count")]
    public long? OnlineCount { get; set; }

    /// <summary>
    /// 获取或设置直播预约人数。
    /// </summary>
    [JsonPropertyName("subscribe_count")]
    public long? SubscribeCount { get; set; }
}
