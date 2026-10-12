// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>获取用户 <c>encryptKey</c> 请求体（<c>POST /wxa/business/getuserencryptkey</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>user-info/internet/api_getuserencryptkey.html</c>。</para>
/// <para>
/// <b>签名算法（官方原文）</b>：<c>signature = hmac_sha256(session_key, openid)</c>，
/// <c>sig_method</c> 固定 <c>hmac_sha256</c>；<c>session_key</c> 由 <c>code2Session</c> 获取。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetUserEncryptKeyRequest
{
    /// <summary>用户唯一标识（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>会话签名（<c>signature</c>，必填；<c>hmac_sha256(session_key, openid)</c>）。</summary>
    [JsonPropertyName("signature")]
    public string? Signature { get; set; }

    /// <summary>签名算法（<c>sig_method</c>，必填；固定 <see cref="WxaSignatureMethods.HmacSha256"/>）。</summary>
    [JsonPropertyName("sig_method")]
    public string? SigMethod { get; set; }
}
