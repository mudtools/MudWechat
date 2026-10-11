// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Channels.Abstractions.Callback;
using Mud.Wechat.Channels.DataModels.Callback;

namespace Mud.Wechat.Channels.Callback;

/// <summary>
/// 微信小店回调注册扩展（组合根期急切登记，无 Freeze；对齐企微 <c>AddWechatCallback</c> / 公众号 <c>AddMpCallback</c> 形态）。
/// </summary>
/// <remarks>
/// <para>
/// 典型用法：
/// <code>
/// builder.Services.AddWechatChannelsCallback(builder.Configuration)
///        .AddHandler&lt;OrderPayHandler&gt;("shop-main");
/// app.UseWechatChannelsWebhook();
/// </code>
/// </para>
/// <para>
/// 载荷契约登记在组合根期一次性完成（<c>ChannelsPayloadContracts.RegisterAll</c>，生成器发射方法体）；
/// 路由前缀与凭据经 <c>IOptionsMonitor</c> 请求期热更。
/// </para>
/// </remarks>
public static class WechatChannelsCallbackServiceCollectionExtensions
{
    /// <summary>注册小店回调接收设施（配置委托版）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">回调配置委托（节名 <see cref="ChannelsCallbackOptions.SectionName"/>）。</param>
    /// <returns>回调注册建造者（链式注册处理器 / 拦截器）。</returns>
    public static WechatChannelsCallbackServiceBuilder AddWechatChannelsCallback(
        this IServiceCollection services, Action<ChannelsCallbackOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }

        return AddWechatChannelsCallbackCore(services);
    }

    /// <summary>注册小店回调接收设施（IConfiguration 版，惰性绑定节 <see cref="ChannelsCallbackOptions.SectionName"/>）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <returns>回调注册建造者。</returns>
    public static WechatChannelsCallbackServiceBuilder AddWechatChannelsCallback(
        this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        // 惰性 Configure（Bind 动作形态，源生成配置绑定器拦截；不得改回注册期急切绑定 ——
        // IOptionsMonitor 支持路由前缀与凭据请求期热更）。
        var section = configuration.GetSection(ChannelsCallbackOptions.SectionName);
        services.Configure<ChannelsCallbackOptions>(options => section.Bind(options));

        return AddWechatChannelsCallbackCore(services);
    }

    private static WechatChannelsCallbackServiceBuilder AddWechatChannelsCallbackCore(IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (services.Any(static d => d.ServiceType == typeof(WechatChannelsCallbackDispatcher)))
        {
            throw new InvalidOperationException(
                "AddWechatChannelsCallback 已被调用过（重复注册会让处理器登记面翻倍）。");
        }

        // 载荷契约登记（组合根期一次性；生成器发射 RegisterAll 方法体，手写登记表是禁止形态）。
        var contracts = new ChannelsPayloadContractRegistry();
        ChannelsPayloadContracts.RegisterAll(contracts);
        services.AddSingleton<IChannelsPayloadContractRegistry>(contracts);

        // 注册表实例急切创建并以实例登记（建造者组合根期直接写入，容器内同实例共享）。
        var handlers = new ChannelsCallbackHandlerRegistry();
        var interceptors = new ChannelsCallbackInterceptorRegistry();
        var replies = new ChannelsCallbackReplyRegistry();

        services.TryAddSingleton<IWechatCallbackReplayGuard, InMemoryWechatCallbackReplayGuard>();
        services.AddSingleton(handlers);
        services.AddSingleton(interceptors);
        services.AddSingleton(replies);
        services.AddSingleton<WechatChannelsCallbackReceiver>();
        services.AddSingleton<IWechatChannelsCallbackReceiver>(static sp => sp.GetRequiredService<WechatChannelsCallbackReceiver>());
        services.AddSingleton<WechatChannelsCallbackDispatcher>();

        return new WechatChannelsCallbackServiceBuilder(services, handlers, interceptors, replies);
    }
}

/// <summary>
/// 小店回调注册建造者：链式登记处理器 / 拦截器（键 = 应用键或通配键）。
/// </summary>
public sealed class WechatChannelsCallbackServiceBuilder
{
    private readonly IServiceCollection _services;
    private readonly ChannelsCallbackHandlerRegistry _handlers;
    private readonly ChannelsCallbackInterceptorRegistry _interceptors;
    private readonly ChannelsCallbackReplyRegistry _replies;

    /// <summary>创建建造者。</summary>
    public WechatChannelsCallbackServiceBuilder(
        IServiceCollection services,
        ChannelsCallbackHandlerRegistry handlers,
        ChannelsCallbackInterceptorRegistry interceptors,
        ChannelsCallbackReplyRegistry replies)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));
        _replies = replies ?? throw new ArgumentNullException(nameof(replies));
    }

    /// <summary>登记事件处理器（信封级或载荷级；类型由容器按 scope 解析）。</summary>
    /// <typeparam name="THandler">处理器类型。</typeparam>
    /// <param name="appKey">应用键（精确匹配；传通配键注册全局兜底桶）。</param>
    /// <returns>建造者（链式）。</returns>
    public WechatChannelsCallbackServiceBuilder AddHandler<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        THandler>(string appKey)
        where THandler : class, IChannelsCallbackEventHandler
    {
        _services.AddScoped<THandler>();
        _handlers.Register(appKey, typeof(THandler));
        return this;
    }

    /// <summary>
    /// 登记结构化应答处理器（仅官方要求同步返回业务数据的事件需要——会员三事件等；其余事件回明文 success）。
    /// </summary>
    /// <typeparam name="THandler">应答处理器类型。</typeparam>
    /// <param name="appKey">应用键（精确匹配；传通配键注册全局兜底桶）。</param>
    /// <returns>建造者（链式）。</returns>
    public WechatChannelsCallbackServiceBuilder AddReplyHandler<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        THandler>(string appKey)
        where THandler : class, IChannelsCallbackReplyHandler
    {
        _services.AddScoped<THandler>();
        _replies.Register(appKey, typeof(THandler));
        return this;
    }

    /// <summary>登记事件拦截器。</summary>
    /// <typeparam name="TInterceptor">拦截器类型。</typeparam>
    /// <param name="appKey">应用键（精确匹配；传通配键注册全局桶）。</param>
    /// <returns>建造者（链式）。</returns>
    public WechatChannelsCallbackServiceBuilder AddInterceptor<
#if NET6_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
        TInterceptor>(string appKey)
        where TInterceptor : class, IChannelsCallbackEventInterceptor
    {
        _services.AddScoped<TInterceptor>();
        _interceptors.Register(appKey, typeof(TInterceptor));
        return this;
    }
}
