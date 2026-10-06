// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Extensions;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信服务集合扩展入口（对齐 <c>AddFeishuServices</c>）。
/// </summary>
/// <remarks>
/// 典型用法：
/// <code>
/// services.AddWechatApp(configuration, "WechatApps");
/// services.AddWechatWorkServices(builder => builder.AddAuthenticationApi());
/// </code>
/// </remarks>
public static class WechatWorkServiceCollectionExtensions
{
    /// <summary>
    /// 创建模块注册器。
    /// </summary>
    public static WechatWorkServiceBuilder CreateWechatWorkServicesBuilder(this IServiceCollection services)
        => new(services);

    /// <summary>
    /// 创建模块注册器（携带宿主配置，用于绑定 <c>WechatAuthorization</c> 配置节）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    public static WechatWorkServiceBuilder CreateWechatWorkServicesBuilder(
        this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        return new WechatWorkServiceBuilder(services, configuration);
    }

    /// <summary>
    /// 按模块注册业务客户端（入口，对齐 <c>AddFeishuServices(params)</c>）。
    /// </summary>
    public static IServiceCollection AddWechatWorkServices(this IServiceCollection services, params WechatModule[] modules)
    {
        if (modules == null || modules.Length == 0)
        {
            throw new ArgumentException("至少需要指定一个模块。", nameof(modules));
        }

        return services.CreateWechatWorkServicesBuilder().AddModules(modules).Build();
    }

    /// <summary>
    /// 按配置委托注册业务客户端（入口，对齐 <c>AddFeishuServices(Action)</c>）。
    /// </summary>
    public static IServiceCollection AddWechatWorkServices(
        this IServiceCollection services,
        Action<WechatWorkServiceBuilder> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = services.CreateWechatWorkServicesBuilder();
        configure(builder);
        return builder.Build();
    }

    /// <summary>
    /// 按配置委托注册业务客户端（携带宿主配置，绑定 <c>WechatAuthorization</c> 配置节）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">宿主配置。</param>
    /// <param name="configure">模块注册委托。</param>
    public static IServiceCollection AddWechatWorkServices(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<WechatWorkServiceBuilder> configure)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var builder = services.CreateWechatWorkServicesBuilder(configuration);
        configure(builder);
        return builder.Build();
    }

}
