// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.HealthReport;

/// <summary>
/// 健康上报任务详情（<c>get_report_job_info</c> 响应 <c>job_info</c> 结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HealthReport")]
public class HealthReportJobInfo
{
    /// <summary>
    /// 获取或设置任务名称。
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// 获取或设置发起人的 userid。
    /// </summary>
    [JsonPropertyName("creator")]
    public string? Creator { get; set; }

    /// <summary>
    /// 获取或设置任务类型（1：企业内部成员；2：家长）。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置适用范围。
    /// </summary>
    [JsonPropertyName("apply_range")]
    public HealthReportApplyRange? ApplyRange { get; set; }

    /// <summary>
    /// 获取或设置汇报对象。
    /// </summary>
    [JsonPropertyName("report_to")]
    public HealthReportReportTo? ReportTo { get; set; }

    /// <summary>
    /// 获取或设置上报方式（1：仅上报一次；2：每天都上报）。
    /// </summary>
    [JsonPropertyName("report_type")]
    public int? ReportType { get; set; }

    /// <summary>
    /// 获取或设置周末是否需要上报（0：周末需要上报；1：周末不需要上报）。
    /// </summary>
    [JsonPropertyName("skip_weekend")]
    public int? SkipWeekend { get; set; }

    /// <summary>
    /// 获取或设置已填表人数。
    /// </summary>
    [JsonPropertyName("finish_cnt")]
    public long? FinishCount { get; set; }

    /// <summary>
    /// 获取或设置健康上报问题列表。
    /// </summary>
    [JsonPropertyName("question_templates")]
    public List<HealthReportQuestionTemplate>? QuestionTemplates { get; set; }
}
