// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相关法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// 营销载体详情（官方 <c>marketing_carrier_detail</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// 官方原文「当营销载体类型是 <c>MARKETING_CARRIER_TYPE_APP_ANDROID</c>、
/// <c>MARKETING_CARRIER_TYPE_APP_IOS</c> 和 <c>MARKETING_CARRIER_TYPE_WECHAT_MINI_GAME</c> 等时使用」，
/// 且该结构内 <c>marketing_carrier_id</c> 标 <c>*</c>（<b>条件必填</b>：载体类型决定本结构是否需要）⇒
/// SDK 不本地判定载体类型。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsMarketingCarrierDetail
{
    /// <summary>营销载体 id（官方 <c>marketing_carrier_id</c>，<c>string</c>，请求侧<b>条件必填</b>；如安卓应用 id、IOS 应用 id、小游戏 id）。</summary>
    [JsonPropertyName("marketing_carrier_id")]
    public string? MarketingCarrierId { get; set; }

    /// <summary>二级营销载体 id（官方 <c>marketing_sub_carrier_id</c>，<c>string</c>）。</summary>
    [JsonPropertyName("marketing_sub_carrier_id")]
    public string? MarketingSubCarrierId { get; set; }

    /// <summary>营销载体名称（官方 <c>marketing_carrier_name</c>，<c>string</c>）。</summary>
    [JsonPropertyName("marketing_carrier_name")]
    public string? MarketingCarrierName { get; set; }
}

/// <summary>营销对象详情（官方 <c>marketing_target_detail</c>，<c>struct</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsMarketingTargetDetail
{
    /// <summary>推广内容资产详情 id（官方 <c>marketing_target_detail_id</c>，<c>string</c>）。</summary>
    [JsonPropertyName("marketing_target_detail_id")]
    public string? MarketingTargetDetailId { get; set; }

    /// <summary>二级推广内容资产详情 id（官方 <c>marketing_target_sub_detail_id</c>，<c>string</c>）。</summary>
    [JsonPropertyName("marketing_target_sub_detail_id")]
    public string? MarketingTargetSubDetailId { get; set; }
}

/// <summary>
/// 产品外部 id 数据（官方 <c>marketing_asset_outer_spec</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <b>官方互斥前提（原文）</b>：「当推广产品类型是以下类型的时候，<b>必须</b>使用该字段，
/// <b>不允许</b>使用 <c>marketing_asset_id</c>」（取值集为
/// <c>MARKETING_TARGET_TYPE_APP_ANDROID</c> / <c>MARKETING_TARGET_TYPE_APP_IOS</c> 等一长串，见官方页原文），
/// 且 <c>adgroups/get</c> 应答对 <c>marketing_asset_id</c> 与 <c>marketing_asset_outer_spec</c>
/// 另写「不能同时为空，也不能同时使用」⇒ 这是<b>由推广产品类型决定</b>的二选一，
/// SDK 不预判类型、两支都建模为可空，组合错误由应答 <c>code</c> 表达。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsMarketingAssetOuterSpec
{
    /// <summary>推广产品类型（官方 <c>marketing_target_type</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("marketing_target_type")]
    public string? MarketingTargetType { get; set; }

    /// <summary>推广产品外部 id（官方 <c>marketing_asset_outer_id</c>，<c>string</c>；如安卓应用 id、IOS 应用 id、小游戏 id、商品库 id）。</summary>
    [JsonPropertyName("marketing_asset_outer_id")]
    public string? MarketingAssetOuterId { get; set; }

    /// <summary>推广产品外部子 id（官方 <c>marketing_asset_outer_sub_id</c>，<c>string</c>；如安卓应用渠道包 id、短剧 sku_id）。</summary>
    [JsonPropertyName("marketing_asset_outer_sub_id")]
    public string? MarketingAssetOuterSubId { get; set; }

    /// <summary>推广产品名称（官方 <c>marketing_asset_outer_name</c>，<c>string</c>）。</summary>
    [JsonPropertyName("marketing_asset_outer_name")]
    public string? MarketingAssetOuterName { get; set; }
}

