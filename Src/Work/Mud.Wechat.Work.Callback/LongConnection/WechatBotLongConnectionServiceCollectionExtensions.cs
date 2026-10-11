// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

#if NET8_0_OR_GREATER

using Microsoft.Extensions.Hosting;
using Mud.Wechat.Work.Abstractions.Callback.Bots;
using Mud.Wechat.Work.Abstractions.Configuration;

namespace Mud.Wechat.Work.Callback.LongConnection;

/// <summary>
/// 智能机器人长连接的装配入口（<c>AddWechatBotLongConnection</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>前置依赖</b>：必须先 <c>AddWechatCallback</c>（分发内核 <see cref="WechatBotEventDispatcher"/>
/// 在其核心装配中注册 —— 长连接与 HTTP 回调共用同一分发实例，ADR-8）；
/// 可选：先 <c>AddWechatRedis</c>（其端口面含 <see cref="IWechatBotConnectionLease"/> 的 Redis 实现，
/// 多实例部署的主备基座）。
/// </para>
/// <para>
/// <b>配置读取</b>：走 <c>Configure&lt;T&gt;(o =&gt; section.Bind(o))</c> 形态（源生成绑定器拦截，
/// 禁用 <c>Configure&lt;T&gt;(IConfiguration)</c> 反射重载 —— AGENTS §3）。
/// </para>
/// </remarks>
public static class WechatBotLongConnectionServiceCollectionExtensions
{
    /// <summary>
    /// 装配智能机器人长连接（配置节 <c>WechatBots</c>；启动期为每个 <c>EnableLongConnection=true</c>
    /// 的机器人建一条主循环任务）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置源（读取 <c>WechatBots</c> 节）。</param>
    /// <returns>服务集合。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> / <paramref name="configuration"/> 为 <c>null</c>。</exception>
    /// <exception cref="InvalidOperationException">未先调用 <c>AddWechatCallback</c>。</exception>
    public static IServiceCollection AddWechatBotLongConnection(
        this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (!services.Any(static s => s.ServiceType == typeof(WechatBotEventDispatcher)))
        {
            throw new InvalidOperationException(
                "请先调用 AddWechatCallback(...)（长连接与 HTTP 回调共用同一分发内核）再调用 AddWechatBotLongConnection()。");
        }

        // 惰性绑定（与 WechatCallbackOptions 的 IConfiguration 重载同形）；启动期快照语义由
        // 裸类型单例承载（Runner 构造注入 WechatBotOptions —— 热更不改变已建连接，与
        // MaxConcurrentEvents 的构造期快照口径一致）。
        services.Configure<WechatBotOptions>(options => configuration.GetSection(WechatBotOptions.DefaultSectionName).Bind(options));
        services.AddSingleton<WechatBotOptions>(static sp => sp.GetRequiredService<IOptions<WechatBotOptions>>().Value);

        services.AddHostedService<WechatBotLongConnectionRunner>();
        return services;
    }
}

#endif
