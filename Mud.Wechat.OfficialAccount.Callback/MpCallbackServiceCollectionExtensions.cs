// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 公众号回调注册建造者（链式注册处理器 / 拦截器 / 被动回复处理器）。
/// </summary>
/// <remarks>
/// <b>与 <c>AddMpApp</c> 的形态差异</b>：<c>AddMpApp</c> 为<b>委托式</b> builder（配置各模块 API），
/// 本建造者只做「注册处理器类型」一件事 —— 两者不可互相替代，勿混用。
/// </remarks>
public sealed class MpCallbackServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly MpCallbackHandlerRegistry _handlers;
    private readonly MpCallbackInterceptorRegistry _interceptors;
    private readonly MpCallbackReplyHandlerRegistry _replies;

    internal MpCallbackServiceBuilder(
        IServiceCollection services,
        MpCallbackHandlerRegistry handlers,
        MpCallbackInterceptorRegistry interceptors,
        MpCallbackReplyHandlerRegistry replies)
    {
        _services = services;
        _handlers = handlers;
        _interceptors = interceptors;
        _replies = replies;
    }

    /// <summary>注册事件处理器（<paramref name="appKey"/> 传通配键 = 对所有应用生效）。</summary>
    /// <typeparam name="THandler">处理器类型（须实现 <see cref="IMpCallbackEventHandler"/>）。</typeparam>
    /// <param name="appKey">应用键；默认通配键（全局）。</param>
    /// <returns>建造者（链式）。</returns>
#if NET6_0_OR_GREATER
    public MpCallbackServiceBuilder AddHandler<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
        string appKey = MpCallbackOptions.WildcardAppKey)
#else
    public MpCallbackServiceBuilder AddHandler<THandler>(string appKey = MpCallbackOptions.WildcardAppKey)
#endif
        where THandler : class, IMpCallbackEventHandler
    {
        _handlers.Register(appKey, typeof(THandler));
        _services.TryAddTransient<THandler>();
        return this;
    }

    /// <summary>注册事件拦截器（<paramref name="appKey"/> 传通配键 = 对所有应用生效）。</summary>
    /// <typeparam name="TInterceptor">拦截器类型（须实现 <see cref="IMpCallbackEventInterceptor"/>）。</typeparam>
    /// <param name="appKey">应用键；默认通配键（全局）。</param>
    /// <returns>建造者（链式）。</returns>
#if NET6_0_OR_GREATER
    public MpCallbackServiceBuilder AddInterceptor<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TInterceptor>(
        string appKey = MpCallbackOptions.WildcardAppKey)
#else
    public MpCallbackServiceBuilder AddInterceptor<TInterceptor>(string appKey = MpCallbackOptions.WildcardAppKey)
#endif
        where TInterceptor : class, IMpCallbackEventInterceptor
    {
        _interceptors.Register(appKey, typeof(TInterceptor));
        _services.TryAddTransient<TInterceptor>();
        return this;
    }

    /// <summary>注册被动回复处理器（返回 <c>null</c> = 无回复，SDK 回明文 <c>success</c>）。</summary>
    /// <typeparam name="THandler">回复处理器类型（须实现 <see cref="IMpCallbackReplyHandler"/>）。</typeparam>
    /// <param name="appKey">应用键；默认通配键（全局）。</param>
    /// <returns>建造者（链式）。</returns>
#if NET6_0_OR_GREATER
    public MpCallbackServiceBuilder AddReplyHandler<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] THandler>(
        string appKey = MpCallbackOptions.WildcardAppKey)
#else
    public MpCallbackServiceBuilder AddReplyHandler<THandler>(string appKey = MpCallbackOptions.WildcardAppKey)
#endif
        where THandler : class, IMpCallbackReplyHandler
    {
        _replies.Register(appKey, typeof(THandler));
        _services.TryAddTransient<THandler>();
        return this;
    }
}

/// <summary>
/// 公众号回调 DI 注册入口。
/// </summary>
public static class MpCallbackServiceCollectionExtensions
{
    /// <summary>
    /// 注册公众号回调服务（配置绑定版；惰性 <c>Configure(o =&gt; section.Bind(o))</c>，支持请求期热更）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置根。</param>
    /// <param name="sectionName">配置节名（默认 <see cref="MpCallbackOptions.SectionName"/>）。</param>
    /// <returns>回调注册建造者（链式注册处理器）。</returns>
    public static MpCallbackServiceBuilder AddMpCallback(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = MpCallbackOptions.SectionName)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var section = configuration.GetSection(sectionName);
        return AddMpCallbackCore(services, options => section.Bind(options));
    }

    /// <summary>注册公众号回调服务（代码配置版）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>回调注册建造者（链式注册处理器）。</returns>
    public static MpCallbackServiceBuilder AddMpCallback(
        this IServiceCollection services,
        Action<MpCallbackOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        return AddMpCallbackCore(services, configure);
    }

    /// <summary>
    /// 回调服务注册核心：配置面校验 + 注册表 + 抗重放守卫 + 接收器 / 读取器 / 分发器 + 契约登记。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置委托。</param>
    /// <returns>回调注册建造者。</returns>
    internal static MpCallbackServiceBuilder AddMpCallbackCore(
        IServiceCollection services, Action<MpCallbackOptions> configure)
    {
        // 配置绑定走源生成器（AOT 净零）；校验在首次解析时 fail-fast。
        services.AddOptions<MpCallbackOptions>()
            .Configure(configure)
            .Validate(options =>
            {
                options.Validate();
                return true;
            }, "公众号回调配置校验失败（详见 MpCallbackOptions.Validate）。");

        var handlers = new MpCallbackHandlerRegistry();
        var interceptors = new MpCallbackInterceptorRegistry();
        var replies = new MpCallbackReplyHandlerRegistry();

        services.AddSingleton(handlers);
        services.AddSingleton(interceptors);
        services.AddSingleton(replies);

        // 契约登记（先生成产物 → 再登记）：`MpPayloadContracts.RegisterAll` 方法体由中立生成器发射。
        services.AddSingleton<IMpPayloadContractRegistry>(_ =>
        {
            var registry = new MpPayloadContractRegistry();
            MpPayloadContracts.RegisterAll(registry);
            return registry;
        });

        // 抗重放：默认进程内实现；多实例部署由宿主以 TryAdd 前置注册分布式实现（Redis）。
        services.TryAddSingleton<IWechatCallbackReplayGuard, InMemoryWechatCallbackReplayGuard>();

        services.AddSingleton<IMpCallbackReceiver>(sp => new MpCallbackReceiver(
            sp.GetRequiredService<IOptionsMonitor<MpCallbackOptions>>(),
            sp.GetRequiredService<IWechatCallbackReplayGuard>(),
            null,
            sp.GetService<ILogger<MpCallbackReceiver>>()));

        services.AddSingleton<IMpPayloadReader>(sp =>
            new MpCallbackPayloadReader(sp.GetRequiredService<IMpPayloadContractRegistry>()));

        services.AddSingleton<MpCallbackDispatcher>();

        return new MpCallbackServiceBuilder(services, handlers, interceptors, replies);
    }
}
