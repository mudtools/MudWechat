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
using Mud.Wechat.Work.DataModels.Contacts.ContactRules;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 通讯录查看权限管理域（Contact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 与成员/部门/标签域的差异点：官方<b>仅向企业自建应用</b>开放本域 4 个端点
/// （第三方应用与服务商代开发均无对应文档），因此父接口为零端点抽象、全部端点收敛于自建子接口
/// <see cref="IWechatWorkInternalContactRulesService"/>，且不设第三方/代开发子接口
/// （应用类型子接口仅覆盖官方实际开放的应用类型）。
/// </para>
/// </remarks>
public class WechatContactRulesContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// 自建子接口继承父实现类——父接口无端点，端点由自建子接口自身声明。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkContactRulesService";

    private const string ContactRegistryGroupName = "Contact";

    /// <summary>官方路由表（4 个端点全部声明于自建子接口；官方无第三方/代开发文档）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkInternalContactRulesService), nameof(IWechatWorkInternalContactRulesService.CreateContactRulesAsync), typeof(PostAttribute), "/cgi-bin/contactrule/create"),
        (typeof(IWechatWorkInternalContactRulesService), nameof(IWechatWorkInternalContactRulesService.GetContactRulesAsync), typeof(PostAttribute), "/cgi-bin/contactrule/list"),
        (typeof(IWechatWorkInternalContactRulesService), nameof(IWechatWorkInternalContactRulesService.UpdateContactRulesAsync), typeof(PostAttribute), "/cgi-bin/contactrule/update"),
        (typeof(IWechatWorkInternalContactRulesService), nameof(IWechatWorkInternalContactRulesService.DeleteContactRulesAsync), typeof(PostAttribute), "/cgi-bin/contactrule/delete"),
    };

    /// <summary>
    /// 契约守卫 CR1：通讯录查看权限管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void ContactRuleEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(4, "官方仅向自建应用开放 4 个端点（创建/读取列表/修改/删除规则）");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：端点必须落在自建子接口自身声明，而非从父接口继承。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 CR2：接口层级与生成器注册形态——官方仅自建应用开放本域，父接口零端点（IsAbstract），
    /// 全部端点收敛自建子接口；官方未向第三方/代开发开放本域，不设对应子接口
    /// （应用类型子接口仅覆盖官方实际开放的应用类型，能力漂移守卫）。
    /// </summary>
    [Fact]
    public void ContactRulesInterfaceHierarchy_ShouldConvergeOnInternalChildWithContactRegistry()
    {
        var parent = typeof(IWechatWorkContactRulesService);
        var internalChild = typeof(IWechatWorkInternalContactRulesService);

        internalChild.Should().BeAssignableTo(parent, $"{internalChild.Name} 必须继承公共父接口 {parent.Name}");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("官方仅向自建应用开放本域端点，父接口不存在公共面，不得声明端点");

        var internalApi = internalChild.GetCustomAttribute<HttpClientApiAttribute>();
        internalApi.Should().NotBeNull($"{internalChild.Name} 必须声明 [HttpClientApi]");
        internalApi!.RegistryGroupName.Should().Be(ContactRegistryGroupName,
            $"{internalChild.Name} 必须挂 Contact 注册组（与成员/部门/标签域共用 Add{ContactRegistryGroupName}WebApiHttpClient()）");
        internalApi.InheritedFrom.Should().Be(ParentImplementationClassName,
            $"{internalChild.Name} 必须继承父接口生成实现类（父接口无端点，此处仅约束生成器继承链）");
        internalChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(4, "全部 4 个端点必须声明在自建子接口");
    }

    /// <summary>
    /// 契约守卫 CR3：令牌绑定——父/自建两接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void ContactRulesTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkContactRulesService),
            typeof(IWechatWorkInternalContactRulesService),
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
    /// 契约守卫 CR4：通讯录查看权限管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void ContactRulesDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = ContactRulesJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(ContactRuleRange), typeof(ContactRule),
            typeof(CreateContactRulesRequest), typeof(UpdateContactRulesRequest),
            typeof(DeleteContactRulesRequest), typeof(CreateContactRulesResponse),
            typeof(UpdateContactRulesResponse), typeof(GetContactRulesResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是通讯录查看权限管理域契约面类型，必须登记进 ContactRulesJsonContext（AOT 源生成）");
        }
    }
}
