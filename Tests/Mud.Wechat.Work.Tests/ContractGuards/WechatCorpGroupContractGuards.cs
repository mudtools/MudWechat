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
using Mud.Wechat.Work.DataModels.CorpGroup;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 上下游域（CorpGroup 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 形态与通讯录查看权限域同构（自建单子接口承载型），但开放面为<b>自建 + 代开发</b>两类应用
/// （第三方应用无对应文档，不设子接口）：6 个端点全部收敛于父接口
/// <see cref="IWechatWorkCorpGroupService"/>，自建与代开发子接口均为空标记；
/// 守卫另以反射断言继承链上<b>不存在其它应用类型子接口</b>（新增应用类型须先核对官方文档）。
/// </para>
/// </remarks>
public class WechatCorpGroupContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkCorpGroupService";

    private const string CorpGroupRegistryGroupName = "CorpGroup";

    /// <summary>官方路由表（6 个端点全部声明于父接口；自建与代开发官方文档路由完全一致，97357/98040 覆盖 2 个端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWechatWorkCorpGroupService), nameof(IWechatWorkCorpGroupService.ListAppShareInfoAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/list_app_share_info"),
        (typeof(IWechatWorkCorpGroupService), nameof(IWechatWorkCorpGroupService.GetCorpGroupTokenAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/gettoken"),
        (typeof(IWechatWorkCorpGroupService), nameof(IWechatWorkCorpGroupService.TransferMiniProgramSessionAsync), typeof(PostAttribute), "/cgi-bin/miniprogram/transfer_session"),
        (typeof(IWechatWorkCorpGroupService), nameof(IWechatWorkCorpGroupService.UnionidToExternalUserIdAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/unionid_to_external_userid"),
        (typeof(IWechatWorkCorpGroupService), nameof(IWechatWorkCorpGroupService.UnionidToPendingIdAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/unionid_to_pending_id"),
        (typeof(IWechatWorkCorpGroupService), nameof(IWechatWorkCorpGroupService.ExternalUserIdToPendingIdAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/batch/external_userid_to_pending_id"),
    };

    /// <summary>
    /// 契约守卫 CG1：上下游域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void CorpGroupEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(6, "官方对自建/代开发应用开放一致的 6 个上下游端点，全部声明于父接口");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(6, "本域各端点路由互不重复");

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
    /// 契约守卫 CG2：接口层级与生成器注册形态——全部端点收敛父接口（IsAbstract），
    /// 自建/代开发子接口为空标记；且继承链上不得出现其它应用类型子接口（应用类型集合漂移守卫）。
    /// </summary>
    [Fact]
    public void CorpGroupInterfaceHierarchy_ShouldConvergeOnAbstractParentWithCorpGroupRegistry()
    {
        var parent = typeof(IWechatWorkCorpGroupService);
        var children = new[]
        {
            typeof(IWechatWorkInternalCorpGroupService),
            typeof(IWechatWorkProviderCorpGroupService),
        };

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");
        }

        // 应用类型集合漂移守卫：官方仅向自建/代开发开放本域（第三方无文档），
        // 继承父接口的接口必须恰好为父接口的实现类 + 上述两个空标记子接口。
        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && parent.IsAssignableFrom(t) && t != parent)
            .Select(t => t.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
        derived.Should().BeEquivalentTo(new[] { nameof(IWechatWorkInternalCorpGroupService), nameof(IWechatWorkProviderCorpGroupService) },
            "官方未向其它应用类型开放上下游端点，新增应用类型子接口须先核对官方文档并同批调整 CG2 与 G5");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");

        foreach (var child in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(CorpGroupRegistryGroupName,
                $"{child.Name} 必须挂 {CorpGroupRegistryGroupName} 注册组（独立模块，经 Add{CorpGroupRegistryGroupName}Api() 注册）");
            childApi.InheritedFrom.Should().Be(ParentImplementationClassName,
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

            // 官方对自建/代开发开放一致端点集：任何子接口不得新增端点（能力集合漂移守卫）。
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty(
                    $"{child.Name} 为应用类型空标记：6 个上下游端点全部声明于父接口，" +
                    "新增差异端点须先核对官方文档并同批调整 CG1/CG2");
        }
    }

    /// <summary>
    /// 契约守卫 CG3：令牌绑定——父/自建/代开发三接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// 注意 transfer_session 官方要求以下级/下游企业凭证调用（由宿主管理，见父接口注释）。
    /// </summary>
    [Fact]
    public void CorpGroupTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkCorpGroupService),
            typeof(IWechatWorkInternalCorpGroupService),
            typeof(IWechatWorkProviderCorpGroupService),
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
    /// 契约守卫 CG4：上下游域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void CorpGroupDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = Mud.Wechat.Work.DataModels.WechatWorkJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(ListAppShareInfoRequest), typeof(GetCorpGroupTokenRequest),
            typeof(TransferMiniProgramSessionRequest), typeof(UnionidToExternalUserIdRequest),
            typeof(UnionidToPendingIdRequest), typeof(ExternalUserIdToPendingIdRequest),
            typeof(AppShareCorpInfo), typeof(ExternalUserIdInfo), typeof(PendingIdResultItem),
            typeof(ListAppShareInfoResponse), typeof(GetCorpGroupTokenResponse),
            typeof(TransferMiniProgramSessionResponse), typeof(UnionidToExternalUserIdResponse),
            typeof(UnionidToPendingIdResponse), typeof(ExternalUserIdToPendingIdResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是上下游域契约面类型，必须登记进 WechatWorkJsonContext（AOT 源生成）");
        }
    }
}
