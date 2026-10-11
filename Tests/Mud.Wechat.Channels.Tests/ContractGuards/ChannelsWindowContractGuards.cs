// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Window;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 「橱窗与本地生活商品」域契约守卫（CWI 系列：Window 域 9 端点路由 + 双前缀并存 +
/// 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 官方矛盾照录 + 枚举常量）。
/// </summary>
/// <remarks>
/// <para>
/// <b>双前缀并存（同域不同前缀，守卫措辞锁定）</b>：本域同时承载 <c>/channels/ec/window/product/*</c>
/// （视频号个人橱窗，4 端点，视频号助手 API）与 <c>/channels/ec/product/locallife/*</c>
/// （小店本地生活团购优惠，5 端点）——两组端点照官方原文各自落位，不合并不重复。
/// </para>
/// <para>
/// <b>与小店「主页商品排序」严格区分</b>：小店店铺主页展示商品走 <c>/channels/ec/store/window/*</c>
/// （<c>ChannelsHomePageContractGuards</c> 锁定），禁止与本域 <c>/channels/ec/window/*</c> 混同。
/// </para>
/// <para>
/// <b>官方契约警示（照录）</b>：橱窗上下架接口（add/off）官方注明「该 api 已无实际作用」——
/// 端点保留以对齐官方开放面，不做删减或改写。
/// </para>
/// </remarks>
public class ChannelsWindowContractGuards
{
    private const string WindowRegistryGroupName = "Window";

