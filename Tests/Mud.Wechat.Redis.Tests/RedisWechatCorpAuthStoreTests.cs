// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// T-R3：企业授权仓储（复合键转义 / JSON 往返 / MGET 批读 / 损坏数据脱敏）。
/// </summary>
public class RedisWechatCorpAuthStoreTests
{
    private static WechatCorpAuthorization CreateAuth(string appKey = "suite1", string authCorpId = "corp1")
        => new()
        {
            AppKey = appKey,
            AuthCorpId = authCorpId,
            PermanentCode = "permanent-code-value",
            AuthMode = 0,
            UpdatedAt = 1_700_000_000_000,
        };

    private static RedisWechatCorpAuthStore CreateStore(FakeRedisBackend backend)
        => new(backend.Multiplexer.Object, new WechatRedisOptions());

    [Fact]
    public async Task SetAsync_ShouldWriteJsonAtCompositeKey()
    {
        var backend = new FakeRedisBackend();
        await CreateStore(backend).SetAsync(CreateAuth());

        backend.Store.Should().ContainKey("wechat:corpa:suite1:corp1");
        backend.Store["wechat:corpa:suite1:corp1"].ToString().Should().Contain("permanent-code-value");
    }

    [Fact]
    public async Task GetAsync_ShouldRoundTripAggregate()
    {
        var backend = new FakeRedisBackend();
        var store = CreateStore(backend);
        await store.SetAsync(CreateAuth());

        var loaded = await store.GetAsync("suite1", "corp1");

        loaded.Should().NotBeNull();
        loaded!.AppKey.Should().Be("suite1");
        loaded.AuthCorpId.Should().Be("corp1");
        loaded.PermanentCode.Should().Be("permanent-code-value");
        loaded.UpdatedAt.Should().Be(1_700_000_000_000);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenKeyMissing()
    {
        var backend = new FakeRedisBackend();
        (await CreateStore(backend).GetAsync("suite1", "corp1")).Should().BeNull();
    }

    [Fact]
    public async Task CompositeSegments_ShouldBeEscaped_AndIsolateFromSiblingKeys()
    {
        var backend = new FakeRedisBackend();
        var store = CreateStore(backend);
        await store.SetAsync(CreateAuth(appKey: "a:b", authCorpId: "corp1"));
        await store.SetAsync(CreateAuth(appKey: "a", authCorpId: "b:corp1"));

        backend.Store.Keys.Should().Contain(new[]
        {
            @"wechat:corpa:a\:b:corp1",
            @"wechat:corpa:a:b\:corp1",
        });

        (await store.GetAsync("a:b", "corp1"))!.PermanentCode.Should().Be("permanent-code-value");
        (await store.GetAsync("a", "b:corp1"))!.PermanentCode.Should().Be("permanent-code-value");
    }

    [Fact]
    public async Task SetAsync_ShouldThrowArgumentNullException_WhenAuthNull()
    {
        var backend = new FakeRedisBackend();
        var act = async () => await CreateStore(backend).SetAsync(null!);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RemoveAsync_ShouldDeleteCompositeKey()
    {
        var backend = new FakeRedisBackend();
        var store = CreateStore(backend);
        await store.SetAsync(CreateAuth());

        await store.RemoveAsync("suite1", "corp1");

        (await store.GetAsync("suite1", "corp1")).Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_ShouldScanAndMgetInBatch()
    {
        var backend = new FakeRedisBackend();
        var store = CreateStore(backend);
        await store.SetAsync(CreateAuth(appKey: "suite1", authCorpId: "corp1"));
        await store.SetAsync(CreateAuth(appKey: "suite1", authCorpId: "corp2"));
        await store.SetAsync(CreateAuth(appKey: "suite2", authCorpId: "corp3"));

        var list = await store.ListAsync("suite1");

        list.Should().HaveCount(2);
        list.Select(a => a.AuthCorpId).Should().BeEquivalentTo("corp1", "corp2");
        backend.Database.Verify(
            d => d.StringGetAsync(It.IsAny<RedisKey[]>(), It.IsAny<CommandFlags>()),
            Times.Once, "SCAN 后应一次 MGET 批读，避免 N+1 往返");
    }

    [Fact]
    public async Task ListAsync_ShouldReturnEmpty_WhenNoAuthForKey()
    {
        var backend = new FakeRedisBackend();
        (await CreateStore(backend).ListAsync("suite1")).Should().BeEmpty();
    }

    [Fact]
    public async Task GetAsync_ShouldThrowWithKeyNameOnly_WhenStoredJsonCorrupted()
    {
        // 聚合含永久授权码：损坏数据只报键名，绝不携带 JSON 内容（§三 安全行）。
        var backend = new FakeRedisBackend();
        backend.SetRaw("wechat:corpa:suite1:corp1", "{ corrupted json with permanent-code-value");

        var act = async () => await CreateStore(backend).GetAsync("suite1", "corp1");
        var ex = (await act.Should().ThrowAsync<WechatRedisException>()).Which;

        ex.FailureKind.Should().Be(WechatRedisFailureKind.Server);
        ex.Message.Should().Contain("wechat:corpa:suite1:corp1");
        ex.Message.Should().NotContain("permanent-code-value");
    }
}
