// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.OfficialAccount.Tests.Authentication;

/// <summary>
/// 多实例令牌共享（M1 交付判定）：经持久层的<b>读穿透</b>共享 + 通道键隔离 + 读故障语义。
/// </summary>
/// <remarks>
/// <para>
/// 组件 <c>TokenStoreBackedTokenCache</c> 实现 <c>IAsyncTokenCache&lt;T&gt;</c>，管理器在异步管线
/// （<c>GetOrRefreshTokenAsync → TryGetValidTokenThroughAsync</c>）上做能力探测后走<b>真读穿透</b>：
/// 镜像未命中 ⇒ 直达 store 读取并回填镜像。故 <b>SDK 无需自建「冷启动水合」</b>——
/// 早期实现曾在外层叠加显式水合，实测为纯冗余（本用例组第二条即锁定「不得再叠加」的行为等价性）。
/// </para>
/// <para>
/// 用同一个 <see cref="InMemoryWechatTokenStore"/> 实例模拟「多实例共享的分布式存储（Redis）」：
/// 两个管理器实例各自的内存镜像相互独立，写穿 / 读穿透走同一份存储。
/// </para>
/// </remarks>
public class MpMultiInstanceTokenSharingTests
{
    private static MpAppConfig Config(bool useStable = true)
        => new()
        {
            AppKey = "mp1",
            AppId = "wx-test",
            AppSecret = "secret-test",
            UseStableToken = useStable,
            TokenRefreshThreshold = 300,
        };

    private static Mock<IMpAuthentication> AuthReturning(int expiresIn, string token)
    {
        var auth = new Mock<IMpAuthentication>();
        auth.Setup(a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTokenResponse { AccessToken = token, ExpiresIn = expiresIn });
        auth.Setup(a => a.GetTokenAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MpGetTokenResponse { AccessToken = token, ExpiresIn = expiresIn });
        return auth;
    }

    /// <summary>等待写穿落库（组件写穿为 fire-and-forget + 补偿重放，测试侧按有界轮询收敛）。</summary>
    private static async Task WaitForStoredAsync(IWechatTokenStore store, int timeoutMs = 3000)
    {
        var deadline = Environment.TickCount64 + timeoutMs;
        while (Environment.TickCount64 < deadline)
        {
            var keys = await store.GetTokenTypesAsync().ConfigureAwait(false);
            if (keys != null && keys.Any())
            {
                return;
            }

            await Task.Delay(10).ConfigureAwait(false);
        }

        throw new TimeoutException("等待令牌写穿落库超时。");
    }

    /// <summary>H1：第二个实例（冷启动、镜像为空）经读穿透共享既有令牌，不再向平台换取。</summary>
    [Fact]
    public async Task SecondInstance_ShouldShareTokenThroughReadThrough()
    {
        var store = new InMemoryWechatTokenStore();
        var auth = AuthReturning(7200, "SHARED-TOKEN");
        var options = Options.Create(Config());

        var instance1 = new MpStableAccessTokenManager(
            auth.Object, options, NullLogger<MpStableAccessTokenManager>.Instance, store);
        (await instance1.GetTokenAsync()).Should().Be("SHARED-TOKEN");
        await WaitForStoredAsync(store);

        // 模拟「另一个进程/实例」：全新管理器，内存镜像为空。
        var instance2 = new MpStableAccessTokenManager(
            auth.Object, options, NullLogger<MpStableAccessTokenManager>.Instance, store);
        (await instance2.GetTokenAsync()).Should().Be("SHARED-TOKEN");

        auth.Verify(
            a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "读穿透后第二个实例不得再向微信平台换一次令牌（省配额 + 更快就绪）");
    }

    /// <summary>
    /// H2：读穿透在<b>通道键前缀</b>上同样隔离——稳定版通道的管理器不得读到普通通道的条目。
    /// </summary>
    [Fact]
    public async Task ReadThrough_ShouldNotCrossChannels()
    {
        var store = new InMemoryWechatTokenStore();
        var stableAuth = AuthReturning(7200, "STABLE-TOKEN");
        var standardAuth = AuthReturning(7200, "STANDARD-TOKEN");
        var options = Options.Create(Config());

        var stable = new MpStableAccessTokenManager(
            stableAuth.Object, options, NullLogger<MpStableAccessTokenManager>.Instance, store);
        (await stable.GetTokenAsync()).Should().Be("STABLE-TOKEN");
        await WaitForStoredAsync(store);

        // 普通通道管理器冷启动：store 中只有稳定版通道的条目 ⇒ 读穿透不命中，必须自行换取。
        var standard = new MpStandardAccessTokenManager(
            standardAuth.Object, options, NullLogger<MpStandardAccessTokenManager>.Instance, store);
        (await standard.GetTokenAsync()).Should().Be("STANDARD-TOKEN");

        standardAuth.Verify(
            a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "两通道的凭据官方声明完全隔离 ⇒ 读穿透不得串通道");
    }

    /// <summary>
    /// H3：读穿透是 <b>fail-closed</b>——store 读故障从取令牌调用上抛（不静默降级为平台换取）。
    /// </summary>
    /// <remarks>
    /// 该语义由组件（<c>IAsyncTokenCache.GetAsync</c> 真穿透）决定，SDK 不叠加吞异常装饰器。
    /// 用例锁定的意义：若将来组件改为 fail-open，本用例会红，从而强制同批评估「存储故障是否应影响
    /// 令牌获取」这一可用性耦合（见方案文档 §10.4-R6）。
    /// </remarks>
    [Fact]
    public async Task ReadThroughFailure_ShouldSurfaceStoreError()
    {
        var auth = AuthReturning(7200, "UNREACHABLE");
        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config()), NullLogger<MpStableAccessTokenManager>.Instance,
            new ReadFailingTokenStore());

        var act = async () => await manager.GetTokenAsync();
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*store unavailable*");

        auth.Verify(
            a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "读穿透失败时不得绕过存储直接向平台换取（当前为 fail-closed 语义）");
    }

    /// <summary>H4：无持久化仓储时走纯进程内缓存（读穿透不参与）。</summary>
    [Fact]
    public async Task NoStore_ShouldUseInProcessCacheOnly()
    {
        var auth = AuthReturning(7200, "LOCAL-TOKEN");
        var manager = new MpStableAccessTokenManager(
            auth.Object, Options.Create(Config()), NullLogger<MpStableAccessTokenManager>.Instance);

        (await manager.GetTokenAsync()).Should().Be("LOCAL-TOKEN");
        (await manager.GetTokenAsync()).Should().Be("LOCAL-TOKEN");
        auth.Verify(
            a => a.GetStableTokenAsync(It.IsAny<MpStableTokenRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>读路径故障的存储替身（写穿仍可用）。</summary>
    private sealed class ReadFailingTokenStore : IWechatTokenStore
    {
        public Task<string?> GetAccessTokenAsync(string tokenType, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("store unavailable");

        public Task SetAccessTokenAsync(string tokenType, string accessToken, long expiresInSeconds, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<string?> GetRefreshTokenAsync(string tokenType, CancellationToken cancellationToken = default)
            => Task.FromResult<string?>(null);

        public Task SetRefreshTokenAsync(string tokenType, string refreshToken, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RemoveAsync(string tokenType, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<IEnumerable<string>> GetTokenTypesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IEnumerable<string>>(Array.Empty<string>());

        public Task ClearAsync(CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
