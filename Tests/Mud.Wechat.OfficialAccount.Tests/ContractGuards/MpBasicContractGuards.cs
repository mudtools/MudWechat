// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.DataModels.Basic;
using Mud.Wechat.OfficialAccount.Extensions;

namespace Mud.Wechat.OfficialAccount.Tests.ContractGuards;

/// <summary>
/// 公众号「基础接口」域 + 「令牌签发」契约守卫：路由表、接口层级、令牌绑定、
/// Query 令牌白名单与 JSON 上下文全量登记锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方「服务端 API → 基础接口」分组下恰 5 个端点，按本仓切分习惯拆为
/// <b>认证 2</b>（令牌签发：<c>getAccessToken</c> / <c>getStableAccessToken</c>，位于 Abstractions 的
/// Authentication 注册组）+ <b>基础 3</b>（<c>get_api_domain_ip</c> / <c>getcallbackip</c> /
/// <c>callbackCheck</c>）。
/// </para>
/// <para>
/// 与企微的关键结构差异：公众号无「自建 / 套件 / 代开发」三类应用形态 ⇒
/// <c>IMpBasicService</c> <b>本身即注册接口</b>（<c>RegistryGroupName = "Basic"</c> 且
/// <b>不</b>标 <c>IsAbstract</c>），继承链上<b>不得</b>出现任何应用类型子接口（能力漂移守卫）。
/// </para>
/// <para>
/// M0 范围边界：基础域 3 个端点官方<b>均支持</b>第三方平台令牌
/// （<c>component_access_token</c> / <c>authorizer_access_token</c>），本 SDK 仅覆盖自建形态；
/// 官方若在第三方形态上产生契约差异，须先核对文档再同批调整本守卫与实现。
/// </para>
/// </remarks>
public class MpBasicContractGuards
{
    private const string BasicRegistryGroupName = "Basic";

    /// <summary>基础域官方路由表（3 个端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] BasicRoutes =
    {
        // 获取微信 API 服务器 IP（官方即 GET、无请求体）。
        (typeof(IMpBasicService),
            nameof(IMpBasicService.GetApiDomainIpAsync),
            typeof(GetAttribute), "/cgi-bin/get_api_domain_ip"),
        // 获取微信推送服务器 IP（官方即 GET、无请求体）。
        (typeof(IMpBasicService),
            nameof(IMpBasicService.GetCallbackIpAsync),
            typeof(GetAttribute), "/cgi-bin/getcallbackip"),
        // 网络通信检测（官方即 POST、请求体 action + check_operator）。
        (typeof(IMpBasicService),
            nameof(IMpBasicService.CheckCallbackAsync),
            typeof(PostAttribute), "/cgi-bin/callback/check"),
    };

    /// <summary>令牌签发族官方路由表（2 个端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AuthenticationRoutes =
    {
        (typeof(IMpAuthentication), nameof(IMpAuthentication.GetTokenAsync),
            typeof(GetAttribute), "/cgi-bin/token"),
        (typeof(IMpAuthentication), nameof(IMpAuthentication.GetStableTokenAsync),
            typeof(PostAttribute), "/cgi-bin/stable_token"),
    };

