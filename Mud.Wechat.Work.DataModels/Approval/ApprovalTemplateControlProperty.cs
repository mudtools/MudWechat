// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批模板控件属性（controls.property）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalTemplateControlProperty
{
    /// <summary>
    /// 获取或设置控件类型：Text-文本；Textarea-多行文本；Number-数字；Money-金额；Date-日期/日期+时间；Selector-单选/多选；Contact-成员/部门；Tips-说明文字；File-附件；Table-明细；Attendance-假勤控件；Vacation-请假控件；Location-位置；RelatedApproval-关联审批单；Formula-公式；DateRange-时长；BankAccount-收款账户。
    /// </summary>
    [JsonPropertyName("control")]
    public string? Control { get; set; }

    /// <summary>
    /// 获取或设置控件id（较早时间创建的模板控件id为数字串形态）。
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// 获取或设置控件名称（多语言，默认为 zh_CN 中文）。
    /// </summary>
    [JsonPropertyName("title")]
    public List<ApprovalLangTextItem>? Title { get; set; }

    /// <summary>
    /// 获取或设置控件说明（向申请者展示的控件填写说明，多语言，默认为 zh_CN 中文）。
    /// </summary>
    [JsonPropertyName("placeholder")]
    public List<ApprovalLangTextItem>? Placeholder { get; set; }

    /// <summary>
    /// 获取或设置是否必填：1-必填；0-非必填。
    /// </summary>
    [JsonPropertyName("require")]
    public int? Require { get; set; }

    /// <summary>
    /// 获取或设置是否参与打印：1-不参与打印；0-参与打印（官方字段名为 un_print）。
    /// </summary>
    [JsonPropertyName("un_print")]
    public int? UnPrint { get; set; }
}
