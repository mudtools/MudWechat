// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Callback;
using Mud.Wechat.Work.Callback.LongConnection;
using Mud.Wechat.Work.DataModels.Aibot;
using Mud.Wechat.Work.Callback.Tests;
using System.Net.WebSockets;

namespace Mud.Wechat.Work.Tests.LongConnection;

/// <summary>
/// 长连接的<b>无网络</b>行为测试：帧编解码往返、流式状态机（req_id 复用 / finish 终结 / 10 分钟窗口）、
/// 运行器启动期校验（配置形状 + <b>跨面互斥</b>）、心跳循环（F1）、停机租约释放与退避计数（F2/F13）、
/// 租约续租（F4）。
/// </summary>
/// <remarks>
/// WebSocket 真连接属集成场景（官方 openws 需真实凭据），本文件全部停在编解码与纯逻辑层 ——
/// 与 Finance 批次「不触原生库」的测试分工同源；治理红线（TFM 门控 / 无指纹闸 / 先夺租约）由
/// <c>WechatBotContractGuards</c> 的 BT10 源码断言承担。P0 修复（F1/F2/F4）按 v1 方案 D2 提取为
/// <c>WechatBotHeartbeat</c> / <c>WechatBotLeaseRenewer</c> 纯逻辑类后在本文件直测；
/// 停机释放用例经 <c>FakeLease</c> + 失败 socket 工厂驱动 <c>WechatBotConnection.RunAsync</c> 真实主循环（不触网）。
/// </remarks>
public class WechatBotLongConnectionTests
{
    // ---------------------------------------------------------------- 帧编解码

    [Fact]
    public void FrameCodec_ShouldRoundTripShell_WithOfficialFieldNames()
    {
        var json = WechatBotFrameCodec.EncodeFrame(
            WechatBotFrameCommands.SendMsg,
            "req-1",
            new AibotSendMessageBody { ChatId = "chat-1", ChatType = 2, MsgType = "markdown" },
            AibotJsonContext.Default.AibotSendMessageBody);

        json.Should().Contain("\"cmd\":\"aibot_send_msg\"")
            .And.Contain("\"req_id\":\"req-1\"")
            .And.Contain("\"chat_type\":2")
            .And.Contain("\"chatid\":\"chat-1\"");

        var frame = WechatBotFrameCodec.ParseFrame(json);
        frame.Cmd.Should().Be(WechatBotFrameCommands.SendMsg);
        frame.Headers!.ReqId.Should().Be("req-1");
        frame.Body.Should().NotBeNull();
    }

    [Fact]
    public void FrameCodec_ShouldRejectFrameWithoutCmd()
    {
        var act = () => WechatBotFrameCodec.ParseFrame("""{"headers":{"req_id":"r1"},"errcode":0}""");

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("cmd");
    }

    [Fact]
    public void FrameCodec_ShouldParseAckWithErrcode()
    {
        var ack = WechatBotFrameCodec.ParseAck("""{"headers":{"req_id":"r1"},"errcode":0,"errmsg":"ok"}""");

        ack.IsSuccess.Should().BeTrue();
        ack.Headers!.ReqId.Should().Be("r1");
        ack.ErrMsg.Should().Be("ok");

        WechatBotFrameCodec.ParseAck("""{"headers":{"req_id":"r2"},"errcode":10008,"errmsg":"denied"}""")
            .IsSuccess.Should().BeFalse();
    }

    // ---------------------------------------------------------------- 流式状态机

    [Fact]
    public void StreamRegistry_ShouldReuseFirstReqId_WhenRefreshingSameStream()
    {
        var registry = new WechatBotStreamRegistry();

        registry.BeginOrRefresh("s1", "req-first", finish: false).Should().Be("req-first",
            "首次发送记账本次 req_id");
        registry.BeginOrRefresh("s1", "req-second", finish: false).Should().Be("req-first",
            "官方 101463：同一次回调的所有流式回复须用相同 req_id ⇒ 刷新复用首次的 req_id");
    }

