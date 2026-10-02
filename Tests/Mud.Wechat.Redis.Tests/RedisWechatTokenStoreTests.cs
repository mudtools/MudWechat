// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R2：令牌存储（键布局逐字节 / 删键语义 / 批量删除映射 / SCAN 前缀剥离 / 异常分类）。
/// </summary>
public class RedisWechatTokenStoreTests
{
    private const string StoreKey = "Wechat.AccessToken:default:default";
    private const string RedisKey = "wechat:token:Wechat.AccessToken:default:default";

    private static RedisWechatTokenStore CreateStore(FakeRedisBackend backend, string keyPrefix = "wechat")
        => new(backend.Multiplexer.Object, new WechatRedisOptions { KeyPrefix = keyPrefix });

    [Fact]
    public async Task GetAccessTokenAsync_ShouldUseLiteralTokenLayoutKey()
    {
        var backend = new FakeRedisBackend();
        await CreateStore(backend).GetAccessTokenAsync(StoreKey);

        backend.Database.Verify(d => d.StringGetAsync(RedisKey, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldReturnValue_WhenKeyExists()
    {
        var backend = new FakeRedisBackend();
        backend.Database.Setup(d => d.StringGetAsync(RedisKey, It.IsAny<CommandFlags>()))
            .ReturnsAsync("token-value");

        (await CreateStore(backend).GetAccessTokenAsync(StoreKey)).Should().Be("token-value");
    }

    [Fact]
    public async Task SetAccessTokenAsync_ShouldWriteWithTtl_WhenExpiresInSecondsPositive()
    {
        var backend = new FakeRedisBackend();
        await CreateStore(backend).SetAccessTokenAsync(StoreKey, "token-value", 7200);

        backend.LastExpiry.Should().Be(TimeSpan.FromSeconds(7200));
        backend.Store[RedisKey].ToString().Should().Be("token-value");
    }

    [Fact]
    public async Task SetAccessTokenAsync_ShouldDeleteKey_WhenExpiresInSecondsIsZero()
    {
        // RD10：<=0 视为「立即过期」→ 删键（规避 SE.Redis TimeSpan.Zero = 永不过期的陷阱）。
        var backend = new FakeRedisBackend();
        await CreateStore(backend).SetAccessTokenAsync(StoreKey, "token-value", 0);

        backend.Database.Verify(d => d.KeyDeleteAsync(RedisKey, It.IsAny<CommandFlags>()), Times.Once);
        backend.Database.Verify(
            d => d.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()),
            Times.Never);
    }

    [Fact]
    public async Task SetAccessTokenAsync_ShouldDeleteKey_WhenExpiresInSecondsNegative()
    {
        var backend = new FakeRedisBackend();
        await CreateStore(backend).SetAccessTokenAsync(StoreKey, "token-value", -10);

        backend.Database.Verify(d => d.KeyDeleteAsync(RedisKey, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task RefreshTokenContract_ShouldBeNullAndNoOp_AlignedWithInMemory()
    {
        var backend = new FakeRedisBackend();
        var store = CreateStore(backend);

        (await store.GetRefreshTokenAsync(StoreKey)).Should().BeNull();
        await store.SetRefreshTokenAsync(StoreKey, "ignored");

        backend.Database.Verify(
            d => d.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTokenTypesAsync_ShouldStripPrefixAndRestoreStoreKeys()
    {
        var backend = new FakeRedisBackend(new[]
        {
            RedisKey,
            "wechat:token:Wechat.AccessToken:app2:corp1",
            "wechat:corpa:app:corp1", // 非令牌域键，必须被忽略
        });

        var storeKeys = await CreateStore(backend).GetTokenTypesAsync();

        storeKeys.Should().BeEquivalentTo(new[]
        {
            "Wechat.AccessToken:default:default",
            "Wechat.AccessToken:app2:corp1",
        });
    }

    [Fact]
    public async Task GetTokenTypesAsync_ShouldUsePatternSingleExit()
    {
        var backend = new FakeRedisBackend();
        await CreateStore(backend).GetTokenTypesAsync();

        backend.Server.Verify(
            s => s.KeysAsync(0, (RedisValue)"wechat:token:*", It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveRangeAsync_ShouldMapStoreKeysToRedisKeys_AndDeleteInBatches()
    {
        var backend = new FakeRedisBackend();
        var storeKeys = Enumerable.Range(0, 1200).Select(i => $"Wechat.AccessToken:app:{i}").ToList();
        var batches = new List<RedisKey[]>();
        backend.Database
            .Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .Callback((RedisKey[] keys, CommandFlags _) => batches.Add(keys))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) => keys.Length);

        var removed = await CreateStore(backend).RemoveRangeAsync(storeKeys);

        removed.Should().Be(1200);
        batches.Should().HaveCount(3); // 500 + 500 + 200（W1/M10：N 次往返收敛为 ⌈N/500⌉ 次）
        batches[0][0].ToString().Should().Be("wechat:token:Wechat.AccessToken:app:0");
        batches[^1].Should().HaveCount(200);
    }

    [Fact]
    public async Task RemoveRangeAsync_ShouldSkipNullKeys()
    {
        var backend = new FakeRedisBackend();
        backend.SetRaw(RedisKey, "stored");
        var removed = await CreateStore(backend).RemoveRangeAsync(new[] { StoreKey, null! });

        removed.Should().Be(1, "null 键跳过，实际存在的一键被删除");
    }

    [Fact]
    public async Task ClearAsync_ShouldDeleteAllTokenDomainKeys()
    {
        var backend = new FakeRedisBackend(new[]
        {
            RedisKey,
            "wechat:token:Wechat.AccessToken:app2:corp1",
            "wechat:corpa:app:corp1",
        });

        await CreateStore(backend).ClearAsync();

        backend.Store.Keys.Should().ContainSingle().Which.Should().Be("wechat:corpa:app:corp1");
    }

    [Fact]
    public async Task Operations_ShouldThrowClassifiedException_WhenConnectionFails()
    {
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisConnectionException(ConnectionFailureType.SocketFailure, "boom"));
        var store = CreateStore(backend);

        await AssertKind(() => store.GetAccessTokenAsync(StoreKey), WechatRedisFailureKind.Connection);
        await AssertKind(() => store.SetAccessTokenAsync(StoreKey, "v", 60), WechatRedisFailureKind.Connection);
        await AssertKind(() => store.RemoveAsync(StoreKey), WechatRedisFailureKind.Connection);
        await AssertKind(() => store.GetTokenTypesAsync(), WechatRedisFailureKind.Connection);
        await AssertKind(() => store.ClearAsync(), WechatRedisFailureKind.Connection);
        await AssertKind(() => store.RemoveRangeAsync(new[] { StoreKey }), WechatRedisFailureKind.Connection);
    }

    [Fact]
    public async Task Operations_ShouldMapTimeoutKind_WhenTimeoutFails()
    {
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisTimeoutException("timeout", CommandStatus.Unknown));
        var store = CreateStore(backend);

        await AssertKind(() => store.GetAccessTokenAsync(StoreKey), WechatRedisFailureKind.Timeout);
    }

    [Fact]
    public async Task Operations_ShouldMapServerKind_WhenServerFails()
    {
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisServerException("server error"));
        var store = CreateStore(backend);

        await AssertKind(() => store.GetAccessTokenAsync(StoreKey), WechatRedisFailureKind.Server);
    }

    [Fact]
    public async Task Exceptions_ShouldCarryKeyName_AndNotTokenValue()
    {
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisServerException("server error"));
        var store = CreateStore(backend);

        var act = async () => await store.SetAccessTokenAsync(StoreKey, "secret-token-value", 60);
        var ex = (await act.Should().ThrowAsync<WechatRedisException>()).Which;

        ex.Message.Should().Contain(RedisKey);
        ex.Message.Should().NotContain("secret-token-value");
        ex.InnerException.Should().BeOfType<RedisServerException>();
    }

    private static async Task AssertKind(Func<Task> act, WechatRedisFailureKind kind)
        => (await act.Should().ThrowAsync<WechatRedisException>()).Which.FailureKind.Should().Be(kind);
}
