// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 知识库修改问答请求体（<c>/cgi-bin/kf/knowledge/mod_intent</c>）。
/// <para>
/// 修改为<b>整体覆盖写</b>：即便只修改部分字段，也须传入完整字段内容。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class UpdateKfKnowledgeIntentRequest
{
    /// <summary>
    /// 获取或设置问答 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("intent_id")]
    public string? IntentId { get; set; }

    /// <summary>
    /// 获取或设置问题（整体覆盖写，须传完整字段内容）。
    /// </summary>
    [JsonPropertyName("question")]
    public KfKnowledgeQuestion? Question { get; set; }

    /// <summary>
    /// 获取或设置相似问题（最多 100 个；整体覆盖写，须传完整字段内容）。
    /// </summary>
    [JsonPropertyName("similar_questions")]
    public KfKnowledgeSimilarQuestions? SimilarQuestions { get; set; }

    /// <summary>
    /// 获取或设置答案列表（目前仅支持 1 个；整体覆盖写，须传完整字段内容）。
    /// </summary>
    [JsonPropertyName("answers")]
    public List<KfKnowledgeAnswer>? Answers { get; set; }
}
