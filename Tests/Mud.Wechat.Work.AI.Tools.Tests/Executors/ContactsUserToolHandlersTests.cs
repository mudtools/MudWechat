using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Contracts.Users;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 成员域执行器：字段投影白名单（不含手机号/邮箱）、<c>fetch_child</c> 的 bool→1/0 收敛、失败回填。
/// </summary>
public class ContactsUserToolHandlersTests
{
    private readonly Mock<IWechatWorkUsersService> _users = new(MockBehavior.Loose);

    [Fact]
    public async Task GetUser_ShouldProjectWhitelistedFieldsOnly()
    {
        _users
            .Setup(u => u.GetUserAsync("zhangsan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserInfo
            {
                ErrorCode = 0,
                UserId = "zhangsan",
                Name = "张三",
                Alias = "三儿",
                Position = "工程师",
                Department = [1, 2],
                MainDepartment = 2,
                Status = 1,
                Enable = 1,
                Mobile = "13800000000",
                Email = "zhangsan@example.com",
                Avatar = "https://example.com/a.png",
                DirectLeader = ["lisi"],
            });

        var result = await Handler().GetUserAsync(Args(("userid", "zhangsan")), CancellationToken.None);

        result.Text.Should().Contain("\"name\":\"张三\"");
        result.Text.Should().Contain("\"department\":[1,2]");
        result.Text.Should().Contain("\"direct_leader\":[\"lisi\"]");
        result.Text.Should().NotContain("13800000000", "手机号属个人敏感信息，不得进入模型上下文");
        result.Text.Should().NotContain("example.com", "邮箱同理");
        result.Text.Should().NotContain("a.png", "头像 URL 同理");
    }

    [Fact]
    public async Task ListUserSummaries_ShouldMapFetchChildToSdkFlag()
    {
        _users
            .Setup(u => u.GetUserSimpleListAsync(1, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserSimpleListResponse
            {
                ErrorCode = 0,
                UserList = [new UserSimpleInfo { UserId = "u1", Name = "员工一", Department = [1] }],
            });

        var result = await Handler().ListUserSummariesAsync(
            Args(("department_id", 1), ("fetch_child", true)),
            CancellationToken.None);

        result.Text.Should().Contain("\"count\":1");
        result.Text.Should().Contain("\"userid\":\"u1\"");
        _users.Verify(u => u.GetUserSimpleListAsync(1, 1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListUserSummaries_WithoutFetchChild_ShouldPassNull()
    {
        _users
            .Setup(u => u.GetUserSimpleListAsync(1, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserSimpleListResponse { ErrorCode = 0, UserList = [] });

        await Handler().ListUserSummariesAsync(Args(("department_id", 1)), CancellationToken.None);

        _users.Verify(u => u.GetUserSimpleListAsync(1, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListUsers_ShouldProjectDetailFieldsWithoutLeaders()
    {
        _users
            .Setup(u => u.GetUserDetailListAsync(1, 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserDetailListResponse
            {
                ErrorCode = 0,
                UserList =
                [
                    new UserInfo { UserId = "u1", Name = "员工一", Position = "PM", Department = [1], DirectLeader = ["boss"] },
                ],
            });

        var result = await Handler().ListUsersAsync(
            Args(("department_id", 1), ("fetch_child", false)),
            CancellationToken.None);

        result.Text.Should().Contain("\"position\":\"PM\"");
        result.Text.Should().NotContain("direct_leader", "详情列表不逐人带上级（减少体积，需要时用 contacts.user.get）");
        _users.Verify(u => u.GetUserDetailListAsync(1, 0, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUser_WhenErrcodeNonZero_ShouldReturnReadableError()
    {
        _users
            .Setup(u => u.GetUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserInfo { ErrorCode = 60111, ErrorMessage = "userid not found" });

        var result = await Handler().GetUserAsync(Args(("userid", "nobody")), CancellationToken.None);

        result.Text.Should().Contain("60111");
        result.Text.Should().Contain("userid not found");
    }

    [Fact]
    public async Task ListIds_ShouldProjectUseridAndDepartmentOnly()
    {
        _users
            .Setup(u => u.ListUserIdsAsync(It.IsAny<ListUserIdsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListUserIdsResponse
            {
                ErrorCode = 0,
                DeptUser =
                [
                    new UserDepartmentInfo { UserId = "zhangsan", Department = 2 },
                    new UserDepartmentInfo { OpenUserId = "wo-open-1", Department = 2 },
                ],
                NextCursor = "c2",
            });

        var result = await Handler().ListIdsAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"open_userid\":\"wo-open-1\"");
        result.Text.Should().Contain("\"department\":2");
        result.Text.Should().Contain("\"count\":2");
        result.Text.Should().Contain("\"page_token\":\"c2\"");
    }

    [Fact]
    public async Task ListIds_LimitOverOfficialMax_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListIdsAsync(Args(("limit", 50000)), CancellationToken.None));

        result.Text.Should().Contain("1~10000");
        _users.Verify(
            u => u.ListUserIdsAsync(It.IsAny<ListUserIdsRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ResolveByMobile_ShouldReturnUseridWithoutEchoingMobile()
    {
        _users
            .Setup(u => u.GetUserIdByMobileAsync(
                It.Is<GetUserIdByMobileRequest>(r => r.Mobile == "13800000000"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserIdByMobileResponse { ErrorCode = 0, UserId = "zhangsan" });

        var result = await Handler().ResolveUserAsync(Args(("mobile", "13800000000")), CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"count\":1");
        result.Text.Should().NotContain("13800000000", "入参手机号不得回显（模型上下文与日志留存期远长于一次请求）");
    }

    [Fact]
    public async Task ResolveByEmail_ShouldReturnUserid()
    {
        _users
            .Setup(u => u.GetUserIdByEmailAsync(
                It.Is<GetUserIdByEmailRequest>(r => r.Email == "zhangsan@example.com"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserIdByEmailResponse { ErrorCode = 0, UserId = "zhangsan" });

        var result = await Handler().ResolveUserAsync(Args(("email", "zhangsan@example.com")), CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().NotContain("zhangsan@example.com");
    }

    [Fact]
    public async Task ResolveByMobile_WhenNotFound_ShouldReturnEmptyMatchesWithHint()
    {
        _users
            .Setup(u => u.GetUserIdByMobileAsync(It.IsAny<GetUserIdByMobileRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserIdByMobileResponse { ErrorCode = 0, UserId = string.Empty });

        var result = await Handler().ResolveUserAsync(Args(("mobile", "13800000001")), CancellationToken.None);

        result.Text.Should().Contain("\"count\":0");
        result.Text.Should().Contain("hint");
    }

    [Fact]
    public async Task ResolveByName_WithoutDepartment_ShouldExplainWhyItNeedsDepartment()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ResolveUserAsync(Args(("name", "张三")), CancellationToken.None));

        result.Text.Should().Contain("department_id");
        _users.Verify(
            u => u.GetUserSimpleListAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ResolveByName_WithDepartment_ShouldMatchWithinDepartmentAndCapResults()
    {
        var users = Enumerable
            .Range(0, 25)
            .Select(i => new UserSimpleInfo { UserId = $"zhangsan{i}", Name = "张三", Department = [2] })
            .ToList();
        users.Add(new UserSimpleInfo { UserId = "other", Name = "李四", Department = [2] });

        _users
            .Setup(u => u.GetUserSimpleListAsync(2, 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserSimpleListResponse { ErrorCode = 0, UserList = users });

        var result = await Handler().ResolveUserAsync(
            Args(("name", "张三"), ("department_id", 2)),
            CancellationToken.None);

        result.Text.Should().Contain("\"count\":20", "结果条数上限防止整部门进上下文");
        result.Text.Should().Contain("缩小部门范围");
        result.Text.Should().NotContain("\"userid\":\"other\"");
    }

    [Fact]
    public async Task Resolve_WithMultipleIdentifiers_ShouldReject()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ResolveUserAsync(
            Args(("mobile", "13800000000"), ("email", "a@b.com")),
            CancellationToken.None));

        result.Text.Should().Contain("三选一");
        _users.Verify(
            u => u.GetUserIdByMobileAsync(It.IsAny<GetUserIdByMobileRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Resolve_WithoutAnyIdentifier_ShouldReject()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ResolveUserAsync(Args(), CancellationToken.None));

        result.Text.Should().Contain("mobile");
    }

    private ContactsUserToolHandlers Handler() => new(_users.Object);

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
