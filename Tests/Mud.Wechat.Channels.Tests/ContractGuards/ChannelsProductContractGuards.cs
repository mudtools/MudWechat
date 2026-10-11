// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Product;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「商品管理」域契约守卫（设计方案 v1 P1 / CP 系列：Product 域 43 端点路由 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域边界（设计方案 v1 §6 P1 口径）</b>：商品管理 = XIAODIAN「商品管理」43 端点（含与 SHIPINHAO
/// 本地生活重叠的 5 个同路由端点 audit/cancel、delete、listing、delisting、stock/update 只计 1）；
/// <c>/channels/ec/product/locallife/*</c> 9 条属 Window 域、<c>/channels/ec/product/taglink/get</c> 等
/// 链接面端点归本域。路由面由 CH-R2 锁定（全 <c>/channels/ec/product/*</c>，无 <c>/shop/*</c> 历史前缀）。
/// </para>
/// <para>
/// <b>草稿 / 线上双份数据语义（守卫措辞锁定）</b>：<c>product/add</c> 只影响草稿，上架 + 审核通过后
/// 才覆盖线上；<c>product/get</c> 以 <c>data_type</c> 分流；<c>product/auditfree</c> 仅对曾经上架成功过的
/// 商品适用。
/// </para>
/// </remarks>
public class ChannelsProductContractGuards
{
    private const string ProductRegistryGroupName = "Product";

