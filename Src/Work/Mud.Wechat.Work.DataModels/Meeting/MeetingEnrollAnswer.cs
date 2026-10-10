// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Meeting;

/// <summary>
/// 会议报名答题对象（获取会议报名信息响应 <c>enroll_list.answer_list</c> 嵌套对象）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Meeting")]
public class MeetingEnrollAnswer
{
    /// <summary>获取或设置回答内容（单选/简答只有一个元素，多选会有多个）。</summary>
    [JsonPropertyName("answer_content")]
    public List<string>? AnswerContent { get; set; }

    /// <summary>获取或设置是否必填：1 - 否；2 - 是。</summary>
    [JsonPropertyName("is_required")]
    public int? IsRequired { get; set; }

    /// <summary>获取或设置问题编号（1,2,3... 等形式）。</summary>
    [JsonPropertyName("question_num")]
    public int? QuestionNum { get; set; }

    /// <summary>获取或设置问题标题。</summary>
    [JsonPropertyName("question_title")]
    public string? QuestionTitle { get; set; }

    /// <summary>获取或设置问题类型：1 - 单选；2 - 多选；3 - 简答。</summary>
    [JsonPropertyName("question_type")]
    public int? QuestionType { get; set; }

    /// <summary>
    /// 获取或设置特殊问题类型：1 - 无；2 - 手机号；3 - 邮箱；4 - 姓名；5 - 公司名称。
    /// <para>官方限制：目前特殊问题均为简答题。</para>
    /// </summary>
    [JsonPropertyName("special_type")]
    public int? SpecialType { get; set; }
}
