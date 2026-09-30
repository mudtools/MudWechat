// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

namespace Mud.Wechat.Work.Abstractions.Authentication;

/// <summary>
/// 企业微信应用管理器（对齐 <c>FeishuAppManager</c> 的 v1 同构实现）。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Singleton + 构造注入 <see cref="IServiceScopeFactory"/>：按上下文建 scope，
/// 避免 Captive Dependency（TMA-13）；</item>
/// <item>懒加载上下文（<see cref="Lazy{T}"/>）：未访问的应用不初始化；</item>
/// <item>退役队列：被替换/移除的旧上下文宽限期（默认 300s）后确定性 Dispose（TMA-07/TMA-24）；</item>
/// <item>令牌管理器三元组按应用装配：自建应用装配 <see cref="InternalAppTokenManager"/>，
/// 第三方/服务商装配 <see cref="ProviderTokenManager"/> / <see cref="SuiteTokenManager"/> /
/// <see cref="CorpTokenManager"/>（provider/suite 实现 <see cref="ISharedTokenManager"/>）。</item>
/// </list>
/// </remarks>
public class WechatAppManager : DefaultAppManager<IWechatAppContext>, IWechatAppManager, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly ILogger<WechatAppManager> _logger;
    private readonly ConcurrentDictionary<string, Lazy<IWechatAppContext>> _lazyContexts = new(StringComparer.Ordinal);
    private readonly object _registryLock = new();
    private readonly List<WechatAppConfig> _configs;
    private WechatAppContextRetirement? _retirement;
    // A-1（对齐 Feishu）：默认应用键本类私有管理——基类 SetDefaultApp 依赖基类注册表，
    // 与懒加载上下文（Lazy）不兼容，故覆写 GetDefaultApp/SetDefaultApp 并隐藏 DefaultAppKey。
    private volatile string? _defaultAppKey;

    /// <summary>创建企业微信应用管理器。</summary>
    /// <param name="serviceProvider">根服务提供器。</param>
    /// <param name="configs">应用配置列表（须已完成 <see cref="WechatAppConfig.Validate"/>）。</param>
    /// <param name="logger">日志器。</param>
    public WechatAppManager(IServiceProvider serviceProvider, IEnumerable<WechatAppConfig> configs, ILogger<WechatAppManager> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configs = (configs ?? throw new ArgumentNullException(nameof(configs))).ToList();

        if (_configs.Count == 0)
        {
            throw new InvalidOperationException("未配置任何企业微信应用。请先经 AddWechatApp 注册应用配置。");
        }

        var duplicate = _configs.GroupBy(c => c.AppKey, StringComparer.Ordinal).FirstOrDefault(g => g.Count() > 1);
        if (duplicate != null)
        {
            throw new InvalidOperationException($"检测到重复的 AppKey '{duplicate.Key}'。每个应用的 AppKey 必须唯一。");
        }

        foreach (var config in _configs)
        {
            config.Validate();
            _lazyContexts[config.AppKey] = new Lazy<IWechatAppContext>(() => CreateAppContext(config), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        // 默认应用：IsDefault=true 优先，其次 AppKey=="default"，最后取首个配置。
        var defaultKey = _configs.FirstOrDefault(c => c.IsDefault)?.AppKey
            ?? _configs.FirstOrDefault(c => c.AppKey.Equals("default", StringComparison.OrdinalIgnoreCase))?.AppKey
            ?? _configs[0].AppKey;
        _defaultAppKey = defaultKey;

        _retirement = new WechatAppContextRetirement(Consts.DefaultContextRetireDelaySeconds, logger);
    }

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

    /// <summary>默认应用键（本类私有管理，隐藏基类基于自身注册表的实现）。</summary>
    public new string? DefaultAppKey => _defaultAppKey;

    /// <summary>获取默认应用（覆写：经本类私有默认键解析）。</summary>
    public override IWechatAppContext GetDefaultApp() => GetApp(_defaultAppKey!);

    /// <summary>设置默认应用（覆写：校验后写入本类私有默认键）。</summary>
    public override void SetDefaultApp(string appKey)
    {
        if (!_lazyContexts.ContainsKey(appKey))
        {
            throw new InvalidOperationException($"无法设置默认应用：应用标识 '{appKey}' 未注册。");
        }

        _defaultAppKey = appKey;
    }

    /// <inheritdoc />
    public WechatAppConfig DefaultConfig => GetApp(_defaultAppKey!).Config;

    /// <inheritdoc />
    public ITokenManager DefaultAccessTokenManager => GetDefaultApp().GetTokenManager(WechatTokenTypes.AccessToken);

    /// <inheritdoc />
    public IWechatProviderTokenManager DefaultProviderTokenManager
        => GetDefaultApp().ProviderTokenManager
           ?? throw new InvalidOperationException($"默认应用 {DefaultAppKey} 未配置服务商令牌管理器（仅第三方/服务商应用）。");

    /// <inheritdoc />
    public IWechatSuiteTokenManager DefaultSuiteTokenManager
        => GetDefaultApp().SuiteTokenManager
           ?? throw new InvalidOperationException($"默认应用 {DefaultAppKey} 未装配套件令牌管理器（仅第三方/服务商应用）。");

    /// <inheritdoc />
    public IWechatCorpTokenManager DefaultCorpTokenManager
        => GetDefaultApp().CorpTokenManager
           ?? throw new InvalidOperationException($"默认应用 {DefaultAppKey} 未装配授权企业令牌管理器（仅第三方/服务商应用）。");

    /// <inheritdoc />
    public override IWechatAppContext GetApp(string appKey)
    {
        if (_lazyContexts.TryGetValue(appKey, out var lazy))
        {
            return lazy.Value;
        }

        throw new InvalidOperationException(
            $"未找到应用 '{appKey}'。已配置应用：{string.Join(", ", ConfiguredAppKeys)}。");
    }

    /// <inheritdoc />
    public override bool HasApp(string appKey) => _lazyContexts.ContainsKey(appKey);

    /// <inheritdoc />
    public override bool TryGetApp(string appKey, out IWechatAppContext? appContext)
    {
        if (_lazyContexts.TryGetValue(appKey, out var lazy))
        {
            appContext = lazy.Value;
            return true;
        }

        appContext = null;
        return false;
    }

    /// <inheritdoc />
    public override IEnumerable<IWechatAppContext> GetAllApps()
    {
        // 只返回已实例化（Lazy 已求值）的上下文，避免枚举触发批量初始化（对齐 Feishu D6）。
        foreach (var pair in _lazyContexts)
        {
            if (pair.Value.IsValueCreated)
            {
                yield return pair.Value.Value;
            }
        }
    }

    /// <inheritdoc />
    public IWechatAppContext AddApp(WechatAppConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        config.Validate();

        lock (_registryLock)
        {
            if (_lazyContexts.ContainsKey(config.AppKey))
            {
                throw new InvalidOperationException($"应用 '{config.AppKey}' 已存在，不能重复注册。");
            }

            _configs.Add(config);
            _lazyContexts[config.AppKey] = new Lazy<IWechatAppContext>(() => CreateAppContext(config), LazyThreadSafetyMode.ExecutionAndPublication);
            if (config.IsDefault)
            {
                _logger.LogWarning("运行时添加的应用 {AppKey} 声明 IsDefault=true，已切换为默认应用。", config.AppKey);
                _defaultAppKey = config.AppKey;
            }
        }

        OnConfigurationChanged(new AppConfigurationChangedEventArgs(config.AppKey, AppConfigurationChangeType.Added));
        return GetApp(config.AppKey);
    }

    /// <inheritdoc />
    public override bool RemoveApp(string appKey)
    {
        lock (_registryLock)
        {
            var removedConfig = _configs.FirstOrDefault(c => c.AppKey == appKey);
            if (removedConfig != null)
            {
                _configs.Remove(removedConfig);
            }

            if (!_lazyContexts.TryRemove(appKey, out var lazy))
            {
                return false;
            }

            if (lazy.IsValueCreated)
            {
                _retirement?.Enqueue(appKey, lazy.Value);
            }

            // 默认应用被移除时确定性提升（对齐 Feishu）。
            if (string.Equals(_defaultAppKey, appKey, StringComparison.Ordinal) && _configs.Count > 0)
            {
                SetDefaultApp(_configs[0].AppKey);
            }
        }

        OnConfigurationChanged(new AppConfigurationChangedEventArgs(appKey, AppConfigurationChangeType.Removed));
        return true;
    }

    /// <inheritdoc />
    public async Task InvalidateTokenAsync(string appKey, string tokenType, string[]? scopes = null, CancellationToken cancellationToken = default)
    {
        var context = GetApp(appKey);
        var manager = context.GetTokenManager(tokenType);
        await manager.InvalidateTokenAsync(scopes, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("应用 {AppKey} 的 {TokenType} 令牌已级联失效（内存 + Store 双清）。", appKey, tokenType);
    }

    /// <summary>
    /// 装配应用上下文（对齐 FeishuAppManager.CreateAppContext）：
    /// 命名客户端 → 认证 API → 令牌管理器 → 恢复执行器 → 恢复型客户端 → 上下文。
    /// </summary>
    protected virtual IWechatAppContext CreateAppContext(WechatAppConfig config)
    {
        // TMA2-11 / D13：装配失败时释放 scope；成功后所有权转移给 WechatAppContext。
        var scope = _scopeFactory?.CreateScope();
        var scopedSp = scope?.ServiceProvider ?? _serviceProvider;

        try
        {
            var httpClientFactory = scopedSp.GetService<IWechatHttpClientFactory>()
                ?? new WechatHttpClientFactory(scopedSp);
            var authFactory = scopedSp.GetService<IWechatAuthenticationFactory>()
                ?? new PerAppWechatAuthenticationFactory(scopedSp, httpClientFactory, scopedSp.GetService<ILogger<PerAppWechatAuthenticationFactory>>());

            var tokenStore = scopedSp.GetService<IWechatTokenStore>();
            var options = Microsoft.Extensions.Options.Options.Create(config);

            IWechatInternalAppTokenManager? internalAppTokenManager = null;
            IWechatProviderTokenManager? providerTokenManager = null;
            IWechatSuiteTokenManager? suiteTokenManager = null;
            IWechatCorpTokenManager? corpTokenManager = null;
            ITokenManager primaryTokenManager;

            if (config.AppType == WechatAppType.Internal)
            {
                internalAppTokenManager = new InternalAppTokenManager(
                    authFactory.CreateInternalAppAuthentication(config.AppKey), options,
                    scopedSp.GetRequiredService<ILogger<InternalAppTokenManager>>(), tokenStore);
                primaryTokenManager = internalAppTokenManager;
            }
            else
            {
                providerTokenManager = new ProviderTokenManager(
                    authFactory.CreateProviderAuthentication(config.AppKey), options,
                    scopedSp.GetRequiredService<ILogger<ProviderTokenManager>>(), tokenStore);
                suiteTokenManager = new SuiteTokenManager(
                    authFactory.CreateProviderAuthentication(config.AppKey),
                    scopedSp.GetRequiredService<IWechatSuiteTicketProvider>(), options,
                    scopedSp.GetRequiredService<ILogger<SuiteTokenManager>>(), tokenStore);
                corpTokenManager = new CorpTokenManager(
                    authFactory.CreateCorpTokenAuthentication(config.AppKey),
                    suiteTokenManager,
                    scopedSp.GetRequiredService<IWechatCorpAuthStore>(), options,
                    scopedSp.GetRequiredService<ILogger<CorpTokenManager>>(), tokenStore);
                primaryTokenManager = corpTokenManager;
            }

            // 恢复执行器：主令牌管理器 + 按令牌类型路由的注册表（errcode 恢复按类型定位管理器）。
            // TMA-02/D2 对齐：不传 ICurrentUserContext，避免租户请求被注入用户令牌。
            var recoveryOptionsMonitor = scopedSp.GetRequiredService<IOptionsMonitor<TokenRecoveryOptions>>();
            var recoveryLogger = scopedSp.GetService<ILogger<TokenRecoveryExecutor>>();
            var recoveryExecutor = new TokenRecoveryExecutor(
                primaryTokenManager,
                null,
                null,
                recoveryOptionsMonitor,
                recoveryLogger,
                new WechatTokenManagerRegistry(primaryTokenManager, providerTokenManager, suiteTokenManager, corpTokenManager));

            var httpClient = httpClientFactory.Create(config.AppKey, recoveryExecutor);

            return new WechatAppContext(
                config, httpClient,
                internalAppTokenManager, corpTokenManager, providerTokenManager, suiteTokenManager,
                scopedSp, scope);
        }
        catch
        {
            scope?.Dispose();
            throw;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        var retirement = _retirement;
        _retirement = null;
        retirement?.Dispose();

        foreach (var pair in _lazyContexts)
        {
            if (pair.Value.IsValueCreated)
            {
                try
                {
                    pair.Value.Value.Dispose();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "停机释放应用上下文失败（AppKey: {AppKey}）。", pair.Key);
                }
            }
        }

        _lazyContexts.Clear();
    }

    /// <summary>
    /// 按令牌类型路由的令牌管理器注册表（恢复执行器经 <see cref="ITokenManagerRegistry"/>
    /// 按 <c>TokenRecoveryContext.TokenManagerKey</c> 定位管理器）。
    /// </summary>
    private sealed class WechatTokenManagerRegistry : ITokenManagerRegistry
    {
        private readonly Dictionary<string, ITokenManager> _managers;

        public WechatTokenManagerRegistry(
            ITokenManager primary,
            IWechatProviderTokenManager? provider,
            IWechatSuiteTokenManager? suite,
            IWechatCorpTokenManager? corp)
        {
            _managers = new Dictionary<string, ITokenManager>(StringComparer.Ordinal)
            {
                [WechatTokenTypes.AccessToken] = corp ?? primary,
            };
            if (provider != null) _managers[WechatTokenTypes.ProviderAccessToken] = provider;
            if (suite != null) _managers[WechatTokenTypes.SuiteAccessToken] = suite;
        }

        public ITokenManager? Resolve(string tokenManagerKey)
            => tokenManagerKey != null && _managers.TryGetValue(tokenManagerKey, out var manager) ? manager : null;
    }
}
