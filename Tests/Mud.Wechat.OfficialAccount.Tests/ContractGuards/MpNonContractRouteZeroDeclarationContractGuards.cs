// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 公众号线非契约路由零声明守卫（追踪号 P1-MP-SKIT-CATALOG-VERIFY-04，2026-10-17 公众号缺口核验；用户分组 6 条为 2026-10-24 总案 D6 拍板追加）：
/// 断言 56 条路由在 <c>Src/</c> 全部源码内<b>零声明</b>，防「SKIT 扩展仍携带」或「网关还能打通」重新诱导加回。
/// <para>
/// 判据与企微线 <c>WechatNonContractRouteZeroDeclarationContractGuards</c>（RD1）同源——判据是官方文档面，非网关可达性：
/// </para>
/// <list type="bullet">
/// <item><b>导购（Shopping Guide）48 条</b>：SKIT <c>WechatApiClientExecuteCgibinGuideExtensions.cs</c> 携全套
/// <c>/cgi-bin/guide/*</c>。逐条核验（2026-10-17）：全部 48 个旧清单 URL
/// （<c>doc/offiaccount/Shopping_Guide/...</c>）现 <b>301 重定向到 <c>/doc/service/new.html</c>「公众号文档已升级」</b>，
/// 目标页内零个 <c>/cgi-bin/guide/</c> 路由——官方导购服务端 API 面已随公众号文档整体升级下架，非当前契约。</item>
/// <item><b>实名认证（政务）<c>intp/realname/getauthurl</c> 1 条</b>：清单出处
/// <c>doc/offiaccount/Non_tax_pay/API_document.html#9.2</c> 现重定向到 <c>/doc/service/guide/product/non_tax_pay.html</c>
/// 「微信非税缴费」；逐关键词核验该现行正文内 <c>realname</c> / <c>实名认证</c> /
/// <c>getauthurl</c> / <c>/intp</c> 全部命中=false——政务实名认证小节已从非税页移除。</item>
/// <item><b>图像处理高清化 <c>cv/img/superresolution</c> 1 条</b>：清单出处
/// <c>doc/offiaccount/Intelligent_Interface/Img_Proc.html</c> 现重定向到 <c>.../Open_API.html</c> 介绍页；
/// 现行图像处理章节页 <c>/doc/service/guide/product/Intelligent_Interface/Img_Proc.html</c>（标题「图像处理」）
/// 正文仅列 <c>/cv/img/aicrop</c>、<c>/cv/img/qrcode</c> 两个端点，<c>superresolution</c> / <c>高清</c> 命中=false——
/// 该端点已从图像处理页移除（与 <see cref="MpQueryTokenWhitelistGuard"/> 注释「图像处理 2 端点，superresolution 已下架不建模」一致，
/// 本守卫补上机制化零声明断言）。</item>
/// <item><b>用户分组（legacy）6 条</b>（<c>/cgi-bin/groups/*</c>，总案 D6 拍板，追踪号 MASTER-REMEDIATION-2026-10-24）：
/// 与上述「官方文档面已下架」判据<b>不同类</b>——分组族是**主动裁定不建模**：官方额度页虽仍列「创建/获取/修改分组名/
/// 移动用户分组」，但标签族（<c>/cgi-bin/tags/*</c>，已全覆盖）是其官方替代形态，分组族属 legacy 接口，
/// 维护成本高于业务价值（《Mud.Wechat 功能完善与 API 缺陷补齐方案 v1》§2 评审建议「降级 P3 或移除」，
/// 2026-10-24 拍板移除）。若日后官方复活该族或业务强需，须先撤销本守卫条目并复核官方文档。</item>
/// </list>
/// </summary>
/// <remarks>
/// 本守卫为「清单即契约」型断言：56 条路由的增减都必须先复核官方文档目录树（或如分组族经总案拍板留痕），
/// 不得因网关可达或 SKIT 仍携带而放宽。
/// </remarks>
public class MpNonContractRouteZeroDeclarationContractGuards
{
    /// <summary>
    /// 非契约路由清单（56 条 = 导购 48 + 政务实名认证 1 + 图像高清化 1 + 用户分组 legacy 6）。
    /// </summary>
    private static readonly string[] NonContractRoutes =
    {
        // 导购（Shopping Guide）48 条：/cgi-bin/guide/* 全套；旧清单 URL 现 301 到「公众号文档已升级」，正文零该路由。
        "/cgi-bin/guide/addguide2guidegroup",
        "/cgi-bin/guide/addguideacct",
        "/cgi-bin/guide/addguidebuyerdisplaytag",
        "/cgi-bin/guide/addguidebuyerrelation",
        "/cgi-bin/guide/addguidebuyertag",
        "/cgi-bin/guide/addguidemassendjob",
        "/cgi-bin/guide/addguidetagoption",
        "/cgi-bin/guide/cancelguidemassendjob",
        "/cgi-bin/guide/delguide2guidegroup",
        "/cgi-bin/guide/delguideacct",
        "/cgi-bin/guide/delguidebuyerrelation",
        "/cgi-bin/guide/delguidebuyertag",
        "/cgi-bin/guide/delguidecardmaterial",
        "/cgi-bin/guide/delguidegroup",
        "/cgi-bin/guide/delguideimagematerial",
        "/cgi-bin/guide/delguidetagoption",
        "/cgi-bin/guide/delguidewordmaterial",
        "/cgi-bin/guide/getgroupbyguide",
        "/cgi-bin/guide/getgroupinfo",
        "/cgi-bin/guide/getguideacct",
        "/cgi-bin/guide/getguideacctconfig",
        "/cgi-bin/guide/getguidebuyerchatrecord",
        "/cgi-bin/guide/getguidebuyerdisplaytag",
        "/cgi-bin/guide/getguidebuyerrelation",
        "/cgi-bin/guide/getguidebuyerrelationbybuyer",
        "/cgi-bin/guide/getguidebuyerrelationlist",
        "/cgi-bin/guide/getguidebuyertag",
        "/cgi-bin/guide/getguidecardmaterial",
        "/cgi-bin/guide/getguideconfig",
        "/cgi-bin/guide/getguidegrouplist",
        "/cgi-bin/guide/getguideimagematerial",
        "/cgi-bin/guide/getguidemassendjob",
        "/cgi-bin/guide/getguidemassendjoblist",
        "/cgi-bin/guide/getguidetagoption",
        "/cgi-bin/guide/getguidewordmaterial",
        "/cgi-bin/guide/guidecreateqrcode",
        "/cgi-bin/guide/newguidegroup",
        "/cgi-bin/guide/newguidetagoption",
        "/cgi-bin/guide/queryguidebuyerbytag",
        "/cgi-bin/guide/rebindguideacctforbuyer",
        "/cgi-bin/guide/setguideacctconfig",
        "/cgi-bin/guide/setguidecardmaterial",
        "/cgi-bin/guide/setguideconfig",
        "/cgi-bin/guide/setguideimagematerial",
        "/cgi-bin/guide/setguidewordmaterial",
        "/cgi-bin/guide/updateguideacct",
        "/cgi-bin/guide/updateguidebuyerrelation",
        "/cgi-bin/guide/updateguidemassendjob",
        // 政务实名认证 1 条：intp/realname/getauthurl 已从非税现行页移除。
        "/intp/realname/getauthurl",
        // 图像高清化 1 条：cv/img/superresolution 已从图像处理现行页移除（仅 aicrop / qrcode 保留）。
        "/cv/img/superresolution",
        // 用户分组（legacy）6 条：总案 D6 拍板主动不建模——标签族已全覆盖为其官方替代，分组族维护成本高于业务价值
        //（《功能完善与 API 缺陷补齐方案 v1》§2 评审建议移除，2026-10-24 拍板；判据与上面三类「官方下架」不同类）。
        "/cgi-bin/groups/create",
        "/cgi-bin/groups/get",
        "/cgi-bin/groups/getid",
        "/cgi-bin/groups/update",
        "/cgi-bin/groups/members/update",
        "/cgi-bin/groups/members/batchupdate",
    };

