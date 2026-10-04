// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 更新记录请求体（<c>/cgi-bin/wedoc/smartsheet/update_records</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UpdateSmartSheetRecordsRequest
{
    /// <summary>获取或设置文档的 docid（官方必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置 Smartsheet 子表 ID（官方必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>
    /// 获取或设置返回记录中单元格的 key 类型（官方 <c>key_type</c>，非必填）。
    /// 官方取值：<c>CELL_VALUE_KEY_TYPE_FIELD_TITLE</c> key 用字段标题表示、<c>CELL_VALUE_KEY_TYPE_FIELD_ID</c> key 用字段 ID 表示。
    /// </summary>
    [JsonPropertyName("key_type")]
    public string? KeyType { get; set; }

    /// <summary>
    /// 获取或设置由需要更新的记录组成的 JSON 数组（官方 <c>records</c>，必填）。
    /// 单次更新建议在 500 行内；不能给创建时间、最后编辑时间、创建人和最后编辑人四种类型的字段更新记录。
    /// </summary>
    [JsonPropertyName("records")]
    public List<SmartSheetUpdateRecord>? Records { get; set; }
}
