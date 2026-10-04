// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 更新编组请求体（<c>/cgi-bin/wedoc/smartsheet/update_field_group</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UpdateSmartSheetFieldGroupRequest
{
    /// <summary>获取或设置文档的 docid（官方必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置表格 ID（官方必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置编组 id（官方必填）。</summary>
    [JsonPropertyName("field_group_id")]
    public string? FieldGroupId { get; set; }

    /// <summary>获取或设置编组名称（官方 <c>name</c>，非必填），不能和已有名称重复。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// 获取或设置编组内容（官方 <c>children</c>，非必填）。
    /// 每个编组最多允许有 150 个字段；字段只能同时存在于一个编组。
    /// </summary>
    [JsonPropertyName("children")]
    public List<SmartSheetFieldGroupChild>? Children { get; set; }
}
