using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.DataModels.Contacts.Tags;
using Mud.Wechat.Work.DataModels.Wedoc;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Security;

/// <summary>
/// 执行链四步的行为锚：白名单默认封闭、无授权器即拒绝（fail-closed）、拒绝文案中性、执行异常归一。
/// </summary>
/// <remarks>
/// 对应方案 §6.2 L2 ④⑤：「未注册授权器且 <c>EnforceToolAuthorization=true</c> ⇒ 默认拒绝」与
/// 「写工具默认白名单空 ⇒ 不可达」——后者由 <see cref="WechatToolPolicy"/> 默认只装只读名单体现。
/// </remarks>
public class WechatToolDispatcherTests
{
    private static readonly Dictionary<string, object?> GetArgs = new(StringComparer.Ordinal) { ["department_id"] = 2 };

    [Fact]
    public async Task Invoke_WhenToolDisabled_ShouldDenyWithoutCallingSdk()
    {
        var departments = Departments();
        using var provider = BuildProvider(
            departments.Object,
            policy: p => p.Disable(WechatToolNames.ContactsDepartmentGet),
            authorizer: null);

        var result = await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("拒绝");
        departments.Verify(d => d.GetDepartmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_WithoutAuthorizer_ShouldFailClosed()
    {
        var departments = Departments();
        using var provider = BuildProvider(departments.Object, policy: null, authorizer: null);

        var result = await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("拒绝");
        departments.Verify(d => d.GetDepartmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_WhenAuthorizerAllows_ShouldExecuteWithContractFacts()
    {
        var departments = Departments();
        WechatToolAuthorizationContext? captured = null;
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .Returns((WechatToolAuthorizationContext context, CancellationToken _) =>
            {
                captured = context;
                return ValueTask.FromResult(WechatToolAuthorizationResult.Allow());
            });

        using var provider = BuildProvider(departments.Object, policy: null, authorizer.Object);

        var result = await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("研发部");
        captured.Should().NotBeNull();
        captured!.ToolName.Should().Be(WechatToolNames.ContactsDepartmentGet);
        captured.AppKey.Should().Be("corp-app");
        captured.AuthCorpId.Should().Be("corp-1");
        captured.Risk.Should().Be(WechatToolRisk.Read);
        captured.RequiredScopes.Should().Contain("contacts.department.read");
        captured.ArgumentsDigest.Should().Contain("department_id", "授权器拿到的是可读摘要（键名 + 值形态），不是原始载荷");
    }

    [Fact]
    public async Task Invoke_WhenAuthorizerDenies_ShouldReturnNeutralText()
    {
        var departments = Departments();
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolAuthorizationResult.Deny("缺少 contacts.write 权限点"));

        using var provider = BuildProvider(departments.Object, policy: null, authorizer.Object);

        var result = await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("拒绝");
        result.Text.Should().NotContain("contacts.write", "拒绝理由只进宿主日志，回填模型的必须中性（否则权限点可被探测）");
        departments.Verify(d => d.GetDepartmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_WhenAuthorizationNotEnforced_ShouldSkipAuthorizer()
    {
        var departments = Departments();
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        using var provider = BuildProvider(
            departments.Object,
            policy: p => p.EnforceAuthorization = false,
            authorizer: authorizer.Object);

        var result = await Dispatcher(provider).InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("研发部");
        authorizer.Verify(
            a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Invoke_WhenExecutorThrows_ShouldNormalizeToReadableError()
    {
        var binding = new WechatToolBinding("corp-app");
        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            WechatToolNames.ContactsDepartmentGet,
            binding,
            (_, _) => throw new InvalidOperationException("upstream down")));
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();

        var result = await new WechatToolDispatcher(registry, policy)
            .InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain("执行失败");
        result.Text.Should().NotContain("upstream down", "内部异常文案不入模型可见面");
    }

    [Fact]
    public async Task Invoke_WhenExecutionThrowsArgumentException_ShouldSurfaceParameterError()
    {
        var binding = new WechatToolBinding("corp-app");
        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            WechatToolNames.ContactsDepartmentGet,
            binding,
            (_, _) => throw new ToolArgumentException(
                "参数 status_filter 的取值非法（合法取值：All/PendingInherit/Inheriting/Inherited）。",
                ["All", "PendingInherit", "Inheriting", "Inherited"])));
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();

        var result = await new WechatToolDispatcher(registry, policy)
            .InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        result.Text.Should().Contain(
            "status_filter",
            "参数类错误（Validation/invalid_args）的文案含参数名与合法取值清单，原样回填模型才能自我纠错");
        result.Text.Should().Contain("All");
    }

    [Fact]
    public async Task Invoke_WhenExecutionCancelled_ShouldPropagate()
    {
        var binding = new WechatToolBinding("corp-app");
        var registry = new WechatToolRegistry();
        registry.Register(new WechatToolExecution(
            WechatToolNames.ContactsDepartmentGet,
            binding,
            (_, _) => throw new OperationCanceledException()));
        var policy = new WechatToolPolicy { EnforceAuthorization = false };
        policy.Validate();

        var act = async () => await new WechatToolDispatcher(registry, policy)
            .InvokeAsync(WechatToolNames.ContactsDepartmentGet, GetArgs);

        await act.Should().ThrowAsync<OperationCanceledException>("取消是控制流，不得被归一为工具结果");
    }

    [Fact]
    public void Policy_WithUnknownToolName_ShouldFailFast()
    {
        var policy = new WechatToolPolicy().Enable("contacts.department.nope");

        var act = policy.Validate;

        act.Should().Throw<InvalidOperationException>("拼错的名单等于静默禁用，必须在装配期暴露");
    }

    [Fact]
    public void Policy_AfterValidate_ShouldBeFrozen()
    {
        var policy = new WechatToolPolicy();
        policy.Validate();

        var act = () => policy.Enable(WechatToolNames.ContactsDepartmentGet);

        act.Should().Throw<InvalidOperationException>("白名单是装配期事实，不是运行期开关");
    }

    [Fact]
    public void DefaultPolicy_ShouldEnableReadonlyOnly()
    {
        var policy = new WechatToolPolicy();

        policy.EnabledTools.Should().Equal(WechatToolNames.ReadonlyAll);
        WechatToolNames.WriteAll.Should().NotBeEmpty("S3 起已有受控写工具；它们不得随默认策略自动生效");
        policy.EnabledTools.Should().NotIntersectWith(WechatToolNames.WriteAll, "写面须宿主显式开放");
        policy.EnforceAuthorization.Should().BeTrue("默认强制授权（写面与未知场景一律 fail-closed）");
        policy.RequireApprovalForWrites.Should().BeTrue("写工具默认要求 HITL 审批");
    }

    [Fact]
    public void Policy_EnablingWriteWithoutAuthorization_ShouldFailFast()
    {
        var policy = new WechatToolPolicy { EnforceAuthorization = false }
            .Enable(WechatToolNames.ContactsTagCreate);

        var act = policy.Validate;

        act.Should().Throw<InvalidOperationException>("写面必须同时受白名单与授权器两段管控");
    }

    [Fact]
    public async Task Invoke_WriteToolByDefault_ShouldBeInvisibleAndDenied()
    {
        var tags = Tags();
        using var provider = BuildProvider(
            Departments().Object,
            policy: null,
            authorizer: AllowAll().Object,
            approval: null,
            tags: tags.Object);

        provider.GetRequiredService<IReadOnlyList<Microsoft.Extensions.AI.AIFunction>>()
            .Select(f => f.Name)
            .Should().NotContain(WechatToolNames.ContactsTagCreate, "未启用的写工具不进模型视野");

        var result = await Dispatcher(provider).InvokeAsync(
            WechatToolNames.ContactsTagCreate,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["tag_name"] = "临时标签" });

        result.Text.Should().Contain("拒绝");
        tags.Verify(t => t.CreateTagAsync(It.IsAny<CreateTagRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_WriteToolWithoutApprovalGate_ShouldFailClosed()
    {
        var tags = Tags();
        using var provider = BuildProvider(
            Departments().Object,
            policy: p => p.Enable(WechatToolNames.ContactsTagCreate),
            authorizer: AllowAll().Object,
            approval: null,
            tags: tags.Object);

        var result = await Dispatcher(provider).InvokeAsync(
            WechatToolNames.ContactsTagCreate,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["tag_name"] = "临时标签" });

        result.Text.Should().Contain("拒绝");
        tags.Verify(t => t.CreateTagAsync(It.IsAny<CreateTagRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_WriteToolWithApproval_ShouldExecute()
    {
        var tags = Tags();
        var approval = new Mock<IWechatToolApprovalGate>(MockBehavior.Strict);
        approval
            .Setup(a => a.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolApprovalDecision.Approve("管理员在工单 #1 中批准"));

        using var provider = BuildProvider(
            Departments().Object,
            policy: p => p.Enable(WechatToolNames.ContactsTagCreate),
            authorizer: AllowAll().Object,
            approval: approval.Object,
            tags: tags.Object);

        var result = await Dispatcher(provider).InvokeAsync(
            WechatToolNames.ContactsTagCreate,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["tag_name"] = "临时标签" });

        result.Text.Should().Contain("tag_id");
        tags.Verify(t => t.CreateTagAsync(
            It.Is<CreateTagRequest>(r => r.TagName == "临时标签"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Invoke_WriteToolWhenApprovalRejected_ShouldReturnNeutralText()
    {
        var tags = Tags();
        var approval = new Mock<IWechatToolApprovalGate>(MockBehavior.Strict);
        approval
            .Setup(a => a.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolApprovalDecision.Reject("批量写超过阈值"));

        using var provider = BuildProvider(
            Departments().Object,
            policy: p => p.Enable(WechatToolNames.ContactsTagCreate),
            authorizer: AllowAll().Object,
            approval: approval.Object,
            tags: tags.Object);

        var result = await Dispatcher(provider).InvokeAsync(
            WechatToolNames.ContactsTagCreate,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["tag_name"] = "临时标签" });

        result.Text.Should().Contain("拒绝");
        result.Text.Should().NotContain("阈值", "审批理由只进宿主日志");
        tags.Verify(t => t.CreateTagAsync(It.IsAny<CreateTagRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_HighRiskWriteWhenAuthorizationDenied_ShouldNotAskApproval()
    {
        var wedoc = Wedoc();
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolAuthorizationResult.Deny("缺少 wedoc.doc.write 权限点"));
        var approval = new Mock<IWechatToolApprovalGate>(MockBehavior.Strict);

        using var provider = BuildProvider(
            Departments().Object,
            policy: p => p.Enable(WechatToolNames.WedocDocDelete),
            authorizer: authorizer.Object,
            approval: approval.Object,
            wedoc: wedoc.Object);

        var result = await Dispatcher(provider).InvokeAsync(
            WechatToolNames.WedocDocDelete,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["docid"] = "doc-1" });

        result.Text.Should().Contain("拒绝");
        approval.Verify(
            a => a.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "先授权后审批：未获授权的动作不得进入人工审批（否则审批就成了绕开授权的通道）");
        wedoc.Verify(w => w.DeleteDocumentAsync(It.IsAny<DeleteWedocDocumentRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_HighRiskWrite_ApprovalShouldSeeHighRiskContext()
    {
        var wedoc = Wedoc();
        WechatToolAuthorizationContext? captured = null;
        var approval = new Mock<IWechatToolApprovalGate>(MockBehavior.Strict);
        approval
            .Setup(a => a.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .Returns((WechatToolAuthorizationContext context, CancellationToken _) =>
            {
                captured = context;
                return ValueTask.FromResult(WechatToolApprovalDecision.Approve("管理员确认删除"));
            });

        using var provider = BuildProvider(
            Departments().Object,
            policy: p => p.Enable(WechatToolNames.WedocDocDelete),
            authorizer: AllowAll().Object,
            approval: approval.Object,
            wedoc: wedoc.Object);

        await Dispatcher(provider).InvokeAsync(
            WechatToolNames.WedocDocDelete,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["docid"] = "doc-1" });

        captured.Should().NotBeNull();
        captured!.ToolName.Should().Be(WechatToolNames.WedocDocDelete);
        captured.Risk.Should().Be(WechatToolRisk.HighRiskWrite, "审批人必须看到「不可逆」这一事实，否则无从判断该不该放行");
        captured.RequiredScopes.Should().Contain("wedoc.doc.write");
        captured.AppKey.Should().Be("corp-app");
        captured.AuthCorpId.Should().Be("corp-1");
        captured.ArgumentsDigest.Should().Contain("docid=str").And.NotContain("doc-1", "审批输入只给可读摘要，不给原始载荷");
    }

    [Fact]
    public void Policy_EnablingHighRiskWriteWithApprovalDisabled_ShouldFailFast()
    {
        var policy = new WechatToolPolicy { RequireApprovalForWrites = false }
            .Enable(WechatToolNames.WedocDocDelete);

        var act = policy.Validate;

        act.Should().Throw<InvalidOperationException>("高危写恒需审批，不得靠开关关闭");
    }

    [Fact]
    public async Task Invoke_HighRiskWriteWithoutApprovalGate_ShouldFailClosed()
    {
        var wedoc = Wedoc();
        using var provider = BuildProvider(
            Departments().Object,
            policy: p => p.Enable(WechatToolNames.WedocDocDelete),
            authorizer: AllowAll().Object,
            approval: null,
            wedoc: wedoc.Object);

        var result = await Dispatcher(provider).InvokeAsync(
            WechatToolNames.WedocDocDelete,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["docid"] = "doc-1" });

        result.Text.Should().Contain("拒绝");
        wedoc.Verify(w => w.DeleteDocumentAsync(It.IsAny<DeleteWedocDocumentRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Invoke_HighRiskWriteWithApproval_ShouldExecute()
    {
        var wedoc = Wedoc();
        var approval = new Mock<IWechatToolApprovalGate>(MockBehavior.Strict);
        approval
            .Setup(a => a.RequestApprovalAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolApprovalDecision.Approve("管理员确认删除"));

        using var provider = BuildProvider(
            Departments().Object,
            policy: p => p.Enable(WechatToolNames.WedocDocDelete),
            authorizer: AllowAll().Object,
            approval: approval.Object,
            wedoc: wedoc.Object);

        var result = await Dispatcher(provider).InvokeAsync(
            WechatToolNames.WedocDocDelete,
            new Dictionary<string, object?>(StringComparer.Ordinal) { ["docid"] = "doc-1" });

        result.Text.Should().Contain("\"ok\":true");
        wedoc.Verify(w => w.DeleteDocumentAsync(
            It.Is<DeleteWedocDocumentRequest>(r => r.Docid == "doc-1"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
    private static WechatToolDispatcher Dispatcher(IServiceProvider provider)
        => provider.GetRequiredService<WechatToolDispatcher>();

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

    private static Mock<IWechatWorkWedocService> Wedoc()
    {
        var wedoc = new Mock<IWechatWorkWedocService>(MockBehavior.Loose);
        wedoc
            .Setup(w => w.DeleteDocumentAsync(It.IsAny<DeleteWedocDocumentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WechatWorkResponse { ErrorCode = 0 });
        return wedoc;
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

    private static ServiceProvider BuildProvider(
        IWechatWorkDepartmentsService departments,
        Action<WechatToolPolicy>? policy,
        IWechatToolAuthorizer? authorizer = null,
        IWechatToolApprovalGate? approval = null,
        IWechatWorkTagsService? tags = null,
        IWechatWorkWedocService? wedoc = null)
    {
        var services = new ServiceCollection().AddSingleton(departments);
        if (authorizer is not null)
        {
            services.AddSingleton(authorizer);
        }

        if (approval is not null)
        {
            services.AddSingleton(approval);
        }

        if (tags is not null)
        {
            services.AddSingleton(tags);
        }

        if (wedoc is not null)
        {
            services.AddSingleton(wedoc);
        }

        return services
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1"), policy)
            .BuildServiceProvider();
    }
}
