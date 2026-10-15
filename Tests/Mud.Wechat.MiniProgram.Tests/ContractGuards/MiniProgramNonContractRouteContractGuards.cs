// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「清单候选 ≠ 契约」守卫（追踪号 MP-D-BUCKET-RULING-01，2026-10-13 D 桶 86 条终判）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何要有这一组用例</b>：缺失 API 清单里有 <b>86 条</b>小程序候选在「当时已爬的官方路由全集」中未命中。
/// 未命中<b>不等于</b>可落地，也<b>不等于</b>可判死 —— 本轮把判据补齐到<b>四棵官方文档树 + 出处页存活状态</b>：
/// ① 现行《小程序服务端 API 目录》（<c>/miniprogram/dev/OpenApiDoc/</c> 索引页，正文含 <b>430</b> 条请求路径）；
/// ② 旧 <c>/miniprogram/dev/server/API/</c> 树（433 条含方法的行）；③ 开放平台 <c>doc/oplatform/</c> 树；
/// ④ 公众号 <c>doc/offiaccount/</c> 树；再对每条候选的<b>清单出处 URL</b> 逐个实测 HTTP 状态与正文
/// （404 / SPA 空壳回落到目录首页 / 正文「此文档已下架」）。若不把这批结论写成可执行断言，
/// 后来者照清单「补」进来的会是<b>已下架能力</b>与<b>别线令牌面</b>——前者违反「官方声明下架即不建模」，
/// 后者直接造成令牌串号（见 P3-R2）。
/// </para>
/// <para>
/// <b>三档处置（86 条全覆盖，留档见 <c>.docs/MudWechat-小程序D桶归属终判留档-2026-10-13.md</c>）</b>：
/// </para>
/// <list type="number">
/// <item><b>非契约 25 条</b>（NC1，<b>全仓 Src/ 零声明</b>）：小程序推广员 14（官方 5 个 API 页正文均为
/// 「此文档已下架」，2026-10-13 浏览器取证）、搜一搜图片/站点搜索 2 与 <c>img_sec_check</c> 1（旧
/// <c>/miniprogram/dev/api-backend/</c> 树整树 <b>404</b>，现行目录无同名路径）、扫码购 4（清单出处是
/// <c>mp.weixin.qq.com/cgi-bin/announce</c> 公告页，无任何接口文档页）、非税实名 <c>getauthurl</c> 1
/// （出处页正文实测<b>不含任何</b> <c>api.weixin.qq.com</c> 请求地址）、以及<b>清单路由拼写错误</b> 3 条
/// （官方真名另有其路：即时配送更新配送单实为 <c>/cgi-bin/express/local/delivery/update_order</c>
/// 且<b>本仓已落位</b>；直播商品库实为 <c>/wxa/business/getgoodswarehouse</c>；短剧查授权实为
/// <c>/wxa/sec/vod/getauthorizeapps</c> 且官方动词是 POST）。错误拼写一旦照清单落地，就是「网关打不通的死路由」。</item>
/// <item><b>归属开放平台线 35 条</b>（NC2，<b>小程序线零声明</b>）：购物订单 <c>/user-order/*</c> 7、
/// We 分析 <c>/wedata/*</c> 7、第三方服务市场 <c>/wxa/servicemarket/*</c> 4、半屏小程序
/// <c>/wxaapi/wxaembedded/*</c> 6、微信认证 <c>/wxa/sec/{wxaauth,reauth,queryauth,uploadauthmaterial,authidentitytree}</c> 5、
/// 第三方登录态 <c>/sns/component/jscode2session</c> 与 <c>/sns/oauth2/component/{access_token,refresh_token}</c> 3、
/// <c>/wxa/business/{applyliveinfo,runtime/adddevice,runtime/pushappmsg}</c> 3。
/// 判据不是「网关可达性」而是<b>文档面归属</b>：这 35 条只在 <c>doc/oplatform/…/miniprogram-management/</c>
/// 树内在册（实测正文含请求方式与请求地址），走 <c>component_access_token</c> / 授权方令牌链
/// ⇒ 属开放平台线（P3-R2 明文禁止把该令牌链塞进小程序基础线）。小程序线声明之＝令牌串号。</item>
/// <item><b>线外 6 条</b>（NC3，<b>小程序线零声明</b>）：多端应用框架 Donut 5（端点在 <c>dev.weixin.qq.com</c> 而非 <c>api.weixin.qq.com</c>，
/// 与本仓 SSRF 白名单不同域，MP-X8）、服务号门店小程序 <c>/wxa/get_merchant_audit_info</c> 1
/// （官方正文在<b>公众号</b>「微信门店」树，同页另有「门店管理插件 2024-07-05 起下线新增门店」公告；
/// <b>本仓已落位于公众号线</b> <c>IMpStoreService</c>，小程序线不得重复声明）。
/// <b>本档原含「小游戏专属 20 条」</b>：出处为 <c>/minigame/dev/api-backend/</c> 独立文档树、页面全部存活，
/// 当时判为「在册但属另一产品形态」⇒ 留档「本项裁决可解除」。<b>2026-10-15 P1-8 收口批已解除该裁决</b>，
/// 20 端点按逐页取证落位于小程序线 <c>Mud.Wechat.MiniProgram</c> 的 Game 域（<see cref="IWxaGameService"/>，
/// 守卫 GM1~GM7）⇒ 从本档移除，同批把 NC3 的规模断言由 26 改为 6。</item>
/// </list>
/// <para>
/// <b>正对照（防空跑恒真）</b>：三档都跑同一趟「声明扫描」（剥离注释后取 HTTP 特性字面量与引号路径常量），
/// 断言必须同时捕获本仓<b>已落位</b>的相邻路由；否则扫描根或正则漂移会让「零声明」静默恒真（同 RD1 / P3-R1 口径）。
/// </para>
/// </remarks>
public class MiniProgramNonContractRouteContractGuards
{
    /// <summary>
    /// NC1：非契约路由 25 条 = 推广员 14 + 搜一搜 2 + <c>img_sec_check</c> 1 + 扫码购 4 + 非税实名 1
    /// + 清单拼写错误 3。扫描根为<b>整个 <c>Src/</c></b>（已下架能力不属于任何产品线）。
    /// </summary>
    private static readonly string[] NonContractRoutes =
    {
        // 小程序推广员 14 条：官方 5 个 API 页（role_manage / manage_promoter / invite_promoter /
        // message / promote_date）正文实测均为「此文档已下架」，现行小程序服务端 API 目录（430 条）无 /promoter/ 前缀。
        "/promoter/addrole",
        "/promoter/getrole",
        "/promoter/updaterole",
        "/promoter/addpromoter",
        "/promoter/getpromoter",
        "/promoter/updatepromoter",
        "/promoter/getinvitationmaterial",
        "/promoter/sendmsg",
        "/promoter/singlesendmsg",
        "/promoter/getmsg",
        "/promoter/getmsgclickdata",
        "/promoter/getsharematerial",
        "/promoter/getrelation",
        "/promoter/getorder",
        // 搜一搜图片/站点搜索 2 条 + 图片内容安全 1 条：出处所在旧 /miniprogram/dev/api-backend/ 整树 404。
        "/wxa/imagesearch",
        "/wxa/sitesearch",
        "/wxa/img_sec_check",
        // 扫码购 4 条：清单出处为公告页（announce），官方无接口文档页 ⇒ 能力已回收。
        "/scan/product/v2/add",
        "/scan/product/v2/status",
        "/scan/product/v2/getinfo",
        "/scan/product/v2/getinfobypage",
        // 非税实名取授权链接 1 条：出处页（微信非税缴费）正文实测不含任何 api.weixin.qq.com 请求地址。
        "/intp/realname/getauthurl",
        // 清单路由拼写错误 3 条：官方真名另有其路（前两条本仓已按真名落位，第三条归短剧批次另证）。
        "/cgi-bin/express/local/business/update_order",   // 真名 /cgi-bin/express/local/delivery/update_order
        "/wxaapi/broadcast/getgoodswarehouse",            // 真名 /wxa/business/getgoodswarehouse
        "/wxa/sec/vod/getauthorizedapps",                 // 真名 /wxa/sec/vod/getauthorizeapps（官方动词 POST）
    };

