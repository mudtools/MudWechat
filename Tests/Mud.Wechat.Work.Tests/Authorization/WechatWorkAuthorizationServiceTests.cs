// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.Models;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.ProviderAuthentication;
using Mud.Wechat.Work.Services.Authorization;
using Mud.Wechat.Work.Services.Authorization.Models;

namespace Mud.Wechat.Work.Tests.Authorization;

/// <summary>
/// 授权编排服务测试（阶段 E）：授权链接拼装 / 换码落库 / 刷新 / 撤销 / 并发单飞 / 参数边界。
/// </summary>
public class WechatWorkAuthorizationServiceTests
{
    private const string SuiteAppKey = "suite-app";
    private const string ProviderAppKey = "provider-app";
    private const string AuthCorpId = "ww-auth-corp";

    private static WechatAppConfig SuiteConfig() => new()
    {
        AppKey = SuiteAppKey,
        AppType = WechatAppType.ThirdParty,
        CorpId = "ww-provider",
        ProviderSecret = "provider-secret",
        SuiteId = "ww-suite-id",
        SuiteSecret = "suite-secret",
        IsDefault = true,
    };

    private static WechatAppConfig ProviderConfig() => new()
    {
        AppKey = ProviderAppKey,
        AppType = WechatAppType.Provider,
        CorpId = "ww-provider",
        ProviderSecret = "provider-secret",
        // K2：代开发模板 id 即 suite_id（不设独立 TemplateId 配置项）。
        SuiteId = "dk-template-id",
        SuiteSecret = "suite-secret",
    };

    private static GetPermanentCodeResponse PermanentCodeResponse(
        string permanentCode = "perm-1", string corpId = AuthCorpId, bool isCustomizedApp = false)
        => new()
        {
            PermanentCode = permanentCode,
            State = "state-1",
            AuthCorpInfo = new AuthCorpDetailInfo { CorpId = corpId, CorpName = "测试企业" },
            AuthInfo = new AuthInfo
            {
                Agents =
                [
                    new Agent { AgentId = 1001, Name = "测试应用", AuthMode = 0, IsCustomizedApp = isCustomizedApp },
                ],
            },
            AuthUserInfo = new AuthUserInfo { UserId = "admin" },
        };

    private sealed class Host
    {
        public ServiceProvider Provider { get; init; } = default!;

        public Mock<IWechatAppManager> AppManager { get; init; } = default!;

        public Mock<IWechatWorkProviderAuthenticationService> ProviderAuth { get; init; } = default!;

        public Mock<IWechatWorkProviderAuthenticationUrl> ProviderUrl { get; init; } = default!;

        public Mock<IWechatProviderTokenManager> ProviderTokenManager { get; init; } = default!;

        public IWechatCorpAuthStore Store { get; init; } = default!;

        public IWechatWorkAuthorizationService Service { get; init; } = default!;

        public IWechatAuthorizationCoordinator Coordinator { get; init; } = default!;
    }

    /// <summary>Moq 对 <c>out</c> 参数的回调委托（<see cref="IWechatAppManager.TryGetApp"/>）。</summary>
    private delegate bool TryGetAppCallback(string appKey, out IWechatAppContext? appContext);

    private static Host CreateHost(
        Action<WechatAuthorizationOptions>? configureOptions = null, bool validateScopes = false)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatApp(new List<WechatAppConfig> { SuiteConfig(), ProviderConfig() });

