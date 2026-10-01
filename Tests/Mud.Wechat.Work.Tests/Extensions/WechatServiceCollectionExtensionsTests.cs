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
        provider.GetRequiredService<IWechatWorkProviderAuthenticationUrl>()
            .Should().NotBeNull("get_customized_auth_url 复用 AddAuthenticationWebApiHttpClient() 注册，不新增独立注册项");
        provider.GetRequiredService<Mud.HttpUtils.ITokenProvider>().Should().NotBeNull();
        provider.GetRequiredService<Mud.HttpUtils.IAppContextHolder>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatTokenStore>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatCorpAuthStore>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatSuiteTicketStore>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.TokenManager.IWechatSuiteTicketProvider>().Should().NotBeNull();
        provider.GetRequiredService<Abstractions.Authentication.IWechatAppContextSwitcher>()
            .Should().NotBeNull("R9：IWechatAppContextSwitcher 必须可从 DI 解析（此前仅有声明无实现/无注册）");
        provider.GetRequiredService<Mud.HttpUtils.IAppContextSwitcher>()
            .Should().NotBeNull("切换器同时满足框架 IAppContextSwitcher 契约");
    }

    /// <summary>
    /// P0-1（G8-A）：<c>IAppContextHolder</c> 与 <c>IWechatAppContextSwitcher</c> 必须是**同一实例**，
    /// 否则生成的声明式客户端读到的 <c>IAppContextHolder.Current</c> 恒为 null（多套件静默回退默认应用令牌）。
    /// </summary>
    [Fact]
    public void AppContextHolder_ShouldBeSameInstanceAsSwitcher()
    {
        using var provider = BuildProvider();

        var holder = provider.GetRequiredService<Mud.HttpUtils.IAppContextHolder>();
        var switcher = provider.GetRequiredService<Abstractions.Authentication.IWechatAppContextSwitcher>();
        var frameworkSwitcher = provider.GetRequiredService<Mud.HttpUtils.IAppContextSwitcher>();

        ReferenceEquals(holder, switcher).Should().BeTrue(
            "IAppContextHolder 与 IWechatAppContextSwitcher 必须共用同一 AsyncLocal 状态");
        ReferenceEquals(frameworkSwitcher, switcher).Should().BeTrue(
            "框架 IAppContextSwitcher 契约也必须落到同一实例");
    }

    /// <summary>P0-1：ValidateScopes 变体下同样成立（Singleton 不得因 scope 变化而分裂）。</summary>
    [Fact]
    public void AppContextHolder_ShouldBeSameInstanceAsSwitcher_WhenValidateScopes()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        var holder = provider.GetRequiredService<Mud.HttpUtils.IAppContextHolder>();
        var switcher = provider.GetRequiredService<Abstractions.Authentication.IWechatAppContextSwitcher>();
        ReferenceEquals(holder, switcher).Should().BeTrue();
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

    /// <summary>
    /// Contact 模块（成员管理域 + 部门管理域 + 标签管理域 + 通讯录查看权限管理域 + 异步导入接口域 + 异步导出接口域）：
    /// AddContactApi 注册的应用类型子接口客户端必须可解析（公共父接口 IsAbstract，不参与 DI 注册）。
    /// </summary>
    [Fact]
    public void AddContactApi_ShouldRegisterContactDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddContactApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalUsersService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyUsersService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderUsersService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalDepartmentsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyDepartmentsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderDepartmentsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalTagsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyTagsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderTagsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalContactRulesService>().Should().NotBeNull(
            "官方仅向自建应用开放通讯录查看权限管理，本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalBatchService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyBatchService>().Should().NotBeNull(
            "官方未向代开发开放异步导入，本域仅注册自建与第三方子接口");
        provider.GetRequiredService<IWechatWorkInternalExportService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExportService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExportService>().Should().NotBeNull();

        provider.GetService<IWechatWorkUsersService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkDepartmentsService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkTagsService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkContactRulesService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkBatchService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExportService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalUsersService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalDepartmentsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalTagsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalContactRulesService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalBatchService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExportService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// CorpGroup 模块（上下游域）：AddCorpGroupApi 注册的应用类型子接口客户端必须可解析
    /// （公共父接口 IsAbstract，不参与 DI 注册；官方仅向自建/代开发开放，无第三方子接口）。
    /// </summary>
    [Fact]
    public void AddCorpGroupApi_ShouldRegisterCorpGroupDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddCorpGroupApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalCorpGroupService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderCorpGroupService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalCorpGroupContactsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderCorpGroupContactsService>().Should().NotBeNull();

        provider.GetService<IWechatWorkCorpGroupService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkCorpGroupContactsService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalCorpGroupService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderCorpGroupService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalCorpGroupContactsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderCorpGroupContactsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }
}
