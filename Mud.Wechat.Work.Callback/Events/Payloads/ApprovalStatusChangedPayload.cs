// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mud.HttpUtils.Attributes;
using Mud.Wechat.Work.Abstractions.Callback.Payloads;

namespace Mud.Wechat.Work.Callback.Events.Payloads;

/// <summary>
/// 审批状态通知事件载荷（<c>open_approval_change</c> 单键；官方 path 90240）。
/// </summary>
/// <remarks>
/// <para>
/// <b>包装布局（ScopeFallback）</b>：全部业务字段位于 <c>ApprovalInfo</c> 节点内，
/// 故契约声明 <c>ScopeFallback = "ApprovalInfo"</c> —— 与 <c>batch_job_result</c> 的 <c>BatchJob</c>
/// 包装同一机制；读取时作用域自动下移一层。
/// </para>
/// <para>
/// <b>不映射根级 <c>AgentID</c></b>：本载荷的作用域是 <c>ApprovalInfo</c>，而 <c>AgentID</c> 与
/// <c>MsgType</c> 同级位于<b>报文根</b>。上游映射表的绑定是<b>单作用域</b>的
/// （<c>Bind</c> 与 <c>ResolveScope</c> 共用同一节点）—— 若把根级元素声明进字段，
/// 作用域判定会命中根节点而使 <c>ApprovalInfo</c> 内的全部字段解析为 <c>null</c>。
/// 应用归属由回调路由的 <c>AppKey</c> 承载，此处不重复。
/// </para>
/// <para>
/// <b>开放面</b>：官方触发时机为「<b>自建/第三方</b>应用调用审批流程引擎」⇒ 仅企业自建 + 第三方
/// （由 <c>OfficialPayloadContracts</c> 的契约声明承载，载荷类型本身不区分模式 —— ADR-14）。
/// </para>
/// <para>
/// <b>字段可能整体缺失</b>：官方按权限分层返回，抄送人（<c>NotifyNodes</c>）与分支
/// （<c>Items</c>）可能为空列表；处理器不得假设必有值。
/// </para>
/// </remarks>
[PayloadContract(Converter = typeof(WechatPayloadConverter), ScopeFallback = "ApprovalInfo")]
public sealed partial class ApprovalStatusChangedPayload : WechatCallbackPayload
{
    /// <summary>审批单编号（官方 <c>ThirdNo</c>，由开发者在发起申请时自定义）。</summary>
    [PayloadField("ThirdNo")]
    public string? ThirdNo { get; set; }

    /// <summary>审批模板名称（官方 <c>OpenSpName</c>）。</summary>
    [PayloadField("OpenSpName")]
    public string? OpenSpName { get; set; }

    /// <summary>审批模板 id（官方 <c>OpenTemplateId</c>）。</summary>
    [PayloadField("OpenTemplateId")]
    public string? OpenTemplateId { get; set; }

    /// <summary>
    /// 申请单当前审批状态（官方 <c>OpenSpStatus</c>：1 审批中 / 2 已通过 / 3 已驳回 / 4 已取消）。
    /// </summary>
    [PayloadField("OpenSpStatus")]
    public long? OpenSpStatus { get; set; }

    /// <summary>提交申请时间（官方 <c>ApplyTime</c>，秒级 Unix 时间戳）。</summary>
    [PayloadField("ApplyTime")]
    public long? ApplyTime { get; set; }

    /// <summary>提交者姓名（官方 <c>ApplyUserName</c>）。</summary>
    [PayloadField("ApplyUserName")]
    public string? ApplyUserName { get; set; }

    /// <summary>提交者 UserId（官方 <c>ApplyUserId</c>）。</summary>
    [PayloadField("ApplyUserId")]
    public string? ApplyUserId { get; set; }

    /// <summary>提交者所在部门（官方 <c>ApplyUserParty</c>）。</summary>
    [PayloadField("ApplyUserParty")]
    public string? ApplyUserParty { get; set; }

    /// <summary>提交者头像（官方 <c>ApplyUserImage</c>）。</summary>
    [PayloadField("ApplyUserImage")]
    public string? ApplyUserImage { get; set; }

    /// <summary>
    /// 审批流程信息（官方 <c>ApprovalNodes/ApprovalNode</c>，可有多个审批节点；节点内含可变分支列表）。
    /// </summary>
    [PayloadField("ApprovalNodes", Method = nameof(WechatPayloadConverter.ParseApprovalNodes))]
    public List<WechatCallbackApprovalNode> ApprovalNodes { get; set; } = new List<WechatCallbackApprovalNode>();

    /// <summary>抄送信息（官方 <c>NotifyNodes/NotifyNode</c>，可能有多个抄送人）。</summary>
    [PayloadField("NotifyNodes", Method = nameof(WechatPayloadConverter.ParseNotifyNodes))]
    public List<WechatCallbackApprovalNotifyNode> NotifyNodes { get; set; } = new List<WechatCallbackApprovalNotifyNode>();

    /// <summary>
    /// 当前审批节点（官方 <c>approverstep</c>：0 第一个审批节点、1 第二个……；官方节点名全小写）。
    /// </summary>
    [PayloadField("approverstep")]
    public long? ApproverStep { get; set; }
}
