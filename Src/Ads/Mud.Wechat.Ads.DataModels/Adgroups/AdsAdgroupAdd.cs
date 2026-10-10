// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相关法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// 创建营销单元的请求体（官方 <c>adgroups/add</c> 请求根字段表，2026-10-10 逐页核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>必填集（官方标 <c>*</c> 的顶层字段）</b>：<c>account_id</c>、<c>adgroup_name</c>、
/// <c>marketing_goal</c>、<c>marketing_carrier_type</c>、<c>begin_date</c>、<c>end_date</c>、
/// <c>time_series</c>；另 <c>marketing_carrier_detail.marketing_carrier_id</c> 在载体类型需要详情时被标为必填。
/// 本类型把<b>全部</b>字段建为可空：<c>time_series</c> 等字段的合法性依赖版位/载体/账号权限组合，
/// 本地必填校验只会在「官方其实允许」的形态上报错，判定面在官方网关。
/// </para>
/// <para>
/// <b>「不传即不改」不适用于本端点</b>：这是<b>创建</b>端点，未出现的可选字段按官方默认值生效
/// （与 <see cref="AdsAdgroupUpdateRequest"/> 的语义相反 —— 更新端点里「字段缺席」= 保持原值，
/// 「字段为 null」在源生成序列化下同样被忽略，故两者在本 SDK 里不可区分，见 <c>AdsResponse</c> 的
/// <c>DefaultIgnoreCondition</c> 口径）。
/// </para>
/// <para>
/// <b>ADX 程序化投放的禁用面</b>（官方逐字段标注「ADX 程序化投放不可填写提交」）：
/// <c>configured_status</c>、<c>daily_budget</c>、<c>targeting</c>、<c>scene_spec</c>、
/// <c>automatic_site_enabled</c>、<c>first_day_begin_time</c>、<c>auto_acquisition_*</c>、
/// <c>smart_bid_type</c>、<c>smart_cost_cap</c>、<c>auto_derived_landing_page_switch</c>、
/// <c>search_expand_targeting_switch</c>、<c>search_expansion_switch</c>、<c>bid_scene</c>、
/// <c>material_package_id</c>、<c>deep_conversion_spec</c> 等 ⇒ 该模式下这些字段必须缺席，
/// SDK 不做模式判定（模式由账号权限决定，不在报文里）。
/// </para>
/// <para>
/// <b>幂等由请求头承担、不在请求体里</b>：官方为本端点单列 <c>X-Request-Id</c>
/// 「资源请求的唯一 id……即使重复请求，API 侧永远不会新建一个全新的投放资源」⇒
/// 做成端点方法的显式可选 <c>[Header]</c> 参数（见 <c>IWechatAdsAdgroupService.AddAsync</c>）而非本类型的字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupAddRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>）。官方原文：有操作权限的帐号 id，<b>不支持代理商 id</b>。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>营销单元名称（官方 <c>adgroup_name</c>，<c>string</c>，<b>必填</b>）。同一帐号下不允许重复；
    /// 长度 1–60 个等宽字符（即最多 60 个中文/全角标点，或 120 个英文半角字符）。</summary>
    [JsonPropertyName("adgroup_name")]
    public string? AdgroupName { get; set; }

    /// <summary>营销目的类型（官方 <c>marketing_goal</c>，<c>enum</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("marketing_goal")]
    public string? MarketingGoal { get; set; }

    /// <summary>二级营销目的类型（官方 <c>marketing_sub_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_sub_goal")]
    public string? MarketingSubGoal { get; set; }

    /// <summary>营销载体类型（官方 <c>marketing_carrier_type</c>，<c>enum</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("marketing_carrier_type")]
    public string? MarketingCarrierType { get; set; }

    /// <summary>营销载体详情（官方 <c>marketing_carrier_detail</c>，<c>struct</c>，载体类型为应用/小游戏等时使用）。</summary>
    [JsonPropertyName("marketing_carrier_detail")]
    public AdsMarketingCarrierDetail? MarketingCarrierDetail { get; set; }

    /// <summary>开始投放日期（官方 <c>begin_date</c>，<c>string</c>，<b>必填</b>，格式 <c>YYYY-MM-DD</c>，须不大于 <see cref="EndDate"/>）。</summary>
    [JsonPropertyName("begin_date")]
    public string? BeginDate { get; set; }

    /// <summary>结束投放日期（官方 <c>end_date</c>，<c>string</c>，<b>必填</b>，格式 <c>YYYY-MM-DD</c>，
    /// 须大于等于今天且不小于 <see cref="BeginDate"/>；微信流量的更新场景另有「结束后至少当前时间 6 小时之后」等附加规则）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>首日开始投放时间（官方 <c>first_day_begin_time</c>，<c>string</c>，格式 <c>HH:ii:ss</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("first_day_begin_time")]
    public string? FirstDayBeginTime { get; set; }

    /// <summary>出价，单位为分（官方 <c>bid_amount</c>，<c>integer</c>。ADX 程序化投放默认填写 200，取值规则见官方「出价规则」）。</summary>
    [JsonPropertyName("bid_amount")]
    public long? BidAmount { get; set; }

    /// <summary>优化目标类型（官方 <c>optimization_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("optimization_goal")]
    public string? OptimizationGoal { get; set; }

    /// <summary>投放时间段（官方 <c>time_series</c>，<c>string</c>，<b>必填</b>，长度 336 字节）：
    /// <c>48 * 7</c> 位 0/1 串，以半小时为最小粒度，从周一零点开始至周日 24 点结束；全传 1 视为全时段投放，
    /// <b>不允许全部传 0</b>；朋友圈营销的投放时间需大于等于 6 小时、小于等于 30 个自然日、且每天至少投放 6 小时并保持时段一致。</summary>
    [JsonPropertyName("time_series")]
    public string? TimeSeries { get; set; }

    /// <summary>是否开启智能版位功能（官方 <c>automatic_site_enabled</c>，<c>boolean</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("automatic_site_enabled")]
    public bool? AutomaticSiteEnabled { get; set; }

    /// <summary>投放站点集合（官方 <c>site_set</c>，<c>enum[]</c>）。官方限制：当前单站点，
    /// 或 <c>SITE_SET_TENCENT_NEWS</c> + <c>SITE_SET_TENCENT_VIDEO</c> 的组合。</summary>
    [JsonPropertyName("site_set")]
    public List<string>? SiteSet { get; set; }

    /// <summary>探索策略（官方 <c>exploration_strategy</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("exploration_strategy")]
    public string? ExplorationStrategy { get; set; }

    /// <summary>优先版位集合（官方 <c>priority_site_set</c>，<c>enum[]</c>）。</summary>
    [JsonPropertyName("priority_site_set")]
    public List<string>? PrioritySiteSet { get; set; }

    /// <summary>日预算，单位为分（官方 <c>daily_budget</c>，<c>integer</c>；0 表示不设预算；
    /// ADX 程序化投放不可填写提交；区间与「不得低于今日已消耗」类约束见官方原文，判定依赖当日消耗 ⇒ 不本地拦截）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>定向详细设置（官方 <c>targeting</c>，<c>struct</c>），见 <see cref="AdsAdgroupTargeting"/>。</summary>
    [JsonPropertyName("targeting")]
    public AdsAdgroupTargeting? Targeting { get; set; }

    /// <summary>场景定向（官方 <c>scene_spec</c>，<c>struct</c>；ADX 程序化投放不可填写提交），见 <see cref="AdsAdgroupSceneSpec"/>。</summary>
    [JsonPropertyName("scene_spec")]
    public AdsAdgroupSceneSpec? SceneSpec { get; set; }

    /// <summary>用户行为数据源（官方 <c>user_action_sets</c>，<c>struct[]</c>），见 <see cref="AdsUserActionSet"/>。</summary>
    [JsonPropertyName("user_action_sets")]
    public List<AdsUserActionSet>? UserActionSets { get; set; }

    /// <summary>oCPA 深度优化内容（官方 <c>deep_conversion_spec</c>，<c>struct</c>；不传或传空视为无限制条件，
    /// ADX 程序化投放不可填写提交），见 <see cref="AdsDeepConversionSpec"/>。</summary>
    [JsonPropertyName("deep_conversion_spec")]
    public AdsDeepConversionSpec? DeepConversionSpec { get; set; }

    /// <summary>转化 id（官方 <c>conversion_id</c>，<c>integer</c>。官方原文：仅 ocpc/ocpm 营销可设置，
    /// <b>设置后不可修改</b>；传入转化 id 后可不再传 <see cref="UserActionSets"/>）。</summary>
    [JsonPropertyName("conversion_id")]
    public long? ConversionId { get; set; }

    /// <summary>深度优化行为出价，单位为分（官方 <c>deep_conversion_behavior_bid</c>，<c>integer</c>；
    /// 使用 <see cref="ConversionId"/> 确认优化目标时可免传 <see cref="DeepConversionSpec"/> 内的同名字段）。</summary>
    [JsonPropertyName("deep_conversion_behavior_bid")]
    public long? DeepConversionBehaviorBid { get; set; }

    /// <summary>深度优化价值出价（比值，官方 <c>deep_conversion_worth_rate</c>，<c>float</c>，简写入口说明同 <see cref="DeepConversionBehaviorBid"/>）。</summary>
    [JsonPropertyName("deep_conversion_worth_rate")]
    public double? DeepConversionWorthRate { get; set; }

    /// <summary>强化优化价值的期望 ROI（官方 <c>deep_conversion_worth_advanced_rate</c>，<c>float</c>，简写入口说明同 <see cref="DeepConversionBehaviorBid"/>）。</summary>
    [JsonPropertyName("deep_conversion_worth_advanced_rate")]
    public double? DeepConversionWorthAdvancedRate { get; set; }

    /// <summary>深度辅助优化 OG 出价，单位为分（官方 <c>deep_conversion_behavior_advanced_bid</c>，<c>integer</c>，简写入口说明同 <see cref="DeepConversionBehaviorBid"/>）。</summary>
    [JsonPropertyName("deep_conversion_behavior_advanced_bid")]
    public long? DeepConversionBehaviorAdvancedBid { get; set; }

    /// <summary>出价方式（官方 <c>bid_mode</c>，<c>enum</c>。ADX 程序化投放仅支持 <c>{ BID_MODE_CPC, BID_MODE_CPM }</c>）。</summary>
    [JsonPropertyName("bid_mode")]
    public string? BidMode { get; set; }

    /// <summary>一键起量开关（官方 <c>auto_acquisition_enabled</c>，<c>boolean</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("auto_acquisition_enabled")]
    public bool? AutoAcquisitionEnabled { get; set; }

    /// <summary>一键起量预算，单位为分（官方 <c>auto_acquisition_budget</c>，<c>integer</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("auto_acquisition_budget")]
    public long? AutoAcquisitionBudget { get; set; }

    /// <summary>出价类型（官方 <c>smart_bid_type</c>，<c>enum</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("smart_bid_type")]
    public string? SmartBidType { get; set; }

    /// <summary>自动出价下系统计算的预计成本上限，单位为分（官方 <c>smart_cost_cap</c>，<c>integer</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("smart_cost_cap")]
    public long? SmartCostCap { get; set; }

    /// <summary>创意增强 MAX 开关（官方 <c>auto_derived_creative_enabled</c>，<c>boolean</c>。
    /// 官方原文：使用即代表已阅读并遵守「妙思创意增强 MAX API 服务协议」）。</summary>
    [JsonPropertyName("auto_derived_creative_enabled")]
    public bool? AutoDerivedCreativeEnabled { get; set; }

    /// <summary>创意增强 MAX 偏好设置（官方 <c>auto_derived_creative_preference</c>，<c>struct</c>，
    /// 不设置则按默认偏好全选生效），见 <see cref="AdsAutoDerivedCreativePreference"/>。</summary>
    [JsonPropertyName("auto_derived_creative_preference")]
    public AdsAutoDerivedCreativePreference? AutoDerivedCreativePreference { get; set; }

    /// <summary>搜索定向拓展开关（官方 <c>search_expand_targeting_switch</c>，<c>enum</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("search_expand_targeting_switch")]
    public string? SearchExpandTargetingSwitch { get; set; }

    /// <summary>是否开启自动衍生落地页开关（官方 <c>auto_derived_landing_page_switch</c>，<c>boolean</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("auto_derived_landing_page_switch")]
    public bool? AutoDerivedLandingPageSwitch { get; set; }

    /// <summary>出价场景（官方 <c>bid_scene</c>，<c>enum</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("bid_scene")]
    public string? BidScene { get; set; }

    /// <summary>客户设置的状态（官方 <c>configured_status</c>，<c>enum</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; set; }

    /// <summary>是否使用自动流量优选（官方 <c>flow_optimization_enabled</c>，<c>boolean</c>。
    /// 官方标注该功能<b>已废弃</b>，原说明仅支持腾讯营销联盟版位且不可与联盟行业精选/媒体类型场景定向同时使用 —— 字段照录）。</summary>
    [JsonPropertyName("flow_optimization_enabled")]
    public bool? FlowOptimizationEnabled { get; set; }

    /// <summary>素材标签 id（官方 <c>material_package_id</c>，<c>integer</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("material_package_id")]
    public long? MaterialPackageId { get; set; }

    /// <summary>产品 id（官方 <c>marketing_asset_id</c>，<c>integer</c>。
    /// 与 <see cref="MarketingAssetOuterSpec"/> 的二选一约束见该类型 remarks）。</summary>
    [JsonPropertyName("marketing_asset_id")]
    public long? MarketingAssetId { get; set; }

    /// <summary>产品外部 id 数据（官方 <c>marketing_asset_outer_spec</c>，<c>struct</c>），见 <see cref="AdsMarketingAssetOuterSpec"/>。</summary>
    [JsonPropertyName("marketing_asset_outer_spec")]
    public AdsMarketingAssetOuterSpec? MarketingAssetOuterSpec { get; set; }

    /// <summary>POI 列表（官方 <c>poi_list</c>，<c>string[]</c>）。</summary>
    [JsonPropertyName("poi_list")]
    public List<string>? PoiList { get; set; }

    /// <summary>电商 PKAM 开关（官方 <c>ecom_pkam_switch</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("ecom_pkam_switch")]
    public string? EcomPkamSwitch { get; set; }

    /// <summary>前置链路辅助（官方 <c>forward_link_assist</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("forward_link_assist")]
    public string? ForwardLinkAssist { get; set; }

    /// <summary>RTA 策略 id（官方 <c>rta_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("rta_id")]
    public long? RtaId { get; set; }

    /// <summary>RTA 目标 id（官方 <c>rta_target_id</c>，<c>string</c>。该值同时是 <c>adgroups/get</c> 的一个过滤字段）。</summary>
    [JsonPropertyName("rta_target_id")]
    public string? RtaTargetId { get; set; }

    /// <summary>动态商品营销规格（官方 <c>mpa_spec</c>，<c>struct</c>），见 <see cref="AdsMpaSpec"/>。</summary>
    [JsonPropertyName("mpa_spec")]
    public AdsMpaSpec? MpaSpec { get; set; }

    /// <summary>成本约束场景（官方 <c>cost_constraint_scene</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("cost_constraint_scene")]
    public string? CostConstraintScene { get; set; }

    /// <summary>自定义成本上限（官方 <c>custom_cost_cap</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("custom_cost_cap")]
    public long? CustomCostCap { get; set; }

    /// <summary>反馈 id（官方 <c>feedback_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("feedback_id")]
    public long? FeedbackId { get; set; }

    /// <summary>短剧付费类型（官方 <c>short_play_pay_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("short_play_pay_type")]
    public string? ShortPlayPayType { get; set; }

    /// <summary>售卖策略 id（官方 <c>sell_strategy_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("sell_strategy_id")]
    public long? SellStrategyId { get; set; }

    /// <summary>动态广告类型（官方 <c>dynamic_ad_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("dynamic_ad_type")]
    public string? DynamicAdType { get; set; }

    /// <summary>动态创意广告规格（官方 <c>dca_spec</c>，<c>struct</c>），见 <see cref="AdsDcaSpec"/>。</summary>
    [JsonPropertyName("dca_spec")]
    public AdsDcaSpec? DcaSpec { get; set; }

    /// <summary>DSP id（官方 <c>dsp_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("dsp_id")]
    public long? DspId { get; set; }

    /// <summary>AOI 优化策略（官方 <c>aoi_optimization_strategy</c>，<c>struct</c>），见 <see cref="AdsAoiOptimizationStrategy"/>。</summary>
    [JsonPropertyName("aoi_optimization_strategy")]
    public AdsAoiOptimizationStrategy? AoiOptimizationStrategy { get; set; }

    /// <summary>云统包规格（官方 <c>cloud_union_spec</c>，<c>struct</c>），见 <see cref="AdsCloudUnionSpec"/>。</summary>
    [JsonPropertyName("cloud_union_spec")]
    public AdsCloudUnionSpec? CloudUnionSpec { get; set; }

    /// <summary>附加商品规格（官方 <c>additional_product_spec</c>，<c>struct</c>），见 <see cref="AdsAdditionalProductSpec"/>。</summary>
    [JsonPropertyName("additional_product_spec")]
    public AdsAdditionalProductSpec? AdditionalProductSpec { get; set; }

    /// <summary>是否开启直播推荐策略（官方 <c>live_recommend_strategy_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("live_recommend_strategy_enabled")]
    public bool? LiveRecommendStrategyEnabled { get; set; }

    /// <summary>自定义成本 ROI 上限（官方 <c>custom_cost_roi_cap</c>，<c>float</c>）。</summary>
    [JsonPropertyName("custom_cost_roi_cap")]
    public double? CustomCostRoiCap { get; set; }

    /// <summary>搜索扩量开关（官方 <c>search_expansion_switch</c>，<c>enum</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("search_expansion_switch")]
    public string? SearchExpansionSwitch { get; set; }

    /// <summary>行业价值探索（官方 <c>industry_value_explore</c>，<c>struct</c>），见 <see cref="AdsIndustryValueExplore"/>。</summary>
    [JsonPropertyName("industry_value_explore")]
    public AdsIndustryValueExplore? IndustryValueExplore { get; set; }

    /// <summary>ADX 实时类型（官方 <c>adx_realtime_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("adx_realtime_type")]
    public string? AdxRealtimeType { get; set; }

    /// <summary>是否开启稳定探索（官方 <c>enable_steady_exploration</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("enable_steady_exploration")]
    public bool? EnableSteadyExploration { get; set; }

    /// <summary>智能定向模式（官方 <c>smart_targeting_mode</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("smart_targeting_mode")]
    public string? SmartTargetingMode { get; set; }

    /// <summary>智能优惠券模式（官方 <c>smart_coupon_mode</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("smart_coupon_mode")]
    public string? SmartCouponMode { get; set; }

    /// <summary>是否为智能投放升级版项目（官方 <c>is_smart_delivery_upgrade_project</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_smart_delivery_upgrade_project")]
    public bool? IsSmartDeliveryUpgradeProject { get; set; }
}

/// <summary>
/// 创建营销单元的应答载荷（官方 <c>data</c> 只有新建资源的 id）。
/// </summary>
/// <remarks>
/// 官方应答示例把值写成占位符 <c>"&lt;ADGROUP_ID&gt;"</c>（字符串）而字段表标 <c>int64</c> ⇒
/// 本类型按<b>字段表</b>取 <see cref="long"/>，占位符只是文档写法（同 <c>advertiser/get</c> 的
/// <c>account_id</c> 情形，见 <c>AdsAdvertiserInfo</c> remarks 的「示例与字段表矛盾照录」）。
/// 该形态被 <c>add</c> / <c>update</c> / <c>delete</c> 三支共用 ⇒ 建为 <see cref="AdsAdgroupIdData"/>。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupIdData
{
    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }
}

/// <summary><c>adgroups/add</c> 应答信封（闭合类型，供源生成上下文解析）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupAddResponse : AdsResponse<AdsAdgroupIdData>
{
}
