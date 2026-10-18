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
/// 支付成功通知处理器：演示 <b>at-least-once 契约下的幂等入账</b>与<b>同名事件键的判别陷阱</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>先判别 <c>resource.original_type</c>，再取载荷</b>：分账动态通知的 <c>event_type</c> 与普通支付成功
/// <b>同为</b> <c>TRANSACTION.SUCCESS</c>，只看事件键会把分账报文当交易解析出一个「字段全空」的对象
/// —— 不抛异常、只静默错位，是这类 SDK 最难查的故障形态。
/// </para>
/// <para>
/// <b>处理器必须快速返回</b>：指纹闸在分发前已消费，处理器抛错<b>不会</b>回滚指纹，
/// 官方重试会被第三闸判为重放（403）⇒「靠重试补救」这条路走不通。重活（对账、发货、发通知）
/// 一律落队列异步做。
/// </para>
/// <para><b>PAY-B7</b>：日志只出现业务单号、通知 ID 与状态；<c>openid</c> 与解密明文不落日志。</para>
/// </remarks>
internal sealed class TransactionPaidHandler(
    PayOrderLedger ledger,
    ILogger<TransactionPaidHandler> logger) : IWechatPayNotificationHandler
{
    /// <inheritdoc />
    public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(
                context.Notification?.Resource?.OriginalType,
                WechatPayNotificationOriginalTypes.Transaction,
                StringComparison.Ordinal))
        {
            // 同一事件键下的其他来源（分账动态通知等）：本 Demo 未建对应处理器，明确跳过而不是硬解。
            logger.LogInformation(
                "事件 {EventType} 的 original_type 为 {OriginalType}，非普通交易，跳过交易入账。",
                context.EventType, context.Notification?.Resource?.OriginalType);
            return Task.CompletedTask;
        }

        var transaction = context.GetTransaction();
        if (transaction == null)
        {
            logger.LogWarning("支付成功通知的交易载荷解析失败（事件 {EventType}），本次不入账。", context.EventType);
            ledger.RecordNotification(context.MerchantKey, context.EventType, context.NotificationId, null, "载荷解析失败");
            return Task.CompletedTask;
        }

        var applied = ledger.MarkPaid(
            transaction.OutTradeNo, context.NotificationId, transaction.TransactionId, transaction.TradeState);

        ledger.RecordNotification(
            context.MerchantKey,
            context.EventType,
            context.NotificationId,
            transaction.OutTradeNo,
            applied ? "首次入账" : "重复投递或本宿主未下单");

        if (applied)
        {
            // 这里是「发货 / 记账」的唯一一次触发点：真实宿主应在同一事务内落业务状态。
            logger.LogInformation(
                "支付成功入账：订单 {OutTradeNo}、微信单号 {TransactionId}、状态 {TradeState}、金额 {Total} 分。",
                transaction.OutTradeNo, transaction.TransactionId, transaction.TradeState, transaction.Amount?.Total);
        }
        else
        {
            logger.LogInformation(
                "支付通知重复投递或订单不属于本宿主，未产生业务效果：订单 {OutTradeNo}、通知 {NotificationId}。",
                transaction.OutTradeNo, context.NotificationId);
        }

        return Task.CompletedTask;
    }
}
