using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Kf;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 微信客服执行器：账号入口、接待人员状态、会话状态语义，以及文本发送的参数收口。
/// </summary>
public class KfToolHandlersTests
{
    private readonly Mock<IWechatWorkKfAccountService> _accounts = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkKfServicerService> _servicers = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkKfSessionService> _sessions = new(MockBehavior.Loose);
    private readonly Mock<IWechatWorkKfStatisticsService> _statistics = new(MockBehavior.Loose);

    [Fact]
    public async Task AccountList_ShouldReturnOpenKfIds()
    {
        _accounts
            .Setup(s => s.GetAccountListAsync(It.IsAny<GetKfAccountListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetKfAccountListResponse
            {
                ErrorCode = 0,
                AccountList =
                [
                    new KfAccount { OpenKfId = "kf-1", Name = "售前客服", ManagePrivilege = true, Avatar = "https://x/a.png" },
                ],
            });

        var result = await Handler().ListAccountsAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"open_kfid\":\"kf-1\"");
        result.Text.Should().Contain("\"name\":\"售前客服\"");
        result.Text.Should().Contain("\"count\":1");
        result.Text.Should().NotContain("a.png", "账号头像不外发");
    }

    [Fact]
    public async Task ServicerList_ShouldProjectStatusAndStopType()
    {
        _servicers
            .Setup(s => s.GetServicerListAsync("kf-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetKfServicerListResponse
            {
                ErrorCode = 0,
                ServicerList =
                [
                    new KfServicer { UserId = "zhangsan", Status = 1, StopType = 1, DepartmentId = 2 },
                ],
            });

        var result = await Handler().ListServicersAsync(Args(("open_kfid", "kf-1")), CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"status\":1");
        result.Text.Should().Contain("\"stop_type\":1");
    }

    [Fact]
    public async Task SessionGet_ShouldDescribeServiceState()
    {
        _sessions
            .Setup(s => s.GetServiceStateAsync(It.IsAny<GetKfServiceStateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetKfServiceStateResponse { ErrorCode = 0, ServiceState = 3, ServicerUserId = "lisi" });

        var result = await Handler().GetSessionAsync(
            Args(("open_kfid", "kf-1"), ("external_userid", "wm-1")),
            CancellationToken.None);

        result.Text.Should().Contain("\"service_state\":3");
        result.Text.Should().Contain("由人工接待");
        result.Text.Should().Contain("\"servicer_userid\":\"lisi\"");
    }

    [Fact]
    public async Task Send_ShouldPassTextBodyAndReturnMsgId()
    {
        _sessions
            .Setup(s => s.SendTextMessageAsync(It.IsAny<SendKfTextMsgRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendKfMsgResponse { ErrorCode = 0, MsgId = "msg-1" });

        var result = await Handler().SendMessageAsync(
            Args(("open_kfid", "kf-1"), ("touser", "wm-1"), ("content", "您好，已收到您的咨询")),
            CancellationToken.None);

        result.Text.Should().Contain("\"ok\":true");
        result.Text.Should().Contain("\"msgid\":\"msg-1\"");
        _sessions.Verify(
            s => s.SendTextMessageAsync(
                It.Is<SendKfTextMsgRequest>(r =>
                    r.OpenKfId == "kf-1"
                    && r.ToUser == "wm-1"
                    && r.MsgType == "text"
                    && r.Text != null
                    && r.Text.Content == "您好，已收到您的咨询"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Send_WhenContentBlank_ShouldRejectBeforeCallingSdk()
    {
        var act = async () => await Handler().SendMessageAsync(
            Args(("open_kfid", "kf-1"), ("touser", "wm-1"), ("content", "   ")),
            CancellationToken.None);

        await act.Should()
            .ThrowAsync<ArgumentException>("content 在声明面是必填（Required），空白值在解包期即被拒")
            .WithMessage("*content*", "失败文案必须点出参数名，模型才能据此改参");
        _sessions.Verify(
            s => s.SendTextMessageAsync(It.IsAny<SendKfTextMsgRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private KfToolHandlers Handler() => new(_accounts.Object, _servicers.Object, _sessions.Object, _statistics.Object);

    [Fact]
    public async Task CorpStatistic_ShouldProjectDailyAggregates()
    {
        _statistics
            .Setup(s => s.GetCorpStatisticAsync(It.IsAny<GetKfCorpStatisticRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetKfCorpStatisticResponse
            {
                ErrorCode = 0,
                StatisticList =
                [
                    new KfCorpStatisticItem
                    {
                        StatTime = 1699920000,
                        Statistic = new KfCorpStatisticData
                        {
                            SessionCount = 12,
                            CustomerCount = 9,
                            CustomerMsgCount = 48,
                            AiSessionReplyCount = 5,
                            AiTransferRate = 0.4,
                            AiKnowledgeHitRate = 0.75,
                            MsgRejectedCustomerCount = 1,
                        },
                    },
                ],
            });

        var result = await Handler().GetCorpStatisticAsync(
            Args(("open_kfid", "wk-1"), ("start_time", "1699920000"), ("end_time", "1700000000")),
            CancellationToken.None);

        result.Text.Should().Contain("\"stat_time\":1699920000");
        result.Text.Should().Contain("\"session_cnt\":12");
        result.Text.Should().Contain("\"customer_cnt\":9");
        result.Text.Should().Contain("\"ai_transfer_rate\":0.4");
        result.Text.Should().Contain("\"ai_knowledge_hit_rate\":0.75");
        result.Text.Should().Contain("\"count\":1");
    }

    [Fact]
    public async Task CorpStatistic_SpanOverThirtyOneDays_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().GetCorpStatisticAsync(
            Args(("open_kfid", "wk-1"), ("start_time", "1690000000"), ("end_time", "1700000000")),
            CancellationToken.None));

        result.Text.Should().Contain("31 天");
        _statistics.Verify(
            s => s.GetCorpStatisticAsync(It.IsAny<GetKfCorpStatisticRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ServicerStatistic_ShouldProjectReplyAndSatisfactionMetrics()
    {
        _statistics
            .Setup(s => s.GetServicerStatisticAsync(It.IsAny<GetKfServicerStatisticRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetKfServicerStatisticResponse
            {
                ErrorCode = 0,
                StatisticList =
                [
                    new KfServicerStatisticItem
                    {
                        StatTime = 1699920000,
                        Statistic = new KfServicerStatisticData
                        {
                            SessionCount = 8,
                            CustomerCount = 7,
                            ReplyRate = 0.9,
                            FirstReplyAverageSec = 42.5,
                            SatisfactionInvestgateCount = 5,
                            SatisfiedRate = 0.8,
                            MiddlingRate = 0.1,
                            DissatisfiedRate = 0.1,
                        },
                    },
                ],
            });

        var result = await Handler().GetServicerStatisticAsync(
            Args(("open_kfid", "wk-1"), ("servicer_userid", "zhangsan"), ("start_time", "1699920000"), ("end_time", "1700000000")),
            CancellationToken.None);

        result.Text.Should().Contain("\"reply_rate\":0.9");
        result.Text.Should().Contain("\"first_reply_average_sec\":42.5");
        result.Text.Should().Contain("\"satisfaction_investgate_count\":5");
        result.Text.Should().Contain("\"satisfied_rate\":0.8");
        result.Text.Should().Contain("\"count\":1");
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
