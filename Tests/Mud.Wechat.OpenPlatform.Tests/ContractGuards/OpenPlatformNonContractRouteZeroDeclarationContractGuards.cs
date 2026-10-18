// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线非契约路由零声明守卫（追踪号 P1-OP-LEGACY20-BETAVERIFY-01，2026-10-17 开放平台缺口逐页核验）：
/// 断言试用小程序转正族的 2 条 <b>2.0 旧版路由</b>在 <c>Src/</c> 全部源码内<b>零声明</b>，
/// 防「旧版文档 URL 还能打开」或「第三方 SDK 仍携带」重新诱导加回。
/// <para>
/// 判据与公众号线 <c>MpNonContractRouteZeroDeclarationContractGuards</c>（MP-NC1）、企微线
/// <c>WechatNonContractRouteZeroDeclarationContractGuards</c>（RD1）同源——判据是<b>官方文档目录树</b>，非网关可达性：
/// </para>
/// <list type="bullet">
/// <item><b><c>/wxa/getmpadminauth</c></b>、<b><c>/wxa/mpverifybetaweapp</c></b>：清单出处均为旧版
/// <c>doc/oplatform/Third-party_Platforms/2.0/api/verify_beta_Mini_Programs/*.html</c>。逐页核验（2026-10-17）：
/// 两页 HTTP 状态仍为 <c>200</c>（<b>无 301</b>，故不能按下架重定向判据处置），但正文渲染仅剩
/// 「优化后重新上线」——内容已摘除待重构；且二者均<b>不在</b>现行
/// <c>doc/oplatform/openApi/OpenApiDoc/</c> 目录树内 ⇒ 按「清单即契约」判据不成立，属非契约路由。</item>
/// <item><b>同族现行面已落位</b>：试用小程序族在当前目录树内的端点是
/// <c>getTrialQRCode</c>（<c>/wxa/get_qrcode</c>，由 QRC 系列非声明式图片流通道承载）与
/// <c>verifyBetaWeapp</c> / <c>setBetaWeappNickname</c>（<c>/wxa/verifybetaweapp</c>、<c>/wxa/setbetaweappnickname</c>，
/// 30-4 已落）——旧版 2.0 的两支没有对应现行页，故本守卫的清单恰为这两支。</item>
/// </list>
/// </summary>
/// <remarks>
/// <para><b>匹配口径必须是整串相等</b>：<c>/wxa/mpverifybetaweapp</c> 与已落位的 <c>/wxa/verifybetaweapp</c>
/// 互为后缀包含关系，任何前缀/子串式判定会把已落位契约路由误判成违规（假红），或反向把本清单路由放行（假绿）。
/// 故断言走 <see cref="HashSet{T}"/>（<see cref="StringComparer.Ordinal"/>）精确命中，正对照路由亦以同前缀整串形式存在性证明扫描有效。</para>
/// <para>路由清单的增减都必须先复核官方文档目录树（下架重定向与「优化后重新上线」两种下架形态都要取证），不得因网关可达或第三方 SDK 仍携带而放宽。</para>
/// </remarks>
public class OpenPlatformNonContractRouteZeroDeclarationContractGuards
{
    /// <summary>
    /// 非契约路由清单（2 条 = 试用小程序转正族的 2.0 旧版路由，判据为官方文档目录树已摘除内容）。
    /// </summary>
    private static readonly string[] NonContractRoutes =
    {
        // 旧版 2.0「查询小程序管理员认证信息」：页内正文仅剩「优化后重新上线」，现行 openApi/OpenApiDoc 目录树无对应页。
        "/wxa/getmpadminauth",
        // 旧版 2.0「试用小程序转正」：同上下架形态；现行页为 /wxa/verifybetaweapp（30-4 已落），二者是不同路由、不同契约。
        "/wxa/mpverifybetaweapp",
    };

    /// <summary>
    /// 正对照路由（本仓<b>已落位</b>的相邻契约路由）：必须被同一趟扫描捕获，
    /// 否则正则或扫描根漂移会让「零声明」断言静默恒真。
    /// </summary>
    /// <remarks>
    /// <c>/wxa/verifybetaweapp</c>、<c>/wxa/setbetaweappnickname</c> 取同族已落位两支（同时钉住整串相等口径：
    /// 二者与被禁路由互为后缀关系）；<c>/wxa/memberauth</c> 钉住 <c>getmpadminauth</c> 的 <c>...auth</c> 尾缀同形面；
    /// <c>/wxa/submit_audit</c> 作 <c>/wxa/</c> 前缀扫描根的一般活体证明。
    /// </remarks>
    private static readonly string[] LandedControlRoutes =
    {
        "/wxa/verifybetaweapp",
        "/wxa/setbetaweappnickname",
        "/wxa/memberauth",
        "/wxa/submit_audit",
    };

