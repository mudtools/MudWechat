// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;

namespace Mud.Wechat.Ads.Tests.Auth;

/// <summary>
/// <see cref="AdsAuthorizationService"/> 的令牌链测试，核心是 <b>ADS-B3：refresh_token 一次性语义</b>
/// 与「<b>先删库、后抛异常</b>」的顺序契约。
/// </summary>
/// <remarks>
/// <para>
/// <b>顺序为什么可测</b>：本类每条失败路径都是 <c>await store.RemoveAsync(...)</c> 之后紧接 <c>throw</c>。
/// 若顺序写成先抛后删，删除语句永不可达 ⇒ 记录里不会出现 remove 事件。
/// 因此「remove 是最后一个存储事件」本身就是顺序证据，而不只是「最终库空」的证据（后者两种写法都能满足）。
/// </para>
/// <para>
/// <b>为什么必须删</b>：官方 <c>oauth/refresh_token</c> 页原文（2026-10-10 核验）——
/// 「获得新的 Refresh Token 及 Access Token，<b>原 Access Token 及 Refresh Token 会失效</b>」。
/// 刷新失败后留在库里的 refresh_token 已是废纸，不删则此后每次取令牌都撞一次无效刷新，
/// 既放大请求量又让「需要重新授权」这一事实被淹没在重复失败里。
/// </para>
/// <para>时钟经 <see cref="IAdsClock"/> 注入 ⇒ 「恰好过期」这类路径不需要 <c>Task.Delay</c> 或改系统时间。</para>
/// </remarks>
public class AdsAuthorizationServiceTests
{
    private const string AppKey = "default";
    private const string ClientId = "12345678";
    private const string ClientSecret = "super-secret-value-never-log";
    private const string RedirectUri = "https://example.com/callback";

    private readonly FixedClock _clock = new();
    private readonly RecordingStore _store = new();
    private readonly List<HttpRequestMessage> _requests = new();
    private readonly Func<HttpRequestMessage, CancellationToken, Task<AdsTokenResponse?>> _defaultResponder;
    private Func<HttpRequestMessage, CancellationToken, Task<AdsTokenResponse?>>? _responder;

    private readonly AdsAppManager _apps;
    private readonly AdsAuthorizationService _service;

    public AdsAuthorizationServiceTests()
    {
        _apps = new AdsAppManager(
            new List<AdsAppConfig>
            {
                new()
                {
                    AppKey = AppKey,
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    RedirectUri = RedirectUri,
                },
            },
            new AdsAppContextSwitcher());

        _defaultResponder = (_, _) => Task.FromResult<AdsTokenResponse?>(SuccessToken("at-1", "rt-1"));

        var http = new Mock<IAdsOAuthHttpClient>(MockBehavior.Strict);
        http.Setup(x => x.SendAsync<AdsTokenResponse>(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<object?>(),
                It.IsAny<CancellationToken>()))
            .Returns((HttpRequestMessage request, object? serializerOptions, CancellationToken cancellationToken) =>
            {
                _requests.Add(request);
                return (_responder ?? _defaultResponder)(request, cancellationToken);
            });

        _service = new AdsAuthorizationService(
            _apps, _store, http.Object, _clock, NullLogger<AdsAuthorizationService>.Instance);
    }

    /// <summary>库里有未过期令牌时直接给，<b>一个请求都不发</b>（令牌链的热路径）。</summary>
    /// <remarks>TTL 取 1 小时：必须明显大于 <c>TokenRefreshThreshold</c>（默认 300 秒），否则命中的是阈值提前刷新分支。</remarks>
    [Fact]
    public async Task GetAccessTokenAsync_ShouldReuseCachedToken_WhenStillUsable()
    {
        await SeedAsync(accessToken: "cached-token", accessTtlMs: 3_600_000, refreshToken: "rt");

        (await _service.GetAccessTokenAsync(AppKey)).Should().Be("cached-token");
        _requests.Should().BeEmpty();
    }

