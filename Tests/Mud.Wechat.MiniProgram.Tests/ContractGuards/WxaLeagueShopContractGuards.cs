// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Match = System.Text.RegularExpressions.Match;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「联盟·店铺（LeagueShop，工作单 <c>/union/shop/*</c> 10 支）」域契约守卫
/// —— <b>全域判非契约，零端点 / 零接口 / 零 DTO / 零注册组</b>（追踪号 <c>P2-MP-LeagueShop-{端点}-01</c>，
/// 2026-10-18 逐页取证终判）。
/// </summary>
/// <remarks>
/// <para>
/// <b>取证结论（与 LeaguePromoter 同一棵树、同一结论）</b>：工作单 10 支的出处共 3 个官方页面
/// （<c>…/union/custom_component/api/shop/set_commission.html</c>、
/// <c>…/union/custom_component/api/shop/order-info.html</c>、
/// <c>…/union/custom_component/api/shop/target_plan/target_plan.html</c>），
/// 与 <c>WxaLeaguePromoterContractGuards</c> 已逐页实测的 <c>union/custom_component</c> 文档树同源：
/// <b>全部 301 永久重定向</b>至《微信小商店升级为微信小店的公告》意见征集文章（<c>mp.weixin.qq.com</c>），
/// 重定向目标是公告页而非接口文档页（无「请求方式 / 请求地址」小节）；现行微信小店文档树
/// <c>https://developers.weixin.qq.com/doc/store/shop/</c> 零处提及 <c>union</c>。
/// </para>
/// <para>
/// <b>判定口径（作业书 §1，不放宽）</b>：契约 = 页 200 + 正文含「请求方式 / 请求地址」+ 仍在现行目录树内。
/// 本域 10 支三条全不满足 ⇒ 一律判非契约，接口与 DTO 一律不声明。SKIT 派生清单 ≠ 契约
/// （同族先例：LeaguePromoter 22 支、小程序推广员 14 条均为小商店时代下架面）。
/// </para>
/// <para>
/// <b>继任面归小店线</b>：联盟商家侧能力现行官方面落 <c>/channels/ec/league/*</c>（含联盟计划与佣金管理），
/// 由小店线 <c>IChannelsLeagueService</c> 承载、令牌为 <c>ChannelsTokenTypes.AccessToken</c>（小店令牌面）。
/// 本域令牌面结论：<b>不存在</b> —— 10 支无一支持落地，不产生任何 <c>[Token]</c> 声明与 QT1 白名单追加。
/// </para>
/// <para>
/// <b>守卫分工</b>：端点聚合计数（本域增量 0）、跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）由
/// <c>MiniProgramContractGuards</c> 与 <c>ContractBaseline</c> 承担；本文件锁定「全域判非契约」的
/// 零声明不变式与跨线负锁，防止后来者照 <c>.docs/_gapdump/mp-LeagueShop.txt</c>（SKIT 派生清单）
/// 把已下架能力「补」回小程序线。
/// </para>
/// </remarks>
public class WxaLeagueShopContractGuards
{
    /// <summary>本域判定不落地的 DTO 命名空间（LS2 断言其类型集恒空，防「先建 DTO 后补接口」的半成品回潮）。</summary>
    private const string ProhibitedDtoNamespace = "Mud.Wechat.MiniProgram.DataModels.LeagueShop";

    /// <summary>本域判定的 JsonContext 登记名（LS2 断言全 DataModels 程序集零命中，防孤儿上下文登记）。</summary>
    private const string ProhibitedSerializerClassName = "LeagueShop";

    /// <summary>
    /// 工作单 10 支端点全集（路由与动词照 <c>.docs/_gapdump/mp-LeagueShop.txt</c> 逐行登记；
    /// 每条挂独立追踪号 <c>P2-MP-LeagueShop-{端点}-01</c>，端点键 = 路由去 <c>/union/shop/</c> 前缀后以 <c>.</c> 连接）。
    /// 出处共 3 个官方页面，与 LeaguePromoter 同属 <c>union/custom_component</c> 下架树（2026-10-18 实测 301 → 升级公告）。
    /// </summary>
    private static readonly (string Verb, string Route, string TrackingId)[] NonContractRoutes =
    {
        // set_commission.html（开放平台佣金 3 支）
        ("POST", "/union/shop/open/delete_commission", "P2-MP-LeagueShop-open.delete_commission-01"),
        ("POST", "/union/shop/open/get_commission", "P2-MP-LeagueShop-open.get_commission-01"),
        ("POST", "/union/shop/open/set_commission", "P2-MP-LeagueShop-open.set_commission-01"),
        // order-info.html（普通订单查询 1 支）
        ("GET", "/union/shop/order/search_normal_order", "P2-MP-LeagueShop-order.search_normal_order-01"),
        // target_plan.html（定向计划 6 支）
        ("POST", "/union/shop/target/add_plan", "P2-MP-LeagueShop-target.add_plan-01"),
        ("GET", "/union/shop/target/plan_list", "P2-MP-LeagueShop-target.plan_list-01"),
        ("GET", "/union/shop/target/product_list", "P2-MP-LeagueShop-target.product_list-01"),
        ("GET", "/union/shop/target/promoter_list", "P2-MP-LeagueShop-target.promoter_list-01"),
        ("POST", "/union/shop/target/update_plan", "P2-MP-LeagueShop-target.update_plan-01"),
        ("POST", "/union/shop/target/update_promoter", "P2-MP-LeagueShop-target.update_promoter-01"),
    };

