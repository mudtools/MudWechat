// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 邮件收件人结构（官方 to / cc / bcc 及 meeting.hosts / meeting.meeting_admins /
/// schedule.schedule_admins 共用的 obj 结构：emails + userids）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailRecipient
{
    /// <summary>获取或设置邮箱地址列表（官方 emails）。</summary>
    [JsonPropertyName("emails")]
    public List<string>? Emails { get; set; }

    /// <summary>获取或设置企业内成员的 userid 列表（官方 userids）。</summary>
    /// <remarks>
    /// <para>官方业务限制：to.userids / cc.userids / bcc.userids 指定的成员需在应用可见范围内；
    /// meeting.hosts / meeting.meeting_admins / schedule.schedule_admins 只支持传 userid。</para>
    /// </remarks>
    [JsonPropertyName("userids")]
    public List<string>? Userids { get; set; }
}
