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
/// OA 审批「子节点」对象（<c>sys_approval_change</c> 报文的 <c>SubNodeList</c> 节点，NodeList 内平铺重复兄弟元素；官方 path 91815）。
/// </summary>
/// <remarks>
/// /// <para>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；列表分派由
/// <c>WechatPayloadConverter.RepeatOaProcessSubNodes</c> 承担。
/// </para>
/// <para><b>官方大小写混用</b>：<c>SpYj</c>/<c>Sptime</c> 为官方原文拼写，照抄勿「修正」。</para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackOaProcessSubNode
{
    /// <summary>处理人信息（官方 <c>SubNodeList/UserInfo</c>，内含 UserId；节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("UserInfo")]
    public WechatCallbackOaApprovalUser? UserInfo { get; set; }

    /// <summary>审批/办理意见（官方 <c>SubNodeList/Speech</c>，可能为空串）。</summary>
    [PayloadField("Speech")]
    public string? Speech { get; set; }

    /// <summary>子节点状态（官方 <c>SubNodeList/SpYj</c>：1 审批中 / 2 同意 / 3 驳回 / 4 转审 / 11 退回给指定审批人 / 12 加签 / 13 同意并加签 / 14 办理 / 15 转交）。</summary>
    [PayloadField("SpYj")]
    public long? SpYj { get; set; }

    /// <summary>操作时间（官方 <c>SubNodeList/Sptime</c>，Unix 时间戳）。</summary>
    [PayloadField("Sptime")]
    public long? Sptime { get; set; }

    /// <summary>备注意见附件 media_id 列表（官方 <c>SubNodeList/MediaIds</c>；样例未出现 ⇒ 空列表）。</summary>
    [PayloadField("MediaIds", Method = nameof(WechatPayloadConverter.RepeatSiblings))]
    public List<string> MediaIds { get; set; } = new List<string>();

}
