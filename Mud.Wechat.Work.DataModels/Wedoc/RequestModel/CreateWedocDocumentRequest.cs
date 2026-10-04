// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 新建文档请求体（<c>/cgi-bin/wedoc/create_doc</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class CreateWedocDocumentRequest
{
    /// <summary>获取或设置空间 spaceid（官方选填；若指定则 fatherid 需同时指定）。</summary>
    [JsonPropertyName("spaceid")]
    public string? Spaceid { get; set; }

    /// <summary>获取或设置父目录 fileid，根目录时为空间 spaceid（官方选填；若指定 spaceid 则需同时指定）。</summary>
    [JsonPropertyName("fatherid")]
    public string? Fatherid { get; set; }

    /// <summary>
    /// 获取或设置文档类型（官方必填）：3 - 文档，4 - 表格，10 - 智能表格，11 - 智能文档。
    /// <para>新建收集表需使用收集表管理相关接口，不支持通过本字段新建。</para>
    /// </summary>
    [JsonPropertyName("doc_type")]
    public long? DocType { get; set; }

    /// <summary>
    /// 获取或设置文档名字（官方必填）。
    /// <para>官方业务限制：最多 255 个字符，超出会被截断。</para>
    /// </summary>
    [JsonPropertyName("doc_name")]
    public string? DocName { get; set; }

    /// <summary>获取或设置文档管理员 userid 列表（官方 admin_users，选填）。</summary>
    [JsonPropertyName("admin_users")]
    public List<string>? AdminUsers { get; set; }
}
