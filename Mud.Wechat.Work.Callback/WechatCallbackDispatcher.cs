// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调事件分发器（v1 方案 §5.4.3，对齐 <c>FeishuWebhookService.HandleEventWithInterceptorsAsync</c>
/// 并按企业微信契约裁剪）：
/// 事件族合法性闸（区分企业自建 / 第三方 / 代开发 × 回调通道的开放面）→ 拦截器 Before（appKey 专属先于全局；
/// 任一返回 false 即中断）→ 并发信号量 → 软超时 CTS → 匹配处理器（精确优先，兜底次之）→ 执行 →
/// 拦截器 After → 无匹配输出 unhandled 告警。
/// </summary>
/// <remarks>
/// <para>
/// <b>抗重放语义</b>：企业微信的一次性指纹去重在<b>接收器</b>完成（分发前），分发器不再去重——
/// 与飞书 <c>event_id</c> 两阶段去重语义不同，显式省略。指纹在分发前已消费，故「业务失败 → 503 → 重推」的
/// 重推报文与原报文同指纹时将被 403 拒绝：这是「一次消费、至多一次有效处理」的 fail-closed 取舍，
/// 503 重推主要为分发器瞬时故障（如软超时）留恢复窗口（v1 方案 §5.4.3 注）。
/// </para>
/// <para>
/// <b>处理器异常隔离</b>：单个处理器抛出非取消异常时记 <see cref="LogLevel.Error"/> 后继续其余处理器、
/// 分发结果仍为 <see cref="WechatCallbackDispatchOutcome.Handled"/>（重推无法通过去重，故障经日志暴露，
/// 处理器须自证幂等健壮）；取消异常（软超时/请求中止）向中间件传播（软超时 → 503）。
/// </para>
/// <para>
/// <b>实例生命周期</b>：处理器/拦截器实例在请求 scope 内解析（Transient/Singleton 由宿主注册决定）；
/// 处理器依赖缺失导致解析失败时按「跳过 + LogError」降级，不打断其余处理器。
/// </para>
/// </remarks>
public sealed class WechatCallbackDispatcher
{
    private readonly WechatCallbackHandlerRegistry _handlerRegistry;
    private readonly WechatCallbackInterceptorRegistry _interceptorRegistry;
    private readonly IWechatPayloadContractRegistry _payloadContracts;
    private readonly IWechatPayloadReader _payloadReader;
    private readonly IOptionsMonitor<WechatCallbackOptions> _optionsMonitor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WechatCallbackDispatcher> _logger;
    private readonly SemaphoreSlim _concurrencyGate;

    /// <summary>创建回调事件分发器。</summary>
    /// <param name="handlerRegistry">处理器注册表（组合根期急切填充）。</param>
    /// <param name="interceptorRegistry">拦截器注册表（组合根期急切填充）。</param>
    /// <param name="payloadContracts">事件键契约注册表（开放面闸 + 载荷分派）。</param>
    /// <param name="payloadReader">载荷读取器（按处理器声明的载荷类型读取）。</param>
    /// <param name="optionsMonitor">回调配置监视器（软超时/并发数热读取；并发容量为构造期快照，v1 方案 §10.8）。</param>
    /// <param name="scopeFactory">scope 工厂（处理器/拦截器实例解析；Singleton 防 captive dependency）。</param>
    /// <param name="logger">日志器。</param>
    public WechatCallbackDispatcher(
        WechatCallbackHandlerRegistry handlerRegistry,
        WechatCallbackInterceptorRegistry interceptorRegistry,
        IWechatPayloadContractRegistry payloadContracts,
        IWechatPayloadReader payloadReader,
        IOptionsMonitor<WechatCallbackOptions> optionsMonitor,
        IServiceScopeFactory scopeFactory,
        ILogger<WechatCallbackDispatcher> logger)
    {
        _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        _interceptorRegistry = interceptorRegistry ?? throw new ArgumentNullException(nameof(interceptorRegistry));
        _payloadContracts = payloadContracts ?? throw new ArgumentNullException(nameof(payloadContracts));
        _payloadReader = payloadReader ?? throw new ArgumentNullException(nameof(payloadReader));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var maxConcurrent = Math.Max(1, optionsMonitor.CurrentValue.MaxConcurrentEvents);
        _concurrencyGate = new SemaphoreSlim(maxConcurrent, maxConcurrent);
    }

