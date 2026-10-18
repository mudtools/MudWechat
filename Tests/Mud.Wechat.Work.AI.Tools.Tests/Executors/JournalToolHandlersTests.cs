using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.DataModels.Journal;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 汇报域执行器：官方跨度约束、两段式读面，以及「表单只取文本类取值」这条边界。
/// </summary>
public class JournalToolHandlersTests
{
    private readonly Mock<IWechatWorkInternalJournalService> _journal = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldReturnUuidsAndCursor()
    {
        _journal
            .Setup(s => s.GetJournalRecordListAsync(It.IsAny<JournalRecordListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JournalRecordListResponse
            {
                ErrorCode = 0,
                JournalIdList = ["j-1", "j-2"],
                Ending = 0,
                NextCursor = 2,
            });

        var result = await Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1700100000")),
            CancellationToken.None);

        result.Text.Should().Contain("\"j-1\"");
        result.Text.Should().Contain("\"count\":2");
        result.Text.Should().Contain("\"ending\":0");
        result.Text.Should().Contain("\"next_cursor\":2");
    }

    [Fact]
    public async Task List_WhenSpanExceeds31Days_ShouldRejectBeforeCallingSdk()
    {
        var result = await Handler().ListAsync(
            Args(("start_time", "1690000000"), ("end_time", "1700000000")),
            CancellationToken.None);

        result.Text.Should().Contain("31 天");
        _journal.Verify(
            s => s.GetJournalRecordListAsync(It.IsAny<JournalRecordListRequest>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "越界参数应在调用前拦下");
    }

    [Fact]
    public async Task Get_ShouldProjectTextFieldsWithoutMembersOrComments()
    {
        _journal
            .Setup(s => s.GetJournalRecordDetailAsync(It.IsAny<JournalRecordDetailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new JournalRecordDetailResponse
            {
                ErrorCode = 0,
                Info = new JournalRecordInfo
                {
                    JournalId = "j-1",
                    TemplateId = "tpl-1",
                    TemplateName = "周报",
                    ReportTimestamp = 1700000000,
                    Submitter = new ApprovalUserRef { Userid = "zhangsan" },
                    SysJournalData = "<p>本周完成三件事</p>",
                    ReceiverList = [new ApprovalUserRef { Userid = "lisi" }],
                    ReadedReceiverList = [],
                    CommentList =
                    [
                        new JournalComment { CommentId = 1, Content = "收到，注意休息", CommentTimestamp = 1700000100 },
                    ],
                    ApplyData = new JournalApplyData
                    {
                        ContentList =
                        [
                            new JournalApplyContent
                            {
                                Control = "Textarea",
                                ControlId = "c-1",
                                Title = [new ApprovalLangTextItem { Text = "本周工作", Lang = "zh_CN" }],
                                Value = new JournalControlValue { Text = "完成审批工具策展" },
                            },
                            new JournalApplyContent
                            {
                                Control = "Number",
                                ControlId = "c-2",
                                Title = [new ApprovalLangTextItem { Text = "工时", Lang = "zh_CN" }],
                                Value = new JournalControlValue { NewNumber = "40" },
                            },
                            new JournalApplyContent
                            {
                                Control = "Contact",
                                ControlId = "c-3",
                                Title = [new ApprovalLangTextItem { Text = "协作人", Lang = "zh_CN" }],
                                Value = new JournalControlValue
                                {
                                    Members = [new ApprovalMemberItem { Userid = "wangwu", Name = "王五" }],
                                },
                            },
                        ],
                    },
                },
            });

        var result = await Handler().GetAsync(Args(("journaluuid", "j-1")), CancellationToken.None);

        result.Text.Should().Contain("\"journal_uuid\":\"j-1\"");
        result.Text.Should().Contain("\"template_name\":\"周报\"");
        result.Text.Should().Contain("\"submitter\":\"zhangsan\"");
        result.Text.Should().Contain("完成审批工具策展");
        result.Text.Should().Contain("\"number\":\"40\"");
        result.Text.Should().Contain("本周完成三件事");
        result.Text.Should().Contain("\"comment_count\":1");
        result.Text.Should().NotContain("收到，注意休息", "评论正文不外发（只给条数）");
        result.Text.Should().NotContain("wangwu", "成员控件不外发（个人数据）");
        result.Text.Should().NotContain("王五");
    }

    private JournalToolHandlers Handler() => new(_journal.Object);

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
