using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Mud.AI.Abstractions.Conversations;
using Mud.AI.Agent.Events;
using Mud.AI.Agent.Sessions;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Configuration;
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
