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
/// OA 审批「节点分支详情」对象（<c>sys_approval_change</c> 报文的 <c>Details</c> 节点，SpRecord 内平铺重复兄弟元素；官方 path 91815）。
/// </summary>
/// <remarks>
/// /// <para>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；列表分派由
/// <c>WechatPayloadConverter.RepeatOaApprovalDetails</c> 承担。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackOaApprovalDetail
{
    /// <summary>分支审批人（官方 <c>Details/Approver</c>，内含 UserId；节点缺失 ⇒ <c>null</c>）。</summary>
    [PayloadField("Approver")]
    public WechatCallbackOaApprovalUser? Approver { get; set; }

    /// <summary>审批意见（官方 <c>Details/Speech</c>，可能为空串）。</summary>
    [PayloadField("Speech")]
    public string? Speech { get; set; }

    /// <summary>分支审批人审批状态（官方 <c>Details/SpStatus</c>：1 审批中 / 2 已同意 / 3 已驳回 / 4 已转审）。</summary>
    [PayloadField("SpStatus")]
    public long? SpStatus { get; set; }

    /// <summary>节点分支审批人审批操作时间（官方 <c>Details/SpTime</c>，Unix 时间戳；0 为尚未操作）。</summary>
    [PayloadField("SpTime")]
    public long? SpTime { get; set; }

    /// <summary>分支审批人审批意见附件（官方 <c>Details/Attach</c>，media_id；第三方页参数表别名 MediaId，样例未出现 ⇒ 缺失为 <c>null</c>）。</summary>
    [PayloadField("Attach")]
    public string? Attach { get; set; }

}
