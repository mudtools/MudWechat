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
/// 经上游 G-ADR-17 的 <c>ItemsObject</c> 嵌套通道声明化（纯标量项，
/// 由外层 <c>NotifyNodes</c> 以 <c>ItemName = "NotifyNode"</c> 收集）。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackApprovalNotifyNode
{
    /// <summary>抄送人姓名（官方 <c>ItemName</c>）。</summary>
    [PayloadField("ItemName")]
    public string? ItemName { get; set; }

    /// <summary>抄送人 UserId（官方 <c>ItemUserId</c>）。</summary>
    [PayloadField("ItemUserId")]
    public string? ItemUserId { get; set; }

    /// <summary>抄送人头像（官方 <c>ItemImage</c>）。</summary>
    [PayloadField("ItemImage")]
    public string? ItemImage { get; set; }
}
