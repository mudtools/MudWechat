// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mud.Wechat.Work.Callback;

/// <summary>
/// 回调接收服务注册扩展。
/// </summary>
/// <remarks>
/// 回调仓储（<see cref="IWechatSuiteTicketStore"/> / <see cref="IWechatCorpAuthStore"/>）
/// 复用令牌底座在 <c>AddWechatApp</c> 中注册的实例（TryAdd 语义：宿主可预注册分布式实现）。
/// </remarks>
public static class WechatCallbackServiceCollectionExtensions
{
    /// <summary>
    /// 注册企业微信回调接收（验签 + AES 解密 + 事件分发）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">回调配置委托（PushToken / PushEncodingAESKey / CorpId）。</param>
    public static IServiceCollection AddWechatCallback(
        this IServiceCollection services,
        Action<WechatCallbackOptions> configure)
    {
        if (configure == null) throw new ArgumentNullException(nameof(configure));

        services.Configure(configure);
        return services.AddWechatCallbackCore();
    }

    /// <summary>
    /// 从配置文件注册企业微信回调接收（配置节默认 <c>WechatCallback</c>）。
    /// </summary>
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

        services.Configure<WechatCallbackOptions>(options => configuration.GetSection(sectionName).Bind(options));
        return services.AddWechatCallbackCore();
    }

    private static IServiceCollection AddWechatCallbackCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IWechatCallbackReceiver, WechatCallbackReceiver>();
        services.TryAddSingleton<WechatCallbackHandler>();
        return services;
    }
}
