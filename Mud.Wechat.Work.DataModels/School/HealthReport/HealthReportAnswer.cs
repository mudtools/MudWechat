// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.School.HealthReport;

/// <summary>
/// 用户填写答案（<c>get_report_answer</c> 响应 <c>answers</c> 元素结构）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "HealthReport")]
public class HealthReportAnswer
{
    /// <summary>
    /// 获取或设置 id 类型（1：返回企业内部成员的 userid；2：返回家长和学生的 userid）。
    /// </summary>
    [JsonPropertyName("id_type")]
    public int? IdType { get; set; }

    /// <summary>
    /// 获取或设置企业内部成员的 userid（id_type 为 1 时返回）。
    /// </summary>
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    /// <summary>
    /// 获取或设置学生的 userid（id_type 为 2 时返回）。
    /// </summary>
    [JsonPropertyName("student_userid")]
    public string? StudentUserId { get; set; }

    /// <summary>
    /// 获取或设置家长的 userid（id_type 为 2 时返回）。
    /// </summary>
    [JsonPropertyName("parent_userid")]
    public string? ParentUserId { get; set; }

    /// <summary>
    /// 获取或设置用户填写时的时间戳。
    /// </summary>
    [JsonPropertyName("report_time")]
    public long? ReportTime { get; set; }

    /// <summary>
    /// 获取或设置用户填写的答案列表。
    /// </summary>
    [JsonPropertyName("report_values")]
    public List<HealthReportAnswerValue>? ReportValues { get; set; }
}
