// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// 启动期连接预热（PING），对齐 Mud.Feishu.Redis 的 <c>RedisConnectionWarmupService</c>。
/// </summary>
/// <remarks>
/// <para>
/// 全 TFM 注册（R-2 改判）：<c>Microsoft.Extensions.Hosting.Abstractions</c> 经 Abstractions 包全 TFM 引用
/// （ns2.0 → 8.0.1）；无通用 Host 的 ns2.0 宿主不会启动 HostedService，无害。
/// </para>
/// <para>
/// <b>AbortOnConnectFail 语义</b>：连接建立失败在 <c>IConnectionMultiplexer</c> 注册处已抛；
/// 本服务只处理「连接成功但 PING 失败」——<see cref="WechatRedisConnectionOptions.AbortOnConnectFail"/>=true
/// （默认）时异常上抛终止启动（fail-fast），否则告警放行交由自动重连。
/// </para>
/// </remarks>
public class RedisConnectionWarmupService : IHostedService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisConnectionWarmupService>? _logger;
    private readonly bool _abortOnConnectFail;

    /// <summary>创建连接预热服务。</summary>
    /// <param name="redis">连接多路复用器。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <param name="abortOnConnectFail">PING 失败时是否终止启动（来自 <see cref="WechatRedisConnectionOptions.AbortOnConnectFail"/>）。</param>
    public RedisConnectionWarmupService(
        IConnectionMultiplexer redis,
        ILogger<RedisConnectionWarmupService>? logger = null,
        bool abortOnConnectFail = true)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger;
        _abortOnConnectFail = abortOnConnectFail;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation(
            "Redis 连接预热开始（端点：{Endpoints}）。",
            string.Join(",", _redis.GetEndPoints().Select(e => e?.ToString() ?? string.Empty)));

        try
        {
            var latency = await _redis.GetDatabase().PingAsync().ConfigureAwait(false);
            _logger?.LogInformation("Redis 连接预热完成，PING 延迟 {LatencyMs}ms。", latency.TotalMilliseconds);
        }
        catch (Exception ex) when (!_abortOnConnectFail)
        {
            _logger?.LogWarning(ex, "Redis 启动预热 PING 失败（AbortOnConnectFail=false），不阻断启动，等待自动重连。");
        }
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
