using System.Text.Json.Serialization;

namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// <para>表示 [POST] /batchimportskill/{TOKEN} 接口的请求。</para>
/// </summary>
public class BatchImportSkillRequest
{

    /// <summary>
    /// 获取或设置管理员 ID。
    /// </summary>
    [JsonPropertyName("managerid")]
    public string ManagetId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置技能列表。
    /// </summary>
    [JsonPropertyName("skill")]
    public IList<Skill> SkillList { get; set; } = new List<Skill>();
}

/// <summary>
/// 技能
/// </summary>
public class Skill
{
    /// <summary>
    /// 获取或设置技能名称。
    /// </summary>
    [JsonPropertyName("skillname")]
    public string SkillName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置标准问题。
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置相似问题列表。
    /// </summary>
    [JsonPropertyName("question")]
    public IList<string> QuestionList { get; set; } = new List<string>();

    /// <summary>
    /// 获取或设置机器人回答列表。
    /// </summary>
    [JsonPropertyName("answer")]
    public IList<string> AnswerList { get; set; } = new List<string>();
}