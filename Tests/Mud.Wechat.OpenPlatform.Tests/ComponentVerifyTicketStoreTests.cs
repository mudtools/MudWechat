// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// <c>component_verify_ticket</c> 存储端口的语义锁定（进程内默认实现）。
/// </summary>
/// <remarks>
/// <b>本组的核心是「空白推送不得覆盖」</b>：推送链路的异常若能清掉好票据，
/// 会让下一次令牌刷新必然失败 —— 而失败直到令牌到期才暴露（埋炸弹形态）。
/// </remarks>
public class ComponentVerifyTicketStoreTests
{
    /// <summary>初始状态无票据（防「空跑」）。</summary>
    [Fact]
    public void TryGet_ShouldReturnFalse_WhenNothingSet()
    {
        var store = new InMemoryComponentVerifyTicketStore(new FakeClock());

        store.TryGet(out var snapshot).Should().BeFalse();
        snapshot.Should().BeNull();
    }

    /// <summary>正常写入后可读取，且记录接收时间与去空白后的内容。</summary>
    [Fact]
    public void Set_ShouldStoreTrimmedTicketWithReceivedAt()
    {
        var clock = new FakeClock(new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero));
        var store = new InMemoryComponentVerifyTicketStore(clock);

        store.Set("  ticket-1  ");

        store.TryGet(out var snapshot).Should().BeTrue();
        snapshot!.Ticket.Should().Be("ticket-1", "两侧空白须去除（官方票据不含空白，混入会直接使令牌请求失败）");
        snapshot.ReceivedAt.Should().Be(clock.UtcNow);
    }

    /// <summary>
    /// <b>空白推送不得覆盖已有有效票据</b>（<c>null</c> / 空串 / 全空白三种形态）。
    /// </summary>
    /// <remarks>
    /// 若这里改成「覆盖」，则一次异常推送就会让凭证链断掉 ——
    /// 而且断点在<b>下一次令牌刷新</b>才显形，属典型的「埋一颗定时炸弹」。
    /// </remarks>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Set_ShouldIgnoreInvalidPush_WithoutClearingExistingTicket(string? invalid)
    {
        var clock = new FakeClock(new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero));
        var store = new InMemoryComponentVerifyTicketStore(clock);
        store.Set("good-ticket");

        clock.Advance(TimeSpan.FromMinutes(5));
        store.Set(invalid);

        store.TryGet(out var snapshot).Should().BeTrue("无效推送必须被忽略");
        snapshot!.Ticket.Should().Be("good-ticket");
        snapshot.ReceivedAt.Should().Be(new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero),
            "被忽略的推送不得更新接收时间（否则会掩盖「票据已陈旧」这一事实）");
    }

    /// <summary>后续有效推送须整体替换（票据会周期性重复推送）。</summary>
    [Fact]
    public void Set_ShouldReplaceWithLatestValidTicket()
    {
        var clock = new FakeClock(new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero));
        var store = new InMemoryComponentVerifyTicketStore(clock);

        store.Set("ticket-1");
        clock.Advance(TimeSpan.FromMinutes(5));
        store.Set("ticket-2");

        store.TryGet(out var snapshot).Should().BeTrue();
        snapshot!.Ticket.Should().Be("ticket-2");
        snapshot.ReceivedAt.Should().Be(clock.UtcNow, "接收时间须随最新一次有效推送更新");
    }

    /// <summary>时间源为 null ⇒ 构造期 fail-fast（依赖缺失不得推迟到首次刷新才炸）。</summary>
    [Fact]
    public void Ctor_ShouldThrow_WhenClockNull()
    {
        var act = () => new InMemoryComponentVerifyTicketStore(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>可控时钟（本线自带时间源端口的测试替身）。</summary>
    private sealed class FakeClock : IOpenPlatformClock
    {
        private DateTimeOffset _now;

        public FakeClock()
            : this(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero))
        {
        }

        public FakeClock(DateTimeOffset now) => _now = now;

        public DateTimeOffset UtcNow => _now;

        public void Advance(TimeSpan delta) => _now = _now.Add(delta);
    }
}