    /// <summary>
    /// 进入 <c>TokenRefreshThreshold</c> 提前量即刷新：官方对时间戳有 300 秒误差容忍，
    /// 「刚好卡在到期边界」的令牌若继续使用会在传输层被判过期。
    /// </summary>
    [Fact]
    public async Task GetAccessTokenAsync_ShouldRefresh_WhenTokenInsideThreshold()
    {
        var thresholdSeconds = _apps.GetRequiredApp(AppKey).TokenRefreshThreshold;
        await SeedAsync(
            accessToken: "about-to-expire",
            accessTtlMs: (thresholdSeconds - 1) * 1000L,
            refreshToken: "rt-current");

        (await _service.GetAccessTokenAsync(AppKey)).Should().Be("at-1");
        Path(0).Should().Be(AdsOAuthRoutes.RefreshToken);
    }

    /// <summary>未显式指定 AppKey 时按作用域/默认应用解析（与传输层取令牌同一条咽喉路径）。</summary>
    [Fact]
    public async Task GetAccessTokenAsync_ShouldResolveDefaultApp_WhenAppKeyIsNull()
    {
        await SeedAsync(accessToken: "cached-by-default", accessTtlMs: 3_600_000, refreshToken: "rt");

        (await _service.GetAccessTokenAsync(null)).Should().Be("cached-by-default");
    }

    /// <summary>
    /// <b>ADS-B3</b>：库里没有任何授权状态 ⇒ 无刷新能力。抛 <see cref="WechatAdsReauthorizationRequiredException"/>，
    /// 且<b>不发</b>任何网络请求、<b>不</b>伪造一次 remove（库里本就没有可删的东西）。
    /// </summary>
    [Fact]
    public async Task GetAccessTokenAsync_ShouldThrowReauthorization_WhenStoreHasNoState()
    {
        var act = () => _service.GetAccessTokenAsync(AppKey);

        var thrown = await act.Should().ThrowAsync<WechatAdsReauthorizationRequiredException>();
        thrown.Which.AppKey.Should().Be(AppKey);
        thrown.Which.ErrorCode.Should().Be(WechatAdsReauthorizationRequiredException.ReauthorizationRequiredSentinel);

        _requests.Should().BeEmpty();
        _store.EventList.Should().NotContain(static e => e.StartsWith("remove", StringComparison.Ordinal));
    }

