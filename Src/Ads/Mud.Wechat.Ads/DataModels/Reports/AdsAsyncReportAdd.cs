// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Reports;

/// <summary>
/// 创建异步报表任务（<c>POST /v3.0/async_reports/add</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域唯一的「同步转异步」入口</b>：官方对同步查询写了硬上限 <c>page * page_size &lt;= 20000</c>，
/// 超限的数据只能建异步任务、轮询 <c>async_reports/get</c> 拿 <c>file_id</c> 再下载。
/// </para>
/// <para>
/// <b>与两支同步查询页的三处形态差异（逐页核验 2026-10-10，不得「对齐」掉）</b>：
/// ① 日期用<b>单支</b> <c>date</c>（配 <c>granularity</c> 决定粒度），<b>没有</b> <c>date_range</c> 结构；
/// ② <b>没有</b> <c>page</c> / <c>page_size</c>（任务只回一个 <c>task_id</c>）；
/// ③ 字段清单那支叫 <c>report_fields</c> 而同步两页叫 <c>fields</c>。
/// </para>
/// <para>
/// <b><c>level</c> 的取值集与两支同步查询页各不相同</b>（daily 17 支 / hourly 8 支 / 本页 21 支）：
/// 本页在 daily 的集合上<b>增</b> <c>AGE</c> / <c>GENDER</c> / <c>REGION</c> / <c>CITY</c> /
/// <c>LANDING_PAGE</c> / <c>AD_UNION</c> / <c>OS</c> 七支、<b>删</b> <c>VIDEO_HIGHLIGHT</c> /
/// <c>WECHAT_SHOP_PRODUCT</c> / <c>PLAYLET</c> 三支。这是官方页面之间的真实差异 ⇒
/// <b>不得</b>收敛成一支公共枚举（收敛必然让某一页拿到官方不接受的值），逐页集合写在各端点 XML，
/// 由守卫 ADS-B2 逐页锁定。
/// </para>
/// <para>
/// <b>本页官方矛盾（照录）</b>：请求参数表里 <c>account_id</c> <b>未标必填</b>而描述写「包括代理商和账户 id」
/// （与两支同步查询页的「不支持代理商 id」正好相反，代理商 id 在本域是<b>可用</b>的）；
/// 官方请求示例把标<b>必填</b>的 <c>report_fields</c> 整个省略。SDK 不做本地必填校验，越界由应答 <c>code</c> 表达。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncReportAddRequest
{
    /// <summary>账户 id（官方 <c>account_id</c>，<c>integer</c>；本页<b>未加星</b>但描述「包括代理商和账户 id」，
    /// 与同步两页的「不支持代理商 id」相反）。</summary>
    [JsonPropertyName("account_id")]
    public long? AccountId { get; set; }

    /// <summary>任务名（官方 <c>task_name</c>，<c>string</c>，<b>必填</b>，1–120 字节）。</summary>
    [JsonPropertyName("task_name")]
    public string? TaskName { get; set; }

    /// <summary>报表字段列表（官方 <c>report_fields</c>，<c>string[]</c>，<b>必填</b>，数组 1–1024、每项 1–64 字节）。
    /// <b>注意本页键名是 <c>report_fields</c></b>，同步查询页才是 <c>fields</c>。</summary>
    [JsonPropertyName("report_fields")]
    public List<string>? ReportFields { get; set; }

    /// <summary>查询业务单元报表的层级（官方 <c>level</c>，<c>enum</c>，<b>必填</b>，本页 21 支取值见
    /// <c>IWechatAdsReportService.AddAsyncReportAsync</c> 的参数文档；官方枚举集会随页面变化 ⇒
    /// 以 <see cref="string"/> 承载、不做本地枚举）。</summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>过滤条件（官方 <c>filtering</c>，<c>struct[]</c>，数组 1–5；本页 <c>field</c> 取值集与
    /// operator 集均比同步页宽，见端点参数文档）。</summary>
    [JsonPropertyName("filtering")]
    public List<AdsFiltering>? Filtering { get; set; }

    /// <summary>数据口径（官方 <c>time_line</c>，<c>enum</c>，可选值
    /// <c>{ REQUEST_TIME, REPORTING_TIME, ACTIVE_TIME }</c>）。</summary>
    [JsonPropertyName("time_line")]
    public string? TimeLine { get; set; }

    /// <summary>分组维度（官方 <c>group_by</c>，<c>string[]</c>，数组 1–5、每项 ≤255 字节；
    /// 原文「所有 level 均可使用 {site_set}」）。</summary>
    [JsonPropertyName("group_by")]
    public List<string>? GroupBy { get; set; }

    /// <summary>报表粒度（官方 <c>granularity</c>，<c>enum</c>，<b>必填</b>，可选值
    /// <c>{ HOURLY, DAILY }</c>；它同时决定 <see cref="Date"/> 允许回溯的天数）。</summary>
    [JsonPropertyName("granularity")]
    public string? Granularity { get; set; }

    /// <summary>报表日期（官方 <c>date</c>，<c>string</c>，<b>必填</b>，<c>YYYY-MM-DD</c>、10 字节；
    /// 原文「<c>HOURLY</c> → 最近 30 天，<c>DAILY</c> → 最近 365 天」）。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>业务单元 id（官方 <c>organization_id</c>，<c>integer</c>，0–9999999999）。</summary>
    [JsonPropertyName("organization_id")]
    public long? OrganizationId { get; set; }
}

/// <summary>
/// <c>async_reports/add</c> 的 <c>data</c> 载荷（官方只回一支 <c>task_id</c>）。
/// </summary>
/// <remarks>
/// <b>官方示例把整数渲染成了占位符字符串</b>（应答示例值为 <c>"&lt;TASK_ID&gt;"</c>，字段表标
/// <c>integer</c>）⇒ 本类型按<b>字段表</b>取 <see cref="long"/>，与 <c>adgroups</c> 批量族
/// <c>AdsAdgroupBatchResultItem</c> 的处置同口径（留档 §9 冲突登记）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncReportTaskData
{
    /// <summary>异步任务 id（官方 <c>task_id</c>，<c>integer</c>）。</summary>
    [JsonPropertyName("task_id")]
    public long? TaskId { get; set; }
}

/// <summary><c>async_reports/add</c> 应答信封（闭合类型）。</summary>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsAsyncReportAddResponse : AdsResponse<AdsAsyncReportTaskData>
{
}
