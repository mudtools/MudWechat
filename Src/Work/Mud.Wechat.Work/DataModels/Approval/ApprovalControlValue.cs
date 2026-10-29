// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批控件值超集结构（value），不同控件类型的赋值参数不同，按控件类型取用对应字段；官方获取审批申请详情的全字段响应示例亦按超集形态返回。各字段均为可空，序列化时 null 字段不写出。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalControlValue
{
    /// <summary>
    /// 获取或设置文本/多行文本控件内容（control 为 Text 或 Textarea 时的输入值；文本控件内容不支持包含换行符）。
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// 获取或设置数字控件内容（control 为 Number 时的输入值）。
    /// </summary>
    /// <remarks>
    /// <para>官方请求与响应示例均按字符串传输（如 "700"），故本模型以字符串承载。</para>
    /// </remarks>
    [JsonPropertyName("new_number")]
    public string? NewNumber { get; set; }

    /// <summary>
    /// 获取或设置金额控件内容（control 为 Money 时的输入值）。
    /// </summary>
    /// <remarks>
    /// <para>官方请求与响应示例均按字符串传输（如 "700"），故本模型以字符串承载。</para>
    /// </remarks>
    [JsonPropertyName("new_money")]
    public string? NewMoney { get; set; }

    /// <summary>
    /// 获取或设置日期/日期+时间控件内容（control 为 Date）。
    /// </summary>
    [JsonPropertyName("date")]
    public ApprovalDateValue? Date { get; set; }

    /// <summary>
    /// 获取或设置单选/多选控件内容（control 为 Selector；请假控件 Vacation 的请假类型选择亦复用此结构）。
    /// </summary>
    [JsonPropertyName("selector")]
    public ApprovalSelectorValue? Selector { get; set; }

    /// <summary>
    /// 获取或设置成员控件内容（control 为 Contact 且 value 参数为 members，多选模式下可能有多个）。
    /// </summary>
    [JsonPropertyName("members")]
    public List<ApprovalMemberItem>? Members { get; set; }

    /// <summary>
    /// 获取或设置部门控件内容（control 为 Contact 且 value 参数为 departments，多选模式下可能有多个）。
    /// </summary>
    [JsonPropertyName("departments")]
    public List<ApprovalDepartmentItem>? Departments { get; set; }

    /// <summary>
    /// 获取或设置说明文字控件内容（control 为 Tips；提交申请时后台自动填充、无需赋值，获取审批申请详情时返回）。
    /// </summary>
    [JsonPropertyName("new_tips")]
    public ApprovalTipsContent? NewTips { get; set; }

    /// <summary>
    /// 获取或设置附件控件内容（control 为 File，可能有多个）。
    /// </summary>
    [JsonPropertyName("files")]
    public List<ApprovalFileItem>? Files { get; set; }

    /// <summary>
    /// 获取或设置明细控件内容（control 为 Table，一个明细控件可能包含多个子明细）。
    /// </summary>
    [JsonPropertyName("children")]
    public List<ApprovalTableChild>? Children { get; set; }

    /// <summary>
    /// 获取或设置时长控件内容（control 为 DateRange）；假勤组件（Vacation/Attendance）的时间范围亦复用此结构。
    /// </summary>
    [JsonPropertyName("date_range")]
    public ApprovalDateRangeValue? DateRange { get; set; }

    /// <summary>
    /// 获取或设置请假控件内容（control 为 Vacation，即申请人在此组件内选择的请假信息）。
    /// </summary>
    [JsonPropertyName("vacation")]
    public ApprovalVacationValue? Vacation { get; set; }

    /// <summary>
    /// 获取或设置假勤控件内容（control 为 Attendance，出差/外出/加班组件）。
    /// </summary>
    [JsonPropertyName("attendance")]
    public ApprovalAttendanceValue? Attendance { get; set; }

    /// <summary>
    /// 获取或设置补卡控件内容（control 为 PunchCorrection）。
    /// </summary>
    /// <remarks>
    /// <para>补卡控件仅在「获取审批申请详情」的官方附录中出现，提交审批申请的官方附录未包含该控件。</para>
    /// </remarks>
    [JsonPropertyName("punch_correction")]
    public ApprovalPunchCorrectionValue? PunchCorrection { get; set; }

    /// <summary>
    /// 获取或设置位置控件内容（control 为 Location）。
    /// </summary>
    [JsonPropertyName("location")]
    public ApprovalLocationValue? Location { get; set; }

    /// <summary>
    /// 获取或设置关联审批单控件内容（control 为 RelatedApproval）。
    /// </summary>
    [JsonPropertyName("related_approval")]
    public List<ApprovalRelatedApprovalItem>? RelatedApproval { get; set; }

    /// <summary>
    /// 获取或设置公式控件内容（control 为 Formula）。
    /// </summary>
    [JsonPropertyName("formula")]
    public ApprovalFormulaValue? Formula { get; set; }

    /// <summary>
    /// 获取或设置收款账户控件内容（control 为 BankAccount）。
    /// </summary>
    /// <remarks>
    /// <para>收款账户控件仅在「获取审批申请详情」的官方附录中出现，提交审批申请的官方附录未包含该控件。</para>
    /// </remarks>
    [JsonPropertyName("bank_account")]
    public ApprovalBankAccountValue? BankAccount { get; set; }
}
