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
/// <para>
/// <see cref="MeetingType"/> / <see cref="Guests"/> / <see cref="HasVote"/> / <see cref="SubMeetings"/> /
/// <see cref="HasMoreSubMeeting"/> / <see cref="RemainSubMeetings"/> / <see cref="CurrentSubMeetingid"/> /
/// <see cref="SubRepeatList"/> 仅预约会议高级管理文档页（98149，官方仅自建应用）声明。
/// </para>
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

    /// <summary>
    /// 获取或设置会议类型：0 - 一次性会议；1 - 周期性会议；2 - 微信专属会议；3 - Rooms 投屏会议；5 - 个人会议号会议；6 - 网络研讨会（仅预约会议高级管理文档页声明该字段）。
    /// </summary>
    [JsonPropertyName("meeting_type")]
    public int? MeetingType { get; set; }

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

    /// <summary>获取或设置会议嘉宾列表（详见 <see cref="MeetingGuest"/>；仅预约会议高级管理文档页声明该字段）。</summary>
    [JsonPropertyName("guests")]
    public List<MeetingGuest>? Guests { get; set; }

    /// <summary>
    /// 获取或设置是否存在投票（仅预约会议高级管理文档页声明该字段）。
    /// <para>官方限制：会议创建人和主持人才有权限查询。</para>
    /// </summary>
    [JsonPropertyName("has_vote")]
    public bool? HasVote { get; set; }

    /// <summary>获取或设置周期性子会议列表（详见 <see cref="MeetingSubMeeting"/>；仅预约会议高级管理文档页声明该字段）。</summary>
    [JsonPropertyName("sub_meetings")]
    public List<MeetingSubMeeting>? SubMeetings { get; set; }

    /// <summary>获取或设置是否还有更多子会议特例：0 - 无更多；1 - 有更多子会议特例（仅预约会议高级管理文档页声明该字段）。</summary>
    [JsonPropertyName("has_more_sub_meeting")]
    public int? HasMoreSubMeeting { get; set; }

    /// <summary>获取或设置剩余子会议场数（仅预约会议高级管理文档页声明该字段）。</summary>
    [JsonPropertyName("remain_sub_meetings")]
    public int? RemainSubMeetings { get; set; }

    /// <summary>获取或设置当前子会议 ID（进行中/即将开始；仅预约会议高级管理文档页声明该字段）。</summary>
    [JsonPropertyName("current_sub_meetingid")]
    public string? CurrentSubMeetingid { get; set; }

    /// <summary>
    /// 获取或设置周期性会议分段信息列表（详见 <see cref="MeetingSubRepeatInfo"/>；仅预约会议高级管理文档页声明该字段）。
    /// <para>企业微信客户端可对周期性会议中某一场子会议执行「修改此会议」和「修改此及后续会议」，修改后会议可能被分裂成不同分段，每个分段有不同的重复规则，通过该字段获取所有分段信息。</para>
    /// </summary>
    [JsonPropertyName("sub_repeat_list")]
    public List<MeetingSubRepeatInfo>? SubRepeatList { get; set; }
}