    /// <summary>
    /// 分发回调事件（拦截器 → 并发闸 → 软超时 → 匹配 → 执行 → After 拦截器）。
    /// </summary>
    /// <param name="appKey">事件归属应用键（路由提取）。</param>
    /// <param name="evt">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌（链接请求中止；分发器内部再链接软超时 CTS）。</param>
    /// <returns>分发结果；软超时/请求中止以 <see cref="OperationCanceledException"/> 传播。</returns>
    public async Task<WechatCallbackDispatchOutcome> DispatchAsync(
        string appKey, WechatCallbackEvent evt, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            throw new ArgumentException("应用键不能为空", nameof(appKey));
        }

        if (evt == null)
        {
            throw new ArgumentNullException(nameof(evt));
        }

        var eventType = evt.EventTypeKey;

        // — 0. 事件族合法性闸（区分企业自建 / 第三方 / 代开发 × 回调通道的开放面）——
        // 授权族仅套件通道、上下游变更族仅自建 + 应用通道；不适用的事件族在此拒绝（返回 200，不触发重推）。
        var app = _optionsMonitor.CurrentValue.ResolveApp(appKey);
        if (app != null && !app.IsEventFamilyAllowed(evt.EventFamily))
        {
            _logger.LogWarning(
                "事件 {EventType} 的事件族 {EventFamily} 不适用于当前应用类型 {AppType} × 回调通道 {Channel}，" +
                "已拒绝接收（返回 200 不触发重推）。AppKey: {AppKey}",
                eventType, evt.EventFamily, app.AppType, app.Channel, appKey);
            return WechatCallbackDispatchOutcome.Rejected;
        }

        // — 0b. 事件键级闸（ADR-15，守卫 CB13b 断言其先于拦截器）——
        // 族级闸只按「事件族」判定；宿主注册新 Event 值会落 Unknown 族而被族闸放行，
        // 故此处按事件键的契约声明（族前置条件 + 应用模式/通道）再判一次。
        // 键未登记 ⇒ 落回族级闸结论（协议外报文不拦截，与 v1 行为一致）。
        if (app != null && _payloadContracts.TryResolve(eventType, out var contract) && contract != null)
        {
            if (!contract.IsOpenFor(evt, app.AppType, app.Channel))
            {
                _logger.LogWarning(
                    "事件 {EventType} 不适用于当前应用类型 {AppType} × 回调通道 {Channel}（事件键级开放面声明），" +
                    "已拒绝接收（返回 200 不触发重推）。AppKey: {AppKey}",
                    eventType, app.AppType, app.Channel, appKey);
                return WechatCallbackDispatchOutcome.Rejected;
            }
        }

        // — 1. 拦截器 Before（appKey 专属先于全局；异常传播 → 中间件 500） —
        using (var interceptorScope = _scopeFactory.CreateScope())
        {
            foreach (var interceptor in ResolveInterceptors(interceptorScope.ServiceProvider, appKey))
            {
                var proceed = await interceptor
                    .BeforeHandleAsync(eventType, evt, cancellationToken)
                    .ConfigureAwait(false);
                if (!proceed)
                {
                    _logger.LogInformation(
                        "事件 {EventType}（appKey: {AppKey}）被拦截器 {Interceptor} 中断，返回 503 触发重推。",
                        eventType, appKey, interceptor.GetType().FullName);
                    return WechatCallbackDispatchOutcome.Interrupted;
                }
            }
        }

