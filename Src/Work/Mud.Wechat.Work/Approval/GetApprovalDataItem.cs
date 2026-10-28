// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 获取审批数据（旧）的审批记录（data 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class GetApprovalDataItem
{
    /// <summary>
    /// 获取或设置审批名称（请假、报销、自定义审批名称；官方字段名即为 spname）。
    /// </summary>
    [JsonPropertyName("spname")]
    public string? Spname { get; set; }

    /// <summary>
    /// 获取或设置申请人姓名。
    /// </summary>
    [JsonPropertyName("apply_name")]
    public string? ApplyName { get; set; }

    /// <summary>
    /// 获取或设置申请人部门。
    /// </summary>
    [JsonPropertyName("apply_org")]
    public string? ApplyOrg { get; set; }

    /// <summary>
    /// 获取或设置审批人姓名列表。
    /// </summary>
    [JsonPropertyName("approval_name")]
    public List<string>? ApprovalName { get; set; }

    /// <summary>
    /// 获取或设置抄送人姓名列表。
    /// </summary>
    [JsonPropertyName("notify_name")]
    public List<string>? NotifyName { get; set; }

    /// <summary>
    /// 获取或设置审批状态：1-审批中；2-已通过；3-已驳回；4-已取消；6-通过后撤销；10-已支付。
    /// </summary>
    [JsonPropertyName("sp_status")]
    public int? SpStatus { get; set; }

    /// <summary>
    /// 获取或设置审批单号。
    /// </summary>
    [JsonPropertyName("sp_num")]
    public long? SpNum { get; set; }

    /// <summary>
    /// 获取或设置审批的附件media_id列表（可使用 media/get 获取附件；官方字段名即为 mediaids）。
    /// </summary>
    [JsonPropertyName("mediaids")]
    public List<string>? Mediaids { get; set; }

    /// <summary>
    /// 获取或设置审批单提交时间（Unix时间戳）。
    /// </summary>
    [JsonPropertyName("apply_time")]
    public long? ApplyTime { get; set; }

    /// <summary>
    /// 获取或设置审批单提交者的userid。
    /// </summary>
    [JsonPropertyName("apply_user_id")]
    public string? ApplyUserId { get; set; }

    /// <summary>
    /// 获取或设置报销类型数据（只有报销模板的审批记录有此数据项）。
    /// </summary>
    [JsonPropertyName("expense")]
    public ApprovalExpenseData? Expense { get; set; }

    /// <summary>
    /// 获取或设置请假类型数据（只有请假模板审批记录有此数据项）。
    /// </summary>
    [JsonPropertyName("leave")]
    public ApprovalLeaveData? Leave { get; set; }

    /// <summary>
    /// 获取或设置审批模板信息（自定义模板审批记录的数据项）。
    /// </summary>
    [JsonPropertyName("comm")]
    public ApprovalCommData? Comm { get; set; }
}
