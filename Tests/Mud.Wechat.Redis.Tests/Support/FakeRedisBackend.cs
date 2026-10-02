// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// 进程内 Redis 桩（T-R2/T-R5/T-R6 共用）：以字典承载键值，Moq 装配
/// <c>IConnectionMultiplexer</c> / <c>IDatabase</c> / <c>IServer</c> 三件套。
/// </summary>
/// <remarks>
/// 匹配语义为最小 glob（<c>pattern*</c> 前缀匹配 / 全等）——仅服务 SCAN 前缀模式的测试诉求，
/// 不复刻 stringmatchlen 全量语义（全量语义在 T-R9 真实 Redis 集成用例覆盖）。
/// </remarks>
internal sealed class FakeRedisBackend
{
    private readonly Dictionary<string, RedisValue> _store = new(StringComparer.Ordinal);

    /// <summary>连接多路复用器桩。</summary>
    public Mock<IConnectionMultiplexer> Multiplexer { get; } = new();

    /// <summary>数据库桩（承载键值语义）。</summary>
    public Mock<IDatabase> Database { get; } = new();

    /// <summary>服务器桩（承载 SCAN 语义）。</summary>
    public Mock<IServer> Server { get; } = new();

    /// <summary>当前存储内容快照（测试断言用）。</summary>
    public IReadOnlyDictionary<string, RedisValue> Store => _store;

    /// <summary>最后一次 StringSet 的 TTL（任一重载统一记录；未调用为 null）。</summary>
    public TimeSpan? LastExpiry { get; private set; }

    /// <summary>最后一次 StringSet 的 When 语义（任一重载统一记录）。</summary>
    public When LastWhen { get; private set; } = When.Always;

    /// <summary>直接写入原始键值（损坏数据等边界构造用）。</summary>
    public void SetRaw(string key, RedisValue value) => _store[key] = value;

    public FakeRedisBackend(IEnumerable<string>? seedKeys = null)
    {
        foreach (var key in seedKeys ?? Enumerable.Empty<string>())
        {
            _store[key] = "seed";
        }

        Database.Setup(d => d.Database).Returns(0);
        Database.Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
                _store.TryGetValue(key.ToString(), out var value) ? value : RedisValue.Null);
        Database.Setup(d => d.StringGetAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) => keys
                .Select(k => _store.TryGetValue(k.ToString(), out var v) ? v : RedisValue.Null)
                .ToArray());
        // 生产代码的 StringSetAsync 在 SE.Redis 3.3.0 的多套重载间按重载决胜绑定（4/5/6 参并存），
        // 桩对三套重载全部 Setup 并统一记录，使测试对实际绑定的重载不敏感。
        Database.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>()))
            .Callback((RedisKey key, RedisValue value, TimeSpan? expiry, When when) => Record(key, value, expiry, when))
            .ReturnsAsync(true);
        Database.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .Callback((RedisKey key, RedisValue value, TimeSpan? expiry, When when, CommandFlags _) => Record(key, value, expiry, when))
            .ReturnsAsync(true);
        Database.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .Callback((RedisKey key, RedisValue value, TimeSpan? expiry, bool _, When when, CommandFlags _) => Record(key, value, expiry, when))
            .ReturnsAsync(true);

        Database.Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .Callback((RedisKey key, CommandFlags _) => _store.Remove(key.ToString()))
            .ReturnsAsync(true);
        Database.Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey[] keys, CommandFlags _) =>
            {
                var removed = 0;
                foreach (var key in keys)
                {
                    if (_store.Remove(key.ToString()))
                    {
                        removed++;
                    }
                }

                return removed;
            });

        Server.Setup(s => s.IsConnected).Returns(true);
        Server.Setup(s => s.IsReplica).Returns(false);
        Server.Setup(s => s.KeysAsync(
                It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(),
                It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns((int _, RedisValue pattern, int _, long _, int _, CommandFlags _) =>
                ToAsyncKeys(MatchKeys(pattern.ToString())));

        Multiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(Database.Object);
        Multiplexer.Setup(m => m.GetEndPoints(It.IsAny<bool>()))
            .Returns(new EndPoint[] { new DnsEndPoint("localhost", 6379) });
        Multiplexer.Setup(m => m.GetServer(It.IsAny<EndPoint>(), It.IsAny<object>())).Returns(Server.Object);
    }

    /// <summary>统一记录 StringSet 语义（任一重载落点一致）。</summary>
    private void Record(RedisKey key, RedisValue value, TimeSpan? expiry, When when)
    {
        _store[key.ToString()] = value;
        LastExpiry = expiry;
        LastWhen = when;
    }

    /// <summary>使存储路径抛出指定 Redis 异常（异常分类用例用）。</summary>
    public void ThrowOn(Func<Exception> exceptionFactory)
    {
        Database.Setup(d => d.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(exceptionFactory());
        Database.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>()))
            .ThrowsAsync(exceptionFactory());
        Database.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(exceptionFactory());
        Database.Setup(d => d.StringSetAsync(
                It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(exceptionFactory());
        Database.Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(exceptionFactory());
        Database.Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()))
            .ThrowsAsync(exceptionFactory());
        Server.Setup(s => s.KeysAsync(
                It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(),
                It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Throws(exceptionFactory());
    }

    private IEnumerable<RedisKey> MatchKeys(string pattern)
    {
        if (pattern.EndsWith("*"))
        {
            var prefix = pattern[..^1];
            return _store.Keys
                .Where(key => key.StartsWith(prefix, StringComparison.Ordinal))
                .Select(key => (RedisKey)key);
        }

        return _store.Keys
            .Where(key => string.Equals(key, pattern, StringComparison.Ordinal))
            .Select(key => (RedisKey)key);
    }

    private static async IAsyncEnumerable<RedisKey> ToAsyncKeys(IEnumerable<RedisKey> keys)
    {
        await Task.CompletedTask;
        foreach (var key in keys)
        {
            yield return key;
        }
    }
}
