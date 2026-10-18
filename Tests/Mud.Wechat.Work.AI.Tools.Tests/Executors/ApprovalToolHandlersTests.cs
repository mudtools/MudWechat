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
    private readonly Mock<IWechatWorkApprovalTemplateService> _template = new(MockBehavior.Loose);

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
    public async Task Get_WhenInfoMissing_ShouldReturnNotFoundAsSuccess()
    {
        _approval
            .Setup(a => a.GetApprovalDetailAsync(It.IsAny<GetApprovalDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApprovalDetailResponse { ErrorCode = 0, Info = null });

        var result = await Handler().GetAsync(Args(("sp_no", "202310100001")), CancellationToken.None);

        result.Error.Should().BeNull("业务空是成功答案，不是调用失败（否则模型会重试或换工具乱试）");
        result.Text.Should().Contain("\"found\":false");
        result.Text.Should().Contain("hint");
    }

    [Fact]
    public async Task GetTemplate_ShouldProjectControlStructureWithoutConfig()
    {
        _template
            .Setup(t => t.GetTemplateDetailAsync(It.IsAny<GetApprovalTemplateDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApprovalTemplateDetailResponse
            {
                ErrorCode = 0,
                TemplateNames = [new ApprovalLangTextItem { Text = "报销申请", Lang = "zh_CN" }],
                TemplateContent = new ApprovalTemplateContent
                {
                    Controls =
                    [
                        new ApprovalTemplateControl
                        {
                            Property = new ApprovalTemplateControlProperty
                            {
                                Control = "Money",
                                Id = "money-1",
                                Title = [new ApprovalLangTextItem { Text = "报销金额" }],
                                Require = 1,
                            },
                            Config = new ApprovalTemplateControlConfig
                            {
                                Tips = new ApprovalTipsContent { TipsContent = [] },
                            },
                        },
                    ],
                },
            });

        var result = await Handler().GetTemplateAsync(Args(("template_id", "tpl-1")), CancellationToken.None);

        result.Text.Should().Contain("\"template_id\":\"tpl-1\"");
        result.Text.Should().Contain("\"报销申请\"");
        result.Text.Should().Contain("\"control\":\"Money\"");
        result.Text.Should().Contain("\"title\":\"报销金额\"");
        result.Text.Should().Contain("\"require\":1");
        result.Text.Should().Contain("\"control_count\":1");
        result.Text.Should().NotContain("元", "模板 config（选项值/单位等）不外发，只给控件结构");
    }

    [Fact]
    public async Task GetTemplate_WhenContentMissing_ShouldReturnNotFoundAsSuccess()
    {
        _template
            .Setup(t => t.GetTemplateDetailAsync(It.IsAny<GetApprovalTemplateDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApprovalTemplateDetailResponse { ErrorCode = 0, TemplateContent = null });

        var result = await Handler().GetTemplateAsync(Args(("template_id", "tpl-1")), CancellationToken.None);

        result.Error.Should().BeNull();
        result.Text.Should().Contain("\"found\":false");
    }

    [Fact]
    public async Task List_ShouldPassFiltersAndProjectSpNoOnly()
    {
        _approval
            .Setup(a => a.BatchGetApprovalNumbersAsync(It.IsAny<BatchGetApprovalNumbersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BatchGetApprovalNumbersResponse
            {
                ErrorCode = 0,
                SpNoList = ["202310100001", "202310100002"],
                NewNextCursor = "c2",
            });

        var result = await Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1700086400"), ("sp_status", 1)),
            CancellationToken.None);

        result.Text.Should().Contain("\"count\":2");
        result.Text.Should().Contain("\"sp_no_list\":[\"202310100001\",\"202310100002\"]");
        result.Text.Should().Contain("\"next_cursor\":\"c2\"");
        _approval.Verify(
            a => a.BatchGetApprovalNumbersAsync(
                It.Is<BatchGetApprovalNumbersRequest>(r =>
                    r.Starttime == "1700000000"
                    && r.Endtime == "1700086400"
                    && r.Size == 100
                    && r.Filters != null
                    && r.Filters.Count == 1
                    && r.Filters[0].Key == "sp_status"
                    && r.Filters[0].Value == "1"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task List_WhenSpanExceedsThirtyOneDays_ShouldRejectBeforeCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1703000000")),
            CancellationToken.None));

        result.Text.Should().Contain("31 天");
        _approval.Verify(
            a => a.BatchGetApprovalNumbersAsync(It.IsAny<BatchGetApprovalNumbersRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task List_WhenEndNotAfterStart_ShouldReject()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(
            Args(("start_time", "1700086400"), ("end_time", "1700000000")),
            CancellationToken.None));

        result.Text.Should().Contain("end_time");
    }

    [Fact]
    public async Task List_WhenTimesAreNotUnixSeconds_ShouldExplainFormat()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(
            Args(("start_time", "2026-10-18"), ("end_time", "2026-10-19")),
            CancellationToken.None));

        result.Text.Should().Contain("Unix");
    }

    [Fact]
    public async Task List_WhenEmpty_ShouldReturnHintWithoutError()
    {
        _approval
            .Setup(a => a.BatchGetApprovalNumbersAsync(It.IsAny<BatchGetApprovalNumbersRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BatchGetApprovalNumbersResponse { ErrorCode = 0, SpNoList = [] });

        var result = await Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1700086400")),
            CancellationToken.None);

        result.Text.Should().Contain("\"count\":0");
        result.Text.Should().Contain("hint");
        result.Text.Should().NotContain("errcode", "空页是合法答案，转错误会让模型改参数重试");
    }

    [Fact]
    public async Task Apply_ShouldMapValuesByTemplateControlType()
    {
        SetupTemplate(
            ("Text-1", "Text", 1),
            ("Number-2", "Number", 1),
            ("Date-3", "Date", 0),
            ("Selector-4", "Selector", 0),
            ("Members-5", "Members", 0));
        _approval
            .Setup(a => a.ApplyApprovalAsync(It.IsAny<ApplyApprovalRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApplyApprovalResponse { ErrorCode = 0, SpNo = "202310100001" });

        var result = await Handler().ApplyAsync(
            Args(
                ("template_id", "tpl-1"),
                ("creator_userid", "zhangsan"),
                ("fields_json", """{"Text-1":"张三","Number-2":"3","Date-3":"1792308600","Selector-4":["o1","o2"],"Members-5":["lisi"]}"""),
                ("summary", "请假申请|3 天")),
            CancellationToken.None);

        result.Text.Should().Contain("\"sp_no\":\"202310100001\"");
        result.Text.Should().Contain("\"filled_controls\":5");
        _approval.Verify(
            a => a.ApplyApprovalAsync(
                It.Is<ApplyApprovalRequest>(r =>
                    r.CreatorUserid == "zhangsan"
                    && r.TemplateId == "tpl-1"
                    && r.UseTemplateApprover == 1
                    && r.ApplyData != null
                    && r.ApplyData.Contents != null
                    && r.ApplyData.Contents.Count == 5
                    && r.SummaryList != null
                    && r.SummaryList.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Apply_WithUnknownControlId_ShouldRejectAndListAvailableIds()
    {
        SetupTemplate(("Text-1", "Text", 0));

        var result = await ToolInvoker.InvokeAsync(() => Handler().ApplyAsync(
            Args(("template_id", "tpl-1"), ("creator_userid", "zhangsan"), ("fields_json", """{"Nope-9":"x"}""")),
            CancellationToken.None));

        result.Text.Should().Contain("Nope-9");
        result.Text.Should().Contain("Text-1");
        _approval.Verify(
            a => a.ApplyApprovalAsync(It.IsAny<ApplyApprovalRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Apply_WhenRequiredControlMissing_ShouldReject()
    {
        SetupTemplate(("Text-1", "Text", 1), ("Money-2", "Money", 1));

        var result = await ToolInvoker.InvokeAsync(() => Handler().ApplyAsync(
            Args(("template_id", "tpl-1"), ("creator_userid", "zhangsan"), ("fields_json", """{"Text-1":"张三"}""")),
            CancellationToken.None));

        result.Text.Should().Contain("必填");
        result.Text.Should().Contain("Money-2");
        _approval.Verify(
            a => a.ApplyApprovalAsync(It.IsAny<ApplyApprovalRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Apply_WithUnsupportedControlType_ShouldRejectInsteadOfDroppingField()
    {
        SetupTemplate(("File-1", "File", 0));

        var result = await ToolInvoker.InvokeAsync(() => Handler().ApplyAsync(
            Args(("template_id", "tpl-1"), ("creator_userid", "zhangsan"), ("fields_json", """{"File-1":["f_1"]}""")),
            CancellationToken.None));

        result.Text.Should().Contain("暂不支持");
        _approval.Verify(
            a => a.ApplyApprovalAsync(It.IsAny<ApplyApprovalRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Apply_WithMalformedJson_ShouldExplainFormat()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ApplyAsync(
            Args(("template_id", "tpl-1"), ("creator_userid", "zhangsan"), ("fields_json", "not json")),
            CancellationToken.None));

        result.Text.Should().Contain("fields_json");
        _template.Verify(
            t => t.GetTemplateDetailAsync(It.IsAny<GetApprovalTemplateDetailRequest>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "JSON 不合法时不必先去取模板");
    }

    private void SetupTemplate(params (string Id, string Control, int Require)[] controls)
    {
        var contentControls = controls
            .Select(c => new ApprovalTemplateControl
            {
                Property = new ApprovalTemplateControlProperty
                {
                    Id = c.Id,
                    Control = c.Control,
                    Require = c.Require,
                    Title = [new ApprovalLangTextItem { Text = c.Id }],
                },
            })
            .ToList();

        _template
            .Setup(t => t.GetTemplateDetailAsync(It.IsAny<GetApprovalTemplateDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetApprovalTemplateDetailResponse
            {
                ErrorCode = 0,
                TemplateContent = new ApprovalTemplateContent { Controls = contentControls },
            });
    }

    private ApprovalToolHandlers Handler() => new(_approval.Object, _template.Object);

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
