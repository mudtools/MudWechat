// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批流程节点（process_list.node_list 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalProcessNodeInfo
{
    /// <summary>
    /// 获取或设置节点类型：1-审批人；2-抄送人；3-办理人。
    /// </summary>
    [JsonPropertyName("node_type")]
    public int? NodeType { get; set; }

    /// <summary>
    /// 获取或设置节点状态：1-审批中；2-同意；3-驳回；4-转审；11-退回给指定审批人；12-加签；13-同意并加签；14-办理；15-转交。
    /// </summary>
    [JsonPropertyName("sp_status")]
    public int? SpStatus { get; set; }

    /// <summary>
    /// 获取或设置多人办理方式：1-会签；2-或签；3-依次审批。
    /// </summary>
    [JsonPropertyName("apv_rel")]
    public int? ApvRel { get; set; }

    /// <summary>
    /// 获取或设置子节点列表。
    /// </summary>
    [JsonPropertyName("sub_node_list")]
    public List<ApprovalSubNodeInfo>? SubNodeList { get; set; }
}
