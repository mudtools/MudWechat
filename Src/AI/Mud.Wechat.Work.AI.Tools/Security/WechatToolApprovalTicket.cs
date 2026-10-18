// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.AI.AgentTools;

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 审批工单：一次待人工确认的写工具调用的可持久化事实（<b>不可变</b>，状态迁移产出新实例）。
/// </summary>
/// <remarks>
/// <para>
/// <b>只存摘要，不存入参</b>：<see cref="ArgumentsDigest"/> 与授权上下文同源（键名排序 + 敏感键掩码）。
/// 续跑时由调用方重新提交入参，状态机校验摘要一致——「批准 A、执行 B」因此不可能发生。
/// </para>
/// <para>
/// <b>不含令牌</b>：身份是「应用 + 企业」，与授权上下文同一事实；工单落库（可能落到分布式存储）
/// 不得把凭据一起带出去。
/// </para>
/// <para>
/// <b>不可变 + <see cref="Revision"/></b>：每次状态迁移产出新实例并 +1 版本，存储据此做乐观并发
/// （防止两个审批人同时判定、或「批准」与「过期」互相覆盖）。可变对象会让
/// 「读—判—写」之间产生别名，版本号随之失效。
/// </para>
/// </remarks>
public sealed class WechatToolApprovalTicket
{
    private WechatToolApprovalTicket(
        string ticketId,
        string toolName,
        string appKey,
        string? authCorpId,
        WechatToolRisk risk,
        IReadOnlyList<string> requiredScopes,
        string argumentsDigest,
        WechatToolApprovalState state,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt,
        DateTimeOffset? decidedAt,
        string? decidedBy,
        string? decisionReason,
        DateTimeOffset? consumedAt,
        long revision)
    {
        TicketId = ticketId;
        ToolName = toolName;
        AppKey = appKey;
        AuthCorpId = authCorpId;
        Risk = risk;
        RequiredScopes = requiredScopes;
        ArgumentsDigest = argumentsDigest;
        State = state;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        DecidedAt = decidedAt;
        DecidedBy = decidedBy;
        DecisionReason = decisionReason;
        ConsumedAt = consumedAt;
        Revision = revision;
    }

    /// <summary>新建一个待审批工单。</summary>
    /// <param name="context">授权上下文（工具 / 应用 / 企业 / 风险 / 权限点 / 入参摘要）。</param>
    /// <param name="timeToLive">工单有效期（超时未判定即过期）。</param>
    /// <returns>待审批工单。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> 为 <see langword="null"/>。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeToLive"/> 不是正数。</exception>
    public static WechatToolApprovalTicket CreatePending(
        WechatToolAuthorizationContext context,
        TimeSpan timeToLive)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (timeToLive <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeToLive), "工单有效期必须是正数（零或负 TTL 等于「不许审批」）。");
        }

        var now = DateTimeOffset.UtcNow;
        return new WechatToolApprovalTicket(
            "wta_" + Guid.NewGuid().ToString("N"),
            context.ToolName,
            context.AppKey,
            context.AuthCorpId,
            context.Risk,
            context.RequiredScopes,
            context.ArgumentsDigest,
            WechatToolApprovalState.Pending,
            now,
            now + timeToLive,
            decidedAt: null,
            decidedBy: null,
            decisionReason: null,
            consumedAt: null,
            revision: 0);
    }

    /// <summary>工单号（对外可见：模型与审批人都以此引用；不含任何业务语义）。</summary>
    public string TicketId { get; }

    /// <summary>被审批的工具名（契约名）。</summary>
    public string ToolName { get; }

    /// <summary>应用键（多应用注册表键）。</summary>
    public string AppKey { get; }

    /// <summary>授权企业 ID（可空 = 未限定企业）。</summary>
    public string? AuthCorpId { get; }

    /// <summary>工具风险档（审批人必须看到「不可逆」这一事实）。</summary>
    public WechatToolRisk Risk { get; }

    /// <summary>工具声明的权限点。</summary>
    public IReadOnlyList<string> RequiredScopes { get; }

    /// <summary>入参审计摘要（已掩码；续跑时按此校验「批准的就是执行的」）。</summary>
    public string ArgumentsDigest { get; }

    /// <summary>当前状态。</summary>
    public WechatToolApprovalState State { get; }

    /// <summary>创建时间（UTC）。</summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>过期时间（UTC）。</summary>
    public DateTimeOffset ExpiresAt { get; }

    /// <summary>判定时间（UTC；未判定为 <see langword="null"/>）。</summary>
    public DateTimeOffset? DecidedAt { get; }

    /// <summary>判定人（宿主侧的审批人标识；未判定为 <see langword="null"/>）。</summary>
    public string? DecidedBy { get; }

    /// <summary>判定理由（批准时记录确认来源；拒绝时为理由）。</summary>
    public string? DecisionReason { get; }

    /// <summary>兑现时间（UTC；未兑现为 <see langword="null"/>）。</summary>
    public DateTimeOffset? ConsumedAt { get; }

    /// <summary>乐观并发凭据（每次状态迁移 +1；写回时须带上读取到的版本）。</summary>
    public long Revision { get; }

    /// <summary>是否仍可被判定（仅待审批且未过期）。</summary>
    public bool IsDecidable(DateTimeOffset nowUtc)
        => State == WechatToolApprovalState.Pending && nowUtc < ExpiresAt;

    /// <summary>是否仍可被兑现（已批准且未过期）。</summary>
    public bool IsConsumable(DateTimeOffset nowUtc)
        => State == WechatToolApprovalState.Approved && nowUtc < ExpiresAt;

    /// <summary>派生「已批准」工单。</summary>
    internal WechatToolApprovalTicket WithApproved(string? approver, string? reason, DateTimeOffset nowUtc)
        => new(
            TicketId, ToolName, AppKey, AuthCorpId, Risk, RequiredScopes, ArgumentsDigest,
            WechatToolApprovalState.Approved, CreatedAt, ExpiresAt,
            nowUtc, approver, reason, ConsumedAt, Revision + 1);

    /// <summary>派生「已拒绝」工单。</summary>
    internal WechatToolApprovalTicket WithRejected(string? approver, string reason, DateTimeOffset nowUtc)
        => new(
            TicketId, ToolName, AppKey, AuthCorpId, Risk, RequiredScopes, ArgumentsDigest,
            WechatToolApprovalState.Rejected, CreatedAt, ExpiresAt,
            nowUtc, approver, reason, ConsumedAt, Revision + 1);

    /// <summary>派生「已过期」工单。</summary>
    internal WechatToolApprovalTicket WithExpired(DateTimeOffset nowUtc)
        => new(
            TicketId, ToolName, AppKey, AuthCorpId, Risk, RequiredScopes, ArgumentsDigest,
            WechatToolApprovalState.Expired, CreatedAt, ExpiresAt,
            nowUtc, DecidedBy, DecisionReason ?? "审批超时，工单已过期", ConsumedAt, Revision + 1);

    /// <summary>派生「已兑现」工单。</summary>
    internal WechatToolApprovalTicket WithConsumed(DateTimeOffset nowUtc)
        => new(
            TicketId, ToolName, AppKey, AuthCorpId, Risk, RequiredScopes, ArgumentsDigest,
            WechatToolApprovalState.Consumed, CreatedAt, ExpiresAt,
            DecidedAt, DecidedBy, DecisionReason, nowUtc, Revision + 1);
}
