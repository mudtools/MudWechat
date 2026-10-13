// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询 CDN 日志下载链接列表」请求体（<c>POST /wxa/sec/vod/getcdnlogs</c>）。
/// </summary>
/// <remarks>
/// <para><b>业务限制（官方原文）</b>：可查最近 30 天内的日志下载链接，单次查询时间跨度不超过 48 小时（<c>end_time - start_time &lt;= 48h</c>）；日志下载链接有效期 24 小时。</para>
/// <para><b><c>limit</c>/<c>offset</c> 官方标注 2024-03-29 起废弃</b>⇒ 保留属性但注释即弃用语义，新接入方不必传。</para>
/// <para><b>官方参数表内这两个参数名前带零宽字符 U+200C</b>（表格单元格的排版残留）⇒ 线格式取净名 <c>limit</c>/<c>offset</c>，零宽字符不可能是 JSON 键的一部分（本仓扫描/生成两侧的归一化都以此为准）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnlogs.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetCdnLogsRequest
{
    /// <summary>start_time（官方类型 <c>number</c>，<b>必填</b>）：起始时间戳。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>end_time（官方类型 <c>number</c>，<b>必填</b>）：结束时间戳，必须大于起始时间，起始到结束时间的跨度不要超过48小时（end_time-start_time&lt;=48h）</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>limit（官方类型 <c>number</c>）：2024年3月29日起废弃。分页拉取的最大返回结果数。默认值：100；最大值：1000</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>offset（官方类型 <c>number</c>）：2024年3月29日起废弃。分页拉取的起始偏移量。默认值：0</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>query_type（官方类型 <c>number</c>）：查询类型，0：通用播放流量和短剧播放器定向流量；1：短剧播放器定向流量；2：通用播放流量。默认值为0。</summary>
    [JsonPropertyName("query_type")]
    public long? QueryType { get; set; }
}
