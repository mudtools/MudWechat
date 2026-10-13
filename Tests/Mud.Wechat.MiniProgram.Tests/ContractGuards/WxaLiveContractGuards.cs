// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Mud.Wechat.MiniProgram.DataModels.Live;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram.Tests.ContractGuards;

/// <summary>
/// 小程序线「直播」域契约守卫（LB 系列：38 端点路由与动词 + 令牌绑定 + GET 参数形态 +
/// DTO 登记计数与 JSON 上下文登记 + 字段抽样与共用 DTO 裁决 + 官方反直觉点锁定）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域构成（官方 2026-10-13 逐页取证，<c>OpenApiDoc/livebroadcast</c> 分区全集 38 端点）</b>：
/// 直播间本体 11（<see cref="IWxaLiveRoomService"/>）+ 小助手与主播副号 8
/// （<see cref="IWxaLiveAssistantService"/>）+ 房间内商品 7（<see cref="IWxaLiveRoomGoodsService"/>）+
/// 商品库管理 7（<see cref="IWxaLiveGoodsService"/>）+ 成员角色 3（<see cref="IWxaLiveRoleService"/>）+
/// 长期订阅群发 2（<see cref="IWxaLiveSubscribeService"/>）。
/// </para>
/// <para>
/// <b>与前缀禁令的关系（本域最需要防误读的一处）</b>：P3-R1 历史上按前缀整族禁止
/// <c>/wxaapi/broadcast/*</c>（依据是「直播插件停止接入」的公开报道）。2026-10-13 逐页取证推翻其前提：
/// 38 个服务端 API 页全部正文可渲染、在目录树内、声明权限集 52 与「服务器端调用」，
/// 真正「页面不存在或已失效」的是 <c>dev/framework/activity/live-player-plugin</c>（前端插件形态）
/// ⇒ P3-R1 于同日<b>解除该不实现裁决</b>，改为「清单式在册」（见
/// <c>MiniProgramP3RulingsContractGuards</c>：broadcast 前缀 34 条 + 文件级豁免六个接口）。
/// 两侧守卫合起来才是该前缀的完整闸门：本文件锁 38 条全集的路由 / 动词 / 参数形态，
/// P3 侧锁「清单外的 broadcast 路由不得凭空建模」。
/// </para>
/// <para>
/// <b>与全局守卫的分工</b>：跨线路由零重叠（MP-X1）、带令牌接口计数（MP-X2）、端点聚合计数
/// （MP-X5 / ContractBaseline）与 DI 装配面由 <c>MiniProgramContractGuards</c> 承担；本文件只锁本域精确契约。
/// </para>
/// </remarks>
public class WxaLiveContractGuards
{
    private const string RegistryGroupName = "Live";

    private const string DtoNamespace = "Mud.Wechat.MiniProgram.DataModels.Live";

