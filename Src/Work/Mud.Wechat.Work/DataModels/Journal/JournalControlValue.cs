// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Approval;

namespace Mud.Wechat.Work.DataModels.Journal;

/// <summary>
/// 汇报控件值超集结构（value），不同控件类型的赋值参数不同，按 <see cref="JournalApplyContent.Control"/> 取用对应字段。
/// <para>
/// 字段名与子结构对齐审批域的控件值 <see cref="ApprovalControlValue"/>（同一套控件体系），
/// 差异是本模型按官方汇报详情的 value 参数表建模：不含审批侧的 new_tips / bank_account，
/// 另含学生 / 班级（students / classes）、文档（docs）与微盘文件（wedrive_files）四个控件位。
/// 各字段均为可空，序列化时 null 字段不写出。
/// </para>
/// <para>
/// 官方「汇报」详情的 value 附录（附1~附17）给出的控件位为 text / new_number / new_money / date / selector /
/// members / departments / files / children / date_range / location / formula / students / classes / docs /
/// wedrive_files，另有 Tips 控件官方标注「不显示在审批详情中，故 value 为空」；
/// 本模型的 attendance / vacation / punch_correction 三个字段位来自审批侧同结构复用，官方汇报附录未列出，
/// 汇报报文按官方口径不会填值；related_approval 在官方汇报响应示例中出现（附录未单独成节），结构沿用审批侧。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Journal")]
public class JournalControlValue
{
    /// <summary>获取或设置文本控件内容（官方 text）。</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>获取或设置数字控件内容的字符串形式（官方 new_number）。</summary>
    [JsonPropertyName("new_number")]
    public string? NewNumber { get; set; }

    /// <summary>获取或设置金额控件金额的字符串形式（官方 new_money）。</summary>
    [JsonPropertyName("new_money")]
    public string? NewMoney { get; set; }

    /// <summary>获取或设置日期控件值（官方 date）。</summary>
    [JsonPropertyName("date")]
    public ApprovalDateValue? Date { get; set; }

    /// <summary>获取或设置选择控件值（官方 selector）。</summary>
    [JsonPropertyName("selector")]
    public ApprovalSelectorValue? Selector { get; set; }

    /// <summary>获取或设置成员控件的成员列表（官方 members）。</summary>
    [JsonPropertyName("members")]
    public List<ApprovalMemberItem>? Members { get; set; }

    /// <summary>获取或设置成员控件的部门列表（官方 departments）。</summary>
    [JsonPropertyName("departments")]
    public List<ApprovalDepartmentItem>? Departments { get; set; }

    /// <summary>获取或设置附件控件的文件列表（官方 files）。</summary>
    [JsonPropertyName("files")]
    public List<ApprovalFileItem>? Files { get; set; }

    /// <summary>获取或设置明细控件的子明细列表（官方 children）。</summary>
    [JsonPropertyName("children")]
    public List<ApprovalTableChild>? Children { get; set; }

    /// <summary>获取或设置假勤控件值（官方 attendance）。</summary>
    [JsonPropertyName("attendance")]
    public ApprovalAttendanceValue? Attendance { get; set; }

    /// <summary>获取或设置请假控件值（官方 vacation）。</summary>
    [JsonPropertyName("vacation")]
    public ApprovalVacationValue? Vacation { get; set; }

    /// <summary>获取或设置位置控件值（官方 location）。</summary>
    [JsonPropertyName("location")]
    public ApprovalLocationValue? Location { get; set; }

    /// <summary>获取或设置关联审批单控件值（官方 related_approval）。</summary>
    [JsonPropertyName("related_approval")]
    public List<ApprovalRelatedApprovalItem>? RelatedApproval { get; set; }

    /// <summary>获取或设置公式控件值（官方 formula）。</summary>
    [JsonPropertyName("formula")]
    public ApprovalFormulaValue? Formula { get; set; }

    /// <summary>获取或设置时长控件值（官方 date_range）。</summary>
    [JsonPropertyName("date_range")]
    public ApprovalDateRangeValue? DateRange { get; set; }

    /// <summary>获取或设置补卡控件值（官方 punch_correction）。</summary>
    [JsonPropertyName("punch_correction")]
    public ApprovalPunchCorrectionValue? PunchCorrection { get; set; }

    /// <summary>获取或设置家校联系人控件的学生列表（官方 students）。</summary>
    [JsonPropertyName("students")]
    public List<JournalSchoolContactValue>? Students { get; set; }

    /// <summary>获取或设置家校联系人控件的班级列表（官方 classes，与 students 同形）。</summary>
    [JsonPropertyName("classes")]
    public List<JournalSchoolContactValue>? Classes { get; set; }

    /// <summary>获取或设置文档控件值（官方 docs）。</summary>
    [JsonPropertyName("docs")]
    public List<JournalDocumentValue>? Documents { get; set; }

    /// <summary>获取或设置微盘文件控件值（官方 wedrive_files）。</summary>
    [JsonPropertyName("wedrive_files")]
    public List<JournalWedriveFileValue>? WedriveFiles { get; set; }
}
