// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 审批流程节点（<c>open_approval_change</c> 报文的 <c>ApprovalInfo/ApprovalNodes/ApprovalNode</c> 节点；
/// 官方 path 90240）。
/// </summary>
/// <remarks>
/// 经上游 G-ADR-17 的 <c>ItemsObject</c> 嵌套通道声明化：本类型作为对象项被外层
/// <c>ApprovalNodes</c>（<c>ItemName = "ApprovalNode"</c>）收集，内层 <see cref="Items"/> 分支列表
/// 再经 <c>ItemsObject</c>（<c>ItemName = "Item"</c>）二级递归绑定。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackApprovalNode
{
    /// <summary>节点审批操作状态（官方 <c>NodeStatus</c>：1 审批中 / 2 已同意 / 3 已驳回 / 4 已转审）。</summary>
    [PayloadField("NodeStatus")]
    public long? NodeStatus { get; set; }

    /// <summary>审批节点属性（官方 <c>NodeAttr</c>：1 或签 / 2 会签）。</summary>
    [PayloadField("NodeAttr")]
    public long? NodeAttr { get; set; }

    /// <summary>审批节点类型（官方 <c>NodeType</c>：1 固定成员 / 2 标签 / 3 上级）。</summary>
    [PayloadField("NodeType")]
    public long? NodeType { get; set; }

    /// <summary>
    /// 审批节点分支（官方 <c>Items/Item</c>；当节点为标签或上级时一个节点可能有多个分支）。
    /// </summary>
    [PayloadField("Items", Format = PayloadFieldFormat.ItemsObject, ItemName = "Item")]
    public List<WechatCallbackApprovalItem> Items { get; set; } = new List<WechatCallbackApprovalItem>();
}
