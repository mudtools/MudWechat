// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.OfficialAccount.Tests.Authentication;

/// <summary>
/// 令牌管理器：通道隔离、<c>expires_in</c> 剩余语义、刷新退化边界与 <c>force_refresh</c> 恒 false。
/// </summary>
public class MpAccessTokenManagerTests
{
    private static MpAppConfig Config(bool useStable = true, int threshold = 300)
        => new()
        {
            AppKey = "mp1",
            AppId = "wx-test",
            AppSecret = "secret-test",
            UseStableToken = useStable,
            TokenRefreshThreshold = threshold,
        };

    private static Mock<IMpAuthentication> AuthReturning(int expiresIn, string token = "TOKEN")
    {
        var auth = new Mock<IMpAuthentication>();
        auth.Setup(a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTokenResponse { AccessToken = token, ExpiresIn = expiresIn });
        auth.Setup(a => a.GetTokenAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTokenResponse { AccessToken = token, ExpiresIn = expiresIn });
        return auth;
    }

    private static int ReadEffectiveThreshold(MpAccessTokenManagerBase manager)
    {
        var property = manager.GetType()
            .GetProperty("ExpireThresholdSeconds", BindingFlags.NonPublic | BindingFlags.Instance);
        property.Should().NotBeNull("生效阈值是模板基座的受保护扩展点，测试需直读以锁定刷新退化边界");
        return (int)property!.GetValue(manager)!;
    }

    private static string ReadTokenTypeKey(MpAccessTokenManagerBase manager)
    {
        var property = manager.GetType()
            .GetProperty("TokenTypeKey", BindingFlags.NonPublic | BindingFlags.Instance);
        property.Should().NotBeNull();
        return (string)property!.GetValue(manager)!;
    }

