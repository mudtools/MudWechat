// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 智能机器人长连接<b>心跳循环</b>（官方 101463 建议固定周期 <c>ping</c>；长时间无心跳服务端主动断开）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么提取为纯逻辑类</b>（方案 v1.1 D2/D13）：<c>ClientWebSocket</c> 不可伪造，连接层读循环无测试缝；
/// 「固定周期 / 与流量正交 / 取消即停」是 P0 缺陷 F1 的核心性质，必须以行为用例锁定，而非只留源码守卫。
/// </para>
/// <para>
/// <b>与流量正交</b>：周期由调用方固定给出，帧到达不得改期心跳 —— 一次性 <c>CancellationTokenSource</c>
/// 的取消不可逆（<c>CancelAfter</c> 只改期不撤销），流量驱动改期会把心跳「暂停到永远」（v1 方案 F1）。
/// </para>
/// </remarks>
internal static class WechatBotHeartbeat
{
    /// <summary>
    /// 按固定周期执行 <paramref name="sendPingAsync"/> 直到 <paramref name="token"/> 取消；
    /// 发送失败经 <paramref name="onSendFailure"/> 告警后退出（连接由读循环 / 服务端关闭）。
    /// </summary>
    /// <param name="interval">心跳周期（连接侧取 <c>WechatBotAppOptions.PingIntervalSeconds</c>，缺省官方建议 30s）。</param>
    /// <param name="sendPingAsync">发送一帧 ping（取消令牌随周期传递）。</param>
    /// <param name="onSendFailure">发送失败回调（异常 + 描述；仅告警不重试 —— 重试必超官方窗口）。</param>
    /// <param name="token">停止信号（连接侧在读循环 finally 取消；取消即静默退出，不抛）。</param>
    public static async Task RunAsync(
        TimeSpan interval,
        Func<CancellationToken, Task> sendPingAsync,
        Action<Exception, string> onSendFailure,
        CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, token).ConfigureAwait(false);
                await sendPingAsync(token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // 正常取消（读循环退出 / 停机）：静默退出。
                return;
            }
            catch (Exception ex)
            {
                onSendFailure(ex, "心跳发送失败（连接将被服务端或读循环关闭）。");
                return;
            }
        }
    }
}

#endif
