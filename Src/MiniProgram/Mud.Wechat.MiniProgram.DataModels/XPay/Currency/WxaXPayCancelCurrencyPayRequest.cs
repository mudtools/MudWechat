// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「代币支付退款」请求体（<c>POST /xpay/cancel_currency_pay</c>）。
/// </summary>
/// <remarks>
/// <para><b>两个单号字段不是同义重复</b>：<c>pay_order_id</c> 是<b>原支付单</b>（<c>currency_pay</c> 时传的 order_id）、<c>order_id</c> 是<b>本次退款单</b>自己的单号 ⇒ 写反会得到 <c>268490005</c>/<c>268490013</c> 一类语义错误。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_cancel_currency_pay.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayCancelCurrencyPayRequest
{
    /// <summary>openid（官方类型 <c>string</c>，<b>必填</b>）：用户的openid</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }

    /// <summary>user_ip（官方类型 <c>string</c>，<b>必填</b>）：用户ip，例如1.1.1.1</summary>
    [JsonPropertyName("user_ip")]
    public string? UserIp { get; set; }

    /// <summary>pay_order_id（官方类型 <c>string</c>，<b>必填</b>）：代币支付(调用currency_pay接口时)时传的order_id</summary>
    [JsonPropertyName("pay_order_id")]
    public string? PayOrderId { get; set; }

    /// <summary>order_id（官方类型 <c>string</c>，<b>必填</b>）：本次退款单的单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>amount（官方类型 <c>number</c>，<b>必填</b>）：退款金额</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }
}
