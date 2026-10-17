// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Adgroups;
using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Estimation;

/// <summary>
/// 预估覆盖人数（<c>POST /v3.0/estimation/get</c>）请求体（2026-10-17 逐页核验，顶层 17 字段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>顶层仅 <c>account_id</c> 标必填</b>，其余全部可空 ⇒ 序列化按 <c>WhenWritingNull</c> 省略，
/// 未填分支由官方按「预估默认形态」处理。子树里的 <c>*</c>（如 <c>location_types</c>、
/// <c>excluded_dimension</c>、<c>marketing_carrier_id</c>）是「父结构在场时条件必填」，
/// 本地拦截会挡掉「整个父结构不传」这一合法形态（口径同 <see cref="AdsAdgroupAddRequest"/>）。
/// </para>
/// <para>
/// <b>定向 / 场景 / 载体 / 营销对象四支子树与 <c>adgroups/add</c> 页逐字段同构</b>
/// （2026-10-17 逐字比对字段名、类型与层级）⇒ <b>复用</b> <see cref="AdsAdgroupTargeting"/> /
/// <see cref="AdsAdgroupSceneSpec"/> / <see cref="AdsMarketingCarrierDetail"/> /
/// <see cref="AdsMarketingTargetDetail"/>，不复制第二套类型（复制后官方改字段名必然只改到其中一份）。
/// </para>
/// <para>
/// <b>本页是「查询型 POST」</b>：动词为 POST、<c>Content-Type: application/json</c>，但语义是只读预估
/// ⇒ 无幂等头需求，页面<b>没有</b>请求头参数表、也没有「特定请求参数」表（守卫 ADS-B5 反证）。
/// </para>
/// <para>
/// <b>ADX 程序化投放禁用面</b>（官方逐字段标注「ADX 程序化投放不可填写提交」）：<c>automatic_site_enabled</c>、
/// <c>targeting</c>、<c>scene_spec</c>；<c>bid_mode</c> 在 ADX 下仅支持
/// <c>{ BID_MODE_CPC, BID_MODE_CPM }</c>、<c>bid_amount</c> 默认填 200、<c>time_series</c> 默认填 336 个 1
/// ⇒ 模式由账号侧决定，SDK 不预判（口径同 <see cref="AdsAdgroupAddRequest"/> remarks）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Estimation")]
public class AdsEstimationGetRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>；原文「有操作权限的帐号 id，不支持代理商 id」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>；按既有单元形态预估时传入）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>营销单元名称（官方 <c>adgroup_name</c>，<c>string</c>；原文「同一帐号下不允许重复」，
    /// 长度 1–60 个等宽字符 / 最大 180 字节，越界由应答 <c>code</c> 表达）。</summary>
    [JsonPropertyName("adgroup_name")]
    public string? AdgroupName { get; set; }

    /// <summary>营销目的类型（官方 <c>marketing_goal</c>，<c>enum</c>；可选值见官方页枚举详情）。</summary>
    [JsonPropertyName("marketing_goal")]
    public string? MarketingGoal { get; set; }

    /// <summary>推广产品类型（官方 <c>marketing_target_type</c>，<c>enum</c>；
    /// <c>MARKETING_TARGET_TYPE_*</c> 大集合，逐值见官方页，勿从其他页猜）。</summary>
    [JsonPropertyName("marketing_target_type")]
    public string? MarketingTargetType { get; set; }

    /// <summary>营销对象详情（官方 <c>marketing_target_detail</c>，<c>struct</c>；其内
    /// <c>marketing_target_detail_id</c> / <c>marketing_target_sub_detail_id</c> 官方标 <c>*</c> =
    /// 本结构在场时必填），复用 <see cref="AdsMarketingTargetDetail"/>。</summary>
    [JsonPropertyName("marketing_target_detail")]
    public AdsMarketingTargetDetail? MarketingTargetDetail { get; set; }

    /// <summary>二级营销目的类型（官方 <c>marketing_sub_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_sub_goal")]
    public string? MarketingSubGoal { get; set; }

    /// <summary>营销载体类型（官方 <c>marketing_carrier_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_carrier_type")]
    public string? MarketingCarrierType { get; set; }

    /// <summary>营销载体详情（官方 <c>marketing_carrier_detail</c>，<c>struct</c>；载体类型为应用/小游戏等时使用），
    /// 复用 <see cref="AdsMarketingCarrierDetail"/>。</summary>
    [JsonPropertyName("marketing_carrier_detail")]
    public AdsMarketingCarrierDetail? MarketingCarrierDetail { get; set; }

    /// <summary>是否开启智能版位功能（官方 <c>automatic_site_enabled</c>，<c>boolean</c>；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("automatic_site_enabled")]
    public bool? AutomaticSiteEnabled { get; set; }

    /// <summary>投放站点集合（官方 <c>site_set</c>，<c>enum[]</c>，数组 1–32。官方限制：当前单站点，
    /// 或 <c>SITE_SET_TENCENT_NEWS</c> + <c>SITE_SET_TENCENT_VIDEO</c> 的组合）。</summary>
    [JsonPropertyName("site_set")]
    public List<string>? SiteSet { get; set; }

    /// <summary>出价方式（官方 <c>bid_mode</c>，<c>enum</c>；ADX 程序化投放仅支持
    /// <c>{ BID_MODE_CPC, BID_MODE_CPM }</c>）。</summary>
    [JsonPropertyName("bid_mode")]
    public string? BidMode { get; set; }

    /// <summary>出价（官方 <c>bid_amount</c>，<c>integer</c>，单位为分；ADX 程序化投放默认填写 200）。</summary>
    [JsonPropertyName("bid_amount")]
    public long? BidAmount { get; set; }

    /// <summary>优化目标类型（官方 <c>optimization_goal</c>，<c>enum</c>；<c>OPTIMIZATIONGOAL_*</c>
    /// 大集合逐值见官方页。定向内 <c>conversion_behavior_list</c> 的枚举值与本字段同集）。</summary>
    [JsonPropertyName("optimization_goal")]
    public string? OptimizationGoal { get; set; }

    /// <summary>投放时间段（官方 <c>time_series</c>，<c>string</c>，336 字节；形态与朋友圈附加规则见
    /// <see cref="AdsAdgroupAddRequest.TimeSeries"/> 同款原文；ADX 程序化投放默认填写 336 个 1）。</summary>
    [JsonPropertyName("time_series")]
    public string? TimeSeries { get; set; }

    /// <summary>定向详细设置（官方 <c>targeting</c>，<c>struct</c>；ADX 程序化投放不可填写提交），
    /// 复用 <see cref="AdsAdgroupTargeting"/>（本页子树与 adgroups 三表逐字段同构）。</summary>
    [JsonPropertyName("targeting")]
    public AdsAdgroupTargeting? Targeting { get; set; }

    /// <summary>场景定向（官方 <c>scene_spec</c>，<c>struct</c>；ADX 程序化投放不可填写提交），
    /// 复用 <see cref="AdsAdgroupSceneSpec"/>。</summary>
    [JsonPropertyName("scene_spec")]
    public AdsAdgroupSceneSpec? SceneSpec { get; set; }
}

