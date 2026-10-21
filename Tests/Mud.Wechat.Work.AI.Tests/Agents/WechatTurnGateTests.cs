using FluentAssertions;
using Mud.AI.Abstractions.Configuration;
using Mud.AI.Agent.Conversations;
using Mud.Wechat.Work.AI.Agents;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Agents;

/// <summary>
/// 回合闸门外壳：<b>同会话串行</b> + <b>进程内削峰</b>两层，且「排队超时」显式失败而不是无限等待。
/// </summary>
/// <remarks>
/// <para>
/// <b>回归点</b>：没有同会话串行时，同一会话的两条消息会并发跑两个回合——两边都读到「上一轮的历史」，
/// 后写覆盖先写，用户看到的就是「回答串了/丢了一句」。这类缺陷在压测里也不报错，只表现为会话内容错乱。
/// </para>
/// <para>
/// <b>获取顺序</b>：先键后配额——一批同会话回合不得占满全局额度再一起等同一个键（吞吐假死）。
/// 用例 <see cref="Acquire_DifferentConversations_ShouldRunConcurrentlyWhenQuotaAllows"/> 反向守护这一点。
/// </para>
/// </remarks>
public class WechatTurnGateTests
{
    [Fact]
    public async Task Acquire_SameConversation_ShouldSerializeTurns()
    {
        var gate = CreateGate(maxConcurrent: 4, queueTimeoutSeconds: 5);

        var first = await gate.AcquireAsync("wechat:corp-app:p:u1");
        first.Should().NotBeNull();

        var secondAcquired = false;
        var second = Task.Run(async () =>
        {
            using var handle = await gate.AcquireAsync("wechat:corp-app:p:u1");
            secondAcquired = true;
        });

        await Task.Delay(150);
        secondAcquired.Should().BeFalse("同一会话的第二个回合必须等第一个回合释放");

        first!.Dispose();
        await second.WaitAsync(TimeSpan.FromSeconds(5));

        secondAcquired.Should().BeTrue();
    }

    [Fact]
    public async Task Acquire_DifferentConversations_ShouldRunConcurrentlyWhenQuotaAllows()
    {
        var gate = CreateGate(maxConcurrent: 2, queueTimeoutSeconds: 5);

        using var a = await gate.AcquireAsync("wechat:corp-app:p:u1");
        using var b = await gate.AcquireAsync("wechat:corp-app:p:u2");

        a.Should().NotBeNull();
        b.Should().NotBeNull("不同会话之间不得互相阻塞（键控闸门只管同键）");
    }

    [Fact]
    public async Task Acquire_WhenQuotaExhausted_ShouldFailFastWithActionableError()
    {
        var gate = CreateGate(maxConcurrent: 1, queueTimeoutSeconds: 1);

        using var held = await gate.AcquireAsync("wechat:corp-app:p:hot");

        var act = async () => await gate.AcquireAsync("wechat:corp-app:p:other");

        (await act.Should().ThrowAsync<InvalidOperationException>("排队超时必须在日志/指标里可见，不能表现成「消息没有回应」"))
            .Which.Message.Should().Contain("MaxConcurrentTurns");
    }

    [Fact]
    public async Task Acquire_WhenQuotaTimesOut_ShouldNotLeakConversationKey()
    {
        var gate = CreateGate(maxConcurrent: 1, queueTimeoutSeconds: 1);

        using (var held = await gate.AcquireAsync("wechat:corp-app:p:blocker"))
        {
            var failing = async () => await gate.AcquireAsync("wechat:corp-app:p:victim");
            await failing.Should().ThrowAsync<InvalidOperationException>();
        }

        // 失败路径若漏放会话键，同一会话后续回合会永久阻塞（信号量泄漏），故此处必须能再拿到。
        using var recovered = await gate.AcquireAsync("wechat:corp-app:p:victim");
        recovered.Should().NotBeNull();
    }

    [Fact]
    public async Task Dispose_ShouldBeIdempotentAndReleaseQuota()
    {
        var limiter = new ConcurrencyLimiter(1, TimeSpan.FromSeconds(5));
        var gate = new WechatTurnGate(new KeyedAsyncGate(), limiter, new AgentRuntimeOptions { TurnQueueTimeoutSeconds = 5 });

        var handle = await gate.AcquireAsync("wechat:corp-app:p:u1");
        limiter.InFlight.Should().Be(1);

        handle!.Dispose();
        handle.Dispose();

        limiter.InFlight.Should().Be(0, "重复释放不得多还许可（否则削峰上限会被悄悄放大）");
        (await gate.AcquireAsync("wechat:corp-app:p:u2")).Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ShouldRejectNullDependencies()
    {
        var options = new AgentRuntimeOptions();

        var act1 = () => new WechatTurnGate(null!, new ConcurrencyLimiter(1), options);
        var act2 = () => new WechatTurnGate(new KeyedAsyncGate(), null!, options);
        var act3 = () => new WechatTurnGate(new KeyedAsyncGate(), new ConcurrencyLimiter(1), null!);

        act1.Should().Throw<ArgumentNullException>();
        act2.Should().Throw<ArgumentNullException>();
        act3.Should().Throw<ArgumentNullException>();
    }

    private static WechatTurnGate CreateGate(int maxConcurrent, int queueTimeoutSeconds)
        => new(
            new KeyedAsyncGate(),
            new ConcurrencyLimiter(maxConcurrent, TimeSpan.FromSeconds(queueTimeoutSeconds)),
            new AgentRuntimeOptions { TurnQueueTimeoutSeconds = queueTimeoutSeconds });
}
