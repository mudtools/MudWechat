// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.OrderShipping;

/// <summary>
/// 订单状态（<c>order_state</c>，1~6 连续；官方两页逐字同一张表）。
/// </summary>
/// <remarks>
/// <para><b>与 <c>xpay/query_order</c> 的订单状态不是同一套</b>（虚拟支付域另有独立状态与码表）⇒
/// 不得跨域复用常量。</para>
/// <para><b>「资金待结算（6）」是本域独有终态</b>：位于已退款之外，官方未把它与
/// <c>is_trade_management_confirmation_completed</c> 的口径互相引用 ⇒ 宿主不要据本值推断结算结果。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorder.html"/> ; <see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getorderlist.html"/></para>
/// </remarks>
public static class WxaOrderStateValues
{
    /// <summary>待发货（<c>1</c>）。</summary>
    public const int PendingDelivery = 1;

    /// <summary>已发货（<c>2</c>）。</summary>
    public const int Delivered = 2;

    /// <summary>确认收货（<c>3</c>）。</summary>
    public const int Received = 3;

    /// <summary>交易完成（<c>4</c>）。</summary>
    public const int Completed = 4;

    /// <summary>已退款（<c>5</c>）。</summary>
    public const int Refunded = 5;

    /// <summary>资金待结算（<c>6</c>）。</summary>
    public const int PendingSettlement = 6;
}
