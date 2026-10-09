// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 授权流程服务的行为锁定（请求形态 / 应答解析 / 无 API 权限语义 / 令牌不外泄）。
/// </summary>
/// <remarks>
/// <b>两条最值得看的断言</b>：①「<c>access_token</c> 为空属正常」—— 只授权扫码登录的账号不会返回
/// 接口调用令牌，把它判成失败会让正常授权流程报错；②「令牌不得出现在异常消息里」——
/// 平台令牌走 URL 查询参数是官方契约，一旦原样进异常就会随日志/监控扩散。
/// </remarks>
public class ComponentAuthorizationServiceTests
{
    private static readonly DateTimeOffset Start = new DateTimeOffset(2026, 10, 9, 12, 0, 0, TimeSpan.Zero);
    private const string ComponentToken = "component-token-xyz";

    /// <summary>预授权码：请求体只含 component_appid，令牌走 query；有效期取官方应答。</summary>
    [Fact]
    public async Task CreatePreAuthCode_ShouldPostOfficialShape()
    {
        string? uri = null;
        string? body = null;
        var clock = new FakeClock(Start);
        var sut = CreateSut(
            """{"pre_auth_code":"pre-1","expires_in":1800}""",
            out var client,
            clock,
            capture: (u, b) => { uri = u; body = b; });

        var result = await sut.CreatePreAuthCodeAsync();

        result.PreAuthCode.Should().Be("pre-1");
        result.ExpiresAt.Should().Be(Start.AddSeconds(1800), "官方原文：预授权码有效期为 1800 秒");

        uri.Should().Be("/cgi-bin/component/api_create_preauthcode?component_access_token=" + ComponentToken);
        body.Should().NotBeNull();
        body!.Should().Contain("component_appid").And.Contain("wx-comp");
        body.Should().NotContain("component_verify_ticket", "预授权码请求体不带票据");
    }

    /// <summary>
    /// 换取授权信息：解析嵌套 <c>authorization_info</c>，返回令牌与到期时刻。
    /// </summary>
    [Fact]
    public async Task QueryAuthorization_ShouldParseNestedAuthorizationInfo()
    {
        string? body = null;
        var clock = new FakeClock(Start);
        var sut = CreateSut(
            """{"authorization_info":{"authorizer_appid":"wx-auth","authorizer_access_token":"auth-token","expires_in":7200,"authorizer_refresh_token":"refresh-1"}}""",
            out _,
            clock,
            capture: (_, b) => body = b);

        var tokens = await sut.QueryAuthorizationAsync("auth-code-1");

        body.Should().NotBeNull();
        body!.Should().Contain("authorization_code").And.Contain("auth-code-1");
        body.Should().Contain("component_appid");

        tokens.AuthorizerAppId.Should().Be("wx-auth");
        tokens.AccessToken.Should().Be("auth-token");
        tokens.RefreshToken.Should().Be("refresh-1", "刷新令牌必须回传给宿主持久化");
        tokens.HasApiScope.Should().BeTrue();
        tokens.AccessTokenExpiresAt.Should().Be(Start.AddSeconds(7200), "官方原文：authorizer_access_token 有效期 2 小时");
    }

    /// <summary>
    /// <b>无 API 权限 ⇒ 没有 <c>authorizer_access_token</c>，这是正常形态而不是失败</b>
    /// （官方原文：仅「具备 API 权限时」才返回该字段）。
    /// </summary>
    [Fact]
    public async Task QueryAuthorization_ShouldTolerateMissingAccessToken()
    {
        var sut = CreateSut(
            """{"authorization_info":{"authorizer_appid":"wx-auth","authorizer_refresh_token":"refresh-1"}}""",
            out _,
            new FakeClock(Start));

        var tokens = await sut.QueryAuthorizationAsync("auth-code-1");

        tokens.HasApiScope.Should().BeFalse("只授权扫码登录等能力的账号不返回接口调用令牌");
        tokens.AccessToken.Should().BeNull();
        tokens.RefreshToken.Should().Be("refresh-1", "即使没有 API 权限，刷新令牌仍需保存");
    }

    /// <summary>刷新令牌：请求体三字段齐全；应答返回的新刷新令牌须覆盖传入值。</summary>
    [Fact]
    public async Task Refresh_ShouldPostThreeFields_AndAdoptReturnedRefreshToken()
    {
        string? body = null;
        var clock = new FakeClock(Start);
        var sut = CreateSut(
            """{"authorizer_access_token":"auth-token-2","expires_in":7200,"authorizer_refresh_token":"refresh-2"}""",
            out _,
            clock,
            capture: (_, b) => body = b);

        var tokens = await sut.RefreshAuthorizerTokenAsync("wx-auth", "refresh-1");

        body.Should().NotBeNull();
        body!.Should().Contain("authorizer_appid").And.Contain("wx-auth");
        body.Should().Contain("authorizer_refresh_token").And.Contain("refresh-1");

        tokens.AccessToken.Should().Be("auth-token-2");
        tokens.RefreshToken.Should().Be("refresh-2",
            "官方未说明刷新令牌是否轮换 ⇒ 返回了就按最新值保存（不变则等价无操作）");
        tokens.AccessTokenExpiresAt.Should().Be(Start.AddSeconds(7200));
    }

