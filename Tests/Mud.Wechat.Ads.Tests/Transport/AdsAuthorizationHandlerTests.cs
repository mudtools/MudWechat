// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Tests.Transport;

/// <summary>
/// <see cref="AdsAuthorizationHandler"/> 的全局参数注入测试（<b>ADS-B1</b> 的行为面证据）。
/// </summary>
/// <remarks>
/// <para>
/// 官方 v3.0「全局参数」（2026-10-10 逐页核验）要求每个业务请求同时携带
/// <c>access_token</c> + <c>timestamp</c>（秒级，允许误差 300 秒）+ <c>nonce</c>（≤32 字节、全局唯一），
/// 且三项必须<b>同源于同一次取令牌</b> —— 这正是本线不用声明式 <c>[Token]</c> 的原因（守卫 ADS-B1）。
/// 本类断言的就是「成组」「同源」「不覆盖」三件事。
/// </para>
/// <para>
/// <see cref="DelegatingHandler.InnerHandler"/> 的 setter 是 protected ⇒ 用派生类把它开放出来，
/// 从而无需真实网络即可拿到「离开 Handler 的请求」。
/// </para>
/// </remarks>
public class AdsAuthorizationHandlerTests
{
    private static readonly DateTimeOffset FixedNow = new(2026, 10, 10, 12, 0, 0, TimeSpan.Zero);

    /// <summary>三项全局参数一次写齐，且 <c>timestamp</c> 取注入时钟的<b>秒</b>值（不做时区换算）。</summary>
    [Fact]
    public async Task SendAsync_ShouldAppendAccessTokenTimestampAndNonce_WhenRequestHasNone()
    {
        var fixture = new Fixture(accessToken: "at-from-provider");

        var sent = await fixture.SendAsync("https://api.e.qq.com/v3.0/advertiser/get?account_id=1");

        var query = ParseQuery(sent);
        query["access_token"].Should().Be("at-from-provider");
        query["timestamp"].Should().Be(FixedNow.ToUnixTimeSeconds().ToString());
        query["nonce"].Should().HaveLength(32);
        query["account_id"].Should().Be("1", "注入是追加，不得吞掉调用方已写的参数");
    }

    /// <summary>令牌与 <c>timestamp</c>/<c>nonce</c> 必须来自<b>同一次</b> SendAsync 调用（nonce 每次新、令牌每次现取）。</summary>
    [Fact]
    public async Task SendAsync_ShouldTakeTokenPerRequest_AndUseFreshNonceEachTime()
    {
        var fixture = new Fixture(accessToken: "at");

        var first = ParseQuery(await fixture.SendAsync("https://api.e.qq.com/v3.0/advertiser/get"));
        var second = ParseQuery(await fixture.SendAsync("https://api.e.qq.com/v3.0/advertiser/get"));

        fixture.TokenCalls.Should().Be(2, "令牌取用端口是每次现取（命中缓存由服务侧负责，Handler 不做二次缓存）");
        first["nonce"].Should().NotBe(second["nonce"], "官方要求 nonce 全局唯一，复用即重放");
    }

    /// <summary>nonce 必须是纯十六进制且恰好 32 字节（官方上限 32 字节，全 ASCII ⇒ 字符数=字节数）。</summary>
    [Fact]
    public void CreateNonce_ShouldBeThirtyTwoHexCharacters()
    {
        var nonce = AdsAuthorizationHandler.CreateNonce();

        nonce.Should().HaveLength(32);
        nonce.Should().MatchRegex("^[0-9a-f]{32}$");
        Encoding.UTF8.GetByteCount(nonce).Should().Be(32, "官方上限以字节计，非 ASCII 形态会越界");
    }

    /// <summary>
    /// <b>幂等放行</b>：请求已自带 <c>access_token</c> 时三项<b>整体</b>不注入。
    /// 覆盖会静默替换调用方显式给出的令牌；追加会产生同名双参数而官方网关取哪一支未定义 ⇒ 留原样。
    /// </summary>
    [Fact]
    public async Task SendAsync_ShouldLeaveRequestUntouched_WhenAccessTokenAlreadyPresent()
    {
        var fixture = new Fixture(accessToken: "should-not-be-used");

        var uri = "https://api.e.qq.com/v3.0/advertiser/get?access_token=host-supplied&account_id=1";
        var sent = await fixture.SendAsync(uri);

        sent.RequestUri!.Query.Should().Be("?access_token=host-supplied&account_id=1");
        fixture.TokenCalls.Should().Be(0, "宿主自管令牌的入口必须明确，不能被 SDK 抢先取一次令牌");
    }

    /// <summary>参数名判定按<b>段首</b>精确匹配：值里出现 <c>access_token</c> 子串不算「已带令牌」。</summary>
    [Fact]
    public async Task SendAsync_ShouldStillInject_WhenAccessTokenOnlyAppearsAsValue()
    {
        var fixture = new Fixture(accessToken: "at");

        var sent = ParseQuery(await fixture.SendAsync(
            "https://api.e.qq.com/v3.0/advertiser/get?memo=access_token"));

        sent.Should().ContainKey("access_token");
        sent["memo"].Should().Be("access_token");
    }

    /// <summary>应用作用域决定用哪个 <c>client_id</c> 的令牌：进入 <c>UseApp</c> 后必须把该键交给取令牌端口。</summary>
    [Fact]
    public async Task SendAsync_ShouldPassCurrentAppKeyToTokenProvider()
    {
        var switcher = new AdsAppContextSwitcher();
        var fixture = new Fixture(accessToken: "at", switcher: switcher);

        using (switcher.UseApp("app-b"))
        {
            await fixture.SendAsync("https://api.e.qq.com/v3.0/advertiser/get");
        }

        fixture.SeenAppKeys.Should().Equal("app-b");
    }

