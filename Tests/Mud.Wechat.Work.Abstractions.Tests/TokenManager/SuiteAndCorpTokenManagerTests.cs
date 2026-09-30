// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.HttpUtils;
using Mud.Wechat.Work.DataModels.CorpTokenAuthentication;
using Mud.Wechat.Work.DataModels.InternalAppAuthentication;
using Mud.Wechat.Work.DataModels.ProviderAuthentication;

namespace Mud.Wechat.Work.Abstractions.Tests.TokenManager;

/// <summary>
/// SuiteTokenManager 依赖 suite_ticket（缺失抛异常）与共享令牌语义测试（详细设计 §18.1）。
/// </summary>
public class SuiteAndCorpTokenManagerTests
{
    private static IOptions<WechatAppConfig> SuiteOptions(string appKey = "saas-suite")
        => Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
        {
            AppKey = appKey,
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite",
            SuiteSecret = "suite-secret",
        });

    private static IOptions<WechatAppConfig> CorpOptions(string appKey = "custom-agent")
        => Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
        {
            AppKey = appKey,
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite",
            SuiteSecret = "suite-secret",
            AgentSecret = "agent-secret",
        });

    private static IOptions<WechatAppConfig> ProviderOptions(string appKey = "custom-agent")
        => Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
        {
            AppKey = appKey,
            AppType = WechatAppType.Provider,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite",
            SuiteSecret = "suite-secret",
            AgentSecret = "agent-secret",
        });

    [Fact]
    public async Task SuiteTokenManager_ShouldThrow_WhenSuiteTicketMissing()
    {
        var authMock = new Mock<IWechatWorkProviderAuthentication>();
        var ticketStore = new InMemoryWechatSuiteTicketStore();
        var provider = new WechatSuiteTicketProvider(ticketStore);

        using var manager = new SuiteTokenManager(
            authMock.Object, provider, SuiteOptions(), NullLogger<SuiteTokenManager>.Instance);

        var act = async () => await manager.GetTokenAsync();
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("suite_ticket")
            .And.Contain("回调", "缺失票据时应提示宿主配置回调接收");
        authMock.Verify(a => a.GetSuiteTokenAsync(It.IsAny<GetSuiteTokenRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SuiteTokenManager_ShouldRefresh_WithLatestTicket()
    {
        var authMock = new Mock<IWechatWorkProviderAuthentication>();
        authMock
            .Setup(a => a.GetSuiteTokenAsync(It.IsAny<GetSuiteTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Mud.Wechat.Work.DataModels.ProviderAuthentication.GetSuiteTokenResponse
            {
                ErrorCode = 0,
                SuiteAccessToken = "suite-token",
                ExpiresIn = 7200,
            });

        var ticketStore = new InMemoryWechatSuiteTicketStore();
        await ticketStore.SetAsync("ww-suite", "ticket-1");
        var provider = new WechatSuiteTicketProvider(ticketStore);

        using var manager = new SuiteTokenManager(
            authMock.Object, provider, SuiteOptions(), NullLogger<SuiteTokenManager>.Instance);

        var token = await manager.GetTokenAsync();
        token.Should().Be("suite-token");
        authMock.Verify(a => a.GetSuiteTokenAsync(
            It.Is<GetSuiteTokenRequest>(r => r.SuiteTicket == "ticket-1" && r.SuiteId == "ww-suite"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void ProviderAndSuiteManagers_ShouldBeSharedTokenManagers()
    {
        var authMock = new Mock<IWechatWorkProviderAuthentication>();
        var provider = new WechatSuiteTicketProvider(new InMemoryWechatSuiteTicketStore());

        using var providerManager = new ProviderTokenManager(
            authMock.Object, SuiteOptions(), NullLogger<ProviderTokenManager>.Instance);
        using var suiteManager = new SuiteTokenManager(
            authMock.Object, provider, SuiteOptions(), NullLogger<SuiteTokenManager>.Instance);

        providerManager.Should().BeAssignableTo<ISharedTokenManager>("provider 令牌为全租户共享凭据（v2.0.9）");
        suiteManager.Should().BeAssignableTo<ISharedTokenManager>("suite 令牌为全租户共享凭据（v2.0.9）");
    }

    [Fact]
    public void InternalAppManager_ShouldNotBeSharedTokenManager()
    {
        var authMock = new Mock<IWechatWorkInternalAppAuthentication>();
        using var manager = new InternalAppTokenManager(
            authMock.Object, Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
            {
                AppKey = "default",
                CorpId = "ww-corp",
                AgentSecret = "agent-secret",
            }), NullLogger<InternalAppTokenManager>.Instance);

        manager.Should().NotBeAssignableTo<ISharedTokenManager>("自建应用令牌保留租户绑定守卫");
    }

    [Fact]
    public async Task CorpTokenManager_ShouldIsolateTokensByAuthCorpIdScope()
    {
        var corpAuthMock = new Mock<IWechatWorkCorpTokenAuthentication>();
        var suiteMock = new Mock<IWechatSuiteTokenManager>();
        suiteMock
            .Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("suite-token");

        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new WechatCorpAuthorization { AppKey = "custom-agent", AuthCorpId = "corp-A", PermanentCode = "permanent-A" });
        await corpAuthStore.SetAsync(new WechatCorpAuthorization { AppKey = "custom-agent", AuthCorpId = "corp-B", PermanentCode = "permanent-B" });

        var responses = new Dictionary<string, string>
        {
            ["corp-A"] = "corp-token-A",
            ["corp-B"] = "corp-token-B",
        };

        corpAuthMock
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, GetCorpTokenRequest request, CancellationToken _) => new GetCorpTokenResponse
            {
                ErrorCode = 0,
                AccessToken = responses[request.AuthCorpId],
                ExpiresIn = 7200,
            });

        using var manager = new CorpTokenManager(
            corpAuthMock.Object, suiteMock.Object, new Mock<IWechatWorkInternalAppAuthentication>().Object,
            corpAuthStore, CorpOptions(), NullLogger<CorpTokenManager>.Instance);

        // 显式 scope 路径：不同企业令牌互不命中（SR-M5 scope 隔离）。
        var tokenA = await manager.GetTokenAsync(new[] { "corp-A" });
        var tokenB = await manager.GetTokenAsync(new[] { "corp-B" });
        var tokenA2 = await manager.GetTokenAsync(new[] { "corp-A" });

        tokenA.Should().Be("corp-token-A");
        tokenB.Should().Be("corp-token-B", "不同企业的令牌必须隔离（scope = authCorpId）");
        tokenA2.Should().Be("corp-token-A", "同企业 scope 内缓存命中");
        corpAuthMock.Verify(
            c => c.GetCorpTokenAsync("suite-token", It.Is<GetCorpTokenRequest>(r => r.PermanentCode == "permanent-A"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CorpTokenManager_ShouldThrow_WhenNoAuthCorpIdAvailable()
    {
        var corpAuthMock = new Mock<IWechatWorkCorpTokenAuthentication>();
        var suiteMock = new Mock<IWechatSuiteTokenManager>();

        using var manager = new CorpTokenManager(
            corpAuthMock.Object, suiteMock.Object, new Mock<IWechatWorkInternalAppAuthentication>().Object,
            new InMemoryWechatCorpAuthStore(), CorpOptions(), NullLogger<CorpTokenManager>.Instance);

        var act = async () => await manager.GetTokenAsync();
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("authCorpId", "无企业上下文时应给出明确指引");
    }

    [Fact]
    public async Task CorpTokenManager_ShouldUseAmbientCorpContext_WhenSetCorpApplied()
    {
        var corpAuthMock = new Mock<IWechatWorkCorpTokenAuthentication>();
        var suiteMock = new Mock<IWechatSuiteTokenManager>();
        suiteMock
            .Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("suite-token");

        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new WechatCorpAuthorization { AppKey = "custom-agent", AuthCorpId = "corp-C", PermanentCode = "permanent-C" });

        corpAuthMock
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTokenResponse { ErrorCode = 0, AccessToken = "corp-token-C", ExpiresIn = 7200 });

        using var manager = new CorpTokenManager(
            corpAuthMock.Object, suiteMock.Object, new Mock<IWechatWorkInternalAppAuthentication>().Object,
            corpAuthStore, CorpOptions(), NullLogger<CorpTokenManager>.Instance);

        WechatCorpContext.SetCorp("custom-agent", "corp-C", null);
        try
        {
            var token = await manager.GetTokenAsync();
            token.Should().Be("corp-token-C", "SetCorp 切换的环境上下文应作为 scope 来源（实施注记方案①）");
            corpAuthMock.Verify(
                c => c.GetCorpTokenAsync("suite-token", It.Is<GetCorpTokenRequest>(r => r.AuthCorpId == "corp-C" && r.PermanentCode == "permanent-C"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    [Fact]
    public async Task CorpAuthStore_ShouldIsolateByCompositeKey()
    {
        var store = new InMemoryWechatCorpAuthStore();
        await store.SetAsync(new WechatCorpAuthorization { AppKey = "suite-a", AuthCorpId = "corp-1", PermanentCode = "pc-a" });
        await store.SetAsync(new WechatCorpAuthorization { AppKey = "suite-b", AuthCorpId = "corp-1", PermanentCode = "pc-b" });

        (await store.GetAsync("suite-a", "corp-1"))!.PermanentCode.Should().Be("pc-a", "同 authCorpId 不同 appKey 互不覆盖");
        (await store.GetAsync("suite-b", "corp-1"))!.PermanentCode.Should().Be("pc-b");
        (await store.GetAsync("suite-c", "corp-1")).Should().BeNull("未授权的应用键不应命中");

        (await store.ListAsync("suite-a")).Should().ContainSingle()
            .Which.AuthCorpId.Should().Be("corp-1", "ListAsync 只返回本应用的授权");
    }

    [Fact]
    public async Task CorpAuthStore_ShouldRemoveOnlyTargetAppEntry()
    {
        var store = new InMemoryWechatCorpAuthStore();
        await store.SetAsync(new WechatCorpAuthorization { AppKey = "suite-a", AuthCorpId = "corp-1", PermanentCode = "pc-a" });
        await store.SetAsync(new WechatCorpAuthorization { AppKey = "suite-b", AuthCorpId = "corp-1", PermanentCode = "pc-b" });

        await store.RemoveAsync("suite-a", "corp-1");

        (await store.GetAsync("suite-a", "corp-1")).Should().BeNull();
        (await store.GetAsync("suite-b", "corp-1")).Should().NotBeNull("移除只作用于复合键命中的条目，不影响其它套件");
    }

    [Fact]
    public async Task SuiteTicketStore_ShouldIsolateBySuiteId()
    {
        var store = new InMemoryWechatSuiteTicketStore();
        await store.SetAsync("suite-a", "ticket-a");
        await store.SetAsync("suite-b", "ticket-b");

        (await store.GetAsync("suite-a")).Should().Be("ticket-a");
        (await store.GetAsync("suite-b")).Should().Be("ticket-b", "不同套件槽位互不覆盖");
        (await store.GetAsync("suite-c")).Should().BeNull("未推送过票据的套件应为空");
    }

    [Fact]
    public async Task SuiteTicketProvider_ShouldThrowWithGuidance_WhenTicketMissing()
    {
        var provider = new WechatSuiteTicketProvider(new InMemoryWechatSuiteTicketStore());

        var act = async () => await provider.GetSuiteTicketAsync("suite-a");
        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("suite-a").And.Contain("suite_ticket").And.Contain("回调");
    }

    [Fact]
    public async Task CorpTokenManager_ProviderPath_ShouldUseGetToken_AndNotTouchSuiteToken()
    {
        var internalAuthMock = new Mock<IWechatWorkInternalAppAuthentication>();
        internalAuthMock
            .Setup(a => a.GetTokenAsync("corp-P", "secret-P", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResponse { ErrorCode = 0, AccessToken = "corp-token-P", ExpiresIn = 7200 });

        var corpAuthMock = new Mock<IWechatWorkCorpTokenAuthentication>();
        var suiteMock = new Mock<IWechatSuiteTokenManager>();

        var store = new InMemoryWechatCorpAuthStore();
        await store.SetAsync(new WechatCorpAuthorization { AppKey = "custom-agent", AuthCorpId = "corp-P", PermanentCode = "secret-P" });

        using var manager = new CorpTokenManager(
            corpAuthMock.Object, suiteMock.Object, internalAuthMock.Object, store,
            ProviderOptions(), NullLogger<CorpTokenManager>.Instance);

        var token = await manager.GetTokenAsync(new[] { "corp-P" });

        token.Should().Be("corp-token-P", "代开发 permanent_code 语义即应用 secret，走 gettoken");
        internalAuthMock.Verify(a => a.GetTokenAsync("corp-P", "secret-P", It.IsAny<CancellationToken>()), Times.Once);
        corpAuthMock.Verify(
            c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
            Times.Never, "代开发路径不应走 get_corp_token");
        suiteMock.Verify(
            s => s.GetTokenAsync(It.IsAny<CancellationToken>()), Times.Never,
            "代开发路径不需要 suite_access_token，不应触发套件令牌往返");
    }

    [Fact]
    public async Task CorpTokenManager_ShouldIgnoreAmbientContext_WhenAppKeyMismatch()
    {
        var corpAuthMock = new Mock<IWechatWorkCorpTokenAuthentication>();
        var suiteMock = new Mock<IWechatSuiteTokenManager>();
        suiteMock.Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("suite-token");

        corpAuthMock
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTokenResponse { ErrorCode = 0, AccessToken = "corp-token-X", ExpiresIn = 7200 });

        var store = new InMemoryWechatCorpAuthStore();
        await store.SetAsync(new WechatCorpAuthorization { AppKey = "custom-agent", AuthCorpId = "corp-X", PermanentCode = "pc-store" });

        using var manager = new CorpTokenManager(
            corpAuthMock.Object, suiteMock.Object, new Mock<IWechatWorkInternalAppAuthentication>().Object,
            store, CorpOptions(), NullLogger<CorpTokenManager>.Instance);

        // 环境上下文归属另一应用（suite-other）且携带错误的 permanentCode。
        WechatCorpContext.SetCorp("suite-other", "corp-X", "pc-wrong");
        try
        {
            var token = await manager.GetTokenAsync(new[] { "corp-X" });

            token.Should().Be("corp-token-X");
            corpAuthMock.Verify(
                c => c.GetCorpTokenAsync("suite-token",
                    It.Is<GetCorpTokenRequest>(r => r.AuthCorpId == "corp-X" && r.PermanentCode == "pc-store"),
                    It.IsAny<CancellationToken>()),
                Times.Once, "归属不一致时应整体忽略上下文并回退仓储取 permanentCode（R9）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }
}
