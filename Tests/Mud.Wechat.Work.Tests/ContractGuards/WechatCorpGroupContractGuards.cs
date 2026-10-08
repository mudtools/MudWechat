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
using Mud.Wechat.Work.DataModels.CorpGroup.ChainContacts;
using Mud.Wechat.Work.DataModels.CorpGroup.Rules;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 上下游域（CorpGroup 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 开放面为<b>自建 + 代开发 + 第三方（仅获取应用共享信息）</b>：6 个端点按官方开放面拆为两段——
/// 公共父接口 <see cref="IWechatWorkCorpGroupService"/> 承载三类应用公共面「获取应用共享信息」1 条
/// （第三方文档号 95324，与自建/代开发同路由同契约），官方无第三方文档的 5 条下沉至
/// 「自建+代开发」公共父接口 <see cref="IWechatWorkCorpGroupInternalProviderService"/>；
/// 自建/代开发子接口继承后者（类型化面 6 条），第三方子接口直接继承前者（类型化面恰 1 条）。
/// 守卫以反射断言两级继承链的接口集合不漂移，并断言第三方子接口<b>不</b>继承自建+代开发公共父接口；
/// 新增应用类型须先核对官方文档。
/// </para>
/// <para>
/// 官方「企业互联」与「上下游」两棵文档树并存（同路由同契约，旧/新编号成对）：
/// 应用共享信息 93403/93405/96816 ↔ 95813/95324/96872，下级企业 access_token 93359/96814 ↔ 95816/96873，
/// 小程序 session 93355/96813 ↔ 95817/96874；企业互联树 93404「获取下级企业付费版本信息」<b>非独立 HTTP 端点</b>，
/// 仅为授权流响应 <c>edition_info.agent[].is_shared_from_other_corp</c> 的说明页（已建模于
/// <c>ProviderAuthentication.Agent.IsSharedFromOtherCorp</c>）——不得据此重复新增端点。
/// </para>
/// </remarks>
public class WechatCorpGroupContractGuards
{
    /// <summary>
    /// 父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string ParentImplementationClassName = "WechatWorkCorpGroupService";

    private const string InternalProviderImplementationClassName = "WechatWorkCorpGroupInternalProviderService";

    private const string CorpGroupRegistryGroupName = "CorpGroup";

