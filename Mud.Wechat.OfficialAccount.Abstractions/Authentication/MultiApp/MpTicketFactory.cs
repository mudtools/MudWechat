// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Abstractions.Authentication.MultiApp;

/// <summary>per-app 票据签发 API 工厂接口（票据管理器使用本应用命名客户端）。</summary>
public interface IMpTicketFactory
{
    /// <summary>创建指定应用的票据签发客户端。</summary>
    /// <param name="appKey">应用键。</param>
    /// <returns>绑定到该应用命名客户端的 <see cref="IMpTicketService"/> 实例。</returns>
    IMpTicketService Create(string appKey);
}

/// <summary>
/// per-app 票据签发 API 工厂。
/// </summary>
/// <remarks>
/// <para>
/// 与 <c>PerAppMpAuthenticationFactory</c> 同构：装配到<b>真实实现类型</b>（生成类
/// <c>Internal.MpTicketService</c>，同程序集可编译期直引），客户端取 <c>CreateBasic(appKey)</c>
/// ——<b>不带</b>令牌恢复装饰。
/// </para>
/// <para>
/// <b>为何不带恢复装饰</b>：恢复执行器依赖令牌管理器注册表，而票据管理器本身就是
/// 「令牌管理器 → 票据」链路上的一环；在此处引入恢复装饰会形成
/// 「票据刷新 → 恢复链路 → 令牌管理器 → 票据客户端」的装配环。
/// <c>40001</c> 的自愈改由票据管理器基座显式处置（失效令牌 + 重试一次）。
/// </para>
/// <para>装配失败 fail-fast，不得静默回退到默认应用端点（否则会用错公众号取票据）。</para>
/// </remarks>
internal sealed class PerAppMpTicketFactory : IMpTicketFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMpHttpClientFactory _httpClientFactory;
    private readonly ILogger<PerAppMpTicketFactory>? _logger;

    /// <summary>创建票据 API 工厂。</summary>
    /// <param name="serviceProvider">服务提供器。</param>
    /// <param name="httpClientFactory">per-app HTTP 客户端工厂。</param>
    /// <param name="logger">日志器（可选）。</param>
    public PerAppMpTicketFactory(
        IServiceProvider serviceProvider,
        IMpHttpClientFactory httpClientFactory,
        ILogger<PerAppMpTicketFactory>? logger = null)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger;
    }

    /// <inheritdoc />
    public IMpTicketService Create(string appKey)
    {
        if (string.IsNullOrWhiteSpace(appKey))
        {
            throw new ArgumentException("应用标识不能为空。", nameof(appKey));
        }

        try
        {
            return new Internal.MpTicketService(
                _httpClientFactory.CreateBasic(appKey),
                _serviceProvider.GetRequiredService<IHttpRequestExecutor>(),
                _serviceProvider.GetService<IHttpResponseCache>(),
                _serviceProvider.GetService<IResiliencePolicyResolver>(),
                _serviceProvider.GetService<IHttpContentSerializer>(),
                _serviceProvider.GetService<ILogger<Internal.MpTicketService>>());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger?.LogError(ex, "per-app 票据客户端装配失败（应用 {AppKey}）。", appKey);
            throw;
        }
    }
}
