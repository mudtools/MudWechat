// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 支付通知处理器：接收<b>已验签且已解密</b>的通知上下文。
/// </summary>
/// <remarks>
/// <para>
/// <b>处理器必须幂等</b>：官方通知为 at-least-once（HTTP 200 未被及时收到即重试），
/// 故须按 <c>out_trade_no</c> / <c>out_refund_no</c> 幂等落库（方案 §2.6「业务幂等」）。
/// </para>
/// <para>
/// <b>指纹在分发前消费</b>：同一密文的重复投递会被第 ③ 道闸以 403 拒绝（fail-closed 优先于
/// at-least-once）；处理器抛出的异常<b>不会</b>让指纹回滚，故重试同指纹仍被拒 ——
/// 这正是「处理器须幂等」之外、还须「尽量不抛」的原因。
/// </para>
/// <para><b>红线</b>：处理器不得把 <see cref="WechatPayCallbackContext.ResourceJson"/>（含 <c>openid</c> 等）写日志。</para>
/// </remarks>
public interface IWechatPayNotificationHandler
{
    /// <summary>处理通知。</summary>
    /// <param name="context">已验签、已解密的通知上下文。</param>
    /// <param name="cancellationToken">取消令牌（分发软超时 / 请求中止）。</param>
    /// <returns>处理任务。</returns>
    Task HandleAsync(WechatPayCallbackContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// 通知处理器注册表（按 <c>event_type</c> 分桶；通配桶对所有事件生效）。
/// </summary>
/// <remarks>
/// 组合根期<b>急切注册</b>、无 Freeze —— 与企微 / 公众号两条线的注册表同形态。
/// 解析顺序：事件精确桶 → 通配桶（两者可叠加，先精确后通配）。
/// </remarks>
public sealed class WechatPayCallbackHandlerRegistry
{
    /// <summary>通配事件类型（对所有 <c>event_type</c> 生效）。</summary>
    public const string WildcardEventType = "*";

    private readonly Dictionary<string, List<Type>> _byEventType = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>登记处理器类型。</summary>
    /// <param name="eventType">事件类型（<c>TRANSACTION.SUCCESS</c> 等；通配用 <see cref="WildcardEventType"/>）。</param>
    /// <param name="handlerType">处理器类型（须实现 <see cref="IWechatPayNotificationHandler"/>）。</param>
    /// <exception cref="ArgumentException"><paramref name="eventType"/> 为空白。</exception>
    /// <exception cref="ArgumentNullException"><paramref name="handlerType"/> 为 <c>null</c>。</exception>
    public void Register(string eventType, Type handlerType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("事件类型不可为空白（通配请用 \"*\")。", nameof(eventType));
        }

        if (handlerType is null)
        {
            throw new ArgumentNullException(nameof(handlerType));
        }

        if (!_byEventType.TryGetValue(eventType, out var list))
        {
            list = new List<Type>();
            _byEventType[eventType] = list;
        }

        list.Add(handlerType);
    }

    /// <summary>解析某事件类型应执行的处理器（精确桶在前、通配桶在后）。</summary>
    /// <param name="eventType">通知中的 <c>event_type</c>（可为 <c>null</c>）。</param>
    /// <returns>处理器类型序列（无匹配时为空）。</returns>
    public IReadOnlyList<Type> Resolve(string? eventType)
    {
        var result = new List<Type>();

        if (!string.IsNullOrWhiteSpace(eventType)
            && _byEventType.TryGetValue(eventType!, out var exact))
        {
            result.AddRange(exact);
        }

        if (_byEventType.TryGetValue(WildcardEventType, out var wildcard))
        {
            result.AddRange(wildcard);
        }

        return result;
    }
}
