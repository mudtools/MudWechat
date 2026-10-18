using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mud.AI.Agent.Tools;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.DataModels.Contacts.Tags;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Security;

/// <summary>
/// 审计端口的行为锚：<b>每条路径都留痕</b>（拒绝 / 审批拒绝 / 成功 / 失败），且租户事实跟随回合上下文。
/// </summary>
/// <remarks>
/// 这些用例锁定的是「合规可回答性」：谁在什么时候让 AI 做了什么。日志里的成功路径不可事后补，
/// 因此成功路径必须有用例守着（只有失败留痕是最容易发生的退化）。
/// </remarks>
public class WechatToolAuditTests
{
    private static readonly Dictionary<string, object?> GetArgs = new(StringComparer.Ordinal) { ["department_id"] = 2 };

    [Fact]
    public async Task Invoke_WhenToolDisabled_ShouldRecordDenied()
    {
        var sink = new RecordingSink();
        using var provider = BuildProvider(sink: sink, policy: p => p.Disable(WechatToolNames.ContactsDepartmentGet));

        await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        sink.Records.Should().ContainSingle();
        sink.Records[0].Outcome.Should().Be(WechatToolAuditOutcome.Denied);
        sink.Records[0].ToolName.Should().Be(WechatToolNames.ContactsDepartmentGet);
    }

    [Fact]
    public async Task Invoke_WhenAuthorizerDenies_ShouldRecordDeniedWithTenant()
    {
        var sink = new RecordingSink();
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolAuthorizationResult.Deny("缺少 contacts.department.read"));
        using var provider = BuildProvider(sink: sink, authorizer: authorizer.Object);

