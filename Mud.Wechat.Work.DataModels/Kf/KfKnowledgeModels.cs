// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 知识库问题文本载体（question / similar_questions.items 共用的 text 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeText
{
    /// <summary>
    /// 获取或设置文本内容（问题与相似问题不多于 200 个字）。
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}

/// <summary>
/// 知识库问答的问题（<c>question.text.content</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeQuestion
{
    /// <summary>
    /// 获取或设置问题文本（官方必填，不多于 200 个字）。
    /// </summary>
    [JsonPropertyName("text")]
    public KfKnowledgeText? Text { get; set; }
}

/// <summary>
/// 知识库问答的相似问题集合（<c>similar_questions.items</c>，最多 100 个）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeSimilarQuestions
{
    /// <summary>
    /// 获取或设置相似问题列表（最多 100 个，每项不多于 200 个字）。
    /// </summary>
    [JsonPropertyName("items")]
    public List<KfKnowledgeText>? Items { get; set; }
}

/// <summary>
/// 知识库问答的答案（目前仅支持 1 个）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeAnswer
{
    /// <summary>
    /// 获取或设置答案文本（官方必填，不多于 500 个字）。
    /// </summary>
    [JsonPropertyName("text")]
    public KfKnowledgeText? Text { get; set; }

    /// <summary>
    /// 获取或设置答案附件列表（最多 4 个）。
    /// </summary>
    [JsonPropertyName("attachments")]
    public List<KfKnowledgeAttachment>? Attachments { get; set; }
}
