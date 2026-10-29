// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;
using Mud.Wechat.Ads.DataModels.Common;

namespace Mud.Wechat.Ads.DataModels.Reports;

/// <summary>
/// 报表查询族的 <c>data</c> 载荷（<c>daily_reports/get</c> 与 <c>hourly_reports/get</c> <b>共用</b>一支）。
/// </summary>
/// <remarks>
/// <para>
/// <b>两页共用是官方事实而非偷懒</b>（2026-10-10 逐页核验）：<c>hourly_reports/get</c> 的应答字段表
/// 与 <c>daily_reports/get</c> 同为 <c>{data.list[] (struct[]), data.page_info{page, page_size,
/// total_number, total_page}}</c>，官方在该页原文即「同 daily」⇒ 只建一支本类型，两支端点各建闭合信封
/// 复用同一载荷（各建一份同形载荷就是双源，官方改字段时只会改到一处）。
/// </para>
/// <para>
/// <b>行形态为什么必须是字典而不是 DTO</b>：官方把 <c>data.list[]</c> 的元素定义为「随请求侧
/// <c>fields</c> 而变」—— <c>fields</c> 是 1–1024 项的自由字段名列表，而官方指标面（hourly 示例行约
/// 150 支指标）在留档里只有<b>平面级</b>证据（无逐字段层级核验）。把某一页示例行的六支字段
/// （<c>adgroup_id</c> / <c>adgroup_name</c> / <c>video_id</c> / <c>site_set</c> / <c>date</c> /
/// <c>view_count</c>）固化成 DTO，等于把「示例」当成「契约」：调用方一旦请求别的指标就静默丢数据。
/// 故本类型按<b>透传</b>建模 —— 键即官方 <c>fields</c> 名，值保留原始 JSON 形态（金额类为整数分、
/// 比率类可能为浮点、维度类为字符串或数组），由调用方按自己请求过的 <c>fields</c> 取值。
/// </para>
/// <para>
/// <b>键名不经命名策略转换</b>：<c>DictionaryKeyPolicy</c> 未设，故 <c>SnakeCaseLower</c> 的
/// <c>PropertyNamingPolicy</c> 只作用于<b>属性名</b>、不作用于字典键 ⇒ 官方返回的 <c>view_count</c>
/// 原名入袋。这一点由 <c>AdsReportJsonTests</c> 的往返用例钉住（若命名策略命中键名，用例即刻变红）。
/// </para>
/// <para>
/// <b>两条只写在官方使用说明、SDK 不做本地拦截的上限</b>：① <c>page * page_size &lt;= 20000</c>，
/// 超限须改用异步报表任务（<c>async_reports/add</c>）；② 金额字段单位为<b>分</b>。
/// 另有一条易踩的形态要求：取分版位数据须<b>同时</b>在 <c>group_by</c> 与 <c>fields</c> 里带上
/// <c>site_set</c> —— 只加一处会得到「看起来成功、版位维度静默丢失」的结果。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsReportListData
{
    /// <summary>
    /// 报表行列表（官方 <c>list</c>，<c>struct[]</c>）。
    /// 每行的键集<b>等于本次请求 <c>fields</c> 里官方实际返回的那组字段名</b>，值保持原始 JSON 形态。
    /// </summary>
    [JsonPropertyName("list")]
    public List<Dictionary<string, JsonElement>>? List { get; set; }

    /// <summary>分页元信息（官方 <c>page_info</c>，与 <see cref="AdsPageInfo"/> 共用公共形状）。</summary>
    [JsonPropertyName("page_info")]
    public AdsPageInfo? PageInfo { get; set; }
}

/// <summary>
/// <c>daily_reports/get</c> 应答信封（闭合类型）。
/// </summary>
/// <remarks>
/// <b>必须声明闭合类型</b>：开放泛型 <c>AdsResponse&lt;TData&gt;</c> 不登记源生成上下文
/// （<c>SYSLIB1030</c>，STJ 源生成对开放泛型不产出元数据），直接以
/// <c>AdsResponse&lt;AdsReportListData&gt;</c> 作方法返回类型会在 Native AOT 下无元数据。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsDailyReportResponse : AdsResponse<AdsReportListData>
{
}

/// <summary>
/// <c>hourly_reports/get</c> 应答信封（闭合类型，载荷与 <see cref="AdsDailyReportResponse"/> 同一支）。
/// </summary>
/// <remarks>
/// <b>本页两处官方自相矛盾（照录，SDK 不本地校验）</b>：① <c>page</c> 上限 100 与
/// 「<c>page * page_size &lt;= 20000</c>」只在 <c>page_size = 200</c> 附近自洽（100 × 2000 = 200000 越界）；
/// ② 示例请求的 <c>start_date</c> 晚于 <c>end_date</c>，直接违反该页自己写的「且等于」约束。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Reports")]
public class AdsHourlyReportResponse : AdsResponse<AdsReportListData>
{
}
