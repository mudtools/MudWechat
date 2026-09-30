// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Abstractions.Tests.MultiApp;

/// <summary>
/// WechatAppManager 多应用治理测试（经 AddWechatApp 真实装配路径）：
/// 懒加载、默认应用、AddApp、令牌路由与隔离（详细设计 §8.2 / §18.3）。
/// </summary>
public class WechatAppManagerTests : IDisposable
{
    private readonly ServiceCollection _services = new();
    private ServiceProvider? _provider;

    public WechatAppManagerTests()
    {
        _services.AddLogging();
    }

    private WechatAppManager CreateManager(params WechatAppConfig[] configs)
    {
        _services.AddWechatApp(configs.ToList());
        _provider = _services.BuildServiceProvider();
        return _provider.GetRequiredService<WechatAppManager>();
    }

    private static WechatAppConfig InternalConfig(string appKey, bool isDefault = false) => new()
    {
        AppKey = appKey,
        IsDefault = isDefault,
        AppType = WechatAppType.Internal,
        CorpId = "ww-corp",
        AgentSecret = "agent-secret",
    };

    private static WechatAppConfig SuiteConfig(string appKey, bool isDefault = false) => new()
    {
        AppKey = appKey,
        IsDefault = isDefault,
        AppType = WechatAppType.ThirdParty,
        CorpId = "ww-provider",
        ProviderSecret = "provider-secret",
        SuiteId = "ww-suite",
        SuiteSecret = "suite-secret",
    };

    [Fact]
    public void AddWechatApp_ShouldFail_WhenNoAppsConfigured()
    {
        var services = new ServiceCollection();
        var act = () => services.AddWechatApp(new List<WechatAppConfig>());
        act.Should().Throw<InvalidOperationException>().WithMessage("*至少需要配置一个*");
    }

    [Fact]
    public void AddWechatApp_ShouldFail_WhenDuplicateAppKey()
    {
        var services = new ServiceCollection();
        var act = () => services.AddWechatApp(new List<WechatAppConfig> { InternalConfig("dup"), InternalConfig("dup") });
        act.Should().Throw<InvalidOperationException>().WithMessage("*重复*");
    }

    [Fact]
    public void Constructor_ShouldPickFirstAsDefault_WhenNoExplicitDefault()
    {
        using var manager = CreateManager(InternalConfig("a"), InternalConfig("b"));

        manager.DefaultAppKey.Should().Be("a", "无显式默认时取首个配置");
        manager.ConfiguredAppKeys.Should().BeEquivalentTo("a", "b");
    }

    [Fact]
    public void GetAllApps_ShouldNotForceInstantiation()
    {
        using var manager = CreateManager(InternalConfig("a"), InternalConfig("b"));

        manager.GetAllApps().Should().BeEmpty("懒加载上下文未被访问时不应实例化（D6）");
        manager.HasApp("a").Should().BeTrue();
        manager.GetApp("a");
        manager.GetAllApps().Should().ContainSingle(c => c.AppKey == "a");
    }

    [Fact]
    public void GetApp_ShouldThrow_WhenUnknownAppKey()
    {
        using var manager = CreateManager(InternalConfig("a"));

        var act = () => manager.GetApp("missing");
        act.Should().Throw<InvalidOperationException>().WithMessage("*missing*");
    }

    [Fact]
    public void AddApp_ShouldRegisterAndSwitchDefault_WhenDeclared()
    {
        using var manager = CreateManager(InternalConfig("default"));

        var added = manager.AddApp(InternalConfig("runtime", isDefault: true));
        added.AppKey.Should().Be("runtime");
        manager.DefaultAppKey.Should().Be("runtime");

        var act = () => manager.AddApp(InternalConfig("runtime"));
        act.Should().Throw<InvalidOperationException>().WithMessage("*已存在*");
    }

