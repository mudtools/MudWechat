using FluentAssertions;
using Mud.AI.Agent.Tools;
using Mud.AI.Agent.Tools.Errors;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Security;

/// <summary>
/// 只读限流退避的行为锚（FIX-20）：<b>只读重试、写零重试、次数有上界</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么写零重试是硬约束</b>：重复的写会产生重复副作用（重复建标签、重复发消息），
/// 而模型无法从"我以为失败了"里恢复正确状态；把不确定性交给用户可见的失败回执，比自作主张再发一次安全。
/// </para>
/// <para>
/// <b>禁止墙钟断言</b>：退避延迟受 jitter 与并发影响，用例只断言<b>调用次数</b>与<b>最终结局</b>。
/// </para>
/// </remarks>
public class WechatToolRetryTests
{
    [Fact]
    public async Task ReadTool_WhenRetryableError_ShouldRetryOnceAndSucceed()
    {
        var calls = 0;
        var sink = new Sink();
        var dispatcher = Dispatcher(
            sink,
            _ => ++calls == 1
                ? Task.FromResult(Retryable(WechatToolNames.ContactsDepartmentGet))
                : Task.FromResult(ToolResultPipeline.Ok("{\"id\":2}", 0)));

        var result = await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        calls.Should().Be(2, "45009 是幂等只读调用的常态失败，重试一次比让模型改参数再试更安全");
        result.Error.Should().BeNull();
        sink.Records.Should().ContainSingle().Which.Outcome.Should().Be(WechatToolAuditOutcome.Succeeded);
    }

    [Fact]
    public async Task WriteTool_WhenRetryableError_ShouldNeverRetry()
    {
        var calls = 0;
        var sink = new Sink();
        var policy = new WechatToolPolicy { RequireApprovalForWrites = false };
        policy.Enable(WechatToolNames.ContactsTagCreate);
        policy.Validate();

        var dispatcher = new WechatToolDispatcher(
            Registry(WechatToolNames.ContactsTagCreate, _ =>
            {
                calls++;
                return Task.FromResult(Retryable(WechatToolNames.ContactsTagCreate));
            }),
            policy,
            authorizer: new AllowAll(),
            auditSink: sink);

        var result = await dispatcher.InvokeAsync(WechatToolNames.ContactsTagCreate, Args);

        calls.Should().Be(1, "写工具重试会重复副作用 —— 零重试不可配置");
        result.Error.Should().NotBeNull();
        sink.Records.Should().ContainSingle().Which.Outcome.Should().Be(WechatToolAuditOutcome.Failed);
    }

    [Fact]
    public async Task ReadTool_WhenAlwaysRetryable_ShouldStopAfterMaxAttempts()
    {
        var calls = 0;
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Retry.MaxAttempts = 2;
        policy.Retry.BaseDelayMilliseconds = 1;
        policy.Retry.MaxDelayMilliseconds = 2;
        policy.Validate();

        var dispatcher = new WechatToolDispatcher(
            Registry(WechatToolNames.ContactsDepartmentGet, _ =>
            {
                calls++;
                return Task.FromResult(Retryable(WechatToolNames.ContactsDepartmentGet));
            }),
            policy);

        var result = await dispatcher.InvokeAsync(WechatToolNames.ContactsDepartmentGet, Args);

        calls.Should().Be(3, "MaxAttempts 是重试次数（不含首次）⇒ 总调用 = 1 + MaxAttempts");
        result.Error.Should().NotBeNull();
    }

    private static readonly Dictionary<string, object?> Args = new(StringComparer.Ordinal) { ["department_id"] = 2 };

    private static ToolResult Retryable(string toolName)
        => ToolResultPipeline.Error(
            toolName,
            ToolErrorCategory.Retryable,
            ToolErrorSubtype.UpstreamError,
            "errcode=45009, errmsg=api freq out of limit",
            maxLength: 0,
            apiCode: 45009,
            attempts: 1);

    private static WechatToolDispatcher Dispatcher(
        Sink sink,
        Func<IReadOnlyDictionary<string, object?>, Task<ToolResult>> execute)
    {
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();
        return new WechatToolDispatcher(Registry(WechatToolNames.ContactsDepartmentGet, execute), policy, auditSink: sink);
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

    private sealed class AllowAll : IWechatToolAuthorizer
    {
        public ValueTask<WechatToolAuthorizationResult> AuthorizeAsync(
            WechatToolAuthorizationContext context,
            CancellationToken cancellationToken)
            => ValueTask.FromResult(WechatToolAuthorizationResult.Allow());
    }

    private sealed class Sink : IWechatToolAuditSink
    {
        public List<WechatToolAuditRecord> Records { get; } = [];

        public ValueTask WriteAsync(WechatToolAuditRecord record, CancellationToken cancellationToken = default)
        {
            Records.Add(record);
            return ValueTask.CompletedTask;
        }
    }
}
