// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.CustomAudiences;

/// <summary>
/// 人群信息容器（官方 <c>audience_spec</c>，<c>struct</c>；2026-10-17 官方页逐表核验）。
/// </summary>
/// <remarks>
/// <b>与 <c>type</c> 相关</b>：五支子 spec 与 <c>custom_audiences/add</c> 的 <c>type</c>
/// 枚举一一对应且互斥（<c>LOOKALIKE</c> → <see cref="LookalikeSpec"/>、<c>USER_ACTION</c> →
/// <see cref="UserActionSpec"/>、<c>KEYWORD</c> → <see cref="KeywordSpec"/>、<c>AD</c> →
/// <see cref="AdRuleSpec"/>、<c>COMBINE</c> → <see cref="CombineSpec"/>）；
/// <c>CUSTOMER_FILE</c> 无 spec（号码包经 <c>custom_audience_files/add</c> 上传）。
/// 互斥校验交官方应答 <c>code</c> 表达，SDK 不做本地拦截。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsAudienceSpec
{
    /// <summary>Lookalike 人群信息（官方 <c>lookalike_spec</c>，当 <c>type=LOOKALIKE</c> 时必填）。</summary>
    [JsonPropertyName("lookalike_spec")]
    public AdsLookalikeSpec? LookalikeSpec { get; set; }

    /// <summary>UserAction 人群信息（官方 <c>user_action_spec</c>，当 <c>type=USER_ACTION</c> 时必填）。</summary>
    [JsonPropertyName("user_action_spec")]
    public AdsUserActionSpec? UserActionSpec { get; set; }

    /// <summary>Keyword 人群信息（官方 <c>keyword_spec</c>，当 <c>type=KEYWORD</c> 时必填）。</summary>
    [JsonPropertyName("keyword_spec")]
    public AdsKeywordSpec? KeywordSpec { get; set; }

    /// <summary>营销人群信息（官方 <c>ad_rule_spec</c>，当 <c>type=AD</c> 时必填）。</summary>
    [JsonPropertyName("ad_rule_spec")]
    public AdsAdRuleSpec? AdRuleSpec { get; set; }

    /// <summary>组合人群信息（官方 <c>combine_spec</c>，当 <c>type=COMBINE</c> 时必填，最多允许用 500 个人群做组合）。</summary>
    [JsonPropertyName("combine_spec")]
    public AdsCombineSpec? CombineSpec { get; set; }
}

/// <summary>Lookalike 人群信息（官方 <c>lookalike_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsLookalikeSpec
{
    /// <summary>种子人群 id（官方 <c>seed_audience_id</c>，必填）。官方原文：种子人群覆盖人数 100–30000000、
    /// 状态必须是「成功可用」、不能是扩展人群。</summary>
    [JsonPropertyName("seed_audience_id")]
    public long? SeedAudienceId { get; set; }

    /// <summary>lookalike 目标人数（官方 <c>expand_user_count</c>，必填）。官方约束：是 500000 的整数倍，
    /// 最小值 500000、最大值 100000000。</summary>
    [JsonPropertyName("expand_user_count")]
    public long? ExpandUserCount { get; set; }
}

/// <summary>UserAction 人群信息（官方 <c>user_action_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsUserActionSpec
{
    /// <summary>用户行为源 id（官方 <c>user_action_set_id</c>，必填；经 <c>user_action_sets</c> 模块创建分配）。</summary>
    [JsonPropertyName("user_action_set_id")]
    public long? UserActionSetId { get; set; }

    /// <summary>匹配规则类型（官方 <c>match_rule_type</c>，必填，<c>enum</c>；可选值 <c>{ URL, ACTION }</c>；
    /// 官方原文：当 user_action_set 为 Android/iOS APP 类型时只可选 ACTION）。</summary>
    [JsonPropertyName("match_rule_type")]
    public string? MatchRuleType { get; set; }

    /// <summary>行为人群提取类型（官方 <c>extract_type</c>，<c>enum</c>；当 <c>match_rule_type=ACTION</c> 时生效，
    /// 默认 FILTER；可选值 <c>{ FILTER, AGGREGATION }</c>）。</summary>
    [JsonPropertyName("extract_type")]
    public string? ExtractType { get; set; }

    /// <summary>时间窗（官方 <c>time_window</c>，必填，<c>integer</c>；最小 0、最大 180。官方原文：
    /// 圈定最近多少天发生过某行为的人群；当 <c>extract_type=AGGREGATION</c> 时最大取值为 90 天）。</summary>
    [JsonPropertyName("time_window")]
    public long? TimeWindow { get; set; }

    /// <summary>url 匹配规则（官方 <c>url_match_rule</c>，当 <c>match_rule_type=URL</c> 时必填）。</summary>
    [JsonPropertyName("url_match_rule")]
    public AdsUrlMatchRule? UrlMatchRule { get; set; }

    /// <summary>行为和参数匹配规则（官方 <c>action_match_rule</c>，当 <c>match_rule_type=ACTION</c> 且
    /// <c>extract_type</c> 为空或 FILTER 时必填）。</summary>
    [JsonPropertyName("action_match_rule")]
    public AdsActionMatchRule? ActionMatchRule { get; set; }

    /// <summary>行为和参数聚合规则（官方 <c>action_aggregation_rule</c>，当 <c>match_rule_type=ACTION</c> 且
    /// <c>extract_type=AGGREGATION</c> 时必填）。</summary>
    [JsonPropertyName("action_aggregation_rule")]
    public AdsActionAggregationRule? ActionAggregationRule { get; set; }
}

