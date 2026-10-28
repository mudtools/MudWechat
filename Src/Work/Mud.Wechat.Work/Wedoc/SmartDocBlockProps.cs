// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 内容块属性容器（官方 BlockProps）。
/// <para>按 <c>BlockInfo.type</c> 取对应属性对象：文本 <c>text_props</c>、图片 <c>image_props</c>、文件 <c>file_props</c>、
/// 链接 <c>link_props</c>、表格 <c>table_props</c>、数据表视图 <c>view_props</c>、分栏 <c>column_props</c>、
/// 背景块 <c>highlight_props</c>、代码块 <c>code_props</c>、待办 <c>todo_props</c>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 官方对 BlockProps 内各属性字段未标注必填性，一律按「非必填」承载；
/// 有序列表、无序列表、分割线、一至六级标题、引用块等类型无对应属性对象。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocBlockProps
{
    /// <summary>获取或设置文本属性（官方 <c>text_props</c>），对应 <c>BLOCK_TYPE_TEXT</c>。</summary>
    [JsonPropertyName("text_props")]
    public SmartDocTextBlockProps? TextProps { get; set; }

    /// <summary>获取或设置图片属性（官方 <c>image_props</c>），对应 <c>BLOCK_TYPE_IMAGE</c>。</summary>
    [JsonPropertyName("image_props")]
    public SmartDocImageBlockProps? ImageProps { get; set; }

    /// <summary>获取或设置文件属性（官方 <c>file_props</c>），对应 <c>BLOCK_TYPE_FILE</c>。</summary>
    [JsonPropertyName("file_props")]
    public SmartDocFileBlockProps? FileProps { get; set; }

    /// <summary>获取或设置链接属性（官方 <c>link_props</c>），对应 <c>BLOCK_TYPE_LINK</c>。</summary>
    [JsonPropertyName("link_props")]
    public SmartDocLinkBlockProps? LinkProps { get; set; }

    /// <summary>获取或设置表格属性（官方 <c>table_props</c>），对应 <c>BLOCK_TYPE_TABLE</c>。</summary>
    [JsonPropertyName("table_props")]
    public SmartDocTableBlockProps? TableProps { get; set; }

    /// <summary>获取或设置数据表视图属性（官方 <c>view_props</c>），对应 <c>BLOCK_TYPE_VIEW</c>。</summary>
    [JsonPropertyName("view_props")]
    public SmartDocSmartSheetViewBlockProps? ViewProps { get; set; }

    /// <summary>获取或设置分栏属性（官方 <c>column_props</c>），对应 <c>BLOCK_TYPE_COLUMN_LIST</c>。</summary>
    [JsonPropertyName("column_props")]
    public SmartDocColumnListProps? ColumnProps { get; set; }

    /// <summary>获取或设置背景属性（官方 <c>highlight_props</c>），对应 <c>BLOCK_TYPE_HIGHLIGHT</c>。</summary>
    [JsonPropertyName("highlight_props")]
    public SmartDocHighlightBlockProps? HighlightProps { get; set; }

    /// <summary>获取或设置代码属性（官方 <c>code_props</c>），对应 <c>BLOCK_TYPE_CODE</c>。</summary>
    [JsonPropertyName("code_props")]
    public SmartDocCodeBlockProps? CodeProps { get; set; }

    /// <summary>获取或设置待办属性（官方 <c>todo_props</c>），对应 <c>BLOCK_TYPE_TODO</c>。</summary>
    [JsonPropertyName("todo_props")]
    public SmartDocTodoBlockProps? TodoProps { get; set; }
}
