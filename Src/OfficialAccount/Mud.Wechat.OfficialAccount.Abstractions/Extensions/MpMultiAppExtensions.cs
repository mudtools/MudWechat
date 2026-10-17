// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Mud.Wechat.Abstractions.TokenManager;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;
using Mud.Wechat.OfficialAccount.Abstractions.Configuration;

namespace Mud.Wechat.OfficialAccount.Abstractions;

/// <summary>
/// 公众号多应用注册扩展（对齐企微 <c>AddWechatApp</c> / <c>AddFeishuApp</c>）。
/// </summary>
/// <remarks>
/// 落位于 Abstractions（而非主包）：本方法需要调用<b>本程序集</b>源生成器产出的
/// internal 注册扩展 <c>HttpClientApiExtensions.AddAuthenticationWebApiHttpClient()</c>，
/// 主包无法访问。
/// </remarks>
public static class MpMultiAppExtensions
{
    /// <summary>
    /// 从配置文件读取多公众号配置并注册（配置节 <c>MpApps:0..N</c> 数组）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="sectionName">配置节名称，默认 "MpApps"。</param>
    /// <returns>服务集合（链式）。</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("反射式配置绑定（ConfigurationBinder.Bind）在裁剪下无法静态分析配置类型成员")]
    [RequiresDynamicCode("反射式配置绑定（ConfigurationBinder.Bind）在 AOT/动态代码生成环境下不可用")]
#endif
    public static IServiceCollection AddMpApp(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "MpApps")
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var section = configuration.GetSection(sectionName);
        var configs = new List<MpAppConfig>();
        section.Bind(configs);

