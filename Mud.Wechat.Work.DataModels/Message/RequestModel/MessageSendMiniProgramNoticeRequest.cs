// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送小程序通知消息请求体（msgtype 固定为 miniprogram_notice，<c>/cgi-bin/message/send</c>）。
/// <para>只允许绑定了小程序的应用发送；不支持 "@all" 全员发送；微工作台不支持展示；
/// 2019-06-28 起通知不再出现在企业群发中，而是出现在各独立应用中。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageSendMiniProgramNoticeRequest : MessageSendRequest
{
    /// <summary>
    /// 获取或设置小程序通知消息体（官方必填）。
    /// </summary>
    [JsonPropertyName("miniprogram_notice")]
    public MessageMiniProgramNoticeBody? MiniProgramNotice { get; set; }
}
