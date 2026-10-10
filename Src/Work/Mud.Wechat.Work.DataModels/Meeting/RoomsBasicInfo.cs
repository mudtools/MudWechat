// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// Rooms 会议室基本信息对象（获取 Rooms 会议室详情响应 <c>basic_info</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class RoomsBasicInfo
{
    /// <summary>获取或设置 Rooms 会议室 ID 列表。</summary>
    [JsonPropertyName("rooms_id_list")]
    public List<string>? RoomsIdList { get; set; }

    /// <summary>获取或设置 Rooms 会议室名称。</summary>
    [JsonPropertyName("meeting_room_name")]
    public string? MeetingRoomName { get; set; }

    /// <summary>获取或设置城市。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>获取或设置建筑。</summary>
    [JsonPropertyName("building")]
    public string? Building { get; set; }

    /// <summary>获取或设置楼层。</summary>
    [JsonPropertyName("floor")]
    public string? Floor { get; set; }

    /// <summary>获取或设置容纳人数。</summary>
    [JsonPropertyName("participant_number")]
    public int? ParticipantNumber { get; set; }

    /// <summary>获取或设置 Rooms 会议室设备。</summary>
    [JsonPropertyName("device")]
    public string? Device { get; set; }

    /// <summary>获取或设置描述（base64 编码）。</summary>
    [JsonPropertyName("desc")]
    public string? Desc { get; set; }

    /// <summary>获取或设置管理员密码（base64 编码）。</summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
