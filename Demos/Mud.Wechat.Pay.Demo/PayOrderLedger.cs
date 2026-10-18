// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Pay.Demo;

/// <summary>
/// 进程内订单台账：Demo 用它替代真实仓储，把「下单 → 通知 → 入账」这条链上的<b>幂等</b>与
/// <b>受理不等于结果</b>两件事演示清楚。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么幂等键是 <c>out_trade_no</c>、辅助键是 <c>notification_id</c></b>：官方通知是
/// at-least-once（5 秒内收不到 HTTP 200 就重试），同一笔支付会来多次；
/// <c>out_trade_no</c> 是商户侧业务唯一键，决定「这笔钱是否已经入账」；
/// <c>notification_id</c> 只用于识别「同一条通知的重投」，不能当业务唯一键（换单重推时它是新值）。
/// </para>
/// <para>
/// <b>生产形态</b>：这里应当是一次带唯一约束的落库（<c>INSERT ... ON CONFLICT DO NOTHING</c> 或
/// 乐观并发），并且「入账」与「写台账」要在同一事务内。本类型刻意只做进程内记账，
/// 一旦宿主重启即失忆 —— 这是 Demo 边界，不是可复制的持久化方案。
/// </para>
/// <para><b>PAY-B7</b>：台账只保存业务单号、金额与状态；<c>openid</c>、解密明文
/// （<c>WechatPayCallbackContext.ResourceJson</c>）、密文与签名原文一律不入内。</para>
/// </remarks>
internal sealed class PayOrderLedger
{
    private const int NotificationCapacity = 50;

    private readonly ConcurrentDictionary<string, OrderRecord> _orders = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, RefundRecord> _refunds = new(StringComparer.Ordinal);
    private readonly ConcurrentQueue<NotificationEntry> _notifications = new();

    /// <summary>登记一次下单结果（下单成功即记账，后续通知据此判「是不是我的单」）。</summary>
    /// <param name="outTradeNo">商户订单号（业务唯一键）。</param>
    /// <param name="description">商品描述。</param>
    /// <param name="total">订单总金额（分）。</param>
    /// <param name="prepayId">JSAPI/APP 下单返回的 <c>prepay_id</c>；Native 为 <c>null</c>。</param>
    /// <param name="codeUrl">Native 下单返回的 <c>code_url</c>；JSAPI 为 <c>null</c>。</param>
    /// <returns>台账记录（已存在则为既有记录，<c>prepay_id</c>/<c>code_url</c> 被刷新）。</returns>
    public OrderRecord TrackOrder(string outTradeNo, string? description, long total, string? prepayId, string? codeUrl)
    {
        var record = _orders.AddOrUpdate(
            outTradeNo,
            static key => new OrderRecord(key),
            static (_, existing) => existing);

        lock (record.SyncRoot)
        {
            record.Description = description;
            record.AmountTotal = total;
            record.PrepayId = prepayId;
            record.CodeUrl = codeUrl;
        }

        return record;
    }

    /// <summary>按商户订单号取台账记录。</summary>
    /// <param name="outTradeNo">商户订单号。</param>
    /// <param name="record">记录。</param>
    /// <returns>是否存在（不存在即「本 Demo 没下过这单」）。</returns>
    public bool TryGetOrder(string outTradeNo, out OrderRecord record)
        => _orders.TryGetValue(outTradeNo, out record!);

    /// <summary>按商户退款单号取退款记录。</summary>
    /// <param name="outRefundNo">商户退款单号。</param>
    /// <param name="record">记录。</param>
    /// <returns>是否存在。</returns>
    public bool TryGetRefund(string outRefundNo, out RefundRecord record)
        => _refunds.TryGetValue(outRefundNo, out record!);

    /// <summary>登记一次退款申请（退款是<b>异步受理</b>，此处只记「已受理」）。</summary>
    /// <param name="outTradeNo">原商户订单号。</param>
    /// <param name="outRefundNo">商户退款单号。</param>
    /// <param name="refund">退款金额（分）。</param>
    /// <param name="total">原订单总金额（分，官方要求与下单一致）。</param>
    /// <returns>退款记录。</returns>
    public RefundRecord TrackRefund(string outTradeNo, string outRefundNo, long refund, long total)
    {
        var record = _refunds.GetOrAdd(outRefundNo, key => new RefundRecord(key, outTradeNo, refund, total));
        return record;
    }

