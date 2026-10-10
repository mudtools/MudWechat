// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 智能表格记录权限的单条条件（官方 <c>record_rule_list</c> 元素；
/// 更新智能表格子表权限请求与查询智能表格子表权限响应共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：仅对人员、单选、多选三种字段类型有效；
/// <c>field_id</c> 为 <c>CREATED_USER</c> 时表示记录创建者，此时 <c>field_type</c> 不填，其余类型必填。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSheetRecordRule
{
    /// <summary>获取或设置字段 id（官方 <c>field_id</c>，必填）；为 <c>CREATED_USER</c> 时表示记录创建者。</summary>
    [JsonPropertyName("field_id")]
    public string? FieldId { get; set; }

    /// <summary>获取或设置字段类型（官方 <c>field_type</c>），<c>field_id</c> 为 <c>CREATED_USER</c> 时不填，其他类型必填。</summary>
    [JsonPropertyName("field_type")]
    public string? FieldType { get; set; }

    /// <summary>
    /// 获取或设置操作类型（官方 <c>oper_type</c>，必填）。
    /// 官方取值：<c>1</c> 包含自己、<c>2</c> 包含 value、<c>3</c> 不包含 value、<c>4</c> 等于 value、
    /// <c>5</c> 不等于 value、<c>6</c> 为空、<c>7</c> 非空。
    /// </summary>
    [JsonPropertyName("oper_type")]
    public uint? OperType { get; set; }

    /// <summary>获取或设置条件取值（官方 <c>value</c>），用于单选、多选字段的 option_id 列表。</summary>
    [JsonPropertyName("value")]
    public List<string>? Value { get; set; }
}
