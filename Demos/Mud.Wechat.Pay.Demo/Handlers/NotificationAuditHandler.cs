// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Callback;

namespace Mud.Wechat.Pay.Demo.Handlers;

/// <summary>
/// 通配审计处理器（默认注册表键 <c>"*"</c>）：任何通过三道闸的通知都会过一遍这里。
/// </summary>
/// <remarks>
/// <para>
/// <b>它存在的理由</b>：官方事件类型不止交易与退款（分账、支付分、发票、合单…），
/// 宿主只挂自己关心的键时，其余通知会「无声消失」（分发器对无匹配处理器同样回 SUCCESS）。
/// 通配桶把每一条通知留一条流水，排查时才有「收到了但没处理」与「根本没收到」的区分。
/// </para>
/// <para>
/// <b>刻意不读 <c>context.ResourceJson</c></b>（解密明文，含 <c>openid</c>）：明文既不入日志，
/// 也不进遥测与异常消息（<c>PAY-B7</c>）。这里只记信封上的事件键、通知 ID 与商户键。
/// </para>
/// <para>
/// <b>顺序</b>：精确桶先跑、通配桶后跑（同一通知内多个处理器按注册顺序执行，异常互隔离），
/// 所以本处理器看到的 <c>Settled</c> 等状态可能已被前面的业务处理器改写。
/// </para>
/// </remarks>
internal sealed class NotificationAuditHandler(
    PayOrderLedger ledger,
    ILogger<NotificationAuditHandler> logger) : IWechatPayNotificationHandler
{
    /// <inheritdoc />
    public Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default)
    {
        ledger.RecordNotification(
            context.MerchantKey,
            context.EventType,
            context.NotificationId,
            businessNo: null,
            outcome: "通配审计");

        logger.LogInformation(
            "收到通知：商户键 {MerchantKey}、事件 {EventType}、通知 {NotificationId}、摘要 {Summary}、资源类型 {ResourceType}。",
            context.MerchantKey,
            context.EventType,
            context.NotificationId,
            context.Notification?.Summary,
            context.Notification?.ResourceType);

        return Task.CompletedTask;
    }
}
