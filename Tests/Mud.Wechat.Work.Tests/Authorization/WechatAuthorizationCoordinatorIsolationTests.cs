// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.Services.Authorization;

namespace Mud.Wechat.Work.Tests.Authorization;

/// <summary>
/// 授权协调器逐应用异常隔离测试（P3-1/F9）：change_auth / cancel_auth / 换码三个逐 appKey 循环中
/// 单应用失败必须记 Error 后继续其余应用，不中断整批、不改变 SuiteId 命中集清理范围（G9 不变）。
/// </summary>
public class WechatAuthorizationCoordinatorIsolationTests
{
    private const string SuiteId = "ww-suite-id";
    private const string AppKey1 = "suite-app-1";
    private const string AppKey2 = "suite-app-2";
    private const string AuthCorpId = "ww-auth-corp";

    private static WechatAppConfig SuiteConfig(string appKey) => new()
    {
        AppKey = appKey,
        AppType = WechatAppType.ThirdParty,
        CorpId = "ww-provider",
        ProviderSecret = "provider-secret",
        // 两个应用归属同一 SuiteId：同一套件的授权事件需逐应用处理（ResolveAppKeys 多命中形态）。
        SuiteId = SuiteId,
        SuiteSecret = "suite-secret",
    };

    private static (ServiceProvider Provider, Mock<IWechatWorkAuthorizationService> AuthorizationService, IWechatAuthorizationCoordinator Coordinator)
        CreateHost()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { SuiteConfig(AppKey1), SuiteConfig(AppKey2) });

        var appManager = new Mock<IWechatAppManager>();
        appManager.SetupGet(m => m.ConfiguredAppKeys).Returns(new[] { AppKey1, AppKey2 });
        appManager.SetupGet(m => m.ConfiguredConfigs)
            .Returns(new[] { SuiteConfig(AppKey1), SuiteConfig(AppKey2) });

        var authService = new Mock<IWechatWorkAuthorizationService>();

        // 先让 SDK 完成真实装配，再以 Mock 覆盖外部边界（IWechatAppManager / 授权编排服务）。
        services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());
        services.AddSingleton(appManager.Object);
        services.AddSingleton(authService.Object);

        var provider = services.BuildServiceProvider();
        var coordinator = provider.GetRequiredService<IWechatAuthorizationCoordinator>();
        return (provider, authService, coordinator);
    }

    [Fact]
    public async Task Coordinator_ShouldContinueNextApp_WhenOneAppFails_OnChangeAuth()
    {
        var (provider, auth, coordinator) = CreateHost();
        using var _provider = provider;

        var refreshed = new WechatCorpAuthorization { AppKey = AppKey2, AuthCorpId = AuthCorpId, PermanentCode = "pc-2" };
        auth
            .Setup(m => m.GetAuthorizationAsync(AuthCorpId, AppKey1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WechatWorkException(60020, "get_auth_info failed"));
        auth
            .Setup(m => m.GetAuthorizationAsync(AuthCorpId, AppKey2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshed);
        auth
            .Setup(m => m.RefreshAuthorizationAsync(AuthCorpId, AppKey2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshed);

        var act = async () => await coordinator.OnAuthorizationChangedAsync(SuiteId, AuthCorpId);

        await act.Should().NotThrowAsync("P3-1：单应用失败不得中断整批（修复前异常直接冒泡中断）");
        auth.Verify(
            m => m.RefreshAuthorizationAsync(AuthCorpId, AppKey2, It.IsAny<CancellationToken>()),
            Times.Once, "失败应用之后的其余应用必须继续处理");
    }

    [Fact]
    public async Task Coordinator_ShouldContinueNextApp_WhenOneAppFails_OnCancelAuth()
    {
        var (provider, auth, coordinator) = CreateHost();
        using var _provider = provider;

        auth
            .Setup(m => m.RevokeAuthorizationAsync(AuthCorpId, AppKey1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("store unavailable"));
        auth
            .Setup(m => m.RevokeAuthorizationAsync(AuthCorpId, AppKey2, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var act = async () => await coordinator.OnAuthorizationCanceledAsync(SuiteId, AuthCorpId);

        await act.Should().NotThrowAsync("P3-1：单应用清理失败不得中断整批（G9 清理范围不变，仍为 SuiteId 命中集）");
        auth.Verify(
            m => m.RevokeAuthorizationAsync(AuthCorpId, AppKey2, It.IsAny<CancellationToken>()),
            Times.Once, "失败应用之后的其余应用必须继续清理");
    }

    [Fact]
    public async Task Coordinator_ShouldContinueNextApp_WhenOneAppFails_OnExchangeAuthCode()
    {
        var (provider, auth, coordinator) = CreateHost();
        using var _provider = provider;

        var exchanged = new WechatCorpAuthorization { AppKey = AppKey2, AuthCorpId = AuthCorpId, PermanentCode = "pc-2" };
        auth
            .Setup(m => m.ExchangeAuthCodeAsync("auth-code-x", AppKey1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WechatWorkException(40029, "invalid auth code"));
        auth
            .Setup(m => m.ExchangeAuthCodeAsync("auth-code-x", AppKey2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exchanged);

        var act = async () => await coordinator.OnAuthorizationSucceededAsync(SuiteId, "auth-code-x");

        await act.Should().NotThrowAsync("P3-1：单应用换码失败不得中断整批");
        auth.Verify(
            m => m.ExchangeAuthCodeAsync("auth-code-x", AppKey2, It.IsAny<CancellationToken>()),
            Times.Once, "失败应用之后的其余应用必须继续换码落库");
    }

    [Fact]
    public async Task Coordinator_ShouldPropagateCancellation_WhenCancelled()
    {
        var (provider, auth, coordinator) = CreateHost();
        using var _provider = provider;

        // 取消异常不在隔离捕获面：必须穿透（不得被「单应用失败继续」逻辑吞掉）。
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        auth
            .Setup(m => m.RevokeAuthorizationAsync(AuthCorpId, AppKey1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException(cts.Token));

        var act = async () => await coordinator.OnAuthorizationCanceledAsync(SuiteId, AuthCorpId, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>("取消必须穿透隔离逻辑");
    }
}
