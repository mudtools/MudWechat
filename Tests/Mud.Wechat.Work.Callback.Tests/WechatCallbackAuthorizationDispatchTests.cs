// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Authentication.Models;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调授权事件分发测试：协调器委派、suiteId → appKey 定位、未注册协调器时的降级与一次性告警（方案 §4.7 / §6）。
/// </summary>
public class WechatCallbackAuthorizationDispatchTests
{
    private const string SuiteId = "ww-suite";
    private const string AppKey = "suite-app";

    private static (Mock<IWechatAppManager> AppManager, IWechatAppContext Context) CreateAppManager(string? suiteId = SuiteId)
    {
        var config = new WechatAppConfig
        {
            AppKey = AppKey,
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = suiteId ?? string.Empty,
            SuiteSecret = "suite-secret",
        };

        var context = new Mock<IWechatAppContext>();
        context.SetupGet(c => c.Config).Returns(config);

        var appManager = new Mock<IWechatAppManager>();
        appManager.Setup(m => m.ConfiguredAppKeys).Returns(new[] { AppKey });
        // P1-6：处理器/协调器只读配置快照（ConfiguredConfigs），不再经 TryGetApp 物化上下文。
        appManager.Setup(m => m.ConfiguredConfigs).Returns(new[] { config });

        return (appManager, context.Object);
    }

    private static (WechatCallbackHandler Handler, Mock<IWechatAuthorizationCoordinator> Coordinator, ServiceProvider Provider)
        CreateHandlerWithCoordinator(
            Mock<IWechatAppManager> appManager,
            IWechatCorpAuthStore corpAuthStore,
            ILogger<WechatCallbackHandler>? logger = null)
    {
        var coordinator = new Mock<IWechatAuthorizationCoordinator>();
        var provider = new ServiceCollection()
            .AddSingleton(coordinator.Object)
            .BuildServiceProvider();

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(),
            corpAuthStore,
            logger ?? NullLogger<WechatCallbackHandler>.Instance,
            appManager.Object,
            provider);

