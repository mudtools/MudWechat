// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取等候室成员记录响应体（<c>/cgi-bin/meeting/waitingroom/get_user_list</c>；包括等候室内所有进出过的成员，会前/会中/会后均可获取）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingWaitingRoomUserListResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否还有更多成员列表。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置分页游标（<c>has_more</c> = true 时有值，下次请求将该字段赋值给 <c>cursor</c> 字段）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置等候室成员记录列表（详见 <see cref="MeetingWaitingRoomUserRecord"/>）。</summary>
    [JsonPropertyName("user_list")]
    public List<MeetingWaitingRoomUserRecord>? UserList { get; set; }
}
