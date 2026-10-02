// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

namespace Mud.Wechat.Work.Abstractions.Tests.MultiApp;

/// <summary>
/// 退役队列（P1-9 / P2-8）：宽限期后释放上下文、异步清理任务（失败隔离）、重入闸与停机确定性释放。
/// </summary>
public class WechatAppContextRetirementTests
{
    private static async Task<bool> WaitAsync(Task task, int timeoutMs = 3000)
        => await Task.WhenAny(task, Task.Delay(timeoutMs)).ConfigureAwait(false) == task;

    [Fact]
    public void Enqueue_ShouldDisposeImmediately_WhenNoGracePeriod()
    {
        using var retirement = new WechatAppContextRetirement(0, NullLogger.Instance);
        var context = new Mock<IWechatAppContext>();

        retirement.Enqueue("a", context.Object);

        context.Verify(c => c.Dispose(), Times.Once, "宽限期为 0 时应立即确定性释放");
    }

    [Fact]
    public void Enqueue_ShouldDeferDispose_UntilPump()
    {
        using var retirement = new WechatAppContextRetirement(1, NullLogger.Instance);
        var context = new Mock<IWechatAppContext>();

        retirement.Enqueue("a", context.Object);

        context.Verify(c => c.Dispose(), Times.Never, "宽限期内不得释放（保护在途请求）");
    }

    [Fact]
    public async Task EnqueueCleanup_ShouldRun_WhenNoGracePeriod()
    {
        using var retirement = new WechatAppContextRetirement(0, NullLogger.Instance);
        var ran = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        retirement.EnqueueCleanup("a", _ =>
        {
            ran.TrySetResult(true);
            return Task.CompletedTask;
        });

        (await WaitAsync(ran.Task)).Should().BeTrue("无巡检泵路径须以 fire-and-forget + 异常观察执行清理");
    }

    [Fact]
    public async Task EnqueueCleanup_ShouldIsolateFailures()
    {
        using var retirement = new WechatAppContextRetirement(0, NullLogger.Instance);
        var second = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        // 第一条抛异常不得阻断后续条目（P1-9：清库失败仅告警）。
        retirement.EnqueueCleanup("a", _ => throw new InvalidOperationException("purge boom"));
        retirement.EnqueueCleanup("b", _ =>
        {
            second.TrySetResult(true);
            return Task.CompletedTask;
        });

        (await WaitAsync(second.Task)).Should().BeTrue();
    }

    [Fact]
    public async Task Dispose_ShouldRunPendingCleanup_Deterministically()
    {
        var retirement = new WechatAppContextRetirement(300, NullLogger.Instance);
        var ran = false;

        retirement.EnqueueCleanup("a", _ =>
        {
            ran = true;
            return Task.CompletedTask;
        });

        retirement.Dispose();

        ran.Should().BeTrue("停机路径必须确定性执行在队清理任务（不等待宽限期）");
        await Task.CompletedTask;
    }

    // ---------------------------------------------------------------- M3（F3）：停机竞态闸

    [Fact]
    public void Enqueue_ShouldDisposeContextImmediately_AfterDispose()
    {
        var retirement = new WechatAppContextRetirement(300, NullLogger.Instance);
        var context = new Mock<IWechatAppContext>();

        retirement.Dispose();
        retirement.Enqueue("a", context.Object);

        // M3：停机后队列已停摆且上下文已脱离注册表，不入队即永久泄漏——立即 Dispose 兜底。
        context.Verify(c => c.Dispose(), Times.Once, "停机后到达的入队必须立即确定性释放（F3 停机竞态闸）");
    }

    [Fact]
    public void Enqueue_ShouldNotEnqueue_WhenDisposed_ContextNull()
    {
        var retirement = new WechatAppContextRetirement(300, NullLogger.Instance);
        retirement.Dispose();

        var act = () => retirement.Enqueue("a", null!);
        act.Should().NotThrow("null 上下文入队为安全空操作");
    }

    // ---------------------------------------------------------------- M5（F5）：清库脱泵 + 停机限时排空

    [Fact]
    public async Task Pump_ShouldNotBlockSubsequentContextEntries_WhenCleanupSlow()
    {
        using var retirement = new WechatAppContextRetirement(1, NullLogger.Instance);
        var cleanupGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var contextDisposed = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var context = new Mock<IWechatAppContext>();
        context.Setup(c => c.Dispose()).Callback(() => contextDisposed.TrySetResult(true));

        // 两条目同时到期：慢清库在前、上下文在后。旧实现泵内联 await 清库 ⇒ 上下文 Dispose 被阻塞；
        // M5 后清库脱泵，同一轮巡检内上下文条目即被处置。
        retirement.EnqueueCleanup("slow", _ => cleanupGate.Task);
        retirement.Enqueue("a", context.Object);

        (await WaitAsync(contextDisposed.Task, timeoutMs: 8000)).Should().BeTrue(
            "慢/悬挂清库不得阻塞同队列后续上下文的确定性 Dispose（F5）");
        context.Verify(c => c.Dispose(), Times.Once);

        // 收尾放行挂起的清库任务，避免测试残留飞行任务。
        cleanupGate.TrySetResult(true);
    }

    [Fact]
    public async Task Dispose_ShouldReturnWithinInjectedTimeout_WhenCleanupHangs()
    {
        var retirement = new WechatAppContextRetirement(300, NullLogger.Instance, TimeSpan.FromMilliseconds(200));
        var cleanupGate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        // 宽限期 300s ⇒ 清库在队（未起飞）；停机排空先起飞再限时等待（MR6）。
        retirement.EnqueueCleanup("hang", _ => cleanupGate.Task);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        retirement.Dispose();
        sw.Stop();

        sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5),
            "MR11：停机限时（注入 200ms）内必须返回，存储悬挂不得阻塞进程退出");
        sw.Elapsed.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(180),
            "停机排空应实际等待注入的限时（起飞 + Wait）");

        // 收尾放行挂起的清库任务。
        cleanupGate.TrySetResult(true);
        await Task.CompletedTask;
    }

    [Fact]
    public async Task Dispose_ShouldReleaseQueuedContext_AndWaitInFlightCleanup()
    {
        var retirement = new WechatAppContextRetirement(300, NullLogger.Instance, TimeSpan.FromSeconds(2));
        var context = new Mock<IWechatAppContext>();
        var cleanupDone = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        retirement.Enqueue("a", context.Object);
        retirement.EnqueueCleanup("a", _ => cleanupDone.Task);

        retirement.Dispose();

        context.Verify(c => c.Dispose(), Times.Once, "停机排空必须确定性释放在队上下文");
        cleanupDone.Task.IsCompleted.Should().BeFalse("本用例的清库任务为悬挂形态，仅验证限时等待不抛");

        cleanupDone.TrySetResult(true);
        await Task.CompletedTask;
    }
}
