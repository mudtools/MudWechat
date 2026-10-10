// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 提醒成员群发请求体（<c>/cgi-bin/externalcontact/remind_groupmsg_send</c>）。
/// <para>24 小时内每个群发最多触发三次提醒。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class RemindGroupMsgSendRequest
{
    /// <summary>
    /// 获取或设置群发消息的 id（官方必填；通过「获取群发记录列表」接口返回）。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }
}