    // ------------------------------------------------------------------
    // LS1：10 条路由全仓 Src/ 零声明（扫描根与正则由已落位路由作正对照）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LS1：非契约路由清单锁定 + <c>Src/</c> 全量零声明 + 已落位路由正对照。</summary>
    [Fact]
    public void LeagueShopRoutes_ShouldStayUndeclared_WhenScanningWholeSrc()
    {
        NonContractRoutes.Should().HaveCount(10,
            "工作单 mp-LeagueShop.txt 恰 10 行（open 3 + order 1 + target 6）；清单本身不是契约，增删须先复核出处页存活状态");
        NonContractRoutes.Select(static r => r.Route).Distinct(StringComparer.Ordinal)
            .Should().HaveCount(10, "路由清单不得出现重复条目");

        // 清单形状自证：本域全部候选一律 /union/shop/ 前缀。
        NonContractRoutes.Should().OnlyContain(static r => r.Route.StartsWith("/union/shop/", StringComparison.Ordinal),
            "/union/* 联盟面在本工作单内全部落 /union/shop/ 子族；出现其它前缀即清单漂移");

        var declared = DeclaredRoutes("Src");
        var offenders = declared.Intersect(NonContractRoutes.Select(static r => r.Route), StringComparer.Ordinal).ToList();

        // 正对照：同一趟扫描必须捕获本仓已落位的相邻路由 —— 小程序线自身一支 + 小店线继任面一支，
        // 否则扫描根或正则漂移会让「零声明」静默恒真（同 LP1 / NC1 / RD1 口径）。
        declared.Should().Contain("/shop/sharer/bind",
            "LS1 的扫描必须能捕获小程序线已落位的推客绑定路由（否则零声明断言静默恒真）");
        declared.Should().Contain("/channels/ec/league/promoter/get",
            "LS1 的扫描必须能捕获小店线继任面路由（继任能力在册，本域零声明结论才不是扫描失效的假象）");

        offenders.Should().BeEmpty(
            "10 条 /union/shop/* 路由不构成应用侧 HTTP 契约：出处 3 页属 union/custom_component 下架树"
            + "（与 LeaguePromoter 同源，实测一律 301 落《微信小商店升级为微信小店的公告》，非接口文档、无请求小节），"
            + "现行小店树零 union 提及；继任面归小店线 /channels/ec/league/*（ChannelsTokenTypes.AccessToken 令牌面）。"
            + "本仓任何产品线都不得声明（逐条追踪号见清单 P2-MP-LeagueShop-【端点】-01，2026-10-18 终判）");
    }

    // ------------------------------------------------------------------
    // LS2：本域对外表面恒空（零接口 / 零 DTO 命名空间 / 零 JsonContext 登记名）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LS2：主包与 DataModels 中本域类型零声明（防「已下架能力」以接口或 DTO 半成品形态回仓）。</summary>
    [Fact]
    public void LeagueShopDomain_ShouldHaveNoPublicTypes()
    {
        var mainAssembly = typeof(IWxaShopSharerService).Assembly;
        var dtoAssembly = typeof(Mud.Wechat.MiniProgram.DataModels.ShopSharer.WxaShopSharerInfo).Assembly;

        // 主包：不得出现任何名字含 LeagueShop / UnionShop 的公开类型（域别名一并锁，
        // 防「换个名字再补进来」；/union/* 其余子族由各自工作单守卫裁决，此处不扩成 Union 子串）。
        var mainOffenders = mainAssembly.GetTypes()
            .Where(static t => !t.IsNested && t.IsPublic
                && (t.Name.Contains("LeagueShop", StringComparison.OrdinalIgnoreCase)
                    || t.Name.Contains("UnionShop", StringComparison.OrdinalIgnoreCase)))
            .Select(static t => t.Name)
            .ToList();
        mainOffenders.Should().BeEmpty(
            "本域判非契约、接口一律不声明（LS1 取证）；出现同名接口即按已下架 SKIT 清单回补的半成品（追踪号 P2-MP-LeagueShop-01 族）");

        // DataModels：禁建本域 DTO 命名空间（类型集恒空即命名空间未启用）。
        var dtoOffenders = dtoAssembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == ProhibitedDtoNamespace)
            .Select(static t => t.Name)
            .ToList();
        dtoOffenders.Should().BeEmpty(
            $"{ProhibitedDtoNamespace} 不得出现任何类型：无端点即无请求/应答 DTO，禁「先建 DTO 后补接口」");

