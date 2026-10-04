// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文档基础信息（<c>/cgi-bin/wedoc/get_doc_base_info</c> 响应的 doc_base_info 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocDocBaseInfo
{
    /// <summary>获取或设置文档 docid。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置文档名字。</summary>
    [JsonPropertyName("doc_name")]
    public string? DocName { get; set; }

    /// <summary>获取或设置文档创建时间。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置文档最后修改时间。</summary>
    [JsonPropertyName("modify_time")]
    public long? ModifyTime { get; set; }

    /// <summary>获取或设置文档类型：3 - 文档，4 - 表格，10 - 智能表格。</summary>
    [JsonPropertyName("doc_type")]
    public long? DocType { get; set; }
}