    /// <summary>
    /// 直播域官方路由表（38 端点：直播间本体 11 + 小助手与副号 8 + 房间内商品 7 + 商品库 7 +
    /// 成员角色 3 + 长期订阅 2；路由与动词照官方原文）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] Routes =
    {
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.CreateRoomAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/create"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.EditRoomAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/editroom"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.DeleteRoomAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/deleteroom"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.GetPushUrlAsync), typeof(GetAttribute), "/wxaapi/broadcast/room/getpushurl"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.GetSharedCodeAsync), typeof(GetAttribute), "/wxaapi/broadcast/room/getsharedcode"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.GetLiveInfoAsync), typeof(PostAttribute), "/wxa/business/getliveinfo"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.UpdateFeedPublicAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/updatefeedpublic"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.UpdateReplayAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/updatereplay"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.UpdateKfAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/updatekf"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.UpdateCommentAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/updatecomment"),
        (typeof(IWxaLiveRoomService), nameof(IWxaLiveRoomService.ImportGoodsAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/addgoods"),

        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.AddAssistantAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/addassistant"),
        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.GetAssistantListAsync), typeof(GetAttribute), "/wxaapi/broadcast/room/getassistantlist"),
        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.ModifyAssistantAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/modifyassistant"),
        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.RemoveAssistantAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/removeassistant"),
        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.AddSubAnchorAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/addsubanchor"),
        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.ModifySubAnchorAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/modifysubanchor"),
        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.DeleteSubAnchorAsync), typeof(PostAttribute), "/wxaapi/broadcast/room/deletesubanchor"),
        (typeof(IWxaLiveAssistantService), nameof(IWxaLiveAssistantService.GetSubAnchorAsync), typeof(GetAttribute), "/wxaapi/broadcast/room/getsubanchor"),

        (typeof(IWxaLiveRoomGoodsService), nameof(IWxaLiveRoomGoodsService.DeleteInRoomAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/deleteInRoom"),
        (typeof(IWxaLiveRoomGoodsService), nameof(IWxaLiveRoomGoodsService.PushGoodsAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/push"),
        (typeof(IWxaLiveRoomGoodsService), nameof(IWxaLiveRoomGoodsService.OnSaleAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/onsale"),
        (typeof(IWxaLiveRoomGoodsService), nameof(IWxaLiveRoomGoodsService.SortGoodsAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/sort"),
        (typeof(IWxaLiveRoomGoodsService), nameof(IWxaLiveRoomGoodsService.GetGoodsVideoAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/getVideo"),
        (typeof(IWxaLiveRoomGoodsService), nameof(IWxaLiveRoomGoodsService.GetGoodsKeyAsync), typeof(GetAttribute), "/wxaapi/broadcast/goods/getkey"),
        (typeof(IWxaLiveRoomGoodsService), nameof(IWxaLiveRoomGoodsService.SetGoodsKeyAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/setkey"),

        (typeof(IWxaLiveGoodsService), nameof(IWxaLiveGoodsService.AddGoodsAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/add"),
        (typeof(IWxaLiveGoodsService), nameof(IWxaLiveGoodsService.UpdateGoodsAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/update"),
        (typeof(IWxaLiveGoodsService), nameof(IWxaLiveGoodsService.ResubmitAuditAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/audit"),
        (typeof(IWxaLiveGoodsService), nameof(IWxaLiveGoodsService.ResetAuditAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/resetaudit"),
        (typeof(IWxaLiveGoodsService), nameof(IWxaLiveGoodsService.DeleteGoodsAsync), typeof(PostAttribute), "/wxaapi/broadcast/goods/delete"),
        (typeof(IWxaLiveGoodsService), nameof(IWxaLiveGoodsService.GetApprovedGoodsAsync), typeof(GetAttribute), "/wxaapi/broadcast/goods/getapproved"),
        (typeof(IWxaLiveGoodsService), nameof(IWxaLiveGoodsService.GetGoodsWarehouseAsync), typeof(PostAttribute), "/wxa/business/getgoodswarehouse"),

        (typeof(IWxaLiveRoleService), nameof(IWxaLiveRoleService.AddRoleAsync), typeof(PostAttribute), "/wxaapi/broadcast/role/addrole"),
        (typeof(IWxaLiveRoleService), nameof(IWxaLiveRoleService.DeleteRoleAsync), typeof(PostAttribute), "/wxaapi/broadcast/role/deleterole"),
        (typeof(IWxaLiveRoleService), nameof(IWxaLiveRoleService.GetRoleListAsync), typeof(GetAttribute), "/wxaapi/broadcast/role/getrolelist"),

        (typeof(IWxaLiveSubscribeService), nameof(IWxaLiveSubscribeService.GetFollowersAsync), typeof(PostAttribute), "/wxa/business/get_wxa_followers"),
        (typeof(IWxaLiveSubscribeService), nameof(IWxaLiveSubscribeService.PushMessageAsync), typeof(PostAttribute), "/wxa/business/push_message"),
    };

    /// <summary>契约守卫 LB1：38 端点路由与动词必须与官方契约一致（前缀只允许两个官方族）。</summary>
    [Fact]
    public void LiveEndpoints_ShouldMatchOfficialRoutes()
    {
        // 38 端点 = 直播间本体 11 + 小助手与副号 8 + 房间内商品 7 + 商品库 7 + 成员角色 3 + 长期订阅 2
        //（官方 OpenApiDoc/livebroadcast 分区 2026-10-13 逐页取证全集）。
        Routes.Should().HaveCount(38,
            "直播域官方开放面为 38 端点（官方文档 2026-10-13 逐页核验：room 11 + assistant 8 + roomgoods 7 + goods 7 + role 3 + subscribe 2）");
        Routes.Select(static r => r.Route).Distinct().Should().HaveCount(38, "路由表不得出现重复条目");

        var allowedPrefixes = new[] { "/wxaapi/broadcast/", "/wxa/business/" };

        foreach (var (iface, method, httpAttribute, route) in Routes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（照官方原文拼写，禁止改写）");
            allowedPrefixes.Any(p => route.StartsWith(p, StringComparison.Ordinal)).Should().BeTrue(
                $"{route} 必须落在官方两个路由族之一（/wxaapi/broadcast/ + /wxa/business/）");
        }

        // 反直觉点锁定 1：同族大小写混排 —— getapproved / editroom / deleteroom 全小写，
        // 但 deleteInRoom 与 getVideo 为驼峰。禁止「顺手规整」成一致风格。
        Routes.Select(static r => r.Route).Should()
            .Contain("/wxaapi/broadcast/goods/getapproved")
            .And.Contain("/wxaapi/broadcast/goods/getVideo")
            .And.Contain("/wxaapi/broadcast/goods/deleteInRoom",
                "官方同族内 getapproved 全小写而 getVideo / deleteInRoom 驼峰，照录不规整");

        // 反直觉点锁定 2：四支「查询」端点走 /wxa/business/ 前缀而非 broadcast 族（官方历史遗留命名）。
        Routes.Where(static r => r.Route.StartsWith("/wxa/business/", StringComparison.Ordinal))
            .Select(static r => r.Route)
            .Should().BeEquivalentTo(new[]
            {
                "/wxa/business/getliveinfo",
                "/wxa/business/getgoodswarehouse",
                "/wxa/business/get_wxa_followers",
                "/wxa/business/push_message",
            }, "直播域四支 business 前缀端点（官方页路由即此，不得改写为 broadcast 前缀）");

        // 反直觉点锁定 3：官方 D 桶里的错误拼写 /wxaapi/broadcast/getgoodswarehouse 不得出现
        //（真名为 /wxa/business/getgoodswarehouse；该错误拼写由零声明守卫 NC1 兜住，此处正向钉住正确落位）。
        Routes.Should().NotContain(static r => r.Route == "/wxaapi/broadcast/getgoodswarehouse");
    }

    /// <summary>
    /// 契约守卫 LB2：GET 端点参数形态 —— 官方在 GET 页给出「请求体」表，但示例一律走查询串
    /// ⇒ GET 侧必须只有 <c>[Query]</c>、零 <c>[Body]</c>；POST 侧恰有一个 <c>[Body]</c> 请求体参数。
    /// </summary>
    [Fact]
    public void LiveEndpoints_ShouldRespectGetQueryAndPostBodyShapes()
    {
        // GET 7 端点的查询参数名集（照官方参数表，含本域唯一的「无业务参数」端点 getkey）。
        var expectedQueryParams = new Dictionary<string, string[]>
        {
            [nameof(IWxaLiveRoomService.GetPushUrlAsync)] = new[] { "roomId" },
            [nameof(IWxaLiveRoomService.GetSharedCodeAsync)] = new[] { "roomId", "params" },
            [nameof(IWxaLiveAssistantService.GetAssistantListAsync)] = new[] { "roomId" },
            [nameof(IWxaLiveAssistantService.GetSubAnchorAsync)] = new[] { "roomId" },
            [nameof(IWxaLiveRoomGoodsService.GetGoodsKeyAsync)] = Array.Empty<string>(),
            [nameof(IWxaLiveGoodsService.GetApprovedGoodsAsync)] = new[] { "offset", "status", "limit" },
            [nameof(IWxaLiveRoleService.GetRoleListAsync)] = new[] { "offset", "limit", "keyword" },
        };

        var getRoutes = Routes.Where(static r => r.HttpAttribute == typeof(GetAttribute)).ToList();
        getRoutes.Should().HaveCount(7, "官方动词为 GET 的端点为 7（room 2 + assistant 2 + roomgoods 1 + goods 1 + role 1）");

        foreach (var (iface, method, _, route) in getRoutes)
        {
            var target = iface.GetMethod(method)!;
            target.GetParameters()
                .SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{route} 官方动词为 GET——全仓零 GET + [Body] 先例，参数须走 [Query]");

            var queryNames = target.GetParameters()
                .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
                .Where(static n => n != null)
                .Select(static n => n!)
                .ToArray();

            queryNames.Should().BeEquivalentTo(expectedQueryParams[method],
                because: $"{route} 的业务参数名须逐字照官方参数表（camelCase 域内原样，不做 snake_case 归一）");
        }

        var postRoutes = Routes.Where(static r => r.HttpAttribute == typeof(PostAttribute)).ToList();
        postRoutes.Should().HaveCount(31, "官方动词为 POST 的端点为 31（room 9 + assistant 6 + roomgoods 6 + goods 6 + role 2 + subscribe 2）");

        foreach (var (iface, method, _, route) in postRoutes)
        {
            var target = iface.GetMethod(method)!;
            target.GetParameters()
                .Count(static p => p.GetCustomAttribute<BodyAttribute>() != null)
                .Should().Be(1, $"{route} 官方请求体为 JSON 对象——必须恰有一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 LB3：六接口令牌绑定与注册组（复用公众号 <c>MpTokenTypes.AccessToken</c>，MP-X2/X3 同口径）。</summary>
    [Fact]
    public void LiveInterfaces_ShouldBindMpAccessToken_AndShareRegistryGroup()
    {
        var interfaces = new[]
        {
            typeof(IWxaLiveRoomService),
            typeof(IWxaLiveAssistantService),
            typeof(IWxaLiveRoomGoodsService),
            typeof(IWxaLiveGoodsService),
            typeof(IWxaLiveRoleService),
            typeof(IWxaLiveSubscribeService),
        };

        interfaces.Should().HaveCount(6, "直播域按官方子目录族分为六接口（合计 38 端点）");

        foreach (var iface in interfaces)
        {
            var token = iface.GetCustomAttribute<TokenAttribute>();
            token.Should().NotBeNull($"{iface.Name} 端点消费小程序 access_token（官方「服务器端调用」页令牌为 access_token，权限集 52）");
            token!.TokenType.Should().Be(MpTokenTypes.AccessToken,
                $"{iface.Name} 必须复用公众号/小程序同域令牌（改绑小店令牌或新增类型会让单槽 Resolve 返回 null，errcode 自愈静默失效）");
            token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入 access_token（MP-X3）");
            token.Name.Should().Be("access_token");

            var api = iface.GetCustomAttribute<HttpClientApiAttribute>();
            api.Should().NotBeNull("必须声明 [HttpClientApi]");
            api!.RegistryGroupName.Should().Be(RegistryGroupName, $"{iface.Name} 必须挂 Live 注册组");
            api.TokenManage.Should().Be(nameof(IMpAppManager), "复用小程序/公众号令牌底座（AddMpApp 硬前置）");
        }

        // 六接口同组 ⇒ DI 侧由同一条生成入口装载；注册面漂移由 MiniProgramContractGuards 的装配用例兜住。
        interfaces.Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .Distinct().Should().HaveCount(1, "本域六接口共用一个注册组（AddLiveWebApiHttpClient）");

        // 端点归属自证：六接口的方法数之和必须等于路由表规模（防「加了接口忘了进路由表」）。
        interfaces.Select(static i => i.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Length)
            .Sum().Should().Be(Routes.Length, "六接口声明方法总数须与 LB1 路由表一致");
    }

    /// <summary>契约守卫 LB4：DTO 全量登记进 AOT JSON 上下文；命名空间清单锁死（常量类恰 6）。</summary>
    [Fact]
    public void LiveDataModels_ShouldBeRegisteredInJsonContext()
    {
        var assembly = typeof(WxaLiveRoomInfoItem).Assembly;

        var domainTypes = assembly.GetTypes()
            .Where(static t => !t.IsNested && t.Namespace == DtoNamespace)
            .ToList();

        domainTypes.Should().NotBeEmpty("直播域命名空间非空——为空说明反射口径失效，守卫会静默假绿");

        var dtoTypes = domainTypes
            .Where(static t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        // 53 DTO = 直播间本体 16（请求 9 + 响应 4 + 条目 3）
        //          + 小助手与副号 8（请求 5 + 响应 2 + 条目 1）
        //          + 房间内商品 7（请求 4 + 响应 2 + 条目 1）
        //          + 商品库 13（请求 5 + 响应 4 + 条目 4）
        //          + 成员角色 4（请求 1 + 响应 2 + 条目 1）
        //          + 长期订阅 5（请求 2 + 响应 2 + 条目 1）。
        dtoTypes.Should().HaveCount(53,
            "直播域 38 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫"
            + "（room 16 + assistant 8 + roomgoods 7 + goods 13 + role 4 + subscribe 5 = 53）");

        foreach (var type in dtoTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(RegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Live");
        }

        var contextType = assembly.GetType(DtoNamespace + ".LiveJsonContext");
        contextType.Should().NotBeNull(
            "LiveJsonContext 尚未生成：须同批跑 scripts/AddHttpJsonSerializable.ps1 + scripts/GenerateJsonContext.ps1"
            + "（Generated/ 为生成物，勿手改）");

        var defaultProperty = contextType!.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
        defaultProperty.Should().NotBeNull("生成的 JsonSerializerContext 必有静态 Default 入口");

        var jsonContext = (JsonSerializerContext)defaultProperty!.GetValue(null)!;

        foreach (var type in dtoTypes)
        {
            jsonContext.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 标注 [HttpJsonSerializable(SerializerClassName = \"Live\")]，"
                + "必须登记进 LiveJsonContext（AOT 生死线：缺失时 JIT 靠反射侥幸可用，Native AOT 下首次调用失败）");
        }

        // 命名空间清单锁死：除已登记 DTO、生成上下文与六个官方枚举常量类外不得出现任何新类型。
        var constClasses = new[]
        {
            nameof(WxaLiveRoomStatus), nameof(WxaLiveRoomType), nameof(WxaLiveInfoAction),
            nameof(WxaLivePriceType), nameof(WxaLiveGoodsAuditStatus), nameof(WxaLiveRoleType),
        };
        var unregistered = domainTypes
            .Where(t => !dtoTypes.Contains(t) && t != contextType && !constClasses.Contains(t.Name))
            .ToList();

        unregistered.Should().BeEmpty(
            "直播域命名空间出现既非 [HttpJsonSerializable] 登记类型、也非生成上下文、亦非已登记常量类的新类型："
            + string.Join(", ", unregistered.Select(static t => t.Name)) + " —— 常量类或新 DTO 须同批更新本守卫");

        constClasses.Select(n => assembly.GetType(DtoNamespace + "." + n))
            .Should().OnlyContain(t => t != null,
            "六个官方枚举常量类必须存在（房间状态 / 房间类型 / getliveinfo action / 价格类型 / 审核状态 / 成员角色）");

        // 官方枚举常量类必须是 static class + const 字段（不得写成 enum：跨 TFM 线格式与序列化口径不一）。
        foreach (var name in constClasses)
        {
            var constType = assembly.GetType(DtoNamespace + "." + name)!;
            constType.IsAbstract.Should().BeTrue($"{name} 须为 static class（全仓常量类统一形态）");
            constType.GetFields().Where(static f => f.IsLiteral).Should().NotBeEmpty($"{name} 必须含 const 字段");
        }
    }

    /// <summary>契约守卫 LB5：官方字段名抽样 + 类型口径 + 共用 DTO 裁决 + 官方反直觉点锁定。</summary>
    [Fact]
    public void LiveDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 直播间本体（create / editroom 侧为 camelCase，官方参数表照录）。
        AssertJsonProperty<WxaLiveCreateRoomRequest>(nameof(WxaLiveCreateRoomRequest.IsFeedsPublic), "isFeedsPublic", "官方原文拼写");
        AssertJsonProperty<WxaLiveCreateRoomRequest>(nameof(WxaLiveCreateRoomRequest.CreaterWechat), "createrWechat", "官方原文拼写（creater 而非 creator，照录不改拼写）");
        AssertJsonProperty<WxaLiveCreateRoomRequest>(nameof(WxaLiveCreateRoomRequest.SubAnchorWechat), "subAnchorWechat", "官方原文拼写（仅创建页有副号）");
        AssertJsonProperty<WxaLiveCreateRoomResponse>(nameof(WxaLiveCreateRoomResponse.QrcodeUrl), "qrcode_url", "官方原文拼写（camel 请求体 + snake 应答混排）");
        AssertJsonProperty<WxaLiveDeleteRoomRequest>(nameof(WxaLiveDeleteRoomRequest.Id), "id", "官方原文拼写（删除/编辑/评论开关三端点的定位键均为 id，非 roomId）");
        AssertJsonProperty<WxaLivePushUrlResponse>(nameof(WxaLivePushUrlResponse.PushAddr), "pushAddr", "官方原文拼写（pushAddr 而非 pushUrl）");
        AssertJsonProperty<WxaLiveSharedCodeResponse>(nameof(WxaLiveSharedCodeResponse.CdnUrl), "cdnUrl", "官方原文拼写");
        AssertJsonProperty<WxaLiveGetLiveInfoRequest>(nameof(WxaLiveGetLiveInfoRequest.RoomId), "room_id", "官方原文拼写（business 前缀端点走 snake_case）");
        AssertJsonProperty<WxaLiveInfoResponse>(nameof(WxaLiveInfoResponse.LiveReplay), "live_replay", "官方原文拼写（action=get_replay 时才出现）");
        AssertJsonProperty<WxaLiveRoomInfoItem>(nameof(WxaLiveRoomInfoItem.Roomid), "roomid", "官方原文拼写（连写，全仓唯一的 roomid 形态）");
        AssertJsonProperty<WxaLiveRoomInfoItem>(nameof(WxaLiveRoomInfoItem.CreaterOpenid), "creater_openid", "官方原文拼写");
        AssertJsonProperty<WxaLiveReplayItem>(nameof(WxaLiveReplayItem.MediaUrl), "media_url", "官方原文拼写");

        // 小助手与主播副号。
        AssertJsonProperty<WxaLiveAssistantItem>(nameof(WxaLiveAssistantItem.Alias), "alias", "官方原文拼写（列表侧是 alias，写入侧是 username，两处不同源不得统一）");
        AssertJsonProperty<WxaLiveAssistantListResponse>(nameof(WxaLiveAssistantListResponse.MaxCount), "maxCount", "官方原文拼写（上限容量随应答返回）");
        AssertJsonProperty<WxaLiveSubAnchorResponse>(nameof(WxaLiveSubAnchorResponse.Username), "username", "官方原文小写拼写");

        // 房间内商品。
        AssertJsonProperty<WxaLiveOnSaleRequest>(nameof(WxaLiveOnSaleRequest.OnSale), "onSale", "官方原文拼写（1 上架 / 0 下架）");
        AssertJsonProperty<WxaLiveGoodsKeyRequest>(nameof(WxaLiveGoodsKeyRequest.GoodsKey), "goodsKey", "官方原文拼写（请求侧键名）");
        AssertJsonProperty<WxaLiveGoodsKeyResponse>(nameof(WxaLiveGoodsKeyResponse.VendorGoodsKey), "vendorGoodsKey", "官方原文拼写（应答侧键名与请求侧不同）");
        AssertJsonProperty<WxaLiveGoodsVideoResponse>(nameof(WxaLiveGoodsVideoResponse.Url), "url", "官方原文拼写");

        // 商品库。
        AssertJsonProperty<WxaLiveAddGoodsResponse>(nameof(WxaLiveAddGoodsResponse.AuditId), "auditId", "官方原文拼写（官方警示「必须保存商品ID与审核单ID」）");
        AssertJsonProperty<WxaLiveResetAuditRequest>(nameof(WxaLiveResetAuditRequest.AuditId), "auditId", "官方原文拼写（resetaudit 需 goodsId + auditId 双定位键）");
        AssertJsonProperty<WxaLiveApprovedGoodsItem>(nameof(WxaLiveApprovedGoodsItem.GoodsId), "goodsId", "官方原文拼写（getapproved 侧为 camelCase 字符串）");
        AssertJsonProperty<WxaLiveWarehouseGoodsItem>(nameof(WxaLiveWarehouseGoodsItem.GoodsId), "goods_id", "官方原文拼写（warehouse 侧为 snake_case）");
        AssertJsonProperty<WxaLiveWarehouseGoodsItem>(nameof(WxaLiveWarehouseGoodsItem.CoverImgUrl), "cover_img_url", "官方原文拼写");
        AssertJsonProperty<WxaLiveGoodsInfo>(nameof(WxaLiveGoodsInfo.CoverImgUrl), "coverImgUrl", "官方原文拼写（add/update 请求体为 camelCase）");
        AssertJsonProperty<WxaLiveGoodsWarehouseRequest>(nameof(WxaLiveGoodsWarehouseRequest.GoodsIds), "goods_ids", "官方原文拼写（数组字段名为 goods_ids）");

        // 成员角色。
        AssertJsonProperty<WxaLiveAddRoleResponse>(nameof(WxaLiveAddRoleResponse.CodeUrl), "codeurl", "官方原文全小写拼写");
        AssertJsonProperty<WxaLiveRoleItem>(nameof(WxaLiveRoleItem.HeadingImg), "headingimg", "官方原文全小写拼写");
        AssertJsonProperty<WxaLiveRoleItem>(nameof(WxaLiveRoleItem.UpdateTimestamp), "updateTimestamp", "官方原文拼写（同页 snake/camel 混排）");

        // 长期订阅与群发。
        AssertJsonProperty<WxaLiveFollowerItem>(nameof(WxaLiveFollowerItem.SubscribeTime), "subscribe_time", "官方原文拼写");
        AssertJsonProperty<WxaLiveFollowersRequest>(nameof(WxaLiveFollowersRequest.PageBreak), "page_break", "官方原文拼写（游标字段）");
        AssertJsonProperty<WxaLivePushMessageRequest>(nameof(WxaLivePushMessageRequest.UserOpenid), "user_openid", "官方原文拼写（键名为 user_openid 而非 openid）");

        // 类型口径锁定（官方参数表逐页口径，含三处「同名字段跨页类型不同」的照录）。
        typeof(WxaLiveRoomGoodsItem).GetProperty(nameof(WxaLiveRoomGoodsItem.Price))!.PropertyType
            .Should().Be(typeof(long?), "房间内商品列表 price 官方为 number（单位分）");
        typeof(WxaLiveApprovedGoodsItem).GetProperty(nameof(WxaLiveApprovedGoodsItem.Price))!.PropertyType
            .Should().Be(typeof(double?), "商品库列表 price 官方为两位小数金额（与房间内商品口径不同，各自照录）");
        typeof(WxaLiveGoodsIdRequest).GetProperty(nameof(WxaLiveGoodsIdRequest.GoodsId))!.PropertyType
            .Should().Be(typeof(long?), "写侧 goodsId 官方为 number");
        typeof(WxaLiveApprovedGoodsItem).GetProperty(nameof(WxaLiveApprovedGoodsItem.GoodsId))!.PropertyType
            .Should().Be(typeof(string), "getapproved 侧 goodsId 官方参数表为 string（与写侧口径不同，照录不统一）");
        typeof(WxaLiveWarehouseGoodsItem).GetProperty(nameof(WxaLiveWarehouseGoodsItem.AuditStatus))!.PropertyType
            .Should().Be(typeof(string), "warehouse 侧 audit_status 官方为 string（与 WxaLiveGoodsAuditStatus 的 int 常量集口径不同）");
        typeof(WxaLiveSortGoodsItem).GetProperty(nameof(WxaLiveSortGoodsItem.GoodsId))!.PropertyType
            .Should().Be(typeof(long?), "sort 请求体 goods 数组项的 goodsId 为 number（官方示例误写为字符串，按参数表为准）");
        typeof(WxaLiveFollowerItem).GetProperty(nameof(WxaLiveFollowerItem.Openid))!.PropertyType
            .Should().Be(typeof(string), "官方 followers[].openid 类型列误标 array，正文描述为单个 openid ⇒ 按 string 建模并在此留档裁决");
        typeof(WxaLivePushMessageRequest).GetProperty(nameof(WxaLivePushMessageRequest.UserOpenid))!.PropertyType
            .Should().Be(typeof(List<string>), "user_openid 官方为数组（单次上限 2000 个）");
        typeof(WxaLiveGoodsKeyRequest).GetProperty(nameof(WxaLiveGoodsKeyRequest.GoodsKey))!.PropertyType
            .Should().Be(typeof(List<string>), "官方明文「key 必须为字符串数组」⇒ 不得建为 number 数组");

        // 共用 DTO 裁决 1：删除房间 / 编辑房间 / 评论开关三端点的官方定位键同为 id ⇒ 各自建类型不共用
        //（字段集不同：editroom 是全量参数、deleteroom 只有 id），但 deletesubanchor 的官方参数表只有 roomId ⇒ 单键类型复用。
        typeof(IWxaLiveAssistantService).GetMethod(nameof(IWxaLiveAssistantService.DeleteSubAnchorAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaLiveRoomIdRequest));

        // 共用 DTO 裁决 2：删除小助手 / 添加副号 / 修改副号的官方请求体二字段完全一致（roomId + username）⇒ 同一类型。
        var roomUserEndpoints = new[]
        {
            nameof(IWxaLiveAssistantService.RemoveAssistantAsync),
            nameof(IWxaLiveAssistantService.AddSubAnchorAsync),
            nameof(IWxaLiveAssistantService.ModifySubAnchorAsync),
        };
        foreach (var method in roomUserEndpoints)
        {
            typeof(IWxaLiveAssistantService).GetMethod(method)!
                .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaLiveRoomUserRequest),
                    $"{method} 与其余两端的官方请求体字段集（roomId + username）完全一致 ⇒ 共用 WxaLiveRoomUserRequest");
        }

        // 共用 DTO 裁决 3：房内商品的「按房间+商品定位」三端点（deleteInRoom / push / getVideo）请求体字段集一致 ⇒ 同一类型。
        var roomGoodsEndpoints = new[]
        {
            nameof(IWxaLiveRoomGoodsService.DeleteInRoomAsync),
            nameof(IWxaLiveRoomGoodsService.PushGoodsAsync),
            nameof(IWxaLiveRoomGoodsService.GetGoodsVideoAsync),
        };
        foreach (var method in roomGoodsEndpoints)
        {
            typeof(IWxaLiveRoomGoodsService).GetMethod(method)!
                .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaLiveRoomGoodsRequest),
                    $"{method} 官方请求体字段集（roomId + goodsId）与其余两端一致 ⇒ 共用 WxaLiveRoomGoodsRequest");
        }

        // 共用 DTO 裁决 4：goods/audit（重新提交审核）与 goods/delete 的官方请求体均为单字段 goodsId ⇒ 同一类型。
        typeof(IWxaLiveGoodsService).GetMethod(nameof(IWxaLiveGoodsService.ResubmitAuditAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaLiveGoodsIdRequest));
        typeof(IWxaLiveGoodsService).GetMethod(nameof(IWxaLiveGoodsService.DeleteGoodsAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaLiveGoodsIdRequest),
            "audit 与 delete 的官方请求体同为单字段 goodsId ⇒ 共用 WxaLiveGoodsIdRequest");

        // 共用 DTO 裁决 5：role/addrole 与 role/deleterole 的官方请求体同为 username + role ⇒ 同一类型。
        typeof(IWxaLiveRoleService).GetMethod(nameof(IWxaLiveRoleService.AddRoleAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaLiveRoleRequest));
        typeof(IWxaLiveRoleService).GetMethod(nameof(IWxaLiveRoleService.DeleteRoleAsync))!
            .GetParameters().Should().Contain(static p => p.ParameterType == typeof(WxaLiveRoleRequest),
            "addrole 与 deleterole 官方请求体字段集一致 ⇒ 共用 WxaLiveRoleRequest");

        // 历史/边界锁定：官方编辑直播间参数表不含副号与创建者字段 ⇒ 不得凭空补进 editroom 请求体。
        typeof(WxaLiveEditRoomRequest).GetProperty("SubAnchorWechat", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("editroom 官方参数表不含 subAnchorWechat（副号由 addsubanchor 一族维护）");
        typeof(WxaLiveEditRoomRequest).GetProperty("CreaterWechat", BindingFlags.Public | BindingFlags.Instance)
            .Should().BeNull("editroom 官方参数表不含 createrWechat（创建者不可改）");

        // 应答基类：本域全部响应继承 WxaResponse（errcode 判错与全线同源）。
        var responseDtos = typeof(WxaLiveRoomInfoItem).Assembly.GetTypes()
            .Where(static t => !t.IsNested
                               && t.Namespace == DtoNamespace
                               && t.IsClass
                               && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToList();

        responseDtos.Should().HaveCount(16,
            "本域官方应答对象为 16（room 4 + assistant 2 + roomgoods 2 + goods 4 + role 2 + subscribe 2）");
        responseDtos.Should().OnlyContain(static t => typeof(WxaResponse).IsAssignableFrom(t),
            "直播域响应 DTO 必须继承 WxaResponse（判错走单点判定）");

        // 官方枚举常量类取值锁定（逐页照官方取值表；官方未公布的枚举不得凭空建常量类）。
        typeof(WxaLiveRoomStatus).GetFields().Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 101, 102, 103, 104, 105, 106, 107 },
                because: "live_status 官方枚举为 101~107 七值（不连续于 0 起点，照录）");
        typeof(WxaLiveRoomType).GetFields().Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 0, 1 }, "type 官方为 0（手机直播）/ 1（推流）");
        typeof(WxaLivePriceType).GetFields().Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 1, 2, 3 }, "priceType 官方为 1（固定）/ 2（区间）/ 3（折扣）");
        typeof(WxaLiveGoodsAuditStatus).GetFields().Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 0, 1, 2, 3 }, "审核状态官方为 0~3 四值（getapproved 的 status 参数同集）");
        typeof(WxaLiveRoleType).GetFields().Where(static f => f.IsLiteral)
            .Select(static f => (int)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { 0, 1, 2, 3 }, "成员角色官方为 0（超级管理员）/ 1（管理员）/ 2（主播）/ 3（运营）");
        typeof(WxaLiveInfoAction).GetFields().Where(static f => f.IsLiteral)
            .Select(static f => (string)f.GetRawConstantValue()!)
            .Should().BeEquivalentTo(new[] { "get_replay" },
            "官方 getliveinfo 的 action 参数只在「获取回放」场景给出唯一取值 get_replay ⇒ 不得凭空补其它值");
    }

    // ------------------------------------------------------------------
    // 内部辅助
    // ------------------------------------------------------------------

    private static void AssertJsonProperty<T>(string propertyName, string expectedJsonName, string because)
        where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 不存在（契约面漂移）");

        var jsonName = property!.GetCustomAttribute<JsonPropertyNameAttribute>();
        jsonName.Should().NotBeNull($"{typeof(T).Name}.{propertyName} 必须显式标注 [JsonPropertyName]");
        jsonName!.Name.Should().Be(expectedJsonName, $"{typeof(T).Name}.{propertyName} 的 JSON 名必须照官方原文（{because}）");
    }
}
