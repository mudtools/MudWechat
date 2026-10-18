using System.Diagnostics.Metrics;
using FluentAssertions;
using Mud.AI.Agent.Tools;
using Mud.AI.Agent.Tools.Results;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Security;

/// <summary>
/// 运行期治理：单调用超时、并发上限、指标埋点。
/// </summary>
/// <remarks>
/// 三者共同解决同一类问题——<b>工具面在压力下的行为不可观测/不可控</b>：慢接口挂住回合、并行 tool_call 一起撞限流、
/// 线上只能看到「模型变慢」而看不到具体工具与 P95。用例把这三条钉成可回归的行为。
/// </remarks>
public class WechatToolGovernanceTests
{
    private static readonly Dictionary<string, object?> Args = new(StringComparer.Ordinal) { ["department_id"] = 2 };

    [Fact]
    public async Task Invoke_WhenExecutionExceedsCallTimeout_ShouldAbortWithActionableError()
    {
        var dispatcher = Dispatcher(
            policy => policy.CallTimeout = TimeSpan.FromSeconds(1),
            async (_, token) =>
            {
                await Task.Delay(TimeSpan.FromSeconds(30), token);
                return ToolResult.FromText("{}");
            });

        var started = DateTime.UtcNow;
        var result = await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);
        var elapsed = DateTime.UtcNow - started;

        result.Text.Should().Contain("超时");
        result.Text.Should().Contain("缩小查询范围", "超时文案必须给换路建议，否则模型会原样重试");
        elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5), "超时必须真正中止执行，而不是返回文案后后台继续跑");
    }

    [Fact]
    public async Task Invoke_WhenManyCallsInParallel_ShouldRespectConcurrencyLimit()
    {
        var current = 0;
        var peak = 0;
        var gate = new object();

        var dispatcher = Dispatcher(
            policy => policy.MaxConcurrentCalls = 2,
            async (_, token) =>
            {
                lock (gate)
                {
                    current++;
                    peak = Math.Max(peak, current);
                }

                try
                {
                    await Task.Delay(60, token);
                    return ToolResult.FromText("{}");
                }
                finally
                {
                    lock (gate)
                    {
                        current--;
                    }
                }
            });

        var calls = Enumerable.Range(0, 6)
            .Select(_ => dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args))
            .ToArray();
        await Task.WhenAll(calls);

        peak.Should().BeLessThanOrEqualTo(2, "不限并发时一批工具会同时撞企微频率上限，模型再逐个重试放大幅值");
        calls.Should().OnlyContain(call => call.Result.Text.Contains("{}"));
    }

    [Fact]
    public async Task Invoke_ShouldEmitInvocationAndResultMetrics()
    {
        long invocations = 0;
        long bytes = 0;

        using var listener = new MeterListener
        {
            InstrumentPublished = (instrument, l) =>
            {
                if (string.Equals(instrument.Meter.Name, "Mud.Wechat", StringComparison.Ordinal))
                {
                    l.EnableMeasurementEvents(instrument);
                }
            },
        };
        listener.SetMeasurementEventCallback<long>((instrument, value, tags, state) =>
        {
            if (string.Equals(instrument.Name, "wechat.ai.tool.invocations", StringComparison.Ordinal))
            {
                Interlocked.Add(ref invocations, value);
            }
            else if (string.Equals(instrument.Name, "wechat.ai.tool.result_bytes", StringComparison.Ordinal))
            {
                Interlocked.Add(ref bytes, value);
            }
        });
        listener.Start();

        var dispatcher = Dispatcher(_ => { }, (_, _) => Task.FromResult(ToolResult.FromText("{\"ok\":true}")));
        await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        invocations.Should().BeGreaterThan(0, "没有调用量指标时，工具面可用性无法被观测");
        bytes.Should().BeGreaterThan(0, "结果体积分布是预算调优的唯一依据");
    }

    private static WechatToolDispatcher Dispatcher(
        Action<WechatToolPolicy> configure,
        Func<IReadOnlyDictionary<string, object?>, CancellationToken, Task<ToolResult>> execute)
    {
        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            WechatToolNames.ContactsDepartmentGet,
            new WechatToolBinding("corp-app"),
            execute));

        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        configure(policy);
        policy.Validate();

        return new WechatToolDispatcher(registry, policy);
    }
}
