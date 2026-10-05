// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议详情响应体（<c>/cgi-bin/meeting/get_info</c>；自建应用与第三方应用响应形态一致）。
/// </summary>
/// <remarks>
/// <para>官方限制：只能拉取该应用创建的会议；快速会议仅返回已参与成员列表。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingInfoResponse : WechatWorkResponse
{
    /// <summary>获取或设置会议管理员的 userid。</summary>
    [JsonPropertyName("admin_userid")]
    public string? AdminUserid { get; set; }

    /// <summary>获取或设置会议标题（最大 60 字节）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置会议开始时间的 Unix 时间戳。</summary>
    [JsonPropertyName("meeting_start")]
    public long? MeetingStart { get; set; }

    /// <summary>获取或设置会议持续时长（秒）。</summary>
    [JsonPropertyName("meeting_duration")]
    public int? MeetingDuration { get; set; }

    /// <summary>获取或设置会议描述（最大 600 字节）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置会议地点（最多 128 个字符）。</summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>获取或设置发起人所在部门。</summary>
    [JsonPropertyName("main_department")]
    public int? MainDepartment { get; set; }

    /// <summary>获取或设置会议状态：1 - 待开始；2 - 会议中；3 - 已结束；4 - 已取消；5 - 已过期。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置应用 agentid。</summary>
    [JsonPropertyName("agentid")]
    public int? Agentid { get; set; }

    /// <summary>获取或设置会议号。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置入会链接。</summary>
    [JsonPropertyName("meeting_link")]
    public string? MeetingLink { get; set; }

    /// <summary>获取或设置关联日历 ID。</summary>
    [JsonPropertyName("cal_id")]
    public string? CalId { get; set; }

    /// <summary>获取或设置会议成员对象（含企业内部成员与会中参会的外部联系人，详见 <see cref="MeetingAttendees"/>）。</summary>
    [JsonPropertyName("attendees")]
    public MeetingAttendees? Attendees { get; set; }

    /// <summary>获取或设置会议配置对象（详见 <see cref="MeetingSettings"/>）。</summary>
    [JsonPropertyName("settings")]
    public MeetingSettings? Settings { get; set; }

    /// <summary>获取或设置重复会议相关配置对象（详见 <see cref="MeetingReminders"/>）。</summary>
    [JsonPropertyName("reminders")]
    public MeetingReminders? Reminders { get; set; }
}
