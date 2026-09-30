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

    /// <summary>默认应用的 access_token 令牌管理器（自建应用为企业令牌；第三方/服务商为企业级令牌）。</summary>
    ITokenManager DefaultAccessTokenManager { get; }

    /// <summary>默认应用的服务商令牌管理器（仅第三方/服务商应用配置存在）。</summary>
    IWechatProviderTokenManager DefaultProviderTokenManager { get; }

    /// <summary>默认应用的套件令牌管理器（仅第三方/服务商应用配置存在）。</summary>
    IWechatSuiteTokenManager DefaultSuiteTokenManager { get; }

    /// <summary>默认应用的授权企业令牌管理器（仅第三方/服务商应用配置存在）。</summary>
    IWechatCorpTokenManager DefaultCorpTokenManager { get; }

    /// <summary>
    /// 级联失效指定应用的指定类型令牌（内存 + 持久化 Store 双清，对齐 TMA-01）。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="tokenType">令牌类型（<see cref="WechatTokenTypes"/> 常量）。</param>
    /// <param name="scopes">企业级令牌的 scope（authCorpId；可为 null 表示全部由环境上下文决定）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task InvalidateTokenAsync(string appKey, string tokenType, string[]? scopes = null, CancellationToken cancellationToken = default);
}