/// <summary>url 匹配规则（官方 <c>url_match_rule</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsUrlMatchRule
{
    /// <summary>匹配规则组（官方 <c>url_matcher_group</c>，必填，<c>struct[]</c>；数组最大长度 16，
    /// 多个组之间是 AND 关系；官方原文：如果为空表示全站浏览人群）。</summary>
    [JsonPropertyName("url_matcher_group")]
    public List<AdsUrlMatcherGroup>? UrlMatcherGroup { get; set; }
}

/// <summary>url 匹配规则组（官方 <c>url_matcher_group</c> 元素，组内规则 OR）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsUrlMatcherGroup
{
    /// <summary>匹配规则（官方 <c>url_matcher</c>，必填，<c>struct[]</c>；数组最大长度 16，多个规则之间是 OR 关系）。</summary>
    [JsonPropertyName("url_matcher")]
    public List<AdsUrlMatcher>? UrlMatcher { get; set; }
}

/// <summary>url 匹配规则（官方 <c>url_matcher</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsUrlMatcher
{
    /// <summary>参数值（官方 <c>param_value</c>，必填；1–128 字节）。</summary>
    [JsonPropertyName("param_value")]
    public string? ParamValue { get; set; }

    /// <summary>运算符（官方 <c>operator</c>，必填，<c>enum</c>；官方原文只允许
    /// <c>{ EQ, NE, CONTAIN, NOT_CONTAIN }</c>；页面枚举全集为 <c>{ LT, GT, EQ, NE, CONTAIN, NOT_CONTAIN }</c>）。</summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }
}

/// <summary>行为和参数匹配规则（官方 <c>action_match_rule</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsActionMatchRule
{
    /// <summary>标准行为类型（官方 <c>action_type</c>，必填，<c>enum</c>；值为 <c>CUSTOM</c> 时配合
    /// <see cref="CustomAction"/>；官方枚举详情约 150 支（<c>{ CUSTOM, REGISTER, VIEW_CONTENT, CONSULT, …,
    /// EFFECTIVE_INSURE }</c>），SDK 以字符串承载、不本地校验）。</summary>
    [JsonPropertyName("action_type")]
    public string? ActionType { get; set; }

    /// <summary>自定义行为类型（官方 <c>custom_action</c>，当 <c>action_type=CUSTOM</c> 时必填；1–128 字节）。</summary>
    [JsonPropertyName("custom_action")]
    public string? CustomAction { get; set; }

    /// <summary>匹配规则组（官方 <c>param_matcher_group</c>，<c>struct[]</c>；数组最大长度 16，多个组之间是 AND 关系）。</summary>
    [JsonPropertyName("param_matcher_group")]
    public List<AdsParamMatcherGroup>? ParamMatcherGroup { get; set; }
}

/// <summary>参数匹配规则组（官方 <c>param_matcher_group</c> 元素，组内规则 OR）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsParamMatcherGroup
{
    /// <summary>匹配规则（官方 <c>param_matcher</c>，必填，<c>struct[]</c>；数组最大长度 16，多个规则之间是 OR 关系）。</summary>
    [JsonPropertyName("param_matcher")]
    public List<AdsParamMatcher>? ParamMatcher { get; set; }
}

