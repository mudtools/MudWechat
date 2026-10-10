// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Mud.Wechat.Channels.Abstractions.Authentication;
using Mud.Wechat.Channels.Abstractions.Authentication.MultiApp;
using Mud.Wechat.Channels.Abstractions.Configuration;
using Mud.Wechat.Channels.Abstractions.Exceptions;

namespace Mud.Wechat.Channels.Abstractions;

/// <summary>
/// 微信小店 / 视频号多应用注册扩展（对齐企微 <c>AddWechatApp</c> / 公众号 <c>AddMpApp</c>）。
/// </summary>
/// <remarks>
/// <para>
/// 落位于 Abstractions（而非主包）：本方法需要调用<b>本程序集</b>源生成器产出的
/// internal 注册扩展 <c>HttpClientApiExtensions.AddAuthenticationWebApiHttpClient()</c>，
/// 主包无法访问。
/// </para>
/// <para>
/// <b>配置绑定形态（设计方案 v1 §3.2 / P0-b）</b>：配置绑定经
/// <c>Configure&lt;List&lt;ChannelsAppConfig&gt;&gt;(o =&gt; …)</c> 动作注册
/// （源生成配置绑定器 <c>EnableConfigurationBindingGenerator</c> 拦截 <c>Bind</c> 调用生成绑定代码），
/// <b>不使用</b> <c>Configure&lt;T&gt;(IConfiguration)</c> 反射重载（破坏 <c>IL2026</c>/<c>IL3050</c> 净零）。
/// 默认应用归一（<c>AppKey == "default"</c> 自动推断 / 无默认时取首个）经 <c>PostConfigure</c> 执行，
/// 再由 <c>ChannelsAppConfigValidator</c> 校验。
/// </para>
/// </remarks>
public static class ChannelsMultiAppExtensions
{
    /// <summary>默认配置节名（小店应用列表）。</summary>
    public const string DefaultSectionName = "ChannelsApps";

    /// <summary>
    /// 从配置文件读取多小店配置并注册（配置节 <c>ChannelsApps:0..N</c> 数组）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="sectionName">配置节名称，默认 <c>ChannelsApps</c>。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <remarks>
    /// 注册期以<see cref="ConfigurationBinder"/> 源生成绑定读取一次配置（用于 per-app 命名 HttpClient
    /// 与 AppManager 装配，二者都必须在注册期完成）；Options 快照同时写入
    /// <c>IOptions&lt;List&lt;ChannelsAppConfig&gt;&gt;</c>（对齐公众号 / 企微同款「绑定 → 快照」形态）。
    /// </remarks>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("配置绑定（ConfigurationBinder.Bind）在裁剪下无法静态分析配置类型成员（源生成绑定器已拦截，本标注为兜底）")]
    [RequiresDynamicCode("配置绑定（ConfigurationBinder.Bind）在 AOT/动态代码生成环境下不可用（源生成绑定器已拦截，本标注为兜底）")]
#endif
    public static IServiceCollection AddChannelsApp(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSectionName)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(sectionName);
        var configs = new List<ChannelsAppConfig>();
        section.Bind(configs);

        if (configs.Count == 0)
        {
            throw new InvalidOperationException(
                $"配置节 '{sectionName}' 未解析到任何小店应用。请检查配置（ChannelsApps:0..N 数组）。");
        }

