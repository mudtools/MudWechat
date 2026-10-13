// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.TransactionGuarantee;

/// <summary>旧评价内容（<c>oldComment.content</c>）。</summary>
/// <remarks>
/// <b>与 <see cref="WxaCommentContent"/> 不可合并</b>：官方本页参数表里旧评价的文本字段名为
/// <c>ext</c>（新评价为 <c>txt</c>），媒体结构则官方原文称「跟上面提到的媒体结构一致」
/// ⇒ 文本字段各自照录、媒体共用 <see cref="WxaCommentMedia"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TransactionGuarantee")]
public class WxaCommentOldContent
{
    /// <summary>评价的文本内容（<c>ext</c>，官方字段名，非 <c>txt</c>）。</summary>
    [JsonPropertyName("ext")]
    public string? Ext { get; set; }

    /// <summary>评价的多媒体内容（<c>media</c>，与 <see cref="WxaCommentContent"/> 同结构）。</summary>
    [JsonPropertyName("media")]
    public List<WxaCommentMedia>? Media { get; set; }
}
