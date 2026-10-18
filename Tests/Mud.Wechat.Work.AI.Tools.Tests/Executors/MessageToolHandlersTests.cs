using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 应用消息执行器：文本参数收口，以及「部分失败必须如实回传」这条语义。
/// </summary>
public class MessageToolHandlersTests
{
    private readonly Mock<IWechatWorkMessageService> _message = new(MockBehavior.Loose);

    [Fact]
    public async Task Send_ShouldReturnMsgIdAndInvalidReceivers()
    {
        _message
            .Setup(s => s.SendTextMessageAsync(It.IsAny<MessageSendTextRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageSendResponse
            {
                ErrorCode = 0,
                MsgId = "msg-1",
                InvalidUser = "lisi",
            });

        var result = await Handler().SendAsync(
            Args(("touser", "zhangsan|lisi"), ("content", "明天 10 点评审"), ("agentid", 1000002)),
            CancellationToken.None);

        result.Text.Should().Contain("\"ok\":true");
        result.Text.Should().Contain("\"msgid\":\"msg-1\"");
        result.Text.Should().Contain("\"invalid_user\":\"lisi\"", "部分收件人失败必须回传，否则模型会谎报全部成功");
        _message.Verify(
            s => s.SendTextMessageAsync(
                It.Is<MessageSendTextRequest>(r =>
                    r.ToUser == "zhangsan|lisi"
                    && r.AgentId == 1000002
                    && r.MsgType == "text"
                    && r.Safe == 0
                    && r.Text != null
                    && r.Text.Content == "明天 10 点评审"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Send_WhenContentExceedsByteLimit_ShouldRejectBeforeCallingSdk()
    {
        var tooLong = new string('中', 700); // 700 × 3 = 2100 字节 > 2048

        var result = await Handler().SendAsync(
            Args(("touser", "zhangsan"), ("content", tooLong), ("agentid", 1000002)),
            CancellationToken.None);

        result.Text.Should().Contain("2048");
        _message.Verify(
            s => s.SendTextMessageAsync(It.IsAny<MessageSendTextRequest>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "官方按字节计，超限应在调用前拦下");
    }

    [Fact]
    public async Task Send_WhenContentBlank_ShouldRejectBeforeCallingSdk()
    {
        var act = async () => await Handler().SendAsync(
            Args(("touser", "zhangsan"), ("content", " "), ("agentid", 1000002)),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<ArgumentException>("content 在声明面是必填（Required），空白值在解包期即被拒")
            .WithMessage("*content*", "失败文案必须点出参数名，模型才能据此改参");
        _message.Verify(
            s => s.SendTextMessageAsync(It.IsAny<MessageSendTextRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Send_WithMarkdownType_ShouldDispatchToMarkdownEndpoint()
    {
        _message
            .Setup(s => s.SendMarkdownMessageAsync(It.IsAny<MessageSendMarkdownRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageSendResponse { ErrorCode = 0, MsgId = "msg-2" });

        var result = await Handler().SendAsync(
            Args(("touser", "zhangsan"), ("content", "**评审**：明天 10 点"), ("agentid", 1000002), ("msg_type", 2)),
            CancellationToken.None);

        result.Text.Should().Contain("\"msg_type\":2");
        result.Text.Should().Contain("\"msgid\":\"msg-2\"");
        _message.Verify(
            s => s.SendMarkdownMessageAsync(
                It.Is<MessageSendMarkdownRequest>(r =>
                    r.MsgType == "markdown" && r.Markdown != null && r.Markdown.Content == "**评审**：明天 10 点"),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _message.Verify(
            s => s.SendTextMessageAsync(It.IsAny<MessageSendTextRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Send_WithTextCardMissingTitle_ShouldRejectBeforeCallingSdk()
    {
        var result = await Handler().SendAsync(
            Args(("touser", "zhangsan"), ("content", "点开查看"), ("agentid", 1000002), ("msg_type", 3), ("url", "https://example.com/a")),
            CancellationToken.None);

        result.Text.Should().Contain("title");
        _message.Verify(
            s => s.SendTextCardMessageAsync(It.IsAny<MessageSendTextCardRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Send_WithTextCardUrlWithoutScheme_ShouldReject()
    {
        var result = await Handler().SendAsync(
            Args(
                ("touser", "zhangsan"),
                ("content", "点开查看"),
                ("agentid", 1000002),
                ("msg_type", 3),
                ("title", "评审通知"),
                ("url", "example.com/a")),
            CancellationToken.None);

        result.Text.Should().Contain("http");
    }

    [Fact]
    public async Task Send_WithValidTextCard_ShouldDispatchToTextCardEndpoint()
    {
        _message
            .Setup(s => s.SendTextCardMessageAsync(It.IsAny<MessageSendTextCardRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MessageSendResponse { ErrorCode = 0, MsgId = "msg-3" });

        var result = await Handler().SendAsync(
            Args(
                ("touser", "zhangsan"),
                ("content", "明天 10 点评审，请确认"),
                ("agentid", 1000002),
                ("msg_type", 3),
                ("title", "评审通知"),
                ("url", "https://example.com/a"),
                ("btn_txt", "查看")),
            CancellationToken.None);

        result.Text.Should().Contain("\"msgid\":\"msg-3\"");
        _message.Verify(
            s => s.SendTextCardMessageAsync(
                It.Is<MessageSendTextCardRequest>(r =>
                    r.MsgType == "textcard"
                    && r.TextCard != null
                    && r.TextCard.Title == "评审通知"
                    && r.TextCard.Description == "明天 10 点评审，请确认"
                    && r.TextCard.Url == "https://example.com/a"
                    && r.TextCard.BtnTxt == "查看"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Recall_ShouldReturnOkAndPassMsgId()
    {
        _message
            .Setup(s => s.RecallMessageAsync(It.IsAny<RecallMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecallMessageResponse { ErrorCode = 0 });

        var result = await Handler().RecallAsync(Args(("msgid", "msg-1")), CancellationToken.None);

        result.Text.Should().Contain("\"ok\":true");
        result.Text.Should().Contain("\"msgid\":\"msg-1\"");
        _message.Verify(
            s => s.RecallMessageAsync(
                It.Is<RecallMessageRequest>(r => r.MsgId == "msg-1"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Recall_WhenApiFails_ShouldReturnReadableError()
    {
        _message
            .Setup(s => s.RecallMessageAsync(It.IsAny<RecallMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecallMessageResponse { ErrorCode = 40056, ErrorMessage = "invalid msgid" });

        var result = await Handler().RecallAsync(Args(("msgid", "nope")), CancellationToken.None);

        result.Text.Should().Contain("40056");
        result.Text.Should().Contain("invalid msgid");
    }

    private MessageToolHandlers Handler() => new(_message.Object);

    private static Dictionary<string, object?> Args(params (string Key, object? Value)[] pairs)
    {
        var args = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var (key, value) in pairs)
        {
            args[key] = value;
        }

        return args;
    }
}
