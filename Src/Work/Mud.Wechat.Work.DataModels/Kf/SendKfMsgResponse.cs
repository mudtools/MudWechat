// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 微信客服消息发送响应体（<c>/cgi-bin/kf/send_msg</c> 与 <c>/cgi-bin/kf/send_msg_on_event</c> 共用）。
/// <para>
/// 接口返回成功不代表消息最终发送成功，须关注「消息发送失败事件」回调。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class SendKfMsgResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置消息 ID；请求指定了 msgid 则原样返回，否则系统自动生成。
    /// </summary>
    [JsonPropertyName("msgid")]
    public string? MsgId { get; set; }
}
