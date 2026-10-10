// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Extensions;

/// <summary>
/// 微信小程序服务集合扩展入口（对齐公众号 <c>AddMpServices</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌底座复用</b>：小程序<b>不新增令牌类型</b>，直接以公众号的 <c>AddMpApp</c> 注册
/// （其 <c>AppId</c> 即小程序 appid、<c>AppSecret</c> 即小程序 secret），令牌沿用
/// <c>Wechat.Mp.AccessToken</c> —— 语义依据是官方侧本就<b>同一平台、同一 <c>/cgi-bin/token</c> 端点</b>
/// （设计方案 §3.2 定案）。
/// </para>
/// <para>
/// 典型用法：
/// <code>
/// services.AddMpApp(configuration, "MpApps");
/// services.AddMiniProgramServices(builder => builder.AddAllApis());
/// </code>
/// </para>
/// </remarks>
public static class MiniProgramServiceCollectionExtensions
{
    /// <summary>按模块注册小程序业务客户端（枚举版）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="modules">模块集合。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentException"><paramref name="modules"/> 为空。</exception>
    public static IServiceCollection AddMiniProgramServices(
        this IServiceCollection services, params MiniProgramModule[] modules)
    {
        if (modules == null || modules.Length == 0)
        {
            throw new ArgumentException("至少需要指定一个模块。", nameof(modules));
        }

        return services.CreateMiniProgramServicesBuilder().AddModules(modules).Build();
    }

    /// <summary>按模块注册小程序业务客户端（委托版）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">模块注册委托。</param>
    /// <returns>服务集合（链式）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="configure"/> 为 <c>null</c>。</exception>
    public static IServiceCollection AddMiniProgramServices(
        this IServiceCollection services, Action<MiniProgramServiceBuilder> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        var builder = services.CreateMiniProgramServicesBuilder();
        configure(builder);
        return builder.Build();
    }

    /// <summary>创建模块注册器。</summary>
    /// <param name="services">服务集合。</param>
    /// <returns>模块注册器。</returns>
    public static MiniProgramServiceBuilder CreateMiniProgramServicesBuilder(this IServiceCollection services)
        => new(services);
}
