// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 上传图片请求体（<c>/cgi-bin/wedoc/image_upload</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：图片内容以 Base64 形态承载于 <c>base64_content</c>（字段名即为 base64，
/// SDK 侧按字符串透传、不做编解码，也不做本地大小硬拦，越界由应答 <c>errcode</c> 表达）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UploadWedocImageRequest
{
    /// <summary>获取或设置文档 docid（官方必填；图片上传至该文档，官方口径「通过新建文档接口创建后获得」）。</summary>
    [JsonPropertyName("docid")]
    public string Docid { get; set; } = string.Empty;

    /// <summary>获取或设置图片内容的 Base64 编码（官方必填；字段名官方为 base64_content，无 file_ 前缀）。</summary>
    [JsonPropertyName("base64_content")]
    public string Base64Content { get; set; } = string.Empty;
}