    [Fact]
    public void StreamRegistry_ShouldRejectAfterFinish_OrExpiredWindow()
    {
        var now = DateTimeOffset.UtcNow;
        var registry = new WechatBotStreamRegistry(() => now);

        registry.BeginOrRefresh("s1", "req-1", finish: true);

        var act = () => registry.BeginOrRefresh("s1", "req-2", finish: false);
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("finish");

        // 10 分钟窗口：超窗拒绝（时钟动态推进 —— 首次在 now、刷新在 now+11min）。
        var live = new WechatBotStreamRegistry(() => now);
        live.BeginOrRefresh("s2", "req-1", finish: false);
        live.BeginOrRefresh("s2", "req-2", finish: false)
            .Should().Be("req-1", "窗口内刷新正常");

        var current = now;
        var expired = new WechatBotStreamRegistry(() => current);
        expired.BeginOrRefresh("s3", "req-1", finish: false);
        current = now.AddMinutes(11);
        var actExpired = () => expired.BeginOrRefresh("s3", "req-2", finish: false);
        actExpired.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("10 分钟");
    }

    [Fact]
    public void StreamRegistry_ShouldRejectEmptyStreamId()
    {
        var registry = new WechatBotStreamRegistry();

        var act = () => registry.BeginOrRefresh(string.Empty, "req-1", finish: false);

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("stream.id");
    }

    // ---------------------------------------------------------------- 流式记账保留窗（F4）

    [Fact]
    public void BeginOrRefresh_ShouldThrow_WhenRefreshAfterFinishWithinRetention()
    {
        // 现状锁定：宽限期内（窗口 + 1 分钟内）finish 后再刷仍抛「已终结」——清扫不改变既有语义。
        var now = DateTimeOffset.UtcNow;
        var registry = new WechatBotStreamRegistry(() => now);

        registry.BeginOrRefresh("s1", "req-1", finish: true);

        var act = () => registry.BeginOrRefresh("s1", "req-2", finish: false);
        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("finish");
    }

    [Fact]
    public void BeginOrRefresh_ShouldTreatAsNewStream_WhenIdReusedBeyondRetention()
    {
        // 新边界锁定：时钟推进越过「窗口 + 宽限」后，同 id 被清扫 ⇒ 再刷按新流处理（返回新 req_id）。
        var now = DateTimeOffset.UtcNow;
        var clock = now;
        var registry = new WechatBotStreamRegistry(() => clock);

        registry.BeginOrRefresh("s1", "req-old", finish: false);
        clock = now.AddMinutes(11).AddMinutes(WechatBotStreamRegistry.RetentionSlackMinutes).AddSeconds(1);

        var reqId = registry.BeginOrRefresh("s1", "req-new", finish: false);

        reqId.Should().Be("req-new", "保留窗外同 id 视为新流（历史残迹已清扫，官方窗口早已关闭）");
        registry.BeginOrRefresh("s1", "req-again", finish: false).Should().Be("req-new",
            "新流刷新复用新 req_id（整流同 req_id 契约对新流继续成立）");
    }

    [Fact]
    public void Registry_ShouldNotGrowUnbounded_WhenStreamsAbandonedOrFinished()
    {
        // 泄漏锁定：批量废弃/终结流 + 时钟推进 + 一次新调用触发清扫 ⇒ Count 回落（应用生命周期单例慢泄漏修复）。
        var now = DateTimeOffset.UtcNow;
        var clock = now;
        var registry = new WechatBotStreamRegistry(() => clock);

        for (var i = 0; i < 20; i++)
        {
            registry.BeginOrRefresh($"abandoned-{i}", $"req-{i}", finish: false);   // 废弃流（永不 finish）
            registry.BeginOrRefresh($"finished-{i}", $"req-{i}", finish: true);     // 终结流
        }

        registry.Count.Should().Be(40, "保留窗内条目全部在册（语义与现状一致）");

        clock = now.AddMinutes(11).AddMinutes(WechatBotStreamRegistry.RetentionSlackMinutes).AddSeconds(1);
        registry.BeginOrRefresh("fresh", "req-fresh", finish: false);               // 触发清扫

        registry.Count.Should().Be(1, "超保留窗条目（无论是否 finish）全部被清扫");
    }

