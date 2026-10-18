// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.Ams;
using Mud.Wechat.OpenPlatform.DataModels.Account;
using Mud.Wechat.OpenPlatform.DataModels.Auth;
using Mud.Wechat.OpenPlatform.DataModels.Category;
using Mud.Wechat.OpenPlatform.DataModels.Component;
using Mud.Wechat.OpenPlatform.DataModels.Embedded;
using Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;
using Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate;
using Mud.Wechat.OpenPlatform.DataModels.OpenAccount;
using Mud.Wechat.OpenPlatform.DataModels.ServiceMarket;
using Mud.Wechat.OpenPlatform.DataModels.Sns;
using Mud.Wechat.OpenPlatform.DataModels.UserOrder;
using Mud.Wechat.OpenPlatform.DataModels.WeData;

namespace Mud.Wechat.OpenPlatform.Tests.ContractGuards;

/// <summary>
/// 开放平台线契约守卫（B1 落地的机械化防线，对齐方案 §3.6.5）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本守卫锁定的五条裁决</b>：① 令牌链 4 端点（<c>api_component_token</c> 等）由 Provider
/// raw HTTP 承载、<b>不得</b>出现声明式第二实现（T2）；② 令牌类型独立键系
/// （<c>Wechat.OpenPlatform.*</c>，不与企微 / 公众号 / 小店串号）；③ 逐接口令牌语义
/// （平台管理面 = component 值入 <c>access_token</c>；网页授权 = component 值入
/// <c>component_access_token</c>；授权方域 = authorizer 值入 <c>access_token</c>；推票引导免令牌）；
/// ④ 跨线路由零交叠；⑤ <c>fastregisterweapp</c> / <c>fastregisterpersonalweapp</c> 同路由双方法以 <c>action</c> 分派。
/// </para>
/// </remarks>
public class OpenPlatformContractGuards
{
    private static readonly Assembly MainAssembly = typeof(IWechatOpenPlatformComponentService).Assembly;

