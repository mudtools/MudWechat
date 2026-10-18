using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.Approval;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 假期余额执行器：三个读数（秒 / 天 / 小时）同时给出，避免模型自己做除法。
/// </summary>
/// <remarks>
/// 官方单位恒为秒，而用户只会问「几天」。让模型换算意味着每次都可能差 24 倍；
/// 执行器直接给 <c>left_days</c> / <c>left_hours</c>，模型只需按问法选一个。
/// </remarks>
public class VacationToolHandlersTests
{
    private readonly Mock<IWechatWorkVacationService> _vacation = new(MockBehavior.Strict);

    [Fact]
    public async Task GetQuota_ShouldExposeSecondsDaysAndHours()
    {
        _vacation
            .Setup(v => v.GetUserVacationQuotaAsync(It.IsAny<GetVacationUserQuotaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetVacationUserQuotaResponse
            {
                ErrorCode = 0,
                Lists =
                [
                    new VacationUserQuotaItem
                    {
                        Id = 1,
                        Vacationname = "年假",
                        Assignduration = 864000,
                        Usedduration = 172800,
                        Leftduration = 691200,
                    },
                ],
            });

        var result = await Handler().GetQuotaAsync(Args(("userid", "zhangsan")), CancellationToken.None);

        result.Text.Should().Contain("\"name\":\"年假\"");
        result.Text.Should().Contain("\"left_seconds\":691200");
        result.Text.Should().Contain("\"left_days\":8", "691200 秒 = 8 天");
        result.Text.Should().Contain("\"left_hours\":192");
        _vacation.Verify(
            v => v.GetUserVacationQuotaAsync(
                It.Is<GetVacationUserQuotaRequest>(r => r.Userid == "zhangsan"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetQuota_WhenEmpty_ShouldExplainWhyInsteadOfReturningSilence()
    {
        _vacation
            .Setup(v => v.GetUserVacationQuotaAsync(It.IsAny<GetVacationUserQuotaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetVacationUserQuotaResponse { ErrorCode = 0, Lists = [] });

        var result = await Handler().GetQuotaAsync(Args(("userid", "zhangsan")), CancellationToken.None);

        result.Text.Should().Contain("\"count\":0");
        result.Text.Should().Contain("假期管理", "空结果必须给出原因，否则模型会重复调用");
    }

    [Fact]
    public async Task GetQuota_WhenApiFails_ShouldReturnReadableError()
    {
        _vacation
            .Setup(v => v.GetUserVacationQuotaAsync(It.IsAny<GetVacationUserQuotaRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetVacationUserQuotaResponse { ErrorCode = 60011, ErrorMessage = "no privilege" });

        var result = await Handler().GetQuotaAsync(Args(("userid", "zhangsan")), CancellationToken.None);

        result.Text.Should().Contain("60011");
    }

    private VacationToolHandlers Handler() => new(_vacation.Object);

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
