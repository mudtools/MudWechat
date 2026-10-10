// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档内容节点（官方 Node；<c>/cgi-bin/wedoc/document/get</c> 响应 document 根节点，children 递归承载）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocumentNode
{
    /// <summary>获取或设置节点起始位置（官方 begin，从 0 开始）。</summary>
    [JsonPropertyName("begin")]
    public long? Begin { get; set; }

    /// <summary>获取或设置节点结束位置（官方 end）。</summary>
    [JsonPropertyName("end")]
    public long? End { get; set; }

    /// <summary>获取或设置节点属性（官方 property，<see cref="WedocDocumentProperty"/>）。</summary>
    [JsonPropertyName("property")]
    public WedocDocumentProperty? Property { get; set; }

    /// <summary>
    /// 获取或设置节点类型（官方 type）：Document - 文档，MainStory - 文档主节点，Section - 节，
    /// Paragraph - 段落，Table - 表格，TableRow - 表格行，TableCell - 单元格，
    /// Text - 相同属性文本容器，Drawing - 图形化对象（如图片）。
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>获取或设置子节点列表（官方 children，<see cref="WedocDocumentNode"/>）。</summary>
    [JsonPropertyName("children")]
    public List<WedocDocumentNode>? Children { get; set; }

    /// <summary>获取或设置文本内容，当节点类型为 Text 时有效（官方 text）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}