        // JsonContext 登记名：全 DataModels 程序集不得有 SerializerClassName == "LeagueShop" 的标注。
        var serializerOffenders = dtoAssembly.GetTypes()
            .Where(static t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() is { } attr
                && attr.SerializerClassName == ProhibitedSerializerClassName)
            .Select(static t => t.Name)
            .ToList();
        serializerOffenders.Should().BeEmpty(
            $"不得存在 [HttpJsonSerializable(SerializerClassName = \"{ProhibitedSerializerClassName}\")] 登记（孤儿上下文名会进生成器产物）");
    }

    // ------------------------------------------------------------------
    // LS3：DI 注册面恒空（模块枚举无本域成员、建造者无本域方法）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LS3：零端点 ⇒ 零注册组；防「枚举/建造者先注册、接口永远缺席」的空域假绿。</summary>
    [Fact]
    public void LeagueShopShouldNotAppearInModuleEnumOrBuilder()
    {
        var moduleNames = Enum.GetNames<MiniProgramModule>();
        moduleNames.Should().NotContain("LeagueShop")
            .And.NotContain("UnionShop",
            "本域判非契约 ⇒ MiniProgramModule 不得新增 LeagueShop / UnionShop 成员（注册三段式的第一段）");

        var builderMethods = typeof(MiniProgramServiceBuilder)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static m => m.Name)
            .ToList();
        builderMethods.Should().NotContain("AddLeagueShopApi")
            .And.NotContain("AddUnionShopApi",
            "建造者不得出现 AddLeagueShopApi / AddUnionShopApi（零接口的注册方法会让各域 Add…Api() 空转成功）");

        // 正对照：枚举与建造者的扫描口径必须能命中已落位域（NotContain 断言才不是空转）。
        moduleNames.Should().Contain("ShopSharer", "LS3 的正对照：枚举成员扫描口径有效");
        builderMethods.Should().Contain(static n => n == "AddAuthApi", "LS3 的正对照：建造者方法扫描口径有效");
    }

    // ------------------------------------------------------------------
    // LS4：跨线不交叉负锁（本线声明面不得出现 /channels/；本域 10 条逐条零命中）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 LS4：小程序线声明面不含 <c>/channels/</c> 前缀（小店线专属），且本域 10 条 <c>/union/shop/*</c> 逐条零命中。</summary>
    [Fact]
    public void MiniProgramLine_ShouldNotDeclareChannelsOrLeagueShopRoutes()
    {
        var declared = DeclaredRoutes(Path.Combine("Src", "MiniProgram"));

        // 正对照：扫描必须能捕获小程序线在册路由，否则空集合会让负锁静默恒真。
        declared.Should().Contain("/shop/sharer/bind",
            "LS4 的扫描必须能捕获小程序线已落位路由（否则零命中断言静默恒真）");

        declared.Where(static r => r.StartsWith("/channels/", StringComparison.Ordinal))
            .Should().BeEmpty(
            "含 /channels/ 的路由专属小店线（Mud.Wechat.Channels，ChannelsTokenTypes.AccessToken 令牌面）："
            + "本域继任面 /channels/ec/league/* 已在小店线落位，小程序线声明任何该前缀路由即跨线抢面 + 令牌串号");

        var unionOffenders = declared.Intersect(NonContractRoutes.Select(static r => r.Route), StringComparer.Ordinal).ToList();
        unionOffenders.Should().BeEmpty(
            "10 条 /union/shop/* 已随微信小商店文档树下架（LS1 取证：出处页一律 301 落升级公告、现行小店树零 union 提及）："
            + "小程序线不得声明之（追踪号 P2-MP-LeagueShop-【端点】-01 族，2026-10-18 终判）");

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
