// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 创建/更新审批模板的控件配置超集（controls.config 请求侧；不同控件类型取用对应字段，各字段均可空，序列化时 null 字段不写出）。
/// </summary>
/// <remarks>
/// <para>官方限制：文本/多行文本、数字、金额、收款账户控件中 config 不需要填写；请假控件（Vacation）中 config 不需要填写。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateSettingControlConfig
{
    /// <summary>
    /// 获取或设置日期/日期+时间控件的配置。
    /// </summary>
    [JsonPropertyName("date")]
    public ApprovalTemplateDateConfig? Date { get; set; }

    /// <summary>
    /// 获取或设置单选/多选控件的配置。
    /// </summary>
    [JsonPropertyName("selector")]
    public ApprovalTemplateSettingSelectorConfig? Selector { get; set; }

    /// <summary>
    /// 获取或设置成员/部门控件的配置。
    /// </summary>
    [JsonPropertyName("contact")]
    public ApprovalTemplateContactConfig? Contact { get; set; }

    /// <summary>
    /// 获取或设置说明文字控件的配置。
    /// </summary>
    [JsonPropertyName("tips")]
    public ApprovalTipsContent? Tips { get; set; }

    /// <summary>
    /// 获取或设置附件控件的配置。
    /// </summary>
    [JsonPropertyName("file")]
    public ApprovalTemplateFileConfig? File { get; set; }

    /// <summary>
    /// 获取或设置明细控件的配置。
    /// </summary>
    [JsonPropertyName("table")]
    public ApprovalTemplateSettingTableConfig? Table { get; set; }

    /// <summary>
    /// 获取或设置假勤控件（外出/出差/加班）的配置。
    /// </summary>
    [JsonPropertyName("attendance")]
    public ApprovalTemplateSettingAttendanceConfig? Attendance { get; set; }

    /// <summary>
    /// 获取或设置位置控件的配置。
    /// </summary>
    [JsonPropertyName("location")]
    public ApprovalTemplateLocationConfig? Location { get; set; }

    /// <summary>
    /// 获取或设置关联审批单控件的配置。
    /// </summary>
    [JsonPropertyName("related_approval")]
    public ApprovalTemplateRelatedApprovalConfig? RelatedApproval { get; set; }

    /// <summary>
    /// 获取或设置时长控件的配置。
    /// </summary>
    [JsonPropertyName("date_range")]
    public ApprovalTemplateDateRangeConfig? DateRange { get; set; }
}
