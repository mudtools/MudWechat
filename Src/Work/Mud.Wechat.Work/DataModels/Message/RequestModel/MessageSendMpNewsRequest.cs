// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送图文消息（mpnews）请求体（msgtype 固定为 mpnews，<c>/cgi-bin/message/send</c>）。
/// <para>mpnews 图文消息的内容存储在企业微信，多次发送以多次计算阅读、点赞统计。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageSendMpNewsRequest : MessageSendRequest
{
    /// <summary>
    /// 获取或设置 mpnews 图文消息体（官方必填）。
    /// </summary>
    [JsonPropertyName("mpnews")]
    public MessageMpNewsBody? MpNews { get; set; }

    /// <summary>
    /// 获取或设置是否保密消息：0 - 可对外分享（默认），1 - 不能分享且内容显示水印，
    /// 2 - 仅限在企业内分享（仅 mpnews 类型支持取值 2）。
    /// </summary>
    [JsonPropertyName("safe")]
    public int? Safe { get; set; }
}
