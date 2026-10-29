// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 获取会议室预定详情响应体（<c>/cgi-bin/oa/meetingroom/bookinfo/get</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomBookInfoGetResponse : WechatWorkResponse
{
    /// <summary>获取或设置会议室 ID（官方 meetingroom_id）。</summary>
    [JsonPropertyName("meetingroom_id")]
    public int? MeetingRoomId { get; set; }

    /// <summary>获取或设置预定信息（官方 schedule，单对象形态，比查询排期多回传 booking_id / master_booking_id）。</summary>
    [JsonPropertyName("schedule")]
    public MeetingRoomSchedule? Schedule { get; set; }
}
