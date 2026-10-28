// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 获取表格数据请求体（<c>/cgi-bin/wedoc/spreadsheet/get_sheet_range_data</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetWedocSpreadsheetDataRequest
{
    /// <summary>获取或设置在线表格唯一标识（官方 docid，必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置工作表 ID，工作表的唯一标识（官方 sheet_id，必填）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>
    /// 获取或设置查询的范围，遵循 A1 表示法（官方 range，必填）。
    /// <para>官方业务限制：如 <c>A1:A1</c> 为单个单元格、<c>A1:B5</c> 为区域；
    /// 左上角单元格必须在右下角单元格左上方（<c>B5:A1</c> 不合法）。</para>
    /// </summary>
    [JsonPropertyName("range")]
    public string? Range { get; set; }
}
