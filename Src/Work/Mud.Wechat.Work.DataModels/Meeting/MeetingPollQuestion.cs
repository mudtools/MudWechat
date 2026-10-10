// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议投票问题对象（创建/修改会议投票主题请求 <c>poll_questions</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingPollQuestion
{
    /// <summary>获取或设置问题描述（官方必填，最多 50 个字符）。</summary>
    [JsonPropertyName("question_desc")]
    public string? QuestionDesc { get; set; }

    /// <summary>获取或设置问题选择类型（官方必填）：0 - 单选；1 - 多选。</summary>
    [JsonPropertyName("question_type")]
    public int? QuestionType { get; set; }

    /// <summary>
    /// 获取或设置问题选项列表（官方必填）。
    /// <para>官方限制：每个问题支持添加 10 个选项（创建投票主题文档页要求最少 2 个选项，修改文档页要求最少 1 个选项）；每个选项最多支持 36 个字符。</para>
    /// </summary>
    [JsonPropertyName("poll_option")]
    public List<string>? PollOption { get; set; }
}
