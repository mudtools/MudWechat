// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PasswordExemptContract;

/// <summary>清算机构模式小程序场景预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471282"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptQsMiniProgramPreSignRequest
{
    /// <summary>【商户AppID】 从业机构商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【子商户AppID】 从业机构的子商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>sub_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【回调通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【用户标识】 用户在商户AppID下的唯一标识（openid和sub_openid字段二选一必填）（官方字段路径 <c>openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【用户子标识】 用户在子商户AppID下的唯一标识（openid和sub_openid字段二选一必填）（官方字段路径 <c>sub_openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>清算机构模式小程序场景预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471282"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptQsMiniProgramPreSignResponse : WechatPayResponse
{
    /// <summary>【预签约ID】 委托代扣签约会话ID，两个小时内有效（官方字段路径 <c>pre_entrustweb_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("pre_entrustweb_id")]
    public string? PreEntrustwebId { get; set; }

    /// <summary>【商户号】 微信支付分配的商户号，请求参数原样返回（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，请求参数原样返回（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，请求参数原样返回（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【跳转签约的小程序AppID】 跳转签约的小程序AppID（官方字段路径 <c>sign_mp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sign_mp_appid")]
    public string? SignMpAppid { get; set; }

    /// <summary>【跳转签约的小程序路径】 跳转签约的小程序路径（官方字段路径 <c>sign_mp_path</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sign_mp_path")]
    public string? SignMpPath { get; set; }

}

/// <summary>清算机构模式支付后场景预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471286"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptQsPayRedirectPreSignRequest
{
    /// <summary>【商户AppID】 从业机构商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【子商户AppID】 从业机构的子商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>sub_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【回调通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【预支付交易会话标识】 预支付交易会话标识，由下单接口返回（官方字段路径 <c>prepay_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

    /// <summary>【用户标识】 用户在商户AppID下的唯一标识（openid和sub_openid字段二选一必填）（官方字段路径 <c>openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【用户子标识】 用户在子商户AppID下的唯一标识（openid和sub_openid字段二选一必填）（官方字段路径 <c>sub_openid</c>，选填，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>清算机构模式支付后场景预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471286"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptQsPayRedirectPreSignResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号，请求参数原样返回（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，请求参数原样返回（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，请求参数原样返回（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

}

/// <summary>服务商模式小程序场景预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471269"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptSpMiniProgramPreSignRequest
{
    /// <summary>【商户AppID】 服务商在微信申请的公众号或移动应用AppID（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【用户标识】 服务商AppID下的用户唯一标识（'sp_openid'和'sub_openid'二选一填写）（官方字段路径 <c>sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户AppID】 子商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>sub_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【用户子标识】 子商户AppID下的用户唯一标识（'sub_openid'和'sp_openid'二选一填写）（官方字段路径 <c>sub_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【回调通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

}

/// <summary>服务商模式小程序场景预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471269"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptSpMiniProgramPreSignResponse : WechatPayResponse
{
    /// <summary>【预签约ID】 委托代扣签约会话ID，两个小时内有效（官方字段路径 <c>pre_entrustweb_id</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("pre_entrustweb_id")]
    public string? PreEntrustwebId { get; set; }

    /// <summary>【商户号】 微信支付分配的商户号，请求参数原样返回（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，请求参数原样返回（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，请求参数原样返回（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，请求参数原样返回（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【跳转签约的小程序AppID】 跳转签约的小程序AppID（官方字段路径 <c>sign_mp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sign_mp_appid")]
    public string? SignMpAppid { get; set; }

    /// <summary>【跳转签约的小程序路径】 跳转签约的小程序路径（官方字段路径 <c>sign_mp_path</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sign_mp_path")]
    public string? SignMpPath { get; set; }

}

/// <summary>服务商模式支付后场景预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471272"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptSpPayRedirectPreSignRequest
{
    /// <summary>【商户AppID】 服务商在微信申请的公众号或移动应用AppID（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【用户标识】 服务商AppID下的用户唯一标识（'sp_openid'和'sub_openid'二选一填写）（官方字段路径 <c>sp_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户AppID】 子商户在微信申请的公众号或移动应用AppID（官方字段路径 <c>sub_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【用户子标识】 子商户AppID下的用户唯一标识（'sub_openid'和'sp_openid'二选一填写）（官方字段路径 <c>sub_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，申请见接入指引中的接入流程相关内容（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，商户侧需保证唯一性，同一个协议号解约后无法重复使用。只能是数字、大小写字母的组合（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【回调通知地址】 接收微信支付异步通知回调地址，通知URL必须为HTTPS且可直接可访问的URL，不能携带参数（官方字段路径 <c>notify_url</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("notify_url")]
    public string? NotifyUrl { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【预支付交易会话标识】 预支付交易会话标识，由下单接口返回（官方字段路径 <c>prepay_id</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("prepay_id")]
    public string? PrepayId { get; set; }

}

/// <summary>服务商模式支付后场景预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012471272"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptSpPayRedirectPreSignResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号，请求参数原样返回（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，请求参数原样返回（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID，请求参数原样返回（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号，请求参数原样返回（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

}
