using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Moq;
using Mud.AI.Abstractions.Conversations;
using Mud.AI.Agent.Events;
using Mud.AI.Agent.Sessions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Configuration;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Agents;

/// <summary>
/// 回合闭环：单轮生成 → 回复下发 → 会话落库；以及 outbox 双状态键的「成对 / 半配 ⇒ 整体禁用」语义。
/// </summary>
/// <remarks>
/// 回归点：outbox 两个状态键只配一半时，中立管线在首轮生成后写 outbox 会抛
/// <see cref="ArgumentNullException"/>，回合失败且不落库（用户收不到回复）。
/// 约束在两处收口：配置校验（<see cref="WechatAgentOptionsValidator"/>）拒绝半配；
/// <see cref="WechatAgent"/> 构造时再按「两键皆非空才启用」兜底，故直接传 <see cref="IOptions{T}"/>
/// 绕过校验器的宿主同样不会在运行期炸掉。
/// </remarks>
public class WechatAgentTurnTests
{
    [Fact]
    public async Task RunTurnAsync_WithOutboxEnabled_ShouldDeliverReplyAndPersistHistory()
    {
        var store = new MemorySessionStore();
        var sink = new Mock<IConversationReplySink>(MockBehavior.Loose);
        var agent = CreateAgent(new WechatAgentOptions(), store);

        var outcome = await agent.RunTurnAsync(Request(), sink.Object);

        outcome.ResponseText.Should().Be("pong");
        sink.Verify(
            s => s.DeliverAsync(It.IsAny<ConversationRequest>(), "pong", It.IsAny<CancellationToken>()),
            Times.Once);
        (await store.GetAsync("wechat:corp-app:g:chat-1")).Should().NotBeNull(
            "回合成功后会话历史必须落库（否则下一轮丢上下文）");
    }

    [Theory]
    [InlineData("", WechatAgentOptions.DefaultPendingReplyTurnStateKey)]
    [InlineData(WechatAgentOptions.DefaultPendingReplyStateKey, "")]
    public async Task RunTurnAsync_WithHalfConfiguredOutboxKeys_ShouldDisableOutboxAndComplete(
        string stateKey,
        string turnKey)
    {
        var sink = new Mock<IConversationReplySink>(MockBehavior.Loose);
        var agent = CreateAgent(
            new WechatAgentOptions
            {
                PendingReplyStateKey = stateKey,
                PendingReplyTurnStateKey = turnKey,
            },
            new MemorySessionStore());

        var act = async () => await agent.RunTurnAsync(Request(), sink.Object);

        var outcome = await act.Should().NotThrowAsync("半配 outbox 只能是「未启用」，不得让回合在生成后失败");
        outcome.Subject.ResponseText.Should().Be("pong");
        sink.Verify(
            s => s.DeliverAsync(It.IsAny<ConversationRequest>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RunTurnAsync_WithOutboxDisabled_ShouldComplete()
    {
        var sink = new Mock<IConversationReplySink>(MockBehavior.Loose);
        var agent = CreateAgent(
            new WechatAgentOptions
            {
                PendingReplyStateKey = string.Empty,
                PendingReplyTurnStateKey = string.Empty,
            },
            new MemorySessionStore());

        var outcome = await agent.RunTurnAsync(Request(), sink.Object);

        outcome.ResponseText.Should().Be("pong");
    }

    private static WechatAgent CreateAgent(WechatAgentOptions options, ISessionStore store)
    {
        var chatClient = new Mock<IChatClient>(MockBehavior.Loose);
        chatClient
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatResponse(new ChatMessage(ChatRole.Assistant, "pong")));

        var switcher = new Mock<IWechatAppContextSwitcher>(MockBehavior.Loose);
        switcher.Setup(s => s.UseAppScope(It.IsAny<string>())).Returns(new FakeScope());
        switcher.Setup(s => s.UseDefaultAppScope()).Returns(new FakeScope());

        return new WechatAgent(
            chatClient.Object,
            Options.Create(options),
            store,
            telemetry: null,
            scopeFactory: new WechatWorkAiScopeFactory(switcher.Object, new WechatAiCorpContextAccessor()));
    }

    private static ConversationRequest Request()
        => new("corp-app", ConversationScope.Group(), "chat-1", "u1", "m1", "hi", "chat-1");

    private sealed class FakeScope : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
