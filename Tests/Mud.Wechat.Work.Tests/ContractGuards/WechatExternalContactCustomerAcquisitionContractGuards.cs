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
using Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「获客助手」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放完全一致的 9 个端点（链接管理 5 条 + 获客客户 1 条 +
/// 额度与统计 2 条 + 成员多次收消息详情 1 条）收敛于父接口
/// <see cref="IWechatWorkExternalContactCustomerAcquisitionService"/>；
/// 自建 / 第三方 / 代开发子接口均为零差异端点空标记。
/// </para>
/// </remarks>
public class WechatExternalContactCustomerAcquisitionContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactCustomerAcquisitionService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 官方路由表（父接口 9 条公共端点；customer_acquisition_quota 为 GET，其余 8 条为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // 获客链接管理族（97297/97394/97398）。
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.GetAcquisitionLinkListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/list_link"),
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.GetAcquisitionLinkAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/get"),
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.CreateAcquisitionLinkAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/create_link"),
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.UpdateAcquisitionLinkAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/update_link"),
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.DeleteAcquisitionLinkAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/delete_link"),
        // 获客客户列表（97298/97395/97399）。
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.GetAcquisitionCustomerListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/customer"),
        // 额度与使用统计（97375/97396/97400）。
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.GetAcquisitionQuotaAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/customer_acquisition_quota"),
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.GetAcquisitionLinkStatisticAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/statistic"),
        // 成员多次收消息详情（100130/100134/100133）。
        (typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            nameof(IWechatWorkExternalContactCustomerAcquisitionService.GetAcquisitionChatInfoAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_acquisition/get_chat_info"),
    };

    /// <summary>
    /// 契约守卫 CA1：获客助手域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意 <c>customer_acquisition/get</c>（链接详情）与 <c>customer_acquisition/customer</c>（客户列表）
    /// 是两条不同路由；quota 查询路由在 <c>externalcontact/customer_acquisition_quota</c>（无
    /// <c>customer_acquisition/</c> 前缀子路径），与其余端点形状不同。
    /// </summary>
    [Fact]
    public void CustomerAcquisitionEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(9,
            "本域 9 个端点为三类应用公共面，全部收敛父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(9, "本域各端点路由互不重复");

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
    /// 契约守卫 CA2：接口层级与生成器注册形态——公共端点收敛父接口（IsAbstract），
    /// 自建 / 第三方 / 代开发三个子接口均为零差异端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void CustomerAcquisitionInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactCustomerAcquisitionService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactCustomerAcquisitionService),
            typeof(IWechatWorkThirdPartyExternalContactCustomerAcquisitionService),
            typeof(IWechatWorkProviderExternalContactCustomerAcquisitionService),
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
                $"（与既有的六个客户联系域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 三类应用开放面完全一致：任何子接口出现差异端点均为能力漂移，须先核对官方文档再落位并同批调整 CA1/CA2。
        foreach (var child in children)
        {
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
        }
    }

    /// <summary>
    /// 契约守卫 CA3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void CustomerAcquisitionTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactCustomerAcquisitionService),
            typeof(IWechatWorkInternalExternalContactCustomerAcquisitionService),
            typeof(IWechatWorkThirdPartyExternalContactCustomerAcquisitionService),
            typeof(IWechatWorkProviderExternalContactCustomerAcquisitionService),
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
    /// 契约守卫 CA4：获客助手域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void CustomerAcquisitionDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = CustomerAcquisitionJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(GetAcquisitionLinkListRequest), typeof(GetAcquisitionLinkDetailRequest),
            typeof(CreateAcquisitionLinkRequest), typeof(UpdateAcquisitionLinkRequest),
            typeof(DeleteAcquisitionLinkRequest), typeof(GetAcquisitionCustomerListRequest),
            typeof(GetAcquisitionLinkStatisticRequest), typeof(GetAcquisitionChatInfoRequest),
            typeof(AcquisitionLinkRange), typeof(AcquisitionPriorityOption), typeof(AcquisitionLink),
            typeof(GetAcquisitionLinkListResponse), typeof(GetAcquisitionLinkDetailResponse),
            typeof(CreateAcquisitionLinkResponse), typeof(GetAcquisitionCustomerListResponse),
            typeof(AcquisitionCustomerItem), typeof(GetAcquisitionQuotaResponse), typeof(AcquisitionQuotaItem),
            typeof(GetAcquisitionLinkStatisticResponse), typeof(GetAcquisitionChatInfoResponse),
            typeof(AcquisitionChatInfo),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是获客助手域契约面类型，必须登记进 CustomerAcquisitionJsonContext（AOT 源生成）");
        }
    }
}
