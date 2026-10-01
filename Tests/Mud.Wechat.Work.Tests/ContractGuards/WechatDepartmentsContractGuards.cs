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
using Mud.Wechat.Work.DataModels.Contacts.Department;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 部门管理域（Contact 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 与成员管理域同构：三类应用路由完全一致（/cgi-bin/department/*），差异仅在令牌作用域与能力集合
/// （写入端点仅自建/第三方通讯录应用；代开发只有读取面）。公共读取面收敛于父接口
/// <see cref="IWechatWorkDepartmentsService"/>，写入端点按官方分文档形态在自建与第三方子接口各自声明。
/// </para>
/// </remarks>
public class WechatDepartmentsContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkDepartmentsService";

    private const string ContactRegistryGroupName = "Contact";

    /// <summary>官方路由表（同一路由在不同应用类型子接口重复声明时逐条列出，与官方分文档形态一致）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // ── 父接口：三类应用公共读取面 ──
        (typeof(IWechatWorkDepartmentsService), nameof(IWechatWorkDepartmentsService.GetDepartmentListAsync), typeof(GetAttribute), "/cgi-bin/department/list"),
        (typeof(IWechatWorkDepartmentsService), nameof(IWechatWorkDepartmentsService.GetChildDepartmentIdListAsync), typeof(GetAttribute), "/cgi-bin/department/simplelist"),
        (typeof(IWechatWorkDepartmentsService), nameof(IWechatWorkDepartmentsService.GetDepartmentAsync), typeof(GetAttribute), "/cgi-bin/department/get"),

        // ── 自建应用子接口：通讯录写入 ──
        (typeof(IWechatWorkInternalDepartmentsService), nameof(IWechatWorkInternalDepartmentsService.CreateDepartmentAsync), typeof(PostAttribute), "/cgi-bin/department/create"),
        (typeof(IWechatWorkInternalDepartmentsService), nameof(IWechatWorkInternalDepartmentsService.UpdateDepartmentAsync), typeof(PostAttribute), "/cgi-bin/department/update"),
        (typeof(IWechatWorkInternalDepartmentsService), nameof(IWechatWorkInternalDepartmentsService.DeleteDepartmentAsync), typeof(GetAttribute), "/cgi-bin/department/delete"),

        // ── 第三方应用子接口：通讯录写入（官方分文档另行声明）──
        (typeof(IWechatWorkThirdPartyDepartmentsService), nameof(IWechatWorkThirdPartyDepartmentsService.CreateDepartmentAsync), typeof(PostAttribute), "/cgi-bin/department/create"),
        (typeof(IWechatWorkThirdPartyDepartmentsService), nameof(IWechatWorkThirdPartyDepartmentsService.UpdateDepartmentAsync), typeof(PostAttribute), "/cgi-bin/department/update"),
        (typeof(IWechatWorkThirdPartyDepartmentsService), nameof(IWechatWorkThirdPartyDepartmentsService.DeleteDepartmentAsync), typeof(GetAttribute), "/cgi-bin/department/delete"),
    };

    /// <summary>
    /// 契约守卫 D1：部门管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void DepartmentEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(9, "父接口 3 条 + 自建 3 条 + 第三方 3 条");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(6, "自建与第三方对写入端点的重复声明共享官方同一路由");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：子接口对同路由端点的重复声明必须落在子接口自身，而非继承链上的父接口成员。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 D2：接口层级与生成器注册形态——公共面收敛父接口（IsAbstract），子接口挂 Contact 注册组并继承父实现类。
    /// </summary>
    [Fact]
    public void DepartmentsInterfaceHierarchy_ShouldConvergeOnAbstractParentWithContactRegistry()
    {
        var parent = typeof(IWechatWorkDepartmentsService);
        var children = new[]
        {
            typeof(IWechatWorkInternalDepartmentsService),
            typeof(IWechatWorkThirdPartyDepartmentsService),
            typeof(IWechatWorkProviderDepartmentsService),
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
                $"{child.Name} 必须挂 Contact 注册组（与成员管理域共用 Add{ContactRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            // 代开发官方端点集与公共读取面完全重合：不得在子接口新增端点（能力集合漂移守卫）。
            if (child == typeof(IWechatWorkProviderDepartmentsService))
            {
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().BeEmpty("官方未向代开发应用开放部门写入端点，代开发子接口不得新增端点");
            }
        }
    }

    /// <summary>
    /// 契约守卫 D3：令牌绑定——四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// </summary>
    [Fact]
    public void DepartmentsTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkDepartmentsService),
            typeof(IWechatWorkInternalDepartmentsService),
            typeof(IWechatWorkThirdPartyDepartmentsService),
            typeof(IWechatWorkProviderDepartmentsService),
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
    /// 契约守卫 D4：部门管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void DepartmentsDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = Mud.Wechat.Work.DataModels.WechatWorkJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(DepartmentInfo), typeof(DepartmentIdInfo),
            typeof(CreateDepartmentRequest), typeof(UpdateDepartmentRequest),
            typeof(CreateDepartmentResponse), typeof(GetDepartmentListResponse),
            typeof(GetChildDepartmentIdListResponse), typeof(GetDepartmentResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是部门管理域契约面类型，必须登记进 WechatWorkJsonContext（AOT 源生成）");
        }
    }
}
