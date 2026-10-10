// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.Extensions;

/// <summary>
/// 微信小店 / 视频号服务集合扩展入口（对齐公众号 <c>AddMpServices</c> / 小程序 <c>AddMiniProgramServices</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌底座独立</b>：小店 AppID 与公众号 / 小程序 AppID 不互通，须以 <c>AddChannelsApp</c>
/// 注册（<c>AppId</c> + <c>AppSecret</c> + 双通道配置）。令牌沿用 <see cref="ChannelsTokenTypes.AccessToken"/>。
/// </para>
/// <para>
/// 典型用法：
/// <code>
/// builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
///         .AddWechatChannelsApi(b => b.AddAllApis());
/// </code>
/// </para>
/// </remarks>
public static class ChannelsServiceCollectionExtensions
{
    /// <summary>按模块注册小店业务客户端（枚举版）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="modules">模块集合。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentException"><paramref name="modules"/> 为空。</exception>
    public static IServiceCollection AddWechatChannelsApi(
        this IServiceCollection services, params ChannelsModule[] modules)
    {
        if (modules == null || modules.Length == 0)
        {
            throw new ArgumentException("至少需要指定一个模块。", nameof(modules));
        }

        return services.CreateChannelsServicesBuilder().AddModules(modules).Build();
    }

    /// <summary>按模块注册小店业务客户端（委托版）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">模块注册委托。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> 为 <c>null</c>。</exception>
    public static IServiceCollection AddWechatChannelsApi(
        this IServiceCollection services, Action<ChannelsServiceBuilder> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = services.CreateChannelsServicesBuilder();
        configure(builder);
        return builder.Build();
    }

    /// <summary>创建模块注册器。</summary>
    /// <param name="services">服务集合。</param>
    /// <returns>模块注册器。</returns>
    public static ChannelsServiceBuilder CreateChannelsServicesBuilder(this IServiceCollection services)
        => new(services);
}
