// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议投票详情选项对象（获取会议投票详情响应 <c>poll_question_data.option_info</c> 元素；含投票结果）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingPollDetailOption
{
    /// <summary>获取或设置选项 ID。</summary>
    [JsonPropertyName("option_id")]
    public int? OptionId { get; set; }

    /// <summary>获取或设置选项描述。</summary>
    [JsonPropertyName("option_desc")]
    public string? OptionDesc { get; set; }

    /// <summary>获取或设置每个选项的投票数。</summary>
    [JsonPropertyName("option_num")]
    public int? OptionNum { get; set; }

    /// <summary>获取或设置投票比率。</summary>
    [JsonPropertyName("rate")]
    public int? Rate { get; set; }

    /// <summary>获取或设置投票用户信息数组（详见 <see cref="MeetingPollOptionUser"/>）。</summary>
    [JsonPropertyName("option_user")]
    public List<MeetingPollOptionUser>? OptionUser { get; set; }
}
