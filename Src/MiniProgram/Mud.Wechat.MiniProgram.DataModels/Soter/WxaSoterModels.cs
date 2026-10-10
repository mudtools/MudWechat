// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Soter;

/// <summary>生物认证秘钥签名验证请求体（<c>POST /cgi-bin/soter/verify_signature</c>）。</summary>
/// <remarks>官方文档：<c>soter/api_verifysignature.html</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Soter")]
public class WxaSoterVerifySignatureRequest
{
    /// <summary>用户唯一标识（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 通过 <c>wx.startSoterAuthentication</c> 成功回调获得的 <c>resultJSON</c> 字段
    /// （<c>json_string</c>，必填）。
    /// </summary>
    [JsonPropertyName("json_string")]
    public string? JsonString { get; set; }

    /// <summary>
    /// 通过 <c>wx.startSoterAuthentication</c> 成功回调获得的 <c>resultJSONSignature</c> 字段
    /// （<c>json_signature</c>，必填）。
    /// </summary>
    [JsonPropertyName("json_signature")]
    public string? JsonSignature { get; set; }
}

/// <summary>生物认证秘钥签名验证应答（<c>is_ok</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Soter")]
public class WxaSoterVerifySignatureResponse : WxaResponse
{
    /// <summary>生物认证秘钥签名验证是否通过（<c>is_ok</c>）。</summary>
    [JsonPropertyName("is_ok")]
    public bool? IsOk { get; set; }
}