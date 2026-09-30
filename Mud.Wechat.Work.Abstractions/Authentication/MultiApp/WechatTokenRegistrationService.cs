// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

namespace Mud.Wechat.Work.Abstractions.Authentication.MultiApp;

/// <summary>
/// 令牌管理器登记服务（对齐 <c>FeishuTokenRegistrationService</c> 职能）：
/// 启动时把默认应用的后台可刷新管理器登记到框架 <see cref="ITokenRefreshBackgroundService"/>。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>登记门槛由框架判定（<c>ITokenManager.SupportsBackgroundRefresh</c>，默认 true）；</item>
/// <item>自建应用 access_token / 服务商 provider_access_token / 套件 suite_access_token 可后台预热；</item>
/// <item>企业级 access_token（代开发）默认<b>不</b>登记——企业可能停用，保持按需刷新；
/// 如需预热由宿主显式经 ITokenRefreshBackgroundService.RegisterTokenManager 登记；</item>
/// <item>本服务只负责"登记"，刷新循环由框架服务承担（SDK 不自建 HostedService）。</item>
/// <item>
/// <b>P2-11 刷新范围语义（明确声明）</b>：仅<b>默认应用</b>在启动期登记。<b>非默认应用按需刷新</b>
/// （首次访问其令牌时懒加载刷新），<b>不</b>为后台预热而遍历 <c>ConfiguredConfigs</c> 物化全部应用——
/// 那会破坏「未访问的应用不初始化」的懒加载不变量（D6），与"控制启动开销"的目标相悖。
/// 需要为非默认应用预热时，宿主可自行登记：
/// <c>background.RegisterTokenManager(appManager.GetApp(key).InternalAppTokenManager!, $"Wechat.InternalApp:{key}")</c>。
/// </item>
/// </list>
/// </remarks>
internal sealed class WechatTokenRegistrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WechatTokenRegistrationService> _logger;

    /// <summary>创建登记服务。</summary>
    public WechatTokenRegistrationService(IServiceProvider serviceProvider, ILogger<WechatTokenRegistrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // P0-1（G8-A）：启动期一致性自检——IAppContextHolder 与 IWechatAppContextSwitcher 必须是同一实例，
        // 否则 UseApp/BeginScope 对环境上下文（AsyncLocal）的写入对声明式（[Token]）客户端无效，
        // 多套件下会静默使用默认应用令牌。仅告警，不阻断启动。
        var holder = _serviceProvider.GetService<IAppContextHolder>();
        var switcher = _serviceProvider.GetService<IWechatAppContextSwitcher>();
        if (holder != null && switcher != null && !ReferenceEquals(holder, switcher))
        {
            _logger.LogWarning(
                "IAppContextHolder 与 IWechatAppContextSwitcher 不是同一实例：UseApp/BeginScope 对声明式（[Token]）客户端可能无效。" +
                "请确保只注册一个 AsyncLocal 上下文实现（宿主自定义 holder 时须与切换器同实例）。");
        }

        // 后台刷新服务未注册（宿主裁剪）时静默跳过。
        var background = _serviceProvider.GetService<ITokenRefreshBackgroundService>();
        if (background == null)
        {
            return Task.CompletedTask;
        }

        try
        {
            var appManager = _serviceProvider.GetRequiredService<WechatAppManager>();
            var appKey = appManager.DefaultAppKey;
            if (string.IsNullOrEmpty(appKey))
            {
                return Task.CompletedTask;
            }

            var context = appManager.GetApp(appKey);

            if (context.InternalAppTokenManager != null)
            {
                background.RegisterTokenManager(context.InternalAppTokenManager, $"Wechat.InternalApp:{appKey}");
            }

            if (context.ProviderTokenManager != null)
            {
                background.RegisterTokenManager(context.ProviderTokenManager, $"Wechat.Provider:{appKey}");
            }

            if (context.SuiteTokenManager != null)
            {
                background.RegisterTokenManager(context.SuiteTokenManager, $"Wechat.Suite:{appKey}");
            }

            _logger.LogInformation(
                "默认应用 {AppKey} 的令牌管理器已登记到后台刷新服务（AppType={AppType}）。",
                appKey, context.AppType);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            // 登记失败不阻断启动（令牌仍可按需懒加载刷新），仅记录告警。
            _logger.LogWarning(ex, "默认应用令牌管理器登记到后台刷新服务失败（将退化为按需刷新）。");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
