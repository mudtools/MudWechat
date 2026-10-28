// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Security;

/// <summary>音视频内容安全识别请求体（<c>POST /wxa/media_check_async</c>）。</summary>
/// <remarks><c>media_url</c> 须为<b>公网可访问</b>地址（官方回源拉取）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Security")]
public class WxaMediaCheckAsyncRequest
{
    /// <summary>要检测的媒体地址（<c>media_url</c>，必填；公网可访问、无需鉴权）。</summary>
    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; set; }

    /// <summary>媒体类型（<c>media_type</c>，必填）：<c>1</c> 音频 / <c>2</c> 图片（官方枚举）。</summary>
    [JsonPropertyName("media_type")]
    public long? MediaType { get; set; }

    /// <summary>接口版本（<c>version</c>，选填；当前为 <c>2</c>）。</summary>
    [JsonPropertyName("version")]
    public long? Version { get; set; }

    /// <summary>场景值（<c>scene</c>，选填，取值同文本审核）。</summary>
    [JsonPropertyName("scene")]
    public long? Scene { get; set; }

    /// <summary>用户 <c>openid</c>（<c>openid</c>，选填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
