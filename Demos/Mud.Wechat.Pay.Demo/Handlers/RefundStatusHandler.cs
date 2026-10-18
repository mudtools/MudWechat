// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Callback;
using Mud.Wechat.Pay.DataModels.Callback;

namespace Mud.Wechat.Pay.Demo.Handlers;

/// <summary>
/// 退款通知处理器：把 <c>REFUND.SUCCESS</c> / <c>REFUND.ABNORMAL</c> / <c>REFUND.CLOSED</c>
/// 三个事件键接到<b>同一个</b>处理器上，演示「一处理器多事件键」的注册形态。
/// </summary>
/// <remarks>
/// <para>
/// <b>退款申请的成功应答 ≠ 退款成功</b>：官方退款是<b>异步受理</b>，申请接口只回受理结果，
/// 终态只能由本通知或 <c>GET /v3/refund/domestic/refunds/{out_refund_no}</c> 查得。
/// 因此 Demo 侧「退款是否到账」只看这里落下的 <c>refund_status</c>。
/// </para>
/// <para>
/// <b>三个键共用一个处理器</b>：注册表按事件键分桶，同一处理器类型可挂多个键；
/// 处理器内部读 <c>context.EventType</c> 区分 SUCCESS 与 ABNORMAL，而不是靠 <c>status</c> 字段猜。
/// </para>
/// </remarks>
internal sealed class RefundStatusHandler(
    PayOrderLedger ledger,
    ILogger<RefundStatusHandler> logger) : IWechatPayNotificationHandler
{
    /// <inheritdoc />
    public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
    {
        var refund = context.GetRefund();
        if (refund == null)
        {
            logger.LogWarning("退款通知载荷解析失败（事件 {EventType}）。", context.EventType);
            ledger.RecordNotification(context.MerchantKey, context.EventType, context.NotificationId, null, "载荷解析失败");
            return Task.CompletedTask;
        }

        var applied = ledger.MarkRefund(
            refund.OutRefundNo, context.NotificationId, refund.RefundId, refund.RefundStatus);

        ledger.RecordNotification(
            context.MerchantKey,
            context.EventType,
            context.NotificationId,
            refund.OutRefundNo,
            applied ? $"状态落账 {refund.RefundStatus}" : "重复投递或本宿主未申请");

        if (string.Equals(context.EventType, WechatPayNotificationEventTypes.RefundAbnormal, StringComparison.Ordinal))
        {
            // ABNORMAL 需要发起「异常退款」补救（ApplyAbnormalRefundAsync），Demo 只在日志里点名下一步。
            logger.LogWarning(
                "退款异常：退款单 {OutRefundNo}（微信单号 {RefundId}）状态 {Status}，需人工发起异常退款处理。",
                refund.OutRefundNo, refund.RefundId, refund.RefundStatus);
            return Task.CompletedTask;
        }

        logger.LogInformation(
            "退款状态通知：退款单 {OutRefundNo} ↔ 订单 {OutTradeNo}，事件 {EventType}、状态 {Status}、实退 {Refund} 分。",
            refund.OutRefundNo, refund.OutTradeNo, context.EventType, refund.RefundStatus, refund.Amount?.Refund);
        return Task.CompletedTask;
    }
}
