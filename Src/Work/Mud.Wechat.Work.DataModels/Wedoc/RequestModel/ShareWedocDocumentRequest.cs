// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 分享文档请求体（<c>/cgi-bin/wedoc/doc_share</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class ShareWedocDocumentRequest
{
    /// <summary>
    /// 获取或设置文档 id（官方 docid 与 formid 只能填其中一个；只能访问该应用创建的文档）。
    /// </summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>
    /// 获取或设置收集表 id（官方 formid 与 docid 只能填其中一个）。
    /// </summary>
    [JsonPropertyName("formid")]
    public string? Formid { get; set; }
}
