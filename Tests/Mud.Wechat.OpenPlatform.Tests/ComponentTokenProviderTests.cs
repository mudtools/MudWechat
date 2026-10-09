// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 第三方平台令牌提供者的行为锁定（缓存 / 提前刷新 / 并发单飞 / 失败回退 / fail-closed）。
/// </summary>
/// <remarks>
/// <b>全部用例都不触网</b>：HTTP 走 Moq 替身。本组锁的正是「看起来成功了但其实错了」的几种形态 ——
/// 尤其是「刷新失败时到底该不该沿用旧令牌」，两种选择在生产上是完全不同的故障表现。
/// </remarks>
public class ComponentTokenProviderTests
{
    private static readonly DateTimeOffset Start = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);

    /// <summary>未收到票据 ⇒ 明确抛出，且<b>不得</b>打官方接口（避免无意义出站与错误码掩盖真因）。</summary>
    [Fact]
    public async Task GetToken_ShouldFailFast_WhenTicketMissing()
    {
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Strict);
        var clock = new FakeClock(Start);
        var sut = CreateSut(client, new InMemoryComponentVerifyTicketStore(clock), clock);

        var act = async () => await sut.GetComponentAccessTokenAsync();

        (await act.Should().ThrowAsync<WechatOpenPlatformException>())
            .Which.Message.Should().Contain("component_verify_ticket");
        client.VerifyNoOtherCalls();
    }

    /// <summary>
    /// 首次取令牌：请求体含官方三字段（照官方原文），成功后<b>缓存命中不再打接口</b>。
    /// </summary>
    [Theory]
    [InlineData("""{"component_access_token":"token-1","expires_in":7200}""")]
    [InlineData("""{"component_access_token":"token-1","expires_in":"7200"}""")]
    public async Task GetToken_ShouldPostOfficialFields_AndCache(string officialJson)
    {
        string? body = null;
        string? uri = null;
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Loose);
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            // ⚠️ 在回调里即时读取请求内容：实现用 using 释放了请求，事后读会得到已释放内容。
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                uri = request.RequestUri?.ToString();
                body = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(Ok(officialJson));

        var clock = new FakeClock(Start);
        var sut = CreateSut(client, CreateStore(clock, "ticket-1"), clock);

        var first = await sut.GetComponentAccessTokenAsync();
        var second = await sut.GetComponentAccessTokenAsync();

        first.Should().Be("token-1");
        second.Should().Be("token-1");

        uri.Should().Be("/cgi-bin/component/api_component_token", "相对路径由命名客户端的 BaseAddress 补全");
        body.Should().NotBeNull();
        body!.Should().Contain("component_appid").And.Contain("wx-comp");
        body.Should().Contain("component_appsecret").And.Contain("secret");
        body.Should().Contain("component_verify_ticket").And.Contain("ticket-1");

        client.Verify(
            c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "第二次调用必须命中缓存（否则等于每次业务请求都去打一次官方令牌接口）");
    }

    /// <summary>
    /// <b>进入提前刷新窗口 ⇒ 主动再刷一次</b>（官方建议 1 小时 50 分即刷新）。
    /// </summary>
    [Fact]
    public async Task GetToken_ShouldRefresh_WhenInsideLeadWindow()
    {
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Loose);
        var calls = 0;
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                calls++;
                return Ok($$"""{"component_access_token":"token-{{calls}}","expires_in":7200}""");
            });

        var clock = new FakeClock(Start);
        var sut = CreateSut(client, CreateStore(clock, "ticket-1"), clock);

        (await sut.GetComponentAccessTokenAsync()).Should().Be("token-1");

        // 走到距失效仅 5 分钟（< 10 分钟提前窗口）：应刷新，且**旧令牌此刻仍可用**。
        clock.Advance(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)));
        (await sut.GetComponentAccessTokenAsync()).Should().Be("token-2");

        client.Verify(
            c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    /// <summary>
    /// <b>并发单飞</b>：多个调用同时发现该刷新时，官方接口<b>只被打一次</b>。
    /// </summary>
    [Fact]
    public async Task GetToken_ShouldSingleFlight_UnderConcurrency()
    {
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Loose);
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await gate.Task.ConfigureAwait(false);
                return Ok("""{"component_access_token":"token-1","expires_in":7200}""");
            });

        var clock = new FakeClock(Start);
        var sut = CreateSut(client, CreateStore(clock, "ticket-1"), clock);

        var first = sut.GetComponentAccessTokenAsync();
        var second = sut.GetComponentAccessTokenAsync();

        gate.SetResult(true);
        var results = await Task.WhenAll(first, second);

        results.Should().AllBe("token-1");
        client.Verify(
            c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "并发刷新必须单飞（官方对令牌获取有频次限制，重复打会被限流）");
    }

    /// <summary>
    /// <b>刷新失败但旧令牌仍可用 ⇒ 沿用旧令牌</b>：进入提前窗口只是「该换了」，不是「已失效」；
    /// 官方偶发失败不应把失败传染给业务请求。
    /// </summary>
    [Fact]
    public async Task GetToken_ShouldFallBackToStillUsableToken_WhenRefreshFails()
    {
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Loose);
        var calls = 0;
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                calls++;
                return calls == 1
                    ? Ok("""{"component_access_token":"token-old","expires_in":7200}""")
                    : new HttpResponseMessage(HttpStatusCode.InternalServerError);
            });

        var clock = new FakeClock(Start);
        var sut = CreateSut(client, CreateStore(clock, "ticket-1"), clock);

        (await sut.GetComponentAccessTokenAsync()).Should().Be("token-old");

        clock.Advance(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)));

        var act = async () => await sut.GetComponentAccessTokenAsync();

        (await act.Should().NotThrowAsync()).Subject.Should().Be("token-old",
            "旧令牌此刻仍然有效 ⇒ 不得因刷新失败而拒绝服务");
    }

    /// <summary>
    /// <b>旧令牌已失效且刷新失败 ⇒ fail-closed 抛出</b>，且官方错误码必须被保留。
    /// </summary>
    [Fact]
    public async Task GetToken_ShouldFailClosed_WhenRefreshFailsAndTokenExpired()
    {
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Loose);
        var calls = 0;
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                calls++;
                return calls == 1
                    ? Ok("""{"component_access_token":"token-old","expires_in":7200}""")
                    : Ok("""{"errcode":40001,"errmsg":"invalid credential"}""");
            });

        var clock = new FakeClock(Start);
        var sut = CreateSut(client, CreateStore(clock, "ticket-1"), clock);

        (await sut.GetComponentAccessTokenAsync()).Should().Be("token-old");

        // 越过失效时刻：此刻旧令牌已不可用 ⇒ 只能抛出。
        clock.Advance(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(1)));

        var act = async () => await sut.GetComponentAccessTokenAsync();

        (await act.Should().ThrowAsync<WechatOpenPlatformException>())
            .Which.ErrorCode.Should().Be("40001",
                "官方 errcode 必须保留（判错与自愈的唯一依据；丢掉它调用方无法分支处理）");
    }

    /// <summary>
    /// <b>HTTP 200 + errcode</b> 是开放平台表达业务失败的常见形态 ⇒ 必须按失败处理
    /// （只看 HTTP 状态码会把 errcode 当成成功应答，再去解析 token 得到 null）。
    /// </summary>
    [Fact]
    public async Task GetToken_ShouldTreatBusinessError_AsFailure()
    {
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Loose);
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Ok("""{"errcode":-1,"errmsg":"system error"}"""));

        var clock = new FakeClock(Start);
        var sut = CreateSut(client, CreateStore(clock, "ticket-1"), clock);

        var act = async () => await sut.GetComponentAccessTokenAsync();

        (await act.Should().ThrowAsync<WechatOpenPlatformException>())
            .Which.ErrorCode.Should().Be("-1");
    }

    /// <summary>
    /// 装配：配置缺 appid / appsecret 时在<b>注册期</b> fail-fast
    /// （否则要等到首次取令牌才炸，而那时通常已在线上了）。
    /// </summary>
    [Theory]
    [InlineData(null, "secret")]
    [InlineData("wx-comp", null)]
    [InlineData("  ", "secret")]
    public void AddOpenPlatform_ShouldFailFast_WhenConfigIncomplete(string? appId, string? secret)
    {
        var act = () => new ServiceCollection().AddOpenPlatform(c =>
        {
            c.ComponentAppId = appId;
            c.ComponentAppSecret = secret;
        });

        act.Should().Throw<InvalidOperationException>("配置不完整必须在注册期就点名（不得推迟到首次取令牌）");
    }

    /// <summary>装配：完整配置下令牌提供者可解析（三段式：命名客户端 → 端口 → 实现）。</summary>
    [Fact]
    public void AddOpenPlatform_ShouldRegisterResolvableProvider()
    {
        var services = new ServiceCollection();
        services.AddOpenPlatform(static c =>
        {
            c.ComponentAppId = "wx-comp";
            c.ComponentAppSecret = "secret";
        });

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });

        provider.GetRequiredService<IComponentTokenProvider>().Should().NotBeNull();
        provider.GetRequiredService<IWechatOpenPlatformHttpClient>().Should().NotBeNull();
        provider.GetRequiredService<IComponentVerifyTicketStore>().Should().NotBeNull(
            "票据存储随本线注册（宿主可预注册 Redis 实现覆盖）");
    }

    // ---- helpers -------------------------------------------------------------

    private static ComponentTokenProvider CreateSut(
        Mock<IWechatOpenPlatformHttpClient> client,
        IComponentVerifyTicketStore store,
        IOpenPlatformClock clock)
        => new(
            client.Object,
            store,
            clock,
            new OpenPlatformAppConfig { ComponentAppId = "wx-comp", ComponentAppSecret = "secret" },
            NullLogger<ComponentTokenProvider>.Instance);

    private static IComponentVerifyTicketStore CreateStore(IOpenPlatformClock clock, string ticket)
    {
        var store = new InMemoryComponentVerifyTicketStore(clock);
        store.Set(ticket);
        return store;
    }

    private static HttpResponseMessage Ok(string json)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };

    /// <summary>可控时钟（用例需要把时间推到「提前窗口内」与「已失效」两侧）。</summary>
    private sealed class FakeClock : IOpenPlatformClock
    {
        private DateTimeOffset _now;

        public FakeClock(DateTimeOffset now) => _now = now;

        public DateTimeOffset UtcNow => _now;

        public void Advance(TimeSpan delta) => _now = _now.Add(delta);
    }
}