        await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        sink.Records.Should().ContainSingle();
        sink.Records[0].Outcome.Should().Be(WechatToolAuditOutcome.Denied);
        sink.Records[0].AppKey.Should().Be("corp-app");
        sink.Records[0].ArgumentsDigest.Should().Contain("department_id");
    }

    [Fact]
    public async Task Invoke_WhenSucceeded_ShouldRecordSucceededWithSizeAndDuration()
    {
        var sink = new RecordingSink();
        var corpContext = new StubCorpContext { Current = new WechatToolPrincipal("corp-app", "corp-1", "zhangsan") };
        using var provider = BuildProvider(sink: sink, corpContext: corpContext);

        var result = await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("研发部");
        sink.Records.Should().ContainSingle();
        var record = sink.Records[0];
        record.Outcome.Should().Be(WechatToolAuditOutcome.Succeeded);
        record.ResultBytes.Should().BeGreaterThan(0);
        record.Truncated.Should().BeFalse();
        record.DurationMs.Should().BeGreaterThanOrEqualTo(0);
        record.SenderId.Should().Be("zhangsan", "发起人是行级鉴权与合规问询的第一维");
    }

    [Fact]
    public async Task Invoke_WhenExecutorThrows_ShouldRecordFailed()
    {
        var sink = new RecordingSink();
        var binding = new WechatToolBinding("corp-app");
        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            WechatToolNames.ContactsDepartmentGet,
            binding,
            (_, _) => throw new InvalidOperationException("upstream down")));
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();

        await new WechatToolDispatcher(registry, policy, auditSink: sink)
            .InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        sink.Records.Should().ContainSingle();
        sink.Records[0].Outcome.Should().Be(WechatToolAuditOutcome.Failed);
    }

    [Fact]
    public async Task Invoke_WhenApprovalRejected_ShouldRecordApprovalRejectedAndNotExecute()
    {
        var sink = new RecordingSink();
        var tags = Tags();
        var approval = new Mock<IWechatToolApprovalGate>(MockBehavior.Strict);
        approval
            .Setup(a => a.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolApprovalDecision.Reject("批量写超过阈值"));

        using var provider = BuildProvider(
            sink: sink,
            policy: p => p.Enable(WechatToolNames.ContactsTagCreate),
            approval: approval.Object,
            tags: tags.Object);

        await Dispatcher(provider).InvokeAsync(
            WechatToolNames.ContactsTagCreate,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["tag_name"] = "临时标签" });

        sink.Records.Should().ContainSingle();
        sink.Records[0].Outcome.Should().Be(WechatToolAuditOutcome.ApprovalRejected);
        tags.Verify(t => t.CreateTagAsync(It.IsAny<CreateTagRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_ShouldFollowTurnTenantInsteadOfAssemblyBinding()
    {
        var sink = new RecordingSink();
        var corpContext = new StubCorpContext
        {
            Current = new WechatToolPrincipal("turn-app", "turn-corp", "lisi"),
        };
        using var provider = BuildProvider(sink: sink, corpContext: corpContext);

        await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        sink.Records.Should().ContainSingle();
        sink.Records[0].AppKey.Should().Be("turn-app", "多应用共进程时鉴权与审计必须跟随回合租户");
        sink.Records[0].AuthCorpId.Should().Be("turn-corp");
        sink.Records[0].SenderId.Should().Be("lisi");
    }

    [Fact]
    public async Task Invoke_WhenAuditSinkThrows_ShouldStillReturnResult()
    {
        var sink = new ThrowingSink();
        using var provider = BuildProvider(sink: sink);

        var result = await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("研发部", "审计存储抖动不得阻断业务执行");
    }

    [Fact]
    public void AddWechatWorkTools_WithoutSink_ShouldRegisterNullSink()
    {
        using var provider = new ServiceCollection()
            .AddSingleton(Departments().Object)
            .AddWechatWorkTools(new WechatToolBinding("corp-app"))
            .BuildServiceProvider();

        provider.GetRequiredService<IWechatToolAuditSink>().Should().BeSameAs(NullWechatToolAuditSink.Instance);
    }

    private static WechatToolDispatcher Dispatcher(IServiceProvider provider)
        => provider.GetRequiredService<WechatToolDispatcher>();

    private static ServiceProvider BuildProvider(
        IWechatToolAuditSink? sink = null,
        Action<WechatToolPolicy>? policy = null,
        IWechatToolAuthorizer? authorizer = null,
        IWechatToolApprovalGate? approval = null,
        IWechatWorkTagsService? tags = null,
        IWechatAiCorpContextAccessor? corpContext = null)
    {
        var services = new ServiceCollection().AddSingleton(Departments().Object);

        services.AddSingleton<IWechatToolAuthorizer>(authorizer ?? AllowAll().Object);
        if (sink is not null)
        {
            services.AddSingleton(sink);
        }

        if (approval is not null)
        {
            services.AddSingleton(approval);
        }

        if (tags is not null)
        {
            services.AddSingleton(tags);
        }

        if (corpContext is not null)
        {
            services.AddSingleton(corpContext);
        }

        return services
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1"), policy)
            .BuildServiceProvider();
    }

    private static Mock<IWechatWorkDepartmentsService> Departments()
    {
        var departments = new Mock<IWechatWorkDepartmentsService>(MockBehavior.Loose);
        departments
            .Setup(d => d.GetDepartmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentResponse
            {
                ErrorCode = 0,
                Department = new DepartmentInfo { Id = 2, Name = "研发部", ParentId = 1 },
            });
        return departments;
    }

    private static Mock<IWechatWorkTagsService> Tags()
    {
        var tags = new Mock<IWechatWorkTagsService>(MockBehavior.Loose);
        tags
            .Setup(t => t.CreateTagAsync(It.IsAny<CreateTagRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateTagResponse { ErrorCode = 0, TagId = 42 });
        return tags;
    }

    private static Mock<IWechatToolAuthorizer> AllowAll()
    {
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolAuthorizationResult.Allow());
        return authorizer;
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

    private sealed class ThrowingSink : IWechatToolAuditSink
    {
        public ValueTask WriteAsync(WechatToolAuditRecord record, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("audit store down");
    }

    private sealed class StubCorpContext : IWechatAiCorpContextAccessor
    {
        public WechatToolPrincipal? Current { get; set; }
    }
}
