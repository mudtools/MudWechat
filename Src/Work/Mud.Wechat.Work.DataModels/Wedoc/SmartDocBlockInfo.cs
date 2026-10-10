// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 内容块（官方 BlockInfo；添加内容块请求与响应、更新内容块请求与响应、获取内容块列表响应元素共用）。
/// </summary>
/// <remarks>
/// <para>
/// 官方对 Block 的文本内容字段存在<b>同义异参</b>：参数表以及「添加内容块」「更新内容块」的响应示例均写作
/// <c>title</c>，而「更新内容块」的请求示例与「获取内容块列表」的响应示例写作 <c>content</c>。
/// 本模型同时承载 <see cref="Title"/>（<c>title</c>）与 <see cref="Content"/>（<c>content</c>）两种拼写，
/// 照抄官方原文，<b>不得合并为一种</b>（否则按其中一种反序列化将丢失另一侧的文本内容）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocBlockInfo
{
    /// <summary>获取或设置 Block 唯一标识 ID（官方 <c>id</c>，由系统生成）；更新内容块请求中为必填定位字段。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 获取或设置 Block 类型（官方 <c>type</c>，见官方 <c>BlockType</c>）。
    /// 官方取值：<c>BLOCK_TYPE_TEXT</c> 文本、<c>BLOCK_TYPE_IMAGE</c> 图片、<c>BLOCK_TYPE_TODO</c> 待办、
    /// <c>BLOCK_TYPE_NUMBERED_LIST</c> 有序列表、<c>BLOCK_TYPE_BULLETED_LIST</c> 无序列表、<c>BLOCK_TYPE_DIVIDER</c> 分割线、
    /// <c>BLOCK_TYPE_LINK</c> 链接、<c>BLOCK_TYPE_COLUMN_LIST</c> 分栏、<c>BLOCK_TYPE_HEADER_1</c>～<c>BLOCK_TYPE_HEADER_6</c> 一至六级标题、
    /// <c>BLOCK_TYPE_HIGHLIGHT</c> 背景块、<c>BLOCK_TYPE_FILE</c> 文件、<c>BLOCK_TYPE_VIEW</c> 视图、
    /// <c>BLOCK_TYPE_TABLE</c> 表格、<c>BLOCK_TYPE_CODE</c> 代码块、<c>BLOCK_TYPE_QUOTE</c> 引用块。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// 获取或设置 Block 的文本内容（官方参数表口径 <c>title</c>）：添加内容块请求、
    /// 添加 / 更新内容块响应、以及「获取内容块列表」响应中以本字段承载文本。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置 Block 的文本内容（官方示例口径 <c>content</c>）：「更新内容块」请求示例
    /// 与「获取内容块列表」响应示例以本字段承载文本，与 <see cref="Title"/> 并存。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>获取或设置父节点 Block ID（官方 <c>parent_id</c>）。</summary>
    [JsonPropertyName("parent_id")]
    public string? ParentId { get; set; }

    /// <summary>
    /// 获取或设置插入位置（官方 <c>after_id</c>），表示排在哪个 Block 之后；为空表示插入到最前。
    /// </summary>
    [JsonPropertyName("after_id")]
    public string? AfterId { get; set; }

    /// <summary>获取或设置子节点 ID 列表（官方 <c>children</c>）。</summary>
    [JsonPropertyName("children")]
    public List<string>? Children { get; set; }

    /// <summary>获取或设置 Block 属性（官方 <c>props</c>），按 <see cref="Type"/> 取对应属性对象。</summary>
    [JsonPropertyName("props")]
    public SmartDocBlockProps? Props { get; set; }
}
