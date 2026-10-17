// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Promoter;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「联盟带货机构推客（promoter）」域契约守卫（缺失 API 域完善方案 P1 / CP 系列：Promoter 域 41 端点路由
/// （推客账号与分佣 7 + 机构优惠券 8 + 短视频 4 + 直播 13 + 文章 2 + 商品与商品分佣 7）+ 全 POST 恰一个
/// <c>[Body]</c> + 令牌绑定与注册形态 + DTO 字段名与共用/分建裁决 + JSON 上下文登记 + 官方枚举常量
/// + 官方参数表↔示例矛盾照录与「示例-only 子树零建模」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域是机构（headsupplier）视角的推客面</b>：令牌为<b>机构自身</b>的联盟带货机构
/// <c>access_token</c>，官方适用范围逐页均为「联盟带货机构 / 不支持云调用 / 不支持第三方平台调用」。
/// </para>
/// <para>
/// <b>「promoter」一词两义（防串线，CP1 锁定）</b>：本域 <c>/channels/ec/promoter/*</c> 的
/// <c>promoter</c> 指<b>推客（sharer）</b>；League 域 <c>/channels/ec/league/promoter/*</c> 的
/// <c>promoter</c> 指<b>带货者（talent）</b>。两前缀互不含，且本域路由不得改写进 League 前缀（反之亦然）。
/// </para>
/// <para>
/// <b>同域双视角</b>：带货者侧（<c>talent_appid</c> + <c>sharer_appid</c>）与小店关联账号侧
/// （<c>shop_appid</c> + <c>promoter_id</c> + <c>promoter_type</c>）三元组构成 8 对同语义端点
/// （短视频列表 / 短视频卡片、直播商品、直播预告分享 link、直播预告列表、直播预告二维码、直播列表、直播二维码、文章）。
/// 小店侧独有错误码 <c>10071055</c>（关联账号不存在）。
/// </para>
/// </remarks>
public class ChannelsPromoterContractGuards
{
    private const string PromoterRegistryGroupName = "Promoter";

