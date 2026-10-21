// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Observability;
using Mud.Wechat.Work.AI.AgentTools;
using Mud.Wechat.Work.AI.Tools.Generated;

namespace Mud.Wechat.Work.AI.Tools.Diagnostics;

/// <summary>
/// 工具面遥测实现：把中立 <see cref="IAgentToolTelemetry"/> 的埋点入口<b>收编</b>到既有
/// <c>wechat.ai.tool.*</c> 命名面（不新增第二套真相），Agent 级成员原样转发给主遥测实现。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是「收编」而不是「并存」</b>：调用次数若同时由端口与自建指标各记一次，同一批调用会出现两份数字
/// （且口径必然漂移——一边按风险档、一边按布尔读写）。故执行链<b>只调端口</b>，端口内部再落到既有指标：
/// 命名面零变化（仪表盘与告警不用改），真相只有一个。
/// </para>
/// <para>
/// <b>风险档是产品语义</b>：中立端口只给 <c>isWrite</c> 布尔，而产品有三档（read / write / high-risk-write）。
/// 三档由<b>契约表</b>（<see cref="WechatToolContracts.ByToolName"/>）在此还原，且只在产品侧完成——
/// 中立端口不因产品风险词表而变形。
/// </para>
/// <para>
/// <b>为什么主端口成员要转发</b>：<see cref="IAgentToolTelemetry"/> 派生自 <see cref="IAgentTelemetry"/>，
/// 装配入口把<b>本实例</b>交给中立管线（同一次调用同时需要工具埋点与回合埋点）。若把主端口成员实现成空操作，
/// 回合/摘要/流式埋点会静默消失——那正是「遥测实现装错顺序」这类缺陷的另一种形态。故未接主端口时回落到
/// <see cref="NoopAgentTelemetry"/>，接入后逐字转发。
/// </para>
/// <para>
/// <b>截断维度不由端口计</b>：端口签名只给 <c>truncated</c> 布尔而不给结果字符数，而产品的
/// <c>wechat.ai.tool.result_chars</c> / <c>truncations</c> 由执行链在同一处（结果已存在时）同源记录。
/// 在端口再记一次会让截断率翻倍，故此处<b>刻意</b>不用该参数。
/// </para>
/// </remarks>
internal sealed class WechatToolTelemetry : IAgentToolTelemetry
{
    /// <summary>无主端口时的默认实例（工具指标照常生效；回合级埋点落空实现）。</summary>
    public static readonly WechatToolTelemetry Instance = new(NoopAgentTelemetry.Instance);

    private readonly IAgentTelemetry _inner;

    /// <summary>初始化工具面遥测。</summary>
    /// <param name="inner">主遥测实现（Agent 级成员转发目标；<see langword="null"/> ⇒ 空实现）。</param>
    public WechatToolTelemetry(IAgentTelemetry? inner = null)
        => _inner = inner ?? NoopAgentTelemetry.Instance;

    /// <inheritdoc />
    public IDisposable? StartTurn(string conversationKey, string? tenantKey)
        => _inner.StartTurn(conversationKey, tenantKey);

    /// <inheritdoc />
    public void GateWaited(IDisposable? turn, double milliseconds) => _inner.GateWaited(turn, milliseconds);

    /// <inheritdoc />
    public void Replayed(IDisposable? turn) => _inner.Replayed(turn);

    /// <inheritdoc />
    public void Streamed(IDisposable? turn) => _inner.Streamed(turn);

    /// <inheritdoc />
    public void LlmDuration(string agentName, long milliseconds) => _inner.LlmDuration(agentName, milliseconds);

    /// <inheritdoc />
    public void Degraded(string reason) => _inner.Degraded(reason);

    /// <inheritdoc />
    public IDisposable? StartSummarize(int summarizedCount, int retainedCount)
        => _inner.StartSummarize(summarizedCount, retainedCount);

    /// <inheritdoc />
    public void SummarizeCompleted(IDisposable? scope, bool converged)
        => _inner.SummarizeCompleted(scope, converged);

    /// <inheritdoc />
    public IDisposable? StartRun(string agentName, bool streaming) => _inner.StartRun(agentName, streaming);

    /// <inheritdoc />
    public void RunCompleted(
        IDisposable? scope,
        bool failed,
        long? inputTokens,
        long? outputTokens,
        long? totalTokens)
        => _inner.RunCompleted(scope, failed, inputTokens, outputTokens, totalTokens);

    /// <inheritdoc />
    public void ToolInvoked(string toolName, bool isWrite, long durationMs, bool truncated, string? errorCategory)
    {
        _ = truncated;

        var risk = RiskLabelOf(toolName, isWrite);
        var outcome = errorCategory is null ? "succeeded" : "failed";

        WechatToolMetrics.RecordInvocation(toolName, risk, outcome);
        WechatToolMetrics.RecordDuration(toolName, risk, outcome, durationMs);
    }

    /// <inheritdoc />
    public void ToolRetried(string toolName, int attempt)
        => WechatToolMetrics.RecordRetry(toolName, attempt);

    /// <inheritdoc />
    public void RetrievalPerformed(int chunks, int injectedChars)
    {
        // RAG 未落地（无宿主向量库、企微无 Embedding API）：此处不埋点是刻意的——
        // 报告「零召回」会让看板显示「检索一直在失败」，而事实是「检索尚未启用」。
    }

    /// <inheritdoc />
    public void ApprovalRequested(string toolName)
        => WechatToolMetrics.RecordApprovalRequest(toolName);

    /// <inheritdoc />
    public void ApprovalResolved(string toolName, bool approved, long waitMs)
        => WechatToolMetrics.RecordApprovalResolved(toolName, approved, waitMs);

    /// <summary>把「读写布尔」还原为产品三档风险标签（契约表优先，缺表时按布尔降级）。</summary>
    private static string RiskLabelOf(string toolName, bool isWrite)
    {
        if (WechatToolContracts.ByToolName.TryGetValue(toolName, out var contract))
        {
            return contract.Risk switch
            {
                WechatToolRisk.Read => "read",
                WechatToolRisk.Write => "write",
                _ => "high-risk-write",
            };
        }

        return isWrite ? "write" : "read";
    }
}
