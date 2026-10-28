// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.League;

/// <summary>订单状态常量（官方 <c>order_info.status</c>，与佣金单状态 <see cref="ChannelsLeagueHeadSupplierCommissionOrderStatuses"/> 是两张枚举表）。</summary>
public static class ChannelsLeagueHeadSupplierTradeOrderStatuses
{
    /// <summary>待付款。</summary>
    public const int WaitPayment = 10;

    /// <summary>待收礼。</summary>
    public const int WaitReceiveGift = 12;

    /// <summary>待使用【本地生活券订单】。</summary>
    public const int WaitUse = 17;

    /// <summary>待发货。</summary>
    public const int WaitShipping = 20;

    /// <summary>部分发货。</summary>
    public const int PartialShipping = 21;

    /// <summary>待收货。</summary>
    public const int WaitReceipt = 30;

    /// <summary>完成。</summary>
    public const int Completed = 100;

    /// <summary>全部商品售后之后，订单取消。</summary>
    public const int Cancelled = 200;

    /// <summary>未付款用户主动取消或超时未付款订单自动取消。</summary>
    public const int Closed = 250;
}
