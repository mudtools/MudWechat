// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Approval;

/// <summary>
/// 审批流程节点（OpenApprovalData.ApprovalNodes.ApprovalNode 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Approval")]
public class OpenApprovalNode
{
    /// <summary>
    /// 获取或设置节点审批操作状态：1-审批中；2-已同意；3-已驳回；4-已转审。
    /// </summary>
    [JsonPropertyName("NodeStatus")]
    public int? NodeStatus { get; set; }

    /// <summary>
    /// 获取或设置审批节点属性：1-或签；2-会签。
    /// </summary>
    [JsonPropertyName("NodeAttr")]
    public int? NodeAttr { get; set; }

    /// <summary>
    /// 获取或设置审批节点类型：1-固定成员；2-标签；3-上级。
    /// </summary>
    [JsonPropertyName("NodeType")]
    public int? NodeType { get; set; }

    /// <summary>
    /// 获取或设置审批节点信息（当节点为标签或上级时，一个节点可能有多个分支；官方响应以对象包裹 Item 数组）。
    /// </summary>
    [JsonPropertyName("Items")]
    public OpenApprovalNodeItems? Items { get; set; }
}
