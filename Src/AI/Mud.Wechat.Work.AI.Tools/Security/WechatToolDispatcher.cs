// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Mud.AI.Agent.Tools;
using Mud.AI.Agent.Tools.Args;
using Mud.AI.Agent.Tools.Errors;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools.Diagnostics;
using Mud.Wechat.Work.AI.Tools.Generated;
using Mud.Wechat.Work.AI.Tools.Registration;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 工具调用执行链（唯一入口）：<b>预算 → 白名单 → 入参净化 → 授权门禁 → 审批门 → 执行 → 出站纪律 → 审计 → 异常归一</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>五步护城河里本类型承担四步</b>：① 白名单命中（<see cref="WechatToolPolicy"/>）；
/// ② 授权门禁（<see cref="IWechatToolAuthorizer"/>，未注册即拒绝）；③ <b>租户切换不在本类型</b> ——
/// 回合级 <c>UseCorpScope</c> 已由作用域端口在工具执行前完成（见 <c>WechatWorkAiScopeFactory</c>），
/// 在此重复进入会与回合上下文打架（嵌套作用域释放顺序决定「退出后读到谁」，重复即不可判定）；
/// ④ 强类型直连（执行器）；⑤ 异常归一（本类型）。
/// </para>
/// <para>
/// <b>租户事实取自回合上下文</b>：授权上下文与审计记录的 <c>AppKey</c>/<c>AuthCorpId</c> 来自
/// <see cref="IWechatAiCorpContextAccessor"/>（事件层唯一建立点），装配期 <see cref="WechatToolBinding"/> 只作兜底。
/// 否则多应用共进程时会出现「鉴权在 A 企业、执行在 B 企业」的失真。
/// </para>
/// <para>
/// <b>审批不在这里续跑</b>：本类型只做「同步确认门」（<see cref="IWechatToolApprovalGate"/>，未注册即拒绝），
/// 挂起式审批由框架（MAF <c>ApprovalRequiredAIFunction</c> + <c>ToolApprovalRequestContent</c>）承担——
/// 框架在会话状态袋里保留<b>原始 tool_call</b>，因此续跑用的是同一次调用的真实入参，不存在「模型重新采样导致
/// 摘要不符」的问题。自建工单状态机是第二真相源，已删除。
/// </para>
/// <para>
/// <b>拒绝文案中性且唯一</b>：不区分「未启用 / 未注册 / 无授权器 / 权限点不足」——区分度会告诉模型哪些工具存在、
/// 哪些权限点缺失，从而把拒绝变成可试探的信息面。细分原因只进宿主日志与审计端口（<c>DenialReason</c> 不落审计记录）。
/// </para>
/// <para>
/// <b>失败必须可结算</b>：结算读的是 <see cref="ToolResult.Error"/>，而 <see cref="ToolResult.FromError(string)"/>
/// <b>不产生</b>结构化错误载荷（实测 <c>Error is null</c>）——历史实现因此把上游失败与参数错误一律记为
/// <c>Succeeded</c>。本类型的所有失败分支都经 <see cref="ToolResultPipeline.Error"/> 构造结构化载荷，
/// 结算、审计 <c>ErrorCode</c>、指标 <c>outcome</c> 三者由此同源。
/// </para>
/// <para>
/// <b>超时覆盖全链路</b>（排队 / 授权 / 审批 / 执行）：只给执行段设超时，慢授权器或队列积压会让回合挂到
/// 调用方超时为止，日志里只有一条「未完成的工具调用」。
/// </para>
/// <para>
/// <b>执行失败也回填而不抛</b>：工具链里抛异常等价于「模型什么都看不到」；统一转成可读错误让模型换路
/// （<see cref="OperationCanceledException"/> 例外 —— 取消是控制流，必须向上传播）。
/// </para>
/// </remarks>
public sealed class WechatToolDispatcher : IDisposable
{
    private const string DeniedText = "工具调用被拒绝：本轮未获授权、该工具未启用，或当前实例未注册该工具。";