    /// <summary>应答未返回刷新令牌 ⇒ 沿用传入值（不得把它清成 null，否则宿主会把长期凭据弄丢）。</summary>
    [Fact]
    public async Task Refresh_ShouldKeepPassedRefreshToken_WhenResponseOmitsIt()
    {
        var sut = CreateSut(
            """{"authorizer_access_token":"auth-token-2","expires_in":7200}""",
            out _,
            new FakeClock(Start));

        var tokens = await sut.RefreshAuthorizerTokenAsync("wx-auth", "refresh-1");

        tokens.RefreshToken.Should().Be("refresh-1");
    }

    /// <summary>
    /// <b>HTTP 200 + errcode 必须按失败处理</b>，且官方错误码保留在异常上。
    /// </summary>
    [Fact]
    public async Task ShouldTreatBusinessError_AsFailure()
    {
        var sut = CreateSut(
            """{"errcode":40001,"errmsg":"invalid credential"}""",
            out _,
            new FakeClock(Start));

        var act = async () => await sut.CreatePreAuthCodeAsync();

        (await act.Should().ThrowAsync<WechatOpenPlatformException>())
            .Which.ErrorCode.Should().Be("40001");
    }

    /// <summary>
    /// <b>令牌不得出现在异常消息里</b>：平台令牌走 URL 查询参数（官方契约强制），
    /// 一旦原样进异常就会随日志/监控扩散。
    /// </summary>
    [Fact]
    public async Task ExceptionMessage_ShouldNotLeakComponentToken()
    {
        var sut = CreateSut(
            """{"errcode":40001,"errmsg":"invalid credential"}""",
            out _,
            new FakeClock(Start));

        var act = async () => await sut.CreatePreAuthCodeAsync();

        var exception = (await act.Should().ThrowAsync<WechatOpenPlatformException>()).Which;
        exception.Message.Should().NotContain(ComponentToken, "异常消息只带路径，绝不带含令牌的查询串");
        exception.Message.Should().Contain("/cgi-bin/component/api_create_preauthcode", "但要保留路径以便定位");
    }

    /// <summary>空白入参 ⇒ 参数错误，且<b>不</b>触达 HTTP（含取平台令牌）。</summary>
    [Fact]
    public async Task ShouldRejectBlankArguments_WithoutSending()
    {
        var client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Strict);
        var provider = new Mock<IComponentTokenProvider>(MockBehavior.Strict);
        var sut = new ComponentAuthorizationService(client.Object, provider.Object, new FakeClock(Start), CreateConfig());

        await FluentActions.Awaiting(() => sut.QueryAuthorizationAsync("  "))
            .Should().ThrowAsync<ArgumentException>();
        await FluentActions.Awaiting(() => sut.RefreshAuthorizerTokenAsync("", "r"))
            .Should().ThrowAsync<ArgumentException>();
        await FluentActions.Awaiting(() => sut.RefreshAuthorizerTokenAsync("wx-auth", " "))
            .Should().ThrowAsync<ArgumentException>();

        client.VerifyNoOtherCalls();
        provider.VerifyNoOtherCalls();
    }

    /// <summary>应答缺少必需字段 ⇒ 明确抛出（不得返回空壳对象让调用方拿着 null 继续跑）。</summary>
    [Fact]
    public async Task ShouldThrow_WhenResponseMissingRequiredField()
    {
        var sut = CreateSut("""{"expires_in":1800}""", out _, new FakeClock(Start));

        var act = async () => await sut.CreatePreAuthCodeAsync();

        (await act.Should().ThrowAsync<WechatOpenPlatformException>())
            .Which.Message.Should().Contain("pre_auth_code");
    }

    // ---- helpers -------------------------------------------------------------

    private static ComponentAuthorizationService CreateSut(
        string responseJson,
        out Mock<IWechatOpenPlatformHttpClient> client,
        IOpenPlatformClock clock,
        Action<string?, string?>? capture = null)
    {
        client = new Mock<IWechatOpenPlatformHttpClient>(MockBehavior.Loose);
        client
            .Setup(c => c.SendRawAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            // ⚠️ 请求在实现内被 using 释放 ⇒ 必须在回调里即时取 URL 与包体。
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capture?.Invoke(
                request.RequestUri?.ToString(),
                request.Content?.ReadAsStringAsync().GetAwaiter().GetResult()))
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json"),
            });

        var provider = new Mock<IComponentTokenProvider>(MockBehavior.Loose);
        provider
            .Setup(p => p.GetComponentAccessTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ComponentToken);

        return new ComponentAuthorizationService(client.Object, provider.Object, clock, CreateConfig());
    }

    private static OpenPlatformAppConfig CreateConfig()
        => new()
        {
            ComponentAppId = "wx-comp",
            ComponentAppSecret = "secret",
            Token = "push-token",
            EncodingAesKey = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFG",
        };

    /// <summary>可控时钟（用于断言到期时刻）。</summary>
    private sealed class FakeClock : IOpenPlatformClock
    {
        private readonly DateTimeOffset _now;

        public FakeClock(DateTimeOffset now) => _now = now;

        public DateTimeOffset UtcNow => _now;
    }
}
