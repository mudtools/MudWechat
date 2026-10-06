// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.TokenManager;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.DataModels.InternalAppAuthentication;

namespace Mud.Wechat.Work.Abstractions.Tests.TokenManager;

/// <summary>
/// 企业微信侧多实例令牌共享：经持久层<b>读穿透</b>共享（组件承担，SDK 不叠加水合层）。
/// </summary>
public class WechatAppTokenManagerMultiInstanceTests
{
    private static IOptions<WechatAppConfig> Options()
        => Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
        {
            AppKey = "default",
            CorpId = "ww-corp",
            AgentSecret = "agent-secret",
            TokenRefreshThreshold = 300,
        });

    private static Mock<IWechatWorkInternalAppAuthentication> AuthReturning(string token)
    {
        var auth = new Mock<IWechatWorkInternalAppAuthentication>();
        auth.Setup(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResponse { ErrorCode = 0, AccessToken = token, ExpiresIn = 7200 });
        return auth;
    }

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

    /// <summary>WH1：共享存储下第二个实例经读穿透拿到既有令牌（平台只被调用一次）。</summary>
    [Fact]
    public async Task SecondInstance_ShouldShareTokenThroughReadThrough()
    {
        var store = new InMemoryWechatTokenStore();
        var auth = AuthReturning("SHARED-TOKEN");

        using var instance1 = new InternalAppTokenManager(
            auth.Object, Options(), NullLogger<InternalAppTokenManager>.Instance, store);
        (await instance1.GetTokenAsync()).Should().Be("SHARED-TOKEN");
        await WaitForStoredAsync(store);

        using var instance2 = new InternalAppTokenManager(
            auth.Object, Options(), NullLogger<InternalAppTokenManager>.Instance, store);
        (await instance2.GetTokenAsync()).Should().Be("SHARED-TOKEN");

        auth.Verify(
            a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once,
            "读穿透后第二个实例不得再向企业微信换一次令牌");
    }

    /// <summary>WH2：无持久化仓储时走纯进程内缓存（读穿透不参与）。</summary>
    [Fact]
    public async Task NoStore_ShouldUseInProcessCacheOnly()
    {
        var auth = AuthReturning("LOCAL-TOKEN");

        using var manager = new InternalAppTokenManager(
            auth.Object, Options(), NullLogger<InternalAppTokenManager>.Instance);

        (await manager.GetTokenAsync()).Should().Be("LOCAL-TOKEN");
        (await manager.GetTokenAsync()).Should().Be("LOCAL-TOKEN");
        auth.Verify(
            a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
