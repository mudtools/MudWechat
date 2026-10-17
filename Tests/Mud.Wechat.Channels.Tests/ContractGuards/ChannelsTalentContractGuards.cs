// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.League;
using Mud.Wechat.Channels.DataModels.Talent;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「带货助手（达人，talent）」域契约守卫（缺失 API 域完善方案 P1 / CT 系列：Talent 域 4 端点路由
/// （佣金单 2 + 达人橱窗商品 2）+ 全 POST 恰一个 <c>[Body]</c> + 令牌绑定与注册形态 + DTO 字段名与
/// 「与 Window / League 不合并」裁决 + JSON 上下文登记 + 官方枚举常量值域与「两张表分建」锁定
/// + 官方参数表↔示例矛盾照录）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域账号归属</b>：四端点官方适用范围逐页均为「<b>带货助手（需申请）</b>」，凭据为带货助手
/// <c>access_token</c> 或服务商代调用的 <c>authorizer_access_token</c>（权限集 190 佣金单 / 189 橱窗），
/// <b>四支均不支持云调用</b> —— 与 Promoter 域「不支持第三方平台代调用」恰好相反，不得互相套用。
/// </para>
/// <para>
/// <b>令牌类型不拆分</b>：带货助手 / 联盟机构 / 小店商家共用本线唯一 <c>Wechat.Channels.AccessToken</c>，
/// 「哪一种账号」由应用配置凭据区分（League / Promoter 先例裁决）。拆分令牌类型会改变注入参数名并破坏共享注册表键语义。
/// </para>
/// <para>
/// <b>三处近邻、零合并</b>：① 本域橱窗 <c>/channels/ec/talent/window/product/*</c>（权限集 189、
/// 状态枚举两值、错误码 102000xx）≠ Window 域 <c>/channels/ec/window/product/*</c>（权限集 133/177、
/// 错误码 100220xx、支持云调用）；② 本域佣金单 <c>/channels/ec/talent/get_order_*</c>（达人查自己的单）
/// ≠ League 域 <c>/channels/ec/league/headsupplier/order/*</c>（机构查全部单）；
/// ③ 两侧的订单状态与佣金单状态<b>是两张数值不同的表</b>（详见 CT6），故常量类分建。
/// </para>
/// </remarks>
public class ChannelsTalentContractGuards
{
    private const string TalentRegistryGroupName = "Talent";
    private const string TalentNamespace = "Mud.Wechat.Channels.DataModels.Talent";

    /// <summary>Talent 域官方路由表（4 端点：佣金单 2 + 达人橱窗商品 2，全部 POST，全部 <c>/channels/ec/talent/*</c>）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] TalentRoutes =
    {
        // 佣金单（2）。
        (typeof(IChannelsTalentService), nameof(IChannelsTalentService.GetOrderDetailAsync), typeof(PostAttribute), "/channels/ec/talent/get_order_detail"),
        (typeof(IChannelsTalentService), nameof(IChannelsTalentService.GetOrderListAsync), typeof(PostAttribute), "/channels/ec/talent/get_order_list"),
        // 达人橱窗商品（2）。
        (typeof(IChannelsTalentService), nameof(IChannelsTalentService.GetWindowProductDetailAsync), typeof(PostAttribute), "/channels/ec/talent/window/product/get"),
        (typeof(IChannelsTalentService), nameof(IChannelsTalentService.GetWindowProductListAsync), typeof(PostAttribute), "/channels/ec/talent/window/product/list/get"),
    };