    /// <summary>
    /// 支付成功通知入账：<b>只有首次</b>返回 <see langword="true"/>。
    /// </summary>
    /// <param name="outTradeNo">商户订单号（通知里的 <c>out_trade_no</c>）。</param>
    /// <param name="notificationId">通知 ID（重投识别用，非业务唯一键）。</param>
    /// <param name="transactionId">微信支付订单号。</param>
    /// <param name="tradeState">通知里的 <c>trade_state</c>。</param>
    /// <returns><c>true</c> = 本次产生业务效果（首次入账）；<c>false</c> = 重复投递或未知订单。</returns>
    public bool MarkPaid(string? outTradeNo, string? notificationId, string? transactionId, string? tradeState)
    {
        if (string.IsNullOrEmpty(outTradeNo) || !_orders.TryGetValue(outTradeNo, out var record))
        {
            // 未知订单：绝不「补一条已支付记录」—— 那会把不属于本宿主的资金通知变成入账。
            return false;
        }

        lock (record.SyncRoot)
        {
            record.TradeState = tradeState;
            if (!string.IsNullOrEmpty(transactionId))
            {
                record.TransactionId = transactionId;
            }

            if (record.Settled)
            {
                return false;
            }

            if (notificationId != null && !record.AppliedNotificationIds.Add(notificationId))
            {
                return false;
            }

            record.Settled = true;
            record.SettledAt = DateTimeOffset.UtcNow;
            return true;
        }
    }

    /// <summary>退款状态通知落账（同 <see cref="MarkPaid"/> 的幂等口径）。</summary>
    /// <param name="outRefundNo">商户退款单号。</param>
    /// <param name="notificationId">通知 ID。</param>
    /// <param name="refundId">微信退款单号。</param>
    /// <param name="status">退款状态。</param>
    /// <returns><c>true</c> = 本次为状态变更（首次落到该终态）；<c>false</c> = 重复投递或未知退款单。</returns>
    public bool MarkRefund(string? outRefundNo, string? notificationId, string? refundId, string? status)
    {
        if (string.IsNullOrEmpty(outRefundNo) || !_refunds.TryGetValue(outRefundNo, out var record))
        {
            return false;
        }

        lock (record.SyncRoot)
        {
            record.Status = status;
            if (!string.IsNullOrEmpty(refundId))
            {
                record.RefundId = refundId;
            }

            if (notificationId != null && !record.AppliedNotificationIds.Add(notificationId))
            {
                return false;
            }

            return true;
        }
    }

    /// <summary>用查单/退款查询的应答刷新台账（官方口径：结果以查单为准，通知只是提示）。</summary>
    /// <param name="record">订单记录。</param>
    /// <param name="tradeState">查得的 <c>trade_state</c>。</param>
    /// <param name="transactionId">查得的微信支付订单号。</param>
    public void ApplyQueryResult(OrderRecord record, string? tradeState, string? transactionId)
    {
        lock (record.SyncRoot)
        {
            record.TradeState = tradeState;
            if (!string.IsNullOrEmpty(transactionId))
            {
                record.TransactionId = transactionId;
            }
        }
    }

    /// <summary>用退款查询的应答刷新台账（与 <see cref="ApplyQueryResult"/> 同一口径：结果以查询为准）。</summary>
    /// <param name="record">退款记录。</param>
    /// <param name="status">查得的 <c>status</c>。</param>
    /// <param name="refundId">查得的微信退款单号。</param>
    public void ApplyRefundQueryResult(RefundRecord record, string? status, string? refundId)
    {
        lock (record.SyncRoot)
        {
            record.Status = status;
            if (!string.IsNullOrEmpty(refundId))
            {
                record.RefundId = refundId;
            }
        }
    }

