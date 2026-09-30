// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.DataModels.CorpTokenAuthentication;

namespace Mud.Wechat.Work.Abstractions.Tests.TokenManager;

/// <summary>
/// 企业级令牌的 scope 隔离与永久授权码同源校验（P2-2），以及 errcode 恢复的能力边界回归（P1-11）。
/// </summary>
public class CorpTokenManagerScopeIsolationTests
{
    private const string AppKey = "custom-agent";

    private static IOptions<WechatAppConfig> CorpOptions() => Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
    {
        AppKey = AppKey,
        AppType = WechatAppType.ThirdParty,
        CorpId = "ww-provider",
        ProviderSecret = "provider-secret",
        SuiteId = "ww-suite",
        SuiteSecret = "suite-secret",
        AgentSecret = "agent-secret",
    });

    private static CorpTokenManager CreateManager(
        Mock<IWechatWorkCorpTokenAuthentication> corpAuth,
        IWechatCorpAuthStore store,
        out Mock<IWechatSuiteTokenManager> suite)
    {
        suite = new Mock<IWechatSuiteTokenManager>();
        suite.Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("suite-token");

        return new CorpTokenManager(
            corpAuth.Object, suite.Object, new Mock<IWechatWorkInternalAppAuthentication>().Object,
            store, CorpOptions(), NullLogger<CorpTokenManager>.Instance);
    }

    /// <summary>
    /// P2-2：显式 scope = corp-X，但环境上下文是 corp-Y 的 permanent_code
    /// ⇒ 必须回退仓储，绝不能拿 Y 的授权码去换 X 的令牌（配对错位）。
    /// </summary>
    [Fact]
    public async Task Scope_ShouldIgnoreAmbientPermanentCode_WhenAuthCorpIdDiffers()
    {
        var corpAuth = new Mock<IWechatWorkCorpTokenAuthentication>();
        corpAuth
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTokenResponse { ErrorCode = 0, AccessToken = "token-X", ExpiresIn = 7200 });

        var store = new InMemoryWechatCorpAuthStore();
        await store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = AppKey, AuthCorpId = "corp-X", PermanentCode = "pc-store-X",
        });

        using var manager = CreateManager(corpAuth, store, out _);

        // 环境上下文指向 corp-Y（另一个企业）——不得被采用。
        WechatCorpContext.SetCorp(AppKey, "corp-Y", "pc-context-Y");
        try
        {
            var token = await manager.GetTokenAsync(new[] { "corp-X" });

            token.Should().Be("token-X");
            corpAuth.Verify(
                c => c.GetCorpTokenAsync("suite-token",
                    It.Is<GetCorpTokenRequest>(r => r.AuthCorpId == "corp-X" && r.PermanentCode == "pc-store-X"),
                    It.IsAny<CancellationToken>()),
                Times.Once,
                "corpId 不同源时必须回退仓储取 permanent_code（P2-2）");
            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(),
                    It.Is<GetCorpTokenRequest>(r => r.PermanentCode == "pc-context-Y"),
                    It.IsAny<CancellationToken>()),
                Times.Never,
                "绝不能把 corp-Y 的授权码用于 corp-X 的令牌换取");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>P2-2 正例：环境 corpId 与 scope 一致时采用上下文 permanent_code（保持既有"方案①"语义）。</summary>
    [Fact]
    public async Task Scope_ShouldUseAmbientPermanentCode_WhenAuthCorpIdMatches()
    {
        var corpAuth = new Mock<IWechatWorkCorpTokenAuthentication>();
        corpAuth
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTokenResponse { ErrorCode = 0, AccessToken = "token-C", ExpiresIn = 7200 });

        using var manager = CreateManager(corpAuth, new InMemoryWechatCorpAuthStore(), out _);

        WechatCorpContext.SetCorp(AppKey, "corp-C", "pc-context-C");
        try
        {
            await manager.GetTokenAsync(new[] { "corp-C" });

            corpAuth.Verify(
                c => c.GetCorpTokenAsync("suite-token",
                    It.Is<GetCorpTokenRequest>(r => r.AuthCorpId == "corp-C" && r.PermanentCode == "pc-context-C"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>
    /// P1-11 能力边界（回归锁定）：企业级令牌按 <c>authCorpId</c> 为 scope 缓存，
    /// 因此 errcode 恢复必须由宿主<b>显式传入 scope</b>；以默认作用域失效对已缓存的企业令牌是<b>空转</b>。
    /// 本用例锁定该语义，防止后续被误判为缺陷而"修"成隐患。
    /// </summary>
    [Fact]
    public async Task CorpToken_ShouldNotBeRecoveredByDefaultScope_WhenErrcodeInvalid()
    {
        var corpAuth = new Mock<IWechatWorkCorpTokenAuthentication>();
        corpAuth
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCorpTokenResponse { ErrorCode = 0, AccessToken = "token-C", ExpiresIn = 7200 });

        using var manager = CreateManager(corpAuth, new InMemoryWechatCorpAuthStore(), out _);


        WechatCorpContext.SetCorp(AppKey, "corp-C", "pc-C");
        try
        {
            var first = await manager.GetTokenAsync(new[] { "corp-C" });

            // 以「默认作用域」失效：不涉及 corp-C 的 scope 槽位。
            await manager.InvalidateTokenAsync(null);

            var second = await manager.GetTokenAsync(new[] { "corp-C" });

            first.Should().Be("token-C");
            second.Should().Be("token-C", "默认作用域失效对按 authCorpId 分槽的企业令牌不生效（能力边界）");
            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Once,
                "未发生真实刷新 ⇒ 证明 errcode 恢复必须显式传 scope（宿主应调用 InvalidateTokenAsync(appKey, AccessToken, new[]{ authCorpId })）");

            // 显式传 scope 时才真正刷新（宿主侧正确做法）。
            await manager.InvalidateTokenAsync(new[] { "corp-C" });
            await manager.GetTokenAsync(new[] { "corp-C" });

            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2),
                "显式按 scope 失效后必须走真实刷新");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }
}
