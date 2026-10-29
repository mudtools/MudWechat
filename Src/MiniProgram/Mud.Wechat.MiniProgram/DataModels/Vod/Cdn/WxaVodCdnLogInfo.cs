// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 单条 CDN 日志下载项。
/// </summary>
/// <remarks>
/// <para><b><c>date</c>/<c>name</c>/<c>url</c> 官方类型列作 <c>number</c>，示例证明是字符串</b>（<c>"date": "2024-03-28"</c>、<c>"name": "2024032819-…-mainland"</c>、url 为下载链接）⇒ 三者照示例落 <c>string</c>；<c>start_time</c>/<c>end_time</c> 示例为无引号数字 ⇒ 保持 <c>long</c>。</para>
/// <para><b>一个小时可能产生多个文件</b>（官方原文：每个文件对应一个域名，平台可能用多域名分发）；该小时无 CDN 访问则不生成日志文件 ⇒ 不得假设「一次查询得到一条」。</para>
/// <para><b>日志字节数为应用层数据</b>（官方原文）：与计费数据存在差异，实际网络流量比纯应用层多 5%-15%。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnlogs.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodCdnLogInfo
{
    /// <summary>date（官方类型 <c>number</c>）：日志所属日期。</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>name（官方类型 <c>number</c>）：日志名称。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>url（官方类型 <c>number</c>）：日志下载链接，24小时内下载有效。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>start_time（官方类型 <c>number</c>）：日志起始时间。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>end_time（官方类型 <c>number</c>）：日志结束时间</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}
