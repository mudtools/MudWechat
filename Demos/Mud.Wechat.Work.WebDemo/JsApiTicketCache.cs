// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.WebDemo;

/// <summary>
/// jsapi_ticket 的<b>宿主侧</b>缓存（SDK 不提供 ticket 缓存端口，官方频率上限要求宿主自己缓存）。
/// </summary>
/// <remarks>
/// <para>
/// 官方对获取 jsapi_ticket 的限制是「一小时内一个企业最多 400 次、<b>单个应用不超过 100 次</b>」，
/// 而每次页面加载都要签名 —— 不缓存的 Demo 很快会把应用配额打光并得到官方限流错误，
/// 表现为「刚跑通就忽然一直 invalid signature」，误导用户去查签名算法。
/// </para>
/// <para>
/// 提前 <see cref="RenewMargin"/> 秒刷新（而非到期瞬间），避免签名请求正好撞在过期边界上；
/// 单飞闸（<see cref="_gate"/>）保证并发请求只有一次真正打官方接口。
/// </para>
/// </remarks>
internal sealed class JsApiTicketCache
{
    private const int RenewMargin = 300;

    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Dictionary<string, (string Ticket, DateTimeOffset ExpiresAt)> _entries = new();

    /// <summary>取缓存的 ticket；缺失或临近过期时经 <paramref name="fetch"/> 重新获取。</summary>
    /// <param name="slot">缓存槽位（企业 ticket 与应用 ticket 有效期独立，须分槽）。</param>
    /// <param name="fetch">实际取票委托，返回官方 ticket 与 <c>expires_in</c>。</param>
    public async Task<(string Ticket, DateTimeOffset ExpiresAt)> GetOrCreateAsync(
        string slot,
        Func<CancellationToken, Task<(string Ticket, int ExpiresIn)>> fetch,
        CancellationToken cancellationToken)
    {
        lock (_entries)
        {
            if (_entries.TryGetValue(slot, out var hit) && hit.ExpiresAt > DateTimeOffset.UtcNow.AddSeconds(RenewMargin))
            {
                return hit;
            }
        }

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 双重检查：等锁期间可能已被并发请求刷新。
            lock (_entries)
            {
                if (_entries.TryGetValue(slot, out var hit) && hit.ExpiresAt > DateTimeOffset.UtcNow.AddSeconds(RenewMargin))
                {
                    return hit;
                }
            }

            var (ticket, expiresIn) = await fetch(cancellationToken).ConfigureAwait(false);
            var entry = (ticket, DateTimeOffset.UtcNow.AddSeconds(expiresIn > 0 ? expiresIn : 7200));
            lock (_entries)
            {
                _entries[slot] = entry;
            }

            return entry;
        }
        finally
        {
            _gate.Release();
        }
    }
}
