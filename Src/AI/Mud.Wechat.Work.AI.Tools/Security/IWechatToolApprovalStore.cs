// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.AI.Tools.Security;

/// <summary>
/// 审批工单存储端口（挂起式审批的持久化面）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么要有端口</b>：工单要跨「挂起的那一轮」与「人工批准后的续跑」存活，进程内默认实现
/// （<see cref="InMemoryWechatToolApprovalStore"/>）只覆盖单实例；多实例部署由宿主前置注册
/// 分布式实现（Redis / 数据库），契约与五个存储端口同形——<b>宿主预注册者按契约胜出</b>。
/// </para>
/// <para>
/// <b>写回带版本</b>：<see cref="UpdateAsync"/> 的 <c>expectedRevision</c> 是乐观并发凭据，
/// 版本不符即返回 <see langword="false"/>（不得静默覆盖别人的判定）。
/// </para>
/// </remarks>
public interface IWechatToolApprovalStore
{
    /// <summary>按工单号读取。</summary>
    /// <param name="ticketId">工单号。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工单；不存在返回 <see langword="null"/>。</returns>
    ValueTask<WechatToolApprovalTicket?> GetAsync(string ticketId, CancellationToken cancellationToken);

    /// <summary>写入新工单。</summary>
    /// <param name="ticket">工单（必须是新建的待审批工单）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    ValueTask SaveAsync(WechatToolApprovalTicket ticket, CancellationToken cancellationToken);

    /// <summary>按版本写回工单（乐观并发）。</summary>
    /// <param name="ticket">工单（已含新状态）。</param>
    /// <param name="expectedRevision">读取时看到的版本。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>是否写回成功（<see langword="false"/> = 版本不符，工单已被他人迁移）。</returns>
    ValueTask<bool> UpdateAsync(
        WechatToolApprovalTicket ticket,
        long expectedRevision,
        CancellationToken cancellationToken);
}
