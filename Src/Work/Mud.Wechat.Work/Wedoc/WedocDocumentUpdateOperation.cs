// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 编辑文档内容的单个更新操作（官方 UpdateRequest）。
/// <para>每个操作对象只能同时填一个字段，填多个仅一个生效。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentUpdateOperation
{
    /// <summary>获取或设置替换指定位置文本内容操作（官方 replace_text，<see cref="WedocDocumentReplaceText"/>）。</summary>
    [JsonPropertyName("replace_text")]
    public WedocDocumentReplaceText? ReplaceText { get; set; }

    /// <summary>获取或设置在指定位置插入文本内容操作（官方 insert_text，<see cref="WedocDocumentInsertText"/>）。</summary>
    [JsonPropertyName("insert_text")]
    public WedocDocumentInsertText? InsertText { get; set; }

    /// <summary>获取或设置删除指定位置内容操作（官方 delete_content，<see cref="WedocDocumentDeleteContent"/>）。</summary>
    [JsonPropertyName("delete_content")]
    public WedocDocumentDeleteContent? DeleteContent { get; set; }

    /// <summary>获取或设置在指定位置插入图片操作（官方 insert_image，<see cref="WedocDocumentInsertImage"/>）。</summary>
    [JsonPropertyName("insert_image")]
    public WedocDocumentInsertImage? InsertImage { get; set; }

    /// <summary>获取或设置在指定位置插入分页符操作（官方 insert_page_break，<see cref="WedocDocumentInsertPageBreak"/>）。</summary>
    [JsonPropertyName("insert_page_break")]
    public WedocDocumentInsertPageBreak? InsertPageBreak { get; set; }

    /// <summary>获取或设置在指定位置插入表格操作（官方 insert_table，<see cref="WedocDocumentInsertTable"/>）。</summary>
    [JsonPropertyName("insert_table")]
    public WedocDocumentInsertTable? InsertTable { get; set; }

    /// <summary>获取或设置在指定位置插入段落操作（官方 insert_paragraph，<see cref="WedocDocumentInsertParagraph"/>）。</summary>
    [JsonPropertyName("insert_paragraph")]
    public WedocDocumentInsertParagraph? InsertParagraph { get; set; }

    /// <summary>获取或设置更新指定位置文本属性操作（官方 update_text_property，<see cref="WedocDocumentUpdateTextProperty"/>）。</summary>
    [JsonPropertyName("update_text_property")]
    public WedocDocumentUpdateTextProperty? UpdateTextProperty { get; set; }
}