    /// <summary>
    /// 正对照路由（本仓<b>已落位</b>的相邻契约路由）：必须被同一趟扫描捕获，
    /// 否则正则或扫描根漂移会让「零声明」断言静默恒真。
    /// </summary>
    /// <remarks>
    /// 导购整域缺失 ⇒ 无 <c>/cgi-bin/guide/</c> 同前缀活体证明，改以多条已落位 <c>/cgi-bin/</c> 路由覆盖该前缀的扫描有效性；
    /// <c>intp/</c> 与 <c>cv/img/</c> 各取其同前缀已落位路由（一物一码 / 图像裁切·二维码识别）作实扫证据。
    /// </remarks>
    private static readonly string[] LandedControlRoutes =
    {
        // /cgi-bin/ 前缀扫描有效性（菜单 / 用户 / 标签 三个已落位域各一）。
        "/cgi-bin/menu/get",
        "/cgi-bin/user/info",
        "/cgi-bin/tags/get",
        // /intp/ 同前缀活体证明（一物一码域；intp/realname/getauthurl 的同前缀替代锚点）。
        "/intp/marketcode/applycode",
        // /cv/img/ 同前缀活体证明（图像处理域；superresolution 的同前缀现行端点）。
        "/cv/img/aicrop",
        "/cv/img/qrcode",
    };

