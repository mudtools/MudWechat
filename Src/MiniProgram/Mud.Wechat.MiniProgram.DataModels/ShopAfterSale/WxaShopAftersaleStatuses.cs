// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>V1 售后状态（<c>aftersale/add|get|update</c> 的 <c>status</c>）。</summary>
/// <remarks>
/// <para>值域<b>不连续</b>：缺 9、10，且 15~19 为 V1 新增的换货分支 ⇒ 不得按 0~N 枚举遍历。</para>
/// </remarks>
public static class WxaShopAftersaleStatuses
{
    /// <summary>0 未受理。</summary>
    public const int Pending = 0;

    /// <summary>1 用户取消。</summary>
    public const int UserCancelled = 1;

    /// <summary>2 商家受理中。</summary>
    public const int MerchantProcessing = 2;

    /// <summary>3 商家逾期未处理。</summary>
    public const int MerchantOverdue = 3;

    /// <summary>4 商家拒绝退款。</summary>
    public const int RefundRejected = 4;

    /// <summary>5 商家拒绝退货退款。</summary>
    public const int ReturnRejected = 5;

    /// <summary>6 待买家退货。</summary>
    public const int WaitingBuyerReturn = 6;

    /// <summary>7 退货退款关闭。</summary>
    public const int ReturnClosed = 7;

    /// <summary>8 待商家收货。</summary>
    public const int WaitingMerchantReceipt = 8;

    /// <summary>11 商家退款中。</summary>
    public const int MerchantRefunding = 11;

    /// <summary>12 商家逾期未退款。</summary>
    public const int RefundOverdue = 12;

    /// <summary>13 退款完成。</summary>
    public const int RefundCompleted = 13;

    /// <summary>14 退货退款完成。</summary>
    public const int ReturnRefundCompleted = 14;

    /// <summary>15 换货完成。</summary>
    public const int ExchangeCompleted = 15;

    /// <summary>16 待商家发货。</summary>
    public const int WaitingMerchantShipment = 16;

    /// <summary>17 待用户确认收货。</summary>
    public const int WaitingUserConfirmReceipt = 17;

    /// <summary>18 商家拒绝换货。</summary>
    public const int ExchangeRejected = 18;

    /// <summary>19 商家已收到货。</summary>
    public const int MerchantReceived = 19;
}
