// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Mud.AI.Agent.Tools;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.AI.Agent.Tools.Args;
using Mud.Wechat.Work.AI.Tools.Generated;
using Mud.Wechat.Work.AI.Tools.Registration;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 工具调用执行链（唯一入口）：<b>白名单 → 授权门禁 → 执行 → 异常归一</b>。
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
/// <b>拒绝文案中性</b>：不区分「未启用 / 无授权器 / 权限点不足」——区分度会告诉模型哪些工具存在、
/// 哪些权限点缺失，从而把拒绝变成可试探的信息面。细分原因只进宿主日志（<see cref="WechatToolAuthorizationResult.DenialReason"/>）。
/// </para>
/// <para>
/// <b>执行失败也回填而不抛</b>：工具链里抛异常等价于「模型什么都看不到」；统一转成可读错误让模型换路
/// （<see cref="OperationCanceledException"/> 例外 —— 取消是控制流，必须向上传播）。
/// </para>
/// </remarks>
public sealed class WechatToolDispatcher
{
    private const string DeniedText = "工具调用被拒绝：本轮未获授权，或该工具未启用。";
    private const string UnavailableText = "工具不可用：未在当前实例注册。";

    private readonly WechatToolRegistry _registry;
    private readonly IWechatToolAuthorizer? _authorizer;
    private readonly IWechatToolApprovalGate? _approvalGate;
    private readonly ILogger<WechatToolDispatcher>? _logger;

    /// <summary>初始化执行链。</summary>
    /// <param name="registry">工具注册表（不得为 <see langword="null"/>）。</param>
    /// <param name="policy">白名单策略（不得为 <see langword="null"/>）。</param>
    /// <param name="authorizer">授权器（可空 ⇒ 强制授权时一律拒绝）。</param>
    /// <param name="approvalGate">审批门（可空 ⇒ 需审批的写工具一律拒绝）。</param>
    /// <param name="logger">日志（可空）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="registry"/> 或 <paramref name="policy"/> 为 <see langword="null"/>。</exception>
    public WechatToolDispatcher(
        WechatToolRegistry registry,
        WechatToolPolicy policy,
        IWechatToolAuthorizer? authorizer = null,
        IWechatToolApprovalGate? approvalGate = null,
        ILogger<WechatToolDispatcher>? logger = null)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        _authorizer = authorizer;
        _approvalGate = approvalGate;
        _logger = logger;
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
            return Deny(toolName, "not-enabled");
        }

        if (!_registry.TryGet(toolName, out var execution))
        {
            _logger?.LogWarning("工具 {Tool} 在白名单内但未注册：装配不完整（契约表与注册表漂移）。", toolName);
            return ToolResult.FromError(UnavailableText);
        }

        if (!WechatToolContracts.ByToolName.TryGetValue(toolName, out var contract))
        {
            return Deny(toolName, "contract-missing");
        }

        var context = new WechatToolAuthorizationContext(
            contract.Name,
            execution.Binding.AppKey,
            execution.Binding.AuthCorpId,
            contract.Risk,
            contract.RequiredScopes,
            ToolArgsDigester.Digest(arguments));

        if (Policy.EnforceAuthorization)
        {
            var decision = await AuthorizeAsync(context, cancellationToken).ConfigureAwait(false);
            if (!decision.IsAllowed)
            {
                return Deny(toolName, decision.DenialReason ?? "denied");
            }
        }

        if (RequiresApproval(contract.Risk))
        {
            var approval = await ApproveAsync(context, cancellationToken).ConfigureAwait(false);
            if (!approval.IsApproved)
            {
                return Deny(toolName, approval.Reason ?? "approval-rejected");
            }
        }

        try
        {
            return await execution.ExecuteAsync(arguments, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            // 参数类错误（执行链分类 Validation/invalid_args）：文案由产品侧写就——含参数名，闭集非法值还含
            // 合法取值清单（ToolArgumentException.Suggestions）——原样回填模型才能自我纠错；
            // 通用分支的文案不含参数名，模型据此无法修正。其余异常一律不透文案（见下）。
            _logger?.LogWarning(ex, "工具 {Tool} 入参校验失败（已回填可读错误）。", toolName);
            return ToolResult.FromError(ex.Message);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "工具 {Tool} 执行失败（已归一为可读错误回填模型）。", toolName);
            return ToolResult.FromError($"工具 {toolName} 执行失败，请稍后重试或改用其他方式获取信息。");
        }
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

    private ToolResult Deny(string toolName, string reason)
    {
        _logger?.LogInformation("工具 {Tool} 被拒绝：{Reason}", toolName, reason);
        return ToolResult.FromError(DeniedText);
    }
}
