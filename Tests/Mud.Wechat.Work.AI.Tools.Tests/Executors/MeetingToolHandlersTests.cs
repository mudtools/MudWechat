using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Meeting;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 会议域执行器：官方跨度约束的调用前校验，以及「入会凭证与参会人行为指纹不外发」这条边界。
/// </summary>
public class MeetingToolHandlersTests
{
    private readonly Mock<IWechatWorkMeetingService> _meeting = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkInternalMeetingService> _meetingInternal = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkInternalMeetingAdvancedService> _advanced = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldReturnMeetingIds()
    {
        _meeting
            .Setup(s => s.GetUserMeetingIdListAsync(It.IsAny<GetUserMeetingIdListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserMeetingIdListResponse
            {
                ErrorCode = 0,
                MeetingidList = ["m-1", "m-2"],
                NextCursor = "c-next",
            });

        var result = await Handler().ListAsync(
            Args(("userid", "zhangsan"), ("begin_time", "1700000000"), ("end_time", "1700100000")),
            CancellationToken.None);

        result.Text.Should().Contain("\"m-1\"");
        result.Text.Should().Contain("\"count\":2");
        result.Text.Should().Contain("\"c-next\"");
    }

    [Fact]
    public async Task List_WhenSpanExceeds180Days_ShouldRejectBeforeCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(
            Args(("userid", "zhangsan"), ("begin_time", "1690000000"), ("end_time", "1710000000")),
            CancellationToken.None));

        result.Text.Should().Contain("180 天");
        _meeting.Verify(
            s => s.GetUserMeetingIdListAsync(It.IsAny<GetUserMeetingIdListRequest>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "越界参数应在调用前拦下，而不是让官方 errcode 回话");
    }

    [Fact]
    public async Task Get_ShouldProjectMeetingFactsWithoutJoinLink()
    {
        _meetingInternal
            .Setup(s => s.GetMeetingInfoAsync(It.IsAny<GetMeetingInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetMeetingInfoResponse
            {
                ErrorCode = 0,
                Title = "周会",
                MeetingStart = 1700000000,
                MeetingDuration = 3600,
                Status = 2,
                MeetingType = 0,
                Location = "3 楼会议室",
                Description = "本周进度同步",
                AdminUserid = "zhangsan",
                MeetingCode = "123456789",
                CalId = "cal-1",
                MeetingLink = "https://meeting.tencent.com/dm/secret-link",
                Attendees = new MeetingAttendees
                {
                    Member =
                    [
                        new MeetingAttendeeMember { Userid = "lisi", Status = 1 },
                    ],
                },
            });

        var result = await Handler().GetAsync(Args(("meetingid", "m-1")), CancellationToken.None);

        result.Text.Should().Contain("\"meetingid\":\"m-1\"");
        result.Text.Should().Contain("\"title\":\"周会\"");
        result.Text.Should().Contain("\"status\":2");
        result.Text.Should().Contain("\"lisi\"");
        result.Text.Should().Contain("\"attendee_count\":1");
        result.Text.Should().NotContain("secret-link", "入会链接是可被任何人使用的凭证，不外发");
    }

    [Fact]
    public async Task Attendees_ShouldProjectWithoutTemporaryOpenidOrCustomerData()
    {
        _advanced
            .Setup(s => s.GetAttendeeListAsync(It.IsAny<GetMeetingAttendeeListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetMeetingAttendeeListResponse
            {
                ErrorCode = 0,
                HasMore = true,
                NextCursor = "c2",
                Attendees =
                [
                    new MeetingAttendedAttendee
                    {
                        Userid = "lisi",
                        Role = 1,
                        JoinTime = "2026-10-10 09:00:00",
                        QuitTime = "2026-10-10 10:00:00",
                        JoinType = 1,
                        TmpOpenid = "tmp-openid-1",
                        Net = "wifi",
                        CustomerData = "客户A",
                    },
                ],
            });

        var result = await Handler().ListAttendeesAsync(Args(("meetingid", "m-1")), CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"lisi\"");
        result.Text.Should().Contain("\"join_time\":\"2026-10-10 09:00:00\"");
        result.Text.Should().Contain("\"has_more\":true");
        result.Text.Should().NotContain("tmp-openid-1", "外部临时身份不外发");
        result.Text.Should().NotContain("\"wifi\"", "网络质量属行为指纹，不外发");
        result.Text.Should().NotContain("客户A", "客户数据字段是自由文本，不外发");
    }

    private MeetingToolHandlers Handler() => new(_meeting.Object, _meetingInternal.Object, _advanced.Object);

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
