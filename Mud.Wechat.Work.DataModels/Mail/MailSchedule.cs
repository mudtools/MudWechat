// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 日程/会议公共数据（官方 schedule 结构，发送日程邮件与发送会议邮件共用；
/// 会议相关的基本设置也放在本结构里，会议设置放在 <see cref="MailMeeting"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailSchedule
{
    /// <summary>获取或设置日程/会议 id（官方 schedule_id；修改/取消日程或会议时必须带上）。</summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }

    /// <summary>
    /// 获取或设置日程/会议方法（官方 method）：
    /// request - 请求（不传 schedule_id 时是创建日程/会议，传了是修改日程/会议）；
    /// cancel - 取消日程/会议（必须带上 schedule_id）；默认为 request。
    /// </summary>
    [JsonPropertyName("method")]
    public string? Method { get; set; }

    /// <summary>获取或设置地点（官方 location）。</summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>获取或设置日程/会议开始时间，Unix 时间戳（官方 start_time，必填）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置日程/会议结束时间，Unix 时间戳（官方 end_time，必填）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置重复和提醒相关字段（官方选填，<see cref="MailReminders"/>）。</summary>
    [JsonPropertyName("reminders")]
    public MailReminders? Reminders { get; set; }

    /// <summary>
    /// 获取或设置管理员 userid 列表（官方 schedule_admins，<see cref="MailRecipient"/>；上限 3 个）。
    /// </summary>
    /// <remarks>
    /// <para>官方业务限制：只支持传 userid，必须是同企业的用户，且在参与人中。</para>
    /// </remarks>
    [JsonPropertyName("schedule_admins")]
    public MailRecipient? ScheduleAdmins { get; set; }
}
