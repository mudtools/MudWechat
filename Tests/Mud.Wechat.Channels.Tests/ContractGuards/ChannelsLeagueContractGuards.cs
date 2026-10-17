// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Funds;
using Mud.Wechat.Channels.DataModels.League;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「优选联盟」域契约守卫（设计方案 v1 P2 / CLG 系列：League 域 41 端点路由（商家侧带货者 4 +
/// 商品操作 7 + 联盟带货机构资金 3 + 机构商品 6 + 机构商家合作 3 + 机构佣金单 2 + 机构橱窗与授权 6 + 机构商品订阅 3 + 机构达人佣金计划 2 +
/// 机构帮卖供货 2 + 机构线索订单 2 + 机构直播保护 1）+ 全 POST + 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 +
/// 官方改版矛盾照录 + 枚举常量（含「type」一字两表辨析）+ 机构侧资金与 Funds 域分建裁决 + 机构侧已脱树旧路由零声明）。
/// </summary>
/// <remarks>
/// <para>
/// <b>带货者改版（官方变更日志，守卫措辞锁定）</b>：2026-06-17 官方删除
/// <c>finder_ids</c>/<c>promoter_ids</c>/<c>fail_promoter_ids</c>/<c>fail_finder_ids</c>/<c>info_id</c>/
/// <c>last_buffer</c> 等旧字段、新增 <c>talent_appids</c>/<c>promotion_plan_id</c>/<c>fail_talent_appids</c>
/// ——本域字段以官方<b>参数表</b>为准（talent 系命名），代码示例中残留的旧字段名在 remarks 照录，不得回潮。
/// </para>
/// <para>
/// <b>「type」一字两表（防串线）</b>：请求侧「获取商品推广类别」（100 全部带货者计划 / 300 指定带货者计划，
/// <see cref="ChannelsLeaguePlanQueryTypes"/>）与计划/佣金侧「普通/定向/专属」三分类
/// （1/2/3，<see cref="ChannelsLeaguePromotionTypes"/>）是两套枚举；另有
/// <see cref="ChannelsLeaguePromotionPlanTypes"/>（100 全部带货者 / 200 圈选带货者 / 300 指定带货者）
/// ——三个「type 语义」常量类互不混用。
/// </para>
/// </remarks>
public class ChannelsLeagueContractGuards
{
    private const string LeagueRegistryGroupName = "League";