    /// <summary>
    /// TM1：<c>expires_in</c> 按「本次返回的剩余时长」计入 + 生效阈值收敛为 <c>min(配置阈值, expires_in/2)</c>。
    /// </summary>
    /// <remarks>
    /// 官方普通模式复用旧 token 时返回剩余秒数（示例 345）。若把它当固定 7200 起算，阈值会错算为 300；
    /// 若不做阈值收敛，冷启动拿到 <c>expires_in ≈ 阈值</c> 的 token 会把提前刷新退化为逐次调用。
    /// 断言 172 = min(300, 345/2) 同时锁定两件事。
    /// </remarks>
    [Fact]
    public async Task Refresh_ShouldUseReturnedRemainingSecondsAndCapThreshold()
    {
        var auth = AuthReturning(345);
        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config()), NullLogger<MpStableAccessTokenManager>.Instance);

        (await manager.GetTokenAsync()).Should().Be("TOKEN");
        ReadEffectiveThreshold(manager).Should().Be(172,
            "expires_in=345 且配置阈值 300 ⇒ 生效阈值 min(300, 172) = 172");
    }

    /// <summary>TM2：配置阈值小于 <c>expires_in/2</c> 时以配置阈值为准（阈值只能收紧，不能放大）。</summary>
    [Fact]
    public async Task Refresh_ShouldNotRaiseConfiguredThreshold()
    {
        var auth = AuthReturning(7200);
        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config(threshold: 300)), NullLogger<MpStableAccessTokenManager>.Instance);

        await manager.GetTokenAsync();
        ReadEffectiveThreshold(manager).Should().Be(300, "7200/2 = 3600 > 300 ⇒ 取配置阈值，不得被放大");
    }

    /// <summary>
    /// TM3：剩余窗口小于配置阈值时不得退化为「每次调用都刷新」（同一 token 连续取用只调用平台一次）。
    /// </summary>
    [Fact]
    public async Task ConsecutiveGets_ShouldNotDegradeToPerCallRefresh()
    {
        var auth = AuthReturning(200);
        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config(threshold: 300)), NullLogger<MpStableAccessTokenManager>.Instance);

        await manager.GetTokenAsync();
        await manager.GetTokenAsync();
        await manager.GetTokenAsync();

        auth.Verify(a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "expires_in=200 ⇒ 生效阈值 100 ⇒ 剩余 200 秒内连续取用不得反复刷新平台（否则空耗 1 万次/分额度）");
    }

    /// <summary>TM4：稳定版通道恒传 <c>force_refresh = false</c>（不提供强刷开关，避免烧 20 次/天配额）。</summary>
    [Fact]
    public async Task StableChannel_ShouldAlwaysSendForceRefreshFalse()
    {
        MpStableTokenRequest? captured = null;
        var auth = new Mock<IMpAuthentication>();
        auth.Setup(a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()))
            .Callback<MpStableTokenRequest, CancellationToken>((request, _) => captured = request)
            .ReturnsAsync(new MpGetTokenResponse { AccessToken = "T", ExpiresIn = 7200 });

        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config()), NullLogger<MpStableAccessTokenManager>.Instance);
        await manager.GetTokenAsync();

        captured.Should().NotBeNull();
        captured!.ForceRefresh.Should().BeFalse("本 SDK 恒以普通模式调用（平台提前 5 分钟轮换，本地阈值更早拦截）");
        captured.GrantType.Should().Be("client_credential");
        captured.AppId.Should().Be("wx-test");
        captured.Secret.Should().Be("secret-test");
    }

    /// <summary>
    /// TM5：两通道持久化键前缀互相隔离（官方「两个接口的凭据完全隔离、互不影响」语义的落点）。
    /// </summary>
    [Fact]
    public async Task Channels_ShouldIsolatePersistenceKeyPrefixes()
    {
        var stable = new MpStableAccessTokenManager(
            AuthReturning(7200).Object, Options.Create(Config(useStable: true)),
            NullLogger<MpStableAccessTokenManager>.Instance);
        var standard = new MpStandardAccessTokenManager(
            AuthReturning(7200).Object, Options.Create(Config(useStable: false)),
            NullLogger<MpStandardAccessTokenManager>.Instance);

        await stable.GetTokenAsync();
        await standard.GetTokenAsync();

        ReadTokenTypeKey(stable).Should().Be($"{MpStableAccessTokenManager.ChannelKeyPrefix}:mp1");
        ReadTokenTypeKey(standard).Should().Be($"{MpStandardAccessTokenManager.ChannelKeyPrefix}:mp1");
        ReadTokenTypeKey(stable).Should().NotBe(ReadTokenTypeKey(standard),
            "两通道共用同一令牌路由键，隔离只能落在持久化键前缀上；前缀相同会导致跨通道串号");
    }

    /// <summary>TM6：签发失败（errcode 非 0）抛 <see cref="MpException"/> 且错误码透传。</summary>
    [Fact]
    public async Task Refresh_ShouldThrowMpExceptionOnBusinessError()
    {
        var auth = new Mock<IMpAuthentication>();
        auth.Setup(a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTokenResponse { ErrorCode = MpErrorCodes.FrozenSecret, ErrorMessage = "frozen" });

        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config()), NullLogger<MpStableAccessTokenManager>.Instance);

        var act = async () => await manager.GetTokenAsync();
        (await act.Should().ThrowAsync<MpException>()).Which.ErrorCode.Should().Be(MpErrorCodes.FrozenSecret);
    }

    /// <summary>TM7：签发返回 null ⇒ 抛 <see cref="MpException"/>（错误码 -1）。</summary>
    [Fact]
    public async Task Refresh_ShouldThrowOnNullResponse()
    {
        var auth = new Mock<IMpAuthentication>();
        auth.Setup(a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((MpGetTokenResponse?)null);

        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config()), NullLogger<MpStableAccessTokenManager>.Instance);

        var act = async () => await manager.GetTokenAsync();
        (await act.Should().ThrowAsync<MpException>()).Which.ErrorCode.Should().Be(-1);
    }

    /// <summary>TM8：成功响应不带 <c>errcode</c>（缺省 0）必须视为成功。</summary>
    [Fact]
    public async Task Refresh_ShouldTreatMissingErrcodeAsSuccess()
    {
        var auth = AuthReturning(7200, "NO_ERRCODE");
        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config()), NullLogger<MpStableAccessTokenManager>.Instance);

        (await manager.GetTokenAsync()).Should().Be("NO_ERRCODE");
    }
}
