using FluentAssertions;
using Moq;
using Mud.Wechat.Work.AI.Tools.Executors;
using Mud.Wechat.Work.DataModels.ExternalContact.ResignedInheritance;
using Mud.Wechat.Work.Interfaces;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.Executors;

/// <summary>
/// 客户继承域执行器：待分配清单的投影形状与分页边界。
/// </summary>
public class ExternalContactInheritanceToolHandlersTests
{
    private readonly Mock<IWechatWorkExternalContactResignedInheritanceService> _resigned = new(MockBehavior.Loose);

    [Fact]
    public async Task UnassignedList_ShouldProjectHandoverFactsAndIsLast()
    {
        _resigned
            .Setup(s => s.GetUnassignedListAsync(It.IsAny<GetUnassignedListRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetUnassignedListResponse
            {
                ErrorCode = 0,
                Info =
                [
                    new UnassignedCustomerItem
                    {
                        HandoverUserid = "zhangsan",
                        ExternalUserid = "wm-1",
                        DimissionTime = 1699000000,
                    },
                ],
                IsLast = false,
                NextCursor = "c2",
            });

        var result = await Handler().ListUnassignedAsync(Args(), CancellationToken.None);

        result.Text.Should().Contain("\"handover_userid\":\"zhangsan\"");
        result.Text.Should().Contain("\"external_userid\":\"wm-1\"");
        result.Text.Should().Contain("\"dimission_time\":1699000000");
        result.Text.Should().Contain("\"is_last\":false");
        result.Text.Should().Contain("\"next_cursor\":\"c2\"");
    }

    [Fact]
    public async Task UnassignedList_PageSizeOverOfficialMax_ShouldReturnErrorWithoutCallingSdk()
    {
        var result = await Handler().ListUnassignedAsync(Args(("page_size", 5000)), CancellationToken.None);

        result.Text.Should().Contain("page_size");
        _resigned.Verify(
            s => s.GetUnassignedListAsync(It.IsAny<GetUnassignedListRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private ExternalContactInheritanceToolHandlers Handler() => new(_resigned.Object);

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
