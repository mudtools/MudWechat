// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 更新智能表格子表权限请求体（<c>/cgi-bin/wedoc/smartsheet/content_priv/update_sheet_priv</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：每个智能表格有且只有一个全员权限；<c>type</c> 为 <c>2</c>（额外权限）时 <c>rule_id</c> 必填；
/// <c>priv_list[].priv</c> 为 <c>2</c>（可编辑）或 <c>3</c>（仅浏览）时 <c>record_priv</c> 必填；
/// <c>field_priv.field_default_rule</c> 在 <c>field_range_type</c> 为 <c>1</c>（所有字段）时必填、为 <c>2</c>（部分字段）时不可指定；
/// 记录条件仅对人员、单选、多选三种字段类型有效。
/// </para>
/// <para>
/// 官方契约陷阱：<c>priv_list[].priv</c> 官方参数表标注为 string，而官方请求/响应示例均为数字（如 <c>2</c>），
/// 本模型以示例为准（整数）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UpdateWedocSheetPrivRequest
{
    /// <summary>获取或设置智能表 id（官方 <c>docid</c>，必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>
    /// 获取或设置权限规则类型（官方 <c>type</c>，必填）。
    /// 官方取值：<c>1</c> 全员权限、<c>2</c> 额外权限。
    /// </summary>
    [JsonPropertyName("type")]
    public uint? Type { get; set; }

    /// <summary>获取或设置需要更新的规则 id（官方 <c>rule_id</c>），<c>type</c> 为 <c>2</c> 时必填。</summary>
    [JsonPropertyName("rule_id")]
    public uint? RuleId { get; set; }

    /// <summary>获取或设置更新的权限名称（官方 <c>name</c>），仅当 <c>type</c> 为 <c>2</c> 时有效。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置针对不同子表设置的内容权限列表（官方 <c>priv_list</c>）。</summary>
    [JsonPropertyName("priv_list")]
    public List<WedocSheetPriv>? PrivList { get; set; }
}
