// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R4：套件票据仓储（TTL 决策 RD5 / 票据值不落日志 / 分槽键布局）。
/// </summary>
public class RedisWechatSuiteTicketStoreTests
{
    private const string SuiteId = "ww-suite-1";
    private const string RedisKey = "wechat:ticket:ww-suite-1";

    private static RedisWechatSuiteTicketStore CreateStore(FakeRedisBackend backend, TimeSpan? ttl = null)
        => new(backend.Multiplexer.Object, new WechatRedisOptions { SuiteTicketTtl = ttl ?? TimeSpan.Zero });

    [Fact]
    public async Task GetAsync_ShouldUseSuiteSlottedKey()
    {
        var backend = new FakeRedisBackend();
        await CreateStore(backend).GetAsync(SuiteId);

        backend.Database.Verify(d => d.StringGetAsync(RedisKey, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenMissing()
    {
        var backend = new FakeRedisBackend();
        (await CreateStore(backend).GetAsync(SuiteId)).Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldWriteWithoutTtl_WhenSuiteTicketTtlIsZero()
    {
        // RD5：默认不过期（对齐 InMemory 永驻 + 覆盖写保新鲜）。
        var backend = new FakeRedisBackend();
        await CreateStore(backend).SetAsync(SuiteId, "ticket-value");

        backend.LastExpiry.Should().BeNull("默认 SuiteTicketTtl=Zero 应写为无 TTL 键");
        backend.Store[RedisKey].ToString().Should().Be("ticket-value");
    }

    [Fact]
    public async Task SetAsync_ShouldWriteWithConfiguredTtl_WhenPositive()
    {
        var backend = new FakeRedisBackend();
        await CreateStore(backend, TimeSpan.FromMinutes(20)).SetAsync(SuiteId, "ticket-value");

        backend.LastExpiry.Should().Be(TimeSpan.FromMinutes(20));
    }

    [Fact]
    public async Task SetAsync_ShouldNotLogTicketValue()
    {
        var backend = new FakeRedisBackend();
        var logger = new CapturingLogger();
        await new RedisWechatSuiteTicketStore(backend.Multiplexer.Object, new WechatRedisOptions(), logger)
            .SetAsync(SuiteId, "secret-ticket-value");

        logger.Messages.Should().NotBeEmpty("正常路径有键名日志");
        logger.Messages.Should().OnlyContain(text => !text.Contains("secret-ticket-value"), "票据值不得落日志");
    }

    /// <summary>手写捕获日志器（Moq 泛型 Log 捕获形态易碎，直接实现接口）。</summary>
    private sealed class CapturingLogger : ILogger<RedisWechatSuiteTicketStore>
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            => Messages.Add(formatter(state, exception));
    }

    [Fact]
    public async Task Operations_ShouldThrowClassifiedException_WhenConnectionFails()
    {
        var backend = new FakeRedisBackend();
        backend.ThrowOn(() => new RedisConnectionException(ConnectionFailureType.SocketFailure, "boom"));
        var store = CreateStore(backend);

        (await ((Func<Task>)(async () => await store.GetAsync(SuiteId))).Should().ThrowAsync<WechatRedisException>())
            .Which.FailureKind.Should().Be(WechatRedisFailureKind.Connection);
        (await ((Func<Task>)(async () => await store.SetAsync(SuiteId, "t"))).Should().ThrowAsync<WechatRedisException>())
            .Which.FailureKind.Should().Be(WechatRedisFailureKind.Connection);
    }
}