    [Fact]
    public void Sweep_ShouldBeThrottled_WhenCalledFrequently()
    {
        // 节流锁定（纯函数判据直接单测）：60 秒内至多一轮；节流窗口到期（含首轮 lastTicks=0）必扫。
        var last = DateTimeOffset.UtcNow.UtcTicks;

        WechatBotStreamRegistry.ShouldSweep(last, last).Should().BeFalse("同刻重复调用不得重复清扫");
        WechatBotStreamRegistry.ShouldSweep(last + TimeSpan.FromSeconds(59).Ticks, last)
            .Should().BeFalse("60 秒节流窗口内不重复清扫");
        WechatBotStreamRegistry.ShouldSweep(last + TimeSpan.FromSeconds(60).Ticks, last)
            .Should().BeTrue("节流窗口到期触发清扫");
        WechatBotStreamRegistry.ShouldSweep(DateTimeOffset.UtcNow.UtcTicks, 0)
            .Should().BeTrue("首轮（lastTicks=0）必扫");
    }

    // ---------------------------------------------------------------- 运行器启动校验

    private static WechatBotOptions NewBotOptions(string botKey = "bot1") => new()
    {
        Bots = new Dictionary<string, WechatBotAppOptions>
        {
            [botKey] = new WechatBotAppOptions { BotId = "BOT-ID", BotSecret = "secret-value", EnableLongConnection = true },
        },
    };

    private static WechatCallbackOptions NewCallbackOptions(string botKey = "bot1") => new();

    [Fact]
    public async Task Runner_StartAsync_ShouldFailFast_WhenBothModesConfiguredForSameBotKey()
    {
        var callbackOptions = NewCallbackOptions();
        callbackOptions.Apps["bot1"] = new WechatAppCallbackOptions
        {
            PushToken = "t",
            PushEncodingAESKey = new string('a', 43),
            AppType = WechatAppType.Internal,
            Channel = WechatCallbackChannel.Bot,
        };

        using var provider = BuildProvider(NewBotOptions(), callbackOptions, out var runner);

        Func<Task> act = () => runner.StartAsync(CancellationToken.None);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("二选一")
            .And.Contain("bot1", "官方 101463：长连接 / 回调地址切换即失效 ⇒ 同 BotKey 双配置必须启动期点名");
    }

    [Fact]
    public async Task Runner_StartAsync_ShouldPass_WhenOnlyLongConnectionConfigured()
    {
        using var provider = BuildProvider(NewBotOptions(), NewCallbackOptions(), out var runner);

        await runner.StartAsync(new CancellationTokenSource(50).Token);
        // StartAsync 通过即跨面互斥校验通过（ExecuteAsync 在取消后自然结束）。
    }

    [Fact]
    public async Task Runner_ShouldValidateBotOptions_WhenBotsMissing()
    {
        var runner = new WechatBotLongConnectionRunner(
            new WechatBotOptions(),
            NewCallbackOptions(),
            CreateDispatcher(),
            lease: null,
            NullLogger<WechatBotLongConnectionRunner>.Instance);

        Func<Task> act = () => runner.StartAsync(CancellationToken.None);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("Bots");
    }

