// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 会话记录消息内容（<see cref="ConversationRecord.Content"/>，按消息类型命中对应字段）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class ConversationContent
{
    /// <summary>
    /// 获取或设置文本消息内容（msgtype=text）。
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 获取或设置媒体文件 ID（msgtype=image/voice/video/file）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public string? MediaId { get; set; }

    /// <summary>
    /// 获取或设置文件名（msgtype=file）。
    /// </summary>
    [JsonPropertyName("filename")]
    public string? FileName { get; set; }

    /// <summary>
    /// 获取或设置文件大小（字节，msgtype=file）。
    /// </summary>
    [JsonPropertyName("filesize")]
    public long? FileSize { get; set; }

    /// <summary>
    /// 获取或设置链接标题（msgtype=link）。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置链接描述（msgtype=link）。
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 获取或设置链接地址（msgtype=link）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置位置纬度（msgtype=location）。
    /// </summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// 获取或设置位置经度（msgtype=location）。
    /// </summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    /// <summary>
    /// 获取或设置位置名称（msgtype=location）。
    /// </summary>
    [JsonPropertyName("location_name")]
    public string? LocationName { get; set; }

    /// <summary>
    /// 获取或设置位置地址（msgtype=location）。
    /// </summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }
}
