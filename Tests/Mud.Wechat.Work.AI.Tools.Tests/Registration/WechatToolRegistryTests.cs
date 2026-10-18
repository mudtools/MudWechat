using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Registration;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Registration;

/// <summary>
/// 注册表与装配的行为锚：重名拒绝、软缺席（依赖未注册 ⇒ 该域工具缺席而不炸）、注册后可按名执行。
/// </summary>
public class WechatToolRegistryTests
{
    [Fact]
    public void Register_DuplicateToolName_ShouldThrow()
    {
        var registry = new WechatToolRegistry();
        var binding = new WechatToolBinding("corp-app");
        registry.Register(new WechatToolExecution("t", binding, (_, _) => Task.FromResult(ToolResult.FromText("{}"))));

        var act = () => registry.Register(
            new WechatToolExecution("t", binding, (_, _) => Task.FromResult(ToolResult.FromText("{}"))));

        act.Should().Throw<InvalidOperationException>("同名注册会让模型调用落到不可预期的实现");
    }

    [Fact]
    public void Register_BlankToolName_ShouldThrow()
    {
        var act = () => new WechatToolExecution("  ", new WechatToolBinding("corp-app"), (_, _) => Task.FromResult(ToolResult.FromText("{}")));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryGet_UnknownTool_ShouldReturnFalse()
    {
        var registry = new WechatToolRegistry();

        registry.TryGet("nope", out _).Should().BeFalse();
    }

    [Fact]
    public async Task AddWechatWorkTools_ShouldRegisterCuratedToolsAndExecute()
    {
        var departments = new Mock<IWechatWorkDepartmentsService>(MockBehavior.Loose);
        departments
            .Setup(d => d.GetDepartmentAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentResponse
            {
                ErrorCode = 0,
                Department = new DepartmentInfo { Id = 2, Name = "研发部", ParentId = 1 },
            });

        using var provider = BuildProvider(
            departments.Object,
            new Mock<IWechatWorkUsersService>(MockBehavior.Loose).Object,
            new Mock<IWechatWorkTagsService>(MockBehavior.Loose).Object,
            new Mock<IWechatWorkWedocService>(MockBehavior.Loose).Object,
            new Mock<IWechatWorkApprovalService>(MockBehavior.Loose).Object,
            new Mock<IWechatWorkInternalCheckinRecordService>(MockBehavior.Loose).Object,
            externalContact: true,
            meeting: true,
            kf: true,
            journal: true,
            living: true,
            message: true);
        var registry = provider.GetRequiredService<WechatToolRegistry>();

        registry.ToolNames.Should().BeEquivalentTo(WechatToolNames.All);
        registry.TryGet(WechatToolNames.ContactsDepartmentGet, out var execution).Should().BeTrue();

        var result = await execution.ExecuteAsync(
            new Dictionary<string, object?> { ["department_id"] = 2 },
            CancellationToken.None);

        result.Text.Should().Contain("研发部");
        departments.Verify(d => d.GetDepartmentAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void AddWechatWorkTools_WithoutSdkService_ShouldStayEmptyInsteadOfThrowing()
    {
        using var provider = new ServiceCollection()
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1"))
            .BuildServiceProvider();

        provider.GetRequiredService<WechatToolRegistry>().Count.Should().Be(
            0,
            "业务域未注册 ⇒ 该域工具软缺席（不得让装配期空引用炸掉整个宿主）");
    }

    private static ServiceProvider BuildProvider(
        IWechatWorkDepartmentsService departments,
        IWechatWorkUsersService? users = null,
        IWechatWorkTagsService? tags = null,
        IWechatWorkWedocService? wedoc = null,
        IWechatWorkApprovalService? approval = null,
        IWechatWorkInternalCheckinRecordService? checkin = null,
        bool externalContact = false,
        bool meeting = false,
        bool kf = false,
        bool journal = false,
        bool living = false,
        bool message = false)
    {
        var services = new ServiceCollection().AddSingleton(departments);
        services.AddSingleton(new Mock<IWechatWorkInternalDialService>(MockBehavior.Loose).Object);
        if (users is not null)
        {
            services.AddSingleton(users);
        }

        if (tags is not null)
        {
            services.AddSingleton(tags);
        }

        if (wedoc is not null)
        {
            services.AddSingleton(wedoc);
        }

        if (approval is not null)
        {
            services.AddSingleton(approval);
            services.AddSingleton(new Mock<IWechatWorkApprovalTemplateService>(MockBehavior.Loose).Object);
        }

        if (checkin is not null)
        {
            services.AddSingleton(checkin);
        }

        if (externalContact)
        {
            services.AddSingleton(new Mock<IWechatWorkExternalContactFollowUserService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactCustomerService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactTagService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactGroupChatService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactMomentService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactStatisticsService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactGroupMsgService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactProductAlbumService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkExternalContactResignedInheritanceService>(MockBehavior.Loose).Object);
        }

        if (meeting)
        {
            services.AddSingleton(new Mock<IWechatWorkMeetingService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkInternalMeetingService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkInternalMeetingAdvancedService>(MockBehavior.Loose).Object);
        }

        if (journal)
        {
            services.AddSingleton(new Mock<IWechatWorkInternalJournalService>(MockBehavior.Loose).Object);
        }

        if (living)
        {
            services.AddSingleton(new Mock<IWechatWorkLivingService>(MockBehavior.Loose).Object);
        }

        if (message)
        {
            services.AddSingleton(new Mock<IWechatWorkMessageService>(MockBehavior.Loose).Object);
        }

        if (kf)
        {
            services.AddSingleton(new Mock<IWechatWorkKfAccountService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkKfServicerService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkKfSessionService>(MockBehavior.Loose).Object);
            services.AddSingleton(new Mock<IWechatWorkKfStatisticsService>(MockBehavior.Loose).Object);
        }

        return services
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1", permanentCode: null))
            .BuildServiceProvider();
    }
}
