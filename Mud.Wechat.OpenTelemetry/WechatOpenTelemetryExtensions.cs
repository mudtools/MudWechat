// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenTelemetry;

/// <summary>
/// Mud.Wechat OpenTelemetry 适配包的 DI 扩展方法。
/// </summary>
/// <remarks>
/// <para>
/// 通过 <see cref="AddWechatOpenTelemetry(IServiceCollection, Action{WechatOpenTelemetryOptions}?)"/> 一键启用
/// 微信 SDK 的分布式追踪与指标采集。
/// </para>
/// <para>
/// 自动注册以下 ActivitySource 和 Meter：
/// </para>
/// <list type="bullet">
/// <item><c>Mud.Wechat</c> — 微信全产品线追踪源（回调接收、事件分发、令牌、授权）</item>
/// <item><c>Mud.Wechat.*</c> — 产品线各自 Meter（通配注册，覆盖现有与未来全部产品线）</item>
/// <item><c>Mud.HttpUtils.HttpClient</c> — HTTP 出站请求追踪、Token 刷新、重试、熔断器指标（可选，默认开启）</item>
/// </list>
/// <para>
/// 默认导出至本地 OTLP gRPC 端点（<c>http://localhost:4317</c>），通过 <see cref="WechatOpenTelemetryOptions.OtlpEndpoint"/> 自定义。
/// </para>
/// </remarks>
public static class WechatOpenTelemetryExtensions
{
    /// <summary>
    /// 一键开启 Mud.Wechat 的 OpenTelemetry 追踪与指标采集。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">可选的配置委托。</param>
    /// <returns>返回 <see cref="OpenTelemetryBuilder"/>，便于调用方继续追加配置。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 null。</exception>
    /// <example>
    /// <code>
    /// builder.Services.AddWechatOpenTelemetry(options =&gt;
    /// {
    ///     options.OtlpEndpoint = new Uri("http://otel-collector:4317");
    ///     options.ServiceName = "my-wechat-app";
    ///     options.SamplingRatio = 0.1;
    /// });
    /// </code>
    /// </example>
    public static OpenTelemetryBuilder AddWechatOpenTelemetry(
        this IServiceCollection services,
        Action<WechatOpenTelemetryOptions>? configure = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        var options = new WechatOpenTelemetryOptions();
        configure?.Invoke(options);

        return AddWechatOpenTelemetryCore(services, options);
    }

    /// <summary>
    /// 一键开启 Mud.Wechat 的 OpenTelemetry 追踪与指标采集，从 <see cref="IConfiguration"/> 绑定选项。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置实例，用于绑定 <see cref="WechatOpenTelemetryOptions"/>。</param>
    /// <param name="sectionPath">配置节点路径，默认 <c>"WechatOpenTelemetry"</c>。</param>
    /// <param name="configure">可选的附加配置委托，在配置绑定之后执行，可覆盖绑定值。</param>
    /// <returns>返回 <see cref="OpenTelemetryBuilder"/>，便于调用方继续追加配置。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 或 <paramref name="configuration"/> 为 null。</exception>
    /// <remarks>
    /// 配置绑定走 <c>Configure&lt;T&gt;(o =&gt; section.Bind(o))</c> 源生成路径（AGENTS.md 红线），
    /// 不使用反射式 <c>GetSection(path).Bind(options)</c> 重载，满足 AOT <c>IL2026</c>/<c>IL3050</c> 净零。
    /// </remarks>
    /// <example>
    /// appsettings.json：
    /// <code>
    /// {
    ///   "WechatOpenTelemetry": {
    ///     "ServiceName": "my-wechat-app",
    ///     "SamplingRatio": 0.1,
    ///     "OtlpEndpoint": "http://otel-collector:4317"
    ///   }
    /// }
    /// </code>
    /// 代码：
    /// <code>
    /// builder.Services.AddWechatOpenTelemetry(builder.Configuration);
    /// </code>
    /// </example>
    public static OpenTelemetryBuilder AddWechatOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionPath = "WechatOpenTelemetry",
        Action<WechatOpenTelemetryOptions>? configure = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        var options = new WechatOpenTelemetryOptions();
        configuration.GetSection(sectionPath).Bind(options);
        configure?.Invoke(options);

