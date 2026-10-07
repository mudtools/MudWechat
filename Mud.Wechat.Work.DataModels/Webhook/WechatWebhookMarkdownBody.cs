// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Webhook;

/// <summary>
/// 群机器人 markdown 消息体（<c>msgtype=markdown</c>）。
/// <para>content 最长 4096 个字节（超出报错）；支持官方子集语法（标题、加粗、斜体、链接、行内代码、引用、字体颜色、换行等）。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Webhook")]
public class WechatWebhookMarkdownBody
{
    /// <summary>
    /// 获取或设置 markdown 内容（官方必填），最长不超过 4096 个字节，超出报错。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
