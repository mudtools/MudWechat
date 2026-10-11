// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.HttpUtils;
using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.Extensions;

namespace Mud.Wechat.OpenPlatform.Tests.Authentication;

/// <summary>
/// 开放平台应用上下文基座行为锁定（B1）：DI 桥接不变量、授权方作用域语义、令牌管理器适配与注册表分流。
/// </summary>
/// <remarks>
/// <b>全部用例都不触网</b>：令牌提供者经接口打桩；DI 用例用真实装配（对齐企微线
/// 「DI 可解析 + 跨 scope 同一实例」用例纪律，<c>ValidateScopes = true</c>）。
/// </remarks>
public class OpenPlatformAuthenticationTests
{
    private const string ComponentAppId = "wx-component";

    private static Action<OpenPlatformAppConfig> ValidConfig => static cfg =>
    {
        cfg.ComponentAppId = ComponentAppId;
        cfg.ComponentAppSecret = "component-secret";
        cfg.Token = new string('a', 43);
        cfg.EncodingAesKey = new string('b', 43);
    };

    // ------------------------------------------------------------------ DI 桥接

    /// <summary>
    /// DI-1：AddOpenPlatform + 全模块注册后，五个声明式客户端与上下文基座全部可解析。
    /// </summary>
    [Fact]
    public void AddOpenPlatform_ShouldResolveAllClients_WhenAllModulesRegistered()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOpenPlatform(ValidConfig);
        services.AddOpenPlatformApis(static builder => builder.AddAllApis());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatOpenPlatformComponentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatOpenPlatformComponentTicketFreeService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatOpenPlatformOpenAccountService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatOpenPlatformAccountService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatOpenPlatformSnsService>().Should().NotBeNull();
        provider.GetRequiredService<IOpenPlatformAppManager>().Should().NotBeNull();
        provider.GetRequiredService<IComponentAppContextSwitcher>().Should().NotBeNull();
        provider.GetRequiredService<Mud.HttpUtils.ITokenProvider>().Should().NotBeNull();
    }

    /// <summary>
    /// DI-2：桥接不变量——<see cref="IComponentAppContextSwitcher"/> / 组件
    /// <see cref="IAppContextHolder"/> / 组件 <see cref="IAppContextSwitcher"/> 是<b>同一实例</b>（跨 scope 同一 Singleton）。
    /// </summary>
    [Fact]
    public void ContextSwitcher_ShouldShareSameInstanceAsHolder_WhenBridged()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOpenPlatform(ValidConfig);

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        var switcher = provider.GetRequiredService<IComponentAppContextSwitcher>();
        var holder = scopeA.ServiceProvider.GetRequiredService<IAppContextHolder>();
        var legacy = scopeB.ServiceProvider.GetRequiredService<IAppContextSwitcher>();

        switcher.Should().BeSameAs(holder,
            "切换器与组件持有器必须同实例：否则 [Token] 客户端读到的上下文与 UseAuthorizerScope 写入的上下文分裂（企微 P0-1 同款缺陷）");
        switcher.Should().BeSameAs(legacy);
    }

    /// <summary>
    /// DI-3：未先装配凭证链即注册模块 ⇒ <c>Build()</c> fail-fast（消息指向修复步骤）。
    /// </summary>
    [Fact]
    public void AddOpenPlatformApis_ShouldFailFast_WhenCredentialChainMissing()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var act = () => services.AddOpenPlatformApis(static builder => builder.AddAllApis());

        act.Should().Throw<InvalidOperationException>()
            .Which.Message.Should().Contain("AddOpenPlatform");
    }

    // ------------------------------------------------------------------ 作用域语义

    /// <summary>
    /// SCOPE-1：<see cref="IComponentAppContextSwitcher.UseAuthorizerScope"/> 进入授权方作用域、
    /// 释放还原（快照语义），且授权方上下文携带 <c>AuthorizerAppId</c>。
    /// </summary>
    [Fact]
    public void UseAuthorizerScope_ShouldEnterAndRestore_WhenDisposed()
    {
        var (manager, switcher) = CreateManagerAndSwitcher();

        IOpenPlatformAppContext? inside = null;
        var before = switcher.Current;

        using (switcher.UseAuthorizerScope("wx-authorizer-1"))
        {
            inside = switcher.Current as IOpenPlatformAppContext;
        }

        inside.Should().NotBeNull();
        inside!.AuthorizerAppId.Should().Be("wx-authorizer-1");
        switcher.Current.Should().BeSameAs(before, "释放后必须还原进入前上下文（残留即串号）");
    }

    /// <summary>
    /// SCOPE-2：授权方参数非法即抛（空白 / 非法字符），不产生半进入状态。
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("wx/illegal")]
    public void UseAuthorizerScope_ShouldThrow_WhenAppIdInvalid(string appId)
    {
        var (_, switcher) = CreateManagerAndSwitcher();

        var act = () => switcher.UseAuthorizerScope(appId);

        act.Should().Throw<ArgumentException>();
    }

    /// <summary>
    /// SCOPE-3：<see cref="IComponentAppContextSwitcher.UseAuthorizerScope"/> 是<b>显式作用域</b>——
    /// 未进入时当前上下文为 <c>null</c>（声明式客户端回退默认平台应用，授权方令牌解析 fail-fast）。
    /// </summary>
    [Fact]
    public void Current_ShouldBeNull_WhenNoScopeEntered()
    {
        var (_, switcher) = CreateManagerAndSwitcher();

        switcher.Current.Should().BeNull(
            "单例切换器不预置上下文（AsyncLocal 语义）：显式作用域是授权方令牌的唯一合法入口");
    }

    // ------------------------------------------------------------------ 管理器语义

    /// <summary>
    /// MGR-1：平台上下文即默认应用；<c>GetApp(平台 appid)</c> 短路返回同一实例。
    /// </summary>
    [Fact]
    public void GetApp_ShouldShortCircuitPlatformKey_WhenKeyEqualsComponentAppId()
    {
        var (manager, _) = CreateManagerAndSwitcher();

        manager.GetDefaultApp().Should().BeSameAs(manager.PlatformContext);
        manager.GetApp(ComponentAppId).Should().BeSameAs(manager.PlatformContext);
        manager.DefaultAppKey.Should().Be(ComponentAppId);
    }

    /// <summary>
    /// MGR-2：授权方上下文按 appid 物化并缓存（同键同实例）。
    /// </summary>
    [Fact]
    public void GetApp_ShouldMaterializeAuthorizerContext_AndCacheIt()
    {
        var (manager, _) = CreateManagerAndSwitcher();

        var first = manager.GetApp("wx-authorizer-1");
        var second = manager.GetApp("wx-authorizer-1");

        first.AuthorizerAppId.Should().Be("wx-authorizer-1");
        second.Should().BeSameAs(first);
        manager.TryGetApp("wx-authorizer-1", out var cached).Should().BeTrue();
        cached.Should().BeSameAs(first);
    }

    /// <summary>
    /// MGR-3：写入口一律 NotSupportedException（防静默写进读不到的表，对齐企微 <c>WechatAppManager</c>）。
    /// </summary>
    [Fact]
    public void WriteMembers_ShouldThrowNotSupported_WhenInvoked()
    {
        var (manager, _) = CreateManagerAndSwitcher();
        var context = manager.GetDefaultApp();

        var act = () => manager.RegisterApp("wx-x", context);
        act.Should().Throw<NotSupportedException>();
        act = () => manager.UpdateApp("wx-x", context);
        act.Should().Throw<NotSupportedException>();
        act = () => manager.RemoveApp("wx-x");
        act.Should().Throw<NotSupportedException>();
        act = () => manager.SetDefaultApp("wx-x");
        act.Should().Throw<NotSupportedException>();
    }

    // ------------------------------------------------------------------ 上下文令牌分流

    /// <summary>
    /// CTX-1：平台上下文可解析平台令牌、<b>不可</b>解析授权方令牌（错配 fail-fast）。
    /// </summary>
    [Fact]
    public void PlatformContext_ShouldResolveComponentToken_ButFailFastOnAuthorizerToken()
    {
        var (manager, _) = CreateManagerAndSwitcher();
        var platform = manager.GetDefaultApp();

        platform.GetTokenManager(OpenPlatformTokenTypes.ComponentAccessToken)
            .Should().NotBeNull("平台自身令牌是平台上下文的合法取用面");

        var act = () => platform.GetTokenManager(OpenPlatformTokenTypes.AuthorizerAccessToken);
        act.Should().Throw<InvalidOperationException>()
            .Which.Message.Should().Contain("UseAuthorizerScope");
    }

    /// <summary>
    /// CTX-2：授权方上下文可解析两种令牌（平台令牌无作用域；授权方令牌绑定本上下文 appid）。
    /// </summary>
    [Fact]
    public void AuthorizerContext_ShouldResolveBothTokens_WhenInScope()
    {
        var (manager, _) = CreateManagerAndSwitcher();
        var authorizer = manager.GetApp("wx-authorizer-1");

        authorizer.GetTokenManager(OpenPlatformTokenTypes.ComponentAccessToken).Should().NotBeNull();
        authorizer.GetTokenManager(OpenPlatformTokenTypes.AuthorizerAccessToken).Should().NotBeNull();

        var act = () => authorizer.GetTokenManager("Wechat.Unknown");
        act.Should().Throw<InvalidOperationException>();
    }

    // ------------------------------------------------------------------ 令牌管理器适配

    /// <summary>
    /// TM-1：平台令牌管理器把取令牌请求适配到 Provider，并拒绝 scope（无 scope 语义）。
    /// </summary>
    [Fact]
    public async Task ComponentTokenManager_ShouldDelegate_AndRejectScopes()
    {
        var provider = new Mock<IComponentTokenProvider>(MockBehavior.Strict);
        provider
            .Setup(static p => p.GetComponentAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("component-token");

        var manager = new Mud.Wechat.OpenPlatform.Authentication.ComponentTokenManager(provider.Object);

        (await manager.GetTokenAsync()).Should().Be("component-token");
        (await manager.GetOrRefreshTokenAsync()).Should().Be("component-token");

        var act = async () => await manager.GetTokenAsync(new[] { "scope" });
        (await act.Should().ThrowAsync<InvalidOperationException>()).Which.Message.Should().Contain("scope");

        provider.Verify(static p => p.GetComponentAccessTokenAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        provider.VerifyNoOtherCalls();
    }

    /// <summary>
    /// TM-2：平台令牌失效路径调 Provider.Invalidate 并返回空结果（不抛）。
    /// </summary>
    [Fact]
    public async Task ComponentTokenManager_ShouldInvalidateProviderCache_WhenInvalidateTokenAsync()
    {
        var provider = new Mock<IComponentTokenProvider>(MockBehavior.Strict);
        provider.Setup(static p => p.Invalidate()).Verifiable();

        var manager = new Mud.Wechat.OpenPlatform.Authentication.ComponentTokenManager(provider.Object);

        var result = await manager.InvalidateTokenAsync();

        result.IsEmpty.Should().BeTrue();
        provider.Verify(static p => p.Invalidate(), Times.Once);
        provider.VerifyNoOtherCalls();
    }

    /// <summary>
    /// TM-3：授权方令牌管理器绑定创建时 appid（实例即作用域），失效路径携带同一 appid。
    /// </summary>
    [Fact]
    public async Task AuthorizerTokenManager_ShouldDelegateWithBoundAppId_WhenGetOrInvalidate()
    {
        var provider = new Mock<IAuthorizerTokenProvider>(MockBehavior.Strict);
        provider
            .Setup(static p => p.GetAuthorizerAccessTokenAsync("wx-a", It.IsAny<CancellationToken>()))
            .ReturnsAsync("authorizer-token");
        provider.Setup(static p => p.Invalidate("wx-a")).Verifiable();

        var manager = new Mud.Wechat.OpenPlatform.Authentication.AuthorizerTokenManager(provider.Object, "wx-a");

        (await manager.GetTokenAsync()).Should().Be("authorizer-token");

        var result = await manager.InvalidateTokenAsync();
        result.IsEmpty.Should().BeTrue();

        var act = async () => await manager.GetTokenAsync(new[] { "scope" });
        (await act.Should().ThrowAsync<InvalidOperationException>()).Which.Message.Should().Contain("UseAuthorizerScope");

        provider.Verify(static p => p.GetAuthorizerAccessTokenAsync("wx-a", It.IsAny<CancellationToken>()), Times.Once);
        provider.Verify(static p => p.Invalidate("wx-a"), Times.Once);
        provider.VerifyNoOtherCalls();
    }

    /// <summary>
    /// TM-4：授权方令牌管理器拒绝空白 appid（构造期 fail-fast）。
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AuthorizerTokenManager_ShouldFailFast_WhenAppIdBlank(string appId)
    {
        var provider = new Mock<IAuthorizerTokenProvider>();

        var act = () => new Mud.Wechat.OpenPlatform.Authentication.AuthorizerTokenManager(provider.Object, appId);

        act.Should().Throw<ArgumentException>();
    }

    // ------------------------------------------------------------------ 注册表分流

    /// <summary>
    /// REG-1：平台键任何作用域都可达；授权方键无作用域 fail-fast；未知键返回 null（组件回退语义）。
    /// </summary>
    [Fact]
    public void Registry_ShouldRouteByScope_WhenKeysResolved()
    {
        var (manager, switcher) = CreateManagerAndSwitcher();
        var holder = (IAppContextHolder)switcher;
        var registry = new Mud.Wechat.OpenPlatform.Authentication.OpenPlatformTokenManagerRegistry(holder, manager);

        registry.Resolve(OpenPlatformTokenTypes.ComponentAccessToken).Should().NotBeNull("平台键无作用域语义");

        var act = () => registry.Resolve(OpenPlatformTokenTypes.AuthorizerAccessToken);
        act.Should().Throw<InvalidOperationException>(
            "授权方键必须显式作用域——静默回退平台上下文即取错令牌");

        using (switcher.UseAuthorizerScope("wx-authorizer-1"))
        {
            registry.Resolve(OpenPlatformTokenTypes.AuthorizerAccessToken).Should().NotBeNull();
        }

        registry.Resolve("Wechat.Unknown").Should().BeNull("未知键返回 null 由调用方回退（组件契约）");
    }

    // ------------------------------------------------------------------ 测试夹具

    /// <summary>构造真实管理器 + 真实切换器（令牌提供者打桩；不触网）。</summary>
    private static (IOpenPlatformAppManager Manager, IComponentAppContextSwitcher Switcher) CreateManagerAndSwitcher()
    {
        var httpClient = new Mock<IWechatOpenPlatformHttpClient>();
        var componentTokenProvider = new Mock<IComponentTokenProvider>();
        componentTokenProvider
            .Setup(static p => p.GetComponentAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("component-token");
        var authorizerTokenProvider = new Mock<IAuthorizerTokenProvider>();
        authorizerTokenProvider
            .Setup(static p => p.GetAuthorizerAccessTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("authorizer-token");

        var config = new OpenPlatformAppConfig
        {
            ComponentAppId = ComponentAppId,
            ComponentAppSecret = "component-secret",
            Token = new string('a', 43),
            EncodingAesKey = new string('b', 43),
        };
        config.EnsureValid();

        var manager = new Mud.Wechat.OpenPlatform.Authentication.OpenPlatformAppManager(
            new ServiceCollection().BuildServiceProvider(),
            config,
            httpClient.Object,
            componentTokenProvider.Object,
            authorizerTokenProvider.Object);

        var switcher = new Mud.Wechat.OpenPlatform.Authentication.ComponentAppContextSwitcher(manager);
        return (manager, switcher);
    }
}
