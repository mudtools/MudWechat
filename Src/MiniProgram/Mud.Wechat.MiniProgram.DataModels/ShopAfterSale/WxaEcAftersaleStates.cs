// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopAfterSale;

/// <summary>V2 售后状态（官方 <c>AfterSalesState</c>）。</summary>
/// <remarks>
/// <para>值域<b>不连续</b>（缺 3、9、10、12、14~20）且以 21~25 表示平台介入分支 ⇒ 不得按序数推导。</para>
/// </remarks>
public static class WxaEcAftersaleStates
{
    /// <summary>1 用户取消售后申请。</summary>
    public const int UserCancelled = 1;

    /// <summary>2 商家处理退款申请中。</summary>
    public const int MerchantRefundProcessing = 2;

    /// <summary>4 商家拒绝退款。</summary>
    public const int MerchantRefundRejected = 4;

    /// <summary>5 商家拒绝退货。</summary>
    public const int MerchantReturnRejected = 5;

    /// <summary>6 待用户退货。</summary>
    public const int WaitingUserReturn = 6;

    /// <summary>7 售后单关闭。</summary>
    public const int Closed = 7;

    /// <summary>8 待商家收货。</summary>
    public const int WaitingMerchantReceipt = 8;

    /// <summary>11 平台退款中。</summary>
    public const int PlatformRefunding = 11;

    /// <summary>13 退款成功。</summary>
    public const int RefundSucceeded = 13;

    /// <summary>21 平台处理退款申请中。</summary>
    public const int PlatformRefundReviewing = 21;

    /// <summary>22 废弃。</summary>
    public const int DeprecatedTwentyTwo = 22;

    /// <summary>23 商家处理退货申请中。</summary>
    public const int MerchantReturnProcessing = 23;

    /// <summary>24 平台处理退货申请中。</summary>
    public const int PlatformReturnProcessing = 24;

    /// <summary>25 平台退款失败。</summary>
    public const int PlatformRefundFailed = 25;
}
