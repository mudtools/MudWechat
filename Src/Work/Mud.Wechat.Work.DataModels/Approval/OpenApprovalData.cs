// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批流程引擎审批单当前状态（getopenapprovaldata 响应的 data 节点；字段为官方原文的 PascalCase 形态）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class OpenApprovalData
{
    /// <summary>
    /// 获取或设置审批单编号（由开发者在发起申请时自定义）。
    /// </summary>
    [JsonPropertyName("ThirdNo")]
    public string? ThirdNo { get; set; }

    /// <summary>
    /// 获取或设置审批模板id。
    /// </summary>
    [JsonPropertyName("OpenTemplateId")]
    public string? OpenTemplateId { get; set; }

    /// <summary>
    /// 获取或设置审批模板名称。
    /// </summary>
    [JsonPropertyName("OpenSpName")]
    public string? OpenSpName { get; set; }

    /// <summary>
    /// 获取或设置申请单当前审批状态：1-审批中；2-已通过；3-已驳回；4-已撤销（官方字段名为 OpenSpstatus）。
    /// </summary>
    [JsonPropertyName("OpenSpstatus")]
    public int? OpenSpStatus { get; set; }

    /// <summary>
    /// 获取或设置提交申请时间（Unix时间戳）。
    /// </summary>
    [JsonPropertyName("ApplyTime")]
    public long? ApplyTime { get; set; }

    /// <summary>
    /// 获取或设置提交者姓名（官方字段名即为 ApplyUsername）。
    /// </summary>
    [JsonPropertyName("ApplyUsername")]
    public string? ApplyUsername { get; set; }

    /// <summary>
    /// 获取或设置提交者所在部门。
    /// </summary>
    [JsonPropertyName("ApplyUserParty")]
    public string? ApplyUserParty { get; set; }

    /// <summary>
    /// 获取或设置提交者头像。
    /// </summary>
    [JsonPropertyName("ApplyUserImage")]
    public string? ApplyUserImage { get; set; }

    /// <summary>
    /// 获取或设置提交者userid。
    /// </summary>
    [JsonPropertyName("ApplyUserId")]
    public string? ApplyUserId { get; set; }

    /// <summary>
    /// 获取或设置审批流程信息（官方响应以对象包裹 ApprovalNode 数组）。
    /// </summary>
    [JsonPropertyName("ApprovalNodes")]
    public OpenApprovalNodes? ApprovalNodes { get; set; }

    /// <summary>
    /// 获取或设置抄送信息（官方响应以对象包裹 NotifyNode 数组，可能有多个抄送人）。
    /// </summary>
    [JsonPropertyName("NotifyNodes")]
    public OpenNotifyNodes? NotifyNodes { get; set; }

    /// <summary>
    /// 获取或设置当前审批节点：0-第一个审批节点；1-第二个审批节点……以此类推。
    /// </summary>
    [JsonPropertyName("ApproverStep")]
    public int? ApproverStep { get; set; }
}
