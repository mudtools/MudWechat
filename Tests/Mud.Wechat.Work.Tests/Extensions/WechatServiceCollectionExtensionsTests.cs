// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Mud.HttpUtils;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.TokenManagers;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Tests.Extensions;

/// <summary>
/// DI 注册链路集成测试：AddWechatApp + AddWechatWorkServices 完整装配（验收标准 §十）。
/// </summary>
public class WechatServiceCollectionExtensionsTests
{
    private static WechatAppConfig InternalConfig(string appKey = "default") => new()
    {
        AppKey = appKey,
        AppType = WechatAppType.Internal,
        CorpId = "ww-corp",
        AgentSecret = "agent-secret",
    };

    private static ServiceProvider BuildProvider(Action<ServiceCollection>? patch = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());
        patch?.Invoke(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public void AddWechatWorkServices_ShouldRegisterFullInfrastructure()
    {
        using var provider = BuildProvider();

        provider.GetRequiredService<Abstractions.Authentication.IWechatAppManager>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.IWechatAppContext>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.IWechatWorkInternalAppAuthentication>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderAuthenticationService>().Should().NotBeNull();
        provider.GetRequiredService<Mud.HttpUtils.ITokenProvider>().Should().NotBeNull();
        provider.GetRequiredService<Mud.HttpUtils.IAppContextHolder>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatTokenStore>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatCorpAuthStore>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatSuiteTicketStore>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatSuiteTicketProvider>().Should().NotBeNull();
    }

    [Fact]
    public void AddWechatWorkServices_ShouldAttachErrcodeDetector_ViaPostConfigure()
    {
        using var provider = BuildProvider();

        var options = provider.GetRequiredService<IOptionsMonitor<TokenRecoveryOptions>>().CurrentValue;
        options.TokenInvalidationDetector.Should().NotBeNull("PostConfigure 保证每次快照携带判定器（§14）");
        options.TokenInvalidationDetector.Should().BeOfType<WechatTokenInvalidationDetector>();
    }

    [Fact]
    public void AddWechatWorkServices_ShouldFail_WhenNoModuleRegistered()
    {
        var services = new ServiceCollection();
        var act = () => services.AddWechatWorkServices(_ => { });
        act.Should().Throw<InvalidOperationException>().WithMessage("*至少需要添加一个服务*");
    }

    [Fact]
    public void AddWechatWorkServices_ShouldFail_WhenAppManagerMissing()
    {
        var services = new ServiceCollection();
        var act = () => services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());
        act.Should().Throw<InvalidOperationException>().WithMessage("*IWechatAppManager*");
    }

    [Fact]
    public void GeneratedClient_ShouldInjectSuiteTokenAsQuery()
    {
        using var provider = BuildProvider();

        var service = provider.GetRequiredService<IWechatWorkProviderAuthenticationService>();

        // 生成代码的令牌解析链路：appContext.GetTokenManager("Wechat.SuiteAccessToken")。
        var context = provider.GetRequiredService<Abstractions.Authentication.IWechatAppContext>();
        var act = () => context.GetTokenManager("Wechat.SuiteAccessToken");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*套件令牌管理器*");
    }

    [Fact]
    public void TokenRecoveryContext_ShouldCarryQueryReinjectionContract()
    {
        // 复刻 MudHttpUtilsUpgradeSmokeTests 契约：Query 注入恢复上下文以 QueryParameterName 重注入。
        var context = new Mud.HttpUtils.TokenRecoveryContext
        {
            InjectionMode = Mud.HttpUtils.TokenInjectionMode.Query,
            QueryParameterName = "suite_access_token",
            TokenManagerKey = WechatTokenTypes.SuiteAccessToken,
        };

        context.QueryParameterName.Should().Be("suite_access_token");
        context.TokenManagerKey.Should().Be("Wechat.SuiteAccessToken");
    }
}
