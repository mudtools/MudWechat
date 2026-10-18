using FluentAssertions;
using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.AI.Agent.Conversations;
using Xunit;

namespace Mud.Wechat.AI.Tests.Conversations;

/// <summary>
/// 层 1 默认事件装配器：中立投影 / 凭据与身份透传 / 必填维度缺失回落空（不进回合）。
/// </summary>
public class DefaultContextAssemblerTests
{
    private readonly DefaultContextAssembler _assembler = new();

    [Fact]
    public void SupportedEventKey_ShouldBeEmptyForFallback()
    {
        _assembler.SupportedEventKey.Should().BeEmpty("空串 = 兜底：未被精确装配器命中的事件族都走中立投影");
    }

    [Fact]
    public async Task Assemble_ShouldProjectFactsAndPassThroughKeyAndPrincipal()
    {
        var facts = Facts();
        var principal = new WechatToolPrincipal("corp-app", "corp-1");
        var input = new ConversationAssemblyInput(facts, "resp-code-1", principal);

        var assembly = await _assembler.AssembleAsync(input);

        assembly.Should().NotBeNull();
        assembly!.Request.AppKey.Should().Be("corp-app");
        assembly.Request.SubjectId.Should().Be("chat-1");
        assembly.Request.SenderId.Should().Be("zhangsan");
        assembly.Request.MessageId.Should().Be("msg-1");
        assembly.Request.MentionedText.Should().Be("帮我查一下");
        assembly.Request.EventKey.Should().Be("text");
        assembly.Request.Scope.IsGroup.Should().BeTrue();
        assembly.ReplyChannelKey.Should().Be("resp-code-1", "回复通道凭据只透传不解释");
        assembly.Principal.Should().BeSameAs(principal, "身份维度原样带回，由各线在执行前写入作用域");
    }

    [Fact]
    public async Task Assemble_WithoutKeyAndPrincipal_ShouldStillProjectRequest()
    {
        var assembly = await _assembler.AssembleAsync(new ConversationAssemblyInput(Facts()));

        assembly.Should().NotBeNull("无凭据/无身份是可装配的：缺凭据的拦截在通道层，不在装配层");
        assembly!.ReplyChannelKey.Should().BeNull();
        assembly.Principal.Should().BeNull();
    }

    [Theory]
    [InlineData(null, "chat-1", "zhangsan", "msg-1")]
    [InlineData("", "chat-1", "zhangsan", "msg-1")]
    [InlineData("corp-app", null, "zhangsan", "msg-1")]
    [InlineData("corp-app", "chat-1", null, "msg-1")]
    [InlineData("corp-app", "chat-1", "zhangsan", null)]
    public async Task Assemble_WhenRequiredDimensionMissing_ShouldReturnNull(
        string? appKey,
        string? subjectId,
        string? senderId,
        string? messageId)
    {
        var facts = new WechatConversationFacts(
            appKey: appKey,
            isGroup: true,
            subjectId: subjectId,
            senderId: senderId,
            messageId: messageId,
            mentionedText: "hi",
            eventKey: "text");

        var assembly = await _assembler.AssembleAsync(new ConversationAssemblyInput(facts, "code"));

        assembly.Should().BeNull(
            "会话键必填维度缺失 = 多租户隔离塌陷前兆：不进回合（调用方按空包处理），而非把投影缺陷升级成回调异常");
    }

    [Fact]
    public async Task Assemble_WithNullInput_ShouldThrow()
    {
        var act = async () => await _assembler.AssembleAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    private static WechatConversationFacts Facts()
        => new(
            appKey: "corp-app",
            isGroup: true,
            subjectId: "chat-1",
            senderId: "zhangsan",
            messageId: "msg-1",
            mentionedText: "帮我查一下",
            chatId: "chat-1",
            eventKey: "text");
}
