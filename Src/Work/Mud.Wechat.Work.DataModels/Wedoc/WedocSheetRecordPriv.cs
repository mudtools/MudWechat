// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 智能表格按记录配置的权限（官方 <c>record_priv</c>；
/// 更新智能表格子表权限请求与查询智能表格子表权限响应共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：记录条件仅对人员、单选、多选三种字段类型有效；
/// <c>field_id</c> 为 <c>CREATED_USER</c> 时表示记录创建者（此时 <c>field_type</c> 不填）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSheetRecordPriv
{
    /// <summary>
    /// 获取或设置记录范围类型（官方 <c>record_range_type</c>，必填）。
    /// 官方取值：<c>1</c> 全部记录、<c>2</c> 满足任意条件的记录、<c>3</c> 满足全部条件的记录。
    /// </summary>
    [JsonPropertyName("record_range_type")]
    public uint? RecordRangeType { get; set; }

    /// <summary>获取或设置记录的条件列表（官方 <c>record_rule_list</c>），<c>record_range_type</c> 为 <c>2</c> 或 <c>3</c> 时生效。</summary>
    [JsonPropertyName("record_rule_list")]
    public List<WedocSheetRecordRule>? RecordRuleList { get; set; }

    /// <summary>
    /// 获取或设置当记录不满足条件时的权限类型（官方 <c>other_priv</c>，必填）。
    /// 官方取值：<c>1</c> 不可编辑、<c>2</c> 不可查看。
    /// </summary>
    [JsonPropertyName("other_priv")]
    public uint? OtherPriv { get; set; }
}
