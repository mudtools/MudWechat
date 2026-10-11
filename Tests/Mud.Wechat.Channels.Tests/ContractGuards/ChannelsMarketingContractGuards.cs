// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Marketing;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「营销管理-优惠券」域契约守卫（CM 系列：Marketing 域 7 端点路由 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量 + 与本地生活团购券的概念辨析）。
/// </summary>
/// <remarks>
/// <para>
/// <b>概念辨析（守卫措辞锁定）</b>：本域是小店电商<b>优惠券</b>（<c>/channels/ec/coupon/*</c>，
/// 满减 / 折扣 / 直减），<b>不是</b>本地生活<b>团购券</b>（<see cref="ChannelsLocallifeContractGuards"/>，
/// <c>/channels/ec/voucher/*</c>，code 核销 + 分账结算）。路由面由 CH-R2 锁定，两域前缀天然无交叠。
/// </para>
/// <para>
/// <b>官方第三方代调用</b>：本域全部端点所属权限集 id 为 <c>132</c>（官方参数表原文）。
/// </para>
/// </remarks>
public class ChannelsMarketingContractGuards
{
    private const string MarketingRegistryGroupName = "Marketing";

    /// <summary>Marketing 域官方路由表（7 端点：/channels/ec/coupon/*，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] MarketingRoutes =
    {
        (typeof(IChannelsMarketingService), nameof(IChannelsMarketingService.CreateCouponAsync), typeof(PostAttribute), "/channels/ec/coupon/create"),
        (typeof(IChannelsMarketingService), nameof(IChannelsMarketingService.GetCouponAsync), typeof(PostAttribute), "/channels/ec/coupon/get"),
        (typeof(IChannelsMarketingService), nameof(IChannelsMarketingService.GetCouponListAsync), typeof(PostAttribute), "/channels/ec/coupon/get_list"),
        (typeof(IChannelsMarketingService), nameof(IChannelsMarketingService.GetUserCouponAsync), typeof(PostAttribute), "/channels/ec/coupon/get_user_coupon"),
        (typeof(IChannelsMarketingService), nameof(IChannelsMarketingService.GetUserCouponListAsync), typeof(PostAttribute), "/channels/ec/coupon/get_user_coupon_list"),
        (typeof(IChannelsMarketingService), nameof(IChannelsMarketingService.UpdateCouponAsync), typeof(PostAttribute), "/channels/ec/coupon/update"),
        (typeof(IChannelsMarketingService), nameof(IChannelsMarketingService.UpdateCouponStatusAsync), typeof(PostAttribute), "/channels/ec/coupon/update_status"),
    };

    /// <summary>契约守卫 CM1：Marketing 域 7 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void MarketingEndpoints_ShouldMatchOfficialRoutes()
    {
        // 7 端点 = 商家券 4（创建/详情/列表/更新内容/更新状态）+ 用户券 2（详情/列表）。
        MarketingRoutes.Should().HaveCount(7, "优惠券域官方开放面为创建/详情/ID列表/用户券详情/用户券ID列表/更新内容/更新状态 7 端点");
        MarketingRoutes.Select(r => r.Route).Distinct().Should().HaveCount(7);

        MarketingRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/coupon/", StringComparison.Ordinal))
            .Should().Be(7, "7 端点全部走 /channels/ec/coupon/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in MarketingRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CM2：Marketing 域 7 端点全部为 POST 且均携带 JSON [Body]（官方无空请求体端点）。</summary>
    [Fact]
    public void MarketingEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in MarketingRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST（优惠券域无 GET 端点）");

            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().ContainSingle($"{iface.Name}.{method} 官方参数表均含请求体字段，必须声明 [Body]");
        }
    }

    /// <summary>契约守卫 CM3：令牌绑定与注册形态。</summary>
    [Fact]
    public void MarketingInterface_ShouldBindChannelsToken_AndRegisterUnderMarketingGroup()
    {
        var token = typeof(IChannelsMarketingService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Marketing 域 7 端点消费小店 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsMarketingService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(MarketingRegistryGroupName, "必须挂 Marketing 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Marketing");
        typeof(ChannelsServiceBuilder).GetMethod("AddMarketingApi").Should().NotBeNull("必须提供 AddMarketingApi 链式注册方法");
    }

    /// <summary>契约守卫 CM4：Marketing 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void MarketingDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = MarketingJsonContext.Default;

        var domainTypes = typeof(ChannelsCouponCreateRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Marketing"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(27,
            "Marketing 域 7 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（请求 7 + 响应 5 + 嵌套 15 = 27）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Marketing 域命名空间，必须登记进 MarketingJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(MarketingRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Marketing");
        }

