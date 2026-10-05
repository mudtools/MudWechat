// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议受邀成员列表响应体（<c>/cgi-bin/meeting/get_invitees</c>）。
/// </summary>
/// <remarks>
/// <para>受邀成员即预约会议时添加到参会人名单的成员；仅返回在应用可见范围内的成员。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingInviteesResponse : WechatWorkResponse
{
    /// <summary>获取或设置是否还有尚未拉取的成员列表。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置分页游标（<c>has_more</c> 为 true 时，下次查询时作为 <c>cursor</c> 请求参数的值）。</summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置受邀成员列表（详见 <see cref="MeetingInvitee"/>）。</summary>
    [JsonPropertyName("invitees")]
    public List<MeetingInvitee>? Invitees { get; set; }
}
