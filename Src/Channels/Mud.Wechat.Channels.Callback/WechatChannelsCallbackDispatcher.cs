// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.DataModels.Callback;

namespace Mud.Wechat.Channels.Callback;

/// <summary>处理器注册表（键 = 应用键或通配键）。</summary>
public sealed class ChannelsCallbackHandlerRegistry : WechatCallbackTypeRegistry<IChannelsCallbackEventHandler>
{
}

/// <summary>拦截器注册表（键 = 应用键或通配键）。</summary>
public sealed class ChannelsCallbackInterceptorRegistry : WechatCallbackTypeRegistry<IChannelsCallbackEventInterceptor>
{
}

/// <summary>结构化应答处理器注册表（键 = 应用键或通配键；仅官方要求同步应答的事件需要注册）。</summary>
public sealed class ChannelsCallbackReplyRegistry : WechatCallbackTypeRegistry<IChannelsCallbackReplyHandler>
{
}

/// <summary>分发结果（结局 + 结构化应答；应答为 <c>null</c> 时中间件回明文 success）。</summary>
public sealed class WechatChannelsCallbackDispatchResult
{
    /// <summary>创建分发结果。</summary>
    /// <param name="outcome">分发结局。</param>
    /// <param name="reply">结构化应答；<c>null</c> = 无应答。</param>
    public WechatChannelsCallbackDispatchResult(WechatCallbackDispatchOutcome outcome, ChannelsCallbackReply? reply)
    {
        Outcome = outcome;
        Reply = reply;
    }

    /// <summary>分发结局。</summary>
    public WechatCallbackDispatchOutcome Outcome { get; }

    /// <summary>结构化应答（无应答为 <c>null</c>）。</summary>
    public ChannelsCallbackReply? Reply { get; }
}

/// <summary>
/// 微信小店回调事件分发器：拦截器 Before → 并发闸 → 软超时 → 匹配处理器（精确优先、兜底次之）→
/// 类型化载荷反序列化（net8+ 且事件键已登记契约）→ 执行 → 拦截器 After。
/// </summary>
/// <remarks>
/// <para>
/// <b>与公众号分发器的差异</b>：小店无被动回复语义（官方应答恒为明文 success / 空串），故无回复注册表；
/// 类型化载荷经契约注册表 + 源生成 JSON 上下文反序列化（net8+；低目标框架或未登记契约时跳过类型化处理器并记 Error）。
/// </para>
/// <para>
/// <b>处理器异常隔离</b>：单处理器抛非取消异常时记 <see cref="LogLevel.Error"/> 后继续其余处理器；
/// 取消异常（软超时/请求中止）向外传播由中间件处置。
/// </para>
/// </remarks>
public sealed class WechatChannelsCallbackDispatcher
{
    private readonly ChannelsCallbackHandlerRegistry _handlerRegistry;
    private readonly ChannelsCallbackInterceptorRegistry _interceptorRegistry;
    private readonly ChannelsCallbackReplyRegistry _replyRegistry;
    private readonly IChannelsPayloadContractRegistry _payloadContracts;
    private readonly IOptionsMonitor<ChannelsCallbackOptions> _optionsMonitor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WechatChannelsCallbackDispatcher> _logger;
    private readonly SemaphoreSlim _concurrencyGate;