    /// <summary>
    /// 契约守卫 OP-NC1：2 条非契约路由清单锁定 + <c>Src/</c> 全量声明式路由扫描零命中 + 已落位路由正对照。
    /// </summary>
    [Fact]
    public void NonContractRoutes_ShouldHaveZeroDeclaredRoutes_WhenScannedAcrossSrc()
    {
        NonContractRoutes.Should().HaveCount(2,
            "试用小程序转正族仅 2.0 旧版两支判非契约；增删须先复核官方文档目录树（追踪号 P1-OP-LEGACY20-BETAVERIFY-01）");
        NonContractRoutes.Distinct(StringComparer.Ordinal).Should().HaveCount(2, "清单内路由不得重复");

        var routeRegex = new Regex(@"\[(?:Get|Post|Put|Delete|Patch)\(""([^""]+)""\)\]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var forbidden = new HashSet<string>(NonContractRoutes, StringComparer.Ordinal);

        var declared = new HashSet<string>(StringComparer.Ordinal);
        var offenders = new List<string>();
        foreach (var file in EnumerateSrcSources())
        {
            foreach (System.Text.RegularExpressions.Match match in routeRegex.Matches(File.ReadAllText(file)))
            {
                var route = match.Groups[1].Value;
                declared.Add(route);
                if (forbidden.Contains(route))
                {
                    offenders.Add($"{file.Replace('\\', '/')}: {route}");
                }
            }
        }

        // 扫描非空跑正对照：已落位的相邻契约路由必须被同一趟扫描捕获。
        LandedControlRoutes.Should().OnlyContain(
            route => declared.Contains(route),
            "OP-NC1 的扫描必须能捕获本仓已声明的 /wxa/ 路由（否则正则/扫描根漂移会让零声明断言静默恒真）");

        offenders.Should().BeEmpty(
            "两条 2.0 旧版路由不构成开放平台应用侧 HTTP 契约（官方页正文仅剩「优化后重新上线」且不在现行 openApi/OpenApiDoc 目录树内，" +
            "同族现行面已由 /wxa/verifybetaweapp + /wxa/get_qrcode 承载），本仓不得在任何产品线接口上声明（追踪号 P1-OP-LEGACY20-BETAVERIFY-01）");
    }

    /// <summary>
    /// 契约守卫 OP-NC2：除声明式路由属性外，两条路由的字面量在 <c>Src/</c> 任何字符串常量中出现次数恒为零——
    /// 堵住「改走手写 <c>SendRawAsync</c> / 拼串路径」绕过声明式扫描的路径。
    /// </summary>
    /// <remarks>
    /// 只扫字符串字面量，<b>不</b>扫注释：现行接口 XML 文档注释中记录「本端点已下架、不建模」的说明是允许且必要的，
    /// 若连注释一起禁会把取证结论本身挤出版本库。
    /// </remarks>
    [Fact]
    public void NonContractRoutes_ShouldNotAppearInAnyStringLiteral_WhenScannedAcrossSrc()
    {
        var literalRegex = new Regex("\"(?:[^\"\\\\\\n]|\\\\.)*\"", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        var hits = new List<string>();
        var capturedLiterals = new HashSet<string>(StringComparer.Ordinal);
        foreach (var file in EnumerateSrcSources())
        {
            foreach (System.Text.RegularExpressions.Match match in literalRegex.Matches(File.ReadAllText(file)))
            {
                var literal = match.Value;
                capturedLiterals.Add(TrimQuotes(literal));
                foreach (var route in NonContractRoutes)
                {
                    // 整串（去引号）相等或作为路径段包含均视为声明；旧版路由无查询串形态，无需拆解 ? 后内容。
                    if (literal.Contains(route, StringComparison.Ordinal))
                    {
                        hits.Add($"{file.Replace('\\', '/')}: {literal}");
                    }
                }
            }
        }

        // 字面量扫描非空跑正对照：已落位路由的字符串字面量必须被同一趟扫描捕获，
        // 否则字面量正则失效（如漏匹配带转义的引号串）会让本断言静默恒真。
        capturedLiterals.Should().NotBeEmpty("字面量正则必须能捕获 Src/ 内的字符串常量");
        foreach (var control in LandedControlRoutes)
        {
            capturedLiterals.Should().Contain(control,
                "OP-NC2 的字面量正则必须能捕获本仓已落位路由的字符串常量（否则零命中不代表零声明）");
        }

        hits.Should().BeEmpty(
            "非契约路由不得以声明式属性、也不得以任何字符串常量（手写通道/拼串路径）形式出现在 Src/ 源码内");
    }

    /// <summary>取字符串字面量匹配值的内部内容（去首尾双引号）。</summary>
    private static string TrimQuotes(string literal)
        => literal.Length >= 2 ? literal.Substring(1, literal.Length - 2) : literal;

    /// <summary><c>Src/</c> 下参与路由扫描的 <c>*.cs</c> 源文件（跳过 <c>obj</c>/<c>bin</c> 与生成物目录）。</summary>
    private static IEnumerable<string> EnumerateSrcSources()
    {
        return Directory.EnumerateFiles(Path.Combine(GetSolutionRoot(), "Src"), "*.cs", SearchOption.AllDirectories)
            .Where(static p =>
            {
                var normalized = p.Replace('\\', '/');
                return !normalized.Contains("/obj/", StringComparison.Ordinal)
                    && !normalized.Contains("/bin/", StringComparison.Ordinal)
                    && !normalized.Contains("/Generated/", StringComparison.Ordinal);
            });
    }

    /// <summary>解决方案根目录（向上查找 Mud.Wechat.slnx）。</summary>
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
