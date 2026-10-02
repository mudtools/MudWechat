// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 文本消息体（<c>msgtype=text</c>），被发送应用消息（<c>/cgi-bin/message/send</c>）与发送「学校通知」（<c>/cgi-bin/externalcontact/message/send</c>）共用。
/// <para>消息内容支持 id 转译；换行须使用转义的 \n；支持 &lt;a href="..."&gt; 标签打开自定义网页。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageTextBody
{
    /// <summary>
    /// 获取或设置消息内容，最长不超过 2048 个字节，超过将截断（支持 id 转译；换行须用转义的 \n；支持 &lt;a href="..."&gt; 标签打开自定义网页）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