    [Fact]
    public async Task StartAsync_ShouldThrow_WhenWildcardBotChannelCoexistsWithLongConnection()
    {
        // R-P3（架构审查报告 v2 P3-3）：通配键「*」条目经 ResolveApp 通配回退可承接任意 botKey 的
        // Bot 回调 ⇒ 与长连接并存时跨面互斥失效。Validate() 对通配键跳过通道校验，互斥校验必须显式覆盖。
        var botOptions = NewBotOptions();
        var callbackOptions = NewCallbackOptions();
        callbackOptions.Apps[WechatCallbackOptions.WildcardAppKey] = new WechatAppCallbackOptions
        {
            PushToken = "token-value",
            PushEncodingAESKey = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLM",
            Channel = WechatCallbackChannel.Bot,
        };
        var runner = new WechatBotLongConnectionRunner(
            botOptions,
            callbackOptions,
            CreateDispatcher(),
            lease: null,
            NullLogger<WechatBotLongConnectionRunner>.Instance);

        Func<Task> act = () => runner.StartAsync(CancellationToken.None);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("通配")
            .And.Contain("互斥", "通配 Bot 通道与长连接并存必须在启动期 fail-fast");
    }

    // ---------------------------------------------------------------- 心跳（F1）

    [Fact]
    public async Task Heartbeat_ShouldKeepPinging_WhenNoTrafficArrives()
    {
        var pings = 0;
        using var cts = new CancellationTokenSource();
        var heartbeat = WechatBotHeartbeat.RunAsync(
            TimeSpan.FromMilliseconds(50),
            _ => { Interlocked.Increment(ref pings); return Task.CompletedTask; },
            (_, _) => Assert.Fail("心跳发送不应失败"),
            cts.Token);

        await Task.Delay(250);
        await cts.CancelAsync();
        await heartbeat;

        pings.Should().BeGreaterThanOrEqualTo(3,
            "心跳与流量正交：无帧到达也必须按固定周期发 ping（旧实现被流量 CancelAfter 杀死，v1 方案 F1）");
    }

    [Fact]
    public async Task Heartbeat_ShouldStop_WhenTokenCancelled()
    {
        var pings = 0;
        using var cts = new CancellationTokenSource();
        var heartbeat = WechatBotHeartbeat.RunAsync(
            TimeSpan.FromMilliseconds(20),
            _ => { Interlocked.Increment(ref pings); return Task.CompletedTask; },
            (_, _) => { },
            cts.Token);

        await Task.Delay(60);
        await cts.CancelAsync();
        await heartbeat;   // 取消即静默退出，不抛

        var afterCancel = pings;
        await Task.Delay(100);
        pings.Should().Be(afterCancel, "取消后不得再有 ping（心跳循环必须可停）");
    }

    // ---------------------------------------------------------------- 停机释放与退避计数（F2 / F13）

    [Fact]
    public async Task RunAsync_ShouldReleaseLease_WhenCancelledDuringAcquireBackoff()
    {
        var lease = new FakeLease { AcquireResult = false };
        var connection = NewConnection(lease, static () => throw new InvalidOperationException("未夺得租约不得建连"));

        using var cts = new CancellationTokenSource();
        var run = connection.RunAsync(cts.Token);
        await Task.Delay(150);            // 进入夺约失败退避（60s）
        await cts.CancelAsync();
        await run;                        // 取消不得穿出 RunAsync

        lease.ReleaseCalls.Should().Be(1, "取消落在夺约退避上也必须走 finally 释放租约");
    }

    [Fact]
    public async Task RunAsync_ShouldReleaseLease_WhenCancelledDuringSessionBackoff()
    {
        // F2 回归（先红后绿）：取消落在「会话失败后的退避」上 —— 旧实现该退避在 try/finally 外，
        // OCE 穿出 RunAsync ⇒ ReleaseAsync 被跳过（优雅停机不让位）。
        var lease = new FakeLease { AcquireResult = true };
        var connection = NewConnection(lease, () => throw new InvalidOperationException("会话失败（测试替身）"));

        using var cts = new CancellationTokenSource();
        var run = connection.RunAsync(cts.Token);
        await Task.Delay(150);            // 会话已失败，处于 60s 退避中
        await cts.CancelAsync();
        await run;                        // 不抛

        lease.ReleaseCalls.Should().Be(1, "停机必须释放租约（释放路径唯一化：全部出口汇入唯一 finally）");
    }

