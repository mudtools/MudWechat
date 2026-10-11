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
using Mud.Wechat.OpenPlatform.DataModels.Account;
using Mud.Wechat.OpenPlatform.DataModels.Component;
using Mud.Wechat.OpenPlatform.DataModels.OpenAccount;
using Mud.Wechat.OpenPlatform.DataModels.Sns;

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
/// ④ 跨线路由零交叠；⑤ <c>fastregisterweapp</c> 同路由双方法以 <c>action</c> 分派。
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
        },
    };

    /// <summary>
    /// OP-B1：接口集合与数量锁定（5 接口 / 26 端点，防漏防多）。
    /// </summary>
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
    /// OP-B5：跨线路由零交叠——本线四个路由前缀在全仓唯一（扫描全部源码接口声明）。
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
            "开放平台的四个路由前缀必须全仓唯一归属（官方同前缀跨线同名是真实风险——" +
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
    /// OP-B7：四个域 + Common 的 DTO 全量登记进对应 JsonContext（AOT 源生成完整性）。
    /// </summary>
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
            (typeof(SnsJsonContext), "Mud.Wechat.OpenPlatform.DataModels.Sns", "Sns"),
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
