// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// markdown 消息体（<c>msgtype=markdown</c>），用于发送应用消息（<c>/cgi-bin/message/send</c>）。
/// <para>仅支持 markdown 语法的子集（标题 1~6 级且 # 与文字间要有空格、加粗、链接、行内代码段、引用）；字体颜色仅支持 &lt;font color="info"&gt;（绿色）、&lt;font color="comment"&gt;（灰色）、&lt;font color="warning"&gt;（橙红色）三种内置颜色。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class MessageMarkdownBody
{
    /// <summary>
    /// 获取或设置 markdown 内容，最长不超过 2048 个字节，必须是 utf8 编码。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