    /// <summary>未处于任何作用域时传 <c>null</c>（由取令牌端口回落默认应用），而非猜测一个键名。</summary>
    [Fact]
    public async Task SendAsync_ShouldPassNullAppKey_OutsideAnyScope()
    {
        var fixture = new Fixture(accessToken: "at");

        await fixture.SendAsync("https://api.e.qq.com/v3.0/advertiser/get");

        fixture.SeenAppKeys.Should().Equal(new string?[] { null }, "回落默认应用的决策留在服务侧");
    }

    /// <summary>缺 <c>RequestUri</c> 无处写入全局参数 ⇒ 抛明确错误而不是 <c>NullReferenceException</c>。</summary>
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenRequestUriMissing()
    {
        var fixture = new Fixture(accessToken: "at");

        var act = () => fixture.SendAsync(requestUri: null);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*缺少 RequestUri*");
    }

    /// <summary>取令牌失败（需要重新授权）必须原样上抛，不能被 Handler 吞成一次「无令牌的请求」。</summary>
    [Fact]
    public async Task SendAsync_ShouldPropagateReauthorization_WhenProviderRefuses()
    {
        var provider = new Mock<IAdsAccessTokenProvider>(MockBehavior.Strict);
        provider.Setup(x => x.GetAccessTokenAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WechatAdsReauthorizationRequiredException("default", "无 refresh_token"));
        var fixture = new Fixture(provider: provider.Object);

        var act = () => fixture.SendAsync("https://api.e.qq.com/v3.0/advertiser/get");

        await act.Should().ThrowAsync<WechatAdsReauthorizationRequiredException>();
        fixture.InnerRequest.Should().BeNull("令牌缺失时不得把请求放上网");
    }

    /// <summary>POST 请求体不受注入影响（只改 URI）。</summary>
    [Fact]
    public async Task SendAsync_ShouldKeepRequestContent_ForPost()
    {
        var fixture = new Fixture(accessToken: "at");

        var sent = await fixture.SendAsync(
            "https://api.e.qq.com/v3.0/advertiser/update",
            HttpMethod.Post,
            new StringContent("{\"account_id\":1}", Encoding.UTF8, "application/json"));

        sent.Content.Should().NotBeNull();
        (await sent.Content!.ReadAsStringAsync()).Should().Be("{\"account_id\":1}");
        ParseQuery(sent).Should().ContainKey("nonce");
    }

    private sealed class Fixture
    {
        private readonly Mock<IAdsAccessTokenProvider> _provider = new(MockBehavior.Strict);
        private readonly StubInnerHandler _inner = new();
        private readonly AdsAuthorizationHandler _handler;

        internal Fixture(
            string accessToken = "at",
            IAdsAppContextSwitcher? switcher = null,
            IAdsAccessTokenProvider? provider = null)
        {
            var tokenProvider = provider;
            if (tokenProvider is null)
            {
                _provider.Setup(x => x.GetAccessTokenAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((string? appKey, CancellationToken _) =>
                    {
                        SeenAppKeys.Add(appKey);
                        TokenCalls++;
                        return accessToken;
                    });
                tokenProvider = _provider.Object;
            }

            _handler = new AdsAuthorizationHandler(
                tokenProvider, switcher ?? new AdsAppContextSwitcher(), new StubClock());
            SetInnerHandler(_handler, _inner);
        }

        public int TokenCalls { get; private set; }

        public List<string?> SeenAppKeys { get; } = new();

        public HttpRequestMessage? InnerRequest => _inner.Captured;

        public async Task<HttpRequestMessage> SendAsync(
            string? requestUri,
            HttpMethod? method = null,
            HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method ?? HttpMethod.Get, requestUri);
            if (content is not null)
            {
                request.Content = content;
            }

            using var invoker = new HttpMessageInvoker(_handler, disposeHandler: false);
            var response = await invoker.SendAsync(request, CancellationToken.None);
            response.EnsureSuccessStatusCode();

            return _inner.Captured ?? request;
        }

        /// <summary>
        /// <see cref="DelegatingHandler.InnerHandler"/> 的 setter 是 protected，而
        /// <see cref="AdsAuthorizationHandler"/> 是 <c>internal sealed</c>（不可派生开放它）⇒ 反射设一次。
        /// 这不是「绕过实现」，只是将 Handler 接上一个记录用的末端 Handler。
        /// </summary>
        private static void SetInnerHandler(DelegatingHandler handler, HttpMessageHandler inner)
        {
            var setter = typeof(DelegatingHandler)
                .GetProperty("InnerHandler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                .GetSetMethod(nonPublic: true)!;
            setter.Invoke(handler, new object[] { inner });
        }

        /// <summary>末端 Handler：只记录到达的请求并回 200（不发真实网络）。</summary>
        private sealed class StubInnerHandler : HttpMessageHandler
        {
            public HttpRequestMessage? Captured { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Captured = request;
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"code\":0}", Encoding.UTF8, "application/json"),
                });
            }
        }

        private sealed class StubClock : IAdsClock
        {
            public DateTimeOffset UtcNow => FixedNow;
        }
    }

    private static Dictionary<string, string> ParseQuery(HttpRequestMessage request)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in request.RequestUri!.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var separator = pair.IndexOf('=');
            var key = separator < 0 ? pair : pair[..separator];
            var value = separator < 0 ? string.Empty : pair[(separator + 1)..];
            result[Uri.UnescapeDataString(key)] = Uri.UnescapeDataString(value);
        }

        return result;
    }
}