    private readonly WechatToolRegistry _registry;
    private readonly IWechatToolAuthorizer? _authorizer;
    private readonly IWechatToolApprovalGate? _approvalGate;
    private readonly IWechatAiCorpContextAccessor? _corpContext;
    private readonly IWechatToolAuditSink _auditSink;
    private readonly WechatToolOutboundGuard _outbound;

    private readonly ToolRetryPolicy _retry;
    private readonly ILogger<WechatToolDispatcher>? _logger;
    private readonly SemaphoreSlim? _concurrency;

    private bool _disposed;

    /// <summary>初始化执行链。</summary>
    /// <param name="registry">工具注册表（不得为 <see langword="null"/>）。</param>
    /// <param name="policy">白名单与出站策略（不得为 <see langword="null"/>）。</param>
    /// <param name="authorizer">授权器（可空 ⇒ 强制授权时一律拒绝）。</param>
    /// <param name="approvalGate">同步审批门（可空 ⇒ 需审批的写工具一律拒绝）。</param>
    /// <param name="logger">日志（可空）。</param>
    /// <param name="auditSink">审计端口（可空 ⇒ 空实现，零成本）。</param>
    /// <param name="corpContext">回合上下文访问器（可空 ⇒ 授权与审计回落装配期绑定）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="registry"/> 或 <paramref name="policy"/> 为 <see langword="null"/>。</exception>
    public WechatToolDispatcher(
        WechatToolRegistry registry,
        WechatToolPolicy policy,
        IWechatToolAuthorizer? authorizer = null,
        IWechatToolApprovalGate? approvalGate = null,
        ILogger<WechatToolDispatcher>? logger = null,
        IWechatToolAuditSink? auditSink = null,
        IWechatAiCorpContextAccessor? corpContext = null)
        : this(registry, policy, outbound: null, authorizer, approvalGate, logger, auditSink, corpContext)
    {
    }

