// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 会议室预定排期（官方 schedule，字段面取两处查询端点的**并集**）。
/// <para>
/// 官方在 get_booking_info 里把 schedule 用作**数组**容器名，在 bookinfo/get 里用作**对象**，
/// 且后者额外回传 booking_id / master_booking_id。AOT 源生成按声明类型序列化、不做运行时多态，
/// 故一份可空超集覆盖两形态，消费方按调用的端点判别哪个字段位有效。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomSchedule
{
    /// <summary>获取或设置预定开始时间戳（官方 start_time，秒级）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置预定结束时间戳（官方 end_time，秒级）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>
    /// 获取或设置会议 ID（官方 meeting_id）。现行 93620 的两处 schedule 参数表均未列该字段，
    /// 保留为可空超集（取消预定请改用 <see cref="BookingId"/>，官方 cancel_book 的定位键是 booking_id）。
    /// </summary>
    [JsonPropertyName("meeting_id")]
    public string? MeetingId { get; set; }

    /// <summary>获取或设置关联日程 ID（官方 schedule_id，按日程预定时回填该值）。</summary>
    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }

    /// <summary>获取或设置预定者的成员账号（官方 booker）。</summary>
    [JsonPropertyName("booker")]
    public string? BookerUserId { get; set; }

    /// <summary>获取或设置预定状态（官方 status）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置预定 ID（官方 booking_id，仅 bookinfo/get 的 schedule 回传）。</summary>
    [JsonPropertyName("booking_id")]
    public string? BookingId { get; set; }

    /// <summary>获取或设置主预定 ID（官方 master_booking_id，仅 bookinfo/get 的 schedule 回传）。</summary>
    [JsonPropertyName("master_booking_id")]
    public string? MasterBookingId { get; set; }
}
