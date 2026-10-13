// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 媒资详细信息（官方 <c>Res.media_info</c>，<c>getmedia</c> 与 <c>listmedia</c> 共用同一字段面）。
/// </summary>
/// <remarks>
/// <para><b><c>file_size</c> 官方类型为 <c>string</c></b>（示例值 <c>"9849163"</c>，带引号）⇒ 照录为 <c>string</c>，<b>不得</b>驼峰化或改成数字（同域 <c>duration</c> 才是 <c>number</c>）。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedia.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodMediaInfo
{
    /// <summary>media_id（官方类型 <c>number</c>）：媒资文件id</summary>
    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }

    /// <summary>create_time（官方类型 <c>number</c>）：上传时间，时间戳</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>expire_time（官方类型 <c>number</c>）：过期时间，时间戳</summary>
    [JsonPropertyName("expire_time")]
    public long? ExpireTime { get; set; }

    /// <summary>drama_id（官方类型 <c>number</c>）：所属剧目id</summary>
    [JsonPropertyName("drama_id")]
    public long? DramaId { get; set; }

    /// <summary>file_size（官方类型 <c>string</c>）：媒资文件大小，单位：字节</summary>
    [JsonPropertyName("file_size")]
    public string? FileSize { get; set; }

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

    /// <summary>original_url（官方类型 <c>string</c>）：原始视频临时链接</summary>
    [JsonPropertyName("original_url")]
    public string? OriginalUrl { get; set; }

    /// <summary>mp4_url（官方类型 <c>string</c>）：mp4格式临时链接</summary>
    [JsonPropertyName("mp4_url")]
    public string? Mp4Url { get; set; }

    /// <summary>hls_url（官方类型 <c>string</c>）：hls格式临时链接</summary>
    [JsonPropertyName("hls_url")]
    public string? HlsUrl { get; set; }

    /// <summary>audit_detail（官方类型 <c>object</c>）：审核信息</summary>
    [JsonPropertyName("audit_detail")]
    public WxaVodMediaAuditDetail? AuditDetail { get; set; }
}
