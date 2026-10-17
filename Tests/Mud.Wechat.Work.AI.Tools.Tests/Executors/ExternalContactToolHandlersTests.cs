using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.ExternalContact.Customer;
using Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;
using Mud.Wechat.Work.DataModels.ExternalContact.Tag;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 客户联系域执行器：三跳自举的投影形状，以及「客户个人数据不外发」这条边界。
/// </summary>
public class ExternalContactToolHandlersTests
{
    private readonly Mock<IWechatWorkExternalContactFollowUserService> _followUser = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkExternalContactCustomerService> _customer = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkExternalContactTagService> _tag = new(MockBehavior.Loose);

    [Fact]
    public async Task FollowUserList_ShouldReturnUseridsOnly()
    {
        _followUser
            .Setup(s => s.GetFollowUserListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetFollowUserListResponse { ErrorCode = 0, FollowUser = ["zhangsan", "lisi"] });

        var result = await Handler().ListFollowUsersAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"zhangsan\"");
        result.Text.Should().Contain("\"count\":2");
    }

    [Fact]
    public async Task ContactList_ShouldReturnExternalUseridsOnly()
    {
        _customer
            .Setup(s => s.GetCustomerListAsync("zhangsan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCustomerListResponse { ErrorCode = 0, ExternalUserid = ["wm-1", "wm-2"] });

        var result = await Handler().ListCustomersAsync(Args(("userid", "zhangsan")), CancellationToken.None);

        result.Text.Should().Contain("\"wm-1\"");
        result.Text.Should().Contain("\"count\":2");
    }

    [Fact]
    public async Task ContactGet_ShouldProjectFollowFactsWithoutPersonalData()
    {
        _customer
            .Setup(s => s.GetCustomerDetailAsync("wm-1", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCustomerDetailResponse
            {
                ErrorCode = 0,
                ExternalContact = new ExternalContactInfo
                {
                    ExternalUserid = "wm-1",
                    Name = "王小明",
                    Type = 1,
                    Gender = 1,
                    Position = "采购经理",
                    CorpName = "某某科技",
                    Avatar = "https://cdn.example/avatar.png",
                    Unionid = "union-abc",
                },
                FollowUser =
                [
                    new CustomerFollowUser
                    {
                        Userid = "zhangsan",
                        Remark = "王总（华东）",
                        Createtime = 1700000000,
                        AddWay = 1,
                        State = "正常",
                        Description = "客户有两个孩子，住天河",
                        RemarkMobiles = ["13900000000"],
                        WechatChannels = new WechatChannelsInfo { Nickname = "视频号昵称" },
                        Tags =
                        [
                            new CustomerFollowTag { GroupName = "客户级别", TagName = "A 类", TagId = "t-1" },
                        ],
                    },
                ],
            });

        var result = await Handler().GetCustomerAsync(Args(("external_userid", "wm-1")), CancellationToken.None);

        result.Text.Should().Contain("\"external_userid\":\"wm-1\"");
        result.Text.Should().Contain("\"name\":\"王小明\"");
        result.Text.Should().Contain("\"corp_name\":\"某某科技\"");
        result.Text.Should().Contain("\"position\":\"采购经理\"");
        result.Text.Should().Contain("\"remark\":\"王总（华东）\"");
        result.Text.Should().Contain("\"A 类\"");
        result.Text.Should().Contain("\"follow_user_count\":1");
        result.Text.Should().NotContain("avatar.png", "头像 URL 不外发");
        result.Text.Should().NotContain("union-abc", "unionid 是跨应用身份关联键，不外发");
        result.Text.Should().NotContain("13800000000", "备注手机号不外发");
        result.Text.Should().NotContain("13900000000");
        result.Text.Should().NotContain("客户有两个孩子", "跟进备注描述是自由文本，可能含个人情况，不外发");
        result.Text.Should().NotContain("视频号昵称");
    }

    [Fact]
    public async Task TagList_ShouldSkipDeletedGroupsAndTags()
    {
        _tag
            .Setup(s => s.GetCorpTagListAsync(It.IsAny<GetCorpTagListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTagListResponse
            {
                ErrorCode = 0,
                TagGroup =
                [
                    new CorpTagGroupItem
                    {
                        GroupId = "g-1",
                        GroupName = "客户级别",
                        Tag =
                        [
                            new CorpTagItem { Id = "t-1", Name = "A 类" },
                            new CorpTagItem { Id = "t-2", Name = "已废弃", Deleted = true },
                        ],
                    },
                    new CorpTagGroupItem { GroupId = "g-2", GroupName = "已删组", Deleted = true, Tag = [] },
                ],
            });

        var result = await Handler().ListTagsAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"group_name\":\"客户级别\"");
        result.Text.Should().Contain("\"A 类\"");
        result.Text.Should().Contain("\"tag_count\":1");
        result.Text.Should().NotContain("已废弃", "软删除的标签不投影");
        result.Text.Should().NotContain("已删组", "整组删除时组内不投影");
    }

    private ExternalContactToolHandlers Handler() => new(_followUser.Object, _customer.Object, _tag.Object);

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