        var providerTokenManager = new Mock<IWechatProviderTokenManager>();
        providerTokenManager
            .Setup(m => m.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("provider-access-token");

        var appManager = new Mock<IWechatAppManager>();
        appManager.Setup(m => m.GetApp(It.IsAny<string>())).Returns<string>(appKey =>
        {
            var config = string.Equals(appKey, ProviderAppKey, StringComparison.Ordinal)
                ? ProviderConfig()
                : SuiteConfig();

            var context = new Mock<IWechatAppContext>();
            context.SetupGet(c => c.Config).Returns(config);
            context.SetupGet(c => c.ProviderTokenManager).Returns(providerTokenManager.Object);
            return context.Object;
        });

        // 协调器经 TryGetApp 读取 Config.SuiteId 完成 suiteId → appKey 映射。
        appManager
            .Setup(m => m.TryGetApp(It.IsAny<string>(), out It.Ref<IWechatAppContext?>.IsAny))
            .Returns(new TryGetAppCallback((string appKey, out IWechatAppContext? context) =>
            {
                if (string.Equals(appKey, ProviderAppKey, StringComparison.Ordinal)
                    || string.Equals(appKey, SuiteAppKey, StringComparison.Ordinal))
                {
                    context = appManager.Object.GetApp(appKey);
                    return true;
                }

                context = null;
                return false;
            }));
        appManager.SetupGet(m => m.DefaultConfig).Returns(SuiteConfig());
        appManager.SetupGet(m => m.ConfiguredAppKeys).Returns(new[] { SuiteAppKey, ProviderAppKey });
        // P1-6：协调器改读配置快照（非物化），不再依赖 TryGetApp。
        appManager.SetupGet(m => m.ConfiguredConfigs).Returns(new[] { SuiteConfig(), ProviderConfig() });
        appManager
            .Setup(m => m.InvalidateTokenAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var providerAuth = new Mock<IWechatWorkProviderAuthenticationService>();
        var providerUrl = new Mock<IWechatWorkProviderAuthenticationUrl>();

        // 先让 SDK 完成真实装配（含 IWechatAppContextSwitcher / 令牌基座 / 生成式客户端），再以 Mock 覆盖外部边界。
        services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());

        services.AddSingleton(appManager.Object);
        services.AddSingleton(providerAuth.Object);
        services.AddSingleton(providerUrl.Object);
        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }

