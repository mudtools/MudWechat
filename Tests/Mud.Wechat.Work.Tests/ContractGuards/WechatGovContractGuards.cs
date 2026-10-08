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
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 政民沟通模块（Gov 模块）契约守卫：路由表、接口层级与令牌绑定锁定
/// （配置网格结构域：4 个端点为自建/代开发公共面，收敛父接口 + 空标记子接口；
/// 获取网格列表：官方权限表标注代开发与第三方均「暂不支持」，独立成族、继承链上恰好只有自建子接口；
/// 配置事件类别域：4 个端点为自建/代开发公共面，收敛父接口 + 空标记子接口；
/// 巡查上报族 + 居民上报族：各 6 个端点官方仅自建开放（官方权限表对代开发/第三方均标注「暂不支持」），
/// 零端点父接口 + 唯一自建子接口承载端点；
/// 第三方应用对政民沟通全部 21 个端点标注「暂不支持」，各族均不设第三方子接口）。
/// </summary>
/// <remarks>
/// <para>
/// 形态对齐：配置网格结构域与配置事件类别域对齐家校沟通基础域（公共面收敛 + 空标记子接口）；
/// 获取网格列表对齐家校沟通健康上报域（官方仅自建开放，仅自建空标记子接口）；
/// 巡查上报族与居民上报族对齐身份验证二次验证族（官方仅自建开放，零端点父接口 + 唯一自建子接口承载端点）。
/// </para>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：政民沟通 21 个端点中除巡查/居民 get_grid_info 官方即 GET 外
/// 全部官方即 POST（含仅查询语义的 grid/list / get_user_grid_info / list_cata /
/// get_order_list / get_order_info / category_statistic）；list_cata 官方无请求包体；
/// list_cata 响应列表字段官方 JSON 示例为 <c>category_list</c>、参数说明表误写为
/// <c>cata_list</c>，以示例为准；grid/list 与巡查/居民 get_grid_info 响应示例的
/// <c>grid_name</c> 误标为数字，参数说明表明确为网格名称字符串，照抄为字符串；
/// 巡查/居民上报的代开发文档页（97146~97158）与自建页逐字一致但权限表同为「暂不支持」，
/// 不得据此为两族补代开发子接口。
/// </para>
/// </remarks>
public class WechatGovContractGuards
{
    /// <summary>
    /// 各父接口生成实现类名（生成器规则：接口名去 <c>I</c> 前缀，落位于
    /// <c>Mud.Wechat.Work.Interfaces.Internal</c>，internal 不可跨程序集引用，故以字面量锁定）。
    /// </summary>
    private const string GovGridParentImplementationClassName = "WechatWorkGovGridService";

    private const string GovGridListParentImplementationClassName = "WechatWorkGovGridListService";

    private const string GovEventCategoryParentImplementationClassName = "WechatWorkGovEventCategoryService";

    private const string GovPatrolParentImplementationClassName = "WechatWorkGovPatrolService";

    private const string GovResidentParentImplementationClassName = "WechatWorkGovResidentService";

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
    /// 巡查上报族官方路由表（官方仅自建应用开放，代开发/第三方「暂不支持」，
    /// 6 条端点全部由唯一自建子接口承载；获取网格及网格负责人官方即 GET，其余官方即 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] GovPatrolRoutes =
    {
        // 获取配置的网格及网格负责人（93531；官方即 GET，无请求参数）。
        (typeof(IWechatWorkInternalGovPatrolService),
            nameof(IWechatWorkInternalGovPatrolService.GetGridInfoAsync),
            typeof(GetAttribute), "/cgi-bin/report/patrol/get_grid_info"),
        // 获取单位巡查上报数据统计（93532；官方即 POST）。
        (typeof(IWechatWorkInternalGovPatrolService),
            nameof(IWechatWorkInternalGovPatrolService.GetCorpStatusAsync),
            typeof(PostAttribute), "/cgi-bin/report/patrol/get_corp_status"),
        // 获取个人巡查上报数据统计（93533；官方即 POST）。
        (typeof(IWechatWorkInternalGovPatrolService),
            nameof(IWechatWorkInternalGovPatrolService.GetUserStatusAsync),
            typeof(PostAttribute), "/cgi-bin/report/patrol/get_user_status"),
        // 获取上报事件分类统计（93534；官方即 POST）。
        (typeof(IWechatWorkInternalGovPatrolService),
            nameof(IWechatWorkInternalGovPatrolService.GetCategoryStatisticAsync),
            typeof(PostAttribute), "/cgi-bin/report/patrol/category_statistic"),
        // 获取巡查上报事件列表（93536；官方即 POST，cursor/limit 分页）。
        (typeof(IWechatWorkInternalGovPatrolService),
            nameof(IWechatWorkInternalGovPatrolService.GetOrderListAsync),
            typeof(PostAttribute), "/cgi-bin/report/patrol/get_order_list"),
        // 获取巡查上报的事件详情信息（93535；官方即 POST）。
        (typeof(IWechatWorkInternalGovPatrolService),
            nameof(IWechatWorkInternalGovPatrolService.GetOrderInfoAsync),
            typeof(PostAttribute), "/cgi-bin/report/patrol/get_order_info"),
    };