/// <summary>
/// 分阶段出价建议（官方 <c>stage_bid_info[]</c> 元素，2026-10-17 逐表核验，5 字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Estimation")]
public class AdsEstimationStageBidInfo
{
    /// <summary>阶段名称（官方 <c>stage_name</c>，<c>string</c>）。</summary>
    [JsonPropertyName("stage_name")]
    public string? StageName { get; set; }

    /// <summary>出价下限（官方 <c>lower_bound</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("lower_bound")]
    public long? LowerBound { get; set; }

    /// <summary>出价上限（官方 <c>upper_bound</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("upper_bound")]
    public long? UpperBound { get; set; }

    /// <summary>默认出价（官方 <c>default_bid</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("default_bid")]
    public long? DefaultBid { get; set; }

    /// <summary>竞胜概率（官方 <c>ratio</c>，<c>float</c>）。</summary>
    [JsonPropertyName("ratio")]
    public double? Ratio { get; set; }
}

/// <summary>
/// <c>estimation/get</c> 的 <c>data</c> 载荷（16 字段，2026-10-17 逐表核验；<b>单结构体、非列表</b>）。
/// </summary>
/// <remarks>
/// <b>金额与曝光/人数均为「预估区间/上限」语义</b>：官方字段名自带 <c>min_/max_/suggest_</c> 前缀，
/// 逐字照录；<c>approximate_count</c> / <c>impression</c> 是「日覆盖 / 日曝光最大值」而非均值。
/// <c>targeting_status</c> 官方标 <c>enum</c> 但本页「枚举详情」未展开取值 ⇒ 建模为 <c>string</c> 留档，
/// 不得从其他页借用枚举集合。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Estimation")]
public class AdsEstimationData
{
    /// <summary>预估最大覆盖用户数（官方 <c>max_active_user_count</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("max_active_user_count")]
    public long? MaxActiveUserCount { get; set; }

