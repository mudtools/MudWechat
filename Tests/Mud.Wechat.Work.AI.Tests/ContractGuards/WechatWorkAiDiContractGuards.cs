using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Mud.AI.Abstractions.Approvals;
using Mud.AI.Abstractions.Configuration;
using Mud.AI.Abstractions.Conversations;
using Mud.AI.Abstractions.Observability;
using Mud.AI.Agent.Events;
using Mud.AI.Agent.Sessions;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Approvals;
using Mud.Wechat.Work.AI.Configuration;
using Mud.Wechat.Work.AI.Diagnostics;
using Mud.Wechat.Work.AI.Events;
using Mud.Wechat.Work.AI.Extensions;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.ContractGuards;

/// <summary>
/// 装配守卫：<c>AddWechatWorkAgent()</c> 的注册语义（先注册者生效 / 单例跨 scope / 缺模型客户端 fail-fast /
/// 配置节绑定）与「AI 域不反向依赖 .AI.Tools / .AI.Mcp」。
/// </summary>
public class WechatWorkAiDiContractGuards
{
    [Fact]
    public void AddWechatWorkAgent_ShouldResolveAgentAsSingletonAcrossScopes()
    {
        using var provider = BuildProvider();

        WechatAgent first;
        WechatAgent second;
        using (var scope = provider.CreateScope())
        {
            first = scope.ServiceProvider.GetRequiredService<WechatAgent>();
            scope.ServiceProvider.GetRequiredService<WechatBotAgentHandler>().SupportedEventType
                .Should().Be("text");
        }

        using (var scope = provider.CreateScope())
        {
            second = scope.ServiceProvider.GetRequiredService<WechatAgent>();
        }

        first.Should().BeSameAs(second, "Agent 无请求级状态（租户维度经作用域端口隔离），必须单例");
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldFailFastWhenChatClientMissing()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<IWechatAppContextSwitcher>());
        services.AddSingleton(Mock.Of<IWechatWorkInternalAibotService>());
        services.AddWechatWorkAgent();

        using var provider = services.BuildServiceProvider();

        var act = () => provider.GetRequiredService<WechatAgent>();