/// <summary>
/// 动态商品营销规格（官方 <c>mpa_spec</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <b>与 <see cref="AdsDcaSpec"/> 是两个不同结构</b>：官方两页分别给出 <c>mpa_spec</c>
/// （<c>recommend_method_ids</c> + <c>product_catalog_id</c> + <c>product_series_id</c>）与
/// <c>dca_spec</c>（<c>recommend_method_ids</c> + <c>set_id</c>），
/// 同名子字段 <c>recommend_method_ids</c> 的类型一致（<c>integer[]</c>）但其余字段完全不同 ⇒ 各建一类，
/// 合并会把对方没有的字段变成永远为空的噪音。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsMpaSpec
{
    /// <summary>推荐方法 id 列表（官方 <c>recommend_method_ids</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("recommend_method_ids")]
    public List<long>? RecommendMethodIds { get; set; }

    /// <summary>商品库 id（官方 <c>product_catalog_id</c>，<c>string</c>；注意本结构内官方标 <c>string</c>，
    /// 而 <see cref="AdsAdditionalProductSpec.ProductCatalogId"/> 同样标 <c>string</c>，
    /// <see cref="AdsAutoDerivedCreativeProduct.CatalogId"/> 却标 <c>integer</c>）。</summary>
    [JsonPropertyName("product_catalog_id")]
    public string? ProductCatalogId { get; set; }

    /// <summary>商品系列 id（官方 <c>product_series_id</c>，<c>string</c>）。</summary>
    [JsonPropertyName("product_series_id")]
    public string? ProductSeriesId { get; set; }
}

/// <summary>动态创意广告规格（官方 <c>dca_spec</c>，<c>struct</c>，字段集见 <see cref="AdsMpaSpec"/> 的对比说明）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsDcaSpec
{
    /// <summary>推荐方法 id 列表（官方 <c>recommend_method_ids</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("recommend_method_ids")]
    public List<long>? RecommendMethodIds { get; set; }

    /// <summary>套装 id（官方 <c>set_id</c>，<c>string</c>）。</summary>
    [JsonPropertyName("set_id")]
    public string? SetId { get; set; }
}

/// <summary>
/// AOI 优化策略（官方 <c>aoi_optimization_strategy</c>，<c>struct</c>）。
/// </summary>
/// <remarks>官方把结构内的 <c>aoi_optimization_strategy_enabled</c> 标 <c>*</c>（<b>条件必填</b>：
/// 传了本结构就必须给出开关）⇒ 本结构的语义是「开关 + 命中 AOI 列表」，不传结构即表示不使用该策略。</remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAoiOptimizationStrategy
{
    /// <summary>是否开启 AOI 优化策略（官方 <c>aoi_optimization_strategy_enabled</c>，<c>boolean</c>，请求侧<b>条件必填</b>）。</summary>
    [JsonPropertyName("aoi_optimization_strategy_enabled")]
    public bool? Enabled { get; set; }

    /// <summary>AOI id 列表（官方 <c>aoi_id_list</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("aoi_id_list")]
    public List<long>? AoiIdList { get; set; }
}

/// <summary>
/// 云统包规格（官方 <c>cloud_union_spec</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <b><c>expected_roi</c> 在本结构内是<b>必填</b></b>（官方标 <c>*</c>、类型 <c>float</c>）——
/// 与 <c>deep_conversion_worth_spec.expected_roi</c> 的选填形态同名不同约束，是两个结构而非一份定义的复用，
/// 故各自建模（见 <see cref="AdsDeepConversionWorthSpec"/>）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsCloudUnionSpec
{
    /// <summary>ROI 目标（官方 <c>roi_goal</c>，<c>enum</c>）。</summary>
    [JsonPropertyName("roi_goal")]
    public string? RoiGoal { get; set; }

    /// <summary>期望 ROI（官方 <c>expected_roi</c>，<c>float</c>，请求侧<b>必填</b>）。</summary>
    [JsonPropertyName("expected_roi")]
    public double? ExpectedRoi { get; set; }
}

/// <summary>附加商品规格（官方 <c>additional_product_spec</c>，<c>struct</c>；两支字段官方均标 <c>*</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdditionalProductSpec
{
    /// <summary>商品库 id（官方 <c>product_catalog_id</c>，<c>string</c>，请求侧<b>必填</b>）。</summary>
    [JsonPropertyName("product_catalog_id")]
    public string? ProductCatalogId { get; set; }

    /// <summary>商品 id（官方 <c>product_outer_id</c>，<c>string</c>，请求侧<b>必填</b>）。</summary>
    [JsonPropertyName("product_outer_id")]
    public string? ProductOuterId { get; set; }
}

/// <summary>
/// 行业价值探索（官方 <c>industry_value_explore</c>，<c>struct</c>）。
/// </summary>
/// <remarks>
/// <b>层级照录</b>：官方把 <c>prospect_retargeting</c> 建在 <c>industry_value_explore</c>
/// <b>之内</b>（三张表一致：get 应答、add 请求、update 请求），而非与它平级 ——
/// 直觉上「潜力再营销」像独立策略，提到平级会造出官方解析不到的报文。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsIndustryValueExplore
{
    /// <summary>高意向线索（官方 <c>high_intention_clue</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("high_intention_clue")]
    public bool? HighIntentionClue { get; set; }

    /// <summary>兴趣群体探索（官方 <c>interest_group_exploration</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("interest_group_exploration")]
    public bool? InterestGroupExploration { get; set; }

    /// <summary>IAA 智能托管（官方 <c>iaa_smart_hosting</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("iaa_smart_hosting")]
    public bool? IaaSmartHosting { get; set; }

    /// <summary>高用量探索（官方 <c>high_volume_exploration</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("high_volume_exploration")]
    public bool? HighVolumeExploration { get; set; }

    /// <summary>潜力再营销（官方 <c>prospect_retargeting</c>，<c>struct</c>，本结构的子字段而非平级字段）。</summary>
    [JsonPropertyName("prospect_retargeting")]
    public AdsProspectRetargeting? ProspectRetargeting { get; set; }
}

