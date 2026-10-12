// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 长连接<b>在途回调分派跟踪器</b>（v1 方案 F5/D7 + D13）：读循环把分派外抛为受追踪任务，
/// 停机时有界收尾 —— 在途任务必须被<b>观察</b>（防 UnobservedTaskException）与被<b>等待</b>（防停机竞态）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么提取为纯逻辑类</b>（D13）：<c>ClientWebSocket</c> 不可伪造，「双慢帧并行 respond &lt; 5s」
/// 的端到端时序用例不可直测；跟踪 / 摘除 / 有界收尾是纯并发逻辑，直测即可覆盖其性质，
/// 「读循环不串行 await 分派」的结构性质由守卫 BT12④ 锁定。
/// </para>
/// <para>
/// <b>并发上界不在这里</b>：容量由分发器既有 <c>_concurrencyGate</c>（<c>MaxConcurrentEvents</c>）
/// 承担（D8 单一并发权威），连接层只做「不阻塞读循环 + 可观察 + 可收尾」。
/// </para>
/// </remarks>
internal sealed class WechatBotDispatchTracker
{
    private readonly List<Task> _inflight = new();
    private readonly Action<Exception> _onDispatchFaulted;
    private readonly Action<Exception> _onDrainIncomplete;

    /// <summary>创建跟踪器。</summary>
    /// <param name="onDispatchFaulted">在途分派故障回调（调用方区分纯取消与真故障定级）。</param>
    /// <param name="onDrainIncomplete">有界收尾未完成回调（界满只告警，不抛）。</param>
    public WechatBotDispatchTracker(Action<Exception> onDispatchFaulted, Action<Exception> onDrainIncomplete)
    {
        _onDispatchFaulted = onDispatchFaulted ?? throw new ArgumentNullException(nameof(onDispatchFaulted));
        _onDrainIncomplete = onDrainIncomplete ?? throw new ArgumentNullException(nameof(onDrainIncomplete));
    }

    /// <summary>登记在途分派（续延负责观察故障与摘除，同步执行不占线程池）。</summary>
    /// <param name="task">分派任务。</param>
    public void Track(Task task)
    {
        lock (_inflight) { _inflight.Add(task); }
        _ = task.ContinueWith(static (t, state) =>
        {
            var self = (WechatBotDispatchTracker)state!;
            if (t.IsFaulted) { self._onDispatchFaulted(t.Exception!); }
            lock (self._inflight) { self._inflight.Remove(t); }
        }, this, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
    }

    /// <summary>
    /// 有界等待全部在途分派完成（界满只告警不抛 —— 连接已关闭，应答帧将发送失败）。
    /// </summary>
    /// <param name="bound">等待上界（停机收尾窗口）。</param>
    /// <remarks>
    /// <para>
    /// <b>归因分工</b>（v1 方案 F5 可观测不变量）：只有 <see cref="TimeoutException"/>（界满）
    /// 上报 <c>onDrainIncomplete</c>；快照内任务的故障 / 取消<b>不得</b>经本路径上报 ——
    /// 故障已由 <see cref="Track"/> 的续延经 <c>onDispatchFaulted</c> 记 Error（取消按设计不报故障），
    /// 此处再报「未在界内完成」属双重归因，且把已瞬间完成（只是故障）的任务说成挂起，误导排障。
    /// </para>
    /// <para>
    /// 故非超时异常静默收尾（不抛、不告警）：收尾语义是「有界等待」，故障可见性由续延保证。
    /// </para>
    /// </remarks>
    public async Task DrainAsync(TimeSpan bound)
    {
        Task[] snapshot;
        lock (_inflight) { snapshot = _inflight.ToArray(); }
        if (snapshot.Length == 0) { return; }
        try { await Task.WhenAll(snapshot).WaitAsync(bound).ConfigureAwait(false); }
        catch (TimeoutException ex)
        {
            _onDrainIncomplete(ex);   // 真·界满：告警不抛（应答帧将发送失败）
        }
        catch (Exception)
        {
            // 快照内任务故障 / 取消：故障已由 Track 续延经 _onDispatchFaulted 上报（Error），
            // 取消不报故障 —— 此处不得双重归因（文案「未在界内完成」对已完成的故障任务是误导）。
        }
    }
}

#endif
