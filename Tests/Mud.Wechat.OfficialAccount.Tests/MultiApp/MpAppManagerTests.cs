// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.MultiApp;

/// <summary>多公众号基座：DI 装配、懒加载、显式不支持面、上下文作用域归还与通道装配。</summary>
public class MpAppManagerTests
{
    private static ServiceProvider BuildProvider(bool validateScopes = false)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(new List<MpAppConfig>
        {
            new()
            {
                AppKey = "mp-default",
                AppId = "wx-a",
                AppSecret = "s-a",
                IsDefault = true,
            },
            new()
            {
                AppKey = "mp-second",
                AppId = "wx-b",
                AppSecret = "s-b",
            },
        });
        services.AddMpServices(builder => builder.AddBasicApi());

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = validateScopes });
    }

    /// <summary>A1：`IMpAppManager` / `MpAppManager` 同一实例；多应用可解析。</summary>
    [Fact]
    public void Manager_ShouldBeResolvableAsSameInstance()
    {
        using var provider = BuildProvider();

        var concrete = provider.GetRequiredService<MpAppManager>();
        var contract = provider.GetRequiredService<IMpAppManager>();

        ReferenceEquals(concrete, contract).Should().BeTrue("契约与实现必须同一实例（避免影子注册表）");
        contract.ConfiguredAppKeys.Should().BeEquivalentTo(new[] { "mp-default", "mp-second" });
        contract.DefaultAppKey.Should().Be("mp-default");
    }

    /// <summary>A2：DI 桥接不变量——切换器 / 组件 `IAppContextSwitcher` / `IAppContextHolder` 三者同一实例。</summary>
    [Fact]
    public void ContextSwitcher_ShouldShareSingleInstance()
    {
        using var provider = BuildProvider();

        var holder = provider.GetRequiredService<IAppContextHolder>();
        var frameworkSwitcher = provider.GetRequiredService<IAppContextSwitcher>();
        var mpSwitcher = provider.GetRequiredService<IMpAppContextSwitcher>();

        ReferenceEquals(holder, mpSwitcher).Should().BeTrue(
            "破坏该不变量 ⇒ 声明式 [Token] 客户端读到的上下文恒为 null，多公众号静默回退默认公众号令牌");
        ReferenceEquals(frameworkSwitcher, mpSwitcher).Should().BeTrue();
    }

    /// <summary>A3：`ValidateScopes=true` 下跨 scope 解析同一实例（避免 Captive Dependency）。</summary>
    [Fact]
    public void Manager_ShouldSurviveScopeValidation()
    {
        using var provider = BuildProvider(validateScopes: true);

        var first = provider.GetRequiredService<IMpAppManager>();
        var second = provider.GetRequiredService<IMpAppManager>();
        ReferenceEquals(first, second).Should().BeTrue();
        first.GetDefaultApp().AppKey.Should().Be("mp-default");
    }

    /// <summary>A4：默认应用装配稳定版通道（`UseStableToken` 默认 true）。</summary>
    [Fact]
    public void DefaultApp_ShouldUseStableChannelByDefault()
    {
        using var provider = BuildProvider();

        var manager = provider.GetRequiredService<IMpAppManager>().DefaultAccessTokenManager;
        manager.GetType().Name.Should().Be("MpStableAccessTokenManager",
            "UseStableToken=true（默认）⇒ 注册期装配稳定版通道实现");
    }

    /// <summary>A5：`UseStableToken=false` 时装配普通通道实现（配置开关的真实消费点）。</summary>
    [Fact]
    public void UseStableTokenFalse_ShouldSwitchChannelImplementation()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(config =>
        {
            config.AppKey = "mp-standard";
            config.AppId = "wx-c";
            config.AppSecret = "s-c";
            config.UseStableToken = false;
        });
        services.AddMpServices(builder => builder.AddBasicApi());

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IMpAppManager>().DefaultAccessTokenManager
            .GetType().Name.Should().Be("MpStandardAccessTokenManager");
    }

    /// <summary>A6：懒加载——未访问的应用不出现在 `GetAllApps`，配置面读取不触发实例化。</summary>
    [Fact]
    public void GetAllApps_ShouldRespectLazySemantics()
    {
        using var provider = BuildProvider();
        var manager = provider.GetRequiredService<IMpAppManager>();

        manager.GetAllApps().Should().BeEmpty("未访问任何应用 ⇒ 无一实例化");

        _ = manager.GetApp("mp-second");
        manager.GetAllApps().Select(c => c.AppKey).Should().BeEquivalentTo(new[] { "mp-second" });
    }

    /// <summary>A7：`ConfiguredConfigs` / `TryGetConfig` 不触发实例化（回调匹配等只读场景）。</summary>
    [Fact]
    public void ConfiguredConfigs_ShouldNotMaterialize()
    {
        using var provider = BuildProvider();
        var manager = provider.GetRequiredService<IMpAppManager>();

        manager.ConfiguredConfigs.Should().HaveCount(2);
        manager.TryGetConfig("mp-second", out var config).Should().BeTrue();
        config!.AppId.Should().Be("wx-b");
        manager.TryGetConfig("nope", out var missing).Should().BeFalse();
        missing.Should().BeNull();

        manager.GetAllApps().Should().BeEmpty("配置面读取不得物化上下文（否则一次只读动作会构造 HttpClient/管理器 Timer）");
    }

    /// <summary>A8：注册表单一来源——实例注册 / 替换 / 切换器工厂全部显式不支持。</summary>
    [Fact]
    public async Task InstanceRegistrationApis_ShouldBeExplicitlyUnsupported()
    {
        using var provider = BuildProvider();
        var manager = provider.GetRequiredService<IMpAppManager>();
        var context = manager.GetDefaultApp();

        manager.Invoking(m => m.RegisterApp("x", context)).Should().Throw<NotSupportedException>();
        await manager.Invoking(m => m.RegisterAppAsync("x", context)).Should().ThrowAsync<NotSupportedException>();
        manager.Invoking(m => m.UpdateApp("x", context)).Should().Throw<NotSupportedException>();
        await manager.Invoking(m => m.UpdateAppAsync("x", context)).Should().ThrowAsync<NotSupportedException>();
        manager.Invoking(m => m.GetWebApi<IAppContextSwitcher>("mp-default")).Should().Throw<NotSupportedException>();
        manager.Invoking(m => m.GetDefaultWebApi<IAppContextSwitcher>()).Should().Throw<NotSupportedException>();
        manager.Invoking(m => m.RegisterSwitcherFactory<IAppContextSwitcher>(_ => null!))
            .Should().Throw<NotSupportedException>();
    }

    /// <summary>A9：默认应用设置（未知键抛异常 / Try 版本返回 false）。</summary>
    [Fact]
    public void SetDefaultApp_ShouldValidateRegistration()
    {
        using var provider = BuildProvider();
        var manager = provider.GetRequiredService<IMpAppManager>();

        manager.Invoking(m => m.SetDefaultApp("nope")).Should().Throw<InvalidOperationException>();
        manager.TrySetDefaultApp("nope").Should().BeFalse();

        manager.TrySetDefaultApp("mp-second").Should().BeTrue();
        manager.DefaultAppKey.Should().Be("mp-second");
        manager.DefaultConfig.AppKey.Should().Be("mp-second");
    }

    /// <summary>A10：`RemoveApp` 从注册表移除并释放上下文；默认键顺延；随后 `GetDefaultApp` 行为明确。</summary>
    [Fact]
    public void RemoveApp_ShouldDropContextAndReassignDefault()
    {
        using var provider = BuildProvider();
        var manager = provider.GetRequiredService<IMpAppManager>();

        var removed = manager.GetApp("mp-default");
        manager.RemoveApp("mp-default").Should().BeTrue();

        manager.HasApp("mp-default").Should().BeFalse();
        manager.GetAllApps().Should().NotContain(removed);
        manager.DefaultAppKey.Should().Be("mp-second", "默认应用被移除后顺延到剩余配置");
        removed.Invoking(c => c.GetService<IMpAppManager>()).Should().Throw<ObjectDisposedException>();
    }

    /// <summary>A11：`GetApp` 未知键抛出且消息含已注册键（启动期定位友好）。</summary>
    [Fact]
    public void GetApp_ShouldThrowForUnknownKey()
    {
        using var provider = BuildProvider();
        var manager = provider.GetRequiredService<IMpAppManager>();

        manager.Invoking(m => m.GetApp("nope"))
            .Should().Throw<InvalidOperationException>().WithMessage("*mp-default*");
    }

    /// <summary>A12：上下文切换作用域在释放时归还进入前的上下文（多公众号串号防线）。</summary>
    [Fact]
    public void UseAppScope_ShouldRestorePreviousContext()
    {
        using var provider = BuildProvider();
        var switcher = provider.GetRequiredService<IMpAppContextSwitcher>();
        var manager = provider.GetRequiredService<IMpAppManager>();

        var defaultContext = manager.GetDefaultApp();
        switcher.SwitchTo(defaultContext);

        using (switcher.UseAppScope("mp-second"))
        {
            switcher.Current!.AppKey.Should().Be("mp-second");
        }

        switcher.Current!.AppKey.Should().Be("mp-default", "作用域释放必须回滚到进入前的上下文");
    }

    /// <summary>A13：非法 appKey 在切换入口即拒（格式校验先于授权判定与解析）。</summary>
    [Fact]
    public void UseAppScope_ShouldRejectIllegalAppKey()
    {
        using var provider = BuildProvider();
        var switcher = provider.GetRequiredService<IMpAppContextSwitcher>();

        switcher.Invoking(s => s.UseAppScope("bad:key")).Should().Throw<ArgumentException>();
    }

    /// <summary>A14：`InvalidateTokenAsync` 对未知令牌类型为无操作（不抛、不误伤）。</summary>
    [Fact]
    public async Task InvalidateTokenAsync_ShouldIgnoreUnknownTokenType()
    {
        using var provider = BuildProvider();
        var manager = provider.GetRequiredService<IMpAppManager>();

        var act = async () => await manager.InvalidateTokenAsync("mp-default", "Wechat.Unknown");
        await act.Should().NotThrowAsync();
    }

    /// <summary>A15：公众号失效判定器——域名预过滤 + 失效码命中（40001 为公众号特有语义）。</summary>
    [Fact]
    public async Task InvalidationDetector_ShouldMatchMpErrcodesOnly()
    {
        var detector = new MpTokenInvalidationDetector();

        using var mpRequest = new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Get, "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token=x");
        using var otherRequest = new System.Net.Http.HttpRequestMessage(
            System.Net.Http.HttpMethod.Get, "https://example.com/api");

        detector.ShouldInspect(mpRequest).Should().BeTrue();
        detector.ShouldInspect(otherRequest).Should().BeFalse("非微信域名请求零捕获开销");

        using var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);

        (await detector.IsTokenInvalidAsync(
                response,
                System.Text.Encoding.UTF8.GetBytes("{\"errcode\":40001,\"errmsg\":\"invalid credential\"}"),
                CancellationToken.None))
            .Should().BeTrue("40001 在业务端点表达「access_token 无效 / 非最新」⇒ 必须触发恢复链路");

        (await detector.IsTokenInvalidAsync(
                response,
                System.Text.Encoding.UTF8.GetBytes("{\"errcode\":42007}"),
                CancellationToken.None))
            .Should().BeFalse("42007 是企微失效码，不属公众号集合（各产品线集合独立）");

        (await detector.IsTokenInvalidAsync(response, null, CancellationToken.None))
            .Should().BeFalse("未捕获响应体时判定退化为「未失效」，不得误判");
    }

    /// <summary>
    /// A16：令牌类型常量唯一（不得为普通/稳定通道各设查找键），且与企微路由键字面量区隔。
    /// </summary>
    /// <remarks>
    /// 产品线不得互相引用（此处以字面量锁定企微键值 "Wechat.AccessToken"，避免测试引入跨产品线依赖）。
    /// </remarks>
    [Fact]
    public void TokenTypeKey_ShouldBeSingle()
    {
        MpTokenTypes.AccessToken.Should().Be("Wechat.Mp.AccessToken");
        MpTokenTypes.AccessToken.Should().NotBe("Wechat.AccessToken",
            "公众号与企微不得共用同一令牌路由键（共享 ITokenManagerRegistry 会串号）");
    }
}
