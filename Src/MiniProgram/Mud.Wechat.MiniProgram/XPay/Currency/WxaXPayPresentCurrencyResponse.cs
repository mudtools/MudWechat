// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「代币赠送」响应：赠送后余额与累计赠送额。
/// </summary>
/// <remarks>
/// <para><b>敏感值</b>：<c>balance</c> / <c>present_balance</c> 是用户资产余额 ⇒ 不得进日志、遥测或异常消息。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_present_currency.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayPresentCurrencyResponse : WxaResponse
{
    /// <summary>balance（官方类型 <c>number</c>）：赠送后用户的代币余额</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>order_id（官方类型 <c>string</c>）：赠送单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>present_balance（官方类型 <c>number</c>）：用户收到的总赠送金额</summary>
    [JsonPropertyName("present_balance")]
    public long? PresentBalance { get; set; }
}
