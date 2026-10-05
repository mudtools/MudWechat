// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 创建会议投票主题请求体（<c>/cgi-bin/meeting/poll/create_theme</c>；为指定的会议创建投票，支持多问题投票）。
/// </summary>
/// <remarks>
/// <para>官方限制：仅进行中的会议可以调用该接口；操作者是主持人或者会议管理员。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class CreateMeetingPollThemeRequest
{
    /// <summary>获取或设置操作者的 openid（官方必填）。</summary>
    [JsonPropertyName("operator_userid")]
    public string? OperatorUserid { get; set; }

    /// <summary>获取或设置操作者入会所用的设备 id（官方必填；取值口径同会控成员设备类型）。</summary>
    [JsonPropertyName("instance_id")]
    public int? InstanceId { get; set; }

    /// <summary>获取或设置会议 ID（官方必填）。</summary>
    [JsonPropertyName("meetingid")]
    public string? Meetingid { get; set; }

    /// <summary>获取或设置投票主题（官方必填，最多 50 个字符）。</summary>
    [JsonPropertyName("poll_topic")]
    public string? PollTopic { get; set; }

    /// <summary>获取或设置投票主题描述（官方必填，最多 100 个字符）。</summary>
    [JsonPropertyName("poll_desc")]
    public string? PollDesc { get; set; }

    /// <summary>获取或设置是否匿名：0 - 实名（默认值）；1 - 匿名。</summary>
    [JsonPropertyName("is_anony")]
    public int? IsAnony { get; set; }

    /// <summary>
    /// 获取或设置投票问题数组（官方必填，详见 <see cref="MeetingPollQuestion"/>）。
    /// <para>官方限制：每个投票支持添加 10 个问题。</para>
    /// </summary>
    [JsonPropertyName("poll_questions")]
    public List<MeetingPollQuestion>? PollQuestions { get; set; }
}
