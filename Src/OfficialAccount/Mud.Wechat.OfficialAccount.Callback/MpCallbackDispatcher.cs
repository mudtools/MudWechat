// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>处理器注册表（键 = 应用键或通配键）。</summary>
public sealed class MpCallbackHandlerRegistry : WechatCallbackTypeRegistry<IMpCallbackEventHandler>
{
}

/// <summary>拦截器注册表（键 = 应用键或通配键）。</summary>
public sealed class MpCallbackInterceptorRegistry : WechatCallbackTypeRegistry<IMpCallbackEventInterceptor>
{
}

/// <summary>被动回复处理器注册表（键 = 应用键或通配键）。</summary>
public sealed class MpCallbackReplyHandlerRegistry : WechatCallbackTypeRegistry<IMpCallbackReplyHandler>
{
}

/// <summary>分发结果（结局 + 被动回复体）。</summary>
public sealed class MpCallbackDispatchResult
{
    /// <summary>创建分发结果。</summary>
    /// <param name="outcome">分发结局。</param>
    /// <param name="reply">被动回复体；<c>null</c> = 无回复（回明文 <c>success</c>）。</param>
    public MpCallbackDispatchResult(WechatCallbackDispatchOutcome outcome, MpCallbackReply? reply)
    {
        Outcome = outcome;
        Reply = reply;
    }

    /// <summary>分发结局。</summary>
    public WechatCallbackDispatchOutcome Outcome { get; }

    /// <summary>被动回复体（无回复为 <c>null</c>）。</summary>
    public MpCallbackReply? Reply { get; }
}

/// <summary>
/// 公众号回调事件分发器：拦截器 Before → 并发闸 → 软超时 → 匹配处理器（精确优先、兜底次之）→ 执行 →
/// 拦截器 After → 被动回复。
/// </summary>
/// <remarks>
/// <para>
/// <b>与企微分发器的差异</b>：公众号无「事件族合法性闸」与「事件键级开放面」（无对应官方语义），
/// 故<b>没有</b> gate 段；超时/中断出口交由中间件按公众号语义映射为明文 <c>success</c> + 200。
/// </para>
/// <para>
/// <b>处理器异常隔离</b>：单处理器抛非取消异常时记 <see cref="LogLevel.Error"/> 后继续其余处理器；
/// 取消异常（软超时/请求中止）向外传播由中间件处置。
/// </para>
/// </remarks>
public sealed class MpCallbackDispatcher
{
    private readonly MpCallbackHandlerRegistry _handlerRegistry;
    private readonly MpCallbackInterceptorRegistry _interceptorRegistry;
    private readonly MpCallbackReplyHandlerRegistry _replyRegistry;
    private readonly IMpPayloadReader _payloadReader;
    private readonly IOptionsMonitor<MpCallbackOptions> _optionsMonitor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MpCallbackDispatcher> _logger;
    private readonly SemaphoreSlim _concurrencyGate;

    /// <summary>创建分发器。</summary>
    /// <param name="handlerRegistry">处理器注册表。</param>
    /// <param name="interceptorRegistry">拦截器注册表。</param>
    /// <param name="replyRegistry">被动回复处理器注册表。</param>
    /// <param name="payloadReader">载荷读取器。</param>
    /// <param name="optionsMonitor">配置监视器（软超时热读取；并发容量为构造期快照）。</param>
    /// <param name="scopeFactory">scope 工厂（处理器/拦截器实例解析）。</param>
    /// <param name="logger">日志器。</param>
    public MpCallbackDispatcher(
        MpCallbackHandlerRegistry handlerRegistry,
        MpCallbackInterceptorRegistry interceptorRegistry,
        MpCallbackReplyHandlerRegistry replyRegistry,
        IMpPayloadReader payloadReader,
        IOptionsMonitor<MpCallbackOptions> optionsMonitor,
        IServiceScopeFactory scopeFactory,
        ILogger<MpCallbackDispatcher> logger)
    {
        _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        _interceptorRegistry = interceptorRegistry ?? throw new ArgumentNullException(nameof(interceptorRegistry));
        _replyRegistry = replyRegistry ?? throw new ArgumentNullException(nameof(replyRegistry));
        _payloadReader = payloadReader ?? throw new ArgumentNullException(nameof(payloadReader));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var maxConcurrent = Math.Max(1, optionsMonitor.CurrentValue.MaxConcurrentEvents);
        _concurrencyGate = new SemaphoreSlim(maxConcurrent, maxConcurrent);
    }

    /// <summary>分发回调事件。</summary>
    /// <param name="appKey">事件归属应用键（路由提取）。</param>
    /// <param name="envelope">回调事件信封。</param>
    /// <param name="cancellationToken">取消令牌（分发器内部再链接软超时 CTS）。</param>
    /// <returns>分发结果（含被动回复体）；软超时/请求中止以 <see cref="OperationCanceledException"/> 传播。</returns>
    public async Task<MpCallbackDispatchResult> DispatchAsync(
        string appKey, MpCallbackEnvelope envelope, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            throw new ArgumentException("应用键不能为空", nameof(appKey));
        }

        if (envelope == null)
        {
            throw new ArgumentNullException(nameof(envelope));
        }

        var eventType = envelope.EventTypeKey;