    /// <summary>Window 域官方路由表（9 端点：/channels/ec/window/product/* 4 + /channels/ec/product/locallife/* 5，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] WindowRoutes =
    {
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.AddWindowProductAsync), typeof(PostAttribute), "/channels/ec/window/product/add"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.GetWindowProductAsync), typeof(PostAttribute), "/channels/ec/window/product/get"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.GetWindowProductListAsync), typeof(PostAttribute), "/channels/ec/window/product/list/get"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.OffWindowProductAsync), typeof(PostAttribute), "/channels/ec/window/product/off"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.AddLocallifeProductAsync), typeof(PostAttribute), "/channels/ec/product/locallife/add"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.GetLocallifeProductListAsync), typeof(PostAttribute), "/channels/ec/product/locallife/list/get"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.GetLocallifeProductAsync), typeof(PostAttribute), "/channels/ec/product/locallife/get"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.AuditFreeUpdateLocallifeProductAsync), typeof(PostAttribute), "/channels/ec/product/locallife/auditfree"),
        (typeof(IChannelsWindowService), nameof(IChannelsWindowService.UpdateLocallifeProductAsync), typeof(PostAttribute), "/channels/ec/product/locallife/update"),
    };

    /// <summary>契约守卫 CWI1：Window 域 9 端点路由必须与官方契约一致（全部 POST，双前缀并存）。</summary>
    [Fact]
    public void WindowEndpoints_ShouldMatchOfficialRoutes()
    {
        // 9 端点 = 视频号橱窗 4 + 本地生活团购优惠 5。
        WindowRoutes.Should().HaveCount(9, "视频号橱窗 4 + 本地生活团购优惠 5 = 9 端点");
        WindowRoutes.Select(r => r.Route).Distinct().Should().HaveCount(9);

        WindowRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/window/product/", StringComparison.Ordinal))
            .Should().Be(4, "视频号个人橱窗 4 端点走 /channels/ec/window/product/*（非小店店铺主页）");
        WindowRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/product/locallife/", StringComparison.Ordinal))
            .Should().Be(5, "本地生活团购优惠 5 端点走 /channels/ec/product/locallife/*");

        foreach (var (iface, method, httpAttribute, route) in WindowRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CWI2：Window 域 9 端点全部为 POST；本域无空请求体端点（全部带 [Body]）。</summary>
    [Fact]
    public void WindowEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in WindowRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（橱窗/本地生活无 GET 端点）");

            // 本域 9 端点官方参数表均有请求体——每个方法必须恰有一个 [Body] 参数。
            target.GetParameters().Count(static p => p.GetCustomAttributes<BodyAttribute>().Any())
                .Should().Be(1, $"{iface.Name}.{method} 官方参数表均有请求体，必须恰有一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 CWI3：令牌绑定与注册形态。</summary>
    [Fact]
    public void WindowInterface_ShouldBindChannelsToken_AndRegisterUnderWindowGroup()
    {
        var token = typeof(IChannelsWindowService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Window 域 9 端点消费小店/视频号助手 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsWindowService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(WindowRegistryGroupName, "必须挂 Window 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Window");
        typeof(ChannelsServiceBuilder).GetMethod("AddWindowApi").Should().NotBeNull("必须提供 AddWindowApi 链式注册方法");
    }

    /// <summary>契约守卫 CWI4：Window 域 DTO 全量登记进 AOT JSON 上下文；常量类不得标序列化特性。</summary>
    [Fact]
    public void WindowDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = WindowJsonContext.Default;

        var domainTypes = typeof(ChannelsAddWindowProductRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Window"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(27,
            "Window 域 9 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（橱窗 12 + 本地生活 15 = 27）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Window 域命名空间，必须登记进 WindowJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(WindowRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Window");
        }

        // 常量类（非序列化 DTO）：橱窗 5（商品状态/禁售原因/分店状态/带货计划类型/商品来源）
        // + 本地生活 5（券类型/券码来源/草稿状态/线上状态/审核状态）= 10，不得标 [HttpJsonSerializable]。
        domainTypes.Where(t => t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .Should().HaveCount(10, "本域常量类应为橱窗 5 个 + 本地生活 5 个共 10 个");
    }

    /// <summary>契约守卫 CWI5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void WindowDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 视频号橱窗。
        AssertJsonProperty<ChannelsAddWindowProductRequest>("product_id", "橱窗商品 ID（参数表 number，矛盾照录）");
        AssertJsonProperty<ChannelsAddWindowProductRequest>("is_hide_for_window", "是否在个人橱窗页隐藏");
        AssertJsonProperty<ChannelsGetWindowProductRequest>("need_cps_product", "是否查询带货商品");
        AssertJsonProperty<ChannelsGetWindowProductResponse>("cps_products", "带货商品列表");
        AssertJsonProperty<ChannelsWindowProduct>("out_product_id", "外部商品 ID");
        AssertJsonProperty<ChannelsWindowProduct>("leaf_category_id", "叶子类目 ID（替代 third_category_id）");
        AssertJsonProperty<ChannelsWindowProduct>("selling_price", "销售价（分）");
        AssertJsonProperty<ChannelsWindowProduct>("product_promotion_link", "跟佣跳转链接");
        AssertJsonProperty<ChannelsWindowPagePath>("half_page_path", "半屏页 path");
        AssertJsonProperty<ChannelsWindowBannedDetails>("need_apply_category_id", "需要申请的类目 ID");
        AssertJsonProperty<ChannelsWindowLimitDiscountInfo>("end_time_ms", "抢购结束时间（参数表 string，矛盾照录）");
        AssertJsonProperty<ChannelsGetWindowProductListRequest>("last_buffer", "顺序翻页上下文");
        AssertJsonProperty<ChannelsGetWindowProductListRequest>("query_cps_product", "是否仅查带货商品（与 appid 互斥）");
        AssertJsonProperty<ChannelsGetWindowProductListResponse>("total_num", "商品总数");
        AssertJsonProperty<ChannelsWindowListItem>("product_source", "商品来源");
        AssertJsonProperty<ChannelsOffWindowProductRequest>("appid", "商品来源店铺 appid");

        // 本地生活团购优惠。
        AssertJsonProperty<ChannelsAddLocallifeProductRequest>("out_product_id", "外部系统 ID");
        AssertJsonProperty<ChannelsAddLocallifeProductRequest>("product_type", "券类型");
        AssertJsonProperty<ChannelsAddLocallifeProductRequest>("category_id", "三级类目 ID");
        AssertJsonProperty<ChannelsAddLocallifeProductRequest>("head_imgs", "主图列表");
        AssertJsonProperty<ChannelsAddLocallifeProductRequest>("code_source_type", "券码来源");
        AssertJsonProperty<ChannelsAddLocallifeProductRequest>("attr_kv_map", "商品属性 KV（字符串键值 map）");
        AssertJsonProperty<ChannelsAddLocallifeProductRequest>("verify_at_store", "是否支持到店核销");
        AssertJsonProperty<ChannelsAddLocallifeProductResponse>("data", "创建结果");
        AssertJsonProperty<ChannelsAddLocallifeProductData>("sku_ids", "SKU ID 列表（与请求顺序一致）");
        AssertJsonProperty<ChannelsGetLocallifeProductListResponse>("next_key", "翻页上下文");
        AssertJsonProperty<ChannelsGetLocallifeProductRequest>("data_type", "数据类型（线上/草稿）");
        AssertJsonProperty<ChannelsGetLocallifeProductResponse>("online_data", "线上数据");
        AssertJsonProperty<ChannelsGetLocallifeProductResponse>("draft_data", "草稿数据");
        AssertJsonProperty<ChannelsLocallifeProductData>("attr_kv_map", "商品属性 KV");
        AssertJsonProperty<ChannelsLocallifeProductData>("online_status", "商品线上状态");
        AssertJsonProperty<ChannelsLocallifeProductData>("verify_at_store", "是否支持到店核销");
        AssertJsonProperty<ChannelsLocallifeAuditResult>("audit_status", "审核状态");
        AssertJsonProperty<ChannelsLocallifeProductQuaInfo>("qua_id", "商品资质 ID");
        AssertJsonProperty<ChannelsLocallifeProductQuaInfo>("qua_url", "商品资质图片列表");
        AssertJsonProperty<ChannelsLocallifeWxAppPath>("path", "小程序页面路径");
        AssertJsonProperty<ChannelsLocallifeSkuRequest>("sku_id", "平台生成的 SKU ID");
        AssertJsonProperty<ChannelsLocallifeSkuRequest>("sale_price", "实际售卖价格（分）");
        AssertJsonProperty<ChannelsLocallifeSku>("stock_num", "库存数量");
        AssertJsonProperty<ChannelsAuditFreeUpdateLocallifeProductRequest>("skus", "售卖单元列表");
        AssertJsonProperty<ChannelsUpdateLocallifeProductRequest>("product_id", "平台生成的商品 ID");

        // 共用 DTO 裁决：add/update/auditfree 的请求 skus 元素字段集一致（sku_id 更新/免审时必填，add 官方示例仅 sale_price）⇒ 共用 ChannelsLocallifeSkuRequest。
        AssertSkuRequestShared(nameof(IChannelsWindowService.AddLocallifeProductAsync));
        AssertSkuRequestShared(nameof(IChannelsWindowService.UpdateLocallifeProductAsync));
        AssertSkuRequestShared(nameof(IChannelsWindowService.AuditFreeUpdateLocallifeProductAsync));

        // 共用 DTO 裁决：verify_page（WxAppPath）在 add/update 请求与 get 响应结构一致 ⇒ 共用 ChannelsLocallifeWxAppPath。
        typeof(ChannelsAddLocallifeProductRequest).GetProperty(nameof(ChannelsAddLocallifeProductRequest.VerifyPage))!
            .PropertyType.Should().Be(typeof(ChannelsLocallifeWxAppPath));
        typeof(ChannelsUpdateLocallifeProductRequest).GetProperty(nameof(ChannelsUpdateLocallifeProductRequest.VerifyPage))!
            .PropertyType.Should().Be(typeof(ChannelsLocallifeWxAppPath));
        typeof(ChannelsLocallifeProductData).GetProperty(nameof(ChannelsLocallifeProductData.VerifyPage))!
            .PropertyType.Should().Be(typeof(ChannelsLocallifeWxAppPath));

        // 共用 DTO 裁决：get 响应 online_data/draft_data 官方文档明示结构相同 ⇒ 共用 ChannelsLocallifeProductData。
        typeof(ChannelsGetLocallifeProductResponse).GetProperty(nameof(ChannelsGetLocallifeProductResponse.OnlineData))!
            .PropertyType.Should().Be(typeof(ChannelsLocallifeProductData));
        typeof(ChannelsGetLocallifeProductResponse).GetProperty(nameof(ChannelsGetLocallifeProductResponse.DraftData))!
            .PropertyType.Should().Be(typeof(ChannelsLocallifeProductData));

        // 共用 DTO 裁决：get 响应 product 与 cps_products[] 元素结构一致（带货商品仅多 cps_item_type 语义）⇒ 共用 ChannelsWindowProduct。
        typeof(ChannelsGetWindowProductResponse).GetProperty(nameof(ChannelsGetWindowProductResponse.Product))!
            .PropertyType.Should().Be(typeof(ChannelsWindowProduct));
        typeof(ChannelsGetWindowProductResponse).GetProperty(nameof(ChannelsGetWindowProductResponse.CpsProducts))!
            .PropertyType.Should().Be(typeof(List<ChannelsWindowProduct>));

        // 官方文档矛盾照录：add 的 product_id 参数表为 number（建模 long），get/off/list 的 product_id 参数表为 string(uint64)（建模 string）。
        typeof(ChannelsAddWindowProductRequest).GetProperty(nameof(ChannelsAddWindowProductRequest.ProductId))!
            .PropertyType.Should().Be(typeof(long), "官方 add 参数表标注 number——SDK 按参数表建模");
        typeof(ChannelsGetWindowProductRequest).GetProperty(nameof(ChannelsGetWindowProductRequest.ProductId))!
            .PropertyType.Should().Be(typeof(string), "官方 get 参数表标注 string(uint64)——SDK 按参数表建模");
        typeof(ChannelsOffWindowProductRequest).GetProperty(nameof(ChannelsOffWindowProductRequest.ProductId))!
            .PropertyType.Should().Be(typeof(string), "官方 off 参数表标注 string(uint64)——SDK 按参数表建模");

        // 官方文档矛盾照录：get 响应 product_id/category_id/brand_id 参数表为 number、返回示例为字符串——按示例与 add/update 请求表建模为 string。
        typeof(ChannelsLocallifeProductData).GetProperty(nameof(ChannelsLocallifeProductData.ProductId))!
            .PropertyType.Should().Be(typeof(string), "官方响应参数表 number 与返回示例字符串矛盾——按示例与同域请求表 string(uint64) 建模");
        typeof(ChannelsLocallifeProductData).GetProperty(nameof(ChannelsLocallifeProductData.CategoryId))!
            .PropertyType.Should().Be(typeof(string));
        typeof(ChannelsLocallifeProductData).GetProperty(nameof(ChannelsLocallifeProductData.BrandId))!
            .PropertyType.Should().Be(typeof(string));
    }

    /// <summary>契约守卫 CWI6：枚举常量字面量锁定（橱窗 5 组 + 本地生活 5 组）。</summary>
    [Fact]
    public void WindowEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsWindowProductStatuses.OnWindow.Should().Be(1);
        ChannelsWindowProductStatuses.NotOnWindow.Should().Be(2);
        ChannelsWindowProductStatuses.DeletedAtSource.Should().Be(3);

        ChannelsWindowBannedReasons.CategoryOrSourceBanned.Should().Be(0);
        ChannelsWindowBannedReasons.CategoryApplyRequired.Should().Be(1);
        ChannelsWindowBannedReasons.BranchClosed.Should().Be(2);

        ChannelsWindowBranchStatuses.Open.Should().Be(0);
        ChannelsWindowBranchStatuses.Closed.Should().Be(1);

        ChannelsWindowCpsItemTypes.Normal.Should().Be(0);
        ChannelsWindowCpsItemTypes.Directed.Should().Be(1);
        ChannelsWindowCpsItemTypes.Exclusive.Should().Be(2);
        ChannelsWindowCpsItemTypes.Leader.Should().Be(3);

        ChannelsWindowProductSources.SelfOperated.Should().Be(0);
        ChannelsWindowProductSources.SelectionCenter.Should().Be(1);

        ChannelsLocallifeProductTypes.Voucher.Should().Be(1);
        ChannelsLocallifeProductTypes.Exchange.Should().Be(2);
        ChannelsLocallifeProductTypes.TimeCard.Should().Be(3);

        ChannelsLocallifeCodeSourceTypes.Platform.Should().Be(1);
        ChannelsLocallifeCodeSourceTypes.Merchant.Should().Be(2);
        ChannelsLocallifeCodeSourceTypes.Realtime.Should().Be(3);

        ChannelsLocallifeDraftStatuses.Init.Should().Be(0);
        ChannelsLocallifeDraftStatuses.Editing.Should().Be(1);

        ChannelsLocallifeOnlineStatuses.OnSale.Should().Be(5);
        ChannelsLocallifeOnlineStatuses.OffSaleByMerchant.Should().Be(11);
        ChannelsLocallifeOnlineStatuses.OffSaleByRiskControl.Should().Be(13);
        ChannelsLocallifeOnlineStatuses.OffSaleByDepositInsufficient.Should().Be(14);
        ChannelsLocallifeOnlineStatuses.OffSaleByBrandExpired.Should().Be(15);
        ChannelsLocallifeOnlineStatuses.Banned.Should().Be(20);

        ChannelsLocallifeAuditStatuses.Waiting.Should().Be(1);
        ChannelsLocallifeAuditStatuses.Failed.Should().Be(2);
        ChannelsLocallifeAuditStatuses.Success.Should().Be(3);
    }

    private static void AssertSkuRequestShared(string methodName)
    {
        var request = typeof(IChannelsWindowService).GetMethod(methodName)!
            .GetParameters().Single(static p => p.GetCustomAttribute<BodyAttribute>() != null);
        request.ParameterType.GetProperty("Skus")!.PropertyType.Should().Be(
            typeof(List<ChannelsLocallifeSkuRequest>),
            $"{methodName} 的 skus 元素与其他两端点字段集一致 ⇒ 共用 ChannelsLocallifeSkuRequest");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
