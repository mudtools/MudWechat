// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Mud.Wechat.Redis.HealthChecks;

/// <summary>
/// Redis 健康检查：唯一判据为 PING 是否成功（R2-12 同款）。
/// </summary>
/// <remarks>
/// 端点连通数仅作 <c>data</c> 呈现、端点级异常不影响整体判定——避免「部分端点抖动即整体 Unhealthy」
/// 的误报（Cluster 场景由 SE.Redis 自动重连恢复）。
/// </remarks>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    /// <summary>创建 Redis 健康检查。</summary>
    /// <param name="redis">连接多路复用器。</param>
    public RedisHealthCheck(IConnectionMultiplexer redis)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var latency = await _redis.GetDatabase().PingAsync().ConfigureAwait(false);
            var (connected, total) = CountEndpoints();
            var data = new Dictionary<string, object>
            {
                ["latency"] = latency.TotalMilliseconds,
                ["connectedEndpoints"] = connected,
                ["totalEndpoints"] = total,
            };
            return HealthCheckResult.Healthy("Redis is healthy", data);
        }
        catch (RedisException ex)
        {
            return HealthCheckResult.Unhealthy("Redis connection failed", ex, new Dictionary<string, object>
            {
                ["message"] = ex.Message,
            });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis health check failed", ex, new Dictionary<string, object>
            {
                ["message"] = ex.Message,
            });
        }
    }

    private (int Connected, int Total) CountEndpoints()
    {
        var endpoints = _redis.GetEndPoints();
        var connected = 0;
        foreach (var endpoint in endpoints)
        {
            try
            {
                if (_redis.GetServer(endpoint).IsConnected)
                {
                    connected++;
                }
            }
            catch
            {
                // 端点级异常只影响计数，不影响整体判定（R2-12）。
            }
        }

        return (connected, endpoints.Length);
    }
}
