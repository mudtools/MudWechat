// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.HealthReport;

/// <summary>
/// 用户填写的单题答案（<c>answers.report_values</c> 元素结构）。
/// <para>
/// 官方按问题类型仅返回其中一组字段：单选题返回 <see cref="SingleChoice"/>、
/// 填空题返回 <see cref="Text"/>、多选题返回 <see cref="MultiChoice"/>、
/// 收集文件/图片题返回 <see cref="FileId"/>、签名题返回 <see cref="Url"/>、
/// 行程卡截图题返回 <see cref="ItineraryCardType"/> 与 <see cref="HighRiskArea"/>。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HealthReport")]
public class HealthReportAnswerValue
{
    /// <summary>
    /// 获取或设置问题的 id。
    /// </summary>
    [JsonPropertyName("question_id")]
    public long? QuestionId { get; set; }

    /// <summary>
    /// 获取或设置单选题答案编号（仅单选题返回）。
    /// </summary>
    [JsonPropertyName("single_choice")]
    public int? SingleChoice { get; set; }

    /// <summary>
    /// 获取或设置填空题答案内容（仅填空题返回）。
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 获取或设置多选题答案编号列表（仅多选题返回）。
    /// </summary>
    [JsonPropertyName("multi_choice")]
    public List<long>? MultiChoice { get; set; }

    /// <summary>
    /// 获取或设置文件 id 列表，可以用下载文件接口下载（仅收集文件/图片题返回）。
    /// </summary>
    [JsonPropertyName("fileid")]
    public List<string>? FileId { get; set; }

    /// <summary>
    /// 获取或设置签名的 url（目前仅签名类型的问题会返回该字段）。
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    /// 获取或设置行程卡类型（仅行程卡截图类型的问题返回）
    /// （0：行程码状态识别失败；1：绿码；2：红码；3：黄码；4：橙码；5：未识别出行程卡）。
    /// </summary>
    [JsonPropertyName("itinerary_card_type")]
    public int? ItineraryCardType { get; set; }

    /// <summary>
    /// 获取或设置高风险行程信息（仅行程卡截图类型的问题返回）。
    /// <para>官方文档仅描述为「高风险行程信息」，未标注字段的具体 JSON 结构。</para>
    /// </summary>
    [JsonPropertyName("high_risk_area")]
    public List<string>? HighRiskArea { get; set; }
}
