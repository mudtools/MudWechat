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
using Mud.Wechat.Work.DataModels.Tags;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 标签管理域（Contact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 与成员 / 部门域的差异点：官方对自建、第三方、代开发三类应用开放了<b>完全一致的 7 个标签端点</b>
/// （含创建 / 更新 / 删除与增删成员），因此全部端点收敛于父接口
/// <see cref="IWechatWorkTagsService"/>，三个应用类型子接口均为<b>空标记</b>（能力集合与公共面重合，
/// 声明于任一子接口都意味着能力漂移，须先核对官方文档再落位）。
/// </para>
/// </remarks>
public class WechatTagsContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkTagsService";

    private const string ContactRegistryGroupName = "Contact";

    /// <summary>官方路由表（7 个端点全部声明于父接口；三类应用官方文档路由完全一致）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkTagsService), nameof(IWechatWorkTagsService.CreateTagAsync), typeof(PostAttribute), "/cgi-bin/tag/create"),
        (typeof(IWechatWorkTagsService), nameof(IWechatWorkTagsService.UpdateTagNameAsync), typeof(PostAttribute), "/cgi-bin/tag/update"),
        (typeof(IWechatWorkTagsService), nameof(IWechatWorkTagsService.DeleteTagAsync), typeof(GetAttribute), "/cgi-bin/tag/delete"),
        (typeof(IWechatWorkTagsService), nameof(IWechatWorkTagsService.GetTagMembersAsync), typeof(GetAttribute), "/cgi-bin/tag/get"),
        (typeof(IWechatWorkTagsService), nameof(IWechatWorkTagsService.AddTagMembersAsync), typeof(PostAttribute), "/cgi-bin/tag/addtagusers"),
        (typeof(IWechatWorkTagsService), nameof(IWechatWorkTagsService.RemoveTagMembersAsync), typeof(PostAttribute), "/cgi-bin/tag/deltagusers"),
        (typeof(IWechatWorkTagsService), nameof(IWechatWorkTagsService.GetTagListAsync), typeof(GetAttribute), "/cgi-bin/tag/list"),
    };

    /// <summary>
    /// 契约守卫 T1：标签管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void TagEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(7, "官方对三类应用开放一致的 7 个标签端点，全部声明于父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(7, "标签域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：端点必须落在父接口自身声明，而非从子接口继承。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 T2：接口层级与生成器注册形态——全部端点收敛父接口（IsAbstract），
    /// 三个应用类型子接口均为空标记（能力漂移守卫：任何子接口不得新增端点）。
    /// </summary>
    [Fact]
    public void TagsInterfaceHierarchy_ShouldConvergeOnAbstractParentWithContactRegistry()
    {
        var parent = typeof(IWechatWorkTagsService);
        var children = new[]
        {
            typeof(IWechatWorkInternalTagsService),
            typeof(IWechatWorkThirdPartyTagsService),
            typeof(IWechatWorkProviderTagsService),
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
            childApi!.RegistryGroupName.Should().Be(ContactRegistryGroupName,
                $"{child.Name} 必须挂 Contact 注册组（与成员/部门管理域共用 Add{ContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            // 官方对三类应用开放一致的端点集：任何子接口不得新增端点（能力集合漂移守卫）。
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty(
                    $"{child.Name} 为应用类型空标记：7 个标签端点官方对三类应用一致开放，" +
                    "新增差异端点须先核对官方文档并同批调整 T1/T2");
        }
    }

    /// <summary>
    /// 契约守卫 T3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void TagsTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkTagsService),
            typeof(IWechatWorkInternalTagsService),
            typeof(IWechatWorkThirdPartyTagsService),
            typeof(IWechatWorkProviderTagsService),
        };

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（三类应用共用键，按应用上下文/scope 路由）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    /// <summary>
    /// 契约守卫 T4：标签管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void TagsDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = Mud.Wechat.Work.DataModels.WechatWorkJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(TagInfo), typeof(TagMemberInfo),
            typeof(CreateTagRequest), typeof(UpdateTagNameRequest),
            typeof(AddTagMembersRequest), typeof(RemoveTagMembersRequest),
            typeof(CreateTagResponse), typeof(GetTagMembersResponse),
            typeof(ChangeTagMembersResponse), typeof(GetTagListResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是标签管理域契约面类型，必须登记进 WechatWorkJsonContext（AOT 源生成）");
        }
    }
}
