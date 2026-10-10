// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.DataModels.Common;

/// <summary>
/// 查询日期区间（v3.0 的 <c>date_range</c>，官方 <c>struct</c>，<b>复合 Query 参数</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>落位 Common 的理由</b>：它与 <see cref="AdsFiltering"/> 同属「各 <c>*/get</c> 接口的复合 Query 形状」，
/// 而这类形状的唯一出口是 <see cref="AdsQueryJson"/>（同在 Common）—— 放到某个业务域会让公用编码器反向
/// 依赖域命名空间。已核验页面上的两处官方差异<b>逐页不同</b>（跨度上限、起止是否必须相等），
/// 故本类型<b>不</b>承载任何校验，只承载形状；两条差异写在报表域端点 XML 里。
/// </para>
/// <para>
/// <b>官方线格式是单个 JSON 对象字面量</b>（curl 逐字：<c>-d 'date_range={"start_date": "2024-01-01",
/// "end_date": "2024-01-01"}'</c>），<b>不是</b>展平成 <c>date_range.start_date=…</c> ⇒
/// 端点签名用 <see cref="string"/> 承载、由 <see cref="AdsQueryJson.DateRange"/> 编码。
/// </para>
/// <para>
/// <b>两处官方自相矛盾（照录，SDK 不校验）</b>：<c>daily_reports/get</c> 与 <c>hourly_reports/get</c>
/// 的 <c>end_date</c> 约束都引用 <c>begin_date</c>，而<b>这两页参数表里根本没有 <c>begin_date</c></b>；
/// <c>hourly_reports/get</c> 更在示例里给出 <c>start_date=2024-01-02</c> / <c>end_date=2024-01-01</c>，
/// 直接违反该页自己写的「且等于」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class AdsDateRange
{
    /// <summary>开始日期（官方 <c>start_date</c>，<c>string</c>，<b>必填</b>，<c>YYYY-MM-DD</c>、10 字节）。</summary>
    [JsonPropertyName("start_date")]
    public string? StartDate { get; set; }

    /// <summary>结束日期（官方 <c>end_date</c>，<c>string</c>，<b>必填</b>，<c>YYYY-MM-DD</c>、10 字节）。</summary>
    [JsonPropertyName("end_date")]
    public string? EndDate { get; set; }
}

/// <summary>
/// 排序条件（v3.0 的 <c>order_by</c> 元素，官方 <c>struct[]</c>，<b>复合 Query 参数</b>）。
/// </summary>
/// <remarks>
/// <b>不做成本地枚举</b>：<c>sort_field</c> 的合法取值即「本页 <c>fields</c> 里能返回的指标名」，随
/// <c>level</c> 与页面而变（报表族约 150 支指标），本地枚举必然失真；<c>sort_type</c> 官方在两页都是
/// <c>{ ASCENDING, DESCENDING }</c>，同样以 <see cref="string"/> 承载。
/// 键名与 <see cref="AdsQueryJson.OrderBy"/> 写出的字面量同源（守卫 ADS-B2）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class AdsOrderBy
{
    /// <summary>排序字段名（官方 <c>sort_field</c>，<c>string</c>，<b>必填</b>）。</summary>
    [JsonPropertyName("sort_field")]
    public string? SortField { get; set; }

    /// <summary>排序方向（官方 <c>sort_type</c>，<c>enum</c>，<b>必填</b>，可选值 <c>{ ASCENDING, DESCENDING }</c>）。</summary>
    [JsonPropertyName("sort_type")]
    public string? SortType { get; set; }
}
