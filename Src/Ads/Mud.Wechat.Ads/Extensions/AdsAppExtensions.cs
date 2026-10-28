// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Mud.Wechat.Ads.Abstractions.Auth;
using Mud.Wechat.Ads.Abstractions.Configuration;
using Mud.Wechat.Ads.Abstractions.Transport;

namespace Mud.Wechat.Ads.Abstractions.Extensions;

/// <summary>
/// 腾讯广告应用底座注册入口（对齐 <c>AddWechatApp</c> / <c>AddMpApp</c> / <c>AddPayApp</c> 的命名与三重载形态）。
/// </summary>
/// <remarks>
/// <para>
/// 典型用法：
/// <code>
/// services.AddAdsApp(configuration, "WechatAds");
/// services.AddWechatAdsApi(builder => builder.AddAllApis());
/// </code>
/// </para>
/// <para>
/// <b>与 <c>AddWechatAdsApi()</c> 的分工</b>：本入口只装配<b>授权与传输底座</b>
/// （配置 → 多应用注册表 → OAuth 编排 → 两个命名客户端）；<c>AddWechatAdsApi()</c> 负责<b>业务接口</b>
/// （<c>AdsModule</c> 域注册）。两者可各自单独调用 —— 只做授权引导（换码 / 刷新）的宿主不需要拉起业务客户端。
/// </para>
/// <para>
/// <b>广告线的接入点与超时取默认应用配置</b>（业务客户端）：v3.0 官方只有一台接入点
/// （<c>https://api.e.qq.com</c>），多应用是「多个 <c>client_id</c> 打同一台接入点」，
/// 因此命名客户端是<b>每线一对</b>而非每应用一对，<see cref="WechatAppConfigBase.BaseUrl"/> 与
/// <see cref="WechatAppConfigBase.TimeoutSeconds"/> 由默认应用取值。
/// OAuth 一支不受此限制 —— 它按 <c>config.BaseUrl</c> 组装<b>绝对</b> URI，因此逐应用生效。
/// 需要逐应用不同接入点/超时的部署：每个进程只注册一个默认应用。
/// </para>
/// </remarks>
public static class AdsAppExtensions
{
    /// <summary>默认配置节名。</summary>
    public const string DefaultSectionName = "WechatAds";

    /// <summary>
    /// 从配置节注册广告应用底座（绑定 <c>List&lt;AdsAppConfig&gt;</c>）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="sectionName">配置节名称，默认 <c>WechatAds</c>。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <remarks>绑定走<b>源生成器</b>（<c>EnableConfigurationBindingGenerator</c>）：显式 <c>Bind(list)</c>，<b>不</b>用 <c>Configure&lt;T&gt;(IConfiguration)</c> 反射重载。</remarks>
    public static IServiceCollection AddAdsApp(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = DefaultSectionName)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var configs = new List<AdsAppConfig>();
        configuration.GetSection(sectionName).Bind(configs);

