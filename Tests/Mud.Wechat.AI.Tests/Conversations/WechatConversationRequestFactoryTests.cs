using FluentAssertions;
using Mud.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Abstractions.Conversations;
using Mud.Wechat.AI.Agent.Conversations;
using Xunit;

namespace Mud.Wechat.AI.Tests.Conversations;

/// <summary>
/// 层 1 会话请求投影工厂：维度映射 / 群聊与单聊判定 / 可空维度透传 / 必填维度 fail-fast。
/// </summary>
public class WechatConversationRequestFactoryTests
{
    [Fact]
    public void Create_GroupChat_ShouldMapToGroupScopeWithChatSubject()
    {
        var facts = new WechatConversationFacts(
            appKey: "corp-app",
            isGroup: true,
            subjectId: "chat-1",
            senderId: "zhangsan",
            messageId: "msg-1",
            mentionedText: "帮我查一下",
            chatId: "chat-1",
            parentId: "msg-0",
            threadId: "thread-1",
            eventKey: "text");

        var request = WechatConversationRequestFactory.Create(facts);

        request.AppKey.Should().Be("corp-app");
        request.Scope.Should().Be(ConversationScope.Group());
        request.Scope.IsGroup.Should().BeTrue();
        request.SubjectId.Should().Be("chat-1");
        request.SenderId.Should().Be("zhangsan");
        request.MessageId.Should().Be("msg-1");
        request.MentionedText.Should().Be("帮我查一下");
        request.ChatId.Should().Be("chat-1");
        request.ParentId.Should().Be("msg-0");
        request.ThreadId.Should().Be("thread-1");
        request.EventKey.Should().Be("text");
        request.EventFacts.Should().BeNull();
    }

    [Fact]
    public void Create_SingleChat_ShouldMapToP2PScope()
    {
        var facts = new WechatConversationFacts(
            appKey: "corp-app",
            isGroup: false,
            subjectId: "lisi",
            senderId: "lisi",
            messageId: "msg-2",
            mentionedText: "hi");

        var request = WechatConversationRequestFactory.Create(facts);

        request.Scope.Should().Be(ConversationScope.P2P());
        request.Scope.ChatType.Should().Be(ConversationScope.ChatTypeP2P);
        request.SubjectId.Should().Be("lisi");
        request.ChatId.Should().BeNull("单聊未提供 chat_id 时不得伪造空串（会污染流式目标回退）");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenAppKeyMissing_ShouldFailFast(string? appKey)
    {
        var facts = new WechatConversationFacts(appKey, isGroup: false, subjectId: "u", senderId: "u", messageId: "m");

        var act = () => WechatConversationRequestFactory.Create(facts);

        act.Should().Throw<ArgumentException>().WithMessage("*AppKey*");
    }

    [Fact]
    public void Create_WhenSubjectIdMissing_ShouldFailFast()
    {
        var facts = new WechatConversationFacts("app", isGroup: true, subjectId: null, senderId: "u", messageId: "m");

        var act = () => WechatConversationRequestFactory.Create(facts);

        act.Should().Throw<ArgumentException>().WithMessage("*SubjectId*");
    }

    [Fact]
    public void Create_WhenSenderOrMessageIdMissing_ShouldFailFast()
    {
        var missingSender = new WechatConversationFacts("app", isGroup: false, subjectId: "u", senderId: " ", messageId: "m");
        var missingMessage = new WechatConversationFacts("app", isGroup: false, subjectId: "u", senderId: "u", messageId: null);

        var actSender = () => WechatConversationRequestFactory.Create(missingSender);
        var actMessage = () => WechatConversationRequestFactory.Create(missingMessage);

        actSender.Should().Throw<ArgumentException>().WithMessage("*SenderId*");
        actMessage.Should().Throw<ArgumentException>().WithMessage("*MessageId*");
    }

    [Fact]
    public void Create_WhenFactsNull_ShouldThrow()
    {
        var act = () => WechatConversationRequestFactory.Create(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
