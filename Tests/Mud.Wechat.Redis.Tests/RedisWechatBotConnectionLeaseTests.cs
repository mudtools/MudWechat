// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using StackExchange.Redis;

namespace Mud.Wechat.Redis.Tests;

/// <summary>
/// 智能机器人长连接租约的 Redis 集成测试（NX 抢占 / Lua 续租与释放 / 非持有者失败）。
/// </summary>
/// <remarks>
/// 环境变量 <c>WECHAT_REDIS_TESTS_CONNECTION</c> 门控（缺失即跳过断言直接返回）——
/// 与 <see cref="RedisIntegrationTests"/> 同款形态；CI 默认无 Redis。
/// </remarks>
public class RedisWechatBotConnectionLeaseTests
{
    private const string ConnectionEnvVar = "WECHAT_REDIS_TESTS_CONNECTION";

    [Fact]
    public async Task TryAcquire_ShouldBeExclusive_AndRenewRelease_ShouldBeHolderOnly()
    {
        if (!Environment.GetEnvironmentVariable(ConnectionEnvVar)?.Equals("1", StringComparison.Ordinal) ?? true)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ConnectionEnvVar)))
            {
                return; // 门控跳过：CI 无 Redis 服务
            }
        }

        var connection = Environment.GetEnvironmentVariable(ConnectionEnvVar)!;
        await using var multiplexer = await ConnectionMultiplexer.ConnectAsync(connection);
        var lease = new RedisWechatBotConnectionLease(
            multiplexer, new WechatRedisOptions { KeyPrefix = "wechat-it" });

        var botKey = "lease-it-" + Guid.NewGuid().ToString("N");

        try
        {
            // ① 首夺成功；第二实例被 NX 拒绝。
            (await lease.TryAcquireAsync(botKey, "instance-a", TimeSpan.FromSeconds(10))).Should().BeTrue();
            (await lease.TryAcquireAsync(botKey, "instance-b", TimeSpan.FromSeconds(10))).Should().BeFalse(
                "官方 101463 每机器人仅一条有效连接 ⇒ 租约互斥是主备切换的基座");

            // ② 非持有者续租/释放失败（值不等 ⇒ Lua 返回 0）。
            (await lease.RenewAsync(botKey, "instance-b", TimeSpan.FromSeconds(10))).Should().BeFalse();
            (await lease.ReleaseAsync(botKey, "instance-b")).Should().BeFalse();

            // ③ 持有者续租成功；释放后其他实例可夺取。
            (await lease.RenewAsync(botKey, "instance-a", TimeSpan.FromSeconds(10))).Should().BeTrue();
            (await lease.ReleaseAsync(botKey, "instance-a")).Should().BeTrue();
            (await lease.TryAcquireAsync(botKey, "instance-b", TimeSpan.FromSeconds(10))).Should().BeTrue();
        }
        finally
        {
            await lease.ReleaseAsync(botKey, "instance-b");
        }
    }
}
