// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Delivery;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「代发与供货」域契约守卫（CD 系列：Delivery 域 16 端点（代发管理 13 + 供货管理 3）路由 +
/// 三前缀分布 + 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量字面量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 Order 域的边界（路由互斥，守卫措辞锁定）</b>：本域的 <c>/channels/ec/order/dropship/*</c> 与
/// <c>/channels/ec/order/supplyorder/*</c> 是<b>代发链路</b>端点（分配 / 取消 / 查代发单 / 查供货单）；
/// Order 域的 <c>order/deliveryinfo</c> / <c>order/deliverynegotiation</c> 是<b>发货履约</b>端点。
/// 两域在 <c>/channels/ec/order/*</c> 下无路由交叠，改动任一侧须核对另一侧路由表。
/// </para>
/// <para>
/// <b>三前缀并存（官方目录形态，照抄原文禁止改写）</b>：供货商关联与自动分配 8 端点走
/// <c>/channels/ec/supplier/relation/*</c>，代发单 5 端点走 <c>/channels/ec/order/dropship/*</c>，
/// 供货单 3 端点走 <c>/channels/ec/order/supplyorder/*</c>。
/// </para>
/// <para>
/// <b>官方文档矛盾照录（SDK 一律按参数表建模）</b>：dropship/list 参数表 <c>order_id</c> 为 number
/// 而示例为字符串；dropship/cancel 示例含表外 <c>supplier_appid</c>；get_product_list 示例含表外
/// <c>is_filter_product_status</c>/<c>product_status</c>；supplyorder/search 示例含表外
/// <c>tel_last4</c>/<c>user_name</c>；supplyorder/list 变更日志声明 <c>shop_appid</c> 已删除改
/// <c>supplier_appid</c>。
/// </para>
/// </remarks>
public class ChannelsDeliveryContractGuards
{
    private const string DeliveryRegistryGroupName = "Delivery";

    /// <summary>Delivery 域官方路由表（16 端点：supplier/relation 8 + order/dropship 5 + order/supplyorder 3，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] DeliveryRoutes =
    {
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetSupplierListAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/get_supplier_list"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.InviteSupplierAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/invite_supplier"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetDistributeAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/get_distribute"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.SetManuallyDistributeAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/set_manually_distribute"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.SetAllDistributionAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/set_all_distribution"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.SetProductDistributeAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/set_product_distribute"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetProductDefaultDistributeAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/get_product_default_distribute"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetProductListAsync), typeof(PostAttribute), "/channels/ec/supplier/relation/get_product_list"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.AssignDropshipAsync), typeof(PostAttribute), "/channels/ec/order/dropship/assign"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.CancelDropshipAsync), typeof(PostAttribute), "/channels/ec/order/dropship/cancel"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetDropshipOrderAsync), typeof(PostAttribute), "/channels/ec/order/dropship/get"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetDropshipOrderListAsync), typeof(PostAttribute), "/channels/ec/order/dropship/list"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.SearchDropshipOrdersAsync), typeof(PostAttribute), "/channels/ec/order/dropship/search"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetSupplyOrderListAsync), typeof(PostAttribute), "/channels/ec/order/supplyorder/list"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.GetSupplyOrderAsync), typeof(PostAttribute), "/channels/ec/order/supplyorder/get"),
        (typeof(IChannelsDeliveryService), nameof(IChannelsDeliveryService.SearchSupplyOrdersAsync), typeof(PostAttribute), "/channels/ec/order/supplyorder/search"),
    };

