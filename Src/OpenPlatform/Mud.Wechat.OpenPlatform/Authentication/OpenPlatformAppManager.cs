// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using Mud.HttpUtils;
using Mud.Wechat.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions;
using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.Abstractions.Transport;

namespace Mud.Wechat.OpenPlatform.Authentication;

/// <summary>
/// 开放平台应用管理器实现：单平台应用 + 按授权方 <c>appid</c> 惰性物化的授权方子上下文。
/// </summary>
/// <remarks>
/// <para>
/// <b>注册表单一来源</b>：平台上下文唯一（注册期 <see cref="OpenPlatformAppConfig"/>），
/// 授权方上下文按 <c>appid</c> 惰性创建并缓存（<see cref="_authorizerContexts"/>）——
/// 无「配置列表」，自然也无配置热更语义。
/// </para>
/// <para>
/// <b>写入口 fail-fast</b>（对齐企微 <c>WechatAppManager</c>）：<c>RegisterApp</c> / <c>UpdateApp</c> /
/// <c>SetDefaultApp</c> / <c>RemoveApp</c> / <c>RegisterSwitcherFactory</c> 一律抛
/// <see cref="NotSupportedException"/> —— 授权关系的建立走 <c>ComponentAuthorizationService</c> 编排
/// （换码成功后经 <c>IAuthorizerTokenProvider.AcceptAuthorization</c> 落库），
/// 不存在「直接注册上下文」的合法路径，静默写会制造读不到的表。
/// </para>
/// <para>
/// <b>应用键语义</b>：平台上下文键 = <see cref="OpenPlatformAppConfig.ComponentAppId"/>；
/// 授权方上下文键 = 授权方 <c>appid</c>（经 <see cref="WechatAppKeyValidator"/> 格式校验）。
/// 两种键共享一个命名空间：<c>GetApp</c> 先按平台键短路，再按授权方键物化。
/// </para>
/// </remarks>
internal sealed class OpenPlatformAppManager : IOpenPlatformAppManager
{
    private readonly OpenPlatformAppContext _platformContext;
    private readonly Func<string, AuthorizerTokenManager> _authorizerTokenManagerFactory;
    private readonly ConcurrentDictionary<string, OpenPlatformAppContext> _authorizerContexts =
        new(StringComparer.Ordinal);

    /// <summary>创建应用管理器（由 <see cref="Extensions.OpenPlatformServiceCollectionExtensions.AddOpenPlatform"/> 装配）。</summary>
    /// <param name="services">根服务提供者（上下文 <c>GetService</c> 的出口）。</param>
    /// <param name="config">平台配置（构造期已由入口 <c>EnsureValid</c>）。</param>
    /// <param name="httpClient">本线命名 HTTP 客户端。</param>
    /// <param name="componentTokenProvider">平台令牌提供者。</param>
    /// <param name="authorizerTokenProvider">授权方令牌提供者。</param>
    /// <exception cref="ArgumentNullException">任一必填依赖为 <c>null</c>。</exception>
    /// <exception cref="ArgumentException">配置缺少 <c>ComponentAppId</c>。</exception>
    public OpenPlatformAppManager(
        IServiceProvider services,
        OpenPlatformAppConfig config,
        IWechatOpenPlatformHttpClient httpClient,
        IComponentTokenProvider componentTokenProvider,
        IAuthorizerTokenProvider authorizerTokenProvider)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        if (httpClient == null)
        {
            throw new ArgumentNullException(nameof(httpClient));
        }

        if (componentTokenProvider == null)
        {
            throw new ArgumentNullException(nameof(componentTokenProvider));
        }

        if (authorizerTokenProvider == null)
        {
            throw new ArgumentNullException(nameof(authorizerTokenProvider));
        }

        if (string.IsNullOrWhiteSpace(config.ComponentAppId))
        {
            throw new ArgumentException("平台配置缺少 ComponentAppId，无法确定平台上下文应用键。", nameof(config));
        }

        _platformContext = new OpenPlatformAppContext(
            config.ComponentAppId,
            httpClient,
            services,
            new ComponentTokenManager(componentTokenProvider));

