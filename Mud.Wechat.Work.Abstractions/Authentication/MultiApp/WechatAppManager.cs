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
/// 企业微信应用管理器（对齐 <c>FeishuAppManager</c> 语义，但<b>直接实现</b>
/// <see cref="IAppManager{TAppContext}"/>，不继承组件 <c>DefaultAppManager&lt;T&gt;</c>）。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><b>P1-7 为何不继承基类</b>：<c>DefaultAppManager&lt;T&gt;</c> 自持一套
/// 「appKey → 上下文实例」注册表（<c>_apps</c>）与默认键，而本类用 <c>Lazy</c> 懒加载注册表。
/// 二者并存时，基类的 <c>RegisterApp</c> / <c>UpdateApp</c> / <c>TrySetDefaultApp</c>（均非 virtual）
/// 会静默写入<b>影子注册表</b>——接口调用看似成功但 <c>GetAllApps</c>/<c>GetApp</c>（本类实现）看不到，
/// 而 <c>DefaultAppKey</c> 经接口读取基类字段恒为 <c>null</c>。任何"逐成员打补丁"都无法穷尽该风险
/// （具体类型调用仍会落影子表），故直接解除继承，使注册表<b>单一来源</b>。</item>
/// <item>Singleton + 构造注入 <see cref="IServiceScopeFactory"/>：按上下文建 scope，
/// 避免 Captive Dependency（TMA-13）；</item>
/// <item>懒加载上下文（<see cref="Lazy{T}"/>）：未访问的应用不初始化；瞬时装配故障可重建（P1-1）；</item>
/// <item>退役队列：被替换/移除的旧上下文与待清库任务在宽限期（默认 300s）后确定性执行（TMA-07/TMA-24、P1-9）；</item>
/// <item>令牌管理器三元组按应用装配：自建应用装配 <see cref="InternalAppTokenManager"/>，
/// 第三方/服务商装配 <see cref="ProviderTokenManager"/> / <see cref="SuiteTokenManager"/> /
/// <see cref="CorpTokenManager"/>（provider/suite 实现 <see cref="ISharedTokenManager"/>）。</item>
/// </list>
/// </remarks>
public class WechatAppManager : IWechatAppManager, IDisposable
{
    /// <summary>瞬时装配故障重建节流（秒）：同一应用在该窗口内最多重建一次（P1-1）。</summary>
    private const int TransientRebuildThrottleSeconds = 5;

    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory? _scopeFactory;
    private readonly ILogger<WechatAppManager> _logger;
    private readonly ConcurrentDictionary<string, Lazy<IWechatAppContext>> _lazyContexts = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, long> _lastRebuildMs = new(StringComparer.Ordinal);
    private readonly object _registryLock = new();
    private readonly List<WechatAppConfig> _configs;
    private WechatAppContextRetirement? _retirement;
    private volatile string? _defaultAppKey;
    private int _disposed;

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

