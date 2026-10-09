// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Abstractions;

/// <summary>可用时间源（本线自带端口，便于逐边界测试刷新策略）。</summary>
public interface IOpenPlatformClock
{
    /// <summary>当前 UTC 时间。</summary>
    DateTimeOffset UtcNow { get; }
}

/// <summary>系统时间源（<see cref="IOpenPlatformClock"/> 的默认实现）。</summary>
public sealed class SystemOpenPlatformClock : IOpenPlatformClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

/// <summary>
/// <c>component_verify_ticket</c> 的一次推送快照。
/// </summary>
public sealed class ComponentVerifyTicketSnapshot
{
    /// <summary>创建快照。</summary>
    /// <param name="ticket">票据（去空白后非空）。</param>
    /// <param name="receivedAt">接收时间（用于诊断；<b>不是</b>官方时间）。</param>
    public ComponentVerifyTicketSnapshot(string ticket, DateTimeOffset receivedAt)
    {
        Ticket = ticket;
        ReceivedAt = receivedAt;
    }

    /// <summary>票据内容。</summary>
    public string Ticket { get; }

    /// <summary>接收时间（本地时钟）。</summary>
    public DateTimeOffset ReceivedAt { get; }
}

/// <summary>
/// <c>component_verify_ticket</c> 存储端口。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有这个端口（本线最关键的架构约束）</b>：该票据由<b>微信后台主动推送</b>给第三方平台
/// （官方原文：「<c>component_verify_ticket</c> string 是 微信后台推送的 ticket」），
/// <b>不能</b>由 SDK 去请求获取 ⇒ 一整套凭证链的<b>起点在外部</b>：
/// 没有票据，<c>component_access_token</c> 就取不到，所有第三方平台接口都不可用。
/// </para>
/// <para>
/// <b>因此本线必须宿主侧提供写入路径</b>（接收推送 → <see cref="Set"/>）；
/// 本包只定义端口 + 进程内默认实现，多实例部署须换共享实现
/// （与各线存储端口同款纪律：每个实例各存一份会在推送只打到其中一台时出现
/// 「部分实例取不到令牌」的间歇性故障）。
/// </para>
/// </remarks>
public interface IComponentVerifyTicketStore
{
    /// <summary>
    /// 写入最新票据。
    /// </summary>
    /// <param name="ticket">票据；<c>null</c> / 空白 / 全空白视为<b>无效推送</b>，实现<b>必须忽略</b>。</param>
    /// <remarks>
    /// <b>为何空白推送必须被忽略（而不是覆盖）</b>：推送链路的异常（网关截断、解析失败、
    /// 人为调用的空值）如果能把已有的好票据清掉，就会让<b>下一次令牌刷新必然失败</b> ——
    /// 而失败直到令牌到期才暴露，属「埋一颗定时炸弹」。宁可靠旧票据多撑一轮。
    /// </remarks>
    void Set(string? ticket);

    /// <summary>读取当前票据。</summary>
    /// <param name="snapshot">命中时的快照；未命中为 <c>null</c>。</param>
    /// <returns>是否存在可用票据。</returns>
    bool TryGet(out ComponentVerifyTicketSnapshot? snapshot);
}

/// <summary>
/// <c>component_verify_ticket</c> 的<b>进程内</b>存储（<see cref="IComponentVerifyTicketStore"/> 的默认实现）。
/// </summary>
/// <remarks>
/// <b>多实例部署须换分布式实现</b>（Redis 等）：推送通常只到达<b>一台</b>实例，
/// 只存本进程会让其它实例拿旧票据甚至无票据 ⇒ 间歇性「令牌获取失败」。
/// 与各线四个存储端口同款纪律。
/// </remarks>
public sealed class InMemoryComponentVerifyTicketStore : IComponentVerifyTicketStore
{
    private readonly IOpenPlatformClock _clock;
    private readonly object _gate = new object();
    private ComponentVerifyTicketSnapshot? _snapshot;

    /// <summary>创建进程内票据存储。</summary>
    /// <param name="clock">时间源（用于记录接收时间）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="clock"/> 为 <c>null</c>。</exception>
    public InMemoryComponentVerifyTicketStore(IOpenPlatformClock clock)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <inheritdoc />
    public void Set(string? ticket)
    {
        // 无效推送一律忽略（见接口 remarks）：绝不让空值清掉已有有效票据。
        if (string.IsNullOrWhiteSpace(ticket))
        {
            return;
        }

        lock (_gate)
        {
            _snapshot = new ComponentVerifyTicketSnapshot(ticket.Trim(), _clock.UtcNow);
        }
    }

    /// <inheritdoc />
    public bool TryGet(out ComponentVerifyTicketSnapshot? snapshot)
    {
        lock (_gate)
        {
            snapshot = _snapshot;
            return _snapshot != null;
        }
    }
}
