// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 编辑文档内容请求体（<c>/cgi-bin/wedoc/document/batch_update</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class BatchUpdateWedocDocumentRequest
{
    /// <summary>获取或设置文档的 docid（官方必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>
    /// 获取或设置操作的文档版本，可通过获取文档数据接口获得；操作后版本更新一版（官方选填）。
    /// <para>官方业务限制：要更新的文档版本与最新文档版本相差不能超过 100 个。</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方文档参数表标注字段名为 <c>version</c>，但请求示例原文拼写为 <c>verison</c>，
    /// 依照本仓「以官方 JSON 示例为准」的口径，本模型以 <c>verison</c> 承载（拼写陷阱，同 <c>universal_domian</c>）。
    /// </para>
    /// </remarks>
    [JsonPropertyName("verison")]
    public long? Verison { get; set; }

    /// <summary>
    /// 获取或设置更新操作列表（官方 requests，<see cref="WedocDocumentUpdateOperation"/>，官方必填）。
    /// <para>官方业务限制：单次批量更新操作数量 &lt;= 30；批量更新请求中若有一个操作报错则全部更新操作不生效。</para>
    /// </summary>
    [JsonPropertyName("requests")]
    public List<WedocDocumentUpdateOperation>? Requests { get; set; }
}