/// <summary>参数匹配规则（官方 <c>param_matcher</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsParamMatcher
{
    /// <summary>参数名称（官方 <c>param_name</c>，必填；1–128 字节）。</summary>
    [JsonPropertyName("param_name")]
    public string? ParamName { get; set; }

    /// <summary>参数值（官方 <c>param_value</c>，必填；1–128 字节）。</summary>
    [JsonPropertyName("param_value")]
    public string? ParamValue { get; set; }

    /// <summary>运算符（官方 <c>operator</c>，必填，<c>enum</c>；可选值 <c>{ LT, GT, EQ, NE, CONTAIN, NOT_CONTAIN }</c>）。</summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }
}

/// <summary>行为和参数聚合规则（官方 <c>action_aggregation_rule</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsActionAggregationRule
{
    /// <summary>标准行为类型（官方 <c>action_type</c>，必填，<c>enum</c>；取值面同
    /// <see cref="AdsActionMatchRule.ActionType"/>）。</summary>
    [JsonPropertyName("action_type")]
    public string? ActionType { get; set; }

    /// <summary>自定义行为类型（官方 <c>custom_action</c>，当 <c>action_type=CUSTOM</c> 时必填；1–128 字节）。</summary>
    [JsonPropertyName("custom_action")]
    public string? CustomAction { get; set; }

    /// <summary>聚合规则数组（官方 <c>aggregation_group</c>，必填，<c>struct[]</c>；最小 1、最大 4，多个组之间是 AND 关系）。</summary>
    [JsonPropertyName("aggregation_group")]
    public List<AdsAggregationGroup>? AggregationGroup { get; set; }

    /// <summary>匹配规则组（官方 <c>filter_group</c>，<c>struct[]</c>；最小 1、最大 16，多个组之间是 AND 关系）。</summary>
    [JsonPropertyName("filter_group")]
    public List<AdsFilterGroup>? FilterGroup { get; set; }
}

/// <summary>聚合规则组（官方 <c>aggregation_group</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsAggregationGroup
{
    /// <summary>匹配规则组（官方 <c>aggregation_matcher</c>，必填，<c>struct[]</c>；数组长度为 1，组间 OR 关系）。</summary>
    [JsonPropertyName("aggregation_matcher")]
    public List<AdsAggregationMatcher>? AggregationMatcher { get; set; }
}

/// <summary>聚合匹配规则（官方 <c>aggregation_matcher</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsAggregationMatcher
{
    /// <summary>聚合类型（官方 <c>aggregation_type</c>，必填，<c>enum</c>；可选值 <c>{ SUM, MAX, MIN, COUNT }</c>）。</summary>
    [JsonPropertyName("aggregation_type")]
    public string? AggregationType { get; set; }

    /// <summary>频次类型（官方 <c>count_type</c>，当 <c>aggregation_type=COUNT</c> 时必填；
    /// 可选值 <c>{ BY_TIMES, BY_DAY }</c>）。</summary>
    [JsonPropertyName("count_type")]
    public string? CountType { get; set; }

    /// <summary>参数名称（官方 <c>param_name</c>，当 <c>aggregation_type != COUNT</c> 时必填；1–128 字节）。</summary>
    [JsonPropertyName("param_name")]
    public string? ParamName { get; set; }

    /// <summary>比较符（官方 <c>comparator</c>，必填，<c>enum</c>；可选值
    /// <c>{ COMPARATOR_GE, COMPARATOR_LE, COMPARATOR_BETWEEN, COMPARATOR_EQ }</c>）。</summary>
    [JsonPropertyName("comparator")]
    public string? Comparator { get; set; }

    /// <summary>参数值（官方 <c>comparison_value</c>，<c>integer</c>；当 <c>comparator != COMPARATOR_BETWEEN</c> 时必填）。</summary>
    [JsonPropertyName("comparison_value")]
    public long? ComparisonValue { get; set; }

    /// <summary>参数值下界（官方 <c>comparison_min_value</c>，<c>integer</c>；当 <c>comparator=COMPARATOR_BETWEEN</c> 时必填）。</summary>
    [JsonPropertyName("comparison_min_value")]
    public long? ComparisonMinValue { get; set; }

    /// <summary>参数值上界（官方 <c>comparison_max_value</c>，<c>integer</c>；当 <c>comparator=COMPARATOR_BETWEEN</c> 时必填）。</summary>
    [JsonPropertyName("comparison_max_value")]
    public long? ComparisonMaxValue { get; set; }
}

/// <summary>过滤规则组（官方 <c>filter_group</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsFilterGroup
{
    /// <summary>匹配规则（官方 <c>param_matcher</c>，必填，<c>struct[]</c>；数组长度为 1，组间 OR 关系）。</summary>
    [JsonPropertyName("param_matcher")]
    public List<AdsParamMatcher>? ParamMatcher { get; set; }
}