    /// <summary>创建分发器。</summary>
    /// <param name="handlerRegistry">处理器注册表。</param>
    /// <param name="interceptorRegistry">拦截器注册表。</param>
    /// <param name="replyRegistry">结构化应答处理器注册表（仅官方要求同步应答的事件需要注册）。</param>
    /// <param name="payloadContracts">载荷契约注册表（事件键 → 载荷类型）。</param>
    /// <param name="optionsMonitor">配置监视器（软超时热读取；并发容量为构造期快照）。</param>
    /// <param name="scopeFactory">scope 工厂（处理器/拦截器实例解析）。</param>
    /// <param name="logger">日志器。</param>
    public WechatChannelsCallbackDispatcher(
        ChannelsCallbackHandlerRegistry handlerRegistry,
        ChannelsCallbackInterceptorRegistry interceptorRegistry,
        ChannelsCallbackReplyRegistry replyRegistry,
        IChannelsPayloadContractRegistry payloadContracts,
        IOptionsMonitor<ChannelsCallbackOptions> optionsMonitor,
        IServiceScopeFactory scopeFactory,
        ILogger<WechatChannelsCallbackDispatcher> logger)
    {
        _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        _interceptorRegistry = interceptorRegistry ?? throw new ArgumentNullException(nameof(interceptorRegistry));
        _replyRegistry = replyRegistry ?? throw new ArgumentNullException(nameof(replyRegistry));
        _payloadContracts = payloadContracts ?? throw new ArgumentNullException(nameof(payloadContracts));
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
    /// <returns>分发结果（含结构化应答）；软超时/请求中止以 <see cref="OperationCanceledException"/> 传播。</returns>
    public async Task<WechatChannelsCallbackDispatchResult> DispatchAsync(
        string appKey, WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken = default)
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
                        "事件 {EventType}（appKey: {AppKey}）被拦截器 {Interceptor} 中断，按官方语义回 success（不触发重推）。",
                        eventType, appKey, interceptor.GetType().FullName);
                    return new WechatChannelsCallbackDispatchResult(WechatCallbackDispatchOutcome.Interrupted, null);
                }
            }
        }

        // — 2. 并发闸 + 软超时（必须 < 官方 5s 契约）—
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
                        if (handler is IChannelsCallbackPayloadHandler payloadHandler)
                        {
                            var payload = ReadPayload(envelope, eventType, payloadHandler.PayloadType);
                            if (payload == null)
                            {
                                _logger.LogError(
                                    "事件 {EventType} 的类型化处理器 {Handler} 所需载荷 {PayloadType} 读取失败" +
                                    "（事件键未登记 [ChannelsCallbackContract] 契约、载荷类型未登记源生成 JSON 上下文、" +
                                    "或当前目标框架不支持类型化通道），已跳过该处理器。AppKey: {AppKey}",
                                    eventType, handler.GetType().FullName, payloadHandler.PayloadType.Name, appKey);
                                continue;
                            }

                            await payloadHandler
                                .HandlePayloadAsync(envelope, payload, dispatchToken)
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

            // — 3. 拦截器 After（观测；异常记日志不打断应答）—
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
            var reply = await TryBuildReplyAsync(scope.ServiceProvider, appKey, envelope, dispatchToken)
                .ConfigureAwait(false);
            return new WechatChannelsCallbackDispatchResult(outcome, reply);
        }
        finally
        {
            _concurrencyGate.Release();
        }
    }

    /// <summary>解析结构化应答：appKey 专属先于全局通配，取第一个返回非 <c>null</c> 的应答处理器。</summary>
    private async Task<ChannelsCallbackReply?> TryBuildReplyAsync(
        IServiceProvider provider, string appKey, WechatChannelsCallbackEvent envelope, CancellationToken cancellationToken)
    {
        foreach (var type in EnumerateWithWildcard(_replyRegistry, appKey))
        {
            IChannelsCallbackReplyHandler? handler;
            try
            {
                handler = provider.GetService(type) as IChannelsCallbackReplyHandler;
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                _logger.LogError(ex, "结构化应答处理器 {Type} 从 DI 解析失败（依赖缺失？），已跳过。", type.FullName);
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

    /// <summary>
    /// 按事件键反序列化强类型载荷（源生成 JSON 上下文，AOT 安全）。
    /// </summary>
    /// <remarks>
    /// 仅 <c>net8.0+</c> 且事件键已登记契约、载荷类型已登记进 <c>CallbackJsonContext</c> 时可用；
    /// 低目标框架恒返回 <c>null</c>（类型化处理器被跳过并记 Error，信封级处理器不受影响）。
    /// </remarks>
    /// <remarks>
    /// <b>编译开关 <c>CHANNELS_CALLBACK_TYPED_PAYLOAD</c></b>：载荷 DTO 与 CallbackJsonContext 由
    /// P3 载荷批次落地并重跑 GenerateJsonContext 后，于 Callback csproj 定义该开关启用类型化通道；
    /// 开关关闭时类型化处理器在运行期被跳过（记 Error），信封级处理器不受影响。
    /// </remarks>
#if NET8_0_OR_GREATER && CHANNELS_CALLBACK_TYPED_PAYLOAD
    private object? ReadPayload(WechatChannelsCallbackEvent envelope, string eventType, Type payloadType)
    {
        if (string.IsNullOrEmpty(eventType) || !_payloadContracts.TryResolve(eventType, out var contract))
        {
            return null;
        }

        if (contract!.PayloadType != payloadType)
        {
            _logger.LogWarning(
                "事件 {EventType} 的处理器载荷类型 {PayloadType} 与契约登记类型 {ContractType} 不一致，跳过类型化读取。",
                eventType, payloadType.Name, contract.PayloadType.Name);
            return null;
        }

        var decrypted = envelope.DecryptedXml;
        if (string.IsNullOrEmpty(decrypted))
        {
            return null;
        }

        try
        {
            var typeInfo = Mud.Wechat.Channels.DataModels.Callback.CallbackJsonContext.Default
                .GetTypeInfo(payloadType);
            return JsonSerializer.Deserialize(decrypted!, typeInfo);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            _logger.LogWarning(ex,
                "事件 {EventType} 的载荷 {PayloadType} 反序列化失败（类型未登记进 CallbackJsonContext？）。",
                eventType, payloadType.Name);
            return null;
        }
    }
#else
    private object? ReadPayload(WechatChannelsCallbackEvent envelope, string eventType, Type payloadType) => null;
#endif

    /// <summary>解析并匹配处理器：appKey 专属精确 → 全局精确 → appKey 专属兜底 → 全局兜底。</summary>
    private List<IChannelsCallbackEventHandler> ResolveHandlers(IServiceProvider provider, string appKey, string eventType)
    {
        var exact = new List<IChannelsCallbackEventHandler>();
        var fallback = new List<IChannelsCallbackEventHandler>();

        foreach (var type in EnumerateWithWildcard(_handlerRegistry, appKey))
        {
            IChannelsCallbackEventHandler? handler;
            try
            {
                handler = provider.GetService(type) as IChannelsCallbackEventHandler;
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

    private List<IChannelsCallbackEventInterceptor> ResolveInterceptors(IServiceProvider provider, string appKey)
    {
        var interceptors = new List<IChannelsCallbackEventInterceptor>();
        foreach (var type in EnumerateWithWildcard(_interceptorRegistry, appKey))
        {
            IChannelsCallbackEventInterceptor? interceptor;
            try
            {
                interceptor = provider.GetService(type) as IChannelsCallbackEventInterceptor;
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

        if (!string.Equals(appKey, ChannelsCallbackOptions.WildcardAppKey, StringComparison.Ordinal))
        {
            foreach (var type in registry.GetAll(ChannelsCallbackOptions.WildcardAppKey))
            {
                yield return type;
            }
        }
    }
}