    /// <summary>接口名 →（HTTP 方法, 路由）期望表（<b>域内精确断言</b>，聚合数在 <see cref="Baseline.OpenPlatform"/>）。</summary>
    private static readonly Dictionary<string, (string Method, string Route)[]> ExpectedEndpoints = new()
    {
        [nameof(IWechatOpenPlatformComponentService)] = new[]
        {
            ("POST", "/cgi-bin/component/clear_quota/v2"),
            ("POST", "/cgi-bin/component/fastregisterweapp"),
            ("POST", "/cgi-bin/component/fastregisterweapp"),
            // 管理员换绑：官方路由在 /cgi-bin/account/ 前缀下，但令牌面属平台侧（见 OP-B3 与接口 XML 注释
            // 追踪号 OP-REBINDADMIN-TOKEN-01），故声明于 Component 接口而非 Account 接口。
            ("POST", "/cgi-bin/account/componentrebindadmin"),
            ("POST", "/cgi-bin/component/modify_wxa_server_domain"),
            ("POST", "/cgi-bin/component/modify_wxa_jump_domain"),
            ("POST", "/cgi-bin/component/get_domain_confirmfile"),
            ("POST", "/cgi-bin/component/getprivacysetting"),
            ("POST", "/cgi-bin/component/setprivacysetting"),
            ("POST", "/cgi-bin/component/uploadprivacyextfile"),
            ("POST", "/cgi-bin/component/api_get_authorizer_info"),
            ("POST", "/cgi-bin/component/api_get_authorizer_list"),
            ("POST", "/cgi-bin/component/api_get_authorizer_option"),
            ("POST", "/cgi-bin/component/api_set_authorizer_option"),
            ("POST", "/wxa/component/fastregisterbetaweapp"),
            ("POST", "/wxa/component/fastregisterpersonalweapp"),
            ("POST", "/wxa/component/fastregisterpersonalweapp"),
        },
        [nameof(IWechatOpenPlatformComponentTicketFreeService)] = new[]
        {
            ("POST", "/cgi-bin/component/api_start_push_ticket"),
        },
        [nameof(IWechatOpenPlatformOpenAccountService)] = new[]
        {
            ("POST", "/cgi-bin/open/create"),
            ("POST", "/cgi-bin/open/get"),
            ("POST", "/cgi-bin/open/bind"),
            ("POST", "/cgi-bin/open/unbind"),
            ("POST", "/cgi-bin/open/have"),
            ("GET", "/cgi-bin/open/sameentity"),
        },
        [nameof(IWechatOpenPlatformAccountService)] = new[]
        {
            ("GET", "/cgi-bin/account/getaccountbasicinfo"),
            ("POST", "/cgi-bin/account/modifyheadimage"),
            ("POST", "/cgi-bin/account/modifysignature"),
            ("POST", "/cgi-bin/account/fastregister"),
        },
        [nameof(IWechatOpenPlatformSnsService)] = new[]
        {
            ("GET", "/sns/oauth2/component/access_token"),
            ("GET", "/sns/oauth2/component/refresh_token"),
            ("GET", "/sns/component/jscode2session"),
        },
        [nameof(IWechatOpenPlatformCategoryService)] = new[]
        {
            ("GET", "/cgi-bin/wxopen/getallcategories"),
            ("GET", "/cgi-bin/wxopen/getcategory"),
            ("POST", "/cgi-bin/wxopen/getcategoriesbytype"),
            ("POST", "/cgi-bin/wxopen/addcategory"),
            ("POST", "/cgi-bin/wxopen/deletecategory"),
            ("POST", "/cgi-bin/wxopen/modifycategory"),
            ("POST", "/cgi-bin/wxopen/qrcodejumpdownload"),
            ("GET", "/cgi-bin/wxopen/getweappsupportversion"),
            ("POST", "/cgi-bin/wxopen/setweappsupportversion"),
            ("POST", "/cgi-bin/wxopen/wxamplinkget"),
            ("POST", "/cgi-bin/wxopen/wxamplink"),
            ("POST", "/cgi-bin/wxopen/wxampunlink"),
        },
    };

