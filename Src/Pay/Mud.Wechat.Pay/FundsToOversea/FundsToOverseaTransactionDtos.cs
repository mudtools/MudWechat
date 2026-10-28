// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Common;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Pay.DataModels.FundsToOversea;

/// <summary>查询订单剩余可出境余额响应。官方页：<see href="https://pay.weixin.qq.com/doc/v3/partner/4012476109"/></summary>
[HttpJsonSerializable(SerializerClassName = "FundsToOversea")]
public class FundsToOverseaTransactionAvailableAbroadAmountResponse : WechatPayResponse
{
    /// <summary>【微信支付订单号】微信支付返回的支付订单号（官方字段路径 <c>transaction_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>【订单剩余可出境金额】订单剩余可出境金额，单位: 分（官方字段路径 <c>available_abroad_amount</c>，<b>必填</b>，类型 <c>integer</c>）。</summary>
    [JsonPropertyName("available_abroad_amount")]
    public long? AvailableAbroadAmount { get; set; }

}
