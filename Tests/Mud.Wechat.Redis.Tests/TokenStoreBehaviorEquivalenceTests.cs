// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R6：行为等价——InMemory 与 Redis（Fake 后端）双实现跑同一契约行为用例集（§三「行为对齐」行）。
/// </summary>
public class TokenStoreBehaviorEquivalenceTests
{
    public static IEnumerable<object[]> Stores()
    {
        yield return new object[] { "InMemory", new Func<IWechatTokenStoreBatchRemove>(() => new InMemoryWechatTokenStore()) };
        yield return new object[] { "RedisFake", new Func<IWechatTokenStoreBatchRemove>(() =>
            new RedisWechatTokenStore(new FakeRedisBackend().Multiplexer.Object, new WechatRedisOptions())) };
    }

    /// <remarks>
    /// U-4 接口现只由 Redis 实现承担：叶层 <see cref="InMemoryWechatTokenStore"/> 已撤下该接口，
    /// 进程内清理走 <c>WechatAppManager.PurgeAppTokensAsync</c>（<c>GetTokenTypesAsync</c> + 批删），
    /// 故本表**不得**再列 InMemory —— 列了就是一次 <c>InvalidCastException</c> 假红。
    /// </remarks>
    public static IEnumerable<object[]> PurgeCapableStores()
    {
        yield return new object[] { "RedisFake", new Func<IWechatTokenStore>(() =>
            new RedisWechatTokenStore(new FakeRedisBackend().Multiplexer.Object, new WechatRedisOptions())) };
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task GetAccessToken_ShouldReturnNull_WhenNeverSet(string _, Func<IWechatTokenStoreBatchRemove> factory)
        => (await factory().GetAccessTokenAsync("Wechat.AccessToken:default:default")).Should().BeNull();

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task SetThenGet_ShouldRoundTrip(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "token-1", 7200);

        (await store.GetAccessTokenAsync("Wechat.AccessToken:default:default")).Should().Be("token-1");
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task SetAgain_ShouldOverwrite(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "token-1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "token-2", 7200);

        (await store.GetAccessTokenAsync("Wechat.AccessToken:default:default")).Should().Be("token-2");
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task Get_ShouldReturnNull_AndClear_WhenExpiresInSecondsNotPositive(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "token-1", 0);

        (await store.GetAccessTokenAsync("Wechat.AccessToken:default:default")).Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task Remove_ShouldMakeKeyUnavailable(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "token-1", 7200);
        await store.RemoveAsync("Wechat.AccessToken:default:default");

        (await store.GetAccessTokenAsync("Wechat.AccessToken:default:default")).Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task RefreshTokenContract_ShouldBeNoOp(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "token-1", 7200);

        (await store.GetRefreshTokenAsync("Wechat.AccessToken:default:default")).Should().BeNull();
        await store.SetRefreshTokenAsync("Wechat.AccessToken:default:default", "refresh");

        (await store.GetRefreshTokenAsync("Wechat.AccessToken:default:default")).Should().BeNull();
        (await store.GetAccessTokenAsync("Wechat.AccessToken:default:default")).Should().Be("token-1", "refresh no-op 不得影响既有令牌");
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task GetTokenTypes_ShouldReturnWrittenStoreKeys(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:app2:corp1", "t2", 7200);

        var keys = (await store.GetTokenTypesAsync()).ToList();

        keys.Should().BeEquivalentTo(new[]
        {
            "Wechat.AccessToken:default:default",
            "Wechat.AccessToken:app2:corp1",
        });
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task RemoveRange_ShouldRemoveOnlyTargetKeys_AndReturnCount(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:app2:corp1", "t2", 7200);
        await store.SetAccessTokenAsync("Wechat.SuiteAccessToken:app2:default", "t3", 7200);

        var removed = await store.RemoveRangeAsync(new[]
        {
            "Wechat.AccessToken:default:default",
            "Wechat.AccessToken:app2:corp1",
        });

        removed.Should().Be(2);
        (await store.GetAccessTokenAsync("Wechat.SuiteAccessToken:app2:default")).Should().Be("t3");
        (await store.GetAccessTokenAsync("Wechat.AccessToken:default:default")).Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task RemoveRange_ShouldReturnZero_WhenKeysEmpty(string _, Func<IWechatTokenStoreBatchRemove> factory)
        => (await factory().RemoveRangeAsync(Array.Empty<string>())).Should().Be(0);

    [Theory]
    [MemberData(nameof(Stores))]
    public async Task Clear_ShouldRemoveAllKeys(string _, Func<IWechatTokenStoreBatchRemove> factory)
    {
        var store = factory();
        await store.SetAccessTokenAsync("Wechat.AccessToken:default:default", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:app2:corp1", "t2", 7200);

        await store.ClearAsync();

        (await store.GetAccessTokenAsync("Wechat.AccessToken:app2:corp1")).Should().BeNull();
    }

    // ---------------------------------------------------------------- U-4：按应用清库（官方 ITokenStoreAppPurge 双实现契约）

    [Theory]
    [MemberData(nameof(PurgeCapableStores))]
    public async Task EnumerateKeys_ShouldFilterByAppKeySegment(string _, Func<IWechatTokenStore> factory)
    {
        var store = factory();
        store.Should().BeAssignableTo<ITokenStoreAppPurge>("U-4：Redis 存储实现须承担官方按应用清库接口");
        var purge = (ITokenStoreAppPurge)store;
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:corp:123", "t2", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:ab:default", "t3", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:b:default", "t4", 7200);

        var keys = await purge.EnumerateKeysAsync("a");

        keys.Should().BeEquivalentTo(new[]
        {
            "Wechat.AccessToken:a:default",
            "Wechat.AccessToken:a:corp:123",
        }, "前缀兄弟应用（ab）与其它应用（b）不得混入；scope 含 ':' 的键须按第二段边界完整返回");
    }

    [Theory]
    [MemberData(nameof(PurgeCapableStores))]
    public async Task PurgeApp_ShouldRemoveOnlyTargetAppKeys_AndReturnCount(string _, Func<IWechatTokenStore> factory)
    {
        var store = factory();
        var purge = (ITokenStoreAppPurge)store;
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:corp:123", "t2", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:b:default", "t3", 7200);

        var removed = await purge.PurgeAppAsync("a");

        removed.Should().Be(2, "返回实际删除键数");
        (await store.GetAccessTokenAsync("Wechat.AccessToken:b:default")).Should().Be("t3",
            "其它应用的键不得被误删（按第二段精确比对）");
    }

    [Theory]
    [MemberData(nameof(PurgeCapableStores))]
    public async Task PurgeApp_ShouldReturnZeroAndKeepStore_WhenAppKeyBlank(string _, Func<IWechatTokenStore> factory)
    {
        var store = factory();
        var purge = (ITokenStoreAppPurge)store;
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t1", 7200);

        var removed = await purge.PurgeAppAsync("   ");

        removed.Should().Be(0, "空白 appKey 返回 0（本院键空间无共享段约定）");
        (await store.GetAccessTokenAsync("Wechat.AccessToken:a:default")).Should().Be("t1",
            "空白 appKey 不得清空整库");
    }
}
