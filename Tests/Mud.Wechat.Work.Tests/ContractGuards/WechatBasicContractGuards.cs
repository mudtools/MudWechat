// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Basic;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 基础接口域（Basic 模块）契约守卫：路由表、接口层级、令牌绑定与 JSON 上下文全量登记锁定
/// （获取企业微信接口IP段 + 获取企业微信回调IP段）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方「服务端API → 开发指南 → 基础接口」分组下恰有 2 个端点，
/// 对<b>企业自建应用（92520 / 92521）与服务商代开发（97073 / 98988）</b>开放同路由同契约，
/// 全部收敛声明于 IsAbstract 父接口，自建 / 代开发均为零差异端点空标记子接口
/// （形态对齐政民沟通·配置网格结构族：父接口 + 自建/代开发两个空标记子接口）。
/// </para>
/// <para>
/// 官方第三方应用开发文档树<b>无「基础接口」分组</b>，故继承链上不得出现第三方子接口
/// （能力漂移守卫；官方后续若开放，须先核对文档再落位并同批调整本守卫与 G5）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：两个端点<b>官方即 GET 且无请求体、无业务 Query 参数</b>
/// （令牌是唯一参数，经 <c>[Token]</c> Query 注入）——勿改成 POST、勿为「统一风格」加请求 DTO；
/// 两条路由的官方尾段形态各异（<c>get_api_domain_ip</c> 为多段下划线、<c>getcallbackip</c> 为无下划线单词），
/// 照抄原文，勿互相「对齐」；<c>ip_list</c> 在失败示例（如 42001）中返回<b>空数组</b>而非缺省，
/// 故以非空集合承载、调用方无需判空。
/// </para>
/// </remarks>
public class WechatBasicContractGuards
{
    /// <summary>
    /// 基础接口族父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string BasicParentImplementationClassName = "WechatWorkBasicService";

    private const string BasicRegistryGroupName = "Basic";

    /// <summary>
    /// 基础接口族官方路由表（父接口 2 条公共端点，官方即 GET、无请求体）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] BasicRoutes =
    {
        // 获取企业微信接口IP段（自建 92520 / 代开发 97073），官方即 GET、无请求体。
        (typeof(IWechatWorkBasicService),
            nameof(IWechatWorkBasicService.GetApiDomainIpAsync),
            typeof(GetAttribute), "/cgi-bin/get_api_domain_ip"),
        // 获取企业微信回调IP段（自建 92521 / 代开发 98988），官方即 GET、无请求体。
        (typeof(IWechatWorkBasicService),
            nameof(IWechatWorkBasicService.GetCallbackIpAsync),
            typeof(GetAttribute), "/cgi-bin/getcallbackip"),
    };

    /// <summary>
    /// 契约守卫 BS1：基础接口族端点路由必须与官方契约一致
    /// （新增/改名端点须同批更新本表；两端点官方即 GET、无请求体，勿改成 POST）。
    /// </summary>
    [Fact]
    public void BasicEndpoints_ShouldMatchOfficialRoutes()
    {
        BasicRoutes.Should().HaveCount(2,
            "基础接口族官方恰 2 个端点（获取接口IP段 + 获取回调IP段），全部收敛父接口");

        var distinctRoutes = BasicRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "基础接口族各端点路由互不重复");
        distinctRoutes.Should().OnlyContain(r => r.StartsWith("/cgi-bin/get", StringComparison.Ordinal),
            "基础接口族全部端点路由位于 /cgi-bin/get 段（get_api_domain_ip / getcallbackip）");

