// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.GoldPlan;

/// <summary>商家小票管理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473788"/></summary>
[HttpJsonSerializable(SerializerClassName = "GoldPlan")]
public class GoldPlanCustomPageStatusChangeRequest
{
    /// <summary>【特约商户号】 开通或关闭“商家自定义小票”的特约商户商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【操作类型】 开通或关闭“商家自定义小票”的动作，枚举值： OPEN：表示开通 CLOSE：表示关闭。可选取值：`OPEN` 开通 / `CLOSE` 关闭（官方字段路径 <c>operation_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("operation_type")]
    public string? OperationType { get; set; }

}

/// <summary>商家小票管理响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473788"/></summary>
[HttpJsonSerializable(SerializerClassName = "GoldPlan")]
public class GoldPlanStatusResponse : WechatPayResponse
{
    /// <summary>【特约商户号】 开通或关闭“商家自定义小票”的特约商户商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

}

/// <summary>点金计划管理请求。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012473796"/></summary>
[HttpJsonSerializable(SerializerClassName = "GoldPlan")]
public class GoldPlanPlanStatusChangeRequest
{
    /// <summary>【特约商户号】 开通或关闭点金计划的特约商户商户号，由微信支付生成并下发（官方字段路径 <c>sub_mchid</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchid { get; set; }

    /// <summary>【操作类型】 开通或关闭点金计划的动作，枚举值： OPEN：表示开通点金计划 CLOSE：表示关闭点金计划。可选取值：`OPEN` 开通 / `CLOSE` 关闭（官方字段路径 <c>operation_type</c>，<b>必填</b>，类型 <c>string</c>）。</summary>
    [JsonPropertyName("operation_type")]
    public string? OperationType { get; set; }

    /// <summary>【支付场景】 支付场景，指定开通/关闭点金计划的支付场景。如果未指定，则默认值是JSAPI。可选取值：`JSAPI_AND_MINIPROGRAM` jsapi支付和小程序支付两个场景 / `JSAPI` jsapi支付场景 / `MINIPROGRAM` 小程序支付场景（官方字段路径 <c>operation_pay_scene</c>，选填，类型 <c>string</c>）。</summary>
    [JsonPropertyName("operation_pay_scene")]
    public string? OperationPayScene { get; set; }

}
