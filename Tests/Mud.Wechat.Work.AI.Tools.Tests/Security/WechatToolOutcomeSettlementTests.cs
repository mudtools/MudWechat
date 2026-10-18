using System.Text.Json.Nodes;
using FluentAssertions;
using Mud.AI.Agent.Tools;
using Mud.AI.Agent.Tools.Errors;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Security;

/// <summary>
/// 结局结算的行为锚：<b>失败必须可观测</b>——审计结局、<c>ErrorCode</c>、指标 <c>outcome</c> 三者同源。
/// </summary>
/// <remarks>
/// <para>
/// 历史实现用 <c>ToolResult.FromError(string)</c> 表达业务失败，而该工厂<b>不产生</b>
/// <see cref="ToolResult.Error"/>；执行链据此判定结局，于是上游错误与参数错误在审计与指标里
/// <b>全部记成成功</b>（失败率恒 0、<c>ErrorCode</c> 恒空）。这几个用例把「结算正确」钉成可回归的行为。
/// </para>
/// <para>
/// 同时锁定：超时覆盖「排队 / 授权 / 审批 / 执行」四段；回合工具预算越限短路；释放后调用 fail-fast。
/// </para>
/// </remarks>
public class WechatToolOutcomeSettlementTests
{
    private static readonly Dictionary<string, object?> Args = new(StringComparer.Ordinal) { ["department_id"] = 2 };

    [Fact]
    public async Task Invoke_WhenExecutorReturnsStructuredUpstreamError_ShouldRecordFailedWithApiCode()
    {
        var sink = new RecordingSink();
        var dispatcher = Dispatcher(
            sink,
            _ => Task.FromResult(ToolResultPipeline.Error(
                WechatToolNames.ContactsDepartmentGet,
                ToolErrorCategory.Api,
                ToolErrorSubtype.UpstreamError,
                "errcode=45009, errmsg=api freq out of limit",
                maxLength: 0,
                apiCode: 45009,
                attempts: 1)));

        var result = await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        result.Error.Should().NotBeNull("结构化载荷是结算与审计的唯一依据");
        result.Text.Should().StartWith("{\"tool_error\"", "首行必须是结构化载荷");

        var record = sink.Records.Should().ContainSingle().Subject;
        record.Outcome.Should().Be(WechatToolAuditOutcome.Failed, "上游失败记成成功会让失败率恒为 0");
        record.ErrorCode.Should().Be(45009);
        record.ErrorCategory.Should().Be("api");
        record.ErrorSubtype.Should().Be("upstream_error");
    }

    [Fact]
    public async Task Invoke_WhenExecutorThrowsToolArgument_ShouldRecordValidationWithSuggestions()
    {
        var sink = new RecordingSink();
        var dispatcher = Dispatcher(
            sink,
            _ => throw new ToolArgumentException(
                "参数非法：msg_type 只能是 text/textcard/markdown。",
                ["text", "textcard", "markdown"]));

        var result = await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        result.Error.Should().NotBeNull();
        result.Error!.Category.Should().Be("validation");
        result.Error.Subtype.Should().Be(ToolErrorSubtype.InvalidArgs);
        result.Error.Suggestions.Should().NotBeNull("闭集非法值必须回填合法取值，否则模型只能盲试");
        result.Text.Should().Contain("textcard");

        var record = sink.Records.Should().ContainSingle().Subject;
        record.Outcome.Should().Be(WechatToolAuditOutcome.Failed);
        record.ErrorSubtype.Should().Be(ToolErrorSubtype.InvalidArgs);
    }

