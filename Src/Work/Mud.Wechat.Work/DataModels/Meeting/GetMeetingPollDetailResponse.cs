// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 获取会议投票详情响应体（<c>/cgi-bin/meeting/poll/get_poll_detail</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class GetMeetingPollDetailResponse : WechatWorkResponse
{
    /// <summary>获取或设置投票主题 id。</summary>
    [JsonPropertyName("poll_theme_id")]
    public string? PollThemeId { get; set; }

    /// <summary>获取或设置投票主题。</summary>
    [JsonPropertyName("poll_topic")]
    public string? PollTopic { get; set; }

    /// <summary>获取或设置投票描述。</summary>
    [JsonPropertyName("poll_desc")]
    public string? PollDesc { get; set; }

    /// <summary>获取或设置是否匿名：0 - 实名；1 - 匿名。</summary>
    [JsonPropertyName("is_anony")]
    public int? IsAnony { get; set; }

    /// <summary>获取或设置投票状态：1 - 投票中；2 - 已结束。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置是否共享。</summary>
    [JsonPropertyName("is_shared")]
    public int? IsShared { get; set; }

    /// <summary>获取或设置投票人数。</summary>
    [JsonPropertyName("vote_total_num")]
    public int? VoteTotalNum { get; set; }

    /// <summary>获取或设置投票结果数组（详见 <see cref="MeetingPollDetailQuestion"/>）。</summary>
    [JsonPropertyName("poll_question_data")]
    public List<MeetingPollDetailQuestion>? PollQuestionData { get; set; }
}
