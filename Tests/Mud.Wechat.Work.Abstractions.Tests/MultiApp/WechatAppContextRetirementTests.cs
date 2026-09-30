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
}
