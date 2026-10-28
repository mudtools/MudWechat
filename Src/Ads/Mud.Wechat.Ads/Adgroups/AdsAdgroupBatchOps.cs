// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Adgroups;

/// <summary>
/// 删除营销单元（<c>POST /v3.0/adgroups/delete</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方本页请求参数<b>只有两个</b>且均必填（2026-10-10 逐页核验）：<c>account_id</c>（integer，
/// 「有操作权限的帐号 id，不支持代理商 id」）与 <c>adgroup_id</c>（int64）。
/// <b>不存在</b>批量删除形态 —— <c>adgroup_ids</c> 这个参数名在 adgroups 全部七页里一次都没出现过，
/// 批量只存在于 <c>update_*_spec</c> 数组内（每条自带 <c>adgroup_id</c>）。
/// </para>
/// <para>
/// <b>官方不写「软删还是硬删」</b>：本页无使用说明章节，没有一句描述删除后资源能否找回；
/// 但 <c>adgroups/get</c> 提供 <c>is_deleted</c> 请求参数与同名应答字段 ⇒ 从可读回「已删除」这一事实推断
/// 官方做的是<b>标记删除</b>。该推断<b>不写进代码语义</b>（不做「删除后仍可查」的本地保证），
/// 只作为调用方排查时的提示。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupDeleteRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>；原文「不支持代理商 id」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }
}

/// <summary><c>adgroups/delete</c> 应答信封（官方 <c>data</c> 只回被删除的 <c>adgroup_id</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupDeleteResponse : AdsResponse<AdsAdgroupIdData>
{
}

/// <summary>
/// 营销单元批量操作族的<b>共用</b> <c>data</c> 载荷与逐条结果。
/// </summary>
/// <remarks>
/// <para>
/// <b>四支端点共用一套应答形状（官方事实，非推测）</b>：2026-10-10 逐页核验
/// <c>update_daily_budget</c> / <c>update_configured_status</c> / <c>update_bid_amount</c> /
/// <c>update_datetime</c> 四页，应答字段表<b>完全一致</b>：
/// <c>data.list[]</c> 元素为 <c>{code, message, message_cn, adgroup_id}</c>，
/// 另有 <c>data.fail_id_list</c>（<c>integer[]</c>，「失败的 id 集合」）⇒
/// 只建一支 <see cref="AdsAdgroupBatchData"/>，四支端点各建闭合信封复用同一载荷。
/// </para>
/// <para>
/// <b>四支端点共用的使用说明（原文两条）</b>：①「返回结果的顺序和 <c>update_xxx_spec</c>
/// 中的参数顺序是一致的」⇒ 可按顺序与请求项配对；②「<c>update_xxx_spec</c> 中
/// <c>adgroup_id</c> 不允许重复」⇒ 重复 id 是官方拒绝项，SDK 不本地去重（去重会静默改变调用方意图）。
/// </para>
/// <para>
/// <b>双层失败语义（判错必须两层都做）</b>：外层信封 <c>code == 0</c> 只表示「请求被受理」，
/// 批量项<b>部分失败</b>时外层仍是 <c>0</c> ⇒ 逐条成败只看 <see cref="AdsBatchResultItem.Code"/>，
/// <c>fail_id_list</c> 是汇总面（与逐条 <c>code</c> 不是同一个判定面，两者不一致时以逐条为准）。
/// </para>
/// <para>
/// <b>数组上限的官方不一致</b>：<c>update_configured_status_spec</c> / <c>update_bid_amount_spec</c> /
/// <c>update_datetime_spec</c> 三页标「数组最大长度 100」，<c>update_daily_budget_spec</c> 本页
/// <b>未标</b>（同族的 <c>advertiser/update_daily_budget</c> 却标了 100）⇒ 视为该页文档遗漏、照录不补，
/// SDK 不做本地长度拦截；超限时由官方 <c>code</c> 表达。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupBatchData
{
    /// <summary>逐条结果列表（官方 <c>list</c>，<c>struct[]</c>，顺序与请求数组一致）。</summary>
    [JsonPropertyName("list")]
    public List<AdsAdgroupBatchResultItem>? List { get; set; }

    /// <summary>失败的营销单元 id 集合（官方 <c>fail_id_list</c>，<c>integer[]</c>）。</summary>
    [JsonPropertyName("fail_id_list")]
    public List<long>? FailIdList { get; set; }
}

