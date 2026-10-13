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
using Mud.Wechat.Work.DataModels.MeetingRoom;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 会议室模块（MeetingRoom 模块）契约守卫：路由表、接口层级、令牌绑定与字段名锁定。
/// <para>
/// 官方「会议室」文档树（分类 93617）只在<b>企业自建应用开发</b>根下存在页面
/// （93619 会议室管理 / 93620 会议室预定管理），第三方与服务商代开发两棵树均无对应页面
/// ⇒ 两族均为「零端点父接口 + 仅自建子接口承载端点」，继承链上不得出现第三方 / 代开发子接口。
/// </para>
/// <para>
/// 与「视频会议」域（<c>/cgi-bin/meeting/rooms/*</c> 的硬件会议室）分属官方两棵章节树：
/// 本域全部路由为 <c>/cgi-bin/oa/meetingroom/</c>，勿与 <c>/cgi-bin/meeting/</c> 混淆
/// （形态对齐公费电话域与紧急通知域 pstncc 路由族的分流先例）。
/// </para>
/// </summary>
public class WechatMeetingRoomContractGuards
{
    private const string MeetingRoomRegistryGroupName = "MeetingRoom";

    /// <summary>
    /// 会议室管理族官方路由表（4 端点，官方页 93619，全部为 POST）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] RoomRoutes =
    {
        (typeof(IWechatWorkInternalMeetingRoomService),
            nameof(IWechatWorkInternalMeetingRoomService.AddMeetingRoomAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/add"),
        (typeof(IWechatWorkInternalMeetingRoomService),
            nameof(IWechatWorkInternalMeetingRoomService.GetMeetingRoomListAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/list"),
        (typeof(IWechatWorkInternalMeetingRoomService),
            nameof(IWechatWorkInternalMeetingRoomService.EditMeetingRoomAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/edit"),
        (typeof(IWechatWorkInternalMeetingRoomService),
            nameof(IWechatWorkInternalMeetingRoomService.DeleteMeetingRoomAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/del"),
    };

    /// <summary>
    /// 会议室预定管理族官方路由表（6 端点，官方页 93620，全部为 POST；
    /// <c>bookinfo/get</c> 是官方原文的斜杠分层路由，勿「统一」为平级命名）。
    /// <para>
    /// 官方 93620（2024-06-24 正文）的锚点只有「查询会议室的预定信息 / 预定会议室 / 通过日程预定会议室 /
    /// 通过会议预定会议室 / 取消预定会议室 / 根据会议室预定ID查询预定详情」六节；
    /// 第三方 SDK 转述的 <c>get_booking_info_by_meeting_id</c> 不在该页与「会议室」树任何节点内
    /// ⇒ 已撤回并移入 RD1 零声明清单（追踪号 P2-QY-MEETINGROOM-BYMEETINGID-07）。
    /// </para>
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] BookingRoutes =
    {
        (typeof(IWechatWorkInternalMeetingRoomBookingService),
            nameof(IWechatWorkInternalMeetingRoomBookingService.GetMeetingRoomBookingInfoAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/get_booking_info"),
        (typeof(IWechatWorkInternalMeetingRoomBookingService),
            nameof(IWechatWorkInternalMeetingRoomBookingService.BookMeetingRoomAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/book"),
        (typeof(IWechatWorkInternalMeetingRoomBookingService),
            nameof(IWechatWorkInternalMeetingRoomBookingService.BookMeetingRoomByScheduleAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/book_by_schedule"),
        (typeof(IWechatWorkInternalMeetingRoomBookingService),
            nameof(IWechatWorkInternalMeetingRoomBookingService.BookMeetingRoomByMeetingAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/book_by_meeting"),
        (typeof(IWechatWorkInternalMeetingRoomBookingService),
            nameof(IWechatWorkInternalMeetingRoomBookingService.CancelMeetingRoomBookAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/cancel_book"),
        (typeof(IWechatWorkInternalMeetingRoomBookingService),
            nameof(IWechatWorkInternalMeetingRoomBookingService.GetMeetingRoomBookInfoAsync),
            typeof(PostAttribute), "/cgi-bin/oa/meetingroom/bookinfo/get"),
    };

    /// <summary>
    /// 契约守卫 MR1：会议室管理族端点路由必须与官方契约一致（4 端点全部官方即 POST）。
    /// </summary>
    [Fact]
    public void MeetingRoomRoomEndpoints_ShouldMatchOfficialRoutes()
    {
        RoomRoutes.Should().HaveCount(4, "会议室管理族官方页 93619 为 add / list / edit / del 四端点");

        foreach (var (iface, method, httpAttribute, route) in RoomRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在（端点须声明于自建子接口）");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方 list 无分页参数：请求体只有四个过滤项，不得混入预定侧字段。
        typeof(MeetingRoomListRequest)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .Should().BeEquivalentTo(new[]
            {
                nameof(MeetingRoomListRequest.City),
                nameof(MeetingRoomListRequest.Building),
                nameof(MeetingRoomListRequest.Floor),
                nameof(MeetingRoomListRequest.EquipmentIdList),
            }, "官方 93619 的查询会议室列表请求体只有 city / building / floor / equipment");
    }

    /// <summary>
    /// 契约守卫 MR2：会议室预定管理族端点路由必须与官方契约一致（6 端点全部官方即 POST）。
    /// </summary>
    [Fact]
    public void MeetingRoomBookingEndpoints_ShouldMatchOfficialRoutes()
    {
        BookingRoutes.Should().HaveCount(6,
            "会议室预定管理族官方页 93620 为一条查询 + 三种预定 + 取消预定 + 预定详情共六端点");

        foreach (var (iface, method, httpAttribute, route) in BookingRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在（端点须声明于自建子接口）");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明 [{httpAttribute.Name.Replace("Attribute", string.Empty)}] 路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }

        // 官方 93620 的「取消预定会议室」以 booking_id 定位（参数表原文：booking_id 是「会议室的预定id」），
        // 第三方 SDK 转述的 meeting_id 无官方出处，不得回填。
        typeof(MeetingRoomCancelBookRequest).GetProperty(nameof(MeetingRoomCancelBookRequest.BookingId))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("booking_id", "cancel_book 的定位键官方为 booking_id（会议室的预定 id）");
        typeof(MeetingRoomCancelBookRequest)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .Should().BeEquivalentTo(new[]
            {
                nameof(MeetingRoomCancelBookRequest.BookingId),
                nameof(MeetingRoomCancelBookRequest.KeepSchedule),
                nameof(MeetingRoomCancelBookRequest.CancelDate),
            }, "cancel_book 官方参数表即 booking_id / keep_schedule / cancel_date 三项");
        typeof(MeetingRoomBookInfoGetRequest).GetProperty(nameof(MeetingRoomBookInfoGetRequest.BookingId))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name
            .Should().Be("booking_id", "bookinfo/get 的定位键官方为 booking_id");
    }

    /// <summary>
    /// 契约守卫 MR3：仅单类应用开放域的接口层级——会议室文档树官方仅自建开放，
    /// 两族各为「零端点父接口 + 仅自建子接口承载端点」，继承链上不得出现第三方 / 代开发子接口。
    /// </summary>
    [Fact]
    public void MeetingRoomSingleAppTypeFamilies_ShouldHaveExactlyOneChildAndAbstractParent()
    {
        var families = new (Type Parent, Type Child, string Reason)[]
        {
            (typeof(IWechatWorkMeetingRoomService),
                typeof(IWechatWorkInternalMeetingRoomService),
                "会议室管理族官方仅自建开放（第三方与代开发文档树无对应页面）"),
            (typeof(IWechatWorkMeetingRoomBookingService),
                typeof(IWechatWorkInternalMeetingRoomBookingService),
                "会议室预定管理族官方仅自建开放（第三方与代开发文档树无对应页面）"),
        };

        foreach (var (parent, child, reason) in families)
        {
            var assignable = parent.Assembly.GetTypes()
                .Where(t => t.IsInterface && t != parent && parent.IsAssignableFrom(t))
                .ToList();

            assignable.Should().BeEquivalentTo(new[] { child },
                $"继承链上不得出现自建之外的子接口：{reason}");

            var parentApi = parent.GetCustomAttribute<HttpClientApiAttribute>();
            parentApi.Should().NotBeNull($"{parent.Name} 必须声明 [HttpClientApi]");
            parentApi!.IsAbstract.Should().BeTrue("公共父接口不参与 DI 注册，必须 IsAbstract = true");
            parentApi.RegistryGroupName.Should().BeNullOrEmpty("父接口不进入注册组（注册面由子接口承载）");
            parent.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().BeEmpty("零端点父接口：端点全部声明于唯一的自建子接口");

            var childApi = child.GetCustomAttribute<HttpClientApiAttribute>();
            childApi.Should().NotBeNull($"{child.Name} 必须声明 [HttpClientApi]");
            childApi!.RegistryGroupName.Should().Be(MeetingRoomRegistryGroupName,
                $"{child.Name} 必须挂 {MeetingRoomRegistryGroupName} 注册组");
            childApi.InheritedFrom.Should().Be(parent.Name.Substring(1),
                "生成器契约：子接口必须以 InheritedFrom 指向父接口实现类名");
        }
    }

    /// <summary>
    /// 契约守卫 MR4：令牌绑定——MeetingRoom 模块四个接口统一消费 AccessToken 路由键并以 Query 注入
    /// （官方契约 access_token；自建子接口必须声明 InternalAccessToken 归属域键，父接口不声明归属域）。
    /// </summary>
    [Fact]
    public void MeetingRoomTokenBinding_ShouldBeAccessTokenInjectedViaQuery()
    {
        var owningChildren = new[]
        {
            typeof(IWechatWorkInternalMeetingRoomService),
            typeof(IWechatWorkInternalMeetingRoomBookingService),
        };
        var abstractParents = new[]
        {
            typeof(IWechatWorkMeetingRoomService),
            typeof(IWechatWorkMeetingRoomBookingService),
        };

        foreach (var iface in owningChildren.Concat(abstractParents))
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 必须声明 [Token]");
            token!.TokenType.Should().Be(WechatTokenTypes.AccessToken,
                $"{iface.Name} 令牌路由键必须为 AccessToken");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query,
                $"{iface.Name} 官方契约强制 Query 注入（MUD005 已知接受风险）");
            token.Name.Should().Be("access_token", $"{iface.Name} Query 注入参数名必须为官方契约的 access_token");
        }

        foreach (var iface in owningChildren)
        {
            iface.GetCustomAttribute<TokenAttribute>()!.TokenManagerKey
                .Should().Be(WechatTokenManagerKeys.InternalAccessToken,
                    $"{iface.Name} 官方仅自建开放，必须声明自建应用级凭据归属域（错配由咽喉点 fail-fast）");
        }

        foreach (var iface in abstractParents)
        {
            // 反射陷阱：父接口未显式书写 TokenManagerKey 时读到的是特性构造参数默认值
            // （TokenTypes.AccessToken），故只能断言「不是任何真实归属域键」。
            var key = iface.GetCustomAttribute<TokenAttribute>()!.TokenManagerKey;
            key.Should().NotBe(WechatTokenManagerKeys.InternalAccessToken,
                $"{iface.Name} 公共父接口不得声明凭据归属域（调用方须按应用类型选择子接口）");
            key.Should().NotBe(WechatTokenManagerKeys.CorpAccessToken,
                $"{iface.Name} 公共父接口不得声明凭据归属域（调用方须按应用类型选择子接口）");
        }
    }

    /// <summary>
    /// 契约守卫 MR5：会议室模块的请求 / 响应 DTO 必须全量登记进 AOT JSON 上下文
    /// （两族 22 个契约面类型落 MeetingRoomJsonContext）。
    /// <para>
    /// 计数为 22 而非 24：官方 93620 无「按会议 ID 查询预定信息」小节，该端点的两个 DTO 已随端点一并撤回。
    /// </para>
    /// </summary>
    [Fact]
    public void MeetingRoomDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = MeetingRoomJsonContext.Default;

        var requiredTypes = new[]
        {
            // 会议室管理族（93619）。
            typeof(MeetingRoomAddRequest), typeof(MeetingRoomAddResponse),
            typeof(MeetingRoomEditRequest), typeof(MeetingRoomDeleteRequest),
            typeof(MeetingRoomListRequest), typeof(MeetingRoomListResponse),
            typeof(MeetingRoomInfo), typeof(MeetingRoomCoordinate), typeof(MeetingRoomRange),
            // 会议室预定管理族（93620）。
            typeof(GetMeetingRoomBookingInfoRequest), typeof(GetMeetingRoomBookingInfoResponse),
            typeof(MeetingRoomBooking), typeof(MeetingRoomSchedule),
            typeof(MeetingRoomBookRequest), typeof(MeetingRoomBookResponse),
            typeof(MeetingRoomBookByScheduleRequest), typeof(MeetingRoomBookByScheduleResponse),
            typeof(MeetingRoomBookByMeetingRequest), typeof(MeetingRoomBookByMeetingResponse),
            typeof(MeetingRoomCancelBookRequest),
            typeof(MeetingRoomBookInfoGetRequest), typeof(MeetingRoomBookInfoGetResponse),
        };

        requiredTypes.Should().HaveCount(22, "两族契约面类型全集（新增 DTO 须同批登记进本守卫与上下文）");

        foreach (var type in requiredTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 是会议室模块契约面类型，必须登记进 MeetingRoomJsonContext（AOT 源生成）");
        }
    }

    /// <summary>
    /// 契约守卫 MR6：官方字段名原文保留——三处官方反直觉点必须锁定
    /// （book_by_meeting 的会议 ID 官方拼写为 meetingid 无下划线；
    /// 排期容器名官方恒为单数 schedule 而在 get_booking_info 是数组、在 bookinfo/get 是对象；
    /// need_approval / master_booking_id / conflict_date / cancel_date / attendees 的下划线与复数形态）。
    /// </summary>
    [Fact]
    public void MeetingRoomOfficialFieldNames_ShouldBePreservedVerbatim()
    {
        var expected = new (Type Owner, string Property, string JsonName, string Reason)[]
        {
            (typeof(MeetingRoomBookByMeetingRequest),
                nameof(MeetingRoomBookByMeetingRequest.MeetingId),
                "meetingid", "官方 93620 的 book_by_meeting 请求字段即 meetingid（无下划线），不得驼峰化或补下划线"),
            (typeof(MeetingRoomBooking),
                nameof(MeetingRoomBooking.ScheduleList),
                "schedule", "官方 get_booking_info 的排期容器名单数为 schedule 而承载数组"),
            (typeof(MeetingRoomBookInfoGetResponse),
                nameof(MeetingRoomBookInfoGetResponse.Schedule),
                "schedule", "官方 bookinfo/get 的 schedule 为单对象形态并额外回传预定 ID"),
            (typeof(MeetingRoomSchedule),
                nameof(MeetingRoomSchedule.MasterBookingId),
                "master_booking_id", "官方 bookinfo/get 的排期以 master_booking_id 回传主预定 ID"),
            (typeof(MeetingRoomInfo),
                nameof(MeetingRoomInfo.NeedApproval),
                "need_approval", "官方查询会议室列表回传 need_approval，不得改写成 require_approval"),
            (typeof(MeetingRoomBookByScheduleResponse),
                nameof(MeetingRoomBookByScheduleResponse.ConflictDateList),
                "conflict_date", "官方冲突日期数组的字段名即单数 conflict_date"),
            (typeof(MeetingRoomCancelBookRequest),
                nameof(MeetingRoomCancelBookRequest.CancelDate),
                "cancel_date", "官方重复日程取消粒度字段名为 cancel_date（当天 0 点时间戳）"),
            (typeof(MeetingRoomBookRequest),
                nameof(MeetingRoomBookRequest.AttendeeUserIdList),
                "attendees", "官方参与人列表字段名为 attendees"),
            (typeof(MeetingRoomRange),
                nameof(MeetingRoomRange.DepartmentIdList),
                "department_list", "官方使用范围的部门列表字段名为 department_list"),
        };

        foreach (var (owner, property, jsonName, reason) in expected)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            prop!.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                .Should().Be(jsonName, reason);
        }

        // 官方无业务字段的三个端点不得另建响应 DTO（一律返回 WechatWorkResponse）。
        var noBodyEndpoints = new (Type Interface, string Method)[]
        {
            (typeof(IWechatWorkInternalMeetingRoomService), nameof(IWechatWorkInternalMeetingRoomService.EditMeetingRoomAsync)),
            (typeof(IWechatWorkInternalMeetingRoomService), nameof(IWechatWorkInternalMeetingRoomService.DeleteMeetingRoomAsync)),
            (typeof(IWechatWorkInternalMeetingRoomBookingService), nameof(IWechatWorkInternalMeetingRoomBookingService.CancelMeetingRoomBookAsync)),
        };

        foreach (var (iface, method) in noBodyEndpoints)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
            target!.ReturnType.Should().Be(typeof(Task<WechatWorkResponse>),
                $"{iface.Name}.{method} 官方成功响应仅有 errcode / errmsg，须直接返回 WechatWorkResponse");
        }
    }

    /// <summary>
    /// 契约守卫 MR7：官方以数字 0/1 承载的布尔语义字段一律以数值类型原样承载
    /// （need_approval 与 keep_schedule 的官方线格式是数字而非 true/false；
    /// 本仓不引入数值-布尔转换器，声明为 <c>bool</c> 会在读侧直接抛 JsonException、
    /// 在写侧发出与官方参数表不符的 JSON 字面量）。
    /// </summary>
    [Fact]
    public void MeetingRoomNumericalBooleanFields_ShouldBeCarriedAsNumbers()
    {
        var expected = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(MeetingRoomInfo), nameof(MeetingRoomInfo.NeedApproval), "need_approval"),
            (typeof(MeetingRoomCancelBookRequest), nameof(MeetingRoomCancelBookRequest.KeepSchedule), "keep_schedule"),
        };

        foreach (var (owner, property, jsonName) in expected)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            prop!.PropertyType.Should().Be(typeof(int?),
                $"{jsonName} 官方以数字 0/1 承载布尔语义，须以 int? 原样承载；改成 bool? 即破坏线格式");
        }
    }

    /// <summary>
    /// 契约守卫 MR8：请求侧「必填」以类型系统表达（B8 官方正文复核结论，2026-10-13，追踪号 P2-QY-B8-MEETINGROOM-REQUIRED-12）。
    /// <para>
    /// 93619 / 93620 两页参数表的「必须」列逐端点核对结果：add 的 name 与 capacity、edit 与 del 的 meetingroom_id、
    /// book 的 meetingroom_id / start_time / end_time / booker、book_by_schedule 的 schedule_id、
    /// book_by_meeting 的 meetingid、bookinfo/get 的 booking_id、cancel_book 的 booking_id 均为<b>必须=是</b>；
    /// 其余参数（含 list 的全部过滤项、get_booking_info 的全部参数、subject / attendees / keep_schedule / cancel_date）
    /// 均为<b>必须=否</b>。官方对必填参数的缺省行为是回错误码而非静默接受，故必填侧必须非可空、可选侧必须保持可空
    /// （非可空即强制每个请求都带该参数，与官方开放面不符）。
    /// </para>
    /// </summary>
    [Fact]
    public void MeetingRoomOfficiallyRequiredRequestFields_ShouldNotBeDeclaredNullable()
    {
        var required = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(MeetingRoomAddRequest), nameof(MeetingRoomAddRequest.Name), "name"),
            (typeof(MeetingRoomAddRequest), nameof(MeetingRoomAddRequest.Capacity), "capacity"),
            (typeof(MeetingRoomEditRequest), nameof(MeetingRoomEditRequest.MeetingRoomId), "meetingroom_id"),
            (typeof(MeetingRoomDeleteRequest), nameof(MeetingRoomDeleteRequest.MeetingRoomId), "meetingroom_id"),
            (typeof(MeetingRoomBookRequest), nameof(MeetingRoomBookRequest.MeetingRoomId), "meetingroom_id"),
            (typeof(MeetingRoomBookRequest), nameof(MeetingRoomBookRequest.StartTime), "start_time"),
            (typeof(MeetingRoomBookRequest), nameof(MeetingRoomBookRequest.EndTime), "end_time"),
            (typeof(MeetingRoomBookRequest), nameof(MeetingRoomBookRequest.BookerUserId), "booker"),
            (typeof(MeetingRoomBookByScheduleRequest), nameof(MeetingRoomBookByScheduleRequest.ScheduleId), "schedule_id"),
            (typeof(MeetingRoomBookByMeetingRequest), nameof(MeetingRoomBookByMeetingRequest.MeetingId), "meetingid"),
            (typeof(MeetingRoomBookInfoGetRequest), nameof(MeetingRoomBookInfoGetRequest.BookingId), "booking_id"),
            (typeof(MeetingRoomCancelBookRequest), nameof(MeetingRoomCancelBookRequest.BookingId), "booking_id"),
        };

        foreach (var (owner, property, jsonName) in required)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            IsNullableMember(prop!).Should().BeFalse(
                $"{jsonName} 官方参数表标为必填，不得声明为可空（可空即允许请求体省略必填参数）");
        }

        var optional = new (Type Owner, string Property, string JsonName)[]
        {
            (typeof(MeetingRoomAddRequest), nameof(MeetingRoomAddRequest.City), "city"),
            (typeof(MeetingRoomAddRequest), nameof(MeetingRoomAddRequest.EquipmentIdList), "equipment"),
            (typeof(MeetingRoomEditRequest), nameof(MeetingRoomEditRequest.Name), "name"),
            (typeof(MeetingRoomListRequest), nameof(MeetingRoomListRequest.Building), "building"),
            (typeof(GetMeetingRoomBookingInfoRequest), nameof(GetMeetingRoomBookingInfoRequest.MeetingRoomId), "meetingroom_id"),
            (typeof(MeetingRoomBookRequest), nameof(MeetingRoomBookRequest.Subject), "subject"),
            (typeof(MeetingRoomBookRequest), nameof(MeetingRoomBookRequest.AttendeeUserIdList), "attendees"),
            (typeof(MeetingRoomCancelBookRequest), nameof(MeetingRoomCancelBookRequest.KeepSchedule), "keep_schedule"),
            (typeof(MeetingRoomCancelBookRequest), nameof(MeetingRoomCancelBookRequest.CancelDate), "cancel_date"),
        };

        foreach (var (owner, property, jsonName) in optional)
        {
            var prop = owner.GetProperty(property);
            prop.Should().NotBeNull($"{owner.Name}.{property} 必须存在");
            IsNullableMember(prop!).Should().BeTrue(
                $"{jsonName} 官方参数表标为「必须=否」，须保持可空以便请求体省略（收敛为非可空即强制每个请求都带该参数）");
        }

        // bookinfo/get 的 meetingroom_id 官方必填，但它位于查询侧参数表，容易被误当成「过滤条件」而放宽
        IsNullableMember(typeof(MeetingRoomBookInfoGetRequest)
                .GetProperty(nameof(MeetingRoomBookInfoGetRequest.MeetingRoomId))!)
            .Should().BeFalse("93620「获取会议室预定记录详情」把 meetingroom_id 与 booking_id 并列标为必填");
    }

    /// <summary>
    /// 属性是否可空：值类型的 <see cref="Nullable{T}"/>，或引用类型的可空标注
    /// （<c>PropertyType</c> 不含引用可空性信息，须走 Nullable Context API）。
    /// </summary>
    private static bool IsNullableMember(PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) != null)
        {
            return true;
        }

        return new NullabilityInfoContext().Create(property).WriteState == NullabilityState.Nullable;
    }
}
