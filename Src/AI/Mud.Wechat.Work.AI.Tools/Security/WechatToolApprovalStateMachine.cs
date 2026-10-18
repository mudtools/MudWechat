// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 审批状态机（挂起 → 人工批准 → 续跑）：把「这一次要不要人确认」从同步门扩展为可跨轮次兑现的工单。
/// </summary>
/// <remarks>
/// <para>
/// <b>与同步门的分工</b>：<see cref="IWechatToolApprovalGate"/> 仍是「调用当下就能判定」的通道（规则自动放行、
/// 宿主已有人机通道）；本类型承接「判定要等一会儿」的通道。二者由执行链二选一（注册了本类型即走挂起式），
/// 不会同时生效——混用会让同一动作有两条相互不知情的审批路径。
/// </para>
/// <para>
/// <b>三条硬约束</b>：① 批准只能兑现一次（<see cref="WechatToolApprovalState.Consumed"/> 是终态）；
/// ② 批准与执行必须同一份入参（摘要校验，防「批准 A、执行 B」）；③ 超时即过期（惰性判定，不做后台清扫）。
/// </para>
/// <para>
/// <b>并发</b>：所有迁移都是「读—判—带版本写回」，版本不符即视为他人已迁移（返回失败/抛错），
/// 不静默覆盖——两个审批人同时点「批准」时，只有一个人成功。
/// </para>
/// </remarks>
/// <param name="store">工单存储（不得为 <see langword="null"/>）。</param>
/// <param name="timeToLive">工单有效期（可空 ⇒ <see cref="DefaultTimeToLive"/>）。</param>
/// <param name="logger">日志（可空）。</param>
/// <exception cref="ArgumentNullException"><paramref name="store"/> 为 <see langword="null"/>。</exception>
public sealed class WechatToolApprovalStateMachine
{
    /// <summary>默认工单有效期（30 分钟：足够人工看一眼，又不至于让半年前的批准被翻出来执行）。</summary>
    public static readonly TimeSpan DefaultTimeToLive = TimeSpan.FromMinutes(30);

    private readonly IWechatToolApprovalStore _store;
    private readonly TimeSpan _timeToLive;
    private readonly ILogger<WechatToolApprovalStateMachine>? _logger;