        NormalizeAndValidate(configs);
        return services.AddChannelsTokenInfrastructure(configs);
    }

    /// <summary>使用代码配置注册单个小店应用。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddChannelsApp(
        this IServiceCollection services,
        Action<ChannelsAppConfig> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var config = new ChannelsAppConfig();
        configure(config);

        var configs = new List<ChannelsAppConfig> { config };
        NormalizeAndValidate(configs);
        return services.AddChannelsTokenInfrastructure(configs);
    }

    /// <summary>使用代码配置注册多个小店应用。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configs">配置列表。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddChannelsApp(
        this IServiceCollection services,
        List<ChannelsAppConfig> configs)
    {
        if (configs == null) throw new ArgumentNullException(nameof(configs));
        if (configs.Count == 0)
        {
            throw new InvalidOperationException("至少需要配置一个微信小店应用。");
        }

        NormalizeAndValidate(configs);
        return services.AddChannelsTokenInfrastructure(configs);
    }

    /// <summary>
    /// 注册令牌与多小店底座：per-app 命名 HttpClient、令牌签发客户端、令牌仓储、
    /// AppManager（Singleton + IServiceScopeFactory）、令牌恢复设施（公用层单点登记）。
    /// </summary>
    internal static IServiceCollection AddChannelsTokenInfrastructure(
        this IServiceCollection services,
        List<ChannelsAppConfig> configs)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configs == null) throw new ArgumentNullException(nameof(configs));

        // 重复调用防护：ChannelsAppManager 是单例且以「本次传入的配置列表」为注册表唯一来源，
        // 重复 AddChannelsApp 会让后一次注册的管理器覆盖前一次（先注册的小店<b>静默丢失</b>）。
        // 故 fail-fast 并指明正确用法（多小店请用 List 重载一次注册）。
        if (services.Any(descriptor => descriptor.ServiceType == typeof(IChannelsAppManager)))
        {
            throw new InvalidOperationException(
                "AddChannelsApp 已被调用过。多小店请使用 AddChannelsApp(List<ChannelsAppConfig>) 一次性注册全部应用" +
                "（重复调用会以最后一次的配置列表覆盖注册表，导致先注册的小店静默丢失）。");
        }

        // ① 官方字符集语义对齐公众号线：把组件序列化管线所用 JsonSerializerOptions 的 Encoder 放宽为
        //    UnsafeRelaxedJsonEscaping（中文商品名 / 地址等非 ASCII 不再被转义为 \uXXXX）。
        //    仅改变「编码形式」，不改变 JSON 结构；库内遥测另有 DefaultSensitiveDataMasker 独立脱敏。
        //    组件 IHttpContentSerializer 是首次解析时经 IOptions<JsonSerializerOptions> 构造的单例，
        //    故此处的 Configure 必须在任何客户端解析之前登记（本方法即注册入口，满足该顺序约束）。
        services.Configure<JsonSerializerOptions>(options =>
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping);

        // ② 公用层唯一登记点：SSRF 白名单（并集单一来源）+ 令牌恢复判定器组合器 + 选项校验器 +
        // 令牌提供器 + 后台刷新框架服务（后者带「已注册即跳过」守卫，多产品线共存时只有一个刷新循环）。
        services.AddWechatTokenRecovery();

        // 带 appKey 的切换入口默认拒绝（未注册授权器即抛出）；本 SDK 的 appKey 始终来源于注册表，
        // 注册放行型授权器恢复多应用切换能力；宿主可先注册更严格实现（TryAdd 先注册者胜出）。
        services.TryAddSingleton<IAppAccessAuthorizer, AllowAllAppAccessAuthorizer>();

        // 重复 AppKey 在注册阶段即失败（命名 HttpClient / 令牌管理器键会冲突）。
        var duplicate = configs.GroupBy(c => c.AppKey, StringComparer.Ordinal).FirstOrDefault(g => g.Count() > 1);
        if (duplicate != null)
        {
            throw new InvalidOperationException(
                $"检测到重复的 AppKey '{duplicate.Key}'。每个应用的 AppKey 必须唯一。");
        }

        // DI 桥接不变量：切换器必须与组件上下文持有器共用同一 AsyncLocal 状态。
        // 顺序敏感：下方 AddMudHttpClient 内部会以 TryAddSingleton 注册 IAppContextHolder ⇒
        // 必须先注册（先注册者胜），否则声明式（[Token]）客户端读到的上下文恒为 null。
        services.TryAddSingleton<IChannelsAppContextSwitcher, ChannelsAppContextSwitcher>();
        services.TryAddSingleton<IAppContextSwitcher>(sp => sp.GetRequiredService<IChannelsAppContextSwitcher>());
        services.TryAddSingleton<IAppContextHolder>(sp => sp.GetRequiredService<IChannelsAppContextSwitcher>());

        // per-app 命名 HttpClient：BaseAddress / Timeout / SSRF 白名单校验。
        // 命名客户端是惰性的 —— 只调用 AddChannelsApp 而不发业务请求不会真的建连。
        foreach (var config in configs)
        {
            var clientName = ChannelsHttpClientFactory.BuildClientName(config.AppKey);
            var baseUrl = string.IsNullOrWhiteSpace(config.BaseUrl) ? WechatApiHosts.OfficialAccountBaseUrl : config.BaseUrl;

            // 显式允许的自定义主机登记到进程级表，供 errcode 判定器的同步预过滤放行
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

        // 注册令牌签发客户端（per-app 实例由 ChannelsAppManager 经 IChannelsAuthenticationFactory 装配，
        // DI 注册作为单应用模式的回退）。必须在 AddMudHttpClient 循环之后。
        services.AddAuthenticationWebApiHttpClient();

        // 客户端 / 认证 API 工厂。
        services.TryAddSingleton<IChannelsHttpClientFactory, ChannelsHttpClientFactory>();
        services.TryAddSingleton<IChannelsAuthenticationFactory>(sp => new PerAppChannelsAuthenticationFactory(
            sp,
            sp.GetRequiredService<IChannelsHttpClientFactory>(),
            sp.GetService<ILogger<PerAppChannelsAuthenticationFactory>>()));

        // 小店 errcode 失效判定器（子判定器身份；由公用层组合器统一消费）。
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ITokenInvalidationDetector, ChannelsTokenInvalidationDetector>());

        // 令牌仓储（默认进程内实现；分布式场景由宿主预注册覆盖，TryAdd 语义）。
        // 与其它产品线共用同一端口与键约定（键前缀含产品线令牌类型，故互不覆盖）。
        services.TryAddSingleton<IWechatTokenStore, InMemoryWechatTokenStore>();

        // 应用管理器（Singleton；懒加载上下文 + IServiceScopeFactory 装配）。
        services.AddSingleton<ChannelsAppManager>(sp => new ChannelsAppManager(
            sp, configs, sp.GetRequiredService<ILogger<ChannelsAppManager>>()));
        services.AddSingleton<IChannelsAppManager>(sp => sp.GetRequiredService<ChannelsAppManager>());

        // 默认应用上下文（首次解析时物化默认应用）。
        services.AddSingleton<IChannelsAppContext>(sp => sp.GetRequiredService<IChannelsAppManager>().GetDefaultApp());

        // 配置快照 + 校验器（IValidateOptions 管线）。
        services.Configure<List<ChannelsAppConfig>>(options =>
        {
            options.Clear();
            options.AddRange(configs);
        });
        services.AddSingleton<IValidateOptions<ChannelsAppConfig>, ChannelsAppConfigValidator>();
        services.AddSingleton<IValidateOptions<List<ChannelsAppConfig>>, ChannelsAppConfigValidator>();