        // — 1. 拦截器 Before（appKey 专属先于全局；异常传播 → 中间件 500）—
        using (var interceptorScope = _scopeFactory.CreateScope())
        {
            foreach (var interceptor in ResolveInterceptors(interceptorScope.ServiceProvider, appKey))
            {
                var proceed = await interceptor
                    .BeforeHandleAsync(eventType, envelope, cancellationToken)
                    .ConfigureAwait(false);
                if (!proceed)
                {
                    _logger.LogInformation(
                        "事件 {EventType}（appKey: {AppKey}）被拦截器 {Interceptor} 中断，按公众号语义回 success（不触发重推）。",
                        eventType, appKey, interceptor.GetType().FullName);
                    return new MpCallbackDispatchResult(WechatCallbackDispatchOutcome.Interrupted, null);
                }
            }
        }

        // — 2. 并发闸 + 软超时（必须 < 平台 5s 契约）—
        var timeoutMs = Math.Max(1, _optionsMonitor.CurrentValue.EventHandlingTimeoutMs);
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeoutMs);
        var dispatchToken = timeoutCts.Token;

        await _concurrencyGate.WaitAsync(dispatchToken).ConfigureAwait(false);
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var handlers = ResolveHandlers(scope.ServiceProvider, appKey, eventType);
            var handled = false;

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
                        if (handler is IMpCallbackPayloadHandler payloadHandler)
                        {
                            var read = _payloadReader.Read(envelope, payloadHandler.PayloadType);
                            if ((read.Status != MpPayloadReadStatus.Matched
                                 && read.Status != MpPayloadReadStatus.GenericFallback)
                                || read.Payload == null)
                            {
                                _logger.LogError(
                                    "事件 {EventType} 的处理器 {Handler} 所需载荷 {PayloadType} 读取失败" +
                                    "（状态 {Status}{Diagnostic}），已跳过该处理器（请确认该事件键已登记契约或改用 GenericCallbackPayload）。" +
                                    "AppKey: {AppKey}",
                                    eventType, handler.GetType().FullName, payloadHandler.PayloadType.Name, read.Status,
                                    read.Diagnostic == null ? string.Empty : "：" + read.Diagnostic, appKey);
                                continue;
                            }

                            await payloadHandler
                                .HandlePayloadAsync(envelope, read.Payload, dispatchToken)
                                .ConfigureAwait(false);
                            handled = true;
                            continue;
                        }

                        await handler.HandleAsync(envelope, dispatchToken).ConfigureAwait(false);
                        handled = true;
                    }
                    catch (OperationCanceledException) when (dispatchToken.IsCancellationRequested)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "事件 {EventType} 的处理器 {Handler} 执行失败（已隔离，继续其余处理器）。",
                            eventType, handler.GetType().FullName);
                        handled = true;
                    }
                }
            }

            // — 3. 被动回复（处理器返回 null 即无回复，由中间件回明文 success）—
            var reply = await TryBuildReplyAsync(scope.ServiceProvider, appKey, envelope, dispatchToken)
                .ConfigureAwait(false);

            // — 4. 拦截器 After（观测；异常记日志不打断应答）—
            foreach (var interceptor in ResolveInterceptors(scope.ServiceProvider, appKey))
            {
                try
                {
                    await interceptor.AfterHandleAsync(eventType, envelope, dispatchToken).ConfigureAwait(false);
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

            var outcome = handled
                ? WechatCallbackDispatchOutcome.Handled
                : WechatCallbackDispatchOutcome.Unhandled;
            return new MpCallbackDispatchResult(outcome, reply);
        }
        finally
        {
            _concurrencyGate.Release();
        }
    }

    private async Task<MpCallbackReply?> TryBuildReplyAsync(
        IServiceProvider provider, string appKey, MpCallbackEnvelope envelope, CancellationToken cancellationToken)
    {
        foreach (var type in EnumerateWithWildcard(_replyRegistry, appKey))
        {
            IMpCallbackReplyHandler? handler;
            try
            {
                handler = provider.GetService(type) as IMpCallbackReplyHandler;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                _logger.LogError(ex, "被动回复处理器 {Type} 从 DI 解析失败（依赖缺失？），已跳过。", type.FullName);
                continue;
            }

            if (handler == null)
            {
                continue;
            }

            var reply = await handler.TryReplyAsync(envelope, cancellationToken).ConfigureAwait(false);
            if (reply != null)
            {
                return reply;
            }
        }

        return null;
    }

    /// <summary>解析并匹配处理器：appKey 专属精确 → 全局精确 → appKey 专属兜底 → 全局兜底。</summary>
    private List<IMpCallbackEventHandler> ResolveHandlers(IServiceProvider provider, string appKey, string eventType)
    {
        var exact = new List<IMpCallbackEventHandler>();
        var fallback = new List<IMpCallbackEventHandler>();

        foreach (var type in EnumerateWithWildcard(_handlerRegistry, appKey))
        {
            IMpCallbackEventHandler? handler;
            try
            {
                handler = provider.GetService(type) as IMpCallbackEventHandler;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
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

    private List<IMpCallbackEventInterceptor> ResolveInterceptors(IServiceProvider provider, string appKey)
    {
        var interceptors = new List<IMpCallbackEventInterceptor>();
        foreach (var type in EnumerateWithWildcard(_interceptorRegistry, appKey))
        {
            IMpCallbackEventInterceptor? interceptor;
            try
            {
                interceptor = provider.GetService(type) as IMpCallbackEventInterceptor;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
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

    private static IEnumerable<Type> EnumerateWithWildcard<T>(WechatCallbackTypeRegistry<T> registry, string appKey)
        where T : class
    {
        foreach (var type in registry.GetAll(appKey))
        {
            yield return type;
        }

        if (!string.Equals(appKey, MpCallbackOptions.WildcardAppKey, StringComparison.Ordinal))
        {
            foreach (var type in registry.GetAll(MpCallbackOptions.WildcardAppKey))
            {
                yield return type;
            }
        }
    }
}
