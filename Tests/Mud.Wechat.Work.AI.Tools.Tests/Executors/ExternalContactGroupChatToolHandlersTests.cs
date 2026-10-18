using FluentAssertions;
using Moq;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.ExternalContact.GroupChat;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 客户群域执行器：两跳自举的投影形状、群主过滤的映射，以及「unionid 不外发」这条边界。
/// </summary>
public class ExternalContactGroupChatToolHandlersTests
{
    private readonly Mock<IWechatWorkExternalContactGroupChatService> _groupChat = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldProjectChatIdAndStatusOnly()
    {
        _groupChat
            .Setup(s => s.GetGroupChatListAsync(It.IsAny<GetGroupChatListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupChatListResponse
            {
                ErrorCode = 0,
                GroupChatList =
                [
                    new GroupChatListItem { ChatId = "wr-1", Status = 0 },
                    new GroupChatListItem { ChatId = "wr-2", Status = 2 },
                ],
                NextCursor = "c2",
            });

        var result = await Handler().ListAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"chat_id\":\"wr-1\"");
        result.Text.Should().Contain("\"chat_id\":\"wr-2\"");
        result.Text.Should().Contain("\"count\":2");
        result.Text.Should().Contain("\"page_token\":\"c2\"");
    }

    [Fact]
    public async Task List_WithOwnerFilter_ShouldMapUsersAndPartyToOwnerFilter()
    {
        GetGroupChatListRequest? captured = null;
        _groupChat
            .Setup(s => s.GetGroupChatListAsync(It.IsAny<GetGroupChatListRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetGroupChatListRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new GetGroupChatListResponse { ErrorCode = 0, GroupChatList = [] });

        await Handler().ListAsync(
            Args(("owner_userid", new[] { "zhangsan" }), ("owner_partyid", 7), ("status_filter", 1)),
            CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.StatusFilter.Should().Be(1);
        captured.OwnerFilter.Should().NotBeNull();
        captured.OwnerFilter!.UserIdList.Should().BeEquivalentTo(new[] { "zhangsan" });
        captured.OwnerFilter.PartyIdList.Should().BeEquivalentTo(new[] { 7 });
    }

    [Fact]
    public async Task List_WithoutOwnerFilter_ShouldSendNullOwnerFilter()
    {
        GetGroupChatListRequest? captured = null;
        _groupChat
            .Setup(s => s.GetGroupChatListAsync(It.IsAny<GetGroupChatListRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetGroupChatListRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new GetGroupChatListResponse { ErrorCode = 0, GroupChatList = [] });

        await Handler().ListAsync(Args(), CancellationToken.None);

        captured!.OwnerFilter.Should().BeNull("无过滤项时官方语义是「应用可见范围内全部群主」");
        captured.Limit.Should().Be(500, "未给 limit 时按官方默认 500 下发");
    }

    [Fact]
    public async Task List_StatusFilterOutOfClosedSet_ShouldBeRejectedBeforeCallingSdk()
    {
        var act = async () => await Handler().ListAsync(Args(("status_filter", 9)), CancellationToken.None);

        await act.Should()
            .ThrowAsync<ToolArgumentException>("status_filter 在声明面是闭集（EnumType），越界值在解包期即被拒")
            .WithMessage("*status_filter*", "文案必须点出参数名并附合法取值清单，模型才能据此改参");
        _groupChat.Verify(
            s => s.GetGroupChatListAsync(It.IsAny<GetGroupChatListRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Get_ShouldProjectMembersAndDropUnionId()
    {
        GetGroupChatDetailRequest? captured = null;
        _groupChat
            .Setup(s => s.GetGroupChatDetailAsync(It.IsAny<GetGroupChatDetailRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetGroupChatDetailRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new GetGroupChatDetailResponse
            {
                ErrorCode = 0,
                GroupChat = new GroupChatDetail
                {
                    ChatId = "wr-1",
                    Name = "华东客户群",
                    Owner = "zhangsan",
                    CreateTime = 1700000000,
                    Notice = "每周三答疑",
                    MemberList =
                    [
                        new GroupChatMember
                        {
                            UserId = "wm-1",
                            Type = 2,
                            UnionId = "union-abc",
                            JoinTime = 1700000100,
                            JoinScene = 3,
                            Invitor = new GroupChatInvitor { UserId = "zhangsan" },
                            GroupNickname = "小王",
                            Name = "王小明",
                        },
                    ],
                    AdminList = [new GroupChatAdmin { UserId = "zhangsan" }],
                    MemberVersion = "v-1",
                },
            });

        var result = await Handler().GetAsync(Args(("chat_id", "wr-1"), ("need_name", true)), CancellationToken.None);

        captured!.ChatId.Should().Be("wr-1");
        captured.NeedName.Should().Be(1, "need_name 是官方的 0/1 整数开关");
        result.Text.Should().Contain("\"name\":\"华东客户群\"");
        result.Text.Should().Contain("\"owner\":\"zhangsan\"");
        result.Text.Should().Contain("\"notice\":\"每周三答疑\"");
        result.Text.Should().Contain("\"member_count\":1");
        result.Text.Should().Contain("\"group_nickname\":\"小王\"");
        result.Text.Should().Contain("\"invitor\":\"zhangsan\"");
        result.Text.Should().NotContain("union-abc", "unionid 是跨应用身份关联键，不外发");
    }

    [Fact]
    public async Task Get_WithoutGroupChatPayload_ShouldReturnNotFoundAsSuccess()
    {
        _groupChat
            .Setup(s => s.GetGroupChatDetailAsync(It.IsAny<GetGroupChatDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupChatDetailResponse { ErrorCode = 0, GroupChat = null });

        var result = await Handler().GetAsync(Args(("chat_id", "wr-1")), CancellationToken.None);

        result.Error.Should().BeNull();
        result.Text.Should().Contain("\"found\":false");
    }

    private ExternalContactGroupChatToolHandlers Handler() => new(_groupChat.Object);

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