#if NET6_0_OR_GREATER
        // 令牌管理器登记（HostedService）：先注册管理器，再启动后台刷新服务（按注册顺序启动）。
        services.AddHostedService<ChannelsTokenRegistrationService>();
#endif

        // 存在已配置小店时自动启用后台刷新（PostConfigure）。
        services.AddOptions<TokenRefreshBackgroundOptions>()
            .PostConfigure<IOptions<List<ChannelsAppConfig>>>((tokenOptions, appOptions) =>
            {
                if (appOptions.Value.Count > 0)
                {
                    tokenOptions.Enabled = true;
                }
            });

        return services;
    }

    /// <summary>
    /// 注册期统一校验 + 默认应用归一（对齐公众号 <c>ValidateAndSetDefaultApp</c>）。
    /// 归一必须早于 <c>AddMudHttpClient</c> 循环（<c>setAsDefault</c> 依赖归一后的 <c>IsDefault</c>）。
    /// </summary>
    private static void NormalizeAndValidate(List<ChannelsAppConfig> configs)
    {
        foreach (var config in configs)
        {
            try
            {
                config.Validate();
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException($"ChannelsAppConfig 配置验证失败: {ex.Message}", nameof(configs), ex);
            }
        }

        // AppKey == "default" 自动推断为默认应用。
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