    [Fact]
    public async Task RunAsync_ShouldNotThrowAndKeepRetrying_WhenLeaseAcquireThrowsStorageException()
    {
        // R-P1（架构审查报告 v2 P1-2）：租约存储故障（Redis 实现抛 WechatRedisException——
        // 继承 InvalidOperationException 族）按「未夺得」退避重试，不得穿出 RunAsync
        // （BackgroundService 默认 StopHost 会打停宿主，违背「尽力维持」注释契约）。
        // 旧实现该异常直接穿出 ⇒ 本用例对旧代码必红（await run 即抛）。
        var lease = new FakeLease
        {
            AcquireException = new InvalidOperationException("Redis 抖动（模拟 WechatRedisException 形态）"),
        };
        var connection = new WechatBotConnection(
            "bot1",
            new WechatBotAppOptions
            {
                BotId = "BOT-ID",
                BotSecret = "secret-value",
                ReconnectBaseDelayMs = 30,
                ReconnectMaxDelayMs = 60,
            },
            CreateDispatcher(),
            lease,
            static () => throw new InvalidOperationException("夺取异常不得建连"),
            "inst1",
            NullLogger<WechatBotConnection>.Instance);

        using var cts = new CancellationTokenSource();
        var run = connection.RunAsync(cts.Token);
        await Task.Delay(200);            // 多轮「夺取失败 → 退避」
        lease.AcquireCalls.Should().BeGreaterThanOrEqualTo(2, "存储故障按未夺得处理并持续重试（不得打停宿主）");

        await cts.CancelAsync();
        await run;                        // 不抛（取消不得穿出）
        lease.ReleaseCalls.Should().Be(1, "停机释放路径不变");
    }

    [Fact]
    public async Task RunAsync_ShouldCountBackoffOncePerRound_WhenSessionKeepsFailing()    {
        // F13 回归：attempt 每轮至多一次自增 —— 旧实现 finally + 底部双自增，日志跳号（第 1 次 ⇒ 第 3 次）。
        var lease = new FakeLease { AcquireResult = true };
        var logger = new CapturingLogger<WechatBotConnection>();
        var connection = new WechatBotConnection(
            "bot1",
            new WechatBotAppOptions
            {
                BotId = "BOT-ID",
                BotSecret = "secret-value",
                ReconnectBaseDelayMs = 30,
                ReconnectMaxDelayMs = 60,
            },
            CreateDispatcher(),
            lease,
            () => throw new InvalidOperationException("会话失败（测试替身）"),
            "inst1",
            logger);

        using var cts = new CancellationTokenSource();
        var run = connection.RunAsync(cts.Token);
        await Task.Delay(400);            // 多轮快速失败
        await cts.CancelAsync();
        await run;

        var attempts = logger.Entries
            .Select(static e => System.Text.RegularExpressions.Regex.Match(e.Message, @"第 (\d+) 次退避"))
            .Where(static m => m.Success)
            .Select(static m => int.Parse(m.Groups[1].Value))
            .ToArray();

        attempts.Should().NotBeEmpty("每轮会话失败都应记一次退避");
        attempts.Should().BeEquivalentTo(
            Enumerable.Range(1, attempts.Length),
            "退避计数严格递增无跳号（每轮至多一次自增）");
    }

    // ---------------------------------------------------------------- 租约续租（F4）

    [Fact]
    public async Task LeaseRenewer_ShouldRenewPeriodically_UntilCancelled()
    {
        var lease = new FakeLease { RenewResult = true };
        using var cts = new CancellationTokenSource();

        var renewal = WechatBotLeaseRenewer.RunAsync(
            lease, "bot1", "inst1", TimeSpan.FromMilliseconds(150), cts.Token);
        await Task.Delay(400);
        await cts.CancelAsync();
        await renewal;   // 取消静默退出不抛

        lease.RenewCalls.Should().BeGreaterThanOrEqualTo(2, "续租周期 = TTL/3（150ms TTL ⇒ 50ms 周期）");
    }

