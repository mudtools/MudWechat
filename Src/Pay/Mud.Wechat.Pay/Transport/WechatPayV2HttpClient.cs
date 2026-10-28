// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;
using Mud.Wechat.Pay.Abstractions.Transport;

namespace Mud.Wechat.Pay.V2.Transport;

/// <summary>
/// <see cref="IWechatPayV2HttpClient"/> 默认实现：命名客户端 + XML 序列化器 + 签名 Handler。
/// </summary>
/// <remarks>
/// 形态照 APIv3 的 <see cref="WechatPayHttpClient"/>（经组件 <c>AddMudHttpClient</c> 拿追踪 Handler 与
/// 连接期 SSRF 严格模式，再用 <see cref="HttpClientFactoryEnhancedClient"/> 包出能力面），
/// 差别只有两处：<b>①换命名客户端</b>（不同 Handler 管道）、<b>②注入 XML 序列化器</b>
/// （构造末位形参 <c>contentSerializer</c>，组件为此留的唯一扩展点）。
/// </remarks>
internal sealed class WechatPayV2HttpClient : HttpClientFactoryEnhancedClient, IWechatPayV2HttpClient
{
    /// <summary>
    /// 创建 APIv2 普通端点客户端（由 DI 解析）。
    /// </summary>
    /// <param name="serviceProvider">服务提供器。</param>
    /// <param name="contentSerializer">本线 XML 序列化器（由 <c>AddWechatPayV2Api</c> 注册）。</param>
    public WechatPayV2HttpClient(
        IServiceProvider serviceProvider, WechatPayV2ContentSerializer contentSerializer)
        : base(
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            WechatPayV2HttpClientNames.ClientName,
            encryptionProvider: null,
            options: CreateOptions(serviceProvider),
            contentSerializer: contentSerializer)
    {
    }

    private static EnhancedHttpClientOptions CreateOptions(IServiceProvider serviceProvider)
    {
        var options = new EnhancedHttpClientOptions
        {
            Logger = serviceProvider.GetService<ILogger<WechatPayV2HttpClient>>(),
            RequestInterceptors = serviceProvider.GetServices<IHttpRequestInterceptor>(),
            ResponseInterceptors = serviceProvider.GetServices<IHttpResponseInterceptor>(),
        };

#if NET8_0_OR_GREATER
        // 与 APIv3 同一处口径：把宿主 IOptions<JsonSerializerOptions> 的解析器链带进能力面，
        // 供「非报文体」的旁路序列化（如组件内部日志脱敏取样）复用；报文本身的 XML 编解码不受它影响
        // （那一路走 WechatPayV2ContentSerializer 里钉死的源生成上下文）。
        options.JsonTypeInfoResolver = serviceProvider
            .GetService<IOptions<JsonSerializerOptions>>()?.Value.TypeInfoResolver;
#endif

        return options;
    }
}
