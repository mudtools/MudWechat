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
/// <see cref="IWechatCallbackReplayGuard"/> 的 Redis 实现：原子 <c>SET key 1 NX EX window</c> 一次性指纹闸。
/// </summary>
/// <remarks>
/// <para>
/// 键布局 <c>{KeyPrefix}:replay:{fingerprint}</c>（指纹为 SHA1，不可逆、含 PushToken 天然跨接收方隔离，
/// 无附加段）。首次返回 <c>true</c>；窗口内重复返回 <c>false</c>。
/// </para>
/// <para>
/// <b>RD3（fail-closed 失败语义）</b>：Redis 异常分类包装后上抛——接收器对 <see cref="TryMarkAsync"/>
/// 无捕获面，异常冒泡为回调 5xx，官方 96238 重试可重新进入管线（解密成功但指纹闸异常不构成消费，
/// 与 P1-1「解密失败不消耗指纹」同方向）。两个反模式显式禁止：吞异常返回 <c>true</c>（放行重放）、
/// 吞异常返回 <c>false</c>（静默丢事件且官方不再重试）。
/// </para>
/// <para>
/// <b>审查报告 v1 P1-1</b>：外层包装为叶层契约异常 <see cref="WechatCallbackInfrastructureException"/>
/// （管线宿主的专用 catch 映射 503，与验证类失败的 403 分流），<see cref="WechatRedisException"/>
/// 作为 <see cref="Exception.InnerException"/> 保留——既有分类语义（<see cref="WechatRedisException.FailureKind"/>）
/// 经 Inner 可达，宿主对 Redis 异常的既有捕获面零破坏。
/// </para>
/// </remarks>
public class RedisWechatCallbackReplayGuard : IWechatCallbackReplayGuard
{
    /// <summary>重放域段名。</summary>
    private const string ReplaySegment = "replay";

    private readonly IConnectionMultiplexer _redis;
    private readonly string _prefix;

    /// <summary>创建 Redis 回调重放守卫。</summary>
    /// <param name="redis">连接多路复用器。</param>
    /// <param name="options">模块配置（取 <see cref="WechatRedisOptions.KeyPrefix"/>）。</param>
    public RedisWechatCallbackReplayGuard(IConnectionMultiplexer redis, WechatRedisOptions options)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        if (options == null) throw new ArgumentNullException(nameof(options));
        _prefix = WechatRedisOptions.NormalizeKeyPrefix(options.KeyPrefix);
    }

    /// <inheritdoc />
    public async Task<bool> TryMarkAsync(string key, TimeSpan window, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
        {
            // 空键视为无法去重：返回 false 且不触达 Redis（与 InMemory 空键语义对齐，
            // 防「空指纹共享同一键」的互相覆盖）。
            return false;
        }

        if (window <= TimeSpan.Zero)
        {
            // 守卫调用方常量（ReplayWindowSeconds）恒正，此为防御断言。
            throw new ArgumentOutOfRangeException(nameof(window), "重放保留窗口必须为正值。");
        }

        var redisKey = WechatRedisKeyBuilder.Combine(_prefix, ReplaySegment, key);
        var metricsScope = RedisMetricsHelper.BeginOperation(null, WorkMetrics.RedisCommands.ReplayTryMark);
        try
        {
            var result = await _redis.GetDatabase()
                .StringSetAsync(redisKey, "1", window, keepTtl: false, when: When.NotExists)
                .ConfigureAwait(false);
            RedisMetricsHelper.RecordSuccess(null, WorkMetrics.RedisCommands.ReplayTryMark);
            return result;
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            // RD3：fail-closed——异常上抛（不得吞成 true/false）。
            // 审查报告 v1 P1-1：包装为契约层基础设施异常（中间件专用 catch 映射 503 → 官方 96238 重推；
            // 严禁落入验证类 403——Redis 故障期事件会静默丢失）。WechatRedisException 作为
            // InnerException 保留，既有分类语义与宿主捕获面零破坏。
            RedisMetricsHelper.RecordFailure(null, WorkMetrics.RedisCommands.ReplayTryMark, ex);
            throw new WechatCallbackInfrastructureException(
                $"回调重放指纹标记失败（分布式守卫存储不可用）：{redisKey}",
                WechatRedisErrors.Map("回调重放指纹标记", redisKey, ex));
        }
        finally
        {
            metricsScope.Dispose();
        }
    }
}
