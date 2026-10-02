// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

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
}
