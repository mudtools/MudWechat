// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 取消会议室预定请求体（<c>/cgi-bin/oa/meetingroom/cancel_book</c>）。
/// <para>
/// 官方 93620 的定位键为 <c>booking_id</c>（会议室的预定 id），而非会议 ID。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomCancelBookRequest
{
    /// <summary>获取或设置会议室的预定 ID（官方 booking_id，必填）。</summary>
    [JsonPropertyName("booking_id")]
    public string BookingId { get; set; } = string.Empty;

    /// <summary>获取或设置是否保留日程（官方 keep_schedule，0-同步删除 / 1-保留，<b>仅对非重复日程有效</b>；官方以数字承载布尔语义，故以 <c>int</c> 原样下发而非序列化为 <c>true</c>/<c>false</c>）。</summary>
    [JsonPropertyName("keep_schedule")]
    public int? KeepSchedule { get; set; }

    /// <summary>获取或设置取消的对应日期（官方 cancel_date，当天 0 点时间戳）。对重复日程，不填表示取消所有重复预定，填写则仅取消对应日期当天的预定。</summary>
    [JsonPropertyName("cancel_date")]
    public long? CancelDate { get; set; }
}
