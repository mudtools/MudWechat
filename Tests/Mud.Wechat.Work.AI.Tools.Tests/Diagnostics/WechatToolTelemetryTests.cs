using System.Diagnostics.Metrics;
using FluentAssertions;
using Moq;
using Mud.AI.Abstractions.Observability;
using Mud.AI.Agent.Tools;
using Mud.AI.Agent.Tools.Errors;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Diagnostics;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Diagnostics;

/// <summary>
/// 工具面遥测收编：执行链<b>只经中立端口</b>计数，端口内部落到既有 <c>wechat.ai.tool.*</c> 命名面；
/// 未进入执行的分支（策略拒绝、审批拒绝、预算超限）不得计入调用次数。
/// </summary>
/// <remarks>
/// <para>
/// <b>回归点（指标翻倍）</b>：若执行链与端口各记一次调用，同一批调用会出现两倍数字，且两边维度不同
/// （端口只有读写布尔、执行链有三档风险）——仪表盘与告警会长期自相矛盾，且没有任何报错。
/// </para>
/// <para>
/// <b>风险档还原</b>：端口只给 <c>isWrite</c>，产品三档（含 <c>high-risk-write</c>）由契约表在端口实现内还原，
/// 故 <c>risk</c> 维度不得退化。
/// </para>
/// </remarks>
[Collection("Serial-Metrics")]
public class WechatToolTelemetryTests
{
    [Fact]
    public void ToolInvoked_ShouldRecordExactlyOneInvocation_WithContractRisk()
    {
        using var metrics = MetricHarness.StartLong("wechat.ai.tool.invocations");

        WechatToolTelemetry.Instance.ToolInvoked(
            WechatToolNames.ContactsDepartmentGet,
            isWrite: false,
            durationMs: 12,
            truncated: false,
            errorCategory: null);

        var sample = metrics.SingleSample(WechatToolNames.ContactsDepartmentGet);
        sample.Value.Should().Be(1, "同一次调用只能计一次（端口 + 执行链双记即翻倍）");
        sample.Tags.Should().ContainKey("risk").WhoseValue.Should().Be("read");
        sample.Tags.Should().ContainKey("outcome").WhoseValue.Should().Be("succeeded");
    }

    [Fact]
    public void ToolInvoked_WithErrorCategory_ShouldRecordFailedOutcome()
    {
        using var metrics = MetricHarness.StartLong("wechat.ai.tool.invocations");

        WechatToolTelemetry.Instance.ToolInvoked(
            WechatToolNames.ContactsTagCreate,
            isWrite: true,
            durationMs: 5,
            truncated: false,
            errorCategory: "invalid");

        var sample = metrics.SingleSample(WechatToolNames.ContactsTagCreate);
        sample.Tags.Should().ContainKey("outcome").WhoseValue.Should().Be("failed");
        sample.Tags.Should().ContainKey("risk").WhoseValue.Should().BeOneOf(
            "write",
            "high-risk-write",
            "写工具的风险档由契约表还原，不得退化为读写两档");
    }

    [Fact]
    public async Task Dispatcher_ShouldReportExecutedCallThroughPort()
    {
        var telemetry = new Mock<IAgentToolTelemetry>(MockBehavior.Loose);
        var dispatcher = Dispatcher(
            WechatToolNames.ContactsDepartmentGet,
            _ => Task.FromResult(ToolResultPipeline.Ok("{\"ok\":true}", 0)),
            telemetry.Object);

        await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        telemetry.Verify(
            t => t.ToolInvoked(WechatToolNames.ContactsDepartmentGet, false, It.IsAny<long>(), false, null),
            Times.Once);
    }

