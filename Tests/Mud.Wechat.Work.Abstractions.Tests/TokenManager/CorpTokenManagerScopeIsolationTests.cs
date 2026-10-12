// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.Abstractions.TokenManager;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.DataModels.CorpTokenAuthentication;

namespace Mud.Wechat.Work.Abstractions.Tests.TokenManager;

/// <summary>
/// 企业级令牌的 scope 隔离与永久授权码同源校验（P2-2），以及 errcode 恢复的能力边界回归（P1-11）。
/// <para>
/// <b>P0-1 闭环后</b>：环境上下文路径（<c>GetTokenAsync()</c> / <c>GetOrRefreshTokenAsync()</c>）
/// 与显式 scopes 路径收敛到<b>同一缓存/持久层键</b>（第三段 = authCorpId，永不落 <c>default</c>）。
/// </para>
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
        out Mock<IWechatSuiteTokenManager> suite,
        IWechatTokenStore? tokenStore = null)
    {
        suite = new Mock<IWechatSuiteTokenManager>();
        suite.Setup(s => s.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("suite-token");

        return new CorpTokenManager(
            corpAuth.Object, suite.Object, new Mock<IWechatWorkInternalAppAuthentication>().Object,
            store, CorpOptions(), NullLogger<CorpTokenManager>.Instance, tokenStore);
    }

    /// <summary>按 authCorpId 返回不同令牌的企业令牌客户端。</summary>
    private static Mock<IWechatWorkCorpTokenAuthentication> CorpAuthMockReturningPerCorp()
    {
        var corpAuth = new Mock<IWechatWorkCorpTokenAuthentication>();
        corpAuth
            .Setup(c => c.GetCorpTokenAsync("suite-token", It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, GetCorpTokenRequest request, CancellationToken _) => new GetCorpTokenResponse
            {
                ErrorCode = 0,
                AccessToken = $"token-{request.AuthCorpId}",
                ExpiresIn = 7200,
            });
        return corpAuth;
    }

    private static async Task<InMemoryWechatCorpAuthStore> StoreWithAsync(params string[] authCorpIds)
    {
        var store = new InMemoryWechatCorpAuthStore();
        foreach (var authCorpId in authCorpIds)
        {
            await store.SetAsync(new WechatCorpAuthorization
            {
                AppKey = AppKey, AuthCorpId = authCorpId, PermanentCode = $"pc-{authCorpId}",
            });
        }

        return store;
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
    /// P0-1 回归：环境上下文路径必须<b>按 authCorpId 分槽</b>——两个企业经环境路径交替取令牌
    /// 不得命中对方槽位（修复前二者共用 <c>default</c> 槽 ⇒ 以 A 企业令牌访问 B 企业数据面）。
    /// </summary>
    [Fact]
    public async Task GetTokenAsync_ShouldReturnDistinctToken_WhenSwitchingCorpScopeEnvironment()
    {
        var corpAuth = CorpAuthMockReturningPerCorp();
        var store = await StoreWithAsync("corp-A", "corp-B");

        using var manager = CreateManager(corpAuth, store, out _);

        try
        {
            WechatCorpContext.SetCorp(AppKey, "corp-A", null);
            var tokenA = await manager.GetTokenAsync();

            WechatCorpContext.SetCorp(AppKey, "corp-B", null);
            var tokenB = await manager.GetTokenAsync();

            // 切回 corp-A：应命中 corp-A 自己的槽（不因 corp-B 写入而串号）
            WechatCorpContext.SetCorp(AppKey, "corp-A", null);
            var tokenA2 = await manager.GetTokenAsync();

            tokenA.Should().Be("token-corp-A");
            tokenB.Should().Be("token-corp-B", "环境路径必须按 authCorpId 分槽（P0-1）");
            tokenA2.Should().Be("token-corp-A", "切回原企业应命中其自身槽位");
            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2),
                "两个企业各刷新一次，切回时命中缓存（若串号则会少刷或多刷）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>
    /// P0-1 核心回归：<b>环境路径与显式 scopes 路径对同一企业必须命中同一槽</b>，
    /// 且持久层键第三段为 <c>authCorpId</c>、<b>不含</b> <c>default</c>。
    /// </summary>
    [Fact]
    public async Task GetTokenAsync_ShouldUseSameStoreKey_WhenAmbientAndExplicitScopeMatchSameCorp()
    {
        var corpAuth = CorpAuthMockReturningPerCorp();
        var store = await StoreWithAsync("corp-A");
        var tokenStore = new InMemoryWechatTokenStore();

        using var manager = CreateManager(corpAuth, store, out _, tokenStore);

        try
        {
            WechatCorpContext.SetCorp(AppKey, "corp-A", null);
            var ambient = await manager.GetTokenAsync();
            var explicitScope = await manager.GetTokenAsync(new[] { "corp-A" });

            ambient.Should().Be("token-corp-A");
            explicitScope.Should().Be("token-corp-A");
            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Once,
                "环境路径与显式路径同键 ⇒ 第二次为缓存命中，不得重复刷新");

            var keys = (await tokenStore.GetTokenTypesAsync()).ToList();
            keys.Should().Contain(k => k.EndsWith(":corp-A", StringComparison.Ordinal),
                "持久层键第三段必须为 authCorpId");
            keys.Should().NotContain(k => k.EndsWith(":default", StringComparison.Ordinal),
                "企业级令牌永不写入 default 段（P0-1 不变量）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>
    /// P1-1 回归：环境路径取到令牌后，<b>无 scopes</b> 的失效必须清掉该企业的 scope 槽
    /// （修复前打在 <c>default</c> 槽 ⇒ 空转，40014 反复且无自愈）。
    /// </summary>
    [Fact]
    public async Task InvalidateTokenAsync_ShouldPurgeCorpScopedSlot_WhenScopesOmittedInAmbientContext()
    {
        var corpAuth = CorpAuthMockReturningPerCorp();
        var store = await StoreWithAsync("corp-A");

        using var manager = CreateManager(corpAuth, store, out _);

        try
        {
            WechatCorpContext.SetCorp(AppKey, "corp-A", null);
            await manager.GetTokenAsync();

            await manager.InvalidateTokenAsync();

            await manager.GetTokenAsync();

            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2),
                "无 scopes 失效必须按环境解析补齐 scope 并清掉 corp-A 槽（P1-1）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>无环境上下文且未传 scopes 时，沿用既有 fail-fast 引导语（原文不变，防文案漂移）。</summary>
    [Fact]
    public async Task GetTokenAsync_ShouldFallbackToGuidanceError_WhenNoAmbientContextAndNoScopes()
    {
        using var manager = CreateManager(CorpAuthMockReturningPerCorp(), new InMemoryWechatCorpAuthStore(), out _);

        var act = async () => await manager.GetTokenAsync();

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("authCorpId")
            .And.Contain("UseCorpScope", "无上下文时须给出既有引导语原文");
    }

    /// <summary>归属不一致（R9）：appA 环境上下文不得被 appB 管理器采用（归属闸不弱化）。</summary>
    [Fact]
    public async Task GetTokenAsync_ShouldIgnoreAmbientContext_WhenAppKeyOwnershipMismatch()
    {
        using var manager = CreateManager(CorpAuthMockReturningPerCorp(), await StoreWithAsync("corp-A"), out _);

        try
        {
            WechatCorpContext.SetCorp("suite-other", "corp-A", "pc-wrong");

            var act = async () => await manager.GetTokenAsync();

            (await act.Should().ThrowAsync<InvalidOperationException>())
                .Which.Message.Should().Contain("authCorpId",
                    "归属不一致时上下文整体被忽略 ⇒ 走无上下文分支（fail-fast）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>第二条环境入口（组件声明式客户端直调的重载）同样必须按环境 scope 取令牌。</summary>
    [Fact]
    public async Task GetOrRefreshTokenAsync_ShouldHonorAmbientScope()
    {
        var corpAuth = CorpAuthMockReturningPerCorp();
        var store = await StoreWithAsync("corp-A");
        var tokenStore = new InMemoryWechatTokenStore();

        using var manager = CreateManager(corpAuth, store, out _, tokenStore);

        try
        {
            WechatCorpContext.SetCorp(AppKey, "corp-A", null);
            var token = await manager.GetOrRefreshTokenAsync();

            token.Should().Be("token-corp-A");
            var keys = (await tokenStore.GetTokenTypesAsync()).ToList();
            keys.Should().Contain(k => k.EndsWith(":corp-A", StringComparison.Ordinal),
                "GetOrRefreshTokenAsync(ct) 亦须落 authCorpId 槽（唯一汇合点）");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }

    /// <summary>
    /// P1-1 能力边界（<b>P0-1 修复后语义反转</b>）：环境上下文存在时，
    /// <b>无 scopes</b> 的失效现已按环境补齐 scope ⇒ 真正清掉该企业槽并触发刷新。
    /// </summary>
    [Fact]
    public async Task CorpToken_ShouldBeRecoveredByAmbientScope_WhenErrcodeInvalid()
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

            // 无 scopes 失效：环境上下文（corp-C）参与解析 ⇒ 清掉 corp-C 槽（修复前为空转）。
            await manager.InvalidateTokenAsync(null);

            var second = await manager.GetTokenAsync(new[] { "corp-C" });

            first.Should().Be("token-C");
            second.Should().Be("token-C");
            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2),
                "无 scopes 失效已按环境补齐 scope ⇒ 必须走真实刷新（P1-1）");

            // 显式传 scope 的失效路径行为不变。
            await manager.InvalidateTokenAsync(new[] { "corp-C" });
            await manager.GetTokenAsync(new[] { "corp-C" });

            corpAuth.Verify(
                c => c.GetCorpTokenAsync(It.IsAny<string>(), It.IsAny<GetCorpTokenRequest>(), It.IsAny<CancellationToken>()),
                Times.Exactly(3),
                "显式按 scope 失效后必须走真实刷新");
        }
        finally
        {
            WechatCorpContext.Clear();
        }
    }
}
