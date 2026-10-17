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

        using var provider = BuildProvider(departments.Object);
        var registry = provider.GetRequiredService<WechatToolRegistry>();

        registry.ToolNames.Should().Equal(WechatToolNames.All);
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

    private static ServiceProvider BuildProvider(IWechatWorkDepartmentsService departments)
        => new ServiceCollection()
            .AddSingleton(departments)
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1", permanentCode: null))
            .BuildServiceProvider();
}
