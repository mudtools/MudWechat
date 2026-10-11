// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.PlatformKf;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「平台客诉工单」域契约守卫（CPK 系列：PlatformKf 域 4 端点路由 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量 + 与商家客服的概念辨析）。
/// </summary>
/// <remarks>
/// <para>
/// <b>概念辨析（守卫措辞锁定）</b>：本域是平台侧客诉工单（<c>/channels/ec/platformkf/*</c>），
/// <b>不是</b>商家客服会话（<c>/channels/ec/commkf/*</c>，见 <c>ChannelsKfContractGuards</c>）。
/// </para>
/// <para>
/// <b>第三方代调用口径</b>：本域 4 端点官方权限集 id 均为 131（服务商可代微信小店商家调用）。
/// </para>
/// </remarks>
public class ChannelsPlatformKfContractGuards
{
    private const string PlatformKfRegistryGroupName = "PlatformKf";

    /// <summary>PlatformKf 域官方路由表（4 端点：/channels/ec/platformkf/*，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] PlatformKfRoutes =
    {
        (typeof(IChannelsPlatformKfService), nameof(IChannelsPlatformKfService.GetTicketClassConfigAsync), typeof(PostAttribute), "/channels/ec/platformkf/getticketclassconfig"),
        (typeof(IChannelsPlatformKfService), nameof(IChannelsPlatformKfService.GetMerchantTicketListAsync), typeof(PostAttribute), "/channels/ec/platformkf/getmchticketlist"),
        (typeof(IChannelsPlatformKfService), nameof(IChannelsPlatformKfService.GetMerchantTicketDetailAsync), typeof(PostAttribute), "/channels/ec/platformkf/getmchticketdetail"),
        (typeof(IChannelsPlatformKfService), nameof(IChannelsPlatformKfService.OperateMerchantTicketAsync), typeof(PostAttribute), "/channels/ec/platformkf/operatemchticket"),
    };

