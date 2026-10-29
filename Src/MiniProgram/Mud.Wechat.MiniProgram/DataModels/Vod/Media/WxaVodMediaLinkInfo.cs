// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 媒资播放信息（官方 <c>Res.media_info</c>，仅 <c>getmedialink</c> 返回的窄字段面）。
/// </summary>
/// <remarks>
/// <para><b>能否取到链接取决于剧目审核状态</b>（官方原文）：可能出现「单集审核通过但剧目整体未通过」，这种情况取不到播放链接。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedialink.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodMediaLinkInfo
{
    /// <summary>media_id（官方类型 <c>number</c>）：媒资文件id</summary>
    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }

    /// <summary>duration（官方类型 <c>number</c>）：播放时长，单位：秒</summary>
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }

    /// <summary>name（官方类型 <c>string</c>）：媒资文件名</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>description（官方类型 <c>string</c>）：描述</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>cover_url（官方类型 <c>string</c>）：封面图临时链接</summary>
    [JsonPropertyName("cover_url")]
    public string? CoverUrl { get; set; }

    /// <summary>mp4_url（官方类型 <c>string</c>）：mp4格式临时链接</summary>
    [JsonPropertyName("mp4_url")]
    public string? Mp4Url { get; set; }

    /// <summary>hls_url（官方类型 <c>string</c>）：hls格式临时链接</summary>
    [JsonPropertyName("hls_url")]
    public string? HlsUrl { get; set; }
}
