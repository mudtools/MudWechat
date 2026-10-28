// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Hr;

/// <summary>
/// 人事助手字段配置信息（<c>/cgi-bin/hr/get_fields</c> 响应内嵌结构）。
/// </summary>
/// <remarks>
/// <para>
/// 官方文档字段信息说明表将字段 id 记作 <c>field_id</c>，但请求/响应示例均为 <c>fieldid</c>
/// （与 get_staff_info / update_staff_info 的请求侧字段名一致），本模型以示例的 <c>fieldid</c> 为准。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Hr")]
public class HrFieldConfig
{
    /// <summary>获取或设置字段的 id（官方 fieldid）。</summary>
    [JsonPropertyName("fieldid")]
    public long? Fieldid { get; set; }

    /// <summary>获取或设置字段的名称（官方 field_name）。</summary>
    [JsonPropertyName("field_name")]
    public string? FieldName { get; set; }

    /// <summary>
    /// 获取或设置字段的类型（官方 field_type）：1 - 文本类型（取值为字符串或电话号码）；
    /// 2 - 选项类型（取值为 32 位非负整数）；3 - 时间类型（取值为 64 位非负整数）。
    /// </summary>
    [JsonPropertyName("field_type")]
    public long? FieldType { get; set; }

    /// <summary>获取或设置字段值的类型（官方 value_type，1 字符串 / 2 64位非负整数 / 3 32位非负整数 / 4 64位整数 / 5 电话号码 / 6 文件）。</summary>
    [JsonPropertyName("value_type")]
    public long? ValueType { get; set; }

    /// <summary>获取或设置字段是否为必填（官方 is_must，布尔值）。</summary>
    [JsonPropertyName("is_must")]
    public bool? IsMust { get; set; }

    /// <summary>获取或设置选项类型字段的枚举列表（官方 option_list，仅当 field_type 为 2 时返回）。</summary>
    [JsonPropertyName("option_list")]
    public List<HrFieldOption>? OptionList { get; set; }
}
