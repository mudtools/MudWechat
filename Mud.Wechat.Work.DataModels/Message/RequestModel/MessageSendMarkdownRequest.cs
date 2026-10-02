// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 发送 markdown 消息请求体（msgtype 固定为 markdown，<c>/cgi-bin/message/send</c>）。
/// <para>微工作台（原企业号）不支持展示 markdown 消息；markdown 消息不支持 id 转译。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageSendMarkdownRequest : MessageSendRequest
{
    /// <summary>
    /// 获取或设置 markdown 消息体（官方必填）。
    /// </summary>
    [JsonPropertyName("markdown")]
    public MessageMarkdownBody? Markdown { get; set; }
}
