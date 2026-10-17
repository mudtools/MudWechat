using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 审批域执行器：状态与流转事实的投影，以及「不外发表单取值与意见正文」这条数据边界。
/// </summary>
public class ApprovalToolHandlersTests
{
    private readonly Mock<IWechatWorkApprovalService> _approval = new(MockBehavior.Loose);

    [Fact]
    public async Task Get_ShouldProjectStatusAndFlowWithoutFormValues()
    {
        _approval
            .Setup(a => a.GetApprovalDetailAsync(It.IsAny<GetApprovalDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApprovalDetailResponse
            {
                ErrorCode = 0,
                Info = new ApprovalDetailInfo
                {
                    SpNo = "202310100001",
                    SpName = "报销",
                    SpStatus = 1,
                    TemplateId = "tpl-1",
                    ApplyTime = 1700000000,
                    Applyer = new ApprovalApplyer { Userid = "zhangsan", Partyid = "2" },
                    SpRecord =
                    [
                        new ApprovalSpRecord
                        {
                            SpStatus = 2,
                            Approverattr = 1,
                            Details =
                            [
                                new ApprovalSpRecordDetail
                                {
                                    Approver = new ApprovalUserRef { Userid = "lisi" },
                                    SpStatus = 2,
                                    Sptime = 1700000100,
                                    Speech = "同意报销",
                                },
                            ],
                        },
                    ],
                    Notifyer = [new ApprovalUserRef { Userid = "wangwu" }],
                    Comments =
                    [
                        new ApprovalComment
                        {
                            CommentUserInfo = new ApprovalUserRef { Userid = "zhaoliu" },
                            Commenttime = 1700000200,
                            Commentcontent = "请尽快处理",
                        },
                    ],
                    ApplyData = new ApprovalApplyData
                    {
                        Contents =
                        [
                            new ApprovalControlItem
                            {
                                Control = "Money",
                                Title = [new ApprovalLangTextItem { Text = "报销金额", Lang = "zh_CN" }],
                                Value = new ApprovalControlValue { NewMoney = "1280.00" },
                            },
                            new ApprovalControlItem
                            {
                                Control = "Text",
                                Title = [new ApprovalLangTextItem { Text = "事由", Lang = "zh_CN" }],
                                Value = new ApprovalControlValue { Text = "客户拜访" },
                            },
                            new ApprovalControlItem
                            {
                                Control = "Text",
                                Hidden = 1,
                                Title = [new ApprovalLangTextItem { Text = "隐藏字段", Lang = "zh_CN" }],
                            },
                        ],
                    },
                },
            });

        var result = await Handler().GetAsync(Args(("sp_no", "202310100001")), CancellationToken.None);

        result.Text.Should().Contain("\"sp_no\":\"202310100001\"");
        result.Text.Should().Contain("\"sp_name\":\"报销\"");
        result.Text.Should().Contain("\"sp_status\":1");
        result.Text.Should().Contain("\"userid\":\"lisi\"");
        result.Text.Should().Contain("\"sptime\":1700000100");
        result.Text.Should().Contain("\"comment_count\":1");
        result.Text.Should().Contain("报销金额", "字段名是结构线索，可以外发");
        result.Text.Should().NotContain("1280.00", "表单控件取值不外发（个人数据进入提示词即不可撤回）");
        result.Text.Should().NotContain("客户拜访");
        result.Text.Should().NotContain("同意报销", "审批意见正文不外发");
        result.Text.Should().NotContain("请尽快处理", "备注正文不外发（只给条数）");
        result.Text.Should().NotContain("隐藏字段", "隐藏控件不投影");
    }

    [Fact]
    public async Task Get_WhenErrcodeNonZero_ShouldReturnReadableError()
    {
        _approval
            .Setup(a => a.GetApprovalDetailAsync(It.IsAny<GetApprovalDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApprovalDetailResponse { ErrorCode = 301025, ErrorMessage = "invalid sp_no" });

        var result = await Handler().GetAsync(Args(("sp_no", "nope")), CancellationToken.None);

        result.Text.Should().Contain("301025");
        result.Text.Should().Contain("invalid sp_no");
    }

    [Fact]
    public async Task Get_WhenInfoMissing_ShouldReturnReadableError()
    {
        _approval
            .Setup(a => a.GetApprovalDetailAsync(It.IsAny<GetApprovalDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApprovalDetailResponse { ErrorCode = 0, Info = null });

        var result = await Handler().GetAsync(Args(("sp_no", "202310100001")), CancellationToken.None);

        result.Text.Should().Contain("info", "空载荷要给出可读原因，而不是一张空 JSON");
    }

    private ApprovalToolHandlers Handler() => new(_approval.Object);

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
