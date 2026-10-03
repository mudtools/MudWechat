// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 审批节点分支（<c>open_approval_change</c> 报文的 <c>ApprovalNode/Items/Item</c> 节点；官方 path 90240）。
/// </summary>
/// <remarks>
/// 由 <c>WechatPayloadConverter.ParseApprovalNodes</c> 在转换器内手工组装。
/// 注意与 <see cref="WechatCallbackApprovalNotifyNode"/>（抄送人）字段集不同：分支多了审批状态与意见。
/// </remarks>
public sealed class WechatCallbackApprovalItem
{
    /// <summary>分支审批人姓名（官方 <c>ItemName</c>）。</summary>
    public string? ItemName { get; set; }

    /// <summary>分支审批人 UserId（官方 <c>ItemUserId</c>）。</summary>
    public string? ItemUserId { get; set; }

    /// <summary>分支审批人头像（官方 <c>ItemImage</c>）。</summary>
    public string? ItemImage { get; set; }

    /// <summary>分支审批操作状态（官方 <c>ItemStatus</c>：1 审批中 / 2 已同意 / 3 已驳回 / 4 已转审）。</summary>
    public long? ItemStatus { get; set; }

    /// <summary>分支审批人审批意见（官方 <c>ItemSpeech</c>，可能为空串）。</summary>
    public string? ItemSpeech { get; set; }

    /// <summary>分支审批人操作时间（官方 <c>ItemOpTime</c>，时间戳）。</summary>
    public long? ItemOpTime { get; set; }
}
