// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Reflection;
using System.Text.Json.Serialization;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.DataModels;
using Mud.Wechat.Work.DataModels.Agent;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 应用管理模块（Agent 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 四功能族形态：
/// 获取应用族（agent/get + agent/list 两端点官方对三类应用开放一致，收敛声明于公共父接口；
/// 设置应用官方仅企业可调用——第三方以及代开发自建应用不可调用，落自建差异端点）；
/// 工作台自定义展示族（5 端点官方对三类应用开放一致，收敛声明于公共父接口 + 空标记子接口）；
/// 自定义菜单族（创建/获取/删除菜单 3 端点官方权限均为「仅企业可调用；第三方不可调用」，
/// 服务商代开发章节无对应 API，零端点父接口 + 唯一自建子接口承载端点）；
/// 自建应用迁移成代开发应用族（官方仅服务商代开发章节提供该端点——自建应用与第三方应用文档树均无对应页面，
/// 但端点消费的是待迁移自建应用自身的 access_token，故零端点父接口 + 唯一自建子接口承载端点）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：自定义菜单三端点的 agentid 为 Query 参数而非请求体字段，
/// 菜单结构经请求体/响应体承载；获取菜单与删除菜单官方即 GET（勿「顺手统一」为 POST），
/// 删除菜单为覆盖式删除（整树删除、无法按单个菜单项删除）；
/// 获取应用与获取应用列表官方即 GET（agentid 走 Query），设置应用官方即 POST；
/// 工作台自定义展示 5 端点官方全部即 POST（含仅查询语义的 get_workbench_template / get_workbench_data）；
/// 工作台模版数据四种类型（keydata/image/list/webview）结构各异：
/// 关键数据型 items ≤4（key/data ≤64 字符）、列表型 items ≤3（title ≤128 字节）、
/// 图片型与网页型为单对象，模版数据在单用户设置接口平铺于请求体顶层、
/// 而批量设置与获取用户数据接口以 data 对象包裹——同域两形态并存；
/// 获取应用详情响应官方字段作 <c>isreportenter</c>（全小写无驼峰）、应用 id 作 <c>agentid</c>（无下划线）、
/// 应用列表作 <c>agentlist</c>；<c>customized_publish_status</c> 仅代开发自建应用返回；
/// 自建应用迁移成代开发应用为双令牌契约：access_token 为 URL 参数（Query 注入）、
/// suite_access_token 为包体参数（请求体显式传入，不经令牌作用域机制表达）。
/// </para>
/// </remarks>
public class WechatAgentContractGuards
{
    private const string AgentRegistryGroupName = "Agent";

