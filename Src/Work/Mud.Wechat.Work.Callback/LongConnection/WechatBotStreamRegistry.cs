// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using System.Collections.Concurrent;

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 长连接<b>流式状态机</b>（官方 101463：同一 <c>stream.id</c> 首次发送创建、重复发送刷新、
/// <c>finish=true</c> 结束；从首次发送起须在 <b>10 分钟</b>内完成，否则自动结束）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么长连接需要本状态机而回调模式不需要</b>（方案 §6.3 的两种相反语义）：回调地址模式下企微
/// <b>反复推送</b> <c>msgtype=stream</c> 刷新报文、宿主对每次推送写回累计内容 —— 状态归宿主；
/// 长连接模式下<b>开发者主动推</b>刷新，且官方要求「同一次回调的所有流式回复须用相同 <c>req_id</c>」⇒
/// SDK 必须记账 <c>stream.id → 首次发送的 req_id</c> 与 10 分钟窗口，宿主才能只凭 <c>stream.id</c> 刷新。
/// </para>
/// <para>
/// 本状态机<b>仅长连接模式</b>使用；<c>finish</c> 后不得再刷（官方流已终结）。
/// </para>
/// <para>
/// <b>条目保留语义（F4，架构审查报告 v2 P2-2）</b>：条目（无论是否 finish）自首次发送起
/// <b>窗口 + 宽限（1 分钟）</b>后可被机会式清扫（<see cref="BeginOrRefresh"/> 入口触发、
/// 至多每 60 秒一轮）。宽限期内行为与「永不清理」逐字节一致（finish 后再刷仍抛「已终结」、
/// 超窗仍抛「超时窗口」）；超出保留窗后同 id 再刷按<b>新流</b>处理（返回新 req_id）——
/// 此时官方 10 分钟窗口早已关闭、官方已自动结束该流，任何有界机制都无法把「新流」与
/// 「历史流残迹」区分（SDK 不可能无限期记住全部历史 id）。注册表随连接对象存活
/// （应用生命周期单例），无保留窗即无界增长。
/// </para>
/// </remarks>
internal sealed class WechatBotStreamRegistry
{
    /// <summary>官方窗口：从首次发送起 10 分钟内须 <c>finish=true</c>。</summary>
    internal const int StreamWindowMinutes = 10;

    /// <summary>清扫保留宽限（分钟）：窗口关闭后再保留这么久，供「已终结/超窗」语义在宽限期内原样可观测。</summary>
    internal const int RetentionSlackMinutes = 1;

    private static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(60);

    private static readonly TimeSpan RetentionLimit = TimeSpan.FromMinutes(StreamWindowMinutes + RetentionSlackMinutes);

    private readonly ConcurrentDictionary<string, StreamEntry> _streams = new(StringComparer.Ordinal);
    private readonly Func<DateTimeOffset> _utcNow;
    private long _lastSweepTicks;

    /// <summary>创建流式状态机。</summary>
    /// <param name="utcNow">时钟缝（测试注入）。</param>
    public WechatBotStreamRegistry(Func<DateTimeOffset>? utcNow = null)
        => _utcNow = utcNow ?? (() => DateTimeOffset.UtcNow);

    /// <summary>记账一次流式发送：首次创建（返回要使用的 req_id）、后续刷新（返回首次的 req_id 并校验窗口）。</summary>
    /// <param name="streamId">官方 <c>stream.id</c>（非空）。</param>
    /// <param name="reqId">本次发送帧的 req_id（首次发送时被记账为该流的固定 req_id）。</param>
    /// <param name="finish">本次是否为终结发送（终结后流不得再刷）。</param>
    /// <returns>本次发送应使用的 req_id（首次 = <paramref name="reqId"/>，刷新 = 首次的 req_id）。</returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="streamId"/> 为空；或该流已 <c>finish</c>；或已超出 10 分钟窗口。</exception>
    /// <remarks>超出<b>保留窗</b>（窗口 + 宽限）的同 id 按新流处理，见类型 remarks。</remarks>
    public string BeginOrRefresh(string streamId, string reqId, bool finish)
    {
        if (string.IsNullOrEmpty(streamId))
        {
            throw new InvalidOperationException("流式应答必须携带 stream.id（官方：首次回复时必须设置自定义唯一 id）。");
        }

        var now = _utcNow();
        SweepExpiredIfNeeded(now);
        while (true)
        {
            if (_streams.TryGetValue(streamId, out var entry))
            {
                if (entry.Finished)
                {
                    throw new InvalidOperationException(
                        $"流式应答 stream.id={streamId} 已 finish 终结（官方：终结后不得再刷新；请换新 stream.id 开启新流）。");
                }

                if (now - entry.FirstSentAt > TimeSpan.FromMinutes(StreamWindowMinutes))
                {
                    _streams.TryRemove(streamId, out _);
                    throw new InvalidOperationException(
                        $"流式应答 stream.id={streamId} 已超出 {StreamWindowMinutes} 分钟窗口（官方：从首次发送起须在 10 分钟内 finish=true，超时由官方自动结束）。" +
                        "请换新 stream.id 开启新流。");
                }

                var reusedReqId = entry.ReqId;
                if (finish)
                {
                    entry.Finished = true;
                }

                return reusedReqId;
            }

            var created = new StreamEntry(reqId, now);
            if (finish)
            {
                created.Finished = true;
            }

            if (_streams.TryAdd(streamId, created))
            {
                return reqId;
            }

            // 并发首次发送同一 stream.id：走下一轮按「已存在」分支收敛（复用先到者的 req_id）。
        }
    }

    /// <summary>取某流的关联 req_id（宿主主动刷新时使用；流不存在返回 <c>null</c>）。</summary>
    public string? TryGetReqId(string streamId)
        => _streams.TryGetValue(streamId, out var entry) && !entry.Finished ? entry.ReqId : null;

    /// <summary>当前在册条目数（活跃流 + 保留窗内残迹；诊断用）。</summary>
    public int Count => _streams.Count;

    /// <summary>机会式清扫（F4）：节流为至多每 <see cref="SweepInterval"/> 一轮；多线程下偶尔多扫一轮无害。</summary>
    private void SweepExpiredIfNeeded(DateTimeOffset now)
    {
        var nowTicks = now.UtcTicks;
        if (!ShouldSweep(nowTicks, Interlocked.Read(ref _lastSweepTicks)))
        {
            return;
        }

        Interlocked.Exchange(ref _lastSweepTicks, nowTicks);

        foreach (var pair in _streams)
        {
            if (now - pair.Value.FirstSentAt > RetentionLimit)
            {
                _streams.TryRemove(pair.Key, out _);
            }
        }
    }

    /// <summary>清扫节流判据（抽为纯函数供单测：60 秒内至多一轮；<c>lastTicks=0</c> 时首轮必扫）。</summary>
    internal static bool ShouldSweep(long nowTicks, long lastTicks)
        => nowTicks - lastTicks >= SweepInterval.Ticks;

    private sealed class StreamEntry(string reqId, DateTimeOffset firstSentAt)
    {
        public string ReqId { get; } = reqId;

        public DateTimeOffset FirstSentAt { get; } = firstSentAt;

        public volatile bool Finished;
    }
}

#endif
