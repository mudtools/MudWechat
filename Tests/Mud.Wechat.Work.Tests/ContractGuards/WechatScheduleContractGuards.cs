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
using Mud.Wechat.Work.DataModels.Schedule;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 日程模块（Schedule 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 管理日历族形态：创建/更新/获取/删除日历 4 端点官方对三类应用开放一致，
/// 全部收敛声明于公共父接口，三个应用类型子接口均为零差异端点空标记。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：创建日历官方路由为 <c>/cgi-bin/oa/calendar/add</c> 而非 create，
/// 删除日历为 <c>calendar/del</c>；日历域 4 条路由官方全部即 POST（含仅查询语义的 calendar/get）；
/// 更新日历为<b>覆盖式</b>而非增量式；创建/更新日历请求的 calendar 对象官方参数表高度同构
///（差异仅为 cal_id 与 set_as_default / is_public / is_corp_calendar 三个创建侧属性），本 SDK 以共用扁平结构承载；
/// 获取日历详情响应的日历管理员字段官方参数表作 <c>admins</c>、三类应用文档页返回示例均作 <c>adminis</c>，以示例为准；
/// 创建/更新日历整体 errcode 为 0 时响应仍可能携带 fail_result.shares（无效通知范围成员逐成员报错）。
/// </para>
/// </remarks>
public class WechatScheduleContractGuards
{
    private const string ScheduleRegistryGroupName = "Schedule";

    // ------------------------------------------------------------------
    // 路由表：4 条官方路由，全部 POST，挂 /cgi-bin/oa/calendar/ 段。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        ScheduleCalendarRoutes =
        {
            // 创建日历（自建 93647 / 第三方 93702 / 代开发 96823；三类公共收敛父接口；官方即 POST、路由为 add 而非 create）。
            (typeof(IWechatWorkScheduleCalendarService),
                nameof(IWechatWorkScheduleCalendarService.AddCalendarAsync),
                typeof(PostAttribute), "/cgi-bin/oa/calendar/add"),
            // 更新日历（自建 97716 / 第三方 97783 / 代开发 97758；覆盖式更新；官方即 POST）。
            (typeof(IWechatWorkScheduleCalendarService),
                nameof(IWechatWorkScheduleCalendarService.UpdateCalendarAsync),
                typeof(PostAttribute), "/cgi-bin/oa/calendar/update"),
            // 获取日历详情（自建 97717 / 第三方 97784 / 代开发 97759；官方即 POST）。
            (typeof(IWechatWorkScheduleCalendarService),
                nameof(IWechatWorkScheduleCalendarService.GetCalendarAsync),
                typeof(PostAttribute), "/cgi-bin/oa/calendar/get"),
            // 删除日历（自建 97718 / 第三方 97785 / 代开发 97760；官方即 POST）。
            (typeof(IWechatWorkScheduleCalendarService),
                nameof(IWechatWorkScheduleCalendarService.DelCalendarAsync),
                typeof(PostAttribute), "/cgi-bin/oa/calendar/del"),
        };

    // ------------------------------------------------------------------
    // SC1：全部端点路由与官方契约一致（4 条官方路由）。
    // ------------------------------------------------------------------

