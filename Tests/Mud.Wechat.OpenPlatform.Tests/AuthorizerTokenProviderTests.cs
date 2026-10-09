// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 授权方令牌提供者的行为锁定（缓存 / 提前刷新 / 分槽单飞 / 失败回退 / fail-closed）。
/// </summary>
/// <remarks>
/// <b>本组最有价值的两条</b>：①「按 appid 分槽」—— 某个授权方的刷新卡住不得阻塞其它授权方
/// （把「一个商户的问题」放大成「全平台抖动」是这类缓存层最常见的生产事故）；
/// ②「刷新失败时旧令牌仍可用则沿用」—— 进提前窗口不等于已失效。
/// </remarks>
public class AuthorizerTokenProviderTests
{
    private static readonly DateTimeOffset Start = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);
    private const string Authorizer = "wx-authorizer";

    /// <summary>已接受的授权（令牌仍新鲜）⇒ 走缓存，<b>不</b>打官方刷新接口。</summary>
    [Fact]
    public async Task GetToken_ShouldUseCache_WithoutRefreshing()
    {
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        sut.AcceptAuthorization(new AuthorizerTokens(Authorizer, "access-1", "refresh-1", Start.AddHours(2)));

        (await sut.GetAuthorizerAccessTokenAsync(Authorizer)).Should().Be("access-1");

        authorization.Verify(
            a => a.RefreshAuthorizerTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "令牌新鲜时必须走缓存 —— 官方明确要求缓存以免触发每日限额");
    }

    /// <summary>
    /// <b>进入提前刷新窗口 ⇒ 用刷新令牌换新令牌并整体替换保存</b>。
    /// </summary>
    [Fact]
    public async Task GetToken_ShouldRefreshInsideLeadWindow_AndPersist()
    {
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        sut.AcceptAuthorization(new AuthorizerTokens(Authorizer, "access-1", "refresh-1", Start.AddHours(2)));

        authorization
            .Setup(a => a.RefreshAuthorizerTokenAsync(Authorizer, "refresh-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthorizerTokens(
                Authorizer, "access-2", "refresh-2", Start.AddHours(1).AddMinutes(55).AddHours(2)));

        // 距失效 5 分钟（< 10 分钟提前窗口）。
        clock.Advance(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)));

        (await sut.GetAuthorizerAccessTokenAsync(Authorizer)).Should().Be("access-2");

        store.TryGet(Authorizer, out var snapshot).Should().BeTrue();
        snapshot!.AccessToken.Should().Be("access-2");
        snapshot.RefreshToken.Should().Be("refresh-2", "官方返回了新刷新令牌 ⇒ 采用新值");
        snapshot.UpdatedAt.Should().Be(clock.UtcNow);
    }

    /// <summary>刷新应答<b>未</b>返回刷新令牌 ⇒ 沿用旧值（不得清空长期凭据）。</summary>
    [Fact]
    public async Task GetToken_ShouldKeepRefreshToken_WhenRefreshResponseOmitsIt()
    {
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        sut.AcceptAuthorization(new AuthorizerTokens(Authorizer, "access-1", "refresh-1", Start.AddHours(2)));
        authorization
            .Setup(a => a.RefreshAuthorizerTokenAsync(Authorizer, "refresh-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthorizerTokens(Authorizer, "access-2", null, Start.AddHours(4)));

        clock.Advance(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)));
        await sut.GetAuthorizerAccessTokenAsync(Authorizer);

        store.TryGet(Authorizer, out var snapshot).Should().BeTrue();
        snapshot!.RefreshToken.Should().Be("refresh-1");
    }

    /// <summary>无刷新令牌 ⇒ fail-closed 抛出，且<b>不</b>打官方接口（无凭据可刷）。</summary>
    [Fact]
    public async Task GetToken_ShouldFailClosed_WhenNoRefreshToken()
    {
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        // 直接塞一个「有 appid、无 refresh、access 已过期」的快照（模拟授权关系已失效）。
        store.Set(new AuthorizerTokenSnapshot(Authorizer, null, null, Start.AddMinutes(-1), Start.AddHours(-1)));

        var act = async () => await sut.GetAuthorizerAccessTokenAsync(Authorizer);

        (await act.Should().ThrowAsync<WechatOpenPlatformException>())
            .Which.Message.Should().Contain("刷新令牌");
        authorization.Verify(
            a => a.RefreshAuthorizerTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>刷新失败但旧接口调用令牌仍可用 ⇒ 沿用旧值（不把失败传染给业务请求）。</summary>
    [Fact]
    public async Task GetToken_ShouldFallBackToStillUsableToken_WhenRefreshFails()
    {
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        sut.AcceptAuthorization(new AuthorizerTokens(Authorizer, "access-1", "refresh-1", Start.AddHours(2)));
        authorization
            .Setup(a => a.RefreshAuthorizerTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WechatOpenPlatformException("boom", "40001"));

        clock.Advance(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)));

        (await sut.GetAuthorizerAccessTokenAsync(Authorizer)).Should().Be("access-1",
            "旧令牌此刻仍然有效 ⇒ 不得因刷新失败而拒绝服务");
    }

    /// <summary>刷新失败且旧令牌已失效 ⇒ fail-closed 抛出（保留官方错误码）。</summary>
    [Fact]
    public async Task GetToken_ShouldFailClosed_WhenRefreshFailsAndTokenExpired()
    {
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        sut.AcceptAuthorization(new AuthorizerTokens(Authorizer, "access-1", "refresh-1", Start.AddHours(2)));
        authorization
            .Setup(a => a.RefreshAuthorizerTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new WechatOpenPlatformException("boom", "40001"));

        clock.Advance(TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(1)));

        var act = async () => await sut.GetAuthorizerAccessTokenAsync(Authorizer);

        (await act.Should().ThrowAsync<WechatOpenPlatformException>()).Which.ErrorCode.Should().Be("40001");
    }

    /// <summary>
    /// <b>同一 appid 并发 ⇒ 单飞</b>：官方刷新接口只被打一次。
    /// </summary>
    [Fact]
    public async Task GetToken_ShouldSingleFlight_ForSameAuthorizer()
    {
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        sut.AcceptAuthorization(new AuthorizerTokens(Authorizer, "access-1", "refresh-1", Start.AddHours(2)));
        authorization
            .Setup(a => a.RefreshAuthorizerTokenAsync(Authorizer, "refresh-1", It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await gate.Task.ConfigureAwait(false);
                return new AuthorizerTokens(Authorizer, "access-2", "refresh-1", Start.AddHours(4));
            });

        clock.Advance(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)));

        var first = sut.GetAuthorizerAccessTokenAsync(Authorizer);
        var second = sut.GetAuthorizerAccessTokenAsync(Authorizer);

        gate.SetResult(true);
        var results = await Task.WhenAll(first, second);

        results.Should().AllBe("access-2");
        authorization.Verify(
            a => a.RefreshAuthorizerTokenAsync(Authorizer, "refresh-1", It.IsAny<CancellationToken>()),
            Times.Once,
            "同一授权方的并发刷新必须单飞（否则会成倍消耗官方每日限额）");
    }

    /// <summary>
    /// <b>不同 appid 互不阻塞</b>：某个授权方刷新卡住时，其它授权方仍能取到令牌。
    /// </summary>
    /// <remarks>
    /// 判据用「B 是否能在 A 仍被阻塞时完成」：若刷新闸做成全局一把，
    /// B 会一直等 A ⇒ <c>WaitAsync</c> 超时（本用例即失败）。这条是「分槽」这一设计的<b>可执行证据</b>。
    /// </remarks>
    [Fact]
    public async Task GetToken_ShouldNotBlockOtherAuthorizers()
    {
        var gate = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var clock = new FakeClock(Start);
        var store = new InMemoryAuthorizerTokenStore();
        var sut = CreateSut(store, clock, out var authorization);

        sut.AcceptAuthorization(new AuthorizerTokens("wx-a", "access-a", "refresh-a", Start.AddHours(2)));
        sut.AcceptAuthorization(new AuthorizerTokens("wx-b", "access-b", "refresh-b", Start.AddHours(2)));
        clock.Advance(TimeSpan.FromHours(1).Add(TimeSpan.FromMinutes(55)));

        authorization
            .Setup(a => a.RefreshAuthorizerTokenAsync("wx-a", "refresh-a", It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await gate.Task.ConfigureAwait(false);
                return new AuthorizerTokens("wx-a", "access-a2", "refresh-a", Start.AddHours(4));
            });
        authorization
            .Setup(a => a.RefreshAuthorizerTokenAsync("wx-b", "refresh-b", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuthorizerTokens("wx-b", "access-b2", "refresh-b", Start.AddHours(4)));

        var taskA = sut.GetAuthorizerAccessTokenAsync("wx-a");
        var taskB = sut.GetAuthorizerAccessTokenAsync("wx-b");

        (await taskB.WaitAsync(TimeSpan.FromSeconds(5))).Should().Be("access-b2",
            "B 不得被 A 的刷新阻塞（刷新闸必须按 appid 分槽，而非全局一把）");

        gate.SetResult(true);
        (await taskA).Should().Be("access-a2");
    }

    /// <summary>空白 appid ⇒ 参数错误，且不触达存储与官方接口。</summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetToken_ShouldRejectBlankAuthorizerId(string authorizerAppId)
    {
        var sut = CreateSut(new InMemoryAuthorizerTokenStore(), new FakeClock(Start), out var authorization);

        await FluentActions.Awaiting(() => sut.GetAuthorizerAccessTokenAsync(authorizerAppId))
            .Should().ThrowAsync<ArgumentException>();

        authorization.Verify(
            a => a.RefreshAuthorizerTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// 接受授权时的入参校验：<c>null</c> / 空 appid / 既无刷新令牌也无接口调用令牌 ⇒ 拒绝
    /// （存一个「空的已授权」会让后续误以为授权关系可用）。
    /// </summary>
    [Fact]
    public void AcceptAuthorization_ShouldValidateInput()
    {
        var sut = CreateSut(new InMemoryAuthorizerTokenStore(), new FakeClock(Start), out _);

        FluentActions.Invoking(() => sut.AcceptAuthorization(null!))
            .Should().Throw<ArgumentNullException>();

        FluentActions.Invoking(() => sut.AcceptAuthorization(
                new AuthorizerTokens("  ", "access", "refresh", Start)))
            .Should().Throw<ArgumentException>();

        FluentActions.Invoking(() => sut.AcceptAuthorization(
                new AuthorizerTokens(Authorizer, null, null, Start)))
            .Should().Throw<ArgumentException>();
    }

    // ---- helpers -------------------------------------------------------------

    private static AuthorizerTokenProvider CreateSut(
        IAuthorizerTokenStore store,
        IOpenPlatformClock clock,
        out Mock<IComponentAuthorizationService> authorization)
    {
        authorization = new Mock<IComponentAuthorizationService>(MockBehavior.Loose);
        return new AuthorizerTokenProvider(store, authorization.Object, clock, NullLogger<AuthorizerTokenProvider>.Instance);
    }

    /// <summary>可控时钟。</summary>
    private sealed class FakeClock : IOpenPlatformClock
    {
        private DateTimeOffset _now;

        public FakeClock(DateTimeOffset now) => _now = now;

        public DateTimeOffset UtcNow => _now;

        public void Advance(TimeSpan delta) => _now = _now.Add(delta);
    }
}