    /// <summary>测试接缝：<paramref name="outbound"/> 非空时替换出站出口（用于锁定「归一失败不得穿透」）。</summary>
    /// <remarks>
    /// 出站失败的<b>真实触发条件</b>（净化/标注/截断任一步抛）在正常输入下不可稳定构造，而「永不因出站失败而抛」
    /// 是本类型的公开承诺。把出口做成可注入而不是把兜底逻辑放进 <see cref="WechatToolOutboundGuard"/> 内部，
    /// 是为了让<b>契约的守门人</b>（执行链）与<b>实现</b>（三步出口）各自可验证：前者证明兜底生效，后者保持纯函数。
    /// </remarks>
    internal WechatToolDispatcher(
        WechatToolRegistry registry,
        WechatToolPolicy policy,
        WechatToolOutboundGuard? outbound,
        IWechatToolAuthorizer? authorizer,
        IWechatToolApprovalGate? approvalGate,
        ILogger<WechatToolDispatcher>? logger,
        IWechatToolAuditSink? auditSink,
        IWechatAiCorpContextAccessor? corpContext)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _authorizer = authorizer;
        _approvalGate = approvalGate;
        _logger = logger;
        _auditSink = auditSink ?? NullWechatToolAuditSink.Instance;
        _corpContext = corpContext;
        _outbound = outbound ?? new WechatToolOutboundGuard(policy.MaxResultLength, policy.ContentSafetyMode);
        // 只读限流退避：写工具在 ShouldRetry 内部恒为 false（isWrite ⇒ 零重试），此处不重复判断。
        _retry = new ToolRetryPolicy(policy.Retry, () => DateTimeOffset.UtcNow.Ticks);
        // 限并发让过载变成「排队」而不是「一起触发限流」：模型一个回合可以并行发多个 tool_call，
        // 而企微多数接口有频率上限（审批查询 600 次/分），不限并发时表现为一批工具同时返回 45009。
        _concurrency = policy.MaxConcurrentCalls > 0 ? new SemaphoreSlim(policy.MaxConcurrentCalls) : null;
    }

    /// <summary>本执行链使用的白名单策略。</summary>
    public WechatToolPolicy Policy { get; }

    /// <summary>按工具名执行一次调用（模型侧 tool_call 的唯一落点）。</summary>
    /// <param name="toolName">工具名（契约名）。</param>
    /// <param name="arguments">模型侧入参。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工具结果（拒绝/失败同样以结果返回，不抛业务异常）。</returns>
    /// <exception cref="ObjectDisposedException">执行链已释放。</exception>
    /// <exception cref="OperationCanceledException">调用方取消（超时不算，超时回填结果）。</exception>
    public async Task<ToolResult> InvokeAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        // 超时从「进入执行链」起算：排队、授权、审批与执行共用同一预算。
        using var deadlineCts = Policy.CallTimeout > TimeSpan.Zero
            ? new CancellationTokenSource(Policy.CallTimeout)
            : null;
        using var linkedCts = deadlineCts is null
            ? null
            : CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, deadlineCts.Token);
        var callToken = linkedCts?.Token ?? cancellationToken;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            return await InvokeCoreAsync(toolName, arguments, callToken, cancellationToken, deadlineCts, stopwatch)
                .ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (IsDeadline(deadlineCts, cancellationToken))
        {
            // 超时可能发生在授权/审批/排队段（执行段自己的 catch 已处理）：同样结算为「失败 + 可换路文案」。
            _logger?.LogWarning(
                "工具 {Tool} 在进入执行前超时（{Seconds}s）。", toolName, (int)Policy.CallTimeout.TotalSeconds);
            stopwatch.Stop();
            return await CompleteTimeoutAsync(toolName, arguments, context: null, stopwatch).ConfigureAwait(false);
        }
    }

    /// <summary>释放执行链持有的并发闸门。</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _concurrency?.Dispose();
    }

    private async Task<ToolResult> InvokeCoreAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken callToken,
        CancellationToken callerToken,
        CancellationTokenSource? deadlineCts,
        Stopwatch stopwatch)
    {
        // 回合工具预算先于白名单：越限不泄露「工具是否存在」。
        if (ToolCallBudget.Current is { } budget && !budget.TryConsume())
        {
            return await DenyAsync(
                toolName,
                arguments,
                "tool-budget-exceeded",
                null,
                ToolErrorSubtype.ToolBudgetExceeded,
                $"本轮工具调用已达上限（{budget.Max} 次），本次调用未执行；请基于已获得的信息作答，"
                + "或请用户缩小范围后重开一轮。").ConfigureAwait(false);
        }

        if (!Policy.IsEnabled(toolName))
        {
            return await DenyAsync(toolName, arguments, "not-enabled", null, ToolErrorSubtype.ToolNotAllowed, DeniedText)
                .ConfigureAwait(false);
        }

        if (!_registry.TryGet(toolName, out var execution))
        {
            _logger?.LogWarning("工具 {Tool} 在白名单内但未注册：装配不完整（契约表与注册表漂移）。", toolName);
            return await DenyAsync(toolName, arguments, "not-registered", null, ToolErrorSubtype.ToolNotAllowed, DeniedText)
                .ConfigureAwait(false);
        }

        if (!WechatToolContracts.ByToolName.TryGetValue(toolName, out var contract))
        {
            return await DenyAsync(toolName, arguments, "contract-missing", null, ToolErrorSubtype.ToolNotAllowed, DeniedText)
                .ConfigureAwait(false);
        }

        var context = BuildContext(execution, contract, arguments);

        // 入参净化先于授权：控制字符/超长/深嵌套是「协议层垃圾」，与谁有权限无关，
        // 放到授权之后会让无权限调用也走一遍解析（并可能在解析里抛非受控异常）。
        // 拒绝而非剥离：剥离会让模型以为参数被接受，下一次仍传同样的垃圾。
        try
        {
            ToolArgumentSanitizer.Validate(arguments);
        }
        catch (Exception ex) when (ex is ArgumentException or ToolArgumentException)
        {
            _logger?.LogWarning(ex, "工具 {Tool} 入参被净化层拒绝。", toolName);
            RecordWithoutTiming(toolName, contract.Risk, WechatToolAuditOutcome.Failed);
            var rejected = ValidationError(toolName, ex.Message, (ex as ToolArgumentException)?.Suggestions);
            await AuditAsync(context, WechatToolAuditOutcome.Failed, 0, rejected).ConfigureAwait(false);
            return rejected;
        }

        if (Policy.EnforceAuthorization)
        {
            var decision = await AuthorizeAsync(context, callToken).ConfigureAwait(false);
            if (!decision.IsAllowed)
            {
                return await DenyAsync(
                    toolName,
                    arguments,
                    decision.DenialReason ?? "denied",
                    context,
                    ToolErrorSubtype.AuthorizationDenied,
                    DeniedText).ConfigureAwait(false);
            }
        }

        if (RequiresApproval(contract.Risk))
        {
            var approval = await ApproveAsync(context, callToken).ConfigureAwait(false);
            if (!approval.IsApproved)
            {
                _logger?.LogInformation("工具 {Tool} 的人工确认被拒绝：{Reason}", toolName, approval.Reason);
                RecordWithoutTiming(toolName, contract.Risk, WechatToolAuditOutcome.ApprovalRejected);
                await AuditAsync(context, WechatToolAuditOutcome.ApprovalRejected, 0, null).ConfigureAwait(false);
                return ToolResultPipeline.Error(
                    toolName,
                    ToolErrorCategory.Confirmation,
                    ToolErrorSubtype.NeedsUserConfirmation,
                    "工具调用被拒绝：该操作需要人工确认，本轮未获确认，因此未执行；"
                    + "请勿重复发起，改为告知用户该操作需要确认后由用户决定。",
                    maxLength: 0,
                    attempts: 1);
            }
        }

        return await ExecuteAsync(execution, toolName, context, arguments, callToken, callerToken, deadlineCts, stopwatch)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// 只读限流退避：同一调用内最多重试 <c>Retry.MaxAttempts</c> 次（<b>写工具恒不重试</b>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为什么在这里而不在执行器里</b>：重试是<b>治理</b>（与超时、并发、预算同层），执行器只负责一次投影；
    /// 放进执行器会让 21 个文件各自实现退避，且写工具"绝不重试"这条硬约束无处集中校验。
    /// </para>
    /// <para>
    /// <b>退避受 deadline 约束</b>：<c>DelayAsync</c> 用本次调用的超时令牌，退避期间超时同样触发
    /// <c>OperationCanceledException</c> ⇒ 由外层统一结算成 <c>Retryable/timeout</c>，不会把成功预算用光。
    /// </para>
    /// </remarks>
    private async Task<ToolResult> ExecuteWithRetryAsync(
        WechatToolExecution execution,
        string toolName,
        WechatToolAuthorizationContext context,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken callToken)
    {
        var isWrite = context.Risk != WechatToolRisk.Read;
        for (var attempt = 0; ; attempt++)
        {
            var result = await execution.ExecuteAsync(arguments, callToken).ConfigureAwait(false);
            if (result.Error is not { } error
                || !_retry.ShouldRetry(error, isWrite, isDryRun: false, hasIdempotencyKey: false, attempt: attempt))
            {
                return result;
            }

            _logger?.LogInformation(
                "工具 {Tool} 第 {Attempt} 次重试（{Category}/{Subtype}，api_code={ApiCode}）。",
                toolName, attempt + 1, error.Category, error.Subtype, error.ApiCode);
            await _retry.DelayAsync(_retry.ComputeDelay(attempt, error.RetryAfterSeconds), callToken)
                .ConfigureAwait(false);
        }
    }

    /// <summary>强类型执行 + 超时/并发治理 + 出站纪律 + 审计 + 指标 + 异常归一（取消例外向上传播）。</summary>
    private async Task<ToolResult> ExecuteAsync(
        WechatToolExecution execution,
        string toolName,
        WechatToolAuthorizationContext context,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken callToken,
        CancellationToken callerToken,
        CancellationTokenSource? deadlineCts,
        Stopwatch stopwatch)
    {
        using var activity = WechatToolMetrics.StartActivity(toolName, RiskLabel(context.Risk));

        var failed = false;
        ToolResult result;
        try
        {
            var acquired = false;
            if (_concurrency is not null)
            {
                await _concurrency.WaitAsync(callToken).ConfigureAwait(false);
                acquired = true;
            }

            try
            {
                // 执行器内部会再套一层 RunAsync 归一（ToolExecutor），此处 catch 兜住未归一的执行器。
                result = await ExecuteWithRetryAsync(execution, toolName, context, arguments, callToken).ConfigureAwait(false);
            }
            finally
            {
                if (acquired)
                {
                    _concurrency!.Release();
                }
            }
        }
        catch (OperationCanceledException) when (IsDeadline(deadlineCts, callerToken))
        {
            // 只认「本次调用的超时令牌真的触发」；执行器自己抛的 OCE 仍按取消上抛（既有契约）。
            _logger?.LogWarning(
                "工具 {Tool} 超时（{Seconds}s）已中止。", toolName, (int)Policy.CallTimeout.TotalSeconds);
            stopwatch.Stop();
            WechatToolMetrics.CompleteActivity(activity, "failed", 0, false);
            return await CompleteTimeoutAsync(toolName, arguments, context, stopwatch).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // 调用方取消 = 控制流，必须上抛；但「已发生的事实」仍要留痕。
            stopwatch.Stop();
            WechatToolMetrics.CompleteActivity(activity, "cancelled", 0, false);
            await CompleteAsync(context, WechatToolAuditOutcome.Failed, stopwatch, null).ConfigureAwait(false);
            throw;
        }
        catch (ToolArgumentException ex)
        {
            // 参数类错误（执行链分类 Validation/invalid_args）：文案由产品侧写就——含参数名，闭集非法值还含
            // 合法取值清单（ToolArgumentException.Suggestions）——原样回填模型才能自我纠错。
            _logger?.LogWarning(ex, "工具 {Tool} 入参校验失败（已回填可读错误）。", toolName);
            failed = true;
            result = ValidationError(toolName, ex.Message, ex.Suggestions);
        }
        catch (ArgumentException ex)
        {
            _logger?.LogWarning(ex, "工具 {Tool} 入参校验失败（未携带建议清单）。", toolName);
            failed = true;
            result = ValidationError(toolName, ex.Message, null);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "工具 {Tool} 执行失败（已归一为可读错误回填模型）。", toolName);
            failed = true;
            result = ToolResultPipeline.Error(
                toolName,
                ToolErrorCategory.Internal,
                ToolErrorSubtype.Unexpected,
                $"工具 {toolName} 执行失败，请稍后重试或改用其他方式获取信息。",
                maxLength: 0,
                attempts: 1);
        }

        stopwatch.Stop();

        var normalized = Normalize(result, toolName, out var normalizeFailed);
        var outcome = failed || normalizeFailed || normalized.Error is not null
            ? WechatToolAuditOutcome.Failed
            : WechatToolAuditOutcome.Succeeded;
        WechatToolMetrics.CompleteActivity(
            activity,
            OutcomeLabel(outcome),
            WechatToolOutboundGuard.CharsOf(normalized.Text),
            normalized.Truncated);
        await CompleteAsync(context, outcome, stopwatch, normalized).ConfigureAwait(false);

        return normalized;
    }

    /// <summary>超时结算（排队/授权/审批/执行四段共用）：审计 + 指标 + 可换路文案。</summary>
    private async Task<ToolResult> CompleteTimeoutAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        WechatToolAuthorizationContext? context,
        Stopwatch stopwatch)
    {
        var normalized = Normalize(TimeoutError(toolName), toolName, out _);
        var risk = context is null ? "unknown" : RiskLabel(context.Risk);
        var outcome = OutcomeLabel(WechatToolAuditOutcome.Failed);
        WechatToolMetrics.RecordInvocation(toolName, risk, outcome);
        WechatToolMetrics.RecordDuration(toolName, risk, outcome, stopwatch.Elapsed.TotalMilliseconds);
        WechatToolMetrics.RecordResult(toolName, WechatToolOutboundGuard.CharsOf(normalized.Text), normalized.Truncated);
        await AuditAsync(toolName, arguments, WechatToolAuditOutcome.Failed, stopwatch.ElapsedMilliseconds, normalized)
            .ConfigureAwait(false);
        return normalized;
    }

    private ToolResult TimeoutError(string toolName) => ToolResultPipeline.Error(
        toolName,
        ToolErrorCategory.Retryable,
        ToolErrorSubtype.Timeout,
        $"工具 {toolName} 超时（{(int)Policy.CallTimeout.TotalSeconds} 秒未返回），本次调用已中止；"
        + "请缩小查询范围（更小的时间区间 / 更少的条数 / 更精确的条件）后重试，不要原样重复调用。",
        maxLength: 0,
        attempts: 1,
        suggestions: ["缩小时间区间", "减少条数/收窄过滤条件", "不要原样重复调用"]);

    private static ToolResult ValidationError(string toolName, string reason, IReadOnlyList<string>? suggestions)
        => ToolResultPipeline.Error(
            toolName,
            ToolErrorCategory.Validation,
            ToolErrorSubtype.InvalidArgs,
            reason,
            maxLength: 0,
            attempts: 1,
            suggestions: suggestions);

    private static bool IsDeadline(CancellationTokenSource? deadlineCts, CancellationToken cancellationToken)
        => deadlineCts?.IsCancellationRequested == true && !cancellationToken.IsCancellationRequested;

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(WechatToolDispatcher));
        }
    }

    /// <summary>一次调用的收尾：审计 + 指标（两者都不影响返回结果）。</summary>
    private async ValueTask CompleteAsync(
        WechatToolAuthorizationContext context,
        WechatToolAuditOutcome outcome,
        Stopwatch stopwatch,
        ToolResult? result)
    {
        var risk = RiskLabel(context.Risk);
        var outcomeLabel = OutcomeLabel(outcome);
        WechatToolMetrics.RecordInvocation(context.ToolName, risk, outcomeLabel);
        WechatToolMetrics.RecordDuration(context.ToolName, risk, outcomeLabel, stopwatch.Elapsed.TotalMilliseconds);
        WechatToolMetrics.RecordResult(
            context.ToolName,
            WechatToolOutboundGuard.CharsOf(result?.Text),
            result?.Truncated ?? false);

        await AuditAsync(context, outcome, stopwatch.ElapsedMilliseconds, result).ConfigureAwait(false);
    }

    /// <summary>只记调用次数（拒绝/不可用等未进入执行的路径：没有耗时与结果体积）。</summary>
    private static void RecordWithoutTiming(string toolName, WechatToolRisk? risk, WechatToolAuditOutcome outcome)
        => WechatToolMetrics.RecordInvocation(
            toolName,
            risk is null ? "unknown" : RiskLabel(risk.Value),
            OutcomeLabel(outcome));

    /// <summary>风险档的指标标签（低基数：三档固定值）。</summary>
    private static string RiskLabel(WechatToolRisk risk) => risk switch
    {
        WechatToolRisk.Read => "read",
        WechatToolRisk.Write => "write",
        _ => "high-risk-write",
    };

    /// <summary>审计结局的指标标签（低基数：四类固定值）。</summary>
    private static string OutcomeLabel(WechatToolAuditOutcome outcome) => outcome switch
    {
        WechatToolAuditOutcome.Succeeded => "succeeded",
        WechatToolAuditOutcome.Denied => "denied",
        WechatToolAuditOutcome.ApprovalRejected => "approval_rejected",
        _ => "failed",
    };

    /// <summary>构造授权上下文：租户与发起人取回合事实，装配期绑定只作兜底。</summary>
    private WechatToolAuthorizationContext BuildContext(
        WechatToolExecution execution,
        WechatToolContract contract,
        IReadOnlyDictionary<string, object?> arguments)
    {
        var principal = _corpContext?.Current;
        var appKey = principal?.AppKey;
        if (string.IsNullOrWhiteSpace(appKey))
        {
            appKey = execution.Binding.AppKey;
        }

        if (string.IsNullOrWhiteSpace(appKey))
        {
            // 装配期未声明归属且回合上下文缺省（如 MCP 单租户直调）：审计仍要能落一条可追踪的记录。
            appKey = "unknown";
        }

        var authCorpId = principal?.AuthCorpId ?? execution.Binding.AuthCorpId;

        return new WechatToolAuthorizationContext(
            contract.Name,
            appKey,
            authCorpId,
            contract.Risk,
            contract.RequiredScopes,
            ToolArgsDigester.Digest(arguments),
            principal,
            WechatToolApprovalPreview.Build(contract.Name, contract.Risk, arguments));
    }

    private async ValueTask<WechatToolAuthorizationResult> AuthorizeAsync(
        WechatToolAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        if (_authorizer is null)
        {
            return WechatToolAuthorizationResult.Deny("未注册 IWechatToolAuthorizer（fail-closed 默认拒绝）");
        }

        return await _authorizer.AuthorizeAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>该风险档是否需过审批门（高危写恒需；受控写由策略开关控制；只读永不需要）。</summary>
    private bool RequiresApproval(WechatToolRisk risk) => risk switch
    {
        WechatToolRisk.Read => false,
        WechatToolRisk.Write => Policy.RequireApprovalForWrites,
        _ => true,
    };

    private async ValueTask<WechatToolApprovalDecision> ApproveAsync(
        WechatToolAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        if (_approvalGate is null)
        {
            return WechatToolApprovalDecision.Reject(
                "未注册 IWechatToolApprovalGate（写工具按 fail-closed 拒绝）");
        }

        return await _approvalGate.RequestApprovalAsync(context, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>拒绝路径：审计（带拒绝事实）后回填中性文案（结构化，便于模型区分「不可用」而非「查不到」）。</summary>
    private async Task<ToolResult> DenyAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        string reason,
        WechatToolAuthorizationContext? context,
        string subtype,
        string text)
    {
        _logger?.LogInformation("工具 {Tool} 被拒绝：{Reason}", toolName, reason);
        var auditContext = context ?? BuildAuditOnlyContext(toolName, arguments);
        RecordWithoutTiming(toolName, auditContext.Risk, WechatToolAuditOutcome.Denied);
        await AuditAsync(auditContext, WechatToolAuditOutcome.Denied, 0, null).ConfigureAwait(false);
        return Normalize(
            ToolResultPipeline.Error(
                toolName,
                ToolErrorCategory.Authorization,
                subtype,
                text,
                maxLength: 0,
                attempts: 0),
            toolName,
            out _);
    }

    /// <summary>出站归一（<b>不得穿透</b>）：净化/标注/截断任一步抛异常时回填固定错误文案。</summary>
    /// <param name="result">执行器原始结果。</param>
    /// <param name="toolName">工具名（回填文案与日志用）。</param>
    /// <param name="failed">归一是否失败（失败即结算为 <c>Failed</c>，不谎报成功）。</param>
    /// <returns>可回填模型的结果。</returns>
    /// <remarks>
    /// <b>为什么兜底而不是上抛</b>：出站失败意味着结果<b>未经净化</b>——原文（控制字符、手机号、凭据、注入载荷）
    /// 一旦进提示词就不可撤回。故既不原样送出，也不让异常穿到模型（模型拿到异常等于「什么都没看见」）：
    /// 只回填一句固定失败文案，并让审计把这次调用记为失败。
    /// </remarks>
    private ToolResult Normalize(ToolResult result, string toolName, out bool failed)
    {
        try
        {
            failed = false;
            return _outbound.Normalize(result);
        }
        catch (Exception ex)
        {
            failed = true;
            _logger?.LogError(ex, "工具 {Tool} 出站归一失败（已回填固定文案，原文不外发）。", toolName);
            return ToolResultPipeline.Error(
                toolName,
                ToolErrorCategory.Internal,
                ToolErrorSubtype.Unexpected,
                $"工具 {toolName} 的结果未能通过出站净化，本次调用按失败处理；请勿原样重试，改用其他方式获取信息。",
                maxLength: 0,
                attempts: 1);
        }
    }

    /// <summary>白名单/契约阶段没有授权上下文时，用装配期绑定构造审计事实（不含发起人）。</summary>
    private WechatToolAuthorizationContext BuildAuditOnlyContext(string toolName, IReadOnlyDictionary<string, object?> arguments)
    {
        var principal = _corpContext?.Current;
        var appKey = principal?.AppKey;
        if (string.IsNullOrWhiteSpace(appKey) && _registry.TryGet(toolName, out var execution))
        {
            appKey = execution.Binding.AppKey;
        }

        var authCorpId = principal?.AuthCorpId;
        if (authCorpId is null && _registry.TryGet(toolName, out var bound))
        {
            authCorpId = bound.Binding.AuthCorpId;
        }

        var risk = WechatToolContracts.ByToolName.TryGetValue(toolName, out var contract)
            ? contract.Risk
            : WechatToolRisk.Read;
        var scopes = contract?.RequiredScopes ?? [];

        return new WechatToolAuthorizationContext(
            toolName,
            appKey ?? "unknown",
            authCorpId,
            risk,
            scopes,
            ToolArgsDigester.Digest(arguments),
            principal);
    }

    private async ValueTask AuditAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        WechatToolAuditOutcome outcome,
        long durationMs,
        ToolResult? result)
        => await AuditAsync(BuildAuditOnlyContext(toolName, arguments), outcome, durationMs, result).ConfigureAwait(false);

    /// <summary>写入审计（<b>尽力而为</b>：失败只记 Warning，绝不阻断执行）。</summary>
    private async ValueTask AuditAsync(
        WechatToolAuthorizationContext context,
        WechatToolAuditOutcome outcome,
        long durationMs,
        ToolResult? result)
    {
        if (ReferenceEquals(_auditSink, NullWechatToolAuditSink.Instance))
        {
            return;
        }

        var record = new WechatToolAuditRecord
        {
            ToolName = context.ToolName,
            AppKey = context.AppKey,
            AuthCorpId = context.AuthCorpId,
            SenderId = context.SenderId,
            Risk = context.Risk,
            RequiredScopes = context.RequiredScopes,
            ArgumentsDigest = context.ArgumentsDigest,
            Outcome = outcome,
            ErrorCode = result?.Error?.ApiCode,
            ErrorCategory = result?.Error?.Category,
            ErrorSubtype = result?.Error?.Subtype,
            Truncated = result?.Truncated ?? false,
            ResultChars = WechatToolOutboundGuard.CharsOf(result?.Text),
            DurationMs = durationMs,
        };

        try
        {
            // 审计用不可取消令牌：调用方取消不应让「已发生的事实」丢痕。
            await _auditSink.WriteAsync(record, CancellationToken.None).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "工具 {Tool} 审计写入失败（已忽略，不影响执行）。", context.ToolName);
        }
    }
}