    [Fact]
    public void ScheduleEndpoints_ShouldMatchOfficialRoutes()
    {
        ScheduleCalendarRoutes.Should().HaveCount(4, "管理日历族 = 创建日历 + 更新日历 + 获取日历详情 + 删除日历");
        ScheduleCalendarRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/oa/calendar/", StringComparison.Ordinal), "管理日历族路由位于 /cgi-bin/oa/calendar/ 段");
        ScheduleCalendarRoutes.Single(r => r.Method == nameof(IWechatWorkScheduleCalendarService.AddCalendarAsync)).Route
            .Should().Be("/cgi-bin/oa/calendar/add", "创建日历官方路由为 calendar/add 而非 create（勿「顺手归位」）");

        AssertRoutes(ScheduleCalendarRoutes);

        // 全部官方路由去重清单锁定。
        ScheduleCalendarRoutes.Select(r => r.Route).Distinct().Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/oa/calendar/add",
            "/cgi-bin/oa/calendar/update",
            "/cgi-bin/oa/calendar/get",
            "/cgi-bin/oa/calendar/del",
        }, "管理日历族全部官方路由须与官方文档一一对应（4 条）");

        // 删除日历无业务负载：响应直接用 WechatWorkResponse，不得新建空响应 DTO。
        typeof(IWechatWorkScheduleCalendarService)
            .GetMethod(nameof(IWechatWorkScheduleCalendarService.DelCalendarAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "删除日历仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
    }

    // ------------------------------------------------------------------
    // SC2：接口层级与生成器注册形态（公共父 4 端点 + 三应用类型空标记子接口）。
    // ------------------------------------------------------------------

    [Fact]
    public void ScheduleInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 管理日历族：官方对三类应用开放一致的 4 端点全部收敛于父接口；三个子接口零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkScheduleCalendarService),
            parentImplementation: "WechatWorkScheduleCalendarService",
            parentDeclaredEndpointCount: 4,
            new[]
            {
                (typeof(IWechatWorkInternalScheduleCalendarService), 0),
                (typeof(IWechatWorkProviderScheduleCalendarService), 0),
                (typeof(IWechatWorkThirdPartyScheduleCalendarService), 0),
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
            childApi!.RegistryGroupName.Should().Be(ScheduleRegistryGroupName,
                $"{child.Name} 必须挂 {ScheduleRegistryGroupName} 注册组（Schedule 模块共用 Add{ScheduleRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 承载官方开放面差异端点数");
        }
    }

    // ------------------------------------------------------------------
    // SC3：令牌绑定——日程域 4 个接口统一 AccessToken 路由键 + Query 注入。
    // ------------------------------------------------------------------

    [Fact]
    public void ScheduleTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkScheduleCalendarService),
            typeof(IWechatWorkInternalScheduleCalendarService),
            typeof(IWechatWorkProviderScheduleCalendarService),
            typeof(IWechatWorkThirdPartyScheduleCalendarService),
        };

        accessTokenInterfaces.Should().HaveCount(4, "日程域管理日历族 = 公共父接口 + 三应用类型子接口");

        foreach (var iface in accessTokenInterfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken（自建为应用自身令牌，第三方/代开发为授权企业级令牌）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }
    }

    // ------------------------------------------------------------------
    // SC4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Schedule）。
    // ------------------------------------------------------------------

    [Fact]
    public void ScheduleDataModels_ShouldBeRegisteredInJsonContext()
    {
        var scheduleContext = ScheduleJsonContext.Default;

        var domainTypes = typeof(ScheduleCalendar).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Schedule"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 ScheduleJsonContext 且 SerializerClassName 统一为 Schedule
        //（生成物 ScheduleJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(13,
            "日程模块契约面类型数漂移须先核对官方文档再同批调整本守卫（端点级 7：创建/更新/获取请求响应 + 删除请求；复用型 6：calendar 对象 + 公开范围 + 通知成员 + fail_result + fail 成员 + 响应日历信息）");

        foreach (var type in domainTypes)
        {
            scheduleContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于日程域命名空间，必须登记进 ScheduleJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Schedule",
                $"{type.Name} 的 SerializerClassName 必须为日程域段 Schedule");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            typeof(AddScheduleCalendarRequest), typeof(AddScheduleCalendarResponse),
            typeof(UpdateScheduleCalendarRequest), typeof(UpdateScheduleCalendarResponse),
            typeof(GetScheduleCalendarRequest), typeof(GetScheduleCalendarResponse),
            typeof(DelScheduleCalendarRequest),
            typeof(ScheduleCalendar), typeof(ScheduleCalendarInfo),
            typeof(ScheduleCalendarPublicRange), typeof(ScheduleCalendarShare),
            typeof(ScheduleCalendarFailResult), typeof(ScheduleCalendarFailShare),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于日程域命名空间");
    }

    // ------------------------------------------------------------------
    // SC5：官方契约陷阱锁定——共用 calendar 扁平结构、adminis 拼写照抄、fail_result 分支。
    // ------------------------------------------------------------------

    [Fact]
    public void ScheduleDataModels_ShouldLockOfficialContractTraps()
    {
        // 创建/更新日历请求的 calendar 对象官方参数表高度同构，本 SDK 以共用扁平结构承载（模板卡片 TemplateCardBody 先例）。
        typeof(AddScheduleCalendarRequest).GetProperty(nameof(AddScheduleCalendarRequest.Calendar))!
            .PropertyType.Should().Be(typeof(ScheduleCalendar), "创建日历 calendar 对象与更新日历共用同一扁平结构");
        typeof(UpdateScheduleCalendarRequest).GetProperty(nameof(UpdateScheduleCalendarRequest.Calendar))!
            .PropertyType.Should().Be(typeof(ScheduleCalendar), "更新日历 calendar 对象与创建日历共用同一扁平结构");

        // 获取日历详情响应管理员字段：官方参数表作 admins、返回示例作 adminis，以示例为准（照抄勿「顺手修正」）。
        JsonNameShouldBe(typeof(ScheduleCalendarInfo), nameof(ScheduleCalendarInfo.Adminis), "adminis");

        // 创建/更新日历响应均携带 fail_result（整体 errcode = 0 时仍可能有无效通知范围成员）。
        typeof(AddScheduleCalendarResponse).GetProperty(nameof(AddScheduleCalendarResponse.FailResult))!
            .PropertyType.Should().Be(typeof(ScheduleCalendarFailResult), "创建日历响应 fail_result 无效输入内容须独立建模");
        typeof(UpdateScheduleCalendarResponse).GetProperty(nameof(UpdateScheduleCalendarResponse.FailResult))!
            .PropertyType.Should().Be(typeof(ScheduleCalendarFailResult), "更新日历响应 fail_result 无效输入内容须独立建模");
        JsonNameShouldBe(typeof(ScheduleCalendarFailShare), nameof(ScheduleCalendarFailShare.Errcode), "errcode");
        JsonNameShouldBe(typeof(ScheduleCalendarFailShare), nameof(ScheduleCalendarFailShare.Userid), "userid");

        // 关键字段名照抄官方原文。
        JsonNameShouldBe(typeof(GetScheduleCalendarRequest), nameof(GetScheduleCalendarRequest.CalIdList), "cal_id_list");
        JsonNameShouldBe(typeof(GetScheduleCalendarResponse), nameof(GetScheduleCalendarResponse.CalendarList), "calendar_list");
        JsonNameShouldBe(typeof(UpdateScheduleCalendarRequest), nameof(UpdateScheduleCalendarRequest.SkipPublicRange), "skip_public_range");
        JsonNameShouldBe(typeof(ScheduleCalendar), nameof(ScheduleCalendar.SetAsDefault), "set_as_default");
        JsonNameShouldBe(typeof(ScheduleCalendar), nameof(ScheduleCalendar.IsCorpCalendar), "is_corp_calendar");
        JsonNameShouldBe(typeof(ScheduleCalendarPublicRange), nameof(ScheduleCalendarPublicRange.Partyids), "partyids");
        JsonNameShouldBe(typeof(DelScheduleCalendarRequest), nameof(DelScheduleCalendarRequest.CalId), "cal_id");
    }

    /// <summary>路由表断言：方法必须存在、必须声明对应 HTTP 方法特性且路由与官方契约一致。</summary>
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
