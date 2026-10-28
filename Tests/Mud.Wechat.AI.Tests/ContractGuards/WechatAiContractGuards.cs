using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Mud.Wechat.AI.Tests.ContractGuards;

/// <summary>
/// AI 域契约守卫（权威描述，改动契约面须同批更新本文件）。
/// </summary>
/// <remarks>
/// <para>AI-G1：<b>业务域零 AI 依赖</b> —— 任何非 AI 源工程不得引用 <c>Mud.AI.*</c> 或 <c>Mud.Wechat.*.AI</c>
/// （AI 是业务域的<b>消费者</b>，反向依赖会让「关掉 AI」变成不可能）。</para>
/// <para>AI-G2：<b>层 1 跨线中立</b> —— <c>Mud.Wechat.AI.Abstractions</c> / <c>Mud.Wechat.AI.Agent</c> 不得引用任何
/// 线专属包（七线 + Core 叶层除外）。</para>
/// <para>AI-G3：<b>布局三段</b> —— AI 工程必须位于 <c>Src/AI/&lt;Project&gt;/</c>：四段形态会被
/// <c>verify-build.ps1</c> 步骤 2 与 <c>audit-config-keys.ps1</c> 的 <c>-Depth 2</c> 枚举静默漏掉（AOT 假绿）。</para>
/// <para>AI-G4：<b><c>Mud.AI.*</c> 单一版本 + 钉版</b> —— 全仓版本集合大小 = 1 且不含浮动区间
/// （上游纪律：产品侧显式钉版、禁浮动；NU1603 在门禁升级为错误）。</para>
/// <para>AI-G7：<b>上游版本与采纳面同批</b> —— 升级 <c>Mud.AI.*</c> 必须同时更新本文件的期望版本，
/// 使「版本升了但能力没接」与「能力接了但版本没升」都变成红灯。</para>
/// <para>（环境量纪律见 <c>WechatAmbientScopeGuards</c>：AI 面禁止裸 <c>AsyncLocal</c>。）</para>
/// </remarks>
public class WechatAiContractGuards
{
    /// <summary>当前采纳的上游版本（升级 Mud.AI.* 时同批修改此处）。</summary>
    private const string ExpectedMudAiVersion = "1.0.2";

    private static readonly string[] LineSpecificPrefixes =
    {
        "Mud.Wechat.Work",
        "Mud.Wechat.OfficialAccount",
        "Mud.Wechat.MiniProgram",
        "Mud.Wechat.Pay",
        "Mud.Wechat.OpenPlatform",
        "Mud.Wechat.Channels",
        "Mud.Wechat.Ads",
    };

    /// <summary>AI-G1：业务域不得引用 AI 域（两个方向：Mud.AI 包 + WeChat AI 包）。</summary>
    [Fact]
    public void BusinessProjects_ShouldNotDependOnAiDomain()
    {
        foreach (var csproj in EnumerateSourceProjects().Where(p => !IsAiProject(p)))
        {
            var content = File.ReadAllText(csproj);
            var name = Path.GetFileName(csproj);

            content.Should().NotContain("Mud.AI.", $"{name} 属业务域，不得引用 Mud.AI.*（AI 是业务的消费者，反向依赖会让关掉 AI 变成不可能）");
            content.Should().NotContain("Mud.Wechat.AI.", $"{name} 属业务域，不得引用 Mud.Wechat.AI.*");
        }
    }

    /// <summary>AI-G2：层 1（跨七线共享）不得引用任何线专属包。</summary>
    /// <remarks>
    /// 2026-10 包合并：层 1 原为 <c>Mud.Wechat.AI.Abstractions</c> + <c>Mud.Wechat.AI.Agent</c> 两工程，
    /// 现并为单工程 <c>Mud.Wechat.AI</c>（命名空间不变），本守卫的扫描面随之收成一个工程。
    /// </remarks>
    [Fact]
    public void SharedAiLayer_ShouldNotReferenceAnyProductLine()
    {
        foreach (var name in new[] { "Mud.Wechat.AI" })
        {
            var csproj = EnumerateSourceProjects().Single(p => Path.GetFileName(p) == $"{name}.csproj");
            var content = File.ReadAllText(csproj);

            foreach (var prefix in LineSpecificPrefixes)
            {
                // 「Mud.Wechat.AI.Agent」自身以 Mud.Wechat.AI 开头，故按「线前缀 + 非 A」判定，避免自匹配。
                var forbidden = new Regex($"{Regex.Escape(prefix)}(?![A-Za-z])", RegexOptions.CultureInvariant);
                forbidden.IsMatch(content).Should().BeFalse(
                    $"{name} 是层 1 跨线共享包，不得引用线专属包 {prefix}*（企微实现须下沉 Mud.Wechat.Work.AI）");
            }
        }
    }

    /// <summary>AI-G3：AI 工程必须平铺三段（<c>Src/AI/&lt;Project&gt;/&lt;Project&gt;.csproj</c>）。</summary>
    [Fact]
    public void AiProjects_ShouldStayTwoLevelInsideSrcAi()
    {
        var aiProjects = EnumerateSourceProjects().Where(IsAiProject).ToList();

        aiProjects.Should().HaveCountGreaterThanOrEqualTo(3, "AI 域当前至少三包（AI.Abstractions / AI.Agent / Work.AI）");

        foreach (var csproj in aiProjects)
        {
            var relative = Path.GetRelativePath(GetSolutionRoot(), csproj).Replace('\\', '/');
            var segments = relative.Split('/');

            segments.Should().HaveCount(4,
                $"{relative} 必须为 Src/AI/<Project>/<Project>.csproj 三段形态；更深嵌套会被 -Depth 2 枚举静默漏掉（AOT 假绿）");
            segments[0].Should().Be("Src");
            segments[1].Should().Be("AI");
            segments[2].Should().Be(Path.GetFileNameWithoutExtension(csproj),
                $"{relative} 的目录名必须与工程同名（门禁按工程名定位）");
        }
    }

