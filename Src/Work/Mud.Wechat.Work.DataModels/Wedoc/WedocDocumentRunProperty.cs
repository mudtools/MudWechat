// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文本属性（官方 RunProperty；文档节点 Text 容器与 Run 的文字样式）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentRunProperty
{
    /// <summary>获取或设置字体名称（官方 font）。</summary>
    [JsonPropertyName("font")]
    public string? Font { get; set; }

    /// <summary>获取或设置是否加粗（官方 bold）。</summary>
    [JsonPropertyName("bold")]
    public bool? Bold { get; set; }

    /// <summary>获取或设置是否斜体（官方 italics）。</summary>
    [JsonPropertyName("italics")]
    public bool? Italics { get; set; }

    /// <summary>获取或设置是否下划线（官方 underline）。</summary>
    [JsonPropertyName("underline")]
    public bool? Underline { get; set; }

    /// <summary>获取或设置是否删除线（官方 strike）。</summary>
    [JsonPropertyName("strike")]
    public bool? Strike { get; set; }

    /// <summary>获取或设置文字颜色，十六进制 RRGGBB 格式（官方 color）。</summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>获取或设置字符间距（官方 spacing）。</summary>
    [JsonPropertyName("spacing")]
    public double? Spacing { get; set; }

    /// <summary>获取或设置字体大小，单位半点（half-points）（官方 size）。</summary>
    [JsonPropertyName("size")]
    public double? Size { get; set; }

    /// <summary>获取或设置底纹（官方 shading，<see cref="WedocDocumentShading"/>）。</summary>
    [JsonPropertyName("shading")]
    public WedocDocumentShading? Shading { get; set; }

    /// <summary>
    /// 获取或设置文字垂直对齐方式（官方 vertical_align）：
    /// RUN_VERTICAL_ALIGN_UNSPECIFIED / BASELINE / SUPER_SCRIPT / SUB_SCRIPT。
    /// </summary>
    [JsonPropertyName("vertical_align")]
    public string? VerticalAlign { get; set; }

    /// <summary>获取或设置是否为占位文本（官方 is_placeholder）。</summary>
    [JsonPropertyName("is_placeholder")]
    public bool? IsPlaceholder { get; set; }
}