    /// <summary>契约守卫 CD1：Delivery 域 16 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void DeliveryEndpoints_ShouldMatchOfficialRoutes()
    {
        // 16 端点 = 供货商关系 2 + 自动分配 6 + 代发单 5 + 供货单 3（设计方案 v1 计数口径：代发管理 13 + 供货管理 3）。
        DeliveryRoutes.Should().HaveCount(16, "代发管理 13 + 供货管理 3 = 16 端点");
        DeliveryRoutes.Select(r => r.Route).Distinct().Should().HaveCount(16);

        DeliveryRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/supplier/relation/", StringComparison.Ordinal))
            .Should().Be(8, "供货商关联 2 + 自动分配 6 端点走 /channels/ec/supplier/relation/* 前缀");
        DeliveryRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/order/dropship/", StringComparison.Ordinal))
            .Should().Be(5, "代发单管理 5 端点走 /channels/ec/order/dropship/* 前缀");
        DeliveryRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/order/supplyorder/", StringComparison.Ordinal))
            .Should().Be(3, "供货单管理 3 端点走 /channels/ec/order/supplyorder/* 前缀");

        // 与 Order 域路由互斥：本域不得出现 order/deliveryinfo / order/deliverynegotiation 等履约端点。
        DeliveryRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/order/", StringComparison.Ordinal) && !r.StartsWith("/channels/ec/order/dropship/", StringComparison.Ordinal) && !r.StartsWith("/channels/ec/order/supplyorder/", StringComparison.Ordinal))
            .Should().Be(0, "本域在 /channels/ec/order/* 下只允许 dropship/* 与 supplyorder/*，履约端点归 Order 域");

        foreach (var (iface, method, httpAttribute, route) in DeliveryRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CD2：Delivery 域 16 端点全部为 POST；空请求体端点不得携带 [Body]，其余端点必须携带。</summary>
    [Fact]
    public void DeliveryEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in DeliveryRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（代发与供货域无 GET 端点）");
        }

        // 官方参数表无 Body 参数的两端点（get_distribute / set_manually_distribute）不得人为造请求体 DTO。
        var emptyBodyMethods = new[]
        {
            nameof(IChannelsDeliveryService.GetDistributeAsync),
            nameof(IChannelsDeliveryService.SetManuallyDistributeAsync),
        };

        foreach (var (iface, method, _, _) in DeliveryRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            var bodyParams = target!.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>()).ToList();

            if (emptyBodyMethods.Contains(method))
            {
                bodyParams.Should().BeEmpty($"{method} 官方参数表无请求体参数——不得声明 [Body]");
            }
            else
            {
                bodyParams.Should().NotBeEmpty($"{method} 官方契约为 POST JSON 请求体——必须声明 [Body]");
            }
        }
    }

    /// <summary>契约守卫 CD3：令牌绑定与注册形态。</summary>
    [Fact]
    public void DeliveryInterface_ShouldBindChannelsToken_AndRegisterUnderDeliveryGroup()
    {
        var token = typeof(IChannelsDeliveryService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Delivery 域 16 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsDeliveryService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(DeliveryRegistryGroupName, "必须挂 Delivery 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Delivery");
        typeof(ChannelsServiceBuilder).GetMethod("AddDeliveryApi").Should().NotBeNull("必须提供 AddDeliveryApi 链式注册方法");
    }

    /// <summary>契约守卫 CD4：Delivery 域 DTO 全量登记进 AOT JSON 上下文 + 常量类计数（未标序列化特性）。</summary>
    /// <remarks>
    /// 常量类（<c>IsAbstract &amp;&amp; IsSealed</c>）必须从<b>不过滤 abstract</b> 的 domainTypes 里取——
    /// 若在第一层过滤 <c>!t.IsAbstract</c>，静态常量类会被上游滤掉导致常量类断言恒 0（前序批次踩坑）。
    /// </remarks>
    [Fact]
    public void DeliveryDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = DeliveryJsonContext.Default;

        var domainTypes = typeof(ChannelsGetSupplierListResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Delivery"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(50,
            "Delivery 域 16 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（供货商关系 13：get_supplier_list 3 + invite_supplier 1 + get_distribute 1 + set_all_distribution 1 + " +
            "set_product_distribute 2 + get_product_default_distribute 2 + get_product_list 3；" +
            "代发单 24：assign 2 + cancel 1 + get 6 + 代发/供货共用发货嵌套 9 + 分页时间共用 2 + list 1 + search 2 + 列表响应 1；" +
            "供货单 13：list 1 + search 2 + 列表响应 1 + get 9）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Delivery 域命名空间，必须登记进 DeliveryJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(DeliveryRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Delivery");
        }

        // 常量类断言：从不过滤的 domainTypes 里取 IsAbstract && IsSealed（静态常量类编译形态）。
        var constantTypes = domainTypes
            .Where(t => t.IsAbstract && t.IsSealed)
            .ToList();

        constantTypes.Should().HaveCount(13,
            "Delivery 域官方枚举常量类计数漂移须先核对官方文档再同批调整本守卫" +
            "（供货商侧 4 + 代发单侧 6 + 供货单侧 3 = 13）");

        foreach (var type in constantTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>().Should().BeNull(
                $"{type.Name} 是常量类，不得标注 [HttpJsonSerializable]");
        }
    }

    /// <summary>契约守卫 CD5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void DeliveryDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 供货商关系。
        AssertJsonProperty<ChannelsGetSupplierListRequest>("offset", "偏移量");
        AssertJsonProperty<ChannelsGetSupplierListRequest>("limit", "单页数量");
        AssertJsonProperty<ChannelsGetSupplierListResponse>("list", "供货商列表");
        AssertJsonProperty<ChannelsGetSupplierListResponse>("total_count", "列表总数");
        AssertJsonProperty<ChannelsSupplier>("name", "供货商名称");
        AssertJsonProperty<ChannelsSupplier>("appid", "供货商appid");
        AssertJsonProperty<ChannelsSupplier>("bind_audit_status", "绑定审核状态");
        AssertJsonProperty<ChannelsInviteSupplierRequest>("supplier_appid", "申请关联的供货商的appid");
        AssertJsonProperty<ChannelsInviteSupplierRequest>("service_id", "第三方服务商ServiceID");
        AssertJsonProperty<ChannelsInviteSupplierRequest>("remark", "第三方服务商备注信息");

        // 自动分配。
        AssertJsonProperty<ChannelsGetDistributeResponse>("distribute_type", "分配类型");
        AssertJsonProperty<ChannelsGetDistributeResponse>("all_supplier_appid", "全店订单自动分配的供货商appid");
        AssertJsonProperty<ChannelsGetDistributeResponse>("all_distribute_supplier_name", "全店订单自动分配的供货商名称");
        AssertJsonProperty<ChannelsSetAllDistributionRequest>("supplier_appid", "全店订单自动分配供货商appid");
        AssertJsonProperty<ChannelsProductDistributeItem>("is_distribute", "该商品是否自动分配给该供货商");
        AssertJsonProperty<ChannelsProductDistributeItem>("apply_to_gift", "主赠品同步模式");
        AssertJsonProperty<ChannelsSetProductDistributeRequest>("list", "设置列表");
        AssertJsonProperty<ChannelsGetProductDefaultDistributeRequest>("product_id", "商品id");
        AssertJsonProperty<ChannelsGetProductDefaultDistributeResponse>("exist_default_supplier", "是否存在已设置自动分配的供货商");
        AssertJsonProperty<ChannelsGetProductListRequest>("is_filter_distribute_status", "是否过滤distribute_status");
        AssertJsonProperty<ChannelsGetProductListRequest>("page_context", "偏移量（分页用）");
        AssertJsonProperty<ChannelsProductDistributeStatus>("distribute_status", "分配状态");
        AssertJsonProperty<ChannelsGetProductListResponse>("page_context", "偏移量（分页用）");

        // 代发单（assign / cancel / get）。
        AssertJsonProperty<ChannelsDropshipProduct>("spu_id", "商品spuid");
        AssertJsonProperty<ChannelsDropshipProduct>("sku_id", "商品skuid");
        AssertJsonProperty<ChannelsDropshipProduct>("product_cnt", "数量");
        AssertJsonProperty<ChannelsDropshipAssignRequest>("dropship_product_list", "代发商品信息");
        AssertJsonProperty<ChannelsDropshipCancelRequest>("ds_order_id", "代发单号");
        AssertJsonProperty<ChannelsDropshipGetRequest>("ds_order_id", "代发单号");
        AssertJsonProperty<ChannelsDropshipGetResponse>("dropship_order", "代发单结构");
        AssertJsonProperty<ChannelsDropshipOrder>("ds_order_id", "代发单号（响应侧官方类型 number）");
        AssertJsonProperty<ChannelsDropshipOrder>("dropship_order_detail", "代发单详情");
        AssertJsonProperty<ChannelsDropshipProductInfo>("sku_deliver_info", "商品发货信息");
        AssertJsonProperty<ChannelsDropshipProductInfo>("product_unique_id", "商品常量编号");
        AssertJsonProperty<ChannelsDropshipProductInfo>("dropship_info", "代发相关信息");
        AssertJsonProperty<ChannelsDropshipInfo>("delivery_status", "发货状态");
        AssertJsonProperty<ChannelsDropshipInfo>("cancel_scene", "取消原因");

        // 代发单（list / search）。
        AssertJsonProperty<ChannelsDropshipListRequest>("order_id", "订单号（参数表类型 number，示例为字符串——照录）");
        AssertJsonProperty<ChannelsDropshipListRequest>("update_time_range", "代发单更新时间范围");
        AssertJsonProperty<ChannelsDropshipSearchRequest>("search_condition", "搜索条件");
        AssertJsonProperty<ChannelsDropshipSearchCondition>("waybill_id", "运单号（只可独立查询）");
        AssertJsonProperty<ChannelsDropshipSearchCondition>("order_create_time_range", "订单下单时间范围");
        AssertJsonProperty<ChannelsDropshipSearchCondition>("supplier_appname", "供货商名称");
        AssertJsonProperty<ChannelsDropshipOrderListResponse>("ds_order_id_list", "代发单号列表");
        AssertJsonProperty<ChannelsDropshipOrderListResponse>("next_page", "分页参数");
        AssertJsonProperty<ChannelsDropshipOrderListResponse>("has_more", "是否还有下一页");

        // 供货单。
        AssertJsonProperty<ChannelsSupplyOrderListRequest>("supplier_appid", "供货商appid（2026-05-25 替换已删除的 shop_appid）");
        AssertJsonProperty<ChannelsSupplyOrderSearchRequest>("search_condition", "搜索条件");
        AssertJsonProperty<ChannelsSupplyOrderSearchCondition>("merchant_notes", "商家备注");
        AssertJsonProperty<ChannelsSupplyOrderListResponse>("supply_order_id_list", "供货单号列表");
        AssertJsonProperty<ChannelsSupplyOrderGetRequest>("supply_order_id", "供货单号");
        AssertJsonProperty<ChannelsSupplyOrderGetResponse>("supply_order", "供货单结构");
        AssertJsonProperty<ChannelsSupplyOrder>("supply_order_id", "供货单号");
        AssertJsonProperty<ChannelsSupplyOrderDetail>("price_info", "价格信息");
        AssertJsonProperty<ChannelsSupplyOrderDetail>("freight", "运费");
        AssertJsonProperty<ChannelsSupplyProductInfo>("sale_price", "售价（单价，分）");
        AssertJsonProperty<ChannelsSupplyProductInfo>("sku_attrs", "sku属性/款式信息");
        AssertJsonProperty<ChannelsSupplyProductInfo>("supply_order_product_info", "供货信息");
        AssertJsonProperty<ChannelsSkuAttr>("attr_key", "sku属性/款式类型");
        AssertJsonProperty<ChannelsSupplyOrderProductInfo>("supply_price", "供货价（单价，分）");
        AssertJsonProperty<ChannelsSupplyPriceInfo>("total_price", "供货单总价（分）");
        AssertJsonProperty<ChannelsSupplyAftersaleOrder>("aftersale_order_id", "售后单号");

        // 代发/供货共用发货嵌套（dropship/get 与 supplyorder/get 共用）。
        AssertJsonProperty<ChannelsSkuDeliverInfo>("stock_type", "商品发货类型");
        AssertJsonProperty<ChannelsSkuDeliverInfo>("full_payment_presale_delivery_type", "预售类型");
        AssertJsonProperty<ChannelsDeliveryInfo>("delivery_product_info", "发货物流信息");
        AssertJsonProperty<ChannelsDeliveryInfo>("ship_done_time", "发货完成时间");
        AssertJsonProperty<ChannelsDeliveryAddress>("virtual_order_tel_number", "虚拟发货订单联系方式");
        AssertJsonProperty<ChannelsDeliveryAddress>("hash_code", "用户收货地址唯一标识");
        AssertJsonProperty<ChannelsTelNumberExtInfo>("real_tel_number", "脱敏手机号");
        AssertJsonProperty<ChannelsDeliveryPackage>("waybill_id", "快递单号");
        AssertJsonProperty<ChannelsDeliveryPackage>("delivery_address", "发货地址（官方原文「字段与 address_info 相同」）");
        AssertJsonProperty<ChannelsPackageProduct>("product_cnt", "商品数量");
        AssertJsonProperty<ChannelsOrderExtInfo>("present_notes", "礼物备注");
        AssertJsonProperty<ChannelsGreetingCardInfo>("greeting_message", "贺卡内容");
        AssertJsonProperty<ChannelsCustomInfo>("custom_type", "定制类型");

        // 共用 DTO 裁决 ①：assign 与 cancel 的 dropship_product_list 元素字段集一致 ⇒ 共用 ChannelsDropshipProduct。
        typeof(ChannelsDropshipAssignRequest).GetProperty(nameof(ChannelsDropshipAssignRequest.DropshipProductList))!
            .PropertyType.Should().Be(typeof(List<ChannelsDropshipProduct>));
        typeof(ChannelsDropshipCancelRequest).GetProperty(nameof(ChannelsDropshipCancelRequest.DropshipProductList))!
            .PropertyType.Should().Be(typeof(List<ChannelsDropshipProduct>));

        // 共用 DTO 裁决 ②：四个拉单/搜单端点的时间范围对象字段集一致（start_time/end_time）⇒ 共用 ChannelsDeliveryTimeRange。
        typeof(ChannelsDropshipListRequest).GetProperty(nameof(ChannelsDropshipListRequest.CreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryTimeRange));
        typeof(ChannelsDropshipListRequest).GetProperty(nameof(ChannelsDropshipListRequest.UpdateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryTimeRange));
        typeof(ChannelsDropshipSearchCondition).GetProperty(nameof(ChannelsDropshipSearchCondition.OrderCreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryTimeRange));
        typeof(ChannelsSupplyOrderListRequest).GetProperty(nameof(ChannelsSupplyOrderListRequest.CreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryTimeRange));
        typeof(ChannelsSupplyOrderSearchCondition).GetProperty(nameof(ChannelsSupplyOrderSearchCondition.OrderCreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryTimeRange));

        // 共用 DTO 裁决 ③：请求 page_info 与响应 next_page 字段集一致（offset/limit/next_key）⇒ 共用 ChannelsDeliveryPageInfo。
        typeof(ChannelsDropshipListRequest).GetProperty(nameof(ChannelsDropshipListRequest.PageInfo))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryPageInfo));
        typeof(ChannelsDropshipSearchRequest).GetProperty(nameof(ChannelsDropshipSearchRequest.PageInfo))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryPageInfo));
        typeof(ChannelsSupplyOrderListRequest).GetProperty(nameof(ChannelsSupplyOrderListRequest.PageInfo))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryPageInfo));
        typeof(ChannelsSupplyOrderSearchRequest).GetProperty(nameof(ChannelsSupplyOrderSearchRequest.PageInfo))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryPageInfo));
        typeof(ChannelsDropshipOrderListResponse).GetProperty(nameof(ChannelsDropshipOrderListResponse.NextPage))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryPageInfo));
        typeof(ChannelsSupplyOrderListResponse).GetProperty(nameof(ChannelsSupplyOrderListResponse.NextPage))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryPageInfo));

        // 共用 DTO 裁决 ④：dropship/list 与 dropship/search 响应字段集一致 ⇒ 共用 ChannelsDropshipOrderListResponse。
        typeof(IChannelsDeliveryService).GetMethod(nameof(IChannelsDeliveryService.GetDropshipOrderListAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsDropshipOrderListResponse>));
        typeof(IChannelsDeliveryService).GetMethod(nameof(IChannelsDeliveryService.SearchDropshipOrdersAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsDropshipOrderListResponse>));

        // 共用 DTO 裁决 ⑤：supplyorder/list 与 supplyorder/search 响应字段集一致 ⇒ 共用 ChannelsSupplyOrderListResponse。
        typeof(IChannelsDeliveryService).GetMethod(nameof(IChannelsDeliveryService.GetSupplyOrderListAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsSupplyOrderListResponse>));
        typeof(IChannelsDeliveryService).GetMethod(nameof(IChannelsDeliveryService.SearchSupplyOrdersAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsSupplyOrderListResponse>));

        // 共用 DTO 裁决 ⑥：dropship/get 与 supplyorder/get 的发货嵌套结构一致 ⇒ 共用 9 个发货相关 DTO。
        typeof(ChannelsDropshipOrderDetail).GetProperty(nameof(ChannelsDropshipOrderDetail.DeliveryInfo))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryInfo));
        typeof(ChannelsSupplyOrderDetail).GetProperty(nameof(ChannelsSupplyOrderDetail.DeliveryInfo))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryInfo));
        typeof(ChannelsDropshipProductInfo).GetProperty(nameof(ChannelsDropshipProductInfo.SkuDeliverInfo))!
            .PropertyType.Should().Be(typeof(ChannelsSkuDeliverInfo));
        typeof(ChannelsSupplyProductInfo).GetProperty(nameof(ChannelsSupplyProductInfo.SkuDeliverInfo))!
            .PropertyType.Should().Be(typeof(ChannelsSkuDeliverInfo));
        typeof(ChannelsDeliveryInfo).GetProperty(nameof(ChannelsDeliveryInfo.AddressInfo))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryAddress));
        typeof(ChannelsDeliveryPackage).GetProperty(nameof(ChannelsDeliveryPackage.DeliveryAddress))!
            .PropertyType.Should().Be(typeof(ChannelsDeliveryAddress));
        typeof(ChannelsDropshipOrderDetail).GetProperty(nameof(ChannelsDropshipOrderDetail.ExtInfo))!
            .PropertyType.Should().Be(typeof(ChannelsOrderExtInfo));
        typeof(ChannelsSupplyOrderDetail).GetProperty(nameof(ChannelsSupplyOrderDetail.ExtInfo))!
            .PropertyType.Should().Be(typeof(ChannelsOrderExtInfo));
        typeof(ChannelsDropshipOrderDetail).GetProperty(nameof(ChannelsDropshipOrderDetail.GreetingCardInfo))!
            .PropertyType.Should().Be(typeof(ChannelsGreetingCardInfo));
        typeof(ChannelsDropshipOrderDetail).GetProperty(nameof(ChannelsDropshipOrderDetail.CustomInfo))!
            .PropertyType.Should().Be(typeof(ChannelsCustomInfo));
        typeof(ChannelsSupplyOrderDetail).GetProperty(nameof(ChannelsSupplyOrderDetail.CustomInfo))!
            .PropertyType.Should().Be(typeof(ChannelsCustomInfo));

        // 共用裁决（负向）①：代发与供货的搜索条件字段集当前一致，但供货侧官方示例已出现表外字段（tel_last4/user_name）
        // 预告两形态可独立演化 ⇒ 分建 ChannelsDropshipSearchCondition / ChannelsSupplyOrderSearchCondition，不得共用。
        typeof(ChannelsDropshipSearchRequest).GetProperty(nameof(ChannelsDropshipSearchRequest.SearchCondition))!
            .PropertyType.Should().Be(typeof(ChannelsDropshipSearchCondition));
        typeof(ChannelsSupplyOrderSearchRequest).GetProperty(nameof(ChannelsSupplyOrderSearchRequest.SearchCondition))!
            .PropertyType.Should().Be(typeof(ChannelsSupplyOrderSearchCondition));
        typeof(ChannelsDropshipSearchRequest).GetProperty(nameof(ChannelsDropshipSearchRequest.SearchCondition))!
            .PropertyType.Should().NotBe(typeof(ChannelsSupplyOrderSearchCondition), "搜索条件按端点分建（供货侧示例已出现表外字段预告演化）");
    }

    /// <summary>契约守卫 CD6：枚举常量字面量锁定（覆盖「同值不同义防串线」——代发/供货 cancel_scene 语义相反）。</summary>
    [Fact]
    public void DeliveryEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsSupplierStatuses.Init.Should().Be(0);
        ChannelsSupplierStatuses.Bound.Should().Be(1);
        ChannelsSupplierStatuses.Unbound.Should().Be(2);

        ChannelsSupplierBindAuditStatuses.Pending.Should().Be(0);
        ChannelsSupplierBindAuditStatuses.Accepted.Should().Be(1);
        ChannelsSupplierBindAuditStatuses.Rejected.Should().Be(2);
        ChannelsSupplierBindAuditStatuses.TimeoutRejected.Should().Be(3);
        ChannelsSupplierBindAuditStatuses.CanceledByShop.Should().Be(4);
        ChannelsSupplierBindAuditStatuses.Init.Should().Be(5);

        ChannelsDistributeTypes.Manual.Should().Be(1);
        ChannelsDistributeTypes.AllAuto.Should().Be(2);
        ChannelsDistributeTypes.ProductAuto.Should().Be(3);

        ChannelsProductDistributeStatuses.Distributed.Should().Be(0);
        ChannelsProductDistributeStatuses.NotDistributed.Should().Be(1);

        ChannelsDropshipOrderStatuses.PendingDelivery.Should().Be(100);
        ChannelsDropshipOrderStatuses.PartialDelivered.Should().Be(199);
        ChannelsDropshipOrderStatuses.Delivered.Should().Be(200);
        ChannelsDropshipOrderStatuses.Canceled.Should().Be(300);

        ChannelsDeliveryStatuses.PendingDelivery.Should().Be(10);
        ChannelsDeliveryStatuses.PartialDelivered.Should().Be(19);
        ChannelsDeliveryStatuses.Delivered.Should().Be(20);
        ChannelsDeliveryStatuses.Canceled.Should().Be(30);

        // 同值不同义：代发侧 cancel_scene 1=售后 2=商家取消代发；供货侧 1=用户售后 2=平台砍单——两常量类不得合并。
        ChannelsDropshipCancelScenes.AfterSale.Should().Be(1);
        ChannelsDropshipCancelScenes.MerchantCancel.Should().Be(2);
        ChannelsSupplyCancelScenes.UserAfterSale.Should().Be(1);
        ChannelsSupplyCancelScenes.PlatformCutOrder.Should().Be(2);

        ChannelsSupplyOrderStatuses.PendingDelivery.Should().Be(100);
        ChannelsSupplyOrderStatuses.PartialDelivered.Should().Be(199);
        ChannelsSupplyOrderStatuses.Delivered.Should().Be(200);
        ChannelsSupplyOrderStatuses.Canceled.Should().Be(300);
        ChannelsSupplyOrderStatuses.Completed.Should().Be(400);

        ChannelsSupplyAftersaleStatuses.NoneOrClosed.Should().Be(0);
        ChannelsSupplyAftersaleStatuses.InAfterSale.Should().Be(1);
        ChannelsSupplyAftersaleStatuses.Finished.Should().Be(2);

        ChannelsStockTypes.InStock.Should().Be(0);
        ChannelsStockTypes.FullPaymentPresale.Should().Be(1);

        ChannelsPresaleDeliveryTypes.AfterPaymentDays.Should().Be(0);
        ChannelsPresaleDeliveryTypes.AfterPresaleEndDays.Should().Be(1);

        ChannelsCustomTypes.Word.Should().Be(1);
        ChannelsCustomTypes.Image.Should().Be(2);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
