// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// per-app 企业微信 HTTP 客户端工厂接口（对齐 <c>IFeishuHttpClientFactory</c>）。
/// </summary>
public interface IWechatHttpClientFactory
{
    /// <summary>创建带令牌恢复装饰的客户端（业务 API 用；errcode 失效自动恢复）。</summary>
    IEnhancedHttpClient Create(string appKey, TokenRecoveryExecutor recoveryExecutor);

    /// <summary>创建不带恢复的客户端（认证/令牌签发接口用，避免恢复自递归）。</summary>
    IEnhancedHttpClient CreateBasic(string appKey);

    /// <summary>创建 per-app 客户端选项（拦截器、脱敏器、AOT resolver 等组件面叠加）。</summary>
    EnhancedHttpClientOptions CreateOptions(string appKey);
}

/// <summary>
/// per-app 企业微信 HTTP 客户端工厂（对齐 <c>FeishuHttpClientFactory</c>）。
/// </summary>
/// <remarks>
/// FeishuV3 不子类化 <see cref="EnhancedHttpClient"/>，而是经工厂按 appKey 创建组件客户端；
/// per-app BaseAddress 由命名 HttpClient（<c>wechat-work-{appKey}</c>）承载，
/// 严格模式（AllowCustomBaseUrl=false）下的连接期 SSRF 校验由组件
/// <c>AddMudHttpClient</c> 自动启用。
/// </remarks>
public class WechatHttpClientFactory : IWechatHttpClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>创建客户端工厂。</summary>
    public WechatHttpClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public IEnhancedHttpClient Create(string appKey, TokenRecoveryExecutor recoveryExecutor)
        => new TokenRecoveryEnhancedClient(
            _serviceProvider.GetRequiredService<IHttpClientFactory>(),
            BuildClientName(appKey),
            recoveryExecutor,
            null,
            CreateOptions(appKey));

    /// <inheritdoc />
    public IEnhancedHttpClient CreateBasic(string appKey)
        => new HttpClientFactoryEnhancedClient(
            _serviceProvider.GetRequiredService<IHttpClientFactory>(),
            BuildClientName(appKey),
            null,
            CreateOptions(appKey));

    /// <inheritdoc />
    public EnhancedHttpClientOptions CreateOptions(string appKey)
    {
        var options = new EnhancedHttpClientOptions
        {
            Logger = _serviceProvider.GetService<ILogger<HttpClientFactoryEnhancedClient>>(),
            RequestInterceptors = _serviceProvider.GetServices<IHttpRequestInterceptor>(),
            ResponseInterceptors = _serviceProvider.GetServices<IHttpResponseInterceptor>(),
        };

#if NET8_0_OR_GREATER
        options.JsonTypeInfoResolver = _serviceProvider
            .GetService<IOptions<System.Text.Json.JsonSerializerOptions>>()?.Value.TypeInfoResolver;
#endif

        return options;
    }

    /// <summary>per-app 命名客户端名（与 AddMudHttpClient 注册命名一致）。</summary>
    public static string BuildClientName(string appKey) => $"{Consts.HttpClientNamePrefix}-{appKey}";
}
