namespace Mud.Wechat.Work.DataModels;

/// <summary>
/// <para>表示 [POST] /setautoreply/{TOKEN} 接口的请求。</para>
/// </summary>
public class SetAutoReplyRequest
{
    /// <summary>
    /// 获取或设置管理员 ID。
    /// </summary>
    [JsonPropertyName("managerid")]
    public string ManagetId { get; set; } = string.Empty;

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
    /// 获取或设置自动回答的内容。
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置相似问题列表。
    /// </summary>
    [JsonPropertyName("list")]
    public QuestionList QuestionList { get; set; } = new QuestionList();
}

public class QuestionList
{
    [JsonPropertyName("question")]
    public IList<string> Items { get; set; } = new List<string>();
}