// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.Metrics;


namespace Mud.Wechat.Work.AI.Tools.Diagnostics;

/// <summary>
/// 工具面指标：调用量 / 耗时 / 结果体积 / 截断次数。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有</b>：审计只回答「发生了什么」（单条事实），回答不了「P95 多少、失败率多少、拒绝率多少、
/// 谁的配额打满了」。没有指标时，模型侧体验退化（工具变慢、频繁截断）无法被定位，只能靠用户投诉。
/// </para>
/// <para>
/// <b>Meter 名取单根源 <c>Mud.Wechat</c></b>：与 <c>Mud.Wechat.OpenTelemetry</c> 的通配订阅一致，
/// 宿主不需要为 AI 工具面单独加一行 <c>AddMeter</c>。指标名用 <c>wechat.ai.tool.*</c>，
/// 与本仓既有 <c>Mud.Wechat</c> 指标（令牌/回调）同前缀不同段，避免语义混淆。
/// </para>
/// <para>
/// <b>标签是低基数维度</b>：<c>tool</c>（61 个固定值）、<c>risk</c>（3 档）、<c>outcome</c>（4 类）。
/// <b>禁止</b>把 <c>userid</c>/<c>appKey</c>/<c>corpId</c> 放进标签——那是高基数，会打爆聚合后端；
/// 租户/发起人维度在审计记录里（可按需查单条），不在指标里。
/// </para>
/// <para>
/// <b>结果体积用直方图而不是计数器</b>：关心的是分布（截断前有多大），不是总量。
/// </para>
/// </remarks>
internal static class WechatToolMetrics
{
    /// <summary>指标与追踪的根源名（与 OpenTelemetry 装配包的通配订阅一致）。</summary>
    public const string MeterName = "Mud.Wechat";

    /// <summary>追踪源（span 名固定为 <c>wechat.ai.tool</c>，工具名进标签而不是进 span 名）。</summary>
    /// <remarks>
    /// <b>为什么工具名进标签不进 span 名</b>：span 名是聚合维度，62 个工具各占一个名字会让追踪后端的分组失去意义；
    /// 工具名作为 <c>wechat.ai.tool.name</c> 标签既能聚合也能筛选。
    /// </remarks>
    private static readonly ActivitySource Source = new(MeterName, "1.0.0");

    /// <summary>开启一次工具调用的 span（无监听者时返回 <see langword="null"/>，零成本）。</summary>
    /// <param name="tool">工具名。</param>
    /// <param name="risk">风险档标签。</param>
    /// <returns>活动的 span（可空）。</returns>
    public static Activity? StartActivity(string tool, string risk)
    {
        var activity = Source.StartActivity("wechat.ai.tool", ActivityKind.Client);
        if (activity is null)
        {
            return null;
        }

        activity.SetTag("wechat.ai.tool.name", tool);
        activity.SetTag("wechat.ai.tool.risk", risk);
        return activity;
    }

    /// <summary>结束一次调用的 span：写结局、体积、截断与错误状态。</summary>
    /// <param name="activity">span（可空）。</param>
    /// <param name="outcome">结局标签。</param>
    /// <param name="chars">结果字符数。</param>
    /// <param name="truncated">是否被预算截断。</param>
    public static void CompleteActivity(Activity? activity, string outcome, long chars, bool truncated)
    {
        if (activity is null)
        {
            return;
        }

        activity.SetTag("wechat.ai.tool.outcome", outcome);
        activity.SetTag("wechat.ai.tool.result_chars", chars);
        activity.SetTag("wechat.ai.tool.truncated", truncated);

        if (!string.Equals(outcome, "succeeded", StringComparison.Ordinal))
        {
            activity.SetStatus(ActivityStatusCode.Error, outcome);
        }
    }

    private static readonly Meter Meter = new(MeterName, "1.0.0");

    private static readonly Counter<long> Invocations = Meter.CreateCounter<long>(
        "wechat.ai.tool.invocations",
        unit: "{call}",
        description: "工具调用次数（按工具 / 风险档 / 结局）。");

    private static readonly Histogram<double> Duration = Meter.CreateHistogram<double>(
        "wechat.ai.tool.duration",
        unit: "ms",
        description: "工具调用耗时（含授权、审批、执行与出站）。");

    private static readonly Histogram<long> ResultChars = Meter.CreateHistogram<long>(
        "wechat.ai.tool.result_chars",
        unit: "{char}",
        description: "出站结果体积（字符数；与策略预算同一单位，截断后的实际体积）。");

    private static readonly Counter<long> Truncations = Meter.CreateCounter<long>(
        "wechat.ai.tool.truncations",
        unit: "{call}",
        description: "因预算被截断的调用次数（预算调优与列表工具分页的依据）。");

    /// <summary>记录一次调用的结局。</summary>
    /// <param name="tool">工具名。</param>
    /// <param name="risk">风险档标签（read / write / high-risk-write）。</param>
    /// <param name="outcome">结局标签（succeeded / failed / denied / approval_rejected）。</param>
    public static void RecordInvocation(string tool, string risk, string outcome)
        => Invocations.Add(1, new TagList { { "tool", tool }, { "risk", risk }, { "outcome", outcome } });

    /// <summary>记录一次调用的耗时。</summary>
    /// <param name="tool">工具名。</param>
    /// <param name="risk">风险档标签。</param>
    /// <param name="outcome">结局标签。</param>
    /// <param name="milliseconds">耗时（毫秒）。</param>
    public static void RecordDuration(string tool, string risk, string outcome, double milliseconds)
        => Duration.Record(milliseconds, new TagList { { "tool", tool }, { "risk", risk }, { "outcome", outcome } });

    /// <summary>记录出站结果体积与是否截断。</summary>
    /// <param name="tool">工具名。</param>
    /// <param name="chars">结果字符数。</param>
    /// <param name="truncated">是否被预算截断。</param>
    public static void RecordResult(string tool, long chars, bool truncated)
    {
        ResultChars.Record(chars, new TagList { { "tool", tool } });
        if (truncated)
        {
            Truncations.Add(1, new TagList { { "tool", tool } });
        }
    }
}
