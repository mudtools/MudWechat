// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.MeetingRoom;

/// <summary>
/// 按会议预定会议室请求体（<c>/cgi-bin/oa/meetingroom/book_by_meeting</c>）。
/// <para>
/// 官方契约陷阱：会议 ID 的字段名在本端点为 <c>meetingid</c>（无下划线），与同域其余端点的
/// <c>meeting_id</c> 不同名，本类型照官方原文映射（见守卫 MR5），不做统一化「修正」。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MeetingRoom")]
public class MeetingRoomBookByMeetingRequest
{
    /// <summary>获取或设置会议室 ID（官方 meetingroom_id，必填）。</summary>
    [JsonPropertyName("meetingroom_id")]
    public int MeetingRoomId { get; set; }

    /// <summary>获取或设置会议 ID（官方 <c>meetingid</c>，无下划线，必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string MeetingId { get; set; } = string.Empty;

    /// <summary>获取或设置预定者的成员账号（官方 booker，必填）。</summary>
    [JsonPropertyName("booker")]
    public string BookerUserId { get; set; } = string.Empty;
}
