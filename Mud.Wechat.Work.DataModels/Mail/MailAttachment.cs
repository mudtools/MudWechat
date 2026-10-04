// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Mail;

/// <summary>
/// 邮件附件项（官方 attachment_list 元素结构：file_name + content）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Mail")]
public class MailAttachment
{
    /// <summary>获取或设置文件名（官方必填）。</summary>
    [JsonPropertyName("file_name")]
    public string? FileName { get; set; }

    /// <summary>获取或设置文件内容的 base64 编码（官方必填）。</summary>
    /// <remarks>
    /// <para>官方业务限制：所有附件加正文的大小不允许超过 50M，且附件个数不能超过 200 个。</para>
    /// </remarks>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
