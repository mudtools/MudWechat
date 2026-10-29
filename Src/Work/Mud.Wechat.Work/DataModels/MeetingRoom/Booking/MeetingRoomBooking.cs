// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 会议室预定条目（<c>/cgi-bin/oa/meetingroom/get_booking_info</c> 返回的 booking_list 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomBooking
{
    /// <summary>获取或设置会议室 ID（官方 meetingroom_id）。</summary>
    [JsonPropertyName("meetingroom_id")]
    public int? MeetingRoomId { get; set; }

    /// <summary>获取或设置排期列表（官方 schedule，容器名为单数而形态是数组，字段面见 <see cref="MeetingRoomSchedule"/>）。</summary>
    [JsonPropertyName("schedule")]
    public List<MeetingRoomSchedule>? ScheduleList { get; set; }
}
