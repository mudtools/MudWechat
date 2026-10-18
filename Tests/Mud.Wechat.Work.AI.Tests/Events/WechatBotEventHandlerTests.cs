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
/// 智能机器人事件处理器族（<c>WechatBotEventHandlerBase</c> 的三个实现）：
/// 文本族端到端闭环（回调 → 会话事实 → 中立回合 → 主动回复写回）；
/// 混排族「文本拼接 + 图片只计张数 + 纯图片不进回合」；语音族「只用官方转换文本」。
/// </summary>
public class WechatBotEventHandlerTests
{
    private readonly List<IReadOnlyList<ChatMessage>> _capturedRequests = new();
    private readonly List<(string Code, AibotMessage Message)> _sent = new();
    private readonly Mock<IChatClient> _chatClient = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkInternalAibotService> _aibot = new(MockBehavior.Loose);
    private readonly Mock<IWechatAppContextSwitcher> _switcher = new(MockBehavior.Loose);
    private readonly WechatAiCorpContextAccessor _corpContext = new();

    public WechatBotEventHandlerTests()
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

    private WechatBotMixedMessageHandler CreateMixedHandler()
        => new(CreateAgent(), _aibot.Object, _corpContext, new InlineScheduler());

    private WechatBotVoiceMessageHandler CreateVoiceHandler()
        => new(CreateAgent(), _aibot.Object, _corpContext, new InlineScheduler());

    private WechatAgent CreateAgent()
    {
        var options = Options.Create(new WechatAgentOptions());
        return new WechatAgent(
            _chatClient.Object,
            options,
            new MemorySessionStore(),
            telemetry: null,
            scopeFactory: new WechatWorkAiScopeFactory(_switcher.Object, _corpContext));
    }

    [Fact]
    public void MixedHandler_ShouldDeclareExactMixedEventKey()
    {
        CreateMixedHandler().SupportedEventType.Should().Be(WechatBotEventTypes.Mixed);
    }

    [Fact]
    public async Task MixedHandler_TextWithImages_ShouldConcatenateTextAndCountImages()
    {
        var handler = CreateMixedHandler();
        var evt = MixedEvent(
        [
            new AibotMixedItem { MsgType = "text", Text = new AibotTextBody { Content = "第一段" } },
            new AibotMixedItem { MsgType = "image", Image = new AibotMediaContent { Url = "https://example.com/a.jpg", AesKey = "k" } },
            new AibotMixedItem { MsgType = "text", Text = new AibotTextBody { Content = "第二段" } },
            new AibotMixedItem { MsgType = "image", Image = new AibotMediaContent { Url = "https://example.com/b.jpg", AesKey = "k" } },
        ]);

        var reply = await handler.HandleAsync(evt);

        reply.Should().BeNull();
        _capturedRequests.Should().ContainSingle();
        var prompt = _capturedRequests[0].Last().Text;
        prompt.Should().Contain("第一段");
        prompt.Should().Contain("第二段");
        prompt.Should().Contain("2 张图片", "图片降级为张数提示，不进模型上下文");
        prompt.Should().NotContain("example.com", "图片 URL/AES key 是宿主二进制资产，不外发");
        _sent.Should().ContainSingle("混排同样走主动回复通道");
    }

    [Fact]
    public async Task MixedHandler_ImagesOnly_ShouldNotRunTurn()
    {
        var handler = CreateMixedHandler();
        var evt = MixedEvent(
        [
            new AibotMixedItem { MsgType = "image", Image = new AibotMediaContent { Url = "https://example.com/a.jpg" } },
            new AibotMixedItem { MsgType = "image", Image = new AibotMediaContent { Url = "https://example.com/b.jpg" } },
        ]);

        var reply = await handler.HandleAsync(evt);

        reply.Should().BeNull();
        _capturedRequests.Should().BeEmpty("纯图片混排无可读文本：不进回合（造一句『你发了图片』让模型演比不演更糟）");
    }

    [Fact]
    public async Task MixedHandler_WithoutItems_ShouldNotRunTurn()
    {
        var handler = CreateMixedHandler();
        var evt = MixedEvent([]);

        var reply = await handler.HandleAsync(evt);

        reply.Should().BeNull();
        _capturedRequests.Should().BeEmpty("空混排无任何可读内容");
    }

    [Fact]
    public void VoiceHandler_ShouldDeclareExactVoiceEventKey()
    {
        CreateVoiceHandler().SupportedEventType.Should().Be(WechatBotEventTypes.Voice);
    }

    [Fact]
    public async Task VoiceHandler_ShouldUseOfficialConvertedText()
    {
        var handler = CreateVoiceHandler();
        var evt = VoiceEvent("帮我查一下昨天的打卡记录");

        var reply = await handler.HandleAsync(evt);

        reply.Should().BeNull();
        _capturedRequests.Should().ContainSingle();
        _capturedRequests[0].Last().Text.Should().Contain("帮我查一下昨天的打卡记录");
        _sent.Should().ContainSingle("语音同样走主动回复通道");
    }

    [Fact]
    public async Task VoiceHandler_WithoutConvertedText_ShouldNotRunTurn()
    {
        var handler = CreateVoiceHandler();
        var evt = VoiceEvent(null);

        var reply = await handler.HandleAsync(evt);

        reply.Should().BeNull();
        _capturedRequests.Should().BeEmpty();
    }

    [Fact]
    public async Task VoiceHandler_BlankConvertedText_ShouldNotRunTurn()
    {
        var handler = CreateVoiceHandler();
        var evt = VoiceEvent("   ");

        var reply = await handler.HandleAsync(evt);

        reply.Should().BeNull();
        _capturedRequests.Should().BeEmpty();
    }

    private const string DefaultResponseUrl =
        "https://qyapi.weixin.qq.com/cgi-bin/aibot/response?response_code=code-1";

    private static WechatBotCallbackEvent TextEvent(
        string text,
        string msgId = "msg-1",
        string? chatType = "group",
        string? chatId = "chat-1",
        string? responseUrl = DefaultResponseUrl)
        => MessageEvent(
            "text",
            new AibotMessageCallback
            {
                MsgId = msgId,
                ChatType = chatType,
                ChatId = chatId,
                MsgType = "text",
                Text = new AibotTextBody { Content = text },
            },
            msgId,
            chatType,
            chatId,
            responseUrl);

    private static WechatBotCallbackEvent MixedEvent(List<AibotMixedItem> items)
        => MessageEvent(
            "mixed",
            new AibotMessageCallback
            {
                MsgId = "msg-1",
                ChatType = "group",
                ChatId = "chat-1",
                MsgType = "mixed",
                Mixed = new AibotMixedContent { MsgItem = items },
            },
            msgId: "msg-1",
            chatType: "group",
            chatId: "chat-1");

    private static WechatBotCallbackEvent VoiceEvent(string? convertedText)
        => MessageEvent(
            "voice",
            new AibotMessageCallback
            {
                MsgId = "msg-1",
                ChatType = "single",
                MsgType = "voice",
                Voice = convertedText is null ? null : new AibotVoiceContent { Content = convertedText },
            },
            msgId: "msg-1",
            chatType: "single",
            chatId: null);

    private static WechatBotCallbackEvent MessageEvent(
        string msgType,
        AibotMessageCallback message,
        string msgId,
        string? chatType,
        string? chatId,
        string? responseUrl = DefaultResponseUrl)
        => new()
        {
            AppKey = "corp-app",
            MsgId = msgId,
            ChatType = chatType,
            ChatId = chatId,
            FromUserId = "zhangsan",
            MsgType = msgType,
            ResponseUrl = responseUrl,
            Message = message,
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