    /// <summary>契约守卫 MP-BS1：基础域 3 端点路由必须与官方契约一致（漂移即红）。</summary>
    [Fact]
    public void BasicEndpoints_ShouldMatchOfficialRoutes()
    {
        BasicRoutes.Should().HaveCount(3,
            "公众号基础域官方恰 3 个业务端点（get_api_domain_ip / getcallbackip / callbackCheck）");

        var distinctRoutes = BasicRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "基础域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in BasicRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 MP-BS2：令牌签发族 2 端点路由与「无 [Token]」自递归约束。</summary>
    [Fact]
    public void AuthenticationEndpoints_ShouldMatchOfficialRoutesWithoutToken()
    {
        AuthenticationRoutes.Should().HaveCount(2,
            "令牌签发族恰 2 个端点（getAccessToken / getStableAccessToken）");

        foreach (var (iface, method, httpAttribute, route) in AuthenticationRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 自递归约束：签发接口自身不得携带 [Token]（否则刷新请求会进入恢复链路）。
        typeof(IMpAuthentication).GetCustomAttribute<TokenAttribute>()
            .Should().BeNull("令牌签发接口必须不带 [Token]（自递归约束）");
    }

    /// <summary>
    /// 契约守卫 MP-BS3：接口层级与注册形态——本接口自身即注册接口，
    /// <b>不得</b>标 IsAbstract、继承链上不得出现应用类型子接口。
    /// </summary>
    [Fact]
    public void BasicInterfaceHierarchy_ShouldNotDeclareApplicationTypeChildren()
    {
        var iface = typeof(IMpBasicService);
        var api = iface.GetCustomAttribute<HttpClientApiAttribute>();

        api.Should().NotBeNull("基础域接口必须声明 [HttpClientApi]");
        api!.IsAbstract.Should().BeFalse(
            "公众号无应用类型分化 ⇒ 本接口直接作注册接口；标 IsAbstract 会使客户端无法从 DI 解析");
        api.RegistryGroupName.Should().Be(BasicRegistryGroupName,
            "基础域必须挂 Basic 注册组（AddBasicWebApiHttpClient() 注册）");
        api.TokenManage.Should().Be(nameof(IMpAppManager), "声明式令牌注入必须经公众号应用管理器定位上下文");

        var derived = iface.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != iface && iface.IsAssignableFrom(t))
            .ToList();
        derived.Should().BeEmpty(
            "公众号无「自建 / 套件 / 代开发」三类形态，基础域不得出现 _Internal / _ThirdParty / _Provider 子接口");
    }

    /// <summary>
    /// 契约守卫 MP-BS4：令牌绑定——基础域统一消费 <c>MpTokenTypes.AccessToken</c> 并以 Query 注入
    /// （官方契约 <c>access_token</c>；与企微 MUD005 同源已知接受风险）。
    /// </summary>
    [Fact]
    public void BasicTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var token = typeof(IMpBasicService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("基础域接口必须声明 [Token]");
        token!.TokenType.Should().Be(MpTokenTypes.AccessToken, "令牌路由键必须为公众号 access_token");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query,
            "公众号官方契约强制 Query 注入（access_token），无法改用 Header（MUD005）");
        token.Name.Should().Be("access_token", "Query 注入参数名必须为官方契约的 access_token");
    }

    /// <summary>
    /// 契约守卫 MP-BS5：公众号侧 Query 令牌白名单（企微 G5 反射的是 <c>Mud.Wechat.Work</c> 程序集，
    /// 无法覆盖本程序集 ⇒ 必须在产品线内自建同形守卫，否则新接口可绕过 MUD005 审计面）。
    /// </summary>
    [Fact]
    public void QueryTokenInjection_ShouldBeLimitedToDeclaredInterfaces()
    {
        var assembly = typeof(MpServiceCollectionExtensions).Assembly;

        var queryInjectionInterfaces = assembly.GetTypes()
            .Where(t => t.IsInterface)
            .Where(i => i.GetCustomAttribute<TokenAttribute>() is { } attr
                        && attr.InjectionMode == TokenInjectionMode.Query)
            .Select(i => i.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();

        // 精确白名单集由 MP-TG8（标签域守卫）持有——两处都断言全集会在新增域时双改，故此处只断言「本域在内」
        // 并防止「整个 Query 注入面为空」的静默空跑。
        queryInjectionInterfaces.Should().Contain(nameof(IMpBasicService),
            "基础域必须在内；全集白名单由 MpTagContractGuards.MP-TG8 锁定");
        queryInjectionInterfaces.Should().NotBeEmpty();
    }

    /// <summary>
    /// 契约守卫 MP-BS6：基础域 DTO 全量登记进 AOT JSON 上下文（<c>SerializerClassName = "Basic"</c>），
    /// 且请求/响应形态与官方契约一致。
    /// </summary>
    [Fact]
    public void BasicDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = BasicJsonContext.Default;

        var domainTypes = typeof(MpGetApiDomainIpResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.OfficialAccount.DataModels.Basic"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        const int expectedCount = 8;
        domainTypes.Should().HaveCount(expectedCount,
            "基础域契约面类型数漂移须先核对官方文档再同批调整本守卫" +
            "（2 令牌 DTO + 2 IP 响应 + 1 检测请求 + 1 检测响应 + 2 嵌套项）");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于基础域命名空间，必须登记进 BasicJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be(BasicRegistryGroupName,
                $"{type.Name} 的 SerializerClassName 必须为基础域段 Basic");
        }
    }

