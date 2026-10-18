// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>V1 订单状态（官方 <c>OrderStatus</c> / <c>RequestOrderStatus</c>）。</summary>
/// <remarks>
/// <para><b>值域不连续</b>（缺 0~9、11~14 等）且 250 在请求枚举里扩大为「各类取消的总和」⇒ 不得按序数遍历。</para>
/// </remarks>
public static class WxaProductOrderStatuses
{
    /// <summary>10 待付款。</summary>
    public const int PendingPayment = 10;

    /// <summary>15 拼团活动支付成功、等待成团中。</summary>
    public const int GroupBuyWaiting = 15;

    /// <summary>16 支付成功等待商家接单（同城配送 / 线下自提）。</summary>
    public const int WaitingMerchantAccept = 16;

    /// <summary>17 支付成功待核销。</summary>
    public const int WaitingVerify = 17;

    /// <summary>20 待发货。</summary>
    public const int WaitingShipment = 20;

    /// <summary>21 部分发货。</summary>
    public const int PartiallyShipped = 21;

    /// <summary>30 待收货。</summary>
    public const int WaitingReceipt = 30;

    /// <summary>100 完成。</summary>
    public const int Completed = 100;

    /// <summary>181 订单自动取消并退款（核销券、团购等）。</summary>
    public const int AutoCancelledRefunded = 181;

    /// <summary>190 商品超卖商家取消订单。</summary>
    public const int OversoldCancelled = 190;

    /// <summary>200 全部商品售后之后订单取消。</summary>
    public const int AllAftersaleCancelled = 200;

    /// <summary>250 取消（未付款取消 / 超时 / 售后 / 超卖 / 券或团购自动退款等）。</summary>
    public const int Cancelled = 250;
}
