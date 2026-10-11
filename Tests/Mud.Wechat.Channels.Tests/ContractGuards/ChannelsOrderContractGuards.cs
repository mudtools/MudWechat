// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels;
using Mud.Wechat.Channels.DataModels.Order;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「订单管理」域契约守卫（设计方案 v1 P1 / CO 系列：Order 域 27 端点路由 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域边界（设计方案 v1 §2.2）</b>：订单管理 = <c>/channels/ec/order/*</c> 24 条 +
/// <c>/channels/ec/merchant/privatenumber/*</c> 3 条 = 27 端点（全部 POST）。
/// 与 SHIPINHAO 本地生活重叠的 <c>order/get</c> 只计 1；<c>order/supplyorder/*</c>、<c>order/dropship*</c>、
/// <c>order/present*</c> 中归 Delivery / MiniStore 域的端点不落本域。
/// </para>
/// <para>
/// <b>收货信息脱敏语义（守卫措辞锁定）</b>：<c>sensitiveinfo/decode</c> 解密隐藏的昵称 / 电话 / 地址；
/// 虚拟号经 <c>virtualnumber/*</c> 申请与延期、<c>privatenumber/*</c> 实名认证；真实号经
/// <c>realnumber/apply</c> + <c>realnumberviewaudit/get</c> 审核。
/// </para>
/// </remarks>
public class ChannelsOrderContractGuards
{
    private const string OrderRegistryGroupName = "Order";

    /// <summary>Order 域官方路由表（27 端点，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] OrderRoutes =
    {
        // 订单主端点（10）
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetOrderAsync), typeof(PostAttribute), "/channels/ec/order/get"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetOrderListAsync), typeof(PostAttribute), "/channels/ec/order/list/get"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.SearchOrderAsync), typeof(PostAttribute), "/channels/ec/order/search"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.ChangeOrderPriceAsync), typeof(PostAttribute), "/channels/ec/order/price/update"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.ChangeOrderAddressAsync), typeof(PostAttribute), "/channels/ec/order/address/update"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.AcceptAddressModifyAsync), typeof(PostAttribute), "/channels/ec/order/addressmodify/accept"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.RejectAddressModifyAsync), typeof(PostAttribute), "/channels/ec/order/addressmodify/reject"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.UpdateMerchantNotesAsync), typeof(PostAttribute), "/channels/ec/order/merchantnotes/update"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.SetBlindBoxItemAsync), typeof(PostAttribute), "/channels/ec/order/blindboxitem/set"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.SubmitFreshInspectAsync), typeof(PostAttribute), "/channels/ec/order/freshinspect/submit"),
        // 礼物单 / 换款（5）
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.AddPresentNoteAsync), typeof(PostAttribute), "/channels/ec/order/presentnote/add"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetPresentSubOrderAsync), typeof(PostAttribute), "/channels/ec/order/presentsuborder/get"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetPreShipmentChangeSkuListAsync), typeof(PostAttribute), "/channels/ec/order/preshipmentchangesku/get"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.ApprovePreShipmentChangeSkuAsync), typeof(PostAttribute), "/channels/ec/order/preshipmentchangesku/approve"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.RejectPreShipmentChangeSkuAsync), typeof(PostAttribute), "/channels/ec/order/preshipmentchangesku/reject"),
        // 发货协商 / 物流变更 / 预约（4）
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.UpdateDeliveryInfoAsync), typeof(PostAttribute), "/channels/ec/order/deliveryinfo/update"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.SubmitDeliveryNegotiationAsync), typeof(PostAttribute), "/channels/ec/order/deliverynegotiation/submit"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetDeliveryNegotiationResultAsync), typeof(PostAttribute), "/channels/ec/order/deliverynegotiation/result/get"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetUserBookingListAsync), typeof(PostAttribute), "/channels/ec/order/userbooking/list"),
        // 敏感信息 / 虚拟号 / 真实号（5）
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.DecodeSensitiveInfoAsync), typeof(PostAttribute), "/channels/ec/order/sensitiveinfo/decode"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.ApplyRealNumberAsync), typeof(PostAttribute), "/channels/ec/order/realnumber/apply"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetRealNumberViewAuditAsync), typeof(PostAttribute), "/channels/ec/order/realnumberviewaudit/get"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.ApplyVirtualNumberAgainAsync), typeof(PostAttribute), "/channels/ec/order/virtualnumber/applyagain"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.DelayVirtualNumberAsync), typeof(PostAttribute), "/channels/ec/order/virtualnumber/delay"),
        // 商家私密号实名认证（3）
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.AddPrivatenumberPhoneAsync), typeof(PostAttribute), "/channels/ec/merchant/privatenumber/addphone"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.GetPrivatenumberPhoneAsync), typeof(PostAttribute), "/channels/ec/merchant/privatenumber/getphone"),
        (typeof(IChannelsOrderService), nameof(IChannelsOrderService.SendPrivatenumberVerifyCodeAsync), typeof(PostAttribute), "/channels/ec/merchant/privatenumber/sendverifycode"),
    };

    /// <summary>契约守卫 CO1：Order 域 27 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void OrderEndpoints_ShouldMatchOfficialRoutes()
    {
        // 27 端点 = /channels/ec/order/* 24 + /channels/ec/merchant/privatenumber/* 3（设计方案 v1 §2.2 计数口径）。
        OrderRoutes.Should().HaveCount(27, "Order 域恰 27 端点（订单管理 24 + 商家私密号 3）");
        OrderRoutes.Select(r => r.Route).Distinct().Should().HaveCount(27);

        // 前缀两分支。/channels/ec/order/* 24 条 + /channels/ec/merchant/privatenumber/* 3 条。
        OrderRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/order/", StringComparison.Ordinal))
            .Should().Be(24, "订单管理 24 条走 /channels/ec/order/* 前缀");
        OrderRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/merchant/privatenumber/", StringComparison.Ordinal))
            .Should().Be(3, "商家私密号 3 条走 /channels/ec/merchant/privatenumber/* 前缀");

        foreach (var (iface, method, httpAttribute, route) in OrderRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CO2：Order 域 27 端点全部为 POST，且关键命令端点必须携带 [Body]。</summary>
    [Fact]
    public void OrderEndpoints_ShouldAllBePost_WithBody()
    {
        foreach (var (iface, method, _, _) in OrderRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（订单管理无 GET 端点）");
        }

        // 全部 27 个端点都有请求体（无「传空 json 串」零载荷形态）。
        foreach (var (iface, method, _, _) in OrderRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().NotBeEmpty($"{iface.Name}.{method} 官方契约均有请求体，必须声明 [Body]");
        }
    }

    /// <summary>契约守卫 CO3：令牌绑定与注册形态。</summary>
    [Fact]
    public void OrderInterface_ShouldBindChannelsToken_AndRegisterUnderOrderGroup()
    {
        var token = typeof(IChannelsOrderService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Order 域 27 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsOrderService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(OrderRegistryGroupName, "必须挂 Order 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Order");
        typeof(ChannelsServiceBuilder).GetMethod("AddOrderApi").Should().NotBeNull("必须提供 AddOrderApi 链式注册方法");
    }

    /// <summary>契约守卫 CO4：Order 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void OrderDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = OrderJsonContext.Default;

        var domainTypes = typeof(ChannelsGetOrderRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Order"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(93,
            "Order 域 27 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（订单主文档 64 + 发货协商/预约 14 + 虚拟号/私密号 15 = 93，口径以 OrderJsonContext.g.cs 登记为准）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Order 域命名空间，必须登记进 OrderJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(OrderRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Order");
        }

        // 静态常量类（static class ⇔ IsAbstract && IsSealed）不得被误标 [HttpJsonSerializable]。
        var constantClasses = domainTypes.Where(static t => t.IsAbstract && t.IsSealed).ToList();
        constantClasses.Should().HaveCount(3,
            "Order 域静态常量类计数漂移须核对（订单状态 1 + 生鲜质检审核项名称 1 + 真实号审核状态 1 = 3）");
        constantClasses.Should().OnlyContain(
            static t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null,
            "常量类只有静态常量、无序列化语义，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CO5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void OrderDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 获取订单详情（顶层 order 结构 + product_infos 商品维度）。
        AssertJsonProperty<ChannelsGetOrderRequest>("order_id", "订单 ID（查询必填）");
        AssertJsonProperty<ChannelsGetOrderResponse>("order", "订单结构");
        AssertJsonProperty<ChannelsOrderInfo>("status", "订单状态");
        AssertJsonProperty<ChannelsOrderInfo>("is_present", "是否礼物订单");
        AssertJsonProperty<ChannelsOrderInfo>("present_order_id", "礼物 id（已废弃，照抄）");
        AssertJsonProperty<ChannelsOrderInfo>("product_infos", "商品列表");
        AssertJsonProperty<ChannelsOrderInfo>("pay_info", "支付信息");
        AssertJsonProperty<ChannelsOrderInfo>("price_info", "价格信息");
        AssertJsonProperty<ChannelsOrderInfo>("delivery_info", "配送信息");
        AssertJsonProperty<ChannelsOrderProductInfo>("product_id", "商品 id");
        AssertJsonProperty<ChannelsOrderProductInfo>("sku_cnt", "sku 数量");
        AssertJsonProperty<ChannelsOrderProductInfo>("is_change_price", "是否改价");
        AssertJsonProperty<ChannelsOrderProductInfo>("change_price", "改价后 sku 总价");
        AssertJsonProperty<ChannelsOrderProductInfo>("out_product_id", "商品外部 spu id");
        AssertJsonProperty<ChannelsOrderProductInfo>("product_unique_id", "商品常量编号");

        // 列表 / 搜索。
        AssertJsonProperty<ChannelsGetOrderListRequest>("create_time_range", "创建时间范围");
        AssertJsonProperty<ChannelsGetOrderListRequest>("next_key", "分页参数");
        AssertJsonProperty<ChannelsOrderTimeRange>("start_time", "开始时间");
        AssertJsonProperty<ChannelsOrderIdListResponse>("order_id_list", "订单号列表");
        AssertJsonProperty<ChannelsOrderIdListResponse>("has_more", "是否还有下一页");
        AssertJsonProperty<ChannelsSearchOrderRequest>("search_condition", "搜索条件");

        // 改价 / 改地址 / 改备注。
        AssertJsonProperty<ChannelsOrderPriceChangeInfo>("change_price", "修改后的商品实付价格");
        AssertJsonProperty<ChannelsChangeOrderPriceRequest>("change_express", "是否修改运费");
        AssertJsonProperty<ChannelsChangeOrderPriceRequest>("express_fee", "修改后的运费价格");
        AssertJsonProperty<ChannelsChangeOrderAddressRequest>("user_address", "新地址");
        AssertJsonProperty<ChannelsOrderAddress>("virtual_order_tel_number", "虚拟商品订单联系方式");
        AssertJsonProperty<ChannelsUpdateMerchantNotesRequest>("merchant_notes", "备注内容");
        AssertJsonProperty<ChannelsUpdateMerchantNotesRequest>("merchant_notes_tag_color", "备注标签颜色");

        // 礼物单 / 换款 / 盲盒。
        AssertJsonProperty<ChannelsAddPresentNoteRequest>("notes", "礼物订单备注信息");
        AssertJsonProperty<ChannelsGetPresentSubOrderResponse>("order_ids", "子单订单号列表");
        AssertJsonProperty<ChannelsGetPreShipmentChangeSkuRequest>("page_size", "每页条数");
        AssertJsonProperty<ChannelsApprovePreShipmentChangeSkuRequest>("order_id", "订单 id（审批类共用）");
        AssertJsonProperty<ChannelsBlindBoxItem>("blind_box_item_id", "盲盒款式 id");
        AssertJsonProperty<ChannelsSetBlindBoxItemResponse>("fail_item_list", "设置失败的盲盒款式列表");

        // 发货协商 / 物流变更 / 预约。
        AssertJsonProperty<ChannelsSubmitDeliveryNegotiationRequest>("predict_delivery_time", "预计发货时间");
        AssertJsonProperty<ChannelsGetDeliveryNegotiationResultRequest>("order_id_list", "订单 ID 数组");
        AssertJsonProperty<ChannelsOrderNegotiation>("original_delivery_time", "原发货时间");
        AssertJsonProperty<ChannelsUpdateDeliveryInfoRequest>("delivery_list", "整单物流信息");
        AssertJsonProperty<ChannelsUpdateDeliveryInfoRequest>("change_infos", "更新包裹物流信息");
        AssertJsonProperty<ChannelsOrderDeliveryInfoPair>("waybill_id", "快递单号");
        AssertJsonProperty<ChannelsUserBookingRecord>("booking_type", "预约类型");
        AssertJsonProperty<ChannelsGetUserBookingListResponse>("next_page", "下一页信息");

        // 生鲜质检 / 敏感信息 / 虚拟号 / 真实号 / 私密号。
        AssertJsonProperty<ChannelsFreshInspectAuditItem>("item_name", "审核项名称");
        AssertJsonProperty<ChannelsDecodeSensitiveInfoResponse>("address_info", "收货信息");
        AssertJsonProperty<ChannelsOrderVirtualNumberInfo>("virtual_number", "虚拟号");
        AssertJsonProperty<ChannelsDelayVirtualNumberResponse>("available_extend_num", "可延期次数");
        AssertJsonProperty<ChannelsApplyRealNumberRequest>("pic_media_ids", "申请原因图片证明");
        AssertJsonProperty<ChannelsGetRealNumberViewAuditResponse>("audit_state", "审核状态");
        AssertJsonProperty<ChannelsPrivatenumberAddPhoneRequest>("wxusername", "小店成员的微信号");
        AssertJsonProperty<ChannelsPrivatenumberAddPhoneResponse>("qrcode_url", "认证链接");
        AssertJsonProperty<ChannelsPrivatenumberSendVerifyCodeRequest>("mobile", "需要实名认证的手机号");

        // 共用 DTO 裁决（严禁同字段重复建模）。
        // (1) 单订单查询共用：merchantnotes/update 之外，approve/reject 换款共用 order_id 请求形态。
        // (2) order/list/get 与 order/search 共用 ChannelsOrderIdListResponse 响应形态（继承锁定）。
        typeof(ChannelsSearchOrderResponse).IsAssignableTo(typeof(ChannelsOrderIdListResponse))
            .Should().BeTrue("order/search 响应在订单号列表形态上扩展，必须继承 ChannelsOrderIdListResponse");
        // (3) sensitiveinfo/decode 的 address_info 与 address/update 的 user_address 共用字段集。
        typeof(ChannelsOrderAddress).GetProperty(nameof(ChannelsOrderAddress.TelNumber))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("tel_number");
    }

    /// <summary>契约守卫 CO6：Order 域枚举常量漂移锁定（订单状态 / 生鲜质检项名 / 真实号审核状态）。</summary>
    [Fact]
    public void OrderEnums_ShouldLockOfficialValues()
    {
        // 订单状态。
        typeof(ChannelsOrderStatuses).GetField(nameof(ChannelsOrderStatuses.PendingPayment))!.GetRawConstantValue()
            .Should().Be(10, "待付款");
        typeof(ChannelsOrderStatuses).GetField(nameof(ChannelsOrderStatuses.PendingDelivery))!.GetRawConstantValue()
            .Should().Be(20, "待发货");
        typeof(ChannelsOrderStatuses).GetField(nameof(ChannelsOrderStatuses.PendingReceipt))!.GetRawConstantValue()
            .Should().Be(30, "待收货");
        typeof(ChannelsOrderStatuses).GetField(nameof(ChannelsOrderStatuses.Completed))!.GetRawConstantValue()
            .Should().Be(100, "完成");

        // 生鲜质检审核项名称。
        typeof(ChannelsFreshInspectItemNames)
            .GetField(nameof(ChannelsFreshInspectItemNames.ProductExpressPicUrl))!.GetRawConstantValue()
            .Should().Be("product_express_pic_url", "商品快递单图片 url");

        // 真实号审核状态。
        typeof(ChannelsRealNumberAuditStates).GetField(nameof(ChannelsRealNumberAuditStates.Auditing))!.GetRawConstantValue()
            .Should().Be(1, "审核中");
        typeof(ChannelsRealNumberAuditStates).GetField(nameof(ChannelsRealNumberAuditStates.Approved))!.GetRawConstantValue()
            .Should().Be(3, "审核通过");
    }

    /// <summary>断言类型的 JSON 字段名（官方参数名照抄）。</summary>
    private static void AssertJsonProperty<T>(string expected, string description)
    {
        var propertyNames = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        propertyNames.Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Should().Contain(expected, $"{typeof(T).Name} 必须按官方参数名建模 {description}（{expected}）");
    }
}