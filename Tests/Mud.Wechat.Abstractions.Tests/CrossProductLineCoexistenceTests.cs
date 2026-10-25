// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Mud.Wechat.OfficialAccount.Abstractions;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions;

namespace Mud.Wechat.Abstractions.Tests;

/// <summary>
/// 跨产品线共存（同一宿主同时引用企业微信与公众号 SDK）：
/// 两处进程级全局态必须保持并集且互不覆盖 —— 这是公用层存在的最硬理由。
/// </summary>
public class CrossProductLineCoexistenceTests
{
    private static ServiceCollection BuildBothProductLines()
    {
        var services = new ServiceCollection();

        services.AddWechatApp(config =>
        {
            config.AppKey = "work-default";
            config.CorpId = "ww-test";
            config.AgentSecret = "work-secret";
            config.IsDefault = true;
        });

        services.AddMpApp(config =>
        {
            config.AppKey = "mp-default";
            config.AppId = "wx-test";
            config.AppSecret = "mp-secret";
            config.IsDefault = true;
        });

        return services;
    }

    /// <summary>
    /// X1：SSRF 白名单为并集且不被后注册的产品线清空。
    /// </summary>
    /// <remarks>
    /// 组件 <c>UrlValidator.ConfigureAllowedDomains</c> 是全局静态 + <b>整体替换</b>语义；
    /// 若两条产品线各自以自身白名单调用，后调用者会清空前者。本用例锁定「公用层单点登记」的收敛结果。
    /// </remarks>
    [Fact]
    public void AllowedDomains_ShouldBeUnionAfterBothProductLinesRegistered()
    {
        _ = BuildBothProductLines();

        var domains = UrlValidator.GetAllowedDomains();
        domains.Should().Contain("weixin.qq.com", "公众号默认域名 api.weixin.qq.com 依赖此项");
        domains.Should().Contain("work.weixin.qq.com", "企业微信默认域名依赖此项");
    }

    /// <summary>
    /// X2：令牌失效判定器经<b>追加集</b>登记，两条产品线的子判定器同时在场（并集语义，互不覆盖）。
    /// </summary>
    /// <remarks>
    /// 组件选项的单槽属性为「后注册者覆盖前者」：若两条产品线各自 PostConfigure 赋值，一方令牌恢复会静默失效。
    /// 追加集（<c>AdditionalTokenInvalidationDetectors</c>）与单槽为并集语义、只增不改他线条目 ⇒
    /// 「两线判定器均已登记」的等价断言是「追加集内容与容器内登记的子判定器逐一同源」。
    /// </remarks>
    [Fact]
    public void TokenInvalidationDetector_ShouldComposeBothProductLines()
    {
        var services = BuildBothProductLines();
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value;
        var registered = provider.GetServices<ITokenInvalidationDetector>().ToList();

        options.TokenInvalidationDetector.Should().BeNull("单槽属性会让后注册的产品线覆盖前者，本仓统一走追加集");
        registered.Should().HaveCountGreaterThanOrEqualTo(2, "企微与公众号两条产品线各登记一个子判定器");
        options.AdditionalTokenInvalidationDetectors.Should().BeEquivalentTo(
            registered, "后置配置必须把全部子判定器追加进追加集（任一命中即判失效，两线互不覆盖）");
    }

    /// <summary>X3：后台刷新框架服务只登记一次（组件注册自 3.0.4 起幂等；多产品线各自调用只会有一份）。</summary>
    [Fact]
    public void TokenRefreshBackgroundService_ShouldBeRegisteredOnce()
    {
        var services = BuildBothProductLines();

        services.Count(d => d.ServiceType == typeof(ITokenRefreshBackgroundService))
            .Should().Be(1, "多产品线共存时只能有一个后台刷新循环（公用层登记点 + 组件幂等注册）");
    }

