// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 路由计数纪律守卫（方案 F9 / F10 的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何需要本守卫</b>：v1 出现过「<c>material/get_material</c> 跨批次重复计数」与
/// 「源内字面量 128 / 假路由 1 条」两处账面误差（方案 §1.1 勘误 E1/E4）；AOT 与端点规模账
/// 必须由脚本/守卫而非人工归纳。
/// </para>
/// <para>
/// <b>计数口径（方案 F10 的定义，勿改）</b>：
/// </para>
/// <list type="number">
/// <item><b>实现面</b> = 主程序集公开接口上的 HTTP 路由特性 <c>RequestUri</c> <b>去重</b>集合
/// （同一路由的多方法形态——如 OCR 的 <c>*ByUpload</c>/<c>*ByUrl</c>——只计 <b>1</b> 条端点）+ Abstractions
/// 令牌 / 票据接口路由 + 下载通道 3 条路径常量（无 HTTP 特性）。</item>
/// <item><b>官方面</b> = 索引页表格「请求路径」单元格取值并集（仅当 <c>.docs/</c> 存在时断言——
/// 该目录已 gitignore，fresh clone 下本项自动跳过，<b>不</b>作为门禁前置）。</item>
/// <item><b>禁止</b>：按路径前缀正则扫描（漏计含 <c>.</c> 的路径）；跨批次累加「新增端点数」；
/// 把 XML/DTO 中的示例假路由（如 <c>/xxx/sns/xxx</c>）或路由前缀常量（<c>/</c>）计入端点。</item>
/// </list>
/// </remarks>
public class MpRouteCountGuard
{
    // 三个聚合计数集中维护在 Tests/ContractBaseline.cs（改数字只改那一处）；
    // 各自的推导算式（各域增量、基座与下载通道的加项）也记在该文件的 remarks 中。
    private const int ExpectedMainInterfaceRoutes = Baseline.OfficialAccount.MainInterfaceRoutes;
    private const int ExpectedTotalRoutes = Baseline.OfficialAccount.TotalRoutes;
    private const int ExpectedOfficialRoutes = Baseline.OfficialAccount.OfficialRoutes;

    /// <summary>核验守卫 RC1：主程序集接口路由去重计数与方案台账一致。</summary>
    [Fact]
    public void MainAssemblyRoutes_ShouldMatchLedger()
    {
        var routes = MainInterfaceRoutes();
        routes.Should().HaveCount(ExpectedMainInterfaceRoutes,
            "新增端点必须同批更新本常量与方案 §1.2 台账（防跨批次重复计数 / 漏计）");

        // 同一接口内不得重复声明同一路由（双形态方法共享路由是允许的，但路由集合必须去重后一致）。
        routes.Should().OnlyHaveUniqueItems();
    }

    /// <summary>核验守卫 RC2：全量去重路由 = 接口特性 + 令牌/票据 + 下载通道路径常量。</summary>
    [Fact]
    public void TotalRoutes_ShouldMatchLedger()
    {
        var main = MainInterfaceRoutes().ToHashSet(StringComparer.Ordinal);

        // Abstractions 侧令牌 / 票据接口（3 条：token / stable_token / ticket/getticket）。
        var authRoutes = typeof(IMpAuthentication).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .SelectMany(t => t.GetMethods())
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(u => u != null)
            .Select(u => u!)
            .ToHashSet(StringComparer.Ordinal);

        // 下载通道 3 条（MpMediaDownloadService 的路径常量，非 HTTP 特性；响应为文件流不走生成管线）。
        var downloadRoutes = new HashSet<string>(StringComparer.Ordinal)
        {
            "/cgi-bin/media/get",
            "/cgi-bin/media/get/jssdk",
            "/cgi-bin/material/get_material",
        };
        typeof(MpMediaDownloadService).Should().NotBeNull("下载通道服务必须存在（3 条路由承载体）");

        var total = new HashSet<string>(main, StringComparer.Ordinal);
        total.UnionWith(authRoutes);
        total.UnionWith(downloadRoutes);

        // 下载 3 条与接口特性路由不得重叠（否则说明下载通道被误建进生成管线）。
        main.Should().NotIntersectWith(downloadRoutes,
            "下载通道走独立请求形态，不得同时出现在接口路由特性中");
        authRoutes.Should().HaveCount(3, "Abstractions 侧仅有 token / stable_token / ticket/getticket 三端点");

        total.Should().HaveCount(ExpectedTotalRoutes,
            "全量去重路由数 = 方案 §1.2 已实现端点总数（P3 完成为 145）");
    }

    /// <summary>核验守卫 RC3：同一路由不得落在两个不同接口上（防路由冲突 / 误复制）。</summary>
    [Fact]
    public void Routes_ShouldNotBeSharedAcrossInterfaces()
    {
        var byRoute = typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .SelectMany(t => t.GetMethods()
                .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
                .Where(u => u != null)
                .Select(u => (Route: u!, Interface: t.Name)))
            .GroupBy(x => x.Route, x => x.Interface)
            .Where(g => g.Distinct().Count() > 1)
            .Select(g => $"{g.Key} → {string.Join(", ", g.Distinct())}")
            .ToList();

        byRoute.Should().BeEmpty("同一路由跨接口出现即契约面冲突（同接口双形态方法共享路由是允许的）");
    }

    /// <summary>
    /// 核验守卫 RC4：官方面索引页唯一路由数（条件断言——<c>.docs/</c> 为 gitignored 本地目录）。
    /// </summary>
    [Fact]
    public void OfficialIndexRoutes_ShouldMatchLedgerWhenDocsAvailable()
    {
        var indexPath = FindOfficialIndex();
        if (indexPath == null)
        {
            // fresh clone 无 .docs/：本项不适用（官方计数由方案 §1.1 的离线脚本承担）。
            return;
        }

        var html = File.ReadAllText(indexPath);
        var routes = new HashSet<string>(StringComparer.Ordinal);
        foreach (System.Text.RegularExpressions.Match cell in Regex.Matches(html, "<td[^>]*>\\s*(/[^<\\s][^<]*?)\\s*</td>"))
        {
            var path = cell.Groups[1].Value.Trim();
            if (Regex.IsMatch(path, "^/[A-Za-z0-9_/\\-.]+$"))
            {
                routes.Add(path);
            }
        }

        routes.Should().HaveCount(ExpectedOfficialRoutes,
            "官方面计数口径：索引页表格「请求路径」单元格取值并集（禁用前缀正则）");
    }

    private static List<string> MainInterfaceRoutes()
        => typeof(MpServiceCollectionExtensions).Assembly.GetTypes()
            .Where(t => t.IsInterface && t.IsPublic)
            .SelectMany(t => t.GetMethods())
            .Select(m => m.GetCustomAttribute<HttpMethodAttribute>()?.RequestUri)
            .Where(u => u != null)
            .Select(u => u!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(u => u, StringComparer.Ordinal)
            .ToList();

    private static string? FindOfficialIndex()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null)
        {
            var candidate = Path.Combine(dir.FullName, ".docs", "公众号服务号", "服务号.html");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            if (File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                return null; // 已到仓库根仍无 .docs/ ⇒ fresh clone。
            }

            dir = dir.Parent;
        }

        return null;
    }
}