        ValidateAndSetDefaultApp(configs);
        return services.AddMpTokenInfrastructure(configs, configuration);
    }

    /// <summary>使用代码配置注册单个公众号。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddMpApp(
        this IServiceCollection services,
        Action<MpAppConfig> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var config = new MpAppConfig();
        configure(config);

        var configs = new List<MpAppConfig> { config };
        ValidateAndSetDefaultApp(configs);
        return services.AddMpTokenInfrastructure(configs);
    }

    /// <summary>使用代码配置注册多个公众号。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configs">配置列表。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddMpApp(
        this IServiceCollection services,
        List<MpAppConfig> configs)
    {
        if (configs == null) throw new ArgumentNullException(nameof(configs));

        ValidateAndSetDefaultApp(configs);
        return services.AddMpTokenInfrastructure(configs);
    }

    /// <summary>
    /// 注册令牌与多公众号底座：per-app 命名 HttpClient、令牌签发客户端、令牌仓储、
    /// AppManager（Singleton + IServiceScopeFactory）、令牌恢复设施（公用层单点登记）。
    /// </summary>
    private static IServiceCollection AddMpTokenInfrastructure(
        this IServiceCollection services,
        List<MpAppConfig> configs,
        IConfiguration? configuration = null)
    {
        // 重复调用防护：MpAppManager 是单例且以「本次传入的配置列表」为注册表唯一来源，
        // 重复 AddMpApp 会让后一次注册的管理器覆盖前一次（先注册的公众号<b>静默丢失</b>）。
        // 故 fail-fast 并指明正确用法（多公众号请用 List 重载一次注册）。
        if (services.Any(descriptor => descriptor.ServiceType == typeof(IMpAppManager)))
        {
            throw new InvalidOperationException(
                "AddMpApp 已被调用过。多公众号请使用 AddMpApp(List<MpAppConfig>) 一次性注册全部应用" +
                "（重复调用会以最后一次的配置列表覆盖注册表，导致先注册的公众号静默丢失）。");
        }

        // ① 官方字符集硬约束的修复（errcode 40033）：
        //    官方「创建自定义菜单 / 创建个性化菜单」错误码明确要求「请求不能包含 \uxxxx 格式的字符，
        //    否则创建失败」；而 .NET 源生成 JSON 上下文默认使用 JavaScriptEncoder.Default，
        //    会把所有非 ASCII 字符（中文菜单名、中文标签名等）转义为 \uXXXX ⇒ 中文菜单直接创建失败。
        //    故把组件序列化管线所用 JsonSerializerOptions 的 Encoder 放宽为 UnsafeRelaxedJsonEscaping：
        //    仅改变「编码形式」（不再转义非 ASCII），不改变 JSON 结构，转义安全性由 JSON 规范本身保证
        //    （引号与反斜杠仍被转义）；库内遥测另有 DefaultSensitiveDataMasker 独立脱敏，不受影响。
        //    组件 IHttpContentSerializer 是首次解析时经 IOptions<JsonSerializerOptions> 构造的单例，
        //    故此处的 Configure 必须在任何客户端解析之前登记（本方法即注册入口，满足该顺序约束）。
        services.Configure<JsonSerializerOptions>(options =>
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping);

        // ② 公用层唯一登记点：SSRF 白名单（并集单一来源）+ 令牌恢复判定器组合器 + 选项校验器 +
        // 令牌提供器 + 后台刷新框架服务（后者带「已注册即跳过」守卫，多产品线共存时只有一个刷新循环）。
        services.AddWechatTokenRecovery();

        // 带 appKey 的切换入口的授权判定：appKey 白名单（名单 = 本次注册的应用集合，与注册表同源，
        // 与企微线同形）。TryAdd ⇒ 宿主仍可先注册更严格实现（TryAdd 先注册者胜出）。
        services.TryAddSingleton<IAppAccessAuthorizer>(
            _ => new AppKeyAllowListAuthorizer(configs.Select(c => c.AppKey)));

        // 重复 AppKey 在注册阶段即失败（命名 HttpClient / 令牌管理器键会冲突）。
        var duplicate = configs.GroupBy(c => c.AppKey, StringComparer.Ordinal).FirstOrDefault(g => g.Count() > 1);
        if (duplicate != null)
        {
            throw new InvalidOperationException(
                $"检测到重复的 AppKey '{duplicate.Key}'。每个应用的 AppKey 必须唯一。");
        }

        // DI 桥接不变量：切换器必须与组件上下文持有器共用同一 AsyncLocal 状态。
        // 组件（Mud.HttpUtils ≥ 3.0.4）默认注册为委托式适配器（DelegatingAppContextHolder，internal）：
        // 本 holder 桥接为「无条件注册（最后注册者胜）」，与企微线同形——无论组件是否先注册，
        // IAppContextHolder 最终都解析到本 SDK 切换器（详见 WechatWorkMultiAppExtensions 同位置注释）。
        services.TryAddSingleton<IMpAppContextSwitcher, MpAppContextSwitcher>();
        services.TryAddSingleton<IAppContextSwitcher>(sp => sp.GetRequiredService<IMpAppContextSwitcher>());
        services.AddSingleton<IAppContextHolder>(sp => sp.GetRequiredService<IMpAppContextSwitcher>());

        // per-app 命名 HttpClient：BaseAddress / Timeout / SSRF 白名单校验。
        // 热更语义（实测结论，W-18）：BaseAddress/Timeout 在 ConfigureHttpClient 委托内赋值（每次 CreateClient 重跑）
        // ⇒ 配置生效不依赖框架缓存失效；本仓不使用 MudHttpClientApplicationOptions，上游 per-client
        // 指纹 Invalidate 对本仓无效果面，勿补自建失效桥接。
        foreach (var config in configs)
        {
            var clientName = MpHttpClientFactory.BuildClientName(config.AppKey);
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

        // 注册令牌签发客户端（per-app 实例由 MpAppManager 经 IMpAuthenticationFactory 装配，
        // DI 注册作为单应用模式的回退）。必须在 AddMudHttpClient 循环之后。
        services.AddAuthenticationWebApiHttpClient();

        // 客户端 / 认证 API 工厂。
        services.TryAddSingleton<IMpHttpClientFactory, MpHttpClientFactory>();
        services.TryAddSingleton<IMpAuthenticationFactory>(sp => new PerAppMpAuthenticationFactory(
            sp,
            sp.GetRequiredService<IMpHttpClientFactory>(),
            sp.GetService<ILogger<PerAppMpAuthenticationFactory>>()));

        // 票据 API 工厂（per-app 票据客户端；由 MpAppManager 装配票据管理器时使用）。
        // 官方对本端点的调用频次有硬约束（「次数非常有限，频繁刷新会导致调用受限」），
        // 故票据一律经管理器缓存取用，不注册可供宿主逐次直调的默认客户端（避免误用面）。
        services.TryAddSingleton<IMpTicketFactory>(sp => new PerAppMpTicketFactory(
            sp,
            sp.GetRequiredService<IMpHttpClientFactory>(),
            sp.GetService<ILogger<PerAppMpTicketFactory>>()));

        // 公众号 errcode 失效判定器（子判定器身份；由公用层组合器统一消费）。
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ITokenInvalidationDetector, MpTokenInvalidationDetector>());

        // 令牌恢复选项的宿主配置节绑定（选项本体与校验器由公用层统一登记）。
        // 绑定走"纯 DTO + Apply"两段式（与企微线同形）：TokenRecoveryOptions 含接口型判定器属性，
        // 直接 Bind 会产 SYSLIB1100/1101；TokenRecoveryConfiguration 只含可生成字段，Apply 不触碰
        // 判定器槽位（公用层 PostConfigure 组装不受影响）。
        if (configuration != null)
        {
            var tokenRecoverySection = configuration.GetSection(TokenRecoveryOptions.SectionName);

            // "纯 DTO + Apply"两段式内联在 Configure 委托中（与企微线同形；不引入第二个选项缓存——
            // 否则纯 DTO 的 IOptions 缓存没有变更令牌源，配置重载后 Apply 会拿到旧值）。
            services.Configure<TokenRecoveryOptions>(o =>
            {
                var dto = new TokenRecoveryConfiguration();
                tokenRecoverySection.Bind(dto);
                o.Apply(dto);
            });

            // TryAdd：多产品线共用同名节（MudHttpTokenRecovery），同一宿主只需一份（先注册者胜）。
            services.TryAddSingleton<IOptionsChangeTokenSource<TokenRecoveryOptions>>(
                new ConfigurationChangeTokenSource<TokenRecoveryOptions>(Options.DefaultName, tokenRecoverySection));
        }

        // 令牌仓储（默认进程内实现；分布式场景由宿主预注册覆盖，TryAdd 语义）。
        // 与企微产品线共用同一端口与键约定（键前缀含产品线令牌类型，故互不覆盖）。
        services.TryAddSingleton<IWechatTokenStore, InMemoryWechatTokenStore>();

        // 应用管理器（Singleton；懒加载上下文 + IServiceScopeFactory 装配）。
        services.AddSingleton<MpAppManager>(sp => new MpAppManager(
            sp, configs, sp.GetRequiredService<ILogger<MpAppManager>>()));
        services.AddSingleton<IMpAppManager>(sp => sp.GetRequiredService<MpAppManager>());

        // 默认应用上下文（首次解析时物化默认应用）。
        services.AddSingleton<IMpAppContext>(sp => sp.GetRequiredService<IMpAppManager>().GetDefaultApp());

        // JS-SDK 票据管理器的根容器桥接。
        // 票据管理器是「按应用」装配在上下文内的（MpAppManager.CreateAppContext ⇒ MpAppContext.GetService 转发），
        // 根容器本身没有该项；而签名服务 IMpJsApiSignatureService 与宿主的构造注入面都从根容器解析它
        // —— 缺此桥接时 ValidateOnBuild 直接把宿主打在建容器阶段。
        // TryAdd ⇒ 多公众号宿主可先注册「按当前上下文取票据」的实现覆盖本默认桥（先注册者胜）。
        services.TryAddSingleton<IMpJsApiTicketManager>(static sp =>
            sp.GetRequiredService<IMpAppContext>().GetService<IMpJsApiTicketManager>()
            ?? throw new InvalidOperationException(
                "默认公众号上下文未装配 jsapi 票据管理器，无法生成 JS-SDK 签名（请确认 AddMpApp 已完成应用注册）。"));

        // 未命名 IOptions<MpAppConfig> = 默认应用配置的快照（与上面的 IMpAppContext / 票据桥同义，均为「默认应用」）。
        // 本方法只登记 List<MpAppConfig> 快照与逐应用命名客户端，未命名项此前恒为空 ⇒ 以 IOptions<MpAppConfig>
        // 取 AppId 的组件（IMpJsApiSignatureService）在解析期被 MpAppConfigValidator 打回「AppKey 不能为空」，
        // 宿主按 README 注入签名服务即在建容器阶段失败。配置本体已在 ValidateAndSetDefaultApp 逐份 Validate()，
        // 故此处直接包裹默认应用实例（不再二次物化），多公众号宿主改取 IMpAppContext.Config。
        services.TryAddSingleton<IOptions<MpAppConfig>>(
            static sp => new OptionsWrapper<MpAppConfig>(sp.GetRequiredService<IMpAppContext>().Config));

        // 配置快照 + 校验器（IValidateOptions 管线）。
        services.Configure<List<MpAppConfig>>(options =>
        {
            options.Clear();
            options.AddRange(configs);
        });
        services.AddSingleton<IValidateOptions<MpAppConfig>, MpAppConfigValidator>();
        services.AddSingleton<IValidateOptions<List<MpAppConfig>>, MpAppConfigValidator>();

#if NET6_0_OR_GREATER
        // 令牌管理器登记（HostedService）：先注册管理器，再启动后台刷新服务（按注册顺序启动）。
        services.AddHostedService<MpTokenRegistrationService>();
#endif

        // 存在已配置公众号时自动启用后台刷新（PostConfigure）。
        services.AddOptions<TokenRefreshBackgroundOptions>()
            .PostConfigure<IOptions<List<MpAppConfig>>>((tokenOptions, appOptions) =>
            {
                if (appOptions.Value.Count > 0)
                {
                    tokenOptions.Enabled = true;
                }
            });

        return services;
    }

    private static void ValidateAndSetDefaultApp(List<MpAppConfig> configs)
    {
        if (configs.Count == 0)
        {
            throw new InvalidOperationException("至少需要配置一个微信公众号。");
        }

        foreach (var config in configs)
        {
            config.Validate();
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