    /// <summary>
    /// <b>ADS-B3 顺序契约</b>：有状态但没有 refresh_token ⇒ 残留状态必须先删（remove 是最后一个存储事件）
    /// 再抛；顺序颠倒时删除语句不可达，本断言即红。
    /// </summary>
    [Fact]
    public async Task GetAccessTokenAsync_ShouldRemoveStateBeforeThrowing_WhenRefreshTokenMissing()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: null);

        var act = () => _service.GetAccessTokenAsync(AppKey);
        await act.Should().ThrowAsync<WechatAdsReauthorizationRequiredException>();

        _store.EventList.Last().Should().Be($"remove:{AppKey}");
        (await _store.GetAsync(AppKey)).Should().BeNull();
    }

    /// <summary>
    /// <b>ADS-B3</b>：refresh_token 自身已过期 ⇒ 同样先删后抛，且<b>不</b>尝试刷新
    /// （向官方发一个必然被拒的请求，等于替调用方多消耗一次失败计数）。
    /// </summary>
    [Fact]
    public async Task RefreshAsync_ShouldRemoveStateAndThrow_WhenRefreshTokenExpired()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-stale", refreshTtlMs: -1);

        var act = () => _service.RefreshAsync(AppKey);
        var thrown = await act.Should().ThrowAsync<WechatAdsReauthorizationRequiredException>();
        thrown.Which.Message.Should().Contain("refresh_token 已过期");

        _requests.Should().BeEmpty();
        _store.EventList.Last().Should().Be($"remove:{AppKey}");
        (await _store.GetAsync(AppKey)).Should().BeNull();
    }

    /// <summary>
    /// <b>ADS-B3</b>：官方拒绝刷新（应答 <c>code != 0</c>）⇒ 旧 refresh_token 已不可信，先删后抛，
    /// 并把官方 <c>code</c> 透传到异常上（宿主据此区分「令牌作废」与「配额耗尽」）。
    /// </summary>
    [Fact]
    public async Task RefreshAsync_ShouldRemoveStateAndCarryOfficialCode_WhenOfficialRejects()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 12345,
            Message = "invalid refresh token",
            MessageCn = "刷新令牌无效",
        });

        var act = () => _service.RefreshAsync(AppKey);
        var thrown = await act.Should().ThrowAsync<WechatAdsReauthorizationRequiredException>();

        thrown.Which.ErrorCode.Should().Be(12345);
        thrown.Which.AppKey.Should().Be(AppKey);
        _store.EventList.Last().Should().Be($"remove:{AppKey}");
        (await _store.GetAsync(AppKey)).Should().BeNull();
    }

    /// <summary>
    /// <b>传输层空应答 ≠ 官方拒绝</b>：null 应答（组件未得到可解析应答体）意味着官方是否已处理请求不可知
    /// ⇒ 一次性 refresh_token 未被确认消耗，抛 <see cref="InvalidOperationException"/>（传输层失败）
    /// 且<b>不删</b>授权状态 —— 删库会把一次可重试的传输抖动升级为强制人工重新授权。
    /// </summary>
    /// <remarks>2026-10-10 修复：此前 null 应答经 ThrowIfFailed 的哨兵（ErrorCode=-1）被
    /// <c>catch (WechatAdsException)</c> 误判为「官方拒绝」而删库；GetAsync 现已把 null 应答
    /// 转成 InvalidOperationException，与官方拒绝在异常类型上区分。</remarks>
    [Fact]
    public async Task RefreshAsync_ShouldKeepState_AndThrowTransportError_WhenResponseIsNull()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-live");
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(null);

        var act = () => _service.RefreshAsync(AppKey);

        (await act.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("可解析的应答");

        _store.EventList.Should().NotContain(static e => e.StartsWith("remove", StringComparison.Ordinal),
            "官方未确认拒绝 ⇒ refresh_token 未被消耗 ⇒ 不得删状态");
        _store.EventList.Should().NotContain(static e => e.StartsWith("set", StringComparison.Ordinal));
    }

    /// <summary>
    /// 刷新端点若<b>少回</b>任一支令牌 ⇒ 无法安全落库（旧值已被官方作废），同样按 ADS-B3 先删后抛。
    /// </summary>
    [Fact]
    public async Task RefreshAsync_ShouldRemoveStateAndThrow_WhenResponseLacksRefreshToken()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 0,
            Data = new AdsTokenData { AccessToken = "at-new" },
        });

        var act = () => _service.RefreshAsync(AppKey);
        var thrown = await act.Should().ThrowAsync<WechatAdsReauthorizationRequiredException>();
        thrown.Which.Message.Should().Contain("缺少 access_token 或 refresh_token");

        _store.EventList.Last().Should().Be($"remove:{AppKey}");
        (await _store.GetAsync(AppKey)).Should().BeNull();
    }

    /// <summary>
    /// 刷新成功 ⇒ 落库的是<b>整对</b>新令牌，绝对过期时刻由时钟换算（<c>*_expires_in</c> 是时长不是时刻），
    /// 且身份字段原样继承（刷新端点<b>不返回</b> <c>authorizer_info</c>）。
    /// </summary>
    [Fact]
    public async Task RefreshAsync_ShouldRotateBothTokensAndKeepIdentity_WhenSucceeded()
    {
        var seeded = await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 0,
            Data = new AdsTokenData
            {
                AccessToken = "at-new",
                RefreshToken = "rt-new",
                AccessTokenExpiresIn = 86400,
                RefreshTokenExpiresIn = 2592000,
                // 刷新端点实际不返回以下三项；本用例正是要证明「即使应答带了身份也不影响继承旧值」这条链路。
                AuthorizerInfo = new AdsAuthorizerInfo { AccountId = 0 },
            },
        });

        var rotated = await _service.RefreshAsync(AppKey);
        var nowMs = _clock.UtcNow.ToUnixTimeMilliseconds();

        // 顺序断言取「完整事件序列」而非「最后一个事件」：末事件断言会被后续诊断读（get）污染，
        // 而全序列同时锁死了「锁后读存储 → 网络 → 整对写穿」这一唯一合法形态。
        _store.EventList.Should().Equal(new[] { $"get:{AppKey}", $"set:{AppKey}" },
            "刷新必须锁后读一次、整对写一次；漏写穿即重启后失去新 refresh_token");

        rotated.AccessToken.Should().Be("at-new");
        rotated.RefreshToken.Should().Be("rt-new");
        rotated.AccessTokenExpireAtMs.Should().Be(nowMs + 86400_000L);
        rotated.RefreshTokenExpireAtMs.Should().Be(nowMs + 2592000_000L);
        rotated.AccountId.Should().Be(seeded.AccountId, "刷新端点不返回 authorizer_info，身份必须继承");
        rotated.AccountUin.Should().Be(seeded.AccountUin);
        rotated.WechatAccountId.Should().Be(seeded.WechatAccountId);
        rotated.AccountRoleType.Should().Be(seeded.AccountRoleType);
        rotated.ScopeList.Should().Equal(seeded.ScopeList!, "权限列表随首次授权落库，刷新不得清空");

        var stored = await _store.GetAsync(AppKey);
        stored!.RefreshToken.Should().Be("rt-new", "一次性 refresh_token 必须写穿存储，否则重启即失去新值");
    }

    /// <summary>官方应答未给出 <c>*_expires_in</c> 时回落官方示例值（86400 / 2592000），而不是「立刻过期」。</summary>
    [Fact]
    public async Task RefreshAsync_ShouldFallBackToOfficialSampleLifetimes_WhenExpiresInOmitted()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 0,
            Data = new AdsTokenData { AccessToken = "at-new", RefreshToken = "rt-new" },
        });

        var rotated = await _service.RefreshAsync(AppKey);
        var nowMs = _clock.UtcNow.ToUnixTimeMilliseconds();

        rotated.AccessTokenExpireAtMs.Should().Be(nowMs + 86400_000L);
        rotated.RefreshTokenExpireAtMs.Should().Be(nowMs + 2592000_000L);
    }

    /// <summary>刷新请求的形态（ADS-B2）：<b>GET</b> <c>/oauth/refresh_token</c>，参数全进 Query，<b>不带</b> <c>grant_type</c>。</summary>
    [Fact]
    public async Task RefreshAsync_ShouldIssueGetWithQueryOnly()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");

        await _service.RefreshAsync(AppKey);

        var request = _requests.Single();
        request.Method.Should().Be(HttpMethod.Get);
        Path(0).Should().Be(AdsOAuthRoutes.RefreshToken);
        request.Content.Should().BeNull("官方 curl 用 -G -d ⇒ 参数进 Query，不是请求体");
        Query(request)["client_id"].Should().Be(ClientId);
        Query(request)["refresh_token"].Should().Be("rt-current");
        Query(request).Should().ContainKey("client_secret");
        Query(request).Should().NotContainKeys("grant_type", "access_token", "timestamp", "nonce");
    }

    /// <summary>取令牌链路只经过 OAuth 客户端 ⇒ 业务客户端（带令牌 Handler）不会被卷进刷新，避免「刷新请求自己带过期令牌」。</summary>
    [Fact]
    public async Task RefreshAsync_ShouldUseRefreshRouteNotTokenRoute()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");

        await _service.RefreshAsync(AppKey);

        Path(0).Should().Be(AdsOAuthRoutes.RefreshToken,
            "SDK 的刷新策略只用 oauth/refresh_token（语义完整），不提供 oauth/token?grant_type=refresh_token 入口");
    }

    /// <summary>
    /// <b>单飞 + 锁后复查</b>：两个并发取令牌只放行一次真实刷新。
    /// 用 <see cref="TaskCompletionSource{TResult}"/> 钉住时序（第一个请求悬挂期间第二个必然排在闸上），
    /// 因此不是靠延时赌竞态。
    /// </summary>
    [Fact]
    public async Task GetAccessTokenAsync_ShouldRefreshOnce_WhenCallersRaceOnExpiredToken()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");

        var blocker = new TaskCompletionSource<AdsTokenResponse?>(TaskCreationOptions.RunContinuationsAsynchronously);
        _responder = (_, _) => blocker.Task;

        var first = _service.GetAccessTokenAsync(AppKey);
        var second = _service.GetAccessTokenAsync(AppKey);

        blocker.SetResult(SuccessToken("at-raced", "rt-raced"));

        var tokens = await Task.WhenAll(first, second);

        tokens.Should().Equal("at-raced", "at-raced");
        _requests.Should().HaveCount(1,
            "一次性 refresh_token 被消耗两次必然有一次失败；每 AppKey 一把闸 + 锁后复查保证只发一次");
    }

    /// <summary>换码请求的形态（ADS-B2）：<b>GET</b> <c>/oauth/token</c> + <c>grant_type=authorization_code</c>，且不带 <c>access_token</c>。</summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldIssueGetToTokenRoute()
    {
        var state = await _service.ExchangeAuthorizationCodeAsync(AppKey, "auth-code-1");

        var request = _requests.Single();
        request.Method.Should().Be(HttpMethod.Get);
        Path(0).Should().Be(AdsOAuthRoutes.Token);
        request.RequestUri!.GetLeftPart(UriPartial.Authority).Should().Be("https://api.e.qq.com");
        Query(request)["grant_type"].Should().Be(AdsOAuthRoutes.GrantTypeAuthorizationCode);
        Query(request)["authorization_code"].Should().Be("auth-code-1");
        Query(request)["client_id"].Should().Be(ClientId);
        Query(request)["redirect_uri"].Should().Be(RedirectUri);
        Query(request).Should().NotContainKeys("access_token", "timestamp", "nonce");

        state.AccountId.Should().Be(87654321);
        state.ScopeList.Should().Equal(new[] { "account_management", "ads_management" }, "身份字段来自 authorizer_info");
        _store.EventList.Last().Should().Be($"set:{AppKey}");
    }

    /// <summary>入参 <c>redirectUri</c> 优先于应用配置（同一 client_id 挂多个回调地址的宿主需要它）。</summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldPreferExplicitRedirectUri()
    {
        await _service.ExchangeAuthorizationCodeAsync(AppKey, "code-r", "https://other.example.com/cb");

        Query(_requests.Single())["redirect_uri"].Should().Be("https://other.example.com/cb");
    }

    /// <summary><c>redirect_uri</c> 官方为选填 ⇒ 两处都留空时整键不上送（送空串与不送在部分网关上语义不同）。</summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldOmitBlankRedirectUri()
    {
        var appsWithoutRedirect = new AdsAppManager(
            new List<AdsAppConfig> { new() { AppKey = AppKey, ClientId = ClientId, ClientSecret = ClientSecret } },
            new AdsAppContextSwitcher());
        var captured = new List<HttpRequestMessage>();
        var http = new Mock<IAdsOAuthHttpClient>(MockBehavior.Strict);
        http.Setup(x => x.SendAsync<AdsTokenResponse>(
                It.IsAny<HttpRequestMessage>(), It.IsAny<object?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HttpRequestMessage request, object? _, CancellationToken _) =>
            {
                captured.Add(request);
                return SuccessToken("at-1", "rt-1");
            });

        var service = new AdsAuthorizationService(appsWithoutRedirect, _store, http.Object, _clock, null);
        await service.ExchangeAuthorizationCodeAsync(AppKey, "code-2");

        Query(captured.Single()).Should().NotContainKeys("redirect_uri");
    }

    /// <summary>
    /// <b>结果记忆</b>：授权码是一次性凭据，已有可用令牌时不得再打网络请求
    /// （第二次换取必被官方拒，并把一次本可成功的授权变成一次失败）。
    /// </summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldSkipNetwork_WhenUsableStateAlreadyStored()
    {
        await SeedAsync(accessToken: "at-existing", accessTtlMs: 60_000, refreshToken: "rt-existing");

        var state = await _service.ExchangeAuthorizationCodeAsync(AppKey, "auth-code-late");

        state.AccessToken.Should().Be("at-existing");
        _requests.Should().BeEmpty();
    }

    /// <summary>
    /// 授权码流程官方<b>恒</b>返回 refresh_token。缺失属必须点名的形态异常 ⇒ 不得静默落库一份无刷新能力的状态
    /// （那会把问题推到 access 过期那一刻）。
    /// </summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldThrowAndNotPersist_WhenResponseLacksRefreshToken()
    {
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 0,
            Data = new AdsTokenData { AccessToken = "at-only" },
        });

        var act = () => _service.ExchangeAuthorizationCodeAsync(AppKey, "auth-code-3");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*缺少 refresh_token*");

        (await _store.GetAsync(AppKey)).Should().BeNull();
    }

    /// <summary>官方拒绝换码（<c>code != 0</c>）⇒ 抛 <see cref="WechatAdsException"/>（<b>不是</b>「重新授权」类，宿主可退避重试）。</summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldThrowBusinessException_WhenOfficialRejects()
    {
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 11016,
            Message = "invalid code",
        });

        var act = () => _service.ExchangeAuthorizationCodeAsync(AppKey, "used-code");
        var thrown = await act.Should().ThrowAsync<WechatAdsException>();

        thrown.Which.Should().NotBeOfType<WechatAdsReauthorizationRequiredException>();
        thrown.Which.ErrorCode.Should().Be(11016);
        (await _store.GetAsync(AppKey)).Should().BeNull();
    }

    /// <summary>空白授权码是调用方编程错误，必须在发起网络请求前拒。</summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldThrowArgument_WhenCodeIsBlank()
    {
        var act = () => _service.ExchangeAuthorizationCodeAsync(AppKey, "   ");

        await act.Should().ThrowAsync<ArgumentNullException>();
        _requests.Should().BeEmpty();
    }

    /// <summary>未注册的 AppKey 必须点名（列出已注册键），而非裸 <c>KeyNotFoundException</c>。</summary>
    [Fact]
    public async Task RefreshAsync_ShouldNameRegisteredKeys_WhenAppKeyUnknown()
    {
        var act = () => _service.RefreshAsync("not-registered");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not-registered*");
        _requests.Should().BeEmpty();
    }

    /// <summary>
    /// <b>ADS-B5 红线</b>：<c>ToString()</c> 只输出存在性与剩余时长，不含任何令牌明文 ——
    /// 本方法是宿主把授权状态写进日志时的唯一出口。
    /// </summary>
    [Fact]
    public async Task AdsAuthorizationState_ShouldNeverPrintTokenValues()
    {
        await SeedAsync(
            accessToken: "super-secret-access-token",
            accessTtlMs: 60_000,
            refreshToken: "super-secret-refresh-token");

        var text = (await _store.GetAsync(AppKey))!.ToString();

        text.Should().NotContain("super-secret-access-token");
        text.Should().NotContain("super-secret-refresh-token");
        text.Should().Contain("AccessToken=present").And.Contain("RefreshToken=present");
    }

    /// <summary>刷新失败的异常消息里不得出现 refresh_token / client_secret 明文（异常消息同样在泄漏面上）。</summary>
    [Fact]
    public async Task ReauthorizationException_ShouldNotContainCredentials()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "secret-refresh-value");
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 12345,
            Message = "invalid refresh token",
        });

        var act = () => _service.RefreshAsync(AppKey);
        var thrown = await act.Should().ThrowAsync<WechatAdsReauthorizationRequiredException>();

        thrown.Which.Message.Should().NotContain("secret-refresh-value");
        thrown.Which.Message.Should().NotContain(ClientSecret);
    }

    /// <summary>配置对象的 <c>ToString()</c> 不得带出 <c>client_secret</c>（同一条红线的配置侧落点）。</summary>
    [Fact]
    public void AdsAppConfig_ShouldNotPrintClientSecret()
    {
        _apps.GetRequiredApp(AppKey).ToString().Should().NotContain(ClientSecret);
    }

    /// <summary>官方未给出 <c>access_token</c> 但给出 <c>data</c> ⇒ 换码期即点名，不留半残状态。</summary>
    [Fact]
    public async Task ExchangeAuthorizationCodeAsync_ShouldThrow_WhenResponseLacksAccessToken()
    {
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse
        {
            Code = 0,
            Data = new AdsTokenData { RefreshToken = "rt-only" },
        });

        var act = () => _service.ExchangeAuthorizationCodeAsync(AppKey, "auth-code-4");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*缺少 access_token*");
    }

    /// <summary>应答整体缺 <c>data</c> 载荷 ⇒ 抛出点名的 <c>InvalidOperationException</c>（而非 NullReferenceException）。</summary>
    [Fact]
    public async Task RefreshAsync_ShouldThrow_WhenResponseHasNoDataPayload()
    {
        await SeedAsync(accessToken: "expired", accessTtlMs: -1, refreshToken: "rt-current");
        _responder = (_, _) => Task.FromResult<AdsTokenResponse?>(new AdsTokenResponse { Code = 0 });

        var act = () => _service.RefreshAsync(AppKey);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*缺少 data 载荷*");
    }

    private async Task<AdsAuthorizationState> SeedAsync(
        string accessToken,
        long accessTtlMs,
        string? refreshToken,
        long refreshTtlMs = 2_592_000_000L)
    {
        var nowMs = _clock.UtcNow.ToUnixTimeMilliseconds();
        var state = new AdsAuthorizationState
        {
            AppKey = AppKey,
            AccessToken = accessToken,
            AccessTokenExpireAtMs = nowMs + accessTtlMs,
            RefreshToken = refreshToken,
            RefreshTokenExpireAtMs = nowMs + refreshTtlMs,
            AccountId = 87654321,
            AccountUin = 10001,
            WechatAccountId = "wxid-1",
            AccountRoleType = "ACCOUNT_ROLE_TYPE_AGENCY",
            ScopeList = new[] { "account_management", "ads_management" },
        };

        await _store.SetAsync(AppKey, state);
        _store.Clear();
        return state;
    }

    private static AdsTokenResponse SuccessToken(string accessToken, string refreshToken) => new()
    {
        Code = 0,
        Message = string.Empty,
        MessageCn = string.Empty,
        Data = new AdsTokenData
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresIn = AdsAuthorizationService.DefaultAccessTokenLifetimeSeconds,
            RefreshTokenExpiresIn = AdsAuthorizationService.DefaultRefreshTokenLifetimeSeconds,
            AuthorizerInfo = new AdsAuthorizerInfo
            {
                AccountId = 87654321,
                AccountUin = 10001,
                WechatAccountId = "wxid-1",
                AccountRoleType = "ACCOUNT_ROLE_TYPE_AGENCY",
                ScopeList = new List<string> { "account_management", "ads_management" },
            },
        },
    };

    private string Path(int index) => _requests[index].RequestUri!.AbsolutePath;

    /// <summary>手工解析 Query（不引额外包：本仓测试面在四档 TFM 与 net8 单档下都要成立）。</summary>
    private static Dictionary<string, string> Query(HttpRequestMessage request)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var query = request.RequestUri!.Query;
        if (string.IsNullOrEmpty(query))
        {
            return result;
        }

        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            var key = separator < 0 ? pair : pair[..separator];
            var value = separator < 0 ? string.Empty : pair[(separator + 1)..];
            result[Uri.UnescapeDataString(key)] = Uri.UnescapeDataString(value);
        }

        return result;
    }

    /// <summary>可控时钟：让「恰好过期」成为可注入的输入而非延时。</summary>
    private sealed class FixedClock : IAdsClock
    {
        public DateTimeOffset UtcNow { get; } = new(2026, 10, 10, 12, 0, 0, TimeSpan.Zero);
    }

    /// <summary>
    /// 记录调用事件的存储包装：内部用真实 <see cref="InMemoryWechatAdsAuthorizationStore"/>，
    /// 因此「删除后确实读不到」由实现本身保证，本类只追加事件序列用于<b>顺序</b>断言。
    /// </summary>
    private sealed class RecordingStore : IWechatAdsAuthorizationStore
    {
        private readonly InMemoryWechatAdsAuthorizationStore _inner = new();
        private readonly ConcurrentQueue<string> _events = new();

        public List<string> EventList => _events.ToList();

        public Task<AdsAuthorizationState?> GetAsync(string appKey, CancellationToken cancellationToken = default)
        {
            _events.Enqueue($"get:{appKey}");
            return _inner.GetAsync(appKey, cancellationToken);
        }

        public async Task SetAsync(string appKey, AdsAuthorizationState state, CancellationToken cancellationToken = default)
        {
            _events.Enqueue($"set:{appKey}");
            await _inner.SetAsync(appKey, state, cancellationToken);
        }

        public async Task RemoveAsync(string appKey, CancellationToken cancellationToken = default)
        {
            _events.Enqueue($"remove:{appKey}");
            await _inner.RemoveAsync(appKey, cancellationToken);
        }

        public void Clear()
        {
            while (!_events.IsEmpty)
            {
                _events.TryDequeue(out _);
            }
        }
    }
}
