// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 预定 Rooms 会议室请求体（<c>/cgi-bin/meeting/rooms/book</c>；对成功预定的会议添加 Rooms 会议室，支持为同一个会议预定多个 Rooms 会议室）。
/// </summary>
/// <remarks>
/// <para>官方限制：Rooms 会议室预定对会议时长有硬性要求，会议时长不得大于 24 小时，且不支持周期性会议。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class BookMeetingRoomRequest
{
    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置 Rooms 会议室 ID 列表（官方必填）。</summary>
    [JsonPropertyName("meeting_room_id_list")]
    public List<string>? MeetingRoomIdList { get; set; }

    /// <summary>
    /// 获取或设置在会议开始前的一小时内是否在 Room 上显示会议主题（默认值为 true）：true - 显示；false - 不显示。
    /// <para>官方说明：该参数并不影响预定时间晚过当前时间一个小时以上的会议，超过一小时的会议默认不显示会议主题。</para>
    /// </summary>
    [JsonPropertyName("subject_visible")]
    public bool? SubjectVisible { get; set; }
}
