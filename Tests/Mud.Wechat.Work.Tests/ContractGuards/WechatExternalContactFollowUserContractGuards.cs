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
using Mud.Wechat.Work.DataModels.ExternalContact.FollowUser;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 客户联系「企业服务人员管理」域（ExternalContact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态：官方对三类应用开放的 <c>get_follow_user_list</c> 收敛于父接口
/// <see cref="IWechatWorkExternalContactFollowUserService"/>；自建子接口为零差异端点空标记；
/// 第三方子接口独有「获取客户可建联成员」（仅营销获客类应用可调用）；
/// 代开发子接口独有「检查用户是否配置了客户联系功能使用权限」。
/// </para>
/// </remarks>
public class WechatExternalContactFollowUserContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkExternalContactFollowUserService";

    private const string ExternalContactRegistryGroupName = "ExternalContact";

    /// <summary>
    /// 官方路由表（父接口 1 条公共端点 + 第三方 1 条差异端点 + 代开发 1 条差异端点）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkExternalContactFollowUserService),
            nameof(IWechatWorkExternalContactFollowUserService.GetFollowUserListAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/get_follow_user_list"),
        (typeof(IWechatWorkThirdPartyExternalContactFollowUserService),
            nameof(IWechatWorkThirdPartyExternalContactFollowUserService.GetCustomerAcquisitionPermitAsync),
            typeof(GetAttribute), "/cgi-bin/externalcontact/customer_acquisition_app/get_permit"),
        (typeof(IWechatWorkProviderExternalContactFollowUserService),
            nameof(IWechatWorkProviderExternalContactFollowUserService.CheckFollowUserAsync),
            typeof(PostAttribute), "/cgi-bin/externalcontact/check_follow_user"),
    };

    /// <summary>
    /// 契约守卫 FU1：企业服务人员管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void FollowUserEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(3,
            "本域为父接口 1 条公共端点 + 第三方/代开发各 1 条差异端点");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(3, "本域各端点路由互不重复");

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
    /// 契约守卫 FU2：接口层级与生成器注册形态——公共端点收敛父接口（IsAbstract），
    /// 自建子接口为零差异端点空标记（能力漂移守卫），第三方/代开发子接口恰持 1 条差异端点。
    /// </summary>
    [Fact]
    public void FollowUserInterfaceHierarchy_ShouldConvergeOnAbstractParentWithExternalContactRegistry()
    {
        var parent = typeof(IWechatWorkExternalContactFollowUserService);
        var children = new[]
        {
            typeof(IWechatWorkInternalExternalContactFollowUserService),
            typeof(IWechatWorkThirdPartyExternalContactFollowUserService),
            typeof(IWechatWorkProviderExternalContactFollowUserService),
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
                $"（与客户管理域共用 Add{ExternalContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        // 自建子接口零差异端点：官方未向自建开放 check_follow_user / customer_acquisition_app 端点。
        typeof(IWechatWorkInternalExternalContactFollowUserService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty(
                "自建子接口为空标记：公共端点全部声明于父接口，新增差异端点须先核对官方文档并同批调整 FU1/FU2");

        // 第三方子接口恰持 1 条差异端点（获取客户可建联成员，仅营销获客类应用）。
        typeof(IWechatWorkThirdPartyExternalContactFollowUserService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, "第三方子接口仅承载「获取客户可建联成员」1 条差异端点");

        // 代开发子接口恰持 1 条差异端点（检查用户是否配置了客户联系功能使用权限）。
        typeof(IWechatWorkProviderExternalContactFollowUserService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, "代开发子接口仅承载「检查用户是否配置了客户联系功能使用权限」1 条差异端点");
    }

    /// <summary>
    /// 契约守卫 FU3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void FollowUserTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkExternalContactFollowUserService),
            typeof(IWechatWorkInternalExternalContactFollowUserService),
            typeof(IWechatWorkThirdPartyExternalContactFollowUserService),
            typeof(IWechatWorkProviderExternalContactFollowUserService),
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
    /// 契约守卫 FU4：企业服务人员管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void FollowUserDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = FollowUserJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(GetFollowUserListResponse), typeof(CheckFollowUserRequest),
            typeof(CheckFollowUserResult), typeof(CheckFollowUserResponse),
            typeof(GetCustomerAcquisitionPermitResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是企业服务人员管理域契约面类型，必须登记进 FollowUserJsonContext（AOT 源生成）");
        }
    }
}