    /// <summary>
    /// OP-B1：接口集合与数量锁定（30 接口 / 191 端点，防漏防多）。
    /// </summary>
    /// <remarks>
    /// 2026-10-15 起本线另有 <c>ComponentTcb</c> 六支接口（50 端点，注册组共用一个），
    /// 其 (方法, 路由) 表与令牌语义由 <c>OpenPlatformComponentTcbContractGuards</c>（CBT 系列）逐条锁定，
    /// <c>Embedded</c>（6 端点）由 <c>OpenPlatformEmbeddedContractGuards</c>（EMB 系列）、
    /// <c>Auth</c>（5 端点）由 <c>OpenPlatformAuthContractGuards</c>（AUTH 系列）、
    /// <c>Ams</c>（19 端点）+ <c>AmsShare</c>（3 端点）由 <c>OpenPlatformAmsContractGuards</c>（AM 系列）、
    /// <c>MiniProgramManage</c>（授权方令牌面）十二支接口（30-1 代码管理 15 + 名称与搜索 6 + 类目 1；30-2 域名与 DNS 9 + 体验成员 3 + 违规申诉 2 + 订阅组件 3；30-3 ICP 备案 JSON 12 + 隐私接口 3 + 订单路径查询 1；30-4 试用小程序核验 2；30-5 multipart 上传 2；30-7 直播能力开通 1，共 60 端点，注册组共用一个）
    /// 由 <c>OpenPlatformMiniProgramManageContractGuards</c>（MPG 系列）逐条锁定；
    /// <c>MiniProgramTemplate</c>（<c>component_access_token</c> 面）两支接口（模板库 4 + 下单路径申请 1，共 5 端点，注册组共用一个）
    /// 由 <c>OpenPlatformTemplateContractGuards</c>（TPL 系列）逐条锁定；
    /// 体验版二维码 <c>/wxa/get_qrcode</c> 为<b>非声明式</b>图片流通道（成功应答是 image/jpeg 二进制），
    /// 不进本反射枚举、<b>不计</b>入端点总数，由 <c>OpenPlatformTrialQrCodeContractGuards</c>（QRC 系列）单独锁定
    /// （同「令牌链 4 端点由 Provider 承载、不计入声明面」口径）。
    /// 试用小程序转正族的两支 <b>2.0 旧版路由</b>（<c>/wxa/getmpadminauth</c>、<c>/wxa/mpverifybetaweapp</c>）
    /// 经逐页核验判为<b>非契约</b>（官方旧页正文仅剩「优化后重新上线」、且不在现行 <c>openApi/OpenApiDoc</c> 目录树内），
    /// 由 <c>OpenPlatformNonContractRouteZeroDeclarationContractGuards</c>（OP-NC 系列）锁定零声明，<b>恒不计</b>入端点总数。
    /// 本守卫只负责「接口全集不漂移 + 端点总数与基线一致」。
    /// </remarks>
    [Fact]
    public void Interfaces_ShouldMatchBaseline_WhenContractSurfaceEnumerated()
    {
        var interfaces = MainAssembly.GetTypes()
            .Where(static t => t.IsInterface && t.IsPublic)
            .Where(static t => t.GetCustomAttributes(false)
                .Any(static a => a.GetType().Name == "HttpClientApiAttribute"))
            .OrderBy(static t => t.Name, StringComparer.Ordinal)
            .ToArray();

        interfaces.Should().HaveCount(Baseline.OpenPlatform.HttpApiInterfaces,
            "开放平台契约面接口数漂移须先核对官方开放面再同批调整 OpenPlatformModule 与 Tests/ContractBaseline.cs");

        interfaces.Select(static t => t.Name).Should().BeEquivalentTo(new[]
        {
            nameof(IWechatOpenPlatformComponentService),
            nameof(IWechatOpenPlatformComponentTicketFreeService),
            nameof(IWechatOpenPlatformOpenAccountService),
            nameof(IWechatOpenPlatformAccountService),
            nameof(IWechatOpenPlatformSnsService),
            nameof(IWechatOpenPlatformCategoryService),
            nameof(IWechatOpenPlatformComponentTcbEnvService),
            nameof(IWechatOpenPlatformComponentTcbDatabaseService),
            nameof(IWechatOpenPlatformComponentTcbFileService),
            nameof(IWechatOpenPlatformComponentTcbScfService),
            nameof(IWechatOpenPlatformComponentTcbCloudRunService),
            nameof(IWechatOpenPlatformComponentTcbStaticStoreService),
            nameof(IWechatOpenPlatformEmbeddedService),
            nameof(IWechatOpenPlatformAuthService),
            nameof(IWechatOpenPlatformAmsService),
            nameof(IWechatOpenPlatformAmsShareService),
            nameof(IWechatOpenPlatformMpCodeService),
            nameof(IWechatOpenPlatformMpBasicInfoService),
            nameof(IWechatOpenPlatformMpCategoryService),
            nameof(IWechatOpenPlatformMpDomainService),
            nameof(IWechatOpenPlatformMpMemberService),
            nameof(IWechatOpenPlatformMpRecordService),
            nameof(IWechatOpenPlatformMpSubscribeService),
            nameof(IWechatOpenPlatformMpIcpService),
            nameof(IWechatOpenPlatformMpPrivacyService),
            nameof(IWechatOpenPlatformMpOrderPathService),
            nameof(IWechatOpenPlatformMpBetaVerifyService),
            nameof(IWechatOpenPlatformMpLiveService),
            nameof(IWechatOpenPlatformTemplateService),
            nameof(IWechatOpenPlatformMpOrderPathSetService),
            nameof(IWechatOpenPlatformUserOrderService),
            nameof(IWechatOpenPlatformWeDataService),
            nameof(IWechatOpenPlatformWeDataUserService),
            nameof(IWechatOpenPlatformServiceMarketService),
        });

        var actualEndpoints = interfaces.Sum(GetEndpoints);
        actualEndpoints.Should().Be(Baseline.OpenPlatform.Endpoints,
            "端点总数漂移须同批更新 Tests/ContractBaseline.cs（令牌链 4 端点由 Provider 承载、不计入声明面）");
    }

