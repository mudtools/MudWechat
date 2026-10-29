// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 投诉单关联订单信息（官方子表）。
/// </summary>
/// <remarks>
/// <para><b>单号有四面且不可混读</b>：<c>transaction_id</c>（微信支付交易单号）、<c>out_trade_no</c>（商户单号）、<c>wxa_out_trade_no</c>（小程序侧商户单号）、<c>wx_order_id</c>（微信内部单号）⇒ 与用户交涉时须先确认口径。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_get_complaint_list.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayComplaintOrderInfo
{
    /// <summary>transaction_id（官方类型 <c>string</c>）：投诉单关联的微信支付交易单号</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>out_trade_no（官方类型 <c>string</c>）：渠道单号，query_order接口返回的channel_order_id</summary>
    [JsonPropertyName("out_trade_no")]
    public string? OutTradeNo { get; set; }

    /// <summary>amount（官方类型 <c>number</c>）：订单金额，单位（分）</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>wxa_out_trade_no（官方类型 <c>string</c>）：商户单号，商家在拉起支付时传的单号</summary>
    [JsonPropertyName("wxa_out_trade_no")]
    public string? WxaOutTradeNo { get; set; }

    /// <summary>wx_order_id（官方类型 <c>string</c>）：小程序侧单号</summary>
    [JsonPropertyName("wx_order_id")]
    public string? WxOrderId { get; set; }
}
