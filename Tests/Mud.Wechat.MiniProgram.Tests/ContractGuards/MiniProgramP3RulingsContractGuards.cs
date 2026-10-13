// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// P3（小程序长尾：<c>minishop</c> / 直播 / 云开发 / 第三方平台）的<b>处置裁决</b>守卫。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何要有这一组用例（而不是「什么都不做」）</b>：设计方案把 P3 列为<b>「按需」</b>，
/// 并在 §3.6 附近点名了四个入口（<c>/wxaapi/minishop/*</c>、<c>/wxaapi/broadcast/*</c>、
/// 云开发、第三方平台令牌链）。若不把「为什么不实现」写成<b>可执行</b>的裁决，后来者会照着
/// 那行清单「补」出能力 —— 而其中<b>至少一项已在官方下线</b>、另有一项被文档明令
/// <b>不得</b>进入小程序基础线。本守卫即是这四条裁决的机械化落地。
/// </para>
/// <para>
/// <b>本轮核验到的四条事实</b>（2026-10-09）：
/// </para>
/// <list type="number">
/// <item>
/// <b>小商店（minishop）的店铺 / 商品管理面已下线</b>：官方《交易组件·组件运营规范》原文
/// 「目前「小商店」已下线，请直接前往 <b>微信小店</b>」⇒ 按本仓纪律「官方声明下架即不建模」
/// <b>不实现</b>。<i>（注意区分：视频号「自定义交易组件」是另一套仍在册的能力，不在本项裁决内。）</i>
/// <br />
/// <b>2026-10-13 收窄（本组唯一一次裁决变更，非放宽）</b>：原条文按<b>前缀</b>整族禁止
/// <c>/wxaapi/minishop/*</c>，但官方小程序服务端 API 的
/// <c>transaction-guarantee/complaint</c> 章节把 <b>5 条投诉处理端点</b>留在册且沿用
/// <c>minishop</c> 这一历史前缀（逐页核验：正文可渲染、在目录树内、适用范围「小程序」账号类型、
/// 令牌为小程序 <c>access_token</c>、第三方代调用权限集 76）⇒ 判据从「前缀」改为「能力面」：
/// 店铺 / 商品管理面仍<b>零声明</b>，投诉处理 5 条按 <see cref="MinishopComplaintRoutes"/>
/// <b>清单式在册</b>。收窄后本用例只会更严（多一条即红、少一条亦红），不存在绕开路径。
/// </item>
/// <item>
/// <b>小程序直播（<c>/wxaapi/broadcast/*</c>）：本项裁决已于 2026-10-13 <b>解除</b></b>。
/// 原条文按「公开报道（2024-04 起）称小程序直播<b>插件</b>已停止接入 + 本轮未取得官方页面」把整个
/// 前缀<b>整族零声明</b>，并明文标注「本项裁决是<b>可解除</b>的（与其它三条『官方已声明』的裁决不同）」。
/// 2026-10-13 逐页取证（38 个服务端 API 页全文渲染 + 参数表完整）推翻其前提：
/// ① 每页均声明「权限集 id 52」「服务器端调用」「云调用方法 <c>liveBroadcast.*</c>」
/// 与「可使用 <c>access_token</c>、<c>authorizer_access_token</c>」；
/// ② 现行 <c>OpenApiDoc/livebroadcast</c> 目录树与旧 <c>miniprogram/dev/server/API/livebroadcast</c>
/// 树的「路由 + 动词」清单<b>逐条相同</b>（38 = 38）；
/// ③ 真正「页面不存在或已失效」的是 <c>dev/framework/activity/live-player-plugin</c>
/// ——即<b>直播插件</b>（前端组件形态），<b>不是</b>服务端 API。
/// ⇒ 判据从「前缀整族禁令」改为「清单式在册」（<see cref="LiveBroadcastRoutes"/> + 文件级豁免），
/// 形态与第 1 项 minishop 的收窄完全一致：38 端点中 <c>/wxaapi/broadcast/*</c> 34 条在册，
/// 另 4 条走 <c>/wxa/business/*</c>（<c>getliveinfo</c> / <c>getgoodswarehouse</c> /
/// <c>get_wxa_followers</c> / <c>push_message</c>，38 条全集的路由与动词由 <c>WxaLiveContractGuards</c> LB1 逐条锁定）。
/// 收窄后本用例只会更严（多一条即红、少一条亦红），不存在绕开路径。
/// </item>
/// <item>
/// <b>云开发不进小程序基础线</b>：官方云开发服务端调用依赖开放平台
/// <c>component_access_token</c>（令牌链与自建小程序线的 <c>access_token</c> <b>不是同一套</b>）。
/// </item>
/// <item>
/// <b>第三方平台令牌链不进小程序基础线</b>：设计方案明确「须<b>独立令牌管理器</b>，
/// <b>不塞进小程序基础线</b>（避免令牌串号）」，且列为「一期<b>可选</b>扩展」。
/// 本仓现状佐证：小程序线<b>没有</b>任何 component / authorizer 令牌类型（见 P3-R2）。
/// </item>
/// </list>
/// </remarks>
public class MiniProgramP3RulingsContractGuards
{
    /// <summary>
    /// P3-R1 收窄后的 <c>/wxaapi/minishop/*</c> <b>在册清单</b>：官方 2026-10-13 仍在小程序
    /// <c>transaction-guarantee/complaint</c> 章节的 5 条投诉处理路由（逐字照官方，含
    /// <c>bussi</c> / <c>busi</c> 两族拼写差异）。
    /// </summary>
    private static readonly string[] MinishopComplaintRoutes =
    {
        "/wxaapi/minishop/complaintOrderDetail",
        "/wxaapi/minishop/bussiRespondComplaint",
        "/wxaapi/minishop/bussiSupplyProof",
        "/wxaapi/minishop/bussiSupplyRefund",
        "/wxaapi/minishop/busiAppeal",
    };

