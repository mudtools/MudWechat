// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Compass;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 视频号「罗盘」域契约守卫（CCP 系列：Compass 域 14 端点路由 + 商家版/达人版双前缀并存 +
/// 令牌绑定 + DTO 字段与共用/分建裁决 + JSON 上下文登记 + 枚举常量 + 类型口径照录）。
/// </summary>
/// <remarks>
/// <para>
/// <b>双前缀并存（官方原文，照抄不得改写）</b>：商家版 <c>/channels/ec/compass/shop/*</c>（10 端点，
/// 微信小店 access_token，权限集 id 175）+ 达人版 <c>/channels/ec/compass/finder/*</c>（4 端点，
/// 视频号助手 access_token，权限集 id 176）。同构端点各自落端点不合并。
/// </para>
/// <para>
/// <b>类型口径照录锁定（官方矛盾防「顺手统一」）</b>：商家版多数指标官方类型为 string
/// （getshopoverall / getshopfinderproductoverall / getshopproductdata / getshopproductlist），
/// 达人版同为 number；getshopfinderlist 与 getshopfinderoverall 参数表 number 但示例字符串；
/// getfindersaleprofiledata 的 dim_value 参数表 number 但示例字符串——全部按参数表建模，禁止改齐。
/// </para>
/// </remarks>
public class ChannelsCompassContractGuards
{
    private const string CompassRegistryGroupName = "Compass";

