// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// 应用上下文退役队列：被替换/移除的旧上下文与「待清库任务」在宽限期（默认 300s）后延迟执行。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><b>TMA-07/TMA-24</b>：上下文内含 Timer/锁等根引用，GC 不会回收，必须确定性 Dispose；
/// 宽限期保护仍在飞行中的请求（旧上下文短期内仍可用）。</item>
/// <item><b>P1-9（R14）</b>：持久层清库（<c>PurgeAppTokensAsync</c>）以 <see cref="Func{TResult}"/> 载荷入队——
/// <c>RemoveApp</c> 是同步 API，直接等待异步清库会引入 sync-over-async 死锁风险；本队列的
/// <c>Timer</c> 回调天然脱离调用栈与注册表锁，是唯一不需要 sync-over-async 的既有执行点。</item>
/// <item><b>P2-8</b>：<c>Timer</c> 回调可以重入（上一次未完成即触发下一次），故 <c>Pump</c> 以
/// <see cref="Interlocked"/> 重入闸 + 全包裹 <c>try/catch</c> 保护（<c>async void</c> 的未观察异常会击穿进程）。</item>
/// </list>
/// </remarks>
internal sealed class WechatAppContextRetirement : IDisposable
{
    private readonly TimeSpan _retireDelay;
    private readonly ILogger _logger;
    private readonly ConcurrentQueue<WechatRetirementEntry> _queue = new();
    private readonly Timer? _pumpTimer;
    private int _pumping;
    private int _disposed;

    /// <summary>创建退役队列。</summary>
    /// <param name="retireDelaySeconds">宽限期（秒）；&lt;= 0 表示立即执行。</param>
    /// <param name="logger">日志器。</param>
    public WechatAppContextRetirement(int retireDelaySeconds, ILogger logger)
    {
        _retireDelay = TimeSpan.FromSeconds(Math.Max(0, retireDelaySeconds));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (_retireDelay > TimeSpan.Zero)
        {
            // 低频巡检泵：每半个宽限期（下限 1s，上限 30s）检查一次到期条目。
            var period = TimeSpan.FromSeconds(Math.Min(30, Math.Max(1, _retireDelay.TotalSeconds / 2)));
            _pumpTimer = new Timer(Pump, null, period, period);
        }
    }

    /// <summary>将退役上下文入队（宽限期后 Dispose；无宽限期时立即释放）。</summary>
    /// <param name="appKey">应用键（仅用于诊断）。</param>
    /// <param name="context">待释放的应用上下文。</param>
    public void Enqueue(string appKey, IWechatAppContext context)
    {
        if (context is null)
        {
            return;
        }

        if (_retireDelay <= TimeSpan.Zero)
        {
            context.Dispose();
            return;
        }

        _queue.Enqueue(WechatRetirementEntry.ForContext(appKey, context, DueAt()));
    }

    /// <summary>
    /// 将异步清理任务入队（宽限期后执行；失败仅告警，不阻断后续条目）。
    /// </summary>
    /// <param name="appKey">应用键（仅用于诊断）。</param>
    /// <param name="cleanup">清理委托（如持久层令牌清库）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="cleanup"/> 为 <c>null</c>。</exception>
    public void EnqueueCleanup(string appKey, Func<CancellationToken, Task> cleanup)
    {
        if (cleanup == null) throw new ArgumentNullException(nameof(cleanup));

        if (_disposed != 0)
        {
            return;
        }

        if (_retireDelay <= TimeSpan.Zero || _pumpTimer == null)
        {
            // 无巡检泵（宽限期为 0 或计时器不可用）：fire-and-forget + 异常观察，避免 sync-over-async。
            _ = RunCleanupAsync(appKey, cleanup);
            return;
        }

        _queue.Enqueue(WechatRetirementEntry.ForCleanup(appKey, cleanup, DueAt()));
    }

    private DateTimeOffset DueAt() => DateTimeOffset.UtcNow + _retireDelay;

    private async void Pump(object? state)
    {
        // P2-8：Timer 回调可重入；未完成时直接跳过本轮，避免并发释放同一批条目。
        if (Interlocked.CompareExchange(ref _pumping, 1, 0) != 0)
        {
            return;
        }

        try
        {
            var now = DateTimeOffset.UtcNow;
            while (_queue.TryPeek(out var entry) && entry.RetireAt <= now)
            {
                if (!_queue.TryDequeue(out entry))
                {
                    break;
                }

                if (entry.Context != null)
                {
                    DisposeContext(entry);
                    continue;
                }

                if (entry.Cleanup != null)
                {
                    await RunCleanupAsync(entry.AppKey, entry.Cleanup).ConfigureAwait(false);
                }
            }
        }
        catch (Exception ex)
        {
            // async void：任何逃逸异常都会击穿进程，必须全包裹兜底。
            _logger.LogWarning(ex, "应用上下文退役巡检发生异常（本轮已中止，下轮重试）。");
        }
        finally
        {
            Interlocked.Exchange(ref _pumping, 0);
        }
    }

    private void DisposeContext(WechatRetirementEntry entry)
    {
        try
        {
            entry.Context!.Dispose();
            _logger.LogDebug("应用上下文已退役释放（AppKey: {AppKey}）。", entry.AppKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "应用上下文退役释放失败（AppKey: {AppKey}）。", entry.AppKey);
        }
    }

    private async Task RunCleanupAsync(string appKey, Func<CancellationToken, Task> cleanup)
    {
        try
        {
            await cleanup(CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // 清库失败不阻断（令牌会随 TTL 自然过期），仅告警。
            _logger.LogWarning(ex, "应用 {AppKey} 的退役清理任务失败（令牌将随 TTL 过期）。", appKey);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        _pumpTimer?.Dispose();

        // 立即释放全部在队条目（管理器停机路径）：上下文确定性 Dispose，清理任务同步等待（停机期允许阻塞）。
        while (_queue.TryDequeue(out var entry))
        {
            if (entry.Context != null)
            {
                DisposeContext(entry);
                continue;
            }

            if (entry.Cleanup != null)
            {
                try
                {
                    entry.Cleanup(CancellationToken.None).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "停机执行退役清理任务失败（AppKey: {AppKey}）。", entry.AppKey);
                }
            }
        }
    }

    private readonly struct WechatRetirementEntry
    {
        private WechatRetirementEntry(
            string appKey, IWechatAppContext? context, Func<CancellationToken, Task>? cleanup, DateTimeOffset retireAt)
        {
            AppKey = appKey;
            Context = context;
            Cleanup = cleanup;
            RetireAt = retireAt;
        }

        public string AppKey { get; }

        public IWechatAppContext? Context { get; }

        public Func<CancellationToken, Task>? Cleanup { get; }

        public DateTimeOffset RetireAt { get; }

        public static WechatRetirementEntry ForContext(string appKey, IWechatAppContext context, DateTimeOffset dueAt)
            => new(appKey, context, null, dueAt);

        public static WechatRetirementEntry ForCleanup(string appKey, Func<CancellationToken, Task> cleanup, DateTimeOffset dueAt)
            => new(appKey, null, cleanup, dueAt);
    }
}
