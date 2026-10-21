// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Mud.Wechat.Work.AI.Diagnostics;

/// <summary>
/// Agent 运行期诊断面的<b>命名与仪器单一来源</b>：ActivitySource / Meter 名、Span 名、指标名与标签键全部在此定义。
/// </summary>
/// <remarks>
/// <para>
/// <b>Meter/Source 名取单根源 <c>Mud.Wechat</c></b>：与 <c>Mud.Wechat.OpenTelemetry</c> 的通配订阅
/// （<c>Mud.Wechat*</c>）一致，宿主不需要为 AI 运行期单独加一行 <c>AddMeter</c> / <c>AddSource</c>。
/// 工具面的 <c>Mud.Wechat</c> 源/表在 <c>Mud.Wechat.Work.AI.Tools</c> 侧（同名不同程序集，监听按<b>名字</b>订阅，
/// 两者都会被采到）——本类不改动工具面既有的 <c>wechat.ai.tool.*</c> 命名面。
/// </para>
/// <para>
/// <b>维度纪律</b>：租户 / 会话键只进 <b>Span 属性</b>，绝不进 Metrics 标签（高基数维度会让聚合后端爆炸）；
/// 指标只有 <c>agent</c>（有界：每进程一个 Agent 名）与 <c>reason</c>（有限枚举）两个维度。
/// </para>
/// <para>
/// <b>零监听者零成本</b>：无 <see cref="ActivityListener"/> 时 <see cref="ActivitySource.StartActivity(string, ActivityKind)"/>
/// 返回 <see langword="null"/>，各入口据此直接返回（不分配作用域对象、不写标签）。
/// </para>
/// </remarks>
internal static class WechatAgentDiagnostics
{
    /// <summary>诊断根源名（与 OpenTelemetry 装配包的通配订阅一致）。</summary>
    public const string SourceName = "Mud.Wechat";

    /// <summary>一轮会话的 Span 名（租户/会话键只作其属性）。</summary>
    public const string TurnActivityName = "wechat.ai.agent.turn";

    /// <summary>一次模型运行的 Span 名（成败与 token 用量作其属性）。</summary>
    public const string RunActivityName = "wechat.ai.agent.run";

    /// <summary>一次会话历史摘要的 Span 名（嵌套在回合内）。</summary>
    public const string SummarizeActivityName = "wechat.ai.agent.summarize";

    /// <summary>模型调用耗时直方图（唯一维度：<c>agent</c>）。</summary>
    public const string LlmDurationMetricName = "wechat.ai.agent.llm.duration";

    /// <summary>降级计数（唯一维度：<c>reason</c>，有限枚举）。</summary>
    public const string DegradedMetricName = "wechat.ai.agent.degraded";

    /// <summary>会话键属性（只进 Span）。</summary>
    public const string ConversationKeyTag = "agent.conversation_key";

    /// <summary>租户/应用标识属性（只进 Span）。</summary>
    public const string TenantKeyTag = "agent.tenant_key";

    /// <summary>闸门等待毫秒（只进 Span：它是单轮事实，不是可分位样本）。</summary>
    public const string GateWaitTag = "agent.gate_wait_ms";

    /// <summary>本轮是否命中「待补发回复」重投递。</summary>
    public const string ReplayedTag = "agent.replayed";

    /// <summary>本轮回复是否已由流式通道送达。</summary>
    public const string StreamedTag = "agent.streamed";

    /// <summary>Agent 名属性/指标维度。</summary>
    public const string AgentTag = "agent";

    /// <summary>运行是否走流式。</summary>
    public const string StreamingTag = "agent.streaming";

    /// <summary>运行是否以异常结束。</summary>
    public const string FailedTag = "agent.failed";

    /// <summary>输入 token 数。</summary>
    public const string InputTokensTag = "agent.tokens.input";

    /// <summary>输出 token 数。</summary>
    public const string OutputTokensTag = "agent.tokens.output";

    /// <summary>总 token 数。</summary>
    public const string TotalTokensTag = "agent.tokens.total";

    /// <summary>被压缩的旧消息条数。</summary>
    public const string SummarizedTag = "agent.summarized";

    /// <summary>保留的最近消息条数。</summary>
    public const string RetainedTag = "agent.retained";

    /// <summary>摘要后是否仍未收敛到 token 预算内（单条消息即超预算）。</summary>
    public const string UnconvergedTag = "agent.summarized_unconverged";

    /// <summary>降级原因维度。</summary>
    public const string ReasonTag = "reason";

    private static readonly ActivitySource Source = new(SourceName, "1.0.0");

    private static readonly Meter Meter = new(SourceName, "1.0.0");

    private static readonly Histogram<double> LlmDurationHistogram = Meter.CreateHistogram<double>(
        LlmDurationMetricName,
        unit: "ms",
        description: "模型调用耗时（失败样本同样记录；维度：agent）。");

    private static readonly Counter<long> DegradedCounter = Meter.CreateCounter<long>(
        DegradedMetricName,
        unit: "{event}",
        description: "被隔离的降级事件次数（维度：reason）。");

    /// <summary>开启一个 Span（无监听者时返回 <see langword="null"/>，零分配）。</summary>
    /// <param name="name">Span 名（取值限本类常量）。</param>
    /// <param name="kind">Span 类型。</param>
    /// <returns>Span 作用域（可空）。</returns>
    public static ActivityScope? Start(string name, ActivityKind kind = ActivityKind.Internal)
    {
        var activity = Source.StartActivity(name, kind);
        return activity is null ? null : new ActivityScope(activity);
    }

    /// <summary>记录一次模型调用耗时。</summary>
    /// <param name="agentName">Agent 名。</param>
    /// <param name="milliseconds">耗时（毫秒）。</param>
    public static void RecordLlmDuration(string agentName, double milliseconds)
        => LlmDurationHistogram.Record(milliseconds, new TagList { { AgentTag, agentName } });

    /// <summary>记录一次已发生的降级。</summary>
    /// <param name="reason">降级原因（有限枚举字面量）。</param>
    public static void RecordDegraded(string reason)
        => DegradedCounter.Add(1, new TagList { { ReasonTag, reason } });

    /// <summary>
    /// Span 作用域：把「停止 Span」这件事与 <see cref="IDisposable"/> 对齐，供中立端口的阶段句柄语义直接消费。
    /// </summary>
    /// <remarks>
    /// 不直接用 <see cref="Activity"/> 作句柄：中立端口各方法需要在<b>句柄上</b>追加标签（如闸门等待、重投递标记），
    /// 而 <see cref="Activity"/> 类型本身属于 BCL——端口层不得命名它（否则产品遥测实现被迫暴露 BCL 类型）。
    /// </remarks>
    /// <param name="activity">已开启的 Span（非空）。</param>
    internal sealed class ActivityScope(Activity activity) : IDisposable
    {
        private int _disposed;

        /// <summary>底层 Span（本程序集内追加标签用）。</summary>
        public Activity Activity { get; } = activity;

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            Activity.Dispose();
        }
    }
}