        return services.AddAdsInfrastructure(configs);
    }

    /// <summary>
    /// 用代码注册单个广告应用（多个请用 <see cref="AddAdsApp(IServiceCollection, List{AdsAppConfig})"/>）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">应用配置委托。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddAdsApp(
        this IServiceCollection services,
        Action<AdsAppConfig> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var config = new AdsAppConfig();
        configure(config);

        return services.AddAdsInfrastructure(new List<AdsAppConfig> { config });
    }

    /// <summary>
    /// 一次性注册多个广告应用（多 <c>client_id</c>）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configs">应用配置集合。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddAdsApp(
        this IServiceCollection services,
        List<AdsAppConfig> configs)
    {
        if (configs == null)
        {
            throw new ArgumentNullException(nameof(configs));
        }

        return services.AddAdsInfrastructure(configs);
    }

    /// <summary>
    /// 装配授权与传输底座：应用注册表 + 时钟缝 + 授权状态存储 + OAuth 编排 + 两个命名客户端 + AOT 上下文登记。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>注册期即构造 <see cref="AdsAppManager"/></b>：把「凭据缺失 / AppKey 重复 / 多应用未标默认」
    /// 钉在 <c>StartUp</c> 而不是第一次真实请求 —— 广告线的 refresh_token 是一次性凭据，
    /// 一次半装配状态的误用可能直接作废一份需要人工重新授权的令牌对。
    /// </para>
    /// <para>
    /// <b>切换器与管理器共用同一实例</b>：<see cref="AdsAppContextSwitcher"/> 的作用域状态是
    /// <c>static AsyncLocal</c>，故即使宿主预注册了自己的 <see cref="IAdsAppContextSwitcher"/>
    /// （<c>TryAdd</c> 让位），本处建的管理器读到的仍是同一份环境态。宿主若替换为<b>有实例状态</b>的
    /// 切换器实现，必须同时预注册自己的 <see cref="IAdsAppManager"/>。
    /// </para>
    /// <para>
    /// <b>重复调用防护</b>：注册表唯一来源是本次传入的配置列表，第二次 <c>AddAdsApp</c> 会让后一次覆盖前一次
    /// （先注册的应用<b>静默丢失</b>）⇒ 直接 fail-fast，正确用法是用 <c>List</c> 重载一次注册全部应用。
    /// </para>
    /// </remarks>
    internal static IServiceCollection AddAdsInfrastructure(
        this IServiceCollection services, List<AdsAppConfig> configs)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (services.Any(static d => d.ServiceType == typeof(IAdsAppManager)))
        {
            throw new InvalidOperationException(
                "AddAdsApp 已被调用过。多应用请使用 AddAdsApp(List<AdsAppConfig>) 一次性注册全部应用" +
                "（重复调用会以最后一次的配置列表覆盖注册表，导致先注册的应用静默丢失）。");
        }

        if (configs.Count == 0)
        {
            // 空集合几乎总是「配置节名写错 / 节缺失」而非有意为之；此时 Bind 得到空列表且**不报错**，
            // 拖到第一次取令牌才抛「未注册任何应用」会让人误以为是凭据问题。
            throw new InvalidOperationException(
                "AddAdsApp 没有收到任何应用配置。请检查配置节（默认节名 WechatAds）是否存在且拼写正确，" +
                "或改用 AddAdsApp(Action<AdsAppConfig>) 以代码注册。");
        }

        // SSRF 白名单（进程级全局态）必须在这里登记：广告线有意不走声明式 [Token]，
        // 因而不会触发 AddWechatTokenRecovery，纯广告宿主若不登记会让**每个请求**被组件的
        // 连接期 SSRF 严格模式拦下。经公用层窄入口登记 ⇒ 不违反 AB-G4「产品线不得自行登记白名单」，
        // 且并集数组零改动（ADS-B4 / AB-G9）。
        services.AddWechatApiHosts();

        var switcher = new AdsAppContextSwitcher();
        services.TryAddSingleton<IAdsAppContextSwitcher>(switcher);

        var manager = new AdsAppManager(configs, switcher);
        services.TryAddSingleton<IAdsAppManager>(manager);

        // 时钟缝：默认系统时钟；测试注入假时钟以覆盖「refresh_token 恰好过期」这类不可能用 Delay 测的路径。
        services.TryAddSingleton<IAdsClock, AdsSystemClock>();

        // 授权状态存储：默认进程内实现（单实例部署够用）。多实例必须由宿主预注册分布式实现
        // —— 一次性 refresh_token 在两个实例各刷一次，第二次的旧值必已被作废。
        services.TryAddSingleton<IWechatAdsAuthorizationStore, InMemoryWechatAdsAuthorizationStore>();

        services.TryAddSingleton<IAdsAuthorizationService>(static sp => new AdsAuthorizationService(
            sp.GetRequiredService<IAdsAppManager>(),
            sp.GetRequiredService<IWechatAdsAuthorizationStore>(),
            sp.GetRequiredService<IAdsOAuthHttpClient>(),
            sp.GetRequiredService<IAdsClock>(),
            sp.GetService<ILogger<AdsAuthorizationService>>()));

        // 传输层取令牌端口**必须**指向同一实例：每 AppKey 的单飞闸挂在服务实例字段上，
        // 各自 new 一份会让「取令牌」与「显式刷新」用两把闸，并发刷新照旧打架（守卫 ADS-B3）。
        services.TryAddSingleton<IAdsAccessTokenProvider>(
            static sp => sp.GetRequiredService<IAdsAuthorizationService>());

        // 命名客户端（组件 AddMudHttpClient 负责追踪 Handler 与连接期 SSRF 严格模式）。
        // 重复注册防护：AddMudHttpClient 同名重复调用会追加配置委托 ⇒ Handler 可能被挂两次。
        if (services.All(static d => d.ServiceType != typeof(IAdsHttpClient)))
        {
            // AddHttpMessageHandler<T> 是**从 DI 解析 T**（不是 new），漏注册会在首次请求才抛
            // 「No service for type ... has been registered」。
            services.TryAddSingleton<AdsAuthorizationHandler>();

            // 注册到这里必然有默认应用：空集合已 fail-fast，多应用未标默认则由 AdsAppManager 构造期抛出。
            var defaultApp = manager.GetRequiredApp(manager.DefaultAppKey!);

            services.AddMudHttpClient(
                AdsHttpClientNames.ClientName,
                client =>
                {
                    client.BaseAddress = new Uri(defaultApp.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(defaultApp.TimeoutSeconds);
                })
                .AddHttpMessageHandler<AdsAuthorizationHandler>();

            // OAuth 客户端**不挂**令牌 Handler（官方 token 页：OAuth 相关接口无需 access_token/timestamp/nonce）。
            services.AddMudHttpClient(
                AdsHttpClientNames.OAuthClientName,
                client =>
                {
                    client.BaseAddress = new Uri(defaultApp.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(defaultApp.TimeoutSeconds);
                });
        }

        services.TryAddSingleton<IAdsHttpClient, AdsHttpClient>();
        services.TryAddSingleton<IAdsOAuthHttpClient, AdsOAuthHttpClient>();

#if NET8_0_OR_GREATER
        // AOT 生死线：OAuth 应答类型必须由源生成上下文提供元数据（JIT 下靠反射侥幸可用，
        // Native AOT 下无元数据即失败）。业务域的上下文同批登记（见该类的 remarks）。
        AdsJsonResolverExtensions.ConfigureDataModelsResolver(services);
#endif

        return services;
    }
}
