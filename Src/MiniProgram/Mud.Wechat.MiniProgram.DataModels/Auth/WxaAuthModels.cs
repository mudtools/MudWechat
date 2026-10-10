// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>
/// 登录凭证校验应答（<c>GET /sns/jscode2session</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<c>user-login/api_code2session.html</c>（2026-10-09 逐字段核验）。
/// </para>
/// <para>
/// <b>红线 MP-X7</b>：<see cref="SessionKey"/> 是用户<b>会话密钥</b>——
/// <b>不得</b>入日志 / 遥测 / 异常消息，<b>不得</b>下发到前端。本 DTO 刻意不重写 <c>ToString</c>
/// 以免被「顺手」用于日志。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaCode2SessionResponse : WxaResponse
{
    /// <summary>用户唯一标识（<c>openid</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>
    /// 会话密钥（<c>session_key</c>）。<b>敏感：不得入日志、不得下发前端</b>（MP-X7）。
    /// </summary>
    [JsonPropertyName("session_key")]
    public string? SessionKey { get; set; }

    /// <summary>用户在开放平台的唯一标识（<c>unionid</c>；未绑定开放平台或未关注同主体公众号时缺省）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
}

/// <summary>获取手机号请求体（<c>POST /wxa/business/getuserphonenumber</c>）。</summary>
/// <remarks>
/// <b><c>code</c> 一次性</b>：由小程序端 <c>&lt;button open-type="getPhoneNumber"&gt;</c> 回调取得，
/// 有效期与可用次数由官方表达；<b>不可复用</b>（重复使用会被官方拒绝）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetPhoneNumberRequest
{
    /// <summary>手机号获取凭证（<c>code</c>，必填），前端 <c>getPhoneNumber</c> 回调返回。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
}

/// <summary>获取手机号应答（<c>POST /wxa/business/getuserphonenumber</c>）。</summary>
/// <remarks>
/// <b>计费与限额</b>：本能力按次计费且受次数上限约束（官方原文），失败时 <c>phone_info</c> 整体缺省。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetPhoneNumberResponse : WxaResponse
{
    /// <summary>手机号信息（<c>phone_info</c>；失败时缺省），见 <see cref="WxaPhoneInfo"/>。</summary>
    [JsonPropertyName("phone_info")]
    public WxaPhoneInfo? PhoneInfo { get; set; }
}

/// <summary>手机号信息（<c>phone_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaPhoneInfo
{
    /// <summary>手机号（<c>phoneNumber</c>，<b>带区号</b>，如 <c>+86 13800000000</c>）。</summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    /// <summary>不含区号的手机号（<c>purePhoneNumber</c>）。</summary>
    [JsonPropertyName("purePhoneNumber")]
    public string? PurePhoneNumber { get; set; }

    /// <summary>区号（<c>countryCode</c>）。</summary>
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    /// <summary>数据水印（<c>watermark</c>，官方要求校验），见 <see cref="WxaWatermark"/>。</summary>
    [JsonPropertyName("watermark")]
    public WxaWatermark? Watermark { get; set; }
}

/// <summary>数据水印（<c>watermark</c>；官方要求宿主校验 <c>appid</c> 以防数据来源伪造）。</summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaWatermark
{
    /// <summary>数据获取时间戳（<c>timestamp</c>，秒级）。</summary>
    [JsonPropertyName("timestamp")]
    public long? Timestamp { get; set; }

    /// <summary>数据来源小程序 AppID（<c>appid</c>）—— 必须与自身一致。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }
}

/// <summary>支付后获取 UnionID 应答（<c>GET /wxa/getpaidunionid</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetPaidUnionIdResponse : WxaResponse
{
    /// <summary>用户在开放平台的唯一标识（<c>unionid</c>）。</summary>
    [JsonPropertyName("unionid")]
    public string? UnionId { get; set; }
}

/// <summary>获取插件用户 <c>openpid</c> 应答（<c>GET /wxa/getpluginopenpid</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>user-info/basic-info/api_getpluginopenpid.html</c>。</para>
/// <para>
/// <b>前置约束（官方原文）</b>：须先经小程序端 <c>wx.pluginLogin</c> 取得插件用户标志凭证
/// <c>code</c> 再传至开发者服务器；<c>plugin_appid</c> 为插件 AppID。
/// 返回的 <c>openpid</c> 是<b>插件维度</b>的用户唯一标识（同用户在不同插件下 openpid 不同）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetPluginOpenPidResponse : WxaResponse
{
    /// <summary>插件用户唯一标识（<c>openpid</c>）。</summary>
    [JsonPropertyName("openpid")]
    public string? OpenPid { get; set; }
}

/// <summary>检查加密信息请求体（<c>POST /wxa/business/checkencryptedmsg</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>user-info/basic-info/api_checkencrypteddata.html</c>。</para>
/// <para>
/// <b>能力边界（官方原文）</b>：当前只支持<b>手机号加密数据</b>，且只能检测<b>最近 3 天</b>生成的加密数据。
/// <c>encrypted_msg_hash</c> 为 <c>to_hexstr(sha1(encrypted_msg))</c>（对加密消息原文做 SHA1 后转十六进制）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaCheckEncryptedMsgRequest
{
    /// <summary>加密消息哈希（<c>encrypted_msg_hash</c>，必填；<c>SHA1(encrypted_msg)</c> 的十六进制串）。</summary>
    [JsonPropertyName("encrypted_msg_hash")]
    public string? EncryptedMsgHash { get; set; }

    /// <summary>用户唯一标识（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}

/// <summary>检查加密信息应答（<c>POST /wxa/business/checkencryptedmsg</c>）。</summary>
/// <remarks>
/// <b>官方字段名照录</b>：返回字段是 <c>vaild</c>（官方原文拼写，非 <c>valid</c>），
/// 取值 <c>true</c> = 微信生成、<c>false</c> = 非微信生成或已超 3 天。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaCheckEncryptedMsgResponse : WxaResponse
{
    /// <summary>是否由微信生成（<c>vaild</c>，官方原文拼写）。</summary>
    [JsonPropertyName("vaild")]
    public bool? Vaild { get; set; }
}

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

/// <summary>获取用户 <c>encryptKey</c> 应答（<c>POST /wxa/business/getuserencryptkey</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaGetUserEncryptKeyResponse : WxaResponse
{
    /// <summary>密钥信息列表（<c>key_info_list</c>），见 <see cref="WxaUserEncryptKeyInfo"/>。</summary>
    [JsonPropertyName("key_info_list")]
    public List<WxaUserEncryptKeyInfo>? KeyInfoList { get; set; }
}

/// <summary>用户 <c>encryptKey</c> 信息条目（<c>key_info_list[]</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaUserEncryptKeyInfo
{
    /// <summary>加密密钥（<c>encrypt_key</c>）。</summary>
    [JsonPropertyName("encrypt_key")]
    public string? EncryptKey { get; set; }

    /// <summary>密钥版本（<c>version</c>）。</summary>
    [JsonPropertyName("version")]
    public long? Version { get; set; }

    /// <summary>剩余有效时间（<c>expire_in</c>，秒）。</summary>
    [JsonPropertyName("expire_in")]
    public long? ExpireIn { get; set; }

    /// <summary>加密初始化向量（<c>iv</c>）。</summary>
    [JsonPropertyName("iv")]
    public string? Iv { get; set; }
}
