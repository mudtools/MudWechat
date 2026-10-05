// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 提交导出任务请求体（<c>/cgi-bin/wedoc/smartdoc/export_task</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方请求示例中额外出现了 <c>url</c> 字段，但官方参数表未定义该字段；
/// 本仓以官方参数表为准，不承载 <c>url</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class ExportSmartDocTaskRequest
{
    /// <summary>获取或设置智能文档的文档 ID（官方 <c>docid</c>，必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>
    /// 获取或设置期望返回的内容格式类型（官方 <c>content_type</c>，必填）。
    /// 官方取值：<c>1</c> Markdown 格式（目前仅支持 Markdown）。
    /// </summary>
    [JsonPropertyName("content_type")]
    public uint? ContentType { get; set; }

    /// <summary>
    /// 获取或设置指定要导出的 Page ID（官方 <c>page_id</c>，非必填）。
    /// <para>不传时导出整个智能文档；传入时仅导出该 Page 及其所有子 Block 的内容。</para>
    /// </summary>
    [JsonPropertyName("page_id")]
    public string? PageId { get; set; }
}
