// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.League;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 小店「优选联盟」域契约守卫（设计方案 v1 P2 / CLG 系列：League 域 11 端点路由（带货者 4 +
/// 商品操作 7）+ 全 POST + 令牌绑定 + DTO 字段与共用裁决 + JSON 上下文登记 + 官方改版矛盾照录 +
/// 枚举常量（含「type」一字两表辨析））。
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

    /// <summary>League 域官方路由表（11 端点：带货者 4 + 商品操作 7，全部 POST，全部 /channels/ec/league/*）。</summary>
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
    };

    /// <summary>契约守卫 CLG1：League 域 11 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void LeagueEndpoints_ShouldMatchOfficialRoutes()
    {
        // 11 端点 = 带货者 4（详情/列表/商品数据/编辑）+ 商品操作 7（批量新增/计划详情/机构推广/商品列表/计划更新/计划列表/带货者列表）。
        LeagueRoutes.Should().HaveCount(11, "带货者 4 + 商品操作 7 = 11 端点");
        LeagueRoutes.Select(r => r.Route).Distinct().Should().HaveCount(11);

        LeagueRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/league/", StringComparison.Ordinal))
            .Should().Be(11, "11 端点全部走 /channels/ec/league/* 主干前缀");

        foreach (var (iface, method, httpAttribute, route) in LeagueRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致（下划线路由 getproductdatalist 等照抄官方）");
        }
    }

    /// <summary>契约守卫 CLG2：League 域 11 端点全部为 POST 且均须携带 [Body]。</summary>
    [Fact]
    public void LeagueEndpoints_ShouldAllBePost_WithBodyRules()
    {
        foreach (var (iface, method, _, _) in LeagueRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约均为 POST（优选联盟无 GET 端点）");

            var bodyParams = target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>()).ToList();
            bodyParams.Should().HaveCount(1,
                $"{method} 官方为 POST 且参数表含请求体，必须声明且仅声明一个 [Body] 参数");
        }
    }

    /// <summary>契约守卫 CLG3：令牌绑定与注册形态。</summary>
    [Fact]
    public void LeagueInterface_ShouldBindChannelsToken_AndRegisterUnderLeagueGroup()
    {
        var token = typeof(IChannelsLeagueService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("League 域 11 端点消费小店 access_token（官方契约）");
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

        dtoTypes.Should().HaveCount(41,
            "League 域 11 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（请求 14（含 2 个列表元素 + 1 个更新形态嵌套）+ 响应根 8 + 嵌套/列表元素 19 = 41）");

        constTypes.Should().HaveCount(10,
            "League 域常量类 = 请求推广类别 + 推广三分类 + 计划状态 + 编辑带货者类型 + 更新操作类别 + " +
            "推广计划类型 + 机构计划类型 + 带货者等级 + 推广关系状态 + 商品数据类型 = 10");

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
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
