// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 按日程预定会议室响应体（<c>/cgi-bin/oa/meetingroom/book_by_schedule</c>）。
/// <para>
/// 与 <see cref="MeetingRoomBookResponse"/> 的字段面不同：本类型回传 conflict_date 而不回传 schedule_id，
/// 故按端点各持一型，不做跨端点共用。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomBookByScheduleResponse : WechatWorkResponse
{
    /// <summary>获取或设置预定 ID（官方 booking_id）。</summary>
    [JsonPropertyName("booking_id")]
    public string? BookingId { get; set; }

    /// <summary>获取或设置冲突日期的时间戳列表（官方 conflict_date，秒级）。</summary>
    [JsonPropertyName("conflict_date")]
    public List<long>? ConflictDateList { get; set; }
}
