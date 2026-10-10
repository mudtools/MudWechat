// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// OA 审批「申请人信息」对象（<c>sys_approval_change</c> 报文的 <c>Applyer</c> 节点；官方 path 91815）。
/// </summary>
/// <remarks>
/// /// 经上游 G-ADR-17 的 <c>Object</c> 嵌套通道声明化：单节点对象，元素缺失 ⇒ <c>null</c>。
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackOaApprovalApplyer
{
    /// <summary>申请人 userid（官方 <c>Applyer/UserId</c>）。</summary>
    [PayloadField("UserId")]
    public string? UserId { get; set; }

    /// <summary>申请人所在部门 pid（官方 <c>Applyer/Party</c>）。</summary>
    [PayloadField("Party")]
    public string? Party { get; set; }

}