    /// <summary>League 域官方路由表（41 端点：商家侧带货者 4 + 商品操作 7 + 机构资金 3 + 机构商品 6 + 机构商家合作 3 + 机构佣金单 2 + 机构橱窗与授权 6 + 机构商品订阅 3 + 机构达人佣金计划 2 + 机构帮卖供货 2 + 机构线索订单 2 + 机构直播保护 1，全部 POST，全部 /channels/ec/league/*）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] LeagueRoutes =
    {
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetPromoterAsync), typeof(PostAttribute), "/channels/ec/league/promoter/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetPromoterListAsync), typeof(PostAttribute), "/channels/ec/league/promoter/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetTalentProductDataListAsync), typeof(PostAttribute), "/channels/ec/league/talent/getproductdatalist"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.UpdateTalentAsync), typeof(PostAttribute), "/channels/ec/league/talent/update"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.BatchAddLeagueItemAsync), typeof(PostAttribute), "/channels/ec/league/item/batchadd"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetLeagueItemAsync), typeof(PostAttribute), "/channels/ec/league/item/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.BatchAddHeadSupplierItemAsync), typeof(PostAttribute), "/channels/ec/league/item/headsupplier/batchadd"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetLeagueItemListAsync), typeof(PostAttribute), "/channels/ec/league/item/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.UpdateLeagueItemAsync), typeof(PostAttribute), "/channels/ec/league/item/upd"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.ListSpuPromotionPlansAsync), typeof(PostAttribute), "/channels/ec/league/promotion/plan/list"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.ListSpuPromotionTalentsAsync), typeof(PostAttribute), "/channels/ec/league/promotion/talent/list"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierBalanceAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/funds/balance/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierFundsFlowListAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/funds/flowlist/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierFundsFlowDetailAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/funds/flowdetail/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.AddHeadSupplierCooperativeItemAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/cooperativeitem/add"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierCooperativeItemListAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/cooperativeitem/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierItemPromotionDetailAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/item/promotiondetail/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierSelectionProductsAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/selectionproducts/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierProductDetailAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/productdetail/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierProductActivityAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/product/getactivity"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierShopAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/shop/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierShopListAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/shop/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.UpdateHeadSupplierRelationAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/headsupplierrelation/update"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierOrderListAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/order/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierOrderAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/order/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.AddHeadSupplierWindowAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/window/add"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierWindowListAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/window/getall"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierWindowProductDetailAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/window/getdetail"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.RemoveHeadSupplierWindowAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/window/remove"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierWindowAuthAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/windowauth/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierWindowAuthStatusAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/windowauth/status/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierSubscribeProductsAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/subscription/getsubscribe"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.SubscribeHeadSupplierProductAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/subscription/subscribe"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.UnsubscribeHeadSupplierProductAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/subscription/unsubscribe"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.AddHeadSupplierSubItemAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/subitem/add"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierSubItemListAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/subitem/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.AddHeadSupplierHelpSellItemAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/secondheadsupplieritem/add"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.UpdateHeadSupplierHelpSellItemAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/secondheadsupplieritem/update"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierClueAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/clue/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierClueListAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/clue/list/get"),
        (typeof(IChannelsLeagueService), nameof(IChannelsLeagueService.GetHeadSupplierLiveProtectionQrCodeAsync), typeof(PostAttribute), "/channels/ec/league/headsupplier/liveprotection/getqrcode"),
    };

    /// <summary>契约守卫 CLG1：League 域 41 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void LeagueEndpoints_ShouldMatchOfficialRoutes()
    {
        // 41 端点 = 商家侧带货者 4 + 商品操作 7 + 机构侧（headsupplier）资金 3 + 商品 6 + 商家合作 3 + 佣金单 2 + 橱窗与授权 6 + 订阅 3 + 达人佣金计划 2 + 帮卖供货 2 + 线索订单 2 + 直播保护 1。
        LeagueRoutes.Should().HaveCount(41, "带货者 4 + 商品操作 7 + 机构资金 3 + 机构商品 6 + 机构商家合作 3 + 机构佣金单 2 + 机构橱窗与授权 6 + 机构订阅 3 + 机构达人佣金计划 2 + 机构帮卖供货 2 + 机构线索订单 2 + 机构直播保护 1 = 41 端点");
        LeagueRoutes.Select(r => r.Route).Distinct().Should().HaveCount(41);

        LeagueRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/league/", StringComparison.Ordinal))
            .Should().Be(41, "41 端点全部走 /channels/ec/league/* 主干前缀");

        // 两视角前缀分布：商家侧优选联盟（/channels/ec/league/{promoter,talent,item,promotion}/*）与
        // 联盟带货机构侧（/channels/ec/league/headsupplier/*）在同一域内并存，前缀不得互相改写。
        LeagueRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/league/headsupplier/", StringComparison.Ordinal))
            .Should().Be(30, "机构侧 headsupplier 族 30 端点（资金 3 + 商品获取 3 + 选品广场/商品详情/活动补贴 3 + 商家合作 3 + 佣金单 2 + 橱窗 4 + 橱窗授权 2 + 订阅 3 + 达人佣金计划 2 + 帮卖供货 2 + 线索订单 2 + 直播保护 1，官方「联盟带货机构」账号类型面）");
        LeagueRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/league/", StringComparison.Ordinal) && !r.StartsWith("/channels/ec/league/headsupplier/", StringComparison.Ordinal))
            .Should().Be(11, "商家侧优选联盟 11 端点（小店账号类型面）");

        foreach (var (iface, method, httpAttribute, route) in LeagueRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（下划线路由 getproductdatalist 等照抄官方）");
        }
    }

    /// <summary>契约守卫 CLG2：League 域 41 端点全部为 POST；官方空请求体端点不得携带 [Body]，其余恰一个。</summary>
    [Fact]
    public void LeagueEndpoints_ShouldAllBePost_WithBodyRules()
    {
        // 官方参数表请求体为「无」的端点（机构余额 / 直播大场保护提报二维码）——余额页注意事项虽写「传空的 json 串即可」，
        // 但不得据此人为造请求体 DTO。
        var emptyBodyMethods = new[]
        {
            nameof(IChannelsLeagueService.GetHeadSupplierBalanceAsync),
            nameof(IChannelsLeagueService.GetHeadSupplierLiveProtectionQrCodeAsync),
        };

        foreach (var (iface, method, _, _) in LeagueRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（优选联盟与机构资金族均无 GET 端点）");

            var bodyParams = target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>()).ToList();
            if (emptyBodyMethods.Contains(method))
            {
                bodyParams.Should().BeEmpty($"{method} 官方请求体参数表为「无」——不得声明 [Body]");
            }
            else
            {
                bodyParams.Should().HaveCount(1,
                    $"{method} 官方为 POST 且参数表含请求体，必须声明且仅声明一个 [Body] 参数");
            }
        }
    }

    /// <summary>契约守卫 CLG3：令牌绑定与注册形态。</summary>
    [Fact]
    public void LeagueInterface_ShouldBindChannelsToken_AndRegisterUnderLeagueGroup()
    {
        var token = typeof(IChannelsLeagueService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("League 域 41 端点消费 channels access_token（商家侧为小店 access_token、机构侧为联盟带货机构 access_token，官方契约同一令牌类型）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsLeagueService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(LeagueRegistryGroupName, "必须挂 League 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("League");
        typeof(ChannelsServiceBuilder).GetMethod("AddLeagueApi").Should().NotBeNull("必须提供 AddLeagueApi 链式注册方法");
    }

    /// <summary>契约守卫 CLG4：League 域 DTO 全量登记进 AOT JSON 上下文（含常量类形态检查）。</summary>
    [Fact]
    public void LeagueDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = LeagueJsonContext.Default;

        // domainTypes 不过滤 abstract（静态常量类编译为 abstract + sealed，此处必须保留），
        // abstract 的排除只在 dtoTypes 子查询进行——否则常量类断言恒 0（前序批次踩坑）。
        var domainTypes = typeof(ChannelsGetPromoterRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.League"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        var constTypes = domainTypes
            .Where(t => t.IsAbstract && t.IsSealed)
            .ToList();

        dtoTypes.Should().HaveCount(149,
            "League 域 41 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（商家侧：请求 14（含 2 个列表元素 + 1 个更新形态嵌套）+ 响应根 8 + 嵌套/列表元素 19 = 41；" +
            "机构侧资金 4：余额响应 + 流水列表请求 + 流水详情响应 + funds_flow 元素；" +
            "机构侧商品获取 13：合作商品添加请求/响应 2 + 合作商品列表（请求 + 上架时间区间 + 条目 + 响应）4 + " +
            "推广参数详情（请求 + 响应 + item + 佣金信息 + 普通推广佣金 + 供货机构信息 + 合作信息）7；" +
            "机构侧选品广场 5：列表请求 + 属性筛选 + 类目筛选 + 条目 + 响应；" +
            "机构侧活动补贴 3：请求 + 响应 + product_activity_list 元素；" +
            "机构侧商品详情 16：请求 + 响应 + item + product_info + sku + sku 预售 + 类目 + 服务保障 + " +
            "春节不打烊配置 + 时间区间（与佣金单时间范围同形共用）+ 物流信息 + 国标地址 + 条件包邮 + 计费方法 + 商品视频 + 店铺信息；" +
            "机构侧商家合作 9：商家详情（请求 + 响应 + shop_detail + base_info + data_info）5 + 商家列表（请求 + 响应 + 列表项）3 + 合作关系更新请求 1（响应仅 errcode/errmsg ⇒ 复用 ChannelsResponse，不计入）；" +
            "机构侧佣金单 20：列表（请求 + 响应 + 条目）3 + 详情（请求 + 响应 + 佣金单 + order_detail + 商家/商品/订单/分佣/买家信息）9 + 分佣侧（达人 / 推客 / 带货达人 / 推广参数 / 推广视频号 / 推广公众号 / 推广带货助手 / 机构分佣）8；" +
            "机构侧橱窗与授权 14：橱窗（添加请求 + 列表请求/响应/条目 + 详情请求/响应/product_detail/product_info + 移除请求）9 + " +
            "橱窗授权（请求 + 响应 + auth_info + 状态请求 + 状态响应）5；" +
            "机构侧商品订阅 5：订阅列表（请求 + filter + 响应 + 订阅商品条目）4 + 订阅/取消订阅共用请求 1（两端点响应仅 errcode/errmsg ⇒ 复用 ChannelsResponse，不计入）；" +
            "机构侧达人佣金计划 5：配置佣金率（请求 + 响应）2 + 计划列表（请求 + 响应 + 条目）3（计划状态为常量类，计入常量断言）；" +
            "机构侧帮卖供货 2：供货添加请求 + 推广计划更新请求（两端点响应仅 errcode/errmsg ⇒ 复用 ChannelsResponse，不计入）；" +
            "机构侧线索订单 11：详情（请求 + 响应 + commssion_clue + clue_detail + 商家/商品/订单/推客信息）8（买家信息复用佣金单 OrderBuyerInfo、时间区间复用同域 TimeRange，均不计入）+ 列表（请求 + 响应 + 条目）3；" +
            "机构侧直播保护 1：提报二维码响应（请求体官方为「无」⇒ 不建请求 DTO）；" +
            "流水列表响应与流水详情请求复用 Funds 域既有类型，商品详情的 desc_info 与 sku_attrs 复用 Product 域既有类型，均不计入本域）");

        constTypes.Should().HaveCount(30,
            "League 域常量类 = 请求推广类别 + 推广三分类 + 计划状态 + 编辑带货者类型 + 更新操作类别 + " +
            "推广计划类型 + 机构计划类型 + 带货者等级 + 推广关系状态 + 商品数据类型 + 机构资金类型 + " +
            "选品广场计划类型 + 选品广场佣金类型 + 供货来源 + 保障标 + 月销量档位 + 发货时间期限 + " +
            "商家合作状态 + 合作关系对象类型 + 机构侧关系操作 + 店铺侧关系操作 + " +
            "佣金单状态 + 交易订单状态 + 联盟技术服务费计算状态 + 分佣推广渠道 + 推客推广类型 + 橱窗授权状态 + " +
            "达人佣金计划状态 + 帮卖计划类型 + 帮卖操作类别 = 30");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 League 域命名空间，必须登记进 LeagueJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(LeagueRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 League");
        }

        foreach (var type in constTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>().Should().BeNull(
                $"{type.Name} 为常量类（不参与 JSON 序列化），不得标 [HttpJsonSerializable]");
        }
    }

    /// <summary>契约守卫 CLG5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用/分建裁决 + 改版矛盾照录）。</summary>
    [Fact]
    public void LeagueDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 获取带货者详情。
        AssertJsonProperty<ChannelsGetPromoterRequest>("talent_appid", "带货者 appid");
        AssertJsonProperty<ChannelsGetPromoterRequest>("talent_search_key", "带货者搜索参数");
        AssertJsonProperty<ChannelsGetPromoterResponse>("talent", "带货者信息");
        AssertJsonProperty<ChannelsLeagueTalent>("base_talent_info", "带货者基础信息");
        AssertJsonProperty<ChannelsLeagueTalent>("data", "带货者数据信息");
        AssertJsonProperty<ChannelsLeagueTalent>("feature_info", "带货者特征信息");
        AssertJsonProperty<ChannelsLeagueTalentData>("total_sale_gmv", "累计销售额");
        AssertJsonProperty<ChannelsLeagueTalentData>("sale_gmv_30d", "近30日销售额");
        AssertJsonProperty<ChannelsLeagueTalentData>("window_product_num", "橱窗商品数");
        AssertJsonProperty<ChannelsLeagueTalentFeatureInfo>("is_blacklisted", "是否被拉黑");

        // 获取带货者列表。
        AssertJsonProperty<ChannelsGetPromoterListRequest>("page_index", "页面下标");
        AssertJsonProperty<ChannelsGetPromoterListRequest>("page_size", "单页带货者数");
        AssertJsonProperty<ChannelsGetPromoterListResponse>("total_num", "带货者总数");
        AssertJsonProperty<ChannelsGetPromoterListResponse>("continue_flag", "翻页标志");
        AssertJsonProperty<ChannelsGetPromoterListResponse>("list", "带货者列表");
        AssertJsonProperty<ChannelsLeaguePromoterListItem>("talent_appid", "带货者 appid");

        // 获取带货者商品数据列表。
        AssertJsonProperty<ChannelsGetTalentProductDataListRequest>("get_data_type", "获取的商品数据类型");
        AssertJsonProperty<ChannelsLeagueTalentProductDataItem>("spu_id", "商品 id");
        AssertJsonProperty<ChannelsLeagueTalentProductDataItem>("product_data", "商品数据");
        AssertJsonProperty<ChannelsLeagueTalentProductDataItem>("talent_commission_info", "带货者佣金信息");
        AssertJsonProperty<ChannelsLeagueTalentCommissionInfo>("commission_type", "佣金类型（1 普通计划 / 2 定向计划 / 3 专属计划）");
        AssertJsonProperty<ChannelsLeagueTalentCommissionInfo>("ratio", "佣金率（1000000 = 100%）");

        // 编辑带货者。
        AssertJsonProperty<ChannelsUpdateTalentRequest>("upd_talent_type", "编辑操作类型（1 拉黑 / 2 取消拉黑）");

        // 批量新增联盟商品（改版矛盾照录：参数表 talent_appids/promotion_plan_id，示例 promoter_ids/info_id）。
        AssertJsonProperty<ChannelsBatchAddLeagueItemRequest>("type", "获取商品推广类别（100/300）");
        AssertJsonProperty<ChannelsBatchAddLeagueItemRequest>("list", "商品列表");
        AssertJsonProperty<ChannelsBatchAddLeagueItemRequest>("is_forerver", "是否永久推广（官方历史拼写照录）");
        AssertJsonProperty<ChannelsBatchAddLeagueItemRequest>("talent_appids", "带货者 appid 列表（参数表字段名，非示例的 promoter_ids）");
        AssertJsonProperty<ChannelsLeagueProductRatio>("product_id", "商品 id");
        AssertJsonProperty<ChannelsLeagueProductRatio>("ratio", "推广佣金 [0,90]%");
        AssertJsonProperty<ChannelsBatchAddLeagueItemResponse>("result_info_list", "逐商品添加结果");
        AssertJsonProperty<ChannelsLeagueItemResultInfo>("promotion_plan_id", "推广计划 id（参数表字段名，非示例的 info_id）");
        AssertJsonProperty<ChannelsLeagueItemResultInfo>("fail_talent_appids", "推广失败带货者列表");
        AssertJsonProperty<ChannelsLeagueItemResultInfo>("errcode", "单商品添加错误码");

        // 获取联盟推广计划详情（改版矛盾照录：exclusive_info/ext_info 按参数表）。
        AssertJsonProperty<ChannelsGetLeagueItemRequest>("promotion_plan_id", "推广计划 id");
        AssertJsonProperty<ChannelsGetLeagueItemResponse>("item", "推广计划信息");
        AssertJsonProperty<ChannelsLeagueItem>("status", "商品推广状态");
        AssertJsonProperty<ChannelsLeagueItem>("exclusive_info", "推广信息");
        AssertJsonProperty<ChannelsLeagueItem>("ext_info", "扩展信息");
        AssertJsonProperty<ChannelsLeagueExclusiveInfo>("promotion_plan_id", "推广计划 id（参数表字段名，非示例的 info_id）");
        AssertJsonProperty<ChannelsLeagueExclusiveInfo>("is_forerver", "是否永久推广（官方历史拼写照录）");
        AssertJsonProperty<ChannelsLeagueExclusiveInfo>("talent_num", "推广带货者数量");
        AssertJsonProperty<ChannelsLeagueItemExtInfo>("is_banned", "是否被官方封禁（参数表字段名，非示例的 is_sale_forbidden）");

        // 批量新增联盟机构推广。
        AssertJsonProperty<ChannelsBatchAddHeadSupplierItemRequest>("headsupplier_appid", "机构开放唯一凭证");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProduct>("service_ratio", "推广服务费 [0,90]%");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProduct>("commission_type", "机构计划类型");
        AssertJsonProperty<ChannelsLeagueHeadSupplierResultInfo>("product_id", "商品 id");

        // 获取联盟商品列表（改版矛盾照录：参数表无 last_buffer）。
        AssertJsonProperty<ChannelsGetLeagueItemListRequest>("page_size", "单页商品数");
        AssertJsonProperty<ChannelsGetLeagueItemListRequest>("need_total_num", "是否需要返回总数");
        AssertJsonProperty<ChannelsGetLeagueItemListResponse>("items", "商品信息");
        AssertJsonProperty<ChannelsGetLeagueItemListResponse>("has_more", "是否还有剩余商品");
        AssertJsonProperty<ChannelsLeagueListProduct>("status", "商品状态");
        typeof(ChannelsGetLeagueItemListRequest).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "last_buffer")
            .Should().BeFalse("官方变更日志 2026-06-17 已删除 last_buffer，参数表无此入参——不得回潮");

        // 更新联盟推广计划信息。
        AssertJsonProperty<ChannelsUpdateLeagueItemRequest>("operate_type", "更新操作类别");
        AssertJsonProperty<ChannelsUpdateLeagueItemRequest>("exclusive_info", "特殊推广信息");
        AssertJsonProperty<ChannelsLeagueUpdExclusiveInfo>("add_talent_appids", "新增推广达人列表（≤20）");
        AssertJsonProperty<ChannelsLeagueUpdExclusiveInfo>("del_talent_appids", "删除推广达人列表（≤20）");
        AssertJsonProperty<ChannelsUpdateLeagueItemResponse>("promotion_plan_id", "推广计划 id");
        AssertJsonProperty<ChannelsUpdateLeagueItemResponse>("fail_talent_appids", "更新失败的达人 appid 列表");

        // 商品推广计划列表 / 推广带货者列表。
        AssertJsonProperty<ChannelsListSpuPromotionPlansRequest>("promotion_plan_type", "推广计划类型（100/200/300）");
        AssertJsonProperty<ChannelsLeaguePromotionPlanItem>("promotion_plan_id", "推广计划 id");
        AssertJsonProperty<ChannelsListSpuPromotionTalentsRequest>("promotion_plan_id", "推广计划 id");
        AssertJsonProperty<ChannelsLeaguePromotionTalentItem>("base_talent_info", "带货者基础信息");
        AssertJsonProperty<ChannelsLeaguePromotionTalentItem>("commission_info", "带货者佣金信息");
        AssertJsonProperty<ChannelsLeaguePromotionTalentItem>("talent_relation_status", "带货者推广状态");
        AssertJsonProperty<ChannelsLeaguePromotionCommissionInfo>("promotion_plan_type", "推广计划类型");
        AssertJsonProperty<ChannelsLeaguePromotionCommissionInfo>("ratio", "推广佣金率");

        // 共用 DTO 裁决 1：promoter/get 与 promotion/talent/list 的 base_talent_info 字段集一致 ⇒ 共用 ChannelsLeagueTalentBaseInfo。
        typeof(ChannelsLeagueTalent).GetProperty(nameof(ChannelsLeagueTalent.BaseTalentInfo))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueTalentBaseInfo));
        typeof(ChannelsLeaguePromotionTalentItem).GetProperty(nameof(ChannelsLeaguePromotionTalentItem.BaseTalentInfo))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueTalentBaseInfo));

        // 分建裁决 2：带货者列表条目（无 head_img）与 base_talent_info 字段集不同 ⇒ 分建。
        typeof(ChannelsLeaguePromoterListItem).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "head_img")
            .Should().BeFalse("promoter/list/get 列表条目官方仅 talent_appid/nickname，与 base_talent_info 分建");

        // 分建裁决 3：product_data（4 字段）与带货者 data（6 字段）字段集不同 ⇒ 分建。
        typeof(ChannelsLeagueProductData).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "window_product_num")
            .Should().BeFalse("talent/getproductdatalist 的 product_data 官方仅 4 字段，与 promoter/get 的 data 分建");

        // 分建裁决 4：commission_info（计划 id/类型/佣金率）与 talent_commission_info（佣金类型/佣金率）字段集不同 ⇒ 分建。
        typeof(ChannelsLeaguePromotionCommissionInfo).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "commission_type")
            .Should().BeFalse("promotion/talent/list 的 commission_info 无 commission_type 入参，与 talent_commission_info 分建");

        // 分建裁决 5：exclusive_info 更新形态（add/del_talent_appids）与响应形态（talent_num）字段集不同 ⇒ 分建。
        typeof(ChannelsLeagueUpdExclusiveInfo).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "talent_num")
            .Should().BeFalse("item/upd 的 exclusive_info 为提交形态（add/del_talent_appids），与响应形态分建");
        typeof(ChannelsLeagueExclusiveInfo).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "add_talent_appids")
            .Should().BeFalse("item/get 的 exclusive_info 为响应形态（talent_num），与提交形态分建");

        // 分域裁决 6：联盟域 product_id 为 number（long），与 Warehouse 域 string 型 product_id 各自照抄官方参数表。
        typeof(ChannelsGetLeagueItemRequest).GetProperty(nameof(ChannelsGetLeagueItemRequest.ProductId))!
            .PropertyType.Should().Be(typeof(long),
                "联盟域参数表 product_id 类型为 number（示例 100000000001），与 Warehouse 域 string 型分域建模");

        // ===== 机构侧（headsupplier）资金族：官方字段名 + 跨域复用/分建裁决 =====
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetBalanceResponse>("available_amount", "可提现余额（分）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetBalanceResponse>("pending_amount", "待结算余额（分）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetFundsFlowListRequest>("funds_type", "机构侧资金类型筛选");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetFundsFlowListRequest>("next_key", "深翻页上下文（page*page_size>=5000 时必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFundsFlow>("order_id", "关联订单号（机构侧专属）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFundsFlow>("withdraw_id", "关联提现单号（机构侧专属）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse>("funds_flow", "流水信息");

        // 复用裁决：机构侧流水列表响应字段集（flow_ids/has_more/next_key）与商家侧逐字段一致 ⇒ 复用 Funds 域类型。
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.GetHeadSupplierFundsFlowListAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsGetFundsFlowListResponse>),
                "机构侧与商家侧 flowlist 响应逐字段一致 ⇒ 复用 Funds 域 ChannelsGetFundsFlowListResponse，不得分建");
        // 复用裁决：流水详情请求单字段 flow_id 同形 ⇒ 复用 Funds 域请求类型。
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.GetHeadSupplierFundsFlowDetailAsync))!
            .GetParameters()[0].ParameterType.Should().Be(typeof(ChannelsGetFundsFlowDetailRequest),
                "机构侧 flowdetail 请求只有 flow_id，与商家侧同形 ⇒ 复用 Funds 域 ChannelsGetFundsFlowDetailRequest");

        // 分建裁决：余额响应（机构侧无 sub_mchid）、流水列表请求（机构侧有 funds_type、无 transaction_id）、
        // funds_flow 元素（机构侧有 order_id/withdraw_id，无 flow_type/related_info_list/funds_type_desc）三处字段集不同 ⇒ 各自分建。
        typeof(ChannelsLeagueHeadSupplierGetBalanceResponse).GetProperty(nameof(ChannelsLeagueHeadSupplierGetBalanceResponse.PendingAmount))!
            .PropertyType.Should().Be(typeof(long?), "机构侧余额为 number（分）");
        typeof(ChannelsLeagueHeadSupplierGetFundsFlowListRequest).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "transaction_id")
            .Should().BeFalse("机构侧 flowlist 参数表无 transaction_id（商家侧才有），与 Funds 域分建");
        typeof(ChannelsLeagueHeadSupplierFundsFlow).GetProperties()
            .Any(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()!.Name == "related_info_list")
            .Should().BeFalse("机构侧 funds_flow 无 related_info_list（商家侧才有），与 ChannelsFundsFlow 分建");
        typeof(ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse).GetProperty(nameof(ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse.FundsFlow))!
            .PropertyType.Should().NotBe(typeof(ChannelsFundsFlow),
                "机构侧 funds_flow 元素字段集与商家侧不同 ⇒ 元素类型分建，不得复用 ChannelsFundsFlow");

        // 机构侧商品获取族（cooperativeitem/add、cooperativeitem/list/get、item/promotiondetail/get）。
        AssertJsonProperty<ChannelsLeagueHeadSupplierAddCooperativeItemRequest>("head_supplier_item_link", "机构推广商品链接（必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierAddCooperativeItemRequest>("hidden_flag", "是否在机构主页隐藏");
        AssertJsonProperty<ChannelsLeagueHeadSupplierAddCooperativeItemResponse>("cooperative_item_id", "机构合作计划 id");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetCooperativeItemListRequest>("commission_type", "佣金分配类型（官方必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetCooperativeItemListRequest>("listing_time_range", "合作商品上架时间范围（嵌套对象）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetCooperativeItemListRequest>("next_key", "游标翻页上下文（官方必填，第一页传空串）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierNumberRange>("min", "区间最小值");
        AssertJsonProperty<ChannelsLeagueHeadSupplierNumberRange>("max", "区间最大值");
        AssertJsonProperty<ChannelsLeagueHeadSupplierCooperativeItem>("product_id", "商品 id");
        AssertJsonProperty<ChannelsLeagueHeadSupplierCooperativeItem>("cooperative_item_id", "机构合作计划 id");
        AssertJsonProperty<ChannelsLeagueHeadSupplierCooperativeItem>("head_supplier_item_link", "机构推广商品链接");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetCooperativeItemListResponse>("list", "合作商品列表");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetItemPromotionDetailRequest>("head_supplier_item_link", "机构推广商品链接（本族唯一入参）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierPromotionDetailItem>("shop_appid", "商品所属店铺 appid");
        AssertJsonProperty<ChannelsLeagueHeadSupplierPromotionDetailItem>("spu_source", "供货来源 1 商家 / 2 机构");
        AssertJsonProperty<ChannelsLeagueHeadSupplierPromotionDetailItem>("head_supplier_info", "供货机构信息");
        AssertJsonProperty<ChannelsLeagueHeadSupplierPromotionDetailItem>("cooperative_info", "商品合作信息");
        AssertJsonProperty<ChannelsLeagueHeadSupplierPromotionCommissionInfo>("plan_type", "计划类型 1 定向 / 2 公开");
        AssertJsonProperty<ChannelsLeagueHeadSupplierPromotionCommissionInfo>("service_ratio", "服务费率");
        AssertJsonProperty<ChannelsLeagueHeadSupplierPromotionCommissionInfo>("normal_commission_info", "普通推广下的佣金信息");
        AssertJsonProperty<ChannelsLeagueHeadSupplierNormalCommissionInfo>("ratio", "佣金费率");
        AssertJsonProperty<ChannelsLeagueHeadSupplierInfo>("head_img", "机构头像（2025-11-03 变更日志新增）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierCooperativeInfo>("cooperative_status", "商品合作状态 1 合作中");
        AssertJsonProperty<ChannelsLeagueHeadSupplierCooperativeInfo>("is_hidden", "是否在商品列表页对外隐藏");
        AssertJsonProperty<ChannelsLeagueHeadSupplierCooperativeInfo>("link", "带货链接（仅商家指定达人佣金且上架中返回）");

        // 官方变更日志 2025-11-03：合作商品列表 product_id 由 string 变更为 number ⇒ 锁定为 long?。
        typeof(ChannelsLeagueHeadSupplierCooperativeItem).GetProperty(nameof(ChannelsLeagueHeadSupplierCooperativeItem.ProductId))!
            .PropertyType.Should().Be(typeof(long?), "官方 2025-11-03 把 list.product_id 类型由 string 改为 number");

        // 分建裁决：机构侧佣金信息与商家侧两套字段集（含 plan_type / commission_type / service_ratio）⇒ 不得复用。
        typeof(ChannelsLeagueHeadSupplierPromotionCommissionInfo).Should().NotBe(typeof(ChannelsLeaguePromotionCommissionInfo),
            "机构侧 commission_info 含 plan_type/commission_type/service_ratio，与商家侧推广佣金对象字段集不同 ⇒ 分建");
        typeof(ChannelsLeagueHeadSupplierPromotionDetailItem).GetProperty(nameof(ChannelsLeagueHeadSupplierPromotionDetailItem.CommissionInfo))!
            .PropertyType.Should().NotBe(typeof(ChannelsLeagueTalentCommissionInfo),
                "机构侧推广详情佣金信息不得复用商家侧带货者佣金对象（字段集不同）");

        // 机构侧选品广场族（selectionproducts/list/get）。
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsRequest>("plan_type", "计划类型（官方必填，1 定向 / 2 公开）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsRequest>("spu_item_condition", "商品查询条件（嵌套对象）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsRequest>("category", "类目查询条件（定向计划不支持）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsRequest>("spu_source", "供货来源（官方默认 1）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsRequest>("commission_type", "佣金类型查询条件（2026-03-09 新增）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsRequest>("page_size", "单页商品数（官方最大 20）");
        // 官方变更日志提及 from_head_supplier_appid，但请求参数表内无该字段 ⇒ 锁定「以参数表为准」。
        typeof(ChannelsLeagueHeadSupplierGetSelectionProductsRequest).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "from_head_supplier_appid")
            .Should().BeFalse("官方 2026-03-09 变更日志提及 from_head_supplier_appid，但同日核验证的参数表无该字段 ⇒ 不得按日志回填");

        // 六个区间对象官方字段集完全同形（min/max）⇒ 全部复用同一区间类型，禁止逐字段 fork。
        var selectionRangeProps = new[]
        {
            nameof(ChannelsLeagueHeadSupplierSelectionProductCondition.SellingPriceRange),
            nameof(ChannelsLeagueHeadSupplierSelectionProductCondition.MonthlySalesRange),
            nameof(ChannelsLeagueHeadSupplierSelectionProductCondition.ServiceFeeRateRange),
            nameof(ChannelsLeagueHeadSupplierSelectionProductCondition.CommissionRateRange),
            nameof(ChannelsLeagueHeadSupplierSelectionProductCondition.PromoteTimeRange),
            nameof(ChannelsLeagueHeadSupplierSelectionProductCondition.CreateTimeRange),
        };
        foreach (var propName in selectionRangeProps)
        {
            typeof(ChannelsLeagueHeadSupplierSelectionProductCondition).GetProperty(propName)!
                .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierNumberRange),
                    $"{propName} 官方字段集与其余区间对象完全同形（min/max）⇒ 必须复用 ChannelsLeagueHeadSupplierNumberRange");
        }
        AssertJsonProperty<ChannelsLeagueHeadSupplierSelectionProductCondition>("flags", "保障标数组（0~7，见常量类）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierSelectionCategoryCondition>("category_id", "商品类目（官方 number）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierSelectionCategoryCondition>("category_ids_1", "一级类目列表（官方 numarray）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierSelectionCategoryCondition>("category_ids_3", "三级类目列表（官方 numarray）");
        typeof(ChannelsLeagueHeadSupplierSelectionCategoryCondition).GetProperty(nameof(ChannelsLeagueHeadSupplierSelectionCategoryCondition.CategoryId))!
            .PropertyType.Should().Be(typeof(long?), "官方 category_id 类型 number（示例为超长值）⇒ 不得建模为 string");
        AssertJsonProperty<ChannelsLeagueHeadSupplierSelectionProduct>("shop_appid", "商品所属店铺 appid");
        AssertJsonProperty<ChannelsLeagueHeadSupplierSelectionProduct>("head_supplier_item_link", "机构商品链接");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsResponse>("product_list", "商品列表");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSelectionProductsResponse>("next_key", "游标翻页上下文");
        // 分建裁决：选品广场条目（含 shop_appid、无 cooperative_item_id）≠ 合作商品条目。
        typeof(ChannelsLeagueHeadSupplierSelectionProduct).GetProperty(nameof(ChannelsLeagueHeadSupplierSelectionProduct.ShopAppid))!
            .Should().NotBeNull("选品广场条目官方含 shop_appid");
        typeof(ChannelsLeagueHeadSupplierCooperativeItem).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "shop_appid")
            .Should().BeFalse("合作商品条目官方参数表无 shop_appid ⇒ 两族条目不得合并");

        // 机构侧活动补贴族（product/getactivity）。
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetProductActivityRequest>("product_id", "商品 ID（必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetProductActivityRequest>("shop_appid", "所属小店 appid（必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetProductActivityResponse>("has_activity", "是否有活动");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetProductActivityResponse>("product_activity_list", "商品活动信息列表");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductActivity>("activity_price", "活动价（单位：分）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductActivity>("platform_subsidy", "平台补贴金额（单位：分）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductActivity>("platform_subsidy_cnt", "平台补贴件数");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductActivity>("activity_sub_type", "活动子类型（官方目前仅返回 3 品牌日 / 4 通用货补）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductActivity>("stock_mode", "库存模式 0 活动库存 / 1 商品自营库存");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductActivity>("stock_number", "库存");

        // 机构侧商品详情族（productdetail/get）。
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetProductDetailRequest>("shop_appid", "机构商品所属小店 appid（必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetProductDetailResponse>("item", "商品详情（官方参数表根字段）");
        // 官方代码示例根字段写 product，与参数表的 item 不一致 ⇒ 以参数表为权威，示例独有字段不得回填。
        typeof(ChannelsLeagueHeadSupplierGetProductDetailResponse).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "product")
            .Should().BeFalse("官方返回参数表根字段为 item，代码示例的 product 属文档自身矛盾 ⇒ 以参数表为准");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductDetailItem>("product_info", "商品信息");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductDetailItem>("shop", "店铺信息");
        AssertJsonProperty<ChannelsLeagueHeadSupplierShopInfo>("score", "店铺评分（官方示例 463）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierShopInfo>("icon", "店铺图标");
        var detailInfoProps = new[]
        {
            "title", "sub_title", "head_imgs", "desc_info", "skus", "cats_v2", "status", "monthly_sales_range",
            "extra_service", "product_spring_festival_delivery_config", "freight_info", "has_qualification",
            "is_hot_brand", "short_title", "head_video_info", "good_evaluation_ratio", "is_brand_good",
        };
        foreach (var jsonName in detailInfoProps)
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierProductDetailInfo>(jsonName, "官方 item.product_info 参数表字段全集逐个在场");
        }
        // 参数表无、仅代码示例出现的两个对象 ⇒ 锁定零声明（express_info / statistical_data）。
        foreach (var notInTable in new[] { "express_info", "statistical_data" })
        {
            typeof(ChannelsLeagueHeadSupplierProductDetailInfo).GetProperties()
                .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == notInTable)
                .Should().BeFalse($"{notInTable} 只出现在官方代码示例、返回参数表内无 ⇒ 官方参数表为权威，不得回填");
        }
        // 复用裁决：desc_info 与 sku_attrs 与商家侧字段集完全同形 ⇒ 直接复用 Product 域类型。
        typeof(ChannelsLeagueHeadSupplierProductDetailInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierProductDetailInfo.DescInfo))!
            .PropertyType.Should().Be(typeof(Mud.Wechat.Channels.DataModels.Product.ChannelsProductDescInfo),
                "官方 desc_info 为 imgs + desc 两字段，与商家侧完全同形 ⇒ 复用，不得 fork");
        typeof(ChannelsLeagueHeadSupplierProductDetailSku).GetProperty(nameof(ChannelsLeagueHeadSupplierProductDetailSku.SkuAttrs))!
            .PropertyType.Should().Be(typeof(List<Mud.Wechat.Channels.DataModels.Product.ChannelsSkuAttr>),
                "官方 sku_attrs 元素为 attr_key + attr_value，与商家侧完全同形 ⇒ 复用，不得 fork");
        // 分建裁决：四个「看着像」的商家侧类型，字段集各不相同 ⇒ 机构侧必须自建。
        typeof(ChannelsLeagueHeadSupplierProductDetailSku).GetProperty(nameof(ChannelsLeagueHeadSupplierProductDetailSku.SkuDeliverInfo))!
            .PropertyType.Should().NotBe(typeof(Mud.Wechat.Channels.DataModels.Product.ChannelsSkuDeliverInfo),
                "商家侧 sku_deliver_info 多 spot_after_presale_end ⇒ 字段集不同，机构侧分建");
        typeof(ChannelsLeagueHeadSupplierProductDetailInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierProductDetailInfo.ExtraService))!
            .PropertyType.Should().NotBe(typeof(Mud.Wechat.Channels.DataModels.Product.ChannelsProductExtraService),
                "商家侧 extra_service 仅 5 个开关、机构侧 8 个 ⇒ 字段集不同，机构侧分建");
        typeof(ChannelsLeagueHeadSupplierProductDetailInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierProductDetailInfo.FreightInfo))!
            .PropertyType.Should().NotBe(typeof(Mud.Wechat.Channels.DataModels.Product.ChannelsProductExpressInfo),
                "机构侧 freight_info 是完整物流模板、商家侧 express_info 是 template_id 三元组 ⇒ 语义与字段集均不同");
        typeof(ChannelsLeagueHeadSupplierProductDetailInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierProductDetailInfo.CatsV2))!
            .PropertyType.Should().NotBe(typeof(List<Mud.Wechat.Channels.DataModels.Product.ChannelsProductCategory>),
                "机构侧 cats_v2 元素含 cat_name、商家侧仅 cat_id ⇒ 字段集不同，机构侧分建");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductDetailSku>("user_inclusive_price", "用户视角普惠价（2026-09-16 新增）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductDetailSku>("thumb_img", "sku 小图（商家侧详情对象无此字段）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductSkuDeliverInfo>("full_payment_presale_delivery_time", "sku 发货时效（仅 stock_type=1 有效）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierProductCategory>("cat_name", "类目名称（机构侧独有）");
        foreach (var jsonName in new[]
        {
            "seven_day_return", "pay_after_use", "freight_insurance", "damage_guarantee",
            "fake_one_pay_three", "exchange_support", "buy_with_confidence", "deliver_after_check",
        })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierProductDetailExtraService>(jsonName, "官方 extra_service 八个开关全集");
        }
        AssertJsonProperty<ChannelsLeagueHeadSupplierSpringFestivalDeliveryConfig>("delivery_time_list", "春节可发货时段（为空则春节不发货）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierSpringFestivalDeliveryConfig>("unaffected_by_newyear", "是否不受春节影响");
        AssertJsonProperty<ChannelsLeagueHeadSupplierTimeRange>("start_time", "时段开始时间戳");
        // 春节时段与佣金单时间区间同形（两个秒级时间戳）⇒ 复用同一类型，不得逐处 fork。
        typeof(ChannelsLeagueHeadSupplierSpringFestivalDeliveryConfig).GetProperty(nameof(ChannelsLeagueHeadSupplierSpringFestivalDeliveryConfig.DeliveryTimeList))!
            .PropertyType.Should().Be(typeof(List<ChannelsLeagueHeadSupplierTimeRange>),
                "delivery_time_list 元素与订单侧 create_time_range 的字段集同形 ⇒ 复用一支类型");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFreightInfo>("send_time", "发货时间期限（官方枚举字面量为字符串）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFreightInfo>("valuation_type", "计费类型 PIECE/WEIGHT");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFreightInfo>("shipping_method", "计费方式 FREE/CONDITION_FREE/NO_FREE");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFreightInfo>("condition_free_detail_list", "条件包邮详情");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFreightInfo>("not_send_area", "不发货区域");
        AssertJsonProperty<ChannelsLeagueHeadSupplierConditionFreeDetail>("min_weight", "最低重量（单位千克）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFreightCalcMethod>("delivery_id", "快递公司");
        AssertJsonProperty<ChannelsLeagueHeadSupplierFreightCalcMethod>("is_default", "是否默认运费");
        AssertJsonProperty<ChannelsLeagueHeadSupplierHeadVideoInfo>("video_url", "视频 url");
        // 国标地址四处同形 ⇒ 同一类型复用（address_info / 两个 address_infos / not_send_area）。
        var freightAddressType = typeof(ChannelsLeagueHeadSupplierFreightAddressInfo);
        typeof(ChannelsLeagueHeadSupplierFreightInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierFreightInfo.AddressInfo))!
            .PropertyType.Should().Be(freightAddressType, "freight_info.address_info 与其余三处同形 ⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierFreightInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierFreightInfo.NotSendArea))!
            .PropertyType.Should().Be(typeof(List<ChannelsLeagueHeadSupplierFreightAddressInfo>), "not_send_area 元素与 address_info 同形 ⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierConditionFreeDetail).GetProperty(nameof(ChannelsLeagueHeadSupplierConditionFreeDetail.AddressInfos))!
            .PropertyType.Should().Be(typeof(List<ChannelsLeagueHeadSupplierFreightAddressInfo>), "条件包邮的 address_infos 同形 ⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierFreightCalcMethod).GetProperty(nameof(ChannelsLeagueHeadSupplierFreightCalcMethod.AddressInfos))!
            .PropertyType.Should().Be(typeof(List<ChannelsLeagueHeadSupplierFreightAddressInfo>), "计费方法的 address_infos 同形 ⇒ 复用");
        // send_time 官方「类型」列写 number、枚举值与示例均为字符串 ⇒ 建模为 string。
        typeof(ChannelsLeagueHeadSupplierFreightInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierFreightInfo.SendTime))!
            .PropertyType.Should().Be(typeof(string),
                "官方 send_time 枚举字面量为字符串（SendTime_TWENTYFOUR_HOUR 等），示例亦为字符串 ⇒ 不得建模为 number");
// —— 机构商家合作（headsupplier/shop/{get,list/get} + headsupplierrelation/update）——
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetShopRequest>("appid", "小店 appid（必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetShopResponse>("shop_detail", "小店详情（2025-11-24 由 shop_list 修正而来）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierShopDetail>("approved_time", "开始合作时间戳");
        foreach (var jsonName in new[]
        {
            "gmv", "product_number", "settle_amount", "unsettle_amount",
            "product_number_today", "product_number_sold_today",
        })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierShopDataInfo>(jsonName, "官方 data_info 六项统计字段全集");
        }
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetShopListRequest>("page_size", "获取小店数量（必填、不超过 30）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetShopListResponse>("shop_list", "小店详情列表");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetShopListResponse>("has_more", "是否还有剩余小店");
        // 复用裁决：详情与列表的 base_info 完全同形 ⇒ 同一类型。
        typeof(ChannelsLeagueHeadSupplierShopDetail).GetProperty(nameof(ChannelsLeagueHeadSupplierShopDetail.BaseInfo))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierShopBaseInfo), "shop_detail.base_info 与列表项 base_info 同形 ⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierShopListItem).GetProperty(nameof(ChannelsLeagueHeadSupplierShopListItem.BaseInfo))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierShopBaseInfo), "shop_list[].base_info 与详情 base_info 同形 ⇒ 复用");
        // 分建裁决：列表项无统计数据与合作时间；base_info（小店三字段）≠ 商品详情的 shop（name/score/icon）。
        typeof(ChannelsLeagueHeadSupplierShopListItem).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "data_info")
            .Should().BeFalse("官方列表项参数表只有 base_info + status ⇒ 不得把详情形态并入列表项");
        typeof(ChannelsLeagueHeadSupplierShopBaseInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierShopBaseInfo.HeadImgUrl))!
            .PropertyType.Should().Be(typeof(string), "官方字段名连写为 headimg_url（无下划线分隔）⇒ 照抄");
        typeof(ChannelsLeagueHeadSupplierShopBaseInfo).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "score")
            .Should().BeFalse("商家合作侧 base_info 无评分字段，商品详情的 shop{name/score/icon} 是另一形态 ⇒ 不得合并");
        foreach (var jsonName in new[] { "object_type", "to_shop_appid", "to_head_supplier_appid", "hs_update_type", "shop_update_type" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierUpdateRelationRequest>(jsonName, "官方关系更新参数全集");
        }
        // 官方返回体仅 errcode/errmsg ⇒ 复用 ChannelsResponse，不得建空壳 DTO。
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.UpdateHeadSupplierRelationAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsResponse>), "headsupplierrelation/update 官方返回体无业务字段 ⇒ 复用中立响应类型");
// —— 机构橱窗与授权（headsupplier/window/* + windowauth/*）——
        foreach (var jsonName in new[] { "finder_id", "product_id", "openfinderid", "is_hide_for_window", "product_link", "opentalentid" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierAddWindowRequest>(jsonName, "官方橱窗添加参数全集");
        }
        foreach (var jsonName in new[] { "finder_id", "offset", "page_size", "need_total_num", "openfinderid", "is_get_all", "opentalentid" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowListRequest>(jsonName, "官方橱窗列表参数全集");
        }
        foreach (var jsonName in new[] { "appid", "finder_id", "product_id", "openfinderid", "product_window_id", "opentalentid" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowProductDetailRequest>(jsonName, "官方橱窗详情参数全集");
        }
        // 分建裁决：移除请求无 appid（详情请求有）⇒ 两支不共用。
        typeof(ChannelsLeagueHeadSupplierRemoveWindowRequest).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "appid")
            .Should().BeFalse("官方 window/remove 参数表无 appid、window/getdetail 有 ⇒ 两请求形态不得合并");
        foreach (var jsonName in new[] { "list", "next_offset", "has_more", "total_num" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowListResponse>(jsonName, "官方橱窗列表返回体全集");
        }
        // 翻页形态差异锁定：本族走 offset/next_offset，机构侧其余列表族走 next_key。
        typeof(ChannelsLeagueHeadSupplierGetWindowListResponse).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "next_key")
            .Should().BeFalse("window/getall 官方用 offset / next_offset 翻页 ⇒ 不得套用 next_key 形态");
        AssertJsonProperty<ChannelsLeagueHeadSupplierWindowProduct>("product_window_id", "橱窗商品 ID（移除 / 查详情的定位键）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierWindowProduct>("is_operation_allowed", "是否有操作权限（官方 2025-02 权限校验矩阵的落地字段）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowProductDetailResponse>("product_detail", "商品所有信息");
        typeof(ChannelsLeagueHeadSupplierWindowProductDetail).GetProperty(nameof(ChannelsLeagueHeadSupplierWindowProductDetail.ProductId))!
            .PropertyType.Should().Be(typeof(string), "官方 window/getdetail 参数表把 product_id 定为 string（其余橱窗族为 number）⇒ 照抄本表");
        foreach (var jsonName in new[] { "title", "sub_title", "head_imgs", "desc_info", "cats", "cats_v2", "product_promotion_link" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierWindowProductInfo>(jsonName, "官方橱窗商品快照七字段全集");
        }
        // 复用裁决：橱窗快照的 desc_info 与 cats / cats_v2 元素同形 ⇒ 复用 Product 域类型。
        typeof(ChannelsLeagueHeadSupplierWindowProductInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierWindowProductInfo.DescInfo))!
            .PropertyType.Should().Be(typeof(Mud.Wechat.Channels.DataModels.Product.ChannelsProductDescInfo),
                "橱窗 product_info.desc_info 与 Product 域 desc_info 同形（imgs + desc）⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierWindowProductInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierWindowProductInfo.Cats))!
            .PropertyType.Should().Be(typeof(List<Mud.Wechat.Channels.DataModels.Product.ChannelsProductCategory>),
                "橱窗 cats 元素只有 cat_id ⇒ 复用 Product 域类目类型（机构商品详情的 cats_v2 含 cat_name，是另一形态）");
        typeof(ChannelsLeagueHeadSupplierWindowProductInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierWindowProductInfo.CatsV2))!
            .PropertyType.Should().Be(typeof(List<Mud.Wechat.Channels.DataModels.Product.ChannelsProductCategory>),
                "橱窗 cats_v2 元素同样只有 cat_id ⇒ 与 cats 共用一支类型");
        // 分建裁决：橱窗快照 ≠ 机构商品详情（后者含 sku / 物流 / 服务保障）。
        typeof(ChannelsLeagueHeadSupplierWindowProductInfo).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "skus")
            .Should().BeFalse("官方 window/getdetail 的 product_info 无 sku ⇒ 不得复用机构商品详情的 product_info");
        foreach (var jsonName in new[] { "finder_id", "openid", "talent_appid" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowAuthRequest>(jsonName, "官方授权链接三选一参数");
        }
        foreach (var jsonName in new[] { "auth_url", "auth_wxa_username", "auth_wxa_appid", "auth_wxa_path" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierWindowAuthInfo>(jsonName, "官方 auth_info 四字段全集");
        }
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowAuthResponse>("auth_info", "授权链接信息");
        // 官方参数表把 openfinderid / opentalentid 列在返回体根（代码示例只出现 opentalentid）⇒ 以参数表为权威。
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowAuthResponse>("openfinderid", "参数表位于返回体根，示例未出现 ⇒ 照参数表建模");
        foreach (var jsonName in new[] { "finder_id", "openfinderid", "talent_appid", "opentalentid" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowAuthStatusRequest>(jsonName, "官方授权状态四选一参数（含废弃的 finder_id）");
        }
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse>("window_auth_status", "授权状态（number 而非 boolean）");
        // 示例未出现但参数表列出：nickname / head_img_url。
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse>("nickname", "达人昵称（参数表有、示例无 ⇒ 以参数表为权威）");
        typeof(ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse).GetProperty(nameof(ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse.WindowAuthStatus))!
            .PropertyType.Should().Be(typeof(int?), "官方 window_auth_status 定为 number（0/1）⇒ 不得建模为 boolean");
        // 两支只回 errcode/errmsg 的写接口 ⇒ 复用中立响应类型，不建空壳 DTO。
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.AddHeadSupplierWindowAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsResponse>), "window/add 官方返回体无业务字段 ⇒ 复用中立响应类型");
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.RemoveHeadSupplierWindowAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsResponse>), "window/remove 官方返回体无业务字段 ⇒ 复用中立响应类型");

        // —— 机构佣金单（headsupplier/order/{list/get,get}）——
        foreach (var jsonName in new[]
        {
            "appid", "finder_id", "page_size", "next_key", "create_time_range", "update_time_range",
            "order_id", "openfinderid", "sharer_appid",
        })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetOrderListRequest>(jsonName, "官方佣金单列表筛选参数全集");
        }
        // 复用裁决：两个时间范围与春节可发货时段同形（秒级 start_time/end_time）⇒ 同一类型。
        typeof(ChannelsLeagueHeadSupplierGetOrderListRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierGetOrderListRequest.CreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierTimeRange), "create_time_range 与 update_time_range、春节可发货时段三者同形 ⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierGetOrderListRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierGetOrderListRequest.UpdateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierTimeRange), "update_time_range 复用同一时间区间类型");
        typeof(ChannelsLeagueHeadSupplierGetOrderListRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierGetOrderListRequest.OrderId))!
            .PropertyType.Should().Be(typeof(string), "官方列表请求把 order_id 定为 string（详情请求为 number）⇒ 两形态不合并");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetOrderListResponse>("list", "佣金单标识列表");
        AssertJsonProperty<ChannelsLeagueHeadSupplierOrderListItem>("order_id", "订单 id（官方参数表为 string）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetOrderRequest>("sku_id", "商品 skuid（必填）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetOrderResponse>("commssion_order", "佣金单结构（官方根字段拼写漏字母 i）");
        // 官方文档拼写差异锁定：不得按「正确拼写」回填 JSON 名。
        typeof(ChannelsLeagueHeadSupplierGetOrderResponse).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "commission_order")
            .Should().BeFalse("官方原文为 commssion_order（漏 i）⇒ 正确拼写名不得出现在 JSON 契约上");
        typeof(ChannelsLeagueHeadSupplierCommissionOrder).GetProperty(nameof(ChannelsLeagueHeadSupplierCommissionOrder.OrderId))!
            .PropertyType.Should().Be(typeof(string), "参数表定 order_id 为 string、示例写数字 ⇒ 以参数表为权威");
        foreach (var jsonName in new[] { "shop_info", "product_info", "order_info", "commission_info", "buyer_info" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierOrderDetail>(jsonName, "官方 order_detail 五个子对象全集");
        }
        AssertJsonProperty<ChannelsLeagueHeadSupplierOrderProductInfo>("product_cnt", "商品数量（2026-03-31 新增）");
        // 官方示例把 order_info 的状态写作 order_status，参数表为 status ⇒ 示例字段名不得回潮。
        typeof(ChannelsLeagueHeadSupplierOrderInfo).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "order_status")
            .Should().BeFalse("参数表为 status、代码示例为 order_status ⇒ 以参数表为权威");
        foreach (var jsonName in new[]
        {
            "finder_info", "service_ratio", "service_amount", "profit_sharding_suc_time", "promotion_channel",
            "sharer_info", "talent_info", "promotion_info", "sub_agency_info", "promoter_agency_info",
            "head_agency_info", "platform_amount", "platform_commission_status", "service_total_amount",
        })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierOrderCommissionInfo>(jsonName, "官方 commission_info 字段全集（含 2026-03-06 / 03-12 / 04-23 新增项）");
        }
        AssertJsonProperty<ChannelsLeagueHeadSupplierOrderPromotionInfo>("custom_token", "机构自定义关联参数（2026-03-12 新增）");
        // 复用裁决：帮卖 / 推客 / 供货三处机构分佣信息完全同形 ⇒ 同一类型。
        var agencyType = typeof(ChannelsLeagueHeadSupplierOrderAgencyInfo);
        typeof(ChannelsLeagueHeadSupplierOrderCommissionInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierOrderCommissionInfo.SubAgencyInfo))!
            .PropertyType.Should().Be(agencyType, "sub_agency_info / promoter_agency_info / head_agency_info 三处字段集同形 ⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierOrderCommissionInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierOrderCommissionInfo.PromoterAgencyInfo))!
            .PropertyType.Should().Be(agencyType, "推客机构分佣与帮卖机构分佣同形 ⇒ 复用");
        typeof(ChannelsLeagueHeadSupplierOrderCommissionInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierOrderCommissionInfo.HeadAgencyInfo))!
            .PropertyType.Should().Be(agencyType, "供货机构分佣与帮卖机构分佣同形 ⇒ 复用");
        // 分建裁决：推广参数内层 finder_info / talent_info 无 ratio/amount，与外层同名字段不同形 ⇒ 各建一支。
        typeof(ChannelsLeagueHeadSupplierOrderPromotionInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierOrderPromotionInfo.FinderInfo))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierOrderPromotionFinderInfo),
                "promotion_info.finder_info 只有 finder_id/nickname/openfinderid ⇒ 不得复用分佣侧 finder_info");
        typeof(ChannelsLeagueHeadSupplierOrderPromotionInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierOrderPromotionInfo.TalentInfo))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierOrderPromotionTalentInfo),
                "promotion_info.talent_info 无 ratio/amount ⇒ 不得复用分佣侧 talent_info");
        typeof(ChannelsLeagueHeadSupplierOrderPromotionInfo).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "sharer_info")
            .Should().BeFalse("官方 promotion_info 参数表无 sharer_info ⇒ 不得按外层形态补齐");

        // —————— 机构侧商品订阅（subscription/{getsubscribe,subscribe,unsubscribe}）——————
        foreach (var jsonName in new[] { "filter", "page_size", "next_key", "need_total_num" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetSubscribeProductsRequest>(jsonName, "官方订阅列表请求体字段全集");
        }
        foreach (var jsonName in new[] { "product_id", "product_name", "appid" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierSubscribeProductFilter>(jsonName, "官方 filter 子表三字段（原文均标必填，实为筛选条件 ⇒ 全部可空）");
        }
        foreach (var jsonName in new[] { "subscribe_info_list", "total_num", "next_key", "has_more" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetSubscribeProductsResponse>(jsonName, "官方订阅列表返回体字段全集");
        }
        typeof(ChannelsLeagueHeadSupplierGetSubscribeProductsResponse).GetProperty(nameof(ChannelsLeagueHeadSupplierGetSubscribeProductsResponse.SubscribeInfoList))!
            .PropertyType.Should().Be(typeof(List<ChannelsLeagueHeadSupplierSubscribeProductInfo>),
                "官方参数表把 subscribe_info_list 定为 objarray（代码示例写成单对象 ⇒ 以参数表为权威）");
        typeof(ChannelsLeagueHeadSupplierGetSubscribeProductsResponse).GetProperty(nameof(ChannelsLeagueHeadSupplierGetSubscribeProductsResponse.HasMore))!
            .PropertyType.Should().Be(typeof(bool?), "官方参数表 has_more 为 boolean（示例写字符串 \"false\" ⇒ 以参数表为权威）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierSubscribeProductInfo>("shop_appid", "店铺 AppID（官方线索/订阅族一律 shop_appid，非 store_appid）");
        // 共用裁决：订阅与取消订阅的请求体完全同形（单 product_id）⇒ 一支类型，且两端点响应仅 errcode/errmsg。
        var subscribeRequestType = typeof(ChannelsLeagueHeadSupplierSubscribeProductRequest);
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.SubscribeHeadSupplierProductAsync))!
            .GetParameters()[0].ParameterType.Should().Be(subscribeRequestType, "subscribe 请求体 ⇒ product_id");
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.UnsubscribeHeadSupplierProductAsync))!
            .GetParameters()[0].ParameterType.Should().Be(subscribeRequestType, "unsubscribe 与 subscribe 字段集同形 ⇒ 复用同一请求类型，不得 fork");
        foreach (var m in new[] { nameof(IChannelsLeagueService.SubscribeHeadSupplierProductAsync), nameof(IChannelsLeagueService.UnsubscribeHeadSupplierProductAsync) })
        {
            typeof(IChannelsLeagueService).GetMethod(m)!.ReturnType
                .Should().Be(typeof(Task<ChannelsResponse>),
                    "官方订阅/取消订阅返回参数表只有 errcode/errmsg ⇒ 复用 ChannelsResponse，不建专属响应 DTO");
        }

        // —————— 机构侧达人佣金计划（subitem/{add,list/get}）——————
        foreach (var jsonName in new[] { "cooperative_item_id", "ratio_ext", "ratio" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierAddSubItemRequest>(jsonName, "官方配置达人佣金率请求体字段全集");
        }
        // 二选一裁决：ratio（整数百分比）与 ratio_ext（万分比精确值）必须并存且同为可空 ⇒ 防「必填=是」被做成双非空。
        typeof(ChannelsLeagueHeadSupplierAddSubItemRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierAddSubItemRequest.Ratio))!
            .PropertyType.Should().Be(typeof(int?), "ratio 官方标必填但与 ratio_ext 二选一 ⇒ 必须可空");
        typeof(ChannelsLeagueHeadSupplierAddSubItemRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierAddSubItemRequest.RatioExt))!
            .PropertyType.Should().Be(typeof(long?), "ratio_ext 官方标必填但与 ratio 二选一 ⇒ 必须可空（值域 [0,900000]，10000 = 1%）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierAddSubItemResponse>("sub_item_id", "机构配置达人佣金计划 id");
        foreach (var jsonName in new[] { "cooperative_item_id", "page_size", "next_key", "need_qr_code" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetSubItemListRequest>(jsonName, "官方计划列表请求体字段全集（need_qr_code 为 2026-03-09 新增）");
        }
        foreach (var jsonName in new[] { "sub_item_id", "head_supplier_item_link", "status", "ratio_to_service_rate", "ratio", "ratio_ext", "qr_code_url" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierSubItem>(jsonName, "官方 list 条目字段全集（三档佣金率并存）");
        }
        // 官方计划列表返回体「无 has_more」（本域多数列表接口有）⇒ 负面对锁定，防按其它族补齐。
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetSubItemListResponse>("next_key", "唯一翻页游标");
        typeof(ChannelsLeagueHeadSupplierGetSubItemListResponse).GetProperties()
            .Any(q => q.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "has_more")
            .Should().BeFalse("官方 subitem/list/get 返回参数表无 has_more ⇒ 不得按本域其它列表族补齐");
        typeof(ChannelsLeagueHeadSupplierSubItem).GetProperty(nameof(ChannelsLeagueHeadSupplierSubItem.Status))!
            .PropertyType.Should().Be(typeof(int?), "status 为 number（1 上架中 / 2 已下架）而非 boolean ⇒ 建常量类");

        // —————— 机构侧帮卖供货（secondheadsupplieritem/{add,update}）——————
        foreach (var jsonName in new[] { "cooperative_item_id", "head_supplier_item_link", "plan_type", "service_ratio", "to_head_supplier_appid", "start_time", "end_time" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierAddHelpSellItemRequest>(jsonName, "官方机构给帮卖机构供货请求体字段全集");
        }
        foreach (var jsonName in new[] { "plan_type", "update_type", "cooperative_item_id", "head_supplier_item_link", "service_ratio", "to_head_supplier_appid" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest>(jsonName, "官方更新/删除推广计划请求体字段全集");
        }
        // 分建裁决：update 面官方无起止时间 ⇒ 与 add 面不同形，不得合并为一支 DTO。
        foreach (var jsonName in new[] { "start_time", "end_time" })
        {
            typeof(ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest).GetProperties()
                .Any(q => q.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
                .Should().BeFalse($"官方 secondheadsupplieritem/update 参数表无 {jsonName} ⇒ 不得按 add 面补齐");
        }
        typeof(ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest.ServiceRatio))!
            .PropertyType.Should().Be(typeof(long?), "service_ratio 官方仅在「更新佣金率」情况下必填 ⇒ 可空（add 面为必填 ⇒ 非空）");
        typeof(ChannelsLeagueHeadSupplierAddHelpSellItemRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierAddHelpSellItemRequest.ServiceRatio))!
            .PropertyType.Should().Be(typeof(long), "add 面 service_ratio 必填（百万分之 N，10000 = 1%，须能被 1000 整除）");
        foreach (var m in new[] { nameof(IChannelsLeagueService.AddHeadSupplierHelpSellItemAsync), nameof(IChannelsLeagueService.UpdateHeadSupplierHelpSellItemAsync) })
        {
            typeof(IChannelsLeagueService).GetMethod(m)!.ReturnType
                .Should().Be(typeof(Task<ChannelsResponse>),
                    "官方帮卖供货添加/更新返回参数表只有 errcode/errmsg ⇒ 复用 ChannelsResponse");
        }

        // —————— 机构侧线索订单（clue/{get,list/get}）——————
        foreach (var jsonName in new[] { "order_id", "sku_id" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetClueRequest>(jsonName, "官方线索订单详情请求体字段全集");
        }
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.GetHeadSupplierClueAsync))!
            .GetParameters()[0].ParameterType.Should().Be(typeof(ChannelsLeagueHeadSupplierGetClueRequest),
                "线索订单详情的 order_id / sku_id 官方为 string（与佣金单详情的 number 不同形 ⇒ 请求 DTO 不合并）");
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetClueResponse>("commssion_clue", "线索订单结构（官方根字段拼写漏字母 i，同佣金单族）");
        typeof(ChannelsLeagueHeadSupplierGetClueResponse).GetProperties()
            .Any(q => q.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "commission_clue")
            .Should().BeFalse("官方原文为 commssion_clue（漏 i）⇒ 正确拼写名不得出现在 JSON 契约上");
        foreach (var jsonName in new[] { "order_id", "sku_id", "create_time", "update_time", "clue_detail" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierCommissionClue>(jsonName, "官方 commssion_clue 字段全集");
        }
        // 官方参数表根层无 status（代码示例却有 "status": 20）⇒ 负面对锁定，状态只落 clue_detail.order_info。
        typeof(ChannelsLeagueHeadSupplierCommissionClue).GetProperties()
            .Any(q => q.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "status")
            .Should().BeFalse("参数表根层无 status（仅代码示例出现）⇒ 以参数表为权威，订单状态只在 order_info.order_status");
        foreach (var jsonName in new[] { "shop_info", "product_info", "order_info", "sharer_info", "buyer_info" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierClueDetail>(jsonName, "官方 clue_detail 五个子对象全集（参数表键为 buyer_info，示例写 buyer ⇒ 以参数表为准）");
        }
        // 复用裁决：线索订单买家信息与佣金单买家信息字段集完全同形（open_id / union_id）⇒ 同一类型。
        typeof(ChannelsLeagueHeadSupplierClueDetail).GetProperty(nameof(ChannelsLeagueHeadSupplierClueDetail.BuyerInfo))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierOrderBuyerInfo),
                "clue_detail.buyer_info 与佣金单 buyer_info 同形 ⇒ 复用，不 fork 第二支");
        // 正对照：线索订单的 order_info 官方参数表即为 order_status（佣金单侧却是 status）⇒ 两族字段名分表。
        AssertJsonProperty<ChannelsLeagueHeadSupplierClueOrderInfo>("order_status", "线索订单参数表把订单状态定为 order_status");
        typeof(ChannelsLeagueHeadSupplierClueOrderInfo).GetProperties()
            .Any(q => q.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == "status")
            .Should().BeFalse("线索订单侧官方只用 order_status（佣金单侧只用 status）⇒ 两族不得互相套用");
        typeof(ChannelsLeagueHeadSupplierClueProductInfo).GetProperty(nameof(ChannelsLeagueHeadSupplierClueProductInfo.ProductId))!
            .PropertyType.Should().Be(typeof(string), "线索订单 product_id 官方为 string（佣金单侧为 number）⇒ 形态分表");
        AssertJsonProperty<ChannelsLeagueHeadSupplierClueSharerInfo>("opensharerid", "推客与机构关系标识（官方无下划线，照抄）");
        foreach (var jsonName in new[] { "appid", "sharer_appid", "create_time_range", "page_size", "next_key" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetClueListRequest>(jsonName, "官方线索订单列表请求体字段全集（时间区间 2026-05-22 起必填）");
        }
        typeof(ChannelsLeagueHeadSupplierGetClueListRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierGetClueListRequest.CreateTimeRange))!
            .PropertyType.Should().Be(typeof(ChannelsLeagueHeadSupplierTimeRange),
                "create_time_range 与佣金单列表 / 春节可发货时段同形 ⇒ 复用同一时间区间类型");
        foreach (var jsonName in new[] { "list", "next_key", "has_more" })
        {
            AssertJsonProperty<ChannelsLeagueHeadSupplierGetClueListResponse>(jsonName, "官方线索订单列表返回体字段全集");
        }
        // 分表裁决：线索列表条目 order_id 官方参数表为 number，佣金单列表条目为 string ⇒ 两支条目不合并。
        typeof(ChannelsLeagueHeadSupplierClueListItem).GetProperty(nameof(ChannelsLeagueHeadSupplierClueListItem.OrderId))!
            .PropertyType.Should().Be(typeof(long?), "线索列表条目 order_id 参数表为 number（佣金单列表条目为 string）⇒ 两族条目不同形");
        typeof(ChannelsLeagueHeadSupplierGetClueListRequest).GetProperty(nameof(ChannelsLeagueHeadSupplierGetClueListRequest.PageSize))!
            .PropertyType.Should().Be(typeof(int), "page_size 必填（官方上限 100，订阅列表为 1000、达人佣金计划为 20 ⇒ 各族上限不同）");

        // —————— 机构侧直播大场保护（liveprotection/getqrcode）——————
        AssertJsonProperty<ChannelsLeagueHeadSupplierGetLiveProtectionQrCodeResponse>("qrcode_url", "提报二维码 URL");
        typeof(IChannelsLeagueService).GetMethod(nameof(IChannelsLeagueService.GetHeadSupplierLiveProtectionQrCodeAsync))!
            .GetParameters().Should().HaveCount(1,
                "官方请求体参数表为「无」⇒ 仅 CancellationToken，不得造请求体 DTO（CLG2 同步锁 [Body]）");
    }

    /// <summary>契约守卫 CLG6：枚举常量字面量锁定（含「type」一字两表辨析）。</summary>
    [Fact]
    public void LeagueEnumConstants_ShouldLockOfficialLiteralValues()
    {
        // 请求侧「获取商品推广类别」。
        ChannelsLeaguePlanQueryTypes.All.Should().Be(100, "官方原文「100=全部带货者计划」");
        ChannelsLeaguePlanQueryTypes.Designated.Should().Be(300, "官方原文「300=指定带货者计划」");

        // 计划/佣金侧「普通/定向/专属」三分类（item.type 与 commission_type 同值域）。
        ChannelsLeaguePromotionTypes.Normal.Should().Be(1, "官方原文「1=普通推广商品/普通计划」");
        ChannelsLeaguePromotionTypes.Targeted.Should().Be(2, "官方原文「2=定向推广商品/定向计划」");
        ChannelsLeaguePromotionTypes.Exclusive.Should().Be(3, "官方原文「3=专属推广商品/专属计划」");

        // 商品推广状态。
        ChannelsLeagueItemStatuses.Listed.Should().Be(1, "官方原文「1 已上架推广」");
        ChannelsLeagueItemStatuses.Delisted.Should().Be(2, "官方原文「2 已下架推广」");
        ChannelsLeagueItemStatuses.Deleted.Should().Be(4, "官方原文「4 已删除」");
        ChannelsLeagueItemStatuses.BelowThreshold.Should().Be(5, "官方原文「5 未达到准入标准」");
        ChannelsLeagueItemStatuses.TerminatedByProductDelisted.Should().Be(7, "官方原文「7 商品下架终止推广」（2026-07-15 新增）");
        ChannelsLeagueItemStatuses.TerminatedByPlanExpired.Should().Be(8, "官方原文「8 计划过期终止推广」（2026-07-15 新增）");
        ChannelsLeagueItemStatuses.TerminatedByProductBanned.Should().Be(9, "官方原文「9 商品被封禁下架终止推广」（2026-07-15 新增）");
        ChannelsLeagueItemStatuses.PendingEffective.Should().Be(10, "官方原文「10 待生效」");

        // 编辑带货者。
        ChannelsLeagueUpdTalentTypes.Blacklist.Should().Be(1, "官方原文「1 拉黑带货者」");
        ChannelsLeagueUpdTalentTypes.Unblacklist.Should().Be(2, "官方原文「2 取消拉黑带货者」");

        // 更新操作类别。
        ChannelsLeagueOperateTypes.EditAndList.Should().Be(1, "官方原文「1 编辑并上架」");
        ChannelsLeagueOperateTypes.Delist.Should().Be(2, "官方原文「2 下架」");
        ChannelsLeagueOperateTypes.List.Should().Be(4, "官方原文「4 上架」");

        // 推广计划类型（promotion/plan/list）。
        ChannelsLeaguePromotionPlanTypes.All.Should().Be(100, "官方原文「100 全部带货者」");
        ChannelsLeaguePromotionPlanTypes.Selected.Should().Be(200, "官方原文「200 圈选带货者」");
        ChannelsLeaguePromotionPlanTypes.Designated.Should().Be(300, "官方原文「300 指定带货者」");

        // 机构计划类型。
        ChannelsLeagueHeadSupplierPlanTypes.MerchantDesignated.Should().Be(0, "官方原文「0 商家指定达人佣金」");
        ChannelsLeagueHeadSupplierPlanTypes.OrgDesignated.Should().Be(1, "官方原文「1 机构指定达人佣金【总包计划类型】」");

        // 带货者等级。
        ChannelsLeagueTalentLevels.Rookie.Should().Be(0, "官方原文「0带货新人」");
        ChannelsLeagueTalentLevels.OneStar.Should().Be(1, "官方原文「1一星带货者」");
        ChannelsLeagueTalentLevels.FiveStar.Should().Be(5, "官方原文「5五星带货者」");

        // 带货者推广关系状态。
        ChannelsLeagueTalentRelationStatuses.Promoting.Should().Be(1, "官方原文「1 推广中」");
        ChannelsLeagueTalentRelationStatuses.Removed.Should().Be(4, "官方原文「4 已移除」");
        ChannelsLeagueTalentRelationStatuses.Blacklisted.Should().Be(10, "官方原文「10 已被拉黑」");

        // 商品数据类型。
        ChannelsLeagueGetDataTypes.Last30DaysDaily.Should().Be(1, "官方原文「1=近30天日成交商品数据」");
        ChannelsLeagueGetDataTypes.CurrentWindow.Should().Be(2, "官方原文「2=当前橱窗商品数据」");

        // 机构侧资金类型（headsupplier/funds/flow*，2026-03-04 与 2026-06-01 变更日志各增一值）。
        ChannelsLeagueHeadSupplierFundsTypes.Withdraw.Should().Be(1, "官方原文「1：提现」");
        ChannelsLeagueHeadSupplierFundsTypes.Allocate.Should().Be(2, "官方原文「2：分账」");
        ChannelsLeagueHeadSupplierFundsTypes.LeagueTechnologyServiceFee.Should().Be(3, "官方原文「3：联盟技术服务费」（2026-03-04 变更日志新增）");
        ChannelsLeagueHeadSupplierFundsTypes.AllocateRollback.Should().Be(4, "官方原文「4：分账回退」（2026-06-01 变更日志新增）");
// 商家合作状态与关系操作（shop/{get,list/get} 与 headsupplierrelation/update 共用同一状态值域）。
        ChannelsLeagueHeadSupplierShopStatuses.Inviting.Should().Be(1, "官方原文「1 邀请中」");
        ChannelsLeagueHeadSupplierShopStatuses.Accepted.Should().Be(2, "官方原文「2 已接受邀请」");
        ChannelsLeagueHeadSupplierShopStatuses.Rejected.Should().Be(3, "官方原文「3 已接拒绝邀请」（列表页写「已拒绝邀请」，同一枚举值）");
        ChannelsLeagueHeadSupplierShopStatuses.InviteCancelled.Should().Be(4, "官方原文「4 取消邀请」");
        ChannelsLeagueHeadSupplierShopStatuses.CooperationCancelled.Should().Be(5, "官方原文「5 取消合作」");
        ChannelsLeagueHeadSupplierRelationObjectTypes.HeadSupplier.Should().Be(1, "官方原文「1: 机构」");
        ChannelsLeagueHeadSupplierRelationObjectTypes.Shop.Should().Be(2, "官方原文「2: 店铺」");
        ChannelsLeagueHeadSupplierHsUpdateTypes.Invite.Should().Be(1, "官方原文「1: 邀请」");
        ChannelsLeagueHeadSupplierHsUpdateTypes.Pass.Should().Be(2, "官方原文「2: 通过」");
        ChannelsLeagueHeadSupplierHsUpdateTypes.Reject.Should().Be(3, "官方原文「3: 不通过」");
        ChannelsLeagueHeadSupplierHsUpdateTypes.CancelInvite.Should().Be(4, "官方原文「4: 取消邀请」");
        ChannelsLeagueHeadSupplierHsUpdateTypes.CancelCooperation.Should().Be(5, "官方原文「5: 取消合作」");
        ChannelsLeagueHeadSupplierShopUpdateTypes.EndCooperation.Should().Be(1, "官方原文「1: 结束合作」");
        ChannelsLeagueHeadSupplierShopUpdateTypes.Reject.Should().Be(2, "官方原文「2: 拒绝」");
        ChannelsLeagueHeadSupplierShopUpdateTypes.Pass.Should().Be(3, "官方原文「3: 通过」");

        // 佣金单侧「一字两表」：佣金单状态与订单状态各有独立枚举表，取值互不混用。
        ChannelsLeagueHeadSupplierCommissionOrderStatuses.Unsettled.Should().Be(20, "官方原文「20 未结算」");
        ChannelsLeagueHeadSupplierCommissionOrderStatuses.Settled.Should().Be(100, "官方原文「100 已结算」");
        ChannelsLeagueHeadSupplierCommissionOrderStatuses.SettlementCancelled.Should().Be(200, "官方原文「200 取消结算」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.WaitPayment.Should().Be(10, "官方原文「10 待付款」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.WaitReceiveGift.Should().Be(12, "官方原文「12 待收礼」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.WaitUse.Should().Be(17, "官方原文「17 待使用【本地生活券订单】」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.WaitShipping.Should().Be(20, "官方原文「20 待发货」（与佣金单状态的 20 未结算同值不同表）");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.PartialShipping.Should().Be(21, "官方原文「21 部分发货」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.WaitReceipt.Should().Be(30, "官方原文「30 待收货」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.Completed.Should().Be(100, "官方原文「100 完成」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.Cancelled.Should().Be(200, "官方原文「200 全部商品售后之后，订单取消」");
        ChannelsLeagueHeadSupplierTradeOrderStatuses.Closed.Should().Be(250, "官方原文「250 未付款用户主动取消或超时未付款订单自动取消」");
        new[] { ChannelsLeagueHeadSupplierCommissionOrderStatuses.Unsettled, ChannelsLeagueHeadSupplierCommissionOrderStatuses.Settled, ChannelsLeagueHeadSupplierCommissionOrderStatuses.SettlementCancelled }
            .Should().Equal(new[] { 20, 100, 200 }, "佣金单状态表与订单状态表是两张表：本表仅三值，不得并入订单侧九值");
        ChannelsLeagueHeadSupplierPlatformCommissionStatuses.Calculated.Should().Be(0, "官方原文「0 已计算」");
        ChannelsLeagueHeadSupplierPlatformCommissionStatuses.Calculating.Should().Be(1, "官方原文「1 计算中」");
        ChannelsLeagueHeadSupplierPromotionChannels.Showcase.Should().Be(0, "官方原文「0-橱窗带货」");
        ChannelsLeagueHeadSupplierPromotionChannels.Sharer.Should().Be(1, "官方原文「1-推客带货」");
        ChannelsLeagueHeadSupplierPromotionChannelTypes.None.Should().Be(0, "官方原文「0：无」");
        ChannelsLeagueHeadSupplierPromotionChannelTypes.PrivateProduct.Should().Be(3, "官方原文「3:私域商品推广」");
        ChannelsLeagueHeadSupplierPromotionChannelTypes.LeagueContent.Should().Be(4, "官方原文「4:带货内容推广」");
        ChannelsLeagueHeadSupplierPromotionChannelTypes.SelfOperatedContent.Should().Be(6, "官方原文「6:自营内容推广」");
// 橱窗授权状态（number 值域，非 boolean）。
        ChannelsLeagueHeadSupplierWindowAuthStatuses.Unauthorised.Should().Be(0, "官方原文「0: 未授权」");
        ChannelsLeagueHeadSupplierWindowAuthStatuses.Authorised.Should().Be(1, "官方原文「1: 已授权」");


        // 选品广场计划类型（plan_type）。
        ChannelsLeagueHeadSupplierSelectionPlanTypes.DirectedPlan.Should().Be(1, "官方原文「1：定向计划」");
        ChannelsLeagueHeadSupplierSelectionPlanTypes.PublicPlan.Should().Be(2, "官方原文「2：公开计划」");

        // 选品广场佣金类型（commission_type，2026-03-09 新增）与「合作商品列表」的 commission_type（0/1）
        // 是两套值域 ⇒ 常量类分建，值集不得合并（防「顺手对齐」）。
        ChannelsLeagueHeadSupplierSelectionCommissionTypes.MerchantDesignated.Should().Be(1, "官方原文「1-商家指定达人佣金」（选品广场侧）");
        ChannelsLeagueHeadSupplierSelectionCommissionTypes.HeadSupplierDesignated.Should().Be(2, "官方原文「2-机构指定达人佣金」（选品广场侧）");
        new[] { ChannelsLeagueHeadSupplierPlanTypes.MerchantDesignated, ChannelsLeagueHeadSupplierPlanTypes.OrgDesignated }
            .Should().Equal(new[] { 0, 1 }, "机构添加合作商品的 commission_type 值域为 0/1，与选品广场的 1/2 不同 ⇒ 两套枚举分建");

        // 供货来源（spu_source，选品广场与推广参数详情共用同一值域）。
        ChannelsLeagueHeadSupplierSpuSources.MerchantSupply.Should().Be(1, "官方原文「1-商家供货；默认值1」");
        ChannelsLeagueHeadSupplierSpuSources.HeadSupplierSupply.Should().Be(2, "官方原文「2-机构供货」");

        // 保障标（spu_item_condition.flags，官方原文顺序）。
        ChannelsLeagueHeadSupplierGuaranteeFlags.SevenDayReturn.Should().Be(0, "官方原文「0-7天无理由」");
        ChannelsLeagueHeadSupplierGuaranteeFlags.FreightInsurance.Should().Be(1, "官方原文「1-运费险」");
        ChannelsLeagueHeadSupplierGuaranteeFlags.Brand.Should().Be(2, "官方原文「2-品牌（已废弃）」，值保留以兼容回包");
        ChannelsLeagueHeadSupplierGuaranteeFlags.BuyWithConfidence.Should().Be(3, "官方原文「3-放心买」");
        ChannelsLeagueHeadSupplierGuaranteeFlags.DamageGuarantee.Should().Be(4, "官方原文「4-损坏包退」");
        ChannelsLeagueHeadSupplierGuaranteeFlags.FakeOnePayThree.Should().Be(5, "官方原文「5-假一赔三」");
        ChannelsLeagueHeadSupplierGuaranteeFlags.PayAfterUse.Should().Be(6, "官方原文「6-先用后付」");
        ChannelsLeagueHeadSupplierGuaranteeFlags.FreeShipping.Should().Be(7, "官方原文「7-包邮」");

        // 近 30 天销量档位（值 = 区间下界的不连续集合，非销量本身）。
        ChannelsLeagueHeadSupplierMonthlySalesRanges.Level0.Should().Be(0, "官方原文「0-销量在0~10」");
        ChannelsLeagueHeadSupplierMonthlySalesRanges.Level10000.Should().Be(10000, "官方原文「10000-销量在10000+」");

        // 发货时间期限（官方枚举字面量，含 FOUTYEIGHT 原文拼写）。
        ChannelsLeagueHeadSupplierSendTimes.TwentyFourHour.Should().Be("SendTime_TWENTYFOUR_HOUR", "官方枚举原文「24小时内发货」");
        ChannelsLeagueHeadSupplierSendTimes.FortyEightHour.Should().Be("SendTime_FOUTYEIGHT_HOUR", "官方枚举原文拼写为 FOUTYEIGHT（非 FORTY）⇒ 照抄不改");
        ChannelsLeagueHeadSupplierSendTimes.ShipToday.Should().Be("SendTime_SHIP_TODAY", "官方枚举原文「今日发」");

        // 达人佣金计划状态（subitem.status，官方原文「1：上架中，2：已下架」）。
        ChannelsLeagueHeadSupplierSubItemStatuses.Listing.Should().Be(1, "官方原文「1：上架中」");
        ChannelsLeagueHeadSupplierSubItemStatuses.Delisted.Should().Be(2, "官方原文「2：已下架」");

        // 帮卖计划类型与「选品广场计划类型」值域相同（1 定向 / 2 公开）但语义族不同 ⇒ 两套常量分建，
        // 防「顺手合并」后一处官方值调整误伤另一处契约。
        ChannelsLeagueHeadSupplierHelpSellPlanTypes.DirectedPlan.Should().Be(1, "官方原文「1：定向计划」（帮卖供货侧，且要求先设公开计划）");
        ChannelsLeagueHeadSupplierHelpSellPlanTypes.PublicPlan.Should().Be(2, "官方原文「2：公开计划」（帮卖供货侧）");
        new[] { ChannelsLeagueHeadSupplierHelpSellPlanTypes.DirectedPlan, ChannelsLeagueHeadSupplierHelpSellPlanTypes.PublicPlan }
            .Should().Equal(new[] { ChannelsLeagueHeadSupplierSelectionPlanTypes.DirectedPlan, ChannelsLeagueHeadSupplierSelectionPlanTypes.PublicPlan },
                "值域相同但分属帮卖供货与选品广场两套官方枚举 ⇒ 类不合并，值集对齐仅为取证");

        // 帮卖推广计划操作类别（update_type，官方原文「1：更新佣金率 2：结束推广」）。
        ChannelsLeagueHeadSupplierHelpSellUpdateTypes.UpdateRatio.Should().Be(1, "官方原文「1：更新佣金率」");
        ChannelsLeagueHeadSupplierHelpSellUpdateTypes.EndPromotion.Should().Be(2, "官方原文「2：结束推广」");
    }

    /// <summary>
    /// 契约守卫 CLG7：官方目录已下架 / 脱树的联盟族 9 支路由在全线零声明。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 2026-10-17 逐页核验（缺失端点清单为 DotNetCore.SKIT.FlurlHttpClient.Wechat 转述，须证官方页仍在）：
    /// ①<c>headsupplier/item/upd</c> 与 <c>headsupplier/item/list/get</c> 的机构文档页
    /// （<c>API/updateitem.html</c> / <c>API/getitemlist.html</c>）返回 <b>HTTP 404</b>；后者另有
    /// 「获取合作商品列表-<b>旧</b>」页面（<c>api/opentalent/getshop/api_getitemlist.html</c>）正文可渲染，
    /// 但<b>无上下篇链接、且不在联盟带货机构 API 目录树内</b> ⇒ 同判已下架，现行面是
    /// <c>headsupplier/cooperativeitem/list/get</c>（本域已声明）；
    /// ②<c>league/item/delete</c> 与 <c>league/promoter/{add,delete,upd}</c> 四支转述自旧
    /// <c>doc/channels/API/league/*</c> 树，该树全部页面现降级为「优选联盟 接口列表」目录页（正文零端点表）；
    /// 小店「优选联盟」现行目录只有商品操作 7 + 带货者操作 4 = 11 支，本域商家侧恰此 11 支。
    /// ④<c>league/funds/getfundsflowlist</c> 与 <c>league/funds/getfundsflowdetail</c>（旧 <c>doc/channels/API/league/</c>
    /// ecleague_getfundsflow*.html 转述）现均 302 落到同一「优选联盟 接口列表」目录页，该页接口表只有
    /// 带货者 4 + 商品操作 7 = 11 支、零资金族小节 ⇒ 同判脱树；机构侧资金面由 <c>headsupplier/funds/*</c> 三支承载（本域已声明）。
    /// </para>
    /// <para>
    /// ③<c>headsupplier/item/get</c>（获取合作商品详情）：<c>API/getitem.html</c> 与 <c>api/getitem.html</c>
    /// 正文均可渲染（200，参数表完整），但官方现行「达人带货-商品获取」目录页
    /// （<c>api/opentalent/getshop/</c>）的链接全集只有 9 支，<b>不含本接口</b>；<c>api/</c> 根目录页的
    /// 同一小节亦无该项 ⇒ 按「页面在正文可渲染但不在目录树内」同判脱树、非契约，
    /// 商品面现行能力由 <c>productdetail/get</c>（基础详情）+ <c>item/promotiondetail/get</c>（推广参数）承载。
    /// </para>
    /// </remarks>
    [Fact]
    public void RemovedLeagueRoutes_ShouldHaveZeroDeclaration()
    {
        var allRoutes = typeof(IChannelsLeagueService).Assembly.GetTypes()
            .Where(static t => t.IsInterface && t.Namespace != null && t.Namespace.StartsWith("Mud.Wechat.Channels", StringComparison.Ordinal))
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .SelectMany(static m => m.GetCustomAttributes().OfType<HttpMethodAttribute>())
            .Select(static a => a.RequestUri ?? string.Empty)
            .ToList();

        allRoutes.Should().NotBeEmpty("反射枚举不得空跑（正对照）");

        foreach (var removed in new[]
        {
            "/channels/ec/league/headsupplier/item/upd",
            "/channels/ec/league/headsupplier/item/list/get",
            "/channels/ec/league/headsupplier/item/get",
            "/channels/ec/league/item/delete",
            "/channels/ec/league/promoter/add",
            "/channels/ec/league/promoter/delete",
            "/channels/ec/league/promoter/upd",
            "/channels/ec/league/funds/getfundsflowlist",
            "/channels/ec/league/funds/getfundsflowdetail",
        })
        {
            allRoutes.Count(r => r.Equals(removed, StringComparison.Ordinal))
                .Should().Be(0,
                    $"{removed} 的官方文档页已 404、整树降级为目录页，或已脱出「联盟带货机构 / 优选联盟」现行目录树（2026-10-17 核验）⇒ 非契约路由零声明，不得按第三方 SDK 清单回填");
        }

        // 正对照：商家侧同族现行路由与机构侧新落路由均在场，证明扫描根覆盖本域接口面。
        allRoutes.Should().Contain("/channels/ec/league/item/upd",
            "正对照：商家侧 item/upd 仍在官方目录内（与机构侧同名族不同前缀），扫描面必须覆盖");
        allRoutes.Should().Contain("/channels/ec/league/headsupplier/funds/flowlist/get",
            "正对照：机构侧 headsupplier/* 前缀路由必须被扫描面覆盖，否则本断言恒真");
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
