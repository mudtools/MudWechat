// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.HttpUtils;
using Mud.Wechat.Work.DataModels.CorpTokenAuthentication;
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
            AppType = WechatAppType.Provider,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = "ww-suite",
            SuiteSecret = "suite-secret",
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
        await ticketStore.SetAsync("ticket-1");
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
        await corpAuthStore.SetAsync(new CorpAuth("corp-A", "permanent-A"));
        await corpAuthStore.SetAsync(new CorpAuth("corp-B", "permanent-B"));

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
            corpAuthMock.Object, suiteMock.Object, corpAuthStore, CorpOptions(), NullLogger<CorpTokenManager>.Instance);

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
            corpAuthMock.Object, suiteMock.Object, new InMemoryWechatCorpAuthStore(), CorpOptions(), NullLogger<CorpTokenManager>.Instance);

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
        await corpAuthStore.SetAsync(new CorpAuth("corp-C", "permanent-C"));

        corpAuthMock
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTokenResponse { ErrorCode = 0, AccessToken = "corp-token-C", ExpiresIn = 7200 });

        using var manager = new CorpTokenManager(
            corpAuthMock.Object, suiteMock.Object, corpAuthStore, CorpOptions(), NullLogger<CorpTokenManager>.Instance);

        WechatCorpContext.SetCorp("corp-C", null);
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
}
