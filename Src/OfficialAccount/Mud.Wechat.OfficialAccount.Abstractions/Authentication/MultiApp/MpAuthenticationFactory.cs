// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;

/// <summary>per-app 认证 API 工厂接口（令牌签发客户端使用本应用命名客户端）。</summary>
public interface IMpAuthenticationFactory
{
    /// <summary>创建指定应用的令牌签发客户端。</summary>
    /// <param name="appKey">应用键。</param>
    /// <returns>绑定到该应用命名客户端的 <see cref="IMpAuthentication"/> 实例。</returns>
    IMpAuthentication Create(string appKey);
}

/// <summary>
/// per-app 公众号认证 API 工厂。
/// </summary>
/// <remarks>
/// per-app 客户端必须装配到<b>真实实现类型</b>（生成类 <c>Internal.MpAuthentication</c>，同程序集可
/// 编译期直引）。生成 ctor 顺序（HttpClient 模式）：
/// <c>(IEnhancedHttpClient, IHttpRequestExecutor, IHttpResponseCache?, IResiliencePolicyResolver?,
/// IHttpContentSerializer?, ILogger?)</c>。装配失败 fail-fast，不得静默回退到默认应用端点。
/// </remarks>
internal sealed class PerAppMpAuthenticationFactory : IMpAuthenticationFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMpHttpClientFactory _httpClientFactory;
    private readonly ILogger<PerAppMpAuthenticationFactory>? _logger;

    /// <summary>创建认证 API 工厂。</summary>
    /// <param name="serviceProvider">服务提供器。</param>
    /// <param name="httpClientFactory">per-app HTTP 客户端工厂。</param>
    /// <param name="logger">日志器（可选）。</param>
    public PerAppMpAuthenticationFactory(
        IServiceProvider serviceProvider,
        IMpHttpClientFactory httpClientFactory,
        ILogger<PerAppMpAuthenticationFactory>? logger = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger;
    }

    /// <inheritdoc />
    public IMpAuthentication Create(string appKey)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentException("应用标识不能为空。", nameof(appKey));
        }

        try
        {
            return new Internal.MpAuthentication(
                _httpClientFactory.CreateBasic(appKey),
                _serviceProvider.GetRequiredService<IHttpRequestExecutor>(),
                _serviceProvider.GetService<IHttpResponseCache>(),
                _serviceProvider.GetService<IResiliencePolicyResolver>(),
                _serviceProvider.GetService<IHttpContentSerializer>(),
                _serviceProvider.GetService<ILogger<Internal.MpAuthentication>>());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger?.LogError(ex, "per-app 认证客户端装配失败（应用 {AppKey}）。", appKey);
            throw;
        }
    }
}
