// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.XPay;

/// <summary>
/// 「通知已发货完成」请求体（<c>POST /xpay/notify_provide_goods</c>）。
/// </summary>
/// <remarks>
/// <para><b><c>order_id</c> 与 <c>wx_order_id</c> 官方标注为「二选一」</b>（原文「微信内部单号(与order_id二选一)」）⇒ 两支同时传并非错误，但 SDK 不校验。</para>
/// <para><b>本端点应答体官方为「无」</b>（只有 <c>errcode</c>/<c>errmsg</c>）⇒ 返回类型直接用 <see cref="Mud.Wechat.MiniProgram.DataModels.WxaResponse"/>，不造 DTO。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_notify_provide_goods.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "XPay")]
public class WxaXPayNotifyProvideGoodsRequest
{
    /// <summary>order_id（官方类型 <c>string</c>，<b>必填</b>）：下单时传的单号</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>wx_order_id（官方类型 <c>string</c>，<b>必填</b>）：微信内部单号(与order_id二选一)</summary>
    [JsonPropertyName("wx_order_id")]
    public string? WxOrderId { get; set; }

    /// <summary>env（官方类型 <c>number</c>，<b>必填</b>）：0-正式环境 1-沙箱环境</summary>
    [JsonPropertyName("env")]
    public long? Env { get; set; }
}
