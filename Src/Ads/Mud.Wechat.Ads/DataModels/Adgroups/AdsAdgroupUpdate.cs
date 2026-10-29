// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相关法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// 更新营销单元的请求体（官方 <c>adgroups/update</c> 请求根字段表，2026-10-10 逐页核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与 <see cref="AdsAdgroupAddRequest"/> 的字段集差异是官方事实、不是遗漏</b>：本页<b>不</b>开放
/// <c>marketing_goal</c> / <c>marketing_sub_goal</c> / <c>marketing_carrier_type</c> /
/// <c>marketing_carrier_detail</c> / <c>site_set</c> / <c>exploration_strategy</c> /
/// <c>priority_site_set</c> / <c>smart_bid_type</c> / <c>smart_cost_cap</c> / <c>bid_scene</c> /
/// <c>material_package_id</c> / <c>marketing_asset_id</c> / <c>marketing_asset_outer_spec</c> /
/// <c>mpa_spec</c> / <c>dca_spec</c> / <c>additional_product_spec</c> / <c>dsp_id</c> /
/// <c>dynamic_ad_type</c> / <c>short_play_pay_type</c> / <c>sell_strategy_id</c> /
/// <c>feedback_id</c> 之外的若干营销目标类字段 ⇒ 这些层级在创建后不可改，本类型<b>不得</b>为了「对齐 add」而补上。
/// </para>
/// <para>
/// <b>「字段缺席」的行为不在 SDK 一侧</b>：官方本页<b>未</b>声明「不传即不改」（<c>advertiser/update</c> 才有该原文），
/// 而本线序列化上下文统一 <c>DefaultIgnoreCondition = WhenWritingNull</c> ⇒ 显式传 <c>null</c> 与不赋值
/// 在报文上<b>不可区分</b>（都不会出现该键）。因此「传 null 表示清空」这类直觉在本 SDK 不成立，
/// 需要清空的字段请照官方页给出的可用取值显式传值。
/// </para>
/// <para><b>必填集</b>：官方仅标 <c>account_id</c> 与 <c>adgroup_id</c> 两支顶层 <c>*</c>；
/// 其余 <c>*</c> 都出现在子结构内部（<c>location_types</c> / <c>min</c> / <c>max</c> /
/// <c>excluded_dimension</c> / <c>aoi_optimization_strategy_enabled</c> / <c>enabled</c> /
/// <c>expected_roi</c> 等），语义是「传了父结构就必须给」⇒ 由官方判定，SDK 不本地拦截。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>；不支持代理商 id）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>营销单元名称（官方 <c>adgroup_name</c>，<c>string</c>；同一帐号下不允许重复，长度约束见 <see cref="AdsAdgroupAddRequest.AdgroupName"/>）。</summary>
    [JsonPropertyName("adgroup_name")]
    public string? AdgroupName { get; set; }

    /// <summary>开始投放日期（官方 <c>begin_date</c>，<c>string</c>，格式 <c>YYYY-MM-DD</c>，须不大于 <see cref="EndDate"/>）。</summary>
    [JsonPropertyName("begin_date")]
    public string? BeginDate { get; set; }

    /// <summary>结束投放日期（官方 <c>end_date</c>，<c>string</c>，格式 <c>YYYY-MM-DD</c>；更新微信流量的结束时间时官方另附加时间隔规则）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>首日开始投放时间（官方 <c>first_day_begin_time</c>，<c>string</c>，格式 <c>HH:ii:ss</c>）。</summary>
    [JsonPropertyName("first_day_begin_time")]
    public string? FirstDayBeginTime { get; set; }

    /// <summary>出价，单位为分（官方 <c>bid_amount</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("bid_amount")]
    public long? BidAmount { get; set; }

    /// <summary>优化目标类型（官方 <c>optimization_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("optimization_goal")]
    public string? OptimizationGoal { get; set; }

    /// <summary>投放时间段（官方 <c>time_series</c>，<c>string</c>，长度 336 字节，形态约束见 <see cref="AdsAdgroupAddRequest.TimeSeries"/>）。</summary>
    [JsonPropertyName("time_series")]
    public string? TimeSeries { get; set; }

    /// <summary>日预算，单位为分（官方 <c>daily_budget</c>，<c>integer</c>；0 表示不设预算）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>定向详细设置（官方 <c>targeting</c>，<c>struct</c>），见 <see cref="AdsAdgroupTargeting"/>。</summary>
    [JsonPropertyName("targeting")]
    public AdsAdgroupTargeting? Targeting { get; set; }

    /// <summary>场景定向（官方 <c>scene_spec</c>，<c>struct</c>），见 <see cref="AdsAdgroupSceneSpec"/>。</summary>
    [JsonPropertyName("scene_spec")]
    public AdsAdgroupSceneSpec? SceneSpec { get; set; }

    /// <summary>用户行为数据源（官方 <c>user_action_sets</c>，<c>struct[]</c>），见 <see cref="AdsUserActionSet"/>。</summary>
    [JsonPropertyName("user_action_sets")]
    public List<AdsUserActionSet>? UserActionSets { get; set; }

    /// <summary>oCPA 深度优化内容（官方 <c>deep_conversion_spec</c>，<c>struct</c>），见 <see cref="AdsDeepConversionSpec"/>。</summary>
    [JsonPropertyName("deep_conversion_spec")]
    public AdsDeepConversionSpec? DeepConversionSpec { get; set; }

    /// <summary>转化 id（官方 <c>conversion_id</c>，<c>integer</c>。官方原文「设置后不可修改」⇒ 更新端点出现该字段属官方表定义，能否改值以官方应答为准）。</summary>
    [JsonPropertyName("conversion_id")]
    public long? ConversionId { get; set; }

    /// <summary>深度优化行为出价，单位为分（官方 <c>deep_conversion_behavior_bid</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("deep_conversion_behavior_bid")]
    public long? DeepConversionBehaviorBid { get; set; }

    /// <summary>深度优化价值出价（比值）（官方 <c>deep_conversion_worth_rate</c>，<c>float</c>）。</summary>
    [JsonPropertyName("deep_conversion_worth_rate")]
    public double? DeepConversionWorthRate { get; set; }

    /// <summary>强化优化价值的期望 ROI（官方 <c>deep_conversion_worth_advanced_rate</c>，<c>float</c>）。</summary>
    [JsonPropertyName("deep_conversion_worth_advanced_rate")]
    public double? DeepConversionWorthAdvancedRate { get; set; }

    /// <summary>深度辅助优化 OG 出价，单位为分（官方 <c>deep_conversion_behavior_advanced_bid</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("deep_conversion_behavior_advanced_bid")]
    public long? DeepConversionBehaviorAdvancedBid { get; set; }

    /// <summary>出价方式（官方 <c>bid_mode</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("bid_mode")]
    public string? BidMode { get; set; }

    /// <summary>一键起量开关（官方 <c>auto_acquisition_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("auto_acquisition_enabled")]
    public bool? AutoAcquisitionEnabled { get; set; }

    /// <summary>一键起量预算，单位为分（官方 <c>auto_acquisition_budget</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("auto_acquisition_budget")]
    public long? AutoAcquisitionBudget { get; set; }

    /// <summary>是否开启自动衍生落地页开关（官方 <c>auto_derived_landing_page_switch</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("auto_derived_landing_page_switch")]
    public bool? AutoDerivedLandingPageSwitch { get; set; }

    /// <summary>创意增强 MAX 开关（官方 <c>auto_derived_creative_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("auto_derived_creative_enabled")]
    public bool? AutoDerivedCreativeEnabled { get; set; }

    /// <summary>创意增强 MAX 偏好设置（官方 <c>auto_derived_creative_preference</c>，<c>struct</c>），见 <see cref="AdsAutoDerivedCreativePreference"/>。</summary>
    [JsonPropertyName("auto_derived_creative_preference")]
    public AdsAutoDerivedCreativePreference? AutoDerivedCreativePreference { get; set; }

    /// <summary>客户设置的状态（官方 <c>configured_status</c>，<c>enum</c>。批量改状态走
    /// <see cref="AdsAdgroupUpdateConfiguredStatusRequest"/>）。</summary>
    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; set; }

    /// <summary>是否使用自动流量优选（官方 <c>flow_optimization_enabled</c>，<c>boolean</c>；官方标注该功能已废弃，字段照录）。</summary>
    [JsonPropertyName("flow_optimization_enabled")]
    public bool? FlowOptimizationEnabled { get; set; }

    /// <summary>POI 列表（官方 <c>poi_list</c>，<c>string[]</c>）。</summary>
    [JsonPropertyName("poi_list")]
    public List<string>? PoiList { get; set; }

    /// <summary>电商 PKAM 开关（官方 <c>ecom_pkam_switch</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("ecom_pkam_switch")]
    public string? EcomPkamSwitch { get; set; }

    /// <summary>RTA 策略 id（官方 <c>rta_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("rta_id")]
    public long? RtaId { get; set; }

    /// <summary>RTA 目标 id（官方 <c>rta_target_id</c>，<c>string</c>）。</summary>
    [JsonPropertyName("rta_target_id")]
    public string? RtaTargetId { get; set; }

    /// <summary>成本约束场景（官方 <c>cost_constraint_scene</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("cost_constraint_scene")]
    public string? CostConstraintScene { get; set; }

    /// <summary>自定义成本上限（官方 <c>custom_cost_cap</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("custom_cost_cap")]
    public long? CustomCostCap { get; set; }

    /// <summary>反馈 id（官方 <c>feedback_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("feedback_id")]
    public long? FeedbackId { get; set; }

    /// <summary>AOI 优化策略（官方 <c>aoi_optimization_strategy</c>，<c>struct</c>），见 <see cref="AdsAoiOptimizationStrategy"/>。</summary>
    [JsonPropertyName("aoi_optimization_strategy")]
    public AdsAoiOptimizationStrategy? AoiOptimizationStrategy { get; set; }

    /// <summary>搜索定向拓展开关（官方 <c>search_expand_targeting_switch</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("search_expand_targeting_switch")]
    public string? SearchExpandTargetingSwitch { get; set; }

    /// <summary>云统包规格（官方 <c>cloud_union_spec</c>，<c>struct</c>），见 <see cref="AdsCloudUnionSpec"/>。</summary>
    [JsonPropertyName("cloud_union_spec")]
    public AdsCloudUnionSpec? CloudUnionSpec { get; set; }

    /// <summary>是否开启直播推荐策略（官方 <c>live_recommend_strategy_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("live_recommend_strategy_enabled")]
    public bool? LiveRecommendStrategyEnabled { get; set; }

    /// <summary>自定义成本 ROI 上限（官方 <c>custom_cost_roi_cap</c>，<c>float</c>）。</summary>
    [JsonPropertyName("custom_cost_roi_cap")]
    public double? CustomCostRoiCap { get; set; }

    /// <summary>行业价值探索（官方 <c>industry_value_explore</c>，<c>struct</c>），见 <see cref="AdsIndustryValueExplore"/>。</summary>
    [JsonPropertyName("industry_value_explore")]
    public AdsIndustryValueExplore? IndustryValueExplore { get; set; }

    /// <summary>智能定向模式（官方 <c>smart_targeting_mode</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("smart_targeting_mode")]
    public string? SmartTargetingMode { get; set; }

    /// <summary>智能优惠券模式（官方 <c>smart_coupon_mode</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("smart_coupon_mode")]
    public string? SmartCouponMode { get; set; }
}

/// <summary><c>adgroups/update</c> 应答信封（官方 <c>data</c> 只回 <see cref="AdsAdgroupIdData"/>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateResponse : AdsResponse<AdsAdgroupIdData>
{
}