/// <summary>
/// 营销单元批量族的逐条结果（官方 <c>data.list[]</c> 元素：公共三字段 + <c>adgroup_id</c>）。
/// </summary>
/// <remarks>
/// <b>官方本页自相矛盾之处（照录）</b>：<c>adgroup_id</c> 字段表标 <c>int64</c>，应答示例里却是
/// 占位符字符串 <c>"&lt;ADGROUP_ID&gt;"</c> ⇒ 本类型按字段表取 <see cref="long"/>。
/// <c>update_daily_budget</c> 页更把嵌套 <c>adgroup_id</c> 的<b>描述</b>写成了父结构的「更新日限额条件」
/// （复制粘贴痕迹），语义仍取「营销单元 id」（<c>update_datetime</c> 页同字段描述为「营销单元 id」可交叉印证）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupBatchResultItem : AdsBatchResultItem
{
    /// <summary>本条结果对应的营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }
}

/// <summary>批量改日预算的单条规格（官方 <c>update_daily_budget_spec[]</c> 元素）。</summary>
/// <remarks>
/// <b>官方对 <c>daily_budget</c> 的四条下限（原文，SDK 不本地校验）</b>：单位为分；<c>0</c> 表示不限预算；
/// 需介于 5,000–400,000,000 分；不得低于「今日已消耗 × 1.5 + 冻结金」；且不得低于「今日已消耗 + 5,000 分」。
/// 后两条依赖当日实时消耗与账户冻结金（调用方与 SDK 都拿不到）⇒ 只有官方网关能判，
/// 本地拦截只会造出假阳性；越界由应答 <c>code</c> 表达。
/// <b>本值域与账户级日预算不同</b>：<c>advertiser/update_daily_budget</c> 的上限是 4,000,000,000 分（十倍），
/// 两者不可互换 ⇒ 见各自类型，不做公共常量。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateDailyBudgetSpec
{
    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>，<b>必填</b>；同一数组内不允许重复）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>日预算（官方 <c>daily_budget</c>，<c>integer</c>，<b>必填</b>，单位为分；<c>0</c> = 不限预算；ADX 程序化投放不可填写提交）。</summary>
    [JsonPropertyName("daily_budget")]
    public long? DailyBudget { get; set; }
}

/// <summary>批量修改营销单元日预算（<c>POST /v3.0/adgroups/update_daily_budget</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateDailyBudgetRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>；原文「不支持代理商 id」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>批量规格列表（官方 <c>update_daily_budget_spec</c>，<c>struct[]</c>，<b>必填</b>；本页未标数组最大长度，见 <see cref="AdsAdgroupBatchData"/> remarks）。</summary>
    [JsonPropertyName("update_daily_budget_spec")]
    public List<AdsAdgroupUpdateDailyBudgetSpec>? UpdateDailyBudgetSpec { get; set; }
}

/// <summary><c>adgroups/update_daily_budget</c> 应答信封（闭合类型）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateDailyBudgetResponse : AdsResponse<AdsAdgroupBatchData>
{
}

/// <summary>批量改投放状态的单条规格（官方 <c>update_configured_status_spec[]</c> 元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateConfiguredStatusSpec
{
    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>，<b>必填</b>；同一数组内不允许重复）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>目标状态（官方 <c>configured_status</c>，<c>enum</c>，<b>必填</b>，
    /// 可选值 <c>{ AD_STATUS_NORMAL, AD_STATUS_SUSPEND }</c>；ADX 程序化投放不可填写提交。
    /// 官方枚举集会随权限变化 ⇒ 以 <see cref="string"/> 承载、不做本地枚举）。</summary>
    [JsonPropertyName("configured_status")]
    public string? ConfiguredStatus { get; set; }
}

/// <summary>批量修改营销单元投放状态（<c>POST /v3.0/adgroups/update_configured_status</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateConfiguredStatusRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>；原文「不支持代理商 id」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>批量规格列表（官方 <c>update_configured_status_spec</c>，<c>struct[]</c>，<b>必填</b>，数组最大长度 100）。</summary>
    [JsonPropertyName("update_configured_status_spec")]
    public List<AdsAdgroupUpdateConfiguredStatusSpec>? UpdateConfiguredStatusSpec { get; set; }
}

/// <summary><c>adgroups/update_configured_status</c> 应答信封（闭合类型）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateConfiguredStatusResponse : AdsResponse<AdsAdgroupBatchData>
{
}

