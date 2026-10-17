// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.Wechat.Work.Abstractions.Callback.Bots;

namespace Mud.Wechat.AI.AgentDemo;

/// <summary>
/// 智能机器人回调台账：记录「哪条事件被哪个处理器接住、回了什么形态」，只存非敏感字段。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要它</b>：机器人通道的应答在成功路径上恒为 <c>200</c> + 加密外壳，
/// 「处理器返回了消息」与「返回了空包」与「无人接线」在三态里<b>看不出差别</b>
/// —— 与 XML 侧的 <c>success</c> 明文同一个观测盲区。台账把这一面补成可读的。
/// </para>
/// <para>
/// <b>敏感面</b>：<see cref="WechatBotCallbackEvent.DecryptedJson"/> 与消息正文一律不落账
/// （对齐 AGENTS §8：解密明文不得进日志 / 遥测 / 异常消息）；只记事件键、msgid、会话类型与应答形态。
/// </para>
/// </remarks>
internal sealed class BotEventLedger
{
    private const int Capacity = 200;

    private readonly ConcurrentQueue<BotEventRecord> _records = new();
    private readonly ConcurrentDictionary<string, int> _counts = new(StringComparer.Ordinal);
    private readonly object _trimGate = new();

    /// <summary>记一条事件（处理器侧唯一写入点）。</summary>
    public void Record(BotEventRecord record)
    {
        if (record is null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        _records.Enqueue(record);
        _counts.AddOrUpdate(record.EventKey, 1, static (_, current) => current + 1);

        // 有界保留：Demo 长跑时不让队列无上限增长（丢弃顺序 = 到达顺序，最早的一条先出）。
        lock (_trimGate)
        {
            while (_records.Count > Capacity && _records.TryDequeue(out _))
            {
            }
        }
    }

    /// <summary>按事件键计数（快照）。</summary>
    public IReadOnlyDictionary<string, int> Counts() => new Dictionary<string, int>(_counts);

    /// <summary>最近若干条（按到达顺序，最多 <paramref name="take"/> 条）。</summary>
    public IReadOnlyList<BotEventRecord> Recent(int take)
    {
        var all = _records.ToArray();
        return take >= all.Length ? all : all[^take..];
    }
}

/// <summary>
/// 一条机器人事件的观测记录（应答形态在返回式处理器里是显式事实，不需要猜）。
/// </summary>
/// <param name="EventKey">事件键（消息族取 msgtype、事件族取 event.eventtype）。</param>
/// <param name="AppKey">回调配置键（机器人键）。</param>
/// <param name="MsgId">官方 msgid（可能重复推送，SDK 不做去重）。</param>
/// <param name="ChatType">会话类型（single / group，可空）。</param>
/// <param name="ReplyShape">应答形态：<c>空包 {}</c> 或具体 msgtype。</param>
internal sealed record BotEventRecord(
    string EventKey,
    string? AppKey,
    string? MsgId,
    string? ChatType,
    string ReplyShape)
{
    public DateTimeOffset ArrivedAt { get; init; } = DateTimeOffset.UtcNow;
}
