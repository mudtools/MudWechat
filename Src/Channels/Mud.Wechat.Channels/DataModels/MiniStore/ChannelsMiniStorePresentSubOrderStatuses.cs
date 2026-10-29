// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.MiniStore;

/// <summary>礼物单子单订单状态常量（官方 <c>status</c> 枚举值，<c>present/get</c>）。</summary>
public static class ChannelsMiniStorePresentSubOrderStatuses
{
    /// <summary>礼物待收下。</summary>
    public const int PendingAccept = 12;

    /// <summary>待发货。</summary>
    public const int PendingDelivery = 20;

    /// <summary>部分发货。</summary>
    public const int PartialDelivery = 21;

    /// <summary>待收货。</summary>
    public const int PendingReceipt = 30;

    /// <summary>完成。</summary>
    public const int Completed = 100;

    /// <summary>礼物单过期。</summary>
    public const int Expired = 180;

    /// <summary>全部商品售后之后，订单取消。</summary>
    public const int CancelledAfterAftersale = 200;
}
