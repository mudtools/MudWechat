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
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Agents;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools.Diagnostics;
using Mud.Wechat.Work.AI.Tools.Generated;
using Mud.Wechat.Work.AI.Tools.Registration;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 工具调用执行链（唯一入口）：<b>白名单 → 授权门禁 → 审批门 → 执行 → 出站纪律 → 审计 → 异常归一</b>。
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
/// <b>拒绝文案中性</b>：不区分「未启用 / 无授权器 / 权限点不足」——区分度会告诉模型哪些工具存在、
/// 哪些权限点缺失，从而把拒绝变成可试探的信息面。细分原因只进宿主日志与审计端口。
/// </para>
/// <para>
/// <b>执行失败也回填而不抛</b>：工具链里抛异常等价于「模型什么都看不到」；统一转成可读错误让模型换路
/// （<see cref="OperationCanceledException"/> 例外 —— 取消是控制流，必须向上传播）。
/// </para>
/// <para>
/// <b>出站最后一步是纪律，不是建议</b>：所有结果（成功/失败/截断）都经 <see cref="WechatToolOutboundGuard"/>
/// 净化 + 注入标注 + 预算截断；审计在出站之后记录，因此「截断」对宿主与模型同时可见。
/// </para>
/// </remarks>
public sealed class WechatToolDispatcher
{
    private const string DeniedText = "工具调用被拒绝：本轮未获授权，或该工具未启用。";
    private const string UnavailableText = "工具不可用：未在当前实例注册。";

