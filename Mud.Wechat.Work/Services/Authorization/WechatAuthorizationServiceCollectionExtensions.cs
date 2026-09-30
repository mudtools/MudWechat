// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Services.Authorization;

/// <summary>
/// 授权编排服务的 DI 注册扩展（由 <c>WechatWorkServiceBuilder.AddAuthenticationApi()</c> 调用）。
/// </summary>
/// <remarks>
/// <c>TryAdd</c> 语义：宿主可预注册分布式 / 定制实现覆盖默认实现。
/// </remarks>
public static class WechatAuthorizationServiceCollectionExtensions
{
    /// <summary>
    /// 注册授权编排服务与其策略选项。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置（可选）；非空时绑定 <c>WechatAuthorization</c> 配置节。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddWechatAuthorizationServices(
        this IServiceCollection services, IConfiguration? configuration = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddOptions<WechatAuthorizationOptions>()
            .Validate(
                options =>
                {
                    options.Validate();
                    return true;
                },
                "WechatAuthorizationOptions 配置无效。");

        if (configuration != null)
        {
            var section = configuration.GetSection(WechatAuthorizationOptions.SectionName);
            services.Configure<WechatAuthorizationOptions>(options => section.Bind(options));
        }

        services.TryAddSingleton<IWechatWorkAuthorizationService, WechatWorkAuthorizationService>();
        services.TryAddSingleton<IWechatAuthorizationCoordinator, WechatAuthorizationCoordinator>();

        return services;
    }
}