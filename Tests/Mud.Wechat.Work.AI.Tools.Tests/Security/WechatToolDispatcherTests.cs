using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.AI.Tools.Security;
using Mud.Wechat.Work.DataModels.Contacts.Department;
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
        WechatToolNames.WriteAll.Should().BeEmpty("S2 阶段尚无写工具；一旦新增写工具，默认白名单必须仍为空");
        policy.EnabledTools.Should().BeSubsetOf(WechatToolNames.ReadonlyAll, "默认只启用只读面，写面须宿主显式开放");
        policy.EnforceAuthorization.Should().BeTrue("默认强制授权（写面与未知场景一律 fail-closed）");
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

    private static ServiceProvider BuildProvider(
        IWechatWorkDepartmentsService departments,
        Action<WechatToolPolicy>? policy,
        IWechatToolAuthorizer? authorizer)
    {
        var services = new ServiceCollection().AddSingleton(departments);
        if (authorizer is not null)
        {
            services.AddSingleton(authorizer);
        }

        return services
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1"), policy)
            .BuildServiceProvider();
    }
}
