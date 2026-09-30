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
}
