// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// 抄送人信息（<c>open_approval_change</c> 报文的 <c>ApprovalInfo/NotifyNodes/NotifyNode</c> 节点；
/// 官方 path 90240）。
/// </summary>
/// <remarks>
/// 由 <c>WechatPayloadConverter.ParseNotifyNodes</c> 在转换器内手工组装。节点缺失 ⇒ 空列表。
/// </remarks>
public sealed class WechatCallbackApprovalNotifyNode
{
    /// <summary>抄送人姓名（官方 <c>ItemName</c>）。</summary>
    public string? ItemName { get; set; }

    /// <summary>抄送人 UserId（官方 <c>ItemUserId</c>）。</summary>
    public string? ItemUserId { get; set; }

    /// <summary>抄送人头像（官方 <c>ItemImage</c>）。</summary>
    public string? ItemImage { get; set; }
}
