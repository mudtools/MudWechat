// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.DataModels.Aibot;

namespace Mud.Wechat.Work.Callback;

/// <summary>智能机器人回调分发结果（由适配层映射为加密应答）。</summary>
public enum WechatBotDispatchOutcome
{
    /// <summary>至少一个处理器被执行（其应答可能为 <c>null</c> = 空包）。</summary>
    Handled,

    /// <summary>无匹配处理器（unhandled，已告警）——适配层按加密空包应答。</summary>
    Unhandled,
}

/// <summary>
/// 智能机器人回调分发结果（结果 + 应答）。
/// </summary>
/// <remarks>
/// <b>应答为显式返回值</b>（非 ambient 通道）：HTTP 被动回复与长连接帧两条传输共用本内核，
/// 而长连接场景没有 HTTP 请求 scope，ambient scoped 通道不可移植。
/// </remarks>
public sealed class WechatBotDispatchResult
{
    /// <summary>创建分发结果。</summary>
    /// <param name="outcome">分发结果类别。</param>
    /// <param name="reply">应答消息；<c>null</c> = 无应答（适配层回加密空包）。</param>
    public WechatBotDispatchResult(WechatBotDispatchOutcome outcome, AibotMessage? reply)
    {
        Outcome = outcome;
        Reply = reply;
    }

    /// <summary>分发结果类别。</summary>
    public WechatBotDispatchOutcome Outcome { get; }

    /// <summary>应答消息（首个返回非 <c>null</c> 的处理器胜出）；<c>null</c> = 无应答。</summary>
    public AibotMessage? Reply { get; }
}

/// <summary>
/// 智能机器人回调分发器：软超时 + 并发闸 → 处理器匹配（精确优先、兜底次之）→ 逐个执行并收集应答。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 XML 回调分发器的差异</b>（不共用同一分发器，避免把 JSON 语义塞进 XML 管线）：
/// ① 处理器契约为<b>返回式</b>（<see cref="IWechatBotCallbackEventHandler.HandleAsync"/> 返回应答）；
/// ② 无事件族 / 事件键级开放面闸（智能机器人官方仅自建、键集恒定，无族级开放面矩阵）；
/// ③ 无拦截器（YAGNI：官方无对应语义，引入即多一个概念面）；
/// ④ <b>无指纹闸</b>（长连接帧无签名；回调地址模式的指纹闸在接收器完成）。
/// </para>
/// <para>
/// <b>超时语义</b>：软超时（<c>WechatCallbackOptions.EventHandlingTimeoutMs</c>，默认 4500ms）
/// 以 <see cref="OperationCanceledException"/> 向适配层传播，由适配层决定应答（HTTP 侧回加密空包，
/// 理由见中间件注释）；长耗时业务必须走「先回空包 + 主动回复 / 长连接流式补发」。
/// </para>
/// <para>
/// <b>实例生命周期</b>：处理器在请求 scope 内解析（Transient/Singleton 由宿主注册决定）；
/// 解析失败按「跳过 + LogError」降级，不打断其余处理器。
/// </para>
/// </remarks>
public sealed class WechatBotEventDispatcher
{
    private readonly WechatBotHandlerRegistry _handlerRegistry;
    private readonly IOptionsMonitor<WechatCallbackOptions> _optionsMonitor;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<WechatBotEventDispatcher> _logger;
    private readonly SemaphoreSlim _concurrencyGate;

    /// <summary>创建智能机器人回调分发器。</summary>
    /// <param name="handlerRegistry">处理器注册表（组合根期急切填充）。</param>
    /// <param name="optionsMonitor">回调配置监视器（软超时热读取；并发容量为构造期快照）。</param>
    /// <param name="scopeFactory">scope 工厂（处理器实例解析；防 Captive Dependency）。</param>
    /// <param name="logger">日志器。</param>
    public WechatBotEventDispatcher(
        WechatBotHandlerRegistry handlerRegistry,
        IOptionsMonitor<WechatCallbackOptions> optionsMonitor,
        IServiceScopeFactory scopeFactory,
        ILogger<WechatBotEventDispatcher> logger)
    {
        _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var maxConcurrent = Math.Max(1, optionsMonitor.CurrentValue.MaxConcurrentEvents);
        _concurrencyGate = new SemaphoreSlim(maxConcurrent, maxConcurrent);
    }

