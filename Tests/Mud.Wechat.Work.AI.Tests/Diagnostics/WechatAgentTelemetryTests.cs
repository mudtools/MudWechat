using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using FluentAssertions;
using Mud.Wechat.Work.AI.Diagnostics;
using Xunit;

namespace Mud.Wechat.Work.AI.Tests.Diagnostics;

/// <summary>
/// 企微侧 <see cref="IAgentTelemetry"/> 实现：把中立回合管线的埋点入口映射到 <c>wechat.ai.agent.*</c> 命名面。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么这些用例必须存在</b>：遥测是「装错了也照常工作」的典型面——装配顺序颠倒（<c>TryAdd</c> 先注册空实现）
/// 或句柄传错（把 <see cref="Activity"/> 当 <see cref="IDisposable"/> 直接返回、中途标签写丢）都<b>不报错</b>，
/// 只表现为「线上没有数据」。因此逐入口断言：Span 存在、关键标签落在<b>同一个</b> Span 上、指标维度正确。
/// </para>
/// <para>
/// <b>监听器过滤</b>：<c>Mud.Wechat</c> 是全局单根源，同程序集的其它用例会并发产出 Span，故一律按
/// 「会话键」二次过滤，不依赖顺序或计数。
/// </para>
/// </remarks>
public class WechatAgentTelemetryTests
{
    private readonly WechatAgentTelemetry _telemetry = new();

    [Fact]
    public void StartTurn_ShouldEmitSpanCarryingConversationAndTenant()
    {
        using var harness = ActivityHarness.Start();

        using (_telemetry.StartTurn("wechat:corp-app:g:turn-case", "corp-app"))
        {
        }

        var activity = harness.Single("wechat.ai.agent.turn", "wechat:corp-app:g:turn-case");
        activity.GetTagItem("agent.tenant_key").Should().Be("corp-app");
    }

    [Fact]
    public void GateWaited_ShouldTagTheTurnSpan_NotANewInstrument()
    {
        using var harness = ActivityHarness.Start();

        using (var turn = _telemetry.StartTurn("wechat:corp-app:g:gate-case", null))
        {
            _telemetry.GateWaited(turn, 123.5);
        }

        harness.Single("wechat.ai.agent.turn", "wechat:corp-app:g:gate-case")
            .GetTagItem("agent.gate_wait_ms")
            .Should()
            .Be(123.5, "闸门等待是单轮事实，只能进 Span；做成指标会引入会话级高基数维度");
    }

    [Fact]
    public void ReplayedAndStreamed_ShouldMarkTheSameTurnSpan()
    {
        using var harness = ActivityHarness.Start();

        using (var turn = _telemetry.StartTurn("wechat:corp-app:g:flags-case", null))
        {
            _telemetry.Replayed(turn);
            _telemetry.Streamed(turn);
        }

        var activity = harness.Single("wechat.ai.agent.turn", "wechat:corp-app:g:flags-case");
        activity.GetTagItem("agent.replayed").Should().Be(true);
        activity.GetTagItem("agent.streamed").Should().Be(true);
    }

    [Fact]
    public void StartSummarize_ShouldTagCounts_AndUnconvergedOnlyWhenNotConverged()
    {
        using var harness = ActivityHarness.Start();

        using (var scope = _telemetry.StartSummarize(summarizedCount: 12, retainedCount: 3))
        {
            _telemetry.SummarizeCompleted(scope, converged: false);
        }

        var activity = harness.Stopped.Single(a => a.DisplayName == "wechat.ai.agent.summarize");
        activity.GetTagItem("agent.summarized").Should().Be(12);
        activity.GetTagItem("agent.retained").Should().Be(3);
        activity.GetTagItem("agent.summarized_unconverged").Should().Be(true);
    }

    [Fact]
    public void StartRun_ShouldTagStreamingAndTokenUsage()
    {
        using var harness = ActivityHarness.Start();

        using (var run = _telemetry.StartRun("wechat-agent", streaming: true))
        {
            _telemetry.RunCompleted(run, failed: false, inputTokens: 11, outputTokens: 7, totalTokens: 18);
        }

        var activity = harness.Stopped.Single(a => a.DisplayName == "wechat.ai.agent.run");
        activity.GetTagItem("agent").Should().Be("wechat-agent");
        activity.GetTagItem("agent.streaming").Should().Be(true);
        activity.GetTagItem("agent.failed").Should().Be(false);
        activity.GetTagItem("agent.tokens.input").Should().Be(11L);
        activity.GetTagItem("agent.tokens.output").Should().Be(7L);
        activity.GetTagItem("agent.tokens.total").Should().Be(18L);
    }

    [Fact]
    public void RunCompleted_WithFailedRun_ShouldMarkErrorAndOmitMissingTokens()
    {
        using var harness = ActivityHarness.Start();

        using (var run = _telemetry.StartRun("wechat-agent", streaming: false))
        {
            _telemetry.RunCompleted(run, failed: true, inputTokens: null, outputTokens: null, totalTokens: null);
        }

        var activity = harness.Stopped.Single(a => a.DisplayName == "wechat.ai.agent.run");
        activity.Status.Should().Be(ActivityStatusCode.Error);
        activity.GetTagItem("agent.failed").Should().Be(true);
        activity.GetTagItem("agent.tokens.input").Should().BeNull(
            "未上报的用量不得填 0——0 会被读成「本轮确实零 token」");
    }

