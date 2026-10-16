// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.EducationPapay;

/// <summary>教育代扣预签约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012522560"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465360"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayContractPresignRequest
{
    /// <summary>【服务商APPID】 微信公众平台为商户的应用分配的ID，与服务商商户号存在绑定关系（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【服务商APPID下的用户标识】 用户在服务商appid下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【签约模版号】 签约模版号，签约模版唯一标识（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【用户账号】 商户应用场景的用户账号，如注册账号，在微信官方签约小程序的签约首页展示（官方字段路径 <c>user_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }

    /// <summary>【首次扣款日期】 扣款日只允许为每月1-28日，使用 rfc3339 所定义的full-date格式：YYYY-MM-DD（官方字段路径 <c>period_start_date</c>，<b>必填</b>，类型 <c>string(10)</c>）。</summary>
    [JsonPropertyName("period_start_date")]
    public string? PeriodStartDate { get; set; }

    /// <summary>【场景信息枚举】 教育续费通 EDUCATION_PERIOD（官方字段路径 <c>trade_scene</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("trade_scene")]
    public string? TradeScene { get; set; }

    /// <summary>【子商户号】 微信支付分配的商户号，服务商商户号与子商户号存在父子关系，如果是直连商户模式接入，不用填写（官方字段路径 <c>sub_mchid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【子商户APPID】 微信公众平台为商户的应用分配的ID，与子商户号存在绑定关系，如果是直连商户模式接入，不用填写（官方字段路径 <c>sub_appid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【子商户APPID下的用户标识】 用户在子商户appid下的唯一标识，如果是直连商户模式接入，不用填写（官方字段路径 <c>sub_openid</c>，选填，仅<b>服务商面</b>官方页印发，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}

/// <summary>教育代扣预签约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012522560"/>、<see href="https://pay.weixin.qq.com/doc/v3/partner/4012465360"/></summary>
[HttpJsonSerializable(SerializerClassName = "EducationPapay")]
public class EducationPapayContractPresignResponse : WechatPayResponse
{
    /// <summary>【预签约号】 预签约号，商户小程序跳转微信官方小程序时需带此参数（官方字段路径 <c>presign_token</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("presign_token")]
    public string? PresignToken { get; set; }

}
