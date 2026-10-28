// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 「查询小程序是否已完成交易结算管理确认」响应（<c>POST /wxa/sec/order/is_trade_management_confirmation_completed</c>）。
/// </summary>
/// <remarks>
/// <para><b>响应键名与请求语义不同名</b>：本端点返回 <c>completed</c>（不是 <c>is_trade_management_confirmation_completed</c>），
/// 与 <see cref="WxaOrderTradeManagedResponse"/> 的 <c>is_trade_managed</c> 形成对照 ⇒ 守卫 OS4 锁定两个响应键。</para>
/// <para><b>语义</b>（官方原文）：是否已对小程序关联的<b>所有</b>商户号完成订单管理授权或解绑；
/// 已完成授权的商户号产生的订单<b>均须</b>走发货信息管理服务发货。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_istrademanagementconfirmationcompleted.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "OrderShipping")]
public class WxaOrderTradeConfirmationResponse : WxaResponse
{
    /// <summary>completed（官方类型 <c>boolean</c>）：是否已完成交易结算管理确认</summary>
    [JsonPropertyName("completed")]
    public bool? Completed { get; set; }
}
