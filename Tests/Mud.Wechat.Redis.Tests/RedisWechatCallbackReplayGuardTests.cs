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
    public async Task TryMarkAsync_ShouldRethrowClassifiedException_WhenRedisFails()
    {
        // RD3 fail-closed：异常上抛，绝不吞成 true/false（吞 true 放行重放；吞 false 丢事件）。
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisConnectionException(ConnectionFailureType.SocketFailure, "redis down"));
        var guard = CreateGuard(backend);

        var act = async () => await guard.TryMarkAsync(Fingerprint, TimeSpan.FromSeconds(300));
        var ex = (await act.Should().ThrowAsync<WechatRedisException>()).Which;

        ex.FailureKind.Should().Be(WechatRedisFailureKind.Connection);
        ex.Message.Should().Contain(RedisKey);
        ex.Message.Should().NotContain("secret");
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