    /// <summary>Compass 域官方路由表（14 端点：/channels/ec/compass/shop/* 10 + /channels/ec/compass/finder/* 4，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] CompassRoutes =
    {
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopFinderAuthorizationListAsync), typeof(PostAttribute), "/channels/ec/compass/shop/finder/authorization/list/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopFinderListAsync), typeof(PostAttribute), "/channels/ec/compass/shop/finder/list/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopFinderOverallAsync), typeof(PostAttribute), "/channels/ec/compass/shop/finder/overall/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopFinderProductListAsync), typeof(PostAttribute), "/channels/ec/compass/shop/finder/product/list/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopFinderProductOverallAsync), typeof(PostAttribute), "/channels/ec/compass/shop/finder/product/overall/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopLiveListAsync), typeof(PostAttribute), "/channels/ec/compass/shop/live/list/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopOverallAsync), typeof(PostAttribute), "/channels/ec/compass/shop/overall/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopProductDataAsync), typeof(PostAttribute), "/channels/ec/compass/shop/product/data/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopProductListAsync), typeof(PostAttribute), "/channels/ec/compass/shop/product/list/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetShopSaleProfileDataAsync), typeof(PostAttribute), "/channels/ec/compass/shop/sale/profile/data/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetFinderOverallAsync), typeof(PostAttribute), "/channels/ec/compass/finder/overall/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetFinderProductDataAsync), typeof(PostAttribute), "/channels/ec/compass/finder/product/data/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetFinderProductListAsync), typeof(PostAttribute), "/channels/ec/compass/finder/product/list/get"),
        (typeof(IChannelsCompassService), nameof(IChannelsCompassService.GetFinderSaleProfileDataAsync), typeof(PostAttribute), "/channels/ec/compass/finder/sale/profile/data/get"),
    };

    /// <summary>契约守卫 CCP1：Compass 域 14 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void CompassEndpoints_ShouldMatchOfficialRoutes()
    {
        // 14 端点 = 商家版 10（授权列表 1 + 达人 4 + 开播 1 + 概览 1 + 商品 2 + 人群 1）+ 达人版 4。
        CompassRoutes.Should().HaveCount(14, "罗盘商家版 10 端点 + 罗盘达人版 4 端点");
        CompassRoutes.Select(r => r.Route).Distinct().Should().HaveCount(14);

        CompassRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/compass/shop/", StringComparison.Ordinal))
            .Should().Be(10, "10 端点走 /channels/ec/compass/shop/* 罗盘商家版前缀");
        CompassRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/compass/finder/", StringComparison.Ordinal))
            .Should().Be(4, "4 端点走 /channels/ec/compass/finder/* 罗盘达人版前缀");

        foreach (var (iface, method, httpAttribute, route) in CompassRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CCP2：Compass 域 14 端点全部为 POST；空请求体端点不得携带 [Body] 参数。</summary>
    [Fact]
    public void CompassEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in CompassRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        // getshopfinderauthorizationlist 官方示例请求体为空（{}），不得人为造请求体 DTO。
        var target0 = typeof(IChannelsCompassService).GetMethod(nameof(IChannelsCompassService.GetShopFinderAuthorizationListAsync))!;
        target0.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
            .Should().BeEmpty("getshopfinderauthorizationlist 官方无请求体参数——不得声明 [Body]");
    }

    /// <summary>契约守卫 CCP3：令牌绑定与注册形态。</summary>
    [Fact]
    public void CompassInterface_ShouldBindChannelsToken_AndRegisterUnderCompassGroup()
    {
        var token = typeof(IChannelsCompassService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Compass 域 14 端点消费小店/视频号助手 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsCompassService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(CompassRegistryGroupName, "必须挂 Compass 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Compass");
        typeof(ChannelsServiceBuilder).GetMethod("AddCompassApi").Should().NotBeNull("必须提供 AddCompassApi 链式注册方法");
    }

    /// <summary>契约守卫 CCP4：Compass 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void CompassDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = CompassJsonContext.Default;

        var domainTypes = typeof(ChannelsCompassDsRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Compass"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(42,
            "Compass 域 14 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（商家版 32：共用 ds 1 + 达人列表 4 + 授权列表 1 + 达人概览 2 + 达人商品列表/详情 6 +" +
            " 开播 3 + 电商概览 2 + 商品详情 4 + 商品列表 4 + 人群 5（含共用请求）= 32；" +
            "达人版 10：电商概览 2 + 商品数据 3 + 商品列表 1 + 人群 4 = 10）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Compass 域命名空间，必须登记进 CompassJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(CompassRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Compass");
        }

        // 常量类（静态常量类不参与序列化）：Compass 域 2 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(2,
            "Compass 域常量类 = 商家版人群用户类型 + 达人版人群用户类型");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CCP5：官方字段名与共用/分建 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void CompassDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 共用 ds 请求（5 端点）。
        AssertJsonProperty<ChannelsCompassDsRequest>("ds", "日期（YYYYMMDD）");

        // 授权视频号列表。
        AssertJsonProperty<ChannelsCompassShopFinderAuthorizationListResponse>("authorized_finder_id_list", "授权视频号 id 列表");

        // 带货达人列表。
        AssertJsonProperty<ChannelsCompassShopFinderListRequest>("version", "v2 返回带货者身份 id");
        AssertJsonProperty<ChannelsCompassShopFinderListResponse>("finder_list", "视频号列表");
        AssertJsonProperty<ChannelsCompassShopFinderItem>("finder_id", "视频号 id");
        AssertJsonProperty<ChannelsCompassShopFinderItem>("talent_id", "带货者 id");
        AssertJsonProperty<ChannelsCompassShopFinderItem>("mp_nickname", "公众号昵称");
        AssertJsonProperty<ChannelsCompassDealData>("pay_gmv", "成交金额（分）");
        AssertJsonProperty<ChannelsCompassDealData>("pay_product_id_cnt", "动销商品数");
        AssertJsonProperty<ChannelsCompassDealData>("pay_refund_gmv", "成交退款金额");

        // 带货数据概览。
        AssertJsonProperty<ChannelsCompassShopFinderOverallResponse>("data", "带货数据");
        AssertJsonProperty<ChannelsCompassShopFinderOverallData>("pay_sales_finder_cnt", "动销达人数");
        AssertJsonProperty<ChannelsCompassShopFinderOverallData>("click_to_pay_uv_ratio", "点击-成交转化率");

        // 带货达人商品列表 / 详情。
        AssertJsonProperty<ChannelsCompassShopFinderProductRequest>("talent_id", "达人号 id（三者必填其一）");
        AssertJsonProperty<ChannelsCompassShopFinderProductRequest>("mp_id", "公众号 id（三者必填其一）");
        AssertJsonProperty<ChannelsCompassShopFinderProductListResponse>("product_list", "商品信息列表");
        AssertJsonProperty<ChannelsCompassShopFinderProductItem>("first_category_id", "商品 1 级类目");
        AssertJsonProperty<ChannelsCompassShopFinderProductCommission>("commission_ratio", "佣金率");
        AssertJsonProperty<ChannelsCompassShopFinderProductOverallData>("pay_gmv", "成交金额（官方类型 string）");

        // 店铺开播列表。
        AssertJsonProperty<ChannelsCompassShopLiveListRequest>("finder_id", "视频号 id");
        AssertJsonProperty<ChannelsCompassShopLiveListResponse>("live_list", "开播列表");
        AssertJsonProperty<ChannelsCompassShopLiveItem>("live_cover_img_url", "直播封面");
        AssertJsonProperty<ChannelsCompassShopLiveItem>("live_time", "开播时间（官方类型 string）");

        // 电商数据概览。
        AssertJsonProperty<ChannelsCompassShopOverallData>("pay_order_cnt", "成交订单数（官方类型 string）");
        AssertJsonProperty<ChannelsCompassShopOverallData>("feed_pay_gmv", "短视频成交金额");

        // 商品详细信息 / 列表。
        AssertJsonProperty<ChannelsCompassShopProductDataRequest>("product_id", "商品 id");
        AssertJsonProperty<ChannelsCompassShopProductDataResponse>("product_info", "商品详细信息");
        AssertJsonProperty<ChannelsCompassShopProductInfo>("third_category_id", "商品三级类目");
        AssertJsonProperty<ChannelsCompassShopProductDetailData>("pure_pay_gmv", "成交金额（剔除退款）");
        AssertJsonProperty<ChannelsCompassShopProductDetailData>("seller_actual_settle_amount", "实际结算金额");
        AssertJsonProperty<ChannelsCompassShopProductDetailData>("finderuin_predict_commission", "预估达人佣金支出");
        AssertJsonProperty<ChannelsCompassShopProductDetailData>("pay_refund_after_send_ratio", "发货后成交退款率（官方类型列 null，照录）");
        AssertJsonProperty<ChannelsCompassShopProductListRequest>("limit", "分页参数");
        AssertJsonProperty<ChannelsCompassShopProductListRequest>("offset", "分页参数");
        AssertJsonProperty<ChannelsCompassShopProductListResponse>("total_count", "商品总数");
        AssertJsonProperty<ChannelsCompassShopProductListData>("pay_refund_cnt", "成交退款订单数（2025-11-24 变更新增）");
        AssertJsonProperty<ChannelsCompassShopProductListData>("refund_gmv", "退款件数（官方说明原文照录）");

        // 人群数据（商家版 / 达人版）。
        AssertJsonProperty<ChannelsCompassSaleProfileRequest>("type", "用户类型");
        AssertJsonProperty<ChannelsCompassShopSaleProfileResponse>("data", "店铺人群数据");
        AssertJsonProperty<ChannelsCompassShopSaleProfile>("field_list", "维度列表");
        AssertJsonProperty<ChannelsCompassShopSaleProfileField>("field_name", "维度类别名");
        AssertJsonProperty<ChannelsCompassShopSaleProfileDim>("dim_value", "维度指标值（官方类型 string）");
        AssertJsonProperty<ChannelsCompassFinderSaleProfileDim>("dim_value", "维度指标值（官方类型 number，两侧不共用）");

        // 达人版。
        AssertJsonProperty<ChannelsCompassFinderOverallData>("window_pay_gmv", "橱窗成交金额");
        AssertJsonProperty<ChannelsCompassFinderOverallData>("other_pay_gmv", "其他渠道成交金额");
        AssertJsonProperty<ChannelsCompassFinderProductDataResponse>("product_info", "带货商品数据");
        AssertJsonProperty<ChannelsCompassFinderProductData>("actual_commission", "实际结算金额");
        AssertJsonProperty<ChannelsCompassFinderProductData>("predict_commission", "预估佣金金额");
        AssertJsonProperty<ChannelsCompassFinderProductListResponse>("product_list", "带货商品列表");
        AssertJsonProperty<ChannelsCompassFinderSaleProfile>("field_list", "维度列表");

        // 共用 DTO 裁决：5 端点请求体仅 ds ⇒ 共用 ChannelsCompassDsRequest。
        foreach (var dsEndpoint in new[]
                 {
                     nameof(IChannelsCompassService.GetShopFinderOverallAsync),
                     nameof(IChannelsCompassService.GetShopOverallAsync),
                     nameof(IChannelsCompassService.GetFinderOverallAsync),
                     nameof(IChannelsCompassService.GetFinderProductDataAsync),
                     nameof(IChannelsCompassService.GetFinderProductListAsync),
                 })
        {
            typeof(IChannelsCompassService).GetMethod(dsEndpoint)!
                .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsCompassDsRequest),
                    $"{dsEndpoint} 请求体字段集仅 ds ⇒ 共用 ChannelsCompassDsRequest");
        }

        // 共用 DTO 裁决：getshopfinderproductlist 与 getshopfinderproductoverall 请求体字段集一致 ⇒ 共用 ChannelsCompassShopFinderProductRequest。
        typeof(IChannelsCompassService).GetMethod(nameof(IChannelsCompassService.GetShopFinderProductListAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsCompassShopFinderProductRequest));
        typeof(IChannelsCompassService).GetMethod(nameof(IChannelsCompassService.GetShopFinderProductOverallAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsCompassShopFinderProductRequest));

        // 共用 DTO 裁决：getshopsaleprofiledata 与 getfindersaleprofiledata 请求体字段集一致 ⇒ 共用 ChannelsCompassSaleProfileRequest（枚举常量分建）。
        typeof(IChannelsCompassService).GetMethod(nameof(IChannelsCompassService.GetShopSaleProfileDataAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsCompassSaleProfileRequest));
        typeof(IChannelsCompassService).GetMethod(nameof(IChannelsCompassService.GetFinderSaleProfileDataAsync))!
            .GetParameters().Should().Contain(p => p.ParameterType == typeof(ChannelsCompassSaleProfileRequest));

        // 共用 DTO 裁决：getfinderproductdata 的 product_info 与 getfinderproductlist 的列表元素字段集一致 ⇒ 共用 ChannelsCompassFinderProductInfo。
        typeof(ChannelsCompassFinderProductDataResponse).GetProperty(nameof(ChannelsCompassFinderProductDataResponse.ProductInfo))!
            .PropertyType.Should().Be(typeof(ChannelsCompassFinderProductInfo));
        typeof(ChannelsCompassFinderProductListResponse).GetProperty(nameof(ChannelsCompassFinderProductListResponse.ProductList))!
            .PropertyType.Should().Be(typeof(List<ChannelsCompassFinderProductInfo>));

        // 分建裁决：商家版/达人版商品明细数据字段集与类型口径均不同（string vs number、seller 结算字段有无）⇒ 分建。
        typeof(ChannelsCompassShopProductInfo).GetProperty(nameof(ChannelsCompassShopProductInfo.Data))!
            .PropertyType.Should().Be(typeof(ChannelsCompassShopProductDetailData));
        typeof(ChannelsCompassFinderProductInfo).GetProperty(nameof(ChannelsCompassFinderProductInfo.Data))!
            .PropertyType.Should().Be(typeof(ChannelsCompassFinderProductData));

        // 类型口径锁定：商家版概览 string、达人版概览 number（官方原文照录，禁止统一）。
        typeof(ChannelsCompassShopOverallData).GetProperty(nameof(ChannelsCompassShopOverallData.PayGmv))!
            .PropertyType.Should().Be(typeof(string), "getshopoverall 参数表 pay_gmv 官方类型为 string");
        typeof(ChannelsCompassFinderOverallData).GetProperty(nameof(ChannelsCompassFinderOverallData.PayGmv))!
            .PropertyType.Should().Be(typeof(long?), "getfinderoverall 参数表 pay_gmv 官方类型为 number");
    }

    /// <summary>契约守卫 CCP6：枚举常量字面量锁定。</summary>
    [Fact]
    public void CompassEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsCompassShopSaleProfileUserTypes.ProductExposureUser.Should().Be(1);
        ChannelsCompassShopSaleProfileUserTypes.ProductClickUser.Should().Be(2);
        ChannelsCompassShopSaleProfileUserTypes.PurchaseUser.Should().Be(3);
        ChannelsCompassShopSaleProfileUserTypes.FirstPurchaseUser.Should().Be(4);
        ChannelsCompassShopSaleProfileUserTypes.RepeatPurchaseUser.Should().Be(5);

        ChannelsCompassFinderSaleProfileUserTypes.ProductExposureUser.Should().Be(1);
        ChannelsCompassFinderSaleProfileUserTypes.ProductClickUser.Should().Be(2);
        ChannelsCompassFinderSaleProfileUserTypes.PurchaseUser.Should().Be(3);
        ChannelsCompassFinderSaleProfileUserTypes.FirstPurchaseUser.Should().Be(4);
        ChannelsCompassFinderSaleProfileUserTypes.RepeatPurchaseUser.Should().Be(5);
        ChannelsCompassFinderSaleProfileUserTypes.LiveWatchUser.Should().Be(6);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
