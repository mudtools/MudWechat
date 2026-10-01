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

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 上下游域（CorpGroup 模块）契约守卫：路由表、接口层级与令牌绑定锁定。
/// </summary>
/// <remarks>
/// <para>
/// 开放面为<b>自建 + 代开发 + 第三方（仅获取应用共享信息）</b>：6 个端点全部收敛于父接口
/// <see cref="IWechatWorkCorpGroupService"/>，自建/代开发/第三方子接口均为空标记
/// （第三方仅开放获取应用共享信息，与自建/代开发同路由同契约，见 95324；
/// 守卫另以反射断言继承链上<b>不存在其它应用类型子接口</b>，新增应用类型须先核对官方文档）。
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
    /// 自建/代开发/第三方子接口均为空标记；且继承链上不得出现其它应用类型子接口（应用类型集合漂移守卫）。
    /// </summary>
    [Fact]
    public void CorpGroupInterfaceHierarchy_ShouldConvergeOnAbstractParentWithCorpGroupRegistry()
    {
        var parent = typeof(IWechatWorkCorpGroupService);
        var children = new[]
        {
            typeof(IWechatWorkInternalCorpGroupService),
            typeof(IWechatWorkThirdPartyCorpGroupService),
            typeof(IWechatWorkProviderCorpGroupService),
        };

        foreach (var child in children)
        {
            child.Should().BeAssignableTo(parent, $"{child.Name} 必须继承公共父接口 {parent.Name}");
        }

        // 应用类型集合漂移守卫：本域 6 端点中仅获取应用共享信息向第三方开放（95324，与自建/代开发同路由同契约），
        // 其余 5 端点仅自建/代开发；继承父接口的接口必须恰好为上述三个空标记子接口。
        var derived = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && parent.IsAssignableFrom(t) && t != parent)
            .Select(t => t.Name)
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToList();
        derived.Should().BeEquivalentTo(new[]
            {
                nameof(IWechatWorkInternalCorpGroupService),
                nameof(IWechatWorkThirdPartyCorpGroupService),
                nameof(IWechatWorkProviderCorpGroupService),
            },
            "新增应用类型子接口须先核对官方文档并同批调整 CG2 与 G5");

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

            // 官方对自建/代开发开放一致端点集（第三方仅获取应用共享信息，随父接口继承）：任何子接口不得新增端点（能力集合漂移守卫）。
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty(
                    $"{child.Name} 为应用类型空标记：6 个上下游端点全部声明于父接口，" +
                    "新增差异端点须先核对官方文档并同批调整 CG1/CG2");
        }
    }

    /// <summary>
    /// 契约守卫 CG3：令牌绑定——父/自建/第三方/代开发四接口统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token）。
    /// 注意 transfer_session 官方要求以下级/下游企业凭证调用（由宿主管理，见父接口注释）。
    /// </summary>
    [Fact]
    public void CorpGroupTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var interfaces = new[]
        {
            typeof(IWechatWorkCorpGroupService),
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
        var context = Mud.Wechat.Work.DataModels.WechatWorkJsonContext.Default;

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
                $"{type.Name} 是上下游通讯录管理域契约面类型，必须登记进 WechatWorkJsonContext（AOT 源生成）");
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

        var context = Mud.Wechat.Work.DataModels.WechatWorkJsonContext.Default;
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
                $"{type.Name} 是上下游规则域契约面类型，必须登记进 WechatWorkJsonContext（AOT 源生成）");
        }
    }
}
