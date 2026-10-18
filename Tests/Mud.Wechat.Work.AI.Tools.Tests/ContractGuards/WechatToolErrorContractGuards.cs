using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Mud.Wechat.Work.AI.Tools.Tests.ContractGuards;

/// <summary>
/// 结果契约守卫（WT-E1~E3）：<b>失败必须结构化</b>、<b>预算单位必须同源</b>。
/// </summary>
/// <remarks>
/// <para>
/// <b>E1 为什么零容忍上游失败绕过骨架</b>：<c>ToolResult.FromError(string)</c> 不产生
/// <c>ToolResult.Error</c>（实测 <c>Error is null</c>），而执行链用 <c>Error is not null</c> 结算失败、
/// 用 <c>Error.ApiCode</c> 填审计。绕过一次 = 一条上游错误被记成「成功」，失败率与错误码口径同时失真。
/// </para>
/// <para>
/// <b>棘轮（ratchet）语义</b>：参数校验类站点尚未全部改为 <c>ToolArgumentException</c>（FIX-01 第二阶段），
/// 故 E2 冻结当前余量并<b>只许降不许升</b>；任何人新增一处旧工厂都会让守卫变红。E2 归零后本守卫改为
/// 「必须为 0」，不再保留基线。
/// </para>
/// <para>
/// <b>E3 为什么盯单位</b>：策略预算是<b>字符</b>，审计与指标若按 UTF-8 <b>字节</b>计量，则同一策略值在
/// 中英文结果上表现为两个预算，且「预算是否被用尽」无法与审计对账。
/// </para>
/// </remarks>
public class WechatToolErrorContractGuards
{
    /// <summary>
    /// 仍以旧工厂表达「业务空」的文件（<b>不是</b>上游失败）：已全部改为
    /// <c>found:false</c> / <c>items:[]</c> 成功语义，白名单为空即「零容忍」。
    /// </summary>
    private static readonly Dictionary<string, int> BusinessEmptyAllowList = new(StringComparer.Ordinal);

    /// <summary>
    /// 旧工厂余量基线（只许降）：FIX-01 第一阶段已把 62 处上游失败改为结构化，
    /// 余下为参数校验字面量（FIX-01 第二阶段改 <c>ToolArgumentException</c>）。
    /// </summary>
    private const int LegacyFactoryBaseline = 101;

    [Fact]
    public void Executors_UpstreamFailures_ShouldGoThroughStructuredFactory()
    {
        var executors = ExecutorsDirectory();
        var pattern = new Regex(@"ToolResult\.FromError\(\w+\.ErrorText", RegexOptions.CultureInvariant);

        var residual = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var file in Directory.GetFiles(executors, "*.cs", SearchOption.TopDirectoryOnly))
        {
            var count = pattern.Matches(File.ReadAllText(file)).Count;
            if (count > 0)
            {
                residual[Path.GetFileName(file)] = count;
            }
        }

        residual.Should().BeEquivalentTo(
            BusinessEmptyAllowList,
            "上游失败必须经 WechatToolOutcomeErrors.From（→ ToolExecutor.FailIfError）产出结构化载荷；"
            + "白名单只容纳「业务空」站点，且 FIX-03 后须清空");
    }

    [Fact]
    public void Executors_LegacyErrorFactory_ShouldNotGrow()
    {
        var executors = ExecutorsDirectory();
        var total = Directory
            .GetFiles(executors, "*.cs", SearchOption.TopDirectoryOnly)
            .Sum(file => Regex.Matches(File.ReadAllText(file), @"ToolResult\.FromError\(").Count);

        total.Should().BeLessThanOrEqualTo(
            LegacyFactoryBaseline,
            "旧工厂不产生 Error 载荷 ⇒ 结局被结算成成功；余量只许下降（新增一处即红）");
    }

    [Fact]
    public void AuditRecord_ShouldMeasureCharsNotBytes()
    {
        var record = Path.Combine(
            RepoRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Tools", "Security", "WechatToolAuditRecord.cs");
        var source = File.ReadAllText(record);

        source.Should().Contain("ResultChars", "审计体积必须与策略预算同一单位（字符）");
        source.Should().NotContain("ResultBytes", "按字节计量会让中英文结果的预算口径分叉");
    }

    [Fact]
    public void Metrics_ShouldMeasureCharsNotBytes()
    {
        var metrics = Path.Combine(
            RepoRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Tools", "Diagnostics", "WechatToolMetrics.cs");
        var source = File.ReadAllText(metrics);

        source.Should().Contain("wechat.ai.tool.result_chars");
        source.Should().NotContain("result_bytes");
    }

    private static string ExecutorsDirectory()
    {
        var executors = Path.Combine(RepoRoot(), "Src", "AI", "Mud.Wechat.Work.AI.Tools", "Executors");
        Directory.Exists(executors).Should().BeTrue($"执行器目录必须可定位（当前解析到 {executors}）");

        var files = Directory.GetFiles(executors, "*.cs", SearchOption.TopDirectoryOnly);
        files.Should().HaveCountGreaterThan(10, "扫不到执行器文件时守卫会假绿（等于没守）");

        return executors;
    }

    /// <summary>从测试输出目录向上定位仓库根（以 <c>Mud.Wechat.slnx</c> 为锚）。</summary>
    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException($"未能从 {AppContext.BaseDirectory} 向上定位仓库根（缺少 Mud.Wechat.slnx）。");
    }
}
