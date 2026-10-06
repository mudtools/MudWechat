// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Configuration;
using Mud.Wechat.OfficialAccount.DataModels.WebDev;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.Authentication;

/// <summary>
/// 票据管理器用例：缓存命中（严禁频繁刷新）、两类票据隔离、40001 自愈、有效期兜底、上下文解析。
/// </summary>
public class MpTicketManagerTests
{
    private const string AppKey = "mp-ticket";

    private static MpAppConfig CreateConfig(bool allowCustomBaseUrl = true)
        => new()
        {
            AppKey = AppKey,
            AppId = "wx-ticket",
            AppSecret = "secret",
            AllowCustomBaseUrl = allowCustomBaseUrl,
        };

    private static IMpAccessTokenManager CreateTokenManager(string token = "access-token-1")
    {
        var mock = new Mock<IMpAccessTokenManager>();
        mock.Setup(m => m.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync(token);
        return mock.Object;
    }

    private static Mock<IMpTicketService> CreateTicketService(string ticket = "ticket-1", int expiresIn = 7200)
    {
        var mock = new Mock<IMpTicketService>();
        mock.Setup(s => s.GetTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTicketResponse { Ticket = ticket, ExpiresIn = expiresIn });
        return mock;
    }

    private static MpJsApiTicketManager CreateJsApiManager(
        IMpTicketService service,
        IMpAccessTokenManager? tokenManager = null,
        MpAppConfig? config = null)
        => new(service,
               tokenManager ?? CreateTokenManager(),
               Options.Create(config ?? CreateConfig()),
               NullLogger<MpJsApiTicketManager>.Instance);

    /// <summary>T1：票据获取成功，且请求带上本应用的 access_token 与官方 <c>type</c>。</summary>
    [Fact]
    public async Task GetTicketAsync_ShouldPassAccessTokenAndTicketType()
    {
        var service = CreateTicketService();
        var manager = CreateJsApiManager(service.Object);

        var ticket = await manager.GetTicketAsync();

        ticket.Should().Be("ticket-1");
        manager.TicketType.Should().Be(MpTicketTypes.JsApi);
        service.Verify(s => s.GetTicketAsync("access-token-1", MpTicketTypes.JsApi, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// T2（**核心业务约束**）：重复取票据必须命中缓存，不得反复调用官方端点
    /// （官方：「api 调用次数非常有限，频繁刷新会导致 api 调用受限」）。
    /// </summary>
    [Fact]
    public async Task GetTicketAsync_ShouldHitCache_AndNotRefreshRepeatedly()
    {
        var service = CreateTicketService();
        var manager = CreateJsApiManager(service.Object);

        for (var i = 0; i < 20; i++)
        {
            await manager.GetTicketAsync();
        }

        service.Verify(s => s.GetTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once, "20 次取用只允许 1 次官方调用（缓存命中）");
    }

    /// <summary>T3：并发取用走单一飞行（组件键控锁），仍只触发一次官方调用。</summary>
    [Fact]
    public async Task GetTicketAsync_ConcurrentCalls_ShouldSingleFlight()
    {
        var service = CreateTicketService();
        var manager = CreateJsApiManager(service.Object);

        var tickets = await Task.WhenAll(Enumerable.Range(0, 16).Select(_ => manager.GetTicketAsync()));

        tickets.Should().OnlyContain(t => t == "ticket-1");
        service.Verify(s => s.GetTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once, "并发刷新必须收敛为单飞，避免触发官方频次限制");
    }

    /// <summary>T4：两类票据互相隔离——卡券票据不得复用 JS-SDK 票据的缓存条目。</summary>
    [Fact]
    public async Task WxCardTicket_ShouldBeIsolatedFromJsApiTicket()
    {
        var service = new Mock<IMpTicketService>();
        service.Setup(s => s.GetTicketAsync(It.IsAny<string>(), MpTicketTypes.JsApi, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTicketResponse { Ticket = "jsapi-ticket", ExpiresIn = 7200 });
        service.Setup(s => s.GetTicketAsync(It.IsAny<string>(), MpTicketTypes.WxCard, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTicketResponse { Ticket = "wxcard-ticket", ExpiresIn = 7200 });

        var tokenManager = CreateTokenManager();
        var config = Options.Create(CreateConfig());

        var jsApi = new MpJsApiTicketManager(service.Object, tokenManager, config, NullLogger<MpJsApiTicketManager>.Instance);
        var wxCard = new MpWxCardTicketManager(service.Object, tokenManager, config, NullLogger<MpWxCardTicketManager>.Instance);

        (await jsApi.GetTicketAsync()).Should().Be("jsapi-ticket");
        (await wxCard.GetTicketAsync()).Should().Be("wxcard-ticket");
        wxCard.TicketType.Should().Be(MpTicketTypes.WxCard);
    }

    /// <summary>
    /// T5：<c>40001</c> 自愈——失效本应用令牌并重试一次（票据端点不带 <c>[Token]</c>，
    /// 无框架恢复链路，故由管理器显式处置）。
    /// </summary>
    [Fact]
    public async Task GetTicketAsync_On40001_ShouldRetryOnceAfterInvalidatingToken()
    {
        var service = new Mock<IMpTicketService>();
        var callCount = 0;
        service.Setup(s => s.GetTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return callCount == 1
                    ? new MpGetTicketResponse { ErrorCode = MpErrorCodes.InvalidCredential, ErrorMessage = "invalid credential" }
                    : new MpGetTicketResponse { Ticket = "ticket-after-retry", ExpiresIn = 7200 };
            });

        var manager = CreateJsApiManager(service.Object);

        var ticket = await manager.GetTicketAsync();

        ticket.Should().Be("ticket-after-retry");
        callCount.Should().Be(2, "40001 后必须重试恰好一次");
    }

    /// <summary>T6：重试后仍失败则按业务异常上抛（不做无限重试）。</summary>
    [Fact]
    public async Task GetTicketAsync_OnPersistent40001_ShouldThrow()
    {
        var service = new Mock<IMpTicketService>();
        service.Setup(s => s.GetTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTicketResponse { ErrorCode = MpErrorCodes.InvalidCredential, ErrorMessage = "invalid credential" });

        var manager = CreateJsApiManager(service.Object);

        await manager.Invoking(m => m.GetTicketAsync())
            .Should().ThrowAsync<MpException>("重试仍失败说明 AppSecret 配置有误，重试无意义");
    }

    /// <summary>T7：<c>expires_in</c> 非正值时用官方 7200 秒兜底（否则票据会被判为立即过期并疯狂刷新）。</summary>
    [Fact]
    public async Task GetTicketAsync_ShouldFallbackToOfficialExpireSeconds()
    {
        var service = CreateTicketService(ticket: "ticket-fallback", expiresIn: 0);
        var manager = CreateJsApiManager(service.Object);

        await manager.GetTicketAsync();
        await manager.GetTicketAsync();

        service.Verify(s => s.GetTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once, "兜底有效期内不得再次刷新");
    }

    /// <summary>T8：票据带空值时必须失败（不得把空票据写进缓存）。</summary>
    [Fact]
    public async Task GetTicketAsync_OnEmptyTicket_ShouldThrow()
    {
        var service = new Mock<IMpTicketService>();
        service.Setup(s => s.GetTicketAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTicketResponse { Ticket = string.Empty, ExpiresIn = 7200 });

        var manager = CreateJsApiManager(service.Object);

        await manager.Invoking(m => m.GetTicketAsync()).Should().ThrowAsync<InvalidOperationException>();
    }

    /// <summary>T9：应用上下文可解析两个票据管理器，且各自独立。</summary>
    [Fact]
    public void AppContext_ShouldResolveTicketManagers()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMpApp(config =>
        {
            config.AppKey = AppKey;
            config.AppId = "wx-ticket";
            config.AppSecret = "secret";
        });
        services.AddMpServices(builder => builder.AddBasicApi());

        using var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<IMpAppContext>();

        var jsApi = context.GetService<IMpJsApiTicketManager>();
        var wxCard = context.GetService<IMpWxCardTicketManager>();
        var byBaseContract = context.GetService<IMpTicketManager>();

        jsApi.Should().NotBeNull();
        wxCard.Should().NotBeNull();
        jsApi.Should().NotBeSameAs(wxCard);
        jsApi!.TicketType.Should().Be(MpTicketTypes.JsApi);
        wxCard!.TicketType.Should().Be(MpTicketTypes.WxCard);
        byBaseContract.Should().BeSameAs(jsApi, "共同契约指向 JS-SDK 票据管理器（最常用形态）");
    }
}
