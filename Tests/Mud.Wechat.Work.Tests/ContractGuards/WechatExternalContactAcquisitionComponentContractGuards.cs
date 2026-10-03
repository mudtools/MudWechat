// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels.ExternalContact.AcquisitionComponent;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「获客助手组件」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方仅向<b>第三方应用</b>开放本域（企业自建应用与服务商代开发均无对应功能），因此两个接口族的
/// 父接口均为零端点抽象、全部端点收敛于第三方子接口，且不设自建/代开发子接口
/// （形态对齐 <see cref="WechatExternalContactServedContactContractGuards"/>）。
/// </para>
/// <para>
/// 双族拆分依据：获取代支付流水（<c>/cgi-bin/service/customer_acquisition/get_bill_list</c>）官方契约以
/// <c>suite_access_token</c>（获客助手组件的应用凭证）鉴权，与本域其余端点的企业级
/// <c>access_token</c> 分属不同令牌路由键 —— 一接口族一令牌路由键。
/// </para>
/// </remarks>
public class WechatExternalContactAcquisitionComponentContractGuards
{
    /// <summary>
    /// 获客助手组件接口族父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，
    /// 落位于 <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ComponentParentImplementationClassName = "WechatWorkExternalContactAcquisitionComponentService";

    /// <summary>代支付流水接口族父接口生成实现类名（同上生成器规则）。</summary>
    private const string BillParentImplementationClassName = "WechatWorkExternalContactAcquisitionComponentBillService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 获客助手组件接口族路由表（全部 POST；组件版与自建/第三方直连版
    /// <c>CustomerAcquisition</c> 域共用路由，但仅可见授权给组件的链接，语义以官方组件文档为准）。
    /// </summary>
    private static readonly (string MethodName, string Route)[] ComponentEndpoints =
    {
        (nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService.GetAcquisitionComponentAuthInfoAsync),
            "/cgi-bin/externalcontact/customer_acquisition/get_comp_auth_info"),
        (nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService.GetAcquisitionComponentLinkListAsync),
            "/cgi-bin/externalcontact/customer_acquisition/list_link"),
        (nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService.GetAcquisitionComponentLinkDetailAsync),
            "/cgi-bin/externalcontact/customer_acquisition/get"),
        (nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService.GetAcquisitionComponentLinkStatisticAsync),
            "/cgi-bin/externalcontact/customer_acquisition/statistic"),
        (nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService.CreateAcquisitionComponentOnceKeyAsync),
            "/cgi-bin/externalcontact/customer_acquisition/create_once_key"),
        (nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService.GetAcquisitionComponentChatInfoAsync),
            "/cgi-bin/externalcontact/customer_acquisition/get_chat_info"),
    };

    /// <summary>
    /// 契约守卫 AC1：获客助手组件 6 个端点与代支付流水 1 个端点的路由必须与官方契约一致
    /// （新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void AcquisitionComponentEndpoints_ShouldMatchOfficialRoutes()
    {
        var componentChild = typeof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService);
        foreach (var (methodName, route) in ComponentEndpoints)
        {
            var method = componentChild.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            method.Should().NotBeNull($"{componentChild.Name}.{methodName} 必须存在");

            var post = method!.GetCustomAttribute<PostAttribute>();
            post.Should().NotBeNull($"{componentChild.Name}.{methodName} 必须声明 [Post] 路由");
            post!.RequestUri.Should().Be(route,
                $"{componentChild.Name}.{methodName} 路由必须与官方契约一致");
        }

        var billChild = typeof(IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService);
        var billMethod = nameof(IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService.GetAcquisitionBillListAsync);
        var billTarget = billChild.GetMethod(billMethod, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        billTarget.Should().NotBeNull($"{billChild.Name}.{billMethod} 必须存在");

        var billPost = billTarget!.GetCustomAttribute<PostAttribute>();
        billPost.Should().NotBeNull($"{billChild.Name}.{billMethod} 必须声明 [Post] 路由");
        billPost!.RequestUri.Should().Be("/cgi-bin/service/customer_acquisition/get_bill_list",
            $"{billChild.Name}.{billMethod} 路由必须与官方契约一致（注意位于 /cgi-bin/service/ 下，非 /cgi-bin/externalcontact/）");
    }

    /// <summary>
    /// 契约守卫 AC2：接口层级与生成器注册形态——官方未向自建/代开发开放本域，
    /// 两个接口族的父接口均为零端点抽象 + 全部端点收敛第三方子接口，继承链上不得出现其它子接口。
    /// </summary>
    [Fact]
    public void AcquisitionComponentInterfaceHierarchy_ShouldConvergeOnThirdPartyChildWithExternalContactRegistry()
    {
        var componentParent = typeof(IWechatWorkExternalContactAcquisitionComponentService);
        var componentChild = typeof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService);
        var billParent = typeof(IWechatWorkExternalContactAcquisitionComponentBillService);
        var billChild = typeof(IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService);

        componentChild.Should().BeAssignableTo(componentParent, $"{componentChild.Name} 必须继承公共父接口 {componentParent.Name}");
        billChild.Should().BeAssignableTo(billParent, $"{billChild.Name} 必须继承公共父接口 {billParent.Name}");

        foreach (var parent in new[] { componentParent, billParent })
        {
            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue($"{parent.Name} 不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty($"{parent.Name} 不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{parent.Name} 为零端点父接口（官方仅第三方应用开放，端点全部落第三方子接口）");
        }

        AssertRegistryChild(componentChild, ExternalContactRegistryGroupName, ComponentParentImplementationClassName);
        componentChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(ComponentEndpoints.Length, "官方仅开放 6 个组件端点，必须声明在第三方子接口");
        AssertRegistryChild(billChild, ExternalContactRegistryGroupName, BillParentImplementationClassName);
        billChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, "官方仅开放 1 个代支付流水端点，必须声明在第三方子接口");

        // 能力漂移守卫：继承链上恰好只有第三方子接口，不得出现自建/代开发子接口。
        GetAssignableInterfaces(componentParent).Should().BeEquivalentTo(new[] { componentChild },
            "获客助手组件域官方仅第三方应用开放，继承链上不得出现自建/代开发子接口");
        GetAssignableInterfaces(billParent).Should().BeEquivalentTo(new[] { billChild },
            "代支付流水接口族官方仅第三方应用开放，继承链上不得出现自建/代开发子接口");
    }

    /// <summary>
    /// 契约守卫 AC3：令牌绑定——组件接口族统一消费 AccessToken 路由键、代支付流水接口族消费
    /// SuiteAccessToken 路由键（获客助手组件的应用凭证），均以 Query 注入（官方契约）。
    /// </summary>
    [Fact]
    public void AcquisitionComponentTokenBinding_ShouldSplitBySuiteAndAccessTokenViaQuery()
    {
        var componentInterfaces = new[]
        {
            typeof(IWechatWorkExternalContactAcquisitionComponentService),
            typeof(IWechatWorkThirdPartyExternalContactAcquisitionComponentService),
        };

        foreach (var iface in componentInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（企业级令牌，按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }

        var billInterfaces = new[]
        {
            typeof(IWechatWorkExternalContactAcquisitionComponentBillService),
            typeof(IWechatWorkThirdPartyExternalContactAcquisitionComponentBillService),
        };

        foreach (var iface in billInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.SuiteAccessToken,
                $"{iface.Name} 令牌路由键必须为 SuiteAccessToken（获客助手组件的应用凭证，套件级无企业 scope）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("suite_access_token",
                $"{iface.Name} Query 注入参数名必须为官方契约的 suite_access_token（已在组件 SensitiveUrlRedactor 词表内）");
        }
    }

    /// <summary>
    /// 契约守卫 AC4：「获客助手组件」域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void AcquisitionComponentDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = AcquisitionComponentJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(GetAcquisitionComponentLinkListRequest),
            typeof(GetAcquisitionComponentLinkDetailRequest),
            typeof(GetAcquisitionComponentLinkStatisticRequest),
            typeof(CreateAcquisitionComponentOnceKeyRequest),
            typeof(GetAcquisitionComponentChatInfoRequest),
            typeof(GetAcquisitionBillListRequest),
            typeof(GetAcquisitionComponentAuthInfoResponse), typeof(AcquisitionComponentAuthApp),
            typeof(GetAcquisitionComponentLinkListResponse),
            typeof(GetAcquisitionComponentLinkDetailResponse), typeof(AcquisitionComponentLinkInfo),
            typeof(GetAcquisitionComponentLinkStatisticResponse),
            typeof(CreateAcquisitionComponentOnceKeyResponse),
            typeof(GetAcquisitionComponentChatInfoResponse), typeof(AcquisitionComponentChatInfo),
            typeof(GetAcquisitionBillListResponse), typeof(AcquisitionBillRecord),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是「获客助手组件」域契约面类型，必须登记进 AcquisitionComponentJsonContext（AOT 源生成）");
        }
    }

    private static void AssertRegistryChild(Type child, string registryGroupName, string implementationClassName)
    {
        var api = child.GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(registryGroupName,
            $"{child.Name} 必须挂 {registryGroupName} 注册组" +
            $"（与既有的客户联系各域共用 Add{registryGroupName}WebApiHttpClient()）");
        api.InheritedFrom.Should().Be(implementationClassName,
            $"{child.Name} 必须继承父接口生成实现类（父接口无端点，此处仅约束生成器继承链）");
    }

    private static List<Type> GetAssignableInterfaces(Type parent)
        => parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
}