    /// <summary>承载上述 5 条在册路由的唯一文件（文件级豁免，店铺 / 商品面出现在其它文件仍判违规）。</summary>
    private static readonly string[] MinishopWhitelistedFiles = { "IWxaTradeComplaintService.cs" };

    /// <summary>
    /// P3-R1 于 2026-10-13 解除「直播不实现」裁决后的 <c>/wxaapi/broadcast/*</c> <b>在册清单</b>：
    /// 官方 <c>OpenApiDoc/livebroadcast</c> 分区 34 条服务端路由（直播间本体 10 + 小助手与副号 8 +
    /// 房间内商品 7 + 商品库 6 + 成员角色 3；另 4 条直播端点走 <c>/wxa/business/*</c>，不在本前缀清单内）。
    /// 逐字照官方，含 <c>getapproved</c>（全小写）与 <c>getVideo</c> / <c>deleteInRoom</c>（驼峰）两族拼写差异。
    /// </summary>
    private static readonly string[] LiveBroadcastRoutes =
    {
        "/wxaapi/broadcast/room/create",
        "/wxaapi/broadcast/room/editroom",
        "/wxaapi/broadcast/room/deleteroom",
        "/wxaapi/broadcast/room/getpushurl",
        "/wxaapi/broadcast/room/getsharedcode",
        "/wxaapi/broadcast/room/addassistant",
        "/wxaapi/broadcast/room/getassistantlist",
        "/wxaapi/broadcast/room/modifyassistant",
        "/wxaapi/broadcast/room/removeassistant",
        "/wxaapi/broadcast/room/addsubanchor",
        "/wxaapi/broadcast/room/modifysubanchor",
        "/wxaapi/broadcast/room/deletesubanchor",
        "/wxaapi/broadcast/room/getsubanchor",
        "/wxaapi/broadcast/room/updatefeedpublic",
        "/wxaapi/broadcast/room/updatereplay",
        "/wxaapi/broadcast/room/updatekf",
        "/wxaapi/broadcast/room/updatecomment",
        "/wxaapi/broadcast/room/addgoods",
        "/wxaapi/broadcast/goods/add",
        "/wxaapi/broadcast/goods/update",
        "/wxaapi/broadcast/goods/audit",
        "/wxaapi/broadcast/goods/resetaudit",
        "/wxaapi/broadcast/goods/delete",
        "/wxaapi/broadcast/goods/getapproved",
        "/wxaapi/broadcast/goods/deleteInRoom",
        "/wxaapi/broadcast/goods/push",
        "/wxaapi/broadcast/goods/onsale",
        "/wxaapi/broadcast/goods/sort",
        "/wxaapi/broadcast/goods/getVideo",
        "/wxaapi/broadcast/goods/getkey",
        "/wxaapi/broadcast/goods/setkey",
        "/wxaapi/broadcast/role/addrole",
        "/wxaapi/broadcast/role/deleterole",
        "/wxaapi/broadcast/role/getrolelist",
    };

