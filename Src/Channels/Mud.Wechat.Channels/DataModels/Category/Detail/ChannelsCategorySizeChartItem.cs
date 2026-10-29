// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>尺码属性项（官方 <c>size_chart.item_list</c> 数组元素；官方 <c>type</c>/<c>format</c>/<c>limit</c> 标注为 string，照抄）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategorySizeChartItem
{
    /// <summary>获取或设置尺码属性名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置单位（官方 <c>unit</c>）。</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>获取或设置尺码属性值的类型（官方 <c>type</c>，官方标注 string：「1：字符型，2：整数型，3：小数型」）。</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置尺码属性值的填写格式（官方 <c>format</c>，官方标注 string：「1：单值填写，2：区间值填写，3：支持单值或区间值」）。</summary>
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    /// <summary>获取或设置尺码属性值的限制（官方 <c>limit</c>，官方标注 string）。</summary>
    [JsonPropertyName("limit")]
    public string? Limit { get; set; }

    /// <summary>获取或设置是否必填（官方 <c>is_required</c>）。</summary>
    [JsonPropertyName("is_required")]
    public bool? IsRequired { get; set; }
}