        _authorizerTokenManagerFactory = appid
            => new AuthorizerTokenManager(authorizerTokenProvider, appid);
    }

    /// <inheritdoc />
    public IOpenPlatformAppContext PlatformContext => _platformContext;

    /// <inheritdoc />
    public IOpenPlatformAppContext GetDefaultApp() => _platformContext;

    /// <inheritdoc />
    public IOpenPlatformAppContext GetApp(string appKey)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentException("应用键不能为空。", nameof(appKey));
        }

        if (string.Equals(appKey, _platformContext.AppKey, StringComparison.Ordinal))
        {
            return _platformContext;
        }

        if (!WechatAppKeyValidator.IsValid(appKey))
        {
            throw new ArgumentException(
                "应用键（授权方 appid）格式非法：只能由字母、数字、'.'、'_'、'-' 组成，"
                + "首字符必须是字母或数字，长度不超过 128。",
                nameof(appKey));
        }

        return _authorizerContexts.GetOrAdd(appKey, CreateAuthorizerContext);
    }

    /// <inheritdoc />
    public bool TryGetApp(string appKey, out IOpenPlatformAppContext? appContext)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            appContext = null;
            return false;
        }

        if (string.Equals(appKey, _platformContext.AppKey, StringComparison.Ordinal))
        {
            appContext = _platformContext;
            return true;
        }

        if (_authorizerContexts.TryGetValue(appKey, out var cached))
        {
            appContext = cached;
            return true;
        }

        // 授权方上下文按需物化：未访问过的 appid 不在此列（与公众号懒加载语义一致）。
        appContext = null;
        return false;
    }

    /// <inheritdoc />
    public IEnumerable<IOpenPlatformAppContext> GetAllApps()
    {
        yield return _platformContext;
        foreach (var context in _authorizerContexts.Values)
        {
            yield return context;
        }
    }

    /// <inheritdoc />
    public bool HasApp(string appKey)
        => TryGetApp(appKey, out _);

    /// <inheritdoc />
    public string? DefaultAppKey => _platformContext.AppKey;

    /// <inheritdoc />
    /// <remarks>单平台形态无切换语义；请使用 <see cref="IComponentAppContextSwitcher"/>。</remarks>
    public TContextSwitcher GetWebApi<TContextSwitcher>(string appKey)
        where TContextSwitcher : IAppContextSwitcher
        => throw NotSupportedSwitcher();

    /// <inheritdoc />
    /// <remarks>单平台形态无切换语义；请使用 <see cref="IComponentAppContextSwitcher"/>。</remarks>
    public TContextSwitcher GetDefaultWebApi<TContextSwitcher>()
        where TContextSwitcher : IAppContextSwitcher
        => throw NotSupportedSwitcher();

    /// <inheritdoc />
    public bool RemoveApp(string appKey)
        => throw new NotSupportedException(
            "开放平台不支持运行时移除应用：平台配置来自注册期 OpenPlatformAppConfig；"
            + "授权方上下文由 UseAuthorizerScope 按需物化，取消授权请清空 IAuthorizerTokenStore 中对应槽位。");

    /// <inheritdoc />
    public void RegisterApp(string appKey, IOpenPlatformAppContext appContext, bool isDefault = false)
        => throw new NotSupportedException(
            "开放平台不支持运行时注册应用：授权关系请经 ComponentAuthorizationService 编排建立。");

    /// <inheritdoc />
    public Task RegisterAppAsync(string appKey, IOpenPlatformAppContext appContext, bool isDefault = false, CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "开放平台不支持运行时注册应用：授权关系请经 ComponentAuthorizationService 编排建立。");

    /// <inheritdoc />
    public void UpdateApp(string appKey, IOpenPlatformAppContext appContext)
        => throw new NotSupportedException(
            "开放平台不支持运行时更新应用：平台配置来自注册期 OpenPlatformAppConfig。");

    /// <inheritdoc />
    public Task UpdateAppAsync(string appKey, IOpenPlatformAppContext appContext, CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "开放平台不支持运行时更新应用：平台配置来自注册期 OpenPlatformAppConfig。");

    /// <inheritdoc />
    public void SetDefaultApp(string appKey)
        => throw new NotSupportedException(
            "开放平台只有一个默认应用（平台自身上下文），不支持改设默认应用。");

    /// <inheritdoc />
    public bool TrySetDefaultApp(string appKey)
        => throw new NotSupportedException(
            "开放平台只有一个默认应用（平台自身上下文），不支持改设默认应用。");

    /// <inheritdoc />
    public void RegisterSwitcherFactory<TContextSwitcher>(Func<IOpenPlatformAppContext, TContextSwitcher> factory)
        where TContextSwitcher : IAppContextSwitcher
        => throw new NotSupportedException(
            "开放平台的切换器经 DI 注册为 IComponentAppContextSwitcher，不支持注册自定义工厂。");

    /// <inheritdoc />
    /// <remarks>单平台形态无配置热更语义，本事件恒不触发。</remarks>
    public event EventHandler<AppConfigurationChangedEventArgs>? ConfigurationChanged
    {
        add { }
        remove { }
    }

    private OpenPlatformAppContext CreateAuthorizerContext(string authorizerAppId)
        => new(
            authorizerAppId,
            _platformContext.HttpClient as IWechatOpenPlatformHttpClient
                ?? throw new InvalidOperationException("平台上下文的 HttpClient 不是开放平台命名客户端。"),
            _platformContext.GetService<IServiceProvider>() ?? throw new InvalidOperationException("平台上下文未携带服务提供者。"),
            _platformContext.GetTokenManager(OpenPlatformTokenTypes.ComponentAccessToken) as ComponentTokenManager
                ?? throw new InvalidOperationException("平台上下文的令牌管理器类型异常。"),
            authorizerAppId,
            _authorizerTokenManagerFactory(authorizerAppId));

    private static NotSupportedException NotSupportedSwitcher()
        => new NotSupportedException(
            "开放平台的应用切换请使用 DI 中的 IComponentAppContextSwitcher"
            + "（UseDefaultAppScope = 平台作用域 / UseAuthorizerScope(appid) = 授权方作用域），"
            + "不走 IAppManager.GetWebApi 通道。");
}