    /// <summary>
    /// 分发智能机器人回调（并发闸 → 软超时 → 匹配处理器 → 逐个执行并收集应答）。
    /// </summary>
    /// <param name="botKey">事件归属的回调配置键（路由提取）。</param>
    /// <param name="botEvent">智能机器人回调信封。</param>
    /// <param name="cancellationToken">取消令牌（链接请求中止；内部再链接软超时 CTS）。</param>
    /// <returns>分发结果（含应答）；软超时 / 请求中止以 <see cref="OperationCanceledException"/> 传播。</returns>
    public async Task<WechatBotDispatchResult> DispatchAsync(
        string botKey, WechatBotCallbackEvent botEvent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(botKey))
        {
            throw new ArgumentException("回调配置键不能为空", nameof(botKey));
        }

        if (botEvent == null)
        {
            throw new ArgumentNullException(nameof(botEvent));
        }

        var eventType = botEvent.EventTypeKey;
        var timeoutMs = Math.Max(1, _optionsMonitor.CurrentValue.EventHandlingTimeoutMs);
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeoutMs);
        var dispatchToken = timeoutCts.Token;

        await _concurrencyGate.WaitAsync(dispatchToken).ConfigureAwait(false);
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var handlers = ResolveHandlers(scope.ServiceProvider, botKey, eventType);

            if (handlers.Count == 0)
            {
                _logger.LogWarning(
                    "未找到智能机器人回调 {EventType}（配置键 {BotKey}）的处理器，已按空包应答（unhandled）。",
                    eventType, botKey);
                return new WechatBotDispatchResult(WechatBotDispatchOutcome.Unhandled, null);
            }

            // 依次执行全部命中处理器（与 XML 侧一致的「全量执行 + 异常隔离」语义）；
            // 应答取【首个非 null】——避免后注册的处理器覆盖先命中的确定性应答。
            AibotMessage? reply = null;
            foreach (var handler in handlers)
            {
                try
                {
                    var current = await handler.HandleAsync(botEvent, dispatchToken).ConfigureAwait(false);
                    if (reply == null && current != null)
                    {
                        reply = current;
                    }
                }
                catch (OperationCanceledException) when (dispatchToken.IsCancellationRequested)
                {
                    // 软超时或请求中止：向适配层传播（由其决定空包 / 中止应答）。
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "智能机器人回调 {EventType} 的处理器 {Handler} 执行失败（已隔离，继续其余处理器）。",
                        eventType, handler.GetType().FullName);
                }
            }

            return new WechatBotDispatchResult(WechatBotDispatchOutcome.Handled, reply);
        }
        finally
        {
            _concurrencyGate.Release();
        }
    }

    /// <summary>
    /// 解析并匹配处理器（匹配序与 XML 侧一致）：配置键专属精确 → 全局精确 → 专属兜底 → 全局兜底；
    /// 有任一精确命中即不执行兜底。
    /// </summary>
    /// <remarks>
    /// 匹配经 <see cref="IWechatBotCallbackEventHandler.SupportedEventType"/> 实例属性判定（无反射，AOT 安全）；
    /// 解析失败（依赖缺失）按「跳过 + LogError」降级。
    /// </remarks>
    private List<IWechatBotCallbackEventHandler> ResolveHandlers(
        IServiceProvider provider, string botKey, string eventType)
    {
        var exact = new List<IWechatBotCallbackEventHandler>();
        var fallback = new List<IWechatBotCallbackEventHandler>();

        foreach (var type in _handlerRegistry.EnumerateWithWildcard(botKey))
        {
            IWechatBotCallbackEventHandler? handler;
            try
            {
                handler = provider.GetService(type) as IWechatBotCallbackEventHandler;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "智能机器人处理器 {Type} 从 DI 解析失败（依赖缺失？），已跳过。", type.FullName);
                continue;
            }

            if (handler == null)
            {
                _logger.LogWarning("智能机器人处理器类型 {Type} 未在 DI 注册，已跳过。", type.FullName);
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
}
