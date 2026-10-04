// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 发送日程邮件请求体（<c>/cgi-bin/exmail/app/compose_send</c>）。
/// <para>subject 同时是日程标题、content 同时是日程描述；发送日程邮件时 schedule 官方必填。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailSendScheduleRequest : MailSendRequest
{
    /// <summary>
    /// 获取或设置日程相关数据（官方必填，发日程邮件必填；<see cref="MailSchedule"/>）。
    /// </summary>
    [JsonPropertyName("schedule")]
    public MailSchedule? Schedule { get; set; }
}
