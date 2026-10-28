// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication;

/// <summary>
/// 微信公众号应用管理器（多公众号基座）。
/// </summary>
/// <remarks>
/// <para>
/// 实现形态：Singleton + 构造注入 <see cref="IServiceScopeFactory"/>（按上下文建 scope，避免
/// Captive Dependency）；注册表<b>单一来源</b> = 注册期传入的配置列表（配置不可变，无运行时替换语义）。
/// </para>
/// <para>
/// <b>与企微 <c>IWechatAppManager</c> 的差异</b>：无企业级 scope（<c>UseCorpScope</c> / <c>SetCorp</c>）、
/// 无授权商店铺、无应用退役宽限期（公众号无授权编排，配置替换语义不存在 ⇒ 不引入 300s 退役队列）。
/// </para>
/// </remarks>
public interface IMpAppManager : IAppManager<IMpAppContext>
{
    /// <summary>默认应用的配置。</summary>
    Configuration.MpAppConfig DefaultConfig { get; }

    /// <summary>已配置的应用键集合（不触发懒加载实例化）。</summary>
    IReadOnlyCollection<string> ConfiguredAppKeys { get; }

    /// <summary>已配置应用的配置快照（不触发懒加载实例化）。</summary>
    IReadOnlyList<Configuration.MpAppConfig> ConfiguredConfigs { get; }

    /// <summary>读取指定应用的配置（不触发懒加载实例化）。</summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="config">命中的配置；未命中时为 <c>null</c>。</param>
    /// <returns>命中返回 <c>true</c>。</returns>
    bool TryGetConfig(string appKey, out Configuration.MpAppConfig? config);

    /// <summary>默认应用的 <c>access_token</c> 令牌管理器。</summary>
    IMpAccessTokenManager DefaultAccessTokenManager { get; }

    /// <summary>
    /// 级联失效指定应用的令牌（内存 + 持久化 Store 双清）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="tokenType">令牌类型（<see cref="MpTokenTypes"/> 常量）。</param>
    /// <param name="scopes">保留参数（公众号无 scope 语义，恒为 null）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task InvalidateTokenAsync(
        string appKey,
        string tokenType = MpTokenTypes.AccessToken,
        string[]? scopes = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理指定应用在持久层中的全部令牌槽位（凭据轮换后清库）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实际删除的键数量。</returns>
    Task<int> PurgeAppTokensAsync(string appKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取已<b>实例化</b>的应用上下文（懒加载语义：未访问过的应用不在此列）。
    /// </summary>
    /// <returns>已实例化应用上下文的集合。</returns>
    new IEnumerable<IMpAppContext> GetAllApps();
}