    [Fact]
    public async Task LeaseRenewer_ShouldThrowLeaseLost_WhenRenewRejected()
    {
        var lease = new FakeLease { RenewResult = false };
        using var cts = new CancellationTokenSource();

        Func<Task> act = () => WechatBotLeaseRenewer.RunAsync(
            lease, "bot1", "inst1", TimeSpan.FromMilliseconds(50), cts.Token);

        await act.Should().ThrowAsync<LeaseLostException>("续租被拒 = 租约已易主，必须快速失败（不挂起）");
    }

    [Fact]
    public async Task LeaseRenewer_ShouldExitSilently_WhenTokenCancelled()
    {
        var lease = new FakeLease { RenewResult = true };
        using var cts = new CancellationTokenSource();

        var renewal = WechatBotLeaseRenewer.RunAsync(
            lease, "bot1", "inst1", TimeSpan.FromSeconds(10), cts.Token);
        await cts.CancelAsync();
        await renewal;

        lease.RenewCalls.Should().Be(0, "取消即静默退出，不发续租");
    }

    // ---------------------------------------------------------------- 在途分派跟踪（F5 / LC-7）

    [Fact]
    public async Task DispatchTracker_ShouldObserveFaultedTask_AndRemoveIt()
    {
        var faulted = new List<Exception>();
        var drained = new List<Exception>();
        var tracker = new WechatBotDispatchTracker(faulted.Add, drained.Add);

        var ok = Task.CompletedTask;
        var boom = Task.FromException(new InvalidOperationException("分派故障（测试替身）"));
        tracker.Track(ok);
        tracker.Track(boom);

        await tracker.DrainAsync(TimeSpan.FromSeconds(2));

        // 回调收到的是 Task.Exception（AggregateException 包装）—— 连接侧 IsPureCancellation 据此分级。
        faulted.Should().ContainSingle().Which.Should().BeOfType<AggregateException>()
            .Which.InnerException.Should().BeOfType<InvalidOperationException>();
        drained.Should().BeEmpty("正常完成不得触发收尾超界告警");
    }

    [Fact]
    public async Task DispatchTracker_ShouldReturnOnBound_WhenTaskStillRunning()
    {
        var faulted = new List<Exception>();
        var drained = new List<Exception>();
        var tracker = new WechatBotDispatchTracker(faulted.Add, drained.Add);

        var never = new TaskCompletionSource();
        tracker.Track(never.Task);

        await tracker.DrainAsync(TimeSpan.FromMilliseconds(200));

        drained.Should().ContainSingle("界满只告警不抛（连接已关闭，应答帧将发送失败）");
        faulted.Should().BeEmpty("挂起不是故障");
    }

    [Fact]
    public async Task DispatchTracker_ShouldNotReportCancelledTask_AsFault()
    {
        // 取消任务（IsCanceled）不报故障；仅包装形态的 OCE 故障会被上报 ——
        // 连接侧 LogDispatchFaulted 据 IsPureCancellation 将其降级为 Debug（F6：停机不制造 Error 噪声）。
        var faulted = new List<Exception>();
        var drained = new List<Exception>();
        var tracker = new WechatBotDispatchTracker(faulted.Add, drained.Add);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        tracker.Track(Task.FromCanceled(cts.Token));
        tracker.Track(Task.FromException(new AggregateException(new OperationCanceledException())));

        await tracker.DrainAsync(TimeSpan.FromSeconds(2));

        faulted.Should().ContainSingle("纯取消任务不上报；包装形态 OCE 上报后由连接侧降级为 Debug");
        drained.Should().BeEmpty();
    }

