// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Mud.Wechat.OfficialAccount.Abstractions.Callback;
using Mud.Wechat.OfficialAccount.Callback;

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>
/// 动态回调来源 IP 白名单注册入口（P10 可选增强；**默认不注册即关闭**）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何用「注册式开关」而非配置项</b>：官方 IP 会变动（V10 核验：建议每天刷新 1 次）且无静态 IP 段文档，
/// 故该能力必然带来**周期性 API 调用**。用配置项会导致「配了但没意识到会产生 API 调用」的隐性成本，
/// 而显式调用本扩展即等于「我确实要这个能力」⇒ 默认不调用即完全关闭（对既有宿主零影响）。
/// </para>
/// <para>
/// <b>与静态白名单的关系</b>：两者的并集生效（静态 <c>MpCallbackOptions.AllowedSourceIPs</c> 仍可用，
/// 便于在刷新失败时保留兜底来源）。
/// </para>
/// </remarks>
public static class MpCallbackSourceIpExtensions
{
    /// <summary>
    /// 注册动态回调来源 IP 白名单：刷新服务 + 内存快照提供者。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="refreshInterval">刷新间隔（默认 24 小时 —— 官方建议每天 1 次；小于等于 0 视为默认值）。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddMpCallbackSourceIpWhitelist(
        this IServiceCollection services, TimeSpan? refreshInterval = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var interval = refreshInterval.GetValueOrDefault(TimeSpan.FromHours(24));
        if (interval <= TimeSpan.Zero)
        {
            interval = TimeSpan.FromHours(24);
        }

        services.TryAddSingleton<MpCallbackSourceIpProvider>();
        services.TryAddSingleton<IMpCallbackSourceIpProvider>(
            sp => sp.GetRequiredService<MpCallbackSourceIpProvider>());
        services.AddHostedService(sp => new MpCallbackSourceIpRefreshService(
            sp.GetRequiredService<IServiceScopeFactory>(),
            sp.GetRequiredService<MpCallbackSourceIpProvider>(),
            interval,
            sp.GetRequiredService<ILogger<MpCallbackSourceIpRefreshService>>()));

        return services;
    }
}
