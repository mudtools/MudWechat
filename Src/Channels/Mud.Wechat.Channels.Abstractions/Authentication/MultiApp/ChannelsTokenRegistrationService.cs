// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Abstractions.Authentication.MultiApp;

/// <summary>
/// 令牌管理器登记服务：启动时把<b>默认小店应用</b>的可刷新管理器登记到组件后台刷新服务。
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>登记门槛由组件判定（<c>ITokenManager.SupportsBackgroundRefresh</c>，默认 true）；</item>
/// <item><b>仅默认应用</b>在启动期登记，非默认应用按需懒加载刷新（不遍历 <c>ConfiguredConfigs</c> 物化
/// 全部应用——那会破坏「未访问的应用不初始化」的懒加载不变量）；</item>
/// <item>本服务只负责「登记」，刷新循环由组件服务承担（SDK 不自建 HostedService）；</item>
/// <item>登记失败不阻断启动（令牌仍可按需懒加载刷新），仅记告警。</item>
/// </list>
/// </remarks>
internal sealed class ChannelsTokenRegistrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ChannelsTokenRegistrationService> _logger;

    /// <summary>创建登记服务。</summary>
    /// <param name="serviceProvider">服务提供器。</param>
    /// <param name="logger">日志器。</param>
    public ChannelsTokenRegistrationService(IServiceProvider serviceProvider, ILogger<ChannelsTokenRegistrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // 启动期一致性自检：IAppContextHolder 与 IChannelsAppContextSwitcher 必须是同一实例，
        // 否则作用域切换对声明式（[Token]）客户端无效（多应用下静默使用默认应用令牌）。仅告警。
        var holder = _serviceProvider.GetService<IAppContextHolder>();
        var switcher = _serviceProvider.GetService<IChannelsAppContextSwitcher>();
        if (holder != null && switcher != null && !ReferenceEquals(holder, switcher))
        {
            _logger.LogWarning(
                "IAppContextHolder 与 IChannelsAppContextSwitcher 不是同一实例：UseAppScope/UseDefaultAppScope 对声明式（[Token]）客户端可能无效。" +
                "请确保只注册一个 AsyncLocal 上下文实现（宿主自定义 holder 时须与切换器同实例）。");
        }

        var background = _serviceProvider.GetService<ITokenRefreshBackgroundService>();
        if (background == null)
        {
            return Task.CompletedTask;
        }

        try
        {
            var appManager = _serviceProvider.GetRequiredService<ChannelsAppManager>();
            var appKey = appManager.DefaultAppKey;
            if (string.IsNullOrEmpty(appKey))
            {
                return Task.CompletedTask;
            }

            var context = appManager.GetApp(appKey);
            background.RegisterTokenManager(context.AccessTokenManager, $"Wechat.Channels.AccessToken:{appKey}");
            _logger.LogInformation("默认小店应用 {AppKey} 的令牌管理器已登记到后台刷新服务。", appKey);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "默认小店应用令牌管理器登记到后台刷新服务失败（将退化为按需刷新）。");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}