// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Mud.Wechat.Redis.HealthChecks;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Authentication;
using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;
using StackExchange.Redis;

#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace Mud.Wechat.Redis.Extensions;

/// <summary>
/// Redis 分布式存储模块的 DI 注册入口（单入口 + 顺序守卫，RD8）。
/// </summary>
/// <remarks>
/// <para>
/// <b>调用顺序 = 契约</b>：必须<b>先</b> <c>AddWechatRedis</c>、<b>后</b>
/// <c>AddWechatApp</c>（或主包 <c>AddWechatWorkServices</c>）与 <c>AddWechatCallback</c>——
/// 四个存储端口的默认实现均为 <c>TryAddSingleton</c> 注册，颠倒顺序时 Redis 实现会被静默跳过
/// （永不生效且无任何错误）。本扩展在注册期检测颠倒顺序并 fail-fast。
/// </para>
/// <para>
/// 四端口注册形态（R-3）：端口接口 <c>TryAddSingleton</c>（宿主预注册的自定义实现按契约胜出）+
/// Redis 具体类型 <c>AddSingleton</c> 恒注册（可自省、可被直接解析）。
/// </para>
/// <para>
/// 重复调用幂等：以 <c>IConnectionMultiplexer</c> 是否已注册为判据跳过连接基座
/// （对齐飞书 T-M3-2）。
/// </para>
/// </remarks>
public static class WechatRedisServiceCollectionExtensions
{
    /// <summary>
    /// 注册 Redis 分布式存储（配置文件形态）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="sectionName">配置节名称，默认 <see cref="WechatRedisOptions.SectionName"/>。</param>
    /// <param name="registerHealthCheck">是否向宿主健康检查管线注册 Redis 健康检查
    /// （false 时仅注册 <see cref="RedisHealthCheck"/> 类型，由宿主自行 <c>AddCheck</c>）。</param>
    /// <returns>服务集合。</returns>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("反射式配置绑定（ConfigurationBinder.Bind）在裁剪下无法静态分析配置类型成员")]
    [RequiresDynamicCode("反射式配置绑定（ConfigurationBinder.Bind）在 AOT/动态代码生成环境下不可用")]
#endif
    public static IServiceCollection AddWechatRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = WechatRedisOptions.SectionName,
        bool registerHealthCheck = true)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        EnsureWechatAppNotRegistered(services, nameof(AddWechatRedis));

        var section = configuration.GetSection(sectionName);
        services.Configure<WechatRedisOptions>(options => section.Bind(options));
        return services.AddWechatRedisCore(registerHealthCheck);
    }

    /// <summary>
    /// 注册 Redis 分布式存储（代码配置形态）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configureOptions">配置委托。</param>
    /// <param name="registerHealthCheck">是否向宿主健康检查管线注册 Redis 健康检查。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddWechatRedis(
        this IServiceCollection services,
        Action<WechatRedisOptions> configureOptions,
        bool registerHealthCheck = true)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

        EnsureWechatAppNotRegistered(services, nameof(AddWechatRedis));

        services.Configure(configureOptions);
        return services.AddWechatRedisCore(registerHealthCheck);
    }

    /// <summary>
    /// 核心注册：IValidateOptions + ValidateOnStart(net6+) → IConnectionMultiplexer（幂等）→ 健康检查 →
    /// 预热 HostedService → 配置实例 → 四端口（具体类型恒注册 + 接口 TryAdd）。
    /// </summary>
    private static IServiceCollection AddWechatRedisCore(this IServiceCollection services, bool registerHealthCheck)
    {
        services.TryAddSingleton<IValidateOptions<WechatRedisOptions>, WechatRedisOptionsValidator>();
#if NET6_0_OR_GREATER
        services.AddOptions<WechatRedisOptions>().ValidateOnStart();
#endif

        if (!services.Any(s => s.ServiceType == typeof(IConnectionMultiplexer)))
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<WechatRedisOptions>>().Value;
                var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("Mud.Wechat.Redis.Connection");
                try
                {
                    var config = RedisConnectionFactory.Build(options);
                    var multiplexer = ConnectionMultiplexer.Connect(config);
                    multiplexer.ConnectionFailed += (_, e) =>
                        logger?.LogWarning(e.Exception, "Redis 连接失败（端点：{EndPoint}），等待自动重连。", e.EndPoint);
                    multiplexer.ConnectionRestored += (_, e) =>
                        logger?.LogInformation("Redis 连接已恢复（端点：{EndPoint}）。", e.EndPoint);
                    return multiplexer;
                }
                catch (Exception ex)
                {
                    // 消息只携带脱敏后的配置描述（Password 已掩码），绝不携带连接串明文。
                    throw new WechatRedisException(
                        WechatRedisFailureKind.Connection,
                        $"Redis 连接初始化失败：{ex.Message}（配置：{options}）",
                        ex);
                }
            });

            services.AddSingleton<RedisHealthCheck>();
            if (registerHealthCheck)
            {
                services.AddHealthChecks()
                    .AddCheck<RedisHealthCheck>("wechat-redis", tags: new[] { "redis", "wechat" });
            }

            // 全 TFM 注册（R-2）：Hosting.Abstractions 经 Abstractions 包全 TFM 引用；
            // 无通用 Host 的 ns2.0 宿主不会启动 HostedService，无害。
            services.AddHostedService(sp => new RedisConnectionWarmupService(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                sp.GetService<ILogger<RedisConnectionWarmupService>>(),
                sp.GetRequiredService<IOptions<WechatRedisOptions>>().Value.Connection.AbortOnConnectFail));
        }

        // 配置实例（首次解析即触发 IValidateOptions 校验；net6+ 另有 ValidateOnStart 启动期闸）。
        services.TryAddSingleton<WechatRedisOptions>(sp => sp.GetRequiredService<IOptions<WechatRedisOptions>>().Value);

        // 四端口（R-3）：具体类型恒注册 + 接口 TryAdd（宿主预注册的自定义实现按契约胜出）。
        services.AddSingleton<RedisWechatTokenStore>();
        services.TryAddSingleton<IWechatTokenStore>(sp => sp.GetRequiredService<RedisWechatTokenStore>());
        services.AddSingleton<RedisWechatCorpAuthStore>();
        services.TryAddSingleton<IWechatCorpAuthStore>(sp => sp.GetRequiredService<RedisWechatCorpAuthStore>());
        services.AddSingleton<RedisWechatSuiteTicketStore>();
        services.TryAddSingleton<IWechatSuiteTicketStore>(sp => sp.GetRequiredService<RedisWechatSuiteTicketStore>());
        services.AddSingleton<RedisWechatCallbackReplayGuard>();
        services.TryAddSingleton<IWechatCallbackReplayGuard>(sp => sp.GetRequiredService<RedisWechatCallbackReplayGuard>());

        return services;
    }

    /// <summary>
    /// 顺序守卫（fail-fast）：检测「先 AddWechatApp / AddWechatWorkServices / AddWechatCallback
    /// 后 AddWechatRedis」的颠倒顺序——TryAddSingleton 语义下 Redis 实现会被既有默认实现静默跳过。
    /// </summary>
    private static void EnsureWechatAppNotRegistered(IServiceCollection services, string callerName)
    {
        if (services.Any(s => s.ServiceType == typeof(IWechatAppManager)))
        {
            throw new InvalidOperationException(
                $"{callerName} 必须在 AddWechatApp（或 AddWechatWorkServices）之前调用。"
                + "四个存储端口的默认实现为 TryAddSingleton 注册，颠倒顺序时 Redis 实现会被静默跳过（永不生效且无任何错误）。"
                + "正确顺序：services.AddWechatRedis(...); services.AddWechatApp(...); services.AddWechatCallback(...);");
        }

        if (HasInMemoryReplayGuardRegistration(services))
        {
            throw new InvalidOperationException(
                $"{callerName} 必须在 AddWechatCallback 之前调用（检测到进程内重放守卫已注册）。"
                + "四个存储端口的默认实现为 TryAddSingleton 注册，颠倒顺序时 Redis 实现会被静默跳过（永不生效且无任何错误）。"
                + "正确顺序：services.AddWechatRedis(...); services.AddWechatApp(...); services.AddWechatCallback(...);");
        }
    }

    /// <summary>
    /// InMemory 重放守卫实现的全名（R-1：Redis 包不引用 Callback 程序集，经全名字符串探测
    /// 「AddWechatCallback 已运行」；全名漂移由 T-R8 契约守卫 RD-G5 以反射锁定——测试工程可引用 Callback）。
    /// </summary>
    internal const string InMemoryReplayGuardTypeName = "Mud.Wechat.Work.Callback.InMemoryWechatCallbackReplayGuard";

    /// <summary>探测「AddWechatCallbackCore 的 TryAddSingleton 已注册进程内重放守卫」。</summary>
    /// <remarks>
    /// 只认 ImplementationType 为 InMemory 实现的描述符——宿主预注册自定义实例/工厂（TryAdd 契约内
    /// 的合法前置覆盖）不视为回调已注册，不拦截。
    /// </remarks>
    private static bool HasInMemoryReplayGuardRegistration(IServiceCollection services)
        => services.Any(d => d.ServiceType == typeof(IWechatCallbackReplayGuard)
                             && d.ImplementationType?.FullName == InMemoryReplayGuardTypeName);
}
