// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;

/// <summary>
/// 发送新客户欢迎语请求体（<c>/cgi-bin/externalcontact/send_welcome_msg</c>）。
/// <para>
/// <see cref="WelcomeCode"/> 为官方必填（来自「添加外部联系人事件」推送，有效期 20 秒）；
/// 仅可在收到事件后 20 秒内调用且只可调用一次；文本与附件不能同时为空（两者可同时发送，
/// 以多条消息形式触达客户）；管理端已配置可用欢迎语时事件不返回 welcome_code。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "GroupMsg")]
public class SendWelcomeMsgRequest
{
    /// <summary>
    /// 获取或设置通过「添加外部联系人事件」推送的欢迎语凭证（官方必填；有效期 20 秒，仅可使用一次）。
    /// </summary>
    [JsonPropertyName("welcome_code")]
    public string? WelcomeCode { get; set; }

    /// <summary>
    /// 获取或设置文本消息（最长 4000 字节；与附件不能同时为空，可同时发送）。
    /// </summary>
    [JsonPropertyName("text")]
    public GroupMsgTextContent? Text { get; set; }

    /// <summary>
    /// 获取或设置附件列表（最多 9 个；各字段须与 msgtype 一致，否则报错）。
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<GroupMsgAttachment>? Attachments { get; set; }
}
