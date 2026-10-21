using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Moq;
using Mud.AI.Abstractions.Approvals;
using Mud.AI.Agent.Approvals;
using Mud.AI.Agent.Sessions;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Approvals;
using Mud.Wechat.Work.AI.Configuration;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Approvals;

/// <summary>
/// HITL 机制侧（写工具人工确认）：批准通道的投递接缝、待确认快照的可查性、以及批准回灌的 fail-closed 边界。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么「机制侧」也要用例</b>：HITL 的所有失败形态都是<b>静默</b>的——通道没装上 ⇒ 只记日志；快照没落库 ⇒
/// 进程重启后待办消失；批准的消费不幂等 ⇒ 同一批准被重放两次写操作。三者都不报错，只能在用例里逐个钉住。
/// </para>
/// <para>
/// <b>不放行任何「自动批准」</b>：本组用例只验证「通知 / 可查 / 拒绝」，不存在「未批准即执行」的路径——
/// 一旦出现，就是安全缺陷而非功能缺陷。
/// </para>
/// </remarks>
public class WechatToolApprovalTests
{
    [Fact]
    public async Task Channel_WithoutNotifier_ShouldReturnNullAndKeepToolsUnexecuted()
    {
        var channel = new WechatToolApprovalChannel();

        var correlationId = await channel.RequestFrameworkApprovalAsync(Request("req-1"));

        correlationId.Should().BeNull("未注册投递端口只能退化为「只记日志 + 快照可查」，不得伪造关联号");
    }

    [Fact]
    public async Task Channel_WithNotifier_ShouldForwardRequestAndReturnCorrelationId()
    {
        var notifier = new RecordingNotifier("card-42");
        var channel = new WechatToolApprovalChannel(notifier);

        var correlationId = await channel.RequestFrameworkApprovalAsync(Request("req-2"));

        correlationId.Should().Be("card-42");
        notifier.Requests.Should().ContainSingle().Which.RequestId.Should().Be("req-2");
    }

    [Fact]
    public async Task Channel_LegacyTokenForm_ShouldNotBeUsedByWechat()
    {
        var notifier = new RecordingNotifier("card-1");
        var channel = new WechatToolApprovalChannel(notifier);

        var correlationId = await channel.RequestApprovalAsync(new ToolApprovalRequest(
            "contacts.department.get",
            "corp-app",
            "u1",
            "wechat:corp-app:p:u1",
            "digest",
            [],
            "需要确认",
            DateTimeOffset.UtcNow.AddMinutes(10)));

        correlationId.Should().BeNull("企微侧写工具走框架原生审批，无「确认令牌往返模型」这条路径");
        notifier.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task Channel_ShouldRejectNullRequest()
    {
        var channel = new WechatToolApprovalChannel();

        await FluentActions.Awaiting(() => channel.RequestFrameworkApprovalAsync(null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task PendingSnapshot_ShouldBeDiscoverableAfterNotify_AndSurviveNewChannelInstance()
    {
        // 「挂起可查」+「重启可查」的机制等价物：快照落在存储里，而不是挂在通道实例上。
        var store = new InMemoryPendingApprovalStore();
        var pending = new[] { Request("req-3") };

        await ApprovalRequestProjector.NotifyAsync(
            new WechatToolApprovalChannel(new RecordingNotifier("card-3")),
            store,
            logger: null,
            pending,
            CancellationToken.None);

        var afterRestart = new WechatToolApprovalResume(
            Agent(),
            new WechatWorkAiScopeFactory(
                Mock.Of<IWechatAppContextSwitcher>(),
                new WechatAiCorpContextAccessor()),
            new WechatAiCorpContextAccessor(),
            pendingApprovals: store);

        var items = await afterRestart.ListPendingAsync("corp-app");

        items.Should().ContainSingle().Which.RequestId.Should().Be("req-3");
    }

    [Fact]
    public async Task ListPendingAsync_WithoutStore_ShouldReturnEmpty()
    {
        var resume = new WechatToolApprovalResume(
            Agent(),
            new WechatWorkAiScopeFactory(Mock.Of<IWechatAppContextSwitcher>(), new WechatAiCorpContextAccessor()),
            new WechatAiCorpContextAccessor());

        (await resume.ListPendingAsync("corp-app")).Should().BeEmpty();
        await FluentActions.Awaiting(() => resume.ListPendingAsync(" "))
            .Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Resume_WhenFrameworkRecordIsGone_ShouldFailClosed()
    {
        // 会话里没有该 requestId 的框架记录 ⇒ 绝不凭空构造批准响应（否则会把「人类批准过的调用」重指到未展示的调用）。
        var resume = new WechatToolApprovalResume(
            Agent(),
            new WechatWorkAiScopeFactory(Mock.Of<IWechatAppContextSwitcher>(), new WechatAiCorpContextAccessor()),
            new WechatAiCorpContextAccessor(),
            pendingApprovals: new InMemoryPendingApprovalStore());

        await FluentActions.Awaiting(() => resume.ResumeAsync(Request("req-absent"), approved: true))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*已失效*");
    }

    [Fact]
    public async Task Resume_ShouldRejectIncompleteApprovalElements()
    {
        var resume = new WechatToolApprovalResume(
            Agent(),
            new WechatWorkAiScopeFactory(Mock.Of<IWechatAppContextSwitcher>(), new WechatAiCorpContextAccessor()),
            new WechatAiCorpContextAccessor());

        await FluentActions.Awaiting(() => resume.ResumeAsync(null!, approved: true))
            .Should().ThrowAsync<ArgumentNullException>();

        await FluentActions.Awaiting(() => resume.ResumeAsync(Request("req-4") with { TenantKey = " " }, approved: true))
            .Should().ThrowAsync<ArgumentException>("多租户隔离禁止默认租户兜底");
    }

    private static FrameworkToolApprovalRequest Request(string requestId)
        => new(
            RequestId: requestId,
            ToolName: "contacts.user.create",
            ToolCallId: "call-1",
            TenantKey: "corp-app",
            UserId: "u1",
            ConversationKey: "wechat:corp-app:p:u1",
            ArgumentsDigest: "{\"userid\":\"u9\"}",
            RequiredScopes: [],
            ChatId: null);

    private static WechatAgent Agent()
    {
        var chatClient = new Mock<IChatClient>(MockBehavior.Loose);
        chatClient
            .Setup(c => c.GetResponseAsync(
                It.IsAny<IEnumerable<ChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChatResponse(new ChatMessage(ChatRole.Assistant, "ok")));

        return new WechatAgent(
            chatClient.Object,
            Options.Create(new WechatAgentOptions()),
            new MemorySessionStore());
    }

    private sealed class RecordingNotifier(string? correlationId) : IWechatToolApprovalNotifier
    {
        public List<FrameworkToolApprovalRequest> Requests { get; } = [];

        public Task<string?> NotifyAsync(
            FrameworkToolApprovalRequest request,
            CancellationToken cancellationToken = default)
        {
            Requests.Add(request);
            return Task.FromResult(correlationId);
        }
    }
}
