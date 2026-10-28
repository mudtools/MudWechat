// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>上传资料（<c>upload</c>）请求体（官方 JSON 形态：upload_type=1 文件 url 上传）。</summary>
/// <remarks><c>upload_type=0</c> 二进制上传官方为 multipart/form-data（openid 走 Query、文件走 form-data），
/// 不在 JSON 请求体语义内；URL 上传支持 &lt;50M 文件。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniStore")]
public class ChannelsMiniStoreUploadMaterialRequest
{
    /// <summary>获取或设置上传资料的用户 openid（官方 <c>openid</c>，必填——upload_type=1 时走请求体）。</summary>
    [JsonPropertyName("openid")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>获取或设置上传文件的 url（官方 <c>file_url</c>，upload_type=1 时必填，支持 &lt;50M 的文件）。</summary>
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }
}
