// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// CDN 统计数据点（区间起点 + 该粒度内总流量）。
/// </summary>
/// <remarks>
/// <para><b><c>value</c> 单位是字节</b>（官方原文），而流量包侧单位是 MB ⇒ 两套单位不得混用。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnusagedata.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodCdnItem
{
    /// <summary>time（官方类型 <c>number</c>）：数据所在时间区间的开始时间戳。</summary>
    [JsonPropertyName("time")]
    public long? Time { get; set; }

    /// <summary>value（官方类型 <c>number</c>）：数据大小，单位：字节。</summary>
    [JsonPropertyName("value")]
    public long? Value { get; set; }
}