    [Fact]
    public async Task Invoke_WhenToolBudgetExhausted_ShouldShortCircuitWithoutExecuting()
    {
        var executed = 0;
        var dispatcher = Dispatcher(
            sink: null,
            _ =>
            {
                executed++;
                return Task.FromResult(ToolResultPipeline.Ok("{}", 0));
            });

        var results = new List<ToolResult>();
        using (new ToolCallBudget(1).Begin())
        {
            results.Add(await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args));
            results.Add(await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args));
        }

        executed.Should().Be(1, "预算越限必须短路，不得再触达 SDK");
        results[1].Error!.Subtype.Should().Be(ToolErrorSubtype.ToolBudgetExceeded);
        results[1].Text.Should().Contain("本轮工具调用已达上限");
    }

    [Fact]
    public async Task Invoke_WhenAuthorizationStageTimesOut_ShouldReturnTimeoutResultInsteadOfThrowing()
    {
        var sink = new RecordingSink();
        var authorizer = new SlowAuthorizer();

        var registry = Registry(_ => Task.FromResult(ToolResultPipeline.Ok("{}", 0)));
        var policy = new WechatToolPolicy { CallTimeout = TimeSpan.FromSeconds(1) };
        policy.Validate();

        var started = DateTime.UtcNow;
        var result = await new WechatToolDispatcher(registry, policy, authorizer: authorizer, auditSink: sink)
            .InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);
        var elapsed = DateTime.UtcNow - started;

        result.Error!.Subtype.Should().Be(ToolErrorSubtype.Timeout, "超时预算必须覆盖授权/审批/排队段，而不只是执行段");
        elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
        sink.Records.Should().ContainSingle().Which.Outcome.Should().Be(WechatToolAuditOutcome.Failed);
    }

    [Fact]
    public async Task Invoke_WhenQueueWaitExceedsCallTimeout_ShouldReturnTimeoutWithoutTouchingExecutor()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var executed = 0;

        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            WechatToolNames.ContactsDepartmentGet,
            new WechatToolBinding("corp-app"),
            async (_, _) =>
            {
                Interlocked.Increment(ref executed);
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
                return ToolResultPipeline.Ok("{}", 0);
            }));

        // 闸门只有 1 个位：第二个调用必然停在排队段，而超时预算从「进入执行链」起算（FIX-17）。
        var policy = new WechatToolPolicy
        {
            EnforceAuthorization = false,
            CallTimeout = TimeSpan.FromSeconds(1),
            MaxConcurrentCalls = 1,
        };
        policy.Validate();
        using var dispatcher = new WechatToolDispatcher(registry, policy);

        var holding = dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);
        await entered.Task.WaitAsync(TimeSpan.FromSeconds(5));

        var queued = await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        queued.Error!.Subtype.Should().Be(
            ToolErrorSubtype.Timeout,
            "满载排队属于超时预算覆盖的四段之一；只给执行段设超时会让队尾调用挂到调用方超时");
        executed.Should().Be(1, "排队超时不得触达执行器（否则超时的调用还在真实消耗上游频率额度）");

        release.SetResult();
        (await holding).Text.Should().Contain("{}");
    }

    [Fact]
    public async Task Invoke_WhenOutboundNormalizeThrows_ShouldReturnStructuredErrorInsteadOfThrowing()
    {
        var sink = new RecordingSink();
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();

        var dispatcher = new WechatToolDispatcher(
            Registry(_ => Task.FromResult(ToolResultPipeline.Ok("{\"raw\":\"未净化原文\"}", 0))),
            policy,
            new ThrowingOutboundGuard(),
            authorizer: null,
            approvalGate: null,
            logger: null,
            auditSink: sink,
            corpContext: null);

        var result = await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        result.Error.Should().NotBeNull("出站失败必须是结构化失败，而不是把异常穿到模型");
        result.Error!.Category.Should().Be("internal");
        result.Text.Should().NotContain("未净化原文", "未经净化的原文一旦进提示词就不可撤回");
        result.Text.Should().Contain("出站净化");

        sink.Records.Should().ContainSingle().Which.Outcome.Should().Be(
            WechatToolAuditOutcome.Failed,
            "归一失败必须结算为失败（谎报成功会让失败率失真）");
    }

    [Fact]
    public async Task Invoke_AfterDispose_ShouldThrowObjectDisposed()
    {
        var dispatcher = Dispatcher(sink: null, _ => Task.FromResult(ToolResultPipeline.Ok("{}", 0)));
        dispatcher.Dispose();

        var act = () => dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        await act.Should().ThrowAsync<ObjectDisposedException>();
    }

    [Fact]
    public void OutcomeErrors_FromFailedOutcome_ShouldCarryApiCodeAndNextHint()
    {
        var outcome = ToolApiOutcome<string>.Fail("errcode=45009, errmsg=api freq out of limit", 45009);

        var result = WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsDepartmentGet);

        result.Error.Should().NotBeNull();
        result.Error!.ApiCode.Should().Be(45009);
        result.Error.Tool.Should().Be(WechatToolNames.ContactsDepartmentGet);
        result.Text.Should().StartWith("{\"tool_error\"");

        var payload = JsonNode.Parse(result.Text.Split('\n')[0])!.AsObject();
        payload["tool_error"]!["api_code"]!.GetValue<int>().Should().Be(45009);
    }

    [Fact]
    public void OutcomeErrors_WhenOutcomeSucceeded_ShouldFailFast()
    {
        var outcome = ToolApiOutcome<string>.Success("ok");

        var act = () => WechatToolOutcomeErrors.From(outcome, WechatToolNames.ContactsDepartmentGet);

        act.Should().Throw<InvalidOperationException>("成功态走 From 属编程错误：会静默吞掉成功载荷");
    }

    private static WechatToolDispatcher Dispatcher(
        RecordingSink? sink,
        Func<IReadOnlyDictionary<string, object?>, Task<ToolResult>> execute)
    {
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();
        return new WechatToolDispatcher(Registry(execute), policy, auditSink: sink);
    }

    private static WechatToolRegistry Registry(Func<IReadOnlyDictionary<string, object?>, Task<ToolResult>> execute)
    {
        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            WechatToolNames.ContactsDepartmentGet,
            new WechatToolBinding("corp-app"),
            (args, _) => execute(args)));
        return registry;
    }

    private sealed class RecordingSink : IWechatToolAuditSink
    {
        public List<WechatToolAuditRecord> Records { get; } = [];

        public ValueTask WriteAsync(WechatToolAuditRecord record, CancellationToken cancellationToken = default)
        {
            Records.Add(record);
            return ValueTask.CompletedTask;
        }
    }

    private sealed class SlowAuthorizer : IWechatToolAuthorizer    {
        public async ValueTask<WechatToolAuthorizationResult> AuthorizeAsync(
            WechatToolAuthorizationContext context,
            CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
            return WechatToolAuthorizationResult.Allow();
        }
    }

    /// <summary>出站出口的失败注入（真实失败形态不可稳定复现，见 <see cref="WechatToolOutboundGuard.Normalize"/> 的 remarks）。</summary>
    private sealed class ThrowingOutboundGuard : WechatToolOutboundGuard
    {
        public ThrowingOutboundGuard()
            : base(8192, contentSafetyMode: null)
        {
        }

        public override ToolResult Normalize(ToolResult result)
            => throw new InvalidOperationException("sanitizer crashed");
    }
}
