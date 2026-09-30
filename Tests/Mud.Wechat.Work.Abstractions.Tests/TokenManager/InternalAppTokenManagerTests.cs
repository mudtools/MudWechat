// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.InternalAppAuthentication;

namespace Mud.Wechat.Work.Abstractions.Tests.TokenManager;

/// <summary>
/// 令牌管理器经模板基座（WechatAppTokenManagerBase）的缓存命中/未命中/异常路径测试（详细设计 §18.1）。
/// </summary>
public class InternalAppTokenManagerTests
{
    private static IOptions<WechatAppConfig> Options(string appKey = "default")
        => Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
        {
            AppKey = appKey,
            CorpId = "ww-corp",
            AgentSecret = "agent-secret",
            TokenRefreshThreshold = 300,
        });

    [Fact]
    public async Task GetTokenAsync_ShouldRefreshOnce_WhenCacheHit()
    {
        var authMock = new Mock<IWechatWorkInternalAppAuthentication>();
        authMock
            .Setup(a => a.GetTokenAsync("ww-corp", "agent-secret", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResponse { ErrorCode = 0, AccessToken = "token-1", ExpiresIn = 7200 });

        using var manager = new InternalAppTokenManager(authMock.Object, Options(), NullLogger<InternalAppTokenManager>.Instance);

        var first = await manager.GetTokenAsync();
        var second = await manager.GetTokenAsync();

        first.Should().Be("token-1");
        second.Should().Be("token-1", "缓存有效期内不应重复刷新");
        authMock.Verify(a => a.GetTokenAsync("ww-corp", "agent-secret", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTokenAsync_ShouldThrowWechatWorkException_WhenErrcodeNotZero()
    {
        var authMock = new Mock<IWechatWorkInternalAppAuthentication>();
        authMock
            .Setup(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResponse { ErrorCode = 40013, ErrorMessage = "invalid corpid" });

        using var manager = new InternalAppTokenManager(authMock.Object, Options(), NullLogger<InternalAppTokenManager>.Instance);

        var act = async () => await manager.GetTokenAsync();
        (await act.Should().ThrowAsync<WechatWorkException>()).Which.ErrorCode.Should().Be(40013);
    }

    [Fact]
    public async Task InvalidateTokenAsync_ShouldForceRefreshNextGet()
    {
        var authMock = new Mock<IWechatWorkInternalAppAuthentication>();
        var sequence = authMock
            .SetupSequence(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResponse { ErrorCode = 0, AccessToken = "token-1", ExpiresIn = 7200 })
            .ReturnsAsync(new GetTokenResponse { ErrorCode = 0, AccessToken = "token-2", ExpiresIn = 7200 });

        using var manager = new InternalAppTokenManager(authMock.Object, Options(), NullLogger<InternalAppTokenManager>.Instance);

        (await manager.GetTokenAsync()).Should().Be("token-1");
        await manager.InvalidateTokenAsync();
        (await manager.GetTokenAsync()).Should().Be("token-2", "级联失效后应强制刷新");
        authMock.Verify(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task InvalidateTokenAsync_ShouldCascadeToStore()
    {
        var authMock = new Mock<IWechatWorkInternalAppAuthentication>();
        authMock
            .Setup(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetTokenResponse { ErrorCode = 0, AccessToken = "token-1", ExpiresIn = 7200 });

        var store = new InMemoryWechatTokenStore();
        using var manager = new InternalAppTokenManager(
            authMock.Object, Options(), NullLogger<InternalAppTokenManager>.Instance, store);

        await manager.GetTokenAsync();
        var keysBefore = (await store.GetTokenTypesAsync()).ToList();
        keysBefore.Should().NotBeEmpty("写穿桥接器应把令牌落到持久化仓储");

        await manager.InvalidateTokenAsync();
        var keysAfter = (await store.GetTokenTypesAsync()).ToList();
        keysAfter.Should().BeEmpty("失效应级联双清持久化仓储（TMA-01）");
    }

    [Fact]
    public async Task GetTokenAsync_WithShortTtl_ShouldClampThresholdInsteadOfRefreshStorm()
    {
        var authMock = new Mock<IWechatWorkInternalAppAuthentication>();
        var callCount = 0;
        authMock
            .Setup(a => a.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                callCount++;
                return new GetTokenResponse { ErrorCode = 0, AccessToken = $"token-{callCount}", ExpiresIn = 60 };
            });

        using var manager = new InternalAppTokenManager(
            authMock.Object, Microsoft.Extensions.Options.Options.Create(new WechatAppConfig
            {
                AppKey = "default",
                CorpId = "ww-corp",
                AgentSecret = "agent-secret",
                TokenRefreshThreshold = 300,
            }), NullLogger<InternalAppTokenManager>.Instance);

        var first = await manager.GetTokenAsync();
        var second = await manager.GetTokenAsync();

        first.Should().Be("token-1");
        second.Should().Be("token-1", "TK-04 TTL 感知钳位：短 TTL 令牌的有效阈值为 min(threshold, ttl/2)，刚签发不触发刷新风暴");
        callCount.Should().Be(1);
    }
}
