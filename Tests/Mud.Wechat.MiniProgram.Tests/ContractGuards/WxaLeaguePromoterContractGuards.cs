// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Match = System.Text.RegularExpressions.Match;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「交易组件推广者 / 联盟（LeaguePromoter，工作单 <c>/union/promoter/*</c> 22 支）」域契约守卫
/// —— <b>全域判非契约，零端点 / 零接口 / 零 DTO / 零注册组</b>（追踪号 <c>P2-MP-LeaguePromoter-{端点}-01</c>，
/// 2026-10-18 逐页取证终判）。
/// </summary>
/// <remarks>
/// <para>
/// <b>取证结论（先取证后编码，逐页实测）</b>：工作单 22 支端点的出处共 9 个官方页面（两支 URL 族：
/// <c>/miniprogram/dev/platform-capabilities/business-capabilities/union/**</c> 与 <c>/doc/ministore/union/**</c>），
/// <b>全部 301 永久重定向</b>至同一篇公告文章
/// <c>https://mp.weixin.qq.com/s/LA9VaipVBYAWkPddQqTrAQ</c>（标题《微信小商店升级为微信小店的公告》意见征集），
/// 重定向目标是<b>公告页而非任何接口文档页</b>（正文不含请求方式 / 请求地址小节）。
/// 整棵文档树一并下架：<c>…/union/index.html</c>、<c>…/union/access-guidelines/promoter/index.html</c>、
/// <c>…/union/custom_component/index.html</c>、<c>/doc/ministore/</c> 四个索引入口实测同落同一 301；
/// 而现行微信小店文档树 <c>https://developers.weixin.qq.com/doc/store/shop/</c>（实测 200）正文
/// <b>零处提及 "union"</b> ⇒ 官方目录树内已无 <c>/union/*</c> 的任何在册页面。
/// </para>
/// <para>
/// <b>判定口径（作业书 §1，不放宽）</b>：契约 = 页 200 + 正文含「请求方式 / 请求地址」+ 仍在现行目录树内。
/// 本域 22 支三条全不满足（出处页 301 落公告 / 无请求小节 / 脱树）⇒ 一律判非契约，
/// 接口与 DTO 一律不声明。与 <c>MiniProgramNonContractRouteContractGuards</c> NC1 对「小程序推广员 14 条」
/// （同为小商店时代 <c>/promoter/*</c>，正文「此文档已下架」）的终判同族同源：SKIT 转述清单 ≠ 契约
/// （网关可达性亦不构成契约）。
/// </para>
/// <para>
/// <b>继任面归小店线（跨线不交叉的裁决依据）</b>：微信小商店升级为微信小店后，联盟推广者能力的现行官方面
/// 落 <c>/channels/ec/league/*</c> 与 <c>/channels/ec/promoter/*</c>，由<b>小店线</b>
/// <c>IChannelsLeagueService</c> / <c>IChannelsPromoterService</c> 承载，令牌为
/// <c>ChannelsTokenTypes.AccessToken</c>（小店令牌面，与小程序 <c>MpTokenTypes.AccessToken</c> 在共享注册表天然隔离）。
/// 本域令牌面结论：<b>不存在</b> —— 22 支无一支持落地，既非「小程序 access_token」也非「联盟独立凭据」，
/// 不产生任何 <c>[Token]</c> 声明与 QT1 白名单追加。
/// </para>
/// <para>
/// <b>守卫分工</b>：端点聚合计数（本域增量 0）、跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）由
/// <c>MiniProgramContractGuards</c> 与 <c>ContractBaseline</c> 承担；本文件锁定「全域判非契约」的
/// 零声明不变式与跨线负锁，防止后来者照 <c>.docs/_gapdump/mp-LeaguePromoter.txt</c>（SKIT 派生清单）
/// 把已下架能力「补」回小程序线。
/// </para>
/// </remarks>
public class WxaLeaguePromoterContractGuards
{
    /// <summary>本域判定不落地的 DTO 命名空间（LP2 断言其类型集恒空，防「先建 DTO 后补接口」的半成品回潮）。</summary>
    private const string ProhibitedDtoNamespace = "Mud.Wechat.MiniProgram.DataModels.LeaguePromoter";

    /// <summary>本域判定的 JsonContext 登记名（LP2 断言全 DataModels 程序集零命中，防孤儿上下文登记）。</summary>
    private const string ProhibitedSerializerClassName = "LeaguePromoter";

