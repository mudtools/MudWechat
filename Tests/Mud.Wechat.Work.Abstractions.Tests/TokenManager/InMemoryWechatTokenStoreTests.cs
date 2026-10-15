// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Abstractions.Tests.TokenManager;

/// <summary>
/// 进程内令牌仓储（P1-9）：过期条目在<b>读路径</b>回收，消除无界驻留（含令牌明文）。
/// </summary>
public class InMemoryWechatTokenStoreTests
{
    [Fact]
    public async Task GetAccessTokenAsync_ShouldReturnToken_WhenNotExpired()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "token-a", 7200);

        (await store.GetAccessTokenAsync("Wechat.AccessToken:a:default")).Should().Be("token-a");
    }

    [Fact]
    public async Task GetAccessTokenAsync_ShouldEvict_WhenExpired()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "token-a", 0);

        (await store.GetAccessTokenAsync("Wechat.AccessToken:a:default")).Should().BeNull();

        (await store.GetTokenTypesAsync()).Should().BeEmpty(
            "P1-9：过期条目必须在读路径即回收（原实现静默驻留 ⇒ 长时间运行无界增长）");
    }

    [Fact]
    public async Task GetTokenTypesAsync_ShouldReflectSetAndRemove()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("k1", "t1", 7200);
        await store.SetAccessTokenAsync("k2", "t2", 7200);

        (await store.GetTokenTypesAsync()).Should().BeEquivalentTo("k1", "k2");

        await store.RemoveAsync("k1");
        (await store.GetTokenTypesAsync()).Should().Equal(new[] { "k2" });
    }

    // ---------------------------------------------------------------- W1（M10）：批量删除能力

    [Fact]
    public async Task RemoveRangeAsync_ShouldRemoveOnlyRequestedKeys_AndReturnCount()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:c1", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:c2", "t2", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:b:default", "t3", 7200);

        var removed = await store.RemoveRangeAsync(new[] { "Wechat.AccessToken:a:c1", "Wechat.AccessToken:a:c2", "missing-key" });

        removed.Should().Be(2, "返回实际删除数量（不存在的键不计入）");
        (await store.GetTokenTypesAsync()).Should().Equal(new[] { "Wechat.AccessToken:b:default" },
            "其它应用的键不得被误删");
    }

    [Fact]
    public async Task RemoveRangeAsync_ShouldReturnZero_WhenKeysEmpty()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("k1", "t1", 7200);

        var removed = await store.RemoveRangeAsync(Array.Empty<string>());

        removed.Should().Be(0);
        (await store.GetTokenTypesAsync()).Should().BeEquivalentTo("k1");
    }

    // ---------------------------------------------------------------- U-4：按应用清库（官方 ITokenStoreAppPurge）

    [Fact]
    public void InMemoryStore_ShouldImplementOfficialAppPurgeInterface()
    {
        var store = new InMemoryWechatTokenStore();

        store.Should().BeAssignableTo<ITokenStoreAppPurge>(
            "U-4：按应用清库能力须由存储实现承担（管理器三层探测的 tier-1；InMemory 与 Redis 两实现同批）");
    }

    [Fact]
    public async Task EnumerateKeysAsync_ShouldReturnOnlyTargetAppKeys()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:corp:123", "t2", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:ab:default", "t3", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:b:default", "t4", 7200);

        var keys = await store.EnumerateKeysAsync("a");

        keys.Should().BeEquivalentTo(new[]
        {
            "Wechat.AccessToken:a:default",
            "Wechat.AccessToken:a:corp:123",
        }, "前缀兄弟应用（ab）与其它应用（b）不得混入；scope 含 ':' 的键须按第二段边界完整返回");
    }

    [Fact]
    public async Task EnumerateKeysAsync_ShouldIgnoreMalformedKeys()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a", "t1", 7200);
        await store.SetAccessTokenAsync("no-colon", "t2", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken::default", "t3", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t4", 7200);

        var keys = await store.EnumerateKeysAsync("a");

        keys.Should().Equal(new[] { "Wechat.AccessToken:a:default" },
            "缺第二段分隔符 / 无 tokenType 段 / appKey 段为空的畸形键不参与按应用匹配（与管理器 TryParseStoreKey 语义一致）");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task EnumerateKeysAsync_ShouldReturnEmpty_WhenAppKeyBlank(string? appKey)
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t1", 7200);

        var keys = await store.EnumerateKeysAsync(appKey!);

        keys.Should().BeEmpty("本院键空间无官方 shared 式共享段约定 ⇒ 空白 appKey 返回空集");
    }

    [Fact]
    public async Task PurgeAppAsync_ShouldRemoveOnlyTargetAppKeys_AndReturnCount()
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t1", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:corp:123", "t2", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:ab:default", "t3", 7200);
        await store.SetAccessTokenAsync("Wechat.AccessToken:b:default", "t4", 7200);

        var removed = await store.PurgeAppAsync("a");

        removed.Should().Be(2, "返回实际删除键数（快照内全部命中键）");
        (await store.GetTokenTypesAsync()).Should().BeEquivalentTo(new[]
        {
            "Wechat.AccessToken:ab:default",
            "Wechat.AccessToken:b:default",
        }, "前缀兄弟应用（ab）与其它应用（b）的键不得被误删（按第二段精确比对）");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task PurgeAppAsync_ShouldReturnZeroAndKeepStore_WhenAppKeyBlank(string? appKey)
    {
        var store = new InMemoryWechatTokenStore();
        await store.SetAccessTokenAsync("Wechat.AccessToken:a:default", "t1", 7200);

        var removed = await store.PurgeAppAsync(appKey!);

        removed.Should().Be(0);
        (await store.GetTokenTypesAsync()).Should().Equal(new[] { "Wechat.AccessToken:a:default" },
            "空白 appKey 不得清空整库（本院无共享段，按「无命中」处置）");
    }

    // ---------------------------------------------------------------- P2-7：容量上限 + 惰性驱逐

    [Fact]
    public async Task SetAccessTokenAsync_ShouldEvictOldest_WhenCapacityExceeded()
    {
        var store = new InMemoryWechatTokenStore();

        // P2-7：读路径只回收被访问键 ⇒「写入后不再被读取」的键（如企业卸载后的 scope 槽位）会永久驻留。
        // 容量上限 8192；此处写入 8300 条，且 expiresIn 递增使「最早到期」为确定性的最旧项。
        const int writes = 8300;
        for (var i = 0; i < writes; i++)
        {
            await store.SetAccessTokenAsync($"k-{i}", $"t-{i}", 7200 + i);
        }

        (await store.GetAccessTokenAsync("k-0")).Should().BeNull(
            "P2-7：超限时必须驱逐最早到期项（防写入后不再被读取的槽位无界驻留）");
        (await store.GetAccessTokenAsync($"k-{writes - 1}")).Should().Be($"t-{writes - 1}",
            "最新写入的槽位必须保留");
    }
}
