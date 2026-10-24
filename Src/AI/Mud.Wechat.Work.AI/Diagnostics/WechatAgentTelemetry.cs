// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using Mud.AI.Abstractions.Observability;

namespace Mud.Wechat.Work.AI.Diagnostics;

/// <summary>
/// 企微侧 <see cref="IAgentTelemetry"/> 实现：把中立回合管线的埋点入口映射到 <c>wechat.ai.agent.*</c> 命名面。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有</b>：中立管线在无实现时落 <c>NoopAgentTelemetry</c>——回合、闸门等待、摘要、流式与 token 用量
/// <b>全部零埋点</b>，线上只剩「模型变慢 / 会话串台」这类无定位信息的主观描述。本类型是这些事实的唯一落点。
/// </para>
/// <para>
/// <b>句柄语义</b>：各 <c>Start*</c> 返回的句柄就是 Span 作用域（无监听者时为 <see langword="null"/>）；
/// 中途追加的标量（闸门等待、重投递、流式标记）写进当前 Span，<b>不</b>另开指标——它们是单轮事实，
/// 做成指标只会引入高基数维度。
/// </para>
/// <para>
/// <b>只实现主端口</b>：工具/检索/审批三类埋点归 <c>Mud.Wechat.Work.AI.Tools</c> 的
/// <c>IAgentToolTelemetry</c> 实现（它需要工具风险表与既有 <c>wechat.ai.tool.*</c> 命名面）；
/// 本类型不实现该扩展端口，避免出现「两套工具指标实现」。
/// </para>
/// </remarks>
internal sealed class WechatAgentTelemetry : IAgentTelemetry
{
    /// <inheritdoc />
    public IDisposable? StartTurn(string conversationKey, string? tenantKey)
    {
        var scope = WechatAgentDiagnostics.Start(WechatAgentDiagnostics.TurnActivityName, ActivityKind.Internal);
        if (scope is null)
        {
            return null;
        }

        scope.Activity.SetTag(WechatAgentDiagnostics.ConversationKeyTag, conversationKey);
        if (!string.IsNullOrWhiteSpace(tenantKey))
        {
            scope.Activity.SetTag(WechatAgentDiagnostics.TenantKeyTag, tenantKey);
        }

        return scope;
    }

    /// <inheritdoc />
    public void GateWaited(IDisposable? turn, double milliseconds)
    {
        if (turn is WechatAgentDiagnostics.ActivityScope scope)
        {
            scope.Activity.SetTag(WechatAgentDiagnostics.GateWaitTag, milliseconds);
        }
    }

    /// <inheritdoc />
    public void Replayed(IDisposable? turn)
    {
        if (turn is WechatAgentDiagnostics.ActivityScope scope)
        {
            scope.Activity.SetTag(WechatAgentDiagnostics.ReplayedTag, true);
        }
    }

    /// <inheritdoc />
    public void Streamed(IDisposable? turn)
    {
        if (turn is WechatAgentDiagnostics.ActivityScope scope)
        {
            scope.Activity.SetTag(WechatAgentDiagnostics.StreamedTag, true);
        }
    }

    /// <inheritdoc />
    public void LlmDuration(string agentName, long milliseconds)
        => WechatAgentDiagnostics.RecordLlmDuration(agentName, milliseconds);

    /// <inheritdoc />
    public void Degraded(string reason)
        => WechatAgentDiagnostics.RecordDegraded(reason);

    /// <inheritdoc />
    public IDisposable? StartSummarize(int summarizedCount, int retainedCount)
    {
        var scope = WechatAgentDiagnostics.Start(WechatAgentDiagnostics.SummarizeActivityName, ActivityKind.Internal);
        if (scope is null)
        {
            return null;
        }

        scope.Activity.SetTag(WechatAgentDiagnostics.SummarizedTag, summarizedCount);
        scope.Activity.SetTag(WechatAgentDiagnostics.RetainedTag, retainedCount);
        return scope;
    }

    /// <inheritdoc />
    public void SummarizeCompleted(IDisposable? scope, bool converged)
    {
        if (scope is WechatAgentDiagnostics.ActivityScope activityScope)
        {
            // 只标「未收敛」：收敛是常态，标 false 只会给追踪后端增加噪声维度。
            activityScope.Activity.SetTag(WechatAgentDiagnostics.UnconvergedTag, !converged);
        }
    }

    /// <inheritdoc />
    public IDisposable? StartRun(string agentName, bool streaming)
    {
        var scope = WechatAgentDiagnostics.Start(WechatAgentDiagnostics.RunActivityName, ActivityKind.Internal);
        if (scope is null)
        {
            return null;
        }

        scope.Activity.SetTag(WechatAgentDiagnostics.AgentTag, agentName);
        scope.Activity.SetTag(WechatAgentDiagnostics.StreamingTag, streaming);
        return scope;
    }

    /// <inheritdoc />
    public void RunCompleted(
        IDisposable? scope,
        bool failed,
        long? inputTokens,
        long? outputTokens,
        long? totalTokens)
    {
        if (scope is not WechatAgentDiagnostics.ActivityScope activityScope)
        {
            return;
        }

        var activity = activityScope.Activity;
        activity.SetTag(WechatAgentDiagnostics.FailedTag, failed);

        // 未上报的用量不得填 0（0 会被读成「本轮确实零 token」）。
        if (inputTokens is { } input)
        {
            activity.SetTag(WechatAgentDiagnostics.InputTokensTag, input);
        }

        if (outputTokens is { } output)
        {
            activity.SetTag(WechatAgentDiagnostics.OutputTokensTag, output);
        }

        if (totalTokens is { } total)
        {
            activity.SetTag(WechatAgentDiagnostics.TotalTokensTag, total);
        }

        if (failed)
        {
            activity.SetStatus(ActivityStatusCode.Error);
        }
    }
}
