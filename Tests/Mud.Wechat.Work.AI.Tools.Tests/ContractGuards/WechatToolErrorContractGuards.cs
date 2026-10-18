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
/// <b>E2 为零容忍</b>：FIX-01 收官后，执行器里<b>调用</b>旧工厂的余量为 0（文档注释中提及该 API 不算）。
/// 三态出口固定为：上游失败 → <c>WechatToolOutcomeErrors.From</c>；参数错 → <c>WechatToolArgumentErrors.Invalid</c>；
/// 业务空 → <c>WechatToolResults.NotFound/EmptyList</c>。
/// </para>
/// <para>
/// <b>E3 为什么盯单位</b>：策略预算是<b>字符</b>，审计与指标若按 UTF-8 <b>字节</b>计量，则同一策略值在
/// 中英文结果上表现为两个预算，且「预算是否被用尽」无法与审计对账。
/// </para>
/// </remarks>
public class WechatToolErrorContractGuards
{
    [Fact]
    public void Executors_UpstreamFailures_ShouldGoThroughStructuredFactory()
    {
        var executors = ExecutorsDirectory();
        var pattern = new Regex(@"ToolResult\.FromError\(\w+\.ErrorText", RegexOptions.CultureInvariant);

        var residual = Directory
            .GetFiles(executors, "*.cs", SearchOption.TopDirectoryOnly)
            .Where(file => pattern.IsMatch(File.ReadAllText(file)))
            .Select(Path.GetFileName)
            .ToArray();

        residual.Should().BeEmpty(
            "上游失败必须经 WechatToolOutcomeErrors.From（→ ToolExecutor.FailIfError）产出结构化载荷");
    }

    [Fact]
    public void Executors_LegacyErrorFactory_ShouldBeGone()
    {
        var offenders = Directory
            .GetFiles(ExecutorsDirectory(), "*.cs", SearchOption.TopDirectoryOnly)
            .Select(file => (Name: Path.GetFileName(file), Source: File.ReadAllText(file)))
            // 文档注释里提到该 API 是合法的（说明「为什么不用它」），只禁**调用**。
            .Select(f => (f.Name, Calls: Regex.Matches(f.Source, @"(?:return|=>)\s*ToolResult\.FromError\(").Count))
            .Where(f => f.Calls > 0)
            .Select(f => $"{f.Name}×{f.Calls}")
            .ToArray();

        offenders.Should().BeEmpty(
            "旧工厂不产生 Error 载荷 ⇒ 上游失败/参数错误被结算成成功；"
            + "上游失败走 WechatToolOutcomeErrors，参数错走 WechatToolArgumentErrors.Invalid，业务空走 WechatToolResults");
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
