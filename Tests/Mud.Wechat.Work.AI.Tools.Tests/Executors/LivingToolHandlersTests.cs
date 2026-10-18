using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Living;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 直播域执行器：列表/详情/观看统计的投影，以及「推流凭证与外部联系人数据不外发」这条边界。
/// </summary>
public class LivingToolHandlersTests
{
    private readonly Mock<IWechatWorkLivingService> _living = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldReturnLivingIds()
    {
        _living
            .Setup(s => s.GetUserAllLivingIdAsync(It.IsAny<GetUserAllLivingIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUserAllLivingIdResponse
            {
                ErrorCode = 0,
                LivingidList = ["lv-1", "lv-2"],
                NextCursor = "c2",
            });

        var result = await Handler().ListAsync(Args(("userid", "zhangsan")), CancellationToken.None);

        result.Text.Should().Contain("\"lv-1\"");
        result.Text.Should().Contain("\"count\":2");
        result.Text.Should().Contain("\"c2\"");
    }

    [Fact]
    public async Task Get_ShouldProjectFactsWithoutPushStreamUrl()
    {
        _living
            .Setup(s => s.GetLivingInfoAsync("lv-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetLivingInfoResponse
            {
                ErrorCode = 0,
                LivingInfo = new LivingDetail
                {
                    Theme = "季度全员会",
                    LivingStart = 1700000000,
                    LivingDuration = 3600,
                    Status = 2,
                    AnchorUserid = "zhangsan",
                    ViewerNum = 120,
                    CommentNum = 30,
                    PushStreamUrl = "rtmp://push.example/secret-key",
                },
            });

        var result = await Handler().GetAsync(Args(("livingid", "lv-1")), CancellationToken.None);

        result.Text.Should().Contain("\"theme\":\"季度全员会\"");
        result.Text.Should().Contain("\"viewer_num\":120");
        result.Text.Should().Contain("\"status\":2");
        result.Text.Should().NotContain("rtmp://push.example/secret-key", "推流地址是主播凭证，不外发");
    }

    [Fact]
    public async Task WatchStat_ShouldProjectMembersButOnlyCountExternals()
    {
        _living
            .Setup(s => s.GetWatchStatAsync(It.IsAny<GetLivingWatchStatRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetLivingWatchStatResponse
            {
                ErrorCode = 0,
                Ending = 1,
                NextKey = "k2",
                StatInfo = new LivingWatchStatInfo
                {
                    Users =
                    [
                        new LivingWatchStatUser
                        {
                            Userid = "lisi",
                            WatchTime = 1800,
                            IsComment = 1,
                            IsMic = 0,
                            InvitorUserid = "zhangsan",
                        },
                    ],
                    ExternalUsers =
                    [
                        new LivingWatchStatExternalUser { ExternalUserid = "wm-secret", WatchTime = 600 },
                    ],
                },
            });

        var result = await Handler().GetWatchStatAsync(Args(("livingid", "lv-1")), CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"lisi\"");
        result.Text.Should().Contain("\"watch_time\":1800");
        result.Text.Should().Contain("\"external_user_count\":1");
        result.Text.Should().Contain("\"has_more\":false", "ending=1 表示已拉完");
        result.Text.Should().NotContain("page_token", "末页不得下发游标（否则模型会重复拉最后一页）");
        result.Text.Should().NotContain("wm-secret", "外部联系人 ID 不外发（只给人数）");
    }

    private LivingToolHandlers Handler() => new(_living.Object);

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