    // ------------------------------------------------------------------
    // 路由表：12 条官方路由（一端点一路由，无同路由多分支）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        AgentBaseRoutes =
        {
            // 获取指定的应用详情（自建 90227 / 第三方 90363 / 代开发 96448；三类公共收敛父接口；
            // 官方即 GET，agentid 走 Query）。
            (typeof(IWechatWorkAgentService),
                nameof(IWechatWorkAgentService.GetAgentAsync),
                typeof(GetAttribute), "/cgi-bin/agent/get"),
            // 获取 access_token 对应的应用列表（自建 90227 / 第三方 90363 / 代开发 96448；
            // 三类公共收敛父接口；官方即 GET，无业务参数）。
            (typeof(IWechatWorkAgentService),
                nameof(IWechatWorkAgentService.GetAgentListAsync),
                typeof(GetAttribute), "/cgi-bin/agent/list"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        AgentSetRoutes =
        {
            // 设置应用（自建 90228；官方仅企业可调用——第三方以及代开发自建应用不可调用；官方即 POST）。
            (typeof(IWechatWorkInternalAgentService),
                nameof(IWechatWorkInternalAgentService.SetAgentAsync),
                typeof(PostAttribute), "/cgi-bin/agent/set"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        AgentWorkbenchRoutes =
        {
            // 设置应用在工作台展示的模版（自建 92535 / 第三方 94620 / 代开发 96454；三类公共收敛父接口）。
            (typeof(IWechatWorkAgentWorkbenchService),
                nameof(IWechatWorkAgentWorkbenchService.SetAgentWorkbenchTemplateAsync),
                typeof(PostAttribute), "/cgi-bin/agent/set_workbench_template"),
            // 获取应用在工作台展示的模版（自建 92535 / 第三方 94620 / 代开发 96454；官方即 POST）。
            (typeof(IWechatWorkAgentWorkbenchService),
                nameof(IWechatWorkAgentWorkbenchService.GetAgentWorkbenchTemplateAsync),
                typeof(PostAttribute), "/cgi-bin/agent/get_workbench_template"),
            // 设置应用在用户工作台展示的数据（自建 92535 / 第三方 94620 / 代开发 96454；
            // 每用户每应用限 10 次/分钟）。
            (typeof(IWechatWorkAgentWorkbenchService),
                nameof(IWechatWorkAgentWorkbenchService.SetAgentWorkbenchDataAsync),
                typeof(PostAttribute), "/cgi-bin/agent/set_workbench_data"),
            // 批量设置应用在用户工作台展示的数据（自建 92535 / 第三方 94620 / 代开发 96454；
            // userid_list 最多 1000 个；每应用限 100000 人次/分钟）。
            (typeof(IWechatWorkAgentWorkbenchService),
                nameof(IWechatWorkAgentWorkbenchService.BatchSetAgentWorkbenchDataAsync),
                typeof(PostAttribute), "/cgi-bin/agent/batch_set_workbench_data"),
            // 获取应用在用户工作台展示的数据（自建 92535 / 第三方 94620 / 代开发 96454；官方即 POST）。
            (typeof(IWechatWorkAgentWorkbenchService),
                nameof(IWechatWorkAgentWorkbenchService.GetAgentWorkbenchDataAsync),
                typeof(PostAttribute), "/cgi-bin/agent/get_workbench_data"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        AgentMenuRoutes =
        {
            // 创建菜单（自建 90231；官方仅企业可调用、第三方不可调用；官方即 POST，agentid 走 Query）。
            (typeof(IWechatWorkInternalAgentMenuService),
                nameof(IWechatWorkInternalAgentMenuService.CreateMenuAsync),
                typeof(PostAttribute), "/cgi-bin/menu/create"),
            // 获取菜单（自建 90232；官方即 GET，agentid 走 Query）。
            (typeof(IWechatWorkInternalAgentMenuService),
                nameof(IWechatWorkInternalAgentMenuService.GetMenuAsync),
                typeof(GetAttribute), "/cgi-bin/menu/get"),
            // 删除菜单（自建 90233；官方即 GET，覆盖式删除整棵菜单树，勿「顺手统一」为 POST）。
            (typeof(IWechatWorkInternalAgentMenuService),
                nameof(IWechatWorkInternalAgentMenuService.DeleteMenuAsync),
                typeof(GetAttribute), "/cgi-bin/menu/delete"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        AgentMigrationRoutes =
        {
            // 自建应用迁移成代开发应用（代开发 99617；官方仅服务商代开发章节提供，
            // 消费待迁移自建应用自身 access_token；suite_access_token 为包体参数）。
            (typeof(IWechatWorkInternalAgentMigrationService),
                nameof(IWechatWorkInternalAgentMigrationService.ClaimCustomizedAppAsync),
                typeof(PostAttribute), "/cgi-bin/agent/claim_customized_app"),
        };

    // ------------------------------------------------------------------
    // AG1：全部端点路由与官方契约一致（12 条路由表项 = 12 条官方路由）。
    // ------------------------------------------------------------------

    [Fact]
    public void AgentEndpoints_ShouldMatchOfficialRoutes()
    {
        // 获取应用族公共面：2 端点收敛父接口，官方即 GET。
        AgentBaseRoutes.Should().HaveCount(2, "获取应用族公共面 = 获取指定的应用详情 + 获取 access_token 对应的应用列表");
        AgentBaseRoutes.Select(r => r.HttpAttribute).Should().OnlyContain(
            a => a == typeof(GetAttribute), "获取应用族两端点官方即 GET（agentid 走 Query 或无业务参数）");

        // 设置应用：官方仅企业可调用（第三方以及代开发自建应用不可调用），落自建差异端点。
        AgentSetRoutes.Should().HaveCount(1, "设置应用为自建应用差异端点");
        AgentSetRoutes.Single().Interface.Should().Be(typeof(IWechatWorkInternalAgentService),
            "设置应用官方仅企业可调用（第三方以及代开发自建应用不可调用）");

        // 工作台自定义展示族：5 端点全部官方即 POST（含仅查询语义的获取端点）。
        AgentWorkbenchRoutes.Should().HaveCount(5, "工作台自定义展示族 = 设置/获取模版 2 + 设置/批量设置/获取用户数据 3 端点");
        AgentWorkbenchRoutes.Select(r => r.HttpAttribute).Should().OnlyContain(
            a => a == typeof(PostAttribute), "工作台自定义展示族 5 端点官方全部即 POST（勿「顺手统一」获取端点为 GET）");
        AgentWorkbenchRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/agent/", StringComparison.Ordinal),
            "工作台自定义展示族路由挂 /cgi-bin/agent/ 段");

        // 自定义菜单族：3 端点全部挂于唯一自建子接口。
        AgentMenuRoutes.Should().HaveCount(3, "自定义菜单族 = 创建 + 获取 + 删除菜单端点");
        AgentMenuRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalAgentMenuService),
            "自定义菜单族官方仅企业可调用（第三方不可调用，代开发章节无对应 API）");

        // 自建应用迁移成代开发应用族：1 端点挂于唯一自建子接口。
        AgentMigrationRoutes.Should().HaveCount(1, "自建应用迁移成代开发应用族 = 单端点");
        AgentMigrationRoutes.Single().Interface.Should().Be(typeof(IWechatWorkInternalAgentMigrationService),
            "端点消费待迁移自建应用自身 access_token，应用上下文须为自建应用（AppType = Internal）");

        AssertRoutes(AgentBaseRoutes);
        AssertRoutes(AgentSetRoutes);
        AssertRoutes(AgentWorkbenchRoutes);
        AssertRoutes(AgentMenuRoutes);
        AssertRoutes(AgentMigrationRoutes);

        // 全部官方路由清单锁定（一端点一路由，共 12 条）。
        var allRoutes = AgentBaseRoutes.Concat(AgentSetRoutes).Concat(AgentWorkbenchRoutes)
            .Concat(AgentMenuRoutes).Concat(AgentMigrationRoutes)
            .Select(r => r.Route).Distinct().ToList();
        allRoutes.Should().BeEquivalentTo(new[]
        {
            // 获取应用族（3 条，含设置应用）。
            "/cgi-bin/agent/get",
            "/cgi-bin/agent/list",
            "/cgi-bin/agent/set",
            // 工作台自定义展示族（5 条）。
            "/cgi-bin/agent/set_workbench_template",
            "/cgi-bin/agent/get_workbench_template",
            "/cgi-bin/agent/set_workbench_data",
            "/cgi-bin/agent/batch_set_workbench_data",
            "/cgi-bin/agent/get_workbench_data",
            // 自定义菜单族（3 条）。
            "/cgi-bin/menu/create",
            "/cgi-bin/menu/get",
            "/cgi-bin/menu/delete",
            // 自建应用迁移成代开发应用族（1 条）。
            "/cgi-bin/agent/claim_customized_app",
        }, "应用管理域全部官方路由须与官方文档一一对应");
        allRoutes.Should().HaveCount(12, "应用管理域共 12 条官方路由（一端点一路由）");

        // 官方无请求体/无业务参数端点的参数形态锁定。
        typeof(IWechatWorkAgentService)
            .GetMethod(nameof(IWechatWorkAgentService.GetAgentListAsync), BindingFlags.Public | BindingFlags.Instance)!
            .GetParameters().Should().ContainSingle("获取应用列表官方无业务参数，方法仅承载 CancellationToken")
            .Which.ParameterType.Should().Be(typeof(CancellationToken),
                "获取应用列表无业务参数，唯一参数必须是 CancellationToken");

        // 自定义菜单三端点与获取应用详情的 agentid 均为 Query 参数（官方契约），不得移入请求体。
        AssertAgentidQueryParameter(typeof(IWechatWorkAgentService), nameof(IWechatWorkAgentService.GetAgentAsync));
        AssertAgentidQueryParameter(typeof(IWechatWorkInternalAgentMenuService), nameof(IWechatWorkInternalAgentMenuService.CreateMenuAsync));
        AssertAgentidQueryParameter(typeof(IWechatWorkInternalAgentMenuService), nameof(IWechatWorkInternalAgentMenuService.GetMenuAsync));
        AssertAgentidQueryParameter(typeof(IWechatWorkInternalAgentMenuService), nameof(IWechatWorkInternalAgentMenuService.DeleteMenuAsync));

        // 无业务负载端点：响应直接用 WechatWorkResponse，不得新建空响应 DTO。
        foreach (var (iface, method) in new[]
        {
            (typeof(IWechatWorkInternalAgentService), nameof(IWechatWorkInternalAgentService.SetAgentAsync)),
            (typeof(IWechatWorkAgentWorkbenchService), nameof(IWechatWorkAgentWorkbenchService.SetAgentWorkbenchTemplateAsync)),
            (typeof(IWechatWorkAgentWorkbenchService), nameof(IWechatWorkAgentWorkbenchService.SetAgentWorkbenchDataAsync)),
            (typeof(IWechatWorkAgentWorkbenchService), nameof(IWechatWorkAgentWorkbenchService.BatchSetAgentWorkbenchDataAsync)),
            (typeof(IWechatWorkInternalAgentMenuService), nameof(IWechatWorkInternalAgentMenuService.CreateMenuAsync)),
            (typeof(IWechatWorkInternalAgentMenuService), nameof(IWechatWorkInternalAgentMenuService.DeleteMenuAsync)),
            (typeof(IWechatWorkInternalAgentMigrationService), nameof(IWechatWorkInternalAgentMigrationService.ClaimCustomizedAppAsync)),
        })
        {
            iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance)!
                .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                    $"{iface.Name}.{method} 仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        }
    }

    // ------------------------------------------------------------------
    // AG2：接口层级与生成器注册形态（四族继承链、父/子端点数与开放面收敛）。
    // ------------------------------------------------------------------

    [Fact]
    public void AgentInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 获取应用族：公共父 2 端点 + 自建 1 差异端点（设置应用）+ 第三方/代开发零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkAgentService),
            parentImplementation: "WechatWorkAgentService",
            parentDeclaredEndpointCount: 2,
            new[]
            {
                (typeof(IWechatWorkInternalAgentService), 1),
                (typeof(IWechatWorkProviderAgentService), 0),
                (typeof(IWechatWorkThirdPartyAgentService), 0),
            });

        // 工作台自定义展示族：公共父 5 端点 + 三个空标记子接口。
        AssertFamily(
            parent: typeof(IWechatWorkAgentWorkbenchService),
            parentImplementation: "WechatWorkAgentWorkbenchService",
            parentDeclaredEndpointCount: 5,
            new[]
            {
                (typeof(IWechatWorkInternalAgentWorkbenchService), 0),
                (typeof(IWechatWorkProviderAgentWorkbenchService), 0),
                (typeof(IWechatWorkThirdPartyAgentWorkbenchService), 0),
            });

        // 自定义菜单族：官方仅自建应用开放（官方权限「仅企业可调用；第三方不可调用」，
        // 代开发章节无对应 API），父接口零端点 + 唯一自建子接口承载端点
        //（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkAgentMenuService),
            parentImplementation: "WechatWorkAgentMenuService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalAgentMenuService), 3),
            });

        // 自建应用迁移成代开发应用族：官方仅代开发章节提供但消费待迁移自建应用自身 access_token
        //（调用上下文须为自建应用），父接口零端点 + 唯一自建子接口承载端点
        //（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkAgentMigrationService),
            parentImplementation: "WechatWorkAgentMigrationService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalAgentMigrationService), 1),
            });
    }

    /// <summary>族断言：父接口 IsAbstract + 指定端点数，子接口集合不漂移 + 指定端点数 + 注册组/继承契约。</summary>
    private static void AssertFamily(
        Type parent,
        string parentImplementation,
        int parentDeclaredEndpointCount,
        (Type Interface, int DeclaredEndpointCount)[] children)
    {
        var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
        parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
        parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
        parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
        parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(parentDeclaredEndpointCount, $"{parent.Name} 承载官方开放面收敛的端点数");

        var assignable = parent.Assembly.GetTypes()
            .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
            .ToList();
        assignable.Should().BeEquivalentTo(children.Select(c => c.Interface),
            $"{parent.Name} 继承链子接口集合不得漂移（官方未开放的应用类型不得补子接口）");

        foreach (var (child, endpointCount) in children)
        {
            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(AgentRegistryGroupName,
                $"{child.Name} 必须挂 {AgentRegistryGroupName} 注册组（Agent 模块共用 Add{AgentRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 承载官方开放面差异端点数");
        }
    }

    // ------------------------------------------------------------------
    // AG3：令牌绑定——应用管理域 12 个接口统一 AccessToken 路由键 + Query 注入
    //（归属域键由 WechatTokenOwnerContractGuards 全局锁定，此处不重复）。
    // ------------------------------------------------------------------

    [Fact]
    public void AgentTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkAgentService),
            typeof(IWechatWorkInternalAgentService),
            typeof(IWechatWorkProviderAgentService),
            typeof(IWechatWorkThirdPartyAgentService),
            typeof(IWechatWorkAgentWorkbenchService),
            typeof(IWechatWorkInternalAgentWorkbenchService),
            typeof(IWechatWorkProviderAgentWorkbenchService),
            typeof(IWechatWorkThirdPartyAgentWorkbenchService),
            typeof(IWechatWorkAgentMenuService),
            typeof(IWechatWorkInternalAgentMenuService),
            typeof(IWechatWorkAgentMigrationService),
            typeof(IWechatWorkInternalAgentMigrationService),
        };

        accessTokenInterfaces.Should().HaveCount(12, "应用管理域四族 = 获取应用族 4 接口 + 工作台自定义展示族 4 接口 + 自定义菜单族 2 接口 + 自建应用迁移成代开发应用族 2 接口");

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌；迁移端点为待迁移自建应用令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    // ------------------------------------------------------------------
    // AG4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Agent）。
    // ------------------------------------------------------------------

    [Fact]
    public void AgentDataModels_ShouldBeRegisteredInJsonContext()
    {
        var agentContext = AgentJsonContext.Default;

        var domainTypes = typeof(GetAgentResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Agent"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 AgentJsonContext 且 SerializerClassName 统一为 Agent
        //（生成物 AgentJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(26,
            "应用管理模块契约面类型数漂移须先核对官方文档再同批调整本守卫（获取应用族 8：获取详情响应 1 + 可见范围嵌套 3 + 应用列表响应 1 + 列表项 1 + 设置应用请求 1 + WechatWorkResponse 复用不计；工作台自定义展示族 14：端点级请求/响应 7 + 模版数据结构 7；自定义菜单族 3：创建请求 1 + 获取响应 1 + 菜单按钮 1；自建应用迁移成代开发应用族 1：请求 1）");

        foreach (var type in domainTypes)
        {
            agentContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于应用管理域命名空间，必须登记进 AgentJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Agent",
                $"{type.Name} 的 SerializerClassName 必须为应用管理域段 Agent");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            // 获取应用族。
            typeof(GetAgentResponse), typeof(AgentAllowUserinfos), typeof(AgentAllowUser),
            typeof(AgentAllowPartys), typeof(AgentAllowTags),
            typeof(GetAgentListResponse), typeof(AgentItem),
            typeof(SetAgentRequest),
            // 工作台自定义展示族（端点级请求/响应 + 模版数据结构全清单）。
            typeof(SetAgentWorkbenchTemplateRequest), typeof(GetAgentWorkbenchTemplateRequest),
            typeof(GetAgentWorkbenchTemplateResponse),
            typeof(SetAgentWorkbenchDataRequest), typeof(BatchSetAgentWorkbenchDataRequest),
            typeof(GetAgentWorkbenchDataRequest), typeof(GetAgentWorkbenchDataResponse),
            typeof(AgentWorkbenchUserData), typeof(AgentWorkbenchKeyData), typeof(AgentWorkbenchKeyDataItem),
            typeof(AgentWorkbenchImage), typeof(AgentWorkbenchList), typeof(AgentWorkbenchListItem),
            typeof(AgentWorkbenchWebview),
            // 自定义菜单族。
            typeof(CreateAgentMenuRequest), typeof(GetAgentMenuResponse), typeof(AgentMenuButton),
            // 自建应用迁移成代开发应用族。
            typeof(ClaimAgentCustomizedAppRequest),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于应用管理域命名空间");
    }

    // ------------------------------------------------------------------
    // AG5：官方契约陷阱锁定——工作台模版数据同域两形态、菜单/应用管理字段拼写形态与双令牌契约。
    // ------------------------------------------------------------------

    [Fact]
    public void AgentDataModels_ShouldLockOfficialContractTraps()
    {
        // 工作台模版数据同域两形态：单用户设置接口的四个模版数据字段平铺于请求体顶层（无 data 包裹），
        // 批量设置接口以 data 对象包裹（官方示例即两形态并存）。
        typeof(SetAgentWorkbenchDataRequest).GetProperty(nameof(SetAgentWorkbenchDataRequest.Keydata))!
            .PropertyType.Should().Be(typeof(AgentWorkbenchKeyData), "单用户设置的 keydata 官方平铺于请求体顶层");
        typeof(SetAgentWorkbenchDataRequest).GetProperty("Data")
            .Should().BeNull("单用户设置接口官方无 data 包裹对象（与批量设置接口形态不同）");
        typeof(BatchSetAgentWorkbenchDataRequest).GetProperty(nameof(BatchSetAgentWorkbenchDataRequest.Data))!
            .PropertyType.Should().Be(typeof(AgentWorkbenchUserData), "批量设置的模版数据官方以 data 对象包裹");
        typeof(GetAgentWorkbenchDataResponse).GetProperty(nameof(GetAgentWorkbenchDataResponse.Data))!
            .PropertyType.Should().Be(typeof(AgentWorkbenchUserData), "获取用户数据响应官方以 data 对象包裹");

        // 设置模版请求与获取模版响应共用四个模版数据结构 + replace_user_data 字段。
        typeof(SetAgentWorkbenchTemplateRequest).GetProperty(nameof(SetAgentWorkbenchTemplateRequest.Image))!
            .PropertyType.Should().Be(typeof(AgentWorkbenchImage), "设置模版请求与获取模版响应共用同一模版数据结构");
        typeof(GetAgentWorkbenchTemplateResponse).GetProperty(nameof(GetAgentWorkbenchTemplateResponse.Image))!
            .PropertyType.Should().Be(typeof(AgentWorkbenchImage), "获取模版响应与设置模版请求共用同一模版数据结构");
        typeof(GetAgentWorkbenchTemplateResponse).GetProperty(nameof(GetAgentWorkbenchTemplateResponse.ReplaceUserData))!
            .PropertyType.Should().Be(typeof(bool?), "获取模版响应承载 replace_user_data 回显");

        // 获取应用详情响应官方字段拼写形态（isreportenter 全小写无驼峰；agentid/agentlist 无下划线）。
        JsonNameShouldBe(typeof(GetAgentResponse), "Agentid", "agentid");
        JsonNameShouldBe(typeof(GetAgentResponse), "AllowUserinfos", "allow_userinfos");
        JsonNameShouldBe(typeof(GetAgentResponse), "AllowPartys", "allow_partys");
        JsonNameShouldBe(typeof(GetAgentResponse), "AllowTags", "allow_tags");
        JsonNameShouldBe(typeof(GetAgentResponse), "Isreportenter", "isreportenter");
        JsonNameShouldBe(typeof(GetAgentResponse), "CustomizedPublishStatus", "customized_publish_status");
        JsonNameShouldBe(typeof(GetAgentListResponse), "Agentlist", "agentlist");
        JsonNameShouldBe(typeof(SetAgentRequest), "LogoMediaid", "logo_mediaid");

        // 自定义菜单：菜单按钮递归结构（sub_button 元素与自身同构）+ 创建请求 button 数组。
        typeof(AgentMenuButton).GetProperty(nameof(AgentMenuButton.SubButton))!
            .PropertyType.Should().Be(typeof(List<AgentMenuButton>), "二级菜单数组元素与一级菜单同构（官方递归结构）");
        JsonNameShouldBe(typeof(AgentMenuButton), "SubButton", "sub_button");
        JsonNameShouldBe(typeof(CreateAgentMenuRequest), "Button", "button");
        typeof(GetAgentMenuResponse).GetProperty(nameof(GetAgentMenuResponse.Button))!
            .PropertyType.Should().Be(typeof(List<AgentMenuButton>), "获取菜单响应与创建菜单请求共用同一菜单按钮结构");

        // 自建应用迁移成代开发应用：suite_access_token 为官方包体参数（请求体字段，非 Query 注入）。
        JsonNameShouldBe(typeof(ClaimAgentCustomizedAppRequest), "SuiteAccessToken", "suite_access_token");
        typeof(ClaimAgentCustomizedAppRequest)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance).Should().ContainSingle(
                "待迁移自建应用的 access_token 走 Query 注入，请求体仅承载 suite_access_token 一个包体参数")
            .Which.Name.Should().Be(nameof(ClaimAgentCustomizedAppRequest.SuiteAccessToken));
    }

    /// <summary>断言指定方法的 agentid 参数为官方 Query 参数（int 类型）。</summary>
    private static void AssertAgentidQueryParameter(Type iface, string method)
    {
        var parameter = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance)!
            .GetParameters()
            .SingleOrDefault(p => p.GetCustomAttribute<QueryAttribute>() is { } q && q.Name == "agentid");
        parameter.Should().NotBeNull($"{iface.Name}.{method} 的 agentid 必须声明为 [Query(\"agentid\")]（官方契约）");
        parameter!.ParameterType.Should().Be(typeof(int), $"{iface.Name}.{method} 的 agentid 官方为整型应用 id");
    }

    private static void AssertRoutes((Type Interface, string Method, Type HttpAttribute, string Route)[] routes)
    {
        foreach (var (iface, method, httpAttribute, route) in routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>JSON 字段名断言：属性映射的官方字段名必须与官方原文一致（拼写差异属官方契约）。</summary>
    private static void JsonNameShouldBe(Type dtoType, string propertyName, string expectedJsonName)
    {
        var property = dtoType.GetProperty(propertyName);
        property.Should().NotBeNull($"{dtoType.Name}.{propertyName} 必须存在");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        jsonName.Should().Be(expectedJsonName,
            $"{dtoType.Name}.{propertyName} 的官方字段名必须照抄原文");
    }
}
