// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;

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
/// <b>RD3（fail-closed 失败语义）</b>：Redis 异常原样包装上抛——接收器对 <see cref="TryMarkAsync"/>
/// 无捕获面，异常冒泡为回调 5xx，官方 96238 重试可重新进入管线（解密成功但指纹闸异常不构成消费，
/// 与 P1-1「解密失败不消耗指纹」同方向）。两个反模式显式禁止：吞异常返回 <c>true</c>（放行重放）、
/// 吞异常返回 <c>false</c>（静默丢事件且官方不再重试）。
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
        try
        {
            return await _redis.GetDatabase()
                .StringSetAsync(redisKey, "1", window, keepTtl: false, when: When.NotExists)
                .ConfigureAwait(false);
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            // RD3：fail-closed——异常上抛（不得吞成 true/false）。
            throw WechatRedisErrors.Map("回调重放指纹标记", redisKey, ex);
        }
    }
}
