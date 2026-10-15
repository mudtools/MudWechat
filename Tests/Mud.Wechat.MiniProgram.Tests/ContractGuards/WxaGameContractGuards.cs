// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using Mud.Wechat.MiniProgram.DataModels.Game;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「小游戏（Game）」域契约守卫（GM 系列：20 端点路由与动词 + 令牌与注册面 +
/// 三重凭据参数面 + 官方文档六处缺陷留档与共用类型裁决 + DTO 字段与 JSON 上下文登记 +
/// 跨线路由零交叠 + <c>pay_sig</c> 脱敏登记闭环 + 敏感面不入日志）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 minigame 文档树「服务端 API」全量 20 端点，2026-10 逐页浏览器取证）</b>：
/// 帧同步房间 4 + 用户数据存储 3 + 对局匹配规则 5 + 米大师索要单 1 + 内容安全 2 + 米大师 V2 游戏币 5，
/// 全部由单接口 <see cref="IWxaGameService"/> 承载、共用 <c>Game</c> 注册组。
/// </para>
/// <para>
/// <b>本域的历史形态缺陷（GM 系列即为其收口而设）</b>：本域首批落地时接口<b>没有</b> <c>[Token]</c>
/// 声明、八支强制签名的端点<b>没有</b>签名 Query 参数、若干「查询」端点被写成 GET、
/// 且 DTO 字段名系按语义推断（<c>keys</c> / <c>room_id</c> / <c>player_id</c> 一类）⇒ 官方网关必然拒绝。
/// 现以取证为准重写，形态由本文件逐条锁定。
/// </para>
/// <para>
/// <b>凭据面与本域的关系</b>：官方 20 页的「第三方调用」一节逐页写明<b>不支持第三方平台调用</b>
/// ⇒ 令牌面只有小程序 <c>access_token</c> 一支（对照云开发域恰好相反）。另有 <b>8</b> 支端点在令牌之外
/// 强制宿主计算的签名（数据存储三支 <c>openid</c>+<c>signature</c>+<c>sig_method</c>；
/// pay_v2 五支 <c>signature</c>+<c>sig_method</c>+<c>pay_sig</c>）⇒ 它们是方法级 <c>[Query]</c>
/// 业务参数，<b>不得</b>改做成 <c>[Token]</c> 注入（SDK 不计算也不缓存签名），GM3 逐端点锁定名称集合与必填性。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）、端点聚合计数（MP-X5 /
/// <c>ContractBaseline</c>）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；Query 令牌注入白名单
/// （QT1）由 <c>MpQueryTokenWhitelistGuard</c> 持有；<c>pay_sig</c> 登记面本体由守卫 XP6 锁定，
/// 本文件 GM6 只锁「Game 模块注册委托确实走同一条登记通路」。
/// </para>
/// </remarks>
public class WxaGameContractGuards
{
    private const string RegistryGroupName = "Game";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.Game";

