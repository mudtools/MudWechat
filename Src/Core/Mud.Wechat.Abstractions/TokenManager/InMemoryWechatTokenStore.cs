// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.TokenManager;

/// <summary>
/// <see cref="IWechatTokenStore"/> 的进程内默认实现（并发字典 + 绝对过期判定），各产品线共用。
/// </summary>
/// <remarks>
/// 进程内逐键删除即等价批量，直接实现批量能力接口（调用方探测后走一次提交）；
/// 并实现官方 U-4 按应用清库接口 <see cref="ITokenStoreAppPurge"/>（键归属判定走
/// <see cref="WechatTokenStoreBridge.IsStoreKeyForApp"/> 共享实现，与 Redis 实现同源）。
/// </remarks>
public sealed class InMemoryWechatTokenStore : IWechatTokenStoreBatchRemove, ITokenStoreAppPurge
{
    private sealed class StoreEntry
    {
        public string AccessToken = string.Empty;
        public long ExpiresAtMs;
    }

    private readonly ConcurrentDictionary<string, StoreEntry> _entries = new(StringComparer.Ordinal);

    /// <summary>进程内槽位容量上限（超限即先清过期项、再驱逐最早到期项；驱逐有效令牌仅损失一次缓存命中）。</summary>
    private const int Capacity = 8192;

    /// <inheritdoc />
    /// <remarks>
    /// <b>P1-9</b>：过期条目在<b>读路径</b>即回收（O(1)）—— 若只在「读取时判过期返回 null」，
    /// 条目（含令牌明文）会永久驻留 <c>_entries</c>，长时间运行 + 多 scope 下无界增长。
    /// </remarks>
    public Task<string?> GetAccessTokenAsync(string tokenType, CancellationToken cancellationToken = default)
    {
        var key = tokenType ?? string.Empty;
        if (_entries.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAtMs > NowMs())
            {
                return Task.FromResult<string?>(entry.AccessToken);
            }

            _entries.TryRemove(key, out _);
        }

        return Task.FromResult<string?>(null);
    }

    /// <inheritdoc />
    /// <remarks>
    /// <b>P2-7</b>：写入即做容量回收（读路径只回收被访问键，<b>写入后不再被读取</b>的键——如企业卸载后的
    /// scope 槽位——会永久驻留 <c>_entries</c>）。回收仅在超限时触发（先清过期项、再驱逐最早到期项），
    /// 摊销成本可忽略；驱逐有效令牌等价于一次缓存未命中，不改变对外语义。
    /// </remarks>
    public Task SetAccessTokenAsync(string tokenType, string accessToken, long expiresInSeconds, CancellationToken cancellationToken = default)
    {
        _entries[tokenType ?? string.Empty] = new StoreEntry
        {
            AccessToken = accessToken,
            ExpiresAtMs = NowMs() + Math.Max(0, expiresInSeconds) * 1000L,
        };

        if (_entries.Count > Capacity)
        {
            Evict();
        }

        return Task.CompletedTask;
    }

    /// <summary>先清已过期项；仍超上限时按到期时间升序驱逐最早到期项。</summary>
    private void Evict()
    {
        var now = NowMs();
        foreach (var pair in _entries)
        {
            if (pair.Value.ExpiresAtMs <= now)
            {
                _entries.TryRemove(pair.Key, out _);
            }
        }

        var overflow = _entries.Count - Capacity;
        if (overflow <= 0)
        {
            return;
        }

        var oldestKeys = _entries.OrderBy(static p => p.Value.ExpiresAtMs)
            .Take(overflow)
            .Select(static p => p.Key)
            .ToList();
        foreach (var key in oldestKeys)
        {
            _entries.TryRemove(key, out _);
        }
    }

    /// <inheritdoc />
    public Task<string?> GetRefreshTokenAsync(string tokenType, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    /// <inheritdoc />
    public Task SetRefreshTokenAsync(string tokenType, string refreshToken, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task RemoveAsync(string tokenType, CancellationToken cancellationToken = default)
    {
        _entries.TryRemove(tokenType ?? string.Empty, out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> GetTokenTypesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<string>>(_entries.Keys.ToArray());

    /// <inheritdoc />
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        _entries.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<int> RemoveRangeAsync(IReadOnlyCollection<string> keys, CancellationToken cancellationToken = default)
    {
        var removed = 0;
        foreach (var key in keys)
        {
            if (key != null && _entries.TryRemove(key, out _))
            {
                removed++;
            }
        }

        return Task.FromResult(removed);
    }

    /// <inheritdoc />
    /// <remarks>
    /// 快照语义：先物化匹配键数组再返回。返回口径与 <see cref="GetTokenTypesAsync"/> 一致
    /// （完整持久层键 <c>{tokenType}:{appKey}:{scopeKey}</c>，可直接回传 <see cref="RemoveRangeAsync"/>）。
    /// 空白 <paramref name="appKey"/> 返回空集（本院键空间无官方 <c>shared</c> 式共享段约定）。
    /// </remarks>
    public Task<IReadOnlyCollection<string>> EnumerateKeysAsync(string appKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            return Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());
        }

        IReadOnlyCollection<string> keys = _entries.Keys
            .Where(key => WechatTokenStoreBridge.IsStoreKeyForApp(key, appKey))
            .ToArray();
        return Task.FromResult(keys);
    }

    /// <inheritdoc />
    /// <remarks>枚举 → <see cref="RemoveRangeAsync"/>（计数为实际删除键数，无静默半删）。</remarks>
    public async Task<int> PurgeAppAsync(string appKey, CancellationToken cancellationToken = default)
    {
        var keys = await EnumerateKeysAsync(appKey, cancellationToken).ConfigureAwait(false);
        return await RemoveRangeAsync(keys, cancellationToken).ConfigureAwait(false);
    }

    private static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
