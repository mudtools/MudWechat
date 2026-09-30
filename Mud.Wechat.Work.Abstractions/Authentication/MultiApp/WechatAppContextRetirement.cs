// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// 应用上下文退役队列：被替换/移除的旧上下文在宽限期（默认 300s）后延迟 Dispose。
/// </summary>
/// <remarks>
/// 对齐 Feishu TMA-07/TMA-24：上下文内含 Timer/锁等根引用，GC 不会回收，
/// 必须确定性 Dispose；宽限期保护仍在飞行中的请求（旧上下文短期内仍可用）。
/// </remarks>
internal sealed class WechatAppContextRetirement : IDisposable
{
    private readonly TimeSpan _retireDelay;
    private readonly ILogger _logger;
    private readonly ConcurrentQueue<WechatRetirementEntry> _queue = new();
    private readonly Timer? _pumpTimer;
    private int _disposed;

    /// <summary>创建退役队列。</summary>
    /// <param name="retireDelaySeconds">宽限期（秒）；&lt;= 0 表示立即释放。</param>
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

        _queue.Enqueue(new WechatRetirementEntry(appKey, context, DateTimeOffset.UtcNow + _retireDelay));
    }

    private void Pump(object? state)
    {
        var now = DateTimeOffset.UtcNow;
        while (_queue.TryPeek(out var entry) && entry.RetireAt <= now)
        {
            if (!_queue.TryDequeue(out entry))
            {
                return;
            }

            try
            {
                entry.Context.Dispose();
                _logger.LogDebug("应用上下文已退役释放（AppKey: {AppKey}）。", entry.AppKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "应用上下文退役释放失败（AppKey: {AppKey}）。", entry.AppKey);
            }
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

        // 立即释放全部在队条目（管理器停机路径）。
        while (_queue.TryDequeue(out var entry))
        {
            try
            {
                entry.Context.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "停机释放退役上下文失败（AppKey: {AppKey}）。", entry.AppKey);
            }
        }
    }

    private readonly struct WechatRetirementEntry
    {
        public WechatRetirementEntry(string appKey, IWechatAppContext context, DateTimeOffset retireAt)
        {
            AppKey = appKey;
            Context = context;
            RetireAt = retireAt;
        }

        public string AppKey { get; }
        public IWechatAppContext Context { get; }
        public DateTimeOffset RetireAt { get; }
    }
}
