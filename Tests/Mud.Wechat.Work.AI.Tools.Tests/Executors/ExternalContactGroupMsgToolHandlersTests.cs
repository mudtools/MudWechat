using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.ExternalContact.GroupMsg;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 客户群发域执行器：三段式读面的投影形状、目标集显式化与字节上限的调用前收口，
/// 以及「附件只给 msgtype、media_id 不外发」这条边界。
/// </summary>
public class ExternalContactGroupMsgToolHandlersTests
{
    private readonly Mock<IWechatWorkExternalContactGroupMsgService> _groupMsg = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldProjectTextAndAttachmentTypesWithoutMediaId()
    {
        _groupMsg
            .Setup(s => s.GetGroupMsgListAsync(It.IsAny<GetGroupMsgListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupMsgListResponse
            {
                ErrorCode = 0,
                GroupMsgList =
                [
                    new GroupMsgRecord
                    {
                        MsgId = "gm-1",
                        Creator = "zhangsan",
                        CreateTime = 1700000000,
                        CreateType = 0,
                        Text = new GroupMsgTextContent { Content = "本周新品" },
                        Attachments =
                        [
                            new GroupMsgAttachment
                            {
                                MsgType = "image",
                                Image = new GroupMsgImageAttachment { MediaId = "media-1" },
                            },
                            new GroupMsgAttachment { MsgType = "link" },
                        ],
                    },
                ],
                NextCursor = "c2",
            });

        var result = await Handler().ListAsync(
            Args(("chat_type", "single"), ("start_time", "1700000000"), ("end_time", "1700086400")),
            CancellationToken.None);

        result.Text.Should().Contain("\"msgid\":\"gm-1\"");
        result.Text.Should().Contain("\"text\":\"本周新品\"");
        result.Text.Should().Contain("\"image\"");
        result.Text.Should().Contain("\"link\"");
        result.Text.Should().Contain("\"count\":1");
        result.Text.Should().Contain("\"page_token\":\"c2\"");
        result.Text.Should().NotContain("media-1", "media_id 是宿主素材资产，不外发");
    }

    [Fact]
    public async Task List_ChatTypeOutsideClosedSet_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(
            Args(("chat_type", "all"), ("start_time", "1700000000"), ("end_time", "1700086400")),
            CancellationToken.None));

        result.Text.Should().Contain("chat_type");
        VerifyNoCall();
    }

    [Fact]
    public async Task List_SpanOverOneMonth_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(
            Args(("chat_type", "group"), ("start_time", "1700000000"), ("end_time", "1702764800")),
            CancellationToken.None));

        result.Text.Should().Contain("1 个月");
        VerifyNoCall();
    }

    [Fact]
    public async Task TaskList_ShouldProjectMemberStatusAndSendTime()
    {
        _groupMsg
            .Setup(s => s.GetGroupMsgTaskListAsync(It.IsAny<GetGroupMsgTaskListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupMsgTaskListResponse
            {
                ErrorCode = 0,
                TaskList =
                [
                    new GroupMsgTaskItem { Userid = "zhangsan", Status = 1, SendTime = 1700000900 },
                    new GroupMsgTaskItem { Userid = "lisi", Status = 0, SendTime = null },
                ],
            });

        var result = await Handler().ListTasksAsync(Args(("msgid", "gm-1")), CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"status\":1");
        result.Text.Should().Contain("\"send_time\":1700000900");
        result.Text.Should().Contain("\"count\":2");
    }

    [Fact]
    public async Task SendResult_ShouldProjectCustomerAndChatDelivery()
    {
        _groupMsg
            .Setup(s => s.GetGroupMsgSendResultAsync(It.IsAny<GetGroupMsgSendResultRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetGroupMsgSendResultResponse
            {
                ErrorCode = 0,
                SendList =
                [
                    new GroupMsgSendResultItem
                    {
                        Userid = "zhangsan",
                        ExternalUserid = "wm-1",
                        Status = 1,
                        SendTime = 1700000900,
                    },
                    new GroupMsgSendResultItem
                    {
                        Userid = "zhangsan",
                        ChatId = "wr-1",
                        Status = 0,
                        SendTime = null,
                    },
                ],
            });

        var result = await Handler().ListSendResultsAsync(
            Args(("msgid", "gm-1"), ("userid", "zhangsan")),
            CancellationToken.None);

        result.Text.Should().Contain("\"external_userid\":\"wm-1\"");
        result.Text.Should().Contain("\"chat_id\":\"wr-1\"");
        result.Text.Should().Contain("\"status\":0");
        result.Text.Should().Contain("\"count\":2");
    }

    [Fact]
    public async Task Send_GroupModeWithoutChatIdList_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().SendAsync(
            Args(("chat_type", "group"), ("sender", "zhangsan"), ("content", "本周新品")),
            CancellationToken.None));

        result.Text.Should().Contain("chat_id_list");
        _groupMsg.Verify(
            s => s.AddGroupMsgTemplateAsync(It.IsAny<AddGroupMsgTemplateRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Send_SingleMode_ShouldBuildCustomerTargetsAndProjectMsgid()
    {
        AddGroupMsgTemplateRequest? captured = null;
        _groupMsg
            .Setup(s => s.AddGroupMsgTemplateAsync(It.IsAny<AddGroupMsgTemplateRequest>(), It.IsAny<CancellationToken>()))
            .Callback<AddGroupMsgTemplateRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new AddGroupMsgTemplateResponse
            {
                ErrorCode = 0,
                MsgId = "gm-9",
                FailList = ["wm-bad"],
            });

        var result = await Handler().SendAsync(
            Args(
                ("chat_type", "single"),
                ("sender", "zhangsan"),
                ("content", "本周新品"),
                ("external_userid", new[] { "wm-1", "wm-2" })),
            CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.ChatType.Should().Be("single");
        captured.Sender.Should().Be("zhangsan");
        captured.ExternalUserid.Should().BeEquivalentTo(new[] { "wm-1", "wm-2" });
        captured.ChatIdList.Should().BeNull();
        captured.Text!.Content.Should().Be("本周新品");
        result.Text.Should().Contain("\"msgid\":\"gm-9\"");
        result.Text.Should().Contain("\"fail_count\":1", "官方即使部分失败也可能返回 errcode 0，fail_list 必须显式给出");
    }

    [Fact]
    public async Task Send_ContentOverOfficialByteLimit_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().SendAsync(
            Args(
                ("chat_type", "single"),
                ("sender", "zhangsan"),
                ("content", new string('新', 2000)),
                ("external_userid", new[] { "wm-1" })),
            CancellationToken.None));

        result.Text.Should().Contain("4000 字节");
        _groupMsg.Verify(
            s => s.AddGroupMsgTemplateAsync(It.IsAny<AddGroupMsgTemplateRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private ExternalContactGroupMsgToolHandlers Handler() => new(_groupMsg.Object);

    private void VerifyNoCall()
        => _groupMsg.Verify(
            s => s.GetGroupMsgListAsync(It.IsAny<GetGroupMsgListRequest>(), It.IsAny<CancellationToken>()),
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
