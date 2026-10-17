// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.TargetingTags;

/// <summary>
/// 定向标签查询条件包装（官方 <c>tag_spec</c>，<c>struct</c>，2026-10-17 逐页核验）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两支兄弟结构逐字段同构</b>（官方表把 <c>behavior_spec</c> 与 <c>interest_spec</c> 的子字段
/// 逐字重复登记了两遍）⇒ 共用 <see cref="AdsTagQuerySpec"/> 一支类型，不复制第二套。
/// </para>
/// <para><b>条件必填</b>：<c>behavior_spec</c> 在 <c>type=BEHAVIOR</c> 时必填、<c>interest_spec</c> 在
/// <c>type=INTEREST</c> 时必填；<c>type</c> 的取值由调用方已在 Query 侧给出，本 SDK 不本地判定。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TargetingTags")]
public class AdsTagSpec
{
    /// <summary>行为定向标签条件（官方 <c>behavior_spec</c>，<c>struct</c>；<c>type=BEHAVIOR</c> 时必填）。</summary>
    [JsonPropertyName("behavior_spec")]
    public AdsTagQuerySpec? BehaviorSpec { get; set; }

    /// <summary>兴趣定向标签条件（官方 <c>interest_spec</c>，<c>struct</c>；<c>type=INTEREST</c> 时必填）。</summary>
    [JsonPropertyName("interest_spec")]
    public AdsTagQuerySpec? InterestSpec { get; set; }
}

/// <summary>
/// 行为 / 兴趣共用的标签查询条件（官方 <c>behavior_spec</c> / <c>interest_spec</c> 的展开形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "TargetingTags")]
public class AdsTagQuerySpec
{
    /// <summary>定向标签查询方式（官方 <c>query_mode</c>，<c>enum</c>，<b>必填</b>）。可选值（2026-10-17 逐字核验）：
    /// <c>{ TARGETING_TAG_QUERY_MODE_SEARCH, TARGETING_TAG_QUERY_MODE_RECOMMEND,
    /// TARGETING_TAG_QUERY_MODE_COMMON, TARGETING_TAG_QUERY_MODE_ADVANCED_RECOMMEND,
    /// TARGETING_TAG_QUERY_MODE_VERIFY_QUERY_LIST }</c>。</summary>
    [JsonPropertyName("query_mode")]
    public string? QueryMode { get; set; }

    /// <summary>查询条件（官方 <c>query_spec</c>，<c>struct</c>；原文「当 query_mode 为
    /// VERIFY_QUERY_LIST、SEARCH 和 RECOMMEND 时必填」⇒ 条件必填不本地拦截）。</summary>
    [JsonPropertyName("query_spec")]
    public AdsTagQueryCondition? QuerySpec { get; set; }

    /// <summary>优质推荐类型（官方 <c>advanced_recommend_type</c>，<c>enum</c>；原文「当 query_mode 为
    /// TARGETING_TAG_QUERY_MODE_ADVANCED_RECOMMEND 时必填」。可选值：
    /// <c>{ ADVANCED_RECOMMEND_TYPE_SYSTEMATIC, ADVANCED_RECOMMEND_TYPE_INDUSTRY_HOT }</c>）。</summary>
    [JsonPropertyName("advanced_recommend_type")]
    public string? AdvancedRecommendType { get; set; }
}

/// <summary>
/// 关键词查询条件（官方 <c>query_spec</c>，<c>struct</c>，4 字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "TargetingTags")]
public class AdsTagQueryCondition
{
    /// <summary>搜索 / 推荐词（官方 <c>query</c>，<c>string</c>，1–200 字节）。</summary>
    [JsonPropertyName("query")]
    public string? Query { get; set; }

    /// <summary>批量检查关键词（官方 <c>query_list</c>，<c>string[]</c>，数组 1–250、每项 1–64 字节；
    /// 原文「当且仅当 query_mode 为 TARGETING_TAG_QUERY_MODE_VERIFY_QUERY_LIST 时可填且必填」，
    /// <b>应答仅返回有效关键词</b>）。</summary>
    [JsonPropertyName("query_list")]
    public List<string>? QueryList { get; set; }

    /// <summary>最大结果数（官方 <c>max_result_number</c>，<c>integer</c>，最小 1、最大 50）。</summary>
    [JsonPropertyName("max_result_number")]
    public long? MaxResultNumber { get; set; }

    /// <summary>已选中标签（官方 <c>excluding_targeting_tags</c>，<c>string[]</c>，数组 1–250；
    /// 原文「已经选中的标签，在结果中不会再次出现」）。</summary>
    [JsonPropertyName("excluding_targeting_tags")]
    public List<string>? ExcludingTargetingTags { get; set; }
}

/// <summary>
/// 定向标签条目（官方 <c>targeting_tags/get</c> 的 <c>data.list[]</c> 元素，2026-10-17 逐表核验，7 字段）。
/// </summary>
/// <remarks>
/// <c>city_level</c> / <c>tag_class</c> 官方标 <c>enum</c> 但本页「枚举详情」未展开取值 ⇒ 建模为
/// <c>string</c> 留档（应答示例中出现 <c>CITY_LEVEL_NONE</c>），不得从其他页借用枚举集合。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "TargetingTags")]
public class AdsTargetingTagInfo
{
    /// <summary>通用 id（官方 <c>id</c>，<c>integer</c>；即地域省市 / 商圈 / 标签的取值 id，
    /// 回填 <c>adgroups</c> 定向的 <c>regions</c> / <c>business_districts</c> 等列表）。</summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>城市级别（官方 <c>city_level</c>，<c>enum</c>；本页枚举详情未展开）。</summary>
    [JsonPropertyName("city_level")]
    public string? CityLevel { get; set; }

    /// <summary>父节点标签名（官方 <c>parent_name</c>；原文「如果没有父节点返回空字符串」——
    /// 是空串不是缺省，判根节点用空串比较）。</summary>
    [JsonPropertyName("parent_name")]
    public string? ParentName { get; set; }

    /// <summary>父节点 id（官方 <c>parent_id</c>，<c>integer</c>；根节点示例返回 <c>0</c>）。</summary>
    [JsonPropertyName("parent_id")]
    public long? ParentId { get; set; }

    /// <summary>标签名称（官方 <c>name</c>，<c>string</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>行为兴趣标签返回类型（官方 <c>tag_class</c>，<c>enum</c>；本页枚举详情未展开）。</summary>
    [JsonPropertyName("tag_class")]
    public string? TagClass { get; set; }

    /// <summary>行业是否可用于个人客户注册（官方 <c>available_for_individual</c>，<c>boolean</c>）。</summary>
    [JsonPropertyName("available_for_individual")]
    public bool? AvailableForIndividual { get; set; }
}

/// <summary><c>targeting_tags/get</c> 的 <c>data</c> 载荷（<b>仅 list、无分页元信息</b>）。</summary>
[HttpJsonSerializable(SerializerClassName = "TargetingTags")]
public class AdsTargetingTagListData
{
    /// <summary>标签列表（官方 <c>list</c>，<c>struct[]</c>）。</summary>
    [JsonPropertyName("list")]
    public List<AdsTargetingTagInfo>? List { get; set; }
}

/// <summary><c>GET /v3.0/targeting_tags/get</c> 的闭合应答。</summary>
[HttpJsonSerializable(SerializerClassName = "TargetingTags")]
public class AdsTargetingTagsGetResponse : AdsResponse<AdsTargetingTagListData>
{
}
