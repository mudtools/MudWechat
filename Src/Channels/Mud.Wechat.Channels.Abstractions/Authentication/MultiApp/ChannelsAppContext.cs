// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.Abstractions.Configuration;

namespace Mud.Wechat.Channels.Abstractions.Authentication.MultiApp;

/// <summary>
/// 微信小店 / 视频号应用上下文实现。
/// </summary>
/// <remarks>
/// 随上下文 Dispose 释放所属 DI scope（避免 Captive Dependency）；令牌路由见
/// <see cref="GetTokenManager(string)"/>。小店只有单一 access_token，无票据 / 无企业级 scope
/// （对齐公众号 <c>MpAppContext</c>，相较其移除了 jsapi / wx_card 两张票据管理器）。
/// </remarks>
public class ChannelsAppContext : IChannelsAppContext
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly IServiceScope? _scope;
    private int _disposed;

    /// <summary>创建应用上下文。</summary>
    /// <param name="config">应用配置。</param>
    /// <param name="httpClient">本应用的恢复型 HTTP 客户端（BaseAddress 已按配置解析）。</param>
    /// <param name="accessTokenManager">本应用的令牌管理器（普通 / 稳定版通道按配置装配）。</param>
    /// <param name="serviceProvider">所属 scope 的服务提供器（可选）。</param>
    /// <param name="scope">所属 DI scope（随上下文 Dispose 释放）。</param>
    public ChannelsAppContext(
        ChannelsAppConfig config,
        IEnhancedHttpClient httpClient,
        IChannelsAccessTokenManager accessTokenManager,
        IServiceProvider? serviceProvider = null,
        IServiceScope? scope = null)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        AccessTokenManager = accessTokenManager ?? throw new ArgumentNullException(nameof(accessTokenManager));
        _serviceProvider = serviceProvider;
        _scope = scope;
    }

    /// <inheritdoc />
    public string AppKey => Config.AppKey;

    /// <summary>应用配置。</summary>
    public ChannelsAppConfig Config { get; }

    /// <inheritdoc />
    public IEnhancedHttpClient HttpClient { get; }

    /// <inheritdoc />
    public string AppId => Config.AppId;

    /// <inheritdoc />
    public string BaseUrl => Config.BaseUrl;

    /// <inheritdoc />
    public IChannelsAccessTokenManager AccessTokenManager { get; }

    /// <summary>
    /// 按令牌类型路由令牌管理器（生成代码与 <c>DefaultTokenProvider</c> 的查找入口）。
    /// </summary>
    /// <param name="tokenType">令牌类型（<see cref="ChannelsTokenTypes"/> 常量）。</param>
    /// <returns>令牌管理器。</returns>
    /// <exception cref="InvalidOperationException">未知令牌类型时抛出。</exception>
    /// <remarks>
    /// 小店只有一种令牌类型（<see cref="ChannelsTokenTypes.AccessToken"/>）；普通 / 稳定版两通道由注册期
    /// 装配的实现承载，<b>不体现在路由键上</b>（否则编译期常量与运行期配置会分裂注入 / 恢复两条链路）。
    /// </remarks>
    public ITokenManager GetTokenManager(string tokenType)
    {
        if (string.Equals(tokenType, ChannelsTokenTypes.AccessToken, StringComparison.Ordinal))
        {
            return AccessTokenManager;
        }

        throw new InvalidOperationException(
            $"未知的小店令牌类型：{tokenType}（合法值：{ChannelsTokenTypes.AccessToken}）。");
    }

    /// <inheritdoc />
    public T GetTokenManager<T>() where T : class, ITokenManager
    {
        if (typeof(T) == typeof(IChannelsAccessTokenManager) || typeof(T).IsInstanceOfType(AccessTokenManager))
        {
            return (T)AccessTokenManager;
        }

        throw new InvalidOperationException(
            $"令牌管理器 {typeof(T).Name} 未注册或类型不匹配（AppKey: {AppKey}）。");
    }

    /// <inheritdoc />
    public T? GetService<T>() where T : class
    {
        // 已释放的上下文不得再向宿主 scope 索取服务（scope 已被 Dispose，取到的可能是已处置实例）。
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(ChannelsAppContext),
                $"应用 {AppKey} 的上下文已释放，不能再解析服务。");
        }

        switch (typeof(T))
        {
            case var t when t == typeof(IChannelsAppContext):
                return (T?)(object)this;
            case var t when t == typeof(IChannelsAccessTokenManager):
                return AccessTokenManager as T;
            case var t when t == typeof(IEnhancedHttpClient):
                return HttpClient as T;
            default:
                return _serviceProvider?.GetService<T>();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        try
        {
            if (AccessTokenManager is IDisposable tokenManager)
            {
                tokenManager.Dispose();
            }

            if (HttpClient is IDisposable httpClient)
            {
                httpClient.Dispose();
            }

            _scope?.Dispose();
        }
        finally
        {
            GC.SuppressFinalize(this);
        }
    }
}