    private readonly WechatToolRegistry _registry;
    private readonly IWechatToolAuthorizer? _authorizer;
    private readonly IWechatToolApprovalGate? _approvalGate;
    private readonly IWechatAiCorpContextAccessor? _corpContext;
    private readonly IWechatToolAuditSink _auditSink;
    private readonly WechatToolOutboundGuard _outbound;
    private readonly ILogger<WechatToolDispatcher>? _logger;
    private readonly SemaphoreSlim? _concurrency;

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
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _authorizer = authorizer;
        _approvalGate = approvalGate;
        _logger = logger;
        _auditSink = auditSink ?? NullWechatToolAuditSink.Instance;
        _corpContext = corpContext;
        _outbound = new WechatToolOutboundGuard(policy.MaxResultLength, policy.ContentSafetyMode);
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
    public async Task<ToolResult> InvokeAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken = default)
    {
        if (!Policy.IsEnabled(toolName))
        {
            return await DenyAsync(toolName, arguments, "not-enabled", null).ConfigureAwait(false);
        }

        if (!_registry.TryGet(toolName, out var execution))
        {
            _logger?.LogWarning("工具 {Tool} 在白名单内但未注册：装配不完整（契约表与注册表漂移）。", toolName);
            RecordWithoutTiming(toolName, null, WechatToolAuditOutcome.Denied);
            await AuditAsync(toolName, arguments, WechatToolAuditOutcome.Denied, 0, null).ConfigureAwait(false);
            return ToolResult.FromError(UnavailableText);
        }

        if (!WechatToolContracts.ByToolName.TryGetValue(toolName, out var contract))
        {
            return await DenyAsync(toolName, arguments, "contract-missing", null).ConfigureAwait(false);
        }

        var context = BuildContext(execution, contract, arguments);

        if (Policy.EnforceAuthorization)
        {
            var decision = await AuthorizeAsync(context, cancellationToken).ConfigureAwait(false);
            if (!decision.IsAllowed)
            {
                return await DenyAsync(toolName, arguments, decision.DenialReason ?? "denied", context)
                    .ConfigureAwait(false);
            }
        }

        if (RequiresApproval(contract.Risk))
        {
            var approval = await ApproveAsync(context, cancellationToken).ConfigureAwait(false);
            if (!approval.IsApproved)
            {
                _logger?.LogInformation("工具 {Tool} 的人工确认被拒绝：{Reason}", toolName, approval.Reason);
                RecordWithoutTiming(toolName, contract.Risk, WechatToolAuditOutcome.ApprovalRejected);
                await AuditAsync(context, WechatToolAuditOutcome.ApprovalRejected, 0, null).ConfigureAwait(false);
                return ToolResult.FromError(DeniedText);
            }
        }

        return await ExecuteAsync(execution, toolName, context, arguments, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>强类型执行 + 超时/并发治理 + 出站纪律 + 审计 + 指标 + 异常归一（取消例外向上传播）。</summary>
    private async Task<ToolResult> ExecuteAsync(
        WechatToolExecution execution,
        string toolName,
        WechatToolAuthorizationContext context,
        IReadOnlyDictionary<string, object?> arguments,
        CancellationToken cancellationToken)
    {
        // 超时用「链接令牌」而不是 when 表达式：执行器内部的 Task.Delay/HttpClient 都会响应取消，
        // 中止是真正的中止（而不是仅返回一句超时文案、后台请求继续跑）。
        using var timeoutCts = Policy.CallTimeout > TimeSpan.Zero
            ? new CancellationTokenSource(Policy.CallTimeout)
            : null;
        using var linkedCts = timeoutCts is null
            ? null
            : CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
        var executionToken = linkedCts?.Token ?? cancellationToken;

        var stopwatch = Stopwatch.StartNew();
        var failed = false;
        ToolResult result;
        try
        {
            var acquired = false;
            if (_concurrency is not null)
            {
                await _concurrency.WaitAsync(cancellationToken).ConfigureAwait(false);
                acquired = true;
            }

            try
            {
                result = await execution.ExecuteAsync(arguments, executionToken).ConfigureAwait(false);
            }
            finally
            {
                if (acquired)
                {
                    _concurrency!.Release();
                }
            }
        }
        catch (OperationCanceledException) when (
            timeoutCts?.IsCancellationRequested == true && !cancellationToken.IsCancellationRequested)
        {
            // 只有「本次调用的超时令牌真的触发」才算超时；执行器自己抛的 OCE 仍按取消上抛（既有契约）。
            _logger?.LogWarning(
                "工具 {Tool} 超时（{Seconds}s）已中止。", toolName, (int)Policy.CallTimeout.TotalSeconds);
            failed = true;
            result = ToolResult.FromError(
                $"工具 {toolName} 超时（{(int)Policy.CallTimeout.TotalSeconds} 秒未返回），本次调用已中止；"
                + "请缩小查询范围（更小的时间区间 / 更少的条数 / 更精确的条件）后重试，不要原样重复调用。");
            stopwatch.Stop();
            await CompleteAsync(context, WechatToolAuditOutcome.Failed, stopwatch, null).ConfigureAwait(false);
            return _outbound.Normalize(result);
        }
        catch (OperationCanceledException)
        {
            // 调用方取消 = 控制流，必须上抛；但「已发生的事实」仍要留痕。
            stopwatch.Stop();
            await CompleteAsync(context, WechatToolAuditOutcome.Failed, stopwatch, null).ConfigureAwait(false);
            throw;
        }
        catch (ArgumentException ex)
        {
            // 参数类错误（执行链分类 Validation/invalid_args）：文案由产品侧写就——含参数名，闭集非法值还含
            // 合法取值清单（ToolArgumentException.Suggestions）——原样回填模型才能自我纠错；
            // 通用分支的文案不含参数名，模型据此无法修正。其余异常一律不透文案（见下）。
            _logger?.LogWarning(ex, "工具 {Tool} 入参校验失败（已回填可读错误）。", toolName);
            failed = true;
            result = ToolResult.FromError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "工具 {Tool} 执行失败（已归一为可读错误回填模型）。", toolName);
            failed = true;
            result = ToolResult.FromError($"工具 {toolName} 执行失败，请稍后重试或改用其他方式获取信息。");
        }

        stopwatch.Stop();

        var normalized = _outbound.Normalize(result);
        var outcome = failed || normalized.Error is not null
            ? WechatToolAuditOutcome.Failed
            : WechatToolAuditOutcome.Succeeded;
        await CompleteAsync(context, outcome, stopwatch, normalized).ConfigureAwait(false);

        return normalized;
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
            WechatToolOutboundGuard.BytesOf(result?.Text),
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
        var appKey = string.IsNullOrWhiteSpace(principal?.AppKey) ? execution.Binding.AppKey : principal!.AppKey!;
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

    /// <summary>拒绝路径：审计（带拒绝事实）后回填中性文案。</summary>
    private async Task<ToolResult> DenyAsync(
        string toolName,
        IReadOnlyDictionary<string, object?> arguments,
        string reason,
        WechatToolAuthorizationContext? context)
    {
        _logger?.LogInformation("工具 {Tool} 被拒绝：{Reason}", toolName, reason);
        var auditContext = context ?? BuildAuditOnlyContext(toolName, arguments);
        RecordWithoutTiming(toolName, auditContext.Risk, WechatToolAuditOutcome.Denied);
        await AuditAsync(auditContext, WechatToolAuditOutcome.Denied, 0, null).ConfigureAwait(false);
        return ToolResult.FromError(DeniedText);
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
            Truncated = result?.Truncated ?? false,
            ResultBytes = WechatToolOutboundGuard.BytesOf(result?.Text),
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
