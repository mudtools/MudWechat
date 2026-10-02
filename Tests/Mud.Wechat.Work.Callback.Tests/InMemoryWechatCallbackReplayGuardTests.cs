// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 进程内指纹守卫测试：窗口语义（一次性标记）、空键 fail-closed、自适应清理间隔（P2-1）。
/// </summary>
public class InMemoryWechatCallbackReplayGuardTests
{
    [Fact]
    public async Task TryMarkAsync_ShouldReturnTrueOnce_WithinWindow()
    {
        var guard = new InMemoryWechatCallbackReplayGuard();

        var first = await guard.TryMarkAsync("fp-1", TimeSpan.FromMinutes(5));
        var second = await guard.TryMarkAsync("fp-1", TimeSpan.FromMinutes(5));

        first.Should().BeTrue("首次消费应成功");
        second.Should().BeFalse("窗口内重复标记必须失败（P0-2 第二道闸语义）");
    }

    [Fact]
    public async Task TryMarkAsync_ShouldFailClosed_WhenKeyEmpty()
    {
        var guard = new InMemoryWechatCallbackReplayGuard();

        (await guard.TryMarkAsync(string.Empty, TimeSpan.FromMinutes(5))).Should().BeFalse(
            "空键无法去重，fail-closed（避免「空键互相覆盖」误放行）");
    }

    [Fact]
    public async Task TryMarkAsync_ShouldCleanup_AdaptiveInterval()
    {
        // P2-1：存量 > 1024 条时触发间隔 = Count/16（>64）。先灌入 3000 条短窗口条目并等待过期，
        // 再以长窗口条目驱动自适应清理；过期条目被回收后同键重标记应成功（若未回收则为 false）。
        var guard = new InMemoryWechatCallbackReplayGuard();

        for (var i = 0; i < 3000; i++)
        {
            (await guard.TryMarkAsync($"expired-{i}", TimeSpan.FromMilliseconds(1))).Should().BeTrue();
        }

        await Task.Delay(50); // 等待灌入条目全部过期

        for (var i = 0; i < 2000; i++)
        {
            await guard.TryMarkAsync($"fresh-{i}", TimeSpan.FromMinutes(5));
        }

        (await guard.TryMarkAsync("expired-0", TimeSpan.FromMinutes(5)))
            .Should().BeTrue("过期条目应在自适应间隔内被机会式回收（固定 64 间隔会在 3000 存量下形成清理风暴）");
    }

    [Fact]
    public async Task TryMarkAsync_ShouldKeepWindowSemantics_RegardlessOfCleanup()
    {
        // 语义安全（P2-1 论证）：未过期条目不受清理节奏影响；清理只移除已过期条目。
        var guard = new InMemoryWechatCallbackReplayGuard();

        for (var i = 0; i < 100; i++)
        {
            await guard.TryMarkAsync($"other-{i}", TimeSpan.FromMinutes(5));
        }

        (await guard.TryMarkAsync("kept", TimeSpan.FromMinutes(5))).Should().BeTrue();
        (await guard.TryMarkAsync("kept", TimeSpan.FromMinutes(5))).Should().BeFalse(
            "窗口内重复标记与清理节奏无关，恒为 false");
    }
}