    [Fact]
    public void InternalAppContext_ShouldRouteAccessTokenToInternalManager()
    {
        using var manager = CreateManager(InternalConfig("default"));

        var context = manager.GetApp("default");
        context.AppType.Should().Be(WechatAppType.Internal);
        context.InternalAppTokenManager.Should().NotBeNull();
        context.CorpTokenManager.Should().BeNull();
        context.ProviderTokenManager.Should().BeNull();
        context.SuiteTokenManager.Should().BeNull();
        context.BaseUrl.Should().Be("https://qyapi.weixin.qq.com");
        context.HttpClient.Should().NotBeNull();

        var routed = context.GetTokenManager(WechatTokenTypes.AccessToken);
        routed.Should().BeSameAs(context.InternalAppTokenManager);

        // 支持后台刷新（默认应用管理器可被 WechatTokenRegistrationService 登记）。
        context.InternalAppTokenManager!.SupportsBackgroundRefresh.Should().BeTrue();
    }

    [Fact]
    public void SuiteAppContext_ShouldRouteTokenTypesIndependently()
    {
        using var manager = CreateManager(SuiteConfig("suite-app"));

        var context = manager.GetApp("suite-app");
        context.AppType.Should().Be(WechatAppType.ThirdParty);
        context.CorpTokenManager.Should().NotBeNull();
        context.ProviderTokenManager.Should().NotBeNull();
        context.SuiteTokenManager.Should().NotBeNull();

        // AccessToken 路由到企业级管理器（第三方/服务商模式）；suite / provider 各自独立。
        context.GetTokenManager(WechatTokenTypes.AccessToken).Should().BeSameAs(context.CorpTokenManager);
        context.GetTokenManager(WechatTokenTypes.SuiteAccessToken).Should().BeSameAs(context.SuiteTokenManager);
        context.GetTokenManager(WechatTokenTypes.ProviderAccessToken).Should().BeSameAs(context.ProviderTokenManager);

        var act = () => context.GetTokenManager("Unknown.TokenType");
        act.Should().Throw<InvalidOperationException>().WithMessage("*Unknown.TokenType*");
    }

    [Fact]
    public async Task InvalidateTokenAsync_ShouldDelegateToRoutedManager()
    {
        using var manager = CreateManager(InternalConfig("default"));

        var context = manager.GetApp("default");
        context.InternalAppTokenManager.Should().NotBeNull();

        var act = async () => await manager.InvalidateTokenAsync("default", WechatTokenTypes.AccessToken);
        await act.Should().NotThrowAsync("级联失效应透传到路由的管理器（内存 + Store 双清；空缓存失效为幂等操作）");

        var actUnknown = async () => await manager.InvalidateTokenAsync("missing", WechatTokenTypes.AccessToken);
        await actUnknown.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void RemoveApp_ShouldRetireAndRestoreDefault()
    {
        using var manager = CreateManager(InternalConfig("a", isDefault: true), InternalConfig("b"));

        manager.GetApp("a");
        manager.RemoveApp("a").Should().BeTrue();
        manager.HasApp("a").Should().BeFalse();
        manager.DefaultAppKey.Should().Be("b", "默认应用被移除后应确定性提升");
        manager.RemoveApp("missing").Should().BeFalse();
    }

    [Fact]
    public void MultiAppContexts_ShouldNotCrossContaminateConfigs()
    {
        using var manager = CreateManager(InternalConfig("default"), SuiteConfig("suite-app"));

        var internalContext = manager.GetApp("default");
        var suiteContext = manager.GetApp("suite-app");

        internalContext.CorpId.Should().Be("ww-corp");
        suiteContext.CorpId.Should().Be("ww-provider");
        internalContext.GetTokenManager(WechatTokenTypes.AccessToken)
            .Should().NotBeSameAs(suiteContext.GetTokenManager(WechatTokenTypes.AccessToken),
                "多应用（自建 vs 第三方）令牌管理器互不串扰");
    }

    public void Dispose()
    {
        _provider?.Dispose();
        (_services as IDisposable)?.Dispose();
    }
}
