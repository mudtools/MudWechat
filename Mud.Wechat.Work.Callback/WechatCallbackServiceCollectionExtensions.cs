// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调接收服务注册扩展（P1-3 统一注册表形态，决策 D11）。
/// </summary>
/// <remarks>
/// <para>
/// <c>AddWechatCallback</c>（单条目便捷入口）与 <c>AddWechatCallbackSuite</c>（逐套件追加条目）
/// 是同一注册动作的两个语义化入口：均登记进统一注册表（接收方 ID 必填且全表唯一，注册期 fail-fast），
/// 无单/多套件模式之分。多套件宿主（每套件独立 Token/AESKey/接收方 ID）逐套件调用
/// <see cref="AddWechatCallbackSuite"/>；POST 报文由组合接收器按外层 XML ToUserName 自动路由。
/// </para>
/// <para>
/// 回调仓储（<see cref="IWechatSuiteTicketStore"/> / <see cref="IWechatCorpAuthStore"/>）
/// 复用令牌基座在 <c>AddWechatApp</c> 中注册的实例（TryAdd 语义：宿主可预注册分布式实现）。
/// </para>
/// </remarks>
public static class WechatCallbackServiceCollectionExtensions
{
    /// <summary>
    /// 注册企业微信回调接收（验签 + AES 解密 + 事件分发；单条目便捷入口）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">回调配置委托（PushToken / PushEncodingAESKey / CorpId=接收方 ID 必填）。</param>
    public static IServiceCollection AddWechatCallback(
        this IServiceCollection services,
        Action<WechatCallbackOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        var options = new WechatCallbackOptions();
        configure(options);
        return services.AddCallbackEntry(options);
    }

    /// <summary>
    /// 登记一套回调配置（多套件入口：每次调用向统一注册表追加一条，接收方 ID 必填且全表唯一）。
    /// </summary>
    /// <remarks>
    /// 与 <see cref="AddWechatCallback(IServiceCollection, Action{WechatCallbackOptions})"/> 行为一致（同一注册表），
    /// 仅命名语义区别：多套件宿主以本方法表达「逐套件登记」。
    /// </remarks>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">回调配置委托（PushToken / PushEncodingAESKey / CorpId=接收方 ID 必填且唯一）。</param>
    public static IServiceCollection AddWechatCallbackSuite(
        this IServiceCollection services,
        Action<WechatCallbackOptions> configure)
        => services.AddWechatCallback(configure);

    /// <summary>
    /// 从配置文件注册企业微信回调接收（配置节默认 <c>WechatCallback</c>）。
    /// </summary>
    /// <remarks>
    /// <b>注册期急切绑定</b>（P1-3/F12）：配置节须在调用时已就绪，缺失必填项在注册期即抛出；
    /// 注册后修改 <see cref="IConfiguration"/> 不会回填（如需动态重载请走委托注册并重建宿主）。
    /// </remarks>
#if NET6_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode("反射式配置绑定（ConfigurationBinder.Bind）在裁剪下无法静态分析配置类型成员")]
    [System.Diagnostics.CodeAnalysis.RequiresDynamicCode("反射式配置绑定（ConfigurationBinder.Bind）在 AOT/动态代码生成环境下不可用")]
#endif
    public static IServiceCollection AddWechatCallback(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "WechatCallback")
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        var options = new WechatCallbackOptions();
        configuration.GetSection(sectionName).Bind(options);
        return services.AddCallbackEntry(options);
    }

    private static IServiceCollection AddCallbackEntry(this IServiceCollection services, WechatCallbackOptions options)
    {
        GetOrCreateRegistry(services).Add(options);
        return services.AddWechatCallbackCore();
    }

    /// <summary>取或创建统一注册表（同一次注册链内的多个 Add 调用共享同一实例，条目追加立即可见）。</summary>
    private static WechatCallbackOptionsRegistry GetOrCreateRegistry(IServiceCollection services)
    {
        foreach (var descriptor in services)
        {
            if (descriptor.ServiceType == typeof(WechatCallbackOptionsRegistry) &&
                descriptor.ImplementationInstance is WechatCallbackOptionsRegistry registry)
            {
                return registry;
            }
        }

        var created = new WechatCallbackOptionsRegistry();
        services.AddSingleton(created);
        return created;
    }

    private static IServiceCollection AddWechatCallbackCore(this IServiceCollection services)
    {
        // P0-2：抗重放去重守卫（进程内默认；多实例部署由宿主 TryAdd 前置注册分布式实现）。
        services.TryAddSingleton<IWechatCallbackReplayGuard, InMemoryWechatCallbackReplayGuard>();

        // P1-3：组合接收器经工厂 lambda 注册（避免反射激活，AOT 安全）；
        // IWechatCallbackUrlVerifier 与 IWechatCallbackReceiver 共享同一组合接收器实例。
        services.TryAddSingleton<IWechatCallbackReceiver>(sp =>
        {
            var registry = sp.GetRequiredService<WechatCallbackOptionsRegistry>();
            var guard = sp.GetRequiredService<IWechatCallbackReplayGuard>();
            var entryLogger = sp.GetService<ILogger<WechatCallbackReceiver>>();
            var groupLogger = sp.GetService<ILogger<WechatCallbackReceiverGroup>>();
            return new WechatCallbackReceiverGroup(registry, guard, entryLogger, groupLogger);
        });
        services.TryAddSingleton<IWechatCallbackUrlVerifier>(sp =>
            (IWechatCallbackUrlVerifier)sp.GetRequiredService<IWechatCallbackReceiver>());
        services.TryAddSingleton<WechatCallbackHandler>();
        return services;
    }
}