    /// <summary>
    /// 工作单 22 支端点全集（路由与动词照 <c>.docs/_gapdump/mp-LeaguePromoter.txt</c> 逐行登记；
    /// 每条挂独立追踪号 <c>P2-MP-LeaguePromoter-{端点}-01</c>，端点键 = 路由去 <c>/union/</c> 前缀后以 <c>.</c> 连接）。
    /// 出处共 9 个官方页面，2026-10-18 实测一律 301 → mp.weixin.qq.com/s/LA9VaipVBYAWkPddQqTrAQ（升级公告，非接口文档）。
    /// </summary>
    private static readonly (string Verb, string Route, string TrackingId)[] NonContractRoutes =
    {
        // customized-userinfo.html（open_account 5 支）
        ("POST", "/union/promoter/open_account/add_customize_info", "P2-MP-LeaguePromoter-promoter.open_account.add_customize_info-01"),
        ("POST", "/union/promoter/open_account/bind", "P2-MP-LeaguePromoter-promoter.open_account.bind-01"),
        ("POST", "/union/promoter/open_account/del_customize_info", "P2-MP-LeaguePromoter-promoter.open_account.del_customize_info-01"),
        ("GET", "/union/promoter/open_account/get", "P2-MP-LeaguePromoter-promoter.open_account.get-01"),
        ("GET", "/union/promoter/open_account/get_customize_info", "P2-MP-LeaguePromoter-promoter.open_account.get_customize_info-01"),
        // openpromotion.html / openproduct.html（open_product 2 支）
        ("POST", "/union/promoter/open_product/generate", "P2-MP-LeaguePromoter-promoter.open_product.generate-01"),
        ("GET", "/union/promoter/open_product/list", "P2-MP-LeaguePromoter-promoter.open_product.list-01"),
        // order-info.html（ministore 树 2 支 + custom_component 树 1 支）
        ("POST", "/union/promoter/order/info", "P2-MP-LeaguePromoter-promoter.order.info-01"),
        ("GET", "/union/promoter/order/search", "P2-MP-LeaguePromoter-promoter.order.search-01"),
        ("GET", "/union/promoter/order/search_normal_order", "P2-MP-LeaguePromoter-promoter.order.search_normal_order-01"),
        // product category.html（4 支）+ parse_tag.html（1 支）
        ("GET", "/union/promoter/product/category", "P2-MP-LeaguePromoter-promoter.product.category-01"),
        ("POST", "/union/promoter/product/generate", "P2-MP-LeaguePromoter-promoter.product.generate-01"),
        ("GET", "/union/promoter/product/list", "P2-MP-LeaguePromoter-promoter.product.list-01"),
        ("GET", "/union/promoter/product/select", "P2-MP-LeaguePromoter-promoter.product.select-01"),
        ("POST", "/union/promoter/product/parse_tag", "P2-MP-LeaguePromoter-promoter.product.parse_tag-01"),
        // promotion.html（推广位 4 支）
        ("POST", "/union/promoter/promotion/add", "P2-MP-LeaguePromoter-promoter.promotion.add-01"),
        ("POST", "/union/promoter/promotion/del", "P2-MP-LeaguePromoter-promoter.promotion.del-01"),
        ("GET", "/union/promoter/promotion/list", "P2-MP-LeaguePromoter-promoter.promotion.list-01"),
        ("POST", "/union/promoter/promotion/upd", "P2-MP-LeaguePromoter-promoter.promotion.upd-01"),
        // target_plan.html（定向计划 3 支）
        ("GET", "/union/promoter/target/apply_status", "P2-MP-LeaguePromoter-promoter.target.apply_status-01"),
        ("POST", "/union/promoter/target/apply_target", "P2-MP-LeaguePromoter-promoter.target.apply_target-01"),
        ("GET", "/union/promoter/target/plan_info", "P2-MP-LeaguePromoter-promoter.target.plan_info-01"),
    };

