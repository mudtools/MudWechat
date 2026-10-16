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

/// <summary>查询ETC签约状态响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012715667"/></summary>
[HttpJsonSerializable(SerializerClassName = "Vehicle")]
public class VehicleEtcContractResponse : WechatPayResponse
{
    /// <summary>【发起签约的appid】 发起签约绑定的appid（官方字段路径 <c>appid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? Appid { get; set; }

    /// <summary>【服务商商户号】 服务商商户号，由微信支付生成并下发（官方字段路径 <c>sp_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_mchid")]
    public string? SpMchid { get; set; }

    /// <summary>【用户在发起签约app的标识】 用户在发起签约app下用户唯一标识（官方字段路径 <c>sp_openid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sp_openid")]
    public string? SpOpenid { get; set; }

    /// <summary>【子appid】微信支付分配的公众账号ID（官方字段路径 <c>sub_appid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_appid")]
    public string? SubAppid { get; set; }

    /// <summary>【特约商户号】特约商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【商户etc绑定号】 商户etc绑定号（官方字段路径 <c>contract_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("contract_id")]
    public string? ContractId { get; set; }

    /// <summary>【etc绑定状态】 etc绑定状态，枚举值： OPENED： 已开通 PAUSE：已暂停 DELETED： 已关闭（官方字段路径 <c>bind_state</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("bind_state")]
    public string? BindState { get; set; }

    /// <summary>【绑定的车牌号】 绑定的车牌号（官方字段路径 <c>plate_number</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("plate_number")]
    public string? PlateNumber { get; set; }

    /// <summary>【用户在发起签约子app的标识】 用户在发起签约子app下用户唯一标识（官方字段路径 <c>sub_openid</c>，选填，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_openid")]
    public string? SubOpenid { get; set; }

}
