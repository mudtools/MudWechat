// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Callback.Events;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 分发器测试（v1 方案 §5.4.2/§5.4.3 / §8）：精确匹配、appKey 专属先于全局、兜底语义、
/// unhandled 告警、拦截器 Before 中断（→ Interrupted → 503）与 After 调用、
/// 单处理器异常隔离、软超时传播（→ 503）。
/// </summary>
public class WechatCallbackDispatcherTests
{
    /// <summary>跨测试共享的执行顺序记录（静态：处理器实例为 Transient，经静态列表聚合断言）。</summary>
    private static readonly List<string> ExecutionOrder = new();

    private static void Record(string marker)
    {
        lock (ExecutionOrder)
        {
            ExecutionOrder.Add(marker);
        }
    }

    private static void Reset() => ExecutionOrder.Clear();

    private static WechatCallbackOptions CreateOptions(int timeoutMs = 4_500, int maxConcurrent = 10) => new()
    {
        EventHandlingTimeoutMs = timeoutMs,
        MaxConcurrentEvents = maxConcurrent,
        Apps =
        {
            [WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions
            {
                PushToken = "token",
                PushEncodingAESKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq",
            },
        },
    };

    /// <summary>经真实 AddWechatCallback + 建造者装配分发器（与生产同路径）。</summary>
    private static WechatCallbackDispatcher CreateDispatcher(
        WechatCallbackOptions? options = null,
        Action<WechatCallbackServiceBuilder>? configure = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        // 兜底处理器（内置）依赖的仓储：注册内存实现避免解析降级噪音。
        services.AddSingleton<IWechatSuiteTicketStore, InMemoryWechatSuiteTicketStore>();
        services.AddSingleton<IWechatCorpAuthStore, InMemoryWechatCorpAuthStore>();

        var builder = services.AddWechatCallback(_ => { });
        options ??= CreateOptions();
        services.Configure<WechatCallbackOptions>(o =>
        {
            o.EventHandlingTimeoutMs = options.EventHandlingTimeoutMs;
            o.MaxConcurrentEvents = options.MaxConcurrentEvents;
            o.Apps = options.Apps;
        });
        configure?.Invoke(builder);

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<WechatCallbackDispatcher>();
    }

    private static WechatCallbackEvent UserCreatedEvent(string userId = "zhangsan")
        => new()
        {
            Event = WechatCallbackEventTypes.ChangeContact,
            ChangeType = WechatCallbackEventTypes.CreateUser,
            DecryptedXml = $"<xml><UserID>{userId}</UserID></xml>",
        };

    private static WechatCallbackEvent AuthorizationEvent(string infoType = WechatCallbackEventTypes.CreateAuth)
        => new()
        {
            InfoType = infoType,
            DecryptedXml = "<xml><InfoType>create_auth</InfoType></xml>",
        };

    private static WechatCallbackEvent ChainChangeEvent(string changeType = WechatCallbackEventTypes.CorpJoin)
        => new()
        {
            Event = WechatCallbackEventTypes.ChangeChain,
            ChangeType = changeType,
            DecryptedXml = "<xml><Event>change_chain</Event></xml>",
        };

    /// <summary>构造指定「应用类型 × 回调通道」的回调配置（appKey = app1，走精确键命中）。</summary>
    private static WechatCallbackOptions CreateTypeChannelOptions(
        WechatAppType appType, WechatCallbackChannel channel, int timeoutMs = 4_500, int maxConcurrent = 10)
        => new()
        {
            EventHandlingTimeoutMs = timeoutMs,
            MaxConcurrentEvents = maxConcurrent,
            Apps =
            {
                ["app1"] = new WechatAppCallbackOptions
                {
                    PushToken = "token",
                    PushEncodingAESKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq",
                    AppType = appType,
                    Channel = channel,
                },
            },
        };

    // ---------------------------------------------------------------- 匹配语义

    [Fact]
    public async Task DispatchAsync_ShouldExecuteExactMatchedHandler()
    {
        Reset();
        var dispatcher = CreateDispatcher(configure: b => b.AddHandler<CreateUserHandler>());

        var outcome = await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        outcome.Should().Be(WechatCallbackDispatchOutcome.Handled);
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Equal(new[] { "create-user" }, "精确键处理器执行");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldRunAppScopedBeforeGlobal_WhenBothExactMatched()
    {
        Reset();
        var dispatcher = CreateDispatcher(configure: b => b
            .AddHandler<GlobalCreateUserHandler>()
            .AddHandler<AppCreateUserHandler>("app1"));

        await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Equal(new[] { "app1:create-user", "global:create-user" },
                "appKey 专属先于全局（§5.4.2），同组按注册序");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldUseFallback_WhenNoExactMatch()
    {
        Reset();
        var dispatcher = CreateDispatcher(configure: b => b
            .AddHandler<CreateUserHandler>()          // 精确键：create_user
            .AddHandler<FallbackHandler>());          // 空键兜底

        var evt = UserCreatedEvent();
        evt.ChangeType = WechatCallbackEventTypes.DeleteParty; // 无精确处理器的事件

        var outcome = await dispatcher.DispatchAsync("app1", evt);

        outcome.Should().Be(WechatCallbackDispatchOutcome.Handled);
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Equal(new[] { "fallback" }, "无精确命中时才落兜底（§5.4.2）");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldPreferExact_OverFallback()
    {
        Reset();
        var dispatcher = CreateDispatcher(configure: b => b
            .AddHandler<CreateUserHandler>()
            .AddHandler<FallbackHandler>());

        await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Equal(new[] { "create-user" }, "精确命中时兜底不执行");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldReturnUnhandled_WhenNoHandlerResolvable()
    {
        Reset();
        // 内置兜底处理器经 AddWechatCallback 默认注册（生产路径恒有兜底）；
        // 本用例以空注册表直构分发器，锁定 Unhandled 分支（无兜底形态，如宿主裸装配）。
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(new WechatCallbackHandlerRegistry());
        services.AddSingleton(new WechatCallbackInterceptorRegistry());
        services.AddSingleton<IOptionsMonitor<WechatCallbackOptions>>(
            new TestOptionsMonitor<WechatCallbackOptions>(CreateOptions()));
        using var provider = services.BuildServiceProvider();

        // 载荷体系依赖：空契约注册表（本用例无载荷处理器） + 真实读取器。
        // 注册官方契约表并非本用例关注点，故留空 ⇒ 键未登记时读取器走 GenericFallback。
        var payloadRegistry = new WechatPayloadContractRegistry();
        var dispatcher = new WechatCallbackDispatcher(
            provider.GetRequiredService<WechatCallbackHandlerRegistry>(),
            provider.GetRequiredService<WechatCallbackInterceptorRegistry>(),
            payloadRegistry,
            new WechatCallbackPayloadReader(payloadRegistry),
            provider.GetRequiredService<IOptionsMonitor<WechatCallbackOptions>>(),
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<WechatCallbackDispatcher>.Instance);

        var outcome = await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        outcome.Should().Be(WechatCallbackDispatchOutcome.Unhandled, "无任何处理器 → unhandled 告警（仍 200）");
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().BeEmpty();
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldEnumerateWildcardOnce_ForWildcardApp()
    {
        Reset();
        // appKey = "*"（通讯录同步助手）：全局桶不得被重复枚举执行两次（D11）。
        var dispatcher = CreateDispatcher(configure: b => b.AddHandler<CreateUserHandler>());

        await dispatcher.DispatchAsync(WechatCallbackOptions.WildcardAppKey, UserCreatedEvent());

        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Equal(new[] { "create-user" }, "通配应用事件只执行一次全局处理器");
        }
    }

    // ---------------------------------------------------------------- 事件族合法性闸（v1.2 D9）

    [Fact]
    public async Task DispatchAsync_ShouldReject_WhenAuthorizationFamilyDeliveredToInternalApp()
    {
        Reset();
        var dispatcher = CreateDispatcher(
            options: CreateTypeChannelOptions(WechatAppType.Internal, WechatCallbackChannel.App),
            configure: b => b.AddHandler<FallbackHandler>());

        var outcome = await dispatcher.DispatchAsync("app1", AuthorizationEvent());

        outcome.Should().Be(WechatCallbackDispatchOutcome.Rejected,
            "授权族仅套件通道，自建应用数据回调收到授权族即拒绝（200 不重推）");
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().BeEmpty("合法性闸先于处理器，兜底处理器不得执行");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldReject_WhenChainChangeDeliveredToThirdPartyApp()
    {
        Reset();
        var dispatcher = CreateDispatcher(
            options: CreateTypeChannelOptions(WechatAppType.ThirdParty, WechatCallbackChannel.App),
            configure: b => b.AddHandler<FallbackHandler>());

        var outcome = await dispatcher.DispatchAsync("app1", ChainChangeEvent());

        outcome.Should().Be(WechatCallbackDispatchOutcome.Rejected,
            "上下游变更族官方仅向自建应用开放，第三方应用数据回调收到即拒绝");
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().BeEmpty("合法性闸先于处理器，兜底处理器不得执行");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldAllow_WhenAuthorizationFamilyDeliveredToSuiteChannel()
    {
        Reset();
        var dispatcher = CreateDispatcher(
            options: CreateTypeChannelOptions(WechatAppType.ThirdParty, WechatCallbackChannel.Suite),
            configure: b => b.AddHandler<FallbackHandler>());

        var outcome = await dispatcher.DispatchAsync("app1", AuthorizationEvent());

        outcome.Should().Be(WechatCallbackDispatchOutcome.Handled,
            "授权族在第三方套件通道合法，正常分发");
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Contain("fallback", "合法事件族落兜底处理器");
        }
    }

    // ---------------------------------------------------------------- 拦截器

    [Fact]
    public async Task DispatchAsync_ShouldInterrupt_AndSkipHandlers_WhenBeforeReturnsFalse()
    {
        Reset();
        var dispatcher = CreateDispatcher(configure: b => b
            .AddHandler<CreateUserHandler>()
            .AddInterceptor<InterruptingInterceptor>());

        var outcome = await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        outcome.Should().Be(WechatCallbackDispatchOutcome.Interrupted, "Before 返回 false → 中断 → 中间件 503");
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().BeEmpty("中断后处理器不执行");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldCallAfter_WhenHandled()
    {
        Reset();
        var dispatcher = CreateDispatcher(configure: b => b
            .AddHandler<CreateUserHandler>()
            .AddInterceptor<AuditingInterceptor>());

        await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Equal(new[] { "create-user", "after:create_user" },
                "处理器执行后调用 After 拦截器");
        }
    }

    // ---------------------------------------------------------------- 异常与超时

    [Fact]
    public async Task DispatchAsync_ShouldIsolateHandlerException_AndContinueRemaining()
    {
        Reset();
        var dispatcher = CreateDispatcher(configure: b => b
            .AddHandler<ThrowingHandler>()
            .AddHandler<CreateUserHandler>());

        var outcome = await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        outcome.Should().Be(WechatCallbackDispatchOutcome.Handled,
            "单处理器异常被隔离（指纹已消费，503 重推无效，故障经日志暴露）");
        lock (ExecutionOrder)
        {
            ExecutionOrder.Should().Equal(new[] { "throwing", "create-user" }, "异常后继续其余处理器");
        }
    }

    [Fact]
    public async Task DispatchAsync_ShouldThrowOperationCanceled_WhenSoftTimeoutElapsed()
    {
        Reset();
        var dispatcher = CreateDispatcher(
            options: CreateOptions(timeoutMs: 50),
            configure: b => b.AddHandler<SlowHandler>());

        var act = async () => await dispatcher.DispatchAsync("app1", UserCreatedEvent());

        await act.Should().ThrowAsync<OperationCanceledException>(
            "软超时以 OCE 传播 → 中间件 503 触发企业微信重推");
    }

    // ---------------------------------------------------------------- 测试替身

    /// <summary>精确键处理器（create_user）。</summary>
    private sealed class CreateUserHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => WechatCallbackEventTypes.CreateUser;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
        {
            Record("create-user");
            return Task.CompletedTask;
        }
    }

    /// <summary>全局精确键处理器（create_user，注册于通配键）。</summary>
    private sealed class GlobalCreateUserHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => WechatCallbackEventTypes.CreateUser;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
        {
            Record("global:create-user");
            return Task.CompletedTask;
        }
    }

    /// <summary>appKey 专属精确键处理器（create_user，注册于 app1）。</summary>
    private sealed class AppCreateUserHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => WechatCallbackEventTypes.CreateUser;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
        {
            Record("app1:create-user");
            return Task.CompletedTask;
        }
    }

    /// <summary>兜底处理器（空键）。</summary>
    private sealed class FallbackHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => string.Empty;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
        {
            Record("fallback");
            return Task.CompletedTask;
        }
    }

    /// <summary>抛异常处理器（异常隔离用）。</summary>
    private sealed class ThrowingHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => WechatCallbackEventTypes.CreateUser;

        public Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
        {
            Record("throwing");
            throw new InvalidOperationException("handler boom");
        }
    }

    /// <summary>慢处理器（尊重取消令牌，软超时用）。</summary>
    private sealed class SlowHandler : IWechatCallbackEventHandler
    {
        public string SupportedEventType => WechatCallbackEventTypes.CreateUser;

        public async Task HandleAsync(WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => await Task.Delay(Timeout.Infinite, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>中断拦截器（Before false）。</summary>
    private sealed class InterruptingInterceptor : IWechatCallbackEventInterceptor
    {
        public Task<bool> BeforeHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task AfterHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    /// <summary>审计拦截器（After 记序）。</summary>
    private sealed class AuditingInterceptor : IWechatCallbackEventInterceptor
    {
        public Task<bool> BeforeHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
            => Task.FromResult(true);

        public Task AfterHandleAsync(string eventType, WechatCallbackEvent eventData, CancellationToken cancellationToken = default)
        {
            Record($"after:{eventType}");
            return Task.CompletedTask;
        }
    }
}
