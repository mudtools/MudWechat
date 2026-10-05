// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调接收服务注册扩展（v1 方案 §5.8）：接收器/分发器/注册表/中间件装配 + 建造者链式注册。
/// </summary>
/// <remarks>
/// 回调仓储（<see cref="IWechatSuiteTicketStore"/> / <see cref="IWechatCorpAuthStore"/>）
/// 复用令牌底座在 <c>AddWechatApp</c> 中注册的实例（TryAdd 语义：宿主可预注册分布式实现）；
/// 未注册授权模块时兜底处理器按「解析失败即跳过 + 告警」降级（R10）。
/// 回调注册不改动主包 DI 顺序（DI 桥接不变量，v1 方案 §1.3）。
/// </remarks>
public static class WechatCallbackServiceCollectionExtensions
{
    /// <summary>
    /// 注册企业微信回调接收（验签 + AES 解密 + 事件分发 + HTTP 中间件依赖）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">回调配置委托（<see cref="WechatCallbackOptions.Apps"/> 多应用凭据等）。</param>
    /// <returns>建造者（链式注册 <see cref="WechatCallbackServiceBuilder.AddHandler{THandler}"/> /
    /// <see cref="WechatCallbackServiceBuilder.AddInterceptor{TInterceptor}"/>）。</returns>
    public static WechatCallbackServiceBuilder AddWechatCallback(
        this IServiceCollection services,
        Action<WechatCallbackOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.Configure(configure);
        return services.AddWechatCallbackCore();
    }

    /// <summary>
    /// 从配置文件注册企业微信回调接收（配置节默认 <c>WechatCallback</c>）。
    /// </summary>
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("反射式配置绑定（ConfigurationBinder.Bind）在裁剪下无法静态分析配置类型成员")]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("反射式配置绑定（ConfigurationBinder.Bind）在 AOT/动态代码生成环境下不可用")]
#endif
    public static WechatCallbackServiceBuilder AddWechatCallback(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "WechatCallback")
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.Configure<WechatCallbackOptions>(options => configuration.GetSection(sectionName).Bind(options));
        return services.AddWechatCallbackCore();
    }

    private static WechatCallbackServiceBuilder AddWechatCallbackCore(this IServiceCollection services)
    {
        // 注册表在组合根期创建为单例实例（急切注册模型，v1.2 §5.6；无 Freeze）。
        var handlerRegistry = new WechatCallbackHandlerRegistry();
        var interceptorRegistry = new WechatCallbackInterceptorRegistry();

        // P0-2：抗重放去重守卫（进程内默认；多实例部署由宿主 TryAdd 前置注册分布式实现）。
        // 注：WechatCallbackMiddleware 为经典约定式中间件（RequestDelegate 经 UseMiddleware 注入），
        // 不进 DI——注册反而会让「可解析性」检查在 RequestDelegate 上失败。
        services.TryAddSingleton<IWechatCallbackReplayGuard, InMemoryWechatCallbackReplayGuard>();
        services.TryAddSingleton<IWechatCallbackReceiver, WechatCallbackReceiver>();
        services.TryAddSingleton<WechatCallbackDispatcher>();
        services.TryAddSingleton<WechatCallbackHandler>();
        services.AddSingleton(handlerRegistry);
        services.AddSingleton(interceptorRegistry);

        // 智能机器人回调面（v1.2）：接收器与分发器<b>无条件注册</b>——
        // ① 中间件为经典约定式（构造注入），若仅由 AddWechatBotCallback 注册则未接线 Bot 的宿主会整体解析失败；
        // ② 两者在无 Bot 条目时完全惰性（接收器只在 JSON 报文到达时被调用；分发器只在命中处理器时工作）；
        // ③ 处理器注册表实例在此创建，AddWechatBotCallback() 复用同一实例（宿主未先调 AddWechatCallback 即 fail-fast）。
        var botHandlerRegistry = new WechatBotHandlerRegistry();
        services.AddSingleton(botHandlerRegistry);
        services.TryAddSingleton<IWechatBotCallbackReceiver, WechatBotCallbackReceiver>();
        services.TryAddSingleton<WechatBotEventDispatcher>();

        // 载荷体系（v2.2）：契约注册表 + 读取器 + 与接收器共享的源缓存。
        // 读取器构造函数为 internal（源缓存是实现细节），故以工厂委托注册。
        var payloadRegistry = new WechatPayloadContractRegistry();
        services.AddSingleton<IWechatPayloadContractRegistry>(payloadRegistry);
        services.AddSingleton(WechatPayloadSourceCache.Shared);
        services.TryAddSingleton<IWechatPayloadReader>(provider =>
            new WechatCallbackPayloadReader(
                provider.GetRequiredService<IWechatPayloadContractRegistry>(),
                WechatPayloadSourceCache.Shared));

        // 官方 41 键契约（组合根期一次性登记；重复登记即 fail-fast）。
        OfficialPayloadContracts.RegisterAll(payloadRegistry);

        // D6/D11：内置授权族兜底处理器默认注册到通配键（全局生效；宿主可用精确键处理器前置接管授权族键）。
        handlerRegistry.Register(WechatCallbackOptions.WildcardAppKey, typeof(WechatCallbackHandler));

        return new WechatCallbackServiceBuilder(services, handlerRegistry, interceptorRegistry, payloadRegistry);
    }
}
