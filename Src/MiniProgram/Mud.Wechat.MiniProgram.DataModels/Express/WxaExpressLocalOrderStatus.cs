// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>即时配送「配送状态 / 订单状态」官方枚举值（<c>order_status</c>）。</summary>
/// <remarks>
/// 官方原文：最终状态包括成功状态 <c>302</c>；失败状态 <c>103</c> / <c>203</c> / <c>204</c> / <c>205</c> /
/// <c>401</c> / <c>501</c> / <c>502</c>。
/// </remarks>
public static class WxaExpressLocalOrderStatus
{
    /// <summary>配送公司接单阶段——等待分配骑手，即初始状态（<c>101</c>）。</summary>
    public const int WaitingForRider = 101;

    /// <summary>配送公司接单阶段——分配骑手成功（<c>102</c>）。</summary>
    public const int RiderAssigned = 102;

    /// <summary>配送公司接单阶段——商家取消订单，订单结束（<c>103</c>）。</summary>
    public const int CanceledByShopBeforePickup = 103;

    /// <summary>骑手取货阶段——骑手到店开始取货（<c>201</c>）。</summary>
    public const int PickingUp = 201;

    /// <summary>骑手取货阶段——取货成功（<c>202</c>）。</summary>
    public const int PickedUp = 202;

    /// <summary>骑手取货阶段——取货失败，商家取消订单，订单结束（<c>203</c>）。</summary>
    public const int PickupFailedCanceledByShop = 203;

    /// <summary>骑手取货阶段——取货失败，骑手因自身原因取消订单，订单结束（<c>204</c>）。</summary>
    public const int PickupFailedCanceledByRider = 204;

    /// <summary>骑手取货阶段——取货失败，骑手因商家原因取消订单，订单结束（<c>205</c>）。</summary>
    public const int PickupFailedDueToShop = 205;

    /// <summary>骑手配送阶段——配送中（<c>301</c>）。</summary>
    public const int Delivering = 301;

    /// <summary>骑手配送阶段——配送成功，订单结束（<c>302</c>）。</summary>
    public const int Delivered = 302;

    /// <summary>骑手配送阶段——商家取消订单，配送物品开始返还商家（<c>303</c>）。</summary>
    public const int ReturningCanceledByShop = 303;

    /// <summary>骑手配送阶段——无法联系收货人，配送物品开始返还商家（<c>304</c>）。</summary>
    public const int ReturningUnreachable = 304;

    /// <summary>骑手配送阶段——收货人拒收，配送物品开始返还商家（<c>305</c>）。</summary>
    public const int ReturningRejected = 305;

    /// <summary>骑手返回配送货品阶段——货品返还商户成功，订单结束（<c>401</c>）。</summary>
    public const int Returned = 401;

    /// <summary>因运力系统原因取消，订单结束（<c>501</c>）。</summary>
    public const int CanceledByDeliverySystem = 501;

    /// <summary>因不可抗拒因素（天气、道路管制等）取消，订单结束（<c>502</c>）。</summary>
    public const int CanceledByForceMajeure = 502;
}
