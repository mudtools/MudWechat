// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 审批工单状态机（挂起 → 人工批准 → 续跑）的五态。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是五态而不是「加一个挂起值」</b>：同步门（<see cref="IWechatToolApprovalGate"/>）的返回值是两态，
/// 硬塞第三态会让调用方拿到「既没执行也没拒绝」的不可判定结果。挂起式审批因此走<b>独立端口</b>：
/// 调用当下返回的是<b>工单</b>（不是判定），人工判定通过状态迁移落库，续跑时按状态执行。
/// </para>
/// <para>
/// 合法迁移：<c>Pending → Approved | Rejected | Expired</c>、<c>Approved → Consumed</c>。
/// <c>Consumed</c> 是终态——批准只能兑现一次（防止「一次批准、多次执行」）。
/// </para>
/// </remarks>
public enum WechatToolApprovalState
{
    /// <summary>待人工审批。</summary>
    Pending = 0,

    /// <summary>已批准（尚未兑现；可被续跑消费一次）。</summary>
    Approved = 1,

    /// <summary>已拒绝（终态）。</summary>
    Rejected = 2,

    /// <summary>已过期（终态；超时未获判定的工单不得再执行）。</summary>
    Expired = 3,

    /// <summary>已兑现（终态；批准对应的那一次执行已完成）。</summary>
    Consumed = 4,
}
