using FluentAssertions;
using Moq;
using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.Work;
using Mud.Wechat.Work.AI.Channels;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Channels;

/// <summary>
/// Aibot 主动回复通道：markdown 形态、一次性凭据透传、字节截断、失败不抛（不可重试）、空文本 no-op。
/// </summary>
public class WechatAibotReplySinkTests
{
    private readonly Mock<IWechatWorkInternalAibotService> _aibot = new(MockBehavior.Loose);
    private readonly List<(string Code, AibotMessage Message)> _sent = new();

    public WechatAibotReplySinkTests()
    {
        _aibot
            .Setup(a => a.ReplyAsync(It.IsAny<string>(), It.IsAny<AibotMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string code, AibotMessage message, CancellationToken _) =>
            {
                _sent.Add((code, message));
                return new AibotReplyResponse { ErrorCode = 0 };
            });
    }

    [Fact]
    public async Task DeliverAsync_ShouldSendMarkdownWithResponseCode()
    {
        var sink = new WechatAibotReplySink(_aibot.Object, "code-1");

        await sink.DeliverAsync(Request(), "**你好**，我是助手。");

        _sent.Should().ContainSingle();
        _sent[0].Code.Should().Be("code-1");
        _sent[0].Message.MsgType.Should().Be(WechatBotReplyTypes.Markdown);
        _sent[0].Message.Markdown!.Content.Should().Be("**你好**，我是助手。");
    }

    [Fact]
    public async Task DeliverAsync_WhenTextBlank_ShouldNotCallEndpoint()
    {
        var sink = new WechatAibotReplySink(_aibot.Object, "code-1");

        await sink.DeliverAsync(Request(), "   ");

        _aibot.Verify(a => a.ReplyAsync(It.IsAny<string>(), It.IsAny<AibotMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeliverAsync_WhenEndpointReportsFailure_ShouldNotThrow()
    {
        _aibot
            .Setup(a => a.ReplyAsync(It.IsAny<string>(), It.IsAny<AibotMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AibotReplyResponse { ErrorCode = 45009, ErrorMessage = "api freq out of limit" });
        var sink = new WechatAibotReplySink(_aibot.Object, "code-1");

        var act = async () => await sink.DeliverAsync(Request(), "hi");

        await act.Should().NotThrowAsync("主动回复不可重试，失败只能可观测（抛异常不会带来补偿）");
    }

    [Fact]
    public async Task DeliverAsync_WhenResponseIsNull_ShouldTreatAsSuccess()
    {
        _aibot
            .Setup(a => a.ReplyAsync(It.IsAny<string>(), It.IsAny<AibotMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AibotReplyResponse?)null);
        var sink = new WechatAibotReplySink(_aibot.Object, "code-1");

        var act = async () => await sink.DeliverAsync(Request(), "hi");

        await act.Should().NotThrowAsync("官方未提供响应体说明，空响应不得视为失败");
    }

    [Fact]
    public async Task DeliverAsync_WhenTextExceedsLimit_ShouldTruncateToByteBudget()
    {
        var sink = new WechatAibotReplySink(_aibot.Object, "code-1");
        var text = new string('中', WechatAibotReplySink.MaxMarkdownBytes); // 每字 3 字节 ⇒ 必超限

        await sink.DeliverAsync(Request(), text);

        var content = _sent[0].Message.Markdown!.Content!;
        System.Text.Encoding.UTF8.GetByteCount(content).Should().BeLessThanOrEqualTo(WechatAibotReplySink.MaxMarkdownBytes);
        content.Should().EndWith("已截断）");
    }

    [Theory]
    [InlineData("https://qyapi.weixin.qq.com/cgi-bin/aibot/response?response_code=abc123&foo=1", "abc123")]
    [InlineData("https://qyapi.weixin.qq.com/cgi-bin/aibot/response?foo=1&response_code=a%2Bb", "a+b")]
    [InlineData("https://qyapi.weixin.qq.com/cgi-bin/aibot/response?response_code=", null)]
    [InlineData("https://qyapi.weixin.qq.com/cgi-bin/aibot/response?other=1", null)]
    [InlineData("not-a-url", null)]
    [InlineData(null, null)]
    [InlineData("", null)]
    public void ExtractResponseCode_ShouldParseQuerySafely(string? url, string? expected)
    {
        WechatAibotResponseCode.Extract(url).Should().Be(expected);
    }

    [Fact]
    public void Constructor_ShouldRejectMissingResponseCode()
    {
        var act = () => new WechatAibotReplySink(_aibot.Object, "  ");

        act.Should().Throw<ArgumentException>();
    }

    private static ConversationRequest Request()
        => new("corp-app", ConversationScope.Group(), "chat-1", "zhangsan", "msg-1", "hi", "chat-1");
}
