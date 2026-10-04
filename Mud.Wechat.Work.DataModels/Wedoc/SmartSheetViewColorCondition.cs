// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 视图填色条件（官方 ViewColorCondition）。
/// </summary>
/// <remarks>
/// <para>
/// 官方「查询视图」文档把 <c>color_config.conditions[].condition</c> 的类型标注为 <c>object[]</c>（数组），而「更新视图」文档标注为 <c>object</c>（单对象），且官方示例均为单对象；本模型按单对象 <see cref="SmartSheetCondition"/> 承载。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetViewColorCondition
{
    /// <summary>获取或设置填色 ID（官方 <c>id</c>，非必填），新增时不需要传入，更新时传入。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 获取或设置填色类型（官方 <c>type</c>，必填）。
    /// 官方取值：<c>VIEW_COLOR_CONDITION_TYPE_ROW</c> 行、<c>VIEW_COLOR_CONDITION_TYPE_COLUMN</c> 列、<c>VIEW_COLOR_CONDITION_TYPE_CELL</c> 单元格。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置颜色（官方 <c>color</c>，必填）。
    /// 官方取值形如 <c>fillColorGray_5</c>、<c>accentBlueLighten_5</c>、<c>chromeCyanLighten_5</c>、<c>chromeMintLighten_5</c>、<c>chromeRedLighten_5</c>、<c>chromeOrangeLighten_5</c>、<c>chromeAmberLighten_5</c>、<c>chromeVioletLighten_5</c>、<c>chromePinkLighten_5</c> 及其 <c>_4</c> / <c>_3</c> 变体。
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>获取或设置判断条件（官方 <c>condition</c>，必填）。</summary>
    [JsonPropertyName("condition")]
    public SmartSheetCondition? Condition { get; set; }
}
