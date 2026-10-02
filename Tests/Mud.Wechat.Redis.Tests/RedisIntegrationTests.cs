// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R9（可选）：真实 Redis 端到端集成用例骨架。
/// </summary>
/// <remarks>
/// 环境变量 <c>WECHAT_REDIS_TESTS_CONNECTION</c> 门控（缺失即跳过断言直接返回）；
/// CI 默认不跑（无 Redis 服务），本地 / 专项 job 启用。Cluster 形态连上后同样走通
/// （GetServers 聚合全部主节点）。
/// </remarks>
public class RedisIntegrationTests
{
    private const string ConnectionEnvVar = "WECHAT_REDIS_TESTS_CONNECTION";

    private static bool TryGetConnection(out string connection)
    {
        connection = Environment.GetEnvironmentVariable(ConnectionEnvVar) ?? string.Empty;
        return connection.Length > 0;
    }

    private static RedisWechatTokenStore CreateTokenStore(ConnectionMultiplexer multiplexer)
        => new(multiplexer, new WechatRedisOptions { KeyPrefix = "wechat-it" });

    [Fact]
    public async Task TokenStore_EndToEnd()
    {
        if (!TryGetConnection(out var connection))
        {
            return; // 门控跳过：CI 无 Redis 服务
        }

        await using var multiplexer = await ConnectionMultiplexer.ConnectAsync(connection);
        var store = CreateTokenStore(multiplexer);
        var storeKey = $"Wechat.AccessToken:it:{Environment.TickCount64:x8}";

        try
        {
            await store.SetAccessTokenAsync(storeKey, "it-token", 60);
            (await store.GetAccessTokenAsync(storeKey)).Should().Be("it-token");
            (await store.GetTokenTypesAsync()).Should().Contain(storeKey);
            (await store.RemoveRangeAsync(new[] { storeKey })).Should().Be(1);
            (await store.GetAccessTokenAsync(storeKey)).Should().BeNull();
        }
        finally
        {
            await multiplexer.GetDatabase().KeyDeleteAsync(WechatRedisKeyBuilder.TokenKey("wechat-it", storeKey));
        }
    }

    [Fact]
    public async Task ReplayGuard_EndToEnd()
    {
        if (!TryGetConnection(out var connection))
        {
            return;
        }

        await using var multiplexer = await ConnectionMultiplexer.ConnectAsync(connection);
        var guard = new RedisWechatCallbackReplayGuard(multiplexer, new WechatRedisOptions { KeyPrefix = "wechat-it" });
        var fingerprint = $"it-{Guid.NewGuid():N}";

        (await guard.TryMarkAsync(fingerprint, TimeSpan.FromSeconds(30))).Should().BeTrue();
        (await guard.TryMarkAsync(fingerprint, TimeSpan.FromSeconds(30))).Should().BeFalse();
        await multiplexer.GetDatabase().KeyDeleteAsync(WechatRedisKeyBuilder.Combine("wechat-it", "replay", fingerprint));
    }

    [Fact]
    public async Task CorpAuthStore_EndToEnd()
    {
        if (!TryGetConnection(out var connection))
        {
            return;
        }

        await using var multiplexer = await ConnectionMultiplexer.ConnectAsync(connection);
        var store = new RedisWechatCorpAuthStore(multiplexer, new WechatRedisOptions { KeyPrefix = "wechat-it" });
        var appKey = $"it{Environment.TickCount64:x8}";
        var auth = new WechatCorpAuthorization { AppKey = appKey, AuthCorpId = "corp-it", PermanentCode = "pc-it" };

        try
        {
            await store.SetAsync(auth);
            (await store.GetAsync(appKey, "corp-it"))!.PermanentCode.Should().Be("pc-it");
            (await store.ListAsync(appKey)).Should().ContainSingle(a => a.AuthCorpId == "corp-it");
        }
        finally
        {
            await store.RemoveAsync(appKey, "corp-it");
        }
    }
}
