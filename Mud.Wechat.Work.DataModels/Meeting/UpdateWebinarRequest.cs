// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 修改网络研讨会请求体（<c>/cgi-bin/meeting/webinar/update</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档陷阱：参数表将 <c>media_setting</c> 标注为 object[]，官方请求示例为单个对象，本模型以单个对象承载；
/// <see cref="StartTime"/> / <see cref="EndTime"/> 参数表与示例均为字符串形态的时间戳（单位秒），本模型按字符串承载。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class UpdateWebinarRequest
{
    /// <summary>获取或设置网络研讨会 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置网络研讨会主题（官方必填，1~255 位字符长度）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置主办方名称（1~40 位字符长度）。</summary>
    [JsonPropertyName("sponsor")]
    public string? Sponsor { get; set; }

    /// <summary>获取或设置会议开始时间戳（官方必填，单位秒，字符串形态；不能少于当前时间戳半小时以上）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>获取或设置会议结束时间戳（官方必填，单位秒，字符串形态）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>获取或设置观众观看限制类型（官方必填）：0 - 公开；1 - 报名；2 - 密码。</summary>
    [JsonPropertyName("admission_type")]
    public int? AdmissionType { get; set; }

    /// <summary>获取或设置主持人的成员 ID 列表（默认为网络研讨会管理员 admin_userid；修改时传入会覆盖原有设置，详见 <see cref="WebinarHostInfo"/>）。</summary>
    [JsonPropertyName("hosts")]
    public List<WebinarHostInfo>? Hosts { get; set; }

    /// <summary>获取或设置观众观看密码（4~6 位数字；<see cref="AdmissionType"/> = 2 时必传且仅此时生效）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>获取或设置封面图片 URL（仅支持 PNG/JPEG，分辨率大于 640*360、推荐 1280*720，5 MB 以内；需开启活动页配置；异步上传，可订阅「素材上传结果」事件获得通知）。</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>获取或设置网络研讨会描述详情（仅支持纯文本，1~5000 位字符长度；需开启活动页配置）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置是否开启通过邀请链接自动成为嘉宾（默认 false）：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_guest_invite_link")]
    public bool? EnableGuestInviteLink { get; set; }

    /// <summary>获取或设置媒体参数配置（详见 <see cref="WebinarMediaSetting"/>）。</summary>
    [JsonPropertyName("media_setting")]
    public WebinarMediaSetting? MediaSetting { get; set; }

    /// <summary>获取或设置是否开启问答（默认 true）：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_qa")]
    public bool? EnableQa { get; set; }

    /// <summary>获取或设置聊天敏感词列表（最多 50 个，单个限制 10 个中文字符长度）。</summary>
    [JsonPropertyName("sensitive_words")]
    public List<string>? SensitiveWords { get; set; }

    /// <summary>获取或设置是否开启人工审核（默认 false）：true - 开启；false - 不开启。</summary>
    [JsonPropertyName("enable_manual_check")]
    public bool? EnableManualCheck { get; set; }

    /// <summary>获取或设置活动页开启配置（默认开启）：true - 开启活动页；false - 不开启活动页。</summary>
    [JsonPropertyName("activity_page")]
    public bool? ActivityPage { get; set; }

    /// <summary>获取或设置活动页展示已报名或已预约人数（默认开启）：0 - 不展示；1 - 展示（需开启活动页配置）。</summary>
    [JsonPropertyName("display_number_of_attendees")]
    public int? DisplayNumberOfAttendees { get; set; }

    /// <summary>获取或设置允许观众观看回放（官方必填，默认值为 false；开启时必须开启云录制，即 auto_record_type 必须为 cloud）。</summary>
    [JsonPropertyName("playback_for_audience")]
    public bool? PlaybackForAudience { get; set; }

    /// <summary>获取或设置是否开启准备模式：true - 开启；false - 关闭。</summary>
    [JsonPropertyName("preparation_mode")]
    public bool? PreparationMode { get; set; }
}