    /// <summary>
    /// 居民上报族官方路由表（官方仅自建应用开放，代开发/第三方「暂不支持」，
    /// 6 条端点全部由唯一自建子接口承载；获取网格及网格负责人官方即 GET，其余官方即 POST；
    /// 与巡查上报族路由段不同（resident vs patrol）、统计字段集不同（pending/total_accepted vs to_be_assigned），
    /// 两族 DTO 不共用端点请求/响应模型）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] GovResidentRoutes =
    {
        // 获取配置的网格及网格负责人（93514；官方即 GET，无请求参数）。
        (typeof(IWechatWorkInternalGovResidentService),
            nameof(IWechatWorkInternalGovResidentService.GetGridInfoAsync),
            typeof(GetAttribute), "/cgi-bin/report/resident/get_grid_info"),
        // 获取单位居民上报数据统计（93515；官方即 POST）。
        (typeof(IWechatWorkInternalGovResidentService),
            nameof(IWechatWorkInternalGovResidentService.GetCorpStatusAsync),
            typeof(PostAttribute), "/cgi-bin/report/resident/get_corp_status"),
        // 获取个人居民上报数据统计（93516；官方即 POST）。
        (typeof(IWechatWorkInternalGovResidentService),
            nameof(IWechatWorkInternalGovResidentService.GetUserStatusAsync),
            typeof(PostAttribute), "/cgi-bin/report/resident/get_user_status"),
        // 获取上报事件分类统计（93517；官方即 POST）。
        (typeof(IWechatWorkInternalGovResidentService),
            nameof(IWechatWorkInternalGovResidentService.GetCategoryStatisticAsync),
            typeof(PostAttribute), "/cgi-bin/report/resident/category_statistic"),
        // 获取居民上报事件列表（93518；官方即 POST，cursor/limit 分页）。
        (typeof(IWechatWorkInternalGovResidentService),
            nameof(IWechatWorkInternalGovResidentService.GetOrderListAsync),
            typeof(PostAttribute), "/cgi-bin/report/resident/get_order_list"),
        // 获取居民上报的事件详情信息（93519；官方即 POST）。
        (typeof(IWechatWorkInternalGovResidentService),
            nameof(IWechatWorkInternalGovResidentService.GetOrderInfoAsync),
            typeof(PostAttribute), "/cgi-bin/report/resident/get_order_info"),
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
    /// 契约守卫 GV1d：巡查上报族全部端点路由必须与官方契约一致
    /// （官方仅自建应用开放，代开发/第三方「暂不支持」，6 条端点由唯一自建子接口承载；
    /// 获取网格及网格负责人官方即 GET，其余官方即 POST，勿「顺手统一」）。
    /// </summary>
    [Fact]
    public void GovPatrolEndpoints_ShouldMatchOfficialRoutes()
    {
        GovPatrolRoutes.Should().HaveCount(6,
            "巡查上报族 6 个端点官方仅自建应用开放，由唯一自建子接口承载");

        var distinctRoutes = GovPatrolRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(6, "巡查上报族各端点路由互不重复");
        distinctRoutes.Should().OnlyContain(r => r.StartsWith("/cgi-bin/report/patrol/", StringComparison.Ordinal),
            "巡查上报族全部路由位于 /cgi-bin/report/patrol/ 段");

        foreach (var (iface, method, httpAttribute, route) in GovPatrolRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 路由段契约：获取网格及网格负责人官方即 GET，其余 5 条官方即 POST。
        GovPatrolRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute))
            .Should().Be(1, "巡查上报族仅获取网格及网格负责人官方即 GET");
        GovPatrolRoutes.Count(r => r.HttpAttribute == typeof(PostAttribute))
            .Should().Be(5, "巡查上报族其余 5 条端点官方即 POST");
    }

