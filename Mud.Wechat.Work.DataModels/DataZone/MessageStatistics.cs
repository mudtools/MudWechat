// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.DataZone;

/// <summary>
/// 消息统计数据条目（<c>/cgi-bin/data/get_message_statistics</c> 响应的 <c>statistics[]</c> 项）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "DataZone")]
public class MessageStatistics
{
    /// <summary>
    /// 获取或设置统计日期（Unix 时间戳，秒；粒度由请求 <c>type</c> 决定）。
    /// </summary>
    [JsonPropertyName("date")]
    public long? Date { get; set; }

    /// <summary>
    /// 获取或设置发送消息总数。
    /// </summary>
    [JsonPropertyName("total_send")]
    public int? TotalSend { get; set; }

    /// <summary>
    /// 获取或设置接收消息总数。
    /// </summary>
    [JsonPropertyName("total_receive")]
    public int? TotalReceive { get; set; }

    /// <summary>
    /// 获取或设置文本消息数量。
    /// </summary>
    [JsonPropertyName("text_count")]
    public int? TextCount { get; set; }

    /// <summary>
    /// 获取或设置图片消息数量。
    /// </summary>
    [JsonPropertyName("image_count")]
    public int? ImageCount { get; set; }

    /// <summary>
    /// 获取或设置语音消息数量。
    /// </summary>
    [JsonPropertyName("voice_count")]
    public int? VoiceCount { get; set; }

    /// <summary>
    /// 获取或设置视频消息数量。
    /// </summary>
    [JsonPropertyName("video_count")]
    public int? VideoCount { get; set; }

    /// <summary>
    /// 获取或设置文件消息数量。
    /// </summary>
    [JsonPropertyName("file_count")]
    public int? FileCount { get; set; }

    /// <summary>
    /// 获取或设置链接消息数量。
    /// </summary>
    [JsonPropertyName("link_count")]
    public int? LinkCount { get; set; }

    /// <summary>
    /// 获取或设置位置消息数量。
    /// </summary>
    [JsonPropertyName("location_count")]
    public int? LocationCount { get; set; }

    /// <summary>
    /// 获取或设置活跃用户数。
    /// </summary>
    [JsonPropertyName("active_users")]
    public int? ActiveUsers { get; set; }

    /// <summary>
    /// 获取或设置活跃群聊数。
    /// </summary>
    [JsonPropertyName("active_groups")]
    public int? ActiveGroups { get; set; }
}
