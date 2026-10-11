// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json.Serialization;
using Mud.Wechat.Channels.DataModels.Live;
using Mud.Wechat.Channels.Extensions;

namespace Mud.Wechat.Channels.Tests.ContractGuards;

/// <summary>
/// 视频号助手「直播」域契约守卫（CLV 系列：Live 域 12 端点路由 + 四前缀并存 + 令牌绑定 +
/// DTO 字段与共用裁决 + JSON 上下文登记 + 枚举常量 + 官方矛盾照录）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「罗盘」域概念辨析（守卫措辞锁定）</b>：本域是视频号侧直播原始数据与留资组件/大屏
/// （视频号助手 <c>access_token</c>）；商家/达人经营分析指标走 Compass 域。
/// </para>
/// <para>
/// <b>四前缀并存（官方原文，照抄不得改写）</b>：<c>/channels/ec/finderlive/*</c>（直播记录/预约 2）+
/// <c>/channels/finderlive/*</c>（留资服务的直播数据 3，无 <c>/ec</c> 段）+ <c>/channels/leads/*</c>
/// （留资组件 5）+ <c>/channels/livedashboard/*</c>（直播大屏 2）。
/// </para>
/// <para>
/// <b>官方矛盾照录锁定</b>：finder_attr 参数表 objarray vs 示例单对象；exposure_click_rate 参数表
/// boolean vs 示例数值；live_notice_list / dist_flow_type_stats / j / sub_channel_source_stats
/// 参数表 object vs 示例数组——均按参数表类型列建模（容器形态按示例数组防真实报文反序列化崩溃），禁止「顺手统一」。
/// </para>
/// </remarks>
public class ChannelsLiveContractGuards
{
    private const string LiveRegistryGroupName = "Live";

