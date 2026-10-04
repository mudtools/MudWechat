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
using Mud.Wechat.Work.DataModels.Gov;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 政民沟通模块（Gov 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （配置网格结构域：4 个端点为自建/代开发公共面，收敛父接口 + 空标记子接口；
/// 获取网格列表：官方权限表标注代开发与第三方均「暂不支持」，独立成族、继承链上恰好只有自建子接口；
/// 配置事件类别域：4 个端点为自建/代开发公共面，收敛父接口 + 空标记子接口；
/// 第三方应用对政民沟通全部 9 个端点标注「暂不支持」，三族均不设第三方子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：配置网格结构域与配置事件类别域对齐家校沟通基础域（公共面收敛 + 空标记子接口）；
/// 获取网格列表对齐家校沟通健康上报域（官方仅自建开放，仅自建空标记子接口）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：政民沟通 9 个端点官方即 POST（含仅查询语义的
/// grid/list / get_user_grid_info / list_cata）；list_cata 官方无请求包体；
/// list_cata 响应列表字段官方 JSON 示例为 <c>category_list</c>、参数说明表误写为
/// <c>cata_list</c>，以示例为准；grid/list 响应示例的 <c>grid_name</c> 误标为数字，
/// 参数说明表明确为网格名称字符串，照抄为字符串。
/// </para>
/// </remarks>
public class WechatGovContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string GovGridParentImplementationClassName = "WechatWorkGovGridService";

    private const string GovGridListParentImplementationClassName = "WechatWorkGovGridListService";

    private const string GovEventCategoryParentImplementationClassName = "WechatWorkGovEventCategoryService";

    private const string GovRegistryGroupName = "Gov";

    /// <summary>
    /// 配置网格结构域官方路由表（自建/代开发公共面，4 条端点全部收敛父接口；
    /// 第三方应用官方「暂不支持」）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] GovGridRoutes =
    {
        // 添加网格（自建 94478、代开发 97136；官方即 POST）。
        (typeof(IWechatWorkGovGridService),
            nameof(IWechatWorkGovGridService.AddGridAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/add"),
        // 编辑网格（自建 94479、代开发 97137；官方即 POST）。
        (typeof(IWechatWorkGovGridService),
            nameof(IWechatWorkGovGridService.UpdateGridAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/update"),
        // 删除网格（自建 94480、代开发 97138；官方即 POST）。
        (typeof(IWechatWorkGovGridService),
            nameof(IWechatWorkGovGridService.DeleteGridAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/delete"),
        // 获取用户负责及参与的网格列表（自建 94482、代开发 97140；官方即 POST）。
        (typeof(IWechatWorkGovGridService),
            nameof(IWechatWorkGovGridService.GetUserGridInfoAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/get_user_grid_info"),
    };

    /// <summary>
    /// 获取网格列表官方路由表（官方仅自建应用开放，代开发/第三方「暂不支持」，1 条端点收敛父接口）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] GovGridListRoutes =
    {
        // 获取网格列表（94481；官方即 POST，grid_id 可选请求体）。
        (typeof(IWechatWorkGovGridListService),
            nameof(IWechatWorkGovGridListService.GetGridListAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/list"),
    };

    /// <summary>
    /// 配置事件类别域官方路由表（自建/代开发公共面，4 条端点全部收敛父接口；
    /// 第三方应用官方「暂不支持」；list_cata 官方无请求包体）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] GovEventCategoryRoutes =
    {
        // 添加事件类别（自建 94536、代开发 97141；官方即 POST）。
        (typeof(IWechatWorkGovEventCategoryService),
            nameof(IWechatWorkGovEventCategoryService.AddEventCategoryAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/add_cata"),
        // 修改事件类别（自建 94537、代开发 97142；官方即 POST）。
        (typeof(IWechatWorkGovEventCategoryService),
            nameof(IWechatWorkGovEventCategoryService.UpdateEventCategoryAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/update_cata"),
        // 删除事件类别（自建 94538、代开发 97143；官方即 POST）。
        (typeof(IWechatWorkGovEventCategoryService),
            nameof(IWechatWorkGovEventCategoryService.DeleteEventCategoryAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/delete_cata"),
        // 获取事件类别列表（自建 94540、代开发 99548；官方即 POST 且无请求包体）。
        (typeof(IWechatWorkGovEventCategoryService),
            nameof(IWechatWorkGovEventCategoryService.GetEventCategoryListAsync),
            typeof(PostAttribute), "/cgi-bin/report/grid/list_cata"),
    };

    /// <summary>
    /// 契约守卫 GV1a：配置网格结构域全部端点路由必须与官方契约一致
    /// （4 个端点为自建/代开发公共面，全部收敛父接口；9 个端点官方即 POST，勿「顺手统一」为 GET）。
    /// </summary>
    [Fact]
    public void GovGridEndpoints_ShouldMatchOfficialRoutes()
    {
        GovGridRoutes.Should().HaveCount(4,
            "配置网格结构域 4 个端点为自建/代开发公共面，全部收敛父接口");

        var distinctRoutes = GovGridRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "配置网格结构域各端点路由互不重复");

        foreach (var (iface, method, httpAttribute, route) in GovGridRoutes)
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
    /// 契约守卫 GV1b：获取网格列表端点路由必须与官方契约一致
    /// （官方仅自建应用开放，代开发/第三方「暂不支持」；官方即 POST，勿「顺手统一」为 GET）。
    /// </summary>
    [Fact]
    public void GovGridListEndpoints_ShouldMatchOfficialRoutes()
    {
        GovGridListRoutes.Should().HaveCount(1,
            "获取网格列表 1 个端点官方仅自建应用开放，收敛父接口");

        var distinctRoutes = GovGridListRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(1, "获取网格列表端点路由唯一");

        foreach (var (iface, method, httpAttribute, route) in GovGridListRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>
    /// 契约守卫 GV1c：配置事件类别域全部端点路由必须与官方契约一致
    /// （4 个端点为自建/代开发公共面，全部收敛父接口；官方即 POST；
    /// list_cata 官方无请求包体，不得凭空补请求体）。
    /// </summary>
    [Fact]
    public void GovEventCategoryEndpoints_ShouldMatchOfficialRoutes()
    {
        GovEventCategoryRoutes.Should().HaveCount(4,
            "配置事件类别域 4 个端点为自建/代开发公共面，全部收敛父接口");

        var distinctRoutes = GovEventCategoryRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(4, "配置事件类别域各端点路由互不重复");
        distinctRoutes.Should().OnlyContain(r => r.StartsWith("/cgi-bin/report/grid/", StringComparison.Ordinal),
            "政民沟通全部端点路由位于 /cgi-bin/report/grid/ 段");

        foreach (var (iface, method, httpAttribute, route) in GovEventCategoryRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // list_cata 官方无请求包体：除 CancellationToken 外不得声明 [Body] 参数。
        var listCata = typeof(IWechatWorkGovEventCategoryService).GetMethod(
            nameof(IWechatWorkGovEventCategoryService.GetEventCategoryListAsync),
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        listCata.Should().NotBeNull();
        listCata!.GetParameters()
            .Any(p => p.GetCustomAttribute<BodyAttribute>() is not null)
            .Should().BeFalse("list_cata 官方无请求包体，不得声明 [Body] 参数");
    }

    /// <summary>
    /// 契约守卫 GV2：接口层级与生成器注册形态——
    /// 配置网格结构域与配置事件类别域：自建/代开发公共面收敛父接口（IsAbstract），
    /// 自建/代开发子接口均为零差异端点空标记（第三方应用官方「暂不支持」，不得出现第三方子接口）；
    /// 获取网格列表：官方仅自建开放（代开发/第三方「暂不支持」），继承链上恰好只有自建子接口
    /// （能力漂移守卫）。
    /// </summary>
    [Fact]
    public void GovInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        var families = new[]
        {
            (typeof(IWechatWorkGovGridService),
                new[] { typeof(IWechatWorkInternalGovGridService), typeof(IWechatWorkProviderGovGridService) },
                GovGridParentImplementationClassName,
                "配置网格结构域 4 个端点为自建/代开发公共面（第三方暂不支持），继承链上不得出现其它子接口"),
            (typeof(IWechatWorkGovGridListService),
                new[] { typeof(IWechatWorkInternalGovGridListService) },
                GovGridListParentImplementationClassName,
                "获取网格列表官方仅向自建应用开放（代开发/第三方暂不支持），继承链上不得出现其它子接口"),
            (typeof(IWechatWorkGovEventCategoryService),
                new[] { typeof(IWechatWorkInternalGovEventCategoryService), typeof(IWechatWorkProviderGovEventCategoryService) },
                GovEventCategoryParentImplementationClassName,
                "配置事件类别域 4 个端点为自建/代开发公共面（第三方暂不支持），继承链上不得出现其它子接口"),
        };

        foreach (var (parent, children, implementationClassName, because) in families)
        {
            var assignable = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
                .ToList();

            assignable.Should().BeEquivalentTo(children, because);

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull("父接口必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeEmpty("公共端点全部声明于父接口");

            foreach (var child in children)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(GovRegistryGroupName,
                    $"{child.Name} 必须挂 {GovRegistryGroupName} 注册组" +
                    $"（Gov 模块共用 Add{GovRegistryGroupName}WebApiHttpClient()）");
                childApi.InheritedFrom.Should().Be(implementationClassName,
                    $"{child.Name} 必须继承父接口生成实现类，避免生成器重复实现公共端点");

                // 开放面完全一致（或仅自建单子接口）：任何子接口出现差异端点均为能力漂移。
                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().BeEmpty($"{child.Name} 为空标记：公共端点全部声明于父接口");
            }
        }

        // 类型化面：自建/代开发为官方开放的 4 条公共面，网格列表自建为官方开放的 1 条。
        CollectInterfaceEndpoints(typeof(IWechatWorkInternalGovGridService)).Should().HaveCount(4,
            "自建应用类型化面为官方开放的 4 条配置网格结构端点");
        CollectInterfaceEndpoints(typeof(IWechatWorkProviderGovGridService)).Should().HaveCount(4,
            "代开发应用类型化面为官方开放的 4 条配置网格结构端点");
        CollectInterfaceEndpoints(typeof(IWechatWorkInternalGovGridListService)).Should().HaveCount(1,
            "自建应用类型化面为官方开放的 1 条获取网格列表端点");
        CollectInterfaceEndpoints(typeof(IWechatWorkInternalGovEventCategoryService)).Should().HaveCount(4,
            "自建应用类型化面为官方开放的 4 条配置事件类别端点");
        CollectInterfaceEndpoints(typeof(IWechatWorkProviderGovEventCategoryService)).Should().HaveCount(4,
            "代开发应用类型化面为官方开放的 4 条配置事件类别端点");
    }

    /// <summary>
    /// 契约守卫 GV3：令牌绑定——Gov 模块三族（配置网格结构域 + 获取网格列表 + 配置事件类别域）
    /// 统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token；
    /// 自建为应用自身令牌，代开发为授权企业级令牌 scope = authCorpId）。
    /// </summary>
    [Fact]
    public void GovTokenBinding_ShouldMatchOfficialTokenRouteKeys()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkGovGridService),
            typeof(IWechatWorkInternalGovGridService),
            typeof(IWechatWorkProviderGovGridService),
            typeof(IWechatWorkGovGridListService),
            typeof(IWechatWorkInternalGovGridListService),
            typeof(IWechatWorkGovEventCategoryService),
            typeof(IWechatWorkInternalGovEventCategoryService),
            typeof(IWechatWorkProviderGovEventCategoryService),
        };

        foreach (var iface in accessTokenInterfaces)
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
    /// 契约守卫 GV4：政民沟通模块的请求/响应 DTO 必须登记进 AOT JSON 上下文
    /// （配置网格结构域 11 型 + 配置事件类别域 6 型，共 17 个契约面类型落 GovJsonContext；
    /// 删除网格 / 修改事件类别 / 删除事件类别官方响应仅 errcode/errmsg，直接复用
    /// <see cref="WechatWorkResponse"/>，list_cata 官方无请求包体、无请求 DTO）。
    /// </summary>
    [Fact]
    public void GovDataModels_ShouldBeRegisteredInJsonContext()
    {
        // 经公共 API GetTypeInfo 判定注册态（源生成上下文对未登记类型返回 null）。
        var govContext = GovJsonContext.Default;

        var govTypes = new[]
        {
            // 配置网格结构域：添加 / 编辑 / 删除。
            typeof(GovAddGridRequest), typeof(GovAddGridResponse),
            typeof(GovUpdateGridRequest), typeof(GovUpdateGridResponse),
            typeof(GovDeleteGridRequest),
            // 配置网格结构域：获取网格列表（官方仅自建）。
            typeof(GovGetGridListRequest), typeof(GovGetGridListResponse), typeof(GovGridInfo),
            // 配置网格结构域：获取用户负责及参与的网格列表。
            typeof(GovGetUserGridInfoRequest), typeof(GovGetUserGridInfoResponse), typeof(GovGridBrief),
            // 配置事件类别域：添加 / 修改 / 删除。
            typeof(GovAddEventCategoryRequest), typeof(GovAddEventCategoryResponse),
            typeof(GovUpdateEventCategoryRequest),
            typeof(GovDeleteEventCategoryRequest),
            // 配置事件类别域：获取事件类别列表。
            typeof(GovGetEventCategoryListResponse), typeof(GovEventCategoryInfo),
        };

        govTypes.Should().HaveCount(17, "政民沟通模块契约面共 17 型");
        govTypes.Should().OnlyHaveUniqueItems("契约面类型不得重复断言");

        foreach (var type in govTypes)
        {
            govContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是政民沟通模块契约面类型，必须登记进 GovJsonContext（AOT 源生成）");
        }

        // list_cata 响应列表字段：官方 JSON 示例为 category_list、参数说明表误写为 cata_list，以示例为准。
        var categoryListProperty = typeof(GovGetEventCategoryListResponse).GetProperty(nameof(GovGetEventCategoryListResponse.CategoryList));
        categoryListProperty.Should().NotBeNull();
        categoryListProperty!.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("category_list",
            "list_cata 响应列表字段必须照抄官方 JSON 示例的 category_list（参数表 cata_list 为官方笔误）");
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
}
