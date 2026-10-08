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
using Mud.Wechat.Work.DataModels.ExternalContact.Customer;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「客户管理」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放一致的 10 个端点（获取客户列表、获取客户详情、批量获取客户详情、修改客户备注信息、
/// 客户联系规则组 6 端点）收敛于父接口 <see cref="IWechatWorkExternalContactCustomerService"/>；
/// 自建与代开发子接口均为零差异端点空标记；第三方子接口独有 3 条身份转换差异端点
/// （unionid 转换、external_userid 查询 pending_id、代开发 external_userid 转换）。
/// </para>
/// </remarks>
public class WechatExternalContactCustomerContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactCustomerService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 官方路由表（父接口 10 条公共端点 + 第三方 1 条差异端点）。
    /// <para><b>单一所有者</b>：<c>/cgi-bin/idconvert/unionid_to_external_userid</c>与
    /// <c>/cgi-bin/idconvert/batch/external_userid_to_pending_id</c> 归「账号ID」域
    /// <see cref="IWechatWorkAccountIdService"/> 唯一声明（其载荷为本域原平行家族的可空超集）。</para>
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.GetCustomerListAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/list"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.GetCustomerDetailAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/get"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.BatchGetCustomerDetailsAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/batch/get_by_user"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.UpdateCustomerRemarkAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/remark"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.GetCustomerStrategyListAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_strategy/list"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.GetCustomerStrategyDetailAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_strategy/get"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.GetCustomerStrategyRangeAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_strategy/get_range"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.CreateCustomerStrategyAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_strategy/create"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.UpdateCustomerStrategyAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_strategy/edit"),
        (typeof(IWechatWorkExternalContactCustomerService),
            nameof(IWechatWorkExternalContactCustomerService.DeleteCustomerStrategyAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/customer_strategy/del"),
        (typeof(IWechatWorkThirdPartyExternalContactCustomerService),
            nameof(IWechatWorkThirdPartyExternalContactCustomerService.ConvertToServiceExternalUserIdAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/to_service_external_userid"),
    };

    /// <summary>
    /// 契约守卫 CU1：客户管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意批量获取详情为 <c>batch/get_by_user</c>，与异步导入的 <c>batch/getresult</c>、异步导出的
    /// <c>export/get_result</c> 均不同（拼写差异属官方契约）。
    /// </summary>
    [Fact]
    public void CustomerEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(11,
            "本域为父接口 10 条公共端点 + 第三方 1 条差异端点（unionid/pending_id 转换归「账号ID」域单一所有者）");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(11, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：差异端点必须落在所属接口自身声明，父接口端点不得下沉到子接口重复声明。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 CU2：接口层级与生成器注册形态——公共端点收敛父接口（IsAbstract），
    /// 自建/代开发子接口为零差异端点空标记（能力漂移守卫），第三方子接口恰持 3 条差异端点。
    /// </summary>
    [Fact]
    public void CustomerInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactCustomerService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactCustomerService),
            typeof(IWechatWorkThirdPartyExternalContactCustomerService),
            typeof(IWechatWorkProviderExternalContactCustomerService),
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
                $"（与企业服务人员管理域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 自建子接口零差异端点：官方未向自建开放身份转换端点。
        typeof(IWechatWorkInternalExternalContactCustomerService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty(
                "自建子接口为空标记：公共端点全部声明于父接口，新增差异端点须先核对官方文档并同批调整 CU1/CU2");

        // 第三方子接口恰持 1 条差异端点（代开发 external_userid 转换）；
        // unionid 转换 / pending_id 查询归「账号ID」域 IWechatWorkAccountIdService 单一所有者。
        typeof(IWechatWorkThirdPartyExternalContactCustomerService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, "第三方子接口仅承载 1 条身份转换差异端点（其余归「账号ID」域单一所有者）");

        // 代开发子接口零差异端点（官方文档未向代开发单列差异端点）。
        typeof(IWechatWorkProviderExternalContactCustomerService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("代开发子接口为空标记：公共端点全部声明于父接口");
    }

    /// <summary>
    /// 契约守卫 CU3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void CustomerTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactCustomerService),
            typeof(IWechatWorkInternalExternalContactCustomerService),
            typeof(IWechatWorkThirdPartyExternalContactCustomerService),
            typeof(IWechatWorkProviderExternalContactCustomerService),
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
    /// 契约守卫 CU4：客户管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void CustomerDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = CustomerJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(ExternalContactInfo), typeof(ExternalProfile), typeof(ExternalAttr),
            typeof(ExternalAttrText), typeof(ExternalAttrWeb), typeof(ExternalAttrMiniProgram),
            typeof(CustomerFollowTag), typeof(WechatChannelsInfo),
            typeof(CustomerFollowUser), typeof(CustomerFollowInfo),
            typeof(GetCustomerListResponse), typeof(GetCustomerDetailResponse),
            typeof(BatchCustomerContactItem), typeof(BatchCustomerFailInfo), typeof(BatchGetCustomerDetailsResponse),
            typeof(CustomerStrategy), typeof(CustomerStrategyPrivilege), typeof(CustomerStrategyRangeNode),
            typeof(GetCustomerStrategyListResponse), typeof(CustomerStrategyIdItem),
            typeof(CreateCustomerStrategyResponse), typeof(GetCustomerStrategyDetailResponse),
            typeof(GetCustomerStrategyRangeResponse),
            typeof(ConvertToServiceExternalUserIdResponse),
            typeof(BatchGetCustomerDetailsRequest), typeof(UpdateCustomerRemarkRequest),
            typeof(GetCustomerStrategyListRequest), typeof(GetCustomerStrategyDetailRequest),
            typeof(GetCustomerStrategyRangeRequest), typeof(CreateCustomerStrategyRequest),
            typeof(UpdateCustomerStrategyRequest), typeof(DeleteCustomerStrategyRequest),
            typeof(ConvertToServiceExternalUserIdRequest),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是客户管理域契约面类型，必须登记进 CustomerJsonContext（AOT 源生成）");
        }
    }
}
