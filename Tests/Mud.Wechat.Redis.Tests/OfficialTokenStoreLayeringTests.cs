// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mud.HttpUtils;
using Mud.HttpUtils.Redis;

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R10（A-5 分层共存）：官方 <c>Mud.HttpUtils.Redis</c> 的 <see cref="RedisTokenStore"/>
/// 经 <c>AddWechatRedis</c> 接入后的键布局、配置映射与「两层键命名空间互不干扰」行为。
/// </summary>
/// <remarks>
/// 裁决口径（上游对账清单 A-5，2026-10-14）：通用 <c>ITokenStore</c> 面迁官方包；
/// 企微专属端口与 <c>IWechatTokenStoreBatchRemove</c> 留下游包；两者键前缀不同
/// （官方 <c>mud:httputils</c> vs 本院 <c>wechat</c>）⇒ 共库无需数据迁移。
/// </remarks>
public class OfficialTokenStoreLayeringTests
{
    private const string TokenType = "Wechat.AccessToken@app-a:ww-corp";

    private static (IServiceProvider Provider, FakeRedisBackend Backend) BuildProvider(
        Action<WechatRedisOptions>? configure = null)
    {
        var backend = new FakeRedisBackend();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatRedis(configure ?? (_ => { }));
        services.AddSingleton(backend.Multiplexer.Object);
        // 生命周期交测试用例持有（using var 会在本方法返回即释放，ObjectDisposedException）。
        var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        return (provider, backend);
    }

    [Fact]
    public async Task OfficialStore_ShouldWriteUnderMudHttpUtilsPrefix_WithRelativeSecondsCodec()
    {
        var (provider, backend) = BuildProvider();
        var store = provider.GetRequiredService<ITokenStore>();

        await store.SetAccessTokenAsync(TokenType, "tok-123", 7200);

        // 键布局 {prefix}:{tokenType}:{appKey}:access（默认前缀 mud:httputils、appKey 空 ⇒ shared）。
        backend.Store.Should().ContainKey("mud:httputils:" + TokenType + ":shared:access");
        // 值经官方 DefaultTokenStoreCodec：{expiresInSeconds}|{token}（相对秒；本院桥接器 U-1 起单写同形态）。
        backend.Store["mud:httputils:" + TokenType + ":shared:access"].ToString().Should().Be("7200|tok-123");
        backend.LastExpiry.Should().Be(TimeSpan.FromSeconds(7200), "访问令牌 TTL 宁缺勿滥：跟随写入方剩余秒数");
        (await store.GetAccessTokenAsync(TokenType)).Should().Be("tok-123");
    }

    [Fact]
    public async Task OfficialStore_ShouldHonorConfiguredPrefixAndAppKey()
    {
        var (provider, backend) = BuildProvider(o =>
        {
            o.TokenStore.KeyPrefix = "mud:dev";
            o.TokenStore.AppKey = "app-a";
        });
        var store = provider.GetRequiredService<ITokenStore>();

        await store.SetAccessTokenAsync(TokenType, "tok-dev", 1800);

        backend.Store.Should().ContainKey("mud:dev:" + TokenType + ":app-a:access");
        backend.Store.Should().NotContainKey("mud:httputils:" + TokenType + ":shared:access");
    }

    [Fact]
    public async Task OfficialStore_ShouldApplyDefaultTtl_WhenNoExpiryInformation()
    {
        var (provider, backend) = BuildProvider(o => o.TokenStore.DefaultTtl = TimeSpan.FromDays(30));
        var store = provider.GetRequiredService<ITokenStore>();

        // expiresInSeconds <= 0 = 无 TTL 信息 ⇒ 回落 DefaultTtl（生产建议显式配置防无界滞留）。
        await store.SetAccessTokenAsync(TokenType, "tok-notl", 0);

        backend.LastExpiry.Should().Be(TimeSpan.FromDays(30));
    }

    [Fact]
    public async Task OfficialStore_ShouldRemoveBothFields_WhenRemoveAsync()
    {
        var (provider, _) = BuildProvider();
        var store = provider.GetRequiredService<ITokenStore>();

        await store.SetAccessTokenAsync(TokenType, "tok-123", 7200);
        await store.SetRefreshTokenAsync(TokenType, "refresh-123");
        await store.RemoveAsync(TokenType);

        (await store.GetAccessTokenAsync(TokenType)).Should().BeNull();
        (await store.GetRefreshTokenAsync(TokenType)).Should().BeNull();
    }

