// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 订单管理（Order）域「订单主文档」DTO（order/get、order/list/get、order/search、order/price/update、
// order/address/update、order/addressmodify/*、order/merchantnotes/update、order/presentnote/add、
// order/presentsuborder/get、order/preshipmentchangesku/*、order/blindboxitem/set 等核心端点）。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Order（功能族子目录仅作组织，不入命名空间）。

namespace Mud.Wechat.Channels.DataModels.Order;

/// <summary>
/// 订单状态（官方 <c>order.status</c> / 列表与搜索 <c>status</c> 过滤枚举）。
/// </summary>
public static class ChannelsOrderStatuses
{
    /// <summary>已下单（订单创建成功、等待支付）。</summary>
    public const int Placed = 5;

    /// <summary>待付款（用户下单成功，等待支付）。</summary>
    public const int PendingPayment = 10;

    /// <summary>待发货（已付款，等待商家发货）。</summary>
    public const int PendingDelivery = 20;

    /// <summary>部分发货（订单内部分商品已发货）。</summary>
    public const int PartiallyDelivered = 21;

    /// <summary>待收货（全部商品已发货，等待买家确认收货）。</summary>
    public const int PendingReceipt = 30;

    /// <summary>完成（订单交易完成）。</summary>
    public const int Completed = 100;

    /// <summary>全部商品售后完成之后，订单取消。</summary>
    public const int CancelledAfterAftersale = 200;

    /// <summary>用户取消（未支付、买家主动取消）。</summary>
    public const int UserCancelled = 250;
}
