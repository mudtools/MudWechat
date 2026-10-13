// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 文件分块上传请求体（<c>/cgi-bin/wedoc/file_upload_part</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约：本端点仅返回 errcode / errmsg（无业务负载）；分块内容字段名沿用整文件上传的
/// <c>file_base64_content</c>，SDK 侧按字符串透传该分块的 Base64。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class UploadWedocFilePartRequest
{
    /// <summary>获取或设置文件上传凭证（官方必填；分块上传初始化在未命中秒传时返回的 upload_key）。</summary>
    [JsonPropertyName("upload_key")]
    public string UploadKey { get; set; } = string.Empty;

    /// <summary>获取或设置文件分块号（官方必填；内容按固定 2M 分块，<b>分块号从 1 开始</b>，官方口径如此，非 0 基）。
    /// <para>官方建议：分块可并发上传，<b>并发数不超过 10</b>。</para></summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }

    /// <summary>获取或设置该分块内容的 Base64 编码（官方必填；同样只填 Base64 本体，不得携带数据类型描述信息）。</summary>
    [JsonPropertyName("file_base64_content")]
    public string FileBase64Content { get; set; } = string.Empty;
}
