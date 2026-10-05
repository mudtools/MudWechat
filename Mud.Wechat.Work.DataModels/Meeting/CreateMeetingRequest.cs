// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 创建预约会议请求体（<c>/cgi-bin/meeting/create</c>；三种应用类型请求形态一致）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CreateMeetingRequest
{
    /// <summary>获取或设置会议管理员 userid（官方必填；发起人和企业内部参与人必须在应用可见范围内）。</summary>
    [JsonPropertyName("admin_userid")]
    public string? AdminUserid { get; set; }

    /// <summary>获取或设置会议标题（官方必填；最多 40 字节或 20 个 utf8 字符）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置会议开始时间的 Unix 时间戳（官方必填；需大于当前时间）。</summary>
    [JsonPropertyName("meeting_start")]
    public long? MeetingStart { get; set; }

    /// <summary>获取或设置会议持续时长（秒，官方必填；最小 300 秒、最大 86399 秒）。</summary>
    [JsonPropertyName("meeting_duration")]
    public int? MeetingDuration { get; set; }

    /// <summary>获取或设置会议描述（最多 500 字节或 utf8 字符）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置会议地点（最多 128 个字符）。</summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>获取或设置授权方安装的应用 agentid（仅旧的第三方多应用套件需要填）。</summary>
    [JsonPropertyName("agentid")]
    public int? Agentid { get; set; }

    /// <summary>获取或设置邀请参会成员对象（任何 userid 不合法或不在应用可见范围内将直接报错）。</summary>
    [JsonPropertyName("invitees")]
    public MeetingInvitees? Invitees { get; set; }

    /// <summary>
    /// 获取或设置会议所属日历 ID（不多于 64 字节，须为 access_token 对应应用创建的日历）。
    /// <para>不填则插入参与者的默认日历；第三方应用必须指定 cal_id。</para>
    /// </summary>
    [JsonPropertyName("cal_id")]
    public string? CalId { get; set; }

    /// <summary>获取或设置会议配置对象（入会密码/等候室/提醒方式等，详见 <see cref="MeetingSettings"/>）。</summary>
    [JsonPropertyName("settings")]
    public MeetingSettings? Settings { get; set; }

    /// <summary>获取或设置重复会议相关配置对象（详见 <see cref="MeetingReminders"/>）。</summary>
    [JsonPropertyName("reminders")]
    public MeetingReminders? Reminders { get; set; }
}
