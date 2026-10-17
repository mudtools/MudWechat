using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Moq;
using Mud.AI.Agent.Sessions;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Channels;
using Mud.Wechat.Work.AI.Configuration;
using Mud.Wechat.Work.AI.Events;
using Mud.Wechat.Work.DataModels.Aibot;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Events;

/// <summary>
/// 最小闭环（S1 端到端）：Bot 文本回调 → 会话事实 → 中立回合 → 主动回复写回；
/// 以及「空包应答 / 软超时形态 / 多轮会话复用 / 缺凭据 fail-closed」。
/// </summary>
public class WechatBotAgentHandlerTests
{
    private readonly List<IReadOnlyList<ChatMessage>> _capturedRequests = new();
    private readonly List<(string Code, AibotMessage Message)> _sent = new();
    private readonly Mock<IChatClient> _chatClient = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkInternalAibotService> _aibot = new(MockBehavior.Loose);
    private readonly Mock<IWechatAppContextSwitcher> _switcher = new(MockBehavior.Loose);
    private readonly WechatAiCorpContextAccessor _corpContext = new();

    public WechatBotAgentHandlerTests()
    {
        _chatClient
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<ChatMessage> messages, ChatOptions? _, CancellationToken _) =>
            {
                _capturedRequests.Add(messages.ToList());
                return new ChatResponse(new ChatMessage(ChatRole.Assistant, $"回复#{_capturedRequests.Count}"));
            });

        _aibot
            .Setup(a => a.ReplyAsync(It.IsAny<string>(), It.IsAny<AibotMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string code, AibotMessage message, CancellationToken _) =>
            {
                _sent.Add((code, message));
                return new AibotReplyResponse { ErrorCode = 0 };
            });

        _switcher.Setup(s => s.UseAppScope(It.IsAny<string>())).Returns(new FakeScope());
        _switcher.Setup(s => s.UseDefaultAppScope()).Returns(new FakeScope());
    }

    [Fact]
    public async Task HandleAsync_TextMessage_ShouldReturnEmptyPackageAndReplyActively()
    {
        var handler = CreateHandler();

        var reply = await handler.HandleAsync(TextEvent("你好"));

        reply.Should().BeNull("软超时（4500ms）约束下必须先回空包，回复经 response_url 主动写回");

        _capturedRequests.Should().ContainSingle();
        _sent.Should().ContainSingle();
        _sent[0].Code.Should().Be("code-1");
        _sent[0].Message.MsgType.Should().Be(WechatBotReplyTypes.Markdown);
        _sent[0].Message.Markdown!.Content.Should().Be("回复#1");
    }

    [Fact]
    public async Task HandleAsync_SecondMessageInSameChat_ShouldReuseSessionHistory()
    {
        var handler = CreateHandler();

        await handler.HandleAsync(TextEvent("第一句"));
        await handler.HandleAsync(TextEvent("第二句", msgId: "msg-2"));

        _capturedRequests.Should().HaveCount(2);
        _capturedRequests[1].Should().Contain(
            m => m.Role == ChatRole.Assistant && m.Text == "回复#1",
            "同一会话的第二次回合必须带上历史（会话门面按 appKey + 群/单聊 + 主体键复用）");
    }

    [Fact]
    public async Task HandleAsync_WithoutResponseUrl_ShouldNotRunTurn()
    {
        var handler = CreateHandler();

        var reply = await handler.HandleAsync(TextEvent("你好", responseUrl: null));

        reply.Should().BeNull();
        _capturedRequests.Should().BeEmpty("无一次性凭据 ⇒ 无法主动回复，回合不得空跑（fail-closed）");
    }

    [Fact]
    public async Task HandleAsync_BlankText_ShouldNotRunTurn()
    {
        var handler = CreateHandler();
        var evt = TextEvent("   ");

        var reply = await handler.HandleAsync(evt);

        reply.Should().BeNull();
        _capturedRequests.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_SingleChat_ShouldUseUserSubject()
    {
        var handler = CreateHandler();
        var sw = new System.Diagnostics.Stopwatch();

        await handler.HandleAsync(TextEvent("单聊消息", chatType: "single", chatId: null));

        _capturedRequests.Should().ContainSingle();
        _sent.Should().ContainSingle("单聊会话同样走主动回复通道");
    }

    [Fact]
    public async Task HandleAsync_ShouldDeclareExactTextEventKey()
    {
        CreateHandler().SupportedEventType.Should().Be(WechatBotEventTypes.Text,
            "精确匹配 text：兜底注册会吞掉 feedback_event「仅空包」等官方硬约束");
    }

    [Fact]
    public void Context_ShouldBeClearedAfterScheduling()
    {
        var handler = CreateHandler();

        _ = handler.HandleAsync(TextEvent("你好"));

        _corpContext.Current.Should().BeNull("事件层写入的身份上下文必须在排入后台回合后复位（同请求后续处理器不得读到残留）");
    }

    [Fact]
    public async Task RunTurn_WhenModelFails_ShouldNotThrow()
    {
        _chatClient
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("upstream down"));
        var handler = CreateHandler();

        var act = async () => await handler.HandleAsync(TextEvent("你好"));

        await act.Should().NotThrowAsync("回合异常在后台收口（已应答的回调不受影响），只记录日志");
        _sent.Should().BeEmpty();
    }

    private WechatBotAgentHandler CreateHandler()
    {
        var options = Options.Create(new WechatAgentOptions());
        var agent = new WechatAgent(
            _chatClient.Object,
            options,
            new MemorySessionStore(),
            telemetry: null,
            scopeFactory: new WechatWorkAiScopeFactory(_switcher.Object, _corpContext));

        return new WechatBotAgentHandler(
            agent,
            _aibot.Object,
            _corpContext,
            new InlineScheduler());
    }

    private static WechatBotCallbackEvent TextEvent(
        string text,
        string msgId = "msg-1",
        string? chatType = "group",
        string? chatId = "chat-1",
        string? responseUrl = "https://qyapi.weixin.qq.com/cgi-bin/aibot/response?response_code=code-1")
        => new()
        {
            AppKey = "corp-app",
            MsgId = msgId,
            ChatType = chatType,
            ChatId = chatId,
            FromUserId = "zhangsan",
            MsgType = "text",
            ResponseUrl = responseUrl,
            Message = new AibotMessageCallback
            {
                MsgId = msgId,
                ChatType = chatType,
                ChatId = chatId,
                MsgType = "text",
                Text = new AibotTextBody { Content = text },
            },
        };

    /// <summary>内联调度器：测试里同步跑完后台回合，使断言确定。</summary>
    private sealed class InlineScheduler : IWechatAgentTurnScheduler
    {
        public async Task ScheduleAsync(Func<Task> turn) => await turn();
    }

    private sealed class FakeScope : IDisposable
    {
        public void Dispose()
        {
        }
    }
}