    [Fact]
    public async Task DrainAsync_ShouldNotAttributeTaskFault_AsDrainIncomplete()
    {
        // 归因分工（v1 方案 F5 可观测不变量）：快照内任务在收尾窗口内故障 ⇒ 只经故障回调记 Error，
        // 不得再触发「收尾未完成」告警 —— 旧实现 catch (Exception) 一律送 onDrainIncomplete，
        // 把已瞬间完成（只是故障）的任务说成「未在界内完成」，误导排障（双重归因）。
        var faulted = new List<Exception>();
        var drained = new List<Exception>();
        var tracker = new WechatBotDispatchTracker(faulted.Add, drained.Add);

        var pending = new TaskCompletionSource();
        tracker.Track(pending.Task);

        var drain = tracker.DrainAsync(TimeSpan.FromSeconds(5));   // 界远大于故障时刻
        await Task.Delay(50);
        pending.SetException(new InvalidOperationException("分派故障（测试替身）"));
        await drain;                                              // 不得抛（收尾不抛）

        faulted.Should().ContainSingle("窗口内故障必须经故障回调上报（Error 归因，续延职责）")
            .Which.Should().BeOfType<AggregateException>();
        drained.Should().BeEmpty("窗口内完成（哪怕是故障）不是收尾超界（TimeoutException 才是）");
    }

    [Fact]
    public async Task DispatchTracker_ShouldDrainEmpty_Immediately()
    {
        var tracker = new WechatBotDispatchTracker(_ => { }, _ => { });

        await tracker.DrainAsync(TimeSpan.FromMilliseconds(50));   // 空在途立即返回，不抛
    }

    // ---------------------------------------------------------------- 测试基建

    private static WechatBotConnection NewConnection(FakeLease lease, Func<ClientWebSocket> webSocketFactory)
        => new(
            "bot1",
            new WechatBotAppOptions
            {
                BotId = "BOT-ID",
                BotSecret = "secret-value",
                ReconnectBaseDelayMs = 60_000,
                ReconnectMaxDelayMs = 60_000,
            },
            CreateDispatcher(),
            lease,
            webSocketFactory,
            "inst1",
            NullLogger<WechatBotConnection>.Instance);

    /// <summary>租约测试替身（内存计数，无网络）。</summary>
    private sealed class FakeLease : IWechatBotConnectionLease
    {
        public int AcquireCalls { get; private set; }

        public int RenewCalls { get; private set; }

        public int ReleaseCalls { get; private set; }

        public bool AcquireResult { get; set; } = true;

        public bool RenewResult { get; set; } = true;

        /// <summary>非空时 <see cref="TryAcquireAsync"/> 抛该异常（模拟 Redis 存储故障形态）。</summary>
        public Exception? AcquireException { get; set; }

        public Task<bool> TryAcquireAsync(string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            AcquireCalls++;
            if (AcquireException != null)
            {
                throw AcquireException;
            }

            return Task.FromResult(AcquireResult);
        }

        public Task<bool> RenewAsync(string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default)
        {
            RenewCalls++;
            return Task.FromResult(RenewResult);
        }

        public Task<bool> ReleaseAsync(string botKey, string instanceId, CancellationToken cancellationToken = default)
        {
            ReleaseCalls++;
            return Task.FromResult(true);
        }
    }

    private static ServiceProvider BuildProvider(
        WechatBotOptions botOptions, WechatCallbackOptions callbackOptions, out WechatBotLongConnectionRunner runner)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Options.Create(botOptions));
        services.AddSingleton(Options.Create(callbackOptions));
        services.AddSingleton(botOptions);
        services.AddSingleton(callbackOptions);

        var dispatcher = CreateDispatcher();
        services.AddSingleton(dispatcher);

        var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        runner = new WechatBotLongConnectionRunner(
            botOptions,
            callbackOptions,
            dispatcher,
            lease: null,
            NullLogger<WechatBotLongConnectionRunner>.Instance);
        return provider;
    }

    private static WechatBotEventDispatcher CreateDispatcher()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(new WechatBotHandlerRegistry());
        var provider = services.BuildServiceProvider();
        return new WechatBotEventDispatcher(
            provider.GetRequiredService<WechatBotHandlerRegistry>(),
            new TestOptionsMonitor<WechatCallbackOptions>(new WechatCallbackOptions()),
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<WechatBotEventDispatcher>.Instance);
    }
}

#endif