    /// <summary>初始化状态机。</summary>
    /// <param name="store">工单存储。</param>
    /// <param name="timeToLive">工单有效期（可空 ⇒ 默认 30 分钟）。</param>
    /// <param name="logger">日志（可空）。</param>
    public WechatToolApprovalStateMachine(
        IWechatToolApprovalStore store,
        TimeSpan? timeToLive = null,
        ILogger<WechatToolApprovalStateMachine>? logger = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _timeToLive = timeToLive ?? DefaultTimeToLive;
        if (_timeToLive <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeToLive), "工单有效期必须是正数（零或负 TTL 等于「不许审批」）。");
        }

        _logger = logger;
    }

    /// <summary>本状态机使用的工单有效期。</summary>
    public TimeSpan TimeToLive => _timeToLive;

    /// <summary>挂起一次写工具调用：创建待审批工单（调用方拿到工单号后即可返回模型）。</summary>
    /// <param name="context">授权上下文。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>待审批工单。</returns>
    public async Task<WechatToolApprovalTicket> SuspendAsync(
        WechatToolAuthorizationContext context,
        CancellationToken cancellationToken = default)
    {
        var ticket = WechatToolApprovalTicket.CreatePending(context, _timeToLive);
        await _store.SaveAsync(ticket, cancellationToken).ConfigureAwait(false);
        _logger?.LogInformation(
            "写工具 {Tool}（{Risk}）已挂起等待人工审批：{Ticket}，有效期至 {ExpiresAt:O}。",
            ticket.ToolName, ticket.Risk, ticket.TicketId, ticket.ExpiresAt);
        return ticket;
    }

    /// <summary>读取工单（惰性过期：超时的待审批工单首次被读到时转为已过期）。</summary>
    /// <param name="ticketId">工单号。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工单；不存在返回 <see langword="null"/>。</returns>
    public async Task<WechatToolApprovalTicket?> FindAsync(
        string ticketId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _store.GetAsync(ticketId, cancellationToken).ConfigureAwait(false);
        if (ticket is null)
        {
            return null;
        }

        if (ticket.State == WechatToolApprovalState.Pending && DateTimeOffset.UtcNow >= ticket.ExpiresAt)
        {
            var expired = ticket.WithExpired(DateTimeOffset.UtcNow);
            if (await _store.UpdateAsync(expired, ticket.Revision, cancellationToken).ConfigureAwait(false))
            {
                return expired;
            }
        }

        return ticket;
    }

    /// <summary>批准工单。</summary>
    /// <param name="ticketId">工单号。</param>
    /// <param name="approver">审批人标识（宿主侧；审计用，可空）。</param>
    /// <param name="reason">确认来源备注（如「管理员在工单 #123 中批准」；可空）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>批准后的工单。</returns>
    /// <exception cref="InvalidOperationException">工单不存在、已判定或已过期。</exception>
    public async Task<WechatToolApprovalTicket> ApproveAsync(
        string ticketId,
        string? approver = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var ticket = await FindAsync(ticketId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"审批工单 {ticketId} 不存在。");

        if (ticket.State != WechatToolApprovalState.Pending)
        {
            throw new InvalidOperationException(
                $"审批工单 {ticketId} 当前状态为 {ticket.State}，只有待审批的工单可以被批准（重复判定须被拒，防止审计出现两个真相）。");
        }

        var approved = ticket.WithApproved(approver, reason, DateTimeOffset.UtcNow);
        if (!await _store.UpdateAsync(approved, ticket.Revision, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException($"审批工单 {ticketId} 已被他人迁移（并发判定冲突）。");
        }

        _logger?.LogInformation("审批工单 {Ticket} 已被 {Approver} 批准。", ticketId, approver ?? "(未署名)");
        return approved;
    }

    /// <summary>拒绝工单。</summary>
    /// <param name="ticketId">工单号。</param>
    /// <param name="approver">审批人标识（宿主侧；可空）。</param>
    /// <param name="reason">拒绝理由（须非空——审计需要可读原因）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>拒绝后的工单。</returns>
    /// <exception cref="ArgumentException"><paramref name="reason"/> 为空白。</exception>
    /// <exception cref="InvalidOperationException">工单不存在、已判定或已过期。</exception>
    public async Task<WechatToolApprovalTicket> RejectAsync(
        string ticketId,
        string reason,
        string? approver = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("拒绝理由不能为空（审计需要可读原因）。", nameof(reason));
        }

        var ticket = await FindAsync(ticketId, cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException($"审批工单 {ticketId} 不存在。");

        if (ticket.State != WechatToolApprovalState.Pending)
        {
            throw new InvalidOperationException(
                $"审批工单 {ticketId} 当前状态为 {ticket.State}，只有待审批的工单可以被拒绝。");
        }

        var rejected = ticket.WithRejected(approver, reason, DateTimeOffset.UtcNow);
        if (!await _store.UpdateAsync(rejected, ticket.Revision, cancellationToken).ConfigureAwait(false))
        {
            throw new InvalidOperationException($"审批工单 {ticketId} 已被他人迁移（并发判定冲突）。");
        }

        _logger?.LogInformation("审批工单 {Ticket} 已被 {Approver} 拒绝。", ticketId, approver ?? "(未署名)");
        return rejected;
    }

    /// <summary>兑现工单：批准 → 执行的唯一通路（一次性，且绑定工具名与入参摘要）。</summary>
    /// <param name="ticketId">工单号。</param>
    /// <param name="toolName">本次调用的工具名（须与工单一致）。</param>
    /// <param name="argumentsDigest">本次入参的摘要（须与工单一致）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>兑现结果；失败时给出可读原因（调用方原样回填模型）。</returns>
    public async Task<WechatToolApprovalConsumeResult> ConsumeAsync(
        string ticketId,
        string toolName,
        string argumentsDigest,
        CancellationToken cancellationToken = default)
    {
        var ticket = await FindAsync(ticketId, cancellationToken).ConfigureAwait(false);
        if (ticket is null)
        {
            return WechatToolApprovalConsumeResult.Fail($"审批单 {ticketId} 不存在。");
        }

        if (ticket.State != WechatToolApprovalState.Approved)
        {
            return WechatToolApprovalConsumeResult.Fail(
                ticket.State == WechatToolApprovalState.Consumed
                    ? $"审批单 {ticketId} 已被使用过（一次批准只能执行一次）。"
                    : $"审批单 {ticketId} 当前状态为 {ticket.State}，尚未获批准。");
        }

        if (!string.Equals(ticket.ToolName, toolName, StringComparison.Ordinal))
        {
            return WechatToolApprovalConsumeResult.Fail(
                $"审批单 {ticketId} 批准的是 {ticket.ToolName}，与本次调用 {toolName} 不一致。");
        }

        if (!string.Equals(ticket.ArgumentsDigest, argumentsDigest, StringComparison.Ordinal))
        {
            return WechatToolApprovalConsumeResult.Fail(
                $"审批单 {ticketId} 批准的入参与本次调用不一致（批准内容与执行内容必须一致）。");
        }

        var consumed = ticket.WithConsumed(DateTimeOffset.UtcNow);
        if (!await _store.UpdateAsync(consumed, ticket.Revision, cancellationToken).ConfigureAwait(false))
        {
            return WechatToolApprovalConsumeResult.Fail($"审批单 {ticketId} 已被他人兑现（并发冲突）。");
        }

        _logger?.LogInformation("审批工单 {Ticket} 已兑现，工具 {Tool} 开始执行。", ticketId, toolName);
        return WechatToolApprovalConsumeResult.Success();
    }
}

/// <summary>工单兑现结果（成功 / 失败 + 可读原因）。</summary>
public readonly struct WechatToolApprovalConsumeResult
{
    private WechatToolApprovalConsumeResult(bool isConsumed, string? failureReason)
    {
        IsConsumed = isConsumed;
        FailureReason = failureReason;
    }

    /// <summary>是否兑现成功（可以执行工具）。</summary>
    public bool IsConsumed { get; }

    /// <summary>失败原因（成功时为 <see langword="null"/>；文案可回填模型）。</summary>
    public string? FailureReason { get; }

    /// <summary>兑现成功。</summary>
    /// <returns>成功结果。</returns>
    public static WechatToolApprovalConsumeResult Success() => new(true, null);

    /// <summary>兑现失败。</summary>
    /// <param name="reason">可读原因（须非空）。</param>
    /// <returns>失败结果。</returns>
    public static WechatToolApprovalConsumeResult Fail(string reason)
        => new(false, string.IsNullOrWhiteSpace(reason) ? "审批单不可用。" : reason);
}
