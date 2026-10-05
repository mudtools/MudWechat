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
using Mud.Wechat.Work.DataModels.Meeting;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 会议模块（Meeting 模块）契约守卫：路由表、接口层级、开放面与令牌绑定锁定。
/// <para>
/// 三功能族形态：
/// 预约会议基础管理族（创建/修改/取消/获取成员会议 ID 列表 4 端点官方对三类应用开放一致，
/// 收敛声明于公共父接口；获取会议详情为自建/第三方差异端点——同路由两分支子接口各自声明，
/// 服务商代开发章节未开放该端点、其子接口为零端点空标记）；
/// 会议统计管理族（获取会议发起记录官方仅自建应用开放——第三方/代开发章节未提供会议统计管理文档页，
/// 零端点父接口 + 唯一自建子接口承载端点）；
/// 预约会议高级管理族（19 端点官方仅自建应用开放——第三方应用开发与服务商代开发章节均无对应 API，
/// 零端点父接口 + 唯一自建子接口承载端点；创建/修改/取消预约会议、获取会议详情、获取成员会议 ID 列表
/// 5 个高级管理文档页与基础管理族同路由，不重复声明端点、仅将请求/响应 DTO 扩展为高级文档参数超集）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 官方反直觉点（勿「顺手修正」）：会议域 25 条路由官方全部即 POST（含仅查询语义的 meeting/get_info /
/// meeting/get_user_meetingid / meeting/statistics/get_start_list / meeting/get_invitees /
/// meeting/get_customer_short_url / meeting/get_realtime_attendee_list / meeting/get_attendee_list /
/// meeting/waitingroom/* / meeting/check_device_in_meeting / meeting/get_guests / meeting/get_quality /
/// meeting/enroll/*）；会议 ID 官方字段名作 <c>meetingid</c>（无下划线）、列表作 <c>meetingid_list</c>；
/// 获取成员会议 ID 列表用 cursor+limit 翻页（cursor 初次调用可填 "0"）；
/// 会议统计管理路由挂 <c>/cgi-bin/meeting/statistics/</c> 段；高级管理报名配置与等候室路由挂
/// <c>/cgi-bin/meeting/enroll/</c> 与 <c>/cgi-bin/meeting/waitingroom/</c> 段；
/// 获取实时会中成员列表官方请求示例将分页游标误写为 <c>cursort</c>、参数表为 <c>cursor</c>，以参数表为准；
/// 创建预约会议响应 meetingid 可用于「进入会议」接口（小程序/JS-SDK）；
/// 创建/修改预约会议请求与获取会议详情响应的 settings/reminders/invitees
/// 三嵌套对象官方参数表高度同构，本 SDK 以共用结构承载；
/// <c>remind_before</c> 为秒数数组（仅支持 0/300/900/3600/86400），非单值；
/// 获取受邀成员列表的 <c>invitees</c> 为 <see cref="MeetingInvitee"/> 对象数组，与创建/修改请求的
/// <see cref="MeetingInvitees"/>（userid 字符串数组包裹对象）同名字段两种形态并存；
/// 删除会议报名信息请求的 <c>enroll_id_list</c> 为对象数组（<see cref="MeetingEnrollIdRef"/>），
/// 而审批会议报名信息请求的同名字段为字符串数组。
/// </para>
/// </remarks>
public class WechatMeetingContractGuards
{
    private const string MeetingRegistryGroupName = "Meeting";

