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
/// OA 审批「审批流程信息」对象（<c>sys_approval_change</c> 报文的 <c>SpRecord</c> 节点，ApprovalInfo 下平铺重复兄弟元素；官方 path 91815）。
/// </summary>
/// <remarks>
/// /// <para>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；列表分派由
/// <c>WechatPayloadConverter.RepeatOaApprovalRecords</c> 承担（平铺重复形态依赖同名兄弟合并投影）。
/// </para>
/// <para><b>可空超集</b>：处理器不得假设列表内节点必有值。</para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackOaApprovalRecord
{
    /// <summary>审批节点状态（官方 <c>SpRecord/SpStatus</c>：1 审批中 / 2 已同意 / 3 已驳回 / 4 已转审）。</summary>
    [PayloadField("SpStatus")]
    public long? SpStatus { get; set; }

    /// <summary>节点审批方式（官方 <c>SpRecord/ApproverAttr</c>：1 或签 / 2 会签）。</summary>
    [PayloadField("ApproverAttr")]
    public long? ApproverAttr { get; set; }

    /// <summary>审批节点详情列表（官方 <c>Details</c>，SpRecord 内平铺重复兄弟元素；「当节点为标签或上级时，一个节点可能有多个分支」；节点缺失 ⇒ 空列表）。</summary>
    [PayloadField("Details", Method = nameof(WechatPayloadConverter.RepeatOaApprovalDetails))]
    public List<WechatCallbackOaApprovalDetail> Details { get; set; } = new List<WechatCallbackOaApprovalDetail>();

}
