// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.TokenManager;

/// <summary>
/// 微信系令牌恢复设施登记入口（**跨产品线唯一登记点**）。
/// </summary>
/// <remarks>
/// <para>
/// 各产品线的 <c>AddXxxApp</c> 必须调用本方法（同值幂等，可重复调用）。它承担两件
/// 「进程级全局态」的登记，二者若由各产品线自行登记必然互相破坏：
/// </para>
/// <list type="number">
/// <item><b>SSRF 域名白名单</b>：组件 <c>UrlValidator.ConfigureAllowedDomains</c> 是全局静态 +
/// 整体替换语义 ⇒ 统一以 <see cref="WechatApiHosts.AllowedBaseUrlDomains"/> 这一<b>并集单一来源</b>登记，
/// 多产品线共存时结果恒等；</item>
/// <item><b>errcode 失效判定器</b>：组件选项属性为单槽 ⇒ 统一登记
/// <see cref="WechatCompositeTokenInvalidationDetector"/>（消费各产品线自注册的子判定器），
/// 避免「后注册者覆盖前者」导致某产品线令牌恢复静默失效。</item>
/// </list>
/// <para>
/// 子判定器的注册方式：产品线以
/// <c>services.TryAddEnumerable(ServiceDescriptor.Singleton&lt;ITokenInvalidationDetector, XxxDetector&gt;())</c>
/// 登记自身判定器；本方法<b>不</b>直接设置选项属性，而是经
/// <see cref="WechatTokenRecoveryPostConfigure"/>（<c>IPostConfigureOptions</c>）在所有配置绑定**之后**
/// 组装组合器，保证不被后续 <c>Configure(section.Bind)</c> 覆盖。
/// </para>
/// <para>未登记任何子判定器时，选项保持 <c>null</c> ⇒ 组件退化为「仅 HTTP 401 触发恢复」的默认语义。</para>
/// </remarks>
public static class WechatTokenRecoveryRegistration
{
    /// <summary>登记微信系令牌恢复设施（白名单 + 组合判定器 + 选项校验器）。</summary>
    /// <param name="services">服务集合。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 null。</exception>
    public static IServiceCollection AddWechatTokenRecovery(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        // ① SSRF 白名单：并集单一来源（同值调用幂等）。
        UrlValidator.ConfigureAllowedDomains(WechatApiHosts.AllowedBaseUrlDomains);

        // ② 令牌恢复选项 + 判定器组合器（PostConfigure 保证晚于任何配置绑定）。
        services.AddOptions<TokenRecoveryOptions>();
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IPostConfigureOptions<TokenRecoveryOptions>, WechatTokenRecoveryPostConfigure>());

        // ③ 选项校验与批量删除能力探测所需的实例注册（各产品线共用）。
        services.TryAddSingleton<IValidateOptions<TokenRecoveryOptions>, TokenRecoveryOptionsValidator>();
        services.TryAddSingleton(sp => sp.GetRequiredService<IOptions<TokenRecoveryOptions>>().Value);

        // ④ 令牌提供器与后台刷新框架服务（各产品线共用）。
        // 组件 AddTokenProvider 为 TryAdd（幂等），但 AddTokenRefreshBackgroundService 内部使用
        // AddSingleton + AddHostedService（**非幂等**）：多产品线各自调用会注册多个后台刷新循环。
        // 故在此以「已注册即跳过」的显式判断收敛为单点，使同一宿主引用多个产品线时只有一个刷新循环。
        services.AddTokenProvider();
        if (!services.Any(descriptor => descriptor.ServiceType == typeof(ITokenRefreshBackgroundService)))
        {
            services.AddTokenRefreshBackgroundService();
        }

        return services;
    }
}

/// <summary>
/// 令牌恢复选项后置配置：把 DI 中登记的全部
/// <see cref="ITokenInvalidationDetector"/>（各产品线子判定器）组装为组合器。
/// </summary>
/// <remarks>
/// 使用 <c>IPostConfigureOptions</c> 而非 <c>PostConfigure(Action&lt;T&gt;)</c> 是因为前者可由
/// 容器注入依赖（子判定器集合），且多个产品线调用
/// <see cref="WechatTokenRecoveryRegistration.AddWechatTokenRecovery"/> 时经
/// <c>TryAddEnumerable</c> 去重 ⇒ 组合器只组装一次。
/// </remarks>
public sealed class WechatTokenRecoveryPostConfigure : IPostConfigureOptions<TokenRecoveryOptions>
{
    private readonly IEnumerable<ITokenInvalidationDetector> _detectors;

    /// <summary>创建后置配置。</summary>
    /// <param name="detectors">各产品线登记的子判定器集合。</param>
    public WechatTokenRecoveryPostConfigure(IEnumerable<ITokenInvalidationDetector> detectors)
    {
        _detectors = detectors ?? throw new ArgumentNullException(nameof(detectors));
    }

    /// <inheritdoc />
    public void PostConfigure(string? name, TokenRecoveryOptions options)
    {
        if (options == null)
        {
            return;
        }

        var detectors = _detectors as IReadOnlyList<ITokenInvalidationDetector> ?? _detectors.ToArray();
        options.TokenInvalidationDetector = detectors.Count == 0
            ? null
            : new WechatCompositeTokenInvalidationDetector(detectors);
    }
}
