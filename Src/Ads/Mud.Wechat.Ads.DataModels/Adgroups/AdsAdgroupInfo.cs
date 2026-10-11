// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// <c>adgroups/get</c> 的 <c>data</c> 载荷（列表 + 两种分页元信息）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两种分页形态同时在场、字段集与本域请求侧不对称</b>：官方把 <c>page_info</c>（普通翻页）与
/// <c>cursor_page_info</c>（游标翻页）列在同一个 <c>data</c> 下、各自「按请求的 <c>pagination_mode</c> 返回」
/// ⇒ 两支均建为可空。<b>游标形态与 <c>advertiser/get</c> 不同名不同型</b>：本页是
/// <c>{page_size, total_number, next_cursor, previous_cursor}</c> 且两支游标都是 <c>string</c>，
/// 而 <c>advertiser/get</c> 是 <c>{page_size, total_number, has_more, cursor}</c> 且 <c>cursor</c> 是
/// <c>integer</c> ⇒ <b>不得</b>收敛成公共类型（收敛等于把某一页的字段名当成全页事实），见
/// <see cref="AdsAdgroupCursorPageInfo"/>。
/// </para>
/// <para>
/// <b>请求侧 <c>cursor</c> 与应答侧 <c>next_cursor</c> 不同名</b>：请求参数叫 <c>cursor</c>（string，0–10 字节），
/// 应答返回 <c>next_cursor</c> / <c>previous_cursor</c> ⇒ 翻页时把应答的 <c>next_cursor</c> 填回请求的
/// <c>cursor</c>，官方报文里没有同名的往返字段。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupListData
{
    /// <summary>营销单元列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsAdgroupInfo>? List { get; set; }

    /// <summary>普通翻页模式的分页信息（官方 <c>page_info</c>；游标模式不返回）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }

    /// <summary>游标翻页模式的分页信息（官方 <c>cursor_page_info</c>；普通模式不返回）。</summary>
    [JsonPropertyName("cursor_page_info")]
    public AdsAdgroupCursorPageInfo? CursorPageInfo { get; set; }
}

/// <summary>
/// <c>adgroups/get</c> 的游标分页信息（字段名与类型照官方 <c>adgroups/get</c> 应答表原文）。
/// </summary>
/// <remarks>
/// <b>本域独有形态，勿与其它域合并</b>：官方各 <c>*/get</c> 页的 <c>cursor_page_info</c> 字段集<b>互不相同</b>
/// （<c>advertiser/get</c> 用 <c>has_more</c> + <c>integer</c> 型 <c>cursor</c>，本页用
/// <c>next_cursor</c> / <c>previous_cursor</c> 两支 <c>string</c>）⇒ 按域分建是本线硬约定，
/// 公共 <see cref="AdsPageInfo"/> 只覆盖 <c>page_info</c>。
/// 官方另注：游标有效期为 24 小时；游标模式下<b>不保存初次数据快照</b> ⇒ 数据变化可能导致结果不一致。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupCursorPageInfo
{
    /// <summary>每页条数（官方 <c>page_size</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("page_size")]
    public long? PageSize { get; set; }

    /// <summary>总条数（官方 <c>total_number</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("total_number")]
    public long? TotalNumber { get; set; }

    /// <summary>下一页游标（官方 <c>next_cursor</c>，<c>string</c>；回填请求的 <c>cursor</c> 参数）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>上一页游标（官方 <c>previous_cursor</c>，<c>string</c>）。</summary>
    [JsonPropertyName("previous_cursor")]
    public string? PreviousCursor { get; set; }
}

