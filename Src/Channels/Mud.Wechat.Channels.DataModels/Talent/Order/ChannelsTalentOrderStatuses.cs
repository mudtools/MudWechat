// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
namespace Mud.Wechat.Channels.DataModels.Talent;

/// <summary>佣金单订单状态（官方 <c>base_info.order_status</c> 枚举表，1~10 连续值域）。</summary>
/// <remarks>
/// <para>本表是<b>带货助手佣金单</b>侧的订单状态（1~10 连续值域），与 League 域「交易订单状态」（10 待付款 / 12 待收礼 / 17 待使用 / 20 待发货 / 21 部分发货 / 30 待收货 / 100 完成 / 200 取消 / 250 关闭）是两张表 ⇒ 常量类分建，取值互不混用。</para>
/// </remarks>
public static class ChannelsTalentOrderStatuses
{
    /// <summary>获取待支付（官方原文「1 待支付」）的值。</summary>
    public const int WaitPayment = 1;

    /// <summary>获取待发货（官方原文「2 待发货」）的值。</summary>
    public const int WaitShipping = 2;

    /// <summary>获取已发货（官方原文「3 已发货」）的值。</summary>
    public const int Shipped = 3;

    /// <summary>获取已收货（官方原文「4 已收货」）的值。</summary>
    public const int Received = 4;

    /// <summary>获取售后中（官方原文「5 售后中」）的值。</summary>
    public const int InAftersale = 5;

    /// <summary>获取已完成（官方原文「6 已完成」）的值。</summary>
    public const int Completed = 6;

    /// <summary>获取已取消（官方原文「7 已取消」）的值。</summary>
    public const int Cancelled = 7;

    /// <summary>获取已退款（官方原文「8 已退款」）的值。</summary>
    public const int Refunded = 8;

    /// <summary>获取部分退款（官方原文「9 部分退款」）的值。</summary>
    public const int PartiallyRefunded = 9;

    /// <summary>获取待使用（官方原文「10 待使用」）的值。</summary>
    public const int WaitUse = 10;

}
