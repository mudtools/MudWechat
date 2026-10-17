using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Checkin;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 打卡记录执行器：考勤事实的投影、官方约束的调用前校验，以及「行踪与设备指纹不外发」这条数据边界。
/// </summary>
public class CheckinToolHandlersTests
{
    private readonly Mock<IWechatWorkInternalCheckinRecordService> _checkin = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldProjectAttendanceFactsWithoutLocation()
    {
        _checkin
            .Setup(c => c.GetCheckinDataAsync(It.IsAny<GetCheckinDataRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCheckinDataResponse
            {
                ErrorCode = 0,
                Checkindata =
                [
                    new CheckinRecord
                    {
                        Userid = "zhangsan",
                        CheckinType = "上班打卡",
                        ExceptionType = "时间异常",
                        CheckinTime = 1700000000,
                        SchCheckinTime = 1699999200,
                        Groupname = "研发中心",
                        Groupid = 101,
                        ScheduleId = 202,
                        LocationTitle = "广州国际金融中心",
                        LocationDetail = "花城大道 1 号",
                        Lat = 23000000,
                        Lng = 113000000,
                        Wifiname = "Corp-WiFi",
                        Wifimac = "AA:BB:CC:DD:EE:FF",
                        Notes = "地铁延误",
                        Mediaids = ["media-1"],
                        Deviceid = "device-1",
                    },
                ],
            });

        var result = await Handler().ListAsync(
            Args(("userid_list", new[] { "zhangsan" }), ("start_time", "1699900000"), ("end_time", "1700100000")),
            CancellationToken.None);

        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"checkin_type\":\"上班打卡\"");
        result.Text.Should().Contain("\"exception_type\":\"时间异常\"");
        result.Text.Should().Contain("\"checkin_time\":1700000000");
        result.Text.Should().Contain("\"groupname\":\"研发中心\"");
        result.Text.Should().Contain("\"count\":1");
        result.Text.Should().NotContain("广州国际金融中心", "打卡地点属行踪信息，不外发");
        result.Text.Should().NotContain("花城大道");
        result.Text.Should().NotContain("Corp-WiFi", "WiFi 名/ MAC 是设备与位置指纹，不外发");
        result.Text.Should().NotContain("AA:BB:CC:DD:EE:FF");
        result.Text.Should().NotContain("地铁延误", "备注可能含个人说明，不外发");
        result.Text.Should().NotContain("media-1");
        result.Text.Should().NotContain("23000000", "经纬度不外发");
    }

    [Fact]
    public async Task List_WhenSpanExceedsThirtyDays_ShouldRejectBeforeCallingSdk()
    {
        var result = await Handler().ListAsync(
            Args(("userid_list", new[] { "zhangsan" }), ("start_time", "1699900000"), ("end_time", "1702600000")),
            CancellationToken.None);

        result.Text.Should().Contain("30 天");
        _checkin.Verify(
            c => c.GetCheckinDataAsync(It.IsAny<GetCheckinDataRequest>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "越界参数应在调用前拦下，而不是让官方 errcode 回话");
    }

    [Fact]
    public async Task List_WhenDataTypeOutOfRange_ShouldRejectBeforeCallingSdk()
    {
        var result = await Handler().ListAsync(
            Args(
                ("userid_list", new[] { "zhangsan" }),
                ("start_time", "1699900000"),
                ("end_time", "1700100000"),
                ("data_type", 9)),
            CancellationToken.None);

        result.Text.Should().Contain("data_type");
        _checkin.Verify(
            c => c.GetCheckinDataAsync(It.IsAny<GetCheckinDataRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task List_WhenErrcodeNonZero_ShouldReturnReadableError()
    {
        _checkin
            .Setup(c => c.GetCheckinDataAsync(It.IsAny<GetCheckinDataRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCheckinDataResponse { ErrorCode = 60011, ErrorMessage = "no privilege to access" });

        var result = await Handler().ListAsync(
            Args(("userid_list", new[] { "zhangsan" }), ("start_time", "1699900000"), ("end_time", "1700100000")),
            CancellationToken.None);

        result.Text.Should().Contain("60011");
        result.Text.Should().Contain("no privilege to access");
    }

    private CheckinToolHandlers Handler() => new(_checkin.Object);

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
