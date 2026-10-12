// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Category;

/// <summary>类目属性项（官方 <c>product_attr_list</c> / <c>sale_attr_list</c> 数组元素，<c>getcategorydetail</c> 形态——
/// 与 <c>getcategoryproductrule</c> 的属性形态不同（那边无 <c>type</c>/<c>is_required</c>、多 <c>required_rule</c>）⇒ 分建）。</summary>
[HttpJsonSerializable(SerializerClassName = "Category")]
public class ChannelsCategoryAttrItem
{
    /// <summary>获取或设置类目必填项名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置属性类型（官方 <c>type</c>）：<c>string</c> 为自定义，<c>select_one</c> 为多选一；
    /// 短期保留，将废弃，用 <c>type_v2</c> 替代。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置可选项列表（官方 <c>value</c>）——<c>type</c> 为 <c>select_one</c>/<c>select_many</c> 时为选项列表，
    /// <c>integer_unit</c>/<c>decimal4_unit</c> 时为单位列表。
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>获取或设置是否类目必填项（官方 <c>is_required</c>）。</summary>
    [JsonPropertyName("is_required")]
    public bool? IsRequired { get; set; }

    /// <summary>获取或设置输入提示（官方 <c>hint</c>）。</summary>
    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    /// <summary>获取或设置允许添加选项（官方 <c>append_allowed</c>）。</summary>
    [JsonPropertyName("append_allowed")]
    public bool? AppendAllowed { get; set; }

    /// <summary>获取或设置属性类型 v2（官方 <c>type_v2</c>，7 种取值，见 <see cref="ChannelsCategoryAttrTypes"/>）。</summary>
    [JsonPropertyName("type_v2")]
    public string? TypeV2 { get; set; }
}
