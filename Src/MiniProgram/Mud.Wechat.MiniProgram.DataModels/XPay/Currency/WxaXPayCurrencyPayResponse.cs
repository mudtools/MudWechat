// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「扣减代币」响应：扣减后的余额与本次消耗的赠送部分。
/// </summary>
/// <remarks>
/// <para><b><c>used_present_amount</c> 揭示扣减顺序</b>（官方原文「使用赠送部分的代币数量」）：官方先扣赠送账户、再扣有价账户 ⇒ 对账时不得把 <c>balance</c> 的减少量全部记作营收。</para>
/// <para><b>敏感值</b>：<c>balance</c> 是用户资产余额 ⇒ 不得写进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_currency_pay.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayCurrencyPayResponse : WxaResponse
{
    /// <summary>order_id（官方类型 <c>string</c>）：订单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>balance（官方类型 <c>number</c>）：总余额，包括有价和赠送部分</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>used_present_amount（官方类型 <c>number</c>）：使用赠送部分的代币数量</summary>
    [JsonPropertyName("used_present_amount")]
    public long? UsedPresentAmount { get; set; }
}
