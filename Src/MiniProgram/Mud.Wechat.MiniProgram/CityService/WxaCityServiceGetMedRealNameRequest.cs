// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 查询老年患者实名信息请求体（<c>POST /cityservice/getmedrealname</c>）。
/// </summary>
/// <remarks>
/// <para><b>实名数据全程加密</b>：回包不是明文实名，而是平台用每家医院专属 256bit 密钥
/// 按 <c>AES_256_ECB_PKCS7Padding</c> 加密后的 base64 串；SDK 不持有该密钥，不代解密。</para>
/// <para><b>敏感面</b>：<c>wxmed_authcode</c> / <c>open_id</c> 与解密后的姓名、证件号、手机号
/// 一律禁入日志、遥测与异常消息。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceGetMedRealNameRequest
{
    /// <summary>业务方 appid（必填）。</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <summary>微信用户 openid（必填）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }

    /// <summary>实名信息 code（必填），对应 URL 中的 <c>wxmed_authcode</c>，<b>有效期 10 分钟</b>。敏感字段，禁入日志。</summary>
    [JsonPropertyName("wxmed_authcode")]
    public string? WxmedAuthcode { get; set; }
}
