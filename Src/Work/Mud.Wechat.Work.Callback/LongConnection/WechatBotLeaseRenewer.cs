// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using Mud.Wechat.Work.Abstractions.Callback.Bots;

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 智能机器人长连接<b>租约续租循环</b>（端口 <see cref="IWechatBotConnectionLease.RenewAsync"/> 的唯一调用点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有</b>（v1 方案 F4，P0）：租约以 45s TTL 夺取后若无续租，健康长连接会在 45s 后静默失去租约，
/// 备实例随即夺租建连并踢掉现连接（官方「新连接完成订阅即踢旧连接」）⇒ 多实例部署两实例每 ~45s 乒乓一次，
/// 期间官方只推一次的回调丢失。端口 <see cref="IWechatBotConnectionLease"/> 的注释语义「失去租约（续租失败）时
/// 立即放弃当前连接并回到夺取流程」由此兑现。
/// </para>
/// <para>
/// <b>续租周期 = TTL/3</b>（45s 租约 ⇒ 15s 续租，容忍两次瞬时失败仍远小于租约寿命）。
/// 续租被拒抛 <see cref="LeaseLostException"/>（预期控制流）；其余异常（如 Redis 抖动）原样上抛，
/// 由 <c>WechatBotConnection.RunAsync</c> 统一记 Error 并退避重夺。
/// </para>
/// </remarks>
internal static class WechatBotLeaseRenewer
{
    /// <summary>
    /// 按 <paramref name="ttl"/> 的三分之一周期续租，直到 <paramref name="token"/> 取消（静默退出，不抛）。
    /// </summary>
    /// <param name="lease">连接租约端口。</param>
    /// <param name="botKey">机器人键（租约分槽键）。</param>
    /// <param name="instanceId">实例标识（持有者凭据）。</param>
    /// <param name="ttl">租约时长（与夺取时一致）。</param>
    /// <param name="token">停止信号（会话结束 / 停机）。</param>
    /// <exception cref="LeaseLostException">续租被拒（租约已易主）。</exception>
    public static async Task RunAsync(
        IWechatBotConnectionLease lease, string botKey, string instanceId,
        TimeSpan ttl, CancellationToken token)
    {
        var period = TimeSpan.FromTicks(ttl.Ticks / 3);
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(period, token).ConfigureAwait(false);
                if (!await lease.RenewAsync(botKey, instanceId, ttl, token).ConfigureAwait(false))
                {
                    throw new LeaseLostException(botKey);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // 取消（会话结束 / 停机）= 静默退出。
                return;
            }
        }
    }
}

#endif
