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
using Mud.Wechat.Work.DataModels.ExternalContact.JobInheritance;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「在职继承」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放完全一致的 3 个端点（分配在职成员的客户、查询客户接替状态、
/// 分配在职成员的客户群）收敛于父接口 <see cref="IWechatWorkExternalContactJobInheritanceService"/>；
/// 自建 / 第三方 / 代开发子接口均为零差异端点空标记（代开发文档树 96325 / 96327 / 96328 与自建 / 第三方同路由）。
/// </para>
/// </remarks>
public class WechatExternalContactJobInheritanceContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactJobInheritanceService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 官方路由表（父接口 3 条公共端点，全部 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkExternalContactJobInheritanceService),
            nameof(IWechatWorkExternalContactJobInheritanceService.TransferCustomerAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/transfer_customer"),
        (typeof(IWechatWorkExternalContactJobInheritanceService),
            nameof(IWechatWorkExternalContactJobInheritanceService.GetTransferResultAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/transfer_result"),
        (typeof(IWechatWorkExternalContactJobInheritanceService),
            nameof(IWechatWorkExternalContactJobInheritanceService.TransferGroupChatAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/groupchat/onjob_transfer"),
    };

    /// <summary>
    /// 契约守卫 JI1：在职继承域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意在职客户群接替为 <c>groupchat/onjob_transfer</c>（"onjob" 拼写属官方契约；
    /// 离职继承的群接替为 <c>groupchat/transfer</c>，勿混淆）。
    /// </summary>
    [Fact]
    public void JobInheritanceEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(3,
            "本域 3 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：公共端点必须落在父接口自身声明，不得下沉到子接口重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 JI2：接口层级与生成器注册形态——公共端点收敛父接口（IsAbstract），
    /// 自建 / 第三方 / 代开发三个子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void JobInheritanceInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactJobInheritanceService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactJobInheritanceService),
            typeof(IWechatWorkThirdPartyExternalContactJobInheritanceService),
            typeof(IWechatWorkProviderExternalContactJobInheritanceService),
        };

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");
        }

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var child in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(ExternalContactRegistryGroupName,
                $"{child.Name} 必须挂 {ExternalContactRegistryGroupName} 注册组" +
                $"（与企业服务人员管理域、客户管理域、客户标签管理域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 JI1/JI2。
        foreach (var child in children)
        {
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 JI3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void JobInheritanceTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactJobInheritanceService),
            typeof(IWechatWorkInternalExternalContactJobInheritanceService),
            typeof(IWechatWorkThirdPartyExternalContactJobInheritanceService),
            typeof(IWechatWorkProviderExternalContactJobInheritanceService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 JI4：在职继承域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void JobInheritanceDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = JobInheritanceJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(TransferCustomerRequest), typeof(TransferCustomerResultItem), typeof(TransferCustomerResponse),
            typeof(GetTransferResultRequest), typeof(CustomerTakeoverStatusItem), typeof(GetTransferResultResponse),
            typeof(TransferGroupChatRequest), typeof(TransferGroupChatFailItem), typeof(TransferGroupChatResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是在职继承域契约面类型，必须登记进 JobInheritanceJsonContext（AOT 源生成）");
        }
    }
}
