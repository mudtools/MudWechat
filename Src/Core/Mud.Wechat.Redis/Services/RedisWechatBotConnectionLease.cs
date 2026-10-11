// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;
using Mud.Wechat.Work.Abstractions.Callback.Bots;

namespace Mud.Wechat.Redis;

/// <summary>
/// 智能机器人长连接<b>租约</b>的 Redis 实现（<c>SET NX EX</c> 夺取 + Lua 原子续租/释放，
/// 支撑多实例部署的主备切换 —— 官方 101463 每机器人仅一条有效连接）。
/// </summary>
/// <remarks>
/// <para>
/// <b>键形态</b>：<c>{KeyPrefix}bot:lease:{botKey}</c>，值为实例标识。TTL 由调用方传入
/// （须大于续租周期；持有者崩溃后等 TTL 自然过期，其他实例即可夺取）。
/// </para>
/// <para>
/// <b>原子性</b>：续租与释放必须「值相等才生效」—— 两步非原子实现会有「校验通过后租约被夺、
/// 却续到了别人头上」的窗口，故一律走 Lua（服务端单脚本原子执行）。
/// <c>SET</c> 按 AGENTS §5.2 用<b>命名参数</b>钉住形态（<c>keepTtl: false</c> / <c>when: When.NotExists</c>）。
/// </para>
/// <para>
/// <b>异常包装</b>：走 <see cref="WechatRedisErrors.ShouldWrap"/>（<c>RedisTimeoutException</c>
/// 继承 <c>TimeoutException</c> 而非 <c>RedisException</c>，裸 catch 会漏超时）。
/// </para>
/// </remarks>
public class RedisWechatBotConnectionLease : IWechatBotConnectionLease
{
    private const string RenewScript =
        "if redis.call('GET', KEYS[1]) == ARGV[1] then " +
        "return redis.call('PEXPIRE', KEYS[1], ARGV[2]) " +
        "else return 0 end";

    private const string ReleaseScript =
        "if redis.call('GET', KEYS[1]) == ARGV[1] then " +
        "return redis.call('DEL', KEYS[1]) " +
        "else return 0 end";

    private readonly IConnectionMultiplexer _redis;
    private readonly string _prefix;
    private readonly ILogger<RedisWechatBotConnectionLease>? _logger;

    /// <summary>创建长连接租约存储。</summary>
    /// <param name="redis">Redis 连接（<c>AddWechatRedis</c> 装配）。</param>
    /// <param name="options">Redis 配置（键前缀来源）。</param>
    /// <param name="logger">日志器（可选）。</param>
    public RedisWechatBotConnectionLease(
        IConnectionMultiplexer redis, WechatRedisOptions options, ILogger<RedisWechatBotConnectionLease>? logger = null)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _prefix = WechatRedisOptions.NormalizeKeyPrefix(
            (options ?? throw new ArgumentNullException(nameof(options))).KeyPrefix);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> TryAcquireAsync(
        string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(botKey))
        {
            throw new ArgumentException("机器人键不能为空。", nameof(botKey));
        }

        if (string.IsNullOrWhiteSpace(instanceId))
        {
            throw new ArgumentException("实例标识不能为空。", nameof(instanceId));
        }

        try
        {
            var redis = _redis.GetDatabase();
            return await redis.StringSetAsync(
                Key(botKey),
                instanceId,
                ttl,
                keepTtl: false,
                when: StackExchange.Redis.When.NotExists).ConfigureAwait(false);
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw new WechatRedisException(
                WechatRedisFailureKind.Server,
                $"智能机器人「{botKey}」租约夺取失败（Redis 读写异常）。",
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> RenewAsync(
        string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(botKey))
        {
            throw new ArgumentException("机器人键不能为空。", nameof(botKey));
        }

        if (string.IsNullOrWhiteSpace(instanceId))
        {
            throw new ArgumentException("实例标识不能为空。", nameof(instanceId));
        }

        try
        {
            var redis = _redis.GetDatabase();
            var renewed = (int)await redis.ScriptEvaluateAsync(
                RenewScript,
                [Key(botKey)],
                [instanceId, ((long)ttl.TotalMilliseconds).ToString(System.Globalization.CultureInfo.InvariantCulture)])
                .ConfigureAwait(false);
            return renewed == 1;
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw new WechatRedisException(
                WechatRedisFailureKind.Server,
                $"智能机器人「{botKey}」租约续期失败（Redis 读写异常）。",
                ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ReleaseAsync(
        string botKey, string instanceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(botKey))
        {
            throw new ArgumentException("机器人键不能为空。", nameof(botKey));
        }

        if (string.IsNullOrWhiteSpace(instanceId))
        {
            throw new ArgumentException("实例标识不能为空。", nameof(instanceId));
        }

        try
        {
            var redis = _redis.GetDatabase();
            var released = (int)await redis.ScriptEvaluateAsync(
                ReleaseScript,
                [Key(botKey)],
                [instanceId])
                .ConfigureAwait(false);
            return released == 1;
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw new WechatRedisException(
                WechatRedisFailureKind.Server,
                $"智能机器人「{botKey}」租约释放失败（Redis 读写异常）。",
                ex);
        }
    }

    private string Key(string botKey) => $"{_prefix}bot:lease:{botKey}";
}
