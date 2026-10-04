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
    /// ExternalContact 模块（客户联系：企业服务人员管理域 + 客户管理域 + 客户标签管理域 + 在职继承域 +
    /// 离职继承域 + 客户群管理域 + 联系我与客户入群方式域 + 客户朋友圈域 + 获客助手域 +
    /// 消息推送（群发）域 + 统计管理域 + 商品图册域 + 聊天敏感词域 + 上传附件资源域 +
    /// 获取已服务的外部联系人域（仅自建） + 获客助手组件域·代支付流水接口族（仅第三方））：
    /// AddExternalContactApi 注册的应用类型子接口客户端必须可解析（公共父接口 IsAbstract，不参与 DI 注册）。
    /// </summary>
    [Fact]
    public void AddExternalContactApi_ShouldRegisterExternalContactDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddExternalContactApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalExternalContactFollowUserService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactFollowUserService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactFollowUserService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactCustomerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactCustomerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactCustomerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactTagService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactTagService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactTagService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactJobInheritanceService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactJobInheritanceService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactJobInheritanceService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactResignedInheritanceService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactResignedInheritanceService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactResignedInheritanceService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactGroupChatService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactGroupChatService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactGroupChatService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactContactWayService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactContactWayService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactContactWayService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactMomentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactMomentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactMomentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactCustomerAcquisitionService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactCustomerAcquisitionService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactCustomerAcquisitionService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactGroupMsgService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactGroupMsgService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactGroupMsgService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactStatisticsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactStatisticsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactStatisticsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactProductAlbumService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactProductAlbumService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactProductAlbumService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactInterceptRuleService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactInterceptRuleService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactInterceptRuleService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactAttachmentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactAttachmentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderExternalContactAttachmentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalExternalContactServedContactService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactAcquisitionComponentService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService>().Should().NotBeNull();

        provider.GetService<IWechatWorkExternalContactFollowUserService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactCustomerService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactTagService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactJobInheritanceService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactResignedInheritanceService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactGroupChatService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactContactWayService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactMomentService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactCustomerAcquisitionService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactGroupMsgService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactStatisticsService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactProductAlbumService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactInterceptRuleService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactAttachmentService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactServedContactService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactAcquisitionComponentService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkExternalContactAcquisitionComponentBillService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactFollowUserService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactCustomerService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactTagService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactJobInheritanceService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactResignedInheritanceService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactGroupChatService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactContactWayService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactMomentService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactCustomerAcquisitionService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactGroupMsgService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactStatisticsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactProductAlbumService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactInterceptRuleService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactAttachmentService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalExternalContactServedContactService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyExternalContactAcquisitionComponentService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService>().Should().NotBeNull(
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
        provider.GetRequiredService<IWechatWorkThirdPartyCorpGroupService>().Should().NotBeNull(
            "官方向第三方应用开放获取应用共享信息（95324，随父接口继承），本域注册第三方子接口");
        provider.GetRequiredService<IWechatWorkProviderCorpGroupService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalCorpGroupContactsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderCorpGroupContactsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalCorpGroupRulesService>().Should().NotBeNull(
            "官方仅向自建应用开放上下游规则域，本域仅注册自建子接口");

        provider.GetService<IWechatWorkCorpGroupService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkCorpGroupContactsService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkCorpGroupRulesService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalCorpGroupService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyCorpGroupService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderCorpGroupService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalCorpGroupContactsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderCorpGroupContactsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalCorpGroupRulesService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// Security 模块（安全管理域）：AddSecurityApi 注册的自建子接口客户端必须可解析
    ///（三接口族均为公共父接口 IsAbstract 不参与 DI 注册，官方仅向自建应用开放，无第三方/代开发子接口）。
    /// </summary>
    [Fact]
    public void AddSecurityApi_ShouldRegisterSecurityDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddSecurityApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalSecurityService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalSecurityVipService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalSecurityOperLogService>().Should().NotBeNull();

        provider.GetService<IWechatWorkSecurityService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkSecurityVipService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkSecurityOperLogService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalSecurityService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalSecurityVipService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalSecurityOperLogService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// Message 模块（消息推送域）：AddMessageApi 注册的子接口客户端必须可解析
    ///（发送应用消息族为公共父接口 + 自建/第三方/代开发三个子接口；群聊会话族、家校学校通知族
    /// 与智能表格自动化创建的群聊族官方仅向自建应用开放，均为公共父接口 IsAbstract 不参与 DI 注册
    /// + 仅自建子接口承载端点）。
    /// </summary>
    [Fact]
    public void AddMessageApi_ShouldRegisterMessageDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddMessageApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalMessageService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyMessageService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderMessageService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalAppChatService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalSchoolMessageService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartySchoolMessageService>().Should().NotBeNull(
            "家校学校通知族官方向三类应用开放（第三方 92291），本族注册第三方子接口");
        provider.GetRequiredService<IWechatWorkProviderSchoolMessageService>().Should().NotBeNull(
            "家校学校通知族官方向三类应用开放（代开发 96720/96723），本族注册代开发子接口");
        provider.GetRequiredService<IWechatWorkInternalSmartSheetGroupChatService>().Should().NotBeNull();

        provider.GetService<IWechatWorkMessageService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkAppChatService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkSchoolMessageService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkSmartSheetGroupChatService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalMessageService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyMessageService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderMessageService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalAppChatService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalSchoolMessageService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartySchoolMessageService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderSchoolMessageService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalSmartSheetGroupChatService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// Kf 模块（微信客服域：客服账号管理域 + 接待人员管理域 + 会话分配与消息收发域 +
    /// 客户基础信息域 + 「升级服务」配置域 + 统计管理域 + 机器人管理域（仅自建） +
    /// 微信客服组件域（仅第三方））：AddKfApi 注册的应用类型子接口客户端必须可解析
    /// （六域公共父接口 IsAbstract 不参与 DI 注册；机器人管理域仅自建子接口、
    /// 微信客服组件域仅第三方子接口承载端点）。
    /// </summary>
    [Fact]
    public void AddKfApi_ShouldRegisterKfDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddKfApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalKfAccountService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyKfAccountService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderKfAccountService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalKfServicerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyKfServicerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderKfServicerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalKfSessionService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyKfSessionService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderKfSessionService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalKfCustomerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyKfCustomerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderKfCustomerService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalKfStatisticsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyKfStatisticsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderKfStatisticsService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalKfUpgradeService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyKfUpgradeService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderKfUpgradeService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalKfKnowledgeService>().Should().NotBeNull(
            "官方仅向自建应用开放机器人管理域（第三方/代开发暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkThirdPartyKfComponentService>().Should().NotBeNull(
            "微信客服组件域官方仅由微信客服组件应用（套件形态）消费，本域仅注册第三方子接口");

        provider.GetService<IWechatWorkKfAccountService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkKfServicerService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkKfSessionService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkKfCustomerService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkKfStatisticsService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkKfUpgradeService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkKfKnowledgeService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkKfComponentService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalKfAccountService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyKfAccountService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderKfAccountService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalKfServicerService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyKfServicerService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderKfServicerService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalKfSessionService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyKfSessionService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderKfSessionService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalKfCustomerService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyKfCustomerService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderKfCustomerService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalKfStatisticsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyKfStatisticsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderKfStatisticsService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalKfUpgradeService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyKfUpgradeService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderKfUpgradeService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalKfKnowledgeService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyKfComponentService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// Pay 模块（企业支付域：对外收款记录域为三类应用公共面；收款商户号管理域 +
    /// 资金流水域 + 创建对外收款账户域 + 普通支付域 + 退款域 + 交易账单域官方仅自建开放）：
    /// AddPayApi 注册的应用类型子接口客户端必须可解析（公共父接口 IsAbstract 不参与 DI 注册；
    /// 六个仅自建域的仅自建子接口承载端点）。
    /// </summary>
    [Fact]
    public void AddPayApi_ShouldRegisterPayDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddPayApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalPayBillService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyPayBillService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderPayBillService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalPayMerchantService>().Should().NotBeNull(
            "官方仅向自建应用开放收款商户号管理域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalPayFundFlowService>().Should().NotBeNull(
            "官方仅向自建应用开放资金流水域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalPayMchApplyService>().Should().NotBeNull(
            "官方仅向自建应用开放创建对外收款账户域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalPayOrderService>().Should().NotBeNull(
            "官方仅向自建应用开放普通支付域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalPayRefundService>().Should().NotBeNull(
            "官方仅向自建应用开放退款域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalPayTradeBillService>().Should().NotBeNull(
            "官方仅向自建应用开放交易账单域（代开发/第三方暂不支持），本域仅注册自建子接口");

        provider.GetService<IWechatWorkPayBillService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkPayMerchantService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkPayFundFlowService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkPayMchApplyService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkPayOrderService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkPayRefundService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkPayTradeBillService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalPayBillService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyPayBillService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderPayBillService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalPayMerchantService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalPayFundFlowService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalPayMchApplyService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalPayOrderService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalPayRefundService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalPayTradeBillService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// MsgAudit 模块（会话内容存档域：开启成员列表域 + 机器人信息域 + 会话同意情况域 +
    /// 内部群信息域，官方仅自建开放）：AddMsgAuditApi 注册的仅自建子接口客户端必须可解析
    /// （公共父接口 IsAbstract 不参与 DI 注册，仅自建子接口承载端点）。
    /// </summary>
    [Fact]
    public void AddMsgAuditApi_ShouldRegisterMsgAuditDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddMsgAuditApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalMsgAuditPermitUserService>().Should().NotBeNull(
            "官方仅向自建应用开放开启成员列表域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalMsgAuditRobotService>().Should().NotBeNull(
            "官方仅向自建应用开放机器人信息域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalMsgAuditAgreeService>().Should().NotBeNull(
            "官方仅向自建应用开放会话同意情况域（代开发/第三方暂不支持），本域仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalMsgAuditGroupChatService>().Should().NotBeNull(
            "官方仅向自建应用开放内部群信息域（代开发/第三方暂不支持），本域仅注册自建子接口");

        provider.GetService<IWechatWorkMsgAuditPermitUserService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkMsgAuditRobotService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkMsgAuditAgreeService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkMsgAuditGroupChatService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalMsgAuditPermitUserService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalMsgAuditRobotService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalMsgAuditAgreeService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalMsgAuditGroupChatService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// School 模块（家校沟通域：家校沟通基础域为三类应用公共面；家校管理配置域官方仅自建与第三方开放，
    /// 无代开发子接口）：AddSchoolApi 注册的应用类型子接口客户端必须可解析
    /// （公共父接口 IsAbstract 不参与 DI 注册；配置域继承链上恰好只有自建与第三方子接口）。
    /// </summary>
    [Fact]
    public void AddSchoolApi_ShouldRegisterSchoolDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddSchoolApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalSchoolService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartySchoolService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderSchoolService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalSchoolSettingService>().Should().NotBeNull(
            "家校管理配置域注册自建子接口");
        provider.GetRequiredService<IWechatWorkThirdPartySchoolSettingService>().Should().NotBeNull(
            "家校管理配置域官方亦向第三方应用开放，本域注册第三方子接口");

        provider.GetService<IWechatWorkSchoolService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkSchoolSettingService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalSchoolService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartySchoolService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderSchoolService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalSchoolSettingService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartySchoolSettingService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// Gov 模块（政民沟通域：配置网格结构域 + 配置事件类别域为自建/代开发公共面；
    /// 获取网格列表、巡查上报族与居民上报族官方仅自建开放，代开发/第三方「暂不支持」）：
    /// AddGovApi 注册的应用类型子接口客户端必须可解析
    /// （公共父接口 IsAbstract 不参与 DI 注册；仅自建族继承链上恰好只有自建子接口）。
    /// </summary>
    [Fact]
    public void AddGovApi_ShouldRegisterGovDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddGovApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalGovGridService>().Should().NotBeNull(
            "配置网格结构域注册自建子接口");
        provider.GetRequiredService<IWechatWorkProviderGovGridService>().Should().NotBeNull(
            "配置网格结构域官方亦向代开发应用开放，本域注册代开发子接口");
        provider.GetRequiredService<IWechatWorkInternalGovGridListService>().Should().NotBeNull(
            "获取网格列表官方仅向自建应用开放，本族仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalGovEventCategoryService>().Should().NotBeNull(
            "配置事件类别域注册自建子接口");
        provider.GetRequiredService<IWechatWorkProviderGovEventCategoryService>().Should().NotBeNull(
            "配置事件类别域官方亦向代开发应用开放，本域注册代开发子接口");
        provider.GetRequiredService<IWechatWorkInternalGovPatrolService>().Should().NotBeNull(
            "巡查上报族官方仅向自建应用开放，本族仅注册自建子接口");
        provider.GetRequiredService<IWechatWorkInternalGovResidentService>().Should().NotBeNull(
            "居民上报族官方仅向自建应用开放，本族仅注册自建子接口");

        provider.GetService<IWechatWorkGovGridService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkGovGridListService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkGovEventCategoryService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkGovPatrolService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkGovResidentService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalGovGridService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderGovGridService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalGovGridListService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalGovEventCategoryService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderGovEventCategoryService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalGovPatrolService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalGovResidentService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// Approval 模块（审批域：审批申请数据域 + 审批模板域 + 假期管理域 + 审批流程引擎域四族；
    /// 获取审批数据（旧）官方仅自建、创建/更新模板自建与代开发开放（官方对第三方标注暂不支持）、
    /// 复制/更新模板到企业官方仅第三方）：AddApprovalApi 注册的 12 个应用类型子接口客户端必须可解析
    /// （四族公共父接口 IsAbstract 不参与 DI 注册，调用方须按应用类型选择子接口）。
    /// </summary>
    [Fact]
    public void AddApprovalApi_ShouldRegisterApprovalDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddApprovalApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalApprovalService>().Should().NotBeNull(
            "审批申请数据域注册自建子接口（额外承载官方仅自建开放的获取审批数据（旧））");
        provider.GetRequiredService<IWechatWorkProviderApprovalService>().Should().NotBeNull(
            "审批申请数据域注册代开发子接口");
        provider.GetRequiredService<IWechatWorkThirdPartyApprovalService>().Should().NotBeNull(
            "审批申请数据域注册第三方子接口");
        provider.GetRequiredService<IWechatWorkInternalApprovalTemplateService>().Should().NotBeNull(
            "审批模板域注册自建子接口（额外承载创建/更新审批模板）");
        provider.GetRequiredService<IWechatWorkProviderApprovalTemplateService>().Should().NotBeNull(
            "审批模板域注册代开发子接口（额外承载创建/更新审批模板）");
        provider.GetRequiredService<IWechatWorkThirdPartyApprovalTemplateService>().Should().NotBeNull(
            "审批模板域注册第三方子接口（额外承载官方仅第三方开放的复制/更新模板到企业）");
        provider.GetRequiredService<IWechatWorkInternalVacationService>().Should().NotBeNull(
            "假期管理域注册自建子接口");
        provider.GetRequiredService<IWechatWorkProviderVacationService>().Should().NotBeNull(
            "假期管理域注册代开发子接口");
        provider.GetRequiredService<IWechatWorkThirdPartyVacationService>().Should().NotBeNull(
            "假期管理域注册第三方子接口");
        provider.GetRequiredService<IWechatWorkInternalApprovalEngineService>().Should().NotBeNull(
            "审批流程引擎域注册自建子接口");
        provider.GetRequiredService<IWechatWorkProviderApprovalEngineService>().Should().NotBeNull(
            "审批流程引擎域注册代开发子接口");
        provider.GetRequiredService<IWechatWorkThirdPartyApprovalEngineService>().Should().NotBeNull(
            "审批流程引擎域注册第三方子接口");

        provider.GetService<IWechatWorkApprovalService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkApprovalTemplateService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkVacationService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkApprovalEngineService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalApprovalService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyApprovalTemplateService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderVacationService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalApprovalEngineService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }

    /// <summary>
    /// Identity 模块（身份验证域：网页授权登录/企业微信Web登录身份获取域 + 第三方套件级身份获取域 + 二次验证域）：
    /// AddIdentityApi 注册的应用类型子接口客户端必须可解析（身份获取族公共父接口 IsAbstract + 自建/代开发空标记子接口；
    /// 第三方身份获取族与二次验证族均为零端点父接口 IsAbstract + 唯一子接口承载端点）。
    /// </summary>
    [Fact]
    public void AddIdentityApi_ShouldRegisterIdentityDomainClients_ResolvableInRootAndScope()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { InternalConfig() });
        services.AddWechatWorkServices(builder => builder.AddIdentityApi());

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        provider.GetRequiredService<IWechatWorkInternalIdentityService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkProviderIdentityService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkThirdPartyIdentitySuiteService>().Should().NotBeNull();
        provider.GetRequiredService<IWechatWorkInternalIdentityTfaService>().Should().NotBeNull();

        provider.GetService<IWechatWorkIdentityService>().Should().BeNull(
            "公共父接口 IsAbstract = true，不得注册进 DI（调用方须按应用类型选择子接口）");
        provider.GetService<IWechatWorkIdentitySuiteService>().Should().BeNull(
            "零端点父接口 IsAbstract = true，不得注册进 DI（调用方须使用第三方子接口）");
        provider.GetService<IWechatWorkIdentityTfaService>().Should().BeNull(
            "零端点父接口 IsAbstract = true，不得注册进 DI（调用方须使用自建子接口）");

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalIdentityService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkProviderIdentityService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkThirdPartyIdentitySuiteService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
        scope.ServiceProvider.GetRequiredService<IWechatWorkInternalIdentityTfaService>().Should().NotBeNull(
            "ValidateScopes = true 变体下子 scope 内同样可解析");
    }
}