        var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = validateScopes });

        return new Host
        {
            Provider = provider,
            AppManager = appManager,
            ProviderAuth = providerAuth,
            ProviderUrl = providerUrl,
            ProviderTokenManager = providerTokenManager,
            Store = provider.GetRequiredService<IWechatCorpAuthStore>(),
            Service = provider.GetRequiredService<IWechatWorkAuthorizationService>(),
            Coordinator = provider.GetRequiredService<IWechatAuthorizationCoordinator>(),
        };
    }

    // ── 授权发起：第三方应用安装链接 ─────────────────────────────

    [Fact]
    public async Task CreateSuiteAuthorizationUrl_ShouldComposeOfficialUrl_AndCarrySessionInfo()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPreAuthCodeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetPreAuthCodeResponse { PreAuthCode = "pre-123", ExpiresIn = 1200 });
        host.ProviderAuth
            .Setup(m => m.SetSessionInfoAsync(It.IsAny<SetSessionInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new WechatWorkResponse());

        var result = await host.Service.CreateSuiteAuthorizationUrlAsync(new WechatAuthorizationUrlRequest
        {
            RedirectUri = "https://host.example.com/callback?x=1",
            State = "st1",
            SessionInfo = new SetSessionInfoRequest { Session = new SessionInfo { AuthType = 1 } },
        });

        result.Url.Should().StartWith("https://open.work.weixin.qq.com/3rdapp/install?");
        result.Url.Should().Contain("suite_id=ww-suite-id");
        result.Url.Should().Contain("pre_auth_code=pre-123");
        result.Url.Should().Contain(Uri.EscapeDataString("https://host.example.com/callback?x=1"));
        result.Url.Should().Contain("state=st1");
        result.PreAuthCode.Should().Be("pre-123");
        result.ExpiresIn.Should().Be(1200);
        result.ExpireAt.Should().BeAfter(DateTimeOffset.UtcNow);

        // set_session_info 必须以本次取得的预授权码覆盖请求中传入的值。
        host.ProviderAuth.Verify(m => m.SetSessionInfoAsync(
            It.Is<SetSessionInfoRequest>(r => r.PreAuthCode == "pre-123" && r.Session.AuthType == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSuiteAuthorizationUrl_ShouldSkipSessionInfo_WhenNotProvided()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPreAuthCodeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetPreAuthCodeResponse { PreAuthCode = "pre-1", ExpiresIn = 0 });

        var result = await host.Service.CreateSuiteAuthorizationUrlAsync(
            new WechatAuthorizationUrlRequest { RedirectUri = "https://host.example.com/cb" });

        // ExpiresIn 缺省回退 1200 秒。
        result.ExpiresIn.Should().Be(1200);
        host.ProviderAuth.Verify(m => m.SetSessionInfoAsync(
            It.IsAny<SetSessionInfoRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateSuiteAuthorizationUrl_ShouldFallbackToOptionsRedirectUri()
    {
        var host = CreateHost(o => o.AuthorizationRedirectUri = "https://options.example.com/cb");
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPreAuthCodeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetPreAuthCodeResponse { PreAuthCode = "pre-1", ExpiresIn = 1200 });

        var result = await host.Service.CreateSuiteAuthorizationUrlAsync(new WechatAuthorizationUrlRequest());

        result.Url.Should().Contain(Uri.EscapeDataString("https://options.example.com/cb"));
    }

    [Fact]
    public async Task CreateSuiteAuthorizationUrl_ShouldThrow_WhenRedirectUriMissing()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var act = () => host.Service.CreateSuiteAuthorizationUrlAsync(new WechatAuthorizationUrlRequest());

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*缺少授权回跳地址*");
    }

    [Fact]
    public async Task CreateSuiteAuthorizationUrl_ShouldThrow_WhenStateExceeds128Bytes()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var act = () => host.Service.CreateSuiteAuthorizationUrlAsync(new WechatAuthorizationUrlRequest
        {
            RedirectUri = "https://host.example.com/cb",
            State = new string('a', 129),
        });

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*128 字节*");
    }

    [Fact]
    public async Task CreateSuiteAuthorizationUrl_ShouldThrow_WhenPreAuthCodeFailed()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPreAuthCodeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetPreAuthCodeResponse { ErrorCode = 40001, ErrorMessage = "invalid credential" });

        var act = () => host.Service.CreateSuiteAuthorizationUrlAsync(
            new WechatAuthorizationUrlRequest { RedirectUri = "https://host.example.com/cb" });

        await act.Should().ThrowAsync<WechatWorkException>();
    }

    // ── 授权发起：代开发带参授权链接 ────────────────────────────

    [Fact]
    public async Task CreateCustomizedAuthorizationUrl_ShouldPassProviderToken_AndReturnQrcodeUrl()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderUrl
            .Setup(m => m.GetCustomizedAuthUrlAsync(
                It.IsAny<string>(), It.IsAny<GetCustomizedAuthUrlRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCustomizedAuthUrlResponse { QrcodeUrl = "https://open.work.weixin.qq.com/qr/abc" });

        var result = await host.Service.CreateCustomizedAuthorizationUrlAsync(
            "abc123", new[] { "dk-template-id" }, ProviderAppKey);

        result.QrcodeUrl.Should().Be("https://open.work.weixin.qq.com/qr/abc");
        // ExpiresIn 缺省回退 10 天。
        result.ExpiresIn.Should().Be(864000);

        host.ProviderUrl.Verify(m => m.GetCustomizedAuthUrlAsync(
            "provider-access-token",
            It.Is<GetCustomizedAuthUrlRequest>(r => r.State == "abc123" && r.TemplateIdList.Count == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a-b")]
    [InlineData("中文")]
    [InlineData("0123456789012345678901234567890123")]
    public async Task CreateCustomizedAuthorizationUrl_ShouldThrow_WhenStateInvalid(string state)
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var act = () => host.Service.CreateCustomizedAuthorizationUrlAsync(
            state, new[] { "dk-template-id" }, ProviderAppKey);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateCustomizedAuthorizationUrl_ShouldThrow_WhenTemplateListOutOfRange()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var empty = () => host.Service.CreateCustomizedAuthorizationUrlAsync(
            "abc", Array.Empty<string>(), ProviderAppKey);
        var tooMany = () => host.Service.CreateCustomizedAuthorizationUrlAsync(
            "abc", Enumerable.Range(0, 10).Select(i => $"dk-{i}").ToArray(), ProviderAppKey);
        var blank = () => host.Service.CreateCustomizedAuthorizationUrlAsync(
            "abc", new[] { "dk-1", " " }, ProviderAppKey);

        await empty.Should().ThrowAsync<ArgumentException>().WithMessage("*不能为空*");
        await tooMany.Should().ThrowAsync<ArgumentException>().WithMessage("*最多 9 个*");
        await blank.Should().ThrowAsync<ArgumentException>().WithMessage("*不得包含空值*");
    }

    // ── 授权落地：换码 ──────────────────────────────────────────

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldPersistAuthorization_AndUseV2EndpointByDefault()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PermanentCodeResponse());

        var result = await host.Service.ExchangeAuthCodeAsync("auth-code-1");

        result.AppKey.Should().Be(SuiteAppKey);
        result.AuthCorpId.Should().Be(AuthCorpId);
        result.PermanentCode.Should().Be("perm-1");
        result.State.Should().Be("state-1");
        result.CorpInfo!.CorpName.Should().Be("测试企业");
        result.Agents.Should().HaveCount(1);
        result.AgentId.Should().Be(1001);
        result.UpdatedAt.Should().BeGreaterThan(0);

        var stored = await host.Store.GetAsync(SuiteAppKey, AuthCorpId);
        stored.Should().NotBeNull();
        stored!.PermanentCode.Should().Be("perm-1");

        host.ProviderAuth.Verify(m => m.GetPermanentCodeAsync(
            It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldUseLegacyEndpoint_WhenV2Disabled()
    {
        var host = CreateHost(o => o.UseV2AuthApi = false);
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeAsync(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PermanentCodeResponse());

        await host.Service.ExchangeAuthCodeAsync("auth-code-1");

        host.ProviderAuth.Verify(m => m.GetPermanentCodeAsync(
            It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        host.ProviderAuth.Verify(m => m.GetPermanentCodeV2Async(
            It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldBeSingleFlight_UnderConcurrency()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var gate = new TaskCompletionSource<GetPermanentCodeResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .Returns<GetPermanentCodeRequest, CancellationToken>((_, _) =>
            {
                Interlocked.Increment(ref calls);
                return gate.Task;
            });

        // 10 个并发调用先同时挂起在单飞门上，再由测试释放。
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => host.Service.ExchangeAuthCodeAsync("auth-code-concurrent"))
            .ToArray();

        gate.SetResult(PermanentCodeResponse());
        var results = await Task.WhenAll(tasks);

        calls.Should().Be(1, "同一 authCode 的并发换码必须收敛为一次 API 调用");
        results.Should().OnlyContain(r => ReferenceEquals(r, results[0]));
        (await host.Store.ListAsync(SuiteAppKey)).Should().HaveCount(1);
    }

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldReturnRememberedResult_OnDuplicateCall()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PermanentCodeResponse());

        var first = await host.Service.ExchangeAuthCodeAsync("auth-code-dup");
        var second = await host.Service.ExchangeAuthCodeAsync("auth-code-dup");

        second.AuthCorpId.Should().Be(first.AuthCorpId);
        host.ProviderAuth.Verify(m => m.GetPermanentCodeV2Async(
            It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldThrow_WhenResponseMissingAuthCorpId()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetPermanentCodeResponse
            {
                PermanentCode = "perm-1",
                AuthCorpInfo = new AuthCorpDetailInfo { CorpId = null },
            });

        var act = () => host.Service.ExchangeAuthCodeAsync("auth-code-1");

        await act.Should().ThrowAsync<WechatWorkException>().WithMessage("*auth_corp_info.corpid*");
        (await host.Store.ListAsync(SuiteAppKey)).Should().BeEmpty();
    }

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldThrow_WhenAuthCodeBlank()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var act = () => host.Service.ExchangeAuthCodeAsync(" ");

        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ── 授权维护：刷新 / 撤销 / 查询 ────────────────────────────

    [Fact]
    public async Task RefreshAuthorizationAsync_ShouldMergeAuthInfo_AndPreservePermanentCode()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        await host.Store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = SuiteAppKey,
            AuthCorpId = AuthCorpId,
            PermanentCode = "perm-keep",
            State = "state-keep",
            CorpInfo = new WechatAuthCorpInfo { CorpId = AuthCorpId, CorpName = "旧名" },
        });

        host.ProviderAuth
            .Setup(m => m.GetAuthInfoV2Async(It.IsAny<GetAuthInfoRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAuthInfoResponse
            {
                AuthCorpInfo = new AuthCorpDetailInfoExt { CorpId = AuthCorpId, CorpName = "新名" },
                AuthInfo = new AuthInfo { Agents = [new Agent { AgentId = 2002, AuthMode = 1 }] },
            });

        var refreshed = await host.Service.RefreshAuthorizationAsync(AuthCorpId);

        refreshed.PermanentCode.Should().Be("perm-keep", "刷新不得覆盖永久授权码");
        refreshed.State.Should().Be("state-keep");
        refreshed.CorpInfo!.CorpName.Should().Be("新名");
        refreshed.Agents.Should().HaveCount(1);
        refreshed.Agents[0].AgentId.Should().Be(2002);
        refreshed.AuthMode.Should().Be(1);
        refreshed.UpdatedAt.Should().BeGreaterThan(0);

        // 请求必须携带仓储中的永久授权码。
        host.ProviderAuth.Verify(m => m.GetAuthInfoV2Async(
            It.Is<GetAuthInfoRequest>(r => r.AuthorizerCorpId == AuthCorpId && r.PermanentAuthCode == "perm-keep"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RefreshAuthorizationAsync_ShouldThrow_WhenNotAuthorized()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var act = () => host.Service.RefreshAuthorizationAsync("ww-unknown");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*未找到*");
    }

    [Fact]
    public async Task RevokeAuthorizationAsync_ShouldOnlyInvalidateOwnAppKey()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        await host.Store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = SuiteAppKey,
            AuthCorpId = AuthCorpId,
            PermanentCode = "perm-1",
        });

        await host.Service.RevokeAuthorizationAsync(AuthCorpId, SuiteAppKey);

        (await host.Store.GetAsync(SuiteAppKey, AuthCorpId)).Should().BeNull();
        host.AppManager.Verify(m => m.InvalidateTokenAsync(
            SuiteAppKey,
            WechatTokenTypes.AccessToken,
            It.Is<string[]>(s => s.Length == 1 && s[0] == AuthCorpId),
            It.IsAny<CancellationToken>()), Times.Once);
        host.AppManager.Verify(m => m.InvalidateTokenAsync(
            ProviderAppKey, It.IsAny<string>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetAndListAuthorizations_ShouldBeScopedByAppKey()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        await host.Store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = SuiteAppKey,
            AuthCorpId = AuthCorpId,
            PermanentCode = "perm-suite",
        });
        await host.Store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = ProviderAppKey,
            AuthCorpId = AuthCorpId,
            PermanentCode = "perm-provider",
        });

        (await host.Service.GetAuthorizationAsync(AuthCorpId, SuiteAppKey))!.PermanentCode
            .Should().Be("perm-suite");
        (await host.Service.GetAuthorizationAsync(AuthCorpId, ProviderAppKey))!.PermanentCode
            .Should().Be("perm-provider");
        (await host.Service.GetAuthorizationAsync("ww-missing", SuiteAppKey)).Should().BeNull();

        (await host.Service.ListAuthorizationsAsync(SuiteAppKey)).Should().HaveCount(1);
        (await host.Service.ListAuthorizationsAsync(ProviderAppKey)).Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateCustomizedAuthorizationUrl_ShouldUseOptionsDefaultAppKey_WhenAppKeyOmitted()
    {
        var host = CreateHost(o => o.DefaultAppKey = ProviderAppKey);
        using var provider = host.Provider;

        host.ProviderUrl
            .Setup(m => m.GetCustomizedAuthUrlAsync(
                It.IsAny<string>(), It.IsAny<GetCustomizedAuthUrlRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetCustomizedAuthUrlResponse { QrcodeUrl = "https://qr" });

        // 未显式传入 appKey → 取 Options.DefaultAppKey（provider-app）。
        await host.Service.CreateCustomizedAuthorizationUrlAsync("abc", new[] { "dk-1" });

        host.AppManager.Verify(m => m.GetApp(ProviderAppKey), Times.AtLeastOnce);
    }

    // ── 授权协调器（Phase F） ────────────────────────────────────

    [Fact]
    public async Task Coordinator_ShouldMapSuiteIdToAppKey_AndStoreAuthorization()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(PermanentCodeResponse("perm-coord"));

        // K2：代开发模板 id 即 suite_id → 仅命中 ProviderAppKey。
        await host.Coordinator.OnAuthorizationSucceededAsync("dk-template-id", "auth-code-coord");

        var auth = await host.Store.GetAsync(ProviderAppKey, AuthCorpId);
        auth.Should().NotBeNull("create_auth 应经 suiteId → appKey 映射后换码落库");
        auth!.PermanentCode.Should().Be("perm-coord");
        (await host.Store.GetAsync(SuiteAppKey, AuthCorpId)).Should().BeNull(
            "suiteId 精确命中，不得写入其它应用");
    }

    [Fact]
    public async Task Coordinator_ShouldSkipExchange_WhenAutoExchangeDisabled()
    {
        var host = CreateHost(options => options.AutoExchangeAuthCode = false);
        using var provider = host.Provider;

        await host.Coordinator.OnAuthorizationSucceededAsync("dk-template-id", "auth-code-skip");

        host.ProviderAuth.Verify(
            m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()),
            Times.Never, "AutoExchangeAuthCode = false 时不应自动换码");
        (await host.Store.ListAsync(ProviderAppKey)).Should().BeEmpty("降级为宿主自行处理，不落库");
    }

    [Fact]
    public async Task Coordinator_ShouldWarn_WhenChangedAuthNotInStore()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var act = async () =>
            await host.Coordinator.OnAuthorizationChangedAsync("ww-suite-id", "unknown-corp");

        await act.Should().NotThrowAsync("本地无该授权记录时仅告警，不抛（回调端点须稳定响应）");
        host.ProviderAuth.Verify(
            m => m.GetAuthInfoV2Async(It.IsAny<GetAuthInfoRequest>(), It.IsAny<CancellationToken>()),
            Times.Never, "无本地授权记录时不做无谓的 get_auth_info 调用");
    }

    [Fact]
    public async Task Coordinator_ShouldRevokeOnlyMatchedAppKey_OnCancel()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        // 同一 authCorpId 在两个套件下是两份独立授权。
        await host.Store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = SuiteAppKey, AuthCorpId = AuthCorpId, PermanentCode = "pc-suite",
        });
        await host.Store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = ProviderAppKey, AuthCorpId = AuthCorpId, PermanentCode = "pc-provider",
        });

        await host.Coordinator.OnAuthorizationCanceledAsync("ww-suite-id", AuthCorpId);

        (await host.Store.GetAsync(SuiteAppKey, AuthCorpId)).Should().BeNull("归属应用应被清理");
        (await host.Store.GetAsync(ProviderAppKey, AuthCorpId)).Should().NotBeNull(
            "其它套件下的同 authCorpId 授权独立，不得被连带清理（R4）");
        host.AppManager.Verify(
            m => m.InvalidateTokenAsync(SuiteAppKey, It.IsAny<string>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()),
            Times.Once);
        host.AppManager.Verify(
            m => m.InvalidateTokenAsync(ProviderAppKey, It.IsAny<string>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ── P1-3 / P1-4：换码单飞门与结果记忆的复合键 ───────────────

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldNotShareFlight_WhenAppKeyDiffers()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                var n = Interlocked.Increment(ref calls);
                await gate.Task;
                return PermanentCodeResponse("perm-" + n);
            });

        // 同一 authCode 在两个 appKey 下并发换码（多套件/多代开发模板）。
        var suiteTask = host.Service.ExchangeAuthCodeAsync("auth-code-shared", SuiteAppKey);
        var providerTask = host.Service.ExchangeAuthCodeAsync("auth-code-shared", ProviderAppKey);

        gate.SetResult(true);
        var suiteAuth = await suiteTask;
        var providerAuth = await providerTask;

        calls.Should().Be(2, "P1-3：单飞门键必须含 appKey，否则第二个应用会静默复用第一个应用的换码结果");
        suiteAuth.AppKey.Should().Be(SuiteAppKey);
        providerAuth.AppKey.Should().Be(ProviderAppKey);
    }

    [Fact]
    public async Task ExchangeAuthCodeAsync_ShouldNotCancelSharedFlight_WhenFirstCallerCancels()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        host.ProviderAuth
            .Setup(m => m.GetPermanentCodeV2Async(It.IsAny<GetPermanentCodeRequest>(), It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await gate.Task;
                return PermanentCodeResponse("perm-shared");
            });

        using var cts = new CancellationTokenSource();
        var first = host.Service.ExchangeAuthCodeAsync("auth-code-cancel", SuiteAppKey, cts.Token);
        var second = host.Service.ExchangeAuthCodeAsync("auth-code-cancel", SuiteAppKey);

        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => first);

        gate.SetResult(true);

        // P1-3：共享任务以 CancellationToken.None 承载，首个调用者取消不得连带取消其它等待者。
        var auth = await second;
        auth.AuthCorpId.Should().Be(AuthCorpId);
    }

    // ── P1-5：撤销的「先失效、后删库」顺序 ──────────────────────

    [Fact]
    public async Task Revoke_ShouldKeepAuthorization_WhenInvalidateFails()
    {
        var host = CreateHost();
        using var provider = host.Provider;

        await host.Store.SetAsync(new WechatCorpAuthorization
        {
            AppKey = SuiteAppKey, AuthCorpId = AuthCorpId, PermanentCode = "pc-keep",
        });

        host.AppManager
            .Setup(m => m.InvalidateTokenAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("invalidate boom"));

        var act = () => host.Service.RevokeAuthorizationAsync(AuthCorpId, SuiteAppKey);
        await act.Should().ThrowAsync<InvalidOperationException>();

        (await host.Store.GetAsync(SuiteAppKey, AuthCorpId)).Should().NotBeNull(
            "P1-5：先失效后删库 —— 失效失败时授权记录保持完整，调用方可直接重试（旧实现会留下记录已删、令牌可用的不一致）");
    }

    // ── DI 装配与生存期 ─────────────────────────────────────────

    [Fact]
    public void AddWechatWorkServices_ShouldRegisterAuthorizationService_WithoutCaptiveDependency()
    {
        // ValidateScopes=true：若单例编排服务捕获了 Scoped 依赖，解析即抛。
        var host = CreateHost(validateScopes: true);
        using var provider = host.Provider;

        provider.GetRequiredService<IWechatWorkAuthorizationService>().Should().NotBeNull();
        var coordinator = provider.GetRequiredService<IWechatAuthorizationCoordinator>();
        coordinator.Should().NotBeNull();

        using var scope = provider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IWechatWorkAuthorizationService>()
            .Should().BeSameAs(host.Service, "编排服务为 Singleton，跨 scope 复用同一实例");
        scope.ServiceProvider.GetRequiredService<IWechatAuthorizationCoordinator>()
            .Should().BeSameAs(coordinator, "授权协调器为 Singleton，跨 scope 复用同一实例");
    }
}