    /// <summary>记一条通知流水（只含非敏感字段，供 <c>GET /api/notifications</c> 回显）。</summary>
    /// <param name="merchantKey">路由解析出的商户键。</param>
    /// <param name="eventType">事件类型。</param>
    /// <param name="notificationId">通知 ID。</param>
    /// <param name="businessNo">业务单号（订单或退款单）。</param>
    /// <param name="outcome">处置结论（首次入账 / 重复投递 / 未知单）。</param>
    public void RecordNotification(
        string? merchantKey, string? eventType, string? notificationId, string? businessNo, string outcome)
    {
        _notifications.Enqueue(new NotificationEntry(
            DateTimeOffset.UtcNow, merchantKey, eventType, notificationId, businessNo, outcome));

        while (_notifications.Count > NotificationCapacity && _notifications.TryDequeue(out _))
        {
            // 容量到限丢最旧：Demo 侧流水只用于观察，生产应落持久队列。
        }
    }

    /// <summary>通知流水快照（新→旧）。</summary>
    /// <returns>流水列表。</returns>
    public IReadOnlyList<NotificationEntry> NotificationSnapshot()
    {
        var entries = _notifications.ToArray();
        Array.Reverse(entries);
        return entries;
    }

    /// <summary>订单快照（新→旧，不含任何用户标识）。</summary>
    /// <returns>订单列表。</returns>
    public IReadOnlyList<OrderRecord> OrderSnapshot()
        => _orders.Values.OrderByDescending(static r => r.CreatedAtUtc).ToArray();

    /// <summary>退款快照（新→旧）。</summary>
    /// <returns>退款记录列表。</returns>
    public IReadOnlyList<RefundRecord> RefundSnapshot()
        => _refunds.Values.OrderByDescending(static r => r.CreatedAtUtc).ToArray();
}

/// <summary>订单台账记录。</summary>
internal sealed class OrderRecord
{
    /// <param name="outTradeNo">商户订单号（业务唯一键）。</param>
    public OrderRecord(string outTradeNo) => OutTradeNo = outTradeNo;

    /// <summary>商户订单号。</summary>
    public string OutTradeNo { get; }

    /// <summary>逐记录锁：状态迁移必须整体可见，不能用无锁字段拼。</summary>
    public object SyncRoot { get; } = new();

    public string? Description { get; set; }

    public long AmountTotal { get; set; }

    public string? PrepayId { get; set; }

    public string? CodeUrl { get; set; }

    public string? TransactionId { get; set; }

    public string? TradeState { get; set; }

    public bool Settled { get; set; }

    public DateTimeOffset SettledAt { get; set; }

    public DateTimeOffset CreatedAtUtc { get; } = DateTimeOffset.UtcNow;

    /// <summary>已消费过的通知 ID（识别同一条通知的重投）。</summary>
    public HashSet<string> AppliedNotificationIds { get; } = new(StringComparer.Ordinal);
}

/// <summary>退款台账记录。</summary>
internal sealed class RefundRecord
{
    public RefundRecord(string outRefundNo, string outTradeNo, long refund, long total)
    {
        OutRefundNo = outRefundNo;
        OutTradeNo = outTradeNo;
        Refund = refund;
        Total = total;
    }

    /// <summary>商户退款单号（业务唯一键）。</summary>
    public string OutRefundNo { get; }

    /// <summary>原商户订单号。</summary>
    public string OutTradeNo { get; }

    /// <summary>退款金额（分）。</summary>
    public long Refund { get; }

    /// <summary>原订单总金额（分）。</summary>
    public long Total { get; }

    public object SyncRoot { get; } = new();

    public string? RefundId { get; set; }

    public string? Status { get; set; }

    public DateTimeOffset CreatedAtUtc { get; } = DateTimeOffset.UtcNow;

    public HashSet<string> AppliedNotificationIds { get; } = new(StringComparer.Ordinal);
}

/// <summary>通知流水条目（刻意只保留非敏感字段）。</summary>
internal sealed record NotificationEntry(
    DateTimeOffset ReceivedAtUtc,
    string? MerchantKey,
    string? EventType,
    string? NotificationId,
    string? BusinessNo,
    string Outcome);