        act.Should().Throw<InvalidOperationException>().WithMessage("*IChatClient*",
            "缺模型客户端时错误必须可操作（点名 AddChatClient 用法）");
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldRespectHostSessionStoreRegisteredFirst()
    {
        var hostStore = Mock.Of<ISessionStore>();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(NewChatClient());
        services.AddSingleton(Mock.Of<IWechatAppContextSwitcher>());
        services.AddSingleton(Mock.Of<IWechatWorkInternalAibotService>());
        services.AddSingleton(hostStore);
        services.AddWechatWorkAgent();

        using var provider = services.BuildServiceProvider(ValidateScopesOption());

        provider.GetRequiredService<ISessionStore>().Should().BeSameAs(hostStore,
            "先注册者生效（TryAdd 语义）：分布式部署由宿主先注册 Redis 实现覆盖默认内存存储");
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldRegisterMemoryStoreWithProductTtl()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(NewChatClient());
        services.AddSingleton(Mock.Of<IWechatAppContextSwitcher>());
        services.AddSingleton(Mock.Of<IWechatWorkInternalAibotService>());
        services.AddWechatWorkAgent(o => o.SessionTtlSeconds = 60);

        using var provider = services.BuildServiceProvider(ValidateScopesOption());

        provider.GetRequiredService<ISessionStore>().Should().BeOfType<MemorySessionStore>(
            "默认件是内存存储，TTL 由产品配置单一来源注入（不得回落到中立骨架的默认 TTL）");
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldBindConfigurationSection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["WechatAi:Instructions"] = "自定义指令",
                ["WechatAi:SessionTtlSeconds"] = "120",
                ["WechatAi:MaxHistoryMessages"] = "20",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(NewChatClient());
        services.AddSingleton(Mock.Of<IWechatAppContextSwitcher>());
        services.AddSingleton(Mock.Of<IWechatWorkInternalAibotService>());
        services.AddWechatWorkAgent(configuration);

        using var provider = services.BuildServiceProvider(ValidateScopesOption());
        var options = provider.GetRequiredService<IOptions<WechatAgentOptions>>().Value;

        options.Instructions.Should().Be("自定义指令");
        options.SessionTtlSeconds.Should().Be(120);
        options.MaxHistoryMessages.Should().Be(20);
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldRegisterNeutralPortsOnce()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(NewChatClient());
        services.AddSingleton(Mock.Of<IWechatAppContextSwitcher>());
        services.AddSingleton(Mock.Of<IWechatWorkInternalAibotService>());
        services.AddWechatWorkAgent();

        using var provider = services.BuildServiceProvider(ValidateScopesOption());

        provider.GetRequiredService<ITurnScopeFactory>().Should().BeOfType<WechatWorkAiScopeFactory>();
        provider.GetServices<ITurnScopeFactory>().Should().ContainSingle("第二个作用域槽位会让宿主不知道该初始化哪个");
        provider.GetServices<IWechatAgentTurnScheduler>().Should().ContainSingle();
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldRegisterProductTelemetryInsteadOfNoop()
    {
        using var provider = BuildProvider();

        // 装配顺序是硬约束：AddAgentRuntimeCore 用 TryAdd 装 NoopAgentTelemetry，
        // 遥测实现若在它之<后>注册就会被「先注册者生效」静默丢弃——运行期一切正常，只是没有任何数据。
        provider.GetRequiredService<IAgentTelemetry>()
            .Should().BeOfType<WechatAgentTelemetry>("遥测实现必须先于中立骨架注册；退化为 Noop 时不会报错，只会全黑盒");
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldRegisterHitlAndGatePorts()
    {
        using var provider = BuildProvider();

        provider.GetRequiredService<WechatTurnGate>().Should().NotBeNull(
            "同会话串行靠它；缺失时同一会话的两条消息会并发跑两个回合（历史互相覆盖）");
        provider.GetRequiredService<IToolApprovalChannel>().Should().BeOfType<WechatToolApprovalChannel>();
        provider.GetRequiredService<IPendingApprovalStore>().Should().NotBeNull(
            "未装快照存储 ⇒ 进程重启后待确认项彻底消失（宿主无从列出待办）");
        provider.GetRequiredService<WechatToolApprovalResume>().Should().NotBeNull();
    }

    [Fact]
    public void AddWechatWorkAgent_ShouldProjectGateKnobsIntoNeutralRuntimeOptions()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(NewChatClient());
        services.AddSingleton(Mock.Of<IWechatAppContextSwitcher>());
        services.AddSingleton(Mock.Of<IWechatWorkInternalAibotService>());
        services.AddWechatWorkAgent(o =>
        {
            o.MaxConcurrentTurns = 7;
            o.TurnQueueTimeoutSeconds = 5;
        });

        using var provider = services.BuildServiceProvider(ValidateScopesOption());
        var runtime = provider.GetRequiredService<AgentRuntimeOptions>();

        runtime.MaxConcurrentTurns.Should().Be(7, "产品旋钮必须投影进中立运行期，否则宿主调了也不生效（阈值双来源）");
        runtime.TurnQueueTimeoutSeconds.Should().Be(5);
    }

    /// <summary>ValidateScopes 打开：单例不得捕获 Scoped 依赖（AGENTS.md §7 陷阱）。</summary>
    private static ServiceProviderOptions ValidateScopesOption()
        => new() { ValidateScopes = true, ValidateOnBuild = true };

    private static ServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(NewChatClient());
        services.AddSingleton(Mock.Of<IWechatAppContextSwitcher>());
        services.AddSingleton(Mock.Of<IWechatWorkInternalAibotService>());
        services.AddWechatWorkAgent();
        return services.BuildServiceProvider(ValidateScopesOption());
    }

    private static IChatClient NewChatClient()
    {
        var mock = new Mock<IChatClient>(MockBehavior.Loose);
        mock.Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatResponse(new ChatMessage(ChatRole.Assistant, "ok")));
        return mock.Object;
    }
}
