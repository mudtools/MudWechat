// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.Vehicle;

/// <summary>ETC自动扣费预开通请求·【实名信息】 实名信息。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715215"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcPreopenRequestIdentify
{
    /// <summary>【姓名】用户的姓名（官方字段路径 <c>identify.encrypted_real_name</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("encrypted_real_name")]
    public string? EncryptedRealName { get; set; }

    /// <summary>【证件号码】 证件号码（官方字段路径 <c>identify.encrypted_credential_id</c>，<b>必填</b>，类型 <c>string(256)</c>）。</summary>
    [JsonPropertyName("encrypted_credential_id")]
    public string? EncryptedCredentialId { get; set; }

    /// <summary>【证件类型】 证件类型,目前只支持身份证（官方字段路径 <c>identify.credential_type</c>，<b>必填</b>，类型 <c>string(8)</c>）。</summary>
    [JsonPropertyName("credential_type")]
    public string? CredentialType { get; set; }

}

/// <summary>ETC自动扣费预开通请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715215"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcPreopenRequest
{
    /// <summary>【服务商appid】 服务商申请的公众号ID（官方字段路径 <c>sp_appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_appid")]
    public string? SpAppid { get; set; }

    /// <summary>【子appid】 特约商户申请的公众号ID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【特约商户id】特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【用户标识】 用户标识，此参数为微信用户在商户对应AppID或sub_appid下的唯一标识（官方字段路径 <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</summary>
    [JsonPropertyName("openid")]
    public string? Openid { get; set; }

    /// <summary>【ETC授权扣费模板ID】 ETC授权扣费模板ID，商户申请后由微信侧提供（官方字段路径 <c>plan_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plan_id")]
    public string? PlanId { get; set; }

    /// <summary>【etc设备号】 ETC设备号（官方字段路径 <c>etc_device_id</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("etc_device_id")]
    public string? EtcDeviceId { get; set; }

    /// <summary>【车牌号】 车牌号（官方字段路径 <c>plate_number</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【实名信息】 实名信息（官方字段路径 <c>identify</c>，<b>必填</b>，类型 <c>object</c>）。</summary>
    [JsonPropertyName("identify")]
    public VehicleEtcPreopenRequestIdentify? Identify { get; set; }

    /// <summary>【用户开通etc时绑定的银行卡】 用户开通etc时绑定的银行卡（官方字段路径 <c>bank_card_no</c>，选填，类型 <c>string(2048)</c>）。</summary>
    [JsonPropertyName("bank_card_no")]
    public string? BankCardNo { get; set; }

}

/// <summary>ETC自动扣费预开通响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715215"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcPreopenResponse : WechatPayResponse
{
    /// <summary>【预开通id】 预开通id（官方字段路径 <c>preopen_id</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("preopen_id")]
    public string? PreopenId { get; set; }

}
