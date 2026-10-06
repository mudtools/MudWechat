// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Abstractions;

/// <summary>
/// 企业微信多应用注册扩展（对齐 <c>FeishuMultiAppExtensions.AddFeishuApp</c>）。
/// </summary>
/// <remarks>
/// 落位于 Abstractions（而非主包）：本方法需要调用<b>本程序集</b>源生成器产出的
/// internal 注册扩展 <c>HttpClientApiExtensions.AddAuthenticationWebApiHttpClient()</c>，
/// 主包无法访问；与 FeishuV3 的落位一致。
/// </remarks>
public static class WechatWorkMultiAppExtensions
{
    /// <summary>
    /// 从配置文件读取多应用配置并注册（配置节 <c>WechatApps:0..N</c> 数组）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="sectionName">配置节名称，默认 "WechatApps"。</param>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("反射式配置绑定（ConfigurationBinder.Bind）在裁剪下无法静态分析配置类型成员")]
    [RequiresDynamicCode("反射式配置绑定（ConfigurationBinder.Bind）在 AOT/动态代码生成环境下不可用")]
#endif
    public static IServiceCollection AddWechatApp(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "WechatApps")
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(sectionName);
        var configs = new List<WechatAppConfig>();
        section.Bind(configs);

        ValidateAndSetDefaultApp(configs);
        return services.AddWechatTokenInfrastructure(configs, configuration);
    }

    /// <summary>
    /// 使用代码配置注册单个应用（多应用请链式调用或使用 <see cref="AddWechatApp(IServiceCollection, List{WechatAppConfig})"/>）。
    /// </summary>
    public static IServiceCollection AddWechatApp(
        this IServiceCollection services,
        Action<WechatAppConfig> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var config = new WechatAppConfig();
        configure(config);

        var configs = new List<WechatAppConfig> { config };
        ValidateAndSetDefaultApp(configs);
        return services.AddWechatTokenInfrastructure(configs);
    }

    /// <summary>
    /// 使用代码配置注册多个应用。
    /// </summary>
    public static IServiceCollection AddWechatApp(
        this IServiceCollection services,
        List<WechatAppConfig> configs)
    {
        if (configs == null) throw new ArgumentNullException(nameof(configs));

        ValidateAndSetDefaultApp(configs);
        return services.AddWechatTokenInfrastructure(configs);
    }

    /// <summary>
    /// 注册令牌与多应用底座（四管理器装配在 WechatAppManager 内按应用类型完成）：
    /// per-app 命名 HttpClient、认证 API、令牌仓储、AppManager（Singleton + IServiceScopeFactory）、
    /// 令牌恢复选项、后台刷新框架服务。
    /// </summary>
    internal static IServiceCollection AddWechatTokenInfrastructure(
        this IServiceCollection services,
        List<WechatAppConfig> configs,
        IConfiguration? configuration = null)
    {
        // 公用层唯一登记点：SSRF 白名单（并集单一来源）+ 令牌恢复判定器组合器 + 选项校验器 +
        // 令牌提供器 + 后台刷新框架服务。
        // 顺序敏感：白名单必须在任何客户端创建前登记；组合判定器经 PostConfigure 组装，
        // 保证不被后续 TokenRecoveryOptions 配置绑定覆盖。
        services.AddWechatTokenRecovery();

        // 企微 errcode 判定器以「子判定器」身份登记（由公用层组合器统一消费）。
        // 放在 AddWechatApp（而非 AddWechatWorkServices）之后仍可解析：判定器无构造期依赖，
        // 组合器经 IPostConfigureOptions 在**选项首次解析时**才读取 DI 集合，故注册顺序无关。
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ITokenInvalidationDetector, Authentication.WechatTokenInvalidationDetector>());

        // MT-02：带 appKey 的切换入口（UseAppScope/UseApp，二者守卫一致）默认拒绝（未注册授权器即抛出）。
        // 本 SDK 的 appKey 始终来源于 WechatAppConfig 注册表（未知 appKey 由 GetApp 校验），
        // 注册放行型授权器恢复多应用切换能力；宿主可先注册更严格实现（TryAdd 先注册者胜出）。
        services.TryAddSingleton<IAppAccessAuthorizer, AllowAllAppAccessAuthorizer>();

        // 重复 AppKey 在注册阶段即失败（命名 HttpClient / 令牌管理器键会冲突）。
        var duplicate = configs.GroupBy(c => c.AppKey, StringComparer.Ordinal).FirstOrDefault(g => g.Count() > 1);
        if (duplicate != null)
        {
            throw new InvalidOperationException(
                $"检测到重复的 AppKey '{duplicate.Key}'。每个应用的 AppKey 必须唯一。");
        }

        // P0-1（G8-A）：切换器必须与框架上下文持有器共用同一 AsyncLocal 状态。
        // 顺序敏感：下方 AddMudHttpClient 内部会以 TryAddSingleton 注册
        // IAppContextHolder → 组件 AsyncLocalAppContextSwitcher（A1 修复：任何客户端注册路径都补齐持有器）。
        // 若在此之后注册，TryAdd 失效 ⇒ 框架/生成代码读到的 IAppContextHolder.Current 恒为 null，
        // 多套件下声明式（[Token]）客户端将静默回退默认应用令牌。故必须先注册（先注册者胜）。
        services.TryAddSingleton<IWechatAppContextSwitcher, WechatAppContextSwitcher>();
        services.TryAddSingleton<IAppContextSwitcher>(sp => sp.GetRequiredService<IWechatAppContextSwitcher>());
        services.TryAddSingleton<IAppContextHolder>(sp => sp.GetRequiredService<IWechatAppContextSwitcher>());

        // per-app 命名 HttpClient：BaseAddress / Timeout / SSRF 白名单校验；
        // 默认应用与 IsDefault 严格对应（setAsDefault 决定 IEnhancedHttpClient 默认注册）。
        foreach (var config in configs)
        {
            var clientName = WechatHttpClientFactory.BuildClientName(config.AppKey);
            var baseUrl = string.IsNullOrWhiteSpace(config.BaseUrl) ? WechatApiHosts.WorkBaseUrl : config.BaseUrl;

            // P2-9：显式允许的自定义主机登记到进程级表，供 errcode 判定器的同步预过滤放行
            // （否则 AllowCustomBaseUrl=true 的私有化/网关部署会静默失去令牌恢复能力）。
            if (config.AllowCustomBaseUrl && Uri.TryCreate(baseUrl, UriKind.Absolute, out var customUri))
            {
                WechatCustomBaseUrlRegistry.Register(customUri.Host);
            }

            services.AddMudHttpClient(
                clientName,
                client =>
                {
                    UrlValidator.ValidateBaseUrl(baseUrl, config.AllowCustomBaseUrl);
                    client.BaseAddress = new Uri(baseUrl);
                    client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
                },
                setAsDefault: config.IsDefault);
        }

        // 注册认证 API（签发接口）的 DI 回退路径（per-app 实例由 WechatAppManager 经
        // IWechatAuthenticationFactory 装配，DI 注册作为单应用模式的向后兼容回退）。
        // 必须在 AddMudHttpClient 循环之后：该扩展内部 TryAdd IHttpRequestExecutor 等基础服务。
        services.AddAuthenticationWebApiHttpClient();

        // 客户端 / 认证 API 工厂。
        services.TryAddSingleton<IWechatHttpClientFactory, WechatHttpClientFactory>();
        services.TryAddSingleton<IWechatAuthenticationFactory>(sp => new PerAppWechatAuthenticationFactory(
            sp,
            sp.GetRequiredService<IWechatHttpClientFactory>(),
            sp.GetService<ILogger<PerAppWechatAuthenticationFactory>>()));

        // 令牌恢复选项的宿主配置节绑定（选项本体、校验器与判定器组合器由
        // WechatTokenRecoveryRegistration.AddWechatTokenRecovery 在本方法开头统一登记）。
        if (configuration != null)
        {
            var tokenRecoverySection = configuration.GetSection(TokenRecoveryOptions.SectionName);
            services.Configure<TokenRecoveryOptions>(options => tokenRecoverySection.Bind(options));
            services.AddSingleton<IOptionsChangeTokenSource<TokenRecoveryOptions>>(
                new ConfigurationChangeTokenSource<TokenRecoveryOptions>(Options.DefaultName, tokenRecoverySection));
        }

        // 令牌 / 授权信息 / 套件票据仓储（默认进程内实现；分布式场景由宿主预注册覆盖，TryAdd 语义）。
        services.TryAddSingleton<IWechatTokenStore, InMemoryWechatTokenStore>();
        services.TryAddSingleton<IWechatCorpAuthStore, InMemoryWechatCorpAuthStore>();
        services.TryAddSingleton<IWechatSuiteTicketStore, InMemoryWechatSuiteTicketStore>();
        services.TryAddSingleton<IWechatSuiteTicketProvider, WechatSuiteTicketProvider>();

        // 应用管理器（Singleton；懒加载上下文 + 退役队列 + IServiceScopeFactory 装配）。
        services.AddSingleton<WechatAppManager>(sp => new WechatAppManager(
            sp, configs, sp.GetRequiredService<ILogger<WechatAppManager>>()));
        services.AddSingleton<IWechatAppManager>(sp => sp.GetRequiredService<WechatAppManager>());

        // 默认应用上下文（首次解析时物化默认应用）。
        services.AddSingleton<IWechatAppContext>(sp => sp.GetRequiredService<IWechatAppManager>().GetDefaultApp());

        // 配置快照 + 校验器（IValidateOptions 管线）。
        services.Configure<List<WechatAppConfig>>(options =>
        {
            options.Clear();
            options.AddRange(configs);
        });
        services.AddSingleton<IValidateOptions<WechatAppConfig>, WechatAppConfigValidator>();
        services.AddSingleton<IValidateOptions<List<WechatAppConfig>>, WechatAppConfigValidator>();

        // 令牌提供器（框架 DefaultTokenProvider：BindTenantGuard + GetTokenManager 路由）与
        // 后台刷新框架服务均由 WechatTokenRecoveryRegistration.AddWechatTokenRecovery（方法开头）
        // 统一登记（后者带「已注册即跳过」守卫，保证多产品线共存时只有一个刷新循环）。
        // 注意：上下文切换器/持有器已在方法开头（AddMudHttpClient 之前）注册，此处不得重复注册——
        // 重复注册会因 TryAdd 语义失效而再次出现「两个独立 AsyncLocal」的 P0-1 缺陷。

