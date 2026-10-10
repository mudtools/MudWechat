// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文本内容块属性（官方 TextBlockProps，对应 <c>BLOCK_TYPE_TEXT</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocTextBlockProps
{
    /// <summary>
    /// 获取或设置块级对齐方式（官方 <c>align</c>，见官方 <c>TextAlign</c>）。
    /// 官方取值：<c>TEXT_ALIGN_UNSPECIFIED</c> 未指定、<c>TEXT_ALIGN_LEFT</c> 左对齐、<c>TEXT_ALIGN_CENTER</c> 居中对齐、
    /// <c>TEXT_ALIGN_RIGHT</c> 右对齐、<c>TEXT_ALIGN_JUSTIFY</c> 两端对齐。
    /// </summary>
    [JsonPropertyName("align")]
    public uint? Align { get; set; }

    /// <summary>
    /// 获取或设置文本对齐方式（官方 <c>text_align</c>，见官方 <c>TextAlign</c>），取值同 <see cref="Align"/>。
    /// </summary>
    [JsonPropertyName("text_align")]
    public uint? TextAlign { get; set; }

    /// <summary>
    /// 获取或设置文本背景色（官方 <c>block_color</c>，见官方 <c>BlockColor</c>）。
    /// 官方取值：<c>default_background</c> 默认背景、<c>light_grey_background</c> 浅灰、<c>grey_background</c> 灰色、
    /// <c>dark_background</c> 深色、<c>light_red_background</c> 浅红、<c>red_background</c> 红色、
    /// <c>light_orange_background</c> 浅橙、<c>orange_background</c> 橙色、<c>light_yellow_background</c> 浅黄、
    /// <c>yellow_background</c> 黄色、<c>light_green_background</c> 浅绿、<c>green_background</c> 绿色、
    /// <c>light_cyan_background</c> 浅青、<c>cyan_background</c> 青色、<c>light_blue_background</c> 浅蓝、
    /// <c>blue_background</c> 蓝色、<c>light_accent_blue_background</c> 浅亮蓝、<c>accent_blue_background</c> 亮蓝、
    /// <c>light_purple_background</c> 浅紫、<c>purple_background</c> 紫色。
    /// </summary>
    [JsonPropertyName("block_color")]
    public string? BlockColor { get; set; }
}
