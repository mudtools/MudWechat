// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.ShortLink;

/// <summary>
/// 「长链接转短链接（旧版）」请求（官方 <c>shorturl</c>）。
/// </summary>
/// <remarks>
/// <para><b>官方已停维</b>：本端点是官方两代短链接口中的旧版（新版为 <c>/cgi-bin/shorten/*</c>，
/// 见 <see cref="MpShortenGenRequest"/>），官方已公告停止维护——仅存量链接兼容场景使用。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShortLink")]
public class MpShortUrlRequest
{
    /// <summary>获取或设置操作类型（官方 <c>action</c>；固定 <c>long2short</c>）。</summary>
    [JsonPropertyName("action")]
    public string Action { get; set; } = "long2short";

    /// <summary>获取或设置需要转换的长链接（官方 <c>long_url</c>，必填；须已通过 ICP 备案）。</summary>
    [JsonPropertyName("long_url")]
    public string LongUrl { get; set; } = string.Empty;
}

/// <summary>
/// 「长链接转短链接（旧版）」响应。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "ShortLink")]
public class MpShortUrlResponse : MpResponse
{
    /// <summary>获取或设置转换后的短链接（官方 <c>short_url</c>）。</summary>
    [JsonPropertyName("short_url")]
    public string? ShortUrl { get; set; }
}