/// <summary>批量改出价金额的单条规格（官方 <c>update_bid_amount_spec[]</c> 元素）。</summary>
/// <remarks>官方对 <c>bid_amount</c> 原文：单位为分，<b>ADX 程序化投放默认填写 200</b>，
/// 并指向「出价规则」章节 —— 出价的合法性依赖 <c>bid_mode</c> / <c>optimization_goal</c> 组合（本结构不携带），
/// 故 SDK 不做范围校验。</remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateBidAmountSpec
{
    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>，<b>必填</b>；同一数组内不允许重复）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>出价金额（官方 <c>bid_amount</c>，<c>integer</c>，<b>必填</b>，单位为分）。</summary>
    [JsonPropertyName("bid_amount")]
    public long? BidAmount { get; set; }
}

/// <summary>批量修改营销单元出价（<c>POST /v3.0/adgroups/update_bid_amount</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateBidAmountRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>；原文「不支持代理商 id」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>批量规格列表（官方 <c>update_bid_amount_spec</c>，<c>struct[]</c>，<b>必填</b>，数组最大长度 100）。</summary>
    [JsonPropertyName("update_bid_amount_spec")]
    public List<AdsAdgroupUpdateBidAmountSpec>? UpdateBidAmountSpec { get; set; }
}

/// <summary><c>adgroups/update_bid_amount</c> 应答信封（闭合类型）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateBidAmountResponse : AdsResponse<AdsAdgroupBatchData>
{
}

/// <summary>
/// 批量改投放时段的单条规格（官方 <c>update_datetime_spec[]</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// <b>三条字段全是「选填」但组合起来必填</b>：官方使用说明要求每条至少给出
/// <c>begin_date</c> / <c>end_date</c> / <c>time_series</c> 之一 ⇒
/// 「条件必填」依赖兄弟字段，本地拦截会误伤合法报文，交由官方 <c>code</c> 表达。
/// </para>
/// <para>
/// <b>微信流量的额外约束（原文）</b>：更新 <c>end_time</c> 时，更新后的结束时间至少要是当前时间的
/// 6 小时之后；朋友圈类投放还要求时段跨度 &lt; 30 自然日、每天覆盖 ≥6 小时且各天时段一致。
/// 这些判定依赖流量类型，SDK 侧不可见。
/// </para>
/// <para>
/// <b>官方本页自相矛盾（照录）</b>：① 使用说明把数组写成 <c>update_date_spec</c>，
/// 而请求参数表写 <c>update_datetime_spec</c> ⇒ 字段名<b>取请求表</b>（请求表才是报文权威）；
/// ② 请求示例体只有 <c>{"account_id": ...}</c>，把标<b>必填</b>的 spec 数组整个省略。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateDatetimeSpec
{
    /// <summary>营销单元 id（官方 <c>adgroup_id</c>，<c>int64</c>，<b>必填</b>；同一数组内不允许重复）。</summary>
    [JsonPropertyName("adgroup_id")]
    public long? AdgroupId { get; set; }

    /// <summary>投放开始日期（官方 <c>begin_date</c>，<c>string</c>，<c>YYYY-MM-DD</c>，长度 10 字节，需 ≤ <c>end_date</c>）。</summary>
    [JsonPropertyName("begin_date")]
    public string? BeginDate { get; set; }

    /// <summary>投放结束日期（官方 <c>end_date</c>，<c>string</c>，<c>YYYY-MM-DD</c>，需 ≥ 今天且 ≥ <c>begin_date</c>；微信流量另有「6 小时之后」约束）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }

    /// <summary>投放时段（官方 <c>time_series</c>，<c>string</c>，48×7 = 336 位的 0/1 字符串，半小时粒度、周一零点起，全 1 = 全时段、不允许全 0）。</summary>
    [JsonPropertyName("time_series")]
    public string? TimeSeries { get; set; }
}

/// <summary>批量修改营销单元投放日期与时段（<c>POST /v3.0/adgroups/update_datetime</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateDatetimeRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>，<b>必填</b>；原文「不支持代理商 id」）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>批量规格列表（官方 <c>update_datetime_spec</c>，<c>struct[]</c>，<b>必填</b>，数组最大长度 100；官方使用说明处误写为 <c>update_date_spec</c>，取请求表名）。</summary>
    [JsonPropertyName("update_datetime_spec")]
    public List<AdsAdgroupUpdateDatetimeSpec>? UpdateDatetimeSpec { get; set; }
}

/// <summary><c>adgroups/update_datetime</c> 应答信封（闭合类型）。</summary>
[HttpJsonSerializable(SerializerClassName = "Adgroups")]
public class AdsAdgroupUpdateDatetimeResponse : AdsResponse<AdsAdgroupBatchData>
{
}
