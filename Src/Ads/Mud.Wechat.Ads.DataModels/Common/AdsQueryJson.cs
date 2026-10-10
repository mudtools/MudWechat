// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;

namespace Mud.Wechat.Ads.DataModels.Common;

/// <summary>
/// 官方「复合类型 Query 参数」的线格式编码器 —— 把 <c>struct[]</c> / <c>string[]</c> 形态的请求参数
/// 编成官方要求的<b>单个 JSON 字符串</b> Query 值。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须由 SDK 编码、而不是直接传集合</b>（2026-10-10 逐页核验 + 组件源码核验）：
/// 官方 curl 示例将复合参数写成<b>一个</b>键值对，值是 JSON 字面量 ——
/// <c>curl -G -d 'filtering=[{"field":"corporation_name","operator":"EQUALS","values":["腾讯计算机系统有限公司"]}]'</c>、
/// <c>-d 'fields=[]'</c>。而组件声明式客户端的 <c>[Query]</c> 绑定规则与这三种形态<b>都不吻合</b>：
/// </para>
/// <list type="bullet">
/// <item><description><c>string[]</c> / <c>T[]</c>（<c>IsSimpleType</c> 命中的数组）⇒ <b>重复同名参数</b>
/// （<c>fields=a&amp;fields=b</c>），且默认无分隔符模式。</description></item>
/// <item><description>复杂类型 ⇒ <b>逐属性展平</b>成多个键，键名取 <b>C# 属性名</b>（PascalCase），
/// 与官方 <c>snake_case</c> 单键形态完全不符。</description></item>
/// <item><description><c>TreatAsString = true</c> 只改变「属性值以 <c>ToString()</c> 而非 JSON 承载」，
/// <b>不会</b>把整个对象收成一个 JSON 值 ⇒ 对本场景无帮助。</description></item>
/// </list>
/// <para>
/// 于是端点签名只能用 <c>string</c> 承载这些复合参数（照官方线格式），并由本类给出<b>唯一的</b>构造入口。
/// 重复同名参数与官方 JSON 字面量两种形态孰为官方实际接受形态<b>无官方说明</b> ⇒
/// <b>不得</b>「赌一把」改成集合参数（UNVERIFIED 的官方事实不得进入实现，更不得进入守卫断言）。
/// </para>
/// <para>
/// <b>AOT 与多 TFM</b>：直接用 <see cref="Utf8JsonWriter"/> 写，而不是 <c>JsonSerializer</c> 反射重载
/// （守卫 ADS-B6），也<b>不</b>依赖生成上下文的属性名（生成物随脚本重跑而变，手写代码不应耦合其命名）。
/// 同一份实现在四档 TFM 下逐字节一致，无需 <c>#if</c> 分支。
/// </para>
/// <para>
/// <b>字段名单源</b>：本类写出的 <c>field</c> / <c>operator</c> / <c>values</c> 与
/// <see cref="AdsFiltering"/>、<c>start_date</c> / <c>end_date</c> 与 <see cref="AdsDateRange"/>、
/// <c>sort_field</c> / <c>sort_type</c> 与 <see cref="AdsOrderBy"/> 的 <c>[JsonPropertyName]</c> 必须一致，
/// 由守卫 ADS-B2 逐条锁定（改名会在「DTO 声明」与「编码器」两处同时失败，不存在静默漂移）。
/// </para>
/// </remarks>
public static class AdsQueryJson
{
    /// <summary>
    /// 编码 <c>fields</c>（官方 <c>string[]</c>，指定返回字段列表）。
    /// </summary>
    /// <param name="fields">字段名列表；<c>null</c> 集合按空列表处理，集合内 <c>null</c> 项跳过。</param>
    /// <returns>JSON 数组字符串，如 <c>["account_id","daily_budget"]</c>；空列表为 <c>[]</c>。</returns>
    /// <remarks>
    /// 官方（<c>advertiser/get</c>，2026-10-10 核验）：<c>fields</c> 标必填，数组最小长度 1、最大长度 256，
    /// 每项 1–64 字节。SDK <b>不做</b>本地长度拦截（官方示例恰好用 <c>fields=[]</c> 传空 ⇒ 拦截会挡掉合法请求），
    /// 越界由应答 <c>code</c> 表达。
    /// </remarks>
    public static string Fields(params string[]? fields)
    {
        return Write(writer =>
        {
            writer.WriteStartArray();
            if (fields is not null)
            {
                foreach (var field in fields)
                {
                    if (field is not null)
                        writer.WriteStringValue(field);
                }
            }

            writer.WriteEndArray();
        });
    }

