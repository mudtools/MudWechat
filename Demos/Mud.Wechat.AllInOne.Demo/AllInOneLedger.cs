// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.AllInOne.Demo;

/// <summary>
/// 跨线回调台账：四条线共用一份「谁被调用、留下什么非敏感摘要」的观测面。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么一份台账而不是每线一份</b>：单线 Demo 里台账的作用是「区分『处理好了』与『没人接』」；
/// 同宿主里它多担一个责任 —— 证明<b>报文落到了正确的那条线</b>。六线各自的应答三态口径不同
/// （企微 403 带语义、小店 403 空体、开放平台只认响应体 <c>success</c>、机器人成功恒 200），
/// 光看状态码判断不了「这条推送被哪条线接走」，台账的 <c>line</c> 字段才是判定依据。
/// </para>
/// <para>
/// <b>只收白名单字段</b>：各线信封上的解密明文属性（企微 <c>DecryptedXml</c>、小店同名属性实为明文 JSON、
/// 支付 <c>ResourceJson</c>）都承载 openid / 订单号级数据，属红线字段 ⇒ 一律不入台账、不进日志、
/// 不进异常消息。台账条目只有线名、应用键、事件键、宿主自造的摘要与到达时间。
/// </para>
/// <para>
/// <b>容量有界</b>：官方失败重试最多 10 次、每 10 分钟一拨，无界队列在多日运行的进程里会一直涨；
/// 超容量丢最旧条目（观测面不是账本）。
/// </para>
/// </remarks>
internal sealed class AllInOneLedger
{
    private const int Capacity = 200;

    private readonly object _gate = new();
    private readonly Queue<Entry> _entries = new();
    private readonly Dictionary<string, Dictionary<string, int>> _counts = new(StringComparer.Ordinal);

    /// <summary>登记一次事件消费。</summary>
    /// <param name="line">线名（work / mp / pay / channels / openplatform）。</param>
    /// <param name="appKey">回调配置键（路由路径段）。</param>
    /// <param name="eventKey">事件键或消息类型（各线口径不同，原样记录）。</param>
    /// <param name="detail">宿主自制的非敏感摘要（如 <c>product_id=…</c>、<c>msgid=…</c>）。</param>
    public void Record(string line, string? appKey, string eventKey, string? detail)
    {
        lock (_gate)
        {
            _entries.Enqueue(new Entry(line, appKey ?? string.Empty, eventKey, DateTimeOffset.UtcNow, detail ?? "(无结构化字段)"));

            var byEvent = _counts.TryGetValue(line, out var existing)
                ? existing
                : (_counts[line] = new Dictionary<string, int>(StringComparer.Ordinal));
            byEvent[eventKey] = byEvent.TryGetValue(eventKey, out var count) ? count + 1 : 1;

            while (_entries.Count > Capacity)
            {
                _entries.Dequeue();
            }
        }
    }

    /// <summary>快照：按线分组的计数 + 最近条目（新→旧）。</summary>
    /// <param name="take">最近条目条数上限（调用方负责夹紧）。</param>
    public IReadOnlyDictionary<string, object> Snapshot(int take)
    {
        lock (_gate)
        {
            return new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["countsByLine"] = _counts.ToDictionary(
                    static kv => kv.Key,
                    static kv => (object)new Dictionary<string, int>(kv.Value, StringComparer.Ordinal),
                    StringComparer.Ordinal),
                ["recent"] = _entries
                    .Reverse<Entry>()
                    .Take(take)
                    .Select(static e => (object)new
                    {
                        line = e.Line,
                        appKey = e.AppKey,
                        @event = e.EventKey,
                        receivedAtUtc = e.ReceivedAtUtc,
                        detail = e.Detail,
                    })
                    .ToList(),
            };
        }
    }

    private sealed record Entry(string Line, string AppKey, string EventKey, DateTimeOffset ReceivedAtUtc, string Detail);
}
