// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 单元格文字样式（官方 TextFormat；CellFormat 的 text_format）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocTextFormat
{
    /// <summary>获取或设置字体名称（官方 font；须在官方字体列表内，如 Microsoft YaHei、SimSun、Arial 等）。</summary>
    [JsonPropertyName("font")]
    public string? Font { get; set; }

    /// <summary>获取或设置字体大小，最大 72（官方 font_size）。</summary>
    [JsonPropertyName("font_size")]
    public long? FontSize { get; set; }

    /// <summary>获取或设置是否加粗（官方 bold）。</summary>
    [JsonPropertyName("bold")]
    public bool? Bold { get; set; }

    /// <summary>获取或设置是否斜体（官方 italic）。</summary>
    [JsonPropertyName("italic")]
    public bool? Italic { get; set; }

    /// <summary>获取或设置是否删除线（官方 strikethrough）。</summary>
    [JsonPropertyName("strikethrough")]
    public bool? Strikethrough { get; set; }

    /// <summary>获取或设置是否下划线（官方 underline）。</summary>
    [JsonPropertyName("underline")]
    public bool? Underline { get; set; }

    /// <summary>获取或设置字体颜色（官方 color，<see cref="WedocColor"/>）。</summary>
    [JsonPropertyName("color")]
    public WedocColor? Color { get; set; }
}