    /// <summary>预估日覆盖用户数最大值（官方 <c>approximate_count</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("approximate_count")]
    public long? ApproximateCount { get; set; }

    /// <summary>预估日曝光量最大值（官方 <c>impression</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("impression")]
    public long? Impression { get; set; }

    /// <summary>出价下限（官方 <c>min_bid_amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("min_bid_amount")]
    public long? MinBidAmount { get; set; }

    /// <summary>出价上限（官方 <c>max_bid_amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("max_bid_amount")]
    public long? MaxBidAmount { get; set; }

    /// <summary>建议最小出价（官方 <c>suggest_min_bid_amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("suggest_min_bid_amount")]
    public long? SuggestMinBidAmount { get; set; }

    /// <summary>建议最大出价（官方 <c>suggest_max_bid_amount</c>，<c>integer</c>，单位为分）。</summary>
    [JsonPropertyName("suggest_max_bid_amount")]
    public long? SuggestMaxBidAmount { get; set; }

    /// <summary>oCPC/oCPM 营销建议内容（官方 <c>suggest_bid_content_ocpa</c>，<c>string</c>；
    /// 原文「oCPC/oCPM 营销返回建议内容」，非该出价方式时缺省）。</summary>
    [JsonPropertyName("suggest_bid_content_ocpa")]
    public string? SuggestBidContentOcpa { get; set; }

    /// <summary>预估日覆盖用户数下限（官方 <c>min_users_daily</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("min_users_daily")]
    public long? MinUsersDaily { get; set; }

    /// <summary>预估日覆盖用户数上限（官方 <c>max_users_daily</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("max_users_daily")]
    public long? MaxUsersDaily { get; set; }

    /// <summary>预估日曝光量下限（官方 <c>min_exposure_daily</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("min_exposure_daily")]
    public long? MinExposureDaily { get; set; }

    /// <summary>预估日曝光量上限（官方 <c>max_exposure_daily</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("max_exposure_daily")]
    public long? MaxExposureDaily { get; set; }

    /// <summary>定向状态（官方 <c>targeting_status</c>，<c>enum</c>；本页枚举详情未展开 ⇒ 留档为字符串）。</summary>
    [JsonPropertyName("targeting_status")]
    public string? TargetingStatus { get; set; }

    /// <summary>建议调整的定向（官方 <c>suggest_targeting</c>，<c>string[]</c>）。</summary>
    [JsonPropertyName("suggest_targeting")]
    public List<string>? SuggestTargeting { get; set; }

    /// <summary>当前请求是否满足真实曝光量预估要求（官方 <c>is_real_exposure_supported</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("is_real_exposure_supported")]
    public bool? IsRealExposureSupported { get; set; }

    /// <summary>分阶段出价建议（官方 <c>stage_bid_info</c>，<c>struct[]</c>），见 <see cref="AdsEstimationStageBidInfo"/>。</summary>
    [JsonPropertyName("stage_bid_info")]
    public List<AdsEstimationStageBidInfo>? StageBidInfo { get; set; }
}

/// <summary>
/// <c>estimation/get</c> 应答信封（闭合类型）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Estimation")]
public class AdsEstimationGetResponse : AdsResponse<AdsEstimationData>
{
}
