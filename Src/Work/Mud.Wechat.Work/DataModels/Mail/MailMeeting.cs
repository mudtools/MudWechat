// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 会议相关字段（官方 meeting 结构，发送会议邮件专用，必须同时带上 <see cref="MailSchedule"/>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailMeeting
{
    /// <summary>
    /// 获取或设置会议主持人列表（官方 hosts，<see cref="MailRecipient"/>；最多 10 个）。
    /// </summary>
    /// <remarks><para>官方注明定义见收件人字段，但只支持填 userid。</para></remarks>
    [JsonPropertyName("hosts")]
    public MailRecipient? Hosts { get; set; }

    /// <summary>
    /// 获取或设置会议管理员（官方 meeting_admins，<see cref="MailRecipient"/>；官方必填，仅可指定 1 人）。
    /// </summary>
    /// <remarks><para>官方业务限制：只支持传 userid，必须是同企业的用户，且在参与人中。</para></remarks>
    [JsonPropertyName("meeting_admins")]
    public MailRecipient? MeetingAdmins { get; set; }

    /// <summary>获取或设置会议相关设置（官方选填，<see cref="MailMeetingOption"/>）。</summary>
    [JsonPropertyName("option")]
    public MailMeetingOption? Option { get; set; }
}
