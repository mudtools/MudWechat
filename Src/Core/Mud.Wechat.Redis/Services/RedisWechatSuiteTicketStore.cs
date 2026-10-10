// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;
using Mud.Wechat.Work.Abstractions.Metrics;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// <see cref="IWechatSuiteTicketStore"/> 的 Redis 实现（按 suiteId 分槽，多套件 / 多代开发模板互不覆盖）。
/// </summary>
/// <remarks>
/// <para>
/// 键布局 <c>{KeyPrefix}:ticket:{suiteId}</c>；TTL 由 <see cref="WechatRedisOptions.SuiteTicketTtl"/> 决定
/// （RD5：默认 <see cref="TimeSpan.Zero"/> = 不带 TTL 永驻，对齐 InMemory 覆盖写保新鲜；
/// 正值建议 ≥ 2× 推送间隔即 ≥ 20 分钟——官方 suite_ticket 有效期 30 分钟、每 10 分钟推送覆盖写）。
/// </para>
/// <para>
/// <b>安全</b>：票据值为敏感凭据——不记日志、不进异常消息；键名（ticket:{suiteId}）可安全记录。
/// </para>
/// </remarks>
public class RedisWechatSuiteTicketStore : IWechatSuiteTicketStore
{
    /// <summary>票据域段名。</summary>
    private const string TicketSegment = "ticket";

    private readonly IConnectionMultiplexer _redis;
    private readonly WechatRedisOptions _options;
    private readonly ILogger<RedisWechatSuiteTicketStore>? _logger;

    /// <summary>创建 Redis 套件票据仓储。</summary>
    /// <param name="redis">连接多路复用器。</param>
    /// <param name="options">模块配置（取 <see cref="WechatRedisOptions.KeyPrefix"/> / <see cref="WechatRedisOptions.SuiteTicketTtl"/>）。</param>
    /// <param name="logger">日志器（可选）。</param>
    public RedisWechatSuiteTicketStore(IConnectionMultiplexer redis, WechatRedisOptions options, ILogger<RedisWechatSuiteTicketStore>? logger = null)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string?> GetAsync(string suiteId, CancellationToken cancellationToken = default)
    {
        var key = WechatRedisKeyBuilder.Combine(NormalizedPrefix, TicketSegment, suiteId);
        var metricsScope = RedisMetricsHelper.BeginOperation(null, WorkMetrics.RedisCommands.TicketGet);
        try
        {
            var value = await _redis.GetDatabase().StringGetAsync(key).ConfigureAwait(false);
            RedisMetricsHelper.RecordSuccess(null, WorkMetrics.RedisCommands.TicketGet);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            RedisMetricsHelper.RecordFailure(null, WorkMetrics.RedisCommands.TicketGet, ex);
            throw WechatRedisErrors.Map("读取套件票据", key, ex);
        }
        finally
        {
            metricsScope.Dispose();
        }
    }

    /// <inheritdoc />
    public async Task SetAsync(string suiteId, string ticket, CancellationToken cancellationToken = default)
    {
        var key = WechatRedisKeyBuilder.Combine(NormalizedPrefix, TicketSegment, suiteId);
        var metricsScope = RedisMetricsHelper.BeginOperation(null, WorkMetrics.RedisCommands.TicketSet);
        try
        {
            var ttl = _options.SuiteTicketTtl;
            var database = _redis.GetDatabase();
            if (ttl > TimeSpan.Zero)
            {
                await database.StringSetAsync(key, ticket ?? string.Empty, ttl, keepTtl: false).ConfigureAwait(false);
            }
            else
            {
                // 默认永驻（对齐 InMemory）；显式 when: 钉住经典重载（见 RedisWechatCorpAuthStore.SetAsync 注释）。
                await database.StringSetAsync(key, ticket ?? string.Empty, expiry: null, keepTtl: false).ConfigureAwait(false);
            }

            RedisMetricsHelper.RecordSuccess(null, WorkMetrics.RedisCommands.TicketSet);
            _logger?.LogInformation("已写入套件票据（suiteId = {SuiteId}，TTL = {Ttl}）。", suiteId ?? string.Empty, ttl);
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            RedisMetricsHelper.RecordFailure(null, WorkMetrics.RedisCommands.TicketSet, ex);
            throw WechatRedisErrors.Map("写入套件票据", key, ex);
        }
        finally
        {
            metricsScope.Dispose();
        }
    }

    private string NormalizedPrefix => WechatRedisOptions.NormalizeKeyPrefix(_options.KeyPrefix);
}
