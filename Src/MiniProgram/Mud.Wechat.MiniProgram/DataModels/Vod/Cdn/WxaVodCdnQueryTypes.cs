// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// CDN 查询类型取值（<c>getcdnusagedata</c> 与 <c>getcdnlogs</c> 的 <c>query_type</c>，两页值集相同）。
/// </summary>
/// <remarks>
/// <para><b>共用裁决</b>：用量数据与日志两支端点的 <c>query_type</c> 值域逐值相同（0/1/2）⇒ 共用本类；<b>但流量包的 <c>query_type</c> 值集只有 0/1</b>（语义也不同）⇒ 用 <see cref="WxaVodPackageQueryTypes"/>，不得混用。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnusagedata.html"/></para>
/// </remarks>
public static class WxaVodCdnQueryTypes
{
    /// <summary>通用播放流量 + 短剧播放器定向流量（<c>0</c>，官方默认值）。</summary>
    public const int All = 0;

    /// <summary>短剧播放器定向流量（<c>1</c>）。</summary>
    public const int ShortDramaPlayer = 1;

    /// <summary>通用播放流量（<c>2</c>）。</summary>
    public const int GeneralPlayback = 2;
}
