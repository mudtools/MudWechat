using FluentAssertions;
using Mud.Wechat.Work.AI.Tools.Security;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Security;

/// <summary>
/// 审批状态机的行为锚：一次性兑现、批准与执行内容绑定、惰性过期、重复判定被拒。
/// </summary>
public class WechatToolApprovalStateMachineTests
{
    private static WechatToolAuthorizationContext Context(string tool = "wedoc.doc.delete")
        => new(tool, "corp-app", "corp-1", Mud.Wechat.Work.AI.AgentTools.WechatToolRisk.HighRiskWrite, ["wedoc.doc.write"], "docid=str");

    [Fact]
    public async Task Suspend_ShouldCreatePendingTicketWithDigestAndExpiry()
    {
        var machine = Machine();

        var ticket = await machine.SuspendAsync(Context());

        ticket.TicketId.Should().StartWith("wta_");
        ticket.State.Should().Be(WechatToolApprovalState.Pending);
        ticket.ToolName.Should().Be("wedoc.doc.delete");
        ticket.Risk.Should().Be(Mud.Wechat.Work.AI.AgentTools.WechatToolRisk.HighRiskWrite);
        ticket.ArgumentsDigest.Should().Be("docid=str", "工单只存摘要，不存原始入参");
        ticket.ExpiresAt.Should().BeAfter(ticket.CreatedAt);
        ticket.Revision.Should().Be(0);
    }

    [Fact]
    public async Task Approve_ThenConsume_ShouldSucceedExactlyOnce()
    {
        var machine = Machine();
        var ticket = await machine.SuspendAsync(Context());

        await machine.ApproveAsync(ticket.TicketId, "admin", "管理员确认删除");

        var first = await machine.ConsumeAsync(ticket.TicketId, "wedoc.doc.delete", "docid=str");
        first.IsConsumed.Should().BeTrue();

        var second = await machine.ConsumeAsync(ticket.TicketId, "wedoc.doc.delete", "docid=str");
        second.IsConsumed.Should().BeFalse();
        second.FailureReason.Should().Contain("已被使用过", "一次批准只能执行一次");
    }

    [Fact]
    public async Task Consume_BeforeApproval_ShouldFail()
    {
        var machine = Machine();
        var ticket = await machine.SuspendAsync(Context());

        var result = await machine.ConsumeAsync(ticket.TicketId, "wedoc.doc.delete", "docid=str");

        result.IsConsumed.Should().BeFalse();
        result.FailureReason.Should().Contain("Pending");
    }

    [Fact]
    public async Task Consume_WithDifferentArguments_ShouldFail()
    {
        var machine = Machine();
        var ticket = await machine.SuspendAsync(Context());
        await machine.ApproveAsync(ticket.TicketId, "admin");

        var result = await machine.ConsumeAsync(ticket.TicketId, "wedoc.doc.delete", "docid=str;formid=str");

        result.IsConsumed.Should().BeFalse();
        result.FailureReason.Should().Contain("不一致", "批准 A、执行 B 必须不可能");
    }

    [Fact]
    public async Task Consume_WithDifferentTool_ShouldFail()
    {
        var machine = Machine();
        var ticket = await machine.SuspendAsync(Context());
        await machine.ApproveAsync(ticket.TicketId, "admin");

        var result = await machine.ConsumeAsync(ticket.TicketId, "wedoc.doc.rename", "docid=str");

        result.IsConsumed.Should().BeFalse();
        result.FailureReason.Should().Contain("wedoc.doc.delete");
    }

    [Fact]
    public async Task Reject_ThenConsume_ShouldFailWithRejectedState()
    {
        var machine = Machine();
        var ticket = await machine.SuspendAsync(Context());

        await machine.RejectAsync(ticket.TicketId, "影响面过大", "admin");

        var result = await machine.ConsumeAsync(ticket.TicketId, "wedoc.doc.delete", "docid=str");
        result.IsConsumed.Should().BeFalse();
        result.FailureReason.Should().Contain("Rejected");
    }

    [Fact]
    public async Task Approve_Twice_ShouldThrow()
    {
        var machine = Machine();
        var ticket = await machine.SuspendAsync(Context());
        await machine.ApproveAsync(ticket.TicketId, "admin");

        var act = async () => await machine.ApproveAsync(ticket.TicketId, "admin2");

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .WithMessage("*只有待审批的工单可以被批准*", "重复判定须被拒，防止审计出现两个真相");
    }

    [Fact]
    public async Task ExpiredTicket_ShouldBecomeExpiredAndRefuseConsume()
    {
        var machine = Machine(TimeSpan.FromMilliseconds(1));
        var ticket = await machine.SuspendAsync(Context());
        await Task.Delay(20);

        var loaded = await machine.FindAsync(ticket.TicketId);

        loaded!.State.Should().Be(WechatToolApprovalState.Expired, "超时判定是惰性的：首次读取即迁移");

        var result = await machine.ConsumeAsync(ticket.TicketId, "wedoc.doc.delete", "docid=str");
        result.IsConsumed.Should().BeFalse();
        result.FailureReason.Should().Contain("Expired");
    }

    [Fact]
    public async Task Find_UnknownTicket_ShouldReturnNull()
    {
        var machine = Machine();

        var loaded = await machine.FindAsync("wta_missing");

        loaded.Should().BeNull();
    }

    [Fact]
    public void CreatePending_WithNonPositiveTtl_ShouldThrow()
    {
        var act = () => WechatToolApprovalTicket.CreatePending(Context(), TimeSpan.Zero);

        act.Should().Throw<ArgumentOutOfRangeException>("零或负 TTL 等于「不许审批」");
    }

    private static WechatToolApprovalStateMachine Machine(TimeSpan? ttl = null)
        => new(new InMemoryWechatToolApprovalStore(), ttl);
}
