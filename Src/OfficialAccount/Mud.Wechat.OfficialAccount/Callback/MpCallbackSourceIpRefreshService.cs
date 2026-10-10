// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mud.Wechat.OfficialAccount;

namespace Mud.Wechat.OfficialAccount.Callback;

/// <summary>
/// 回调推送来源 IP 的定时刷新后台服务（默认**不注册**；宿主显式调用
/// <c>AddMpCallbackSourceIpWhitelist</c> 才生效）。
/// </summary>
/// <remarks>
/// <para>
/// <b>刷新策略（V10 已核验）</b>：启动后立即刷新一次，其后按间隔刷新（默认 24 小时 —— 官方建议每天 1 次）；
/// 刷新失败**不抛出**（记 <c>Warning</c> 后等待下个周期），以保持既有快照可用。
/// </para>
/// <para>
/// <b>多应用说明</b>：<c>IMpBasicService</c> 为声明式客户端（令牌由环境应用上下文注入），
/// 故本服务刷新的是<b>环境应用（未显式切换作用域时即默认应用）</b>的推送 IP。
/// 多应用且各应用推送 IP 不同的部署，须由宿主按 appKey 自行调用并写入自己的提供者实现。
/// </para>
/// </remarks>
public sealed class MpCallbackSourceIpRefreshService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly MpCallbackSourceIpProvider _provider;
    private readonly TimeSpan _interval;
    private readonly ILogger<MpCallbackSourceIpRefreshService> _logger;

    /// <summary>创建刷新服务。</summary>
    /// <param name="scopeFactory">scope 工厂（声明式 API 客户端须在 scope 内解析）。</param>
    /// <param name="provider">IP 快照提供者。</param>
    /// <param name="interval">刷新间隔（默认 24 小时）。</param>
    /// <param name="logger">日志器。</param>
    public MpCallbackSourceIpRefreshService(
        IServiceScopeFactory scopeFactory,
        MpCallbackSourceIpProvider provider,
        TimeSpan interval,
        ILogger<MpCallbackSourceIpRefreshService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _interval = interval <= TimeSpan.Zero ? TimeSpan.FromHours(24) : interval;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // 启动即刷新一次（避免「首次校验前无数据」的窗口被拉长）。
        await RefreshOnceAsync(stoppingToken).ConfigureAwait(false);

        // 用 Task.Delay 而非 PeriodicTimer：本包含 netstandard2.0 目标（无 PeriodicTimer）。
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_interval, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await RefreshOnceAsync(stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task RefreshOnceAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var basic = scope.ServiceProvider.GetRequiredService<IMpBasicService>();
            var response = await basic.GetCallbackIpAsync(cancellationToken).ConfigureAwait(false);

            var ips = response?.IpList;
            if (ips == null || ips.Count == 0)
            {
                // 失败响应由声明式客户端的判错契约（IWechatApiResponse）负责区分；此处仅保留既有快照。
                _logger.LogWarning("getcallbackip 未返回 ip_list，保留既有快照。");
                return;
            }

            _provider.Update(ips, DateTimeOffset.UtcNow);
            _logger.LogInformation(
                "已刷新回调推送来源 IP 列表：{Count} 条（下次刷新间隔 {Interval}）。",
                ips.Count, _interval);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            // 刷新失败不得影响回调接收：保留既有快照，下个周期重试。
            _logger.LogWarning(ex, "刷新回调推送来源 IP 失败，保留既有快照并等待下次刷新。");
        }
    }
}
