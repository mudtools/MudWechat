// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询 CDN 用量数据」请求体（<c>POST /wxa/sec/vod/getcdnusagedata</c>）。
/// </summary>
/// <remarks>
/// <para><b>业务限制（官方原文）</b>：可查最近 365 天内的数据、查询时间跨度不超过 90 天；流量为查询时间粒度内的总流量；换算规则 1GB=1000MB、1MB=1000KB（十进制，非 1024）。</para>
/// <para><b><c>data_interval</c> 的官方类型列两侧不一致，以请求示例为准取 <c>number</c></b>：请求表列作 <c>string</c>、返回表列作 <c>number</c>，而官方请求示例送 <c>"data_interval": 5</c>（无引号）⇒ 本 SDK 两侧都用 <c>long</c>，与 <c>auditdrama</c> 的 <c>authorized</c>、<c>getcdnlogs</c> 的 <c>domestic_cdn_logs</c> 同判据（示例优先于表格）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnusagedata.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetCdnUsageDataRequest
{
    /// <summary>start_time（官方类型 <c>number</c>，<b>必填</b>）：起始时间戳。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>end_time（官方类型 <c>number</c>，<b>必填</b>）：截止时间戳。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>data_interval（官方类型 <c>string</c>，<b>必填</b>）：用量数据的时间粒度，单位：分钟，取值有：5：5 分钟粒度，返回指定查询时间内5分钟粒度的明细数据。60：小时粒度，返回指定查询时间内1小时粒度的数据。1440：天粒度，返回指定查询时间内1天粒度的数据。默认值为1440，返回天粒度的数据。</summary>
    [JsonPropertyName("data_interval")]
    public long? DataInterval { get; set; }

    /// <summary>query_type（官方类型 <c>number</c>）：查询类型，0：通用播放流量和短剧播放器定向流量；1：短剧播放器定向流量；2：通用播放流量。默认值为0。</summary>
    [JsonPropertyName("query_type")]
    public long? QueryType { get; set; }
}