    /// <summary>
    /// 编码 <c>filtering</c>（官方 <c>struct[]</c>，过滤条件）。
    /// </summary>
    /// <param name="filtering">过滤条件列表；<c>null</c> 集合按空列表处理，集合内 <c>null</c> 项跳过。</param>
    /// <returns>JSON 数组字符串，如 <c>[{"field":"corporation_name","operator":"EQUALS","values":["…"]}]</c>；空列表为 <c>[]</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>不传 <c>filtering</c> 与传 <c>[]</c> 官方等价</b>（原文「若此字段不传，或传空则视为无限制条件」），
    /// 故调用方可在无条件时直接传 <c>null</c> 参数而不必调本方法。
    /// </para>
    /// <para>逐端点可用的 <c>field</c>/<c>operator</c> 组合与数组长度上限写在对应端点 XML（守卫 ADS-B2 锁定可选值）。</para>
    /// </remarks>
    public static string Filtering(params AdsFiltering?[]? filtering)
    {
        return Write(writer =>
        {
            writer.WriteStartArray();
            if (filtering is not null)
            {
                foreach (var item in filtering)
                {
                    if (item is null)
                        continue;

                    // 官方三字段名与 AdsFiltering 的 [JsonPropertyName] 同源（守卫 ADS-B2）。
                    writer.WriteStartObject();
                    if (item.Field is not null)
                        writer.WriteString("field", item.Field);
                    if (item.Operator is not null)
                        writer.WriteString("operator", item.Operator);
                    writer.WritePropertyName("values");
                    writer.WriteStartArray();
                    if (item.Values is not null)
                    {
                        foreach (var value in item.Values)
                        {
                            if (value is not null)
                                writer.WriteStringValue(value);
                        }
                    }

                    writer.WriteEndArray();
                    writer.WriteEndObject();
                }
            }

            writer.WriteEndArray();
        });
    }

    /// <summary>
    /// 编码 <c>date_range</c>（官方 <c>struct</c>，查询日期区间）。
    /// </summary>
    /// <param name="dateRange">区间对象；<c>null</c> 属性按「不写该键」处理。</param>
    /// <returns>JSON 对象字符串，如 <c>{"start_date":"2024-01-01","end_date":"2024-01-01"}</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方线格式逐字取自 curl</b>（2026-10-10 核验 <c>daily_reports/get</c>）：
    /// <c>-d 'date_range={"start_date": "2024-01-01", "end_date": "2024-01-01"}'</c> ——
    /// <b>一个</b>键、值是<b>整个</b> JSON 对象。展平成 <c>date_range.start_date=…</c> 或拆成
    /// <c>start_date=…&amp;end_date=…</c> 都不是该页声明的形态 ⇒ 只能由本方法收成一支字符串。
    /// </para>
    /// <para>
    /// <b>SDK 不做日期合法性与跨度校验</b>：官方两页的约束互相矛盾（<c>end_date</c> 的约束引用了
    /// 参数表里<b>不存在</b>的 <c>begin_date</c>；hourly 示例的 <c>start_date</c> 晚于 <c>end_date</c>），
    /// 且「最早支持 1 年内 / 最多 90 天、最长跨度 1 天」这类上限随 <c>level</c> 变化 ⇒ 本地拦截必然
    /// 造出假阳性，越界由应答 <c>code</c> 表达。
    /// </para>
    /// </remarks>
    public static string DateRange(AdsDateRange? dateRange)
    {
        return Write(writer =>
        {
            writer.WriteStartObject();
            if (dateRange?.StartDate is not null)
                writer.WriteString("start_date", dateRange.StartDate);
            if (dateRange?.EndDate is not null)
                writer.WriteString("end_date", dateRange.EndDate);

            writer.WriteEndObject();
        });
    }

