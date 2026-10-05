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
using Mud.Wechat.Work.DataModels.JsSdk;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// JS-SDK 模块（JsSdk 模块）契约守卫：路由表、固定 Query、接口层级、令牌绑定
/// 与 JSON 上下文全量登记锁定（获取企业 jsapi_ticket + 获取应用 jsapi_ticket）。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对自建应用（90506）、第三方应用（90539）与服务商代开发（96909）开放
/// 逐字一致的「JS-SDK 签名算法」文档页，2 个端点为三类应用公共面——
/// 全部收敛声明于父接口（IsAbstract），自建/第三方/代开发均为零差异端点空标记子接口。
/// JS-SDK 签名算法本身为页面/服务器端 SHA-1 约定，不属于 HTTP 端点面，不在本守卫锁定范围。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：获取应用 jsapi_ticket 的请求地址以固定 Query 值
/// <c>type=agent_config</c> 携带鉴权类型（官方参数表不再单列该参数），SDK 以方法级固定
/// <c>[Query("type", "agent_config")]</c> 发射，勿改成调用方可变参数；两端点官方即 GET、无请求体；
/// get_jsapi_ticket 路由不在 <c>ticket/get</c> 之下（勿「顺手统一」成 ticket/get?type=jsapi 形态——
/// 企业 ticket 官方路由即独立的 get_jsapi_ticket）。
/// </para>
/// </remarks>
public class WechatJsSdkContractGuards
{
    private const string JsSdkRegistryGroupName = "JsSdk";

    /// <summary>
    /// JS-SDK 族父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string JsSdkParentImplementationClassName = "WechatWorkJsSdkService";

    /// <summary>
    /// JS-SDK 族官方路由表（父接口 2 条公共端点：get_jsapi_ticket GET + ticket/get GET）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] JsSdkRoutes =
    {
        // 获取企业 jsapi_ticket（自建 90506 / 第三方 90539 / 代开发 96909），官方即 GET、无请求体。
        (typeof(IWechatWorkJsSdkService),
            nameof(IWechatWorkJsSdkService.GetCorpJsapiTicketAsync),
            typeof(GetAttribute), "/cgi-bin/get_jsapi_ticket"),
        // 获取应用 jsapi_ticket（同上三份文档），官方即 GET，固定 Query type=agent_config 由方法级特性发射。
        (typeof(IWechatWorkJsSdkService),
            nameof(IWechatWorkJsSdkService.GetAgentJsapiTicketAsync),
            typeof(GetAttribute), "/cgi-bin/ticket/get"),
    };

