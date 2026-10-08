// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;
using Mud.Wechat.Work.Abstractions.Metrics;

namespace Mud.Wechat.Redis.Services;

/// <summary>
/// <see cref="IWechatCorpAuthStore"/> 的 Redis 实现：企业授权聚合（含永久授权码）JSON 持久化。
/// </summary>
/// <remarks>
/// <para>
/// 键布局 <c>{KeyPrefix}:corpa:{appKey}:{authCorpId}</c>（复合键，组合段经
/// <see cref="WechatRedisKeyBuilder.Combine"/> 转义）；值不过期——凭据轮换 / 撤销授权经
/// <see cref="RemoveAsync"/> 显式删除（与 InMemory 语义一致）。
/// </para>
/// <para>
/// 序列化（RD4，条件编译）：NET8+ 走 <see cref="AuthenticationJsonContext"/> 的 <c>JsonTypeInfo</c>
///（AOT 安全，camelCase + 忽略 null）；ns2.0/net6 反射 STJ + pragma 收敛——AOT strict 门禁仅对 net8.0
/// 构建，该分支是「禁反射 JsonSerializer」红线的<b>唯一</b>开口（选项口径与 JsonContext 对齐，
/// 保证跨 TFM 存储格式一致）。
/// </para>
/// <para>
/// <b>安全</b>：聚合含 <c>PermanentCode</c>（不可重取凭据）——完整聚合 JSON 与字段值不得写入日志、
/// 遥测或异常消息；损坏数据只报键名。
/// </para>
/// </remarks>
public class RedisWechatCorpAuthStore : IWechatCorpAuthStore
{
    /// <summary>授权域段名。</summary>
    private const string CorpAuthSegment = "corpa";

    /// <summary>MGET 批读批大小。</summary>
    private const int ReadBatchSize = 500;

    /// <summary>SCAN 每页键数（对齐飞书模块）。</summary>
    private const int ScanPageSize = 250;

    private readonly IConnectionMultiplexer _redis;
    private readonly string _prefix;
    private readonly ILogger<RedisWechatCorpAuthStore>? _logger;

    /// <summary>创建 Redis 企业授权仓储。</summary>
    /// <param name="redis">连接多路复用器。</param>
    /// <param name="options">模块配置（取 <see cref="WechatRedisOptions.KeyPrefix"/>）。</param>
    /// <param name="logger">日志器（可选）。</param>
    public RedisWechatCorpAuthStore(IConnectionMultiplexer redis, WechatRedisOptions options, ILogger<RedisWechatCorpAuthStore>? logger = null)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        if (options == null) throw new ArgumentNullException(nameof(options));
        _prefix = WechatRedisOptions.NormalizeKeyPrefix(options.KeyPrefix);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<WechatCorpAuthorization?> GetAsync(string appKey, string authCorpId, CancellationToken cancellationToken = default)
    {
        var key = WechatRedisKeyBuilder.Combine(_prefix, CorpAuthSegment, appKey, authCorpId);
        var metricsScope = RedisMetricsHelper.BeginOperation(appKey, WorkMetrics.RedisCommands.CorpStoreGet);
        try
        {
            var json = await _redis.GetDatabase().StringGetAsync(key).ConfigureAwait(false);
            if (json.IsNullOrEmpty)
            {
                RedisMetricsHelper.RecordSuccess(appKey, WorkMetrics.RedisCommands.CorpStoreGet);
                return null;
            }

            try
            {
                var result = Deserialize(json!);
                RedisMetricsHelper.RecordSuccess(appKey, WorkMetrics.RedisCommands.CorpStoreGet);
                return result;
            }
            catch (JsonException ex)
            {
                // 聚合含永久授权码：损坏数据只报键名，绝不携带 JSON 内容。
                RedisMetricsHelper.RecordFailure(appKey, WorkMetrics.RedisCommands.CorpStoreGet, ex);
                throw new WechatRedisException(
                    WechatRedisFailureKind.Server,
                    $"企业授权记录反序列化失败（存储数据损坏，key = {key}），请联系运维清理该键后重新授权。",
                    ex);
            }
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            RedisMetricsHelper.RecordFailure(appKey, WorkMetrics.RedisCommands.CorpStoreGet, ex);
            throw WechatRedisErrors.Map("读取企业授权", key, ex);
        }
        finally
        {
            metricsScope.Dispose();
        }
    }

