// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批模板控件配置超集（controls.config，获取审批模板详情响应侧；不同控件类型取用对应字段，各字段均可空，序列化时 null 字段不写出）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateControlConfig
{
    /// <summary>
    /// 获取或设置日期/日期+时间控件的配置（control 为 Date 时包含此参数）。
    /// </summary>
    [JsonPropertyName("date")]
    public ApprovalTemplateDateConfig? Date { get; set; }

    /// <summary>
    /// 获取或设置单选/多选控件的配置（control 为 Selector 时包含此参数）。
    /// </summary>
    [JsonPropertyName("selector")]
    public ApprovalTemplateSelectorConfig? Selector { get; set; }

    /// <summary>
    /// 获取或设置成员/部门控件的配置（control 为 Contact 时包含此参数）。
    /// </summary>
    [JsonPropertyName("contact")]
    public ApprovalTemplateContactConfig? Contact { get; set; }

    /// <summary>
    /// 获取或设置明细控件的配置（control 为 Table 时包含此参数）。
    /// </summary>
    [JsonPropertyName("table")]
    public ApprovalTemplateTableConfig? Table { get; set; }

    /// <summary>
    /// 获取或设置假勤控件的配置（control 为 Attendance 时包含此参数，【出差】【加班】【外出】模板特有）。
    /// </summary>
    [JsonPropertyName("attendance")]
    public ApprovalTemplateAttendanceConfig? Attendance { get; set; }

    /// <summary>
    /// 获取或设置假期类型数组（control 为 Vacation 时包含此参数，【请假】模板特有）。
    /// </summary>
    [JsonPropertyName("vacation_list")]
    public ApprovalVacationListConfig? VacationList { get; set; }

    /// <summary>
    /// 获取或设置说明文字控件的配置（control 为 Tips 时包含此参数）。
    /// </summary>
    [JsonPropertyName("tips")]
    public ApprovalTipsContent? Tips { get; set; }

    /// <summary>
    /// 获取或设置附件控件的配置（control 为 File 时包含此参数）。
    /// </summary>
    [JsonPropertyName("file")]
    public ApprovalTemplateFileConfig? File { get; set; }

    /// <summary>
    /// 获取或设置位置控件的配置（control 为 Location 时包含此参数）。
    /// </summary>
    [JsonPropertyName("location")]
    public ApprovalTemplateLocationConfig? Location { get; set; }

    /// <summary>
    /// 获取或设置关联审批单控件的配置（control 为 RelatedApproval 时包含此参数）。
    /// </summary>
    [JsonPropertyName("related_approval")]
    public ApprovalTemplateRelatedApprovalConfig? RelatedApproval { get; set; }

    /// <summary>
    /// 获取或设置时长控件的配置（control 为 DateRange 时包含此参数）。
    /// </summary>
    [JsonPropertyName("date_range")]
    public ApprovalTemplateDateRangeConfig? DateRange { get; set; }
}
