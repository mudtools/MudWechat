// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// 企业微信令牌持久化仓储（可选的跨进程共享 / 落盘层）。
/// </summary>
/// <remarks>
/// <para>
/// 定位（Mud.HttpUtils v2.0.9 契约核实）：令牌的进程内缓存、过期判定、并发刷新去重由
/// <see cref="TokenManagerBase"/> 内置（<see cref="ITokenCache{T}"/>，可在管理器构造时注入替换实现）。
/// 本仓储重新定位为<b>可选的持久化 / 跨进程共享层</b>（多实例部署共享令牌或令牌落盘时实现），
/// 默认注册为进程内实现、可不接分布式存储。
/// </para>
/// <para>
/// 启用持久化时，<c>WechatAppTokenManagerBase</c> 经框架桥接器
/// <see cref="TokenStoreBackedTokenCache{T}"/> 将本仓储接入管理器管线
/// （内存镜像 + 异步写穿 + 补偿重放 + 冷启动水合/读穿透），无需自建写穿/恢复叠层。
/// </para>
/// </remarks>
public interface IWechatTokenStore : ITokenStore
{
}

/// <summary>
/// 持久化用令牌快照（SDK 自有 DTO，与框架 <see cref="CredentialToken"/> 互转）。
/// </summary>
public sealed class WechatTokenSnapshot
{
    /// <summary>创建令牌快照。</summary>
    public WechatTokenSnapshot(string accessToken, long expire, long issuedAt)
    {
        AccessToken = accessToken;
        Expire = expire;
        IssuedAt = issuedAt;
    }

    /// <summary>访问令牌。</summary>
    public string AccessToken { get; }

    /// <summary>过期时间（Unix 毫秒时间戳）。</summary>
    public long Expire { get; }

    /// <summary>签发时间（Unix 毫秒时间戳）。</summary>
    public long IssuedAt { get; }
}

/// <summary>
/// <see cref="IWechatTokenStore"/> 的进程内默认实现（并发字典 + 绝对过期判定）。
/// </summary>
public sealed class InMemoryWechatTokenStore : IWechatTokenStore
{
    private sealed class StoreEntry
    {
        public string AccessToken = string.Empty;
        public long ExpiresAtMs;
    }

    private readonly ConcurrentDictionary<string, StoreEntry> _entries = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<string?> GetAccessTokenAsync(string tokenType, CancellationToken cancellationToken = default)
    {
        if (_entries.TryGetValue(tokenType ?? string.Empty, out var entry) && entry.ExpiresAtMs > NowMs())
        {
            return Task.FromResult<string?>(entry.AccessToken);
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

    private static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
