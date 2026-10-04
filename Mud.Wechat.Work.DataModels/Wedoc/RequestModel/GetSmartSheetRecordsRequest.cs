// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询记录请求体（<c>/cgi-bin/wedoc/smartsheet/get_records</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartSheetRecordsRequest
{
    /// <summary>获取或设置文档的 docid（官方必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置 Smartsheet 子表 ID（官方必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置视图 ID（官方 <c>view_id</c>，非必填）。</summary>
    [JsonPropertyName("view_id")]
    public string? ViewId { get; set; }

    /// <summary>获取或设置由记录 ID 组成的 JSON 数组（官方 <c>record_ids</c>，非必填）。</summary>
    [JsonPropertyName("record_ids")]
    public List<string>? RecordIds { get; set; }

    /// <summary>
    /// 获取或设置返回记录中单元格的 key 类型（官方 <c>key_type</c>，非必填）。
    /// 官方取值：<c>CELL_VALUE_KEY_TYPE_FIELD_TITLE</c> key 用字段标题表示、<c>CELL_VALUE_KEY_TYPE_FIELD_ID</c> key 用字段 ID 表示。
    /// </summary>
    [JsonPropertyName("key_type")]
    public string? KeyType { get; set; }

    /// <summary>获取或设置返回指定列，由字段标题组成的 JSON 数组（官方 <c>field_titles</c>，非必填）；<c>key_type</c> 为 <c>CELL_VALUE_KEY_TYPE_FIELD_TITLE</c> 时有效。</summary>
    [JsonPropertyName("field_titles")]
    public List<string>? FieldTitles { get; set; }

    /// <summary>获取或设置返回指定列，由字段 ID 组成的 JSON 数组（官方 <c>field_ids</c>，非必填）；<c>key_type</c> 为 <c>CELL_VALUE_KEY_TYPE_FIELD_ID</c> 时有效。</summary>
    [JsonPropertyName("field_ids")]
    public List<string>? FieldIds { get; set; }

    /// <summary>获取或设置对返回记录进行排序（官方 <c>sort</c>，非必填）。</summary>
    [JsonPropertyName("sort")]
    public List<SmartSheetRecordSort>? Sort { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>offset</c>，非必填），初始值为 0。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// 获取或设置分页大小，每页返回多少条数据（官方 <c>limit</c>，非必填）。
    /// 最大值为 1000；不填写或设置为 0 时，总数大于 1000 一次性返回 1000 行记录，总数小于 1000 时返回全部记录。
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置版本号（官方 <c>ver</c>，非必填）。</summary>
    [JsonPropertyName("ver")]
    public int? Ver { get; set; }

    /// <summary>获取或设置过滤设置（官方 <c>filter_spec</c>，非必填），不支持和 <c>sort</c> 一起使用。</summary>
    [JsonPropertyName("filter_spec")]
    public SmartSheetFilterSpec? FilterSpec { get; set; }
}