    /// <summary>
    /// 编码 <c>group_by</c>（官方 <c>string[]</c>，分组维度）。
    /// </summary>
    /// <param name="groupBy">维度名列表；<c>null</c> 集合按空列表处理，集合内 <c>null</c> 项跳过。</param>
    /// <returns>JSON 数组字符串，如 <c>["date"]</c>；空列表为 <c>[]</c>。</returns>
    /// <remarks>
    /// <para>
    /// 与 <see cref="Fields"/> 同为 <c>string[]</c>，但<b>元素长度上限不同</b>（官方逐页给 255 字节或 64 字节），
    /// 且 <c>daily_reports/get</c> / <c>hourly_reports/get</c> 把它标为<b>必填</b>而
    /// <c>async_reports/add</c> 标为选填 ⇒ 形态一致、约束逐页不同，故只在各端点 XML 写上限、不做本地拦截。
    /// </para>
    /// <para>
    /// <b>分版位数据的配套要求（官方原文）</b>：需<b>同时</b>在 <c>group_by</c> 和 <c>fields</c> 里加
    /// <c>site_set</c> 维度 —— 只加一处不会报错，但版位维度静默丢失，是本族最难归因的一类结果偏差。
    /// </para>
    /// </remarks>
    public static string GroupBy(params string[]? groupBy)
    {
        return Write(writer =>
        {
            writer.WriteStartArray();
            if (groupBy is not null)
            {
                foreach (var item in groupBy)
                {
                    if (item is not null)
                        writer.WriteStringValue(item);
                }
            }

            writer.WriteEndArray();
        });
    }

    /// <summary>
    /// 编码 <c>order_by</c>（官方 <c>struct[]</c>，排序条件）。
    /// </summary>
    /// <param name="orderBy">排序条件；<c>null</c> 集合按空列表处理，集合内 <c>null</c> 项跳过。</param>
    /// <returns>JSON 数组字符串，如 <c>[{"sort_field":"cost","sort_type":"DESCENDING"}]</c>；空列表为 <c>[]</c>。</returns>
    /// <remarks>
    /// 官方上限「数组最小长度 1、最大长度 2」，<c>sort_field</c> 取值即本页可返回的指标名（随 <c>level</c> 而变，
    /// 报表族约 150 支）⇒ 不做本地枚举也不做长度拦截；两键名与 <see cref="AdsOrderBy"/> 的
    /// <c>[JsonPropertyName]</c> 同源（守卫 ADS-B2）。
    /// </remarks>
    public static string OrderBy(params AdsOrderBy?[]? orderBy)
    {
        return Write(writer =>
        {
            writer.WriteStartArray();
            if (orderBy is not null)
            {
                foreach (var item in orderBy)
                {
                    if (item is null)
                        continue;

                    writer.WriteStartObject();
                    if (item.SortField is not null)
                        writer.WriteString("sort_field", item.SortField);
                    if (item.SortType is not null)
                        writer.WriteString("sort_type", item.SortType);

                    writer.WriteEndObject();
                }
            }

            writer.WriteEndArray();
        });
    }

    /// <summary>写出紧凑 JSON 字符串（转义由 <see cref="Utf8JsonWriter"/> 负责，不做手工拼接）。</summary>
    private static string Write(Action<Utf8JsonWriter> writeBody)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writeBody(writer);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