    /// <summary>
    /// OP-B2：逐接口（方法, 路由）表与官方契约一致（T4 逐页核验后的定稿形态）。
    /// </summary>
    [Fact]
    public void Endpoints_ShouldMatchOfficialRoutes_WhenEnumeratedPerInterface()
    {
        foreach (var (interfaceName, expected) in ExpectedEndpoints)
        {
            var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{interfaceName}")
                ?? throw new InvalidOperationException($"接口 {interfaceName} 不存在（契约面漂移）");

            var actual = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.DeclaringType == type)
                .Select(static m =>
                {
                    var attr = m.GetCustomAttributes<HttpMethodAttribute>(false).Single();
                    return (attr.HttpMethod.Method.ToUpperInvariant(), attr.RequestUri!);
                })
                .ToArray();

            actual.Should().BeEquivalentTo(expected,
                because: $"{interfaceName} 的官方路由表——差集即缺口或漂移，须先核对官方文档再同批调整");
        }
    }

    /// <summary>
    /// OP-B3：逐接口令牌语义（类型键 + Query 参数名 + 免令牌豁免）——错配即「取错令牌/漏令牌」的静默故障。
    /// </summary>
    [Fact]
    public void TokenSemantics_ShouldMatchOfficialContract_WhenInterfacesDeclared()
    {
        var component = AssertToken(nameof(IWechatOpenPlatformComponentService));
        component.TokenType.Should().Be(OpenPlatformTokenTypes.ComponentAccessToken,
            "平台管理面消费平台自身令牌（T2 裁定后唯一入口）");
        component.QueryName.Should().Be("access_token",
            "官方 /cgi-bin/component/* 的令牌 query 参数名为 access_token（已按官方页面核验）");

        var ticketFree = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{nameof(IWechatOpenPlatformComponentTicketFreeService)}")!;
        ticketFree.GetCustomAttributes<TokenAttribute>(false).Should().BeEmpty(
            "api_start_push_ticket 以 component_secret 请求体直呼、发生在票据链建立之前——声明 [Token] 即编程错误");

        var openAccount = AssertToken(nameof(IWechatOpenPlatformOpenAccountService));
        openAccount.TokenType.Should().Be(OpenPlatformTokenTypes.AuthorizerAccessToken,
            "开放账号操作的是「授权账号」绑定的开放平台账号 ⇒ 消费授权方令牌");
        openAccount.QueryName.Should().Be("access_token");

        var account = AssertToken(nameof(IWechatOpenPlatformAccountService));
        account.TokenType.Should().Be(OpenPlatformTokenTypes.AuthorizerAccessToken);
        account.QueryName.Should().Be("access_token");

        var sns = AssertToken(nameof(IWechatOpenPlatformSnsService));
        sns.TokenType.Should().Be(OpenPlatformTokenTypes.ComponentAccessToken,
            "组件网页授权以平台令牌换用户级令牌");
        sns.QueryName.Should().Be("component_access_token",
            "官方 /sns/oauth2/component/* 的令牌 query 参数名为 component_access_token（已按官方页面核验）");
    }

    /// <summary>
    /// OP-B4：令牌类型常量独立键系（<c>Wechat.</c> 前缀 + 两键互异 + 不与既有产品线撞键）。
    /// </summary>
    [Fact]
    public void TokenTypes_ShouldStayInIndependentKeySpace_WhenSharedWithOtherLines()
    {
        OpenPlatformTokenTypes.ComponentAccessToken.Should().StartWith("Wechat.",
            "全仓令牌类型一律 Wechat. 前缀（AGENTS §5.1）");
        OpenPlatformTokenTypes.AuthorizerAccessToken.Should().StartWith("Wechat.");

        new[]
        {
            OpenPlatformTokenTypes.ComponentAccessToken,
            OpenPlatformTokenTypes.AuthorizerAccessToken,
        }.Should().OnlyHaveUniqueItems("平台令牌与授权方令牌必须可区分（错配即取错令牌）");

        // 与既有产品线的键集互斥（同宿主并存多线时不串号）。
        new[]
        {
            OpenPlatformTokenTypes.ComponentAccessToken,
            OpenPlatformTokenTypes.AuthorizerAccessToken,
        }.Should().NotContain("Wechat.AccessToken")
            .And.NotContain("Wechat.Mp.AccessToken")
            .And.NotContain("Wechat.Channels.AccessToken")
            .And.NotContain("Wechat.ProviderAccessToken")
            .And.NotContain("Wechat.SuiteAccessToken");
    }

    /// <summary>
    /// OP-B5：跨线路由零交叠——本线路由前缀在全仓唯一（扫描全部源码接口声明）。
    /// </summary>
    [Fact]
    public void RoutePrefixes_ShouldBeOwnedExclusively_WhenScannedAcrossAllLines()
    {
        var repoRoot = FindRepoRoot();
        var interfacesDir = Path.Combine(repoRoot, "Src");
        var routeRegex = new Regex(@"\[(?:Get|Post|Put|Delete|Patch)\(""([^""]+)""\)\]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        var openPlatformRoutes = new[]
        {
            "/cgi-bin/component/",
            "/cgi-bin/open/",
            "/cgi-bin/account/",
            "/sns/oauth2/component/",
            // 半屏小程序族独占：小程序线由守卫 NC2 锁定零声明，本线声明者唯一 ⇒ 其它线不得重复声明。
            "/wxaapi/wxaembedded/",
            // 微信认证族独占（逐条全路径，不用宽前缀 /wxa/sec/ —— 小程序线合法持有 /wxa/sec/vod/* 短剧媒资面）：
            // 同样由 NC2 锁定小程序线零声明，本线以 authorizer_access_token 承载。
            "/wxa/sec/authidentitytree",
            "/wxa/sec/wxaauth",
            "/wxa/sec/reauth",
            "/wxa/sec/queryauth",
            "/wxa/sec/uploadauthmaterial",
            // 流量主代运营（AMS）族独占（逐条全路径）：Ams 单路由 action 分派 + AmsShare 两支查询/设置路由，
            // 均为开放平台第三方面，其它线不得声明（零交叠由 AM 系列守卫与本条双向锁定）。
            "/wxa/operationams",
            "/wxa/getdefaultamsinfo",
            "/wxa/setdefaultamsinfo",
            // 代小程序管理（MiniProgramManage）族独占（逐条全路径，30-1 落地 21 端点）：本线以 authorizer_access_token
            // 代授权方小程序调用同一批官方 /wxa/* 路由，小程序线自管面未声明这些路由（现状零交叠，逐条锁定防后续撞车）。
            "/wxa/commit",
            "/wxa/submit_audit",
            "/wxa/get_auditstatus",
            "/wxa/get_latest_auditstatus",
            "/wxa/speedupaudit",
            "/wxa/undocodeaudit",
            "/wxa/queryquota",
            "/wxa/release",
            "/wxa/grayrelease",
            "/wxa/revertgrayrelease",
            "/wxa/revertcoderelease",
            "/wxa/get_page",
            "/wxa/getversioninfo",
            "/wxa/getvisitstatus",
            "/wxa/change_visitstatus",
            "/wxa/setnickname",
            "/wxa/api_wxa_querynickname",
            "/wxa/changewxasearchstatus",
            "/wxa/getwxasearchstatus",
            "/wxa/fetchdatasetting",
            "/wxa/get_category",
            // 30-2 并入本注册组四族（域名与 DNS 9 + 体验成员 3 + 违规申诉 2 + 订阅组件 3），同样逐条全路径独占。
            "/wxa/get_effective_domain",
            "/wxa/get_effective_webviewdomain",
            "/wxa/get_prefetchdnsdomain",
            "/wxa/get_webviewdomain_confirmfile",
            "/wxa/modify_domain",
            "/wxa/modify_domain_directly",
            "/wxa/set_prefetchdnsdomain",
            "/wxa/setwebviewdomain",
            "/wxa/setwebviewdomain_directly",
            "/wxa/bind_tester",
            "/wxa/unbind_tester",
            "/wxa/memberauth",
            "/wxa/getappealrecords",
            "/wxa/getillegalrecords",
            "/wxa/getshowwxaitem",
            "/wxa/getwxamplinkforshow",
            "/wxa/updateshowwxaitem",
            // 30-3 并入本注册组三支（ICP 备案 JSON 面 12 + 隐私接口 3 + 订单路径查询 1）。
            "/wxa/icp/apply_icp_filing",
            "/wxa/icp/cancel_apply_icp_filing",
            "/wxa/icp/cancel_icp_filing",
            "/wxa/icp/create_icp_verifytask",
            "/wxa/icp/query_icp_verifytask",
            "/wxa/icp/get_icp_entrance_info",
            "/wxa/icp/get_online_icp_order",
            "/wxa/icp/query_icp_certificate_types",
            "/wxa/icp/query_icp_nrlx_types",
            "/wxa/icp/query_icp_subject_types",
            "/wxa/icp/query_icp_service_content_types",
            "/wxa/icp/query_icp_district_code",
            "/wxa/security/apply_privacy_interface",
            "/wxa/security/get_privacy_interface",
            "/wxa/security/get_code_privacy_info",
            "/wxa/security/getorderpathinfo",
            // 30-4 试用小程序核验 2 支并入本授权方组（verifybetaweapp / setbetaweappnickname，权限集 81、authorizer_access_token）。
            "/wxa/verifybetaweapp",
            "/wxa/setbetaweappnickname",
            // 30-4 新注册组 MiniProgramTemplate（component_access_token 面）逐条全路径独占：模板库 4 + 下单路径申请 1。
            // 令牌归属与授权方 MiniProgramManage 相反 ⇒ 独立成组，路由前缀仍本线唯一。
            "/wxa/gettemplatedraftlist",
            "/wxa/gettemplatelist",
            "/wxa/addtotemplate",
            "/wxa/deletetemplate",
            "/wxa/security/applysetorderpathinfo",
            // 30-5 特殊形态三支：两支 multipart（表单由宿主拼装、零 JSON 请求 DTO）+ 一支**非声明式**图片流通道路由
            // （get_qrcode 成功应答为 image/jpeg 二进制，由 QRC 系列守卫锁定「全仓仅此一处承载」）。
            "/wxa/uploadmedia",
            "/wxa/icp/upload_icp_media",
            "/wxa/get_qrcode",
            // 30-7 直播能力开通（applyliveinfo）：官方页面明写仅支持「第三方平台」账号类型代调用、
            // 以 authorizer_access_token 承载 ⇒ 本线独占，小程序线由 NC2 锁定零声明。
            "/wxa/business/applyliveinfo",
            // 小程序购物订单域（UserOrder）独占：全 /user-order/* 前缀，小程序线由 NC2 锁定零声明。
            "/user-order/",
            // We 分析（WeData）独占：全 /wedata/* 前缀，小程序线由 NC2 锁定零声明。
            "/wedata/",
            // 第三方服务市场（ServiceMarket）独占：逐条全路径（服务商 component 令牌子集）。
            // 小程序线合法持有 /wxa/servicemarket（自建面）与 /wxa/servicemarketretrieve，故不用宽前缀。
            "/wxa/servicemarket/service/get_service_buyer_list",
            "/wxa/servicemarket/service/get_service_buyer",
            "/wxa/servicemarket/get_paid_order_list",
        };

        var offenders = new List<string>();
        foreach (var file in Directory.EnumerateFiles(interfacesDir, "*.cs", SearchOption.AllDirectories))
        {
            var normalized = file.Replace('\\', '/');
            if (normalized.Contains("/obj/", StringComparison.Ordinal)
                || normalized.Contains("/bin/", StringComparison.Ordinal)
                || normalized.Contains("/Generated/", StringComparison.Ordinal))
            {
                continue;
            }

            var isThisLine = normalized.Contains("/OpenPlatform/", StringComparison.Ordinal);
            foreach (var match in routeRegex.Matches(File.ReadAllText(file)))
            {
                var route = ((System.Text.RegularExpressions.Match)match).Groups[1].Value;
                foreach (var prefix in openPlatformRoutes)
                {
                    if (route.StartsWith(prefix, StringComparison.Ordinal) && !isThisLine)
                    {
                        offenders.Add($"{normalized}: {route}");
                    }
                }
            }
        }

        offenders.Should().BeEmpty(
            "开放平台的路由前缀必须全仓唯一归属（官方同前缀跨线同名是真实风险——" +
            "公众号线 /cgi-bin/account/* 若被误建即双实现撞车）");
    }

    /// <summary>
    /// OP-B6：<c>fastregisterweapp</c> 同路由双方法的 <c>action</c> 分派锁定（create / search 各一字面量）。
    /// </summary>
    [Fact]
    public void FastRegisterWeappMethods_ShouldDispatchByAction_WhenSameRouteDeclaredTwice()
    {
        var type = typeof(IWechatOpenPlatformComponentService);
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == type)
            .Where(static m => m.GetCustomAttributes<HttpMethodAttribute>(false)
                .Any(static a => a.RequestUri == "/cgi-bin/component/fastregisterweapp"))
            .ToArray();

        methods.Should().HaveCount(2, "同路由双方法（create / search）——少一个即缺口，多一个即漂移");

        // action 分派语义落在方法参数默认值（C# default），而非特性——生成器按参数默认值回填 query。
        var actions = methods
            .Select(static m => m.GetParameters()
                .Where(static p => p.GetCustomAttributes<QueryAttribute>(false)
                    .Any(static q => q.Name == "action"))
                .Select(static p => p.DefaultValue as string)
                .FirstOrDefault())
            .ToArray();

        actions.Should().BeEquivalentTo(new[] { "create", "search" },
            "action query 参数的默认值即官方分派语义（缺省时请求落到错误分支）");
    }

    /// <summary>
    /// OP-B6b：<c>fastregisterpersonalweapp</c> 同路由双方法（create / query）的 <c>action</c> 分派锁定。
    /// </summary>
    [Fact]
    public void FastRegisterPersonalWeappMethods_ShouldDispatchByAction_WhenSameRouteDeclaredTwice()
    {
        var type = typeof(IWechatOpenPlatformComponentService);
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType == type)
            .Where(static m => m.GetCustomAttributes<HttpMethodAttribute>(false)
                .Any(static a => a.RequestUri == "/wxa/component/fastregisterpersonalweapp"))
            .ToArray();

        methods.Should().HaveCount(2, "同路由双方法（create / query）——少一个即缺口，多一个即漂移");

        var actions = methods
            .Select(static m => m.GetParameters()
                .Where(static p => p.GetCustomAttributes<QueryAttribute>(false)
                    .Any(static q => q.Name == "action"))
                .Select(static p => p.DefaultValue as string)
                .FirstOrDefault())
            .ToArray();

        actions.Should().BeEquivalentTo(new[] { "create", "query" },
            "action query 参数的默认值即官方分派语义（缺省时请求落到错误分支）");
    }

    /// <summary>
    /// OP-B7：五个域 + Common 的 DTO 全量登记进对应 JsonContext（AOT 源生成完整性）。
    /// </summary>
    /// <remarks>
    /// <c>ComponentTcb</c>（166 DTO）不在本表内——它由域守卫 CBT4 逐条锁定并附类型数，本表覆盖其余各域。
    /// </remarks>
    [Fact]
    public void DataModels_ShouldBeRegisteredInDomainJsonContexts_WhenContractSurfaceEnumerated()
    {
        var dataModelsAssembly = typeof(OpenPlatformResponse).Assembly;
        var expectations = new (Type ContextType, string Namespace, string Segment)[]
        {
            (typeof(CommonJsonContext), "Mud.Wechat.OpenPlatform.DataModels", "Common"),
            (typeof(ComponentJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Component", "Component"),
            (typeof(OpenAccountJsonContext), "Mud.Wechat.OpenPlatform.DataModels.OpenAccount", "OpenAccount"),
            (typeof(AccountJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Account", "Account"),
            (typeof(CategoryJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Category", "Category"),
            (typeof(SnsJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Sns", "Sns"),
            (typeof(EmbeddedJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Embedded", "Embedded"),
            (typeof(AuthJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Auth", "Auth"),
            (typeof(AmsJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Ams", "Ams"),
            (typeof(MiniProgramManageJsonContext), "Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage", "MiniProgramManage"),
            (typeof(MiniProgramTemplateJsonContext), "Mud.Wechat.OpenPlatform.DataModels.MiniProgramTemplate", "MiniProgramTemplate"),
            (typeof(UserOrderJsonContext), "Mud.Wechat.OpenPlatform.DataModels.UserOrder", "UserOrder"),
            (typeof(WeDataJsonContext), "Mud.Wechat.OpenPlatform.DataModels.WeData", "WeData"),
            (typeof(ServiceMarketJsonContext), "Mud.Wechat.OpenPlatform.DataModels.ServiceMarket", "ServiceMarket"),
        };

        foreach (var (contextType, ns, segment) in expectations)
        {
            var context = (System.Text.Json.Serialization.JsonSerializerContext)(contextType
                .GetProperty("Default", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null) ?? throw new InvalidOperationException($"{contextType.Name}.Default 缺失（生成物漂移）"));

            var domainTypes = dataModelsAssembly.GetTypes()
                .Where(static t => t.IsClass && !t.IsNested)
                .Where(t => t.Namespace == ns)
                .Where(static t => !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
                .ToArray();

            domainTypes.Should().NotBeEmpty($"{ns} 域类型数不可为 0（枚举空跑防护）");

            foreach (var type in domainTypes)
            {
                context.GetTypeInfo(type).Should().NotBeNull(
                    $"{type.Name} 位于 {ns}，必须登记进 {contextType.Name}（AOT 源生成）");

                var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
                serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
                serializable!.SerializerClassName.Should().Be(segment,
                    $"{type.Name} 的 SerializerClassName 必须为域段 {segment}");
            }
        }
    }

    /// <summary>
    /// OP-B8：响应基底实现公用层判错契约（与公众号 / 支付线同款形态）。
    /// </summary>
    [Fact]
    public void ResponseBase_ShouldImplementSharedErrorContract_WhenDeclared()
    {
        typeof(OpenPlatformResponse).Should().BeAssignableTo<Mud.Wechat.Abstractions.Contracts.IWechatApiResponse>(
            "开放平台响应基底必须实现公用层判错契约，判错出口才能跨产品线统一");

        new OpenPlatformResponse().IsSuccess.Should().BeTrue(
            "api_start_push_ticket 等成功响应可能缺省 errcode ⇒ ErrorCode 缺省 0 必须视为成功");
    }

    private static (string TokenType, string QueryName) AssertToken(string interfaceName)
    {
        var type = MainAssembly.GetType($"Mud.Wechat.OpenPlatform.{interfaceName}")
            ?? throw new InvalidOperationException($"接口 {interfaceName} 不存在（契约面漂移）");
        var token = type.GetCustomAttributes<TokenAttribute>(false).Single();
        return (token.TokenType, token.Name!);
    }

    private static int GetEndpoints(Type type)
        => type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Count(m => m.DeclaringType == type
                && m.GetCustomAttributes<HttpMethodAttribute>(false).Any());

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName
            ?? throw new InvalidOperationException("未找到仓库根（缺 Mud.Wechat.slnx）——请在检出树内运行测试。");
    }
}
