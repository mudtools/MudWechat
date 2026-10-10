// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信应用管理器。
/// </summary>
/// <remarks>
/// 实现形态（对齐 FeishuAppManager）：Singleton + 构造注入 <see cref="IServiceScopeFactory"/>
/// （按上下文建 scope，避免 Captive Dependency，对齐 TMA-13）；被替换的旧上下文经
/// 退役队列延迟 Dispose（默认 300s 宽限期，对齐 TMA-07/TMA-24，不依赖 GC）。
/// </remarks>
public interface IWechatAppManager : IAppManager<IWechatAppContext>
{
    /// <summary>
    /// 运行时添加应用。
    /// </summary>
    /// <param name="config">应用配置。</param>
    /// <returns>新创建的应用上下文。</returns>
    /// <exception cref="InvalidOperationException">当应用已存在或配置无效时抛出。</exception>
    IWechatAppContext AddApp(WechatAppConfig config);

    /// <summary>
    /// 默认的应用配置。
    /// </summary>
    WechatAppConfig DefaultConfig { get; }

    /// <summary>
    /// 已配置的应用键集合（不触发懒加载实例化）。
    /// </summary>
    IReadOnlyCollection<string> ConfiguredAppKeys { get; }

    /// <summary>
    /// 已配置应用的配置快照（不触发懒加载实例化）。
    /// </summary>
    /// <remarks>
    /// <b>P1-6 语义</b>：<see cref="IAppManager{TAppContext}.TryGetApp"/> 会<b>物化</b>应用上下文
    /// （构造命名 HttpClient / DI scope / 令牌管理器 Timer）；仅需读取配置（如回调事件的
    /// <c>SuiteId → appKey</c> 匹配）时必须使用本属性，避免一次回调实例化全部已配置应用。
    /// </remarks>
    IReadOnlyList<WechatAppConfig> ConfiguredConfigs { get; }

    /// <summary>
    /// 读取指定应用的配置（不触发懒加载实例化）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="config">命中的配置；未命中时为 <c>null</c>。</param>
    /// <returns>命中返回 <c>true</c>。</returns>
    bool TryGetConfig(string appKey, out WechatAppConfig? config);

    /// <summary>默认应用的 access_token 令牌管理器（自建应用为企业令牌；第三方/服务商为企业级令牌）。</summary>
    ITokenManager DefaultAccessTokenManager { get; }

    /// <summary>默认应用的服务商令牌管理器（仅第三方/服务商应用配置存在）。</summary>
    IWechatProviderTokenManager DefaultProviderTokenManager { get; }

    /// <summary>默认应用的套件令牌管理器（仅第三方/服务商应用配置存在）。</summary>
    IWechatSuiteTokenManager DefaultSuiteTokenManager { get; }

    /// <summary>默认应用的授权企业令牌管理器（仅第三方/服务商应用配置存在）。</summary>
    IWechatCorpTokenManager DefaultCorpTokenManager { get; }

    /// <summary>
    /// 获取已<b>实例化</b>的应用上下文（懒加载语义：未访问过的应用不在此列）。
    /// </summary>
    /// <returns>已实例化应用上下文的集合。</returns>
    /// <remarks>
    /// M6（F6）：重声明组件 <c>IAppManager{TAppContext}.GetAllApps</c>「所有已注册上下文」的表述
    /// ——本方法只返回已实例化的上下文，刻意不批量物化（D6 懒加载不变量）。
    /// 需要全部已配置应用时配合 <see cref="ConfiguredAppKeys"/> / <see cref="ConfiguredConfigs"/>
    /// （配置面读取不触发实例化）。
    /// </remarks>
    new IEnumerable<IWechatAppContext> GetAllApps();

    /// <summary>
    /// 级联失效指定应用的指定类型令牌（内存 + 持久化 Store 双清，对齐 TMA-01）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="tokenType">令牌类型（<see cref="WechatTokenTypes"/> 常量）。</param>
    /// <param name="scopes">企业级令牌的 scope（authCorpId；可为 null 表示全部由环境上下文决定）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <remarks>
    /// <b>P1-6</b>：应用上下文尚未实例化时只清理持久层（不物化 APP 上下文——避免为只读失效动作构造
    /// HttpClient / DI scope / 令牌管理器 Timer）。
    /// </remarks>
    Task InvalidateTokenAsync(string appKey, string tokenType, string[]? scopes = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理指定应用在持久层中的<b>全部</b>令牌槽位（应用下线 / 凭据轮换后清库）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实际删除的键数量。</returns>
    /// <remarks>
    /// <b>P1-9</b>：凭据轮换（改了 AgentSecret / SuiteSecret）后旧令牌会被持久层读穿透恢复 ⇒ 持续 401，
    /// 故需要"按应用清库"能力。<see cref="IAppManager{TAppContext}.RemoveApp"/> 已自动入队本操作（异步执行）。
    /// <para>多实例下仅清理持久层；各实例的进程内镜像为「最终一致」（由 TTL/阈值自然过期）。</para>
    /// </remarks>
    Task<int> PurgeAppTokensAsync(string appKey, CancellationToken cancellationToken = default);
}
