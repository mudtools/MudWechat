// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// <see cref="IWechatCallbackReplayGuard"/> 的进程内默认实现（并发字典 + 机会式过期回收，无 Timer）。
/// </summary>
/// <remarks>
/// <para>
/// 单 key 语义幂等：<see cref="ConcurrentDictionary{TKey,TValue}.TryAdd"/> 的原子性保证
/// 「并发同键只有一个调用者获得 <c>true</c>」。
/// </para>
/// <para>
/// 过期回收为<b>机会式</b>（每 <see cref="CleanupInterval"/> 次写入触发一次全量清理），
/// 避免长时间运行下无界增长，同时不引入后台线程/定时器（多实例部署由宿主替换为分布式实现）。
/// </para>
/// </remarks>
public sealed class InMemoryWechatCallbackReplayGuard : IWechatCallbackReplayGuard
{
    private const int CleanupInterval = 64;

    private readonly ConcurrentDictionary<string, long> _seen = new(StringComparer.Ordinal);
    private int _writes;

    /// <inheritdoc />
    public Task<bool> TryMarkAsync(string key, TimeSpan window, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            // 空键视为无法去重：fail-closed 由调用方决定，这里返回 false 以免「空键互相覆盖」造成误放行。
            return Task.FromResult(false);
        }

        var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var retainedUntilMs = nowMs + (long)Math.Max(0, window.TotalMilliseconds);

        if (!_seen.TryAdd(key, retainedUntilMs))
        {
            return Task.FromResult(false);
        }

        if (Interlocked.Increment(ref _writes) % CleanupInterval == 0)
        {
            Cleanup(nowMs);
        }

        return Task.FromResult(true);
    }

    private void Cleanup(long nowMs)
    {
        foreach (var pair in _seen)
        {
            if (pair.Value <= nowMs)
            {
                _seen.TryRemove(pair.Key, out _);
            }
        }
    }
}
