// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 写工具审批门端口（HITL 的<b>同步确认</b>形态）：宿主在调用当下回答「放行 / 拒绝」。
/// </summary>
/// <remarks>
/// <para>
/// <b>与授权器的分工</b>：授权器回答「这个人有没有权限」，审批门回答「这一次要不要人确认」——
/// 前者是静态策略（角色/权限点），后者是动态门（金额、影响面、批量规模、高危动作）。
/// 二者串联：<b>先授权后审批</b>，任一不放行即不执行。
/// </para>
/// <para>
/// <b>为什么这里是同步返回</b>：本包只承接「调用当下就能判定」的确认（例如宿主侧已有人机通道、
/// 或按规则自动放行）。「挂起 → 等人工批准 → 续跑」需要持久化与重投递，属审批状态机（下一增量），
/// 不在本端口上开 second 语义（两种语义混在一个返回值里会让调用方能拿到「既没执行也没拒绝」的第三态）。
/// </para>
/// </remarks>
public interface IWechatToolApprovalGate
{
    /// <summary>请求审批一次写工具调用。</summary>
    /// <param name="context">审批输入（工具 / 应用 / 企业 / 风险 / 权限点 / 入参摘要）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>放行或拒绝。</returns>
    ValueTask<WechatToolApprovalDecision> RequestApprovalAsync(
        WechatToolAuthorizationContext context,
        CancellationToken cancellationToken);
}

/// <summary>审批判定（两态：放行 / 拒绝；无「挂起」）。</summary>
public readonly struct WechatToolApprovalDecision
{
    private WechatToolApprovalDecision(bool isApproved, string? reason)
    {
        IsApproved = isApproved;
        Reason = reason;
    }

    /// <summary>是否放行。</summary>
    public bool IsApproved { get; }

    /// <summary>审批备注（放行时可记录确认来源；拒绝时为理由；仅宿主可见）。</summary>
    public string? Reason { get; }

    /// <summary>放行。</summary>
    /// <param name="reason">确认来源（如「管理员在工单 #123 中批准」；可空）。</param>
    /// <returns>放行判定。</returns>
    public static WechatToolApprovalDecision Approve(string? reason = null) => new(true, reason);

    /// <summary>拒绝。</summary>
    /// <param name="reason">宿主可见理由（须非空）。</param>
    /// <returns>拒绝判定。</returns>
    /// <exception cref="ArgumentException"><paramref name="reason"/> 为空白。</exception>
    public static WechatToolApprovalDecision Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("拒绝理由不能为空（审计需要可读原因）。", nameof(reason));
        }

        return new WechatToolApprovalDecision(false, reason);
    }
}
