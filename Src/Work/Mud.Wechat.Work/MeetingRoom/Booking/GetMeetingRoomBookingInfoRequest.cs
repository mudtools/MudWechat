// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 查询会议室预定信息请求体（<c>/cgi-bin/oa/meetingroom/get_booking_info</c>）。
/// <para>
/// meetingroom_id 与 city / building / floor 两类定位条件均为可选，start_time / end_time 限定时间窗。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class GetMeetingRoomBookingInfoRequest
{
    /// <summary>获取或设置指定会议室 ID（官方 meetingroom_id）。</summary>
    [JsonPropertyName("meetingroom_id")]
    public int? MeetingRoomId { get; set; }

    /// <summary>获取或设置查询的起始时间戳（官方 start_time，秒级）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置查询的结束时间戳（官方 end_time，秒级）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置指定城市（官方 city）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置指定楼宇（官方 building）。</summary>
    [JsonPropertyName("building")]
    public string? Building { get; set; }

    /// <summary>获取或设置指定楼层（官方 floor）。</summary>
    [JsonPropertyName("floor")]
    public string? Floor { get; set; }
}
