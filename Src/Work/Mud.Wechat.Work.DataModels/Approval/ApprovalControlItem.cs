// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批申请单控件项（contents 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalControlItem
{
    /// <summary>
    /// 获取或设置控件类型：Text-文本；Textarea-多行文本；Number-数字；Money-金额；Date-日期/日期+时间；Selector-单选/多选；Contact-成员/部门；Tips-说明文字；File-附件；Table-明细；Attendance-假勤；Vacation-请假；PunchCorrection-补卡；Location-位置；RelatedApproval-关联审批单；Formula-公式；DateRange-时长；BankAccount-收款账户。
    /// </summary>
    [JsonPropertyName("control")]
    public string? Control { get; set; }

    /// <summary>
    /// 获取或设置控件id（控件的唯一id，可通过「获取审批模板详情」接口获取）。
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 获取或设置控件名称（多语言，若配置了多语言则会包含中英文的控件名称）；提交审批申请的请求体无此字段，仅获取审批申请详情、获取审批模板详情场景返回。
    /// </summary>
    [JsonPropertyName("title")]
    public List<ApprovalLangTextItem>? Title { get; set; }

    /// <summary>
    /// 获取或设置控件值，包含了申请人在各类型控件中输入的值，不同控件有不同的赋值参数（超集结构，按控件类型取用对应字段）。
    /// </summary>
    [JsonPropertyName("value")]
    public ApprovalControlValue? Value { get; set; }

    /// <summary>
    /// 获取或设置控件隐藏标识，为 1 表示控件被隐藏（仅获取审批申请详情场景返回）。
    /// </summary>
    [JsonPropertyName("hidden")]
    public int? Hidden { get; set; }
}
