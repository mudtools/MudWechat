// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R5：回调重放守卫（SET NX EX / RD3 fail-closed / 空键防御 / 窗口防御断言）。
/// </summary>
public class RedisWechatCallbackReplayGuardTests
{
    private const string Fingerprint = "356a192b7913b04c54574d18c28d46e6395428ab";
    private const string RedisKey = "wechat:replay:" + Fingerprint;

    private static RedisWechatCallbackReplayGuard CreateGuard(FakeRedisBackend backend)
        => new(backend.Multiplexer.Object, new WechatRedisOptions());

    [Fact]
    public async Task TryMarkAsync_ShouldUseReplayDomainKey()
    {
        var backend = new FakeRedisBackend();
        await CreateGuard(backend).TryMarkAsync(Fingerprint, TimeSpan.FromSeconds(300));

        backend.Database.Verify(
            d => d.StringSetAsync(RedisKey, "1", TimeSpan.FromSeconds(300), It.IsAny<bool>(), When.NotExists, It.IsAny<CommandFlags>()),
            Times.Once);
    }

    [Fact]
    public async Task TryMarkAsync_ShouldReturnTrue_WhenFirstMark()
    {
        var backend = new FakeRedisBackend();
        (await CreateGuard(backend).TryMarkAsync(Fingerprint, TimeSpan.FromSeconds(300))).Should().BeTrue();
    }

    [Fact]
    public async Task TryMarkAsync_ShouldReturnFalse_WhenAlreadyMarked()
    {
        var backend = new FakeRedisBackend();
        var guard = CreateGuard(backend);
        (await guard.TryMarkAsync(Fingerprint, TimeSpan.FromSeconds(300))).Should().BeTrue();

        backend.Database
            .Setup(d => d.StringSetAsync(RedisKey, "1", It.IsAny<TimeSpan?>(), It.IsAny<bool>(), When.NotExists, It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        (await guard.TryMarkAsync(Fingerprint, TimeSpan.FromSeconds(300))).Should().BeFalse();
    }

    [Fact]
    public async Task TryMarkAsync_ShouldThrowArgumentOutOfRange_WhenWindowNotPositive()
    {
        var backend = new FakeRedisBackend();
        var act = async () => await CreateGuard(backend).TryMarkAsync(Fingerprint, TimeSpan.Zero);
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task TryMarkAsync_ShouldReturnFalseWithoutTouchingRedis_WhenKeyEmpty()
    {
        // R-6：空键返回 false 且不触达 Redis（与 InMemory 空键语义对齐）。
        var backend = new FakeRedisBackend();
        var guard = CreateGuard(backend);

        (await guard.TryMarkAsync(string.Empty, TimeSpan.FromSeconds(300))).Should().BeFalse();

        backend.Database.Verify(
            d => d.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()),
            Times.Never);
    }

    [Fact]
    public async Task TryMarkAsync_ShouldThrowCallbackInfrastructureException_WhenStorageFails()
    {
        // RD3 fail-closed：异常上抛，绝不吞成 true/false（吞 true 放行重放；吞 false 丢事件）。
        // 审查报告 v1 P1-1：外层包装为叶层契约异常 WechatCallbackInfrastructureException
        // （中间件专用 catch 映射 503 → 官方 96238 重推，严禁与验证类 403 混流），
        // WechatRedisException 作为 InnerException 保留（分类语义与既有宿主捕获面零破坏）。
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisConnectionException(
            ConnectionFailureType.SocketFailure, CommandFlags.None, "redis down", null!, CommandStatus.Unknown));
        var guard = CreateGuard(backend);

        var act = async () => await guard.TryMarkAsync(Fingerprint, TimeSpan.FromSeconds(300));
        var ex = (await act.Should().ThrowAsync<WechatCallbackInfrastructureException>()).Which;

        var inner = ex.InnerException.Should().BeOfType<WechatRedisException>().Which;
        inner.FailureKind.Should().Be(WechatRedisFailureKind.Connection);
        ex.Message.Should().Contain(RedisKey);
        ex.Message.Should().NotContain("secret");
    }

    [Fact]
    public async Task TryMarkAsync_ShouldThrowCallbackInfrastructureException_WhenStorageTimesOut()
    {
        // SE.Redis 3.3.0 陷阱：RedisTimeoutException 继承 TimeoutException 而非 RedisException，
        // 包装面经 WechatRedisErrors.ShouldWrap 显式并入——超时同样映射为基础设施异常（Kind=Timeout）。
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisTimeoutException(CommandFlags.None, "redis timeout", CommandStatus.Unknown));
        var guard = CreateGuard(backend);

        var act = async () => await guard.TryMarkAsync(Fingerprint, TimeSpan.FromSeconds(300));
        var ex = (await act.Should().ThrowAsync<WechatCallbackInfrastructureException>()).Which;

        ex.InnerException.Should().BeOfType<WechatRedisException>()
            .Which.FailureKind.Should().Be(WechatRedisFailureKind.Timeout);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenArgumentsNull()
    {
        var act = () => new RedisWechatCallbackReplayGuard(null!, new WechatRedisOptions());
        act.Should().Throw<ArgumentNullException>();

        var act2 = () => new RedisWechatCallbackReplayGuard(new Mock<IConnectionMultiplexer>().Object, null!);
        act2.Should().Throw<ArgumentNullException>();
    }
}