    /// <summary>
    /// 小游戏域官方路由表（20 端点：帧同步 4 + 存储 3 + 匹配 5 + 索要单 1 + 内容安全 2 + pay_v2 5）。
    /// 路由与动词照官方「请求地址」原文，含官方自身「接口英文名与路径不同形」的两处（setmatchopenstate、queryorderinfo）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    [
        (typeof(IWxaGameService), nameof(IWxaGameService.CreateGameRoomAsync), typeof(PostAttribute), "/wxa/createwxagameroom"),
        (typeof(IWxaGameService), nameof(IWxaGameService.GetGameFrameAsync), typeof(PostAttribute), "/wxa/getwxagameframe"),
        (typeof(IWxaGameService), nameof(IWxaGameService.GetGameIdentityInfoAsync), typeof(GetAttribute), "/wxa/getwxagameidentityinfo"),
        (typeof(IWxaGameService), nameof(IWxaGameService.GetGameRoomInfoAsync), typeof(GetAttribute), "/wxa/getwxagameroominfo"),
        (typeof(IWxaGameService), nameof(IWxaGameService.RemoveUserStorageAsync), typeof(PostAttribute), "/wxa/remove_user_storage"),
        (typeof(IWxaGameService), nameof(IWxaGameService.SetUserInteractiveDataAsync), typeof(PostAttribute), "/wxa/setuserinteractivedata"),
        (typeof(IWxaGameService), nameof(IWxaGameService.SetUserStorageAsync), typeof(PostAttribute), "/wxa/set_user_storage"),
        (typeof(IWxaGameService), nameof(IWxaGameService.CreateMatchRuleAsync), typeof(PostAttribute), "/wxa/business/gamematch/creatematchrule"),
        (typeof(IWxaGameService), nameof(IWxaGameService.DeleteMatchRuleAsync), typeof(PostAttribute), "/wxa/business/gamematch/deletematchrule"),
        (typeof(IWxaGameService), nameof(IWxaGameService.GetAllMatchRuleAsync), typeof(PostAttribute), "/wxa/business/gamematch/getallmatchrule"),
        (typeof(IWxaGameService), nameof(IWxaGameService.SetMatchIdOpenStateAsync), typeof(PostAttribute), "/wxa/business/gamematch/setmatchopenstate"),
        (typeof(IWxaGameService), nameof(IWxaGameService.UpdateMatchRuleAsync), typeof(PostAttribute), "/wxa/business/gamematch/updatematchrule"),
        (typeof(IWxaGameService), nameof(IWxaGameService.GetPayForOrderAsync), typeof(PostAttribute), "/wxa/business/getpayfororder"),
        (typeof(IWxaGameService), nameof(IWxaGameService.MsgSecCheckAsync), typeof(PostAttribute), "/wxa/game/content_spam/msg_sec_check"),
        (typeof(IWxaGameService), nameof(IWxaGameService.MediaCheckAsync), typeof(PostAttribute), "/wxa/game/content_spam/media_check_sync"),
        (typeof(IWxaGameService), nameof(IWxaGameService.GetBalanceAsync), typeof(PostAttribute), "/wxa/game/getbalance"),
        (typeof(IWxaGameService), nameof(IWxaGameService.PayAsync), typeof(PostAttribute), "/wxa/game/pay"),
        (typeof(IWxaGameService), nameof(IWxaGameService.CancelPayAsync), typeof(PostAttribute), "/wxa/game/cancelpay"),
        (typeof(IWxaGameService), nameof(IWxaGameService.PresentAsync), typeof(PostAttribute), "/wxa/game/present"),
        (typeof(IWxaGameService), nameof(IWxaGameService.QueryOrderAsync), typeof(PostAttribute), "/wxa/game/queryorderinfo"),
    ];

    /// <summary>官方「请求体为无」的三支 ⇒ 只能有 Query 参数，不得凭空造 request DTO。</summary>
    private static readonly string[] ZeroBodyRoutes =
    [
        "/wxa/getwxagameidentityinfo", "/wxa/getwxagameroominfo", "/wxa/business/gamematch/getallmatchrule",
    ];

    /// <summary>官方以 <c>access_info</c>（createGameRoom 返回的凭证）定位对局的两支 GET。</summary>
    private static readonly string[] AccessInfoRoutes =
    [
        "/wxa/getwxagameidentityinfo", "/wxa/getwxagameroominfo",
    ];

    /// <summary>用户数据存储三支：Query 三件 <c>openid</c> + <c>signature</c> + <c>sig_method</c>（官方全部强制）。</summary>
    private static readonly string[] StorageSignatureRoutes =
    [
        "/wxa/remove_user_storage", "/wxa/setuserinteractivedata", "/wxa/set_user_storage",
    ];

    /// <summary>米大师 V2 五支：Query 三件 <c>signature</c> + <c>sig_method</c> + <c>pay_sig</c>（官方全部强制）。</summary>
    private static readonly string[] PayV2SignatureRoutes =
    [
        "/wxa/game/getbalance", "/wxa/game/pay", "/wxa/game/cancelpay", "/wxa/game/present", "/wxa/game/queryorderinfo",
    ];

    /// <summary>
    /// 契约守卫 GM1：20 端点路由与动词必须与官方「请求地址」逐字一致（三族前缀、动词分布、官方拼写缺陷照抄）。
    /// </summary>
    [Fact]
    public void GameEndpoints_ShouldMatchOfficialRoutesAndVerbs()
    {
        Routes.Should().HaveCount(20,
            "小游戏域官方开放面为 minigame 文档树全量 20 端点（帧同步 4 + 存储 3 + 匹配 5 + 索要单 1 + 内容安全 2 + pay_v2 5）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(20, "路由表不得出现重复条目");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方「请求地址」逐字符一致（照原文，禁止改写）");
            route.Should().StartWith("/wxa/", "本域 20 条路由全部落在 /wxa/ 之下（无 /cgi-bin/ 混入）");
        }

        // 动词分布：2 GET + 18 POST。GET 只有两支 access_info，其余「查询」语义端点官方一律 POST
        //（getwxagameframe / getallmatchrule / getbalance / queryorderinfo / getpayfororder 五支极易被想当然写成 GET）。
        Routes.Count(static r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(2,
            "本域仅 getwxagameidentityinfo / getwxagameroominfo 两支官方调用方式为 GET");
        Routes.Where(static r => r.HttpAttribute != typeof(GetAttribute))
            .Should().OnlyContain(static r => r.HttpAttribute == typeof(PostAttribute),
            "余下 18 支官方一律 POST ⇒ 不得为「名字像查询」的端点擅自改 GET");
        Routes.Should().NotContain(static r => r.HttpAttribute == typeof(PutAttribute) || r.HttpAttribute == typeof(DeleteAttribute),
            "本域无 PUT / DELETE 形态");

        // 三族前缀分布：/wxa/ 直属 7（帧同步 4 + 存储 3）+ /wxa/business/ 6（匹配 5 + 索要单 1）+ /wxa/game/ 7（内容安全 2 + pay_v2 5）。
        Routes.Count(static r => r.Route.StartsWith("/wxa/business/", StringComparison.Ordinal)).Should().Be(6,
            "对局匹配五支与索要单一直属 /wxa/business/ 族");
        Routes.Count(static r => r.Route.StartsWith("/wxa/game/", StringComparison.Ordinal)).Should().Be(7,
            "内容安全两支与米大师 V2 五支同落 /wxa/game/ 族（内容安全再多一层 content_spam 子族）");

        // 反直觉点锁定 1：同族存储三支的路径形态不成对 —— remove_user_storage / set_user_storage 带下划线，
        // setuserinteractivedata 连写。网关按原文路由，「顺手统一」即 404。
        Routes.Select(static r => r.Route).Should()
            .Contain("/wxa/remove_user_storage")
            .And.Contain("/wxa/set_user_storage")
            .And.Contain("/wxa/setuserinteractivedata",
            "官方三支存储路径两种形态并存 ⇒ 照原文，不得归一（set_user_interactive_data 那支官方不存在）");
        Routes.Should().NotContain(static r => r.Route == "/wxa/set_user_interactive_data");

        // 反直觉点锁定 2：接口英文名与请求地址不同形的两支 —— setMatchIdOpenState → setmatchopenstate、
        // pay_v2.queryOrder → queryorderinfo ⇒ 按请求地址落地，并按英文名拼路径的两支必须零声明。
        Routes.Select(static r => r.Route).Should().Contain("/wxa/business/gamematch/setmatchopenstate");
        Routes.Should().NotContain(static r => r.Route == "/wxa/business/gamematch/setmatchidopenstate",
            "按接口英文名 setMatchIdOpenState 拼出的路径官方不存在");
        Routes.Should().NotContain(static r => r.Route == "/wxa/game/queryorder",
            "按英文名 pay_v2.queryOrder 拼出的路径官方不存在，实际是 queryorderinfo");

        // 反直觉点锁定 3：帧同步两支「读」接口的请求体走 Body 而非 Query（getwxagameframe 是 POST + JSON 体）。
        Routes.Select(static r => r.Route).Should().Contain("/wxa/getwxagameframe");
    }

    /// <summary>
    /// 契约守卫 GM2：令牌绑定与注册形态 —— 复用小程序 <c>access_token</c>（Query 注入）、
    /// 单接口单注册组、三段式注册面成立、方法数与路由表自证；本域<b>无</b>第三方代调用形态。
    /// </summary>
    [Fact]
    public void GameInterface_ShouldBindMpAccessToken_AndBeRegisteredInThreeStage()
    {
        var token = typeof(IWxaGameService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("本域 20 端点全部消费小程序 access_token ⇒ 必须接口级 [Token]（首批落地时缺此项，属本域最重缺陷）");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
            "小游戏域必须复用公众号/小程序同域令牌 Wechat.Mp.AccessToken（新增令牌类型会让 MpTokenManagerRegistry 单槽 Resolve 返回 null ⇒ errcode 自愈静默失效）");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3 / QT1）");
        token.Name.Should().Be("access_token", "Query 参数名须照官方原文");

        var api = typeof(IWxaGameService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(RegistryGroupName, "本域单接口挂 Game 注册组（一支注册器装载 20 端点）");
        api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");

        typeof(IWxaGameService).GetCustomAttribute<AllowAnyStatusCodeAttribute>()
            .Should().NotBeNull("本域端点回 errcode/errmsg 非 200 语义体 ⇒ 须允许任意状态码（线内一致形态）");

        MethodCount(typeof(IWxaGameService)).Should().Be(Routes.Length,
            "IWxaGameService 声明方法总数须与 GM1 路由表一致（防「加了端点忘了进路由表」）");

        // 三段式注册面：枚举成员 ↔ AddGameApi() ↔ 源生成注册组同批成立。
        Enum.IsDefined(typeof(MiniProgramModule), MiniProgramModule.Game).Should().BeTrue(
            "MiniProgramModule.Game 必须存在（模块枚举 ↔ 注册入口 ↔ 源生成注册组三段式）");
        typeof(MiniProgramServiceBuilder).GetMethod(nameof(MiniProgramServiceBuilder.AddGameApi))
            .Should().NotBeNull("AddGameApi() 必须存在，否则本域 20 端点无装配入口");

        // 无第三方代调用形态：官方 20 页「第三方调用」逐页写明不支持 ⇒ 本域不得声明 component 令牌。
        typeof(IWxaGameService)
            .GetCustomAttributes().Where(static a => a.GetType().Name == "TokenAttribute")
            .Should().HaveCount(1, "本域只有一个令牌面（access_token），不得为「代调用」再声明第二支 [Token]");
    }

    /// <summary>
    /// 契约守卫 GM3：<b>凭据参数面逐端点锁定</b> —— 两支 <c>access_info</c> 定位、三支存储签名、
    /// 五支 pay_v2 三签，其余十支零 Query；同时锁定请求体面（17 支恰一个 <c>[Body]</c>、3 支零 <c>[Body]</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 该面写错的症状是「签名校验失败」（<c>90010</c> / <c>90011</c>）或与官方参数表不符，
    /// 与「签名算错」几乎无法区分 ⇒ 不许靠调用方试错发现，故逐端点钉死。
    /// </para>
    /// <para>
    /// <b>三类清单须无重叠且全覆盖</b>：GM3 用「并集 == 路由表全集」自证，新增端点漏归类即红。
    /// </para>
    /// </remarks>
    [Fact]
    public void GameEndpoints_ShouldLockCredentialAndBodyFace()
    {
        foreach (var (iface, method, _, route) in Routes)
        {
            var parameters = iface.GetMethod(method)!.GetParameters();
            var queryNames = parameters
                .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
                .Where(static n => !string.IsNullOrEmpty(n))
                .Select(static n => n!)
                .ToList();

            var bodyCount = parameters.Count(static p => p.GetCustomAttribute<BodyAttribute>() != null);

            if (AccessInfoRoutes.Contains(route))
            {
                queryNames.Should().Equal(new[] { "access_info" },
                    $"{route} 官方请求体为「无」⇒ 只有 Query 的 access_info（不得为它造 request DTO）");
                bodyCount.Should().Be(0, $"{route} 官方请求体表写「无」");
            }
            else if (StorageSignatureRoutes.Contains(route))
            {
                queryNames.Should().Equal(new[] { "openid", "signature", "sig_method" },
                    $"{route} 官方强制三件 Query（用户态签名由宿主以 session_key 计算，SDK 不生成）");
                bodyCount.Should().Be(1, $"{route} 官方请求体为 JSON 对象 ⇒ 恰有一个 [Body] 参数");
            }
            else if (PayV2SignatureRoutes.Contains(route))
            {
                queryNames.Should().Equal(new[] { "signature", "sig_method", "pay_sig" },
                    $"{route} 官方米大师 V2 强制三签 Query（pay_sig 基串含 uri，缺一支即 90010/90011）");
                bodyCount.Should().Be(1, $"{route} 官方请求体为 JSON 对象 ⇒ 恰有一个 [Body] 参数");
            }
            else if (ZeroBodyRoutes.Contains(route))
            {
                // getallmatchrule：官方「请求体为无」且除 access_token 外零 Query 凭据 ⇒ 接口面不得有任何参数
                //（既不得造 request DTO，也不得挂签名参数）。
                queryNames.Should().BeEmpty($"{route} 官方除 access_token 外无任何 Query 凭据参数");
                bodyCount.Should().Be(0, $"{route} 官方请求体为「无」⇒ 不得凭空造 request DTO");
            }
            else
            {
                queryNames.Should().BeEmpty($"{route} 官方除 access_token 外无任何 Query 凭据参数");
                bodyCount.Should().Be(1, $"{route} 官方请求体为 JSON 对象 ⇒ 恰有一个 [Body] 参数");
            }

            // 签名类参数一律必填：官方全部标注必填 ⇒ 不得写成带默认值的可选参数。
            foreach (var parameter in parameters.Where(static p => p.GetCustomAttribute<QueryAttribute>() != null))
            {
                parameter.ParameterType.Should().Be(typeof(string),
                    $"{route} 的 Query 参数 {parameter.GetCustomAttribute<QueryAttribute>()!.Name} 官方是标量（字符串）");
                parameter.IsOptional.Should().BeFalse(
                    $"{route} 的 Query 凭据参数官方必填 ⇒ 不得给默认值（漏传即签名校验失败，与算法错误难以区分）");
            }
        }

        // 零请求体三支与「Body 计数」双向自证：清单 ⇔ 反射实测的零 [Body] 端点全集。
        ZeroBodyRoutes.Should().HaveCount(3, "官方请求体为「无」的是两支 access_info + 一支 getallmatchrule");
        Routes.Count(static r => !ZeroBodyRoutes.Contains(r.Route)).Should().Be(17,
            "本域 20 支里 17 支带 JSON 请求体（与 GM2 的方法数自证、GM1 的路由表规模互为交叉验证）");

        var routesWithoutBody = Routes
            .Where(static r => r.Interface
                .GetMethod(r.Method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
                .GetParameters().Count(static p => p.GetCustomAttribute<BodyAttribute>() != null) == 0)
            .Select(static r => r.Route)
            .ToArray();

        ZeroBodyRoutes.Should().BeEquivalentTo(routesWithoutBody,
            "零请求体清单须恰为接口面上真的没有 [Body] 的端点全集（3 支）⇒ 新增「请求体为无」端点漏归类时本条先红");

        // 全覆盖与不交叉自证：三支凭据清单两两不交，且并集恰等于「接口面上真的带 Query 参数的端点」全集。
        var credentialRoutes = AccessInfoRoutes.Concat(StorageSignatureRoutes).Concat(PayV2SignatureRoutes).ToArray();
        credentialRoutes.Distinct().Should().HaveCount(credentialRoutes.Length,
            "三支凭据清单不得交叉（同一端点被归两档即分类失效，逐端点分支会走错）");

        var routesWithQueryParameter = Routes
            .Where(static r => r.Interface
                .GetMethod(r.Method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
                .GetParameters().Any(static p => p.GetCustomAttribute<QueryAttribute>() != null))
            .Select(static r => r.Route)
            .ToArray();

        credentialRoutes.Should().BeEquivalentTo(routesWithQueryParameter,
            "凭据清单的并集须恰为带 Query 参数的端点全集（10 支）⇒ 新增端点漏归类时本条先红，不靠记忆兜");

        AccessInfoRoutes.Should().HaveCount(2, "帧同步两支 GET 以 access_info 定位对局");
        StorageSignatureRoutes.Should().HaveCount(3, "用户数据存储三支（remove / setuserinteractivedata / set）");
        PayV2SignatureRoutes.Should().HaveCount(5, "米大师 V2 五支（getbalance / pay / cancelpay / present / queryorderinfo）");

        // 签名算法取值的唯一性：官方 sig_method 只有 hmac_sha256 一支。
        WxaSignatureMethods.HmacSha256.Should().Be("hmac_sha256", "官方 sig_method 唯一取值原文");
    }

    /// <summary>
    /// 契约守卫 GM4：官方文档六处缺陷/矛盾的<b>留档面</b> + 三处「共用类型」裁决 + 官方字段名与类型抽样。
    /// </summary>
    /// <remarks>
    /// 留档面用<b>接口源码文本</b>断言：这些裁决只存在于 XML 注释里，若注释被删（信息丢失），
    /// 反射面看不出来 ⇒ 必须以文本扫描兜住（与 TCB / XP 系列对「官方空缺登记」的处理同形）。
    /// </remarks>
    [Fact]
    public void GameOfficialDocDefectsAndSharedTypeRulings_ShouldStayRegistered()
    {
        var source = File.ReadAllText(Path.Combine(GetSolutionRoot(),
            "Src/MiniProgram/Mud.Wechat.MiniProgram/Interfaces/Game/IWxaGameService.cs"));

        // 六处官方缺陷留档（逐条须能在源码文本里找到锚点）。
        source.Should().Contain("setmatchopenstate", "官方缺陷①：接口英文名 setMatchIdOpenState 与实际路径不同形");
        source.Should().Contain("need_room_service_info", "官方缺陷②：getAllMatchRule 返回表把该字段误写成第二个 team_member_count");
        source.Should().Contain("number", "官方缺陷③：match_id 类型列标 number 而示例是字符串");
        source.Should().Contain("不臆造", "官方缺陷④⑤⑥ 的总处置原则（updateMatchRule 参数表复用、请求体表重复行、废弃但在册）");
        source.Should().Contain("废弃", "官方缺陷⑥：getPayForOrder 页面标注已废弃但在册，须留档该裁决");
        source.Should().Contain("1 万/分钟", "本域仅有的端点级频率数字一：文本内容安全 1 万/分钟、1000 万/天");
        source.Should().Contain("2 千/分钟", "本域仅有的端点级频率数字二：图片内容安全 2 千/分钟、20 万/天");

        // —— 三处「共用类型」裁决（防「顺手拆分」造成双份维护，也防「过度合并」丢语义）——
        RequestTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.SetMatchIdOpenStateAsync))
            .Should().Be(RequestTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.UpdateMatchRuleAsync)),
            "官方 updateMatchRule 页参数表只有 match_id + open_state 两支 ⇒ 与状态页共用 GameMatchOpenStateRequest（不臆造未出现的参数）");
        RequestTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.SetMatchIdOpenStateAsync))
            .Should().Be(typeof(GameMatchOpenStateRequest), "共用类型即 GameMatchOpenStateRequest");

        var matchRuleIdResults = new[]
        {
            nameof(IWxaGameService.CreateMatchRuleAsync),
            nameof(IWxaGameService.SetMatchIdOpenStateAsync),
            nameof(IWxaGameService.UpdateMatchRuleAsync),
        };
        matchRuleIdResults.Select(m => ResultTypeOf(typeof(IWxaGameService), m)).Distinct()
            .Should().Equal(new[] { typeof(GameMatchRuleIdResponse) },
            "三支官方应答体均只有 match_id（+ errcode/errmsg）⇒ 共用 GameMatchRuleIdResponse");

        ResultTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.PayAsync))
            .Should().Be(ResultTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.CancelPayAsync)),
            "官方 cancelpay 应答表是 pay 页的复制件（含「扣款后的余额」措辞）⇒ 共用 GamePayResponse");
        ResultTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.PresentAsync))
            .Should().Be(typeof(GamePresentResponse),
            "present 应答只有 bill_no + balance（无 used_present_amount）⇒ 单独建模，不得与前两支合并");

        // 存储两支请求形同（kv_list）但类型独立：路由与语义不同，不得互相替代。
        RequestTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.SetUserStorageAsync))
            .Should().NotBe(RequestTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.SetUserInteractiveDataAsync)),
            "set_user_storage 与 setuserinteractivedata 请求体同形但语义不同 ⇒ 两支各自建模");

        // —— 字段名 / 类型抽样（照官方原文，含官方自身标注冲突的处置）——
        AssertJsonProperty<GameRemoveUserStorageRequest>(nameof(GameRemoveUserStorageRequest.Key), "key",
            "官方参数名是单数 key、类型为字符串数组（不是 keys）");
        AssertPropertyType<GameRemoveUserStorageRequest>(nameof(GameRemoveUserStorageRequest.Key), typeof(List<string>),
            "官方请求示例 [\"gold\", \"score\"]");

        AssertJsonProperty<GameDeleteMatchRuleRequest>(nameof(GameDeleteMatchRuleRequest.MatchId), "match_id", "官方原文");
        AssertPropertyType<GameDeleteMatchRuleRequest>(nameof(GameDeleteMatchRuleRequest.MatchId), typeof(string),
            "官方类型列标 number、请求示例是 44 位字符串 ⇒ 按线格式取 string（GM4 缺陷③）");
        AssertPropertyType<GameMatchOpenStateRequest>(nameof(GameMatchOpenStateRequest.MatchId), typeof(string),
            "同上：两支共用请求类型的 match_id 取 string");

        AssertJsonProperty<GameGetAllMatchRuleResponseMatchRuleListItem>(
            nameof(GameGetAllMatchRuleResponseMatchRuleListItem.NeedRoomServiceInfo), "need_room_service_info",
            "官方返回表第二行 team_member_count 的描述实为该字段、返回示例亦用前者 ⇒ 取真实字段名（GM4 缺陷②）");

        AssertJsonProperty<GameGetPayForOrderResponse>(nameof(GameGetPayForOrderResponse.Payfororder), "payfororder",
            "官方应答外层键即全小写 payfororder（不是 PayForOrder）");

        AssertJsonProperty<GameGetFrameResponseDataFrameListItemPkgListItem>(
            nameof(GameGetFrameResponseDataFrameListItemPkgListItem.B), "b",
            "官方空帧标记原文即单字母 b");
        AssertJsonProperty<GameGetFrameResponseDataFrameListItemPkgListItem>(
            nameof(GameGetFrameResponseDataFrameListItemPkgListItem.DpId), "dp_id",
            "官方明写「无需理解，为了更快速返回数据没有去掉该字段内容」⇒ 照原文保留，不得据其做业务判断");

        AssertPropertyType<GameSetUserStorageRequestKvListItem>(nameof(GameSetUserStorageRequestKvListItem.Value),
            typeof(JsonElement?),
            "官方参数表标 number，而排行榜形态要求 value 是含 wxgame.score / wxgame.update_time 的 JSON 串 ⇒ 取 JsonElement?");
        AssertPropertyType<GameSetUserInteractiveDataRequestKvListItem>(
            nameof(GameSetUserInteractiveDataRequestKvListItem.Value), typeof(long?),
            "互动数据的 value 官方明确是数值 ⇒ 与上报用户数据那支不得共用类型");

        AssertJsonProperty<GameMsgSecCheckResult>(nameof(GameMsgSecCheckResult.ReplacedContent), "replaced_content",
            "文本支独有（图片支应答无该字段）");
        AssertJsonProperty<GameMsgSecCheckDetailItem>(nameof(GameMsgSecCheckDetailItem.Keyword), "keyword",
            "文本支明细项含 keyword，图片支明细项无 ⇒ 两支各持类型不共用");
        AssertJsonProperty<GameMediaCheckRequest>(nameof(GameMediaCheckRequest.MediaType), "media_type",
            "官方 media_type 固定 2（图片），非法回 750034");

        // 图片支与文本支应答的字段差集是官方参数表的真实差异，不得合并成一套类型。
        RequestTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.MsgSecCheckAsync))
            .Should().NotBe(RequestTypeOf(typeof(IWxaGameService), nameof(IWxaGameService.MediaCheckAsync)),
            "两支请求字段集不同（content+nickname / media_url+media_type）⇒ 类型不共用");
    }

    /// <summary>
    /// 契约守卫 GM5：46 个 DTO 全量登记进 <c>GameJsonContext</c>、SerializerClassName 恒为 Game、
    /// 且上下文已合并进主包序列化管线（Native AOT 生死线）。
    /// </summary>
    [Fact]
    public void GameDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(GameCreateRoomRequest).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("Game 域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 46 = 帧同步 14 + 存储 5 + 匹配 7 + 米大师索要单 3 + pay_v2 12 + 内容安全 8（一类型一文件，子目录仅作组织）。
        dtoTypes.Should().HaveCount(46,
            "小游戏域 20 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方参数表与应答表再同批调整本守卫");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Game（= 命名空间域段）");
        }

        // 本域无常量类（官方枚举值一律写在 XML 注释里，不建 *Statuses 表）⇒ 域内类型 = 46 DTO + 生成上下文本体。
        // 上下文是生成器落在同一命名空间的 abstract 类型，不属 DTO 面，须显式排除而非放进度量。
        domainTypes.Where(static t => !t.Name.EndsWith("JsonContext", StringComparison.Ordinal))
            .Should().BeEquivalentTo(dtoTypes,
            "Game 域命名空间内除 DTO 与 GameJsonContext 不得有其他类型（若新增常量类须同批在本守卫登记，形态同 XP5）");

        domainTypes.Where(static t => t.Name.EndsWith("JsonContext", StringComparison.Ordinal))
            .Select(static t => t.Name).Should().Equal(new[] { "GameJsonContext" },
            "正对照：命名空间内被排除的非 DTO 类型只可能是 GameJsonContext 一支（排除口径跑偏会让上一条恒真）");

        var contextType = assembly.GetType(DtoNamespace + ".GameJsonContext");
        contextType.Should().NotBeNull(
            "GameJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"Game\")]，"
                + "必须登记进 GameJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        var mergeSource = File.ReadAllText(Path.Combine(GetSolutionRoot(),
            "Src/MiniProgram/Mud.Wechat.MiniProgram/Extensions/MiniProgramJsonResolverExtensions.cs"));

        var mergedContextNames = Regex
            .Matches(mergeSource, @"([A-Za-z]+JsonContext)\.Default")
            .Select(static m => m.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        mergedContextNames.Should().Contain("GameJsonContext",
            "GameJsonContext 未合并进组件序列化管线 ⇒ 本域 46 DTO 在 Native AOT 下无生成器路径（正对照：XPay/CloudBase 两支在册）");
        mergedContextNames.Should().Contain("XPayJsonContext").And.Contain("CloudBaseJsonContext",
            "正对照：合并清单确实被读到（读空文件或正则跑偏时上一条会假绿）");
    }

    /// <summary>
    /// 契约守卫 GM6：<b>本域 <c>pay_sig</c> 与 XPay 共用同一登记通路</b> ——
    /// <c>AddGameApi()</c> 的注册委托必须先 <c>XPaySensitiveQueryKeys.RegisterAll()</c> 再装载客户端。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>pay_sig</c> 不在组件精确匹配脱敏词表内（XP6 已锁登记面本体）。本域米大师 V2 五支同样传
    /// <c>pay_sig</c>，而进程级登记只在模块注册委托里发生 ⇒ <b>只注册 Game 不注册 XPay 的宿主</b>
    /// 若走的是 <c>static s => s.AddGameWebApiHttpClient()</c>，支付签名会随
    /// <c>ApiException.RequestUri</c> / 遥测 URL 明文外泄。
    /// </para>
    /// <para>
    /// <b>为何文本断言与运行期断言都要</b>：xunit 同进程内 XP6 可能先跑并已完成登记，
    /// 纯运行期断言在「Game 委托漏登记」时仍会绿（键已被别的模块登记）⇒ 委托体文本是必要条件，
    /// 运行期掩码是充分条件。登记面本体（键集与词表不交叉）不在本条重复，仍由 XP6 锁定。
    /// </para>
    /// </remarks>
    [Fact]
    public void GameModuleRegistration_ShouldReusePaySigRegistrationPath()
    {
        var builderSource = File.ReadAllText(Path.Combine(GetSolutionRoot(),
            "Src/MiniProgram/Mud.Wechat.MiniProgram/Extensions/MiniProgramServiceBuilder.cs"));

        var gameEntry = ModuleEntryOf(builderSource, MiniProgramModule.Game);
        var payEntry = ModuleEntryOf(builderSource, MiniProgramModule.XPay);

        gameEntry.Should().NotBeNull("MiniProgramServiceBuilder 必须含 [MiniProgramModule.Game] 注册条目");
        payEntry.Should().NotBeNull("正对照：XPay 条目必须能被同一定位器取到（定位器失效时下一条断言会恒真）");

        var registerCall = gameEntry!.IndexOf("XPaySensitiveQueryKeys.RegisterAll()", StringComparison.Ordinal);
        registerCall.Should().BeGreaterThanOrEqualTo(0,
            "Game 注册委托必须复用 XPaySensitiveQueryKeys.RegisterAll()（否则只注册 Game 的宿主会把 pay_sig 明文留在 URL 里）");

        var loadCall = gameEntry.IndexOf("AddGameWebApiHttpClient()", StringComparison.Ordinal);
        loadCall.Should().BeGreaterThanOrEqualTo(0, "Game 委托须装载本域客户端");
        registerCall.Should().BeLessThan(loadCall,
            "登记必须先于装载（顺序颠倒时首次请求仍可能落进未登记的窗口）");

        // 运行期充分条件：真实装配路径（AddMpApp + AddGameApi）后，pay_sig 必须已被掩码。
        var services = new ServiceCollection();
        services.AddMpApp(static c =>
        {
            c.AppKey = "game-guard";
            c.AppId = "wx-guard";
            c.AppSecret = "secret-guard";
        });

        services.AddMiniProgramServices(static b => b.AddGameApi());

        var redactor = typeof(Mud.HttpUtils.ApiException).Assembly.GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（GM6 功能断言依赖其 Redact 通路）");

        var redact = redactor!.GetMethod("Redact",
            BindingFlags.Public | BindingFlags.Static, binder: null, new[] { typeof(string) }, modifiers: null);
        redact.Should().NotBeNull("组件 SensitiveUrlRedactor.Redact(string) 必须存在");

        const string Url = "https://api.weixin.qq.com/wxa/game/pay?access_token=AT&signature=SU&sig_method=hmac_sha256&pay_sig=PS&foo=bar";
        var redacted = redact!.Invoke(null, new object?[] { Url }) as string;

        redacted.Should().NotBeNull();
        redacted.Should().Contain("pay_sig=***REDACTED***",
            "AddGameApi 之后 pay_sig 必须被强制掩码（米大师 V2 支付签名不得随 URL 明文输出）");
        redacted.Should().Contain("signature=***REDACTED***",
            "用户态签名由组件静态词表覆盖（同 URL 一并掩码，本线刻意不重复登记）");
        redacted.Should().Contain("sig_method=hmac_sha256",
            "sig_method 是算法名而非凭据（官方唯一取值 hmac_sha256）⇒ 保留原文以利排障");
        redacted.Should().Contain("foo=bar", "词表与登记面之外的参数保留原文（精确匹配语义）");

        // 装配面自证：Game 模块注册后 IWxaGameService 可解析。
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });

        provider.GetRequiredService<IWxaGameService>().Should().NotBeNull("IWxaGameService 须随 Game 模块注册");
    }

    /// <summary>
    /// 契约守卫 GM7：跨线零交叠（本域 20 条路由只允许出现在小程序线源码内）+ 敏感面不入日志。
    /// </summary>
    [Fact]
    public void GameRoutes_ShouldBeDeclaredOnlyInMiniProgramLine_AndSensitiveValuesNeverLogged()
    {
        var declared = RouteLiteralsByFile("Src");
        declared.Should().NotBeEmpty("防「扫描口径失效导致真空断言」：扫描根必须能扫出源码内的路由字面量");

        var gameRoutes = Routes.Select(static r => r.Route).ToArray();

        // 正对照（活体证明）：同一扫描必须能扫出本域 20 条路由，否则零交叠断言静默恒真。
        var inMiniProgram = declared
            .Where(kv => kv.Key.Contains("/MiniProgram/", StringComparison.Ordinal))
            .SelectMany(static kv => kv.Value)
            .Where(r => gameRoutes.Contains(r, StringComparer.Ordinal))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        inMiniProgram.Should().BeEquivalentTo(gameRoutes,
            "扫描口径必须能扫出小程序线声明的 20 条小游戏路由（扫不出即扫描根或正则失效，下面的零交叠断言会恒真）");

        var offenders = declared
            .Where(kv => !kv.Key.Contains("/MiniProgram/", StringComparison.Ordinal))
            .Where(kv => kv.Value.Any(r => gameRoutes.Contains(r, StringComparer.Ordinal)))
            .Select(kv => $"{kv.Key} -> {string.Join(", ", kv.Value.Where(r => gameRoutes.Contains(r, StringComparer.Ordinal)))}")
            .ToList();

        offenders.Should().BeEmpty(
            "本域 20 条路由（/wxa/getwxagame* / /wxa/*_storage / /wxa/business/gamematch/* / /wxa/game/*）是"
            + "小程序 access_token 令牌面专属；开放平台线只有 /componenttcb/* 一类代调用面，跨线声明同一路径即制造双份维护与凭据串号");

        // 正对照之二：扫描根覆盖到小程序线以外的源码树。
        declared
            .Where(kv => kv.Key.Contains("/OfficialAccount/", StringComparison.Ordinal))
            .SelectMany(static kv => kv.Value)
            .Should().Contain("/cgi-bin/token",
            "扫描根必须覆盖公众号线源码（只扫到小程序线时，跨线零交叠断言会静默恒真）");

        // 敏感面不入日志：主包源码内真实日志调用语句不得出现签名/凭据/余额靶名。
        var sensitiveTokens = new[] { "pay_sig", "signature", "session_key", "Balance", "BillNo" };

        var files = Directory.GetFiles(SourceProjectDir("Mud.Wechat.MiniProgram"), "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToList();

        files.Should().Contain(static f => Path.GetFileName(f).Equals("IWxaGameService.cs", StringComparison.Ordinal),
            "GM7 的扫描根必须覆盖小游戏接口源文件（定位失效会让日志断言静默恒真）");

        var logOffenders = new List<string>();
        foreach (var file in files)
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                // 剥行内注释：裁决注释会原样写出 pay_sig / signature 等靶名，不排除即假红。
                var code = StripLineComment(lines[i]);
                if (!LogInvocation.IsMatch(code))
                {
                    continue;
                }

                var statement = string.Concat(
                    code,
                    i + 1 < lines.Length ? StripLineComment(lines[i + 1]) : string.Empty,
                    i + 2 < lines.Length ? StripLineComment(lines[i + 2]) : string.Empty);

                var hit = sensitiveTokens.FirstOrDefault(p => statement.Contains(p, StringComparison.Ordinal));
                if (hit is not null)
                {
                    logOffenders.Add($"{Path.GetFileName(file)}:{i + 1}({hit})");
                }
            }
        }

        logOffenders.Should().BeEmpty(
            "本域签名类凭据（pay_sig / signature / session_key）与代币余额、订单号不得出现在任何日志调用中（AGENTS §8）");
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    /// <summary>取接口声明的方法数（DeclaredOnly，不含继承面）。</summary>
    private static int MethodCount(Type type)
        => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length;

    /// <summary>取端点方法的应答类型（<c>Task&lt;T&gt;</c> 解包为 <c>T</c>）。</summary>
    private static Type ResultTypeOf(Type interfaceType, string methodName)
    {
        var method = interfaceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        method.Should().NotBeNull($"{interfaceType.Name}.{methodName} 不存在（契约面漂移）");

        var returnType = method!.ReturnType;
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : returnType;
    }

    /// <summary>取端点方法的 <c>[Body]</c> 请求 DTO 类型（零请求体端点返回 <c>null</c>）。</summary>
    private static Type? RequestTypeOf(Type interfaceType, string methodName)
        => interfaceType
            .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
            .GetParameters()
            .SingleOrDefault(static p => p.GetCustomAttribute<BodyAttribute>() != null)
            ?.ParameterType;

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }

    private static void AssertPropertyType<T>(string propertyName, Type expectedType, string because = "")
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");
        property!.PropertyType.Should().Be(expectedType,
            $"{typeof(T).Name}.{propertyName} 的类型必须是 {expectedType.Name}（共用/口径裁决{(because.Length > 0 ? "：" + because : string.Empty)}）");
    }

    /// <summary>
    /// 定位 <c>MiniProgramServiceBuilder</c> 中某模块的注册委托体（从枚举条目起，到该委托的收尾逗号止）。
    /// 用文本而非反射：委托体是 <c>static</c> lambda，反射看不到其内部调用顺序。
    /// </summary>
    private static string? ModuleEntryOf(string builderSource, MiniProgramModule module)
    {
        var marker = $"[MiniProgramModule.{module}]";
        var start = builderSource.IndexOf(marker, StringComparison.Ordinal);
        if (start < 0)
        {
            return null;
        }

        var end = builderSource.IndexOf("},", start, StringComparison.Ordinal);
        if (end < 0)
        {
            end = builderSource.IndexOf(",", start, StringComparison.Ordinal);
        }

        if (end < 0)
        {
            end = builderSource.Length;
        }

        return builderSource.Substring(start, end - start);
    }

    /// <summary>
    /// 扫描目录下全部源码里的「路由字面量」（剥离注释后取引号包裹且以 <c>/</c> 开头的字符串），
    /// 返回「相对路径 → 字面量列表」。剥离注释是必要的：裁决注释里刻意不实现的路径不算声明。
    /// </summary>
    private static Dictionary<string, List<string>> RouteLiteralsByFile(string relativeRoot)
    {
        var root = Path.Combine(GetSolutionRoot(), relativeRoot);
        var literalRegex = new Regex(@"""(/[A-Za-z0-9_\-./{}]+)""", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);

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

            var hits = literalRegex.Matches(code).Select(static m => m.Groups[1].Value).ToList();
            if (hits.Count > 0)
            {
                map[normalized] = hits;
            }
        }

        return map;
    }

    /// <summary>
    /// 日志调用起点：<c>Log</c> + 级别 + <c>(</c>（级别可省）。只匹配调用形态，
    /// 不用裸 <c>Contains("Log")</c> —— 后者会把类型名里的 Log 判成日志调用，属假红。
    /// </summary>
    private static readonly Regex LogInvocation = new(
        @"\bLog(?:Information|Warning|Error|Debug|Trace|Critical)?\s*\(",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>剥行内与整行注释（<c>///</c> 亦经此路归空）：裁决注释会原样写出靶名，不排除即假红。</summary>
    private static string StripLineComment(string line)
    {
        var quote = 0;
        for (var i = 0; i < line.Length - 1; i++)
        {
            if (line[i] == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                quote ^= 1;
            }
            else if (quote == 0 && line[i] == '/' && line[i + 1] == '/')
            {
                return line[..i];
            }
        }

        return line;
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();

    /// <summary>按工程名定位小程序线源工程目录（避开长路径与硬编码相对层级）。</summary>
    private static string SourceProjectDir(string projectName)
        => SourceProjectDirs.GetOrAdd(projectName, static name =>
        {
            var root = GetSolutionRoot();
            foreach (var candidate in Directory.EnumerateDirectories(Path.Combine(root, "Src")))
            {
                var project = Path.Combine(candidate, name, name + ".csproj");
                if (File.Exists(project))
                {
                    return Path.GetDirectoryName(project)!;
                }
            }

            throw new InvalidOperationException($"未在 Src/ 下找到源工程 {name}.csproj（GM7 扫描根定位失效）");
        });

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