    [Fact]
    public async Task OfficialStore_ShouldWriteV2Envelope_WhenVersionedEnvelopeEnabledAndAcknowledged()
    {
        // U-2 镜像透传：本院两开关原样进官方 RedisTokenStoreOptions ⇒ 写入侧切 VersionedTokenStoreCodec
        // （v2:{秒}|{token}）；读取面本就兼容 v1，双闸均过时形态才变化。
        var (provider, backend) = BuildProvider(o =>
        {
            o.TokenStore.UseVersionedEnvelope = true;
            o.TokenStore.AcknowledgeVersionedEnvelopeReadersUpgraded = true;
        });
        var store = provider.GetRequiredService<ITokenStore>();

        await store.SetAccessTokenAsync(TokenType, "tok-v2", 7200);

        backend.Store["mud:httputils:" + TokenType + ":shared:access"].ToString().Should().Be("v2:7200|tok-v2");
        backend.LastExpiry.Should().Be(TimeSpan.FromSeconds(7200), "v2 信封不改变 TTL 策略（宁缺勿滥跟随写入方）");
        (await store.GetAccessTokenAsync(TokenType)).Should().Be("tok-v2");
    }

    [Fact]
    public void OfficialStore_ShouldFailFast_WhenVersionedEnvelopeEnabledWithoutAcknowledgement()
    {
        // U-2 前置闸：本院 Validate（IValidateOptions 管线）在配置解析期即拒绝——无需等官方存储
        // 构造期的第二道闸（双闸形态；错误经 OptionsFactory 包装为 OptionsValidationException）。
        var (provider, _) = BuildProvider(o => o.TokenStore.UseVersionedEnvelope = true);

        var act = () => provider.GetRequiredService<ITokenStore>();

        act.Should().Throw<OptionsValidationException>().WithMessage("*UseVersionedEnvelope*");
    }

    [Fact]
    public void OfficialStore_ShouldNotImplementWechatBatchRemove()
    {
        // B-3：官方 ITokenStore 维持最小 SPI（A-5 明令批删不上收）⇒ is 探测不命中，
        // 退役清库走逐键回退路径（行为正确、无批量优化）。该前提由本用例钉死。
        var (provider, _) = BuildProvider();

        provider.GetRequiredService<ITokenStore>().Should().NotBeAssignableTo<IWechatTokenStoreBatchRemove>();
    }

    [Fact]
    public async Task TwoLayers_ShouldNotInterfere_WhenClearingNamespaces()
    {
        var (provider, backend) = BuildProvider();
        var official = provider.GetRequiredService<ITokenStore>();
        var wechat = provider.GetRequiredService<IWechatTokenStore>();

        await official.SetAccessTokenAsync(TokenType, "official-tok", 7200);
        await wechat.SetAccessTokenAsync(TokenType, "wechat-tok", 7200);

        backend.Store.Should().ContainKey("mud:httputils:" + TokenType + ":shared:access");
        backend.Store.Should().ContainKey("wechat:token:" + TokenType);

        // 本院清库（SCAN wechat:token:*）不得触碰官方命名空间键，反之亦然（A-5：前缀不同互不干扰）。
        await wechat.ClearAsync();
        backend.Store.Should().NotContainKey("wechat:token:" + TokenType);
        backend.Store.Should().ContainKey("mud:httputils:" + TokenType + ":shared:access");

        await official.ClearAsync();
        backend.Store.Should().NotContainKey("mud:httputils:" + TokenType + ":shared:access");
    }

    [Fact]
    public async Task WechatLayer_ShouldKeepByteIdenticalKeySpace_AndBatchRemove()
    {
        // 分层不下沉既有契约：本院令牌面键形 {prefix}:token:{storeKey} 与 InMemory 逐字节一致（RD2），
        // 批删扩展仍由 RedisWechatTokenStore 实现（W1/M10）——迁移官方层不改变这两条。
        var (provider, backend) = BuildProvider();
        var wechat = provider.GetRequiredService<IWechatTokenStore>();

        wechat.Should().BeAssignableTo<IWechatTokenStoreBatchRemove>();
        wechat.Should().BeAssignableTo<ITokenStoreAppPurge>(
            "U-4：本院 Redis 实现同批承担官方按应用清库接口（管理器三层探测的 tier-1）");
        await wechat.SetAccessTokenAsync(TokenType, "wechat-tok", 7200);

        backend.Store.Should().ContainKey("wechat:token:" + TokenType);
        backend.Store["wechat:token:" + TokenType].ToString().Should().Be("wechat-tok",
            "本院存储原样存取（值编码 {expiresInSeconds}|{token} 由组件桥接器在更外层完成，U-1 迁移期双读）");
    }
}
