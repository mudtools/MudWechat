// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Bots;

namespace Mud.Wechat.AI.AgentDemo;

/// <summary>
/// 长连接租约的<b>进程内</b>实现：让单实例宿主能装配起长连接面（Demo 用，不防跨实例）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么 Demo 要自己写一个</b>：Runner 的构造参数 <see cref="IWechatBotConnectionLease"/> 声明为可选
/// （<c>null</c> = 按「进程内唯一」假设运行并在启动期告警一次），但 ASP.NET Core 在 Development 环境下
/// 默认 <c>ValidateOnBuild=true</c> —— 容器在建容器阶段就要求每个构造依赖<b>可解析</b>，
/// 「留空 + 运行期告警」这条被 SDK 文档承诺的降级路径在开发机上根本走不到（建连开关为 <c>false</c> 也拦，
/// 因为校验只看构造签名、不看运行期是否会真的建连）。全仓唯一实现来自可选的 <c>Mud.Wechat.Redis</c>，
/// 于是「不引 Redis 的单实例宿主」在 Development 下无法装配长连接 —— 这是本轮实测暴露的第三处装配缺口。
/// </para>
/// <para>
/// <b>能力边界（必须在读代码时就看懂）</b>：状态只在进程内 ⇒ 只保证「同进程同 BotKey 一份连接」，
/// <b>不保证</b>官方「每个机器人同一时间仅一条有效连接」—— 多实例部署必须换成
/// <c>RedisWechatBotConnectionLease</c>（<c>AddWechatRedis</c>），否则各实例各自夺「自己的」租约、
/// 照样互相踢下线。真实生产宿主不要把本类当分布式租约用。
/// </para>
/// </remarks>
internal sealed class InProcessBotConnectionLease : IWechatBotConnectionLease
{
    private readonly object _gate = new();

    private readonly Dictionary<string, Slot> _slots = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public Task<bool> TryAcquireAsync(
        string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default)
        => Task.FromResult(Mutate(botKey, current =>
        {
            // 自己已持有 ⇒ 续期即可（幂等重入）；他人持有且未过期 ⇒ 让位。
            if (current == null || current.InstanceId == instanceId || current.ExpiresAt <= TimeNow())
            {
                return (new Slot(instanceId, TimeNow().Add(ttl)), true);
            }

            return (current, false);
        }));

    /// <inheritdoc />
    public Task<bool> RenewAsync(
        string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default)
        => Task.FromResult(Mutate(botKey, current =>
            current != null && current.InstanceId == instanceId && current.ExpiresAt > TimeNow()
                ? (new Slot(instanceId, TimeNow().Add(ttl)), true)
                : (current, false)));

    /// <inheritdoc />
    public Task<bool> ReleaseAsync(string botKey, string instanceId, CancellationToken cancellationToken = default)
        => Task.FromResult(Mutate(botKey, current =>
        {
            if (current != null && current.InstanceId == instanceId)
            {
                return (null, true);
            }

            return (current, false);
        }));

    private static DateTimeOffset TimeNow() => DateTimeOffset.UtcNow;

    /// <summary>读取 → 判定 → 写回，全程在锁内（夺约/续租是「比较后再交换」，无锁会双持有）。</summary>
    private bool Mutate(string botKey, Func<Slot?, (Slot? Next, bool Allowed)> decide)
    {
        lock (_gate)
        {
            _slots.TryGetValue(botKey, out var current);
            var (next, allowed) = decide(current);

            if (next == null)
            {
                _slots.Remove(botKey);
            }
            else
            {
                _slots[botKey] = next;
            }

            return allowed;
        }
    }

    private sealed record Slot(string InstanceId, DateTimeOffset ExpiresAt);
}
