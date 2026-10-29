// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>
/// 查询老年患者实名信息回包（<c>POST /cityservice/getmedrealname</c>）。
/// </summary>
/// <remarks>
/// <para><b>加密而非明文</b>：<c>cipher_real_name</c> 为 base64 密文，默认算法
/// <c>AES_256_ECB_PKCS7Padding</c>；解密后明文含 <c>real_name</c> / <c>id_card_no</c> /
/// <c>id_card_type</c>（1 居民身份证 / 4 澳门居民往来内地通行证 / 5 台湾居民往来内地通行证 /
/// 6 香港居民往来内地通行证）/ <c>phone</c> / <c>timestamp</c>。</para>
/// <para><b>官方返回示例与字段表不一致（照录）</b>：示例仅给 <c>app_id</c> 与 <c>openid_id</c>
/// 两个空字段，与字段表中的 <c>cipher_real_name</c> 等不符 ⇒ 本类按字段表建模。</para>
/// <para>错误码：11200 <c>wxmed_authcode</c> 已过期（有效期 10 分钟）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CityService")]
public class WxaCityServiceGetMedRealNameResponse : WxaResponse
{
    /// <summary>加密后的实名信息（base64 密文）。敏感字段，禁入日志。</summary>
    [JsonPropertyName("cipher_real_name")]
    public string? CipherRealName { get; set; }

    /// <summary>加密算法，默认 <c>AES_256_ECB_PKCS7Padding</c>。</summary>
    [JsonPropertyName("cipher_algorithm")]
    public string? CipherAlgorithm { get; set; }

    /// <summary>实名信息加密 key 版本号，初始版本号为 0（用于识别后续密钥升级）。</summary>
    [JsonPropertyName("key_version")]
    public int? KeyVersion { get; set; }

    /// <summary>业务方 appid。</summary>
    [JsonPropertyName("app_id")]
    public string? AppId { get; set; }

    /// <summary>微信用户 openid。敏感字段，禁入日志。</summary>
    [JsonPropertyName("open_id")]
    public string? OpenId { get; set; }
}
