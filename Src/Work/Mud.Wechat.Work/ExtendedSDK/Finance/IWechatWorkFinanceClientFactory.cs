// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.ExtendedSDK.Finance;

/// <summary>
/// 会话存档客户端工厂：按机器人键装配并缓存 <see cref="IWechatWorkFinanceClient"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么必须有工厂</b>：一个机器人对应一份原生 SDK 实例（<c>NewSdk</c> + <c>Init</c>）。
/// 让宿主各自 <c>new</c> 会得到两份互相不知情的原生实例，各自维护自己的拉取进度 ——
/// 症状是「同一段记录被拉两遍、游标互不推进」，比报错更难排查。
/// </para>
/// <para>
/// <b>同一机器人单飞</b>：并发首取时只有一个线程执行 <c>NewSdk</c> + <c>Init</c>，其余等待同一结果。
/// 装配失败<b>不</b>留缓存条目（下次调用重试）—— 否则「密钥中心临时不可达」会永久毒化该机器人。
/// </para>
/// <para>
/// <b>配置的热更对本域无效</b>：<c>corpid</c> 与存档 <c>secret</c> 只在 <c>Init</c> 一刻交予原生库，
/// 之后改配置不会重新 <c>Init</c>。换 secret 或换机器人键之外的配置需重建工厂（见实现 remarks）。
/// </para>
/// </remarks>
public interface IWechatWorkFinanceClientFactory : IDisposable
{
    /// <summary>
    /// 取（或首次装配）指定机器人的客户端。
    /// </summary>
    /// <param name="robotKey">机器人键（<c>WechatFinance:Robots</c> 的字典键）。</param>
    /// <param name="cancellationToken">取消令牌（只在装配的等待点生效，不中断已进入的原生调用）。</param>
    /// <returns>该机器人的共享客户端。</returns>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="robotKey"/> 未配置、或 <c>ISecretProvider</c> 中查无存档 secret 时抛出。
    /// </exception>
    /// <exception cref="WechatFinanceNativeException">
    /// 原生 <c>NewSdk</c> 返回空句柄、或 <c>Init</c> 返回非 0 时抛出（异常消息不含 secret）。
    /// </exception>
    Task<IWechatWorkFinanceClient> GetClientAsync(
        string robotKey, CancellationToken cancellationToken = default);
}