/// <summary>
/// <c>adgroups/get</c> 应答的营销单元（<c>data.list[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// <b>层级权威来自 DOM，不来自平面阅读</b>（本类型是最容易建错的一支）：官方页面的字段嵌套藏在表格单元格的
/// <c>level-two</c> … <c>level-five</c> class 里，把渲染文本按平面读法抽取会<b>读错三处父子归属</b> ——
/// ① <c>targeting_translation</c> 实际与 <c>targeting</c> <b>平级</b>（不是它的子字段）；
/// ② <c>poi_list</c> 实际与 <c>marketing_asset_outer_spec</c> <b>平级</b>（不是它的子字段）；
/// ③ <c>scene_spec.wechat_scene</c> 下只有 <c>official_account_media_category</c> /
/// <c>mini_program_and_mini_game</c> / <c>pay_scene</c> 三支，<c>wechat_position</c> 等六支是
/// <c>scene_spec</c> 的直接子字段。本类型按 <b>DOM 层级</b>建模。
/// </para>
/// <para>
/// <b>无法用应答示例交叉验证（官方缺陷，照录不规避）</b>：本页应答示例只有
/// <c>{"code":0,"message":"","message_cn":""}</c>，<b>整个 <c>data</c> 缺席</b>，而应答字段表有近 80 个顶层字段
/// ⇒ 本类型的字段集唯一来源是字段表；「示例里没有」在本域<b>不能</b>当作「字段不存在」的证据。
/// </para>
/// <para>
/// <b>字段可空性由两个机制决定，两者都不构成数据缺失</b>：
/// ① 请求侧 <c>fields</c>（<c>string[]</c>，数组 1–1024、每项 1–64 字节）可裁剪返回字段 ⇒
/// 未请求的字段为 <c>null</c> 是正常形态；② 子结构按营销目标类型/流量出现（如
/// <c>marketing_target_ext</c> / <c>negative_word_cnt</c> 只在特定场景有值）⇒ 全部字段建为可空。
/// </para>
/// <para>
/// <b>只读字段（本类型独有、请求侧不存在）</b>：<c>system_status</c>（系统状态，与可写的
/// <c>configured_status</c> 是两支不同语义）、<c>targeting_translation</c>（定向条件的人读文本）、
/// <c>data_model_version</c>、<c>deep_optimization_type</c>、<c>promoted_asset_type</c>、
/// <c>marketing_scene</c>、<c>auto_acquisition_status</c>、<c>smart_targeting_status</c>、
/// <c>og_completion_type</c>、<c>cost_guarantee_status</c> / <c>cost_guarantee_money</c>、
/// <c>enable_breakthrough_siteset</c>、<c>conversion_name</c>、<c>created_time</c>、
/// <c>last_modified_time</c>、<c>is_deleted</c> ⇒ 这些字段<b>不得</b>回填进
/// <see cref="AdsAdgroupAddRequest"/> / <see cref="AdsAdgroupUpdateRequest"/>（官方两张请求表里没有它们）。
/// </para>
/// <para>
/// <b>官方跨页描述冲突（照录）</b>：<c>site_set</c> 在本页描述为「投放站点集合，当前单站点或者
/// <c>SITE_SET_TENCENT_NEWS</c> + <c>SITE_SET_TENCENT_VIDEO</c> 的组合」，而 <c>adgroups/add</c> 页描述为
/// 「投放版位集合，推荐使用智能版位…使用智能版位时 <c>site_set</c> 字段无需传值」⇒ 两处语义不互通，
/// 本类型只承载值、不做取值校验。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupInfo
{
    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>营销单元名称（官方 <c>adgroup_name</c>，<c>string</c>）。</summary>
    [JsonPropertyName("adgroup_name")]
    public string? AdgroupName { get; set; }

    /// <summary>营销目标（官方 <c>marketing_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_goal")]
    public string? MarketingGoal { get; set; }

    /// <summary>子营销目标（官方 <c>marketing_sub_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_sub_goal")]
    public string? MarketingSubGoal { get; set; }

    /// <summary>营销载体类型（官方 <c>marketing_carrier_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_carrier_type")]
    public string? MarketingCarrierType { get; set; }

    /// <summary>营销载体明细（官方 <c>marketing_carrier_detail</c>，<c>struct</c>），见 <see cref="AdsMarketingCarrierDetail"/>。</summary>
    [JsonPropertyName("marketing_carrier_detail")]
    public AdsMarketingCarrierDetail? MarketingCarrierDetail { get; set; }

    /// <summary>营销对象类型（官方 <c>marketing_target_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_target_type")]
    public string? MarketingTargetType { get; set; }

    /// <summary>营销对象明细（官方 <c>marketing_target_detail</c>，<c>struct</c>），见 <see cref="AdsMarketingTargetDetail"/>。</summary>
    [JsonPropertyName("marketing_target_detail")]
    public AdsMarketingTargetDetail? MarketingTargetDetail { get; set; }

    /// <summary>营销对象 id（官方 <c>marketing_target_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("marketing_target_id")]
    public long? MarketingTargetId { get; set; }

    /// <summary>被推广资产的类型（官方 <c>promoted_asset_type</c>，<c>enum</c>；只读，请求表无此字段）。</summary>
    [JsonPropertyName("promoted_asset_type")]
    public string? PromotedAssetType { get; set; }

    /// <summary>营销资产 id（官方 <c>marketing_asset_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("marketing_asset_id")]
    public long? MarketingAssetId { get; set; }

    /// <summary>营销资产外部信息（官方 <c>marketing_asset_outer_spec</c>，<c>struct</c>），见 <see cref="AdsMarketingAssetOuterSpec"/>。</summary>
    [JsonPropertyName("marketing_asset_outer_spec")]
    public AdsMarketingAssetOuterSpec? MarketingAssetOuterSpec { get; set; }

    /// <summary>POI 列表（官方 <c>poi_list</c>，<c>string[]</c>；<b>与 <c>marketing_asset_outer_spec</c> 平级</b>，平面读法会误判为其子字段）。</summary>
    [JsonPropertyName("poi_list")]
    public List<string>? PoiList { get; set; }

    /// <summary>营销场景（官方 <c>marketing_scene</c>，<c>enum</c>；只读）。</summary>
    [JsonPropertyName("marketing_scene")]
    public string? MarketingScene { get; set; }

    /// <summary>定向详细设置（官方 <c>targeting</c>，<c>struct</c>），见 <see cref="AdsAdgroupTargeting"/>。</summary>
    [JsonPropertyName("targeting")]
    public AdsAdgroupTargeting? Targeting { get; set; }

    /// <summary>定向条件的可读文案（官方 <c>targeting_translation</c>，<c>string</c>；<b>与 <c>targeting</c> 平级</b>、只读，由官方按定向内容生成）。</summary>
    [JsonPropertyName("targeting_translation")]
    public string? TargetingTranslation { get; set; }

    /// <summary>场景定向（官方 <c>scene_spec</c>，<c>struct</c>），见 <see cref="AdsAdgroupSceneSpec"/>。</summary>
    [JsonPropertyName("scene_spec")]
    public AdsAdgroupSceneSpec? SceneSpec { get; set; }

    /// <summary>用户行为数据源（官方 <c>user_action_sets</c>，<c>struct[]</c>），见 <see cref="AdsUserActionSet"/>。</summary>
    [JsonPropertyName("user_action_sets")]
    public List<AdsUserActionSet>? UserActionSets { get; set; }

    /// <summary>oCPA 深度优化内容（官方 <c>deep_conversion_spec</c>，<c>struct</c>），见 <see cref="AdsDeepConversionSpec"/>。</summary>
    [JsonPropertyName("deep_conversion_spec")]
    public AdsDeepConversionSpec? DeepConversionSpec { get; set; }

    /// <summary>转化目标 id（官方 <c>conversion_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("conversion_id")]
    public long? ConversionId { get; set; }

    /// <summary>转化目标名称（官方 <c>conversion_name</c>，<c>string</c>；只读）。</summary>
    [JsonPropertyName("conversion_name")]
    public string? ConversionName { get; set; }

    /// <summary>深度优化行为出价（官方 <c>deep_conversion_behavior_bid</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("deep_conversion_behavior_bid")]
    public long? DeepConversionBehaviorBid { get; set; }

    /// <summary>深度优化 ROI 期望系数（官方 <c>deep_conversion_worth_rate</c>，<c>float</c>）。</summary>
    [JsonPropertyName("deep_conversion_worth_rate")]
    public double? DeepConversionWorthRate { get; set; }

    /// <summary>深度优化进阶 ROI 期望系数（官方 <c>deep_conversion_worth_advanced_rate</c>，<c>float</c>）。</summary>
    [JsonPropertyName("deep_conversion_worth_advanced_rate")]
    public double? DeepConversionWorthAdvancedRate { get; set; }

    /// <summary>深度优化进阶行为出价（官方 <c>deep_conversion_behavior_advanced_bid</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("deep_conversion_behavior_advanced_bid")]
    public long? DeepConversionBehaviorAdvancedBid { get; set; }

    /// <summary>深度优化类型（官方 <c>deep_optimization_type</c>，<c>enum</c>；只读，请求表无此字段）。</summary>
    [JsonPropertyName("deep_optimization_type")]
    public string? DeepOptimizationType { get; set; }

    /// <summary>出价金额（官方 <c>bid_amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("bid_amount")]
    public long? BidAmount { get; set; }

    /// <summary>出价方式（官方 <c>bid_mode</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("bid_mode")]
    public string? BidMode { get; set; }

    /// <summary>出价场景（官方 <c>bid_scene</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("bid_scene")]
    public string? BidScene { get; set; }

    /// <summary>优化目标（官方 <c>optimization_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("optimization_goal")]
    public string? OptimizationGoal { get; set; }

    /// <summary>出价策略类型（官方 <c>smart_bid_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("smart_bid_type")]
    public string? SmartBidType { get; set; }

    /// <summary>成本上限（官方 <c>smart_cost_cap</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("smart_cost_cap")]
    public long? SmartCostCap { get; set; }

    /// <summary>成本约束场景（官方 <c>cost_constraint_scene</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("cost_constraint_scene")]
    public string? CostConstraintScene { get; set; }

    /// <summary>自定义成本上限（官方 <c>custom_cost_cap</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("custom_cost_cap")]
    public long? CustomCostCap { get; set; }

    /// <summary>自定义 ROI 上限（官方 <c>custom_cost_roi_cap</c>，<c>float</c>）。</summary>
    [JsonPropertyName("custom_cost_roi_cap")]
    public double? CustomCostRoiCap { get; set; }

    /// <summary>成本保障状态（官方 <c>cost_guarantee_status</c>，<c>enum</c>；只读）。</summary>
    [JsonPropertyName("cost_guarantee_status")]
    public string? CostGuaranteeStatus { get; set; }

    /// <summary>成本保障金额（官方 <c>cost_guarantee_money</c>，<c>integer</c>；只读）。</summary>
    [JsonPropertyName("cost_guarantee_money")]
    public long? CostGuaranteeMoney { get; set; }

    /// <summary>o 目标完成类型（官方 <c>og_completion_type</c>，<c>enum</c>；只读）。</summary>
    [JsonPropertyName("og_completion_type")]
    public string? OgCompletionType { get; set; }

    /// <summary>是否开启一键起量（官方 <c>auto_acquisition_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("auto_acquisition_enabled")]
    public bool? AutoAcquisitionEnabled { get; set; }

    /// <summary>一键起量预算（官方 <c>auto_acquisition_budget</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("auto_acquisition_budget")]
    public long? AutoAcquisitionBudget { get; set; }

    /// <summary>一键起量状态（官方 <c>auto_acquisition_status</c>，<c>enum</c>；只读）。</summary>
    [JsonPropertyName("auto_acquisition_status")]
    public string? AutoAcquisitionStatus { get; set; }

    /// <summary>创意增强 MAX 开关（官方 <c>auto_derived_creative_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("auto_derived_creative_enabled")]
    public bool? AutoDerivedCreativeEnabled { get; set; }

    /// <summary>创意增强 MAX 偏好设置（官方 <c>auto_derived_creative_preference</c>，<c>struct</c>），见 <see cref="AdsAutoDerivedCreativePreference"/>。</summary>
    [JsonPropertyName("auto_derived_creative_preference")]
    public AdsAutoDerivedCreativePreference? AutoDerivedCreativePreference { get; set; }

    /// <summary>是否开启自动衍生落地页（官方 <c>auto_derived_landing_page_switch</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("auto_derived_landing_page_switch")]
    public bool? AutoDerivedLandingPageSwitch { get; set; }

    /// <summary>搜索扩量定向开关（官方 <c>search_expand_targeting_switch</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("search_expand_targeting_switch")]
    public string? SearchExpandTargetingSwitch { get; set; }

    /// <summary>搜索扩量开关（官方 <c>search_expansion_switch</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("search_expansion_switch")]
    public string? SearchExpansionSwitch { get; set; }

    /// <summary>智能定向状态（官方 <c>smart_targeting_status</c>，<c>enum</c>；只读）。</summary>
    [JsonPropertyName("smart_targeting_status")]
    public string? SmartTargetingStatus { get; set; }

    /// <summary>是否使用自动流量优选（官方 <c>flow_optimization_enabled</c>，<c>boolean</c>；官方标注该功能已废弃，字段照录）。</summary>
    [JsonPropertyName("flow_optimization_enabled")]
    public bool? FlowOptimizationEnabled { get; set; }

    /// <summary>是否开启智能版位（官方 <c>automatic_site_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("automatic_site_enabled")]
    public bool? AutomaticSiteEnabled { get; set; }

    /// <summary>投放版位集合（官方 <c>site_set</c>，<c>enum[]</c>；本页与 add 页的中文描述不一致，见本类型 remarks）。</summary>
    [JsonPropertyName("site_set")]
    public List<string>? SiteSet { get; set; }

    /// <summary>优先投放版位集合（官方 <c>priority_site_set</c>，<c>enum[]</c>）。</summary>
    [JsonPropertyName("priority_site_set")]
    public List<string>? PrioritySiteSet { get; set; }

    /// <summary>版位探索策略（官方 <c>exploration_strategy</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("exploration_strategy")]
    public string? ExplorationStrategy { get; set; }

    /// <summary>是否开启稳健探索（官方 <c>enable_steady_exploration</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("enable_steady_exploration")]
    public bool? EnableSteadyExploration { get; set; }

    /// <summary>是否允许突破版位（官方 <c>enable_breakthrough_siteset</c>，<c>boolean</c>；只读）。</summary>
    [JsonPropertyName("enable_breakthrough_siteset")]
    public bool? EnableBreakthroughSiteSet { get; set; }

    /// <summary>投放开始日期（官方 <c>begin_date</c>，<c>string</c>，<c>YYYY-MM-DD</c>）。</summary>
    [JsonPropertyName("begin_date")]
    public string? BeginDate { get; set; }

    /// <summary>投放结束日期（官方 <c>end_date</c>，<c>string</c>，<c>YYYY-MM-DD</c>）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>首日投放开始时间（官方 <c>first_day_begin_time</c>，<c>string</c>，<c>HH:mm:ss</c>）。</summary>
    [JsonPropertyName("first_day_begin_time")]
    public string? FirstDayBeginTime { get; set; }

    /// <summary>投放时段（官方 <c>time_series</c>，<c>string</c>，48×7 位 0/1 字符串）。</summary>
    [JsonPropertyName("time_series")]
    public string? TimeSeries { get; set; }

    /// <summary>日预算（官方 <c>daily_budget</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }

    /// <summary>客户设置的状态（官方 <c>configured_status</c>，<c>enum</c>，可由批量接口改写）。</summary>
    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; set; }

    /// <summary>系统状态（官方 <c>system_status</c>，<c>enum</c>；<b>与 <see cref="ConfiguredStatus"/> 是两支不同语义</b>：
    /// 前者由系统判定（审核/余额等），后者由客户设置 ⇒ 判「能否投放」须同时看两支，只看一支会误判）。</summary>
    [JsonPropertyName("system_status")]
    public string? SystemStatus { get; set; }

    /// <summary>素材包 id（官方 <c>material_package_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("material_package_id")]
    public long? MaterialPackageId { get; set; }

    /// <summary>动态商品营销规格（官方 <c>mpa_spec</c>，<c>struct</c>），见 <see cref="AdsMpaSpec"/>。</summary>
    [JsonPropertyName("mpa_spec")]
    public AdsMpaSpec? MpaSpec { get; set; }

    /// <summary>动态创意广告规格（官方 <c>dca_spec</c>，<c>struct</c>），见 <see cref="AdsDcaSpec"/>。</summary>
    [JsonPropertyName("dca_spec")]
    public AdsDcaSpec? DcaSpec { get; set; }

    /// <summary>附加商品规格（官方 <c>additional_product_spec</c>，<c>struct</c>），见 <see cref="AdsAdditionalProductSpec"/>。</summary>
    [JsonPropertyName("additional_product_spec")]
    public AdsAdditionalProductSpec? AdditionalProductSpec { get; set; }

    /// <summary>AOI 优化策略（官方 <c>aoi_optimization_strategy</c>，<c>struct</c>），见 <see cref="AdsAoiOptimizationStrategy"/>。</summary>
    [JsonPropertyName("aoi_optimization_strategy")]
    public AdsAoiOptimizationStrategy? AoiOptimizationStrategy { get; set; }

    /// <summary>行业价值探索（官方 <c>industry_value_explore</c>，<c>struct</c>，内含 <c>prospect_retargeting</c>），见 <see cref="AdsIndustryValueExplore"/>。</summary>
    [JsonPropertyName("industry_value_explore")]
    public AdsIndustryValueExplore? IndustryValueExplore { get; set; }

    /// <summary>营销对象扩展信息（官方 <c>marketing_target_ext</c>，<c>struct</c>；官方四个子字段的描述全部误写为「出价场景」），见 <see cref="AdsMarketingTargetExt"/>。</summary>
    [JsonPropertyName("marketing_target_ext")]
    public AdsMarketingTargetExt? MarketingTargetExt { get; set; }

    /// <summary>营销对象附加信息（官方 <c>marketing_target_attachment</c>，<c>struct</c>），见 <see cref="AdsMarketingTargetAttachment"/>。</summary>
    [JsonPropertyName("marketing_target_attachment")]
    public AdsMarketingTargetAttachment? MarketingTargetAttachment { get; set; }

    /// <summary>否定词个数（官方 <c>negative_word_cnt</c>，<c>struct</c>），见 <see cref="AdsNegativeWordCount"/>。</summary>
    [JsonPropertyName("negative_word_cnt")]
    public AdsNegativeWordCount? NegativeWordCount { get; set; }

    /// <summary>短剧付费类型（官方 <c>short_play_pay_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("short_play_pay_type")]
    public string? ShortPlayPayType { get; set; }

    /// <summary>售卖策略 id（官方 <c>sell_strategy_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("sell_strategy_id")]
    public long? SellStrategyId { get; set; }

    /// <summary>电商 PKAM 开关（官方 <c>ecom_pkam_switch</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("ecom_pkam_switch")]
    public string? EcomPkamSwitch { get; set; }

    /// <summary>前向链路辅助（官方 <c>forward_link_assist</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("forward_link_assist")]
    public string? ForwardLinkAssist { get; set; }

    /// <summary>直播推荐策略开关（官方 <c>live_recommend_strategy_enabled</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("live_recommend_strategy_enabled")]
    public bool? LiveRecommendStrategyEnabled { get; set; }

    /// <summary>ADX 实时类型（官方 <c>adx_realtime_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("adx_realtime_type")]
    public string? AdxRealtimeType { get; set; }

    /// <summary>数据模型版本（官方 <c>data_model_version</c>，<c>integer</c>；只读。官方把它同时列为
    /// <c>dynamic_creatives/get</c> 的 <c>filtering.field</c> 可选值，但本域应答未给出取值含义 ⇒ 不做本地解释）。</summary>
    [JsonPropertyName("data_model_version")]
    public long? DataModelVersion { get; set; }

    /// <summary>创建时间（官方 <c>created_time</c>，<c>integer</c>，Unix 秒）。</summary>
    [JsonPropertyName("created_time")]
    public long? CreatedTime { get; set; }

    /// <summary>最后修改时间（官方 <c>last_modified_time</c>，<c>integer</c>，Unix 秒）。</summary>
    [JsonPropertyName("last_modified_time")]
    public long? LastModifiedTime { get; set; }

    /// <summary>是否已删除（官方 <c>is_deleted</c>，<c>boolean</c>；与请求侧同名参数配对，用于拉取已标记删除的资源）。</summary>
    [JsonPropertyName("is_deleted")]
    public bool? IsDeleted { get; set; }
}

/// <summary>
/// <c>adgroups/get</c> 应答信封（闭合类型，满足 <c>AdsResponse&lt;T&gt;</c> 不得直接作返回类型的约束）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupGetResponse : AdsResponse<AdsAdgroupListData>
{
}
