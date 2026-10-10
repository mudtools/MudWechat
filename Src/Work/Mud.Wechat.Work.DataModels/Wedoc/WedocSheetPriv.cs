// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 智能表格单个子表的内容权限（官方 <c>priv_list</c> 元素；
/// 更新智能表格子表权限请求与查询智能表格子表权限响应共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约陷阱：<c>priv</c> 官方参数表标注为 string，而官方请求/响应示例均为数字（如 <c>2</c>），本模型以示例为准（整数）。
/// 官方业务限制：<c>priv</c> 为 <c>2</c>（可编辑）或 <c>3</c>（仅浏览）时 <c>record_priv</c> 必填；
/// <c>can_insert_record</c> / <c>can_delete_record</c> 仅当子表权限为可编辑时有意义；
/// <c>clear</c> 为 <c>true</c> 时清除该子表的设置并恢复默认权限。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocSheetPriv
{
    /// <summary>获取或设置子表 id（官方 <c>sheet_id</c>，必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>
    /// 获取或设置子表权限（官方 <c>priv</c>，必填）。
    /// 官方取值：<c>1</c> 全部权限、<c>2</c> 可编辑、<c>3</c> 仅浏览、<c>4</c> 无权限。
    /// </summary>
    [JsonPropertyName("priv")]
    public uint? Priv { get; set; }

    /// <summary>获取或设置是否可以新增记录（官方 <c>can_insert_record</c>），仅当子表权限为可编辑时有意义。</summary>
    [JsonPropertyName("can_insert_record")]
    public bool? CanInsertRecord { get; set; }

    /// <summary>获取或设置是否可以删除记录（官方 <c>can_delete_record</c>），仅当子表权限为可编辑时有意义。</summary>
    [JsonPropertyName("can_delete_record")]
    public bool? CanDeleteRecord { get; set; }

    /// <summary>获取或设置是否可以增、删、改视图（官方 <c>can_create_modify_delete_view</c>）。</summary>
    [JsonPropertyName("can_create_modify_delete_view")]
    public bool? CanCreateModifyDeleteView { get; set; }

    /// <summary>获取或设置按字段配置的权限（官方 <c>field_priv</c>）。</summary>
    [JsonPropertyName("field_priv")]
    public WedocSheetFieldPriv? FieldPriv { get; set; }

    /// <summary>获取或设置按记录配置的权限（官方 <c>record_priv</c>），<c>priv</c> 为 <c>2</c> 或 <c>3</c> 时必填。</summary>
    [JsonPropertyName("record_priv")]
    public WedocSheetRecordPriv? RecordPriv { get; set; }

    /// <summary>获取或设置是否清除该子表的设置并恢复默认权限（官方 <c>clear</c>）。</summary>
    [JsonPropertyName("clear")]
    public bool? Clear { get; set; }
}