    /// <summary>契约守卫 MP-BS7：响应基底实现公用层判错契约，且缺省 errcode 语义为「成功」。</summary>
    [Fact]
    public void ResponseBase_ShouldImplementSharedErrorContract()
    {
        typeof(MpResponse).Should().BeAssignableTo<Mud.Wechat.Abstractions.Contracts.IWechatApiResponse>(
            "公众号响应基底必须实现公用层判错契约，判错出口才能跨产品线统一");

        new MpResponse().IsSuccess.Should().BeTrue(
            "token / stable_token 成功响应不带 errcode ⇒ ErrorCode 缺省 0 必须视为成功");

        new MpResponse { ErrorCode = 40013 }.IsSuccess.Should().BeFalse();
    }

    /// <summary>契约守卫 MP-BS8：官方契约陷阱——两个 IP 端点的响应结构同构且 ip_list 可空。</summary>
    [Fact]
    public void IpListResponses_ShouldBeIsomorphicAndNullable()
    {
        var api = typeof(MpGetApiDomainIpResponse).GetProperty(nameof(MpGetApiDomainIpResponse.IpList));
        var callback = typeof(MpGetCallbackIpResponse).GetProperty(nameof(MpGetCallbackIpResponse.IpList));

        api.Should().NotBeNull();
        callback.Should().NotBeNull();
        api!.PropertyType.Should().Be(callback!.PropertyType,
            "官方两页正文逐项一致 ⇒ 两个响应 DTO 结构必须同构（刻意分别声明以保持端点↔DTO 一对一）");

        var jsonName = api.GetCustomAttribute<System.Text.Json.Serialization.JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be("ip_list", "字段名照抄官方原文");
    }

    /// <summary>契约守卫 MP-BS9：防静默空跑——程序集内契约面接口数下限（枚举失效即红）。</summary>
    [Fact]
    public void ContractSurface_ShouldMeetMinimumInterfaceCount()
    {
        var assembly = typeof(MpServiceCollectionExtensions).Assembly;
        var interfaces = assembly.GetTypes().Where(t => t.IsInterface && t.IsPublic).ToList();

        interfaces.Should().HaveCountGreaterOrEqualTo(1,
            "主包程序集必须至少声明 IMpBasicService（防反射枚举静默空跑导致守卫假绿）");
        interfaces.Should().Contain(typeof(IMpBasicService));

        typeof(IMpBasicService).Assembly.Should().NotBeSameAs(typeof(IMpAuthentication).Assembly,
            "基础域在主包、令牌签发族在 Abstractions（注册面分离是设计约束，非偶然）");
    }

    /// <summary>
    /// 契约守卫 MP-BS10：官方契约陷阱锁定——两个 IP 端点无业务 Query 参数与请求体（签名仅剩取消令牌），
    /// <c>callbackCheck</c> 为 POST + 请求体。
    /// </summary>
    [Fact]
    public void BasicEndpoints_ShouldLockOfficialContractTraps()
    {
        foreach (var method in new[]
                 {
                     nameof(IMpBasicService.GetApiDomainIpAsync),
                     nameof(IMpBasicService.GetCallbackIpAsync),
                 })
        {
            var target = typeof(IMpBasicService).GetMethod(
                method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"IMpBasicService.{method} 必须存在");

            target!.GetCustomAttributes<QueryAttribute>()
                .Should().BeEmpty($"{method} 官方无业务 Query 参数（access_token 经 [Token] 注入）");
            target.GetParameters().Should().ContainSingle(
                p => p.ParameterType == typeof(CancellationToken),
                $"{method} 无请求体亦无业务 Query 参数，签名仅剩取消令牌");
        }

        var check = typeof(IMpBasicService).GetMethod(
            nameof(IMpBasicService.CheckCallbackAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        check.Should().NotBeNull();
        check!.GetParameters().Should().ContainSingle(p => p.ParameterType == typeof(MpCallbackCheckRequest),
            "callbackCheck 官方为 POST + JSON 请求体（action + check_operator 均必填）");
    }
}
