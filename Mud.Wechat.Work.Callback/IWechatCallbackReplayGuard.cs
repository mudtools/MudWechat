// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调重放防护：把「时间戳 + 随机数 + 密文指纹」标记为一次性消费（P0-2 抗重放第二道闸）。
/// </summary>
/// <remarks>
/// <para>
/// 默认实现（<see cref="InMemoryWechatCallbackReplayGuard"/>）仅进程内有效；多实例部署须由宿主以
/// <c>TryAdd</c> <b>前置</b>注册分布式实现（如 Redis <c>SET key 1 NX EX window</c>），
/// 与 <see cref="IWechatSuiteTicketStore"/> 同款约定。
/// </para>
/// <para>
/// 本接口只做「窗口内一次性」语义；时效窗口校验由接收器自行完成（两者缺一不可）。
/// </para>
/// </remarks>
public interface IWechatCallbackReplayGuard
{
    /// <summary>
    /// 尝试标记并返回是否为首次消费。
    /// </summary>
    /// <param name="key">去重键（<b>不得</b>为敏感明文；本 SDK 传入 SHA1 指纹，不可逆）。</param>
    /// <param name="window">保留窗口（须 ≥ 时间戳容差，保证窗口覆盖期内不可重放）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>首次消费返回 <c>true</c>；窗口内重复返回 <c>false</c>。</returns>
    Task<bool> TryMarkAsync(string key, TimeSpan window, CancellationToken cancellationToken = default);
}
