// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// <see cref="IWechatTokenStore"/> 的 Redis 实现（多实例共享令牌持久层），
/// 并实现 <see cref="IWechatTokenStoreBatchRemove"/>——W1/M10 批量删除能力接口的首个 Redis 落地。
/// </summary>
/// <remarks>
/// <para>
/// 键布局 <c>{KeyPrefix}:token:{storeKey}</c>（RD2：<c>storeKey</c> 为不透明叶子原样拼接，
/// 与 <c>InMemoryWechatTokenStore</c> 键空间逐字节一致）。值为调用方传入的不透明令牌字符串
/// （组件桥接器写入 <c>{expireMs}|{token}</c> 编码形态），本存储原样存取、不编解码。
/// </para>
/// <para>
/// TTL 语义：TTL 完全跟随调用方 <c>expiresInSeconds</c>（RD12 无封顶旋钮）；
/// <c>expiresInSeconds &lt;= 0</c> 时<b>删键</b>（RD10，对齐 InMemory「立即过期」语义，
/// 并规避 SE.Redis <c>TimeSpan.Zero</c> = 永不过期的陷阱）。refresh token 契约与 InMemory 逐成员一致：
/// 恒 null / no-op（WeChat 无 refresh token 流）。
/// </para>
/// <para>
/// 令牌值与 suite_ticket 同级敏感：不写日志、不进异常消息；键名（含 appKey/scope，均非凭据）可安全记录。
/// 异常映射：<see cref="RedisException"/> 家族 → <see cref="WechatRedisException"/> 上抛，不吞异常。
/// </para>
/// </remarks>
public class RedisWechatTokenStore : IWechatTokenStoreBatchRemove
{
    /// <summary>SCAN 每页键数（对齐飞书模块）。</summary>
    private const int ScanPageSize = 250;

    /// <summary>批量删除每批键数（R2-08：减少 RTT 与「删一半后异常」窗口）。</summary>
    private const int DeleteBatchSize = 500;

    private readonly IConnectionMultiplexer _redis;
    private readonly string _prefix;
    private readonly ILogger<RedisWechatTokenStore>? _logger;

    /// <summary>创建 Redis 令牌存储。</summary>
    /// <param name="redis">连接多路复用器。</param>
    /// <param name="options">模块配置（取 <see cref="WechatRedisOptions.KeyPrefix"/>）。</param>
    /// <param name="logger">日志器（可选）。</param>
    public RedisWechatTokenStore(IConnectionMultiplexer redis, WechatRedisOptions options, ILogger<RedisWechatTokenStore>? logger = null)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        if (options == null) throw new ArgumentNullException(nameof(options));
        _prefix = WechatRedisOptions.NormalizeKeyPrefix(options.KeyPrefix);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string?> GetAccessTokenAsync(string tokenType, CancellationToken cancellationToken = default)
    {
        var key = WechatRedisKeyBuilder.TokenKey(_prefix, tokenType);
        try
        {
            var value = await _redis.GetDatabase().StringGetAsync(key).ConfigureAwait(false);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("读取令牌", key, ex);
        }
    }

    /// <inheritdoc />
    public async Task SetAccessTokenAsync(string tokenType, string accessToken, long expiresInSeconds, CancellationToken cancellationToken = default)
    {
        var key = WechatRedisKeyBuilder.TokenKey(_prefix, tokenType);
        try
        {
            var database = _redis.GetDatabase();
            if (expiresInSeconds <= 0)
            {
                // RD10：<=0 视为「立即过期」，执行删键而非写 TTL=0（SE.Redis 的 TimeSpan.Zero = 永不过期，方向相反）。
                await database.KeyDeleteAsync(key).ConfigureAwait(false);
                return;
            }

            await database
                .StringSetAsync(key, accessToken ?? string.Empty, TimeSpan.FromSeconds(expiresInSeconds), keepTtl: false)
                .ConfigureAwait(false);
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("写入令牌", key, ex);
        }
    }