    /// <summary>Promoter 域官方路由表（41 端点：推客账号与分佣 7 + 机构优惠券 8 + 短视频 4 + 直播 13 + 文章 2 + 商品与商品分佣 7，全部 POST，全部 <c>/channels/ec/promoter/*</c>）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] PromoterRoutes =
    {
        // 推客账号与分佣（7）。
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetBindSharerListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_bind_sharer_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetBindShopListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_bind_shop_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetBindShopPromoterListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_bind_shop_promoter_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetBindTalentListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_bind_talent_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetPromoterRegisterAndBindStatusAsync), typeof(PostAttribute), "/channels/ec/promoter/get_promoter_register_and_bind_status"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.HeadsupplierUnbindPromoterAsync), typeof(PostAttribute), "/channels/ec/promoter/headsupplier_unbind_promoter"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.SetSharerCommissionInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/set_sharer_commission_info"),
        // 机构优惠券（8）。
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetCooperativeCouponListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_cooperative_coupon_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetPublicCouponListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_public_coupon_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetCouponDetailAsync), typeof(PostAttribute), "/channels/ec/promoter/get_coupon_detail"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetCouponPromoterShareLinkAsync), typeof(PostAttribute), "/channels/ec/promoter/get_coupon_promoter_share_link"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetCouponQrCodeAsync), typeof(PostAttribute), "/channels/ec/promoter/get_coupon_qr_code"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetCouponShortLinkAsync), typeof(PostAttribute), "/channels/ec/promoter/get_coupon_short_link"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetCouponToHeadSupplierAsync), typeof(PostAttribute), "/channels/ec/promoter/get_coupon_to_headsupplier"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.SetCouponToHeadSupplierAsync), typeof(PostAttribute), "/channels/ec/promoter/set_coupon_to_headsupplier"),
        // 短视频（4）。
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetFeedListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_feed_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetFeedPromotionInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/get_feed_promotion_info"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopFeedListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_feed_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopFeedPromotionInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_feed_promotion_info"),
        // 直播（13）。
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetLiveCommissionProductListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_live_commission_product_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopLiveCommissionProductListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_live_commission_product_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetLiveNoticePromoterShareLinkAsync), typeof(PostAttribute), "/channels/ec/promoter/get_live_notice_promoter_share_link"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopLiveNoticePromoterShareLinkAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_live_notice_promoter_share_link"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetLiveNoticeRecordListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_live_notice_record_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopLiveNoticeRecordListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_live_notice_record_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetLiveNoticeRecordQrCodeAsync), typeof(PostAttribute), "/channels/ec/promoter/get_live_notice_record_qr_code"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopLiveNoticeRecordQrCodeAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_live_notice_record_qr_code"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetLiveNoticeReservationInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/get_live_notice_reservation_info"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetLiveRecordListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_live_record_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopLiveRecordListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_live_record_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetLiveRecordQrCodeAsync), typeof(PostAttribute), "/channels/ec/promoter/get_live_record_qr_code"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopLiveRecordQrCodeAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_live_record_qr_code"),
        // 文章（2）。
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetShopMpArticleListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_shop_mp_article_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetTalentBizArticleListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_talent_biz_article_list"),
        // 商品与商品分佣（7）。
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetProductPromotionLinkInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/get_product_promotion_link_info"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetProductPromotionQrCodeInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/get_product_promotion_qrcode_info"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetPromoterSingleProductPromotionInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/get_promoter_single_product_promotion_info"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetPromoteProductDetailAsync), typeof(PostAttribute), "/channels/ec/promoter/get_promote_product_detail"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetPromoteProductListAsync), typeof(PostAttribute), "/channels/ec/promoter/get_promote_product_list"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.GetSharerProductCommissionInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/get_sharer_product_commission_info"),
        (typeof(IChannelsPromoterService), nameof(IChannelsPromoterService.SetSharerProductCommissionInfoAsync), typeof(PostAttribute), "/channels/ec/promoter/set_sharer_product_commission_info"),
    };

    /// <summary>官方响应体仅 errcode/errmsg（⇒ 复用 <see cref="ChannelsResponse"/>，不建域内响应 DTO）的 4 支写端点。</summary>
    private static readonly string[] ChannelsResponseMethods =
    {
        nameof(IChannelsPromoterService.HeadsupplierUnbindPromoterAsync),
        nameof(IChannelsPromoterService.SetSharerCommissionInfoAsync),
        nameof(IChannelsPromoterService.SetCouponToHeadSupplierAsync),
        nameof(IChannelsPromoterService.SetSharerProductCommissionInfoAsync),
    };

    /// <summary>契约守卫 CP1：Promoter 域 41 端点路由必须与官方契约一致（全部 POST、全部本域前缀、与 League 前缀互斥）。</summary>
    [Fact]
    public void PromoterEndpoints_ShouldMatchOfficialRoutes()
    {
        // 41 端点 = 推客账号与分佣 7 + 机构优惠券 8 + 短视频 4 + 直播 13 + 文章 2 + 商品与商品分佣 7。
        PromoterRoutes.Should().HaveCount(41,
            "账号与分佣 7 + 优惠券 8 + 短视频 4 + 直播 13 + 文章 2 + 商品与商品分佣 7 = 41 端点" +
            "（直播 13 = 商品 2 + 预告分享 link 2 + 预告列表 2 + 预告二维码 2 + 预约人数 1 + 直播列表 2 + 直播二维码 2）");
        PromoterRoutes.Select(r => r.Route).Distinct().Should().HaveCount(41, "路由不得重复");

        PromoterRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/promoter/", StringComparison.Ordinal))
            .Should().Be(41, "41 端点全部走 /channels/ec/promoter/* 主干前缀");

        // 族内计数（路由名可辨识，防「加一支忘改统计」）。
        PromoterRoutes.Select(r => r.Route).Count(static r => r.Contains("coupon", StringComparison.Ordinal))
            .Should().Be(8, "优惠券族 8 支：合作券列表 + 公开券列表 + 券详情 + 推广 link / 二维码 / 短链 3 + 机构间给予券查询与设置 2");
        PromoterRoutes.Select(r => r.Route).Count(static r => r.Contains("live", StringComparison.Ordinal))
            .Should().Be(13, "直播族 13 支（含 live_notice 6 支）：商品 2 + 预告分享 link 2 + 预告列表 2 + 预告二维码 2 + 预约人数 1 + 直播列表 2 + 直播二维码 2");
        PromoterRoutes.Select(r => r.Route).Count(static r => r.Contains("feed", StringComparison.Ordinal))
            .Should().Be(4, "短视频族 4 支（带货者侧 2 + 小店关联账号侧 2）");
        PromoterRoutes.Select(r => r.Route).Count(static r => r.Contains("article", StringComparison.Ordinal))
            .Should().Be(2, "文章族 2 支");

        // 同域双视角：小店关联账号视角的 9 支端点统一以 get_shop_ 前缀成对出现（带货者侧对应面去前缀）。
        PromoterRoutes.Select(r => r.Route).Count(static r => r.StartsWith("/channels/ec/promoter/get_shop_", StringComparison.Ordinal))
            .Should().Be(9,
                "小店关联账号视角 9 支 = 短视频 2（feed_list / feed_promotion_info）+ 直播 6（商品 / 预告 link / 预告列表 / 预告二维码 / 直播列表 / 直播二维码）" +
                "+ 文章 1（mp_article_list）；带货者侧独有预约人数端点，绑定店铺面走 get_bind_shop_* 前缀不计入");

        foreach (var (iface, method, httpAttribute, route) in PromoterRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（下划线命名照抄官方，无驼峰改写）");
        }
    }

    /// <summary>契约守卫 CP2：全 POST + 恰一个 [Body] + 4 支写端点复用 ChannelsResponse（官方响应体只有 errcode/errmsg）。</summary>
    [Fact]
    public void PromoterEndpoints_ShouldAllBePost_WithSingleBody_AndResponseReuse()
    {
        foreach (var (iface, method, _, _) in PromoterRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（本域全部端点参数表在请求体，无 GET 端点）");

            // 令牌参数不经方法签名声明（由 [Token] 注入 Query），故签名参数只有 [Body] 请求体 + CancellationToken。
            var bodyParams = target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>()).ToList();
            bodyParams.Should().HaveCount(1,
                $"{method} 官方为 POST 且参数表含请求体，必须声明且仅声明一个 [Body] 参数" +
                "（本域无「请求体为无」的端点：官方参数表逐支均有字段）");

            var returnType = target.ReturnType.GenericTypeArguments[0];
            if (ChannelsResponseMethods.Contains(method))
            {
                returnType.Should().Be(typeof(ChannelsResponse),
                    $"{method} 官方响应体只有 errcode/errmsg ⇒ 必须复用 ChannelsResponse，不得新建仅两字段的域内 DTO");
            }
            else
            {
                returnType.Should().NotBe(typeof(ChannelsResponse),
                    $"{method} 官方响应含业务字段，必须使用域内响应 DTO");
                returnType.Namespace.Should().Be("Mud.Wechat.Channels.DataModels.Promoter",
                    $"{method} 的响应类型必须落 Promoter 域命名空间（平铺，子目录不入命名空间）");
            }
        }
    }

    /// <summary>契约守卫 CP3：令牌绑定与注册形态（机构侧 access_token + Query 注入 + Promoter 注册组）。</summary>
    [Fact]
    public void PromoterInterface_ShouldBindChannelsToken_AndRegisterUnderPromoterGroup()
    {
        var token = typeof(IChannelsPromoterService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Promoter 域 41 端点全部消费联盟带货机构 access_token");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsPromoterService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(PromoterRegistryGroupName, "必须挂 Promoter 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Promoter");
        typeof(ChannelsServiceBuilder).GetMethod("AddPromoterApi").Should().NotBeNull("必须提供 AddPromoterApi 链式注册方法");

        // 防串线：本域接口不得声明 League 前缀路由；League 域也不得声明本域前缀路由
        // （「promoter」一词两义：本域=推客 sharer，League 域=带货者 talent）。
        var ownRoutes = typeof(IChannelsPromoterService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .SelectMany(static m => m.GetCustomAttributes().OfType<HttpMethodAttribute>())
            .Select(static a => a.RequestUri ?? string.Empty)
            .ToList();
        ownRoutes.Should().NotBeEmpty("反射枚举不得空跑（正对照）");
        ownRoutes.Should().OnlyContain(static r => r.StartsWith("/channels/ec/promoter/", StringComparison.Ordinal),
            "本域路由全部在 /channels/ec/promoter/ 前缀下，不得混入 League 前缀");

        var leagueRoutes = typeof(IChannelsLeagueService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .SelectMany(static m => m.GetCustomAttributes().OfType<HttpMethodAttribute>())
            .Select(static a => a.RequestUri ?? string.Empty)
            .ToList();
        leagueRoutes.Should().NotContain(static r => r.StartsWith("/channels/ec/promoter/", StringComparison.Ordinal),
            "League 域的 /channels/ec/league/promoter/* 指带货者，与本域推客面互不改写前缀");
    }

    /// <summary>契约守卫 CP4：Promoter 域 DTO 全量登记进 AOT JSON 上下文（含常量类形态检查）。</summary>
    [Fact]
    public void PromoterDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = PromoterJsonContext.Default;

        // domainTypes 不过滤 abstract（静态常量类编译为 abstract + sealed，此处必须保留），
        // abstract 的排除只在 dtoTypes 子查询进行——否则常量类断言恒 0（前序批次踩坑）。
        var domainTypes = typeof(ChannelsPromoterGetBindSharerListRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Promoter"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        var constTypes = domainTypes
            .Where(t => t.IsAbstract && t.IsSealed)
            .ToList();

        dtoTypes.Should().HaveCount(111,
            "Promoter 域 41 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（账号与分佣 16 = 请求 7（bind_sharer / bind_shop / bind_shop_promoter / bind_talent / register_and_bind_status / unbind_promoter / set_sharer_commission）+ 响应 5 + 条目 4（bind_sharer / bind_shop / bind_talent / shop_promoter）；" +
            "优惠券 24 = 请求 6（合作券列表 / 公开券列表 / 券详情 / 推广产物共用 1 / 机构间给予查询 / 机构间给予设置）+ 响应 7 + 券子树 8（coupon / coupon_info / discount_info / discount_condition / receive_info / valid_info / ext_info / stock_info）+ 列表条目 3（cooperative / public / to_headsupplier）；" +
            "短视频 12 = 请求 4 + 响应 3（feed_list / shop_feed_list / 卡片响应共用 1）+ 条目 5（feed_export_item 请求元素 / feed_info / feed_product_info / feed_token_info / shop_feed_info）；" +
            "直播 24 = 请求 13 + 响应 7（两端点共用形态的 share_link / notice_record_list / record_list / qr_code 各计一支 + talent/shop 两侧商品列表 2 + 预约人数 1）+ 条目 4（live_commission_product / shop_live_commission_product / live_notice_record / live_record）；" +
            "文章 9 = 请求 2 + 响应 2 + 条目与子树 5（article_info / shop_article_info / talent_article_info / shop_article_product / talent_article_product）；" +
            "商品与商品分佣 26 = 请求 6（两促销产物共用 1 + 单商品推广 1 + 商品详情 1 + 商品列表 1 + 商品分佣读写 2）+ 响应 6 + 子树 14（promote_product / product_info / sku / sku_attr / sku_deliver_info / desc_info / cat / commission_info / head_supplier_info / promote_product_coupon / list_item / spu_item_condition / promotion_range / category_condition）；" +
            "共用的 PromotionRange 只计一支（SpuItemCondition 内 7 处引用不重复计数）；写端点响应复用 ChannelsResponse、不计入本域）");

        constTypes.Should().HaveCount(10,
            "Promoter 域常量类 = 小店关联账号类型 + 推客注册状态 + 分佣类型 + 优惠券类型 + 优惠券状态 + " +
            "优惠券有效期类型 + 推广计划类型 + 商品保障标 + 商品推广状态 + 商品状态 = 10");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Promoter 域命名空间，必须登记进 PromoterJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(PromoterRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Promoter");
        }

        foreach (var type in constTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>().Should().BeNull(
                $"{type.Name} 为常量类（不参与 JSON 序列化），不得标 [HttpJsonSerializable]");
        }
    }

    /// <summary>契约守卫 CP5：官方字段名与共用 / 分建裁决锁定。</summary>
    [Fact]
    public void PromoterDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // ---- 推客账号与分佣 ----
        AssertJsonProperty<ChannelsPromoterGetBindSharerListRequest>("next_key", "分页参数");
        AssertJsonProperty<ChannelsPromoterGetBindSharerListRequest>("sharer_openid", "推客 openid");
        AssertJsonProperty<ChannelsPromoterGetBindSharerListResponse>("sharer_info_list", "推客列表（官方字段名，非 sharer_list）");
        AssertJsonProperty<ChannelsPromoterBindSharerInfo>("sharer_appid", "推客 appid");
        AssertJsonProperty<ChannelsPromoterBindSharerInfo>("commission_ratio", "推客分佣比例");
        AssertJsonProperty<ChannelsPromoterBindSharerInfo>("commission_type", "分佣类型");
        AssertJsonProperty<ChannelsPromoterBindSharerInfo>("head_img_url", "推客头像（官方字段名 head_img_url，与 shop 侧 headimg_url 不同形）");

        AssertJsonProperty<ChannelsPromoterGetBindShopListResponse>("shop_list", "绑定小店列表");
        AssertJsonProperty<ChannelsPromoterGetBindShopListResponse>("has_more", "翻页标志");
        AssertJsonProperty<ChannelsPromoterBindShopInfo>("shop_head_img", "小店头像（官方字段名 shop_head_img，非 headimg_url）");

        AssertJsonProperty<ChannelsPromoterGetBindShopPromoterListRequest>("shop_appid", "小店 appid");
        AssertJsonProperty<ChannelsPromoterGetBindShopPromoterListResponse>("promoter_list", "小店关联账号列表");
        AssertJsonProperty<ChannelsPromoterShopPromoterInfo>("promoter_id", "关联账号 id");
        AssertJsonProperty<ChannelsPromoterShopPromoterInfo>("promoter_type", "关联账号类型");
        AssertJsonProperty<ChannelsPromoterShopPromoterInfo>("avatar_image_url", "关联账号头像（官方字段名，非 head_img_url）");

        AssertJsonProperty<ChannelsPromoterGetBindTalentListRequest>("talent_appid", "带货者 appid");
        AssertJsonProperty<ChannelsPromoterGetBindTalentListResponse>("talent_list", "绑定带货者列表");
        AssertJsonProperty<ChannelsPromoterBindTalentInfo>("related_finder_exportname_list", "关联视频号列表");
        AssertJsonProperty<ChannelsPromoterBindTalentInfo>("related_mp_biz_appid_list", "关联公众号列表");

        AssertJsonProperty<ChannelsPromoterGetRegisterAndBindStatusRequest>("is_simple_register", "是否简版注册");
        AssertJsonProperty<ChannelsPromoterGetRegisterAndBindStatusResponse>("register_status", "注册状态");
        AssertJsonProperty<ChannelsPromoterGetRegisterAndBindStatusResponse>("bind_status", "绑定状态");
        AssertJsonProperty<ChannelsPromoterGetRegisterAndBindStatusResponse>("register_business_type", "注册业务类型");
        AssertJsonProperty<ChannelsPromoterGetRegisterAndBindStatusResponse>("register_query_string", "注册跳转 query");
        AssertJsonProperty<ChannelsPromoterUnbindPromoterRequest>("sharer_appid", "解绑推客 appid");
        AssertJsonProperty<ChannelsPromoterSetSharerCommissionInfoRequest>("commission_ratio", "分佣比例");
        AssertJsonProperty<ChannelsPromoterSetSharerCommissionInfoRequest>("commission_type", "分佣类型（0 平台 / 1 机构自营）");

        // ---- 优惠券：三支推广产物端点共用一支请求（官方参数表逐字相同）----
        AssertJsonProperty<ChannelsPromoterCouponPromotionRequest>("coupon_id", "优惠券 id");
        AssertJsonProperty<ChannelsPromoterCouponPromotionRequest>("sharer_appid", "推客 appid");
        AssertJsonProperty<ChannelsPromoterCouponPromotionRequest>("from_head_supplier_appid", "来源机构 appid");
        AssertJsonProperty<ChannelsPromoterCouponShareLinkResponse>("promoter_share_link", "推广 link");
        AssertJsonProperty<ChannelsPromoterCouponQrCodeResponse>("qrcode_url", "二维码");
        AssertJsonProperty<ChannelsPromoterCouponShortLinkResponse>("short_link", "短链");

        AssertJsonProperty<ChannelsPromoterGetCooperativeCouponListRequest>("is_from_head_supplier", "是否机构来源优惠券");
        AssertJsonProperty<ChannelsPromoterGetCooperativeCouponListRequest>("from_head_supplier_appid", "来源机构 appid");
        AssertJsonProperty<ChannelsPromoterGetCooperativeCouponListResponse>("coupons", "优惠券列表（官方字段名复数，非 coupon_list）");
        AssertJsonProperty<ChannelsPromoterCooperativeCouponInfo>("from_head_supplier_appid", "来源机构 appid");
        AssertJsonProperty<ChannelsPromoterGetPublicCouponListResponse>("coupons", "公开券列表");
        AssertJsonProperty<ChannelsPromoterPublicCouponInfo>("coupon_id", "公开券 id（列表侧仅回 id）");

        AssertJsonProperty<ChannelsPromoterGetCouponDetailRequest>("from_head_supplier_appid", "机构券须传来源 appid");
        AssertJsonProperty<ChannelsPromoterGetCouponDetailResponse>("coupon", "券详情");
        AssertJsonProperty<ChannelsPromoterCoupon>("coupon_info", "券信息");
        AssertJsonProperty<ChannelsPromoterCoupon>("stock_info", "库存信息");
        AssertJsonProperty<ChannelsPromoterCoupon>("shop_appid", "所属小店 appid");
        AssertJsonProperty<ChannelsPromoterCouponInfo>("discount_info", "优惠信息");
        AssertJsonProperty<ChannelsPromoterCouponInfo>("receive_info", "领取信息");
        AssertJsonProperty<ChannelsPromoterCouponInfo>("valid_info", "有效期信息");
        AssertJsonProperty<ChannelsPromoterCouponInfo>("ext_info", "扩展信息");
        AssertJsonProperty<ChannelsPromoterCouponDiscountInfo>("discount_condition", "优惠门槛");
        AssertJsonProperty<ChannelsPromoterCouponDiscountInfo>("is_can_up", "是否可叠加");
        AssertJsonProperty<ChannelsPromoterCouponDiscountInfo>("can_up_times", "可叠加次数");
        AssertJsonProperty<ChannelsPromoterCouponDiscountCondition>("product_cnt", "商品件数门槛");
        AssertJsonProperty<ChannelsPromoterCouponDiscountCondition>("product_ids", "限定商品 id 列表");
        AssertJsonProperty<ChannelsPromoterCouponValidInfo>("valid_type", "有效期类型");
        AssertJsonProperty<ChannelsPromoterCouponValidInfo>("valid_day_num", "有效天数");
        AssertJsonProperty<ChannelsPromoterCouponReceiveInfo>("limit_num_one_person", "每人限领数");
        AssertJsonProperty<ChannelsPromoterCouponStockInfo>("issued_num", "发放量");
        AssertJsonProperty<ChannelsPromoterGetCouponToHeadSupplierRequest>("page_context", "翻页上下文（本端点不用 next_key）");
        AssertJsonProperty<ChannelsPromoterGetCouponToHeadSupplierRequest>("limit", "单页条数（本端点不用 page_size）");
        AssertJsonProperty<ChannelsPromoterGetCouponToHeadSupplierResponse>("page_context", "下一页上下文");
        AssertJsonProperty<ChannelsPromoterGetCouponToHeadSupplierResponse>("total_count", "总数");
        AssertJsonProperty<ChannelsPromoterCouponToHeadSupplierInfo>("to_head_supplier_appid", "目标机构 appid");
        AssertJsonProperty<ChannelsPromoterCouponToHeadSupplierInfo>("give_time", "给予时间");
        AssertJsonProperty<ChannelsPromoterSetCouponToHeadSupplierRequest>("to_head_supplier_appid", "目标机构 appid");

        // ---- 短视频 ----
        AssertJsonProperty<ChannelsPromoterGetFeedListRequest>("finder_exportusername", "视频号 exportname");
        AssertJsonProperty<ChannelsPromoterGetFeedListResponse>("feed_list", "短视频列表");
        AssertJsonProperty<ChannelsPromoterFeedInfo>("predict_commission_amount", "预计佣金");
        AssertJsonProperty<ChannelsPromoterFeedInfo>("talent_appid", "带货者 appid（响应条目内回显）");
        AssertJsonProperty<ChannelsPromoterFeedProductInfo>("product_mini_price", "商品小程序价");
        AssertJsonProperty<ChannelsPromoterGetFeedPromotionInfoRequest>("mini_program_appid", "挂载小程序 appid");
        AssertJsonProperty<ChannelsPromoterFeedExportItem>("export_id", "短视频 id");
        AssertJsonProperty<ChannelsPromoterFeedTokenInfo>("feed_token", "内嵌卡片 token");
        AssertJsonProperty<ChannelsPromoterGetShopFeedListRequest>("promoter_type", "关联账号类型");
        AssertJsonProperty<ChannelsPromoterShopFeedInfo>("export_id", "小店侧短视频 id（不回显 talent_appid）");

        // ---- 直播 ----
        AssertJsonProperty<ChannelsPromoterGetLiveCommissionProductListResponse>("product_list", "直播商品列表");
        AssertJsonProperty<ChannelsPromoterLiveCommissionProductInfo>("product_price", "带货者侧价格字段");
        AssertJsonProperty<ChannelsPromoterShopLiveCommissionProductInfo>("commission_rate", "小店侧独有佣金率字段");
        AssertJsonProperty<ChannelsPromoterGetLiveNoticePromoterShareLinkRequest>("notice_id", "直播预告 id");
        AssertJsonProperty<ChannelsPromoterGetLiveNoticeReservationInfoRequest>("live_notice_id", "预约信息端点官方字段名为 live_notice_id（同域他支为 notice_id）");
        AssertJsonProperty<ChannelsPromoterGetLiveNoticeReservationInfoResponse>("count", "预约人数");
        AssertJsonProperty<ChannelsPromoterLiveNoticeRecordInfo>("notice_id", "预告 id");
        AssertJsonProperty<ChannelsPromoterLiveNoticeRecordListResponse>("live_notice_record_list", "预告列表（官方字段名）");
        AssertJsonProperty<ChannelsPromoterLiveRecordInfo>("export_id", "直播 export_id");
        AssertJsonProperty<ChannelsPromoterLiveRecordListResponse>("live_record_list", "直播列表（官方字段名）");
        AssertJsonProperty<ChannelsPromoterGetLiveRecordQrCodeRequest>("export_id", "直播二维码按 export_id 定位");
        AssertJsonProperty<ChannelsPromoterGetShopLiveRecordListRequest>("promoter_type", "小店侧三元组之一");

        // ---- 文章 ----
        AssertJsonProperty<ChannelsPromoterGetShopMpArticleListRequest>("promoter_id", "小店关联账号 id");
        AssertJsonProperty<ChannelsPromoterGetShopMpArticleListResponse>("article_list", "文章列表");
        AssertJsonProperty<ChannelsPromoterArticleInfo>("article_title", "文章标题");
        AssertJsonProperty<ChannelsPromoterArticleInfo>("cover_image", "封面图");
        AssertJsonProperty<ChannelsPromoterArticleInfo>("promoter_share_link", "推广 link");
        AssertJsonProperty<ChannelsPromoterGetTalentBizArticleListRequest>("biz_appid", "公众号 biz_appid");
        AssertJsonProperty<ChannelsPromoterTalentArticleInfo>("product_info_list", "带货者侧商品列表字段名");
        AssertJsonProperty<ChannelsPromoterShopArticleInfo>("product_list", "小店侧商品列表字段名（与带货者侧不同名 ⇒ 分建）");
        AssertJsonProperty<ChannelsPromoterTalentArticleProductInfo>("product_mini_price", "带货者侧商品价格字段");
        AssertJsonProperty<ChannelsPromoterShopArticleProductInfo>("commission_rate", "小店侧佣金率字段");

        // ---- 商品与商品分佣 ----
        AssertJsonProperty<ChannelsPromoterProductPromotionRequest>("product_short_link", "商品短链定位");
        AssertJsonProperty<ChannelsPromoterProductPromotionRequest>("head_supplier_appid", "供货机构 appid");
        AssertJsonProperty<ChannelsPromoterProductPromotionLinkInfoResponse>("http_short_link", "http 短链");
        AssertJsonProperty<ChannelsPromoterGetSingleProductPromotionInfoRequest>("custom_token", "自定义 token（仅单商品推广信息端点有）");
        AssertJsonProperty<ChannelsPromoterGetPromoteProductDetailRequest>("get_available_coupon", "是否返回可用券");
        AssertJsonProperty<ChannelsPromoterGetPromoteProductDetailResponse>("publish_coupons", "可领公开券");
        AssertJsonProperty<ChannelsPromoterGetPromoteProductDetailResponse>("cooperative_coupons", "合作券");
        AssertJsonProperty<ChannelsPromoterGetPromoteProductListRequest>("spu_item_condition", "商品筛选条件");
        AssertJsonProperty<ChannelsPromoterGetPromoteProductListRequest>("from_head_supplier_appid", "来源机构 appid");
        AssertJsonProperty<ChannelsPromoterSpuItemCondition>("selling_price_range", "售价区间");
        AssertJsonProperty<ChannelsPromoterSpuItemCondition>("monthly_sales_range", "月销区间");
        AssertJsonProperty<ChannelsPromoterSpuItemCondition>("promote_time_range", "推广时间区间");
        AssertJsonProperty<ChannelsPromoterSpuItemCondition>("good_evaluation_ratio_range", "好评率区间");
        AssertJsonProperty<ChannelsPromoterPromotionRange>("min", "区间下界");
        AssertJsonProperty<ChannelsPromoterPromotionRange>("max", "区间上界");
        AssertJsonProperty<ChannelsPromoterProductCategoryCondition>("category_ids_1", "一级类目 id 列表");
        AssertJsonProperty<ChannelsPromoterPromoteProduct>("head_supplier_info", "供货机构信息");
        AssertJsonProperty<ChannelsPromoterPromoteProductInfo>("head_imgs", "主图（官方字段名 head_imgs）");
        AssertJsonProperty<ChannelsPromoterPromoteProductInfo>("desc_info", "图文详情");
        AssertJsonProperty<ChannelsPromoterPromoteProductInfo>("cats_v2", "类目 v2");
        AssertJsonProperty<ChannelsPromoterPromoteProductInfo>("product_promotion_link", "推广链接");
        AssertJsonProperty<ChannelsPromoterPromoteProductSku>("user_inclusive_price", "用户到手价");
        AssertJsonProperty<ChannelsPromoterPromoteProductSku>("sku_deliver_info", "sku 发货信息");
        AssertJsonProperty<ChannelsPromoterSkuDeliverInfo>("full_payment_presale_delivery_type", "全款预售类型");
        AssertJsonProperty<ChannelsPromoterProductCommissionInfo>("service_ratio", "服务费率");

        // ---- 共用 / 分建裁决 ----
        // 共用：三支券推广产物端点（share_link / qr_code / short_link）官方参数表逐字相同 ⇒ 一支请求。
        CouponPromotionRequestUsedByThreeEndpoints();
        // 共用：直播四类响应（分享 link / 预告列表 / 直播列表 / 二维码）在 talent 与 shop 两视角同形 ⇒ 各一支响应类型被两端点复用。
        typeof(IChannelsPromoterService).GetMethod(nameof(IChannelsPromoterService.GetLiveNoticePromoterShareLinkAsync))!
            .ReturnType.GenericTypeArguments[0].Should().Be(
                typeof(IChannelsPromoterService).GetMethod(nameof(IChannelsPromoterService.GetShopLiveNoticePromoterShareLinkAsync))!.ReturnType.GenericTypeArguments[0],
                "直播预告分享 link 两端点响应同形 ⇒ 共用");
        typeof(IChannelsPromoterService).GetMethod(nameof(IChannelsPromoterService.GetLiveRecordQrCodeAsync))!
            .ReturnType.GenericTypeArguments[0].Should().Be(
                typeof(IChannelsPromoterService).GetMethod(nameof(IChannelsPromoterService.GetShopLiveRecordQrCodeAsync))!.ReturnType.GenericTypeArguments[0],
                "直播二维码两端点响应同形 ⇒ 共用（notice 侧亦共用同一支）");

        // 分建：券列表条目三支（cooperative 回 from_head_supplier_appid / public 仅回 coupon_id /
        // to_headsupplier 回 to_head_supplier_appid + give_time）字段集互不相同 ⇒ 不复用。
        typeof(ChannelsPromoterCooperativeCouponInfo).Should().NotBeSameAs(typeof(ChannelsPromoterPublicCouponInfo));
        typeof(ChannelsPromoterCouponToHeadSupplierInfo).Should().NotBeSameAs(typeof(ChannelsPromoterCooperativeCouponInfo));

        // 分建：feed 条目 talent 侧回 talent_appid、shop 侧不回 ⇒ 两支不合并。
        typeof(ChannelsPromoterFeedInfo).GetProperty("TalentAppId").Should().NotBeNull();
        typeof(ChannelsPromoterShopFeedInfo).GetProperties().Should().NotContain(
            static p => p.Name == "TalentAppId", "小店侧短视频条目官方参数表无 talent_appid ⇒ 分建，不得顺手合并");

        // 分建：Promoter 券子树 vs Marketing 域同族券（跨域不共用，避免 Marketing 调整误伤本域契约）。
        // 官方推客侧 coupon 比营销侧多回 shop_appid ⇒ 两支字段集不同，天然分建。
        typeof(ChannelsPromoterCoupon).Should().NotBeSameAs(typeof(Mud.Wechat.Channels.DataModels.Marketing.ChannelsCoupon));
        JsonPropertyNamesOf<ChannelsPromoterCoupon>().Should().Contain(
            JsonPropertyNamesOf<Mud.Wechat.Channels.DataModels.Marketing.ChannelsCoupon>().ToArray(),
            "推客侧 coupon 是营销侧 coupon 的超集（多 shop_appid）⇒ 同族不同形，跨域不共用");

        // 分建：ProductPromotionRequest（两支促销产物端点共用）vs GetSingleProductPromotionInfoRequest（多一支 custom_token）。
        JsonPropertyNamesOf<ChannelsPromoterGetSingleProductPromotionInfoRequest>()
            .Should().Contain("custom_token").And.HaveCount(JsonPropertyNamesOf<ChannelsPromoterProductPromotionRequest>().Count() + 1,
                "单商品推广信息端点官方参数表多 custom_token 一支 ⇒ 分建，不得给共用请求加字段");

        // 共用：PromotionRange 一支被 SpuItemCondition 的 7 个区间字段复用（不建 7 支）。
        typeof(ChannelsPromoterSpuItemCondition).GetProperties()
            .Count(static p => p.PropertyType == typeof(ChannelsPromoterPromotionRange))
            .Should().Be(7, "售价/月销/服务费/佣金/推广时间/店铺评分/好评率七个区间共用同一支 PromotionRange");
        // 共用：feed_list 请求元素（FeedExportItem）在两端点请求体共用。
        typeof(ChannelsPromoterGetFeedPromotionInfoRequest).GetProperty("FeedList")!
            .PropertyType.GenericTypeArguments[0].Should().Be(
                typeof(ChannelsPromoterGetShopFeedPromotionInfoRequest).GetProperty("FeedList")!.PropertyType.GenericTypeArguments[0],
                "短视频卡片两端点请求的 feed_list 元素同形 ⇒ 共用 FeedExportItem");
    }

    /// <summary>契约守卫 CP6：官方枚举常量字面量锁定。</summary>
    [Fact]
    public void PromoterEnumConstants_ShouldLockOfficialLiteralValues()
    {
        // 小店关联账号类型（promoter_type）。
        ChannelsPromoterShopPromoterTypes.ChannelsVideo.Should().Be(1, "官方原文「1 视频号」");
        ChannelsPromoterShopPromoterTypes.OfficialAccount.Should().Be(4, "官方原文「4 公众号」");
        ChannelsPromoterShopPromoterTypes.ServiceAccount.Should().Be(5, "官方原文「5 服务号」");

        // 推客注册状态。
        ChannelsPromoterRegisterStatuses.NotRegistered.Should().Be(0, "官方原文「0 未注册」");
        ChannelsPromoterRegisterStatuses.Registering.Should().Be(1, "官方原文「1 注册中」");
        ChannelsPromoterRegisterStatuses.Registered.Should().Be(2, "官方原文「2 已注册」");
        ChannelsPromoterRegisterStatuses.NeedRealNameAndBankCard.Should().Be(3, "官方原文「3 需要实名与银行卡」");

        // 推客分佣类型（set_sharer_commission_info.commission_type）。
        ChannelsPromoterCommissionTypes.Platform.Should().Be(0, "官方原文「0 平台分佣」");
        ChannelsPromoterCommissionTypes.HeadSupplierSelf.Should().Be(1, "官方原文「1 机构自营分佣」");

        // 优惠券类型（商品券 1~4 与店铺券 101~104 两套同族值域 ⇒ 一支常量类分节照录）。
        ChannelsPromoterCouponTypes.ProductConditionDiscount.Should().Be(1, "官方原文「1 商品满减券（条件优惠）」");
        ChannelsPromoterCouponTypes.ProductFullReduce.Should().Be(2, "官方原文「2 商品满减」");
        ChannelsPromoterCouponTypes.ProductUniformDiscount.Should().Be(3, "官方原文「3 商品统一折扣」");
        ChannelsPromoterCouponTypes.ProductDirectReduce.Should().Be(4, "官方原文「4 商品直减」");
        ChannelsPromoterCouponTypes.ShopConditionDiscount.Should().Be(101, "官方原文「101 店铺条件优惠」");
        ChannelsPromoterCouponTypes.ShopFullReduce.Should().Be(102, "官方原文「102 店铺满减」");
        ChannelsPromoterCouponTypes.ShopUniformDiscount.Should().Be(103, "官方原文「103 店铺统一折扣」");
        ChannelsPromoterCouponTypes.ShopDirectReduce.Should().Be(104, "官方原文「104 店铺直减」");

        // 优惠券状态。
        ChannelsPromoterCouponStatuses.Editing.Should().Be(1, "官方原文「1 编辑中」");
        ChannelsPromoterCouponStatuses.Effective.Should().Be(2, "官方原文「2 生效中」");
        ChannelsPromoterCouponStatuses.Expired.Should().Be(3, "官方原文「3 已过期」");
        ChannelsPromoterCouponStatuses.Invalidated.Should().Be(4, "官方原文「4 已作废」");
        ChannelsPromoterCouponStatuses.Deleted.Should().Be(5, "官方原文「5 已删除」");

        // 优惠券有效期类型。
        ChannelsPromoterCouponValidTypes.TimeRange.Should().Be(1, "官方原文「1 固定时间区间」");
        ChannelsPromoterCouponValidTypes.ValidDays.Should().Be(2, "官方原文「2 领券后有效天数」");

        // 推广计划类型（plan_type）。
        ChannelsPromoterPlanTypes.Directed.Should().Be(1, "官方原文「1 定向计划」");
        ChannelsPromoterPlanTypes.Public.Should().Be(2, "官方原文「2 公开计划」");
        ChannelsPromoterPlanTypes.HeadSupplierDirected.Should().Be(3, "官方原文「3 机构定向计划」");
        ChannelsPromoterPlanTypes.HeadSupplierNormal.Should().Be(4, "官方原文「4 机构普通计划」");

        // 商品保障标（flags）。
        ChannelsPromoterProductFlags.SevenDayReturn.Should().Be(0, "官方原文「0 7 天无理由」");
        ChannelsPromoterProductFlags.FreightInsurance.Should().Be(1, "官方原文「1 运费险」");
        ChannelsPromoterProductFlags.Brand.Should().Be(2, "官方原文「2 品牌」（已废弃值保留以兼容回包）");
        ChannelsPromoterProductFlags.BuyWithConfidence.Should().Be(3, "官方原文「3 放心买」");
        ChannelsPromoterProductFlags.DamageReturn.Should().Be(4, "官方原文「4 损坏包退」");
        ChannelsPromoterProductFlags.FakeOnePayThree.Should().Be(5, "官方原文「5 假一赔三」");
        ChannelsPromoterProductFlags.PayAfterUse.Should().Be(6, "官方原文「6 先用后付」");
        ChannelsPromoterProductFlags.FreeShipping.Should().Be(7, "官方原文「7 包邮」");

        // 商品推广状态（promote_product.status）。
        ChannelsPromoterProductPromotionStatuses.OnShelf.Should().Be(1, "官方原文「1 上架中」");
        ChannelsPromoterProductPromotionStatuses.OffShelf.Should().Be(2, "官方原文「2 已下架」");
        ChannelsPromoterProductPromotionStatuses.PendingSelection.Should().Be(3, "官方原文「3 待 selection」（待选品）");
        ChannelsPromoterProductPromotionStatuses.Retired.Should().Be(5, "官方原文「5 已终止」");

        // 商品状态（product_info.status，与上面的「推广状态」是两张表 ⇒ 常量类分建）。
        ChannelsPromoterProductStatuses.Initial.Should().Be(0, "官方原文「0 初始值」");
        ChannelsPromoterProductStatuses.OnSale.Should().Be(5, "官方原文「5 在售」");
        ChannelsPromoterProductStatuses.RecycleBin.Should().Be(6, "官方原文「6 放入回收站」");
        ChannelsPromoterProductStatuses.Deleted.Should().Be(9, "官方原文「9 已删除」");
        ChannelsPromoterProductStatuses.SelfOffShelf.Should().Be(11, "官方原文「11 自己下架」");
        ChannelsPromoterProductStatuses.ViolationOffShelf.Should().Be(13, "官方原文「13 违规下架」");
        ChannelsPromoterProductStatuses.DepositInsufficient.Should().Be(14, "官方原文「14 保证金不足额」");
        ChannelsPromoterProductStatuses.BrandExpired.Should().Be(15, "官方原文「15 品牌授权到期」");
        ChannelsPromoterProductStatuses.Banned.Should().Be(20, "官方原文「20 封禁中」");
        new[] { ChannelsPromoterProductPromotionStatuses.OnShelf, ChannelsPromoterProductPromotionStatuses.OffShelf }
            .Should().Equal(new[] { 1, 2 }, "推广状态 1/2 与商品状态表无对应关系 ⇒ 两张表不得合并");
    }

    /// <summary>契约守卫 CP7：官方参数表↔代码示例矛盾照录 + 「示例-only 子树零建模」。</summary>
    [Fact]
    public void PromoterDataModels_ShouldLockDocContradictionsAndNotModelExampleOnlySubtrees()
    {
        // ① commission_info.status：参数表标 string、枚举与示例均为 number ⇒ 按枚举取 int?。
        typeof(ChannelsPromoterProductCommissionInfo).GetProperty("Status")!
            .PropertyType.Should().Be(typeof(int?),
                "官方参数表标 string、枚举表与代码示例标 number ⇒ 取 number（int?），不得被表格带回 string");

        // ② category_id：参数表标 number、示例值 11099070301657575487 超 Int64 安全范围 ⇒ 取 string。
        typeof(ChannelsPromoterProductCategoryCondition).GetProperty("CategoryId")!
            .PropertyType.Should().Be(typeof(string),
                "官方示例类目 id 为 11099070301657575487（超 JS 安全整数）⇒ 按示例取字符串，避免精度丢失");
        // category_ids_1/2/3 官方参数表类型列为「-」⇒ 按示例数组取 List<string>。
        typeof(ChannelsPromoterProductCategoryCondition).GetProperty("CategoryIds1")!
            .PropertyType.Should().Be(typeof(List<string>), "官方类型列为「-」但示例为字符串数组 ⇒ 照示例建表");

        // ③ sku_attrs：参数表标 object、示例为 array ⇒ 取列表形态。
        typeof(ChannelsPromoterPromoteProductSku).GetProperty("SkuAttrs")!
            .PropertyType.Should().Be(typeof(List<ChannelsPromoterPromoteProductSkuAttr>),
                "官方参数表标 object、代码示例为数组 ⇒ 按示例取 List");

        // ④ set_sharer_product_commission_info.commission_ratio 官方参数表标 string，
        //    而读取端点 get_sharer_product_commission_info 回 number ⇒ 读写两侧形态照录、不互相「对齐」。
        typeof(ChannelsPromoterSetSharerProductCommissionInfoRequest).GetProperty("CommissionRatio")!
            .PropertyType.Should().Be(typeof(string), "写入侧官方参数表标 string ⇒ 照录");
        typeof(ChannelsPromoterGetSharerProductCommissionInfoResponse).GetProperty("CommissionRatio")!
            .PropertyType.Should().Be(typeof(long?), "读取侧官方参数表标 number ⇒ 照录（与写入侧不同形是官方矛盾，非本仓设计）");

        // ⑤ product_id：列表侧（PromoteProductListItem）官方标 string、详情侧（PromoteProduct）标 number ⇒ 两支分建、不合并。
        typeof(ChannelsPromoterPromoteProductListItem).GetProperty("ProductId")!
            .PropertyType.Should().Be(typeof(string), "列表侧官方标 string ⇒ 照录");
        typeof(ChannelsPromoterPromoteProduct).GetProperty("ProductId")!
            .PropertyType.Should().Be(typeof(long?), "详情侧官方标 number ⇒ 照录");
        typeof(ChannelsPromoterGetPromoteProductDetailRequest).GetProperty("ProductId")!
            .PropertyType.Should().Be(typeof(long), "详情请求侧 product_id 官方为必填 number ⇒ 非可空");

        // ⑥ ext_info 的 valid_time / invalid_time 官方说明文字互换（同一页两条描述对调）⇒ 字段名照官方、语义以字段名为准。
        AssertJsonProperty<ChannelsPromoterCouponExtInfo>("valid_time", "官方字段名（描述文字与 invalid_time 互换，见接口 remarks）");
        AssertJsonProperty<ChannelsPromoterCouponExtInfo>("invalid_time", "官方字段名（描述文字与 valid_time 互换，见接口 remarks）");

        // ⑦ get_coupon_detail 官方响应「只有 coupon」，但示例与参数表同时给出 next_key / has_more ⇒ 照录保留翻页字段。
        AssertJsonProperty<ChannelsPromoterGetCouponDetailResponse>("next_key", "官方参数表照录（本域唯一「详情端点带回翻页字段」）");
        AssertJsonProperty<ChannelsPromoterGetCouponDetailResponse>("has_more", "官方参数表照录");
        // ⑧ get_bind_sharer_list 反而不回 has_more（同域其余 next_key 分页端点均有）⇒ 负断言防「顺手补齐」。
        typeof(ChannelsPromoterGetBindSharerListResponse).GetProperties().Should().NotContain(
            static p => p.Name == "HasMore", "官方 get_bind_sharer_list 响应参数表无 has_more ⇒ 不得按同域惯例补齐");

        // ⑨ get_sharer_product_commission_info 官方示例回 commission_ratio = 10（与「1000000 = 100%」的量纲说明冲突）
        //    ⇒ 数值原样透传（long?），不在 SDK 侧做换算。

        // ⑩ notice_id vs live_notice_id 一字两表：预约信息端点官方用 live_notice_id，其余预告端点用 notice_id ⇒ 分建请求。
        typeof(ChannelsPromoterGetLiveNoticeReservationInfoRequest).GetProperties().Should().ContainSingle(
            static p => p.Name == "LiveNoticeId", "预约信息端点官方字段名为 live_notice_id");
        typeof(ChannelsPromoterGetLiveNoticeReservationInfoRequest).GetProperties().Should().NotContain(
            static p => p.Name == "NoticeId", "预约信息端点官方无 notice_id ⇒ 不并入他支请求");

        // ⑪ get_shop_feed_promotion_info 官方参数表把 promoter_type 标为「选填」，而同族他支均必填 ⇒ 取可空 int? 照录。
        typeof(ChannelsPromoterGetShopFeedPromotionInfoRequest).GetProperty("PromoterType")!
            .PropertyType.Should().Be(typeof(int?), "官方本端点参数表 promoter_type 标选填 ⇒ 可空照录");
        typeof(ChannelsPromoterGetShopFeedListRequest).GetProperty("PromoterType")!
            .PropertyType.Should().Be(typeof(int), "同族列表端点 promoter_type 官方必填 ⇒ 非可空");

        // ⑫「示例-only 子树零建模」：get_promote_product_detail 的代码示例出现 express_info / extra_service /
        // statistical_data / normal_commission_info 四棵子树，但官方参数表未列 ⇒ 不建 DTO、不留字段
        //（Delivery 域同形态裁决：以参数表为准，示例多出的字段不进契约面）。
        AssertNoJsonProperty<ChannelsPromoterPromoteProduct>("express_info", "官方参数表未列，仅示例出现 ⇒ 零建模");
        AssertNoJsonProperty<ChannelsPromoterPromoteProduct>("extra_service", "官方参数表未列，仅示例出现 ⇒ 零建模");
        AssertNoJsonProperty<ChannelsPromoterPromoteProductInfo>("statistical_data", "官方参数表未列，仅示例出现 ⇒ 零建模");
        AssertNoJsonProperty<ChannelsPromoterProductCommissionInfo>("normal_commission_info", "官方参数表未列，仅示例出现 ⇒ 零建模");
        // 正对照：同页参数表列出的字段在场，证明扫描面覆盖该 DTO。
        AssertJsonProperty<ChannelsPromoterProductCommissionInfo>("service_ratio", "参数表列出的正对照字段");

        // ⑬ 官方路由名 get_product_promotion_qrcode_info（全小写 qrcode）⇒ 照抄，不得写成 qrCode 驼峰。
        PromoterRoutes.Select(static r => r.Route).Should().Contain("/channels/ec/promoter/get_product_promotion_qrcode_info",
            "官方路由为 qrcode 连写，同域券端点则写作 get_coupon_qr_code 下划线分词 ⇒ 两处形态互不统一，各自照抄");
    }

    private static void CouponPromotionRequestUsedByThreeEndpoints()
    {
        string[] shareLinkMethods =
        {
            nameof(IChannelsPromoterService.GetCouponPromoterShareLinkAsync),
            nameof(IChannelsPromoterService.GetCouponQrCodeAsync),
            nameof(IChannelsPromoterService.GetCouponShortLinkAsync),
        };

        var bodyTypes = shareLinkMethods
            .Select(static m => typeof(IChannelsPromoterService).GetMethod(m)!
                .GetParameters().Single(static p => p.GetCustomAttributes<BodyAttribute>().Any()).ParameterType)
            .Distinct()
            .ToList();

        bodyTypes.Should().ContainSingle()
            .Which.Should().Be(typeof(ChannelsPromoterCouponPromotionRequest),
                "三支优惠券推广产物端点官方参数表逐字相同（coupon_id + sharer_appid + from_head_supplier_appid）⇒ 共用一支请求");
    }

    private static IEnumerable<string> JsonPropertyNamesOf<T>() =>
        typeof(T).GetProperties().Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name!);

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }

    private static void AssertNoJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeFalse($"{typeof(T).Name} 不得含 '{jsonName}'（{because}）");
    }
}