    /// <summary>
    /// X3b（W-19）：两条产品线经<b>配置节重载</b>注册（均指向同名节 <c>MudHttpTokenRecovery</c>）时，
    /// <c>IOptionsChangeTokenSource&lt;TokenRecoveryOptions&gt;</c> 只登记一份（TryAdd，先注册者胜）。
    /// AddSingleton 时代后注册者会覆盖前者 ⇒ 两者指向同一节时虽无行为差，但登记数膨胀、语义不可审计。
    /// </summary>
    [Fact]
    public void TokenRecoveryChangeTokenSource_ShouldBeRegisteredOnce_AcrossProductLines()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WechatApps:0:AppKey"] = "work-cfg",
                ["WechatApps:0:CorpId"] = "ww-corp",
                ["WechatApps:0:AgentSecret"] = "work-secret",
                ["MpApps:0:AppKey"] = "mp-cfg",
                ["MpApps:0:AppId"] = "wx-mp",
                ["MpApps:0:AppSecret"] = "mp-secret",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddWechatApp(configuration);
        services.AddMpApp(configuration);

        services.Count(d => d.ServiceType == typeof(IOptionsChangeTokenSource<TokenRecoveryOptions>))
            .Should().Be(1, "多产品线共用 TokenRecoveryOptions（同名节），变更令牌来源只需一份（TryAdd 先注册者胜）");
    }

    /// <summary>X4：两条产品线的令牌存储端口共用同一实现实例（键前缀含产品线令牌类型，故键空间不交叠）。</summary>
    [Fact]
    public void TokenStore_ShouldBeSharedAcrossProductLines()
    {
        var services = BuildBothProductLines();
        using var provider = services.BuildServiceProvider();

        var store = provider.GetRequiredService<IWechatTokenStore>();
        store.Should().BeOfType<InMemoryWechatTokenStore>();
    }

    /// <summary>X5：两条产品线的核心服务可同时解析（DI 图无冲突）。</summary>
    [Fact]
    public void BothProductLineManagers_ShouldBeResolvable()
    {
        var services = BuildBothProductLines();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<Mud.Wechat.Work.Abstractions.Authentication.IWechatAppManager>()
            .Should().NotBeNull();
        provider.GetRequiredService<MpAppManager>().Should().NotBeNull();
    }

    /// <summary>
    /// X6：后台刷新的启用是「两条产品线各自 PostConfigure 的并集」——任一产品线存在已配置应用即启用。
    /// </summary>
    /// <remarks>
    /// 两条产品线各注册一个 <c>PostConfigure&lt;TokenRefreshBackgroundOptions&gt;</c>（依赖各自的配置列表类型），
    /// 属隐式并集语义；须有用例锁定，否则将来某产品线改默认值时会静默关闭整体刷新。
    /// </remarks>
    [Fact]
    public void BackgroundRefresh_ShouldBeEnabledWhenAnyProductLineHasApps()
    {
        var services = BuildBothProductLines();
        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IOptions<TokenRefreshBackgroundOptions>>().Value.Enabled
            .Should().BeTrue("存在已配置应用时后台刷新必须自动启用（两产品线的 PostConfigure 取并集）");
    }

    /// <summary>
    /// X7：多应用注册必须一次传列表——重复调用 <c>AddXxxApp</c> 会覆盖注册表（先注册的应用静默丢失）。
    /// </summary>
    /// <remarks>
    /// 两条产品线均以「注册表单一来源 = 本次传入的配置列表」实现，故都要 fail-fast；
    /// 公众号侧由 <c>AddMpApp</c> 守卫，企微侧由 <c>AddWechatApp</c> 守卫。
    /// </remarks>
    [Fact]
    public void RepeatedAppRegistration_ShouldFailFastOnBothProductLines()
    {
        var workServices = new ServiceCollection();
        workServices.AddWechatApp(config => { config.AppKey = "w1"; config.CorpId = "c"; config.AgentSecret = "s"; });
        var workAct = () => workServices.AddWechatApp(config => { config.AppKey = "w2"; config.CorpId = "c"; config.AgentSecret = "s"; });
        workAct.Should().Throw<InvalidOperationException>().WithMessage("*已被调用过*");

        var mpServices = new ServiceCollection();
        mpServices.AddMpApp(config => { config.AppKey = "m1"; config.AppId = "a"; config.AppSecret = "s"; });
        var mpAct = () => mpServices.AddMpApp(config => { config.AppKey = "m2"; config.AppId = "b"; config.AppSecret = "s"; });
        mpAct.Should().Throw<InvalidOperationException>().WithMessage("*已被调用过*");
    }
}