    /// <summary>官方路由表（6 个端点：公共面 1 条在父接口，自建/代开发专属 5 条在自建+代开发公共父接口；97357/98040 覆盖 2 个端点）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        // ── 父接口：三类应用公共面（官方仅「获取应用共享信息」对第三方开放） ──
        (typeof(IWechatWorkCorpGroupService), nameof(IWechatWorkCorpGroupService.ListAppShareInfoAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/list_app_share_info"),

        // ── 自建+代开发公共父接口：官方无第三方文档，第三方子接口不继承 ──
        (typeof(IWechatWorkCorpGroupInternalProviderService), nameof(IWechatWorkCorpGroupInternalProviderService.GetCorpGroupTokenAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/gettoken"),
        (typeof(IWechatWorkCorpGroupInternalProviderService), nameof(IWechatWorkCorpGroupInternalProviderService.TransferMiniProgramSessionAsync), typeof(PostAttribute), "/cgi-bin/miniprogram/transfer_session"),
        (typeof(IWechatWorkCorpGroupInternalProviderService), nameof(IWechatWorkCorpGroupInternalProviderService.UnionidToExternalUserIdAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/unionid_to_external_userid"),
        (typeof(IWechatWorkCorpGroupInternalProviderService), nameof(IWechatWorkCorpGroupInternalProviderService.UnionidToPendingIdAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/unionid_to_pending_id"),
        (typeof(IWechatWorkCorpGroupInternalProviderService), nameof(IWechatWorkCorpGroupInternalProviderService.ExternalUserIdToPendingIdAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/batch/external_userid_to_pending_id"),
    };

    /// <summary>
    /// 契约守卫 CG1：上下游域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void CorpGroupEndpoints_ShouldMatchOfficialRoutes()
    {
        Routes.Should().HaveCount(6, "官方对自建/代开发开放 6 个上下游端点 = 父接口公共面 1 条 + 自建/代开发公共父接口 5 条");

        var distinctRoutes = Routes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(6, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            // DeclaredOnly：端点必须落在自身声明的接口上，而非从子接口继承。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 CG2：接口层级与生成器注册形态——公共父接口（IsAbstract）仅承载三类应用公共面
    /// 「获取应用共享信息」1 条；官方无第三方文档的 5 条下沉至「自建+代开发」公共父接口
    /// （IsAbstract，继承公共父接口），自建/代开发子接口继承该中间父接口且均为空标记，
    /// 第三方子接口直接继承公共父接口且为空标记（类型化端点面恰为 1 条）。
    /// 两级继承链均以反射断言集合不漂移，新增应用类型须先核对官方文档。
    /// </summary>
    [Fact]
    public void CorpGroupInterfaceHierarchy_ShouldConvergeOnAbstractParentWithCorpGroupRegistry()
    {
        var parent = typeof(IWechatWorkCorpGroupService);
        var internalProviderParent = typeof(IWechatWorkCorpGroupInternalProviderService);
        var internalChild = typeof(IWechatWorkInternalCorpGroupService);
        var thirdPartyChild = typeof(IWechatWorkThirdPartyCorpGroupService);
        var providerChild = typeof(IWechatWorkProviderCorpGroupService);

        internalProviderParent.Should().BeAssignableTo(parent,
            $"{internalProviderParent.Name} 必须继承公共父接口 {parent.Name}（继承 1 条公共面）");
        internalChild.Should().BeAssignableTo(internalProviderParent,
            $"{internalChild.Name} 必须继承自建+代开发公共父接口 {internalProviderParent.Name}");
        providerChild.Should().BeAssignableTo(internalProviderParent,
            $"{providerChild.Name} 必须继承自建+代开发公共父接口 {internalProviderParent.Name}");
        thirdPartyChild.Should().BeAssignableTo(parent,
            $"{thirdPartyChild.Name} 必须继承公共父接口 {parent.Name}");

        // 第三方子接口不得继承自建+代开发公共父接口：否则第三方类型化面会重新暴露官方未开放的 5 个端点。
        internalProviderParent.IsAssignableFrom(thirdPartyChild).Should().BeFalse(
            $"{thirdPartyChild.Name} 不得继承 {internalProviderParent.Name}：官方第三方文档树未开放该 5 个端点");

        // 应用类型集合漂移守卫：两级继承链各自的有序接口集合被锁定。
        parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && parent.IsAssignableFrom(t) && t != parent)
            .Select(t => t.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList()
            .Should().BeEquivalentTo(
                new[]
                {
                    nameof(IWechatWorkCorpGroupInternalProviderService),
                    nameof(IWechatWorkInternalCorpGroupService),
                    nameof(IWechatWorkProviderCorpGroupService),
                    nameof(IWechatWorkThirdPartyCorpGroupService),
                },
                "公共父接口的直接子接口恰为自建+代开发公共父接口 + 三个应用类型子接口；" +
                "新增应用类型须先核对官方文档并同批调整 CG2 与 G5");

        internalProviderParent.Assembly.GetTypes()
            .Where(t => t.IsInterface && internalProviderParent.IsAssignableFrom(t) && t != internalProviderParent)
            .Select(t => t.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList()
            .Should().BeEquivalentTo(
                new[]
                {
                    nameof(IWechatWorkInternalCorpGroupService),
                    nameof(IWechatWorkProviderCorpGroupService),
                },
                "自建+代开发公共父接口的直接子接口恰为自建与代开发两个应用类型子接口");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(1, "官方仅「获取应用共享信息」向三类应用开放，父接口只承载该 1 条公共面");

        var internalProviderApi = internalProviderParent.GetCustomAttribute<HttpClientApiAttribute>();
        internalProviderApi.Should().NotBeNull($"{internalProviderParent.Name} 必须声明 [HttpClientApi]");
        internalProviderApi!.IsAbstract.Should().BeTrue(
            $"{internalProviderParent.Name} 为自建/代开发公共父接口，不参与 DI 注册，必须 IsAbstract = true");
        internalProviderApi.RegistryGroupName.Should().BeNullOrEmpty(
            $"{internalProviderParent.Name} 不进入注册组（注册面由应用类型子接口承载）");
        internalProviderApi.InheritedFrom.Should().Be(ParentImplementationClassName,
            $"{internalProviderParent.Name} 必须继承公共父接口生成实现类，避免生成器重复实现公共面端点");
        internalProviderParent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(5, "官方无第三方文档的 5 条端点必须声明在自建+代开发公共父接口");

        foreach (var child in new[] { internalChild, providerChild, thirdPartyChild })
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(CorpGroupRegistryGroupName,
                $"{child.Name} 必须挂 {CorpGroupRegistryGroupName} 注册组（独立模块，经 Add{CorpGroupRegistryGroupName}Api() 注册）");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty(
                    $"{child.Name} 为应用类型空标记：6 个上下游端点全部由父接口/自建代开发公共父接口声明，" +
                    "新增差异端点须先核对官方文档并同批调整 CG1/CG2");
        }

        internalChild.GetCustomAttribute<HttpClientApiAttribute>()!.InheritedFrom
            .Should().Be(InternalProviderImplementationClassName,
                $"{internalChild.Name} 必须继承自建+代开发公共父接口生成实现类");
        providerChild.GetCustomAttribute<HttpClientApiAttribute>()!.InheritedFrom
            .Should().Be(InternalProviderImplementationClassName,
                $"{providerChild.Name} 必须继承自建+代开发公共父接口生成实现类");
        thirdPartyChild.GetCustomAttribute<HttpClientApiAttribute>()!.InheritedFrom
            .Should().Be(ParentImplementationClassName,
                $"{thirdPartyChild.Name} 必须继承公共父接口生成实现类");

        // 端点面收窄断言：第三方类型化面恰为 1 条（父接口公共面），自建/代开发为官方开放的 6 条。
        CollectInterfaceEndpoints(thirdPartyChild).Should().HaveCount(1,
            "第三方类型化面只暴露「获取应用共享信息」");
        CollectInterfaceEndpoints(thirdPartyChild).Single().Name
            .Should().Be(nameof(IWechatWorkCorpGroupService.ListAppShareInfoAsync),
                "第三方类型化面唯一端点为官方向第三方开放的「获取应用共享信息」");
        CollectInterfaceEndpoints(internalChild).Should().HaveCount(6, "自建应用类型化面为官方开放的 6 条上下游端点");
        CollectInterfaceEndpoints(providerChild).Should().HaveCount(6, "代开发应用类型化面为官方开放的 6 条上下游端点");
    }

    /// <summary>
    /// 沿接口继承链收集端点方法声明。
    /// <para>接口反射<b>不</b>返回继承成员（<c>Type.GetMethods()</c> 对接口仅返回自身声明），
    /// 故类型化端点面必须逐级上溯接口继承链后统计。</para>
    /// </summary>
    private static List<MethodInfo> CollectInterfaceEndpoints(Type iface)
    {
        var methods = new List<MethodInfo>();
        for (var current = iface; current is not null && current != typeof(object); current = current.GetInterfaces().FirstOrDefault())
        {
            methods.AddRange(current.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
        }

        return methods;
    }

    /// <summary>
    /// 契约守卫 CG3：令牌绑定——公共父接口 / 自建+代开发公共父接口 / 自建 / 第三方 / 代开发五接口
    /// 统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// 注意 transfer_session 官方要求以下级/下游企业凭证调用（由宿主管理，见自建+代开发公共父接口注释）。
    /// </summary>
    [Fact]
    public void CorpGroupTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkCorpGroupService),
            typeof(IWechatWorkCorpGroupInternalProviderService),
            typeof(IWechatWorkInternalCorpGroupService),
            typeof(IWechatWorkThirdPartyCorpGroupService),
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
    /// <remarks>
    /// ExternalUserIdToPendingIdRequest/Response 与客户联系「客户管理」域同名，
    /// 二者按命名空间分属上下游域 <c>CorpGroupJsonContext</c> 与客户管理域
    /// <c>CustomerJsonContext</c> 两个上下文（同名 DTO 不能同上下文）。
    /// </remarks>
    [Fact]
    public void CorpGroupDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = CorpGroupJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(ListAppShareInfoRequest), typeof(GetCorpGroupTokenRequest),
            typeof(TransferMiniProgramSessionRequest), typeof(UnionidToExternalUserIdRequest),
            typeof(UnionidToPendingIdRequest),
            typeof(AppShareCorpInfo), typeof(ExternalUserIdInfo), typeof(PendingIdResultItem),
            typeof(ListAppShareInfoResponse), typeof(GetCorpGroupTokenResponse),
            typeof(TransferMiniProgramSessionResponse), typeof(UnionidToExternalUserIdResponse),
            typeof(UnionidToPendingIdResponse), typeof(ExternalUserIdToPendingIdRequest),
            typeof(ExternalUserIdToPendingIdResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是上下游域契约面类型，必须登记进 CorpGroupJsonContext（AOT 源生成）");
        }
    }

    /// <summary>上下游通讯录管理域官方路由表（公共读取面 4 条在父接口，写入/任务/查询 5 条在自建子接口）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ContactsRoutes =
    {
        // ── 父接口：自建/代开发公共读取面（95820/96876 同页 4 端点） ──
        (typeof(IWechatWorkCorpGroupContactsService), nameof(IWechatWorkCorpGroupContactsService.GetChainListAsync), typeof(GetAttribute), "/cgi-bin/corpgroup/corp/get_chain_list"),
        (typeof(IWechatWorkCorpGroupContactsService), nameof(IWechatWorkCorpGroupContactsService.GetChainGroupAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/get_chain_group"),
        (typeof(IWechatWorkCorpGroupContactsService), nameof(IWechatWorkCorpGroupContactsService.GetChainCorpInfoListAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/get_chain_corpinfo_list"),
        (typeof(IWechatWorkCorpGroupContactsService), nameof(IWechatWorkCorpGroupContactsService.GetChainCorpInfoAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/get_chain_corpinfo"),

        // ── 自建子接口：导入 / 任务 / 移除 / 查询（97441/97442 官方权限说明提及代开发，但代开发文档树无对应页，按需求清单落自建） ──
        (typeof(IWechatWorkInternalCorpGroupContactsService), nameof(IWechatWorkInternalCorpGroupContactsService.ImportChainContactsAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/import_chain_contact"),
        (typeof(IWechatWorkInternalCorpGroupContactsService), nameof(IWechatWorkInternalCorpGroupContactsService.GetChainImportResultAsync), typeof(GetAttribute), "/cgi-bin/corpgroup/getresult"),
        (typeof(IWechatWorkInternalCorpGroupContactsService), nameof(IWechatWorkInternalCorpGroupContactsService.RemoveChainCorpAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/remove_corp"),
        (typeof(IWechatWorkInternalCorpGroupContactsService), nameof(IWechatWorkInternalCorpGroupContactsService.GetChainUserCustomIdAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/corp/get_chain_user_custom_id"),
        (typeof(IWechatWorkInternalCorpGroupContactsService), nameof(IWechatWorkInternalCorpGroupContactsService.GetCorpSharedChainListAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/get_corp_shared_chain_list"),
    };

    /// <summary>
    /// 契约守卫 CG5：上下游通讯录管理域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// 注意本域导入任务结果为 <c>corpgroup/getresult</c>（无下划线），与导出域 <c>export/get_result</c>、
    /// 异步导入域 <c>batch/getresult</c> 拼写各异。
    /// </summary>
    [Fact]
    public void CorpGroupContactsEndpoints_ShouldMatchOfficialRoutes()
    {
        ContactsRoutes.Should().HaveCount(9, "父接口 4 条公共读取面 + 自建子接口 5 条导入/移除/查询端点");

        var distinctRoutes = ContactsRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(9, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in ContactsRoutes)
        {
            // DeclaredOnly：子接口对端点的声明必须落在子接口自身，父接口端点必须落在父接口自身。
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 CG6：上下游通讯录管理域接口层级——父接口（IsAbstract）承载 4 条公共读取面，
    /// 自建子接口承载 5 条写入/任务/查询端点，代开发子接口为零端点空标记（能力漂移守卫）。
    /// </summary>
    [Fact]
    public void CorpGroupContactsInterfaceHierarchy_ShouldConvergeOnAbstractParentWithInternalWriteEndpoints()
    {
        var parent = typeof(IWechatWorkCorpGroupContactsService);
        var internalChild = typeof(IWechatWorkInternalCorpGroupContactsService);
        var providerChild = typeof(IWechatWorkProviderCorpGroupContactsService);

        internalChild.Should().BeAssignableTo(parent, $"{internalChild.Name} 必须继承公共父接口 {parent.Name}");
        providerChild.Should().BeAssignableTo(parent, $"{providerChild.Name} 必须继承公共父接口 {parent.Name}");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(4, "官方向自建/代开发开放一致的 4 条读取端点全部声明于父接口");

        foreach (var child in new[] { internalChild, providerChild })
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(CorpGroupRegistryGroupName,
                $"{child.Name} 必须挂 {CorpGroupRegistryGroupName} 注册组（与上下游基础接口共用 Add{CorpGroupRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be("WechatWorkCorpGroupContactsService",
                $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");
        }

        internalChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(5, "导入/任务/移除/查询 5 条端点必须声明在自建子接口");
        providerChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty(
                $"{providerChild.Name} 为代开发空标记：官方代开发文档树未开放导入/移除/查询端点，" +
                "新增端点须先核对官方文档并同批调整 CG5/CG6");
    }

    /// <summary>
    /// 契约守卫 CG7：上下游通讯录管理域令牌绑定——三接口统一消费 AccessToken 路由键并以 Query 注入。
    /// </summary>
    [Fact]
    public void CorpGroupContactsTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkCorpGroupContactsService),
            typeof(IWechatWorkInternalCorpGroupContactsService),
            typeof(IWechatWorkProviderCorpGroupContactsService),
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
    /// 契约守卫 CG8：上下游通讯录管理域的请求/响应 DTO 必须登记进 AOT JSON 上下文。
    /// </summary>
    [Fact]
    public void CorpGroupContactsDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var context = ChainContactsJsonContext.Default;

        var requiredTypes = new[]
        {
            typeof(ChainInfo), typeof(ChainGroupInfo), typeof(ChainGroupCorpInfo),
            typeof(ChainImportFailedContact), typeof(ChainImportFailedCorp),
            typeof(ChainImportCorpItem), typeof(ChainImportContactItem),
            typeof(ImportChainContactsRequest), typeof(GetChainGroupRequest),
            typeof(GetChainCorpInfoListRequest), typeof(GetChainCorpInfoRequest),
            typeof(RemoveChainCorpRequest), typeof(GetChainUserCustomIdRequest),
            typeof(GetCorpSharedChainListRequest),
            typeof(GetChainListResponse), typeof(GetChainGroupResponse),
            typeof(GetChainCorpInfoListResponse), typeof(GetChainCorpInfoResponse),
            typeof(ImportChainContactsResponse), typeof(ChainImportResult),
            typeof(GetChainImportResultResponse), typeof(GetChainUserCustomIdResponse),
            typeof(GetCorpSharedChainListResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是上下游通讯录管理域契约面类型，必须登记进 ChainContactsJsonContext（AOT 源生成）");
        }
    }

    /// <summary>上下游规则域官方路由表（5 个端点全部声明于自建子接口；官方无第三方/代开发文档）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] RuleRoutes =
    {
        (typeof(IWechatWorkInternalCorpGroupRulesService), nameof(IWechatWorkInternalCorpGroupRulesService.ListChainRuleIdsAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/rule/list_ids"),
        (typeof(IWechatWorkInternalCorpGroupRulesService), nameof(IWechatWorkInternalCorpGroupRulesService.DeleteChainRuleAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/rule/delete_rule"),
        (typeof(IWechatWorkInternalCorpGroupRulesService), nameof(IWechatWorkInternalCorpGroupRulesService.GetChainRuleInfoAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/rule/get_rule_info"),
        (typeof(IWechatWorkInternalCorpGroupRulesService), nameof(IWechatWorkInternalCorpGroupRulesService.AddChainRuleAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/rule/add_rule"),
        (typeof(IWechatWorkInternalCorpGroupRulesService), nameof(IWechatWorkInternalCorpGroupRulesService.ModifyChainRuleAsync), typeof(PostAttribute), "/cgi-bin/corpgroup/rule/modify_rule"),
    };

    /// <summary>
    /// 契约守卫 CG9：上下游规则域全部端点路由必须与官方契约一致（新增/改名端点须同批更新本表）。
    /// </summary>
    [Fact]
    public void CorpGroupRuleEndpoints_ShouldMatchOfficialRoutes()
    {
        RuleRoutes.Should().HaveCount(5, "官方仅向自建应用开放 5 个对接规则端点（增删改查）");

        var distinctRoutes = RuleRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(5, "本域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in RuleRoutes)
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
    /// 契约守卫 CG10：上下游规则域接口层级——父接口零端点（IsAbstract），全部端点收敛自建子接口，
    /// 不设第三方/代开发子接口（官方无文档；落位形态同通讯录查看权限管理域）。
    /// </summary>
    [Fact]
    public void CorpGroupRuleInterfaceHierarchy_ShouldConvergeOnInternalChild()
    {
        var parent = typeof(IWechatWorkCorpGroupRulesService);
        var internalChild = typeof(IWechatWorkInternalCorpGroupRulesService);

        internalChild.Should().BeAssignableTo(parent, $"{internalChild.Name} 必须继承公共父接口 {parent.Name}");

        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().BeEmpty("官方仅向自建应用开放本域端点，父接口不存在公共面，不得声明端点");

        var internalApi = internalChild.GetCustomAttribute<HttpClientApiAttribute>();
        internalApi.Should().NotBeNull($"{internalChild.Name} 必须声明 [HttpClientApi]");
        internalApi!.RegistryGroupName.Should().Be(CorpGroupRegistryGroupName,
            $"{internalChild.Name} 必须挂 {CorpGroupRegistryGroupName} 注册组（与上下游既有接口族共用 Add{CorpGroupRegistryGroupName}WebApiHttpClient()）");
        internalApi.InheritedFrom.Should().Be("WechatWorkCorpGroupRulesService",
            $"{internalChild.Name} 必须继承父接口生成实现类");
        internalChild.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(5, "全部 5 个端点必须声明在自建子接口");
    }

    /// <summary>
    /// 契约守卫 CG11：上下游规则域令牌绑定 + JSON 上下文登记。
    /// </summary>
    [Fact]
    public void CorpGroupRuleTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkCorpGroupRulesService),
            typeof(IWechatWorkInternalCorpGroupRulesService),
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

        var context = RulesJsonContext.Default;
        var requiredTypes = new[]
        {
            typeof(ChainRuleOwnerRange), typeof(ChainRuleMemberRange), typeof(ChainRuleInfo),
            typeof(ListChainRuleIdsRequest), typeof(DeleteChainRuleRequest),
            typeof(GetChainRuleInfoRequest), typeof(AddChainRuleRequest), typeof(ModifyChainRuleRequest),
            typeof(ListChainRuleIdsResponse), typeof(GetChainRuleInfoResponse), typeof(AddChainRuleResponse),
        };

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是上下游规则域契约面类型，必须登记进 RulesJsonContext（AOT 源生成）");
        }
    }
}