        // — 2. 并发闸 + 软超时（必须 < 企业微信 5s 契约，v1.2 D7） —
        var timeoutMs = Math.Max(1, _optionsMonitor.CurrentValue.EventHandlingTimeoutMs);
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeoutMs);
        var dispatchToken = timeoutCts.Token;

        await _concurrencyGate.WaitAsync(dispatchToken).ConfigureAwait(false);
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var handlers = ResolveHandlers(scope.ServiceProvider, appKey, eventType);

            if (handlers.Count == 0)
            {
                _logger.LogWarning(
                    "未找到事件 {EventType}（appKey: {AppKey}）的处理器，事件已接收但未处理（unhandled）。",
                    eventType, appKey);
            }
            else
            {
                foreach (var handler in handlers)
                {
                    try
                    {
                        // 载荷感知处理器：先按声明的载荷类型读取（同一事件的多处理器共享一次节点投影，ADR-6）。
                        if (handler is IWechatCallbackPayloadHandler payloadHandler)
                        {
                            var read = _payloadReader.Read(evt, payloadHandler.PayloadType);
                            if (read.Status != WechatPayloadReadStatus.Matched || read.Payload == null)
                            {
                                _logger.LogError(
                                    "事件 {EventType} 的处理器 {Handler} 所需载荷 {PayloadType} 读取失败" +
                                    "（状态 {Status}{Diagnostic}），已跳过该处理器（宿主接线错误：请确认该事件键已登记" +
                                    "对应载荷类型的契约，或改用 GenericCallbackPayload）。AppKey: {AppKey}",
                                    eventType, handler.GetType().FullName, payloadHandler.PayloadType.Name,
                                    read.Status, read.Diagnostic == null ? string.Empty : "：" + read.Diagnostic,
                                    appKey);
                                continue;
                            }

                            await payloadHandler
                                .HandlePayloadAsync(evt, read.Payload, dispatchToken)
                                .ConfigureAwait(false);
                            continue;
                        }

                        await handler.HandleAsync(evt, dispatchToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) when (dispatchToken.IsCancellationRequested)
                    {
                        // 软超时或请求中止：向中间件传播（超时 → 503 → 企业微信重推）。
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "事件 {EventType} 的处理器 {Handler} 执行失败（已隔离，继续其余处理器）。",
                            eventType, handler.GetType().FullName);
                    }
                }
            }

            // — 3. 拦截器 After（后置审计/埋点；异常记日志不打断应答） —
            foreach (var interceptor in ResolveInterceptors(scope.ServiceProvider, appKey))
            {
                try
                {
                    await interceptor.AfterHandleAsync(eventType, evt, dispatchToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (dispatchToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "事件 {EventType} 的拦截器 {Interceptor} AfterHandle 失败（已忽略）。",
                        eventType, interceptor.GetType().FullName);
                }
            }

            return handlers.Count == 0
                ? WechatCallbackDispatchOutcome.Unhandled
                : WechatCallbackDispatchOutcome.Handled;
        }
        finally
        {
            _concurrencyGate.Release();
        }
    }

    /// <summary>
    /// 解析并匹配处理器（v1 方案 §5.4.2 顺序）：appKey 专属精确 → 全局精确 → appKey 专属兜底 → 全局兜底；
    /// 有任一精确命中即不执行兜底。
    /// </summary>
    /// <remarks>
    /// 匹配经 <see cref="IWechatCallbackEventHandler.SupportedEventType"/> 实例属性判定（无反射，AOT 安全）；
    /// 解析失败（依赖缺失）按「跳过 + LogError」降级。
    /// </remarks>
    private List<IWechatCallbackEventHandler> ResolveHandlers(IServiceProvider provider, string appKey, string eventType)
    {
        var exact = new List<IWechatCallbackEventHandler>();
        var fallback = new List<IWechatCallbackEventHandler>();

        foreach (var type in EnumerateRegistryTypes(_handlerRegistry, appKey))
        {
            IWechatCallbackEventHandler? handler;
            try
            {
                handler = provider.GetService(type) as IWechatCallbackEventHandler;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "处理器 {Type} 从 DI 解析失败（依赖缺失？），已跳过。", type.FullName);
                continue;
            }

            if (handler == null)
            {
                _logger.LogWarning("处理器类型 {Type} 未在 DI 注册，已跳过。", type.FullName);
                continue;
            }

            var supported = handler.SupportedEventType ?? string.Empty;
            if (supported.Length == 0)
            {
                fallback.Add(handler);
            }
            else if (string.Equals(supported, eventType, StringComparison.Ordinal))
            {
                exact.Add(handler);
            }
        }

        return exact.Count > 0 ? exact : fallback;
    }

    /// <summary>解析拦截器（appKey 专属先于全局；解析失败按「跳过 + LogError」降级）。</summary>
    private List<IWechatCallbackEventInterceptor> ResolveInterceptors(IServiceProvider provider, string appKey)
    {
        var interceptors = new List<IWechatCallbackEventInterceptor>();
        foreach (var type in EnumerateRegistryTypes(_interceptorRegistry, appKey))
        {
            IWechatCallbackEventInterceptor? interceptor;
            try
            {
                interceptor = provider.GetService(type) as IWechatCallbackEventInterceptor;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "拦截器 {Type} 从 DI 解析失败（依赖缺失？），已跳过。", type.FullName);
                continue;
            }

            if (interceptor != null)
            {
                interceptors.Add(interceptor);
            }
        }

        return interceptors;
    }

    /// <summary>按「appKey 专属 → 通配全局」顺序枚举注册表类型（D11）。</summary>
    private IEnumerable<Type> EnumerateRegistryTypes<T>(WechatCallbackTypeRegistry<T> registry, string appKey)
        where T : class
    {
        foreach (var type in registry.GetAll(appKey))
        {
            yield return type;
        }

        if (!string.Equals(appKey, WechatCallbackOptions.WildcardAppKey, StringComparison.Ordinal))
        {
            foreach (var type in registry.GetAll(WechatCallbackOptions.WildcardAppKey))
            {
                yield return type;
            }
        }
    }
}
