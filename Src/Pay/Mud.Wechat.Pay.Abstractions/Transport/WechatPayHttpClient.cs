// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mud.HttpUtils;
using System.Text.Json;

namespace Mud.Wechat.Pay.Abstractions.Transport;

/// <summary>
/// <see cref="IWechatPayHttpClient"/> 默认实现：命名客户端 + 支付签名 Handler。
/// </summary>
/// <remarks>
/// 形态照本仓既有先例 <c>MpHttpClientFactory</c>：不自造传输，而是经组件的
/// <c>AddMudHttpClient</c> 注册命名客户端（由它带上追踪 Handler 与<b>连接期 SSRF 严格模式</b>），
/// 再用 <see cref="HttpClientFactoryEnhancedClient"/> 包一层拿到 <c>IEnhancedHttpClient</c> 能力面。
/// </remarks>
internal sealed class WechatPayHttpClient : HttpClientFactoryEnhancedClient, IWechatPayHttpClient
{
    /// <summary>
    /// 创建支付客户端（由 DI 解析，构造期即绑定命名客户端与签名 Handler）。
    /// </summary>
    /// <param name="serviceProvider">服务提供器（解析 <c>IHttpClientFactory</c> 与拦截器等组件面）。</param>
    public WechatPayHttpClient(IServiceProvider serviceProvider)
        : base(
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            WechatPayHttpClientNames.ClientName,
            encryptionProvider: null,
            options: CreateOptions(serviceProvider))
    {
    }

    private static EnhancedHttpClientOptions CreateOptions(IServiceProvider serviceProvider)
    {
        var options = new EnhancedHttpClientOptions
        {
            // 组件面叠加（与 MpHttpClientFactory 同形）：宿主注册的拦截器对支付线同样生效。
            Logger = serviceProvider.GetService<ILogger<WechatPayHttpClient>>(),
            RequestInterceptors = serviceProvider.GetServices<IHttpRequestInterceptor>(),
            ResponseInterceptors = serviceProvider.GetServices<IHttpResponseInterceptor>(),
        };

#if NET8_0_OR_GREATER
        options.JsonTypeInfoResolver = serviceProvider
            .GetService<IOptions<JsonSerializerOptions>>()?.Value.TypeInfoResolver;
#endif

        return options;
    }
}
