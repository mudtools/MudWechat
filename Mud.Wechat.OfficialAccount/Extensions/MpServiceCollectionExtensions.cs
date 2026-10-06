// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.Extensions;

/// <summary>
/// 公众号服务集合扩展入口（对齐企微 <c>AddWechatWorkServices</c>）。
/// </summary>
/// <remarks>
/// 典型用法：
/// <code>
/// services.AddMpApp(configuration, "MpApps");
/// services.AddMpServices(builder => builder.AddBasicApi());
/// </code>
/// </remarks>
public static class MpServiceCollectionExtensions
{
    /// <summary>创建模块注册器。</summary>
    /// <param name="services">服务集合。</param>
    /// <returns>模块注册器。</returns>
    public static MpServiceBuilder CreateMpServicesBuilder(this IServiceCollection services)
        => new(services);

    /// <summary>按模块注册业务客户端（入口）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="modules">模块集合。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddMpServices(this IServiceCollection services, params MpModule[] modules)
    {
        if (modules == null || modules.Length == 0)
        {
            throw new ArgumentException("至少需要指定一个模块。", nameof(modules));
        }

        return services.CreateMpServicesBuilder().AddModules(modules).Build();
    }

    /// <summary>按配置委托注册业务客户端（入口）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">模块注册委托。</param>
    /// <returns>服务集合（链式）。</returns>
    public static IServiceCollection AddMpServices(
        this IServiceCollection services,
        Action<MpServiceBuilder> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = services.CreateMpServicesBuilder();
        configure(builder);
        return builder.Build();
    }
}