    [Fact]
    public async Task Dispatcher_WhenRetried_ShouldReportToolRetried()
    {
        var telemetry = new Mock<IAgentToolTelemetry>(MockBehavior.Loose);
        var calls = 0;
        var dispatcher = Dispatcher(
            WechatToolNames.ContactsDepartmentGet,
            _ => ++calls == 1
                ? Task.FromResult(Retryable(WechatToolNames.ContactsDepartmentGet))
                : Task.FromResult(ToolResultPipeline.Ok("{\"id\":2}", 0)),
            telemetry.Object);

        await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        telemetry.Verify(t => t.ToolRetried(WechatToolNames.ContactsDepartmentGet, 1), Times.Once);
        telemetry.Verify(
            t => t.ToolInvoked(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<string?>()),
            Times.Once,
            "重试同一轮只结算一次调用（每次失败重试不是一次工具调用）");
    }

    [Fact]
    public async Task Dispatcher_WhenPolicyDenied_ShouldNotReportToolInvoked()
    {
        var telemetry = new Mock<IAgentToolTelemetry>(MockBehavior.Loose);
        var policy = new WechatToolPolicy();
        policy.Validate();

        // 未启用即按策略拒绝：工具没有执行，不能被计入调用次数。
        var dispatcher = new WechatToolDispatcher(
            Registry(WechatToolNames.ContactsDepartmentGet, _ => Task.FromResult(ToolResultPipeline.Ok("{}", 0))),
            policy,
            telemetry: telemetry.Object);

        await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        telemetry.Verify(
            t => t.ToolInvoked(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Fact]
    public async Task Dispatcher_WithApprovalGate_ShouldReportApprovalRequestedAndResolved()
    {
        var telemetry = new Mock<IAgentToolTelemetry>(MockBehavior.Loose);
        var policy = new WechatToolPolicy { RequireApprovalForWrites = true };
        policy.Enable(WechatToolNames.ContactsTagCreate);
        policy.Validate();

        var gate = new Mock<IWechatToolApprovalGate>(MockBehavior.Loose);
        gate.Setup(g => g.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(WechatToolApprovalDecision.Approve("管理员在工单 #1 中批准")));

        var dispatcher = new WechatToolDispatcher(
            Registry(WechatToolNames.ContactsTagCreate, _ => Task.FromResult(ToolResultPipeline.Ok("{\"tagid\":1}", 0))),
            policy,
            authorizer: AllowAll.Instance,
            approvalGate: gate.Object,
            telemetry: telemetry.Object);

        await dispatcher.InvokeAsync(WechatToolNames.ContactsTagCreate, Args);

        telemetry.Verify(t => t.ApprovalRequested(WechatToolNames.ContactsTagCreate), Times.Once);
        telemetry.Verify(
            t => t.ApprovalResolved(WechatToolNames.ContactsTagCreate, true, It.IsAny<long>()),
            Times.Once,
            "确认结果与等待时长只有执行链知道，必须在此上报（HITL 健康度）");
    }

    [Fact]
    public async Task Dispatcher_WhenApprovalRejected_ShouldReportRejectionAndKeepToolUnexecuted()
    {
        var telemetry = new Mock<IAgentToolTelemetry>(MockBehavior.Loose);
        var executed = false;
        var policy = new WechatToolPolicy { RequireApprovalForWrites = true };
        policy.Enable(WechatToolNames.ContactsTagCreate);
        policy.Validate();

        var gate = new Mock<IWechatToolApprovalGate>(MockBehavior.Loose);
        gate.Setup(g => g.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(WechatToolApprovalDecision.Reject("业务方拒绝")));

        var dispatcher = new WechatToolDispatcher(
            Registry(WechatToolNames.ContactsTagCreate, _ =>
            {
                executed = true;
                return Task.FromResult(ToolResultPipeline.Ok("{}", 0));
            }),
            policy,
            authorizer: AllowAll.Instance,
            approvalGate: gate.Object,
            telemetry: telemetry.Object);

        await dispatcher.InvokeAsync(WechatToolNames.ContactsTagCreate, Args);

        executed.Should().BeFalse("审批拒绝 ⇒ 工具不得执行（fail-closed）");
        telemetry.Verify(t => t.ApprovalResolved(WechatToolNames.ContactsTagCreate, false, It.IsAny<long>()), Times.Once);
        telemetry.Verify(
            t => t.ToolInvoked(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<string?>()),
            Times.Never,
            "未执行的调用不得计入调用次数（否则错误率与调用量的分母都会失真）");
    }

    private static readonly Dictionary<string, object?> Args = new(StringComparer.Ordinal) { ["department_id"] = 2 };

    private static WechatToolDispatcher Dispatcher(
        string toolName,
        Func<IReadOnlyDictionary<string, object?>, Task<ToolResult>> execute,
        IAgentToolTelemetry telemetry)
    {
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();
        return new WechatToolDispatcher(Registry(toolName, execute), policy, telemetry: telemetry);
    }

    private static WechatToolRegistry Registry(
        string toolName,
        Func<IReadOnlyDictionary<string, object?>, Task<ToolResult>> execute)
    {
        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            toolName,
            new WechatToolBinding("corp-app"),
            (args, _) => execute(args)));
        return registry;
    }