    // ------------------------------------------------------------------
    // LP1：22 条路由全仓 Src/ 零声明（扫描根与正则由已落位路由作正对照）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LP1：非契约路由清单锁定 + <c>Src/</c> 全量零声明 + 已落位路由正对照。</summary>
    [Fact]
    public void LeaguePromoterRoutes_ShouldStayUndeclared_WhenScanningWholeSrc()
    {
        NonContractRoutes.Should().HaveCount(22,
            "工作单 mp-LeaguePromoter.txt 恰 22 行（open_account 5 + open_product 2 + order 3 + product 5 + promotion 4 + target 3）；"
            + "清单本身不是契约，增删须先复核出处页存活状态");
        NonContractRoutes.Select(static r => r.Route).Distinct(StringComparer.Ordinal)
            .Should().HaveCount(22, "路由清单不得出现重复条目");

        // 清单形状自证：本域全部候选一律 /union/promoter/ 前缀（LP4 的跨线负锁以此清单为全集基础）。
        NonContractRoutes.Should().OnlyContain(static r => r.Route.StartsWith("/union/promoter/", StringComparison.Ordinal),
            "/union/* 联盟面在本工作单内全部落 /union/promoter/ 子族；出现其它前缀即清单漂移");

        var declared = DeclaredRoutes("Src");
        var offenders = declared.Intersect(NonContractRoutes.Select(static r => r.Route), StringComparer.Ordinal).ToList();

        // 正对照：同一趟扫描必须捕获本仓已落位的相邻路由 —— 小程序线自身一支 + 小店线继任面一支，
        // 否则扫描根或正则漂移会让「零声明」静默恒真（同 NC1 / RD1 口径）。
        declared.Should().Contain("/shop/sharer/bind",
            "LP1 的扫描必须能捕获小程序线已落位的推客绑定路由（否则零声明断言静默恒真）");
        declared.Should().Contain("/channels/ec/league/promoter/get",
            "LP1 的扫描必须能捕获小店线继任面路由（继任能力在册，本域零声明结论才不是扫描失效的假象）");

        offenders.Should().BeEmpty(
            "22 条 /union/promoter/* 路由不构成应用侧 HTTP 契约：出处 9 页实测一律 301 落《微信小商店升级为微信小店的公告》"
            + "意见征集页（非接口文档、无请求小节）、整棵 union/ministore 树脱目录，现行小店树零 union 提及；"
            + "继任面归小店线 /channels/ec/league|promoter/*（ChannelsTokenTypes.AccessToken 令牌面）。"
            + "本仓任何产品线都不得声明（逐条追踪号见清单 P2-MP-LeaguePromoter-【端点】-01，2026-10-18 终判）");
    }

    // ------------------------------------------------------------------
    // LP2：本域对外表面恒空（零接口 / 零 DTO 命名空间 / 零 JsonContext 登记名）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LP2：主包与 DataModels 中本域类型零声明（防「已下架能力」以接口或 DTO 半成品形态回仓）。</summary>
    [Fact]
    public void LeaguePromoterDomain_ShouldHaveNoPublicTypes()
    {
        var mainAssembly = typeof(IWxaShopSharerService).Assembly;
        var dtoAssembly = typeof(Mud.Wechat.MiniProgram.DataModels.ShopSharer.WxaShopSharerInfo).Assembly;

        // 主包：不得出现任何名字含 LeaguePromoter / UnionPromoter 的公开类型（域别名一并锁，
        // .docs v2 方案曾建议落位名 UnionPromoter，两形皆禁以防「换个名字再补进来」）。
        var mainOffenders = mainAssembly.GetTypes()
            .Where(static t => !t.IsNested && t.IsPublic
                && (t.Name.Contains("LeaguePromoter", StringComparison.OrdinalIgnoreCase)
                    || t.Name.Contains("UnionPromoter", StringComparison.OrdinalIgnoreCase)))
            .Select(static t => t.Name)
            .ToList();
        mainOffenders.Should().BeEmpty(
            "本域判非契约、接口一律不声明（LP1 取证）；出现同名接口即按已下架 SKIT 清单回补的半成品（追踪号 P2-MP-LeaguePromoter-01 族）");

        // DataModels：禁建本域 DTO 命名空间（类型集恒空即命名空间未启用）。
        var dtoOffenders = dtoAssembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == ProhibitedDtoNamespace)
            .Select(static t => t.Name)
            .ToList();
        dtoOffenders.Should().BeEmpty(
            $"{ProhibitedDtoNamespace} 不得出现任何类型：无端点即无请求/应答 DTO，禁「先建 DTO 后补接口」");