    /// <summary>
    /// NC2：归属开放平台线 35 条（令牌面为 <c>component_access_token</c> / 授权方令牌链）⇒ 小程序线零声明。
    /// </summary>
    private static readonly string[] OpenPlatformOwnedRoutes =
    {
        // 购物订单（第三方面）7 条：官方落位 doc/oplatform/…/shopping-orders/，正文实测含请求方式与地址。
        "/user-order/orders",
        "/user-order/orders/shippings",
        "/user-order/combine-orders",
        "/user-order/combine-orders/shippings",
        "/user-order/shoppinginfo/verify",
        "/user-order/orders-permission/open",
        "/user-order/orders-permission/confirm",
        // We 分析 7 条：doc/oplatform/…/we-analysis/ 树在册。
        "/wedata/wedata_get_login_config",
        "/wedata/wedata_set_login_config",
        "/wedata/wedata_get_perm_list",
        "/wedata/wedata_set_user_perm",
        "/wedata/wedata_query_bind_list",
        "/wedata/wedata_unbind_user",
        "/wedata/wedata_login",
        // 第三方服务市场（增值类）4 条：doc/oplatform/service_market/interface/value_added/ 树在册；
        // 与本线自建面 /wxa/servicemarket 与 /wxa/servicemarketretrieve 是不同令牌面的两组接口。
        "/wxa/servicemarket/service/get_service_buyer_list",
        "/wxa/servicemarket/service/get_service_buyer",
        "/wxa/servicemarket/get_paid_order_list",
        "/wxa/servicemarket/service/login_auth",
        // 半屏小程序（嵌入式管理）6 条：doc/oplatform/…/embedded-management/ 树在册。
        "/wxaapi/wxaembedded/get_list",
        "/wxaapi/wxaembedded/get_own_list",
        "/wxaapi/wxaembedded/add_embedded",
        "/wxaapi/wxaembedded/del_embedded",
        "/wxaapi/wxaembedded/del_authorize",
        "/wxaapi/wxaembedded/set_authorize",
        // 微信认证（第三方代提审）5 条：doc/oplatform/…/weapp-wxverify/ 树在册。
        "/wxa/sec/wxaauth",
        "/wxa/sec/reauth",
        "/wxa/sec/queryauth",
        "/wxa/sec/uploadauthmaterial",
        "/wxa/sec/authidentitytree",
        // 第三方登录态 3 条：component 令牌链，绝不可进小程序基础线（P3-R2）。
        "/sns/component/jscode2session",
        "/sns/oauth2/component/access_token",
        "/sns/oauth2/component/refresh_token",
        // 开放平台侧小程序能力 3 条：申请开通直播 + 多端框架(WMPF)后端 2 条。
        "/wxa/business/applyliveinfo",
        "/wxa/business/runtime/adddevice",
        "/wxa/business/runtime/pushappmsg",
    };