    private static ToolResult Retryable(string toolName)
        => ToolResultPipeline.Error(
            toolName,
            ToolErrorCategory.Retryable,
            ToolErrorSubtype.UpstreamError,
            "errcode=45009, errmsg=api freq out of limit",
            maxLength: 0,
            apiCode: 45009,
            attempts: 1);

    private sealed class AllowAll : IWechatToolAuthorizer
    {
        public static readonly AllowAll Instance = new();

        public ValueTask<WechatToolAuthorizationResult> AuthorizeAsync(
            WechatToolAuthorizationContext context,
            CancellationToken cancellationToken)
            => ValueTask.FromResult(WechatToolAuthorizationResult.Allow());
    }

    /// <summary>单仪器指标采集架子。</summary>
    private sealed class MetricHarness : IDisposable
    {
        private readonly MeterListener _listener;
        private readonly List<(long Value, Dictionary<string, object?> Tags)> _samples = [];

        /// <summary>本用例所在线程（<see cref="MeterListener"/> 回调在「记录线程」上同步执行，据此与并行测试类隔离）。</summary>
        private readonly int _ownerThreadId = Environment.CurrentManagedThreadId;

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

            _listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
            {
                if (!string.Equals(instrument.Name, instrumentName, StringComparison.Ordinal))
                {
                    return;
                }

                // 指标是<b>进程级</b>的：并行运行的其它测试类经真实 dispatcher 记录到同一批 instrument，
                // 只按标签过滤挡不住「同一个工具名」的噪声（contacts.tag.create 被多个用例使用）。
                // MeterListener 的回调在记录线程上同步触发，故按线程过滤可精确取到本用例自己的样本。
                if (Environment.CurrentManagedThreadId != _ownerThreadId)
                {
                    return;
                }

                var map = new Dictionary<string, object?>(tags.Length, StringComparer.Ordinal);
                foreach (var pair in tags)
                {
                    map[pair.Key] = pair.Value;
                }

                lock (_samples)
                {
                    _samples.Add((value, map));
                }
            });

            _listener.Start();
        }

        public static MetricHarness StartLong(string instrumentName) => new(instrumentName);

        /// <summary>
        /// 取唯一一条 <paramref name="tool"/> 名下的样本。两道过滤缺一不可：线程（见构造函数里的
        /// <c>_ownerThreadId</c>，挡掉并行测试类的噪声）+ tool（挡掉同一次调用中其它工具的样本）。
        /// </summary>
        public (long Value, Dictionary<string, object?> Tags) SingleSample(string tool)
            => _samples.Where(s => string.Equals(s.Tags.TryGetValue("tool", out var t) ? t as string : null, tool, StringComparison.Ordinal))
                .Should().ContainSingle().Which;

        public void Dispose() => _listener.Dispose();
    }
}
