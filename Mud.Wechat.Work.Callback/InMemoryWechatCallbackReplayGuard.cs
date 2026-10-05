// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

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
/// 过期回收为<b>机会式</b>（P2-1：触发间隔随存量自适应 <c>Max(64, Count/16)</c>，高存量下
/// 降低全量扫描频率，避免清理风暴），不引入后台线程/定时器（多实例部署由宿主替换为分布式实现）。
/// </para>
/// <para>
/// 语义安全：过期但未清理的条目<b>不可能</b>误拒合法新报文——指纹含 timestamp，指纹相同 ⇒
/// timestamp 相同 ⇒ 该报文必已被时间窗闸（±300s）拒绝；时间窗内的合法重试在保留窗口过期前到达，
/// 不受清理节奏影响。
/// </para>
/// </remarks>
public sealed class InMemoryWechatCallbackReplayGuard : IWechatCallbackReplayGuard
{
    /// <summary>触发全量清理的最小写入间隔（存量 <c>&lt; 1024</c> 条时恒为该值）。</summary>
    private const int MinCleanupInterval = 64;

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

        // P2-1：触发间隔随存量自适应——1000 QPS、30 万存量时约每 1.9 万次写入（≈19s）扫描一次，
        // 均摊成本从「每 64 次写入扫 30 万条」降为可忽略。
        var interval = Math.Max(MinCleanupInterval, _seen.Count / 16);
        if (Interlocked.Increment(ref _writes) % interval == 0)
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
