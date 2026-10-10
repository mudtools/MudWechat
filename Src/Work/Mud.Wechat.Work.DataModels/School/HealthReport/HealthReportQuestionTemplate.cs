// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.HealthReport;

/// <summary>
/// 健康上报问题模板（<c>job_info.question_templates</c> 元素结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HealthReport")]
public class HealthReportQuestionTemplate
{
    /// <summary>
    /// 获取或设置问题的 question_id。
    /// </summary>
    [JsonPropertyName("question_id")]
    public long? QuestionId { get; set; }

    /// <summary>
    /// 获取或设置问题的标题。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置问题类型
    /// （1：填空题（含地理位置和日期）；2：单选题；3：多选题；4：收集文件/图片题；
    /// 5：签名链接；6：行程卡截图）。
    /// </summary>
    [JsonPropertyName("question_type")]
    public int? QuestionType { get; set; }

    /// <summary>
    /// 获取或设置是否必填（1：是；0：否）。
    /// </summary>
    [JsonPropertyName("is_required")]
    public int? IsRequired { get; set; }

    /// <summary>
    /// 获取或设置选项列表（仅单选题与多选题返回该字段）。
    /// </summary>
    [JsonPropertyName("option_list")]
    public List<HealthReportQuestionOption>? OptionList { get; set; }
}
