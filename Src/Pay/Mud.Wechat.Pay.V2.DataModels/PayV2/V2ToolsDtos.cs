// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.DataModels.PayV2;

/// <summary>
/// APIv2 付款码查询 openid 请求体。
/// </summary>
/// <remarks>官方本页字段面就 5 个（无 <c>sign_type</c>、无服务商 <c>sub_*</c> 字段），照抄不得扩充。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2AuthCodeToOpenIdRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>付款码：扫码支付的条码/二维码原文（String(128)）。</summary>
    [JsonPropertyName("auth_code")] public string? AuthCode { get; set; }

    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
}

/// <summary>
/// APIv2 付款码查询 openid 应答。
/// </summary>
/// <remarks>本支应答<b>没有</b> <c>err_code_des</c>（官方页面如此），失败原因只在 <c>err_code</c> 里给。</remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2AuthCodeToOpenIdResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码：SYSTEMERROR / AUTHCODEEXPIRE / AUTH_CODE_ERROR / AUTH_CODE_INVALID。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>用户在商户 appid 下的唯一标识。<b>属用户身份标识，不得进日志与异常消息。</b></summary>
    [JsonPropertyName("openid")] public string? OpenId { get; set; }
}

/// <summary>
/// APIv2 转换短链接请求体。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ShortUrlRequest
{
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }

    /// <summary>待转换的长链接（String(512)）：<b>签名用原串，传输需 URLencode</b>（官方原文）。</summary>
    [JsonPropertyName("long_url")] public string? LongUrl { get; set; }

    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("sign_type")] public string? SignType { get; set; }
}

/// <summary>
/// APIv2 转换短链接应答。
/// </summary>
/// <remarks>
/// <para><b>路径是 <c>/tools/shorturl</c> 而字段是 <c>short_url</c></b>（下划线只在字段名上），
/// 且官方把 <c>short_url</c> 与 <c>result_code</c> 放在<b>同一张</b>由 <c>return_code</c> 把关的表里
/// （没有第三级 result_code 门控块）。</para>
/// <para>本支应答<b>没有</b> <c>err_code_des</c>。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "PayV2")]
public class V2ShortUrlResponse
{
    [JsonPropertyName("return_code")] public string? ReturnCode { get; set; }
    [JsonPropertyName("return_msg")] public string? ReturnMsg { get; set; }
    [JsonPropertyName("appid")] public string? AppId { get; set; }
    [JsonPropertyName("mch_id")] public string? MchId { get; set; }
    [JsonPropertyName("nonce_str")] public string? NonceStr { get; set; }
    [JsonPropertyName("sign")] public string? Sign { get; set; }
    [JsonPropertyName("result_code")] public string? ResultCode { get; set; }

    /// <summary>错误代码：SYSTEMERROR / URLFORMATERROR。</summary>
    [JsonPropertyName("err_code")] public string? ErrCode { get; set; }

    /// <summary>转换后的短链接（weixin://wxpay/s/XXXXXX）。</summary>
    [JsonPropertyName("short_url")] public string? ShortUrl { get; set; }
}
