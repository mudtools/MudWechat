// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调 DI 装配测试（P0-2 + v1 方案 §5.8）：抗重放守卫可解析、跨 scope 同一实例、宿主可前置覆盖为分布式实现；
/// v1.2 追加注册表/分发器/中间件装配、智能机器人接收面装配与两侧建造者注册行为。
/// </summary>
public class WechatCallbackServiceCollectionExtensionsTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";

    private static ServiceCollection CreateServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatCallback(options =>
        {
            options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions
            {
                PushToken = "push-token",
                PushEncodingAESKey = AesKey,
            };
        });

        return services;
    }

    [Fact]
    public void ReplayGuard_ShouldBeResolvableAndSingleton()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        var guard = provider.GetRequiredService<IWechatCallbackReplayGuard>();
        guard.Should().BeOfType<InMemoryWechatCallbackReplayGuard>();

        using var scope = provider.CreateScope();
        ReferenceEquals(guard, scope.ServiceProvider.GetRequiredService<IWechatCallbackReplayGuard>())
            .Should().BeTrue("重放守卫必须跨 scope 同一实例，否则去重窗口形同虚设");
    }

    [Fact]
    public void CallbackPipeline_ShouldBeResolvableAsSingletons()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider();

        var receiver = provider.GetRequiredService<IWechatCallbackReceiver>();
        var dispatcher = provider.GetRequiredService<WechatCallbackDispatcher>();
        var handlers = provider.GetRequiredService<WechatCallbackHandlerRegistry>();
        var interceptors = provider.GetRequiredService<WechatCallbackInterceptorRegistry>();

        using var scope = provider.CreateScope();
        ReferenceEquals(receiver, scope.ServiceProvider.GetRequiredService<IWechatCallbackReceiver>()).Should().BeTrue();
        ReferenceEquals(dispatcher, scope.ServiceProvider.GetRequiredService<WechatCallbackDispatcher>())
            .Should().BeTrue("分发器 Singleton（并发信号量跨请求共享）");
        ReferenceEquals(handlers, scope.ServiceProvider.GetRequiredService<WechatCallbackHandlerRegistry>())
            .Should().BeTrue("注册表为组合根期创建的单例实例（D11）");
        interceptors.Should().NotBeNull();
    }

    [Fact]
    public void AddWechatCallback_ShouldRegisterBuiltinFallbackHandler_ToWildcardBucket()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider();

        var registry = provider.GetRequiredService<WechatCallbackHandlerRegistry>();
        registry.GetAll(WechatCallbackOptions.WildcardAppKey)
            .Should().Contain(typeof(WechatCallbackHandler), "D6/D11：内置授权族兜底处理器默认注册到通配键");
    }

    [Fact]
    public void Builder_ShouldRegisterHandlerAndInterceptor()
    {
        var services = CreateServices();
        services.AddWechatCallback(_ => { })
            .AddHandler<StubEventHandler>()
            .AddHandler<AppScopedEventHandler>("app1")
            .AddInterceptor<StubInterceptor>();

        using var provider = services.BuildServiceProvider();
        var handlers = provider.GetRequiredService<WechatCallbackHandlerRegistry>();
        var interceptors = provider.GetRequiredService<WechatCallbackInterceptorRegistry>();

        handlers.GetAll(WechatCallbackOptions.WildcardAppKey)
            .Should().Contain(typeof(StubEventHandler), "无 appKey 重载注册到通配键（全局生效，D11）");
        handlers.GetAll("app1").Should().Contain(typeof(AppScopedEventHandler), "显式 appKey 注册到应用专属桶");
        interceptors.GetAll(WechatCallbackOptions.WildcardAppKey).Should().Contain(typeof(StubInterceptor));
    }

    [Fact]
    public void ReplayGuard_ShouldBeOverridableByHost_WhenRegisteredFirst()
    {
        // TryAdd 前置注册语义：宿主的分布式实现必须先于 AddWechatCallback 注册。
        var services = new ServiceCollection();
        services.AddLogging();
        var distributed = new Mock<IWechatCallbackReplayGuard>();
        services.AddSingleton(distributed.Object);

        services.AddWechatCallback(options =>
        {
            options.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions
            {
                PushToken = "push-token",
                PushEncodingAESKey = AesKey,
            };
        });

        using var provider = services.BuildServiceProvider();

        ReferenceEquals(provider.GetRequiredService<IWechatCallbackReplayGuard>(), distributed.Object)
            .Should().BeTrue("宿主注册的分布式实现优先（多实例部署必需）");
    }

    /// <summary>
    /// v1.2：<c>AddWechatBotCallback</c> 的前置依赖契约——未先调 <c>AddWechatCallback</c> 即 fail-fast。
    /// 处理器注册表实例在 <c>AddWechatCallback</c> 核心装配中创建，静默空转会让宿主注册的机器人处理器永不执行。
    /// </summary>
    [Fact]
    public void AddWechatBotCallback_ShouldFailFast_WhenAddWechatCallbackNotCalled()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var act = () => services.AddWechatBotCallback();

        act.Should().Throw<InvalidOperationException>().WithMessage("*AddWechatCallback*");
    }

    /// <summary>
    /// v1.2：机器人接收面（接收器 / 分发器 / 注册表）在 <c>AddWechatCallback</c> 中<b>无条件注册</b>为单例——
    /// 中间件为经典约定式构造注入，未接线机器人处理器的宿主也须能整体解析（否则整个回调面解析失败）。
    /// </summary>
    [Fact]
    public void BotCallbackPipeline_ShouldBeResolvableAsSingletons()
    {
        var services = CreateServices();
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

        var receiver = provider.GetRequiredService<IWechatBotCallbackReceiver>();
        var dispatcher = provider.GetRequiredService<WechatBotEventDispatcher>();
        var registry = provider.GetRequiredService<WechatBotHandlerRegistry>();

        using var scope = provider.CreateScope();
        ReferenceEquals(receiver, scope.ServiceProvider.GetRequiredService<IWechatBotCallbackReceiver>())
            .Should().BeTrue("接收器 Singleton（无状态，凭据按 appKey 请求期解析）");
        ReferenceEquals(dispatcher, scope.ServiceProvider.GetRequiredService<WechatBotEventDispatcher>())
            .Should().BeTrue("分发器 Singleton（并发信号量跨请求共享）");
        ReferenceEquals(registry, scope.ServiceProvider.GetRequiredService<WechatBotHandlerRegistry>())
            .Should().BeTrue("注册表为组合根期创建的单例实例（AddWechatBotCallback 复用同一实例）");
    }

    /// <summary>v1.2：机器人建造者按 botKey 分桶（未传 botKey ⇒ 通配键全局生效）。</summary>
    [Fact]
    public void BotBuilder_ShouldRegisterHandlerToBotKeyOrWildcardBucket()
    {
        var services = CreateServices();
        services.AddWechatBotCallback()
            .AddHandler<StubBotEventHandler>("bot1")
            .AddHandler<FallbackBotEventHandler>();

        using var provider = services.BuildServiceProvider();
        var registry = provider.GetRequiredService<WechatBotHandlerRegistry>();

        registry.GetAll("bot1").Should().Contain(typeof(StubBotEventHandler));
        registry.GetAll(WechatCallbackOptions.WildcardAppKey)
            .Should().Contain(typeof(FallbackBotEventHandler), "未传 botKey 时注册到通配键（全局生效）");
    }

    /// <summary>测试用精确键处理器（create_user）。</summary>
    private sealed class StubEventHandler : IWechatCallbackEventHandler    {
        public string SupportedEventType => WechatCallbackEventTypes.CreateUser;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    /// <summary>测试用 appKey 专属处理器（batch_job_result）。</summary>
    private sealed class AppScopedEventHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => WechatCallbackEventTypes.BatchJobResult;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    /// <summary>测试用拦截器（全放行）。</summary>
    private sealed class StubInterceptor : IWechatCallbackEventInterceptor
    {
        public Task<bool> BeforeHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task AfterHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    /// <summary>测试用机器人处理器（精确键 text；返回 null = 空包）。</summary>
    private sealed class StubBotEventHandler : IWechatBotCallbackEventHandler
    {
        public string SupportedEventType => WechatBotEventTypes.Text;

        public Task<AibotMessage?> HandleAsync(
            WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.FromResult<AibotMessage?>(null);
    }

    /// <summary>测试用机器人兜底处理器（空键）。</summary>
    private sealed class FallbackBotEventHandler : IWechatBotCallbackEventHandler
    {
        public string SupportedEventType => string.Empty;

        public Task<AibotMessage?> HandleAsync(
            WechatBotCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.FromResult<AibotMessage?>(null);
    }
}