        return (handler, coordinator, provider);
    }

    [Fact]
    public async Task Handler_ShouldDelegateCreateAuthToCoordinator()
    {
        var (appManager, _) = CreateAppManager();
        var (handler, coordinator, provider) = CreateHandlerWithCoordinator(appManager, new InMemoryWechatCorpAuthStore());
        using var _disposable = provider;

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "create_auth",
            SuiteId = SuiteId,
            AuthCode = "auth-code-1",
        });

        coordinator.Verify(
            c => c.OnAuthorizationSucceededAsync(SuiteId, "auth-code-1", It.IsAny<CancellationToken>()),
            Times.Once, "create_auth 应委派协调器自动换码落库");
        coordinator.Verify(
            c => c.OnPermanentCodeResetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldDelegateResetPermanentCodeToCoordinator()
    {
        var (appManager, _) = CreateAppManager();
        var (handler, coordinator, provider) = CreateHandlerWithCoordinator(appManager, new InMemoryWechatCorpAuthStore());
        using var _disposable = provider;

        // R11：reset_permanent_code 报文本体不含 AuthCorpId，仅 SuiteId + AuthCode。
        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "reset_permanent_code",
            SuiteId = SuiteId,
            AuthCode = "auth-code-2",
        });

        coordinator.Verify(
            c => c.OnPermanentCodeResetAsync(SuiteId, "auth-code-2", It.IsAny<CancellationToken>()),
            Times.Once, "reset_permanent_code 应委派协调器重新换码覆盖仓储");
        coordinator.Verify(
            c => c.OnAuthorizationSucceededAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldRefreshBeforeInvalidating_OnChangeAuth()
    {
        var (appManager, _) = CreateAppManager();
        var callOrder = new List<string>();

        var coordinator = new Mock<IWechatAuthorizationCoordinator>();
        coordinator
            .Setup(c => c.OnAuthorizationChangedAsync(SuiteId, "corp-Y", It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("refresh"))
            .Returns(Task.CompletedTask);

        appManager
            .Setup(m => m.InvalidateTokenAsync(AppKey, Abstractions.WechatTokenTypes.AccessToken,
                It.IsAny<string[]?>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("invalidate"))
            .Returns(Task.CompletedTask);

        var provider = new ServiceCollection().AddSingleton(coordinator.Object).BuildServiceProvider();
        using var _disposable = provider;

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), new InMemoryWechatCorpAuthStore(),
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object, provider);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "change_auth",
            SuiteId = SuiteId,
            AuthCorpId = "corp-Y",
        });

        callOrder.Should().Equal(new[] { "refresh", "invalidate" }, "change_auth 应先刷新授权信息再级联失效企业令牌");
    }

    [Fact]
    public async Task Handler_ShouldRevokeOnlyMatchedAppKey_OnCancelAuth()
    {
        var (appManager, _) = CreateAppManager();
        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new WechatCorpAuthorization
        {
            AppKey = AppKey,
            AuthCorpId = "corp-X",
            PermanentCode = "pc-X",
        });

        var (handler, coordinator, provider) = CreateHandlerWithCoordinator(appManager, corpAuthStore);
        using var _disposable = provider;

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            SuiteId = SuiteId,
            AuthCorpId = "corp-X",
        });

        coordinator.Verify(
            c => c.OnAuthorizationCanceledAsync(SuiteId, "corp-X", It.IsAny<CancellationToken>()),
            Times.Once, "suiteId 精确命中时应委派协调器按复合键清理该 appKey");
        appManager.Verify(
            m => m.InvalidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string[]?>(), It.IsAny<CancellationToken>()),
            Times.Never, "企业令牌失效由编排服务在协调器内级联完成，不应在处理器内重复失效");
        (await corpAuthStore.GetAsync(AppKey, "corp-X")).Should().NotBeNull(
            "处理器不做直接清理，清理职责已收敛到协调器");
    }

    [Fact]
    public async Task Handler_ShouldNotDeleteOtherSuiteAuth_WhenCancelAuthSuiteIdUnmatched()
    {
        // P0-3：该 SuiteId 归属**另一个**已配置应用；被处理的 appKey 只是"碰巧存在企业授权记录"，
        // 旧实现会回退「全部应用清理」从而删除本应用的有效授权（跨套件数据损坏）。
        var (appManager, _) = CreateAppManager(suiteId: "other-suite");

        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new WechatCorpAuthorization
        {
            AppKey = AppKey,
            AuthCorpId = "corp-X",
            PermanentCode = "pc-X",
        });

        var (handler, coordinator, provider) = CreateHandlerWithCoordinator(appManager, corpAuthStore);
        using var _disposable = provider;

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            SuiteId = "unmatched-suite",
            AuthCorpId = "corp-X",
        });

        coordinator.Verify(
            c => c.OnAuthorizationCanceledAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never, "suiteId 未命中不应委派协调器");
        (await corpAuthStore.GetAsync(AppKey, "corp-X")).Should().NotBeNull(
            "suiteId 未命中时不得删除任何应用的授权记录（专治跨套件误删）");
        appManager.Verify(
            m => m.InvalidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string[]?>(), It.IsAny<CancellationToken>()),
            Times.Never, "未命中的降级路径不得级联失效任何应用的企业令牌");
    }

    [Fact]
    public async Task Handler_ShouldDeleteOnlyMatchedApp_WhenCancelAuthSuiteIdMatched()
    {
        // 两个应用归属不同 SuiteId：仅命中集内的应用被清理，另一个应用的授权记录逐条仍可读。
        var matchedConfig = new WechatAppConfig
        {
            AppKey = "matched-app",
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite-matched",
            SuiteSecret = "suite-secret",
        };
        var otherConfig = new WechatAppConfig
        {
            AppKey = "other-app",
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite-other",
            SuiteSecret = "suite-secret",
        };

        var appManager = new Mock<IWechatAppManager>();
        appManager.Setup(m => m.ConfiguredAppKeys).Returns(new[] { "matched-app", "other-app" });
        appManager.Setup(m => m.ConfiguredConfigs).Returns(new[] { matchedConfig, otherConfig });
        appManager
            .Setup(m => m.InvalidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string[]?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        // 企业 corp-X 在两个套件下各有一份独立授权。
        await corpAuthStore.SetAsync(new WechatCorpAuthorization
        {
            AppKey = "matched-app", AuthCorpId = "corp-X", PermanentCode = "pc-matched",
        });
        await corpAuthStore.SetAsync(new WechatCorpAuthorization
        {
            AppKey = "other-app", AuthCorpId = "corp-X", PermanentCode = "pc-other",
        });

        // 不注入协调器：走处理器自身的降级清理路径（清理范围仍为命中集）。
        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), corpAuthStore,
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            SuiteId = "ww-suite-matched",
            AuthCorpId = "corp-X",
        });

        (await corpAuthStore.GetAsync("matched-app", "corp-X")).Should().BeNull("命中应用应清理永久授权码");
        (await corpAuthStore.GetAsync("other-app", "corp-X")).Should().NotBeNull(
            "未命中应用（不同 SuiteId）的独立授权记录不得被连带删除");
        appManager.Verify(
            m => m.InvalidateTokenAsync("matched-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]?>(s => s != null && s[0] == "corp-X"), It.IsAny<CancellationToken>()),
            Times.Once);
        appManager.Verify(
            m => m.InvalidateTokenAsync("other-app", It.IsAny<string>(),
                It.IsAny<string[]?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handler_ShouldNotThrowAndWarnOnce_WhenCoordinatorMissing()
    {
        var logger = new CapturingLogger<WechatCallbackHandler>();
        var corpAuthStore = new InMemoryWechatCorpAuthStore();

        // 宿主未注册授权模块：仅注入 appManager（协调器解析返回 null）。
        var (appManager, _) = CreateAppManager();
        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), corpAuthStore, logger, appManager.Object);

        var evt = new WechatCallbackEvent { InfoType = "create_auth", SuiteId = SuiteId, AuthCode = "auth-code-3" };

        var act = async () =>
        {
            await handler.HandleAsync(evt);
            await handler.HandleAsync(evt);
        };

        await act.Should().NotThrowAsync("R10：未注册协调器时降级不抛异常，保持回调端点稳定响应");

        logger.Entries.Count(e => e.Level == LogLevel.Warning && e.Message.Contains("未注册授权协调器"))
            .Should().Be(1, "R10：高危配置错误仅首次命中输出一次性告警");
        (await corpAuthStore.ListAsync(AppKey)).Should().BeEmpty("降级路径不写仓储");
    }

    [Fact]
    public async Task Handler_ShouldIgnoreAuthCodeEvent_WhenAuthCodeEmpty()
    {
        var (appManager, _) = CreateAppManager();
        var (handler, coordinator, provider) = CreateHandlerWithCoordinator(appManager, new InMemoryWechatCorpAuthStore());
        using var _disposable = provider;

        var act = async () => await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "create_auth",
            SuiteId = SuiteId,
        });

        await act.Should().NotThrowAsync();
        coordinator.Verify(
            c => c.OnAuthorizationSucceededAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never, "AuthCode 为空的事件不委派协调器");
    }

    /// <summary>记录日志条目的最小 <see cref="ILogger{T}"/> 实现（用于断言「一次性告警」）。</summary>
    private sealed class CapturingLogger<T> : ILogger<T>
    {
        /// <summary>已记录的日志条目。</summary>
        public List<(LogLevel Level, string Message)> Entries { get; } = new();

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc />
        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }
}