    /// <summary>
    /// AI-G5：<b>工具面/协议面单向依赖</b> —— <c>Mud.Wechat.Work.AI</c>（Agent）不得引用 <c>.AI.Tools</c> /
    /// <c>.AI.Mcp</c>，两者之间也互不引用；宿主侧只通过 DI 的 <c>IReadOnlyList&lt;AIFunction&gt;</c> 相遇。
    /// </summary>
    /// <remarks>
    /// 反向引用会让「关掉工具面」变成不可能（Agent 包一装上就拖着 HTTP 执行器与业务域），
    /// 也会把工具面的候选集变成 Agent 的编译期事实——白名单投影因此失去意义。
    /// </remarks>
    [Fact]
    public void AgentPackage_ShouldNotReferenceToolOrMcpPackages()
    {
        var projects = EnumerateSourceProjects().ToList();
        var agent = projects.Single(p => Path.GetFileName(p) == "Mud.Wechat.Work.AI.csproj");

        ReferencedProjects(agent).Should().NotContain(
            name => name.StartsWith("Mud.Wechat.Work.AI.Tools", StringComparison.Ordinal)
                    || name.StartsWith("Mud.Wechat.Work.AI.Mcp", StringComparison.Ordinal),
            "Agent 包不得引用工具面/协议面（DI 的 AIFunction 列表是唯一接缝）");

        foreach (var forbidden in new[] { "Mud.Wechat.Work.AI.Tools", "Mud.Wechat.Work.AI.Mcp" })
        {
            var peer = projects.SingleOrDefault(p => Path.GetFileName(p) == $"{forbidden}.csproj");
            if (peer is null)
            {
                continue;
            }

            var other = forbidden.EndsWith("Tools", StringComparison.Ordinal)
                ? "Mud.Wechat.Work.AI.Mcp"
                : "Mud.Wechat.Work.AI.Tools";

            ReferencedProjects(peer).Should().NotContain(
                name => name.Equals(other, StringComparison.Ordinal),
                $"{forbidden} 与 {other} 互不引用（工具面与协议面各自演进）");
        }
    }

    /// <summary>AI-G4：<c>Mud.AI.*</c> 全仓单一版本且为钉版（禁浮动）。</summary>
    [Fact]
    public void MudAiPackages_ShouldBePinnedToSingleVersion()
    {
        var pattern = new Regex(
            "PackageReference\\s+Include=\"(?<id>Mud\\.AI\\.[A-Za-z.]+)\"\\s+Version=\"(?<version>[^\"]+)\"",
            RegexOptions.CultureInvariant);

        var references = EnumerateSourceProjects()
            .SelectMany(path => pattern.Matches(File.ReadAllText(path)).Select(m => new
            {
                Project = Path.GetFileName(path),
                Id = m.Groups["id"].Value,
                Version = m.Groups["version"].Value,
            }))
            .ToList();

        references.Should().NotBeEmpty($"AI 域各包必须显式钉版消费 Mud.AI.*（当前期望 {ExpectedMudAiVersion}）");

        var versions = references.Select(r => r.Version).Distinct().ToList();
        versions.Should().HaveCount(1,
            "Mud.AI.* 三包必须同版本（Agent 经 NuGet 传递其 Abstractions 依赖，混版会产生程序集重定向歧义）");
        versions[0].Should().Be(ExpectedMudAiVersion,
            "版本与采纳面必须同批（升级上游却仍按旧 API 写，或接了新 API 却忘升版本，都只能靠门禁发现）");

        foreach (var reference in references)
        {
            Regex.IsMatch(reference.Version, @"^\d+\.\d+\.\d+$").Should().BeTrue(
                $"{reference.Project} 的 {reference.Id} 必须是精确版本（禁 * / 区间 / 浮动），实际值: {reference.Version}");
        }
    }

    /// <summary>读出工程文件里的 ProjectReference 目标名（只看 Include 属性，注释与描述文本不参与判定）。</summary>
    private static IEnumerable<string> ReferencedProjects(string csprojPath)
    {
        var pattern = new Regex(
            "<ProjectReference\\s+Include=\"(?<path>[^\"]+)\"",
            RegexOptions.CultureInvariant);

        return pattern.Matches(File.ReadAllText(csprojPath))
            .Select(m => Path.GetFileNameWithoutExtension(m.Groups["path"].Value.Replace('\\', '/')));
    }

    private static bool IsAiProject(string csprojPath)
        => csprojPath.Replace('\\', '/').Contains("/Src/AI/", StringComparison.Ordinal);

    private static IEnumerable<string> EnumerateSourceProjects()
        => Directory.EnumerateFiles(Path.Combine(GetSolutionRoot(), "Src"), "*.csproj", SearchOption.AllDirectories)
            .Where(static p => !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("未找到 Mud.Wechat.slnx");
    }
}
