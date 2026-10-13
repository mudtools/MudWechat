// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;
using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work.Abstractions.Tests;

/// <summary>
/// 智能机器人回调契约层测试：匹配键判别（事件族 / 消息族）、应答工厂的结构自洽、
/// 以及各传输支持面的 fail-fast 校验。
/// </summary>
public class WechatBotCallbackEventTests
{
    [Fact]
    public void EventTypeKey_ShouldUseEventTypeForEvents_AndMsgTypeForMessages()
    {
        new WechatBotCallbackEvent { EventType = WechatBotEventTypes.TemplateCardEvent, MsgType = "event" }
            .EventTypeKey.Should().Be(WechatBotEventTypes.TemplateCardEvent, "事件回调以 event.eventtype 为匹配键");
        new WechatBotCallbackEvent { MsgType = WechatBotEventTypes.Stream }
            .EventTypeKey.Should().Be(WechatBotEventTypes.Stream, "消息回调以顶层 msgtype 为匹配键");
        new WechatBotCallbackEvent()
            .EventTypeKey.Should().BeEmpty("两者皆空返回空串（仅兜底处理器可见）");
    }

    [Fact]
    public void IsEventCallback_ShouldFollowEventTypePresence()
    {
        new WechatBotCallbackEvent { MsgType = "event" }.IsEventCallback.Should().BeFalse(
            "判别依据是 event.eventtype 是否存在，而非顶层 msgtype == event（缺失 eventtype 即不可识别）");
        new WechatBotCallbackEvent { EventType = WechatBotEventTypes.EnterChat }.IsEventCallback.Should().BeTrue();
    }

    [Fact]
    public void Replies_ShouldProduceSelfConsistentStructures()
    {
        var card = new TemplateCardBody { CardType = "text_notice" };

        WechatBotReplies.Text("hi").Should().Match<AibotMessage>(m =>
            m.MsgType == WechatBotReplyTypes.Text && m.Text!.Content == "hi");

        var markdown = WechatBotReplies.Markdown("# t", "fb1");
        markdown.MsgType.Should().Be(WechatBotReplyTypes.Markdown);
        markdown.Markdown!.Feedback!.Id.Should().Be("fb1",
            "markdown/template_card/stream 三处 feedback 为同一结构（单一声明，跨域复用）");

        var stream = WechatBotReplies.Stream("sid", "acc", finish: true, feedbackId: "fb2");
        stream.Stream!.Id.Should().Be("sid");
        stream.Stream.Finish.Should().BeTrue();
        stream.Stream.Feedback!.Id.Should().Be("fb2");

        var combined = WechatBotReplies.StreamWithTemplateCard("sid", "acc", card);
        combined.Stream!.Id.Should().Be("sid");
        combined.TemplateCard.Should().BeSameAs(card);

        var update = WechatBotReplies.UpdateTemplateCard(card, new[] { "u1" });
        update.ResponseType.Should().Be(WechatBotReplyTypes.UpdateTemplateCard);
        update.MsgType.Should().BeNull("更新卡片应答以 response_type 表达，不声明 msgtype");
        update.UserIds.Should().Equal("u1");
    }

