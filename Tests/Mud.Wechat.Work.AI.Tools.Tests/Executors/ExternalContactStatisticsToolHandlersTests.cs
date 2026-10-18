using FluentAssertions;
using Moq;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.ExternalContact.Statistics;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 客户统计域执行器：聚合投影形状、官方跨度/闭集参数在调用前的收口，以及「未给主体/默认分页」的边界。
/// </summary>
public class ExternalContactStatisticsToolHandlersTests
{
    private readonly Mock<IWechatWorkExternalContactStatisticsService> _statistics = new(MockBehavior.Loose);

    [Fact]
    public async Task UserBehavior_ShouldProjectDailyAggregates()
    {
        _statistics
            .Setup(s => s.GetUserBehaviorDataAsync(It.IsAny<GetUserBehaviorDataRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserBehaviorDataResponse
            {
                ErrorCode = 0,
                BehaviorData =
                [
                    new UserBehaviorData
                    {
                        StatTime = 1700000000,
                        NewApplyCnt = 3,
                        NewContactCnt = 5,
                        ChatCnt = 42,
                        MessageCnt = 180,
                        ReplyPercentage = 0.86,
                        AvgReplyTime = 120.5,
                        NegativeFeedbackCnt = 1,
                    },
                ],
            });

        var result = await Handler().QueryUserBehaviorAsync(
            Args(("userid", new[] { "zhangsan" }), ("start_time", "1700000000"), ("end_time", "1700086400")),
            CancellationToken.None);

        result.Text.Should().Contain("\"new_contact_cnt\":5");
        result.Text.Should().Contain("\"chat_cnt\":42");
        result.Text.Should().Contain("\"reply_percentage\":0.86");
        result.Text.Should().Contain("\"count\":1");
    }

    [Fact]
    public async Task UserBehavior_WithoutUseridAndDepartment_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().QueryUserBehaviorAsync(
            Args(("start_time", "1700000000"), ("end_time", "1700086400")),
            CancellationToken.None));

        result.Text.Should().Contain("至少要给一个");
        VerifyNoCall();
    }

    [Fact]
    public async Task UserBehavior_SpanOverThirtyDays_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().QueryUserBehaviorAsync(
            Args(
                ("userid", new[] { "zhangsan" }),
                ("start_time", "1700000000"),
                ("end_time", "1702764800")),
            CancellationToken.None));

        result.Text.Should().Contain("30 天");
        VerifyNoCall();
    }

    [Fact]
    public async Task GroupChat_ShouldProjectOwnerRanking()
    {
        _statistics
            .Setup(s => s.GetGroupChatStatisticAsync(It.IsAny<GetGroupChatStatisticRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupChatStatisticResponse
            {
                ErrorCode = 0,
                Total = 1,
                NextOffset = 1,
                Items =
                [
                    new GroupChatStatisticItem
                    {
                        Owner = "zhangsan",
                        Data = new GroupChatStatisticData
                        {
                            NewChatCnt = 2,
                            ChatTotal = 12,
                            ChatHasMsg = 9,
                            NewMemberCnt = 15,
                            MemberTotal = 340,
                            MemberHasMsg = 210,
                            MsgTotal = 1580,
                            MigrateTraineeChatCnt = 1,
                        },
                    },
                ],
            });

        var result = await Handler().QueryGroupChatAsync(
            Args(("day_begin_time", "1700000000")),
            CancellationToken.None);

        result.Text.Should().Contain("\"owner\":\"zhangsan\"");
        result.Text.Should().Contain("\"chat_total\":12");
        result.Text.Should().Contain("\"member_total\":340");
        result.Text.Should().Contain("\"total\":1");
        result.Text.Should().Contain("\"has_more\":true");
        result.Text.Should().Contain("\"page_token\":\"1\"");
    }

    [Fact]
    public async Task GroupChat_WithoutOptionalArgs_ShouldSendOfficialDefaults()
    {
        GetGroupChatStatisticRequest? captured = null;
        _statistics
            .Setup(s => s.GetGroupChatStatisticAsync(It.IsAny<GetGroupChatStatisticRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetGroupChatStatisticRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new GetGroupChatStatisticResponse { ErrorCode = 0, Items = [] });

        await Handler().QueryGroupChatAsync(Args(("day_begin_time", "1700000000")), CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.DayEndTime.Should().BeNull("未给结束日期时由官方回落为「同起始日期」");
        captured.OwnerFilter.Should().BeNull("无过滤项时官方语义是「应用可见范围内全部群主」");
        captured.OrderAsc.Should().BeNull();
        captured.Offset.Should().Be(0);
        captured.Limit.Should().Be(500);
    }

    [Fact]
    public async Task GroupChat_OrderByOutOfClosedSet_ShouldBeRejectedBeforeCallingSdk()
    {
        var act = async () => await Handler().QueryGroupChatAsync(
            Args(("day_begin_time", "1700000000"), ("order_by", 9)),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<ToolArgumentException>("order_by 在声明面是闭集（EnumType），越界值在解包期即被拒")
            .WithMessage("*order_by*", "文案必须点出参数名并附合法取值清单，模型才能据此改参");
        VerifyNoCall();
    }

    [Fact]
    public async Task GroupChatByDay_ShouldProjectDailySeries()
    {
        _statistics
            .Setup(s => s.GetGroupChatStatisticGroupByDayAsync(It.IsAny<GetGroupChatStatisticGroupByDayRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupChatStatisticGroupByDayResponse
            {
                ErrorCode = 0,
                Items =
                [
                    new GroupChatStatisticDayItem
                    {
                        StatTime = 1699920000,
                        Data = new GroupChatStatisticData { NewChatCnt = 1, ChatTotal = 8, MsgTotal = 96 },
                    },
                ],
            });

        var result = await Handler().QueryGroupChatByDayAsync(
            Args(("day_begin_time", "1699920000"), ("day_end_time", "1700000000")),
            CancellationToken.None);

        result.Text.Should().Contain("\"stat_time\":1699920000");
        result.Text.Should().Contain("\"chat_total\":8");
        result.Text.Should().Contain("\"msg_total\":96");
        result.Text.Should().Contain("\"count\":1");
    }

    [Fact]
    public async Task GroupChatByDay_EndBeforeBegin_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().QueryGroupChatByDayAsync(
            Args(("day_begin_time", "1700086400"), ("day_end_time", "1700000000")),
            CancellationToken.None));

        result.Text.Should().Contain("day_end_time");
        VerifyNoCall();
    }

    private ExternalContactStatisticsToolHandlers Handler() => new(_statistics.Object);

    private void VerifyNoCall()
    {
        _statistics.Verify(
            s => s.GetUserBehaviorDataAsync(It.IsAny<GetUserBehaviorDataRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _statistics.Verify(
            s => s.GetGroupChatStatisticAsync(It.IsAny<GetGroupChatStatisticRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _statistics.Verify(
            s => s.GetGroupChatStatisticGroupByDayAsync(It.IsAny<GetGroupChatStatisticGroupByDayRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

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