/// <summary><see cref="WechatAuthorizationOptions"/> 边界值校验测试。</summary>
public class WechatAuthorizationOptionsTests
{
    [Fact]
    public void Validate_ShouldAcceptDefaults()
    {
        var act = () => new WechatAuthorizationOptions().Validate();

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(59)]
    [InlineData(3601)]
    public void Validate_ShouldRejectAuthCodeMemoTtlOutOfRange(int ttl)
    {
        var options = new WechatAuthorizationOptions { AuthCodeMemoTtlSeconds = ttl };

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*AuthCodeMemoTtlSeconds*");
    }

    [Theory]
    [InlineData("http://host.example.com/cb")]
    [InlineData("/callback")]
    [InlineData("not-a-uri")]
    public void Validate_ShouldRejectNonAbsoluteHttpsRedirectUri(string redirectUri)
    {
        var options = new WechatAuthorizationOptions { AuthorizationRedirectUri = redirectUri };

        var act = () => options.Validate();

        act.Should().Throw<InvalidOperationException>().WithMessage("*AuthorizationRedirectUri*");
    }

    [Fact]
    public void Validate_ShouldAcceptAbsoluteHttpsRedirectUri()
    {
        var options = new WechatAuthorizationOptions
        {
            AuthorizationRedirectUri = "https://host.example.com/callback",
        };

        var act = () => options.Validate();

        act.Should().NotThrow();
    }
}