    /// <summary>Product 域官方路由表（43 端点，全部 POST，前缀 <c>/channels/ec/product/</c>）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ProductRoutes =
    {
        // 商品主端点（25）
        (typeof(IChannelsProductService), nameof(IChannelsProductService.AddProductAsync), typeof(PostAttribute), "/channels/ec/product/add"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.UpdateProductAsync), typeof(PostAttribute), "/channels/ec/product/update"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductAsync), typeof(PostAttribute), "/channels/ec/product/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductListAsync), typeof(PostAttribute), "/channels/ec/product/list/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.ListingProductAsync), typeof(PostAttribute), "/channels/ec/product/listing"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.DelistingProductAsync), typeof(PostAttribute), "/channels/ec/product/delisting"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.DeleteProductAsync), typeof(PostAttribute), "/channels/ec/product/delete"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.CancelProductAuditAsync), typeof(PostAttribute), "/channels/ec/product/audit/cancel"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.UpdateProductAuditFreeAsync), typeof(PostAttribute), "/channels/ec/product/auditfree"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductAuditStrategyAsync), typeof(PostAttribute), "/channels/ec/product/auditstrategy/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.SetProductAuditStrategyAsync), typeof(PostAttribute), "/channels/ec/product/auditstrategy/set"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductAuditQuotaAsync), typeof(PostAttribute), "/channels/ec/product/getauditquota"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductRestrictedInfoAsync), typeof(PostAttribute), "/channels/ec/product/getproductrestrictedinfo"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.ClassifyProductCategoryAsync), typeof(PostAttribute), "/channels/ec/product/category/classify"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.CategoryPrecheckAsync), typeof(PostAttribute), "/channels/ec/product/categoryprecheck"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.AddProductThirdPartySourceAsync), typeof(PostAttribute), "/channels/ec/product/addproductthirdpartysource"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.RecommendProductBrandAsync), typeof(PostAttribute), "/channels/ec/product/productbrandrecommend"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.MapExternalProductAttributeAsync), typeof(PostAttribute), "/channels/ec/product/externalproductmapping"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.MapExternalProductAttributeNewAsync), typeof(PostAttribute), "/channels/ec/product/externalproductmappingnew"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.BeginTimingSaleAsync), typeof(PostAttribute), "/channels/ec/product/begintimingsale"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.CancelTimingSaleAsync), typeof(PostAttribute), "/channels/ec/product/canceltimingsale"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductTagLinkAsync), typeof(PostAttribute), "/channels/ec/product/taglink/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductQrcodeAsync), typeof(PostAttribute), "/channels/ec/product/qrcode/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductH5UrlAsync), typeof(PostAttribute), "/channels/ec/product/h5url/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetProductSchemeAsync), typeof(PostAttribute), "/channels/ec/product/scheme/get"),
        // 库存（4）
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetStockAsync), typeof(PostAttribute), "/channels/ec/product/stock/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.UpdateStockAsync), typeof(PostAttribute), "/channels/ec/product/stock/update"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.BatchGetStockAsync), typeof(PostAttribute), "/channels/ec/product/stock/batchget"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetStockFlowAsync), typeof(PostAttribute), "/channels/ec/product/stock/getflow"),
        // 赠品（6）
        (typeof(IChannelsProductService), nameof(IChannelsProductService.AddGiftProductAsync), typeof(PostAttribute), "/channels/ec/product/gift/add"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetGiftProductAsync), typeof(PostAttribute), "/channels/ec/product/gift/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetGiftProductListAsync), typeof(PostAttribute), "/channels/ec/product/gift/list/get"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.SetGiftOnsaleAsync), typeof(PostAttribute), "/channels/ec/product/gift/onsale/set"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.UpdateGiftStockAsync), typeof(PostAttribute), "/channels/ec/product/gift/stock/update"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.UpdateGiftProductAsync), typeof(PostAttribute), "/channels/ec/product/gift/update"),
        // 买赠活动（3）
        (typeof(IChannelsProductService), nameof(IChannelsProductService.AddGiftActivityAsync), typeof(PostAttribute), "/channels/ec/product/activity/add"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.DeleteGiftActivityAsync), typeof(PostAttribute), "/channels/ec/product/activity/del"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.StopGiftActivityAsync), typeof(PostAttribute), "/channels/ec/product/activity/stop"),
        // 限时抢购（5）
        (typeof(IChannelsProductService), nameof(IChannelsProductService.AddLimitedDiscountTaskAsync), typeof(PostAttribute), "/channels/ec/product/limiteddiscounttask/add"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.UpdateLimitedDiscountTaskAsync), typeof(PostAttribute), "/channels/ec/product/limiteddiscounttask/update"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.DeleteLimitedDiscountTaskAsync), typeof(PostAttribute), "/channels/ec/product/limiteddiscounttask/delete"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.StopLimitedDiscountTaskAsync), typeof(PostAttribute), "/channels/ec/product/limiteddiscounttask/stop"),
        (typeof(IChannelsProductService), nameof(IChannelsProductService.GetLimitedDiscountTaskListAsync), typeof(PostAttribute), "/channels/ec/product/limiteddiscounttask/list/get"),
    };

    /// <summary>官方「传空的json串即可」的端点（空请求体，不得声明 [Body]）。</summary>
    private static readonly string[] EmptyBodyEndpoints =
    {
        nameof(IChannelsProductService.GetProductAuditStrategyAsync),
        nameof(IChannelsProductService.GetProductAuditQuotaAsync),
    };

    /// <summary>契约守卫 CP1：Product 域 43 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void ProductEndpoints_ShouldMatchOfficialRoutes()
    {
        // 43 端点 = 商品主端点 25 + 库存 4 + 赠品 6 + 买赠活动 3 + 限时抢购 5（设计方案 v1 §2.2 计数口径）。
        ProductRoutes.Should().HaveCount(43, "Product 域恰 43 端点（XIAODIAN 商品管理 43 条，与 SHIPINHAO 重叠的 5 条只计 1）");
        ProductRoutes.Select(r => r.Route).Distinct().Should().HaveCount(43);

        // 全部走 /channels/ec/product/* 主干前缀，无 /shop/ 历史前缀并存。
        ProductRoutes.Select(r => r.Route)
            .All(static r => r.StartsWith("/channels/ec/product/", StringComparison.Ordinal))
            .Should().BeTrue("Product 域 43 端点全部走 /channels/ec/product/* 前缀（设计方案 v1 §4.3）");

        foreach (var (iface, method, httpAttribute, route) in ProductRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CP2：Product 域 43 端点全部为 POST；空请求体端点不得携带 [Body] 参数。</summary>
    [Fact]
    public void ProductEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in ProductRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（商品管理无 GET 端点）");
        }

        foreach (var emptyEndpoint in EmptyBodyEndpoints)
        {
            var target = typeof(IChannelsProductService).GetMethod(emptyEndpoint)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{emptyEndpoint} 官方规定空请求体（调用接口时传空的json串即可）——不得声明 [Body]");
        }
    }

    /// <summary>契约守卫 CP3：令牌绑定与注册形态。</summary>
    [Fact]
    public void ProductInterface_ShouldBindChannelsToken_AndRegisterUnderProductGroup()
    {
        var token = typeof(IChannelsProductService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Product 域 43 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsProductService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(ProductRegistryGroupName, "必须挂 Product 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Product");
        typeof(ChannelsServiceBuilder).GetMethod("AddProductApi").Should().NotBeNull("必须提供 AddProductApi 链式注册方法");
    }

    /// <summary>契约守卫 CP4：Product 域 DTO 全量登记进 AOT JSON 上下文。</summary>
    [Fact]
    public void ProductDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = ProductJsonContext.Default;

        var domainTypes = typeof(ChannelsAddProductRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Product"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(122,
            "Product 域 43 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（商品主 67 + 库存 13 + 赠品 18 + 买赠活动 12 + 限时抢购 12 = 122，口径以 ProductJsonContext.g.cs 登记为准）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Product 域命名空间，必须登记进 ProductJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(ProductRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Product");
        }

        // 静态常量类（static class ⇔ IsAbstract && IsSealed）不得被误标 [HttpJsonSerializable]。
        var constantClasses = domainTypes.Where(static t => t.IsAbstract && t.IsSealed).ToList();
        constantClasses.Should().HaveCount(12,
            "Product 域静态常量类计数漂移须核对（商品线上/草稿/类型/运费/限购/七天无理由 6 + 受限场景/来源 2 + 货源场景 1 + 限时抢购状态 1 + 库存事件类型/子类型 2 = 12）");
        constantClasses.Should().OnlyContain(
            static t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null,
            "常量类只有静态常量、无序列化语义，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CP5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void ProductDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 添加商品（代表性必填/关键字段）。
        AssertJsonProperty<ChannelsAddProductRequest>("title", "标题");
        AssertJsonProperty<ChannelsAddProductRequest>("head_imgs", "主图列表");
        AssertJsonProperty<ChannelsAddProductRequest>("deliver_method", "发货方式");
        AssertJsonProperty<ChannelsAddProductRequest>("cats_v2", "新类目树类目");
        AssertJsonProperty<ChannelsAddProductRequest>("extra_service", "额外服务");
        AssertJsonProperty<ChannelsAddProductData>("product_id", "商品 ID（data 内）");
        AssertJsonProperty<ChannelsAddProductResponse>("data", "data 包装对象");
        AssertJsonProperty<ChannelsAddProductData>("create_time", "创建时间（data 内）");

        // 更新 / 免审（共享嵌套对象字段）。
        AssertJsonProperty<ChannelsUpdateProductRequest>("product_id", "更新必填商品 ID");
        AssertJsonProperty<ChannelsUpdateProductAuditFreeRequest>("skus", "免审更新 sku 列表");
        AssertJsonProperty<ChannelsStockInfo>("diff_type", "库存修改类型");
        AssertJsonProperty<ChannelsStockInfo>("num", "库存修改值");
        AssertJsonProperty<ChannelsProductSkuUpdate>("is_delete", "删除 sku 标记");
        AssertJsonProperty<ChannelsProductSkuUpdate>("sku_id", "sku_id");

        // 获取商品（data_type 分流 + 限量信息）。
        AssertJsonProperty<ChannelsGetProductRequest>("data_type", "获取数据类型");
        AssertJsonProperty<ChannelsGetProductResponse>("edit_product", "草稿数据");
        AssertJsonProperty<ChannelsGetProductResponse>("sale_limit_info", "当日售卖上限提醒");

        // 列表 / 上下架共用请求（ChannelsProductIdRequest 五端点共用）。
        AssertJsonProperty<ChannelsProductListRequest>("page_size", "每页数量");
        AssertJsonProperty<ChannelsProductIdRequest>("product_id", "商品 ID（listing/delisting/delete/audit-cancel/canceltimingsale 共用）");
        AssertJsonProperty<ChannelsProductIdListResponse>("product_ids", "商品 id 列表（list/get 与 gift/list/get、gift/onsale/set 共用）");
        AssertJsonProperty<ChannelsProductIdListResponse>("next_key", "翻页上下文");
        AssertJsonProperty<ChannelsProductIdListResponse>("total_num", "总数");

        // 库存（batchget 多维库存）。
        AssertJsonProperty<ChannelsGetStockRequest>("sku_id", "内部 sku_id");
        AssertJsonProperty<ChannelsUpdateStockRequest>("diff_type", "diff_type（商品库存与赠品库存更新共用）");
        AssertJsonProperty<ChannelsBatchGetStockRequest>("product_id", "商品 ID 列表（上限 50）");
        AssertJsonProperty<ChannelsSkuStock>("normal_stock_num", "普通库存数量");
        AssertJsonProperty<ChannelsSkuStock>("finder_total_num", "达人专属计划营销库存数量");
        AssertJsonProperty<ChannelsWarehouseStock>("out_warehouse_id", "区域库存外部 id");
        AssertJsonProperty<ChannelsStockFlowInfo>("op_type", "库存事件类型");

        // 赠品 / 买赠活动 / 限时抢购。
        AssertJsonProperty<ChannelsAddGiftProductRequest>("cats_v2", "赠品类目（新类目树）");
        AssertJsonProperty<ChannelsGiftSku>("out_sku_id", "外部 sku_id");
        AssertJsonProperty<ChannelsGiftActivityReceiveLimit>("is_limited", "是否限领");
        AssertJsonProperty<ChannelsGiftActivityGiftSet>("gift_set_num", "赠品总套数");
        AssertJsonProperty<ChannelsAddGiftActivityRequest>("detail", "活动详情");
        AssertJsonProperty<ChannelsAddLimitedDiscountTaskRequest>("limited_discount_skus", "限时抢购 SKU 列表");
        AssertJsonProperty<ChannelsLimitedDiscountTask>("discount_pay_gmv", "活动支付 GMV");
        AssertJsonProperty<ChannelsLimitedDiscountSkuDetail>("remaining_stock", "SKU 剩余抢购库存");

        // 共用 DTO 裁决（不得出现同字段重复建模）。
        typeof(ChannelsProductIdRequest).GetProperty(nameof(ChannelsProductIdRequest.ProductId))!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!.Name.Should().Be("product_id");

        // 官方文档矛盾照录：activity/del 的 activity_id 为 number、activity/stop 的 activity_id 为 string。
        AssertJsonProperty<ChannelsGiftActivityDelRequest>("activity_id", "买赠活动 ID（del 官方类型 number）");
        typeof(ChannelsGiftActivityDelRequest).GetProperty(nameof(ChannelsGiftActivityDelRequest.ActivityId))!
            .PropertyType.Should().Be(typeof(long), "activity/del 的 activity_id 官方类型为 number");
        AssertJsonProperty<ChannelsGiftActivityStopRequest>("activity_id", "买赠活动 ID（stop 官方类型 string）");
        typeof(ChannelsGiftActivityStopRequest).GetProperty(nameof(ChannelsGiftActivityStopRequest.ActivityId))!
            .PropertyType.Should().Be(typeof(string), "activity/stop 的 activity_id 官方类型为 string（与 del 不同，照录）");
    }

    /// <summary>契约守卫 CP6：Product 域枚举常量漂移锁定（商品线上状态 / 限时抢购状态 / 库存事件类型）。</summary>
    [Fact]
    public void ProductEnums_ShouldLockOfficialValues()
    {
        // 商品线上状态（官方 status 枚举）。
        typeof(ChannelsProductStatuses).GetField(nameof(ChannelsProductStatuses.Listing))!.GetRawConstantValue()
            .Should().Be(5, "上架");
        typeof(ChannelsProductStatuses).GetField(nameof(ChannelsProductStatuses.AuditPassed))!.GetRawConstantValue()
            .Should().Be(17, "审核通过");

        // 限时抢购任务状态。
        typeof(ChannelsLimitedDiscountTaskStatuses).GetField(nameof(ChannelsLimitedDiscountTaskStatuses.Pending))!.GetRawConstantValue()
            .Should().Be(0, "创建完成未开始");
        typeof(ChannelsLimitedDiscountTaskStatuses).GetField(nameof(ChannelsLimitedDiscountTaskStatuses.Running))!.GetRawConstantValue()
            .Should().Be(1, "进行中");

        // 库存事件类型（getflow op_type）。
        typeof(ChannelsStockFlowOpTypes).GetField(nameof(ChannelsStockFlowOpTypes.OrderLock))!.GetRawConstantValue()
            .Should().Be(4, "下单占用");
        typeof(ChannelsStockFlowOpTypes).GetField(nameof(ChannelsStockFlowOpTypes.MoveIn))!.GetRawConstantValue()
            .Should().Be(7, "归还（转入）");
    }

    /// <summary>断言类型的 JSON 字段名（官方参数名照抄）。</summary>
    private static void AssertJsonProperty<T>(string expected, string description)
    {
        var propertyNames = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        propertyNames.Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Should().Contain(expected, $"{typeof(T).Name} 必须按官方参数名建模 {description}（{expected}）");
    }
}