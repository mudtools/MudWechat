// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 进程内审批工单存储（默认实现）：<b>只覆盖单实例部署</b>。
/// </summary>
/// <remarks>
/// <para>
/// 多实例部署必须由宿主前置注册分布式实现（工单的创建与消费很可能落在不同进程）；
/// 契约与五个存储端口同形——<b>宿主预注册者按契约胜出</b>。
/// </para>
/// <para>
/// 本实现不做过期清扫：超时判定是惰性的（读取时按 <see cref="WechatToolApprovalTicket.ExpiresAt"/> 生效），
/// 与令牌存储「惰性过期」同一取舍——清扫是运维关注点，不是正确性关注点。
/// 版本写回在锁内完成（读—比—写原子），避免「两个审批人同时判定」在进程内也赢两次。
/// </para>
/// </remarks>
public sealed class InMemoryWechatToolApprovalStore : IWechatToolApprovalStore
{
    private readonly ConcurrentDictionary<string, WechatToolApprovalTicket> _tickets = new(StringComparer.Ordinal);
    private readonly object _gate = new();

    /// <inheritdoc />
    public ValueTask<WechatToolApprovalTicket?> GetAsync(string ticketId, CancellationToken cancellationToken)
        => ValueTask.FromResult(_tickets.TryGetValue(ticketId, out var ticket) ? ticket : null);

    /// <inheritdoc />
    public ValueTask SaveAsync(WechatToolApprovalTicket ticket, CancellationToken cancellationToken)
    {
        if (ticket is null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        if (!_tickets.TryAdd(ticket.TicketId, ticket))
        {
            throw new InvalidOperationException($"审批工单 {ticket.TicketId} 已存在（工单号冲突）。");
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<bool> UpdateAsync(
        WechatToolApprovalTicket ticket,
        long expectedRevision,
        CancellationToken cancellationToken)
    {
        if (ticket is null)
        {
            throw new ArgumentNullException(nameof(ticket));
        }

        lock (_gate)
        {
            // 读到的版本与当前存储版本不一致 ⇒ 判定/过期已抢先迁移，拒绝静默覆盖。
            if (_tickets.TryGetValue(ticket.TicketId, out var current) && current.Revision != expectedRevision)
            {
                return ValueTask.FromResult(false);
            }

            _tickets[ticket.TicketId] = ticket;
            return ValueTask.FromResult(true);
        }
    }
}
