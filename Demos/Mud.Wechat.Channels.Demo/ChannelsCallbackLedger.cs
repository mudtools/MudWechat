// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Demo;

/// <summary>
/// 小店回调事件台账：把「推送确实穿透了整条接收链」变成可用 HTTP 查询的事实，只记非敏感字段。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要它</b>：小店回调中间件对「验签/时效/重放/AppId 失败」回 <c>403 + 空体</c>、
/// 对「正常」回 <c>200 + success</c>，两种形态都不告诉调用方<b>事件被哪个处理器消费</b>
/// （分发器对未命中处理器只记 Warning，应答照旧是 success）。没有台账，"跑通了" 与
/// "官方推来了但 Demo 没人接" 在浏览器里长得一模一样。
/// </para>
/// <para>
/// <b>为什么不存 <c>envelope.DecryptedXml</c></b>：该属性承载的是<b>解密后的明文 JSON</b>
/// （接口属性名是跨线统一命名，小店线实为 JSON），内含 openid 级 <c>FromUserName</c>、订单号等
/// 用户与交易数据 ⇒ 属敏感载体，不得整体进日志 / 遥测 / 异常消息，同样也不落本台账。
/// 台账只收白名单字段：事件键、应用键、到达时间、商品 ID 与操作码。
/// </para>
/// <para>
/// <b>容量有界</b>：官方失败重试最多 10 次、每 10 分钟一拨，无界队列在多日运行的 Demo 进程里会一直涨。
/// 超出容量丢最旧条目（台账是观测面，不是账本，丢旧不影响正确性）。
/// </para>
/// </remarks>
internal sealed class ChannelsCallbackLedger
{
    private const int Capacity = 200;

    private readonly object _gate = new();
    private readonly Queue<EventRecord> _records = new();
    private readonly Dictionary<string, int> _countsByEvent = new(StringComparer.Ordinal);

    /// <summary>登记一次事件消费（由处理器调用，故事件键即 <c>SupportedEventType</c> 命中值）。</summary>
    /// <param name="appKey">回调配置键（路由路径段）。</param>
    /// <param name="eventKey">事件键（明文顶层 <c>Event</c>）。</param>
    /// <param name="detail">非敏感摘要（如「product_id=…/status=0」），无则为空。</param>
    public void Record(string? appKey, string eventKey, string? detail)
    {
        lock (_gate)
        {
            _records.Enqueue(new EventRecord(
                appKey ?? string.Empty,
                eventKey,
                DateTimeOffset.UtcNow,
                string.IsNullOrWhiteSpace(detail) ? "(无结构化字段)" : detail));
            _countsByEvent[eventKey] = _countsByEvent.TryGetValue(eventKey, out var count) ? count + 1 : 1;
            while (_records.Count > Capacity)
            {
                _records.Dequeue();
            }
        }
    }

    /// <summary>最近事件列表（新→旧）。</summary>
    /// <param name="take">取用条数上限。</param>
    public IReadOnlyList<object> Recent(int take)
    {
        lock (_gate)
        {
            return _records.Reverse<EventRecord>()
                .Take(take)
                .Select(r => (object)new
                {
                    appKey = r.AppKey,
                    @event = r.EventKey,
                    receivedAtUtc = r.ReceivedAtUtc,
                    detail = r.Detail,
                })
                .ToList();
        }
    }

    /// <summary>事件键 → 累计条数。</summary>
    public IReadOnlyDictionary<string, int> Counts()
    {
        lock (_gate)
        {
            return new Dictionary<string, int>(_countsByEvent, StringComparer.Ordinal);
        }
    }

    /// <summary>判断指定商品 ID 是否已被消费过（供自测探针断言「载荷真的被类型化读出来了」）。</summary>
    /// <param name="productId">官方 <c>product_id</c> 值。</param>
    public bool ContainsProduct(string productId)
    {
        lock (_gate)
        {
            foreach (var record in _records)
            {
                if (record.Detail.Contains(productId, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private sealed record EventRecord(string AppKey, string EventKey, DateTimeOffset ReceivedAtUtc, string Detail);
}