    /// <summary>承载上述 34 条在册路由的六个直播接口文件（文件级豁免；其它文件出现该前缀仍判违规）。</summary>
    private static readonly string[] LiveWhitelistedFiles =
    {
        "IWxaLiveRoomService.cs",
        "IWxaLiveAssistantService.cs",
        "IWxaLiveRoomGoodsService.cs",
        "IWxaLiveGoodsService.cs",
        "IWxaLiveRoleService.cs",
        "IWxaLiveSubscribeService.cs",
    };

    /// <summary>路由字面量的终止符（用于从代码行截出完整路由）。</summary>
    private static readonly char[] RouteTerminators = { '"', '\'', ' ', ';', ')' };

    /// <summary>
    /// P3-R1：P3 点名的小程序侧入口<b>不得被建模</b>
    /// （小商店<b>店铺 / 商品管理面</b>已下线；直播 2026-10-13 依官方 38 页取证<b>解除</b>后按清单在册；
    /// 云开发与第三方平台按下方 P3-R2 另判）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 机械化判定走<b>源码扫描</b>而非只看路由：这些入口若被「补」进来，通常会先以
    /// 常量 / 注释 / 半成品路由的形式出现，只看已注册路由会漏掉早期形态。
    /// </para>
    /// <para>
    /// <b>minishop 与 broadcast 两条均按清单在册</b>（2026-10-13，理由见类头第 1 / 2 项）：
    /// 文件级豁免 + 路由清单等式双闸 —— 店铺 / 商品面回潮即红，在册清单被删改亦红。
    /// 两侧「多一条即红」与「少一条即红」同时成立，故收窄不会留下绕开路径。
    /// </para>
    /// </remarks>
    [Fact]
    public void P3EntryPoints_ShouldStayUnmodeled()
    {
        var root = SourcePath("Mud.Wechat.MiniProgram");
        var sources = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        sources.Should().NotBeEmpty("防「扫描口径失效导致白名单真空」的静默空跑");

        var minishopOffenders = OffendingFiles(sources, "wxaapi/minishop")
            .Except(MinishopWhitelistedFiles, StringComparer.Ordinal)
            .ToArray();
        minishopOffenders.Should().BeEmpty(
            "官方原文「目前「小商店」已下线，请直接前往微信小店」⇒ 小商店的**店铺 / 商品管理面**不得建模；"
            + "白名单只放官方 2026-10-13 仍在小程序目录在册的投诉处理 5 条（详见本用例 ② 的清单断言）："
            + string.Join(", ", minishopOffenders));

        // ② 收窄后的精确清单：minishop 前缀的**代码行**路由必须**逐条**等于官方在册的 5 条投诉处理路由。
        //    清单式断言自带正对照（白名单本身即命中项），扫描口径一旦漂移会因「一条都找不到」而假红，
        //    而店铺 / 商品面回潮则因「多出一条」而红 —— 两个方向都关得住。
        var minishopRoutes = RoutesInCodeLines(sources, "/wxaapi/minishop");
        minishopRoutes.Should().NotBeEmpty(
            "minishop 前缀一条路由都没扫到：扫描根或路由写法漂移，本裁决会静默恒真");
        minishopRoutes.Should().BeEquivalentTo(MinishopComplaintRoutes,
            because: "/wxaapi/minishop/* 前缀为历史遗留命名，官方已把投诉处理 5 端点整族迁入小程序 "
                     + "transaction-guarantee/complaint 章节（逐页适用范围「小程序」账号类型、令牌 access_token、"
                     + "权限集 76）⇒ 这 5 条在册；其余 minishop 面（店铺 / 商品 / 交易组件管理）随小商店下线**零声明**");

        // ③ broadcast 一条按清单在册（2026-10-13 解除，理由见类头第 2 项）：双闸形态与 ① / ② 完全一致。
        var broadcastOffenders = OffendingFiles(sources, "wxaapi/broadcast")
            .Except(LiveWhitelistedFiles, StringComparer.Ordinal)
            .ToArray();
        broadcastOffenders.Should().BeEmpty(
            "直播的**服务端 API** 面官方逐页在册（权限集 52 + 服务器端调用 + access_token），"
            + "但清单外的 broadcast 路由不得凭空建模 —— 停止接入的是「直播插件」前端组件页，不是本域；"
            + "白名单只放官方 livebroadcast 分区在册的 34 条（详见 ④ 的清单断言）："
            + string.Join(", ", broadcastOffenders));

        // ④ broadcast 前缀的**代码行**路由必须**逐条**等于官方在册的 34 条（正对照内建于清单本身）。
        var broadcastRoutes = RoutesInCodeLines(sources, "/wxaapi/broadcast");
        broadcastRoutes.Should().NotBeEmpty(
            "broadcast 前缀一条路由都没扫到：扫描根或路由写法漂移，本裁决会静默恒真");
        broadcastRoutes.Should().BeEquivalentTo(LiveBroadcastRoutes,
            because: "官方 OpenApiDoc/livebroadcast 分区（2026-10-13 逐页取证）的服务端路由全集为 34 条 "
                     + "broadcast + 4 条 /wxa/business/*；本清单只锁 broadcast 前缀，4 条 business 由 "
                     + "WxaLiveContractGuards（LB1）与小程序线整体路由守卫承担");

        // ⑤ 在册清单的规模自证：34 + 4 = 38，与官方分区页数一致（防「清单抄漏」被 ④ 的等式掩盖）。
        (LiveBroadcastRoutes.Length + 4).Should().Be(38,
            "官方 livebroadcast 分区 38 页 = broadcast 34 + business 4（getliveinfo / getgoodswarehouse / "
            + "get_wxa_followers / push_message）；任一档数量变化须先重新逐页核验官方目录树");
    }