/// <summary>Keyword 人群信息（官方 <c>keyword_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsKeywordSpec
{
    /// <summary>包含关键词（官方 <c>include_keyword</c>，必填，<c>string[]</c>；数组最小 1、最大 100，
    /// 每项 1–10 字节）。</summary>
    [JsonPropertyName("include_keyword")]
    public List<string>? IncludeKeyword { get; set; }

    /// <summary>排除关键词（官方 <c>exclude_keyword</c>，<c>string[]</c>；数组最大 20，每项 1–10 字节）。</summary>
    [JsonPropertyName("exclude_keyword")]
    public List<string>? ExcludeKeyword { get; set; }
}

/// <summary>营销人群信息（官方 <c>ad_rule_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsAdRuleSpec
{
    /// <summary>营销行为类型（官方 <c>rule_type</c>，必填，<c>enum</c>；可选值 <c>{ EXPOSURE, CLICK, CONVERSION }</c>；
    /// 官方原文：EXPOSURE 类型需要线下联系运营开通白名单方可使用）。</summary>
    [JsonPropertyName("rule_type")]
    public string? RuleType { get; set; }

    /// <summary>转化类型（官方 <c>conversion_type</c>，<c>enum[]</c>；数组最大 5；官方原文：
    /// 当 <c>rule_type=CONVERSION</c> 时必填、否则不能填写；可选值
    /// <c>{ APP_START_DOWNLOAD, APP_FINISH_DOWNLOAD, APP_INSTALL, APP_ACTIVATE, ORDER, RESERVATION,
    /// REGISTRATION_APP, REGISTRATION_WEB, PURCHASE_APP, PURCHASE_WEB, APPLY_APP, APPLY_WEB,
    /// ADD_TO_CART_WEB, ADD_TO_CART_APP, CONSULTATION_WEB, CONSULTATION_PHONE, CALL_BACK,
    /// ADD_TO_WISHLIST, SHARE }</c>）。</summary>
    [JsonPropertyName("conversion_type")]
    public List<string>? ConversionType { get; set; }

    /// <summary>数据起始日期（官方 <c>start_date</c>，必填；格式 <c>yyyy-MM-dd</c>；官方约束：必须在 90 天以内、
    /// 且在今天（不包含）以前）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>数据结束日期（官方 <c>end_date</c>；格式 <c>yyyy-MM-dd</c>；官方原文：必须在
    /// <c>start_date</c>（包含）之后、且在今天（不包含）以前；未填写则人群数据随源投放数据更新）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>营销单元 id 列表（官方 <c>adgroup_id_list</c>，<c>integer[]</c>；数组最大 75、元素 1–2147483647；
    /// 经 <c>adgroups/get</c> 获得）。</summary>
    [JsonPropertyName("adgroup_id_list")]
    public List<long>? AdgroupIdList { get; set; }
}

/// <summary>组合人群信息（官方 <c>combine_spec</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCombineSpec
{
    /// <summary>包含的人群（官方 <c>include</c>，必填，<c>struct[][]</c>；外层数组最小 1、最大 500；
    /// 官方原文「这是个二维数组，第一层数组元素之间为 AND 关系，第二层数组元素之间为 OR 关系」）。</summary>
    [JsonPropertyName("include")]
    public List<List<AdsCombineAudience>>? Include { get; set; }

    /// <summary>排除的人群（官方 <c>exclude</c>，<c>struct[][]</c>；外层数组最大 500；二维语义同
    /// <see cref="Include"/>）。</summary>
    [JsonPropertyName("exclude")]
    public List<List<AdsCombineAudience>>? Exclude { get; set; }
}

/// <summary>组合人群元素（官方 <c>include</c> / <c>exclude</c> 二维数组的内层元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "CustomAudiences")]
public class AdsCombineAudience
{
    /// <summary>人群 id 或标签 id（官方 <c>audience_id</c>，必填；官方原文：不支持 LOOKALIKE 和 COMBINE 人群的 id）。</summary>
    [JsonPropertyName("audience_id")]
    public long? AudienceId { get; set; }

    /// <summary>时间窗（官方 <c>time_window</c>，<c>integer</c>；官方原文：仅对客户标签有效，
    /// 目前仅支持 <c>{ 1, 3, 7, 10, 15, 30, 90, 180, 365 }</c>）。</summary>
    [JsonPropertyName("time_window")]
    public long? TimeWindow { get; set; }
}
