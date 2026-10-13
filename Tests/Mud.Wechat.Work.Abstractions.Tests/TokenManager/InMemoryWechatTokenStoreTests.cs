// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

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
