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

/// <summary>免密支付协议查询响应·【协议解约信息】 协议解约信息，仅当contract_state=TERMINA。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715321"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptContractQueryResponseContractTerminateInfo
{
    /// <summary>【协议解约方式】 协议解约方式。可选取值：`USER_TERMINATE` 用户发起的解约 / `MCH_API_TERMINATE` 商户通过API发起的解约 / `WEPAY_WEB_TERMINATE` 商户在商户平台发起的解约 / `CUSTOM_SERVICE_TERMINATE` 用户联系微信支付客服发起的解约 / `SYSTEM_TERMINATE` 微信支付系统主动发起的解约（官方字段路径 <c>contract_terminate_info.contract_termination_mode</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_termination_mode")]
    public string? ContractTerminationMode { get; set; }

    /// <summary>【协议解约时间】 按照使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>contract_terminate_info.contract_terminated_time</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_terminated_time")]
    public string? ContractTerminatedTime { get; set; }

    /// <summary>【解约备注】 解约原因的备注说明，如：签约信息有误，须重新签约（官方字段路径 <c>contract_terminate_info.contract_termination_remark</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_termination_remark")]
    public string? ContractTerminationRemark { get; set; }

}

/// <summary>免密支付协议查询响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715321"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptContractQueryResponse : WechatPayResponse
{
    /// <summary>【商户号】 微信支付分配的商户号（官方字段路径 <c>mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("mchid")]
    public string? Mchid { get; set; }

    /// <summary>【免密支付协议ID】 免密支付协议的主键，唯一定义此资源的标识（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【商户AppID】 AppID是商户在微信申请公众号或移动应用成功后分配的账号ID，登录平台为mp.weixin.qq.com或open.weixin.qq.com（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【用户标识】 商户AppID下的用户唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【子商户AppID】 AppID是商户在微信申请公众号或移动应用成功后分配的账号ID，登录平台为mp.weixin.qq.com或open.weixin.qq.com（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【用户标识】 子商户AppID下的用户唯一标识（官方字段路径 <c>sub_openid</c>，选填，类型 <c>string(64)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

    /// <summary>【免密支付服务ID】 免密支付服务ID（官方字段路径 <c>service_id</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("service_id")]
    public long? ServiceId { get; set; }

    /// <summary>【商户签约协议号】 商户侧的签约协议号（官方字段路径 <c>out_contract_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("out_contract_code")]
    public string? OutContractCode { get; set; }

    /// <summary>【用户账户展示名称】 签约用户的名称，用于页面展示，在签约时由商户传入（官方字段路径 <c>contract_display_account</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_display_account")]
    public string? ContractDisplayAccount { get; set; }

    /// <summary>【委托代扣协议状态】 委托代扣协议状态。可选取值：`SIGNED` 签约生效中。 / `TERMINATED` 生效的签约协议已被解约。（官方字段路径 <c>contract_state</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("contract_state")]
    public string? ContractState { get; set; }

    /// <summary>【协议签署时间】 协议签署时间，按照使用rfc3339所定义的格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区（+08:00表示东八区时间，领先UTC 8小时，即北京时间）。例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒（官方字段路径 <c>contract_signed_time</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_signed_time")]
    public string? ContractSignedTime { get; set; }

    /// <summary>【协议解约信息】 协议解约信息，仅当contract_state=TERMINATED时，该值有效（官方字段路径 <c>contract_terminate_info</c>，选填，类型 <c>object</c>）。</summary>
    [JsonPropertyName("contract_terminate_info")]
    public PasswordExemptContractQueryResponseContractTerminateInfo? ContractTerminateInfo { get; set; }

}

/// <summary>免密支付协议解约请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715383"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptContractTerminateRequest
{
    /// <summary>【解约备注】 解约原因的备注说明，如：签约信息有误，须重新签约（官方字段路径 <c>contract_termination_remark</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("contract_termination_remark")]
    public string? ContractTerminationRemark { get; set; }

}

/// <summary>免密支付协议解约响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715383"/></summary>
[HttpJsonSerializable(SerializerClassName = "PasswordExemptContract")]
public class PasswordExemptContractTerminateResponse : WechatPayResponse
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
