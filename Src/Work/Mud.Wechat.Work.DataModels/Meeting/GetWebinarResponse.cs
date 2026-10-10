// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取网络研讨会详情响应体（<c>/cgi-bin/meeting/webinar/get</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：
/// ① 参数表将主题字段列为 <c>subject</c>，官方响应示例为 <c>title</c>（与创建响应一致），本模型以示例为准；
/// ② <see cref="Status"/> 为字符串枚举（MEETING_STATE_*，官方契约如此，非整数）；
/// ③ 响应 <c>media_setting</c> 的入会静音字段官方示例作 <c>mute_enable_join</c>（参数表作 enable_enter_mute），
/// 详见 <see cref="WebinarMediaSettingInfo"/>；
/// ④ <see cref="DisplayNumberOfAttendees"/> 响应参数表标注 string、示例为数字，本模型按整数承载。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetWebinarResponse : WechatWorkResponse
{
    /// <summary>获取或设置网络研讨会主题（官方响应示例字段名 title，参数表作 subject）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置网络研讨会 ID。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置网络研讨会的会议号。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>
    /// 获取或设置当前会议状态（官方为字符串枚举）。
    /// <para>取值：MEETING_STATE_INVALID - 非法或未知的会议状态；MEETING_STATE_INIT - 会议待开始；
    /// MEETING_STATE_CANCELLED - 会议已取消；MEETING_STATE_STARTED - 会议已开始；
    /// MEETING_STATE_ENDED - 会议已删除；MEETING_STATE_ABOUT_TO_START - 会议即将开始（已过预定开始时间且未到结束时间、会中无人）；
    /// MEETING_STATE_RECYCLED - 会议已回收（已过预定结束时间且会中无人，会议号被后台回收）。</para>
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>获取或设置主办方名称。</summary>
    [JsonPropertyName("sponsor")]
    public string? Sponsor { get; set; }

    /// <summary>获取或设置会议开始时间戳（单位秒，字符串形态）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>获取或设置会议结束时间戳（单位秒，字符串形态）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>获取或设置观众观看限制类型：0 - 公开；1 - 报名；2 - 密码。</summary>
    [JsonPropertyName("admission_type")]
    public int? AdmissionType { get; set; }

    /// <summary>获取或设置主持人的成员 ID 列表（详见 <see cref="WebinarHostInfo"/>）。</summary>
    [JsonPropertyName("hosts")]
    public List<WebinarHostInfo>? Hosts { get; set; }

    /// <summary>获取或设置观众观看密码（4~6 位数字）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>获取或设置封面图片 URL（需开启活动页配置）。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>获取或设置网络研讨会描述详情（仅支持纯文本，1~5000 位字符长度）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置是否开启通过邀请链接自动成为嘉宾：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_guest_invite_link")]
    public bool? EnableGuestInviteLink { get; set; }

    /// <summary>获取或设置观众入会链接。</summary>
    [JsonPropertyName("audience_join_link")]
    public string? AudienceJoinLink { get; set; }

    /// <summary>获取或设置嘉宾入会链接（enable_guest_invite_link = true 时通过此链接入会的成员自动成为嘉宾）。</summary>
    [JsonPropertyName("guest_join_link")]
    public string? GuestJoinLink { get; set; }

    /// <summary>获取或设置媒体参数配置（详见 <see cref="WebinarMediaSettingInfo"/>）。</summary>
    [JsonPropertyName("media_setting")]
    public WebinarMediaSettingInfo? MediaSetting { get; set; }

    /// <summary>获取或设置是否开启问答：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_qa")]
    public bool? EnableQa { get; set; }

    /// <summary>获取或设置人工审核链接（enable_manual_check 开启后返回该字段）。</summary>
    [JsonPropertyName("manual_check_link")]
    public string? ManualCheckLink { get; set; }

    /// <summary>获取或设置人工审核密码（enable_manual_check 开启后返回该字段）。</summary>
    [JsonPropertyName("manual_check_password")]
    public string? ManualCheckPassword { get; set; }

    /// <summary>获取或设置活动页开启配置（查询时返回默认值 true）。</summary>
    [JsonPropertyName("activity_page")]
    public bool? ActivityPage { get; set; }

    /// <summary>获取或设置活动页展示已报名或已预约人数：0 - 不展示；1 - 展示（响应参数表标注 string、示例为数字，本模型按整数承载）。</summary>
    [JsonPropertyName("display_number_of_attendees")]
    public int? DisplayNumberOfAttendees { get; set; }

    /// <summary>获取或设置允许观众观看回放：true - 允许；false - 不允许（开启时必须开启云录制）。</summary>
    [JsonPropertyName("playback_for_audience")]
    public bool? PlaybackForAudience { get; set; }

    /// <summary>获取或设置回放地址。</summary>
    [JsonPropertyName("playback_url")]
    public string? PlaybackUrl { get; set; }

    /// <summary>获取或设置是否开启准备模式：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("preparation_mode")]
    public bool? PreparationMode { get; set; }

    /// <summary>
    /// 获取或设置暖场图片地址。
    /// <para>暖场图片与暖场视频只能选择一个，同时传入时以图片为准；推荐 1280*720 尺寸，支持 png/jpg 格式，大小不超过 5M；会议开始前 15 分钟进入的观众可观看暖场，仅支持 3.3.0 及以上版本客户端观看。</para>
    /// </summary>
    [JsonPropertyName("warm_up_picture")]
    public string? WarmUpPicture { get; set; }

    /// <summary>获取或设置暖场视频地址（推荐 1280*720 尺寸，支持 mp4 格式，大小不超过 1G）。</summary>
    [JsonPropertyName("warm_up_video")]
    public string? WarmUpVideo { get; set; }

    /// <summary>获取或设置允许参会者在暖场中邀请成员（默认允许）：true - 允许；false - 不允许。</summary>
    [JsonPropertyName("allow_attendees_invite_others")]
    public bool? AllowAttendeesInviteOthers { get; set; }
}