    /// <summary>应用配置变更事件（新增/移除应用时触发；订阅者异常被隔离，不影响注册表状态）。</summary>
    public event EventHandler<AppConfigurationChangedEventArgs>? ConfigurationChanged;

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
    public IReadOnlyList<WechatAppConfig> ConfiguredConfigs
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
    public bool TryGetConfig(string appKey, out WechatAppConfig? config)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            config = null;
            return false;
        }

        lock (_registryLock)
        {
            var found = _configs.FirstOrDefault(c => string.Equals(c.AppKey, appKey, StringComparison.Ordinal));
            config = found;
            return found != null;
        }
    }

    /// <summary>默认应用键（注册表单一来源：本类私有字段）。</summary>
    public string? DefaultAppKey => _defaultAppKey;

    /// <inheritdoc />
    public IWechatAppContext GetDefaultApp() => GetApp(_defaultAppKey!);

    /// <inheritdoc />
    public void SetDefaultApp(string appKey)
    {
        if (appKey == null) throw new ArgumentNullException(nameof(appKey));
        ThrowIfDisposed();

        // P1-2（MT-08 同源）：把 check-then-act 收敛到注册表锁内，
        // 杜绝「设置成功后应用被并发移除」造成的默认键悬空。
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
    public IWechatAppContext GetApp(string appKey) => GetApp(appKey, allowRebuild: true);

    /// <inheritdoc />
    public bool HasApp(string appKey) => !string.IsNullOrEmpty(appKey) && _lazyContexts.ContainsKey(appKey);

    /// <inheritdoc />
    public bool TryGetApp(string appKey, out IWechatAppContext? appContext)
        => TryGetApp(appKey, out appContext, allowRebuild: true);

    /// <inheritdoc />
    public IEnumerable<IWechatAppContext> GetAllApps()
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
        ThrowIfDisposed();
        config.Validate();

        lock (_registryLock)
        {
            if (_lazyContexts.ContainsKey(config.AppKey))
            {
                throw new InvalidOperationException($"应用 '{config.AppKey}' 已存在，不能重复注册。");
            }

            if (config.IsDefault)
            {
                // P1-8：维持「唯一 IsDefault」不变量（与 WechatAppConfigValidator 的判据同源）——
                // 原实现只改默认键，会留下两个 IsDefault=true 的配置，使 Options 校验必然失败。
                foreach (var existing in _configs)
                {
                    existing.IsDefault = false;
                }

                _logger.LogInformation("运行时添加的应用 {AppKey} 已声明 IsDefault=true，原默认应用的 IsDefault 标记已清除。", config.AppKey);
                _defaultAppKey = config.AppKey;
            }

            _configs.Add(config);
            _lazyContexts[config.AppKey] = new Lazy<IWechatAppContext>(() => CreateAppContext(config), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        OnConfigurationChanged(new AppConfigurationChangedEventArgs(config.AppKey, AppConfigurationChangeType.Added));
        return GetApp(config.AppKey);
    }

    /// <inheritdoc />
    public bool RemoveApp(string appKey)
    {
        if (string.IsNullOrEmpty(appKey))
        {
            return false;
        }

        ThrowIfDisposed();

        var removed = false;
        lock (_registryLock)
        {
            var removedConfig = _configs.FirstOrDefault(c => string.Equals(c.AppKey, appKey, StringComparison.Ordinal));
            if (removedConfig != null)
            {
                _configs.Remove(removedConfig);
            }

            if (!_lazyContexts.TryRemove(appKey, out var lazy))
            {
                return false;
            }

            removed = true;

            if (lazy.IsValueCreated)
            {
                _retirement?.Enqueue(appKey, lazy.Value);
            }

            // 默认应用被移除时确定性提升（对齐 Feishu）。
            if (string.Equals(_defaultAppKey, appKey, StringComparison.Ordinal) && _configs.Count > 0)
            {
                _defaultAppKey = _configs[0].AppKey;
            }
        }

        // P1-9（R14）：下线即清库——同步 API 不做 sync-over-async，改为入队异步清理（退役队列 Timer 回调执行）。
        _retirement?.EnqueueCleanup(appKey, ct => PurgeAppTokensAsync(appKey, ct));

        OnConfigurationChanged(new AppConfigurationChangedEventArgs(appKey, AppConfigurationChangeType.Removed));
        return removed;
    }

    /// <inheritdoc />
    public async Task InvalidateTokenAsync(string appKey, string tokenType, string[]? scopes = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(appKey)) throw new ArgumentNullException(nameof(appKey));
        if (string.IsNullOrEmpty(tokenType)) throw new ArgumentNullException(nameof(tokenType));

        if (!_lazyContexts.TryGetValue(appKey, out var lazy))
        {
            throw new InvalidOperationException(
                $"未找到应用 '{appKey}'。已配置应用：{string.Join(", ", ConfiguredAppKeys)}。");
        }

        // P1-5/P1-6：已实例化（业务热路径）→ 走管理器（内存 + Store 双清，且经键控锁保证与刷新互斥）。
        if (lazy.IsValueCreated)
        {
            var manager = lazy.Value.GetTokenManager(tokenType);
            await manager.InvalidateTokenAsync(scopes, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("应用 {AppKey} 的 {TokenType} 令牌已级联失效（内存 + Store 双清）。", appKey, tokenType);
            return;
        }

        // 未实例化：内存本无条目，只需清持久层；避免为「只读失效」动作物化 APP 上下文（HttpClient/DI scope/Timer）。
        var removed = await PurgeStoreAsync(appKey, tokenType, scopes, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("应用 {AppKey} 的 {TokenType} 令牌已级联失效（持久层清理 {Count} 条）。", appKey, tokenType, removed);
    }

    /// <summary>
    /// 清理指定应用在持久层（<see cref="IWechatTokenStore"/>）中的全部令牌槽位。
    /// </summary>
    /// <param name="appKey">应用键。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>实际删除的键数量。</returns>
    /// <remarks>
    /// 用途：应用下线（<see cref="RemoveApp"/>）、凭据轮换（改了 AgentSecret / SuiteSecret 后旧令牌会被
    /// 读穿透恢复 ⇒ 持续 401）。
    /// <para>
    /// <b>多实例语义</b>：本方法能清理其它实例写入的持久层条目，但<b>不能</b>清理其它实例的进程内镜像
    /// ⇒ 失效为「最终一致」；本地镜像由管理器自身失效/TTL 负责。
    /// </para>
    /// </remarks>
    public async Task<int> PurgeAppTokensAsync(string appKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(appKey)) throw new ArgumentNullException(nameof(appKey));

        var store = _serviceProvider.GetService<IWechatTokenStore>();
        if (store == null)
        {
            return 0;
        }

        var keys = await store.GetTokenTypesAsync(cancellationToken).ConfigureAwait(false);
        var removed = 0;
        foreach (var key in keys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (key == null || !IsStoreKeyForApp(key, appKey))
            {
                continue;
            }

            await store.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
            removed++;
        }

        if (removed > 0)
        {
            _logger.LogInformation("已清理应用 {AppKey} 的持久化令牌槽位 {Count} 条。", appKey, removed);
        }

        return removed;
    }

    /// <summary>
    /// 装配应用上下文（对齐 FeishuAppManager.CreateAppContext）：
    /// 命名客户端 → 认证 API → 令牌管理器 → 恢复执行器 → 恢复型客户端 → 上下文。
    /// </summary>
    /// <param name="config">应用配置。</param>
    /// <returns>应用上下文。</returns>
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
                    // §4.8：代开发路径走 gettoken，必须注入 per-app 自建应用认证客户端（不可取 DI 默认实例）。
                    authFactory.CreateInternalAppAuthentication(config.AppKey),
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
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

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
        GC.SuppressFinalize(this);
    }

    // ---------------------------------------------------------------- IAppManager<TAppContext> 显式不支持成员
    // P1-7：本类注册表以「配置」为唯一来源，直接注册上下文实例会绕过
    // CreateAppContext 的装配链、配置校验、唯一默认维护与退役语义 ⇒ 显式拒绝（而非静默写影子注册表）。

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">始终抛出：请使用 <see cref="AddApp(WechatAppConfig)"/>。</exception>
    public void RegisterApp(string appKey, IWechatAppContext appContext, bool isDefault = false)
        => throw new NotSupportedException(
            "WechatAppManager 不支持直接注册上下文实例，请使用 AddApp(WechatAppConfig)（配置驱动的装配链）。");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">始终抛出：请使用 <see cref="AddApp(WechatAppConfig)"/>。</exception>
    public Task RegisterAppAsync(string appKey, IWechatAppContext appContext, bool isDefault = false, CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "WechatAppManager 不支持直接注册上下文实例，请使用 AddApp(WechatAppConfig)（配置驱动的装配链）。");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">始终抛出：本 SDK 不提供上下文实例替换语义。</exception>
    public void UpdateApp(string appKey, IWechatAppContext appContext)
        => throw new NotSupportedException(
            "WechatAppManager 不支持上下文实例替换，请使用 AddApp(WechatAppConfig)；如需下线请使用 RemoveApp(appKey)。");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">始终抛出：本 SDK 不提供上下文实例替换语义。</exception>
    public Task UpdateAppAsync(string appKey, IWechatAppContext appContext, CancellationToken cancellationToken = default)
        => throw new NotSupportedException(
            "WechatAppManager 不支持上下文实例替换，请使用 AddApp(WechatAppConfig)；如需下线请使用 RemoveApp(appKey)。");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">始终抛出：SDK 的切换器为 DI 单例，不按上下文实例化。</exception>
    public TContextSwitcher GetWebApi<TContextSwitcher>(string appKey)
        where TContextSwitcher : IAppContextSwitcher
        => throw new NotSupportedException(
            "WechatAppManager 的上下文切换器为 DI 单例（IWechatAppContextSwitcher），请从容器解析后使用 UseAppScope/UseDefaultAppScope。");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">始终抛出：SDK 的切换器为 DI 单例，不按上下文实例化。</exception>
    public TContextSwitcher GetDefaultWebApi<TContextSwitcher>()
        where TContextSwitcher : IAppContextSwitcher
        => throw new NotSupportedException(
            "WechatAppManager 的上下文切换器为 DI 单例（IWechatAppContextSwitcher），请从容器解析后使用 UseDefaultAppScope/UseAppScope。");

    /// <inheritdoc />
    /// <exception cref="NotSupportedException">始终抛出：SDK 的切换器为 DI 单例，无需工厂委托。</exception>
    public void RegisterSwitcherFactory<TContextSwitcher>(Func<IWechatAppContext, TContextSwitcher> factory)
        where TContextSwitcher : IAppContextSwitcher
        => throw new NotSupportedException(
            "WechatAppManager 的上下文切换器为 DI 单例（IWechatAppContextSwitcher），不支持按上下文实例化的工厂委托。");

    // ---------------------------------------------------------------- 私有实现

    /// <summary>
    /// 获取应用上下文；瞬时装配故障时重置 Lazy（P1-1）。
    /// </summary>
    /// <remarks>
    /// <see cref="Lazy{T}"/> 会<b>缓存异常</b>（含 <c>LazyThreadSafetyMode.ExecutionAndPublication</c>），
    /// 一次瞬时故障（网络/超时）后该应用将永久不可用。故对<b>白名单内的瞬时异常</b>重置 Lazy，
    /// 对确定性失败（配置/参数错误）保持直抛，避免把配置错误伪装成可重试。
    /// </remarks>
    private IWechatAppContext GetApp(string appKey, bool allowRebuild)
    {
        ThrowIfDisposed();

        if (!_lazyContexts.TryGetValue(appKey, out var lazy))
        {
            throw new InvalidOperationException(
                $"未找到应用 '{appKey}'。已配置应用：{string.Join(", ", ConfiguredAppKeys)}。");
        }

        try
        {
            return lazy.Value;
        }
        catch (Exception ex) when (allowRebuild && IsTransientInitFailure(ex))
        {
            if (TryRebuildLazy(appKey, lazy, ex))
            {
                // 只重建一次：重建后仍失败则直接抛（不递归，避免热循环）。
                return GetApp(appKey, allowRebuild: false);
            }

            throw;
        }
    }

    /// <summary>Try 语义的上下文获取（P1-1 同源：瞬时失败尝试重建一次，仍失败返回 <c>false</c>）。</summary>
    private bool TryGetApp(string appKey, out IWechatAppContext? appContext, bool allowRebuild)
    {
        appContext = null;
        if (string.IsNullOrEmpty(appKey) || !_lazyContexts.TryGetValue(appKey, out var lazy))
        {
            return false;
        }

        try
        {
            appContext = lazy.Value;
            return true;
        }
        catch (Exception ex) when (allowRebuild && IsTransientInitFailure(ex))
        {
            if (!TryRebuildLazy(appKey, lazy, ex))
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "应用 {AppKey} 上下文装配失败（TryGetApp 返回 false）。", appKey);
            return false;
        }

        return TryGetApp(appKey, out appContext, allowRebuild: false);
    }

    /// <summary>
    /// 瞬时装配故障白名单（确定性失败不重建，避免掩盖配置错误）。
    /// </summary>
    private static bool IsTransientInitFailure(Exception ex)
    {
        if (ex is OperationCanceledException)
        {
            return false;
        }

        if (ex is InvalidOperationException || ex is HttpRequestException || ex is TimeoutException
            || ex is IOException || ex is SocketException)
        {
            return true;
        }

        return ex.InnerException != null && IsTransientInitFailure(ex.InnerException);
    }

    /// <summary>重置失败的 Lazy（带节流；失败返回 <c>false</c>）。</summary>
    private bool TryRebuildLazy(string appKey, Lazy<IWechatAppContext> failed, Exception cause)
    {
        var nowMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var last = _lastRebuildMs.TryGetValue(appKey, out var value) ? value : 0L;
        if (nowMs - last < TransientRebuildThrottleSeconds * 1000L)
        {
            // 节流：瞬时故障可能持续，避免"每次调用都重建"的热循环。
            return false;
        }

        _lastRebuildMs[appKey] = nowMs;

        WechatAppConfig? config;
        lock (_registryLock)
        {
            config = _configs.FirstOrDefault(c => string.Equals(c.AppKey, appKey, StringComparison.Ordinal));
        }

        if (config == null)
        {
            return false;
        }

        var fresh = new Lazy<IWechatAppContext>(
            () => CreateAppContext(config), LazyThreadSafetyMode.ExecutionAndPublication);

        if (!_lazyContexts.TryUpdate(appKey, fresh, failed))
        {
            return false;
        }

        _logger.LogWarning(cause, "应用 {AppKey} 上下文装配遇瞬时故障，已重置 Lazy（下次调用重建）。", appKey);
        return true;
    }

    /// <summary>
    /// 清理持久层中指定应用 + 令牌类型的槽位（可选按 scope 过滤）。
    /// </summary>
    private async Task<int> PurgeStoreAsync(
        string appKey, string tokenType, string[]? scopes, CancellationToken cancellationToken)
    {
        var store = _serviceProvider.GetService<IWechatTokenStore>();
        if (store == null)
        {
            return 0;
        }

        var keys = await store.GetTokenTypesAsync(cancellationToken).ConfigureAwait(false);
        var removed = 0;

        foreach (var key in keys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (key == null || !TryParseStoreKey(key, out var parsedTokenType, out var parsedAppKey, out var scopeKey))
            {
                continue;
            }

            if (!string.Equals(parsedAppKey, appKey, StringComparison.Ordinal)
                || !string.Equals(parsedTokenType, tokenType, StringComparison.Ordinal))
            {
                continue;
            }

            if (scopes is { Length: > 0 } && !MatchesScope(scopeKey, scopes))
            {
                continue;
            }

            await store.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
            removed++;
        }

        return removed;
    }

    /// <summary>
    /// 判断持久层键是否属于指定应用。键布局为 <c>{tokenType}:{appKey}:{scopeKey}</c>
    /// （由 <c>WechatAppTokenManagerBase.BuildCache</c> 的 storeKeyMapper 构造），按第二段精确比对。
    /// </summary>
    private static bool IsStoreKeyForApp(string storeKey, string appKey)
        => TryParseStoreKey(storeKey, out _, out var parsedAppKey, out _)
           && string.Equals(parsedAppKey, appKey, StringComparison.Ordinal);

    /// <summary>
    /// 解析持久层键的三段结构 <c>{tokenType}:{appKey}:{scopeKey}</c>。
    /// </summary>
    /// <remarks>
    /// <c>tokenType</c> 为 <see cref="WechatTokenTypes"/> 常量（形如 <c>Wechat.AccessToken</c>，不含 <c>:</c>），
    /// 故以<b>首个</b> <c>:</c> 分隔 tokenType、<b>第二个</b> <c>:</c> 分隔 appKey；
    /// <c>scopeKey</c> 可能含 <c>:</c>，取剩余全部（AppKey 形状校验保证第二段不含 <c>:</c>，见 P1-7）。
    /// </remarks>
    private static bool TryParseStoreKey(string storeKey, out string tokenType, out string appKey, out string scopeKey)
    {
        tokenType = string.Empty;
        appKey = string.Empty;
        scopeKey = string.Empty;

        var firstColon = storeKey.IndexOf(':');
        if (firstColon <= 0)
        {
            return false;
        }

        var secondColon = storeKey.IndexOf(':', firstColon + 1);
        if (secondColon < firstColon)
        {
            return false;
        }

        tokenType = storeKey.Substring(0, firstColon);
        appKey = secondColon > firstColon + 1
            ? storeKey.Substring(firstColon + 1, secondColon - firstColon - 1)
            : string.Empty;
        scopeKey = secondColon >= 0 && secondColon + 1 < storeKey.Length
            ? storeKey.Substring(secondColon + 1)
            : string.Empty;

        return appKey.Length > 0;
    }

    private static bool MatchesScope(string scopeKey, string[] scopes)
    {
        foreach (var scope in scopes)
        {
            if (string.IsNullOrEmpty(scope))
            {
                continue;
            }

            // 精确命中，或形如 "{scope}:{子维度}" 的前缀命中（scopeKey 可能被基座拼接扩展）。
            if (string.Equals(scopeKey, scope, StringComparison.Ordinal)
                || scopeKey.StartsWith(scope + ":", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>触发配置变更事件（逐订阅者隔离：单个订阅者异常不得把已提交的状态变更表现为注册失败）。</summary>
    private void OnConfigurationChanged(AppConfigurationChangedEventArgs e)
    {
        var handlers = ConfigurationChanged;
        if (handlers is null)
        {
            return;
        }

        foreach (var handler in handlers.GetInvocationList())
        {
            try
            {
                ((EventHandler<AppConfigurationChangedEventArgs>)handler).Invoke(this, e);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "应用配置变更事件订阅者执行失败（AppKey: {AppKey}）。", e.AppKey);
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed != 0)
        {
            throw new ObjectDisposedException(nameof(WechatAppManager));
        }
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
