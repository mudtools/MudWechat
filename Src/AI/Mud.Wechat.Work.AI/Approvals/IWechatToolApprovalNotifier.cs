// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.AI.Abstractions.Approvals;

namespace Mud.Wechat.Work.AI.Approvals;

/// <summary>
/// 待人工确认项的<b>企微侧投递端口</b>：把框架审批请求送到审批人看得见的地方（应用消息 / 机器人卡片 / 工单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么端口落在产品侧而不在 SDK 中立层</b>：投递渠道是产品事实（企微是应用消息 + 机器人卡片，飞书是卡片 + 交互回调），
/// 中立层只定「提交审批请求」这一件事（<see cref="IToolApprovalChannel"/>）。本端口是其产品投递的<b>实现接缝</b>——
/// SDK 不自建卡片模板：审批卡片的字段取舍（是否展示入参摘要、按钮文案、是否可批量批准）是产品需求单的事，
/// 一旦写进 SDK 就变成「改不了也删不掉的默认 UX」。
/// </para>
/// <para>
/// <b>未注册 = 降级为「只记日志 + 快照可查」</b>：写工具仍保持未执行（fail-closed），审批人可经
/// <see cref="WechatToolApprovalResume.ListPendingAsync"/> 拉取待办后另行批准。
/// </para>
/// <para>
/// <b>实现纪律</b>：不得阻塞等待人类操作（这是<b>通知</b>，不是同步确认闸门）；不得把
/// <see cref="FrameworkToolApprovalRequest.ArgumentsDigest"/> 以外的入参原文送进任何审批界面之外的通道。
/// </para>
/// </remarks>
public interface IWechatToolApprovalNotifier
{
    /// <summary>提交一条待确认项给审批人。</summary>
    /// <param name="request">框架审批请求要素（含 RequestId / 工具名 / 脱敏入参摘要）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>渠道侧关联号（可空）；抛出即视为「通知失败」——工具保持未执行，绝不自动放行。</returns>
    Task<string?> NotifyAsync(FrameworkToolApprovalRequest request, CancellationToken cancellationToken = default);
}