#if NET6_0_OR_GREATER
        // 令牌管理器登记（HostedService）：先注册管理器，再启动后台刷新服务（IHostedService 按注册顺序启动）。
        services.AddHostedService<WechatTokenRegistrationService>();
#endif

        // 存在已配置应用时自动启用后台刷新（PostConfigure）。
        services.AddOptions<TokenRefreshBackgroundOptions>()
            .PostConfigure<IOptions<List<WechatAppConfig>>>((tokenOptions, appOptions) =>
            {
                if (appOptions.Value.Count > 0)
                {
                    tokenOptions.Enabled = true;
                }
            });

        return services;
    }

    private static void ValidateAndSetDefaultApp(List<WechatAppConfig> configs)
    {
        if (configs.Count == 0)
        {
            throw new InvalidOperationException("至少需要配置一个企业微信应用。");
        }

        foreach (var config in configs)
        {
            config.Validate();
        }

        // AppKey == "default" 自动推断为默认应用（对齐 Feishu）。
        foreach (var config in configs)
        {
            if (config.AppKey.Equals("default", StringComparison.OrdinalIgnoreCase))
            {
                config.IsDefault = true;
            }
        }

        var deduped = configs.GroupBy(c => c.AppKey, StringComparer.Ordinal).Select(g => g.Last()).ToList();
        var defaultCount = deduped.Count(c => c.IsDefault);
        if (defaultCount == 0)
        {
            configs[0].IsDefault = true;
        }
        else if (defaultCount > 1)
        {
            throw new InvalidOperationException(
                "检测到多个 IsDefault=true 的应用（" +
                string.Join(", ", deduped.Where(c => c.IsDefault).Select(c => c.AppKey)) +
                "），仅允许一个默认应用。");
        }
    }
}
