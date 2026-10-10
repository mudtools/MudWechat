// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback;

/// <summary>通知分发结果（由中间件映射为 HTTP 应答）。</summary>
public enum WechatPayCallbackDispatchOutcome
{
    /// <summary>匹配到处理器并已全部执行（含单处理器异常被隔离）→ 200 SUCCESS。</summary>
    Handled = 0,

    /// <summary>无匹配处理器（已告警）→ 200 SUCCESS（通知已接收，官方不应重推）。</summary>
    Unhandled = 1,

    /// <summary>分发软超时 → 5xx（触发官方重试）。</summary>
    TimedOut = 2,
}

/// <summary>
/// 支付通知分发器：按 <c>event_type</c> 解析处理器、在<b>独立 DI 作用域</b>内执行、软超时收敛、单处理器异常隔离。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何开作用域</b>：处理器常需 <c>DbContext</c> 等 scoped 服务；从根容器直接解析会在
/// <c>ValidateScopes=true</c> 下抛错（否则产生单例捕获 scoped 的生命周期缺陷）。
/// </para>
/// <para>
/// <b>单处理器异常隔离</b>：任一处理器抛错只记 Error 并继续后续处理器（对齐企微 / 公众号线），
/// 避免一个订阅者的缺陷让整封通知变成 5xx。
/// </para>
/// <para>
/// <b>软超时</b>：平台 5 秒断连契约不可协商，故在 <c>EventHandlingTimeoutMs</c>（&lt;5000ms）到点即收敛为
/// <see cref="WechatPayCallbackDispatchOutcome.TimedOut"/>。注意此时<b>指纹已被消费</b>
/// （闸③ 在分发前）：官方重试会被判重放而 403 —— 这是「fail-closed 优先于 at-least-once」的既知取舍，
/// 也是处理器必须<b>快速返回</b>的原因（重活请落队列异步做）。
/// </para>
/// <para>请求中止（客户端断开）<b>不</b>算软超时，原样上抛 <see cref="OperationCanceledException"/>。</para>
/// </remarks>
public sealed class WechatPayCallbackDispatcher
{
    private readonly WechatPayCallbackHandlerRegistry _registry;
    private readonly IServiceProvider _services;
    private readonly IOptionsMonitor<WechatPayCallbackOptions> _optionsMonitor;
    private readonly ILogger<WechatPayCallbackDispatcher>? _logger;

    /// <summary>创建分发器。</summary>
    /// <param name="registry">处理器注册表。</param>
    /// <param name="services">根服务提供器（为每条通知开独立作用域）。</param>
    /// <param name="optionsMonitor">回调配置监视器（软超时热更）。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <exception cref="ArgumentNullException">任一必填参数为 <c>null</c>。</exception>
    public WechatPayCallbackDispatcher(
        WechatPayCallbackHandlerRegistry registry,
        IServiceProvider services,
        IOptionsMonitor<WechatPayCallbackOptions> optionsMonitor,
        ILogger<WechatPayCallbackDispatcher>? logger = null)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _logger = logger;
    }

    /// <summary>分发一条通知。</summary>
    /// <param name="context">已验签、已解密的通知上下文。</param>
    /// <param name="cancellationToken">请求中止令牌。</param>
    /// <returns>分发结果。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 <c>null</c>。</exception>
    /// <exception cref="OperationCanceledException">请求中止（非软超时）时抛出。</exception>
    public async Task<WechatPayCallbackDispatchOutcome> DispatchAsync(
        WechatPayCallbackContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var handlers = _registry.Resolve(context.EventType);
        if (handlers.Count == 0)
        {
            _logger?.LogInformation(WechatPayCallbackLogEvents.Unhandled,
                "无匹配处理器（事件已接收并应答 SUCCESS）。MerchantKey: {MerchantKey}, EventType: {EventType}",
                context.MerchantKey, context.EventType);
            return WechatPayCallbackDispatchOutcome.Unhandled;
        }

        using var timeoutCts = new CancellationTokenSource(_optionsMonitor.CurrentValue.EventHandlingTimeoutMs);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        var token = linkedCts.Token;

        using var scope = _services.CreateScope();

        foreach (var handlerType in handlers)
        {
            try
            {
                var handler = (IWechatPayNotificationHandler)scope.ServiceProvider.GetRequiredService(handlerType);
                await handler.HandleAsync(context, token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                // 软超时（请求未中止 ⇒ 取消只可能来自本方法的定时器）。
                _logger?.LogWarning(WechatPayCallbackLogEvents.DispatchTimedOut,
                    "通知分发软超时（{Timeout}ms），应答 5xx 触发官方重试。" +
                    "注意：指纹已消费，重试将被判重放 —— 处理器须快速返回。MerchantKey: {MerchantKey}, EventType: {EventType}",
                    _optionsMonitor.CurrentValue.EventHandlingTimeoutMs, context.MerchantKey, context.EventType);

                return WechatPayCallbackDispatchOutcome.TimedOut;
            }
            catch (OperationCanceledException)
            {
                // 请求中止：上抛交由中间件静默结束（不写应答）。
                throw;
            }
            catch (Exception ex)
            {
                // 单处理器异常隔离：记 Error 后继续后续处理器（对齐企微 / 公众号线）。
                _logger?.LogError(WechatPayCallbackLogEvents.HandlerFailed, ex,
                    "通知处理器执行失败（已隔离，继续后续处理器）。Handler: {Handler}, MerchantKey: {MerchantKey}, EventType: {EventType}",
                    handlerType.Name, context.MerchantKey, context.EventType);
            }
        }

        return WechatPayCallbackDispatchOutcome.Handled;
    }
}