    /// <summary>
    /// 契约守卫 JSS1：JS-SDK 族端点路由必须与官方契约一致
    /// （新增/改名端点须同批更新本表；两端点官方即 GET，勿改成 POST）。
    /// </summary>
    [Fact]
    public void JsSdkEndpoints_ShouldMatchOfficialRoutes()
    {
        JsSdkRoutes.Should().HaveCount(2,
            "JS-SDK 族 2 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = JsSdkRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "JS-SDK 族各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in JsSdkRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 获取应用 jsapi_ticket：官方请求地址固定携带 type=agent_config，须以方法级固定
        // [Query("type", "agent_config")] 发射（官方参数表不再单列该参数，不得改为调用方可变参数）。
        // QueryAttribute 无 Value 属性（第二个位置参数在属性面映射为 Format），生成器固定值
        // 取自构造位置参数 [1]，故本断言走 CustomAttributeData 构造参数。
        var getAgentTicket = typeof(IWechatWorkJsSdkService).GetMethod(
            nameof(IWechatWorkJsSdkService.GetAgentJsapiTicketAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        getAgentTicket.Should().NotBeNull();
        var fixedTypeQueryData = getAgentTicket!.GetCustomAttributesData()
            .SingleOrDefault(d => d.AttributeType == typeof(QueryAttribute)
                                  && d.ConstructorArguments.Count > 0
                                  && string.Equals(d.ConstructorArguments[0].Value as string, "type", StringComparison.Ordinal));
        fixedTypeQueryData.Should().NotBeNull("ticket/get 必须以方法级 [Query(\"type\", \"agent_config\")] 固定鉴权类型");
        fixedTypeQueryData!.ConstructorArguments.Should().HaveCount(2,
            "方法级固定 Query 须以双位置参数 (name, value) 声明");
        fixedTypeQueryData.ConstructorArguments[1].Value.Should().Be("agent_config",
            "type 官方固定值为 agent_config（勿改成可变参数或其它取值）");

        // 获取企业 jsapi_ticket：官方无业务参数（令牌经 [Token] Query 注入），不得引入方法级固定 Query。
        var getCorpTicket = typeof(IWechatWorkJsSdkService).GetMethod(
            nameof(IWechatWorkJsSdkService.GetCorpJsapiTicketAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        getCorpTicket.Should().NotBeNull();
        getCorpTicket!.GetCustomAttributes<QueryAttribute>()
            .Should().BeEmpty("get_jsapi_ticket 官方无业务 Query 参数（令牌经 [Token] 注入，不在方法级固定 Query 之列）");
        getCorpTicket.GetParameters().Should().ContainSingle(
            p => p.ParameterType == typeof(CancellationToken),
            "get_jsapi_ticket 无请求体亦无业务 Query 参数，签名仅剩取消令牌");
    }

    /// <summary>
    /// 契约守卫 JSS2：接口层级与生成器注册形态——2 个端点收敛于 IsAbstract 父接口，
    /// 自建/第三方/代开发均为零端点空标记子接口（继承链恰 3 个子接口、注册组统一 JsSdk）。
    /// </summary>
    [Fact]
    public void JsSdkInterfaceHierarchy_ShouldConvergeOnJsSdkRegistry()
    {
        var parent = typeof(IWechatWorkJsSdkService);
        var children = new[]
        {
            typeof(IWechatWorkInternalJsSdkService),
            typeof(IWechatWorkProviderJsSdkService),
            typeof(IWechatWorkThirdPartyJsSdkService),
        };

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, "JS-SDK 族父接口恰持 get_jsapi_ticket/ticket/get 2 条公共端点");

        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .OrderBy(t => t.Name, StringComparer.Ordinal).ToList();
        derived.Should().BeEquivalentTo(children, "JS-SDK 族继承链恰为自建/第三方/代开发三个空标记子接口");

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");

            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(JsSdkRegistryGroupName,
                $"{child.Name} 必须挂 {JsSdkRegistryGroupName} 注册组" +
                $"（JsSdk 模块共用 Add{JsSdkRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(JsSdkParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类 {JsSdkParentImplementationClassName}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(0, $"{child.Name} 为空标记子接口，不承载差异端点");
        }
    }

    /// <summary>
    /// 契约守卫 JSS3：令牌绑定——JS-SDK 族 4 个接口统一消费 AccessToken 路由键（access_token），
    /// 全部 Query 注入（归属域键由 WechatTokenOwnerContractGuards 全局锁定，此处不重复）。
    /// </summary>
    [Fact]
    public void JsSdkTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkJsSdkService),
            typeof(IWechatWorkInternalJsSdkService),
            typeof(IWechatWorkProviderJsSdkService),
            typeof(IWechatWorkThirdPartyJsSdkService),
        };

        interfaces.Should().HaveCount(4, "JS-SDK 族 = 父接口 + 自建/第三方/代开发三个空标记子接口");

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 JSS4：JS-SDK 模块的请求/响应 DTO 必须全量登记进 AOT JSON 上下文
    ///（SerializerClassName 统一为 JsSdk）。
    /// </summary>
    [Fact]
    public void JsSdkDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = JsSdkJsonContext.Default;

        var domainTypes = typeof(GetCorpJsapiTicketResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.JsSdk"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 JsSdkJsonContext
        //（企业 ticket 响应 1 + 应用 ticket 响应 1；两端点均无请求体、不设请求 DTO，
        // errcode/errmsg 由 WechatWorkResponse 基类承载、落 CommonJsonContext 不计入本域）。
        domainTypes.Should().HaveCount(2,
            "JS-SDK 模块契约面类型数漂移须先核对官方文档再同批调整本守卫");

        foreach (var type in domainTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 JS-SDK 域命名空间，必须登记进 JsSdkJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("JsSdk",
                $"{type.Name} 的 SerializerClassName 必须为 JS-SDK 域段 JsSdk");
        }
    }
}
