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
        var result = await Handler().SendAsync(
            Args(("touser", "zhangsan"), ("content", " "), ("agentid", 1000002)),
            CancellationToken.None);

        result.Text.Should().Contain("content");
        _message.Verify(
            s => s.SendTextMessageAsync(It.IsAny<MessageSendTextRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
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
