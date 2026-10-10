// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取单个录制文件详情响应体（<c>/cgi-bin/meeting/record/get_file</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档陷阱：<see cref="StartTime"/> / <see cref="EndTime"/> 参数表标注 int64，官方示例为字符串形态（如 "1743131564453"），
/// 本模型以参数表为准（整数承载）；<see cref="MeetingSummary"/> 参数表标注 object[]（列表），官方示例为单个对象，本模型以参数表为准（数组承载）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetRecordFileResponse : WechatWorkResponse
{
    /// <summary>获取或设置录制文件 ID。</summary>
    [JsonPropertyName("record_file_id")]
    public string? RecordFileId { get; set; }

    /// <summary>获取或设置会议 ID。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置入会码。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置播放地址。</summary>
    [JsonPropertyName("view_address")]
    public string? ViewAddress { get; set; }

    /// <summary>获取或设置下载地址（默认 6 小时过期）。</summary>
    [JsonPropertyName("download_address")]
    public string? DownloadAddress { get; set; }

    /// <summary>获取或设置下载视频文件格式（例如：mp4）。</summary>
    [JsonPropertyName("download_address_file_type")]
    public string? DownloadAddressFileType { get; set; }

    /// <summary>获取或设置音频下载地址（默认 6 小时过期）。</summary>
    [JsonPropertyName("audio_address")]
    public string? AudioAddress { get; set; }

    /// <summary>获取或设置下载音频文件格式（例如：m4a）。</summary>
    [JsonPropertyName("audio_address_file_type")]
    public string? AudioAddressFileType { get; set; }

    /// <summary>获取或设置会议纪要文件列表（详见 <see cref="RecordFileDownload"/>）。</summary>
    [JsonPropertyName("meeting_summary")]
    public List<RecordFileDownload>? MeetingSummary { get; set; }

    /// <summary>获取或设置录制转写文件（智能优化版）列表（详见 <see cref="RecordFileDownload"/>）。</summary>
    [JsonPropertyName("ai_meeting_transcripts")]
    public List<RecordFileDownload>? AiMeetingTranscripts { get; set; }

    /// <summary>获取或设置录制文件名。</summary>
    [JsonPropertyName("record_name")]
    public string? RecordName { get; set; }

    /// <summary>获取或设置录制开始时间（参数表标注 int64；官方示例为字符串形态，本模型以参数表为准）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置录制结束时间（参数表标注 int64；官方示例为字符串形态，本模型以参数表为准）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置会议录制名。</summary>
    [JsonPropertyName("meeting_record_name")]
    public string? MeetingRecordName { get; set; }
}