    /// <summary>
    /// 契约守卫 GV1e：居民上报族全部端点路由必须与官方契约一致
    /// （官方仅自建应用开放，代开发/第三方「暂不支持」，6 条端点由唯一自建子接口承载；
    /// 获取网格及网格负责人官方即 GET，其余官方即 POST，勿「顺手统一」；
    /// 居民上报与巡查上报路由仅 patrol/resident 段不同，逐条独立断言防串族）。
    /// </summary>
    [Fact]
    public void GovResidentEndpoints_ShouldMatchOfficialRoutes()
    {
        GovResidentRoutes.Should().HaveCount(6,
            "居民上报族 6 个端点官方仅自建应用开放，由唯一自建子接口承载");

        var distinctRoutes = GovResidentRoutes.Select(r => r.Route).Distinct().ToList();
        distinctRoutes.Should().HaveCount(6, "居民上报族各端点路由互不重复");
        distinctRoutes.Should().OnlyContain(r => r.StartsWith("/cgi-bin/report/resident/", StringComparison.Ordinal),
            "居民上报族全部路由位于 /cgi-bin/report/resident/ 段");

        foreach (var (iface, method, httpAttribute, route) in GovResidentRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 路由段契约：获取网格及网格负责人官方即 GET，其余 5 条官方即 POST。
        GovResidentRoutes.Count(r => r.HttpAttribute == typeof(GetAttribute))
            .Should().Be(1, "居民上报族仅获取网格及网格负责人官方即 GET");
        GovResidentRoutes.Count(r => r.HttpAttribute == typeof(PostAttribute))
            .Should().Be(5, "居民上报族其余 5 条端点官方即 POST");

        // 串族防漂移：巡查/居民上报路由尾段同名但 patrol/resident 段互斥。
        GovPatrolRoutes.Select(r => r.Route)
            .Should().NotIntersectWith(GovResidentRoutes.Select(r => r.Route).ToList(),
                "巡查上报与居民上报路由不得共用同一路径（patrol/resident 段互斥）");
    }

    /// <summary>
    /// 契约守卫 GV2：接口层级与生成器注册形态——
    /// 配置网格结构域与配置事件类别域：自建/代开发公共面收敛父接口（IsAbstract），
    /// 自建/代开发子接口均为零差异端点空标记（第三方应用官方「暂不支持」，不得出现第三方子接口）；
    /// 获取网格列表：官方仅自建开放（代开发/第三方「暂不支持」），继承链上恰好只有自建子接口
    /// （能力漂移守卫）；
    /// 巡查上报族与居民上报族：官方仅自建开放（代开发/第三方「暂不支持」），
    /// 零端点父接口 + 唯一自建子接口承载端点（对齐身份验证二次验证族，能力漂移守卫）。
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

        // ── 巡查上报族 / 居民上报族：官方仅自建开放（代开发/第三方「暂不支持」），
        //    零端点父接口 + 唯一自建子接口承载端点（对齐身份验证二次验证族，能力漂移守卫）──
        var selfBuildOnlyFamilies = new[]
        {
            (typeof(IWechatWorkGovPatrolService),
                new[] { typeof(IWechatWorkInternalGovPatrolService) },
                GovPatrolParentImplementationClassName, 6,
                "巡查上报族官方仅向自建应用开放（代开发/第三方暂不支持），继承链上不得出现其它子接口"),
            (typeof(IWechatWorkGovResidentService),
                new[] { typeof(IWechatWorkInternalGovResidentService) },
                GovResidentParentImplementationClassName, 6,
                "居民上报族官方仅向自建应用开放（代开发/第三方暂不支持），继承链上不得出现其它子接口"),
        };

        foreach (var (parent, children, implementationClassName, endpointCount, because) in selfBuildOnlyFamilies)
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
                .Should().BeEmpty("官方仅自建开放：父接口零端点，端点全部由唯一自建子接口承载");

            foreach (var child in children)
            {
                var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
                childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
                childApi!.RegistryGroupName.Should().Be(GovRegistryGroupName,
                    $"{child.Name} 必须挂 {GovRegistryGroupName} 注册组" +
                    $"（Gov 模块共用 Add{GovRegistryGroupName}WebApiHttpClient()）");
                childApi.InheritedFrom.Should().Be(implementationClassName,
                    $"{child.Name} 必须继承父接口生成实现类");

                child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Should().HaveCount(endpointCount, $"{child.Name} 承载本族全部 {endpointCount} 条官方端点");
            }

            CollectInterfaceEndpoints(children[0]).Should().HaveCount(endpointCount,
                "自建应用类型化面为官方开放的本族全部端点");
        }
    }

    /// <summary>
    /// 契约守卫 GV3：令牌绑定——Gov 模块五族（配置网格结构域 + 获取网格列表 + 配置事件类别域 +
    /// 巡查上报族 + 居民上报族）统一消费 AccessToken 路由键并以 Query 注入（官方契约 access_token；
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
            typeof(IWechatWorkGovPatrolService),
            typeof(IWechatWorkInternalGovPatrolService),
            typeof(IWechatWorkGovResidentService),
            typeof(IWechatWorkInternalGovResidentService),
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
    /// （配置网格结构域 11 型 + 配置事件类别域 6 型 + 巡查上报族 12 型 + 居民上报族 12 型 +
    /// 巡查/居民上报两族复用嵌套 4 型，共 45 个契约面类型落 GovJsonContext；
    /// 删除网格 / 修改事件类别 / 删除事件类别官方响应仅 errcode/errmsg，直接复用
    /// <see cref="WechatWorkResponse"/>，list_cata 与巡查/居民 get_grid_info 官方无请求包体、无请求 DTO）。
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
            // 巡查上报族（官方仅自建）：网格及负责人 / 单位统计 / 个人统计。
            typeof(GovPatrolGetGridInfoResponse),
            typeof(GovPatrolGetCorpStatusRequest), typeof(GovPatrolGetCorpStatusResponse),
            typeof(GovPatrolGetUserStatusRequest), typeof(GovPatrolGetUserStatusResponse),
            // 巡查上报族：分类统计 / 事件列表 / 事件详情。
            typeof(GovPatrolGetCategoryStatisticRequest), typeof(GovPatrolGetCategoryStatisticResponse),
            typeof(GovPatrolGetOrderListRequest), typeof(GovPatrolGetOrderListResponse),
            typeof(GovPatrolGetOrderInfoRequest), typeof(GovPatrolGetOrderInfoResponse),
            typeof(GovPatrolOrder),
            // 居民上报族（官方仅自建）：网格及负责人 / 单位统计 / 个人统计。
            typeof(GovResidentGetGridInfoResponse),
            typeof(GovResidentGetCorpStatusRequest), typeof(GovResidentGetCorpStatusResponse),
            typeof(GovResidentGetUserStatusRequest), typeof(GovResidentGetUserStatusResponse),
            // 居民上报族：分类统计 / 事件列表 / 事件详情。
            typeof(GovResidentGetCategoryStatisticRequest), typeof(GovResidentGetCategoryStatisticResponse),
            typeof(GovResidentGetOrderListRequest), typeof(GovResidentGetOrderListResponse),
            typeof(GovResidentGetOrderInfoRequest), typeof(GovResidentGetOrderInfoResponse),
            typeof(GovResidentOrder),
            // 巡查/居民上报两族复用嵌套（报文结构完全一致，按业务对象命名共享）。
            typeof(GovReportGridInfo), typeof(GovReportCategoryStatistic),
            typeof(GovReportLocation), typeof(GovReportProcessItem),
        };

        govTypes.Should().HaveCount(45, "政民沟通模块契约面共 45 型");
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
