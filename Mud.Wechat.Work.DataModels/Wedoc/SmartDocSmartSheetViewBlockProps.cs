// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 数据表视图内容块属性（官方 SmartSheetViewBlockProps，对应 <c>BLOCK_TYPE_VIEW</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocSmartSheetViewBlockProps
{
    /// <summary>获取或设置数据表所属文档 ID（官方 <c>doc_id</c>）。</summary>
    [JsonPropertyName("doc_id")]
    public string? DocId { get; set; }

    /// <summary>获取或设置数据表 ID（官方 <c>table_id</c>）。</summary>
    [JsonPropertyName("table_id")]
    public string? TableId { get; set; }

    /// <summary>获取或设置视图 ID（官方 <c>view_id</c>）。</summary>
    [JsonPropertyName("view_id")]
    public string? ViewId { get; set; }

    /// <summary>获取或设置数据表的 Block ID（官方 <c>table_block_id</c>）。</summary>
    [JsonPropertyName("table_block_id")]
    public string? TableBlockId { get; set; }

    /// <summary>获取或设置是否显示视图标题（官方 <c>enable_table_title</c>）。</summary>
    [JsonPropertyName("enable_table_title")]
    public bool? EnableTableTitle { get; set; }

    /// <summary>获取或设置是否显示添加记录按钮（官方 <c>enable_add_row</c>）。</summary>
    [JsonPropertyName("enable_add_row")]
    public bool? EnableAddRow { get; set; }
}
