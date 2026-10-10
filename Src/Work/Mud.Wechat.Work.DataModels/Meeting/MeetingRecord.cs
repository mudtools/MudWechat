// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议录制对象（获取会议录制列表响应 <c>record_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingRecord
{
    /// <summary>获取或设置会议录制 ID。</summary>
    [JsonPropertyName("meeting_record_id")]
    public string? MeetingRecordId { get; set; }

    /// <summary>获取或设置会议 ID。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置会议 code。</summary>
    [JsonPropertyName("meeting_code")]
    public string? MeetingCode { get; set; }

    /// <summary>获取或设置主持人 userid。</summary>
    [JsonPropertyName("host_user_id")]
    public string? HostUserId { get; set; }

    /// <summary>获取或设置会议开始时间（UNIX 时间戳，单位毫秒）。</summary>
    [JsonPropertyName("meeting_start_time")]
    public long? MeetingStartTime { get; set; }

    /// <summary>获取或设置会议主题。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置录制状态：1 - 录制中；3 - 转码完成（当状态为转码完成才会返回录制文件列表）。</summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>获取或设置录制文件列表（详见 <see cref="MeetingRecordFile"/>）。</summary>
    [JsonPropertyName("record_file_list")]
    public List<MeetingRecordFile>? RecordFileList { get; set; }
}
