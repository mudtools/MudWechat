// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 发送会议邮件请求体（<c>/cgi-bin/exmail/app/compose_send</c>）。
/// <para>subject 同时是会议标题、content 同时是会议描述；schedule 与 meeting 官方必填
/// （会议相关的基本设置放在 schedule 里，会议设置放在 meeting 里且必须同时带上 schedule）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailSendMeetingRequest : MailSendRequest
{
    /// <summary>
    /// 获取或设置会议相关数据（官方必填，发会议邮件必须带上本字段；<see cref="MailSchedule"/>）。
    /// </summary>
    [JsonPropertyName("schedule")]
    public MailSchedule? Schedule { get; set; }

    /// <summary>
    /// 获取或设置会议相关字段（官方标注会议邮件必填，且必须同时带上 schedule；<see cref="MailMeeting"/>）。
    /// </summary>
    [JsonPropertyName("meeting")]
    public MailMeeting? Meeting { get; set; }
}
