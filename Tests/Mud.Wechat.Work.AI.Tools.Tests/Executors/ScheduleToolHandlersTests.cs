using FluentAssertions;
using Moq;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Schedule;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 日程执行器：创建（官方字符口径 + 参与者解析）、按 ID 取详情（投影 + 取消态可见）、取消（重复日程参数配对）。
/// </summary>
/// <remarks>
/// 日程是「外部可见 + 造成他人日程占用」的写操作，用例重点在<b>调用前就把非法入参挡住</b>：
/// 一旦发到服务端，参与者已经收到邀请，本地再报错也收不回来。
/// </remarks>
public class ScheduleToolHandlersTests
{
    private readonly Mock<IWechatWorkScheduleService> _schedule = new(MockBehavior.Strict);

    [Fact]
    public async Task Add_ShouldParseUnixSecondsAttendeesAndReminder()
    {
        _schedule
            .Setup(s => s.AddScheduleAsync(It.IsAny<AddScheduleRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AddScheduleResponse { ErrorCode = 0, ScheduleId = "sch-1" });

        var result = await Handler().AddAsync(
            Args(
                ("summary", "周会"),
                ("start_time", "1792308600"),
                ("end_time", "1792312200"),
                ("attendees", "zhangsan|lisi"),
                ("remind_before_secs", 900),
                ("is_whole_day", false)),
            CancellationToken.None);

        result.Text.Should().Contain("\"schedule_id\":\"sch-1\"");
        result.Text.Should().Contain("\"ok\":true");
        _schedule.Verify(
            s => s.AddScheduleAsync(
                It.Is<AddScheduleRequest>(r =>
                    r.Schedule != null
                    && r.Schedule.Summary == "周会"
                    && r.Schedule.StartTime == 1792308600
                    && r.Schedule.EndTime == 1792312200
                    && r.Schedule.IsWholeDay == 0
                    && r.Schedule.Attendees != null
                    && r.Schedule.Attendees.Count == 2
                    && r.Schedule.Attendees[1].Userid == "lisi"
                    && r.Schedule.Reminders != null
                    && r.Schedule.Reminders.RemindBeforeEventSecs == 900),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Add_WhenEndNotAfterStart_ShouldRejectBeforeCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().AddAsync(
            Args(("summary", "周会"), ("start_time", "1792312200"), ("end_time", "1792308600")),
            CancellationToken.None));

        result.Text.Should().Contain("end_time");
        _schedule.Verify(
            s => s.AddScheduleAsync(It.IsAny<AddScheduleRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Add_WhenTimesAreNotUnixSeconds_ShouldExplainFormat()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().AddAsync(
            Args(("summary", "周会"), ("start_time", "2026-10-18T10:00:00"), ("end_time", "1792312200")),
            CancellationToken.None));

        result.Text.Should().Contain("Unix");
    }

    [Fact]
    public void Add_WhenReminderNotInOfficialLadder_ShouldBeRejectedByClosedSet()
    {
        // 官方只支持固定档位；闭集在参数层就拒绝任意秒数（错误文案会列出合法档位名），执行器不必重复校验。
        var act = () => ScheduleAddArgs.Unpack(
            Args(
                ("summary", "周会"),
                ("start_time", "1792308600"),
                ("end_time", "1792312200"),
                ("remind_before_secs", 120)));

        act.Should().Throw<ToolArgumentException>();
    }

    [Fact]
    public async Task Add_WhenTitleTooLong_ShouldReject()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().AddAsync(
            Args(
                ("summary", new string('会', 129)),
                ("start_time", "1792308600"),
                ("end_time", "1792312200")),
            CancellationToken.None));

        result.Text.Should().Contain("128");
    }

    [Fact]
    public async Task Get_ShouldProjectScheduleFieldsAndExposeCancelledStatus()
    {
        _schedule
            .Setup(s => s.GetScheduleAsync(It.IsAny<GetScheduleRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetScheduleResponse
            {
                ErrorCode = 0,
                ScheduleList =
                [
                    new ScheduleDetail
                    {
                        ScheduleId = "sch-1",
                        Summary = "周会",
                        Description = "评审",
                        Location = "3F",
                        Status = 1,
                        StartTime = 1792308600,
                        EndTime = 1792312200,
                        IsWholeDay = 0,
                        CalId = "cal-1",
                        Admins = ["zhangsan"],
                        Attendees = [new ScheduleAttendee { Userid = "lisi", ResponseStatus = 1 }],
                    },
                ],
            });

        var result = await Handler().GetAsync(Args(("schedule_id", "sch-1")), CancellationToken.None);

        result.Text.Should().Contain("\"status\":1");
        result.Text.Should().Contain("\"summary\":\"周会\"");
        result.Text.Should().Contain("\"userid\":\"lisi\"");
        result.Text.Should().Contain("\"response_status\":1");
        _schedule.Verify(
            s => s.GetScheduleAsync(
                It.Is<GetScheduleRequest>(r => r.ScheduleIdList != null && r.ScheduleIdList.Count == 1 && r.ScheduleIdList[0] == "sch-1"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Cancel_WhenOpModeGivenWithoutOpStartTime_ShouldReject()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().CancelAsync(
            Args(("schedule_id", "sch-1"), ("op_mode", 1)),
            CancellationToken.None));

        result.Text.Should().Contain("op_start_time");
        _schedule.Verify(
            s => s.DelScheduleAsync(It.IsAny<DelScheduleRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Cancel_WithOpModeAndStartTime_ShouldPassBoth()
    {
        _schedule
            .Setup(s => s.DelScheduleAsync(It.IsAny<DelScheduleRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WechatWorkResponse { ErrorCode = 0 });

        var result = await Handler().CancelAsync(
            Args(("schedule_id", "sch-1"), ("op_mode", 1), ("op_start_time", "1792308600")),
            CancellationToken.None);

        result.Text.Should().Contain("\"ok\":true");
        _schedule.Verify(
            s => s.DelScheduleAsync(
                It.Is<DelScheduleRequest>(r =>
                    r.ScheduleId == "sch-1" && r.OpMode == 1 && r.OpStartTime == 1792308600),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Cancel_WhenApiFails_ShouldReturnReadableError()
    {
        _schedule
            .Setup(s => s.DelScheduleAsync(It.IsAny<DelScheduleRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WechatWorkResponse { ErrorCode = 301002, ErrorMessage = "schedule not found" });

        var result = await Handler().CancelAsync(Args(("schedule_id", "nope")), CancellationToken.None);

        result.Text.Should().Contain("301002");
    }

    private ScheduleToolHandlers Handler() => new(_schedule.Object);

    private static Dictionary<string, object?> Args(params (string Key, object? Value)[] values)
    {
        var args = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var (key, value) in values)
        {
            args[key] = value;
        }

        return args;
    }
}