    /// <summary>
    /// P3-R2：小程序线<b>不得</b>引入开放平台令牌链
    /// （<c>component_access_token</c> / <c>authorizer_access_token</c> / <c>component_verify_ticket</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>这条是本组里最硬的一条（防令牌串号）</b>：设计方案明文要求第三方平台令牌链
    /// 「须独立令牌管理器，<b>不塞进小程序基础线</b>」—— 因为小程序线<b>复用</b>公众号线的令牌域
    /// （同一 <c>MpTokenTypes.AccessToken</c>、同一单槽 <c>Resolve</c>），
    /// 一旦把 component 令牌塞进来，两种令牌会共用同一把槽 ⇒ 互相覆盖、
    /// <b>errCode 自愈静默失效</b>，且表现为「偶发 40001」这种最难定位的形态。
    /// </para>
    /// <para>
    /// 将来要做第三方平台，正确落点是一条<b>独立产品线 + 独立令牌管理器</b>
    /// （参照企微线已存在的多级令牌形态：<c>IWechatProviderTokenManager</c> /
    /// <c>IWechatSuiteTokenManager</c> / <c>WechatTokenRouting</c> 的归属单源做法），
    /// <b>不是</b>往小程序线里加接口。
    /// </para>
    /// </remarks>
    [Fact]
    public void MiniProgramLine_ShouldNotIntroduceComponentTokenChain()
    {
        // ① 源码层面：不得出现开放平台令牌字样。
        var root = SourcePath("Mud.Wechat.MiniProgram");
        var sources = Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        foreach (var token in new[] { "component_access_token", "authorizer_access_token", "component_verify_ticket" })
        {
            OffendingFiles(sources, token).Should().BeEmpty(
                $"小程序线不得引用开放平台令牌 {token}（设计方案红线：须独立令牌管理器，避免令牌串号）");
        }

        // ② 类型层面：小程序线不得出现 component / authorizer 令牌管理器实现。
        typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsPublic && !t.IsNested)
            .Select(static t => t.Name)
            .Where(static n => n.Contains("ComponentToken", StringComparison.OrdinalIgnoreCase)
                               || n.Contains("AuthorizerToken", StringComparison.OrdinalIgnoreCase))
            .Should().BeEmpty("第三方平台令牌管理器属**独立产品线**，不得落在小程序线内");

