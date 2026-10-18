// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopOrder;

/// <summary>
/// 订单状态枚举值（官方 <c>status</c> 表，订单详情 / 列表 / 关单 / 发货 / 收货五处共用同一套值）。
/// </summary>
/// <remarks>
/// <para><b>11 不是独立状态</b>：官方原文「收银台支付完成（自动流转，对商家来说和 10 同等对待即可）」⇒ 判「待付款」须写 <c>10 or 11</c>。</para>
/// <para><b>181 与 250 都会落到「取消」语义但成因不同</b>：181 是超时未支付（由 <c>expire_time</c> 驱动）、250 是用户/商家主动取消。</para>
/// <para><b>本域官方未在订单页给出 21（部分发货）的流转入口</b>：该值只由 <c>/shop/delivery/send</c> 的 <c>finish_all_delivery = 0</c> 产生。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/order/get_order.html"/></para>
/// </remarks>
public static class WxaShopOrderStatuses
{
    /// <summary>10 待付款。</summary>
    public const int ToBePaid = 10;

    /// <summary>11 收银台支付完成（自动流转，官方要求与 10 同等对待）。</summary>
    public const int CashierPaid = 11;

    /// <summary>20 待发货（已付款 / 用户已付尾款）。</summary>
    public const int ToBeDelivered = 20;

    /// <summary>21 部分发货（仅由发货侧 <c>finish_all_delivery = 0</c> 产生）。</summary>
    public const int PartlyDelivered = 21;

    /// <summary>30 待收货。</summary>
    public const int ToBeReceived = 30;

    /// <summary>100 完成。</summary>
    public const int Finished = 100;

    /// <summary>181 超时未支付取消。</summary>
    public const int PaidTimeoutCancelled = 181;

    /// <summary>200 全部商品售后之后，订单取消。</summary>
    public const int AfterSaleCancelled = 200;

    /// <summary>250 用户主动取消 / 待付款超时取消 / 商家取消。</summary>
    public const int Cancelled = 250;
}
