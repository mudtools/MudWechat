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
using Mud.Wechat.Work.DataModels.Emergency;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 紧急通知模块（Emergency 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定
/// （发起语音电话 + 获取接听状态官方仅自建应用开放，零端点父接口 + 唯一自建子接口承载端点，
/// 继承链上不得出现代开发 / 第三方子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：对齐政民沟通巡查/居民上报族与身份验证二次验证族
/// （官方仅自建开放，零端点父接口 + 唯一自建子接口承载端点）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：紧急通知 2 个端点官方均即 POST
/// （含仅查询语义的 getstates）；<c>callee_userid</c> 在发起语音电话端点为 userid 字符串数组、
/// 在获取接听状态端点为单数字符串，两形态官方不一致，分别以各自形态承载；
/// 服务商代开发文档页（97115/97116）与自建页（91627/91628）逐字一致
/// 但权限表对代开发 / 第三方均标注「暂不支持」，不得据此补代开发子接口；
/// 获取接听状态仅支持查询七天内的 callid 状态；两端点无独立频率限制，走官方全局访问频率限制。
/// </para>
/// </remarks>
public class WechatEmergencyContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string EmergencyParentImplementationClassName = "WechatWorkEmergencyService";

    private const string EmergencyRegistryGroupName = "Emergency";

    /// <summary>
    /// 紧急通知域官方路由表（官方仅自建应用开放，2 条端点全部由唯一自建子接口承载）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] EmergencyRoutes =
    {
        // 发起语音电话（自建 91627；官方即 POST）。
        (typeof(IWechatWorkInternalEmergencyService),
            nameof(IWechatWorkInternalEmergencyService.CallAsync),
            typeof(PostAttribute), "/cgi-bin/pstncc/call"),
        // 获取接听状态（自建 91628；官方即 POST，勿「顺手统一」为 GET）。
        (typeof(IWechatWorkInternalEmergencyService),
            nameof(IWechatWorkInternalEmergencyService.GetCallStatesAsync),
            typeof(PostAttribute), "/cgi-bin/pstncc/getstates"),
    };

    /// <summary>
    /// 契约守卫 EM1：紧急通知域全部端点路由必须与官方契约一致
    /// （2 个端点官方仅自建应用开放，全部由唯一自建子接口承载；官方即 POST，勿「顺手统一」为 GET）。
    /// </summary>
    [Fact]
    public void EmergencyEndpoints_ShouldMatchOfficialRoutes()
    {
        EmergencyRoutes.Should().HaveCount(2,
            "紧急通知域发起语音电话 + 获取接听状态官方仅自建应用开放，全部由唯一自建子接口承载");

        var distinctRoutes = EmergencyRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(2, "紧急通知域各端点路由互不重复");
        distinctRoutes.Should().OnlyContain(r => r.StartsWith("/cgi-bin/pstncc/", StringComparison.Ordinal),
            "紧急通知域全部端点路由位于 /cgi-bin/pstncc/ 段");

        AssertRoutes(EmergencyRoutes);
    }

    /// <summary>
    /// 契约守卫 EM2：接口层级与生成器注册形态——官方权限表对代开发 / 第三方应用均标注「暂不支持」，
    /// 继承链上必须恰好只有唯一自建子接口（能力漂移守卫，代开发文档页存在亦不得补子接口）。
    /// </summary>
    [Fact]
    public void EmergencyInterfaceHierarchy_ShouldConvergeOnSelfBuildOnlyChild()
    {
        var assignable = typeof(IWechatWorkEmergencyService).Assembly.GetTypes()
            .Where(t => t.IsInterface && t != typeof(IWechatWorkEmergencyService)
                        && typeof(IWechatWorkEmergencyService).IsAssignableFrom(t))
            .ToList();

        assignable.Should().BeEquivalentTo(new[] { typeof(IWechatWorkInternalEmergencyService) },
            "紧急通知域官方仅向自建应用开放（代开发/第三方暂不支持），继承链上不得出现其它子接口");

        var parent = typeof(IWechatWorkEmergencyService);
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("官方仅自建开放：父接口零端点，端点全部由唯一自建子接口承载");

        var child = typeof(IWechatWorkInternalEmergencyService);
        var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
        childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        childApi!.RegistryGroupName.Should().Be(EmergencyRegistryGroupName,
            $"{child.Name} 必须挂 {EmergencyRegistryGroupName} 注册组" +
            $"（Emergency 模块共用 Add{EmergencyRegistryGroupName}WebApiHttpClient()）");
        childApi.InheritedFrom.Should().Be(EmergencyParentImplementationClassName,
            $"{child.Name} 必须继承父接口生成实现类");

        child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, $"{child.Name} 承载本域全部 2 条官方端点");
    }

    /// <summary>
    /// 契约守卫 EM3：令牌绑定——紧急通知模块父子两接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；自建为应用自身令牌）。
    /// </summary>
    [Fact]
    public void EmergencyTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkEmergencyService),
            typeof(IWechatWorkInternalEmergencyService),
        };

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（应用自身令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 EM4：紧急通知模块的请求/响应 DTO 必须全部登记进 AOT JSON 上下文
    /// （5 个契约面类型落 EmergencyJsonContext，SerializerClassName 统一为 Emergency；
    /// 两端点均有业务负载响应体，无空响应 DTO）。
    /// </summary>
    [Fact]
    public void EmergencyDataModels_ShouldBeRegisteredInJsonContext()
    {
        var emergencyContext = EmergencyJsonContext.Default;

        // 端点级请求/响应 DTO 全量清单（守卫是权威描述，新增/删减端点须同批更新）。
        var endpointContractTypes = new Type[]
        {
            // 发起语音电话。
            typeof(EmergencyCallRequest), typeof(EmergencyCallResponse), typeof(EmergencyCallState),
            // 获取接听状态。
            typeof(EmergencyGetCallStatesRequest), typeof(EmergencyGetCallStatesResponse),
        };

        foreach (var type in endpointContractTypes)
        {
            emergencyContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是紧急通知模块端点契约面类型，必须登记进 EmergencyJsonContext（AOT 源生成）");
        }

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 EmergencyJsonContext 且 SerializerClassName 统一为 Emergency
        //（生成物 EmergencyJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        var domainTypes = typeof(EmergencyCallState).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Emergency"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        domainTypes.Should().HaveCount(5,
            "紧急通知模块契约面类型数漂移须先核对官方文档再同批调整本守卫");
        domainTypes.Should().Contain(endpointContractTypes,
            "端点级请求/响应 DTO 必须落位于紧急通知域命名空间");

        foreach (var type in domainTypes)
        {
            emergencyContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于紧急通知域命名空间，必须登记进 EmergencyJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Emergency",
                $"{type.Name} 的 SerializerClassName 必须为紧急通知域段 Emergency");
        }
    }

    /// <summary>
    /// 契约守卫 EM5：官方契约陷阱锁定——<c>callee_userid</c> 在发起语音电话端点为字符串数组、
    /// 在获取接听状态端点为单数字符串，两形态分别以各自形态承载；字段名照抄官方原文。
    /// </summary>
    [Fact]
    public void EmergencyDataModels_ShouldLockOfficialContractTraps()
    {
        // 发起语音电话：callee_userid 官方参数名单数、值为 userid 字符串数组（批量被呼叫）。
        typeof(EmergencyCallRequest).GetProperty(nameof(EmergencyCallRequest.CalleeUserid))!
            .PropertyType.Should().Be(typeof(List<string>),
                "发起语音电话 callee_userid 官方请求示例按字符串数组传输");
        JsonNameShouldBe(typeof(EmergencyCallRequest), nameof(EmergencyCallRequest.CalleeUserid), "callee_userid");

        // 获取接听状态：callee_userid 为单数字符串（单次查询指定单人），区别于发起语音电话端点的数组形态。
        typeof(EmergencyGetCallStatesRequest).GetProperty(nameof(EmergencyGetCallStatesRequest.CalleeUserid))!
            .PropertyType.Should().Be(typeof(string),
                "获取接听状态 callee_userid 官方请求示例按单数字符串传输");
        JsonNameShouldBe(typeof(EmergencyGetCallStatesRequest), nameof(EmergencyGetCallStatesRequest.CalleeUserid), "callee_userid");
        JsonNameShouldBe(typeof(EmergencyGetCallStatesRequest), nameof(EmergencyGetCallStatesRequest.Callid), "callid");

        // 字段名照抄官方原文。
        JsonNameShouldBe(typeof(EmergencyCallResponse), nameof(EmergencyCallResponse.States), "states");
        JsonNameShouldBe(typeof(EmergencyCallState), nameof(EmergencyCallState.Code), "code");
        JsonNameShouldBe(typeof(EmergencyCallState), nameof(EmergencyCallState.Callid), "callid");
        JsonNameShouldBe(typeof(EmergencyCallState), nameof(EmergencyCallState.Userid), "userid");
        JsonNameShouldBe(typeof(EmergencyGetCallStatesResponse), nameof(EmergencyGetCallStatesResponse.Istalked), "istalked");
        JsonNameShouldBe(typeof(EmergencyGetCallStatesResponse), nameof(EmergencyGetCallStatesResponse.Calltime), "calltime");
        JsonNameShouldBe(typeof(EmergencyGetCallStatesResponse), nameof(EmergencyGetCallStatesResponse.Talktime), "talktime");
        JsonNameShouldBe(typeof(EmergencyGetCallStatesResponse), nameof(EmergencyGetCallStatesResponse.Reason), "reason");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
    private static void AssertRoutes((Type Interface, string Method, Type HttpAttribute, string Route)[] routes)
    {
        foreach (var (iface, method, httpAttribute, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致（拼写差异属官方契约）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