    /// <inheritdoc />
    /// <remarks>WeChat 无 refresh token 流，恒返回 null（与 InMemory 逐成员一致）。</remarks>
    public Task<string?> GetRefreshTokenAsync(string tokenType, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>(null);

    /// <inheritdoc />
    /// <remarks>WeChat 无 refresh token 流，no-op（与 InMemory 逐成员一致）。</remarks>
    public Task SetRefreshTokenAsync(string tokenType, string refreshToken, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public async Task RemoveAsync(string tokenType, CancellationToken cancellationToken = default)
    {
        var key = WechatRedisKeyBuilder.TokenKey(_prefix, tokenType);
        try
        {
            await _redis.GetDatabase().KeyDeleteAsync(key).ConfigureAwait(false);
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("删除令牌", key, ex);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// SCAN 运维/清库路径（非热路径，消费点仅 <c>WechatAppManager.PurgeAppTokensAsync</c> /
    /// <c>PurgeStoreAsync</c>）：Cluster 下聚合全部主节点、副本跳过，模式经
    /// <see cref="WechatRedisKeyBuilder.Pattern"/> 单一出口产出；按已知字面前缀剥离还原 storeKey（RD2）。
    /// </remarks>
    public async Task<IEnumerable<string>> GetTokenTypesAsync(CancellationToken cancellationToken = default)
    {
        var pattern = WechatRedisKeyBuilder.Pattern(_prefix, "token");
        var result = new HashSet<string>(StringComparer.Ordinal);
        try
        {
            var databaseId = _redis.GetDatabase().Database;
            foreach (var server in RedisServerHelper.GetServers(_redis))
            {
                await foreach (var key in server
                    .KeysAsync(databaseId, pattern, ScanPageSize, flags: RedisServerHelper.ToCommandFlags(cancellationToken))
                    .ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (WechatRedisKeyBuilder.TryStripTokenKeyPrefix(key.ToString(), _prefix, out var storeKey))
                    {
                        result.Add(storeKey);
                    }
                }
            }
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("枚举令牌键", pattern, ex);
        }

        return result;
    }

    /// <inheritdoc />
    /// <remarks>与 <see cref="GetTokenTypesAsync"/> 同一 SCAN 面，<see cref="DeleteBatchSize"/> 每批删除。</remarks>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        var pattern = WechatRedisKeyBuilder.Pattern(_prefix, "token");
        var database = _redis.GetDatabase();
        var removed = 0;
        var batch = new List<RedisKey>(DeleteBatchSize);
        try
        {
            foreach (var server in RedisServerHelper.GetServers(_redis))
            {
                await foreach (var key in server
                    .KeysAsync(database.Database, pattern, ScanPageSize, flags: RedisServerHelper.ToCommandFlags(cancellationToken))
                    .ConfigureAwait(false))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    batch.Add(key);
                    if (batch.Count >= DeleteBatchSize)
                    {
                        removed += await DeleteBatchAsync(database, batch).ConfigureAwait(false);
                    }
                }
            }

            if (batch.Count > 0)
            {
                removed += await DeleteBatchAsync(database, batch).ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("清空令牌", pattern, ex);
        }

        if (removed > 0)
        {
            _logger?.LogInformation("已清空 Redis 令牌前缀 {Prefix} 下 {Count} 个键。", _prefix, removed);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// 入参为 storeKey（三段式持久层键，来自 <see cref="GetTokenTypesAsync"/> 的返回口径），
    /// 经 <see cref="WechatRedisKeyBuilder.TokenKey"/> 映射为 Redis 键后 <see cref="DeleteBatchSize"/> 每批删除
    /// ——N 次往返收敛为 ⌈N/500⌉ 次（W1/M10）。
    /// </remarks>
    public async Task<int> RemoveRangeAsync(IReadOnlyCollection<string> keys, CancellationToken cancellationToken = default)
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));

        var database = _redis.GetDatabase();
        var removed = 0;
        var batch = new List<RedisKey>(Math.Min(keys.Count, DeleteBatchSize));
        try
        {
            foreach (var storeKey in keys)
            {
                if (storeKey == null)
                {
                    continue;
                }

                batch.Add(WechatRedisKeyBuilder.TokenKey(_prefix, storeKey));
                if (batch.Count >= DeleteBatchSize)
                {
                    removed += await DeleteBatchAsync(database, batch).ConfigureAwait(false);
                }
            }

            if (batch.Count > 0)
            {
                removed += await DeleteBatchAsync(database, batch).ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("批量删除令牌", $"{_prefix}:token:({batch.Count} 键)", ex);
        }

        return removed;
    }

    private static async Task<int> DeleteBatchAsync(IDatabase database, List<RedisKey> batch)
    {
        try
        {
            var removed = await database.KeyDeleteAsync(batch.ToArray()).ConfigureAwait(false);
            return (int)removed;
        }
        finally
        {
            batch.Clear();
        }
    }
}
