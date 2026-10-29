// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 智能表格按字段配置的权限（官方 <c>field_priv</c>；更新智能表格子表权限请求与查询智能表格子表权限响应共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSheetFieldPriv
{
    /// <summary>
    /// 获取或设置字段范围类型（官方 <c>field_range_type</c>，必填）。
    /// 官方取值：<c>1</c> 所有字段、<c>2</c> 部分字段。
    /// </summary>
    [JsonPropertyName("field_range_type")]
    public uint? FieldRangeType { get; set; }

    /// <summary>获取或设置按字段分别配置的权限列表（官方 <c>field_rule_list</c>，必填）。</summary>
    [JsonPropertyName("field_rule_list")]
    public List<WedocSheetFieldRule>? FieldRuleList { get; set; }

    /// <summary>
    /// 获取或设置未指定字段和后续新增字段的默认配置（官方 <c>field_default_rule</c>）；
    /// <c>field_range_type</c> 为 <c>1</c>（所有字段）时必填、为 <c>2</c>（部分字段）时不可指定。
    /// </summary>
    [JsonPropertyName("field_default_rule")]
    public WedocSheetFieldDefaultRule? FieldDefaultRule { get; set; }
}
