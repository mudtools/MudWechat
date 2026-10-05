// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 删除会议投票请求体（<c>/cgi-bin/meeting/poll/delete</c>）。
/// </summary>
/// <remarks>
/// <para>官方限制：仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class DeleteMeetingPollRequest
{
    /// <summary>获取或设置操作者的 openid（官方必填）。</summary>
    [JsonPropertyName("operator_userid")]
    public string? OperatorUserid { get; set; }

    /// <summary>获取或设置操作者入会的设备 id（官方必填；取值口径同会控成员设备类型）。</summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>
    /// 获取或设置投票主题 ID（传入则代表删除投票主题，删除投票主题不影响投票实例）。
    /// <para>官方限制：投票主题 ID 和投票 ID 二选一，如果都传入，会使用投票 ID。</para>
    /// </summary>
    [JsonPropertyName("poll_theme_id")]
    public string? PollThemeId { get; set; }

    /// <summary>
    /// 获取或设置投票 ID（传入则代表删除投票实例；当主题下所有投票实例被删，则投票主题也被删除）。
    /// <para>官方限制：投票主题 ID 和投票 ID 二选一，如果都传入，会使用投票 ID。</para>
    /// </summary>
    [JsonPropertyName("poll_id")]
    public string? PollId { get; set; }
}