        foreach (var (iface, method, httpAttribute, route) in BasicRoutes)
        {
            // DeclaredOnly：端点必须声明在父接口自身，而非下沉到应用类型子接口。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 BS2：接口层级与生成器注册形态——2 个端点收敛于 IsAbstract 父接口，
    /// 自建 / 代开发均为零端点空标记子接口且继承链恰为 2 个子接口
    /// （官方第三方文档树无「基础接口」分组，能力漂移守卫）。
    /// </summary>
    [Fact]
    public void BasicInterfaceHierarchy_ShouldConvergeOnInternalAndProviderChildrenOnly()
    {
        var parent = typeof(IWechatWorkBasicService);
        var children = new[]
        {
            typeof(IWechatWorkInternalBasicService),
            typeof(IWechatWorkProviderBasicService),
        };

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, "基础接口族父接口恰持 get_api_domain_ip / getcallbackip 2 条公共端点");

        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal).ToList();
        derived.Should().BeEquivalentTo(children,
            "基础接口族继承链恰为自建/代开发两个空标记子接口；官方第三方文档树无「基础接口」分组，不得补第三方子接口");

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");

            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(BasicRegistryGroupName,
                $"{child.Name} 必须挂 {BasicRegistryGroupName} 注册组" +
                $"（Basic 模块经 Add{BasicRegistryGroupName}WebApiHttpClient() 注册）");
            childApi.InheritedFrom.Should().Be(BasicParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类 {BasicParentImplementationClassName}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty(
                    $"{child.Name} 为空标记：官方自建与代开发的端点集同路由同契约，2 个端点全部由父接口承载");
        }
    }

    /// <summary>
    /// 契约守卫 BS3：令牌绑定——基础接口族 3 个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；归属域键由 <c>WechatTokenOwnerContractGuards</c> 全局锁定，此处不重复）。
    /// </summary>
    [Fact]
    public void BasicTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkBasicService),
            typeof(IWechatWorkInternalBasicService),
            typeof(IWechatWorkProviderBasicService),
        };

        interfaces.Should().HaveCount(3, "基础接口族 = 父接口 + 自建/代开发两个空标记子接口");

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }

        // 父接口不声明凭据归属域（归属域是应用类型子接口的必填声明，见 WechatTokenOwnerContractGuards TO1）。
        // 反射陷阱：未显式书写 TokenManagerKey 时读到的是构造函数参数的默认值哨兵（TokenTypes.AccessToken），
        // 故此处断言「不等于两个归属域键」，而非断言某个具体取值。
        var parentTokenManagerKey = typeof(IWechatWorkBasicService)
            .GetCustomAttribute<TokenAttribute>()!.TokenManagerKey;
        parentTokenManagerKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
            "父接口不得声明自建归属域键（IsAbstract 且不参与 DI 注册）");
        parentTokenManagerKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
            "父接口不得声明授权企业归属域键（IsAbstract 且不参与 DI 注册）");
    }

    /// <summary>
    /// 契约守卫 BS4：基础接口模块的请求/响应 DTO 必须全量登记进 AOT JSON 上下文
    /// （SerializerClassName 统一为 Basic）。
    /// </summary>
    [Fact]
    public void BasicDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = BasicJsonContext.Default;

        var domainTypes = typeof(GetApiDomainIpResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Basic"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 BasicJsonContext
        //（两个响应 DTO；两端点均无请求体、不设请求 DTO，
        // errcode/errmsg 由 WechatWorkResponse 基类承载、落 CommonJsonContext 不计入本域）。
        domainTypes.Should().HaveCount(2,
            "基础接口模块契约面类型数漂移须先核对官方文档再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于基础接口域命名空间，必须登记进 BasicJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be(BasicRegistryGroupName,
                $"{type.Name} 的 SerializerClassName 必须为基础接口域段 Basic");
        }
    }

    /// <summary>
    /// 契约守卫 BS5：官方契约陷阱锁定——两端点无请求体亦无业务 Query 参数（签名仅剩取消令牌）；
    /// <c>ip_list</c> 为非空字符串集合（官方失败示例返回空数组而非缺省），字段名照抄官方原文。
    /// </summary>
    [Fact]
    public void BasicDataModels_ShouldLockOfficialContractTraps()
    {
        foreach (var method in new[]
                 {
                     nameof(IWechatWorkBasicService.GetApiDomainIpAsync),
                     nameof(IWechatWorkBasicService.GetCallbackIpAsync),
                 })
        {
            var target = typeof(IWechatWorkBasicService).GetMethod(
                method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"IWechatWorkBasicService.{method} 必须存在");

            target!.GetCustomAttributes<QueryAttribute>()
                .Should().BeEmpty(
                    $"{method} 官方无业务 Query 参数（access_token 是唯一参数且经 [Token] 注入），不得新增方法级固定 Query");
            target.GetParameters().Should().ContainSingle(
                p => p.ParameterType == typeof(CancellationToken),
                $"{method} 无请求体亦无业务 Query 参数，签名仅剩取消令牌（不得引入请求 DTO）");
            target.ReturnType.Should().Be(typeof(Task<>).MakeGenericType(
                    method == nameof(IWechatWorkBasicService.GetApiDomainIpAsync)
                        ? typeof(GetApiDomainIpResponse)
                        : typeof(GetCallbackIpResponse)),
                $"{method} 必须返回对应的具名响应 DTO（不得退化为 WechatWorkResponse，官方有 ip_list 业务负载）");
        }

        // ip_list：官方标注 StringArray；失败示例（42001）返回空数组而非缺省字段 ⇒ 非空集合承载。
        foreach (var dtoType in new[] { typeof(GetApiDomainIpResponse), typeof(GetCallbackIpResponse) })
        {
            var property = dtoType.GetProperty("IpList");
            property.Should().NotBeNull($"{dtoType.Name}.IpList 必须存在");
            property!.PropertyType.Should().Be(typeof(List<string>),
                $"{dtoType.Name}.IpList 官方类型为 StringArray");
            property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name.Should().Be("ip_list",
                $"{dtoType.Name}.IpList 的官方字段名必须照抄原文 ip_list");
        }

        // 两响应须继承 WechatWorkResponse（承载 errcode/errmsg/IsSuccess）。
        typeof(GetApiDomainIpResponse).Should().BeDerivedFrom<WechatWorkResponse>(
            "GetApiDomainIpResponse 必须继承 WechatWorkResponse 以承载 errcode/errmsg");
        typeof(GetCallbackIpResponse).Should().BeDerivedFrom<WechatWorkResponse>(
            "GetCallbackIpResponse 必须继承 WechatWorkResponse 以承载 errcode/errmsg");
    }
}
