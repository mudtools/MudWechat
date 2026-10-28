// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.PayrollCard;

/// <summary>获取代发薪用户授权令牌请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461589"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardTokenRequest
{
    /// <summary>【用户OpenID】 用户OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【服务商AppID】 当输入服务商AppID时，会校验其与服务商商户号的绑定关系。 服务商AppID和与特约商户AppID至少输入一个，且必须要有拉起微工卡小程序时使用的AppID（官方字段路径 <c>appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【特约商户号】 特约商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【特约商户AppID】 当输入特约商户AppID时，会校验其与特约商户号的绑定关系。 服务商AppID和与特约商户AppID至少输入一个，且必须要有拉起微工卡小程序时使用的AppID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【用户实名】 用户实名信息，按照APIV3标准加密该字段（官方字段路径 <c>user_name</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    /// <summary>【用户证件号】 用户证件号，按照APIV3标准加密该字段（官方字段路径 <c>id_card_number</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("id_card_number")]
    public string? IdCardNumber { get; set; }

    /// <summary>【用工类型】 微工卡服务仅支持用于与商户有用工关系的用户，需明确用工类型；参考值：长期用工：LONG_TERM_EMPLOYMENT，短期用工：SHORT_TERM_EMPLOYMENT，合作关系：COOPERATION_EMPLOYMENT。可选取值：`LONG_TERM_EMPLOYMENT` 长期用工：有劳务合同关系的长期用工 / `SHORT_TERM_EMPLOYMENT` 短期用工：有劳务合同关系的短期用工 / `COOPERATION_EMPLOYMENT` 合作关系：有业务服务、合作关系的人员（官方字段路径 <c>employment_type</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("employment_type")]
    public string? EmploymentType { get; set; }

}

/// <summary>获取代发薪用户授权令牌响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012461589"/></summary>
[HttpJsonSerializable(SerializerClassName = "PayrollCard")]
public class PayrollCardTokenResponse : WechatPayResponse
{
    /// <summary>【微信用户OpenID】 微信用户OpenID（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【商户号】 商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【特约商户号】 特约商户号（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【token值】 token值（官方字段路径 <c>token</c>，<b>必填</b>，类型 <c>string(1024)</c>）。</summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>【token有效时间】 token有效时间，单位秒（官方字段路径 <c>expires_in</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("expires_in")]
    public long? ExpiresIn { get; set; }

}