    /// <summary>契约守卫 MP-NC1：56 条非契约路由清单锁定 + <c>Src/</c> 全量扫描零命中 + 已落位路由正对照。</summary>
    [Fact]
    public void NonContractRoutes_ShouldHaveZeroDeclaredRoutes_WhenScannedAcrossSrc()
    {
        NonContractRoutes.Should().HaveCount(56,
            "导购 48 + 政务实名认证 1 + 图像高清化 1 + 用户分组 legacy 6：清单本身即契约，增删须先复核官方文档目录树（或经总案拍板留痕，追踪号 P1-MP-SKIT-CATALOG-VERIFY-04 / MASTER-REMEDIATION-2026-10-24）");
        NonContractRoutes.Distinct(StringComparer.Ordinal).Should().HaveCount(56, "清单内路由不得重复");

        var routeRegex = new Regex(@"\[(?:Get|Post|Put|Delete|Patch)\(""([^""]+)""\)\]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var forbidden = new HashSet<string>(NonContractRoutes, StringComparer.Ordinal);

        var declared = new HashSet<string>(StringComparer.Ordinal);
        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(Path.Combine(GetSolutionRoot(), "Src"), "*.cs",
                     SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal)
                || normalized.Contains("/Generated/", StringComparison.Ordinal))
            {
                continue;
            }

            foreach (System.Text.RegularExpressions.Match match in routeRegex.Matches(File.ReadAllText(file)))
            {
                var route = match.Groups[1].Value;
                declared.Add(route);
                if (forbidden.Contains(route))
                {
                    offenders.Add($"{normalized}: {route}");
                }
            }
        }

        // 扫描非空跑正对照：已落位的相邻契约路由必须被同一趟扫描捕获。
        LandedControlRoutes.Should().OnlyContain(
            route => declared.Contains(route),
            "MP-NC1 的扫描必须能捕获本仓已声明的相邻路由（否则正则/扫描根漂移会让零声明断言静默恒真）");

        offenders.Should().BeEmpty(
            "56 条路由不构成公众号应用侧 HTTP 契约（官方文档目录已移除：导购整域 301 下架 + 政务实名认证/图像高清化小节删除；" +
            "用户分组族经总案 D6 拍板 legacy 不建模，标签族为官方替代），" +
            "本仓不得在任何产品线接口上声明（SKIT 仍携带只是历史镜像，追踪号 P1-MP-SKIT-CATALOG-VERIFY-04 / MASTER-REMEDIATION-2026-10-24）");
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
