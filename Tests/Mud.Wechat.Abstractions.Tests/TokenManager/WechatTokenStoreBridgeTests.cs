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
/// U-2 追加（2026-10-15）：v2 信封读取面兼容——有效态重建绝对过期戳；
/// 显式过期态（<c>v2:expired|</c>）与无 TTL 信息态（<c>v2:0|tok</c>）均判 miss。
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

    [Fact]
    public async Task ReadBack_ShouldDecodeV2EnvelopeValue()
    {
        // U-2：官方写入侧启用 v2 信封后（v2:{秒}|{token}），本侧读取面经迁移 codec 直通默认 codec
        // 的 v2 前缀识别——升级库即得、无需改配置（读取面无条件兼容；本桥写入侧维持相对秒单写）。
        var (cache, store) = Build();
        await store.SetAccessTokenAsync(StoreKey, "v2:3600|tok-v2", 3600);

        var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var token = await cache.GetAsync(CacheKey);
        var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        token.Should().NotBeNull("v2 自描述值不参与旧格式量级判别，须由新格式 codec 的前缀识别直读");
        token!.AccessToken.Should().Be("tok-v2");
        token.Expire.Should().BeInRange(before + 3_600_000, after + 3_600_000,
            "v2 信封有效态按「读取时刻 + 剩余秒」重建绝对毫秒过期戳（与相对秒形态同口径）");
    }

    [Fact]
    public async Task ReadBack_ShouldReturnNull_WhenV2ExpiredMarker()
    {
        // U-2 显式过期态（v2:expired|）解码投影为 (null, 0) ⇒ miss（与缺失谓词同义，读侧不消费）。
        var (cache, store) = Build();
        await store.SetAccessTokenAsync(StoreKey, "v2:expired|", 3600);

        (await cache.GetAsync(CacheKey)).Should().BeNull("显式过期态不携带可消费令牌，读取面按无令牌处置");
    }

    [Fact]
    public async Task ReadBack_ShouldReturnNull_WhenV2NoTtlInformation()
    {
        // v2:0|tok 解出 (tok, 0)——「相对秒 ≤ 0 = 无 TTL 信息」的 0 值语义不动（v2 只把
        // 「已过期」拆为独立标记）；本桥读路径对无剩余秒维持 miss 口径（管线重新取令牌）。
        var (cache, store) = Build();
        await store.SetAccessTokenAsync(StoreKey, "v2:0|tok-notl", 3600);

        (await cache.GetAsync(CacheKey)).Should().BeNull("无 TTL 信息（剩余秒 ≤ 0）在本桥读路径维持 miss 口径");
    }
}
