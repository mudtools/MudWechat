using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Contacts.Tags;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 标签域执行器：零参工具的调用形态、成员/部门双列投影、name 缺省原样透传。
/// </summary>
public class ContactsTagToolHandlersTests
{
    private readonly Mock<IWechatWorkTagsService> _tags = new(MockBehavior.Loose);

    [Fact]
    public async Task ListTags_ShouldProjectTagIdAndName()
    {
        _tags
            .Setup(t => t.GetTagListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTagListResponse
            {
                ErrorCode = 0,
                TagList = [new TagInfo { TagId = 1, TagName = "管理层" }, new TagInfo { TagId = 2, TagName = "研发" }],
            });

        var result = await Handler().ListTagsAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"count\":2");
        result.Text.Should().Contain("\"tag_id\":1");
        result.Text.Should().Contain("管理层");
    }

    [Fact]
    public async Task ListTags_WhenErrcodeNonZero_ShouldReturnReadableError()
    {
        _tags
            .Setup(t => t.GetTagListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTagListResponse { ErrorCode = 60011, ErrorMessage = "no privilege" });

        var result = await Handler().ListTagsAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("60011");
    }

    [Fact]
    public async Task GetTagMembers_ShouldProjectUsersAndDepartments()
    {
        _tags
            .Setup(t => t.GetTagMembersAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTagMembersResponse
            {
                ErrorCode = 0,
                TagName = "研发",
                UserList = [new TagMemberInfo { UserId = "u1", Name = "员工一" }, new TagMemberInfo { UserId = "u2", Name = null }],
                PartyList = [1, 2],
            });

        var result = await Handler().GetTagMembersAsync(Args(("tag_id", 7)), CancellationToken.None);

        result.Text.Should().Contain("\"tagname\":\"研发\"");
        result.Text.Should().Contain("\"user_count\":2");
        result.Text.Should().Contain("\"department_count\":2");
        result.Text.Should().Contain("\"userid\":\"u2\"", "name 缺省时仍保留 userid（不猜名字，由模型按需再查详情）");
    }

    [Fact]
    public async Task GetTagMembers_WithoutTagId_ShouldThrowParameterError()
    {
        var act = async () => await Handler().GetTagMembersAsync(Args(), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    private ContactsTagToolHandlers Handler() => new(_tags.Object);

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
