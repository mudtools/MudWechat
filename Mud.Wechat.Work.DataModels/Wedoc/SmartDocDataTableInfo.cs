// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 数据表信息（官方 <c>info</c>；添加数据表响应与更新数据表响应共用）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartDocDataTableInfo
{
    /// <summary>获取或设置数据表对应的 Block ID（官方 <c>block_id</c>，由系统生成）。</summary>
    [JsonPropertyName("block_id")]
    public string? BlockId { get; set; }

    /// <summary>获取或设置数据表标题（官方 <c>title</c>）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置数据表的 sheet_id（官方 <c>sheet_id</c>）。</summary>
    [JsonPropertyName("sheet_id")]
    public string? SheetId { get; set; }

    /// <summary>获取或设置智能文档的 docid（官方 <c>ss_docid</c>，即该数据表所属智能文档）。</summary>
    [JsonPropertyName("ss_docid")]
    public string? SsDocid { get; set; }

    /// <summary>获取或设置排序位置（官方 <c>after_id</c>），表示数据表位于哪个 Block 之后。</summary>
    [JsonPropertyName("after_id")]
    public string? AfterId { get; set; }
}
