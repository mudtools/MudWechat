using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Dial;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 公费电话域执行器：通话事实的投影形状，以及「被叫电话号码不外发」这条数据边界。
/// </summary>
public class DialToolHandlersTests
{
    private readonly Mock<IWechatWorkInternalDialService> _dial = new(MockBehavior.Loose);

    [Fact]
    public async Task List_ShouldProjectCallFactsWithoutCalleePhone()
    {
        _dial
            .Setup(d => d.GetDialRecordAsync(It.IsAny<GetDialRecordRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetDialRecordResponse
            {
                ErrorCode = 0,
                Record =
                [
                    new DialRecord
                    {
                        CallTime = 1700000000,
                        TotalDuration = 95,
                        CallType = 1,
                        Caller = new DialCaller { Userid = "zhangsan", Duration = 95 },
                        Callee =
                        [
                            new DialCallee { Userid = "lisi", Phone = "13900000000", Duration = 60 },
                        ],
                    },
                ],
            });

        var result = await Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1700086400")),
            CancellationToken.None);

        result.Text.Should().Contain("\"call_time\":1700000000");
        result.Text.Should().Contain("\"total_duration\":95");
        result.Text.Should().Contain("\"userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"userid\":\"lisi\"");
        result.Text.Should().Contain("\"has_more\":false");
        result.Text.Should().NotContain("13900000000", "被叫电话号码是外部个人数据，不外发");
    }

    [Fact]
    public async Task List_WithoutTimeRange_ShouldSendNullsAndLetOfficialDefault()
    {
        GetDialRecordRequest? captured = null;
        _dial
            .Setup(d => d.GetDialRecordAsync(It.IsAny<GetDialRecordRequest>(), It.IsAny<CancellationToken>()))
            .Callback<GetDialRecordRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new GetDialRecordResponse { ErrorCode = 0, Record = [] });

        var result = await Handler().ListAsync(Args(), CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.StartTime.Should().BeNull("都不给时由官方默认查最近 30 天");
        captured.EndTime.Should().BeNull();
        captured.Limit.Should().Be(100);
        result.Text.Should().Contain("\"count\":0", "空列表是合法答案（该窗口内没有通话），不转错误");
    }

    [Fact]
    public async Task List_WithHalfTimeRange_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(Args(("start_time", "1700000000")), CancellationToken.None));

        result.Text.Should().Contain("同时给出");
        _dial.Verify(
            d => d.GetDialRecordAsync(It.IsAny<GetDialRecordRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task List_WithSpanOverThirtyDays_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await ToolInvoker.InvokeAsync(() => Handler().ListAsync(
            Args(("start_time", "1700000000"), ("end_time", "1702764800")),
            CancellationToken.None));

        result.Text.Should().Contain("30 天");
        _dial.Verify(
            d => d.GetDialRecordAsync(It.IsAny<GetDialRecordRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private DialToolHandlers Handler() => new(_dial.Object);

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
