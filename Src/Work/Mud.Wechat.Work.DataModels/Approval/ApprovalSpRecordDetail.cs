// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批节点分支详情（sp_record.details 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalSpRecordDetail
{
    /// <summary>
    /// 获取或设置分支审批人。
    /// </summary>
    [JsonPropertyName("approver")]
    public ApprovalUserRef? Approver { get; set; }

    /// <summary>
    /// 获取或设置审批意见。
    /// </summary>
    [JsonPropertyName("speech")]
    public string? Speech { get; set; }

    /// <summary>
    /// 获取或设置分支审批人审批状态：1-审批中；2-已同意；3-已驳回；4-已转审；11-已退回；12-已加签；13-已同意并加签。
    /// </summary>
    [JsonPropertyName("sp_status")]
    public int? SpStatus { get; set; }

    /// <summary>
    /// 获取或设置节点分支审批人审批操作时间戳，0 表示未操作。
    /// </summary>
    [JsonPropertyName("sptime")]
    public long? Sptime { get; set; }

    /// <summary>
    /// 获取或设置节点分支审批人审批意见附件（微盘文件无法获取，media_id 具体使用请参考「文档-获取临时素材」）。
    /// </summary>
    [JsonPropertyName("media_id")]
    public List<string>? MediaId { get; set; }
}