        // JsonContext 登记名：全 DataModels 程序集不得有 SerializerClassName == "LeaguePromoter" 的标注
        //（主控收口跑 GenerateJsonContext.ps1 时也不会为本域生成上下文）。
        var serializerOffenders = dtoAssembly.GetTypes()
            .Where(static t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() is { } attr
                && attr.SerializerClassName == ProhibitedSerializerClassName)
            .Select(static t => t.Name)
            .ToList();
        serializerOffenders.Should().BeEmpty(
            $"不得存在 [HttpJsonSerializable(SerializerClassName = \"{ProhibitedSerializerClassName}\")] 登记（孤儿上下文名会进生成器产物）");
    }

    // ------------------------------------------------------------------
    // LP3：DI 注册面恒空（模块枚举无本域成员、建造者无本域方法）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LP3：零端点 ⇒ 零注册组；防「枚举/建造者先注册、接口永远缺席」的空域假绿。</summary>
    [Fact]
    public void LeaguePromoterShouldNotAppearInModuleEnumOrBuilder()
    {
        var moduleNames = Enum.GetNames<MiniProgramModule>();
        moduleNames.Should().NotContain("LeaguePromoter")
            .And.NotContain("UnionPromoter",
            "本域判非契约 ⇒ MiniProgramModule 不得新增 LeaguePromoter / UnionPromoter 成员（注册三段式的第一段）；"
            + "仅锁这两个本域名号，不得顺手扩成 Union 子串（并行的联盟·店铺域 UnionShop 是另一工作单）");

        var builderMethods = typeof(MiniProgramServiceBuilder)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static m => m.Name)
            .ToList();
        builderMethods.Should().NotContain("AddLeaguePromoterApi")
            .And.NotContain("AddUnionPromoterApi",
            "建造者不得出现 AddLeaguePromoterApi / AddUnionPromoterApi（零接口的注册方法会让各域 Add…Api() 空转成功）");

        // 正对照：枚举与建造者的扫描口径必须能命中已落位域（NotContain 断言才不是空转）。
        moduleNames.Should().Contain("ShopSharer", "LP3 的正对照：枚举成员扫描口径有效");
        builderMethods.Should().Contain(static n => n == "AddAuthApi", "LP3 的正对照：建造者方法扫描口径有效");
    }

    // ------------------------------------------------------------------
    // LP4：跨线不交叉负锁（本线声明面不得出现 /channels/；本域 22 条逐条零命中）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LP4：小程序线声明面不含 <c>/channels/</c> 前缀（小店线专属），且本域 22 条 <c>/union/promoter/*</c> 逐条零命中。</summary>
    [Fact]
    public void MiniProgramLine_ShouldNotDeclareChannelsOrLeaguePromoterRoutes()
    {
        var declared = DeclaredRoutes(Path.Combine("Src", "MiniProgram"));

        // 正对照：扫描必须能捕获小程序线在册路由，否则空集合会让负锁静默恒真。
        declared.Should().Contain("/shop/sharer/bind",
            "LP4 的扫描必须能捕获小程序线已落位路由（否则零命中断言静默恒真）");

        declared.Where(static r => r.StartsWith("/channels/", StringComparison.Ordinal))
            .Should().BeEmpty(
            "含 /channels/ 的路由专属小店线（Mud.Wechat.Channels，ChannelsTokenTypes.AccessToken 令牌面）："
            + "本域继任面 /channels/ec/league/* 已在小店线落位，小程序线声明任何该前缀路由即跨线抢面 + 令牌串号");

        // 本域 22 条逐条零命中（LP1 是全仓口径，这里补本线口径并解释为何不做 /union/ 整前缀封锁：
        // /union/ 前缀的其余条目（联盟·店铺 /union/bd/* 等）属并行工作单 UnionShop 的取证范围，
        // 是否在册由那一路的守卫裁决，本守卫不得越界封锁兄弟域的令牌面）。
        var unionOffenders = declared.Intersect(NonContractRoutes.Select(static r => r.Route), StringComparer.Ordinal).ToList();
        unionOffenders.Should().BeEmpty(
            "22 条 /union/promoter/* 已随微信小商店文档树下架（LP1 取证：出处页一律 301 落升级公告、现行小店树零 union 提及）："
            + "小程序线不得声明之（追踪号 P2-MP-LeaguePromoter-【端点】-01 族，2026-10-18 终判）");

        // 继任面在册自证（全仓扫描根）：/channels/ec/league/ 族确实存在于 Src/，
        // 证明上面第一条负锁是「能力已归小店线」的分工裁决，而非小店线也没落的悬空结论。
        var wholeSrc = DeclaredRoutes("Src");
        wholeSrc.Should().Contain(static r => r.StartsWith("/channels/ec/league/", StringComparison.Ordinal),
            "继任面 /channels/ec/league/* 须在小店线在册（跨线分工的事实基础）");
    }

    // ------------------------------------------------------------------
    // 内部辅助（扫描口径与 MiniProgramNonContractRouteContractGuards 同源）
    // ------------------------------------------------------------------

    /// <summary>
    /// 扫描根下全部「已声明路由」：剥离注释后取引号包裹的路径字面量（HTTP 特性与路由常量一并覆盖）。
    /// 剥离注释是必要的——本守卫与相邻裁决里的下架路由名会以注释形态出现在 Src，不得计入声明面。
    /// </summary>
    private static HashSet<string> DeclaredRoutes(string relativeRoot)
    {
        var root = Path.Combine(GetSolutionRoot(), relativeRoot);
        Directory.Exists(root).Should().BeTrue($"扫描根必须存在（{root}）——目录漂移会让零声明断言静默恒真");

        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./{}]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var set = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal))
            {
                continue;
            }

            var code = string.Join("\n", Regex.Replace(File.ReadAllText(file), @"/\*[\s\S]*?\*/", " ")
                .Split('\n')
                .Where(static l => !l.TrimStart().StartsWith("//", StringComparison.Ordinal)));

            foreach (Match m in literalRegex.Matches(code))
            {
                set.Add(m.Groups[1].Value);
            }
        }

        set.Should().NotBeEmpty("防「扫描口径失效导致真空断言」：扫描根必须能扫出已声明路由");
        return set;
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
