// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取成员会议 ID 列表响应体（<c>/cgi-bin/meeting/get_user_meetingid</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：只能拉取该应用创建的会议 ID。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetUserMeetingIdListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置分页游标（当前数据最后一个 key 值，下次调用带上该值则从该 key 值往后拉取）。
    /// <para>未返回或为空字符串时表示数据已取完。</para>
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>获取或设置会议 ID 列表（可能为空）。</summary>
    [JsonPropertyName("meetingid_list")]
    public List<string>? MeetingidList { get; set; }
}
