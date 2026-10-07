// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;

/// <summary>
/// 微信公众号应用管理器（多公众号基座）。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><b>注册表单一来源 = 配置列表</b>：不继承组件 <c>DefaultAppManager&lt;T&gt;</c>（其影子注册表会让
/// 直接注册写进读不到的表，语义与企微 P1-7 同源）；<c>RegisterApp</c> / <c>UpdateApp</c> 等显式
/// <c>NotSupportedException</c>。</item>
/// <item>Singleton + 构造注入 <see cref="IServiceScopeFactory"/>：按上下文建 scope，避免 Captive Dependency。</item>
/// <item>懒加载上下文（<see cref="Lazy{T}"/>）：未访问的应用不初始化（不构造命名 HttpClient / 令牌管理器 Timer）。</item>
/// <item><b>无退役队列</b>：公众号无「应用配置替换」语义（配置不可变、无授权编排），
/// <see cref="RemoveApp"/> 直接释放上下文，不引入 300s 宽限期（YAGNI）。</item>
/// <item><b>通道装配</b>：按 <see cref="MpAppConfig.UseStableToken"/> 在装配期为该应用构造
/// <c>MpStableAccessTokenManager</c> 或 <c>MpStandardAccessTokenManager</c>。</item>
/// </list>
/// </remarks>
public sealed class MpAppManager : IMpAppManager, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly ILogger<MpAppManager> _logger;
    private readonly ConcurrentDictionary<string, Lazy<IMpAppContext>> _lazyContexts = new(StringComparer.Ordinal);
    private readonly object _registryLock = new();
    private readonly List<MpAppConfig> _configs;
    private volatile string? _defaultAppKey;
    private int _disposed;

    /// <summary>创建应用管理器。</summary>
    /// <param name="serviceProvider">根服务提供器。</param>
    /// <param name="configs">应用配置列表（须已完成 <see cref="MpAppConfig.Validate"/>）。</param>
    /// <param name="logger">日志器。</param>
    public MpAppManager(IServiceProvider serviceProvider, IEnumerable<MpAppConfig> configs, ILogger<MpAppManager> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configs = (configs ?? throw new ArgumentNullException(nameof(configs))).ToList();

        if (_configs.Count == 0)
        {
            throw new InvalidOperationException("未配置任何微信公众号。请先经 AddMpApp 注册应用配置。");
        }

        var duplicate = _configs.GroupBy(c => c.AppKey, StringComparer.Ordinal).FirstOrDefault(g => g.Count() > 1);
        if (duplicate != null)
        {
            throw new InvalidOperationException($"检测到重复的 AppKey '{duplicate.Key}'。每个应用的 AppKey 必须唯一。");
        }

        foreach (var config in _configs)
        {
            config.Validate();
            var captured = config;
            _lazyContexts[config.AppKey] = new Lazy<IMpAppContext>(
                () => CreateAppContext(captured), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        // 默认应用：IsDefault=true 优先，其次 AppKey=="default"，最后取首个配置。
        _defaultAppKey = _configs.FirstOrDefault(c => c.IsDefault)?.AppKey
            ?? _configs.FirstOrDefault(c => c.AppKey.Equals("default", StringComparison.OrdinalIgnoreCase))?.AppKey
            ?? _configs[0].AppKey;
    }

    /// <summary>应用配置变更事件（移除应用时触发；订阅者异常被隔离，不影响注册表状态）。</summary>
    public event EventHandler<AppConfigurationChangedEventArgs>? ConfigurationChanged;

    /// <summary>默认应用键（全部应用被移除后为 <c>null</c>）。</summary>
    public string? DefaultAppKey => _defaultAppKey;

    /// <inheritdoc />
    public IReadOnlyCollection<string> ConfiguredAppKeys
    {
        get
        {
            lock (_registryLock)
            {
                return _configs.Select(c => c.AppKey).ToArray();
            }
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<MpAppConfig> ConfiguredConfigs
    {
        get
        {
            lock (_registryLock)
            {
                return _configs.ToArray();
            }
        }
    }

    /// <inheritdoc />
    public MpAppConfig DefaultConfig
    {
        get
        {
            var key = _defaultAppKey;
            if (key == null)
            {
                throw new InvalidOperationException("当前无默认应用：注册表为空或默认应用已被移除。");
            }

            return TryGetConfig(key, out var config) && config != null
                ? config
                : throw new InvalidOperationException($"默认应用 '{key}' 的配置不存在。");
        }
    }

    /// <inheritdoc />
    public bool TryGetConfig(string appKey, out MpAppConfig? config)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            config = null;
            return false;
        }

        lock (_registryLock)
        {
            config = _configs.FirstOrDefault(c => string.Equals(c.AppKey, appKey, StringComparison.Ordinal));
        }

        return config != null;
    }

    /// <inheritdoc />
    public IMpAccessTokenManager DefaultAccessTokenManager => GetDefaultApp().AccessTokenManager;

    /// <inheritdoc />
    public IMpAppContext GetDefaultApp()
    {
        var key = _defaultAppKey;
        if (key == null)
        {
            throw new InvalidOperationException(
                "当前无默认应用：注册表为空或默认应用已被移除。请先经 AddMpApp 注册应用，或调用 SetDefaultApp 指定默认。");
        }

        return GetApp(key);
    }

    /// <inheritdoc />
    public IMpAppContext GetApp(string appKey)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(appKey))
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        if (!_lazyContexts.TryGetValue(appKey, out var lazy))
        {
            throw new InvalidOperationException($"未注册的应用标识：'{appKey}'。已注册：{string.Join(", ", ConfiguredAppKeys)}。");
        }

        return lazy.Value;
    }

    /// <inheritdoc />
    public bool TryGetApp(string appKey, out IMpAppContext? appContext)
    {
        appContext = null;
        if (string.IsNullOrEmpty(appKey) || !_lazyContexts.TryGetValue(appKey, out var lazy))
        {
            return false;
        }

        appContext = lazy.Value;
        return true;
    }

    /// <inheritdoc />
    public IEnumerable<IMpAppContext> GetAllApps()
        => _lazyContexts.Values.Where(l => l.IsValueCreated).Select(l => l.Value).ToArray();

    /// <inheritdoc />
    public bool HasApp(string appKey)
        => !string.IsNullOrEmpty(appKey) && _lazyContexts.ContainsKey(appKey);

    /// <inheritdoc />
    public void SetDefaultApp(string appKey)
    {
        if (appKey == null)
        {
            throw new ArgumentNullException(nameof(appKey));
        }

        ThrowIfDisposed();

        lock (_registryLock)
        {
            if (!_lazyContexts.ContainsKey(appKey))
            {
                throw new InvalidOperationException($"无法设置默认应用：应用标识 '{appKey}' 未注册。");
            }

            _defaultAppKey = appKey;
        }
    }

    /// <summary>尝试设置默认应用（不抛异常；应用不存在时返回 <c>false</c>）。</summary>
    /// <param name="appKey">应用键。</param>
    /// <returns>设置成功返回 <c>true</c>。</returns>
    public bool TrySetDefaultApp(string appKey)
    {
        if (string.IsNullOrEmpty(appKey) || !HasApp(appKey))
        {
            return false;
        }

        SetDefaultApp(appKey);
        return true;
    }

    /// <inheritdoc />
    public bool RemoveApp(string appKey)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            return false;
        }

        ThrowIfDisposed();

        IMpAppContext? context = null;
        lock (_registryLock)
        {
            if (!_lazyContexts.TryRemove(appKey, out var lazy))
            {
                return false;
            }

            if (lazy.IsValueCreated)
            {
                context = lazy.Value;
            }

            _configs.RemoveAll(c => string.Equals(c.AppKey, appKey, StringComparison.Ordinal));

            if (string.Equals(_defaultAppKey, appKey, StringComparison.Ordinal))
            {
                _defaultAppKey = _configs.FirstOrDefault()?.AppKey;
            }
        }

        try
        {
            context?.Dispose();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "释放应用 {AppKey} 的上下文失败（已从注册表移除）。", appKey);
        }

        OnConfigurationChanged(new AppConfigurationChangedEventArgs(appKey, AppConfigurationChangeType.Removed));
        return true;
    }

    /// <inheritdoc />
    public async Task InvalidateTokenAsync(
        string appKey,
        string tokenType = MpTokenTypes.AccessToken,
        string[]? scopes = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(appKey) || !string.Equals(tokenType, MpTokenTypes.AccessToken, StringComparison.Ordinal))
        {
            return;
        }

        if (_lazyContexts.TryGetValue(appKey, out var lazy) && lazy.IsValueCreated)
        {
            await lazy.Value.AccessTokenManager.InvalidateTokenAsync(scopes, cancellationToken).ConfigureAwait(false);
            return;
        }

        // 上下文尚未实例化：进程内无镜像，只需清理持久层（不物化上下文——避免为只读失效动作
        // 构造命名 HttpClient / DI scope / 令牌管理器 Timer）。
        await PurgeAppTokensAsync(appKey, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<int> PurgeAppTokensAsync(string appKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            return 0;
        }

        var store = _serviceProvider.GetService<IWechatTokenStore>();
        if (store == null)
        {
            return 0;
        }

        var all = (await store.GetTokenTypesAsync(cancellationToken).ConfigureAwait(false))?.ToList()
                  ?? new List<string>();

        var keys = new List<string>();
        foreach (var prefix in new[]
                 {
                     MpStableAccessTokenManager.ChannelKeyPrefix,
                     MpStandardAccessTokenManager.ChannelKeyPrefix,
                 })
        {
            var tokenTypeKey = WechatTokenStoreBridge.BuildTokenTypeKey(prefix, appKey) + ":";
            keys.AddRange(all.Where(k => k != null && k.StartsWith(tokenTypeKey, StringComparison.Ordinal)));
        }

        if (keys.Count == 0)
        {
            return 0;
        }

        // 批删能力探测：未实现批量端口的仓储回退逐键删除（行为等价）。
        if (store is IWechatTokenStoreBatchRemove batchRemove)
        {
            return await batchRemove.RemoveRangeAsync(keys, cancellationToken).ConfigureAwait(false);
        }

        var removed = 0;
        foreach (var key in keys)
        {
            await store.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
            removed++;
        }

        return removed;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        foreach (var lazy in _lazyContexts.Values)
        {
            if (!lazy.IsValueCreated)
            {
                continue;
            }

            try
            {
                lazy.Value.Dispose();
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "释放应用 {AppKey} 的上下文失败。", lazy.Value.AppKey);
            }
        }

        _lazyContexts.Clear();
        GC.SuppressFinalize(this);
    }

    // ---------------------------------------------------------------- IAppManager<TAppContext> 显式不支持成员
    // 注册表以「配置」为唯一来源，直接注册上下文实例会绕过 CreateAppContext 的装配链与配置校验 ⇒
    // 显式拒绝（而非静默写影子注册表）。

    /// <inheritdoc />
    public void RegisterApp(string appKey, IMpAppContext appContext, bool isDefault = false)
        => throw new NotSupportedException(
            "MpAppManager 不支持直接注册上下文实例（注册表单一来源 = AddMpApp 传入的配置列表）。");

    /// <inheritdoc />
    public Task RegisterAppAsync(string appKey, IMpAppContext appContext, bool isDefault = false, CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "MpAppManager 不支持直接注册上下文实例（注册表单一来源 = AddMpApp 传入的配置列表）。");

    /// <inheritdoc />
    public void UpdateApp(string appKey, IMpAppContext appContext)
        => throw new NotSupportedException(
            "MpAppManager 不支持上下文实例替换；如需下线请使用 RemoveApp(appKey)。");

    /// <inheritdoc />
    public Task UpdateAppAsync(string appKey, IMpAppContext appContext, CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "MpAppManager 不支持上下文实例替换；如需下线请使用 RemoveApp(appKey)。");

    /// <inheritdoc />
    public TContextSwitcher GetWebApi<TContextSwitcher>(string appKey)
        where TContextSwitcher : IAppContextSwitcher
        => throw new NotSupportedException(
            "MpAppManager 的上下文切换器为 DI 单例（IMpAppContextSwitcher），请从容器解析后使用 UseAppScope/UseDefaultAppScope。");

    /// <inheritdoc />
    public TContextSwitcher GetDefaultWebApi<TContextSwitcher>()
        where TContextSwitcher : IAppContextSwitcher
        => throw new NotSupportedException(
            "MpAppManager 的上下文切换器为 DI 单例（IMpAppContextSwitcher），请从容器解析后使用 UseDefaultAppScope/UseAppScope。");

    /// <inheritdoc />
    public void RegisterSwitcherFactory<TContextSwitcher>(Func<IMpAppContext, TContextSwitcher> factory)
        where TContextSwitcher : IAppContextSwitcher
        => throw new NotSupportedException(
            "MpAppManager 的上下文切换器为 DI 单例（IMpAppContextSwitcher），不支持按上下文实例化的工厂委托。");

    // ---------------------------------------------------------------- 私有实现

    /// <summary>装配应用上下文（命名 HttpClient + 令牌管理器 + 可选 DI scope）。</summary>
    private IMpAppContext CreateAppContext(MpAppConfig config)
    {
        IServiceScope? scope = null;
        try
        {
            var provider = _serviceProvider;
            if (_scopeFactory != null)
            {
                scope = _scopeFactory.CreateScope();
                provider = scope.ServiceProvider;
            }

            var clientFactory = provider.GetRequiredService<IMpHttpClientFactory>();
            var tokenManager = CreateAccessTokenManager(provider, config);

            // 恢复执行器：主令牌管理器 + 按令牌类型路由的注册表（errcode 恢复按类型定位管理器）。
            var recoveryExecutor = new TokenRecoveryExecutor(
                tokenManager,
                null,
                null,
                provider.GetRequiredService<IOptionsMonitor<TokenRecoveryOptions>>(),
                provider.GetService<ILogger<TokenRecoveryExecutor>>(),
                new MpTokenManagerRegistry(tokenManager));

            var httpClient = clientFactory.Create(config.AppKey, recoveryExecutor);

            // 票据管理器（jsapi / wx_card 各一份；与令牌管理器同应用、同持久化仓储、键空间隔离）。
            var ticketFactory = provider.GetRequiredService<IMpTicketFactory>();
            var ticketService = ticketFactory.Create(config.AppKey);
            var ticketStore = provider.GetService<IWechatTokenStore>();

            var jsApiTicketManager = new MpJsApiTicketManager(
                ticketService,
                tokenManager,
                Options.Create(config),
                provider.GetRequiredService<ILogger<MpJsApiTicketManager>>(),
                ticketStore);

            var wxCardTicketManager = new MpWxCardTicketManager(
                ticketService,
                tokenManager,
                Options.Create(config),
                provider.GetRequiredService<ILogger<MpWxCardTicketManager>>(),
                ticketStore);

            return new MpAppContext(
                config, httpClient, tokenManager, provider, scope, jsApiTicketManager, wxCardTicketManager);
        }
        catch
        {
            // 装配中途失败不得泄漏已创建的 scope。
            scope?.Dispose();
            throw;
        }
    }

    /// <summary>按 <see cref="MpAppConfig.UseStableToken"/> 装配令牌管理器（通道差异的唯一定点）。</summary>
    private static IMpAccessTokenManager CreateAccessTokenManager(IServiceProvider provider, MpAppConfig config)
    {
        var authFactory = provider.GetRequiredService<IMpAuthenticationFactory>();
        var auth = authFactory.Create(config.AppKey);
        var options = Options.Create(config);
        var tokenStore = provider.GetService<IWechatTokenStore>();

        if (config.UseStableToken)
        {
            return new MpStableAccessTokenManager(
                auth,
                options,
                provider.GetRequiredService<ILogger<MpStableAccessTokenManager>>(),
                tokenStore);
        }

        return new MpStandardAccessTokenManager(
            auth,
            options,
            provider.GetRequiredService<ILogger<MpStandardAccessTokenManager>>(),
            tokenStore);
    }

    private void OnConfigurationChanged(AppConfigurationChangedEventArgs args)
    {
        var handler = ConfigurationChanged;
        if (handler == null)
        {
            return;
        }

        try
        {
            handler(this, args);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // 订阅者异常隔离：不得影响注册表状态。
            _logger.LogWarning(ex, "应用配置变更订阅者抛出异常（已隔离）。");
        }
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(MpAppManager));
        }
    }
}
