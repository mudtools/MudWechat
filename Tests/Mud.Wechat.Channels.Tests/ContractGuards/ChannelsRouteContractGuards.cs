// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.Extensions;
using Mud.Wechat.Channels.Abstractions.Authentication;
using Mud.Wechat.Channels.Extensions;
using Mud.Wechat.MiniProgram.Extensions;
using Mud.Wechat.OfficialAccount.Extensions;
using Mud.Wechat.Pay.Extensions;
using Mud.Wechat.Work.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店线路由守卫（设计方案 v1 P0-c / CH-R1、R2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>CH-R1（重叠路由零回潮）</b>：小店线（<c>/channels/ec/*</c>、<c>/shop/*</c>、<c>/cgi-bin/*</c>、
/// <c>/channels/finderlive|leads|livedashboard/*</c>）与既有五线（公众号 / 企微 / 小程序 / 支付 / 广告）
/// 的路由<b>交叠必须为空</b>，公众号线「产品卡」先例（<c>IMpProductCardService</c>，
/// <c>/channels/ec/service/product/getcardinfo</c>，公众号令牌）不得在小店线重复声明。
/// 唯一豁免：基础面<b>同令牌族内</b>的同路由重复（token / stable_token，授权于设计方案 §4.5
/// 「新令牌类型 + 同路由，两线各自声明各自消费」，以及 P1 落位的 8 个 Basic 端点 —— 新增豁免
/// 必须同批扩展 <see cref="SharedInfrastructureRoutes"/> 并注明追踪理由）。
/// </para>
/// <para>
/// <b>CH-R2（逐域路由表）</b>：27 个业务域（对齐设计方案 §2.2 注册方法表）+ 令牌基座的路由表
/// 精确断言；<c>/shop/</c> 历史前缀照抄官方原文，禁止「对齐规范」改写（设计方案 §4.3）。
/// 脚手架期业务域<b>零端点</b>，P1 起逐域落端点时<b>同批</b>更新本表（守卫是权威描述，不是收尾项）。
/// </para>
/// </remarks>
public class ChannelsRouteContractGuards
{
    /// <summary>
    /// 官方基础面「同令牌族内允许重复声明」的共享基础设施路由白名单（对齐 §4.5）。
    /// 新增（P1：quota / rid / clear_quota / callback check / 双 IP）必须同批扩展并注明追踪理由；
    /// 白名单之外任何与既有五线的路由交叠都是变红项。
    /// </summary>
    /// <remarks>
    /// <b>P1 落位（设计方案 v1 §4.5）</b>：8 个 Basic 端点与公众号线<b>云端同路由</b>，但令牌凭据
    /// 体系不同（小店 AppID vs 公众号 AppID）→ 本线自建 <c>AddBasicApi</c>，不抽共享、不并入公众号线。
    /// 该 8 路由与公众号线 <c>IMpBasicService</c> / <c>IMpOpenApiService</c> / <c>IMpOpenApiTokenFreeService</c>
    /// 路由精确重叠，故列入白名单放行（两线各自声明各自消费）。
    /// </remarks>
    private static readonly IReadOnlySet<string> SharedInfrastructureRoutes =
        new HashSet<string>(StringComparer.Ordinal)
        {
            // token 签发（小店令牌基座；公众号 IMpAuthentication 亦声明同路由）
            "/cgi-bin/token",
            "/cgi-bin/stable_token",

            // P1 Basic 域（设计方案 §4.5：云端同路由、凭据独立，与公众号线各自声明各自消费）
            "/cgi-bin/get_api_domain_ip",
            "/cgi-bin/getcallbackip",
            "/cgi-bin/callback/check",
            "/cgi-bin/clear_quota",
            "/cgi-bin/clear_quota/v2",
            "/cgi-bin/openapi/quota/get",
            "/cgi-bin/openapi/quota/clear",
            "/cgi-bin/openapi/rid/get",
        };

    /// <summary>设计方案 §2.2：27 个业务域 → 注册方法名（守卫按域枚举接口，命名即契约）。</summary>
    private static readonly string[] Domains =
    {
        "Basic", "Resource", "Shop", "HomePage", "Product", "Favorite", "Category", "Order",
        "Funds", "Marketing", "Aftersale", "Kf", "Qic", "Logistics", "Warehouse", "League",
        "Brand", "Delivery", "Wecom", "MiniStore", "Compass", "Vip", "Live", "Window",
        "Locallife", "Subsidy", "PlatformKf",
    };

    /// <summary>
    /// 逐域路由表（CH-R2 权威描述）：<c>域 → 精确路由数组</c>。
    /// 脚手架期业务域全部为空；P1 起每落一个端点必须同时更新本表与
    /// <see cref="DomainRouteTables_ShouldMatchDeclaredEndpoints"/> 的计数断言。
    /// </summary>
    private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> ExpectedRoutesByDomain =
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal)
        {
            ["Basic"] = new[]
            {
                "/cgi-bin/callback/check",
                "/cgi-bin/clear_quota",
                "/cgi-bin/clear_quota/v2",
                "/cgi-bin/get_api_domain_ip",
                "/cgi-bin/getcallbackip",
                "/cgi-bin/openapi/quota/clear",
                "/cgi-bin/openapi/quota/get",
                "/cgi-bin/openapi/rid/get",
            },
            ["Resource"] = Array.Empty<string>(),
            ["Shop"] = Array.Empty<string>(),
            ["HomePage"] = Array.Empty<string>(),
            // Product：43 端点（商品主 25 + 库存 4 + 赠品 6 + 买赠活动 3 + 限时抢购 5）。
            // 与 SHIPINHAO 本地生活重叠的 5 个同路由（audit/cancel、delete、listing、delisting、stock/update）只计 1。
            ["Product"] = new[]
            {
                "/channels/ec/product/add",
                "/channels/ec/product/update",
                "/channels/ec/product/get",
                "/channels/ec/product/list/get",
                "/channels/ec/product/listing",
                "/channels/ec/product/delisting",
                "/channels/ec/product/delete",
                "/channels/ec/product/audit/cancel",
                "/channels/ec/product/auditfree",
                "/channels/ec/product/auditstrategy/get",
                "/channels/ec/product/auditstrategy/set",
                "/channels/ec/product/getauditquota",
                "/channels/ec/product/getproductrestrictedinfo",
                "/channels/ec/product/category/classify",
                "/channels/ec/product/categoryprecheck",
                "/channels/ec/product/addproductthirdpartysource",
                "/channels/ec/product/productbrandrecommend",
                "/channels/ec/product/externalproductmapping",
                "/channels/ec/product/externalproductmappingnew",
                "/channels/ec/product/begintimingsale",
                "/channels/ec/product/canceltimingsale",
                "/channels/ec/product/taglink/get",
                "/channels/ec/product/qrcode/get",
                "/channels/ec/product/h5url/get",
                "/channels/ec/product/scheme/get",
                "/channels/ec/product/stock/get",
                "/channels/ec/product/stock/update",
                "/channels/ec/product/stock/batchget",
                "/channels/ec/product/stock/getflow",
                "/channels/ec/product/gift/add",
                "/channels/ec/product/gift/get",
                "/channels/ec/product/gift/list/get",
                "/channels/ec/product/gift/onsale/set",
                "/channels/ec/product/gift/stock/update",
                "/channels/ec/product/gift/update",
                "/channels/ec/product/activity/add",
                "/channels/ec/product/activity/del",
                "/channels/ec/product/activity/stop",
                "/channels/ec/product/limiteddiscounttask/add",
                "/channels/ec/product/limiteddiscounttask/update",
                "/channels/ec/product/limiteddiscounttask/delete",
                "/channels/ec/product/limiteddiscounttask/stop",
                "/channels/ec/product/limiteddiscounttask/list/get",
            },
            // Order：27 端点（/channels/ec/order/* 24 + /channels/ec/merchant/privatenumber/* 3）。
            ["Order"] = new[]
            {
                "/channels/ec/merchant/privatenumber/addphone",
                "/channels/ec/merchant/privatenumber/getphone",
                "/channels/ec/merchant/privatenumber/sendverifycode",
                "/channels/ec/order/address/update",
                "/channels/ec/order/addressmodify/accept",
                "/channels/ec/order/addressmodify/reject",
                "/channels/ec/order/blindboxitem/set",
                "/channels/ec/order/deliveryinfo/update",
                "/channels/ec/order/deliverynegotiation/result/get",
                "/channels/ec/order/deliverynegotiation/submit",
                "/channels/ec/order/freshinspect/submit",
                "/channels/ec/order/get",
                "/channels/ec/order/list/get",
                "/channels/ec/order/merchantnotes/update",
                "/channels/ec/order/presentnote/add",
                "/channels/ec/order/presentsuborder/get",
                "/channels/ec/order/preshipmentchangesku/approve",
                "/channels/ec/order/preshipmentchangesku/get",
                "/channels/ec/order/preshipmentchangesku/reject",
                "/channels/ec/order/price/update",
                "/channels/ec/order/realnumber/apply",
                "/channels/ec/order/realnumberviewaudit/get",
                "/channels/ec/order/search",
                "/channels/ec/order/sensitiveinfo/decode",
                "/channels/ec/order/userbooking/list",
                "/channels/ec/order/virtualnumber/applyagain",
                "/channels/ec/order/virtualnumber/delay",
            },
            ["Favorite"] = Array.Empty<string>(),
            ["Category"] = Array.Empty<string>(),
            ["Funds"] = new[]
            {
                // /channels/ec/funds/*（9 端点：资金账户 + 提现 + 流水）
                "/channels/ec/funds/getbalance",
                "/channels/ec/funds/getbankacct",
                "/channels/ec/funds/setbankacct",
                "/channels/ec/funds/submitwithdraw",
                "/channels/ec/funds/getwithdrawlist",
                "/channels/ec/funds/getwithdrawdetail",
                "/channels/ec/funds/getfundsflowlist",
                "/channels/ec/funds/getfundsflowdetail",
                "/channels/ec/funds/listorderflow",
                // /shop/funds/*（7 端点：官方历史前缀照抄原文，设计方案 v1 §4.3）
                "/shop/funds/getcity",
                "/shop/funds/getprovince",
                "/shop/funds/getbanklist",
                "/shop/funds/getsubbranch",
                "/shop/funds/getbankbynum",
                "/shop/funds/qrcode/get",
                "/shop/funds/qrcode/check",
            },
            ["Marketing"] = Array.Empty<string>(),
            // Aftersale：27 端点（售后单 16 + 纠纷单 4 + 保障单 6 + 全量售后原因 / 拒绝原因）。
            // 与 SHIPINHAO 本地生活重叠的 getaftersaleorder 只计 1。
            ["Aftersale"] = new[]
            {
                "/channels/ec/aftersale/acceptapply",
                "/channels/ec/aftersale/acceptexchangereship",
                "/channels/ec/aftersale/addcomplaintmaterial",
                "/channels/ec/aftersale/addcomplaintproof",
                "/channels/ec/aftersale/applyvirtualtelnum",
                "/channels/ec/aftersale/genaftersaleorder",
                "/channels/ec/aftersale/getaftersalelist",
                "/channels/ec/aftersale/getaftersaleorder",
                "/channels/ec/aftersale/getcomplaintorder",
                "/channels/ec/aftersale/getexchangeableskulist",
                "/channels/ec/aftersale/getguaranteeorder",
                "/channels/ec/aftersale/handlefastexchangereceipt",
                "/channels/ec/aftersale/merchantacceptguarantee",
                "/channels/ec/aftersale/merchantmodifyguarantee",
                "/channels/ec/aftersale/merchantproofguarantee",
                "/channels/ec/aftersale/merchantrefuseguarantee",
                "/channels/ec/aftersale/merchantreship",
                "/channels/ec/aftersale/merchantupdateaftersale",
                "/channels/ec/aftersale/merchantupdatereshipexpress",
                "/channels/ec/aftersale/reason/get",
                "/channels/ec/aftersale/refundpricediff",
                "/channels/ec/aftersale/rejectapply",
                "/channels/ec/aftersale/rejectexchangereship",
                "/channels/ec/aftersale/rejectreason/get",
                "/channels/ec/aftersale/searchguaranteeorder",
                "/channels/ec/aftersale/syncworkorder",
                "/channels/ec/aftersale/uploadrefundcertificate",
            },
            ["Kf"] = Array.Empty<string>(),
            ["Qic"] = Array.Empty<string>(),
            // Logistics：28 端点（地址 5 + 运费模板 4 + 电子面单 16 + 订单发货 3）。
            ["Logistics"] = new[]
            {
                // 地址（5）：/channels/ec/merchant/address/*
                "/channels/ec/merchant/address/add",
                "/channels/ec/merchant/address/get",
                "/channels/ec/merchant/address/list",
                "/channels/ec/merchant/address/delete",
                "/channels/ec/merchant/address/update",
                // 运费模板（4）：/channels/ec/merchant/freight*
                "/channels/ec/merchant/addfreighttemplate",
                "/channels/ec/merchant/getfreighttemplatedetail",
                "/channels/ec/merchant/getfreighttemplatelist",
                "/channels/ec/merchant/updatefreighttemplate",
                // 电子面单（16）：/channels/ec/logistics/ewaybill/biz/*
                "/channels/ec/logistics/ewaybill/biz/order/create",
                "/channels/ec/logistics/ewaybill/biz/order/precreate",
                "/channels/ec/logistics/ewaybill/biz/order/get",
                "/channels/ec/logistics/ewaybill/biz/order/cancel",
                "/channels/ec/logistics/ewaybill/biz/order/print",
                "/channels/ec/logistics/ewaybill/biz/order/batchprint",
                "/channels/ec/logistics/ewaybill/biz/order/addsuborder",
                "/channels/ec/logistics/ewaybill/biz/delivery/get",
                "/channels/ec/logistics/ewaybill/biz/print/get",
                "/channels/ec/logistics/ewaybill/biz/template/config",
                "/channels/ec/logistics/ewaybill/biz/template/getbyid",
                "/channels/ec/logistics/ewaybill/biz/template/get",
                "/channels/ec/logistics/ewaybill/biz/template/create",
                "/channels/ec/logistics/ewaybill/biz/template/update",
                "/channels/ec/logistics/ewaybill/biz/template/delete",
                "/channels/ec/logistics/ewaybill/biz/account/get",
                // 订单发货（3）：/channels/ec/order/delivery*
                "/channels/ec/order/delivery/send",
                "/channels/ec/order/deliverycompanylist/new/get",
                "/channels/ec/order/delivery/compensation",
            },
            ["Warehouse"] = Array.Empty<string>(),
            ["League"] = Array.Empty<string>(),
            ["Brand"] = Array.Empty<string>(),
            ["Delivery"] = Array.Empty<string>(),
            ["Wecom"] = Array.Empty<string>(),
            ["MiniStore"] = Array.Empty<string>(),
            ["Compass"] = Array.Empty<string>(),
            ["Vip"] = Array.Empty<string>(),
            ["Live"] = Array.Empty<string>(),
            ["Window"] = Array.Empty<string>(),
            ["Locallife"] = Array.Empty<string>(),
            ["Subsidy"] = Array.Empty<string>(),
            ["PlatformKf"] = Array.Empty<string>(),
        };

    /// <summary>
    /// CH-R1：重叠路由零回潮 —— 小店线路由 ⊆ 白名单 ∪ ∅，与既有五线路由的交叠必须为空。
    /// </summary>
    /// <remarks>
    /// 参照集（公众号 / 企微 / 小程序 / 支付 / 广告五线主包路由并集）强制<b>非空</b>：参照集为空说明
    /// 反射口径失效，守卫会假绿（AGENTS §6「数量下限防枚举空跑」同款纪律）。开放平台线用显式客户端、
    /// 无声明式路由属性，故不入参照集。
    /// </remarks>
    [Fact]
    public void OverlappingRoutes_ShouldNotBeDeclared_WhenChannelsLineAddsEndpoints()
    {
        var existingRoutes = new HashSet<string>(StringComparer.Ordinal);
        existingRoutes.UnionWith(CollectRequestUris(LoadProductLine(typeof(MpModule))));
        existingRoutes.UnionWith(CollectRequestUris(LoadProductLine(typeof(WechatModule))));
        existingRoutes.UnionWith(CollectRequestUris(LoadProductLine(typeof(MiniProgramModule))));
        existingRoutes.UnionWith(CollectRequestUris(LoadProductLine(typeof(PayModule))));
        // 广告线（2026-10 并入本仓）亦为声明式路由线，必须纳入参照集：漏掉它 = 小店线可静默回潮广告线路由。
        existingRoutes.UnionWith(CollectRequestUris(LoadProductLine(typeof(AdsModule))));

        existingRoutes.Should().NotBeEmpty("既有五线路由是本守卫的参照集，为空说明反射口径失效，守卫会假绿");

        // 公众号「产品卡」先例路由必须存在于参照集（防参照集漏检该唯一例外）。
        existingRoutes.Should().Contain("/channels/ec/service/product/getcardinfo",
            "公众号线 IMpProductCardService 的产品卡路由是 CH-R1 的锚点（设计方案 §4.2），不得从参照集消失");

        var channelsRoutes = new HashSet<string>(StringComparer.Ordinal);
        channelsRoutes.UnionWith(CollectRequestUris(LoadProductLine(typeof(ChannelsModule))));
        channelsRoutes.UnionWith(CollectRequestUris(LoadProductLine(typeof(IChannelsAuthentication))));
        // 本包为纯宿主包（零手写类型），无编译期锚点可用 ⇒ 名称加载（含探测失败兜底，见 LoadProductLine）。
        channelsRoutes.UnionWith(CollectRequestUris(LoadProductLine("Mud.Wechat.Channels.Callback")));

        var overlap = channelsRoutes
            .Where(static r => !SharedInfrastructureRoutes.Contains(r))
            .Intersect(existingRoutes, StringComparer.Ordinal)
            .ToArray();
        overlap.Should().BeEmpty(
            "小店线不得重复声明既有五线路由（设计方案 CH-R1）；基础面同令牌族重复仅限白名单豁免：" +
            string.Join(", ", overlap));
    }

    /// <summary>
    /// CH-R2：逐域路由表精确断言 —— 主包全部 <c>IChannels{Domain}Service</c> 接口的路由
    /// 必须与 <see cref="ExpectedRoutesByDomain"/> 完全一致（域不存在 / 多落 / 路由漂移均变红）。
    /// </summary>
    /// <remarks>
    /// 域枚举按接口前缀 <c>IChannels{Xxx}Service</c> 反射定位（接口尚未声明时按域存在性判定：
    /// 存在接口即须能到表里查域 —— 防止「落了接口没落表」的半成品状态）。
    /// </remarks>
    [Fact]
    public void DomainRouteTables_ShouldMatchDeclaredEndpoints()
    {
        var asm = LoadProductLine(typeof(ChannelsModule));
        var interfaces = asm.GetTypes()
            .Where(static t => t.IsInterface)
            .ToArray();

        // 每个域：存在对应接口则断言其路由表；不存在接口则断言「表中该域仍为空」（P0 全空）。
        // 双接口同域形态（对齐公众号 OpenApi 域先例）：主接口 IChannels{Domain}Service +
        // 免令牌变体 IChannels{Domain}TokenFreeService（clear_quota/v2 等应急逃生端点）。
        var actualByDomain = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        foreach (var domain in Domains)
        {
            var expectedName = "IChannels" + domain + "Service";
            var tokenFreeName = "IChannels" + domain + "TokenFreeService";
            var ifaces = interfaces
                .Where(t => t.Name.Equals(expectedName, StringComparison.Ordinal)
                            || t.Name.Equals(tokenFreeName, StringComparison.Ordinal))
                .ToArray();
            actualByDomain[domain] = ifaces
                .SelectMany(static i => CollectRequestUrisOfType(i))
                .OrderBy(static r => r, StringComparer.Ordinal)
                .ToList();
        }

        var expectedFlattened = ExpectedRoutesByDomain
            .SelectMany(static kv => kv.Value)
            .OrderBy(static r => r, StringComparer.Ordinal)
            .ToArray();
        var actualFlattened = actualByDomain
            .SelectMany(static kv => kv.Value)
            .OrderBy(static r => r, StringComparer.Ordinal)
            .ToArray();

        actualFlattened.Should().Equal(expectedFlattened,
            "主包路由并集必须与 27 域路由表完全一致（缺端点 / 多端点均变红；P0 应为空）");

        // 逐域计数断言（防「路由进了并集但域归属漂移」）。
        foreach (var domain in Domains)
        {
            actualByDomain[domain].Should().BeEquivalentTo(ExpectedRoutesByDomain[domain],
                $"域 {domain} 的路由表必须与守卫表一致（设计方案 §2.2 计数口径）");
        }
    }

    /// <summary>
    /// CH-R2 补充：令牌基座（Abstractions）只允许声明 /cgi-bin/token 与 /cgi-bin/stable_token
    /// 两条签发路由（双通道），不得混入业务路由。
    /// </summary>
    [Fact]
    public void AuthenticationBase_ShouldOnlyExposeTokenRoutes()
    {
        var routes = CollectRequestUrisOfType(typeof(IChannelsAuthentication))
            .OrderBy(static r => r, StringComparer.Ordinal)
            .ToArray();

        routes.Should().Equal(
            new[] { "/cgi-bin/stable_token", "/cgi-bin/token" },
            "小店令牌基座只签发双通道令牌（设计方案 §3.2），路由面不得扩张");
    }

    /// <summary>
    /// CH-R2 计数斜杠：本守卫断言的总路由数（P0 = 2 条签发路由）。
    /// </summary>
    [Fact]
    public void AuthBaseRouteCount_ShouldBeTwo_WhenScaffold()
    {
        CollectRequestUrisOfType(typeof(IChannelsAuthentication))
            .Should().HaveCount(2, "双通道 token/stable_token 恰 2 条签发路由（P0 形态）");
    }

    // ---- helpers ------------------------------------------------------------

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }

    /// <summary>
    /// 按编译期类型锚点取产品线程序集（<c>anchor.Assembly</c>）：引用在编译期即绑定，
    /// 加载由 CLR 统一管理，<b>不经</b>运行时按名称探测 —— 字符串版 <see cref="LoadProductLine(string)"/>
    /// 曾在 CI 上偶发 <c>FileNotFoundException</c>（deps.json 探测瞬态失败，trx 留档 2026-10-10）。
    /// </summary>
    private static Assembly LoadProductLine(Type anchor)
    {
        var asm = anchor.Assembly;
        asm.GetName().Name.Should().NotBeNullOrEmpty();
        return asm;
    }

    /// <summary>
    /// 按名称取程序集（仅用于<b>零手写类型</b>的纯宿主包）：先查已加载表，再按名称加载；
    /// 名称探测偶发失败而构建产物内文件确在（构建门禁保证）时，按文件路径兜底一次。
    /// </summary>
    private static Assembly LoadProductLine(string name)
    {
        var asm = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == name);
        if (asm is not null)
        {
            return asm;
        }

        try
        {
            asm = Assembly.Load(name);
        }
        catch (FileNotFoundException) when (File.Exists(Path.Combine(AppContext.BaseDirectory, name + ".dll")))
        {
            asm = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, name + ".dll"));
        }

        asm.GetName().Name.Should().Be(name);
        return asm;
    }

    private static HashSet<string> CollectRequestUris(Assembly asm)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var type in asm.GetTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                var uri = method.GetCustomAttributes(false)
                    .Select(static a => a.GetType().GetProperty("RequestUri")?.GetValue(a) as string)
                    .FirstOrDefault(static u => !string.IsNullOrWhiteSpace(u));
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    set.Add(uri!);
                }
            }
        }
        return set;
    }

    private static HashSet<string> CollectRequestUrisOfType(Type type)
    {
        var set = new HashSet<string>(StringComparer.Ordinal);
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
        {
            foreach (var attr in method.GetCustomAttributes(false))
            {
                var uri = attr.GetType().GetProperty("RequestUri")?.GetValue(attr) as string;
                if (!string.IsNullOrWhiteSpace(uri))
                {
                    set.Add(uri!);
                }
            }
        }
        return set;
    }
}