        // 常量类（静态常量类不参与序列化）：Marketing 域 4 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(4,
            "Marketing 域常量类 = 优惠券类型 + 推广类型 + 优惠券状态 + 用户优惠券状态");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CM5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void MarketingDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 创建优惠券。
        AssertJsonProperty<ChannelsCouponCreateRequest>("type", "优惠券类型");
        AssertJsonProperty<ChannelsCouponCreateRequest>("name", "优惠券名称");
        AssertJsonProperty<ChannelsCouponCreateRequest>("promote_info", "推广信息");
        AssertJsonProperty<ChannelsCouponCreateRequest>("discount_info", "优惠信息");
        AssertJsonProperty<ChannelsCouponCreateRequest>("receive_info", "领取信息");
        AssertJsonProperty<ChannelsCouponCreateRequest>("valid_info", "有效期信息");
        AssertJsonProperty<ChannelsCouponCreateRequest>("ext_info", "扩展信息");
        AssertJsonProperty<ChannelsCouponCreateRequest>("auto_valid_info", "自动生效信息（仅创建形态）");
        AssertJsonProperty<ChannelsCouponPromoteInfo>("promote_type", "推广类型");
        AssertJsonProperty<ChannelsCouponDiscountInfo>("discount_condition", "优惠门槛");
        AssertJsonProperty<ChannelsCouponDiscountInfo>("discount_num", "折扣数");
        AssertJsonProperty<ChannelsCouponDiscountInfo>("discount_fee", "减免金额（分）");
        AssertJsonProperty<ChannelsCouponDiscountCondition>("product_cnt", "商品件数门槛");
        AssertJsonProperty<ChannelsCouponDiscountCondition>("product_price", "商品价格门槛（分）");
        AssertJsonProperty<ChannelsCouponDiscountCondition>("product_ids", "商品 id 列表");
        AssertJsonProperty<ChannelsCouponReceiveInfo>("limit_num_one_person", "单人限领张数");
        AssertJsonProperty<ChannelsCouponReceiveInfo>("total_num", "领用总数");
        AssertJsonProperty<ChannelsCouponValidInfo>("valid_type", "有效期类型");
        AssertJsonProperty<ChannelsCouponValidInfo>("valid_day_num", "有效天数");
        AssertJsonProperty<ChannelsCouponExtInfo>("jump_product_id", "领取后跳转的商品 id");
        AssertJsonProperty<ChannelsCouponExtInfo>("notes", "备注信息");
        AssertJsonProperty<ChannelsCouponExtInfo>("valid_time", "优惠券有效时间（仅 getcoupon 响应）");
        AssertJsonProperty<ChannelsCouponExtInfo>("invalid_time", "优惠券失效时间（仅 getcoupon 响应）");
        AssertJsonProperty<ChannelsCouponAutoValidInfo>("auto_valid_type", "自动生效类型");

        // 获取优惠券详情。
        AssertJsonProperty<ChannelsGetCouponRequest>("coupon_id", "优惠券 ID");
        AssertJsonProperty<ChannelsGetCouponResponse>("coupon", "优惠券信息");
        AssertJsonProperty<ChannelsCoupon>("coupon_info", "优惠券内容信息");
        AssertJsonProperty<ChannelsCoupon>("stock_info", "库存信息");
        AssertJsonProperty<ChannelsCouponInfo>("name", "优惠券名称");
        AssertJsonProperty<ChannelsCouponStockInfo>("issued_num", "优惠券剩余量");
        AssertJsonProperty<ChannelsCouponStockInfo>("receive_num", "优惠券领用量");
        AssertJsonProperty<ChannelsCouponStockInfo>("used_num", "优惠券已用量");

        // 获取优惠券 ID 列表。
        AssertJsonProperty<ChannelsGetCouponListRequest>("status", "优惠券状态");
        AssertJsonProperty<ChannelsGetCouponListRequest>("page_ctx", "翻页上下文");
        AssertJsonProperty<ChannelsGetCouponListResponse>("coupons", "优惠券列表");
        AssertJsonProperty<ChannelsGetCouponListResponse>("total_num", "优惠券总数");
        AssertJsonProperty<ChannelsCouponIdItem>("coupon_id", "优惠券 ID（参数表 number，示例字符串——矛盾照录）");

        // 用户优惠券。
        AssertJsonProperty<ChannelsGetUserCouponRequest>("user_coupon_id", "用户优惠券 ID");
        AssertJsonProperty<ChannelsGetUserCouponRequest>("openid", "用户 openid");
        AssertJsonProperty<ChannelsGetUserCouponResponse>("unionid", "用户 unionid");
        AssertJsonProperty<ChannelsGetUserCouponResponse>("user_coupon", "用户优惠券详情（官方 2026-06-03 由 user_coupon_list 修正）");
        AssertJsonProperty<ChannelsUserCoupon>("order_id", "优惠券使用的订单 ID");
        AssertJsonProperty<ChannelsUserCouponExtInfo>("use_time", "优惠券核销时间");
        AssertJsonProperty<ChannelsGetUserCouponListResponse>("user_coupon_list", "用户优惠券列表");
        AssertJsonProperty<ChannelsUserCouponIdItem>("user_coupon_id", "用户优惠券 ID（参数表 number，示例字符串——矛盾照录）");

        // 更新内容 / 更新状态。
        AssertJsonProperty<ChannelsCouponUpdateRequest>("coupon_id", "优惠券 ID");
        AssertJsonProperty<ChannelsCouponUpdateRequest>("valid_info", "有效期信息（更新形态必填）");
        AssertJsonProperty<ChannelsCouponUpdateStatusRequest>("status", "目标状态（2=生效 4=作废 5=删除）");

        // 共用 DTO 裁决 1：createcoupon 与 updatecoupon 响应字段集完全一致（data.coupon_id）⇒ 共用 ChannelsCouponDataResponse。
        AssertJsonProperty<ChannelsCouponDataResponse>("data", "返回数据");
        typeof(IChannelsMarketingService).GetMethod(nameof(IChannelsMarketingService.CreateCouponAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsCouponDataResponse>), "createcoupon 响应走共用 data.coupon_id 形态");
        typeof(IChannelsMarketingService).GetMethod(nameof(IChannelsMarketingService.UpdateCouponAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsCouponDataResponse>), "updatecoupon 响应走共用 data.coupon_id 形态");

        // 共用 DTO 裁决 2：promote_info / discount_info / receive_info / valid_info / ext_info 在
        // 创建 / 更新请求与 getcoupon 响应 coupon_info 内字段集完全一致 ⇒ 五个嵌套对象三端点共用。
        AssertNestedShared<ChannelsCouponCreateRequest>(nameof(ChannelsCouponCreateRequest.PromoteInfo));
        AssertNestedShared<ChannelsCouponUpdateRequest>(nameof(ChannelsCouponUpdateRequest.PromoteInfo));
        AssertNestedShared<ChannelsCouponInfo>(nameof(ChannelsCouponInfo.PromoteInfo));
        typeof(ChannelsCouponCreateRequest).GetProperty(nameof(ChannelsCouponCreateRequest.ReceiveInfo))!.PropertyType
            .Should().Be(typeof(ChannelsCouponReceiveInfo));
        typeof(ChannelsCouponUpdateRequest).GetProperty(nameof(ChannelsCouponUpdateRequest.ReceiveInfo))!.PropertyType
            .Should().Be(typeof(ChannelsCouponReceiveInfo));
        typeof(ChannelsCouponInfo).GetProperty(nameof(ChannelsCouponInfo.ValidInfo))!.PropertyType
            .Should().Be(typeof(ChannelsCouponValidInfo));
        typeof(ChannelsCouponInfo).GetProperty(nameof(ChannelsCouponInfo.ExtInfo))!.PropertyType
            .Should().Be(typeof(ChannelsCouponExtInfo), "ext_info 为三端点共用超集（valid_time/invalid_time 仅响应返回）");

        // 创建与更新请求按端点分建裁决：更新形态无 auto_valid_info 且 valid_info 必填（官方参数表原文）。
        // 注：不能写 nameof(ChannelsCouponUpdateRequest.AutoValidInfo)——该属性有意不存在于更新请求。
        typeof(ChannelsCouponUpdateRequest).GetProperty("AutoValidInfo")
            .Should().BeNull("官方 updatecoupon 参数表无 auto_valid_info——创建/更新请求语义可独立演化，按端点分建");
    }

    /// <summary>契约守卫 CM6：枚举常量字面量锁定。</summary>
    [Fact]
    public void MarketingEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsCouponTypes.GoodsConditionalDiscount.Should().Be(1);
        ChannelsCouponTypes.GoodsFullReduction.Should().Be(2);
        ChannelsCouponTypes.GoodsUniformDiscount.Should().Be(3);
        ChannelsCouponTypes.GoodsDirectReduction.Should().Be(4);
        ChannelsCouponTypes.ShopConditionalDiscount.Should().Be(101);
        ChannelsCouponTypes.ShopFullReduction.Should().Be(102);
        ChannelsCouponTypes.ShopUniformDiscount.Should().Be(103);
        ChannelsCouponTypes.ShopDirectReduction.Should().Be(104);

        ChannelsCouponPromoteTypes.InShop.Should().Be(1);
        ChannelsCouponPromoteTypes.MemberCoupon.Should().Be(9);
        ChannelsCouponPromoteTypes.MemberCardGift.Should().Be(10);
        ChannelsCouponPromoteTypes.MiniProgramMemberCoupon.Should().Be(50);
        ChannelsCouponPromoteTypes.MiniProgramMemberCardGift.Should().Be(51);

        ChannelsCouponStatuses.Editing.Should().Be(1);
        ChannelsCouponStatuses.Active.Should().Be(2);
        ChannelsCouponStatuses.Expired.Should().Be(3);
        ChannelsCouponStatuses.Invalid.Should().Be(4);
        ChannelsCouponStatuses.Deleted.Should().Be(5);
        ChannelsCouponStatuses.ExpiredOrInvalid.Should().Be(200);

        ChannelsUserCouponStatuses.Active.Should().Be(100);
        ChannelsUserCouponStatuses.Expired.Should().Be(101);
        ChannelsUserCouponStatuses.Used.Should().Be(102);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }

    private static void AssertNestedShared<TRequest>(string propertyName)
    {
        typeof(TRequest).GetProperty(propertyName)!.PropertyType
            .Should().Be(typeof(ChannelsCouponPromoteInfo),
                $"{typeof(TRequest).Name}.{propertyName} 与创建/更新/详情三端点共用同一嵌套对象");
    }
}
