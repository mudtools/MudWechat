// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.Work.Abstractions.Callback.Payloads;

/// <summary>
/// OA 审批「备注」对象（<c>sys_approval_change</c> 报文的 <c>Comments</c> 节点，ApprovalInfo 下平铺重复兄弟元素；官方 path 91815）。
/// </summary>
/// <remarks>
/// /// <para>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；列表分派由
/// <c>WechatPayloadConverter.RepeatOaApprovalComments</c> 承担（「可能有多个备注节点」）。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackOaApprovalComment
{
    /// <summary>备注人信息（官方 <c>Comments/CommentUserInfo</c>，内含 UserId；节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("CommentUserInfo")]
    public WechatCallbackOaApprovalUser? CommentUserInfo { get; set; }

    /// <summary>备注提交时间（官方 <c>Comments/CommentTime</c>，Unix 时间戳）。</summary>
    [PayloadField("CommentTime")]
    public long? CommentTime { get; set; }

    /// <summary>备注文本内容（官方 <c>Comments/CommentContent</c>）。</summary>
    [PayloadField("CommentContent")]
    public string? CommentContent { get; set; }

    /// <summary>备注 id（官方 <c>Comments/CommentId</c>）。</summary>
    [PayloadField("CommentId")]
    public string? CommentId { get; set; }

    /// <summary>备注意见附件（官方 <c>Comments/Attach</c>，media_id；样例未出现 ⇒ 缺失为 <c>null</c>）。</summary>
    [PayloadField("Attach")]
    public string? Attach { get; set; }

}
