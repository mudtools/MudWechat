// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ProductOrder;

/// <summary>V1 售后状态（官方 <c>AfterSaleStatus</c>，订单内 <c>aftersale_order_list[].status</c> 用数字）。</summary>
/// <remarks>
/// <para><b>三方值域不同</b>：本族 0~19 与 V2 售后 1~25、V1 <c>/shop/aftersale</c> 0~19 均不同名不同值</para>
/// <para>（如 <c>15</c> 在本族是「平台退款中」、在 V1 商店售后是「换货完成」）⇒ 三套常量禁止互用。</para>
/// <para><b>单查应答用符号名</b>：<c>getaftersaleorder</c> 的 <c>status</c> 是字符串（如 <c>MERCHANT_REFUND_SUCCESS</c>），</para>
/// <para>本类给的是数字值域，二者语义对应但序列化形态不同。</para>
/// </remarks>
public static class WxaProductOrderAfterSaleStatuses
{
    /// <summary>0 无效。</summary>
    public const int Invalid = 0;

    /// <summary>1 用户取消申请（官方拼写 <c>USER_CANCELD</c>）。</summary>
    public const int UserCancelled = 1;

    /// <summary>2 商家受理中。</summary>
    public const int MerchantProcessing = 2;

    /// <summary>4 商家拒绝退款。</summary>
    public const int MerchantRejectRefund = 4;

    /// <summary>5 商家拒绝退货退款。</summary>
    public const int MerchantRejectReturn = 5;

    /// <summary>6 待买家退货。</summary>
    public const int UserWaitReturn = 6;

    /// <summary>7 退货退款关闭。</summary>
    public const int ReturnClosed = 7;

    /// <summary>8 待商家收货。</summary>
    public const int MerchantWaitReceipt = 8;

    /// <summary>12 商家逾期未退款。</summary>
    public const int MerchantOverdueRefund = 12;

    /// <summary>13 退款完成。</summary>
    public const int MerchantRefundSuccess = 13;

    /// <summary>14 退货退款完成。</summary>
    public const int MerchantReturnSuccess = 14;

    /// <summary>15 平台退款中。</summary>
    public const int PlatformRefunding = 15;

    /// <summary>16 平台退款失败。</summary>
    public const int PlatformRefundFail = 16;

    /// <summary>17 待用户确认。</summary>
    public const int UserWaitConfirm = 17;

    /// <summary>18 商家打款失败，客服关闭售后。</summary>
    public const int MerchantRefundRetryFail = 18;

    /// <summary>19 售后关闭。</summary>
    public const int MerchantFail = 19;
}
