// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 修改文档安全设置请求体（<c>/cgi-bin/wedoc/mod_doc_safty_setting</c>；官方路由 safty 疑为 safety 笔误，照抄官方原文）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UpdateWedocDocSafetySettingRequest
{
    /// <summary>获取或设置操作的文档 id（官方 <c>docid</c>，必填）。</summary>
    [JsonPropertyName("docid")]
    public string? Docid { get; set; }

    /// <summary>获取或设置是否允许只读成员复制、下载文档（官方 <c>enable_readonly_copy</c>），有值则覆盖。</summary>
    [JsonPropertyName("enable_readonly_copy")]
    public bool? EnableReadonlyCopy { get; set; }

    /// <summary>获取或设置水印设置（官方 <c>watermark</c>），有值则覆盖。</summary>
    [JsonPropertyName("watermark")]
    public WedocDocWatermark? Watermark { get; set; }
}