    /// <summary>
    /// NC3：线外 6 条 = 多端应用框架(Donut) 5 + 公众号门店 1 ⇒ 小程序线零声明
    ///（原含小游戏 20 条，2026-10-15 P1-8 收口批解除裁决、落位 Game 域）。
    /// </summary>
    private static readonly string[] OutOfLineRoutes =
    {
        // 多端应用框架（Donut）5 条：端点域名是 dev.weixin.qq.com，非 api.weixin.qq.com ⇒ 不同 SSRF 白名单（MP-X8）。
        "/donut/code2verifyinfo",
        "/donut/unbindphone",
        "/donut/unbindweixin",
        "/donut/unbindapple",
        "/donut/unregisteruser",
        // 服务号门店小程序 1 条：官方正文在公众号「微信门店」树，本仓已落位于 IMpStoreService ⇒ 本线不重复声明。
        "/wxa/get_merchant_audit_info",
    };

    // ------------------------------------------------------------------
    // NC1：25 条非契约路由在全仓 Src/ 零声明（扫描根与正则由已落位路由作正对照）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 NC1：非契约路由清单锁定 + <c>Src/</c> 全量零声明 + 已落位路由正对照。</summary>
    [Fact]
    public void NonContractRoutes_ShouldStayUndeclared_WhenScanningWholeSrc()
    {
        NonContractRoutes.Should().HaveCount(25,
            "推广员 14 + 搜一搜 2 + img_sec_check 1 + 扫码购 4 + 非税实名 1 + 清单拼写错误 3："
            + "清单本身不是契约，增删须先复核四棵官方文档树与出处页存活状态");
        NonContractRoutes.Distinct(StringComparer.Ordinal).Should().HaveCount(25, "清单内路由不得重复");

        var declared = DeclaredRoutes("Src");
        var offenders = declared.Intersect(NonContractRoutes, StringComparer.Ordinal).ToList();

        // 正对照：同一趟扫描必须捕获本仓已声明的相邻路由（含「拼写错误」条目的官方真名）。
        foreach (var control in new[]
                 {
                     "/wxa/msg_sec_check",                            // 与 img_sec_check 同前缀同域
                     "/wxa/media_check_async",                        // 图片审核的现行替代（异步媒体审核）
                     "/cgi-bin/express/local/delivery/update_order",  // local/business/update_order 的官方真名，已落位
                 })
        {
            declared.Should().Contain(control,
                "NC1 的扫描必须能捕获已落位的相邻路由（否则扫描根或正则漂移会让零声明断言静默恒真）");
        }

        offenders.Should().BeEmpty(
            "25 条路由不构成应用侧 HTTP 契约（官方页正文「此文档已下架」/ 出处页 404 / 无接口文档页 / 清单拼写错误），"
            + "本仓任何产品线都不得声明（追踪号 MP-D-BUCKET-RULING-01）");
    }