    [Fact]
    public void LlmDuration_ShouldRecordHistogramWithAgentDimensionOnly()
    {
        using var metrics = MetricHarness.StartDouble("wechat.ai.agent.llm.duration");

        _telemetry.LlmDuration("wechat-agent", 42);

        var sample = metrics.SingleSample();
        sample.Value.Should().Be(42);
        sample.Tags.Should().ContainKey("agent").WhoseValue.Should().Be("wechat-agent");
        sample.Tags.Keys.Should().BeEquivalentTo(["agent"], "租户/会话键不得进指标标签（高基数）");
    }

    [Fact]
    public void Degraded_ShouldCountByReason()
    {
        using var metrics = MetricHarness.StartLong("wechat.ai.agent.degraded");

        _telemetry.Degraded("summary_failed");

        var sample = metrics.SingleLongSample();
        sample.Value.Should().Be(1);
        sample.Tags.Should().ContainKey("reason").WhoseValue.Should().Be("summary_failed");
    }

    [Fact]
    public void WithoutAnyListener_ScopesShouldBeNull()
    {
        // 无监听者时不得分配作用域对象，也不得抛（零监听者零成本）。
        var telemetry = new WechatAgentTelemetry();

        telemetry.StartTurn("wechat:corp-app:g:no-listener", null).Should().BeNull();
        telemetry.StartRun("wechat-agent", streaming: false).Should().BeNull();
        telemetry.StartSummarize(1, 1).Should().BeNull();

        // 句柄为空时各中间标签入口不得抛。
        telemetry.GateWaited(null, 1);
        telemetry.Replayed(null);
        telemetry.Streamed(null);
        telemetry.SummarizeCompleted(null, converged: true);
        telemetry.RunCompleted(null, failed: false, inputTokens: null, outputTokens: null, totalTokens: null);
    }

    /// <summary>按「Span 名 + 会话键」取唯一 Span 的监听器架子。</summary>
    private sealed class ActivityHarness : IDisposable
    {
        private readonly ActivityListener _listener;

        private ActivityHarness()
        {
            _listener = new ActivityListener
            {
                ShouldListenTo = source => string.Equals(source.Name, "Mud.Wechat", StringComparison.Ordinal),
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStopped = activity => Stopped.Add(activity),
            };

            ActivitySource.AddActivityListener(_listener);
        }

        public ConcurrentBag<Activity> Stopped { get; } = [];

        public static ActivityHarness Start() => new();

        public Activity Single(string name, string conversationKey)
            => Stopped
                .Where(a => string.Equals(a.DisplayName, name, StringComparison.Ordinal)
                            && string.Equals(a.GetTagItem("agent.conversation_key") as string, conversationKey, StringComparison.Ordinal))
                .Should()
                .ContainSingle($"必须产出一个名为 {name} 且会话键为 {conversationKey} 的 Span")
                .Which;

        public void Dispose() => _listener.Dispose();
    }

    /// <summary>单仪器指标采集架子（只采一个指标名，避免同一 Meter 下其它仪器干扰）。</summary>
    private sealed class MetricHarness : IDisposable
    {
        private readonly MeterListener _listener;
        private readonly List<(double Value, Dictionary<string, object?> Tags)> _doubleSamples = [];
        private readonly List<(long Value, Dictionary<string, object?> Tags)> _longSamples = [];

        private MetricHarness(string instrumentName)
        {
            _listener = new MeterListener
            {
                InstrumentPublished = (instrument, listener) =>
                {
                    if (string.Equals(instrument.Meter.Name, "Mud.Wechat", StringComparison.Ordinal)
                        && string.Equals(instrument.Name, instrumentName, StringComparison.Ordinal))
                    {
                        listener.EnableMeasurementEvents(instrument);
                    }
                },
            };

            _listener.SetMeasurementEventCallback<double>((instrument, value, tags, _) =>
            {
                if (string.Equals(instrument.Name, instrumentName, StringComparison.Ordinal))
                {
                    lock (_doubleSamples)
                    {
                        _doubleSamples.Add((value, ToDictionary(tags)));
                    }
                }
            });

            _listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
            {
                if (string.Equals(instrument.Name, instrumentName, StringComparison.Ordinal))
                {
                    lock (_longSamples)
                    {
                        _longSamples.Add((value, ToDictionary(tags)));
                    }
                }
            });

            _listener.Start();
        }

        public static MetricHarness StartDouble(string instrumentName) => new(instrumentName);

        public static MetricHarness StartLong(string instrumentName) => new(instrumentName);

        public (double Value, Dictionary<string, object?> Tags) SingleSample()
            => _doubleSamples.Should().ContainSingle().Which;

        public (long Value, Dictionary<string, object?> Tags) SingleLongSample()
            => _longSamples.Should().ContainSingle().Which;

        private static Dictionary<string, object?> ToDictionary(ReadOnlySpan<KeyValuePair<string, object?>> tags)
        {
            var result = new Dictionary<string, object?>(tags.Length, StringComparer.Ordinal);
            foreach (var pair in tags)
            {
                result[pair.Key] = pair.Value;
            }

            return result;
        }

        public void Dispose() => _listener.Dispose();
    }
}
