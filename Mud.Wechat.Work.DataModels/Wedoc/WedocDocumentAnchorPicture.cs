// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 浮动图片（官方 AnchorPicture）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentAnchorPicture
{
    /// <summary>获取或设置图片 url（官方 uri）。</summary>
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }

    /// <summary>获取或设置图片相对区域（官方 relative_rect，<see cref="WedocDocumentRelativeRect"/>）。</summary>
    [JsonPropertyName("relative_rect")]
    public WedocDocumentRelativeRect? RelativeRect { get; set; }

    /// <summary>获取或设置图形属性（官方 shape，<see cref="WedocDocumentShapeProperties"/>）。</summary>
    [JsonPropertyName("shape")]
    public WedocDocumentShapeProperties? Shape { get; set; }

    /// <summary>获取或设置水平位置（官方 position_horizontal，<see cref="WedocDocumentPositionHorizontal"/>）。</summary>
    [JsonPropertyName("position_horizontal")]
    public WedocDocumentPositionHorizontal? PositionHorizontal { get; set; }

    /// <summary>获取或设置垂直位置（官方 position_vertical，<see cref="WedocDocumentPositionVertical"/>）。</summary>
    [JsonPropertyName("position_vertical")]
    public WedocDocumentPositionVertical? PositionVertical { get; set; }

    /// <summary>获取或设置是否无环绕（官方 wrap_none）。</summary>
    [JsonPropertyName("wrap_none")]
    public bool? WrapNone { get; set; }

    /// <summary>获取或设置四周环绕配置（官方 wrap_square，<see cref="WedocDocumentWrapSquare"/>）。</summary>
    [JsonPropertyName("wrap_square")]
    public WedocDocumentWrapSquare? WrapSquare { get; set; }

    /// <summary>获取或设置是否上下环绕（官方 wrap_top_and_bottom）。</summary>
    [JsonPropertyName("wrap_top_and_bottom")]
    public bool? WrapTopAndBottom { get; set; }

    /// <summary>获取或设置是否衬于文字下方（官方 behind_doc）。</summary>
    [JsonPropertyName("behind_doc")]
    public bool? BehindDoc { get; set; }

    /// <summary>获取或设置是否允许重叠（官方 allow_overlap）。</summary>
    [JsonPropertyName("allow_overlap")]
    public bool? AllowOverlap { get; set; }
}
