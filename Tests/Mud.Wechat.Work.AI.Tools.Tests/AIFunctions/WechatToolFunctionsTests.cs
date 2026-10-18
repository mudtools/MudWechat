using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Mud.Wechat.Work.AI.Tools;
using Mud.Wechat.Work.AI.Tools.Extensions;
using Mud.Wechat.Work.AI.Tools.Security;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.AIFunctions;

/// <summary>
/// 工具面 → MAF 投影：按白名单投影、Schema 只取 parameters 子块、调用回落到执行链（含门禁）。
/// </summary>
public class WechatToolFunctionsTests
{
    [Fact]
    public void Create_WithDefaultPolicy_ShouldProjectReadonlyToolsOnly()
    {
        using var provider = BuildProvider(_ => { }, authorizer: null);

        var functions = provider.GetRequiredService<IReadOnlyList<AIFunction>>();

        functions.Select(f => f.Name).Should().Equal(WechatToolNames.ReadonlyAll);
        functions.Should().OnlyContain(f => !string.IsNullOrWhiteSpace(f.Description));
    }

    [Fact]
    public void Create_ShouldProjectParametersSubSchemaOnly()
    {
        using var provider = BuildProvider(_ => { }, authorizer: null);

        var function = provider.GetRequiredService<IReadOnlyList<AIFunction>>()
            .Single(f => f.Name == WechatToolNames.ContactsDepartmentGet);

        function.JsonSchema.ValueKind.Should().Be(JsonValueKind.Object);
        function.JsonSchema.TryGetProperty("properties", out var properties).Should().BeTrue();
        properties.TryGetProperty("department_id", out _).Should().BeTrue();
        function.JsonSchema.TryGetProperty("x-wechat", out _).Should().BeFalse(
            "厂商扩展块不属于参数 schema——塞给模型会被当作参数名");
    }

    [Fact]
    public void Create_WhenToolDisabled_ShouldNotProjectIt()
    {
        using var provider = BuildProvider(p => p.Disable(WechatToolNames.ContactsDepartmentGet), authorizer: null);

        var names = provider.GetRequiredService<IReadOnlyList<AIFunction>>().Select(f => f.Name);

        names.Should().NotContain(WechatToolNames.ContactsDepartmentGet);
        names.Should().HaveCount(WechatToolNames.ReadonlyAll.Length - 1);
    }

    [Fact]
    public async Task InvokeAsync_ShouldRouteThroughDispatcherAndAuthorizer()
    {
        var departments = Departments();
        var authorizer = AllowAll();
        using var provider = BuildProvider(_ => { }, authorizer.Object, departments);
        var function = provider.GetRequiredService<IReadOnlyList<AIFunction>>()
            .Single(f => f.Name == WechatToolNames.ContactsDepartmentGet);

        var result = await function.InvokeAsync(
            new AIFunctionArguments(new Dictionary<string, object?> { ["department_id"] = 2 }));

        result.Should().BeOfType<string>();
        ((string)result!).Should().Contain("研发部");
        authorizer.Verify(
            a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()),
            Times.Once);
        departments.Verify(d => d.GetDepartmentAsync(2, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithoutAuthorizer_ShouldReturnNeutralDenial()
    {
        using var provider = BuildProvider(_ => { }, authorizer: null);
        var function = provider.GetRequiredService<IReadOnlyList<AIFunction>>()
            .Single(f => f.Name == WechatToolNames.ContactsDepartmentGet);

        var result = await function.InvokeAsync(
            new AIFunctionArguments(new Dictionary<string, object?> { ["department_id"] = 2 }));

        ((string)result!).Should().Contain("拒绝");
    }

    private static Mock<IWechatToolAuthorizer> AllowAll()
    {
        var authorizer = new Mock<IWechatToolAuthorizer>(MockBehavior.Strict);
        authorizer
            .Setup(a => a.AuthorizeAsync(It.IsAny<WechatToolAuthorizationContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(WechatToolAuthorizationResult.Allow());
        return authorizer;
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

    private static ServiceProvider BuildProvider(
        Action<WechatToolPolicy> policy,
        IWechatToolAuthorizer? authorizer,
        Mock<IWechatWorkDepartmentsService>? departments = null)
    {
        var services = new ServiceCollection().AddSingleton((departments ?? Departments()).Object);
        if (authorizer is not null)
        {
            services.AddSingleton(authorizer);
        }

        return services
            .AddWechatWorkTools(new WechatToolBinding("corp-app", "corp-1"), policy)
            .BuildServiceProvider();
    }
}