    // ------------------------------------------------------------------
    // NC2：35 条开放平台归属路由在小程序线零声明（防令牌串号，形态同 MP-X1 / P3-R2）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 NC2：开放平台令牌面路由在 <c>Src/MiniProgram</c> 零声明 + 小程序线自身路由正对照。</summary>
    [Fact]
    public void OpenPlatformOwnedRoutes_ShouldStayUndeclared_InMiniProgramLine()
    {
        OpenPlatformOwnedRoutes.Should().HaveCount(35,
            "购物订单 7 + We 分析 7 + 第三方服务市场 4 + 半屏小程序 6 + 微信认证 5 + 第三方登录态 3 + 开放平台侧小程序能力 3");

        var declared = DeclaredRoutes(Path.Combine("Src", "MiniProgram"));
        var offenders = declared.Intersect(OpenPlatformOwnedRoutes, StringComparer.Ordinal).ToList();

        foreach (var control in new[]
                 {
                     "/wxa/servicemarket",                    // 本线自建服务面：与第三方 servicemarket/* 同前缀不同令牌面
                     "/wxa/servicemarketretrieve",
                     "/wxa/business/checkencryptedmsg",       // /wxa/business/ 前缀的活体证明（applyliveinfo/runtime 的同前缀对照）
                     "/wxa/msg_sec_check",                    // /wxa/sec/* 与 /wxa/* 族的活体证明
                 })
        {
            declared.Should().Contain(control,
                "NC2 的扫描必须能捕获小程序线已声明的相邻路由（否则零声明断言静默恒真）");
        }

        offenders.Should().BeEmpty(
            "35 条路由只在 doc/oplatform/…/miniprogram-management/ 树内在册，令牌面是 component_access_token / "
            + "授权方令牌链 ⇒ 属开放平台线；小程序线声明之即令牌串号（P3-R2）");
    }

    // ------------------------------------------------------------------
    // NC3：6 条线外路由在小程序线零声明（小游戏 20 条已解除裁决、落位 Game 域）。
    // ------------------------------------------------------------------

    /// <summary>契约守卫 NC3：Donut / 公众号门店路由在 <c>Src/MiniProgram</c> 零声明。</summary>
    [Fact]
    public void OutOfLineRoutes_ShouldStayUndeclared_InMiniProgramLine()
    {
        OutOfLineRoutes.Should().HaveCount(6,
            "多端应用框架(Donut) 5 + 公众号门店 1：两档「线外」理由各不相同，见类型 remarks 第 3 项");

        var declared = DeclaredRoutes(Path.Combine("Src", "MiniProgram"));
        var offenders = declared.Intersect(OutOfLineRoutes, StringComparer.Ordinal).ToList();

        foreach (var control in new[]
                 {
                     "/wxa/getwxacodeunlimit",      // /wxa/ 前缀的活体证明（与 Donut / 门店那 6 条同为 /wxa/* 形态）
                     "/cgi-bin/soter/verify_signature", // /wxa/soter/ 能力的现行落位（同域活体证明）
                 })
        {
            declared.Should().Contain(control,
                "NC3 的扫描必须能捕获小程序线已声明的相邻路由（否则零声明断言静默恒真）");
        }

        offenders.Should().BeEmpty(
            "6 条路由不属小程序服务端 API 面："
            + "Donut 端点在 dev.weixin.qq.com（MP-X8 SSRF 白名单不同域）、门店面本仓已落位于公众号线");
    }

    /// <summary>
    /// 扫描根下全部「已声明路由」：剥离注释后取 HTTP 特性字面量与引号包裹的路径常量。
    /// 剥离注释是必要的——既有裁决注释（如 <c>/wxa/img_sec_check</c> 刻意不实现）会被原样命中。
    /// </summary>
    private static HashSet<string> DeclaredRoutes(string relativeRoot)
    {
        var root = Path.Combine(GetSolutionRoot(), relativeRoot);
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

            foreach (System.Text.RegularExpressions.Match m in literalRegex.Matches(code))
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
