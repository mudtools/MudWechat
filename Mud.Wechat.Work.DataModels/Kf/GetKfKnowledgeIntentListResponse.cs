// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 知识库获取问答列表响应体（<c>/cgi-bin/kf/knowledge/list_intent</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfKnowledgeIntentListResponse : WechatWorkResponse
{
    /// <summary>
    /// 获取或设置分页游标（has_more = 1 时用于拉取下一页）。
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; set; }

    /// <summary>
    /// 获取或设置是否还有更多数据：0 - 否，1 - 是。
    /// </summary>
    [JsonPropertyName("has_more")]
    public int? HasMore { get; set; }

    /// <summary>
    /// 获取或设置问答列表。
    /// </summary>
    [JsonPropertyName("intent_list")]
    public List<KfKnowledgeIntent>? IntentList { get; set; }
}

/// <summary>
/// 知识库问答信息（<see cref="GetKfKnowledgeIntentListResponse.IntentList"/> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class KfKnowledgeIntent
{
    /// <summary>
    /// 获取或设置问答所属的分组 ID。
    /// </summary>
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; }

    /// <summary>
    /// 获取或设置问答 ID。
    /// </summary>
    [JsonPropertyName("intent_id")]
    public string? IntentId { get; set; }

    /// <summary>
    /// 获取或设置问题。
    /// </summary>
    [JsonPropertyName("question")]
    public KfKnowledgeQuestion? Question { get; set; }

    /// <summary>
    /// 获取或设置相似问题。
    /// </summary>
    [JsonPropertyName("similar_questions")]
    public KfKnowledgeSimilarQuestions? SimilarQuestions { get; set; }

    /// <summary>
    /// 获取或设置答案列表。
    /// </summary>
    [JsonPropertyName("answers")]
    public List<KfKnowledgeAnswer>? Answers { get; set; }
}