    /// <summary>契约守卫 CPK1：PlatformKf 域 4 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void PlatformKfEndpoints_ShouldMatchOfficialRoutes()
    {
        // 4 端点 = 工单分类配置 1 + 工单列表 1 + 工单详情 1 + 商户处理 1。
        PlatformKfRoutes.Should().HaveCount(4, "客诉工单域开放分类配置 / 列表 / 详情 / 处理四个端点");
        PlatformKfRoutes.Select(r => r.Route).Distinct().Should().HaveCount(4);

        PlatformKfRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/platformkf/", StringComparison.Ordinal))
            .Should().Be(4, "4 端点全部走 /channels/ec/platformkf/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in PlatformKfRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CPK2：PlatformKf 域 4 端点全部为 POST；空请求体端点（getticketclassconfig）不得携带 [Body] 参数。</summary>
    [Fact]
    public void PlatformKfEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in PlatformKfRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        // getticketclassconfig 官方请求体为空（Request Payload 无参数），不得人为造请求体 DTO。
        var target0 = typeof(IChannelsPlatformKfService).GetMethod(nameof(IChannelsPlatformKfService.GetTicketClassConfigAsync))!;
        target0.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("getticketclassconfig 官方无请求体参数——不得声明 [Body]");
    }

    /// <summary>契约守卫 CPK3：令牌绑定与注册形态。</summary>
    [Fact]
    public void PlatformKfInterface_ShouldBindChannelsToken_AndRegisterUnderPlatformKfGroup()
    {
        var token = typeof(IChannelsPlatformKfService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("PlatformKf 域端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsPlatformKfService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(PlatformKfRegistryGroupName, "必须挂 PlatformKf 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("PlatformKf");
        typeof(ChannelsServiceBuilder).GetMethod("AddPlatformKfApi").Should().NotBeNull("必须提供 AddPlatformKfApi 链式注册方法");
    }

    /// <summary>契约守卫 CPK4：PlatformKf 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void PlatformKfDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = PlatformKfJsonContext.Default;

        var domainTypes = typeof(ChannelsPlatformKfGetTicketListRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.PlatformKf"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(15,
            "PlatformKf 域 4 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（分类配置响应 1 + mch_reply_options 1 + 分类配置项 1 + 回复选项 1 + 列表请求 1 + 列表响应 1 + 工单条目 1 +" +
            " order_info 1 + goods 1 + user_info 1 + merchant_operate 1 + platform_operate 1 + 详情请求 1 + 详情响应 1 + 处理请求 1 = 15）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 PlatformKf 域命名空间，必须登记进 PlatformKfJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(PlatformKfRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 PlatformKf");
        }

        // 常量类（静态常量类不参与序列化）：PlatformKf 域 4 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(4,
            "PlatformKf 域常量类 = 工单状态 + 工单分类类型 + 排序方式 + 平台审核结果");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CPK5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void PlatformKfDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 工单分类配置。
        AssertJsonProperty<ChannelsPlatformKfGetTicketClassConfigResponse>("mch_reply_options", "商家回复选项配置");
        AssertJsonProperty<ChannelsPlatformKfMchReplyOptions>("conf_list", "分类配置列表");
        AssertJsonProperty<ChannelsPlatformKfTicketClassConfig>("ticket_class_type", "工单分类类型");
        AssertJsonProperty<ChannelsPlatformKfTicketClassConfig>("ticket_class_name", "工单分类名称");
        AssertJsonProperty<ChannelsPlatformKfTicketClassConfig>("option_list", "可选回复配置");
        AssertJsonProperty<ChannelsPlatformKfTicketReplyOption>("option_id", "工单回复选项");
        AssertJsonProperty<ChannelsPlatformKfTicketReplyOption>("is_must_description", "补充说明是否必填");
        AssertJsonProperty<ChannelsPlatformKfTicketReplyOption>("is_must_urls", "图片/视频凭证是否必填");
        AssertJsonProperty<ChannelsPlatformKfTicketReplyOption>("is_must_confer_time", "协商时间是否必填");

        // 工单列表。
        AssertJsonProperty<ChannelsPlatformKfGetTicketListRequest>("ticket_state", "工单状态");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListRequest>("ticket_class_type", "工单类型");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListRequest>("search_ticket_id", "工单id/订单id 任意其一");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListRequest>("search_start_time", "查询起始时间");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListRequest>("sort_order_type", "排序方式");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListRequest>("limit", "每页数量");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListRequest>("start", "分页起始偏移量");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListResponse>("ticket_item_list", "工单列表");
        AssertJsonProperty<ChannelsPlatformKfGetTicketListResponse>("total", "工单总数");

        // 工单条目（列表元素与详情 merchant_ticket 共用）。
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("ticket_id", "商家工单 ID");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("ticket_begin_time", "工单开始时间");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("ticket_deal_end_time", "工单处理截止时间");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("aftersale_id", "关联售后 ID");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("platform_review_result", "平台审核结果");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("order_info", "订单信息");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("user_info", "用户信息");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("merchant_operate", "商户处理记录");
        AssertJsonProperty<ChannelsPlatformKfTicketItem>("platform_operate", "平台处理记录");

        // 嵌套对象（good_name/good_count 为官方历史拼写照录，勿纠正）。
        AssertJsonProperty<ChannelsPlatformKfTicketOrderInfo>("actual_price", "实付款（分）");
        AssertJsonProperty<ChannelsPlatformKfTicketOrderInfo>("goods_list", "商品列表");
        AssertJsonProperty<ChannelsPlatformKfTicketGoods>("good_name", "商品名称（官方历史拼写）");
        AssertJsonProperty<ChannelsPlatformKfTicketGoods>("good_count", "商品数量（官方历史拼写）");
        AssertJsonProperty<ChannelsPlatformKfTicketGoods>("thumb_img", "商品缩略图 URL");
        AssertJsonProperty<ChannelsPlatformKfTicketMerchantOperate>("merchant_reply", "商家回复选项 ID");
        AssertJsonProperty<ChannelsPlatformKfTicketMerchantOperate>("confer_time", "协商时间");
        AssertJsonProperty<ChannelsPlatformKfTicketPlatformOperate>("audit_result", "平台审核结果");
        AssertJsonProperty<ChannelsPlatformKfTicketPlatformOperate>("reply_description", "回复用户话术");

        // 工单详情与处理。
        AssertJsonProperty<ChannelsPlatformKfGetTicketDetailRequest>("ticket_id", "工单id");
        AssertJsonProperty<ChannelsPlatformKfGetTicketDetailResponse>("merchant_ticket", "工单详细信息");
        AssertJsonProperty<ChannelsPlatformKfOperateTicketRequest>("reply_option", "回复选项 ID");
        AssertJsonProperty<ChannelsPlatformKfOperateTicketRequest>("description", "补充说明文字");
        AssertJsonProperty<ChannelsPlatformKfOperateTicketRequest>("confer_time", "协商时间");

        // 共用 DTO 裁决：官方契约「getmchticketdetail 的 merchant_ticket 结构同工单列表中的
        // ticket_item_list 元素」⇒ 列表元素与详情共用 ChannelsPlatformKfTicketItem。
        typeof(ChannelsPlatformKfGetTicketListResponse).GetProperty(nameof(ChannelsPlatformKfGetTicketListResponse.TicketItemList))!
            .PropertyType.Should().Be(typeof(List<ChannelsPlatformKfTicketItem>));
        typeof(ChannelsPlatformKfGetTicketDetailResponse).GetProperty(nameof(ChannelsPlatformKfGetTicketDetailResponse.MerchantTicket))!
            .PropertyType.Should().Be(typeof(ChannelsPlatformKfTicketItem));

        // 共用 DTO 裁决：user_info（对象）与 merchant_operate.description 等处的「工单描述」语义虽近，
        // 但官方 user_info 仅 description 一字段、独立演化 ⇒ 单独建模；列表与详情天然同构共用条目 DTO。
    }

    /// <summary>契约守卫 CPK6：枚举常量字面量锁定。</summary>
    [Fact]
    public void PlatformKfEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsPlatformKfTicketStates.PendingMerchant.Should().Be(1);
        ChannelsPlatformKfTicketStates.PendingPlatform.Should().Be(2);
        ChannelsPlatformKfTicketStates.Closed.Should().Be(3);

        ChannelsPlatformKfTicketClassTypes.UrgeDelivery.Should().Be(9);

        ChannelsPlatformKfSortOrderTypes.ByCreateTime.Should().Be(1);
        ChannelsPlatformKfSortOrderTypes.ByDealEndTimeAsc.Should().Be(2);

        ChannelsPlatformKfReviewResults.ApprovedSupportUser.Should().Be(1);
        ChannelsPlatformKfReviewResults.RejectedDefaultSupportUser.Should().Be(2);
        ChannelsPlatformKfReviewResults.IntervenedSupportMerchant.Should().Be(3);
        ChannelsPlatformKfReviewResults.IntervenedSupportUser.Should().Be(4);
        ChannelsPlatformKfReviewResults.PlatformClosed.Should().Be(5);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
