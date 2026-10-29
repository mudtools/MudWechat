// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 关联类型字段属性（官方 PropertyReference / ReferenceFieldProperty）。
/// </summary>
/// <remarks>
/// <para>
/// 官方对同一字段在「添加字段 / 更新字段」文档中拼写为 <c>field_id</c>，在「查询字段」文档中拼写为 <c>filed_id</c>（多一个 d），属官方拼写陷阱；本模型同时提供两个互斥的可空属性以精确承载两种形态，不得「顺手修正」任一拼写。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetReferenceFieldProperty
{
    /// <summary>获取或设置关联的子表 ID（官方 <c>sub_id</c>），为空时表示关联本子表。</summary>
    [JsonPropertyName("sub_id")]
    public string? SubId { get; set; }

    /// <summary>获取或设置关联的字段 ID（官方 <c>field_id</c>，添加 / 更新字段侧官方拼写）。</summary>
    [JsonPropertyName("field_id")]
    public string? FieldId { get; set; }

    /// <summary>获取或设置关联的字段 ID（官方 <c>filed_id</c>，查询字段侧官方拼写，多一个 d）。</summary>
    [JsonPropertyName("filed_id")]
    public string? FiledId { get; set; }

    /// <summary>获取或设置是否允许多选（官方 <c>is_multiple</c>）。</summary>
    [JsonPropertyName("is_multiple")]
    public bool? IsMultiple { get; set; }

    /// <summary>获取或设置视图 ID（官方 <c>view_id</c>）。</summary>
    [JsonPropertyName("view_id")]
    public string? ViewId { get; set; }
}
