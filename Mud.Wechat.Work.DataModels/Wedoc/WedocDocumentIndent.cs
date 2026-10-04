// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 段落缩进（官方 Indent）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentIndent
{
    /// <summary>获取或设置左缩进（官方 left）。</summary>
    [JsonPropertyName("left")]
    public double? Left { get; set; }

    /// <summary>获取或设置左缩进，单位字符（官方 left_chars）。</summary>
    [JsonPropertyName("left_chars")]
    public double? LeftChars { get; set; }

    /// <summary>获取或设置右缩进（官方 right）。</summary>
    [JsonPropertyName("right")]
    public double? Right { get; set; }

    /// <summary>获取或设置右缩进，单位字符（官方 right_chars）。</summary>
    [JsonPropertyName("right_chars")]
    public double? RightChars { get; set; }

    /// <summary>获取或设置悬挂缩进（官方 hanging）。</summary>
    [JsonPropertyName("hanging")]
    public double? Hanging { get; set; }

    /// <summary>获取或设置悬挂缩进，单位字符（官方 hanging_chars）。</summary>
    [JsonPropertyName("hanging_chars")]
    public double? HangingChars { get; set; }

    /// <summary>获取或设置首行缩进（官方 first_line）。</summary>
    [JsonPropertyName("first_line")]
    public double? FirstLine { get; set; }

    /// <summary>获取或设置首行缩进，单位字符（官方 first_line_chars）。</summary>
    [JsonPropertyName("first_line_chars")]
    public double? FirstLineChars { get; set; }
}