    /// <inheritdoc />
    public async Task SetAsync(WechatCorpAuthorization auth, CancellationToken cancellationToken = default)
    {
        if (auth == null) throw new ArgumentNullException(nameof(auth));

        var key = WechatRedisKeyBuilder.Combine(_prefix, CorpAuthSegment, auth.AppKey, auth.AuthCorpId);
        var metricsScope = RedisMetricsHelper.BeginOperation(auth.AppKey, WorkMetrics.RedisCommands.CorpStoreSet);
        try
        {
            // when: 显式传参钉住 (key,value,expiry,when) 经典重载——3.3.0 新增 Expiration 形态重载后
            // 裸 2/3 参调用的重载决胜不再确定，显式命名参数消除绑定漂移。
            await _redis.GetDatabase().StringSetAsync(key, Serialize(auth), expiry: null, keepTtl: false).ConfigureAwait(false);
            RedisMetricsHelper.RecordSuccess(auth.AppKey, WorkMetrics.RedisCommands.CorpStoreSet);
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            RedisMetricsHelper.RecordFailure(auth.AppKey, WorkMetrics.RedisCommands.CorpStoreSet, ex);
            throw WechatRedisErrors.Map("写入企业授权", key, ex);
        }
        finally
        {
            metricsScope.Dispose();
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string appKey, string authCorpId, CancellationToken cancellationToken = default)
    {
        var key = WechatRedisKeyBuilder.Combine(_prefix, CorpAuthSegment, appKey, authCorpId);
        var metricsScope = RedisMetricsHelper.BeginOperation(appKey, WorkMetrics.RedisCommands.CorpStoreRemove);
        try
        {
            await _redis.GetDatabase().KeyDeleteAsync(key).ConfigureAwait(false);
            RedisMetricsHelper.RecordSuccess(appKey, WorkMetrics.RedisCommands.CorpStoreRemove);
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            RedisMetricsHelper.RecordFailure(appKey, WorkMetrics.RedisCommands.CorpStoreRemove, ex);
            throw WechatRedisErrors.Map("删除企业授权", key, ex);
        }
        finally
        {
            metricsScope.Dispose();
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// SCAN <c>{prefix}:corpa:{appKey}:*</c>（模式经 <see cref="WechatRedisKeyBuilder.Pattern"/> 单一出口）+
    /// <see cref="ReadBatchSize"/> 每批 MGET 批读（避免 SCAN 后逐键 GET 的 N+1 往返）。
    /// 定位为 SaaS 批量运维 / 后台同步（非热路径）；授权企业规模极大时耗时可感知（见方案 §十四）。
    /// </remarks>
    public async Task<IReadOnlyList<WechatCorpAuthorization>> ListAsync(string appKey, CancellationToken cancellationToken = default)
    {
        var pattern = WechatRedisKeyBuilder.Pattern(_prefix, CorpAuthSegment, appKey);
        var keys = new List<RedisKey>();
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
                    keys.Add(key);
                }
            }
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("枚举企业授权键", pattern, ex);
        }

        var result = new List<WechatCorpAuthorization>(keys.Count);
        var database = _redis.GetDatabase();
        try
        {
            for (var offset = 0; offset < keys.Count; offset += ReadBatchSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var batch = keys.GetRange(offset, Math.Min(ReadBatchSize, keys.Count - offset));
                var values = await database.StringGetAsync(batch.ToArray()).ConfigureAwait(false);
                for (var i = 0; i < batch.Count; i++)
                {
                    var value = values[i];
                    if (value.IsNullOrEmpty)
                    {
                        continue;
                    }

                    try
                    {
                        var auth = Deserialize(value.ToString());
                        if (auth != null)
                        {
                            result.Add(auth);
                        }
                    }
                    catch (JsonException ex)
                    {
                        // 聚合含永久授权码：损坏数据只报键名，绝不携带 JSON 内容。
                        throw new WechatRedisException(
                            WechatRedisFailureKind.Server,
                            $"企业授权记录反序列化失败（存储数据损坏，key = {batch[i]}），请联系运维清理该键后重新授权。",
                            ex);
                    }
                }
            }
        }
        catch (Exception ex) when (WechatRedisErrors.ShouldWrap(ex))
        {
            throw WechatRedisErrors.Map("批量读取企业授权", pattern, ex);
        }

        _logger?.LogDebug("枚举应用 {AppKey} 的授权企业 {Count} 条。", appKey, result.Count);
        return result;
    }

#if NET8_0_OR_GREATER
    private static string Serialize(WechatCorpAuthorization auth)
        => JsonSerializer.Serialize(auth, AuthenticationJsonContext.Default.WechatCorpAuthorization);

    private static WechatCorpAuthorization? Deserialize(string json)
        => JsonSerializer.Deserialize(json, AuthenticationJsonContext.Default.WechatCorpAuthorization);
#else
    // 与 AuthenticationJsonContext 的 [JsonSourceGenerationOptions] 口径对齐（camelCase + 忽略 null + 大小写不敏感），
    // 保证跨 TFM 存储格式一致。
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private static string Serialize(WechatCorpAuthorization auth)
    {
        // RD4：ns2.0/net6 无 AuthenticationJsonContext（其本体为 NET8_0_OR_GREATER 条件编译）。
        // AOT strict 门禁仅对 net8.0 构建，本分支不参与 AOT；宿主在低 TFM 做 Native AOT 非受支持场景。
#pragma warning disable IL2026 // 反射序列化在裁剪下无法静态分析成员
        return JsonSerializer.Serialize(auth, SerializerOptions);
#pragma warning restore IL2026
    }

    private static WechatCorpAuthorization? Deserialize(string json)
    {
#pragma warning disable IL2026 // 反射反序列化在裁剪下无法静态分析成员
#pragma warning disable IL3050 // 反射反序列化在 AOT 下需动态代码
        return JsonSerializer.Deserialize<WechatCorpAuthorization>(json, SerializerOptions);
#pragma warning restore IL3050
#pragma warning restore IL2026
    }
#endif
}
