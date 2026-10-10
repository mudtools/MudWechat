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
