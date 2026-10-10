// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 创建网络研讨会响应体（<c>/cgi-bin/meeting/webinar/create</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CreateWebinarResponse : WechatWorkResponse
{
    /// <summary>获取或设置网络研讨会主题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置网络研讨会 ID。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置网络研讨会的会议号。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置会议开始时间戳（单位秒，字符串形态）。</summary>
    [JsonPropertyName("start_time")]
    public string? StartTime { get; set; }

    /// <summary>获取或设置会议结束时间戳（单位秒，字符串形态）。</summary>
    [JsonPropertyName("end_time")]
    public string? EndTime { get; set; }

    /// <summary>获取或设置观众观看限制类型：0 - 公开；1 - 报名；2 - 密码。</summary>
    [JsonPropertyName("admission_type")]
    public int? AdmissionType { get; set; }

    /// <summary>获取或设置观众观看密码（4~6 位数字）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>获取或设置观众入会链接。</summary>
    [JsonPropertyName("audience_join_link")]
    public string? AudienceJoinLink { get; set; }

    /// <summary>
    /// 获取或设置嘉宾入会链接。
    /// <para>如果 enable_guest_invite_link = true，通过此链接入会的成员自动成为嘉宾，否则和观众入会链接功能一致。</para>
    /// </summary>
    [JsonPropertyName("guest_join_link")]
    public string? GuestJoinLink { get; set; }

    /// <summary>获取或设置人工审核链接（enable_manual_check 开启后返回该字段）。</summary>
    [JsonPropertyName("manual_check_link")]
    public string? ManualCheckLink { get; set; }

    /// <summary>获取或设置人工审核密码（enable_manual_check 开启后返回该字段）。</summary>
    [JsonPropertyName("manual_check_password")]
    public string? ManualCheckPassword { get; set; }
}
