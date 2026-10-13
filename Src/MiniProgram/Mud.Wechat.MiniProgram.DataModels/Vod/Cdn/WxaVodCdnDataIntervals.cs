// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// CDN 用量数据时间粒度取值（<c>data_interval</c>，单位分钟）。
/// </summary>
/// <remarks>
/// <para><b>默认值为 <c>1440</c>（天粒度）</b>（官方原文）；默认值不传即是天粒度，别按「5 分钟」预期读数。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnusagedata.html"/></para>
/// </remarks>
public static class WxaVodCdnDataIntervals
{
    /// <summary>5 分钟粒度，返回明细数据（<c>5</c>）。</summary>
    public const int FiveMinutes = 5;

    /// <summary>小时粒度（<c>60</c> 分钟）。</summary>
    public const int Hourly = 60;

    /// <summary>天粒度（<c>1440</c> 分钟，官方默认值）。</summary>
    public const int Daily = 1440;
}
