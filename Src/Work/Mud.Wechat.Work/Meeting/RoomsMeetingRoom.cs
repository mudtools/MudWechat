// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室对象（预定 Rooms 会议室响应与获取 Rooms 会议室列表响应 <c>meeting_room_list</c> 嵌套对象）。
/// </summary>
/// <remarks>
/// <para>预定 Rooms 会议室文档页的 MeetingRoom 仅含 id/名称/地址三个基础字段，获取列表文档页在此基础上扩展账号/状态等字段，
/// 本模型以字段超集承载（预定响应中扩展字段为空）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsMeetingRoom
{
    /// <summary>获取或设置 Rooms 会议室 ID。</summary>
    [JsonPropertyName("meeting_room_id")]
    public string? MeetingRoomId { get; set; }

    /// <summary>获取或设置 Rooms 会议室名称。</summary>
    [JsonPropertyName("meeting_room_name")]
    public string? MeetingRoomName { get; set; }

    /// <summary>获取或设置 Rooms 会议室地址。</summary>
    [JsonPropertyName("meeting_room_location")]
    public string? MeetingRoomLocation { get; set; }

    /// <summary>获取或设置账号类型：0 - 普通；1 - 专款；2 - 试用。</summary>
    [JsonPropertyName("account_type")]
    public int? AccountType { get; set; }

    /// <summary>获取或设置激活码。</summary>
    [JsonPropertyName("active_code")]
    public string? ActiveCode { get; set; }

    /// <summary>获取或设置容纳人数。</summary>
    [JsonPropertyName("participant_number")]
    public int? ParticipantNumber { get; set; }

    /// <summary>获取或设置 Rooms 会议室状态：0 - 未激活；1 - 未绑定；2 - 空闲；3 - 使用中；4 - 离线。</summary>
    [JsonPropertyName("meeting_room_status")]
    public int? MeetingRoomStatus { get; set; }

    /// <summary>获取或设置预定状态：0 - 未开放预定；1 - 开放预定。</summary>
    [JsonPropertyName("scheduled_status")]
    public int? ScheduledStatus { get; set; }

    /// <summary>获取或设置是否允许被呼叫：true - 是；false - 否。</summary>
    [JsonPropertyName("is_allow_call")]
    public bool? IsAllowCall { get; set; }
}
