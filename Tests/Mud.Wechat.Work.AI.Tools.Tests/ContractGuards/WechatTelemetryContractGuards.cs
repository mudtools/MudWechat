using FluentAssertions;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.ContractGuards;

/// <summary>
/// 遥测收编守卫：<b>实现存在</b> + <b>执行链只经端口计数（不双记）</b> + <b>指标不含高基数维度</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么用源码守卫而不是只靠行为用例</b>：行为用例只能证明「当前这条路径计了一次」，证明不了
/// 「以后新增分支不会在调用点再记一次」。端口收编的失败形态是指标<b>翻倍</b>——系统照常运行、日志干净，
/// 只是仪表盘长期撒谎，故必须把「调用点不得自记」写成对源码形状的断言。
/// </para>
/// <para>
/// <b>判据精度</b>：只数代码调用形态 <c>WechatToolMetrics.RecordInvocation(</c> / <c>RecordDuration(</c>；
/// 注释里提到这些名字是合法的（说明文字必须能写清「为什么只经端口」）。
/// </para>
/// </remarks>
public class WechatTelemetryContractGuards
{
    [Fact]
    public void AiSurface_ShouldProvideBothTelemetryImplementations()
    {
        var root = RepoRoot();
        var sources = Directory
            .EnumerateFiles(Path.Combine(root, "Src", "AI"), "*.cs", SearchOption.AllDirectories)
            .Where(static p => !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(File.ReadAllText)
            .ToList();

        sources.Should().Contain(
            s => s.Contains(": IAgentTelemetry", StringComparison.Ordinal),
            "主端口必须有产品实现——否则中立管线落 NoopAgentTelemetry，回合/摘要/流式/token 全零埋点（黑盒运行）");

        sources.Should().Contain(
            s => s.Contains(": IAgentToolTelemetry", StringComparison.Ordinal),
            "工具/审批埋点必须有产品实现（端口派生自主端口，故同一实例可同时服务两条链路）");
    }

    [Fact]
    public void Dispatcher_ShouldCountExecutedCallsOnlyThroughPort()
    {
        var dispatcher = ReadSource("Mud.Wechat.Work.AI.Tools", "Security", "WechatToolDispatcher.cs");

        Count(dispatcher, "WechatToolMetrics.RecordInvocation(").Should().Be(1,
            "执行路径必须只经 IAgentToolTelemetry 计数；未执行分支（拒绝/审批拒绝/预算）保留一处直记 ⇒ 全文件恰好 1 处");
        Count(dispatcher, "WechatToolMetrics.RecordDuration(").Should().Be(0,
            "耗时只能经端口上报：未执行分支没有耗时，若这里出现直记即说明执行路径仍在自记（翻倍）");
        Count(dispatcher, "WechatToolMetrics.RecordResult(").Should().Be(2,
            "结果体积/截断由执行链直记（端口不携带字符数）：成功结算与超时结算各一处");
    }

    [Fact]
    public void ToolTelemetry_ShouldForwardToProductMetricSurface()
    {
        var telemetry = ReadSource("Mud.Wechat.Work.AI.Tools", "Diagnostics", "WechatToolTelemetry.cs");

        telemetry.Should().Contain("WechatToolMetrics.RecordInvocation(",
            "端口实现内部必须转发到既有 wechat.ai.tool.* 命名面（命名面零变化，避免两套真相）");
        telemetry.Should().Contain("WechatToolMetrics.RecordDuration(");
        telemetry.Should().Contain("RiskLabelOf(",
            "风险三档是产品语义：由契约表在端口实现内还原，中立端口只给 isWrite 布尔");
    }

    [Fact]
    public void Metrics_ShouldNotCarryHighCardinalityDimensions()
    {
        // 租户/会话/用户维度只允许出现在 Span 属性里；一旦进指标标签，聚合后端会被基数打爆。
        var files = new[]
        {
            ReadSource("Mud.Wechat.Work.AI", "Diagnostics", "WechatAgentDiagnostics.cs"),
            ReadSource("Mud.Wechat.Work.AI.Tools", "Diagnostics", "WechatToolMetrics.cs"),
        };

        foreach (var source in files)
        {
            foreach (var forbidden in new[] { "\"tenant", "\"conversation", "\"user", "\"chat", "\"session" })
            {
                source.Should().NotContain(forbidden,
                    $"指标标签不得含高基数维度（命中的字面量: {forbidden}）——这类维度只进 Span 属性");
            }
        }
    }

    private static int Count(string source, string token) => source.Split(token, StringSplitOptions.None).Length - 1;

    private static string ReadSource(params string[] segments)
    {
        var parts = new List<string> { RepoRoot(), "Src", "AI" };
        parts.AddRange(segments);
        return File.ReadAllText(Path.Combine([.. parts]));
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }
}
