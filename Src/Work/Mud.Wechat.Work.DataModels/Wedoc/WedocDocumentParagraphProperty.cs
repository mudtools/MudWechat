// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档段落属性（官方 ParagraphProperty）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentParagraphProperty
{
    /// <summary>获取或设置段落编号属性（官方 number_property，<see cref="WedocDocumentNumberProperty"/>）。</summary>
    [JsonPropertyName("number_property")]
    public WedocDocumentNumberProperty? NumberProperty { get; set; }

    /// <summary>获取或设置段落间距（官方 spacing，<see cref="WedocDocumentSpacing"/>）。</summary>
    [JsonPropertyName("spacing")]
    public WedocDocumentSpacing? Spacing { get; set; }

    /// <summary>获取或设置段落缩进（官方 indent，<see cref="WedocDocumentIndent"/>）。</summary>
    [JsonPropertyName("indent")]
    public WedocDocumentIndent? Indent { get; set; }

    /// <summary>
    /// 获取或设置段落对齐方式（官方 alignment_type）：
    /// ALIGNMENT_TYPE_UNSPECIFIED / CENTER / BOTH / DISTRIBUTE / LEFT / RIGHT。
    /// </summary>
    [JsonPropertyName("alignment_type")]
    public string? AlignmentType { get; set; }

    /// <summary>
    /// 获取或设置文本方向（官方 text_direction）：
    /// TEXT_DIRECTION_UNSPECIFIED / RIGHT_TO_LEFT / LEFT_TO_RIGHT。
    /// </summary>
    [JsonPropertyName("text_direction")]
    public string? TextDirection { get; set; }
}