        // ③ 令牌注入仍必须是自建形态的单级 access_token（与 MP-X2 同口径的定点复述）。
        typeof(MiniProgramServiceBuilder).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false).Any(static a => a.GetType().Name == "TokenAttribute"))
            .Select(static t => t.GetCustomAttributes(false).Single(static a => a.GetType().Name == "TokenAttribute"))
            .Select(static a => a.GetType().GetProperty("TokenType")!.GetValue(a))
            .Should().OnlyContain(t => Equals(t, MpTokenTypes.AccessToken),
                "小程序线所有带令牌接口都复用公众号 access_token，不得新增令牌类型");
    }

    // ---- helpers -------------------------------------------------------------

    /// <summary>
    /// 找出<b>代码行</b>里出现 <paramref name="needle"/> 的文件。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>只在非注释行里找，是刻意的</b>：本线多处文件以注释<b>声明边界</b>
    /// （例如「第三方平台模式下需 authorizer_access_token，本线不支持」）——
    /// 这类<b>文档化边界</b>必须保留，也<b>不应</b>被判为违规；真正的红线是把它写成<b>代码</b>
    /// （引用、常量、注入名）。故本辅助方法先剥掉 <c>//</c> / <c>///</c> / <c>*</c> / <c>/*</c> 开头的行。
    /// </para>
    /// <para>
    /// 本轮实测正是这条区分救了场：首版「整文件包含」的粗口径把
    /// <c>IWxaDataAnalysisService.cs</c> 的边界注释误判为违规而假红 ——
    /// <b>机械判据的粒度过粗，会把正确的东西判错</b>，这是守卫类代码最常见的自伤形态。
    /// </para>
    /// </remarks>
    private static string[] OffendingFiles(string[] sources, string needle)
        => sources
            .Where(f => File.ReadAllLines(f).Any(line => IsCodeLine(line)
                                                         && line.Contains(needle, StringComparison.OrdinalIgnoreCase)))
            .Select(static f => Path.GetFileName(f))
            .ToArray();

    private static bool IsCodeLine(string line)
    {
        var trimmed = line.TrimStart();
        return !(trimmed.StartsWith("//", StringComparison.Ordinal)
                 || trimmed.StartsWith("/*", StringComparison.Ordinal)
                 || trimmed.StartsWith("*", StringComparison.Ordinal));
    }

    /// <summary>
    /// 提取<b>代码行</b>中以 <paramref name="routePrefix"/> 开头的全部路由字面量
    /// （注释行不算，口径同 <see cref="OffendingFiles"/>）。
    /// </summary>
    private static string[] RoutesInCodeLines(string[] sources, string routePrefix)
        => sources
            .SelectMany(static f => File.ReadAllLines(f))
            .Where(line => IsCodeLine(line) && line.Contains(routePrefix, StringComparison.Ordinal))
            .Select(line =>
            {
                var rest = line.Substring(line.IndexOf(routePrefix, StringComparison.Ordinal));
                var cut = rest.IndexOfAny(RouteTerminators);
                return cut < 0 ? rest : rest.Substring(0, cut);
            })
            .ToArray();

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
    /// 解析源工程内路径。源码已归类至 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c>（2026-10 源码归类迁移），
    /// 守卫按 csproj 名称定位工程目录（带缓存），不再硬编码层级 —— 目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