    /// <summary>Live 域官方路由表（12 端点：/channels/ec/finderlive/* 2 + /channels/finderlive/* 3 + /channels/leads/* 5 + /channels/livedashboard/* 2，全部 POST）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] LiveRoutes =
    {
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetFinderLiveRecordListAsync), typeof(PostAttribute), "/channels/ec/finderlive/getfinderliverecordlist"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetFinderLiveNoticeRecordListAsync), typeof(PostAttribute), "/channels/ec/finderlive/getfinderlivenoticerecordlist"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetLeadsInfoByRequestIdAsync), typeof(PostAttribute), "/channels/leads/get_leads_info_by_request_id"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetLeadsInfoByComponentIdAsync), typeof(PostAttribute), "/channels/leads/get_leads_info_by_component_id"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetLeadsRequestIdListAsync), typeof(PostAttribute), "/channels/leads/get_leads_request_id"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetLeadsComponentPromoteRecordAsync), typeof(PostAttribute), "/channels/leads/get_leads_component_promote_record"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetLeadsComponentIdListAsync), typeof(PostAttribute), "/channels/leads/get_leads_component_id"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetFinderAttrByAppidAsync), typeof(PostAttribute), "/channels/finderlive/get_finder_attr_by_appid"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetFinderLiveDataListAsync), typeof(PostAttribute), "/channels/finderlive/get_finder_live_data_list"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetFinderLiveLeadsDataAsync), typeof(PostAttribute), "/channels/finderlive/get_finder_live_leads_data"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetLiveDashboardListAsync), typeof(PostAttribute), "/channels/livedashboard/getlivelist"),
        (typeof(IChannelsLiveService), nameof(IChannelsLiveService.GetLiveDashboardDataAsync), typeof(PostAttribute), "/channels/livedashboard/getlivedata"),
    };

    /// <summary>契约守卫 CLV1：Live 域 12 端点路由必须与官方契约一致（全部 POST）。</summary>
    [Fact]
    public void LiveEndpoints_ShouldMatchOfficialRoutes()
    {
        // 12 端点 = 直播记录/预约 2 + 留资组件 5 + 留资服务的直播数据 3 + 直播大屏 2（四前缀并存）。
        LiveRoutes.Should().HaveCount(12, "直播记录 2 + 留资组件 5 + 留资直播数据 3 + 直播大屏 2 = 12 端点");
        LiveRoutes.Select(r => r.Route).Distinct().Should().HaveCount(12);

        LiveRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/ec/finderlive/", StringComparison.Ordinal))
            .Should().Be(2, "2 端点走 /channels/ec/finderlive/* 直播记录/预约前缀");
        LiveRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/finderlive/", StringComparison.Ordinal))
            .Should().Be(3, "3 端点走 /channels/finderlive/* 留资服务的直播数据前缀（无 /ec 段，官方原文）");
        LiveRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/leads/", StringComparison.Ordinal))
            .Should().Be(5, "5 端点走 /channels/leads/* 留资组件前缀");
        LiveRoutes.Select(r => r.Route)
            .Count(static r => r.StartsWith("/channels/livedashboard/", StringComparison.Ordinal))
            .Should().Be(2, "2 端点走 /channels/livedashboard/* 直播大屏前缀");

        foreach (var (iface, method, httpAttribute, route) in LiveRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明对应 HTTP 方法路由");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约一致");
        }
    }

    /// <summary>契约守卫 CLV2：Live 域 12 端点全部为 POST；空请求体端点不得携带 [Body] 参数。</summary>
    [Fact]
    public void LiveEndpoints_ShouldAllBePost_WithEmptyBodyRules()
    {
        foreach (var (iface, method, _, _) in LiveRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!;
            target!.GetCustomAttribute<PostAttribute>().Should().NotBeNull(
                $"{iface.Name}.{method} 官方契约为 POST");
        }

        // 官方请求体为空的三端点（getfinderliverecordlist / getfinderlivenoticerecordlist /
        // get_finder_attr_by_appid「调用接口时传空的json串即可」）不得人为造请求体 DTO。
        foreach (var emptyEndpoint in new[]
                 {
                     nameof(IChannelsLiveService.GetFinderLiveRecordListAsync),
                     nameof(IChannelsLiveService.GetFinderLiveNoticeRecordListAsync),
                     nameof(IChannelsLiveService.GetFinderAttrByAppidAsync),
                 })
        {
            var target = typeof(IChannelsLiveService).GetMethod(emptyEndpoint)!;
            target.GetParameters().SelectMany(static p => p.GetCustomAttributes<BodyAttribute>())
                .Should().BeEmpty($"{emptyEndpoint} 官方规定空请求体——不得声明 [Body]");
        }
    }

    /// <summary>契约守卫 CLV3：令牌绑定与注册形态。</summary>
    [Fact]
    public void LiveInterface_ShouldBindChannelsToken_AndRegisterUnderLiveGroup()
    {
        var token = typeof(IChannelsLiveService).GetCustomAttribute<TokenAttribute>();
        token.Should().NotBeNull("Live 域 12 端点消费视频号助手 access_token（官方契约）");
        token!.TokenType.Should().Be(ChannelsTokenTypes.AccessToken);
        token.InjectionMode.Should().Be(TokenInjectionMode.Query, "官方契约强制 Query 注入（MUD005）");
        token.Name.Should().Be("access_token");

        var api = typeof(IChannelsLiveService).GetCustomAttribute<HttpClientApiAttribute>();
        api.Should().NotBeNull("必须声明 [HttpClientApi]");
        api!.RegistryGroupName.Should().Be(LiveRegistryGroupName, "必须挂 Live 注册组");
        api.TokenManage.Should().Be(nameof(IChannelsAppManager));

        Enum.GetNames(typeof(ChannelsModule)).Should().Contain("Live");
        typeof(ChannelsServiceBuilder).GetMethod("AddLiveApi").Should().NotBeNull("必须提供 AddLiveApi 链式注册方法");
    }

    /// <summary>契约守卫 CLV4：Live 域 DTO 全量登记进 AOT JSON 上下文；常量类不标序列化特性。</summary>
    [Fact]
    public void LiveDataModels_ShouldBeRegisteredInJsonContext()
    {
        var context = LiveJsonContext.Default;

        var domainTypes = typeof(ChannelsLeadsInfoResponse).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsNested
                        && t.Namespace == "Mud.Wechat.Channels.DataModels.Live"
                        && !typeof(JsonSerializerContext).IsAssignableFrom(t))
            .ToList();

        var dtoTypes = domainTypes
            .Where(t => !t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() != null)
            .ToList();

        dtoTypes.Should().HaveCount(53,
            "Live 域 12 端点的请求/响应/嵌套对象 DTO 总数漂移须先核对官方文档再同批调整本守卫" +
            "（直播记录 2 + 预约记录 2 + 账号信息 2 + 留资直播数据 3 + 留资数量 3 + 大屏列表 3 +" +
            " 大屏数据 24 + 场次留资请求 2 + 共用留资响应与元素 3 + request_id 3 + 推广记录 3 + 组件 id 3 = 53）");

        foreach (var type in dtoTypes)
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 位于 Live 域命名空间，必须登记进 LiveJsonContext");

            type.GetCustomAttribute<HttpJsonSerializableAttribute>()!.SerializerClassName
                .Should().Be(LiveRegistryGroupName, $"{type.Name} 的 SerializerClassName 必须为 Live");
        }

        // 常量类（静态常量类不参与序列化）：Live 域 7 个，均不得标 [HttpJsonSerializable]。
        var constantTypes = domainTypes
            .Where(t => t.IsSealed && t.IsAbstract && t.GetCustomAttribute<HttpJsonSerializableAttribute>() == null)
            .ToList();
        constantTypes.Should().HaveCount(7,
            "Live 域常量类 = 留资组件状态 + 留资来源类型 + 留资组件类型 + 留资流量来源 + 大屏画像维度 + 渠道场景类型 + 渠道流量类型");
        constantTypes.Should().OnlyContain(
            t => !t.IsDefined(typeof(HttpJsonSerializableAttribute), false),
            "常量类仅承载枚举字面量，不得标 [HttpJsonSerializable]");
    }

    /// <summary>契约守卫 CLV5：官方字段名与共用 DTO 裁决锁定（抽样关键端点 + 全部共用裁决）。</summary>
    [Fact]
    public void LiveDataModels_ShouldLockOfficialFieldsAndSharedDtos()
    {
        // 直播记录 / 预约记录。
        AssertJsonProperty<ChannelsLiveGetFinderLiveRecordListResponse>("live_list", "直播记录列表");
        AssertJsonProperty<ChannelsLiveRecord>("export_id", "直播 id");
        AssertJsonProperty<ChannelsLiveRecord>("cover_url", "直播封面");
        AssertJsonProperty<ChannelsLiveGetFinderLiveNoticeRecordListResponse>("live_notice_list", "直播预约记录列表");
        AssertJsonProperty<ChannelsLiveNoticeRecord>("notice_id", "预约 id");
        AssertJsonProperty<ChannelsLiveNoticeRecord>("status_wording", "预约状态说明");
        AssertJsonProperty<ChannelsLiveNoticeRecord>("start_time", "预约直播开始时间（官方类型 string，照抄）");

        // 视频号账号信息（finder_attr 参数表 objarray vs 示例单对象——按参数表列表建模）。
        AssertJsonProperty<ChannelsGetFinderAttrResponse>("finder_attr", "视频号信息");
        AssertJsonProperty<ChannelsFinderAttr>("uniq_id", "视频号唯一标识");
        AssertJsonProperty<ChannelsFinderAttr>("fans_count", "粉丝数");

        // 留资服务直播数据。
        AssertJsonProperty<ChannelsGetFinderLiveDataListRequest>("last_buffer", "顺序翻页上下文");
        AssertJsonProperty<ChannelsGetFinderLiveDataListResponse>("continue_flag", "是否还有直播");
        AssertJsonProperty<ChannelsFinderLiveItem>("live_duration_in_seconds", "直播时长");
        AssertJsonProperty<ChannelsFinderLiveItem>("audiences_avg_seconds", "人均观看时长");
        AssertJsonProperty<ChannelsFinderLiveItem>("new_follow_count_biz", "公众号新增粉丝");
        AssertJsonProperty<ChannelsGetFinderLiveLeadsDataRequest>("source_type", "来源类型");
        AssertJsonProperty<ChannelsFinderLiveLeadsItem>("component_type", "组件类型");
        AssertJsonProperty<ChannelsFinderLiveLeadsItem>("leads_count", "留资条数");

        // 留资组件。
        AssertJsonProperty<ChannelsLeadsInfoByRequestIdRequest>("request_id", "留资信息请求 id");
        AssertJsonProperty<ChannelsLeadsInfoByRequestIdRequest>("version", "接口版本号");
        AssertJsonProperty<ChannelsLeadsInfoByComponentIdRequest>("leads_component_id", "留资组件 id");
        AssertJsonProperty<ChannelsLeadsInfoResponse>("user_data", "用户留资信息列表");
        AssertJsonProperty<ChannelsLeadsInfoResponse>("last_buffer", "翻页上下文");
        AssertJsonProperty<ChannelsLeadsUserData>("anchor_nickname", "主播昵称");
        AssertJsonProperty<ChannelsLeadsUserData>("close_flag", "关闭标识");
        AssertJsonProperty<ChannelsLeadsDataField>("title", "标题");
        AssertJsonProperty<ChannelsLeadsDataField>("value", "值");
        AssertJsonProperty<ChannelsLeadsRequestIdResponse>("item", "直播场次列表");
        AssertJsonProperty<ChannelsLeadsRequestIdItem>("request_id", "某一场直播对应的留资信息请求 id");
        AssertJsonProperty<ChannelsLeadsPromoteRecordResponse>("record_data", "直播推广记录列表");
        AssertJsonProperty<ChannelsLeadsPromoteRecord>("exposure_click_rate", "点击转化率（官方参数表 boolean，照录）");
        AssertJsonProperty<ChannelsLeadsPromoteRecord>("click_uv", "点击率（官方说明原文照录）");
        AssertJsonProperty<ChannelsLeadsComponentIdItem>("leads_component_id", "留资组件 id");
        AssertJsonProperty<ChannelsLeadsComponentIdItem>("status", "留资组件状态码");

        // 直播大屏。
        AssertJsonProperty<ChannelsGetLiveDashboardListRequest>("ds", "日期（number 形态）");
        AssertJsonProperty<ChannelsGetLiveDashboardListResponse>("trace_id", "追踪 ID");
        AssertJsonProperty<ChannelsGetLiveDashboardListResponse>("live_items", "直播信息列表");
        AssertJsonProperty<ChannelsGetLiveDashboardListResponse>("has_more", "是否还有更多的直播");
        AssertJsonProperty<ChannelsLiveDashboardItem>("export_id", "直播唯一 ID");
        AssertJsonProperty<ChannelsGetLiveDashboardDataRequest>("export_id", "直播唯一 ID");
        AssertJsonProperty<ChannelsGetLiveDashboardDataResponse>("live_dashboard_data", "直播大屏基础数据");
        AssertJsonProperty<ChannelsGetLiveDashboardDataResponse>("live_comparison_index", "内容力数据");
        AssertJsonProperty<ChannelsGetLiveDashboardDataResponse>("live_ec_data_summary", "电商数据概要");
        AssertJsonProperty<ChannelsGetLiveDashboardDataResponse>("live_ec_conversion_metric", "电商转化力数据");
        AssertJsonProperty<ChannelsGetLiveDashboardDataResponse>("live_ec_profile", "电商画像数据");
        AssertJsonProperty<ChannelsGetLiveDashboardDataResponse>("live_distribution_channel", "电商渠道分布");
        AssertJsonProperty<ChannelsGetLiveDashboardDataResponse>("single_live_ec_spu_data_page_v2", "电商商品数据");
        AssertJsonProperty<ChannelsLiveDashboardData>("LiveDashboardData2", "官方字面键名（大小写照抄，不改写）");
        AssertJsonProperty<ChannelsLiveDashboardSummary>("new_watch_uv", "观看人数");
        AssertJsonProperty<ChannelsLiveDashboardSummary>("average_watch_seconds_per_audience", "平均观看时长");
        AssertJsonProperty<ChannelsLiveDashboardSummary>("new_fans_club_uv", "新增粉丝团人数");
        AssertJsonProperty<ChannelsLiveDashboardSummary>("hot_quota", "热度");
        AssertJsonProperty<ChannelsLiveDashboardSource>("new_watch_uv", "渠道分布（维度组合动态键）");
        AssertJsonProperty<ChannelsLiveDashboardPortrait>("online_watch_uv", "在线人数画像（维度组合动态键）");
        AssertJsonProperty<ChannelsLiveComparisonIndex>("is_living", "是否正在直播");
        AssertJsonProperty<ChannelsLiveComparisonOnAir>("recommend_effective_new_watch_2_uv_over_impression_uv", "曝光有效 CTR");
        AssertJsonProperty<ChannelsLiveOnAirIndexItem>("last_2n_to_n_mins_value", "最近 2n~n 分钟值");
        AssertJsonProperty<ChannelsLiveEndedIndexItem>("median_7_days", "近 7 天中位数");
        AssertJsonProperty<ChannelsLiveEcDataSummary>("total_gmv", "成交金额（分）");
        AssertJsonProperty<ChannelsLiveEcConversionMetric>("recent_10_min_conversion", "近 10 分钟转化率数据");
        AssertJsonProperty<ChannelsLiveConversionMetric>("shelftime", "上架时间（官方实体示例字段）");
        AssertJsonProperty<ChannelsLiveEcProfile>("profiles", "用户群体统计数据（实体未展开，JsonElement 保真）");
        AssertJsonProperty<ChannelsLiveDistributionChannel>("live_dist_channel_source_by_scene_stats", "按场景划分的渠道统计");
        AssertJsonProperty<ChannelsLiveDistSceneStats>("metric_value_total", "指标值总数");
        AssertJsonProperty<ChannelsLiveDistFlowTypeStats>("live_dst_channel_type", "流量类型");
        AssertJsonProperty<ChannelsLiveDistChannelNode>("sub_channel_source_stats", "二级渠道来源统计");
        AssertJsonProperty<ChannelsLiveDistChannelNode>("source_channel_name", "渠道来源名称");
        AssertJsonProperty<ChannelsLiveDistSubChannelNode>("metric_value_ratio", "指标值占比");
        AssertJsonProperty<ChannelsLiveSpuDataPage>("spu_data_list", "商品明细数据（参数表标注 object，照录）");
        AssertJsonProperty<ChannelsLiveSpuDataPage>("total_cnt", "总长度");
        AssertJsonProperty<ChannelsLiveSpuData>("exp_clk_ratio", "商品曝光点击率");
        AssertJsonProperty<ChannelsLiveSpuBaseData>("src_spu_id", "商品 id");
        AssertJsonProperty<ChannelsLiveSpuBaseData>("src", "店铺名称（官方类型 number，照录）");

        // 类型口径锁定：动态键画像/画像列表以 JsonElement 保真（官方未展开实体结构）。
        typeof(ChannelsLiveDashboardSource).GetProperty(nameof(ChannelsLiveDashboardSource.NewWatchUv))!
            .PropertyType.Should().Be(typeof(Dictionary<string, System.Text.Json.JsonElement>),
                "source.new_watch_uv 键为维度组合字符串（动态键），官方未定义值结构");
        typeof(ChannelsLiveEcProfile).GetProperty(nameof(ChannelsLiveEcProfile.Profiles))!
            .PropertyType.Should().Be(typeof(List<System.Text.Json.JsonElement>),
                "profiles 官方仅给实体名未给字段表，保真透传");

        // 共用 DTO 裁决：get_leads_info_by_request_id 与 get_leads_info_by_component_id 响应字段集完全一致 ⇒ 共用 ChannelsLeadsInfoResponse。
        typeof(IChannelsLiveService).GetMethod(nameof(IChannelsLiveService.GetLeadsInfoByRequestIdAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsLeadsInfoResponse>));
        typeof(IChannelsLiveService).GetMethod(nameof(IChannelsLiveService.GetLeadsInfoByComponentIdAsync))!
            .ReturnType.Should().Be(typeof(Task<ChannelsLeadsInfoResponse>));

        // 共用 DTO 裁决：live_dist_channel_source_stats 与 dist_flow_type_stats 官方子表字段集完全一致 ⇒ 共用 ChannelsLiveDistFlowTypeStats。
        typeof(ChannelsLiveDistributionChannel).GetProperty(nameof(ChannelsLiveDistributionChannel.LiveDistChannelSourceStats))!
            .PropertyType.Should().Be(typeof(ChannelsLiveDistFlowTypeStats));
    }

    /// <summary>契约守卫 CLV6：枚举常量字面量锁定。</summary>
    [Fact]
    public void LiveEnumConstants_ShouldLockOfficialLiteralValues()
    {
        ChannelsLeadsComponentStatuses.NotSubmitted.Should().Be(1);
        ChannelsLeadsComponentStatuses.PendingReview.Should().Be(2);
        ChannelsLeadsComponentStatuses.Rejected.Should().Be(3);
        ChannelsLeadsComponentStatuses.Approved.Should().Be(4);
        ChannelsLeadsComponentStatuses.Deleted.Should().Be(5);

        ChannelsLeadsSourceTypes.Live.Should().Be(0);
        ChannelsLeadsComponentTypes.Form.Should().Be(0);
        ChannelsLeadsComponentTypes.WecomCard.Should().Be(1);
        ChannelsLeadsComponentTypes.WecomKf.Should().Be(2);
        ChannelsLeadsTrafficTypes.Natural.Should().Be(0);
        ChannelsLeadsTrafficTypes.Advertisement.Should().Be(1);

        ChannelsLiveDimensionTypes.PrimaryChannel.Should().Be(1);
        ChannelsLiveDimensionTypes.AgeGroup.Should().Be(2);
        ChannelsLiveDimensionTypes.Gender.Should().Be(3);
        ChannelsLiveDimensionTypes.SecondaryChannel.Should().Be(7);
        ChannelsLiveDimensionTypes.StrategyAudience.Should().Be(9);
        ChannelsLiveDimensionTypes.Province.Should().Be(10);
        ChannelsLiveDimensionTypes.CategoryPreference.Should().Be(12);
        ChannelsLiveDimensionTypes.PriceRange.Should().Be(13);
        ChannelsLiveDimensionTypes.TrafficType.Should().Be(16);

        ChannelsLiveDistributionSceneTypes.ProductExposure.Should().Be(6);
        ChannelsLiveDistributionSceneTypes.LiveExposure.Should().Be(7);
        ChannelsLiveDistributionSceneTypes.ProductClick.Should().Be(8);
        ChannelsLiveDistributionSceneTypes.OrderCreate.Should().Be(9);
        ChannelsLiveDistributionSceneTypes.OrderPay.Should().Be(10);

        ChannelsLiveDistributionFlowTypes.Invalid.Should().Be(0);
        ChannelsLiveDistributionFlowTypes.Natural.Should().Be(1);
        ChannelsLiveDistributionFlowTypes.Boosted.Should().Be(2);
        ChannelsLiveDistributionFlowTypes.Advertisement.Should().Be(3);
        ChannelsLiveDistributionFlowTypes.PublicDomain.Should().Be(4);
        ChannelsLiveDistributionFlowTypes.PrivateDomain.Should().Be(5);
    }

    private static void AssertJsonProperty<T>(string jsonName, string because)
    {
        typeof(T).GetProperties()
            .Any(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            .Should().BeTrue($"{typeof(T).Name} 必须含官方字段名 '{jsonName}'（{because}）");
    }
}
