// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 背景内容块属性（官方 HighlightBlockProps，对应 <c>BLOCK_TYPE_HIGHLIGHT</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocHighlightBlockProps
{
    /// <summary>获取或设置背景图链接（官方 <c>background_image_url</c>）。</summary>
    [JsonPropertyName("background_image_url")]
    public string? BackgroundImageUrl { get; set; }

    /// <summary>获取或设置是否扩展背景（官方 <c>extend_background</c>），即背景图是否撑满整个 Block 区域。</summary>
    [JsonPropertyName("extend_background")]
    public bool? ExtendBackground { get; set; }

    /// <summary>
    /// 获取或设置 Block 背景色（官方 <c>block_color</c>，见官方 <c>BlockColor</c>），取值同文本内容块属性的 <c>block_color</c>。
    /// </summary>
    [JsonPropertyName("block_color")]
    public string? BlockColor { get; set; }

    /// <summary>
    /// 获取或设置边框颜色（官方 <c>border_color</c>，见官方 <c>BorderColor</c>）。
    /// 官方取值：<c>default</c> 默认、<c>grey</c> 灰色、<c>blue</c> 蓝色、<c>sky_blue</c> 天蓝色、<c>green</c> 绿色、
    /// <c>yellow</c> 黄色、<c>orange</c> 橙色、<c>red</c> 红色、<c>rose_red</c> 玫红色、<c>purple</c> 紫色。
    /// </summary>
    [JsonPropertyName("border_color")]
    public string? BorderColor { get; set; }
}
