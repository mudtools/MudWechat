// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档内容节点属性（官方 Property；各节点按类型仅承载其中一组属性）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentProperty
{
    /// <summary>获取或设置节属性（官方 section_property，<see cref="WedocDocumentSectionProperty"/>）。</summary>
    [JsonPropertyName("section_property")]
    public WedocDocumentSectionProperty? SectionProperty { get; set; }

    /// <summary>获取或设置段落属性（官方 paragraph_property，<see cref="WedocDocumentParagraphProperty"/>）。</summary>
    [JsonPropertyName("paragraph_property")]
    public WedocDocumentParagraphProperty? ParagraphProperty { get; set; }

    /// <summary>获取或设置文本属性（官方 run_property，<see cref="WedocDocumentRunProperty"/>）。</summary>
    [JsonPropertyName("run_property")]
    public WedocDocumentRunProperty? RunProperty { get; set; }

    /// <summary>获取或设置表格属性（官方 table_property，<see cref="WedocDocumentTableProperty"/>）。</summary>
    [JsonPropertyName("table_property")]
    public WedocDocumentTableProperty? TableProperty { get; set; }

    /// <summary>获取或设置表格行属性（官方 table_row_property，<see cref="WedocDocumentTableRowProperty"/>）。</summary>
    [JsonPropertyName("table_row_property")]
    public WedocDocumentTableRowProperty? TableRowProperty { get; set; }

    /// <summary>获取或设置单元格属性（官方 table_cell_property，<see cref="WedocDocumentTableCellProperty"/>）。</summary>
    [JsonPropertyName("table_cell_property")]
    public WedocDocumentTableCellProperty? TableCellProperty { get; set; }

    /// <summary>获取或设置图形化对象属性（官方 drawing_property，<see cref="WedocDocumentDrawingProperty"/>）。</summary>
    [JsonPropertyName("drawing_property")]
    public WedocDocumentDrawingProperty? DrawingProperty { get; set; }
}
