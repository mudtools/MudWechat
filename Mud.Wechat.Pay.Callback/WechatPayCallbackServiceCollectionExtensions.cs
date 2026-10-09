// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 支付通知处理器注册建造者（链式注册；<b>只做一件事</b>：登记处理器类型）。
/// </summary>
/// <remarks>
/// 与 <c>AddPayApp</c>（凭据底座）/ <c>AddWechatPayApi</c>（业务接口）的建造者形态一致 —— 三者不可互相替代。
/// </remarks>
public sealed class WechatPayCallbackServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly WechatPayCallbackHandlerRegistry _handlers;

    internal WechatPayCallbackServiceBuilder(
        IServiceCollection services, WechatPayCallbackHandlerRegistry handlers)
    {
        _services = services;
        _handlers = handlers;
    }

    /// <summary>
    /// 注册通知处理器。
    /// </summary>
    /// <typeparam name="THandler">处理器类型（须实现 <see cref="IWechatPayNotificationHandler"/>）。</typeparam>
    /// <param name="eventType">
    /// 限定的事件类型（如 <c>TRANSACTION.SUCCESS</c>）；默认通配 <c>"*"</c> = 对所有事件生效。
    /// </param>
    /// <returns>建造者（链式）。</returns>
    /// <remarks>处理器以 <c>Transient</c> 注册；每条通知在独立 DI 作用域内解析（见分发器 remarks）。</remarks>
#if NET6_0_OR_GREATER
    public WechatPayCallbackServiceBuilder AddHandler<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
        string eventType = WechatPayCallbackHandlerRegistry.WildcardEventType)
#else
    public WechatPayCallbackServiceBuilder AddHandler<THandler>(
        string eventType = WechatPayCallbackHandlerRegistry.WildcardEventType)
#endif
        where THandler : class, IWechatPayNotificationHandler
    {
        _handlers.Register(eventType, typeof(THandler));
        _services.TryAddTransient<THandler>();
        return this;
    }
}

/// <summary>
/// 支付回调 DI 注册入口（<c>AddWechatPayCallback</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>前置依赖</b>：必须先调用 <c>AddPayApp</c>（提供 <c>IWechatPayMerchantManager</c> 与
/// <c>IWechatPaySignatureProviderFactory</c>）。漏装会在<b>注册期</b> fail-fast 并点名修复方式，
/// 而不是等到首条通知到达才报 DI 缺失。
/// </para>
/// <para>
/// <b>与业务接口包的关系</b>：只装回调的宿主<b>无需</b> <c>AddWechatPayApi</c>（不必拉起业务客户端），
/// 与「<c>AddPayApp</c> 单独可调」的分工一致。
/// </para>
/// </remarks>
public static class WechatPayCallbackServiceCollectionExtensions
{
    /// <summary>注册支付回调服务（配置绑定版；惰性 <c>Configure(o =&gt; section.Bind(o))</c>，支持请求期热更）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置根。</param>
    /// <param name="sectionName">配置节名（默认 <see cref="WechatPayCallbackOptions.SectionName"/>）。</param>
    /// <returns>回调注册建造者（链式注册处理器）。</returns>
    public static WechatPayCallbackServiceBuilder AddWechatPayCallback(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = WechatPayCallbackOptions.SectionName)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var section = configuration.GetSection(sectionName);
        return AddWechatPayCallbackCore(services, options => section.Bind(options));
    }

    /// <summary>注册支付回调服务（代码配置版）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>回调注册建造者（链式注册处理器）。</returns>
    public static WechatPayCallbackServiceBuilder AddWechatPayCallback(
        this IServiceCollection services,
        Action<WechatPayCallbackOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        return AddWechatPayCallbackCore(services, configure);
    }

    /// <summary>回调注册核心：前置校验 + 配置面校验 + 注册表 + 抗重放守卫 + 接收器 / 分发器。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>回调注册建造者。</returns>
    internal static WechatPayCallbackServiceBuilder AddWechatPayCallbackCore(
        IServiceCollection services, Action<WechatPayCallbackOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // 前置依赖 fail-fast：缺凭据底座时回调既无法验签也无法解密，早失败远优于运行期 500。
        if (services.All(static d => d.ServiceType != typeof(IWechatPayMerchantManager)))
        {
            throw new InvalidOperationException(
                "未注册 IWechatPayMerchantManager。请先调用 AddPayApp 注册微信支付商户配置，再调用 AddWechatPayCallback。" +
                "示例：services.AddPayApp(configuration, \"WechatPayMerchants\")" +
                ".AddWechatPayCallback(configuration);");
        }

        // 配置绑定走源生成器（AOT 净零）；校验在首次解析时 fail-fast。
        services.AddOptions<WechatPayCallbackOptions>()
            .Configure(configure)
            .Validate(
                static options =>
                {
                    options.Validate();
                    return true;
                },
                "微信支付回调配置校验失败（详见 WechatPayCallbackOptions.Validate）。");

        var handlers = new WechatPayCallbackHandlerRegistry();
        services.AddSingleton(handlers);

        // 抗重放：默认进程内实现；多实例部署由宿主以 TryAdd 前置注册分布式实现（Mud.Wechat.Redis）。
        services.TryAddSingleton<IWechatCallbackReplayGuard, InMemoryWechatCallbackReplayGuard>();

        services.AddSingleton(sp => new WechatPayCallbackReceiver(
            sp.GetRequiredService<IWechatPayMerchantManager>(),
            sp.GetRequiredService<IWechatPaySignatureProviderFactory>(),
            sp.GetRequiredService<IWechatPayMerchantCredentialProvider>(),
            sp.GetRequiredService<IWechatCallbackReplayGuard>(),
            sp.GetRequiredService<IOptionsMonitor<WechatPayCallbackOptions>>(),
            sp.GetService<ILogger<WechatPayCallbackReceiver>>()));

        services.AddSingleton(sp => new WechatPayCallbackDispatcher(
            sp.GetRequiredService<WechatPayCallbackHandlerRegistry>(),
            sp,
            sp.GetRequiredService<IOptionsMonitor<WechatPayCallbackOptions>>(),
            sp.GetService<ILogger<WechatPayCallbackDispatcher>>()));

        return new WechatPayCallbackServiceBuilder(services, handlers);
    }
}
