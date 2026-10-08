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
/// 装配本身委托给上游共享内核
/// <see cref="MudObservabilityBootstrap.AddMudObservability(IServiceCollection, MudObservabilityContribution, MudObservabilityOptions)"/>，
/// 本包只负责「贡献描述 + 选项映射 + 注册期校验」：自动注册以下 ActivitySource 和 Meter：
/// </para>
/// <list type="bullet">
/// <item><c>Mud.Wechat</c> — 微信全产品线追踪源（回调接收、事件分发、令牌、授权）</item>
/// <item><c>Mud.Wechat*</c> — 产品线各自 Meter（通配注册，覆盖现有与未来全部产品线）</item>
/// <item><c>Mud.HttpUtils.HttpClient</c> — HTTP 出站请求追踪、Token 刷新、重试、熔断器指标（可选，由
/// <see cref="WechatOpenTelemetryOptions.IncludeMudHttpUtils"/> 控制，默认开启；由内核去重注册）</item>
/// </list>
/// <para>
/// 默认导出至本地 OTLP gRPC 端点（<c>http://localhost:4317</c>），通过 <see cref="WechatOpenTelemetryOptions.OtlpEndpoint"/> 自定义。
/// </para>
/// <para>
/// <b>单一入口禁令</b>：本方法内部经由共享内核装配，内核按产品名拒绝「不同产品重复注册」。宿主
/// <b>不得</b>同时调用 <c>AddMudHttpOpenTelemetry()</c>（会让两条完整管道叠加：Resource
/// <c>service.name</c> 被覆盖、同一 Span 被两个 OTLP 导出器重复上报）；仅使用微信 SDK 的宿主
/// 调用本方法即可，Mud.HttpUtils 的源与指标已由 <see cref="WechatOpenTelemetryOptions.IncludeMudHttpUtils"/> 覆盖。
/// </para>
/// </remarks>
public static class WechatOpenTelemetryExtensions
{
    /// <summary>贡献描述的默认 ServiceName（与 <see cref="WechatOpenTelemetryOptions.ServiceName"/> 默认值一致）。</summary>
    private const string DefaultServiceName = "Mud.Wechat.Application";

    /// <summary>产品线 Meter 通配模式：覆盖 <c>Mud.Wechat.Work</c> / <c>Mud.Wechat.OfficialAccount</c> 等全部产品线。</summary>
    private const string WechatMeterWildcard = "Mud.Wechat*";

    /// <summary>
    /// 一键开启 Mud.Wechat 的 OpenTelemetry 追踪与指标采集。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">可选的配置委托。</param>
    /// <returns>返回 <see cref="OpenTelemetryBuilder"/>，便于调用方继续追加配置。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 null。</exception>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="WechatOpenTelemetryOptions.SamplingRatio"/> 越界。</exception>
    /// <exception cref="OptionsValidationException">其余配置校验失败（如空白 ServiceName、相对 OtlpEndpoint）。</exception>
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
        // 配置绑定走 Configure<T>(o => section.Bind(o)) 源生成路径（AGENTS.md 红线），
        // 不使用 Configure<T>(IConfiguration) 反射重载。此处 section.Bind(options) 由
        // EnableConfigurationBindingGenerator 拦截为源生成代码，满足 AOT IL2026/IL3050 净零。
        configuration.GetSection(sectionPath).Bind(options);
        configure?.Invoke(options);

        return AddWechatOpenTelemetryCore(services, options);
    }

    /// <summary>
    /// 注册期校验 + 产品贡献描述 + 委托共享装配内核。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="options">已绑定/已配置的微信可观测性选项。</param>
    /// <returns>返回 <see cref="OpenTelemetryBuilder"/>。</returns>
    /// <remarks>
    /// <para>装配（<c>AddOpenTelemetry</c> / Resource / Sampler / 源与 Meter 注册 / Instrumentation 开关 /
    /// OTLP 导出器 / 批量导出 / ns2.0 AspNetCore 兜底 / 重复入口守卫）**全部在共享内核内完成**，
    /// 本方法只保留「校验 + 贡献 + 映射」三项产品侧职责。</para>
    /// </remarks>
    private static OpenTelemetryBuilder AddWechatOpenTelemetryCore(
        IServiceCollection services,
        WechatOpenTelemetryOptions options)
    {
        // 1) SamplingRatio 越界：保留既有 ArgumentOutOfRangeException 语义与优先级（先于完整校验）。
        if (options.SamplingRatio < 0 || options.SamplingRatio > 1)
            throw new ArgumentOutOfRangeException(nameof(options.SamplingRatio),
                $"SamplingRatio 必须在 0.0~1.0 范围内，当前值为 {options.SamplingRatio}。");

        // 2) 完整校验（CFG-10 口径：显式调用。注册进 DI 的 IValidateOptions 永不被 .NET options 管道触发，
        //    因为下方把预构建实例经 OptionsWrapper 直接注册为 IOptions<>）。
        //    修复此前「非法 ServiceName / 相对 OtlpEndpoint 静默通过」的缺陷。
        var validationResult = options.Validate(Options.DefaultName, options);
        if (validationResult.Failed)
        {
            throw new OptionsValidationException(
                Options.DefaultName, typeof(WechatOpenTelemetryOptions), validationResult.Failures!);
        }

        // 3) 保留既有 DI 面（既有测试与宿主契约锁定，勿删）：
        //    - IValidateOptions<T> 供宿主自行接线（如 AddOptions<T>().ValidateOnStart()）；
        //    - IOptions<T> 返回本次装配使用的同一实例，使容器内消费者读到一致配置。
        services.AddSingleton<IValidateOptions<WechatOpenTelemetryOptions>, WechatOpenTelemetryOptions>();
        services.AddSingleton<IOptions<WechatOpenTelemetryOptions>>(new OptionsWrapper<WechatOpenTelemetryOptions>(options));

        // 4) 产品线贡献描述（本包唯一的产品特有信息）+ 委托共享装配内核。
        return services.AddMudObservability(
            CreateContribution(options),
            WechatOpenTelemetryOptionsMapper.ToCore(options));
    }

    /// <summary>
    /// 构造本产品线对共享内核的贡献描述。
    /// </summary>
    /// <param name="options">微信可观测性选项（仅 <see cref="WechatOpenTelemetryOptions.IncludeMudHttpUtils"/> 参与贡献）。</param>
    /// <returns>贡献描述。</returns>
    /// <remarks>
    /// internal 而非 private：供测试直读（经 <c>InternalsVisibleTo</c>），锁定「产品名 / 单根源 /
    /// 产品线通配 Meter / MudHttp 源开关」四项产品特有契约。
    /// </remarks>
    internal static MudObservabilityContribution CreateContribution(WechatOpenTelemetryOptions options)
    {
        return new MudObservabilityContribution
        {
            ProductName = "Mud.Wechat",
            ActivitySourceName = WechatActivitySource.Name,
            MeterWildcard = WechatMeterWildcard,
            DefaultServiceName = DefaultServiceName,
            DefaultServiceVersion = WechatActivitySource.Version,
            IncludeMudHttpSources = options.IncludeMudHttpUtils,
        };
    }
}