    /// <summary>契约守卫 CT1：Talent 域 4 端点路由必须与官方契约一致（族计数 + 前缀独占）。</summary>
    [Fact]
    public void TalentEndpoints_ShouldMatchOfficialRoutes()
    {
        TalentRoutes.Should().HaveCount(4, "佣金单 2（详情 / 列表）+ 达人橱窗商品 2（详情 / 列表）= 4 端点");
        TalentRoutes.Select(r => r.Route).Distinct().Should().HaveCount(4, "路由不得重复");

        TalentRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/talent/", StringComparison.Ordinal))
            .Should().Be(4, "4 端点全部走 /channels/ec/talent/ 主干前缀");

        // 族内计数（路由名可辨识，防「加一支忘改统计」）。
        TalentRoutes.Select(r => r.Route).Count(static r => r.Contains("/talent/window/", StringComparison.Ordinal))
            .Should().Be(2, "橱窗族 2 支：product/get + product/list/get（详情与列表各一支）");
        TalentRoutes.Select(r => r.Route).Count(static r => r.StartsWith("/channels/ec/talent/get_order", StringComparison.Ordinal))
            .Should().Be(2, "佣金单族 2 支：get_order_detail + get_order_list（本域佣金单只有读面，官方无写端点）");

        foreach ((Type iface, string method, Type httpAttribute, string route) in TalentRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（下划线命名照抄官方，无驼峰改写）");
        }
    }

    /// <summary>契约守卫 CT2：全 POST + 恰一个 [Body] + 四支均返回域内响应 DTO（本域无 errcode/errmsg-only 端点）。</summary>
    [Fact]
    public void TalentEndpoints_ShouldAllBePost_WithSingleBody_AndTypedResponses()
    {
        foreach (var (iface, method, _, _) in TalentRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（四支端点参数表均在请求体，无 GET 端点）");

            var bodyParams = target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>()).ToList();
            bodyParams.Should().HaveCount(1,
                $"{method} 必须声明且仅声明一个 [Body] 参数（令牌不经签名声明，由 [Token] 注入 Query）");

            var returnType = target.ReturnType.GenericTypeArguments[0];
            returnType.Should().NotBe(typeof(ChannelsResponse),
                $"{method} 官方响应体均含业务字段（详情回对象、列表回数组 + 翻页上下文），必须使用域内响应 DTO");
            returnType.Namespace.Should().Be(TalentNamespace,
                $"{method} 的响应类型必须落 Talent 域命名空间（平铺，Order/ 与 Window/ 子目录仅作组织、不入命名空间）");
        }

