// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 视图过滤 / 填色的判断条件（官方 Condition）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetCondition
{
    /// <summary>获取或设置字段 ID（官方 <c>field_id</c>，必填）。</summary>
    [JsonPropertyName("field_id")]
    public string? FieldId { get; set; }

    /// <summary>获取或设置字段类型（官方 <c>field_type</c>，必填），见官方 <c>FieldType</c>。</summary>
    [JsonPropertyName("field_type")]
    public string? FieldType { get; set; }

    /// <summary>
    /// 获取或设置判断类型（官方 <c>operator</c>，必填）。
    /// 官方取值：<c>OPERATOR_UNKNOWN</c> 未知、<c>OPERATOR_IS</c> 等于、<c>OPERATOR_IS_NOT</c> 不等于、<c>OPERATOR_CONTAINS</c> 包含、<c>OPERATOR_DOES_NOT_CONTAIN</c> 不包含、<c>OPERATOR_IS_GREATER</c> 大于、<c>OPERATOR_IS_GREATER_OR_EQUAL</c> 大于或等于、<c>OPERATOR_IS_LESS</c> 小于、<c>OPERATOR_IS_LESS_OR_EQUAL</c> 小于或等于、<c>OPERATOR_IS_EMPTY</c> 为空、<c>OPERATOR_IS_NOT_EMPTY</c> 不为空。
    /// </summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }

    /// <summary>获取或设置文本值（官方 <c>string_value</c>），文本、网址、电话、邮箱、地理位置、单选、多选等列类型使用。</summary>
    [JsonPropertyName("string_value")]
    public SmartSheetFilterStringValue? StringValue { get; set; }

    /// <summary>获取或设置数字值（官方 <c>number_value</c>），数字、进度列类型使用。</summary>
    [JsonPropertyName("number_value")]
    public SmartSheetFilterNumberValue? NumberValue { get; set; }

    /// <summary>获取或设置复选框值（官方 <c>bool_value</c>），复选框列类型使用。</summary>
    [JsonPropertyName("bool_value")]
    public SmartSheetFilterBoolValue? BoolValue { get; set; }

    /// <summary>获取或设置人员值（官方 <c>user_value</c>），人员、创建人、最后编辑人列类型使用，值为成员 ID。</summary>
    [JsonPropertyName("user_value")]
    public SmartSheetFilterUserValue? UserValue { get; set; }

    /// <summary>获取或设置日期值（官方 <c>date_time_value</c>），日期、创建时间、最后编辑时间列类型使用。</summary>
    [JsonPropertyName("date_time_value")]
    public SmartSheetFilterDateTimeValue? DateTimeValue { get; set; }
}
