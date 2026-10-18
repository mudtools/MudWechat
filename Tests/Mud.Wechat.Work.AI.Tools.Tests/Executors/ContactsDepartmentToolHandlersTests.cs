using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Contacts.Department;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 执行器行为：成功投影为模型可读 JSON、业务失败回填可读错误（不抛）、必填参数缺失按参数错误抛出。
/// </summary>
public class ContactsDepartmentToolHandlersTests
{
    private readonly Mock<IWechatWorkDepartmentsService> _departments = new(MockBehavior.Loose);

    [Fact]
    public async Task GetDepartment_ShouldProjectDepartmentFields()
    {
        _departments
            .Setup(d => d.GetDepartmentAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentResponse
            {
                ErrorCode = 0,
                Department = new DepartmentInfo
                {
                    Id = 2,
                    Name = "研发部",
                    NameEn = "R&D",
                    ParentId = 1,
                    DepartmentLeader = ["zhangsan"],
                },
            });

        var result = await Handler().GetDepartmentAsync(Args(("department_id", 2)), CancellationToken.None);

        result.Text.Should().Contain("\"id\":2");
        result.Text.Should().Contain("\"name\":\"研发部\"");
        result.Text.Should().Contain("\"parentid\":1");
        result.Text.Should().Contain("zhangsan");
        result.Truncated.Should().BeFalse();
    }

    [Fact]
    public async Task GetDepartment_WhenErrcodeNonZero_ShouldReturnReadableError()
    {
        _departments
            .Setup(d => d.GetDepartmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentResponse { ErrorCode = 60011, ErrorMessage = "no privilege to access/modify contact/party/agent" });

        var result = await Handler().GetDepartmentAsync(Args(("department_id", 2)), CancellationToken.None);

        result.Text.Should().Contain("60011");
        result.Text.Should().Contain("no privilege");
    }

    [Fact]
    public async Task GetDepartment_WhenPayloadMissing_ShouldReturnNotFoundAsSuccess()
    {
        _departments
            .Setup(d => d.GetDepartmentAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentResponse { ErrorCode = 0, Department = null });

        var result = await Handler().GetDepartmentAsync(Args(("department_id", 2)), CancellationToken.None);

        result.Error.Should().BeNull("权限内确实没有这个部门 ⇒ 成功语义，模型据实回答「没有」");
        result.Text.Should().Contain("\"found\":false");
    }

    [Fact]
    public async Task GetDepartment_WithoutRequiredArgument_ShouldThrowParameterError()
    {
        var act = async () => await Handler().GetDepartmentAsync(Args(), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>("必填参数缺失是参数类错误（执行链归类 Validation/invalid_args）");
    }

    [Fact]
    public async Task ListDepartments_WithoutRootId_ShouldQueryWholeTree()
    {
        _departments
            .Setup(d => d.GetDepartmentListAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDepartmentListResponse
            {
                ErrorCode = 0,
                Department = [new DepartmentInfo { Id = 1, Name = "总部" }],
            });

        var result = await Handler().ListDepartmentsAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"count\":1");
        _departments.Verify(d => d.GetDepartmentListAsync(null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListChildDepartmentIds_ShouldProjectIdAndParent()
    {
        _departments
            .Setup(d => d.GetChildDepartmentIdListAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetChildDepartmentIdListResponse
            {
                ErrorCode = 0,
                DepartmentIds = [new DepartmentIdInfo { Id = 2, ParentId = 1 }],
            });

        var result = await Handler().ListChildDepartmentIdsAsync(Args(("department_id", 1)), CancellationToken.None);

        result.Text.Should().Contain("\"id\":2");
        result.Text.Should().Contain("\"parentid\":1");
    }

    private ContactsDepartmentToolHandlers Handler() => new(_departments.Object);

    private static Dictionary<string, object?> Args(params (string Key, object? Value)[] pairs)
    {
        var args = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var (key, value) in pairs)
        {
            args[key] = value;
        }

        return args;
    }
}
