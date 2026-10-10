// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 表格内容块属性（官方 TableBlockProps，对应 <c>BLOCK_TYPE_TABLE</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocTableBlockProps
{
    /// <summary>获取或设置单元格合并信息（官方 <c>cell_positions</c>）。</summary>
    [JsonPropertyName("cell_positions")]
    public List<SmartDocTableCellMerge>? CellPositions { get; set; }

    /// <summary>获取或设置是否启用行标题（官方 <c>enable_row_header</c>）。</summary>
    [JsonPropertyName("enable_row_header")]
    public bool? EnableRowHeader { get; set; }

    /// <summary>获取或设置是否启用列标题（官方 <c>enable_column_header</c>）。</summary>
    [JsonPropertyName("enable_column_header")]
    public bool? EnableColumnHeader { get; set; }
}