        return AddWechatOpenTelemetryCore(services, options);
    }

    private static OpenTelemetryBuilder AddWechatOpenTelemetryCore(
        IServiceCollection services,
        WechatOpenTelemetryOptions options)
    {
        // 校验 SamplingRatio 范围（即时反馈）
        if (options.SamplingRatio < 0 || options.SamplingRatio > 1)
            throw new ArgumentOutOfRangeException(nameof(options.SamplingRatio),
                $"SamplingRatio 必须在 0.0~1.0 范围内，当前值为 {options.SamplingRatio}。");

        // netstandard2.0 不支持 AspNetCore Instrumentation（无 FrameworkReference）
#if NETSTANDARD2_0
        options.EnableAspNetCoreInstrumentation = false;
#endif

        // 注册 IValidateOptions 以支持 IOptions<> / ValidateOnStart 集成
        services.AddSingleton<IValidateOptions<WechatOpenTelemetryOptions>, WechatOpenTelemetryOptions>();

        // 将预构建的 options 注册为 IOptions<>，使 DI 容器中的消费者可以获取到一致的配置
        services.AddSingleton<IOptions<WechatOpenTelemetryOptions>>(new OptionsWrapper<WechatOpenTelemetryOptions>(options));

        // 配置 Resource：service.name / service.version / deployment.environment
        var builder = services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(serviceName: options.ServiceName, serviceVersion: options.ServiceVersion)
                .AddAttributes(new[]
                {
                    new KeyValuePair<string, object>("deployment.environment", options.DeploymentEnvironment)
                }));

        if (options.EnableTracing)
        {
            builder.WithTracing(tp =>
            {
                // 采样器：ParentBased + TraceIdRatioBased
                tp.SetSampler(new ParentBasedSampler(
                    new TraceIdRatioBasedSampler(options.SamplingRatio)));

                // Wechat ActivitySource（全产品线单根源）
                tp.AddSource(WechatActivitySource.Name);

                // Mud.HttpUtils ActivitySource（HTTP 出站请求 + Token 恢复）
                if (options.IncludeMudHttpUtils)
                    tp.AddSource(MudHttpActivitySource.Name);

                if (options.EnableHttpClientInstrumentation)
                    tp.AddHttpClientInstrumentation();

                if (options.EnableAspNetCoreInstrumentation)
                    tp.AddAspNetCoreInstrumentation();

                if (options.OtlpEndpoint != null)
                    tp.AddOtlpExporter(o => o.Endpoint = options.OtlpEndpoint);

                options.ConfigureTracing?.Invoke(tp);
            });
        }

        if (options.EnableMetrics)
        {
            builder.WithMetrics(mp =>
            {
                // Wechat 产品线 Meter（通配注册，覆盖现有与未来全部产品线）
                mp.AddMeter("Mud.Wechat*");

                // Mud.HttpUtils Meter（HTTP 请求、Token 刷新、重试、熔断器、下载）
                if (options.IncludeMudHttpUtils)
                    mp.AddMeter(MudHttpMeter.MeterName);

                if (options.EnableHttpClientInstrumentation)
                    mp.AddHttpClientInstrumentation();

                if (options.OtlpEndpoint != null)
                    mp.AddOtlpExporter(o => o.Endpoint = options.OtlpEndpoint);

                options.ConfigureMetrics?.Invoke(mp);
            });
        }

        if (options.EnableLogging)
        {
            builder.WithLogging(lp =>
            {
                if (options.OtlpEndpoint != null)
                    lp.AddOtlpExporter(o => o.Endpoint = options.OtlpEndpoint);

                options.ConfigureLogging?.Invoke(lp);
            });
        }

        return builder;
    }
}
