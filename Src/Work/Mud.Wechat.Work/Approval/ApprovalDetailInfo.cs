// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批申请详情（getapprovaldetail 响应的 info 节点）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalDetailInfo
{
    /// <summary>
    /// 获取或设置审批编号。
    /// </summary>
    [JsonPropertyName("sp_no")]
    public string? SpNo { get; set; }

    /// <summary>
    /// 获取或设置审批申请类型名称（审批模板名称）。
    /// </summary>
    [JsonPropertyName("sp_name")]
    public string? SpName { get; set; }

    /// <summary>
    /// 获取或设置申请单状态：1-审批中；2-已通过；3-已驳回；4-已撤销；6-通过后撤销；7-已删除；10-已支付。
    /// </summary>
    [JsonPropertyName("sp_status")]
    public int? SpStatus { get; set; }

    /// <summary>
    /// 获取或设置审批模板id。
    /// </summary>
    [JsonPropertyName("template_id")]
    public string? TemplateId { get; set; }

    /// <summary>
    /// 获取或设置审批申请提交时间（Unix时间戳）。
    /// </summary>
    [JsonPropertyName("apply_time")]
    public long? ApplyTime { get; set; }

    /// <summary>
    /// 获取或设置申请人信息。
    /// </summary>
    [JsonPropertyName("applyer")]
    public ApprovalApplyer? Applyer { get; set; }

    /// <summary>
    /// 获取或设置批量申请人信息（和 applyer 字段互斥）。
    /// </summary>
    [JsonPropertyName("batch_applyer")]
    public List<ApprovalUserRef>? BatchApplyer { get; set; }

    /// <summary>
    /// 获取或设置审批流程信息，可能有多个审批节点。
    /// </summary>
    [JsonPropertyName("sp_record")]
    public List<ApprovalSpRecord>? SpRecord { get; set; }

    /// <summary>
    /// 获取或设置抄送信息（官方字段名即为 notifyer），可能有多个抄送节点。
    /// </summary>
    [JsonPropertyName("notifyer")]
    public List<ApprovalUserRef>? Notifyer { get; set; }

    /// <summary>
    /// 获取或设置审批申请数据。
    /// </summary>
    [JsonPropertyName("apply_data")]
    public ApprovalApplyData? ApplyData { get; set; }

    /// <summary>
    /// 获取或设置审批申请备注信息，可能有多个备注节点。
    /// </summary>
    [JsonPropertyName("comments")]
    public List<ApprovalComment>? Comments { get; set; }

    /// <summary>
    /// 获取或设置审批流程列表。
    /// </summary>
    [JsonPropertyName("process_list")]
    public ApprovalProcessListInfo? ProcessList { get; set; }
}
