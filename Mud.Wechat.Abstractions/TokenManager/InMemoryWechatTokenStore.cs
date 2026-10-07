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
/// <remarks>进程内逐键删除即等价批量，直接实现批量能力接口（调用方探测后走一次提交）。</remarks>
public sealed class InMemoryWechatTokenStore : IWechatTokenStoreBatchRemove
{
    private sealed class StoreEntry
    {
        public string AccessToken = string.Empty;
        public long ExpiresAtMs;
    }

    private readonly ConcurrentDictionary<string, StoreEntry> _entries = new(StringComparer.Ordinal);

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
    public Task SetAccessTokenAsync(string tokenType, string accessToken, long expiresInSeconds, CancellationToken cancellationToken = default)
    {
        _entries[tokenType ?? string.Empty] = new StoreEntry
        {
            AccessToken = accessToken,
            ExpiresAtMs = NowMs() + Math.Max(0, expiresInSeconds) * 1000L,
        };
        return Task.CompletedTask;
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

    private static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
