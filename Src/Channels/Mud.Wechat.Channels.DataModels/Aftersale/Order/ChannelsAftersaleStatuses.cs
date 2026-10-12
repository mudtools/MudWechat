// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 售后管理（Aftersale）域「售后单主文档」DTO（aftersale/getaftersaleorder、aftersale/getaftersalelist、
// aftersale/reason/get、aftersale/rejectreason/get）+ 售后单状态 / 原因 / 类型常量。
// 命名空间恒为 Mud.Wechat.Channels.DataModels.Aftersale。

namespace Mud.Wechat.Channels.DataModels.Aftersale;

/// <summary>售后单当前状态（官方 <c>after_sale_order.status</c> 枚举）。</summary>
public static class ChannelsAftersaleStatuses
{
    /// <summary>用户取消申请。</summary>
    public const string UserCanceld = "USER_CANCELD";

    /// <summary>商家受理中。</summary>
    public const string MerchantProcessing = "MERCHANT_PROCESSING";

    /// <summary>商家拒绝退款。</summary>
    public const string MerchantRejectRefund = "MERCHANT_REJECT_REFUND";

    /// <summary>商家拒绝退货退款。</summary>
    public const string MerchantRejectReturn = "MERCHANT_REJECT_RETURN";

    /// <summary>待买家退货。</summary>
    public const string UserWaitReturn = "USER_WAIT_RETURN";

    /// <summary>退货退款关闭。</summary>
    public const string ReturnClosed = "RETURN_CLOSED";

    /// <summary>待商家收货。</summary>
    public const string MerchantWaitReceipt = "MERCHANT_WAIT_RECEIPT";

    /// <summary>商家逾期未退款。</summary>
    public const string MerchantOverdueRefund = "MERCHANT_OVERDUE_REFUND";

    /// <summary>退款完成。</summary>
    public const string MerchantRefundSuccess = "MERCHANT_REFUND_SUCCESS";

    /// <summary>退货退款完成。</summary>
    public const string MerchantReturnSuccess = "MERCHANT_RETURN_SUCCESS";

    /// <summary>平台退款中。</summary>
    public const string PlatformRefunding = "PLATFORM_REFUNDING";

    /// <summary>平台退款失败。</summary>
    public const string PlatformRefundFail = "PLATFORM_REFUND_FAIL";

    /// <summary>待用户确认。</summary>
    public const string UserWaitConfirm = "USER_WAIT_CONFIRM";

    /// <summary>商家打款失败，客服关闭售后。</summary>
    public const string MerchantRefundRetryFail = "MERCHANT_REFUND_RETRY_FAIL";

    /// <summary>售后关闭。</summary>
    public const string MerchantFail = "MERCHANT_FAIL";

    /// <summary>待用户处理商家协商。</summary>
    public const string UserWaitConfirmUpdate = "USER_WAIT_CONFIRM_UPDATE";

    /// <summary>待用户处理商家代发起的售后申请。</summary>
    public const string UserWaitHandleMerchantAfterSale = "USER_WAIT_HANDLE_MERCHANT_AFTER_SALE";

    /// <summary>物流线上拦截中。</summary>
    public const string WaitPackageIntercept = "WAIT_PACKAGE_INTERCEPT";

    /// <summary>商家拒绝换货。</summary>
    public const string MerchantRejectExchange = "MERCHANT_REJECT_EXCHANGE";

    /// <summary>商家拒绝发货。</summary>
    public const string MerchantRejectReship = "MERCHANT_REJECT_RESHIP";

    /// <summary>待用户收货。</summary>
    public const string UserWaitReceipt = "USER_WAIT_RECEIPT";

    /// <summary>换货完成。</summary>
    public const string MerchantExchangeSuccess = "MERCHANT_EXCHANGE_SUCCESS";
}
