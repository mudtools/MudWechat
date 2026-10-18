using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.ExternalContact.Moment;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 客户朋友圈域执行器：三跳自举的投影形状、30 天窗口与时间格式的先行校验，
/// 以及「media_id（宿主素材）与经纬度（行踪）不外发」这条边界。
/// </summary>
public class ExternalContactMomentToolHandlersTests
{
    private readonly Mock<IWechatWorkExternalContactMomentService> _moment = new(MockBehavior.Loose);

    [Fact]
    public async Task MomentList_ShouldProjectContentFactsWithoutMediaIdOrCoordinates()
    {
        _moment
            .Setup(s => s.GetMomentListAsync(It.IsAny<GetMomentListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetMomentListResponse
            {
                ErrorCode = 0,
                MomentList =
                [
                    new MomentRecord
                    {
                        MomentId = "mo-1",
                        Creator = "zhangsan",
                        CreateTime = 1700000000,
                        CreateType = 1,
                        VisibleType = 1,
                        Text = new MomentText { Content = "新品上市" },
                        Image =
                        [
                            new MomentImageMedia { MediaId = "media-1" },
                            new MomentImageMedia { MediaId = "media-2" },
                        ],
                        Video = new MomentVideoMedia { MediaId = "media-v", ThumbMediaId = "thumb-v" },
                        Link = new MomentLinkMedia { Title = "活动详情", Url = "https://example.com/a" },
                        Location = new MomentLocation { Latitude = 23.1234, Longitude = 113.4567, Name = "广州塔" },
                    },
                ],
                NextCursor = "c2",
            });

        var result = await Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1700086400")),
            CancellationToken.None);

        result.Text.Should().Contain("\"moment_id\":\"mo-1\"");
        result.Text.Should().Contain("\"text\":\"新品上市\"");
        result.Text.Should().Contain("\"image_count\":2");
        result.Text.Should().Contain("\"has_video\":true");
        result.Text.Should().Contain("\"title\":\"活动详情\"");
        result.Text.Should().Contain("\"location\":\"广州塔\"");
        result.Text.Should().Contain("\"next_cursor\":\"c2\"");
        result.Text.Should().NotContain("media-1", "media_id 是宿主素材资产，模型无法消费二进制");
        result.Text.Should().NotContain("thumb-v");
        result.Text.Should().NotContain("23.1234", "经纬度是行踪，不外发");
        result.Text.Should().NotContain("113.4567");
    }

    [Fact]
    public async Task MomentList_SpanOverThirtyDays_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1702764800")),
            CancellationToken.None);

        result.Text.Should().Contain("30 天");
        VerifyNoCall();
    }

    [Fact]
    public async Task MomentList_TimeWithoutOffset_ShouldReturnFormatHint()
    {
        var result = await Handler().ListAsync(
            Args(("start_time", "2026-10-10 09:00:00"), ("end_time", "2026-10-11T09:00:00+08:00")),
            CancellationToken.None);

        result.Text.Should().Contain("时间格式非法");
        VerifyNoCall();
    }

    [Fact]
    public async Task MomentList_LimitOverOfficialMax_ShouldReturnError()
    {
        var result = await Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1700086400"), ("limit", 50)),
            CancellationToken.None);

        result.Text.Should().Contain("1~20");
        VerifyNoCall();
    }

    [Fact]
    public async Task PublishList_ShouldProjectUseridAndPublishStatus()
    {
        _moment
            .Setup(s => s.GetMomentPublishListAsync(It.IsAny<GetMomentPublishListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetMomentPublishListResponse
            {
                ErrorCode = 0,
                TaskList =
                [
                    new MomentPublishTaskItem { Userid = "zhangsan", PublishStatus = 1 },
                    new MomentPublishTaskItem { Userid = "lisi", PublishStatus = 0 },
                ],
            });

        var result = await Handler().ListPublishTasksAsync(
            Args(("moment_id", "mo-1")),
            CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"publish_status\":0");
        result.Text.Should().Contain("\"count\":2");
    }

    [Fact]
    public async Task Comments_ShouldDistinguishCustomerAndMemberActors()
    {
        _moment
            .Setup(s => s.GetMomentCommentsAsync(It.IsAny<GetMomentCommentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetMomentCommentsResponse
            {
                ErrorCode = 0,
                CommentList =
                [
                    new MomentCommentItem { ExternalUserid = "wm-1", CreateTime = 1700000200 },
                    new MomentCommentItem { Userid = "zhangsan", CreateTime = 1700000300 },
                ],
                LikeList =
                [
                    new MomentCommentItem { ExternalUserid = "wm-2", CreateTime = 1700000400 },
                ],
            });

        var result = await Handler().GetCommentsAsync(
            Args(("moment_id", "mo-1"), ("userid", "zhangsan")),
            CancellationToken.None);

        result.Text.Should().Contain("\"actor\":\"customer\"");
        result.Text.Should().Contain("\"external_userid\":\"wm-1\"");
        result.Text.Should().Contain("\"actor\":\"member\"");
        result.Text.Should().Contain("\"comment_count\":2");
        result.Text.Should().Contain("\"like_count\":1");
    }

    private ExternalContactMomentToolHandlers Handler() => new(_moment.Object);

    private void VerifyNoCall()
        => _moment.Verify(
            s => s.GetMomentListAsync(It.IsAny<GetMomentListRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);

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
