// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mud.Wechat.AI.Abstractions.Tools;

namespace Mud.Wechat.Work.AI.Mcp.Extensions;

/// <summary>
/// MCP 协议层的 DI 装配入口（在 <c>AddWechatWorkTools</c> 之后调用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>顺序无关但依赖存在</b>：工具面（执行链 + 注册表 + 策略）必须先注册——本方法只是把
/// <see cref="WechatMcpServer"/> 以单例挂上去，解析期才取 <see cref="WechatToolDispatcher"/>。
/// 未注册工具面即解析失败（fail-fast，不会得到「空工具列表」的假 server）。
/// </para>
/// <para>
/// <b>不注册 stdio 循环</b>：传输是宿主进程的事（谁持有 stdin/stdout 谁负责启停），
/// 本方法只交付 server 实例；宿主在自己的 HostedService / Main 里调
/// <see cref="WechatMcpServer.RunStdioAsync"/>，或把 HTTP 请求体喂给 <see cref="WechatMcpServer.HandleAsync"/>。
/// </para>
/// </remarks>
public static class WechatMcpServiceCollectionExtensions
{
    /// <summary>注册 MCP server（单例）。</summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">自描述信息配置（可空 ⇒ 默认名与版本）。</param>
    /// <returns>服务集合（可串联）。</returns>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> 为 <see langword="null"/>。</exception>
    public static IServiceCollection AddWechatWorkMcpServer(
        this IServiceCollection services,
        Action<WechatMcpServerOptions>? configure = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.TryAddSingleton(provider =>
        {
            var options = new WechatMcpServerOptions();
            configure?.Invoke(options);
            return new WechatMcpServer(provider.GetRequiredService<IWechatToolSurface>(), options);
        });

        return services;
    }
}