/// <summary>潜力再营销（官方 <c>prospect_retargeting</c>，<c>struct</c>；<c>enabled</c> 官方标 <c>*</c>，为<b>条件必填</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsProspectRetargeting
{
    /// <summary>是否开启（官方 <c>enabled</c>，<c>boolean</c>，请求侧<b>条件必填</b>）。</summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>出价系数（官方 <c>bid_coefficient</c>，<c>float</c>）。</summary>
    [JsonPropertyName("bid_coefficient")]
    public double? BidCoefficient { get; set; }
}

/// <summary>
/// 营销对象扩展数据（官方 <c>marketing_target_ext</c>，<c>struct</c>，仅 <c>adgroups/get</c> 应答出现）。
/// </summary>
/// <remarks>
/// <b>官方描述缺陷照录</b>：本结构的四个子字段在官方 <c>adgroups/get</c> 应答表里的描述
/// <b>全部写成「出价场景」</b>（与 <c>bid_scene</c> 的描述同源，疑为文档复制粘贴错误），
/// 官方未给出各字段的真实语义。SDK 只照字段名与类型建模，<b>不</b>替官方补写语义
/// （写了就是未经核验的断言，见方案「UNVERIFIED 不得成为契约」）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsMarketingTargetExt
{
    /// <summary>营销对象名称（官方 <c>marketing_target_name</c>，<c>string</c>；官方描述原文为「出价场景」，见本类型 remarks）。</summary>
    [JsonPropertyName("marketing_target_name")]
    public string? MarketingTargetName { get; set; }

    /// <summary>一级类目名（官方 <c>category_name1</c>，<c>string</c>；官方描述原文为「出价场景」）。</summary>
    [JsonPropertyName("category_name1")]
    public string? CategoryName1 { get; set; }

    /// <summary>二级类目名（官方 <c>category_name2</c>，<c>string</c>；官方描述原文为「出价场景」）。</summary>
    [JsonPropertyName("category_name2")]
    public string? CategoryName2 { get; set; }

    /// <summary>三级类目名（官方 <c>category_name3</c>，<c>string</c>；官方描述原文为「出价场景」）。</summary>
    [JsonPropertyName("category_name3")]
    public string? CategoryName3 { get; set; }
}

/// <summary>营销对象附加信息（官方 <c>marketing_target_attachment</c>，<c>struct</c>，仅 <c>adgroups/get</c> 应答出现）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsMarketingTargetAttachment
{
    /// <summary>安卓应用渠道包 id（官方 <c>android_channel_id</c>，<c>string</c>）。官方原文：当前特指投放腾讯开放平台
    /// 安卓应用的渠道包 id，从推广目标模块的读取接口可获取，渠道包 id 由 <c>"xx;yy"</c> 两部分组成，
    /// 读取时 <c>xx</c> 会被置为 <c>0</c>，<c>0</c> 为正常。</summary>
    [JsonPropertyName("android_channel_id")]
    public string? AndroidChannelId { get; set; }
}

/// <summary>
/// 否定词个数（官方 <c>negative_word_cnt</c>，<c>struct</c>，仅 <c>adgroups/get</c> 应答出现）。
/// </summary>
/// <remarks>
/// <b>官方类型与描述的矛盾照录</b>：两支字段官方类型标 <c>number</c>，描述却都写「整数」⇒
/// 本类型按<b>描述</b>取 <see cref="long"/>（计数语义），若线上出现小数形态属官方报文缺陷，
/// 处置是改型而非加本地兼容层。<c>exact</c> / <c>phrase</c> 两支的可用上限属搜索投放的配额，
/// 官方未在本报答表给出数字 ⇒ 不写任何上限断言。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsNegativeWordCount
{
    /// <summary>精确否定词个数（官方 <c>exact_negative_word_cnt</c>，类型标 <c>number</c>、描述标「整数」）。</summary>
    [JsonPropertyName("exact_negative_word_cnt")]
    public long? ExactNegativeWordCount { get; set; }

    /// <summary>短语否定词个数（官方 <c>phrase_negative_word_cnt</c>，类型标 <c>number</c>、描述标「整数」）。</summary>
    [JsonPropertyName("phrase_negative_word_cnt")]
    public long? PhraseNegativeWordCount { get; set; }
}