    [Fact]
    public void ReplySupport_ShouldAcceptOfficialPassiveSurface_AndRejectOthers()
    {
        var card = new TemplateCardBody { CardType = "button_interaction" };

        FluentActions.Invoking(() => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.TemplateCard(card)))
            .Should().NotThrow();
        FluentActions.Invoking(() => WechatBotReplySupport.ValidateHttpPassiveReply(
                WechatBotReplies.UpdateTemplateCard(card)))
            .Should().NotThrow();
        FluentActions.Invoking(() => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.Markdown("x")))
            .Should().Throw<InvalidOperationException>("markdown 为主动回复 / 长连接支持面，被动回复拒绝");
        FluentActions.Invoking(() => WechatBotReplySupport.ValidateHttpPassiveReply(new AibotMessage()))
            .Should().Throw<InvalidOperationException>("空应答应返回 null 由分发器统一回空包，而非声明空 msgtype");
    }

    // ---------------------------------------------------------------- 事件维度规则（v1 方案 F10，BT13）

    [Fact]
    public void ReplySupport_ShouldRejectNonEmptyReply_ForFeedbackEvent_OnBothTransports()
    {
        var card = new TemplateCardBody { CardType = "button_interaction" };

        // 官方 101027/101031：feedback_event 仅支持回复空包（处理器返回 null）—— 非 null 一律拒绝。
        foreach (var reply in new AibotMessage?[]
                 {
                     WechatBotReplies.Text("x"),
                     WechatBotReplies.Markdown("x"),
                     WechatBotReplies.Stream("sid", "acc"),
                     WechatBotReplies.TemplateCard(card),
                     WechatBotReplies.UpdateTemplateCard(card),
                 })
        {
            FluentActions.Invoking(() => WechatBotReplySupport.ValidateHttpPassiveReply(reply!, WechatBotEventTypes.FeedbackEvent))
                .Should().Throw<InvalidOperationException>("HTTP 被动回复：feedback_event 仅空包");
            FluentActions.Invoking(() => WechatBotReplySupport.ValidateLongConnectionReply(reply!, WechatBotEventTypes.FeedbackEvent))
                .Should().Throw<InvalidOperationException>("长连接：feedback_event 同样仅空包");
        }
    }

    [Fact]
    public void ReplySupport_ShouldAllowText_OnlyForEnterChatEvent_OnHttpPassive()
    {
        // 官方 101031 单向规则：text ⇒ enter_chat。
        FluentActions.Invoking(() =>
                WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.Text("欢迎语"), WechatBotEventTypes.EnterChat))
            .Should().NotThrow("进入会话回调事件的 text 欢迎语合法");

        foreach (var eventType in new[]
                 {
                     WechatBotEventTypes.TemplateCardEvent,
                     "text",   // 消息回调（匹配键为顶层 msgtype）
                 })
        {
            FluentActions.Invoking(() =>
                    WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.Text("x"), eventType))
                .Should().Throw<InvalidOperationException>("HTTP 被动回复的 text 仅支持 enter_chat（101031）");
        }
    }

    [Fact]
    public void ReplySupport_ShouldNotInventReverseMatrix_ForEnterChat()
    {
        // D10 不臆造矩阵：官方只约束「text ⇒ enter_chat」，不得反向拒绝 enter_chat 携带其它合法形态。
        var card = new TemplateCardBody { CardType = "text_notice" };

        FluentActions.Invoking(() =>
                WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.TemplateCard(card), WechatBotEventTypes.EnterChat))
            .Should().NotThrow("欢迎语携带 template_card 合法（反向矩阵是臆造）");
        FluentActions.Invoking(() =>
                WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.Stream("sid", "acc"), WechatBotEventTypes.EnterChat))
            .Should().NotThrow("欢迎语携带 stream 合法（反向矩阵是臆造）");
    }

    [Fact]
    public void ReplySupport_ShouldAllowLongConnectionSurface_ForMessageEvents()
    {
        // 长连接面的事件维度只多一条 feedback_event 规则；消息事件的 markdown 仍合法（回归防误伤）。
        FluentActions.Invoking(() =>
                WechatBotReplySupport.ValidateLongConnectionReply(WechatBotReplies.Markdown("x"), WechatBotEventTypes.Stream))
            .Should().NotThrow("长连接 markdown 对消息事件合法（101463）");
        FluentActions.Invoking(() =>
                WechatBotReplySupport.ValidateLongConnectionReply(WechatBotReplies.Text("x"), WechatBotEventTypes.TemplateCardEvent))
            .Should().NotThrow("长连接 text 不受「text ⇒ enter_chat」约束（该规则属 HTTP 被动回复面）");
    }

    [Fact]
    public void ReplySupport_ShouldKeepLegacyOverloadBehavior_WhenEventTypeUnknown()
    {
        // 回归：旧重载（无事件键）行为不变 —— 只做 msgtype 维度校验，两条事件维度规则均不触发。
        var card = new TemplateCardBody { CardType = "button_interaction" };

        FluentActions.Invoking(() => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.Text("x")))
            .Should().NotThrow("事件键未知时 text 不触发 enter_chat 限定");
        FluentActions.Invoking(() => WechatBotReplySupport.ValidateLongConnectionReply(WechatBotReplies.Markdown("x")))
            .Should().NotThrow("事件键未知时 markdown 在长连接面合法");
        FluentActions.Invoking(() => WechatBotReplySupport.ValidateHttpPassiveReply(WechatBotReplies.TemplateCard(card)))
            .Should().NotThrow();
    }
}
