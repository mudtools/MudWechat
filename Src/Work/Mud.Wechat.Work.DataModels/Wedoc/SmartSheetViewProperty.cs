// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 视图属性（官方 ViewProperty；视图的排序、过滤、分组、字段显示、冻结列及填色配置，更新视图与查询视图共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetViewProperty
{
    /// <summary>获取或设置记录变更后是否自动重新排序（官方 <c>auto_sort</c>）。</summary>
    [JsonPropertyName("auto_sort")]
    public bool? AutoSort { get; set; }

    /// <summary>获取或设置排序设置（官方 <c>sort_spec</c>）。</summary>
    [JsonPropertyName("sort_spec")]
    public SmartSheetSortSpec? SortSpec { get; set; }

    /// <summary>获取或设置分组设置（官方 <c>group_spec</c>）。</summary>
    [JsonPropertyName("group_spec")]
    public SmartSheetGroupSpec? GroupSpec { get; set; }

    /// <summary>获取或设置过滤设置（官方 <c>filter_spec</c>）。</summary>
    [JsonPropertyName("filter_spec")]
    public SmartSheetFilterSpec? FilterSpec { get; set; }

    /// <summary>获取或设置是否使用数据统计（官方 <c>is_field_stat_enabled</c>）。</summary>
    [JsonPropertyName("is_field_stat_enabled")]
    public bool? IsFieldStatEnabled { get; set; }

    /// <summary>
    /// 获取或设置字段显示配置（官方 <c>field_visibility</c>）。
    /// 官方说明「类似 map」：key 为字段 ID，value 为布尔值，表示是否显示该字段。
    /// </summary>
    [JsonPropertyName("field_visibility")]
    public Dictionary<string, bool>? FieldVisibility { get; set; }

    /// <summary>获取或设置冻结列数量（官方 <c>frozen_field_count</c>），从首列开始。</summary>
    [JsonPropertyName("frozen_field_count")]
    public int? FrozenFieldCount { get; set; }

    /// <summary>获取或设置填色设置（官方 <c>color_config</c>）。</summary>
    [JsonPropertyName("color_config")]
    public SmartSheetViewColorConfig? ColorConfig { get; set; }
}
