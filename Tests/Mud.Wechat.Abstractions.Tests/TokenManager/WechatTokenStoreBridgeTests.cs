// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;

namespace Mud.Wechat.Abstractions.Tests.TokenManager;

/// <summary>
/// U-1 迁移期值编解码（2026-10-15）在 <c>WechatTokenStoreBridge</c> 上的语义锁定：
/// 写侧单写官方相对秒形态 <c>{expiresInSeconds}|{token}</c>；读侧双读
/// （新相对秒直读 / 旧绝对毫秒戳 ≥ 10^10 重投影 / 不可解析或已过期 ⇒ miss）。
/// 自建私有 <c>WechatTokenBridgeCodec</c> 已删除（D1=A）。
/// </summary>
public class WechatTokenStoreBridgeTests
{
    private const string TokenTypeKey = "Wechat.AccessToken:app-a";
    private const string StoreKey = TokenTypeKey + ":default";
    private const string CacheKey = "default";

    private static (IAsyncTokenCache<CredentialToken> Cache, InMemoryWechatTokenStore Store) Build()
    {
        var store = new InMemoryWechatTokenStore();
        var cache = (IAsyncTokenCache<CredentialToken>)WechatTokenStoreBridge.CreateCache(
            store, TokenTypeKey, NullLogger.Instance);
        return (cache, store);
    }

    [Fact]
    public async Task WriteThrough_ShouldEncodeAsRelativeSecondsFormat()
    {
        var (cache, store) = Build();
        var expiresAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 7_200_999;

        await cache.SetAsync(CacheKey, new CredentialToken { AccessToken = "tok-new", Expire = expiresAt });

        var stored = await store.GetAccessTokenAsync(StoreKey);
        stored.Should().NotBeNull();
        var separator = stored!.IndexOf('|');
        separator.Should().BeGreaterThan(0, "写侧必须为 {expiresInSeconds}|{token} 形态（U-1 单写相对秒，C-2 裁决）");
        long.Parse(stored.Substring(0, separator), CultureInfo.InvariantCulture)
            .Should().BeInRange(7198, 7200, "前导段是剩余秒数而非 13 位绝对毫秒戳（旧形态的量级应为 1.7×10^12）");
        stored.Substring(separator + 1).Should().Be("tok-new");
    }

    [Fact]
    public async Task ReadBack_ShouldDecodeRelativeSecondsFormat_AndRebuildAbsoluteExpire()
    {
        var (cache, store) = Build();
        await store.SetAccessTokenAsync(StoreKey, "3600|tok-new", 3600);

        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var token = await cache.GetAsync(CacheKey);
        var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        token.Should().NotBeNull();
        token!.AccessToken.Should().Be("tok-new");
        token.Expire.Should().BeInRange(before + 3_600_000, after + 3_600_000,
            "相对秒形态按「读取时刻 + 剩余秒」重建绝对毫秒过期戳");
    }

    [Fact]
    public async Task ReadBack_ShouldDecodeLegacyAbsoluteMillisecondsFormat()
    {
        var (cache, store) = Build();
        var legacyExpiresAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 7_200_000;
        await store.SetAccessTokenAsync(
            StoreKey, legacyExpiresAt.ToString(CultureInfo.InvariantCulture) + "|tok-legacy", 7200);

        var token = await cache.GetAsync(CacheKey);

        token.Should().NotBeNull();
        token!.AccessToken.Should().Be("tok-legacy");
        token.Expire.Should().BeCloseTo(legacyExpiresAt, 2000,
            "旧 {expireMs}|{token} 存量值经双读重投影为剩余秒后再重建，容差覆盖 floor 取整与调用耗时");
    }

    [Fact]
    public async Task ReadBack_ShouldReturnNull_WhenStoredValueHasNoSeparator()
    {
        var (cache, store) = Build();
        await store.SetAccessTokenAsync(StoreKey, "tok-bare", 3600);

        (await cache.GetAsync(CacheKey)).Should().BeNull(
            "无分隔符裸值两侧 codec 均不可解析 ⇒ miss（不抛异常、不返回残缺条目）");
    }

    [Fact]
    public async Task ReadBack_ShouldReturnNull_WhenLegacyValueAlreadyExpired()
    {
        var (cache, store) = Build();
        var expiredAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 5_000;
        await store.SetAccessTokenAsync(
            StoreKey, expiredAt.ToString(CultureInfo.InvariantCulture) + "|tok-stale", 3600);

        (await cache.GetAsync(CacheKey)).Should().BeNull(
            "旧值重投影 ≤ 0 ⇒ miss（管线重新取令牌），不返回带过去过期戳的条目");
    }
}
