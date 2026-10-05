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
/// 管理日程族形态：创建/更新日程 + 新增/删除日程参与者 + 获取日历下的日程列表 + 获取日程详情 + 取消日程 7 端点
/// 官方对三类应用开放一致，同样为公共父接口 + 三个应用类型空标记子接口。
/// 待办族形态：获取待办详情 + 更新待办状态 2 端点官方仅向企业自建应用开放
///（第三方应用开发与服务商代开发均无对应 API），为零端点父接口 + 唯一自建子接口承载端点（对齐紧急通知域形态）。
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
/// <para>
/// 管理日程族官方反直觉点（勿「顺手修正」）：取消日程官方路由为 <c>schedule/del</c>（官方标题作「取消日程」而非删除日程）；
/// 日程域 7 条路由官方全部即 POST（含仅查询语义的 schedule/get 与 get_by_calendar）；
/// 更新日程为<b>覆盖式</b>而非增量式，官方「更新重复日程」文档页（96204/96198/96826）与更新日程同一路由、非独立 HTTP API；
/// 创建/更新日程请求的 schedule 对象官方参数表高度同构（差异仅为创建侧 cal_id 与更新侧 schedule_id），本 SDK 以共用扁平结构承载；
/// 获取日历下的日程列表分页采用 offset + limit（区别于本仓多数域的 cursor + limit）；
/// 被取消的日程仍可拉取详情（status = 1），调用方须自行检查 status。
/// </para>
/// <para>
/// 待办族官方反直觉点（勿「顺手修正」）：待办 2 条路由挂 <c>/cgi-bin/todo/</c> 段（区别于日历/日程族的 /cgi-bin/oa/ 段），
/// 且官方全部即 POST（含仅查询语义的 todo/get）；
/// 更新待办状态为按需增量修改（status 与 attendees 均官方选填，attendees 不传或为空数组时不修改参与人列表）；
/// 官方请求示例中 attendees.status 出现值 2，与参数表仅列出 0/1 两种状态不一致，以参数表为准；
/// 获取待办详情响应整体状态口径为「0 - 已完成；1 - 进行中」，更新请求整体状态口径为「0 - 完成；1 - 进行中」，照抄各自原文。
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
    // 管理日程族路由表：7 条官方路由，全部 POST，挂 /cgi-bin/oa/schedule/ 段。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        ScheduleRoutes =
        {
            // 创建日程（自建 93648 / 第三方 93703 / 代开发 96824；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkScheduleService),
                nameof(IWechatWorkScheduleService.AddScheduleAsync),
                typeof(PostAttribute), "/cgi-bin/oa/schedule/add"),
            // 更新日程（自建 97720 / 第三方 97787 / 代开发 97761；覆盖式更新；官方即 POST；
            // 「更新重复日程」说明页 96204/96198/96826 与本端点同一路由、非独立 HTTP API）。
            (typeof(IWechatWorkScheduleService),
                nameof(IWechatWorkScheduleService.UpdateScheduleAsync),
                typeof(PostAttribute), "/cgi-bin/oa/schedule/update"),
            // 新增日程参与者（自建 97721 / 第三方 97789 / 代开发 97763；增量式；官方即 POST）。
            (typeof(IWechatWorkScheduleService),
                nameof(IWechatWorkScheduleService.AddScheduleAttendeesAsync),
                typeof(PostAttribute), "/cgi-bin/oa/schedule/add_attendees"),
            // 删除日程参与者（自建 97722 / 第三方 97794 / 代开发 97764；增量式；官方即 POST）。
            (typeof(IWechatWorkScheduleService),
                nameof(IWechatWorkScheduleService.DelScheduleAttendeesAsync),
                typeof(PostAttribute), "/cgi-bin/oa/schedule/del_attendees"),
            // 获取日历下的日程列表（自建 97723 / 第三方 97796 / 代开发 97765；官方即 POST、分页 offset + limit）。
            (typeof(IWechatWorkScheduleService),
                nameof(IWechatWorkScheduleService.ListSchedulesByCalendarAsync),
                typeof(PostAttribute), "/cgi-bin/oa/schedule/get_by_calendar"),
            // 获取日程详情（自建 97724 / 第三方 97798 / 代开发 97766；官方即 POST）。
            (typeof(IWechatWorkScheduleService),
                nameof(IWechatWorkScheduleService.GetScheduleAsync),
                typeof(PostAttribute), "/cgi-bin/oa/schedule/get"),
            // 取消日程（自建 97725 / 第三方 97799 / 代开发 97767；官方标题「取消日程」而路由为 del，勿「顺手归位」）。
            (typeof(IWechatWorkScheduleService),
                nameof(IWechatWorkScheduleService.DelScheduleAsync),
                typeof(PostAttribute), "/cgi-bin/oa/schedule/del"),
        };

    // ------------------------------------------------------------------
    // 待办族路由表：2 条官方路由，全部 POST，挂 /cgi-bin/todo/ 段（区别于日历/日程族的 /cgi-bin/oa/ 段）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        ScheduleTodoRoutes =
        {
            // 获取待办详情（自建 101524；官方仅自建应用开放，第三方应用开发与服务商代开发均无对应 API；官方即 POST）。
            (typeof(IWechatWorkInternalScheduleTodoService),
                nameof(IWechatWorkInternalScheduleTodoService.GetTodoAsync),
                typeof(PostAttribute), "/cgi-bin/todo/get"),
            // 更新待办状态（自建 101534；官方仅自建应用开放；官方即 POST；按需增量修改，attendees 不传不修改参与人列表）。
            (typeof(IWechatWorkInternalScheduleTodoService),
                nameof(IWechatWorkInternalScheduleTodoService.UpdateTodoAsync),
                typeof(PostAttribute), "/cgi-bin/todo/update"),
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
    // SC1b：管理日程族全部端点路由与官方契约一致（7 条官方路由）。
    // ------------------------------------------------------------------

    [Fact]
    public void ManageScheduleEndpoints_ShouldMatchOfficialRoutes()
    {
        ScheduleRoutes.Should().HaveCount(7,
            "管理日程族 = 创建日程 + 更新日程 + 新增/删除日程参与者 + 获取日历下的日程列表 + 获取日程详情 + 取消日程");
        ScheduleRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/oa/schedule/", StringComparison.Ordinal), "管理日程族路由位于 /cgi-bin/oa/schedule/ 段");
        ScheduleRoutes.Single(r => r.Method == nameof(IWechatWorkScheduleService.DelScheduleAsync)).Route
            .Should().Be("/cgi-bin/oa/schedule/del", "取消日程官方标题为「取消日程」而路由为 del（勿「顺手归位」为 cancel）");

        AssertRoutes(ScheduleRoutes);

        // 全部官方路由去重清单锁定。
        ScheduleRoutes.Select(r => r.Route).Distinct().Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/oa/schedule/add",
            "/cgi-bin/oa/schedule/update",
            "/cgi-bin/oa/schedule/add_attendees",
            "/cgi-bin/oa/schedule/del_attendees",
            "/cgi-bin/oa/schedule/get_by_calendar",
            "/cgi-bin/oa/schedule/get",
            "/cgi-bin/oa/schedule/del",
        }, "管理日程族全部官方路由须与官方文档一一对应（7 条）");

        // 无业务负载端点：响应直接用 WechatWorkResponse，不得新建空响应 DTO。
        var noPayloadMethods = new[]
        {
            nameof(IWechatWorkScheduleService.DelScheduleAsync),
            nameof(IWechatWorkScheduleService.AddScheduleAttendeesAsync),
            nameof(IWechatWorkScheduleService.DelScheduleAttendeesAsync),
        };
        foreach (var method in noPayloadMethods)
        {
            typeof(IWechatWorkScheduleService)
                .GetMethod(method, BindingFlags.Public | BindingFlags.Instance)!
                .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                    $"{method} 仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        }
    }

    // ------------------------------------------------------------------
    // SC1c：待办族全部端点路由与官方契约一致（2 条官方路由，仅自建开放）。
    // ------------------------------------------------------------------

    [Fact]
    public void TodoEndpoints_ShouldMatchOfficialRoutes()
    {
        ScheduleTodoRoutes.Should().HaveCount(2, "待办族 = 获取待办详情 + 更新待办状态");
        ScheduleTodoRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/todo/", StringComparison.Ordinal), "待办族路由位于 /cgi-bin/todo/ 段（区别于日历/日程族的 /cgi-bin/oa/ 段）");

        AssertRoutes(ScheduleTodoRoutes);

        // 全部官方路由去重清单锁定。
        ScheduleTodoRoutes.Select(r => r.Route).Distinct().Should().BeEquivalentTo(new[]
        {
            "/cgi-bin/todo/get",
            "/cgi-bin/todo/update",
        }, "待办族全部官方路由须与官方文档一一对应（2 条）");

        // 更新待办状态无业务负载：响应直接用 WechatWorkResponse，不得新建空响应 DTO。
        typeof(IWechatWorkInternalScheduleTodoService)
            .GetMethod(nameof(IWechatWorkInternalScheduleTodoService.UpdateTodoAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "更新待办状态仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
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

    // ------------------------------------------------------------------
    // SC2b：管理日程族接口层级与生成器注册形态（公共父 7 端点 + 三应用类型空标记子接口）。
    // ------------------------------------------------------------------

    [Fact]
    public void ManageScheduleInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 管理日程族：官方对三类应用开放一致的 7 端点全部收敛于父接口；三个子接口零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkScheduleService),
            parentImplementation: "WechatWorkScheduleService",
            parentDeclaredEndpointCount: 7,
            new[]
            {
                (typeof(IWechatWorkInternalScheduleService), 0),
                (typeof(IWechatWorkProviderScheduleService), 0),
                (typeof(IWechatWorkThirdPartyScheduleService), 0),
            });
    }

    // ------------------------------------------------------------------
    // SC2c：待办族接口层级与生成器注册形态（仅自建开放：零端点父接口 + 唯一自建子接口承载 2 端点）。
    // ------------------------------------------------------------------

    [Fact]
    public void TodoInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 待办族：官方仅向企业自建应用开放（第三方应用开发与服务商代开发均无对应 API），
        // 继承链上不得出现代开发 / 第三方子接口（能力漂移守卫）。
        AssertFamily(
            parent: typeof(IWechatWorkScheduleTodoService),
            parentImplementation: "WechatWorkScheduleTodoService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalScheduleTodoService), 2),
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
    // SC3：令牌绑定——日程域 10 个接口统一 AccessToken 路由键 + Query 注入；
    // 待办族仅自建子接口须声明凭据归属域键（InternalAccessToken）。
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
            typeof(IWechatWorkScheduleService),
            typeof(IWechatWorkInternalScheduleService),
            typeof(IWechatWorkProviderScheduleService),
            typeof(IWechatWorkThirdPartyScheduleService),
            typeof(IWechatWorkScheduleTodoService),
            typeof(IWechatWorkInternalScheduleTodoService),
        };

        accessTokenInterfaces.Should().HaveCount(10,
            "日程域 = 管理日历族（父 + 三子）+ 管理日程族（父 + 三子）+ 待办族（零端点父 + 仅自建子，官方未对第三方/代开发开放）");

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

        // 待办族零端点父接口不声明归属域（默认旧键 "AccessToken"，运行期不校验）；仅自建子接口必须声明内部应用令牌归属域键。
        var parentTokenKey = typeof(IWechatWorkScheduleTodoService).GetCustomAttribute<TokenAttribute>()!.TokenManagerKey;
        parentTokenKey.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken, "待办族父接口不声明凭据归属域（归属域由应用类型子接口承载）");
        parentTokenKey.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken, "待办族父接口不声明凭据归属域（归属域由应用类型子接口承载）");
        typeof(IWechatWorkInternalScheduleTodoService).GetCustomAttribute<TokenAttribute>()!
            .TokenManagerKey.Should().Be(WechatTokenManagerKeys.InternalAccessToken,
                "待办族官方仅自建应用开放，自建子接口必须声明 InternalAccessToken 归属域键");
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
        domainTypes.Should().HaveCount(35,
            "日程模块契约面类型数漂移须先核对官方文档再同批调整本守卫（端点级 21：管理日历 7 = 创建/更新/获取请求响应 + 删除请求、" +
            "管理日程 11 = 创建/更新/获取/获取日历下日程列表请求响应 + 取消/新增参与者/删除参与者请求、" +
            "待办 3 = 获取请求响应 + 更新请求；复用型 14：日历族 6 = calendar 对象 + 公开范围 + 通知成员 + fail_result + fail 成员 + 响应日历信息、" +
            "日程族 6 = schedule 对象 + 请求提醒 + 响应提醒 + 参与者 + 排除日期 + 响应日程详情、" +
            "待办族 2 = 参与人 + 提醒）");

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
            typeof(AddScheduleRequest), typeof(AddScheduleResponse),
            typeof(UpdateScheduleRequest), typeof(UpdateScheduleResponse),
            typeof(GetScheduleRequest), typeof(GetScheduleResponse),
            typeof(ListSchedulesByCalendarRequest), typeof(ListSchedulesByCalendarResponse),
            typeof(DelScheduleRequest),
            typeof(AddScheduleAttendeesRequest), typeof(DelScheduleAttendeesRequest),
            typeof(ScheduleInfo), typeof(ScheduleDetail),
            typeof(ScheduleReminders), typeof(ScheduleRemindersInfo),
            typeof(ScheduleAttendee), typeof(ScheduleExcludeTime),
            typeof(GetScheduleTodoRequest), typeof(GetScheduleTodoResponse),
            typeof(UpdateScheduleTodoRequest),
            typeof(ScheduleTodoAttendee), typeof(ScheduleTodoReminder),
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

        // 创建/更新日程请求的 schedule 对象官方参数表高度同构，本 SDK 以共用扁平结构承载（Calendar 家族同型先例）。
        typeof(AddScheduleRequest).GetProperty(nameof(AddScheduleRequest.Schedule))!
            .PropertyType.Should().Be(typeof(ScheduleInfo), "创建日程 schedule 对象与更新日程共用同一扁平结构");
        typeof(UpdateScheduleRequest).GetProperty(nameof(UpdateScheduleRequest.Schedule))!
            .PropertyType.Should().Be(typeof(ScheduleInfo), "更新日程 schedule 对象与创建日程共用同一扁平结构");

        // 获取日程详情/日历下日程列表两读端点共用响应侧日程对象（字段差异以可空性承载）。
        typeof(GetScheduleResponse).GetProperty(nameof(GetScheduleResponse.ScheduleList))!
            .PropertyType.Should().Be(typeof(List<ScheduleDetail>), "获取日程详情与日历下日程列表响应共用同一日程对象");
        typeof(ListSchedulesByCalendarResponse).GetProperty(nameof(ListSchedulesByCalendarResponse.ScheduleList))!
            .PropertyType.Should().Be(typeof(List<ScheduleDetail>), "获取日历下的日程列表与获取日程详情响应共用同一日程对象");

        // 管理日程族关键字段名照抄官方原文。
        JsonNameShouldBe(typeof(AddScheduleRequest), nameof(AddScheduleRequest.Schedule), "schedule");
        JsonNameShouldBe(typeof(AddScheduleRequest), nameof(AddScheduleRequest.AgentId), "agentid");
        JsonNameShouldBe(typeof(AddScheduleResponse), nameof(AddScheduleResponse.ScheduleId), "schedule_id");
        JsonNameShouldBe(typeof(UpdateScheduleRequest), nameof(UpdateScheduleRequest.SkipAttendees), "skip_attendees");
        JsonNameShouldBe(typeof(UpdateScheduleRequest), nameof(UpdateScheduleRequest.OpMode), "op_mode");
        JsonNameShouldBe(typeof(UpdateScheduleRequest), nameof(UpdateScheduleRequest.OpStartTime), "op_start_time");
        JsonNameShouldBe(typeof(UpdateScheduleResponse), nameof(UpdateScheduleResponse.ScheduleId), "schedule_id");
        JsonNameShouldBe(typeof(GetScheduleRequest), nameof(GetScheduleRequest.ScheduleIdList), "schedule_id_list");
        JsonNameShouldBe(typeof(ListSchedulesByCalendarRequest), nameof(ListSchedulesByCalendarRequest.CalId), "cal_id");
        JsonNameShouldBe(typeof(DelScheduleRequest), nameof(DelScheduleRequest.ScheduleId), "schedule_id");
        JsonNameShouldBe(typeof(DelScheduleRequest), nameof(DelScheduleRequest.OpMode), "op_mode");
        JsonNameShouldBe(typeof(DelScheduleRequest), nameof(DelScheduleRequest.OpStartTime), "op_start_time");
        JsonNameShouldBe(typeof(ScheduleInfo), nameof(ScheduleInfo.ScheduleId), "schedule_id");
        JsonNameShouldBe(typeof(ScheduleInfo), nameof(ScheduleInfo.CalId), "cal_id");
        JsonNameShouldBe(typeof(ScheduleInfo), nameof(ScheduleInfo.IsWholeDay), "is_whole_day");
        JsonNameShouldBe(typeof(ScheduleReminders), nameof(ScheduleReminders.RemindBeforeEventSecs), "remind_before_event_secs");
        JsonNameShouldBe(typeof(ScheduleReminders), nameof(ScheduleReminders.RemindTimeDiffs), "remind_time_diffs");
        JsonNameShouldBe(typeof(ScheduleReminders), nameof(ScheduleReminders.IsCustomRepeat), "is_custom_repeat");
        JsonNameShouldBe(typeof(ScheduleReminders), nameof(ScheduleReminders.RepeatUntil), "repeat_until");
        JsonNameShouldBe(typeof(ScheduleReminders), nameof(ScheduleReminders.RepeatDayOfWeek), "repeat_day_of_week");
        JsonNameShouldBe(typeof(ScheduleReminders), nameof(ScheduleReminders.RepeatDayOfMonth), "repeat_day_of_month");
        JsonNameShouldBe(typeof(ScheduleRemindersInfo), nameof(ScheduleRemindersInfo.ExcludeTimeList), "exclude_time_list");
        JsonNameShouldBe(typeof(ScheduleAttendee), nameof(ScheduleAttendee.ResponseStatus), "response_status");
        JsonNameShouldBe(typeof(ScheduleAttendee), nameof(ScheduleAttendee.EventTime), "event_time");
        JsonNameShouldBe(typeof(ScheduleDetail), nameof(ScheduleDetail.Sequence), "sequence");

        // 待办族关键字段名照抄官方原文（todo/get 与 todo/update 均官方即 POST，路由挂 /cgi-bin/todo/ 段）。
        JsonNameShouldBe(typeof(GetScheduleTodoRequest), nameof(GetScheduleTodoRequest.TodoId), "todo_id");
        JsonNameShouldBe(typeof(UpdateScheduleTodoRequest), nameof(UpdateScheduleTodoRequest.TodoId), "todo_id");
        JsonNameShouldBe(typeof(UpdateScheduleTodoRequest), nameof(UpdateScheduleTodoRequest.Status), "status");
        JsonNameShouldBe(typeof(UpdateScheduleTodoRequest), nameof(UpdateScheduleTodoRequest.Attendees), "attendees");
        JsonNameShouldBe(typeof(GetScheduleTodoResponse), nameof(GetScheduleTodoResponse.Content), "content");
        JsonNameShouldBe(typeof(GetScheduleTodoResponse), nameof(GetScheduleTodoResponse.Creator), "creator");
        JsonNameShouldBe(typeof(GetScheduleTodoResponse), nameof(GetScheduleTodoResponse.Status), "status");
        JsonNameShouldBe(typeof(GetScheduleTodoResponse), nameof(GetScheduleTodoResponse.CreateTime), "create_time");
        JsonNameShouldBe(typeof(GetScheduleTodoResponse), nameof(GetScheduleTodoResponse.Attendees), "attendees");
        JsonNameShouldBe(typeof(GetScheduleTodoResponse), nameof(GetScheduleTodoResponse.EndTime), "end_time");
        JsonNameShouldBe(typeof(GetScheduleTodoResponse), nameof(GetScheduleTodoResponse.Reminders), "reminders");
        JsonNameShouldBe(typeof(ScheduleTodoAttendee), nameof(ScheduleTodoAttendee.Userid), "userid");
        JsonNameShouldBe(typeof(ScheduleTodoAttendee), nameof(ScheduleTodoAttendee.Status), "status");
        JsonNameShouldBe(typeof(ScheduleTodoReminder), nameof(ScheduleTodoReminder.RemindTime), "remind_time");
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
