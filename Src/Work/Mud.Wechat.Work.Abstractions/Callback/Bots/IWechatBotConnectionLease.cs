// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Callback.Bots;

/// <summary>
/// 智能机器人长连接<b>租约</b>端口（多实例部署的主备切换基座，官方 101463「每个机器人同一时间仅一条有效连接」）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么接口在 Abstractions</b>：<c>Mud.Wechat.Redis</c> 只引用 Abstractions（AGENTS §4 依赖边界），
/// 默认 Redis 实现（<c>SET NX EX</c> + Lua 续租/释放）落 Redis 包；单实例部署可缺省（不注册 ⇒ 连接服务以
/// 「进程内唯一」假设运行并告警一次）。
/// </para>
/// <para>
/// <b>语义</b>：租约键 = 机器人键（同机器人多实例竞争一条连接的使用权），值 = 实例标识。
/// <b>重连必须先夺租约再建连</b>：官方把「新连接完成订阅即踢旧连接」作为唯一切换机制，
/// 抢先建连会制造连接乒乓；收到 <c>disconnected_event</c> 后<b>不得</b>立即重试 —— 先退避、再夺租约、夺到才建连。
/// </para>
/// </remarks>
public interface IWechatBotConnectionLease
{
    /// <summary>
    /// 尝试为机器人夺取连接租约（不持有者返回 <c>false</c>，不得建连）。
    /// </summary>
    /// <param name="botKey">机器人键（租约分槽键）。</param>
    /// <param name="instanceId">实例标识（进程唯一，如 <c>Environment.MachineName</c> + GUID）。</param>
    /// <param name="ttl">租约时长（须大于续租周期；过期未续租即视为持有者已死，其他实例可夺取）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>是否夺取成功。</returns>
    Task<bool> TryAcquireAsync(
        string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default);

    /// <summary>
    /// 续租（仅持有者成功；失去租约返回 <c>false</c> ⇒ 调用方必须立即停止使用连接并重走夺取流程）。
    /// </summary>
    Task<bool> RenewAsync(
        string botKey, string instanceId, TimeSpan ttl, CancellationToken cancellationToken = default);

    /// <summary>
    /// 释放租约（仅持有者成功；用于优雅停机 —— 停机即让位，避免等 TTL 自然过期）。
    /// </summary>
    Task<bool> ReleaseAsync(
        string botKey, string instanceId, CancellationToken cancellationToken = default);
}
