// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;
using Mud.AI.Abstractions.Approvals;
using Mud.AI.Abstractions.Observability;
using Mud.AI.Agent.Approvals;
using Mud.AI.Agent.Events;
using Mud.Wechat.AI.Abstractions.Tools;
using Mud.Wechat.Work.AI.Agents;

namespace Mud.Wechat.Work.AI.Approvals;

/// <summary>
/// 企微侧 HITL 的<b>宿主批准入口</b>：查询待确认项、回灌批准/拒绝并续跑同一会话。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么需要这层</b>：中立闭环 <see cref="AgentApprovalContinuation"/> 要求调用方补齐四件事——同一会话键、
/// 框架记录的原始请求、<b>续跑前重建工具执行上下文</b>、落库后由派生处理器投递回复。前三件在企微侧各有产品事实
/// （键命名空间、租户环境量、企业维度），第四件不在本层（回复走 <c>WechatAibotReplySink</c>）。把这些事实收在一处，
/// 宿主批准回调只需「拿到 <see cref="FrameworkToolApprovalRequest"/> + 调用本方法」。
/// </para>
/// <para>
/// <b>批准 ≠ 放行</b>：续跑轮会重建「应用 + 企业」作用域（否则写工具 100% 结构化拒绝），并由执行链二次咨询
/// 授权器与审批门；本层不签发任何凭据，也不缓存「已批准」状态。
/// </para>
/// <para>
/// <b>幂等</b>：装配了 <see cref="IPendingApprovalStore"/> 时同一 <c>RequestId</c> 只生效一次——迟到/重复的批准
/// 一律 fail-closed 抛出（绝不重放一次人类批准过的写操作）。
/// </para>
/// </remarks>
public sealed class WechatToolApprovalResume
{
    private readonly WechatAgent _agent;
    private readonly ITurnScopeFactory _scopeFactory;
    private readonly IWechatAiCorpContextAccessor _corpContext;
    private readonly IPendingApprovalStore? _pendingApprovals;
    private readonly IToolApprovalChannel? _approvalChannel;
    private readonly IAgentTelemetry? _telemetry;
    private readonly Func<string, CancellationToken, Task<IDisposable?>>? _acquireGate;
    private readonly ILogger<WechatToolApprovalResume>? _logger;

    /// <summary>初始化批准入口。</summary>
    /// <param name="agent">目标 Agent（发起轮与续跑轮必须同一实例/同一会话存储）。</param>
    /// <param name="scopeFactory">产品作用域端口（续跑轮重建租户上下文；必填，缺失即 fail-fast）。</param>
    /// <param name="corpContext">工具身份环境量（续跑轮还原，作用域工厂据此切「应用 + 企业」）。</param>
    /// <param name="pendingApprovals">待确认快照存储（可空 ⇒ 无幂等消费，重复批准由框架绑定层兜底拒绝）。</param>
    /// <param name="approvalChannel">宿主批准通道（可空 ⇒ 续跑轮再产出审批请求时只记日志）。</param>
    /// <param name="telemetry">遥测端口（可空；实现 <c>IAgentToolTelemetry</c> 时记录 <c>ApprovalResolved</c>）。</param>
    /// <param name="acquireGate">会话闸门获取委托（可空 ⇒ 续跑轮不与事件轮串行）。</param>
    /// <param name="logger">日志（可空）。</param>
    /// <exception cref="ArgumentNullException"><paramref name="agent"/> / <paramref name="scopeFactory"/> / <paramref name="corpContext"/> 为 <see langword="null"/>。</exception>
    public WechatToolApprovalResume(
        WechatAgent agent,
        ITurnScopeFactory scopeFactory,
        IWechatAiCorpContextAccessor corpContext,
        IPendingApprovalStore? pendingApprovals = null,
        IToolApprovalChannel? approvalChannel = null,
        IAgentTelemetry? telemetry = null,
        Func<string, CancellationToken, Task<IDisposable?>>? acquireGate = null,
        ILogger<WechatToolApprovalResume>? logger = null)
    {
        _agent = agent ?? throw new ArgumentNullException(nameof(agent));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _corpContext = corpContext ?? throw new ArgumentNullException(nameof(corpContext));
        _pendingApprovals = pendingApprovals;
        _approvalChannel = approvalChannel;
        _telemetry = telemetry;
        _acquireGate = acquireGate;
        _logger = logger;
    }

