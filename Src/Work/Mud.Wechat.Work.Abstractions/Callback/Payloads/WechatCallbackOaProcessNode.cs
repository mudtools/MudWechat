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
/// OA 审批「流程节点」对象（<c>sys_approval_change</c> 报文的 <c>NodeList</c> 节点，ProcessList 内平铺重复兄弟元素；官方 path 91815）。
/// </summary>
/// <remarks>
/// /// <para>
/// 经上游 G-ADR-17 的 <c>[PayloadContract]</c> 生成字段映射；列表分派由
/// <c>WechatPayloadConverter.RepeatOaProcessNodes</c> 承担。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter))]
public sealed partial class WechatCallbackOaProcessNode
{
    /// <summary>节点类型（官方 <c>NodeList/NodeType</c>：1 审批人 / 2 抄送人 / 3 办理人）。</summary>
    [PayloadField("NodeType")]
    public long? NodeType { get; set; }

    /// <summary>节点状态（官方 <c>NodeList/SpStatus</c>：1 审批中 / 2 同意 / 3 驳回 / 4 转审 / 11 退回给指定审批人 / 12 加签 / 13 同意并加签 / 14 办理 / 15 转交）。</summary>
    [PayloadField("SpStatus")]
    public long? SpStatus { get; set; }

    /// <summary>多人办理方式（官方 <c>NodeList/ApvRel</c>：1 会签 / 2 或签 / 3 依次审批）。</summary>
    [PayloadField("ApvRel")]
    public long? ApvRel { get; set; }

    /// <summary>子节点列表（官方 <c>SubNodeList</c>，NodeList 内平铺重复兄弟元素；节点缺失 ⇒ 空列表）。</summary>
    [PayloadField("SubNodeList", Method = nameof(WechatPayloadConverter.RepeatOaProcessSubNodes))]
    public List<WechatCallbackOaProcessSubNode> SubNodes { get; set; } = new List<WechatCallbackOaProcessSubNode>();

}
