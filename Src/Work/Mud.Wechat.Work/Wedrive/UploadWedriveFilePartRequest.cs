// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedrive;

/// <summary>
/// 分块上传文件请求体（<c>/cgi-bin/wedrive/file_upload_part</c>）。
/// </summary>
/// <remarks>
/// <para>官方契约：文件内容按 2M 分块，index 从 1 开始；分块可并发上传，官方建议并发数不超过 10。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedrive")]
public class UploadWedriveFilePartRequest
{
    /// <summary>获取或设置文件上传凭证（官方必填；分块上传初始化返回的 upload_key）。</summary>
    [JsonPropertyName("upload_key")]
    public string? UploadKey { get; set; }

    /// <summary>获取或设置文件分块号（官方必填；文件内容按 2M 分块，从 1 开始）。</summary>
    [JsonPropertyName("index")]
    public int? Index { get; set; }

    /// <summary>
    /// 获取或设置分块的文件内容 Base64（官方必填；只填该分块内容的 Base64，
    /// 不要添加数据类型描述信息前缀）。
    /// </summary>
    [JsonPropertyName("file_base64_content")]
    public string? FileBase64Content { get; set; }
}