    /// <summary>列出某租户下未过期的待确认项（宿主审批界面/运维排查用）。</summary>
    /// <param name="tenantKey">租户/应用标识（隔离维度，非空）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>未过期的快照（未装配存储时为空集合）。</returns>
    public async Task<IReadOnlyList<PendingApprovalSnapshot>> ListPendingAsync(
        string tenantKey,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tenantKey))
        {
            throw new ArgumentException("必须给出租户/应用标识（隔离维度）", nameof(tenantKey));
        }

        if (_pendingApprovals is null)
        {
            return [];
        }

        return await _pendingApprovals.ListPendingAsync(tenantKey, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>回灌一次人工批准（或拒绝）并续跑同一会话。</summary>
    /// <param name="approval">框架审批请求要素（宿主须原样回传，尤其 <c>RequestId</c>）。</param>
    /// <param name="approved">是否批准（<see langword="false"/> 同样走完整流程：拒绝结果回填模型）。</param>
    /// <param name="reason">批准/拒绝原因（可空；会回填给模型，不得含敏感信息）。</param>
    /// <param name="principal">
    /// 续跑轮的工具身份（可空 ⇒ 用 <c>TenantKey</c> + <c>UserId</c> 构造）。
    /// 第三方/代开发场景<b>应显式传入</b> <c>AuthCorpId</c>：它是企业级令牌的唯一 scope 来源，缺它只能切到应用级作用域。
    /// </param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>续跑轮的模型回应（含可能新增的审批请求内容）。</returns>
    /// <exception cref="InvalidOperationException">该确认已失效（不存在/已过期/已被消费）——fail-closed，绝不重放。</exception>
    public async Task<AgentResponse> ResumeAsync(
        FrameworkToolApprovalRequest approval,
        bool approved,
        string? reason = null,
        WechatToolPrincipal? principal = null,
        CancellationToken cancellationToken = default)
    {
        if (approval is null)
        {
            throw new ArgumentNullException(nameof(approval));
        }

        // 快照只在「这次批准确实可见」时读一次：它同时给出工具名与人类等待时长（续跑闭环本身只知道「已批准」结论）。
        var snapshot = _pendingApprovals is null
            ? null
            : await _pendingApprovals
                .FindAsync(approval.TenantKey, approval.RequestId, cancellationToken)
                .ConfigureAwait(false);

        var effectivePrincipal = principal ?? new WechatToolPrincipal(approval.TenantKey, openId: approval.UserId);

        var response = await AgentApprovalContinuation.RunAsync(
            agent: _agent,
            sessions: _agent,
            scopeFactory: _scopeFactory,
            approval: approval,
            approved: approved,
            reason: reason,
            acquireGate: _acquireGate,
            approvalChannel: _approvalChannel,
            logger: _logger,
            pendingApprovalStore: _pendingApprovals,
            cancellationToken: cancellationToken,
            telemetry: _telemetry,
            // 产品环境量恢复（外层作用域）：先于工具作用域建立，覆盖续跑全程（模型调用 + 落库）。
            restoreScope: _ => _corpContext.Begin(effectivePrincipal)).ConfigureAwait(false);

        // HITL 健康度指标的归属点就在这里：等待时长只有宿主批准入口知道（中立闭环取不到起止时刻，不在那里伪造 0）。
        var waitMs = snapshot is null
            ? 0L
            : (long)Math.Max(0, (DateTimeOffset.UtcNow - snapshot.CreatedAt).TotalMilliseconds);
        (_telemetry as IAgentToolTelemetry)?.ApprovalResolved(
            snapshot?.ToolName ?? approval.ToolName,
            approved,
            waitMs);

        return response;
    }
}