    // ------------------------------------------------------------------
    // 路由表：25 条官方路由（get_info 同路由两分支；高级管理文档页与基础管理族 5 条同路由不重复建端点），
    // 全部 POST（勿「顺手统一」为 GET）。
    // ------------------------------------------------------------------

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingBaseRoutes =
        {
            // 创建预约会议（自建 99104 / 第三方 93706 / 代开发 97454；三类公共收敛父接口；官方即 POST）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.CreateMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/create"),
            // 修改预约会议（自建 99047 / 第三方 93710 / 代开发 97455；三类公共收敛父接口；
            // meeting_start 与 meeting_duration 须成对修改）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.UpdateMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/update"),
            // 取消预约会议（自建 99048 / 第三方 93709 / 代开发 97456；三类公共收敛父接口；仅预约状态可取消）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.CancelMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/cancel"),
            // 获取成员会议 ID 列表（自建 99050 / 第三方 93707 / 代开发 97457；三类公共收敛父接口）。
            (typeof(IWechatWorkMeetingService),
                nameof(IWechatWorkMeetingService.GetUserMeetingIdListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_user_meetingid"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingInfoRoutes =
        {
            // 获取会议详情（自建 99049 / 第三方 93708；自建/第三方差异端点——同路由两分支子接口各自声明；
            // 服务商代开发章节未开放该端点；官方即 POST）。
            (typeof(IWechatWorkInternalMeetingService),
                nameof(IWechatWorkInternalMeetingService.GetMeetingInfoAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_info"),
            (typeof(IWechatWorkThirdPartyMeetingService),
                nameof(IWechatWorkThirdPartyMeetingService.GetMeetingInfoAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_info"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingStatisticsRoutes =
        {
            // 获取会议发起记录（自建 99651；官方仅自建应用开放；官方即 POST）。
            (typeof(IWechatWorkInternalMeetingStatisticsService),
                nameof(IWechatWorkInternalMeetingStatisticsService.GetMeetingStartListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/statistics/get_start_list"),
        };

    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[]
        MeetingAdvancedRoutes =
        {
            // 获取会议受邀成员列表（自建 98160；官方仅自建应用开放；官方即 POST）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetInviteesAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_invitees"),
            // 更新会议受邀成员列表（自建 98162；最多 2000 名受邀成员，管理员必须在受邀成员列表中）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.SetInviteesAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/set_invitees"),
            // 创建用户专属参会链接（自建 98818；不支持网络研讨会；customer_data 需 Base64 编码且 ≤256 字节）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.CreateCustomerShortUrlAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/create_customer_short_url"),
            // 获取用户专属参会链接（自建 98819；不支持个人会议号会议、网络研讨会）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetCustomerShortUrlAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_customer_short_url"),
            // 获取实时会中成员列表（自建 98157；官方请求示例 cursort 为拼写陷阱，参数表为 cursor）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetRealtimeAttendeeListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_realtime_attendee_list"),
            // 获取已参会成员列表（自建 98156；时间区间 ≤31 天，时间跨度最大 90 天）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetAttendeeListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_attendee_list"),
            // 获取实时等候室成员列表（自建 98163；需开启等候室且会议进行中；路由挂 waitingroom 段）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetWaitingRoomCurrentUserListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/waitingroom/get_current_user_list"),
            // 获取等候室成员记录（自建 98164；会前/会中/会后均可获取）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetWaitingRoomUserListAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/waitingroom/get_user_list"),
            // 获取成员设备是否入会（自建 98165）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.CheckDeviceInMeetingAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/check_device_in_meeting"),
            // 获取会议嘉宾列表（自建 99039）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetGuestsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_guests"),
            // 更新会议嘉宾列表（自建 99040）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.SetGuestsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/set_guests"),
            // 获取会议健康度（自建 98821；已结束会议；start_time 查询区间为过去 7 天到现在）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetQualityAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/get_quality"),
            // 修改会议报名配置（自建 98797；需会议已开启报名；路由挂 enroll 段）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.SetEnrollConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/set_config"),
            // 获取会议报名配置（自建 98800；未开启报名返回错误）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.GetEnrollConfigAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/get_config"),
            // 获取会议成员报名 ID（自建 98794；tmp_openid_list 单次最多 500 条）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.QueryEnrollIdsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/query_by_tmp_openid"),
            // 获取会议报名信息（自建 98810）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.ListEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/list"),
            // 审批会议报名信息（自建 98807；enroll_id_list 为字符串数组）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.ApproveEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/approve"),
            // 导入会议报名信息（自建 98816）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.ImportEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/import"),
            // 删除会议报名信息（自建 98817；enroll_id_list 为对象数组）。
            (typeof(IWechatWorkInternalMeetingAdvancedService),
                nameof(IWechatWorkInternalMeetingAdvancedService.DeleteEnrollsAsync),
                typeof(PostAttribute), "/cgi-bin/meeting/enroll/delete"),
        };

    // ------------------------------------------------------------------
    // MT1：全部端点路由与官方契约一致（26 条路由表项去重后 25 条官方路由，
    // get_info 同路由两分支；基础管理族 4 端点与高级管理族 5 个文档页同路由不重复建端点）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingEndpoints_ShouldMatchOfficialRoutes()
    {
        // 预约会议基础管理族公共面：4 端点收敛父接口。
        MeetingBaseRoutes.Should().HaveCount(4, "预约会议基础管理族公共面 = 创建 + 修改 + 取消 + 获取成员会议 ID 列表");
        MeetingBaseRoutes.Select(r => r.Route).Should().OnlyContain(
            r => r.StartsWith("/cgi-bin/meeting/", StringComparison.Ordinal), "预约会议基础管理族路由位于 /cgi-bin/meeting/ 段");

        // 获取会议详情：自建/第三方差异端点同路由两分支，代开发不承载。
        MeetingInfoRoutes.Should().HaveCount(2, "获取会议详情为自建/第三方差异端点（同路由两分支）");
        MeetingInfoRoutes.Select(r => r.Interface).Should().BeEquivalentTo(new[]
        {
            typeof(IWechatWorkInternalMeetingService),
            typeof(IWechatWorkThirdPartyMeetingService),
        }, "获取会议详情仅自建应用与第三方应用开放（代开发章节未提供该端点）");

        // 会议统计管理族：路由挂 statistics 段（官方原文如此）。
        MeetingStatisticsRoutes.Should().HaveCount(1, "会议统计管理族 = 获取会议发起记录单端点");
        MeetingStatisticsRoutes.Single().Route.Should().StartWith(
            "/cgi-bin/meeting/statistics/", "会议统计管理路由挂 /cgi-bin/meeting/statistics/ 段（get_start_list）");

        // 预约会议高级管理族：19 端点全部挂于唯一自建子接口。
        MeetingAdvancedRoutes.Should().HaveCount(19, "预约会议高级管理族 = 受邀成员 2 + 专属链接 2 + 会中/已参会成员 2 + 等候室 2 + 设备入会 1 + 嘉宾 2 + 健康度 1 + 报名 7 端点");
        MeetingAdvancedRoutes.Select(r => r.Interface).Should().OnlyContain(
            i => i == typeof(IWechatWorkInternalMeetingAdvancedService),
            "预约会议高级管理族官方仅自建应用开放（第三方/代开发章节均无对应 API）");
        MeetingAdvancedRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/enroll/", StringComparison.Ordinal))
            .Should().HaveCount(7, "报名配置与报名信息 7 端点路由挂 /cgi-bin/meeting/enroll/ 段");
        MeetingAdvancedRoutes.Where(r => r.Route.StartsWith("/cgi-bin/meeting/waitingroom/", StringComparison.Ordinal))
            .Should().HaveCount(2, "等候室 2 端点路由挂 /cgi-bin/meeting/waitingroom/ 段");

        AssertRoutes(MeetingBaseRoutes);
        AssertRoutes(MeetingInfoRoutes);
        AssertRoutes(MeetingStatisticsRoutes);
        AssertRoutes(MeetingAdvancedRoutes);

        // 全部官方路由去重清单锁定（get_info 两分支去重后 25 条；基础管理族与高级管理族的
        // create/update/cancel/get_info/get_user_meetingid 5 条同路由仅计一次）。
        var allRoutes = MeetingBaseRoutes.Concat(MeetingInfoRoutes).Concat(MeetingStatisticsRoutes)
            .Concat(MeetingAdvancedRoutes)
            .Select(r => r.Route).Distinct().ToList();
        allRoutes.Should().BeEquivalentTo(new[]
        {
            // 预约会议基础管理族（5 条；与高级管理族 5 个文档页同路由，不重复建端点）。
            "/cgi-bin/meeting/create",
            "/cgi-bin/meeting/update",
            "/cgi-bin/meeting/cancel",
            "/cgi-bin/meeting/get_user_meetingid",
            "/cgi-bin/meeting/get_info",
            // 会议统计管理族。
            "/cgi-bin/meeting/statistics/get_start_list",
            // 预约会议高级管理族（19 条）。
            "/cgi-bin/meeting/get_invitees",
            "/cgi-bin/meeting/set_invitees",
            "/cgi-bin/meeting/create_customer_short_url",
            "/cgi-bin/meeting/get_customer_short_url",
            "/cgi-bin/meeting/get_realtime_attendee_list",
            "/cgi-bin/meeting/get_attendee_list",
            "/cgi-bin/meeting/waitingroom/get_current_user_list",
            "/cgi-bin/meeting/waitingroom/get_user_list",
            "/cgi-bin/meeting/check_device_in_meeting",
            "/cgi-bin/meeting/get_guests",
            "/cgi-bin/meeting/set_guests",
            "/cgi-bin/meeting/get_quality",
            "/cgi-bin/meeting/enroll/set_config",
            "/cgi-bin/meeting/enroll/get_config",
            "/cgi-bin/meeting/enroll/query_by_tmp_openid",
            "/cgi-bin/meeting/enroll/list",
            "/cgi-bin/meeting/enroll/approve",
            "/cgi-bin/meeting/enroll/import",
            "/cgi-bin/meeting/enroll/delete",
        }, "会议域全部官方路由须与官方文档一一对应");
        allRoutes.Should().HaveCount(25, "会议域共 25 条官方路由（get_info 同路由两分支去重；高级管理文档页与基础管理族 5 条同路由不重复计入）");

        // 取消预约会议、更新受邀成员列表、更新嘉宾列表无业务负载：响应直接用 WechatWorkResponse，不得新建空响应 DTO。
        typeof(IWechatWorkMeetingService)
            .GetMethod(nameof(IWechatWorkMeetingService.CancelMeetingAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "取消预约会议仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingAdvancedService)
            .GetMethod(nameof(IWechatWorkInternalMeetingAdvancedService.SetInviteesAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "更新会议受邀成员列表仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
        typeof(IWechatWorkInternalMeetingAdvancedService)
            .GetMethod(nameof(IWechatWorkInternalMeetingAdvancedService.SetGuestsAsync), BindingFlags.Public | BindingFlags.Instance)!
            .ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                "更新会议嘉宾列表仅返回 errcode/errmsg，响应类型必须为 WechatWorkResponse");
    }

    // ------------------------------------------------------------------
    // MT2：接口层级与生成器注册形态（三族继承链、父/子端点数与开放面收敛）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces()
    {
        // 预约会议基础管理族：公共父 4 端点 + 自建/第三方各 1 差异端点（获取会议详情）+ 代开发零端点空标记。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingService),
            parentImplementation: "WechatWorkMeetingService",
            parentDeclaredEndpointCount: 4,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingService), 1),
                (typeof(IWechatWorkProviderMeetingService), 0),
                (typeof(IWechatWorkThirdPartyMeetingService), 1),
            });

        // 会议统计管理族：官方仅自建开放，父接口零端点 + 唯一自建子接口承载端点
        //（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingStatisticsService),
            parentImplementation: "WechatWorkMeetingStatisticsService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingStatisticsService), 1),
            });

        // 预约会议高级管理族：官方仅自建应用开放（第三方应用开发与服务商代开发章节均无对应 API），
        // 父接口零端点 + 唯一自建子接口承载全部 19 端点（继承链上不得出现代开发/第三方子接口）。
        AssertFamily(
            parent: typeof(IWechatWorkMeetingAdvancedService),
            parentImplementation: "WechatWorkMeetingAdvancedService",
            parentDeclaredEndpointCount: 0,
            new[]
            {
                (typeof(IWechatWorkInternalMeetingAdvancedService), 19),
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
            childApi!.RegistryGroupName.Should().Be(MeetingRegistryGroupName,
                $"{child.Name} 必须挂 {MeetingRegistryGroupName} 注册组（Meeting 模块共用 Add{MeetingRegistryGroupName}WebApiHttpClient()）");
            childApi.InheritedFrom.Should().Be(parentImplementation,
                $"{child.Name} 必须继承父接口生成实现类 {parentImplementation}");
            child.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().HaveCount(endpointCount, $"{child.Name} 承载官方开放面差异端点数");
        }
    }

    // ------------------------------------------------------------------
    // MT3：令牌绑定——会议域 8 个接口统一 AccessToken 路由键 + Query 注入
    //（归属域键由 WechatTokenOwnerContractGuards 全局锁定，此处不重复）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var accessTokenInterfaces = new[]
        {
            typeof(IWechatWorkMeetingService),
            typeof(IWechatWorkInternalMeetingService),
            typeof(IWechatWorkProviderMeetingService),
            typeof(IWechatWorkThirdPartyMeetingService),
            typeof(IWechatWorkMeetingStatisticsService),
            typeof(IWechatWorkInternalMeetingStatisticsService),
            typeof(IWechatWorkMeetingAdvancedService),
            typeof(IWechatWorkInternalMeetingAdvancedService),
        };

        accessTokenInterfaces.Should().HaveCount(8, "会议域三族 = 预约会议基础管理族 4 接口 + 会议统计管理族 2 接口 + 预约会议高级管理族 2 接口");

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
    // MT4：请求/响应 DTO 全量登记进 AOT JSON 上下文（SerializerClassName 统一 Meeting）。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingDataModels_ShouldBeRegisteredInJsonContext()
    {
        var meetingContext = MeetingJsonContext.Default;

        var domainTypes = typeof(CreateMeetingRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested && t.Namespace == "Mud.Wechat.Work.DataModels.Meeting"
                        && !typeof(System.Text.Json.Serialization.JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        // 全量守卫：命名空间下所有顶层 DTO 均须登记进 MeetingJsonContext 且 SerializerClassName 统一为 Meeting
        //（生成物 MeetingJsonContext 自身亦落同命名空间，按 JsonSerializerContext 派生类型排除）。
        domainTypes.Should().HaveCount(75,
            "会议模块契约面类型数漂移须先核对官方文档再同批调整本守卫（预约会议基础管理族 17：创建 2 + 修改 2 + 取消 1 + 获取详情 2 + 成员会议 ID 列表 2 + 共用嵌套 8；会议统计管理族 3；预约会议高级管理族 55：端点级请求/响应 36 + 嵌套对象 19）");

        foreach (var type in domainTypes)
        {
            meetingContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于会议域命名空间，必须登记进 MeetingJsonContext（AOT 源生成）");

            var serializable = type.GetCustomAttribute<HttpJsonSerializableAttribute>();
            serializable.Should().NotBeNull($"{type.Name} 必须标注 [HttpJsonSerializable]");
            serializable!.SerializerClassName.Should().Be("Meeting",
                $"{type.Name} 的 SerializerClassName 必须为会议域段 Meeting");
        }

        // 端点级请求/响应 DTO 落位抽查（关键端点契约面清单）。
        var endpointContractTypes = new Type[]
        {
            // 预约会议基础管理族。
            typeof(CreateMeetingRequest), typeof(CreateMeetingResponse),
            typeof(UpdateMeetingRequest), typeof(UpdateMeetingResponse),
            typeof(CancelMeetingRequest),
            typeof(GetMeetingInfoRequest), typeof(GetMeetingInfoResponse),
            typeof(GetUserMeetingIdListRequest), typeof(GetUserMeetingIdListResponse),
            // 共用嵌套对象。
            typeof(MeetingInvitees), typeof(MeetingSettings), typeof(MeetingReminders),
            typeof(MeetingHosts), typeof(MeetingRingUsers),
            typeof(MeetingAttendees), typeof(MeetingAttendeeMember), typeof(MeetingTmpExternalUser),
            // 会议统计管理族。
            typeof(GetMeetingStartListRequest), typeof(GetMeetingStartListResponse), typeof(MeetingStartRecord),
            // 预约会议高级管理族（端点级请求/响应 + 嵌套对象全清单）。
            typeof(GetMeetingInviteesRequest), typeof(GetMeetingInviteesResponse), typeof(MeetingInvitee),
            typeof(SetMeetingInviteesRequest),
            typeof(CreateMeetingCustomerShortUrlRequest), typeof(CreateMeetingCustomerShortUrlResponse),
            typeof(GetMeetingCustomerShortUrlRequest), typeof(GetMeetingCustomerShortUrlResponse),
            typeof(MeetingCustomerShortUrl),
            typeof(GetMeetingRealtimeAttendeeListRequest), typeof(GetMeetingRealtimeAttendeeListResponse),
            typeof(MeetingRealtimeAttendee),
            typeof(GetMeetingAttendeeListRequest), typeof(GetMeetingAttendeeListResponse),
            typeof(MeetingAttendedAttendee),
            typeof(GetMeetingWaitingRoomCurrentUserListRequest), typeof(GetMeetingWaitingRoomCurrentUserListResponse),
            typeof(MeetingWaitingRoomCurrentUser),
            typeof(GetMeetingWaitingRoomUserListRequest), typeof(GetMeetingWaitingRoomUserListResponse),
            typeof(MeetingWaitingRoomUserRecord),
            typeof(CheckMeetingDeviceInMeetingRequest), typeof(CheckMeetingDeviceInMeetingResponse),
            typeof(MeetingDeviceCheckResult),
            typeof(GetMeetingGuestsRequest), typeof(GetMeetingGuestsResponse), typeof(MeetingGuest),
            typeof(SetMeetingGuestsRequest),
            typeof(GetMeetingQualityRequest), typeof(GetMeetingQualityResponse), typeof(MeetingQualityAttendee),
            typeof(SetMeetingEnrollConfigRequest), typeof(SetMeetingEnrollConfigResponse),
            typeof(GetMeetingEnrollConfigRequest), typeof(GetMeetingEnrollConfigResponse),
            typeof(MeetingEnrollQuestion), typeof(MeetingEnrollQuestionOption),
            typeof(QueryMeetingEnrollIdsRequest), typeof(QueryMeetingEnrollIdsResponse), typeof(MeetingEnrollId),
            typeof(ListMeetingEnrollsRequest), typeof(ListMeetingEnrollsResponse),
            typeof(MeetingEnrollInfo), typeof(MeetingEnrollAnswer),
            typeof(ApproveMeetingEnrollsRequest), typeof(ApproveMeetingEnrollsResponse),
            typeof(ImportMeetingEnrollsRequest), typeof(ImportMeetingEnrollsResponse),
            typeof(MeetingEnrollImportItem), typeof(MeetingEnrollImportResult),
            typeof(DeleteMeetingEnrollsRequest), typeof(DeleteMeetingEnrollsResponse), typeof(MeetingEnrollIdRef),
            typeof(MeetingSubMeeting), typeof(MeetingSubRepeatInfo),
        };
        domainTypes.Should().Contain(endpointContractTypes, "端点级请求/响应 DTO 必须落位于会议域命名空间");
    }

    // ------------------------------------------------------------------
    // MT5：官方契约陷阱锁定——共用嵌套结构、meetingid 拼写形态与翻页/数组形态。
    // ------------------------------------------------------------------

    [Fact]
    public void MeetingDataModels_ShouldLockOfficialContractTraps()
    {
        // 创建/修改预约会议请求与获取会议详情响应的 settings/reminders/invitees 共用同一结构
        //（官方参数表高度同构；对齐 Schedule calendar 扁平结构先例）。
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Settings))!
            .PropertyType.Should().Be(typeof(MeetingSettings), "创建预约会议 settings 与修改/详情共用同一配置结构");
        typeof(UpdateMeetingRequest).GetProperty(nameof(UpdateMeetingRequest.Settings))!
            .PropertyType.Should().Be(typeof(MeetingSettings), "修改预约会议 settings 与创建/详情共用同一配置结构");
        typeof(GetMeetingInfoResponse).GetProperty(nameof(GetMeetingInfoResponse.Settings))!
            .PropertyType.Should().Be(typeof(MeetingSettings), "获取会议详情响应 settings 与创建/修改共用同一配置结构");
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Reminders))!
            .PropertyType.Should().Be(typeof(MeetingReminders), "创建预约会议 reminders 与修改/详情共用同一重复配置结构");
        typeof(GetMeetingInfoResponse).GetProperty(nameof(GetMeetingInfoResponse.Reminders))!
            .PropertyType.Should().Be(typeof(MeetingReminders), "获取会议详情响应 reminders 与创建/修改共用同一重复配置结构");
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Invitees))!
            .PropertyType.Should().Be(typeof(MeetingInvitees), "创建预约会议 invitees 与修改预约会议共用同一成员结构");
        typeof(UpdateMeetingRequest).GetProperty(nameof(UpdateMeetingRequest.Invitees))!
            .PropertyType.Should().Be(typeof(MeetingInvitees), "修改预约会议 invitees 与创建预约会议共用同一成员结构");

        // 会议 ID 官方字段名作 meetingid（无下划线），列表作 meetingid_list——照抄勿「顺手修正」。
        JsonNameShouldBe(typeof(UpdateMeetingRequest), nameof(UpdateMeetingRequest.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(CancelMeetingRequest), nameof(CancelMeetingRequest.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(GetMeetingInfoRequest), nameof(GetMeetingInfoRequest.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.Meetingid), "meetingid");
        JsonNameShouldBe(typeof(GetUserMeetingIdListResponse), nameof(GetUserMeetingIdListResponse.MeetingidList), "meetingid_list");

        // 翻页游标字段名照抄官方原文。
        JsonNameShouldBe(typeof(GetUserMeetingIdListResponse), nameof(GetUserMeetingIdListResponse.NextCursor), "next_cursor");
        JsonNameShouldBe(typeof(GetMeetingStartListResponse), nameof(GetMeetingStartListResponse.NextCursor), "next_cursor");

        // remind_before 为秒数数组（仅支持 0/300/900/3600/86400），非单值。
        typeof(MeetingReminders).GetProperty(nameof(MeetingReminders.RemindBefore))!
            .PropertyType.Should().Be(typeof(List<int>), "remind_before 官方为提前提醒秒数数组");

        // 创建/修改预约会议响应均携带 excess_users（购买会议专业版企业部分参会人无有效会议账号时返回）。
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.ExcessUsers), "excess_users");
        JsonNameShouldBe(typeof(UpdateMeetingResponse), nameof(UpdateMeetingResponse.ExcessUsers), "excess_users");

        // 获取会议详情响应的成员字段名照抄官方原文。
        JsonNameShouldBe(typeof(MeetingAttendees), nameof(MeetingAttendees.TmpExternalUser), "tmp_external_user");
        JsonNameShouldBe(typeof(MeetingTmpExternalUser), nameof(MeetingTmpExternalUser.TmpExternalUserid), "tmp_external_userid");
        JsonNameShouldBe(typeof(MeetingAttendeeMember), nameof(MeetingAttendeeMember.TotalJoinCount), "total_join_count");

        // 会议统计：type 决定记录成败口径、has_more 为布尔翻页标记。
        JsonNameShouldBe(typeof(GetMeetingStartListRequest), nameof(GetMeetingStartListRequest.BeginTime), "begin_time");
        JsonNameShouldBe(typeof(GetMeetingStartListRequest), nameof(GetMeetingStartListRequest.EndTime), "end_time");
        JsonNameShouldBe(typeof(GetMeetingStartListResponse), nameof(GetMeetingStartListResponse.HasMore), "has_more");
        JsonNameShouldBe(typeof(GetMeetingStartListResponse), nameof(GetMeetingStartListResponse.MeetingList), "meeting_list");
        JsonNameShouldBe(typeof(MeetingStartRecord), nameof(MeetingStartRecord.StartTime), "start_time");

        // ---- 预约会议高级管理族契约陷阱 ----

        // 高级管理文档页与基础管理族 5 条同路由：请求/响应 DTO 扩展为高级文档参数超集（同结构承载，不另建 DTO）。
        typeof(CreateMeetingRequest).GetProperty(nameof(CreateMeetingRequest.Guests))!
            .PropertyType.Should().Be(typeof(List<MeetingGuest>), "创建预约会议 guests 仅高级管理文档页声明，与嘉宾列表端点共用 MeetingGuest");
        typeof(GetMeetingInfoResponse).GetProperty(nameof(GetMeetingInfoResponse.Guests))!
            .PropertyType.Should().Be(typeof(List<MeetingGuest>), "获取会议详情 guests 与嘉宾列表端点共用 MeetingGuest");
        typeof(CancelMeetingRequest).GetProperty(nameof(CancelMeetingRequest.SubMeetingid))!
            .PropertyType.Should().Be(typeof(string), "取消预约会议 sub_meetingid 仅高级管理文档页声明（不传则取消整个周期系列）");
        typeof(GetMeetingInfoRequest).GetProperty(nameof(GetMeetingInfoRequest.MeetingCode))!
            .PropertyType.Should().Be(typeof(string), "获取会议详情高级文档口径为 meetingid 与 meeting_code 必须填一个");
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.MeetingCode), "meeting_code");
        JsonNameShouldBe(typeof(CreateMeetingResponse), nameof(CreateMeetingResponse.MeetingLink), "meeting_link");

        // MeetingSettings 超集字段（报名/主持人密钥/录制/同声传译/联席主持人等）沿用 Meeting 结构，勿拆分类型。
        typeof(MeetingSettings).GetProperty(nameof(MeetingSettings.EnableEnroll))!
            .PropertyType.Should().Be(typeof(bool?), "enable_enroll 为布尔可空（官方未赋值字段不输出）");
        typeof(MeetingSettings).GetProperty(nameof(MeetingSettings.HostKey))!
            .PropertyType.Should().Be(typeof(string), "host_key 为 6 位数字字符串");
        typeof(MeetingSettings).GetProperty(nameof(MeetingSettings.CoHosts))!
            .PropertyType.Should().Be(typeof(MeetingHosts), "co_hosts 与 hosts 共用 userid 数组包裹结构");
        JsonNameShouldBe(typeof(MeetingSettings), nameof(MeetingSettings.AllowUnmuteSelf), "allow_unmute_self");
        JsonNameShouldBe(typeof(MeetingSettings), nameof(MeetingSettings.AutoRecordType), "auto_record_type");

        // MeetingReminders 自定义重复字段：repeat_until_type 官方创建文档页参数表作 uint32[]，示例为单值，按单值承载。
        typeof(MeetingReminders).GetProperty(nameof(MeetingReminders.RepeatUntilType))!
            .PropertyType.Should().Be(typeof(int?), "repeat_until_type 官方示例为单值整数（参数表 uint32[] 为文档笔误）");
        typeof(MeetingReminders).GetProperty(nameof(MeetingReminders.RepeatDayOfWeek))!
            .PropertyType.Should().Be(typeof(List<int>), "repeat_day_of_week 官方为周几数组（1~7）");

        // 受邀成员两种形态并存：创建/修改请求为 userid 数组包裹对象，受邀成员列表端点为对象数组。
        typeof(MeetingInvitees).GetProperty(nameof(MeetingInvitees.Userid))!
            .PropertyType.Should().Be(typeof(List<string>), "创建/修改预约会议 invitees.userid 为字符串数组");
        typeof(MeetingInvitee).GetProperty(nameof(MeetingInvitee.Userid))!
            .PropertyType.Should().Be(typeof(string), "受邀成员列表端点 invitees 元素为 {userid} 对象");

        // 受邀成员列表端点字段名照抄官方原文。
        JsonNameShouldBe(typeof(GetMeetingInviteesResponse), nameof(GetMeetingInviteesResponse.HasMore), "has_more");
        JsonNameShouldBe(typeof(GetMeetingInviteesResponse), nameof(GetMeetingInviteesResponse.Invitees), "invitees");
        JsonNameShouldBe(typeof(SetMeetingInviteesRequest), nameof(SetMeetingInviteesRequest.Invitees), "invitees");

        // 实时会中成员列表：官方请求示例 cursort 为拼写陷阱，cursor 以参数表为准。
        JsonNameShouldBe(typeof(GetMeetingRealtimeAttendeeListRequest), nameof(GetMeetingRealtimeAttendeeListRequest.Cursor), "cursor");
        JsonNameShouldBe(typeof(GetMeetingRealtimeAttendeeListRequest), nameof(GetMeetingRealtimeAttendeeListRequest.SubMeetingid), "sub_meetingid");
        JsonNameShouldBe(typeof(GetMeetingRealtimeAttendeeListResponse), nameof(GetMeetingRealtimeAttendeeListResponse.Attendees), "attendees");
        JsonNameShouldBe(typeof(MeetingRealtimeAttendee), nameof(MeetingRealtimeAttendee.TmpOpenid), "tmp_openid");
        JsonNameShouldBe(typeof(MeetingRealtimeAttendee), nameof(MeetingRealtimeAttendee.ScreenSharedState), "screen_shared_state");

        // 已参会成员列表：支持网络研讨会角色与专属链接 customer_data。
        JsonNameShouldBe(typeof(MeetingAttendedAttendee), nameof(MeetingAttendedAttendee.WebinarRole), "webinar_role");
        JsonNameShouldBe(typeof(MeetingAttendedAttendee), nameof(MeetingAttendedAttendee.CustomerData), "customer_data");

        // 等候室两列表端点 user_list 字段名一致、记录对象不同构（实时含 customer_data，记录含毫秒级进出时间）。
        JsonNameShouldBe(typeof(GetMeetingWaitingRoomCurrentUserListResponse), nameof(GetMeetingWaitingRoomCurrentUserListResponse.UserList), "user_list");
        JsonNameShouldBe(typeof(GetMeetingWaitingRoomUserListResponse), nameof(GetMeetingWaitingRoomUserListResponse.UserList), "user_list");
        JsonNameShouldBe(typeof(MeetingWaitingRoomUserRecord), nameof(MeetingWaitingRoomUserRecord.JoinTime), "join_time");
        JsonNameShouldBe(typeof(MeetingWaitingRoomUserRecord), nameof(MeetingWaitingRoomUserRecord.QuitTime), "quit_time");

        // 专属参会链接与嘉宾列表字段名照抄官方原文。
        JsonNameShouldBe(typeof(CreateMeetingCustomerShortUrlResponse), nameof(CreateMeetingCustomerShortUrlResponse.MeetingShortUrlCustomerData), "meeting_short_url_customer_data");
        JsonNameShouldBe(typeof(GetMeetingCustomerShortUrlResponse), nameof(GetMeetingCustomerShortUrlResponse.MeetingShortUrlCustomerDataList), "meeting_short_url_customer_data_list");
        JsonNameShouldBe(typeof(MeetingGuest), nameof(MeetingGuest.PhoneNumber), "phone_number");
        JsonNameShouldBe(typeof(MeetingGuest), nameof(MeetingGuest.GuestName), "guest_name");

        // 健康度：会议级与成员级同构字段（quality/audio_quality/video_quality/screen_share_quality/network_quality/problems）。
        JsonNameShouldBe(typeof(GetMeetingQualityRequest), nameof(GetMeetingQualityRequest.StartTime), "start_time");
        JsonNameShouldBe(typeof(GetMeetingQualityResponse), nameof(GetMeetingQualityResponse.ScreenShareQuality), "screen_share_quality");
        JsonNameShouldBe(typeof(GetMeetingQualityResponse), nameof(GetMeetingQualityResponse.Problems), "problems");
        JsonNameShouldBe(typeof(MeetingQualityAttendee), nameof(MeetingQualityAttendee.NetworkQuality), "network_quality");

        // 报名域：审批请求 enroll_id_list 为字符串数组、删除请求为对象数组（官方两种形态并存，勿统一）。
        typeof(ApproveMeetingEnrollsRequest).GetProperty(nameof(ApproveMeetingEnrollsRequest.EnrollIdList))!
            .PropertyType.Should().Be(typeof(List<string>), "审批会议报名信息 enroll_id_list 官方为字符串数组");
        typeof(DeleteMeetingEnrollsRequest).GetProperty(nameof(DeleteMeetingEnrollsRequest.EnrollIdList))!
            .PropertyType.Should().Be(typeof(List<MeetingEnrollIdRef>), "删除会议报名信息 enroll_id_list 官方为对象数组（{enroll_id}）");
        JsonNameShouldBe(typeof(SetMeetingEnrollConfigRequest), nameof(SetMeetingEnrollConfigRequest.ApproveType), "approve_type");
        JsonNameShouldBe(typeof(SetMeetingEnrollConfigRequest), nameof(SetMeetingEnrollConfigRequest.NoRegistrationNeededForStaff), "no_registration_needed_for_staff");
        JsonNameShouldBe(typeof(GetMeetingEnrollConfigResponse), nameof(GetMeetingEnrollConfigResponse.QuestionList), "question_list");
        JsonNameShouldBe(typeof(QueryMeetingEnrollIdsRequest), nameof(QueryMeetingEnrollIdsRequest.TmpOpenidList), "tmp_openid_list");
        JsonNameShouldBe(typeof(QueryMeetingEnrollIdsResponse), nameof(QueryMeetingEnrollIdsResponse.EnrollIdList), "enroll_id_list");
        JsonNameShouldBe(typeof(MeetingEnrollId), nameof(MeetingEnrollId.EnrollId), "enroll_id");
        JsonNameShouldBe(typeof(ListMeetingEnrollsResponse), nameof(ListMeetingEnrollsResponse.EnrollList), "enroll_list");
        JsonNameShouldBe(typeof(MeetingEnrollInfo), nameof(MeetingEnrollInfo.EnrollSourceType), "enroll_source_type");
        JsonNameShouldBe(typeof(MeetingEnrollInfo), nameof(MeetingEnrollInfo.EnrollCode), "enroll_code");
        JsonNameShouldBe(typeof(MeetingEnrollAnswer), nameof(MeetingEnrollAnswer.AnswerContent), "answer_content");
        JsonNameShouldBe(typeof(MeetingEnrollAnswer), nameof(MeetingEnrollAnswer.QuestionNum), "question_num");
        JsonNameShouldBe(typeof(ApproveMeetingEnrollsResponse), nameof(ApproveMeetingEnrollsResponse.HandledCount), "handled_count");
        JsonNameShouldBe(typeof(ImportMeetingEnrollsResponse), nameof(ImportMeetingEnrollsResponse.TotalCount), "total_count");
        JsonNameShouldBe(typeof(DeleteMeetingEnrollsResponse), nameof(DeleteMeetingEnrollsResponse.TotalCount), "total_count");

        // 获取会议详情高级文档页扩展字段：周期性子会议与分段信息字段名照抄官方原文（meetingid 无下划线）。
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.MeetingType), "meeting_type");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.HasVote), "has_vote");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.SubMeetings), "sub_meetings");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.HasMoreSubMeeting), "has_more_sub_meeting");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.RemainSubMeetings), "remain_sub_meetings");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.CurrentSubMeetingid), "current_sub_meetingid");
        JsonNameShouldBe(typeof(GetMeetingInfoResponse), nameof(GetMeetingInfoResponse.SubRepeatList), "sub_repeat_list");
        JsonNameShouldBe(typeof(MeetingSubMeeting), nameof(MeetingSubMeeting.SubMeetingid), "sub_meetingid");
        JsonNameShouldBe(typeof(MeetingSubMeeting), nameof(MeetingSubMeeting.RepeatId), "repeat_id");
        JsonNameShouldBe(typeof(MeetingSubRepeatInfo), nameof(MeetingSubRepeatInfo.RepeatId), "repeat_id");
        JsonNameShouldBe(typeof(MeetingSubRepeatInfo), nameof(MeetingSubRepeatInfo.RepeatUntilCount), "repeat_until_count");
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
