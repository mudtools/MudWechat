// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 预定会议室请求体（<c>/cgi-bin/oa/meetingroom/book</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomBookRequest
{
    /// <summary>获取或设置会议室 ID（官方 meetingroom_id，必填）。</summary>
    [JsonPropertyName("meetingroom_id")]
    public int MeetingRoomId { get; set; }

    /// <summary>获取或设置会议主题（官方 subject）。</summary>
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }

    /// <summary>获取或设置预定开始时间戳（官方 start_time，秒级，必填）。</summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>获取或设置预定结束时间戳（官方 end_time，秒级，必填）。</summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>获取或设置预定者的成员账号（官方 booker，必填）。</summary>
    [JsonPropertyName("booker")]
    public string BookerUserId { get; set; } = string.Empty;

    /// <summary>获取或设置参与人的成员账号列表（官方 attendees）。</summary>
    [JsonPropertyName("attendees")]
    public List<string>? AttendeeUserIdList { get; set; }
}
