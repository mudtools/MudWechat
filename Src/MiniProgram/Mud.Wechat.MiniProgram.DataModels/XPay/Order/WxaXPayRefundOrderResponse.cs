// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「启动订单退款任务」响应：退款单与关联支付单的双侧单号。
/// </summary>
/// <remarks>
/// <para><b>四个单号是两两配对</b>：<c>refund_order_id</c> / <c>refund_wx_order_id</c> 属退款单，<c>pay_order_id</c> / <c>pay_wx_order_id</c> 属原支付单（商户单号 vs 微信内部单号）⇒ 落库时须各自成对保存。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_refund_order.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayRefundOrderResponse : WxaResponse
{
    /// <summary>refund_order_id（官方类型 <c>string</c>）：退款单号</summary>
    [JsonPropertyName("refund_order_id")]
    public string? RefundOrderId { get; set; }

    /// <summary>refund_wx_order_id（官方类型 <c>string</c>）：退款单的微信侧单号</summary>
    [JsonPropertyName("refund_wx_order_id")]
    public string? RefundWxOrderId { get; set; }

    /// <summary>pay_order_id（官方类型 <c>string</c>）：该退款单对应的支付单单号</summary>
    [JsonPropertyName("pay_order_id")]
    public string? PayOrderId { get; set; }

    /// <summary>pay_wx_order_id（官方类型 <c>string</c>）：该退款单对应的支付单微信侧单号</summary>
    [JsonPropertyName("pay_wx_order_id")]
    public string? PayWxOrderId { get; set; }
}
