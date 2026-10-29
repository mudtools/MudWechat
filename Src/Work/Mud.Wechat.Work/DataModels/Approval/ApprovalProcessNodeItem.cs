// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 提交审批申请的流程节点（process.node_list 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class ApprovalProcessNodeItem
{
    /// <summary>
    /// 获取或设置节点类型：1-审批人；2-抄送人；3-办理人。
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    /// 获取或设置多人审批方式：1-会签；2-或签；3-依次审批（type 为 1、3 时必填）。
    /// </summary>
    [JsonPropertyName("apv_rel")]
    public int? ApvRel { get; set; }

    /// <summary>
    /// 获取或设置用户id列表。
    /// </summary>
    [JsonPropertyName("userid")]
    public List<string>? Userid { get; set; }
}