        // 本域四支恰为「2 详情 + 2 列表」，详情响应不得带翻页字段、列表响应必须带翻页字段。
        JsonPropertyNamesOf<ChannelsTalentGetOrderDetailResponse>().Should().NotContain("has_more",
            "详情端点官方返回体无翻页字段（不得从列表端点复制粘贴）");
        JsonPropertyNamesOf<ChannelsTalentGetOrderListResponse>().Should().Contain("has_more", "列表端点官方回 has_more");
        JsonPropertyNamesOf<ChannelsTalentGetWindowProductListResponse>().Should().Contain("last_buffer",
            "橱窗列表官方回 last_buffer 顺序翻页上下文（本域橱窗用 last_buffer，佣金单用 next_key，两套翻页字段不共用）");
    }

    /// <summary>契约守卫 CT3：令牌绑定与注册形态（带货助手 access_token + Query 注入 + Talent 注册组）+ 与 Window 前缀互斥。</summary>
    [Fact]
    public void TalentInterface_ShouldBindChannelsToken_AndRegisterUnderTalentGroup()
    {
        var token = typeof(IChannelsTalentService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Talent 域 4 端点全部消费带货助手 access_token（或服务商代调用的 authorizer_access_token）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken,
            "带货助手与联盟机构 / 小店共用本线唯一令牌类型，账号差异由应用配置凭据区分，不按账号类型拆分令牌类型");
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsTalentService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(TalentRegistryGroupName, "必须挂 Talent 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Talent");
        typeof(ChannelsServiceBuilder).GetMethod("AddTalentApi").Should().NotBeNull("必须提供 AddTalentApi 链式注册方法");

        // 与 Window 域前缀互斥（两套官方契约，权限集 189 对 133/177）：双向扫描。
        var ownRoutes = RoutesOf(typeof(IChannelsTalentService));
        ownRoutes.Should().NotBeEmpty("反射枚举不得空跑（正对照）");
        ownRoutes.Should().OnlyContain(static r => r.StartsWith("/channels/ec/talent/", StringComparison.Ordinal),
            "本域路由全部在 /channels/ec/talent/ 前缀下，不得混入 Window 域 /channels/ec/window/ 前缀");

        var windowRoutes = RoutesOf(typeof(IChannelsWindowService));
        windowRoutes.Should().NotBeEmpty("Window 域路由枚举不得空跑（正对照）");
        windowRoutes.Should().NotContain(static r => r.StartsWith("/channels/ec/talent/", StringComparison.Ordinal),
            "Window 域（视频号助手橱窗）不得反向吸收本域 talent 前缀路由");

        // 与 League 域佣金单前缀互斥（达人侧自查 vs 机构侧全查）。
        var leagueRoutes = RoutesOf(typeof(IChannelsLeagueService));
        leagueRoutes.Should().NotBeEmpty("League 域路由枚举不得空跑（正对照）");
        leagueRoutes.Should().NotContain(static r => r.StartsWith("/channels/ec/talent/", StringComparison.Ordinal),
            "League 域机构侧佣金单不得改写到本域达人侧前缀");
    }

    /// <summary>契约守卫 CT4：Talent 域 DTO 全量登记进 AOT JSON 上下文（含常量类形态检查）。</summary>
    [Fact]
    public void TalentDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = TalentJsonContext.Default;

        // domainTypes 不过滤 abstract（静态常量类编译为 abstract + sealed，此处必须保留），
        // abstract 的排除只在 dtoTypes 子查询进行——否则常量类断言恒 0（前序批次踩坑）。
        var domainTypes = typeof(ChannelsTalentGetOrderDetailRequest).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == TalentNamespace
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        var constTypes = domainTypes
            .Where(t => t.IsAbstract && t.IsSealed)
            .ToList();

        dtoTypes.Should().HaveCount(17,
            "Talent 域 4 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（佣金单 11 = 请求 2（详情 / 列表）+ 响应 2 + 子树与条目 7（base_info / buyer_info / channel_info / " +
            "commission_info / order_list_item / product_info / promotion_head_supplier_info）；" +
            "橱窗 6 = 请求 2 + 响应 2 + 详情商品体 1 + 列表条目 1）");

        constTypes.Should().HaveCount(4,
            "Talent 域常量类 = 订单状态 + 佣金单状态 + 橱窗商品状态 + 橱窗商品来源 = 4");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Talent 域命名空间，必须登记进 TalentJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(TalentRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Talent");
        }

        foreach (var type in constTypes)
        {
            type.GetCustomAttribute<HttpJsonSerializableAttribute>().Should().BeNull(
                $"{type.Name} 为常量类（不参与 JSON 序列化），不得标 [HttpJsonSerializable]");
        }
    }

    /// <summary>契约守卫 CT5：官方字段名与「不共用 / 分建」裁决锁定。</summary>
    [Fact]
    public void TalentDataModels_ShouldLockOfficialFieldsAndSplitDecisions()
    {
        // ---- 佣金单详情 ----
        AssertJsonProperty<ChannelsTalentGetOrderDetailRequest>("order_id", "订单号（必填，来自列表）");
        AssertJsonProperty<ChannelsTalentGetOrderDetailRequest>("sku_id", "商品 sku_id（必填，来自列表）");
        AssertJsonProperty<ChannelsTalentGetOrderDetailRequest>("special_id", "订单额外参数（选填，回传列表值）");

        AssertJsonProperty<ChannelsTalentGetOrderDetailResponse>("base_info", "订单基础信息");
        AssertJsonProperty<ChannelsTalentGetOrderDetailResponse>("commission_info", "订单佣金信息");
        AssertJsonProperty<ChannelsTalentGetOrderDetailResponse>("channel_info", "下单通道信息");
        AssertJsonProperty<ChannelsTalentGetOrderDetailResponse>("promotion_head_supplier_info", "内容推广推客机构信息");
        AssertJsonProperty<ChannelsTalentGetOrderDetailResponse>("product_info", "商品信息");

        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("order_id", "订单 id");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("spu_id", "商品 spu_id");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("sku_id", "商品 sku_id");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("special_id", "特殊 id（官方注明「针对本地生活」）");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("order_status", "订单状态（枚举见 CT6）");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("actual_payment", "实际支付金额（官方注明单位分）");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("order_create_time", "订单创建时间");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("order_update_time", "订单更新时间");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("order_pay_time", "订单支付时间（2025-11-17 变更日志新增）");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("settle_payment", "分佣基数（2026-01-15 变更日志新增）");
        AssertJsonProperty<ChannelsTalentOrderBaseInfo>("buyer_info", "下单用户");

        AssertJsonProperty<ChannelsTalentOrderBuyerInfo>("open_id", "下单用户 openid");
        AssertJsonProperty<ChannelsTalentOrderBuyerInfo>("union_id", "下单用户 unionid");

        AssertJsonProperty<ChannelsTalentOrderCommissionInfo>("state", "佣金单状态（枚举见 CT6）");
        AssertJsonProperty<ChannelsTalentOrderCommissionInfo>("ratio", "佣金比例（官方标 string）");
        AssertJsonProperty<ChannelsTalentOrderCommissionInfo>("expect_settle_time", "预期结算时间");
        AssertJsonProperty<ChannelsTalentOrderCommissionInfo>("expect_settlement", "预期结算金额");
        AssertJsonProperty<ChannelsTalentOrderCommissionInfo>("actual_settle_time", "实际结算时间");
        AssertJsonProperty<ChannelsTalentOrderCommissionInfo>("actual_settlement", "实际结算金额");

        AssertJsonProperty<ChannelsTalentOrderChannelInfo>("channel_type", "渠道类型（1 视频号 / 2 公众号）");
        AssertJsonProperty<ChannelsTalentOrderChannelInfo>("channel_id", "渠道 id");
        AssertJsonProperty<ChannelsTalentOrderChannelInfo>("channel_name", "渠道名称");

        AssertJsonProperty<ChannelsTalentPromotionHeadSupplierInfo>("id", "机构 id（官方字段名为裸 id）");
        AssertJsonProperty<ChannelsTalentPromotionHeadSupplierInfo>("name", "机构名称");
        AssertJsonProperty<ChannelsTalentPromotionHeadSupplierInfo>("ratio", "佣金单比例");
        AssertJsonProperty<ChannelsTalentPromotionHeadSupplierInfo>("fee", "佣金");

        AssertJsonProperty<ChannelsTalentOrderProductInfo>("title", "商品标题");
        AssertJsonProperty<ChannelsTalentOrderProductInfo>("thumb_img", "商品头图（官方字段名 thumb_img，非 img_url）");

        // ---- 佣金单列表 ----
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("create_time_gt", "创建时间范围起");
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("create_time_lt", "创建时间范围止");
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("update_time_gt", "更新时间范围起");
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("update_time_lt", "更新时间范围止");
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("order_id", "订单 id 过滤");
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("spu_id", "商品 id 过滤");
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("page_size", "单页佣金单数（官方上限 10）");
        AssertJsonProperty<ChannelsTalentGetOrderListRequest>("next_key", "顺序翻页上下文");

        AssertJsonProperty<ChannelsTalentGetOrderListResponse>("order_list", "佣金单列表");
        AssertJsonProperty<ChannelsTalentGetOrderListResponse>("has_more", "是否还有剩余订单");
        AssertJsonProperty<ChannelsTalentGetOrderListResponse>("next_key", "本次翻页上下文");

        AssertJsonProperty<ChannelsTalentOrderListItem>("order_id", "列表条目订单 id");
        AssertJsonProperty<ChannelsTalentOrderListItem>("sku_id", "列表条目 sku_id");
        AssertJsonProperty<ChannelsTalentOrderListItem>("special_id", "列表条目特殊标识");

        // ---- 达人橱窗商品 ----
        AssertJsonProperty<ChannelsTalentGetWindowProductDetailRequest>("product_id", "橱窗商品 ID（官方标 string）");
        AssertJsonProperty<ChannelsTalentGetWindowProductDetailResponse>("product", "橱窗商品详情");

        AssertJsonProperty<ChannelsTalentWindowProduct>("product_id", "橱窗商品 ID");
        AssertJsonProperty<ChannelsTalentWindowProduct>("appid", "商品来源店铺 appid");
        AssertJsonProperty<ChannelsTalentWindowProduct>("out_product_id", "货源店铺商品 ID（带货商品才回）");
        AssertJsonProperty<ChannelsTalentWindowProduct>("title", "商品标题");
        AssertJsonProperty<ChannelsTalentWindowProduct>("img_url", "商品头图 url（详情侧官方字段名 img_url）");
        AssertJsonProperty<ChannelsTalentWindowProduct>("leaf_category_id", "叶子类目 ID");
        AssertJsonProperty<ChannelsTalentWindowProduct>("status", "商品状态（枚举见 CT6）");
        AssertJsonProperty<ChannelsTalentWindowProduct>("selling_price", "价格区间最小值（官方注明单位分）");
        AssertJsonProperty<ChannelsTalentWindowProduct>("stock", "剩余库存");
        AssertJsonProperty<ChannelsTalentWindowProduct>("sales", "销量");
        AssertJsonProperty<ChannelsTalentWindowProduct>("is_hide", "是否在橱窗设置对外隐藏");
        AssertJsonProperty<ChannelsTalentWindowProduct>("product_promotion_link", "跳转小店添加商品时传递跟佣信息用串");

        AssertJsonProperty<ChannelsTalentGetWindowProductListRequest>("page_size", "单页商品数（官方上限 500）");
        AssertJsonProperty<ChannelsTalentGetWindowProductListRequest>("page_index", "页码（与 last_buffer 互斥）");
        AssertJsonProperty<ChannelsTalentGetWindowProductListRequest>("last_buffer", "顺序翻页上下文（与 page_index 互斥）");

        AssertJsonProperty<ChannelsTalentGetWindowProductListResponse>("products", "橱窗商品列表");
        AssertJsonProperty<ChannelsTalentGetWindowProductListResponse>("last_buffer", "本次翻页上下文");

        AssertJsonProperty<ChannelsTalentWindowProductListItem>("product_id", "橱窗商品 id（官方参数表标 number，见 CT7）");
        AssertJsonProperty<ChannelsTalentWindowProductListItem>("appid", "自营商品时回来源店铺 appid");
        AssertJsonProperty<ChannelsTalentWindowProductListItem>("product_source", "商品来源类型（1 自营 / 2 带货）");
        AssertJsonProperty<ChannelsTalentWindowProductListItem>("out_product_id", "带货商品时回货源小店商品 id");

        // ---- 共用 / 分建裁决 ----
        // 详情商品体（12 字段）与列表条目（4 字段）官方参数表字段集差异大，且列表条目独立带 product_source，
        // 两支不共用一支 DTO；头图字段名详情侧为 img_url、佣金单商品信息侧为 thumb_img，各自照抄。
        JsonPropertyNamesOf<ChannelsTalentWindowProduct>().Should().HaveCount(12, "详情商品体官方参数表 12 行");
        JsonPropertyNamesOf<ChannelsTalentWindowProductListItem>().Should().HaveCount(4, "列表条目官方参数表 4 行");
        JsonPropertyNamesOf<ChannelsTalentWindowProduct>().Should().NotContain("product_source",
            "官方详情参数表无 product_source（来源类型只在列表侧回）");

        // 本域佣金单条目三元组（order_id / sku_id / special_id）与详情 base_info 不共用：
        // 详情侧为 11 字段全量单，条目侧仅回定位三元组。
        JsonPropertyNamesOf<ChannelsTalentOrderListItem>().Should().BeEquivalentTo(new[] { "order_id", "sku_id", "special_id" },
            "列表条目恰官方「Res.order_list(Array) Object Payload」三行");

        // 橱窗详情响应字段名与 Window 域同名响应不共用（两套契约、两套 DTO，命名空间隔离即证据）。
        JsonPropertyNamesOf<ChannelsTalentGetWindowProductDetailResponse>().Should().Contain("product");
        typeof(ChannelsTalentGetWindowProductDetailResponse).Assembly
            .GetType("Mud.Wechat.Channels.DataModels.Window.ChannelsGetWindowProductResponse", throwOnError: false)
            .Should().NotBeNull("Window 域详情响应必须仍独立存在（本域合并不掉它）");
    }

    /// <summary>契约守卫 CT6：官方枚举常量值域 + 与 League 两张表「数值不同故分建」锁定。</summary>
    [Fact]
    public void TalentConstants_ShouldLockOfficialEnumValues_AndStaySplitFromLeague()
    {
        var talentOrder = ConstValuesOf(typeof(ChannelsTalentOrderStatuses));
        var talentCommission = ConstValuesOf(typeof(ChannelsTalentCommissionOrderStates));

        talentOrder.Should().Equal(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            "本域订单状态官方为 1..10 十值（待支付…待使用）");
        talentCommission.Should().Equal(new[] { 1, 2, 3, 4 },
            "本域佣金单状态官方为 1..4 四值（待结算 / 已结算 / 取消结算 / 结算异常）");
        ConstValuesOf(typeof(ChannelsTalentWindowProductStatuses)).Should().Equal(new[] { 1, 2 },
            "橱窗商品状态官方仅两值（1 生效中 / 2 被禁止售卖）");
        ConstValuesOf(typeof(ChannelsTalentWindowProductSources)).Should().Equal(new[] { 1, 2 },
            "商品来源官方仅两值（1 自营 / 2 带货）");

        // 与 League 域是两张表（同值不同义 ⇒ 合并即静默错读）：
        // League 交易订单状态用 10/12/17/20/21/30/100/200，其 10=「待付款」而本域 10=「待使用」。
        var leagueTrade = ConstValuesOf(typeof(ChannelsLeagueHeadSupplierTradeOrderStatuses));
        leagueTrade.Should().NotBeEquivalentTo(talentOrder,
            "League 机构侧交易订单状态与本域达人侧订单状态是两张表（值集不同），不得合并常量类");
        leagueTrade.Should().Contain(10, "正对照：League 侧确有值 10（待付款）");
        talentOrder.Should().Contain(10, "正对照：本域侧亦有值 10（待使用）⇒ 同值不同义");

        var leagueCommission = ConstValuesOf(typeof(ChannelsLeagueHeadSupplierCommissionOrderStatuses));
        leagueCommission.Should().NotBeEquivalentTo(talentCommission,
            "League 机构侧佣金单状态（20/100/200）与本域达人侧（1..4）是两张表，不得合并");

        // 同名成员跨表不共用：League 的 Settled=100、本域 Settled=2（结算态在两表里数值不同）。
        ConstValueOf(typeof(ChannelsTalentCommissionOrderStates), nameof(ChannelsTalentCommissionOrderStates.Settled))
            .Should().Be(2, "本域「已结算」官方值为 2");
        ConstValueOf(typeof(ChannelsLeagueHeadSupplierCommissionOrderStatuses), nameof(ChannelsLeagueHeadSupplierCommissionOrderStatuses.Settled))
            .Should().Be(100, "League「已结算」官方值为 100 —— 与上一行同成员名不同值，正是分建理由");
    }

    /// <summary>契约守卫 CT7：官方参数表 ↔ 示例矛盾照录（建模口径逐条锁定，防「顺手统一」）。</summary>
    [Fact]
    public void TalentDataModels_ShouldRecordOfficialContradictions()
    {
        // ① 橱窗列表条目 product_id 官方参数表标 number、返回示例为带引号字符串 ⇒ SDK 按参数表建模为数值；
        //    同域详情侧参数表与示例均为 string ⇒ 两处类型不一致属官方原文，本守卫锁定「不做强统一」。
        PropertyOf<ChannelsTalentWindowProductListItem>("product_id").PropertyType
            .Should().Be(typeof(long?), "列表条目按官方参数表 number 建模（示例为字符串属文档矛盾）");
        PropertyOf<ChannelsTalentWindowProduct>("product_id").PropertyType
            .Should().Be(typeof(string), "详情侧按官方参数表 string 建模（示例亦为字符串，两处一致）");

        // ② actual_payment 官方注明「单位：分」而返回示例为 "99.99"（元形态）⇒ 照抄类型、不做单位换算。
        PropertyOf<ChannelsTalentOrderBaseInfo>("actual_payment").PropertyType
            .Should().Be(typeof(string), "官方参数表标 string ⇒ 照抄，不因示例像元而改数值或换算");
        PropertyOf<ChannelsTalentOrderBaseInfo>("settle_payment").PropertyType
            .Should().Be(typeof(string), "分佣基数与实付同为 string（官方两行类型标注一致）");

        // ③ 佣金侧金额/比例一律 string（ratio / expect_settlement / actual_settlement / 机构 ratio+fee），
        //    而时间戳一律数值：两类混排是官方原文，不得「对齐」成同一形态。
        PropertyOf<ChannelsTalentOrderCommissionInfo>("ratio").PropertyType.Should().Be(typeof(string));
        PropertyOf<ChannelsTalentOrderCommissionInfo>("expect_settlement").PropertyType.Should().Be(typeof(string));
        PropertyOf<ChannelsTalentOrderCommissionInfo>("actual_settlement").PropertyType.Should().Be(typeof(string));
        PropertyOf<ChannelsTalentOrderCommissionInfo>("expect_settle_time").PropertyType.Should().Be(typeof(long?),
            "时间戳官方标 number ⇒ 数值建模");
        PropertyOf<ChannelsTalentPromotionHeadSupplierInfo>("ratio").PropertyType.Should().Be(typeof(string));
        PropertyOf<ChannelsTalentPromotionHeadSupplierInfo>("fee").PropertyType.Should().Be(typeof(string));

        // ④ 详情请求：order_id / sku_id 官方标必填（SDK 侧仍以可空属性承载官方 JSON，必填性写在 XML 注释），
        //    选填的 special_id 走可空注解 —— 用 NullabilityInfoContext 真判可空性（PropertyType 对 string? 恒为 string，
        //    只断言 PropertyType 会恒真）。
        IsNullableRead(PropertyOf<ChannelsTalentGetOrderDetailRequest>("special_id"))
            .Should().BeTrue("special_id 官方标选填 ⇒ 读面属性须为可空引用");

        // ⑤ 橱窗详情商品的状态 / 价格 / 库存 / 销量为数值、is_hide 为布尔；列表请求 page_size 必填、
        //    page_index 与 last_buffer 均选填（官方「不可以与…一起填写」是互斥而非必填）。
        PropertyOf<ChannelsTalentWindowProduct>("is_hide").PropertyType.Should().Be(typeof(bool?));
        PropertyOf<ChannelsTalentWindowProduct>("selling_price").PropertyType.Should().Be(typeof(long?));
        PropertyOf<ChannelsTalentGetWindowProductListRequest>("page_index").PropertyType.Should().Be(typeof(int?),
            "page_index 官方标选填 ⇒ 数值可空");
        PropertyOf<ChannelsTalentGetWindowProductListRequest>("page_size").PropertyType.Should().Be(typeof(int),
            "page_size 官方标必填 ⇒ 非空数值（上限 500 由 XML 注释承载）");
        IsNullableRead(PropertyOf<ChannelsTalentGetWindowProductListRequest>("last_buffer")).Should().BeTrue(
            "last_buffer 官方标选填（与 page_index 互斥，两者皆可空、由调用方择一填写）");

        // ⑥ 官方返回示例缺失的字段仍按参数表建模（示例-only 反向：不得因示例没写就删字段）：
        //    详情示例无 promotion_head_supplier_info / order_pay_time / settle_payment，列表示例无 next_key。
        PropertyOf<ChannelsTalentOrderBaseInfo>("order_pay_time").Should().NotBeNull(
            "2025-11-17 变更日志新增字段，官方返回示例尚未回显 ⇒ 仍须建模");
        PropertyOf<ChannelsTalentOrderBaseInfo>("settle_payment").Should().NotBeNull(
            "2026-01-15 变更日志新增字段，官方返回示例尚未回显 ⇒ 仍须建模");
        PropertyOf<ChannelsTalentGetOrderDetailResponse>("promotion_head_supplier_info").Should().NotBeNull(
            "参数表有该对象而返回示例整段缺失 ⇒ 仍须建模");

        // ⑦ 翻页字段两套不混用：佣金单用 next_key + has_more，橱窗用 last_buffer（官方各自参数表）。
        JsonPropertyNamesOf<ChannelsTalentGetOrderListResponse>().Should().NotContain("last_buffer",
            "佣金单列表官方回 next_key，不回 last_buffer（橱窗侧才是 last_buffer）");
        JsonPropertyNamesOf<ChannelsTalentGetWindowProductListResponse>().Should().NotContain("next_key",
            "橱窗列表官方回 last_buffer，不回 next_key");
        JsonPropertyNamesOf<ChannelsTalentGetWindowProductListResponse>().Should().NotContain("has_more",
            "橱窗列表官方参数表无 has_more（只有 last_buffer）");

        // ⑧ 文档渲染缺陷照录（不作为契约字段建模）：两支列表响应的类型列在官方页渲染为
        //    「" data-v-…>objarray」，SDK 按各自的 Object Payload 小节建对象数组，不产 string 字段。
        JsonPropertyNamesOf<ChannelsTalentGetOrderListResponse>().Should().NotContain("data",
            "官方响应只有 order_list / has_more / next_key 三行，渲染残留的 objarray 不是字段");

        // ⑨ 官方请求示例存在畸形 JSON（get_order_detail 的 "order_id": 123""）⇒ 以参数表为准建模为 string，
        //    不据示例放宽为「数字或字符串」双形态。
        PropertyOf<ChannelsTalentGetOrderDetailRequest>("order_id").PropertyType.Should().Be(typeof(string),
            "官方参数表标 string；示例为畸形 JSON（多余引号），不作建模依据");

        // 正对照：若扫描/反射整体失效（属性名漂移），上面全部断言会以「找不到属性」变红而非恒真。
        JsonPropertyNamesOf<ChannelsTalentOrderBaseInfo>().Should().HaveCount(11,
            "base_info 官方参数表 11 行（含两次变更日志新增字段）全量建模");
    }

    // ---- helpers ------------------------------------------------------------

    private static List<string> RoutesOf(Type iface) =>
        iface.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .SelectMany(static m => m.GetCustomAttributes().OfType<HttpMethodAttribute>())
            .Select(static a => a.RequestUri ?? string.Empty)
            .ToList();

    private static IEnumerable<string> JsonPropertyNamesOf<T>() =>
        typeof(T).GetProperties().Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n is not null)
            .Select(static n => n!)
            .ToList();

    private static void AssertJsonProperty<T>(string name, string because)
    {
        var names = JsonPropertyNamesOf<T>();
        names.Should().Contain(name, $"{typeof(T).Name} 必须含官方字段 {name}（{because}）");
    }

    private static PropertyInfo PropertyOf<T>(string jsonName)
    {
        var property = typeof(T).GetProperties()
            .FirstOrDefault(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName);
        property.Should().NotBeNull($"{typeof(T).Name} 必须存在映射到官方字段 {jsonName} 的属性");
        return property!;
    }

    /// <summary>
    /// 读面可空性判定：<c>PropertyType</c> 对 <c>string?</c> 恒为 <c>typeof(string)</c>，
    /// 只比类型会恒真 ⇒ 必填性断言必须走 <see cref="NullabilityInfoContext"/>。
    /// </summary>
    private static bool IsNullableRead(PropertyInfo property) =>
        new NullabilityInfoContext().Create(property).ReadState == NullabilityState.Nullable;

    private static int[] ConstValuesOf(Type owner) =>
        owner.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(static f => f.IsLiteral && f.FieldType == typeof(int))
            .Select(static f => (int)f.GetRawConstantValue()!)
            .OrderBy(static v => v)
            .ToArray();

    private static int ConstValueOf(Type owner, string fieldName)
    {
        var field = owner.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull($"{owner.Name} 必须含常量 {fieldName}");
        return (int)field!.GetRawConstantValue()!;
    }
}
