// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.Options;
using Mud.Wechat.Ads.Abstractions.Auth;
using System.Text.Json;

namespace Mud.Wechat.Ads.Abstractions.Transport;

/// <summary>
/// 广告线命名 HttpClient 的名称、默认接入点与类型名（供 <c>[HttpClientApi]</c> 引用）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么是两个客户端而不是一个</b>：官方 <c>oauth/token</c> 页「使用说明」原文是
/// 「OAuth 相关接口<b>无需</b>提供 access_token、timestamp、nonce 等通用请求参数」，
/// 而业务接口的「全局参数」表要求每个请求都带这三项。
/// 令牌注入只能挂在客户端管道上（<c>AdsAuthorizationHandler</c>），挂在<b>请求</b>上又无法区分两类端点
/// ⇒ 唯一的干净解法是<b>两个命名客户端各占一个 DI 类型键</b>：
/// <c>ads</c>（带令牌 Handler）与 <c>ads-oauth</c>（不带）。
/// </para>
/// <para>
/// <b>但官方自己并不自洽</b>：<c>oauth/refresh_token</c> 页<b>照抄</b>了全局参数表。
/// 建模取「换取与刷新均不带令牌」（依 token 页的显式说明），并把该矛盾写进
/// <c>IAdsAuthorizationService</c> 的 XML；若真实调用被拒，属官方文档缺陷，由应答 <c>code</c> 表达。
/// </para>
/// <para>
/// <b>类型名必须写全限定名</b>：生成器 <c>ValidateHttpClientType</c> 对自定义类型按 metadata name
/// 精确查找（仅 <c>IEnhancedHttpClient</c> / <c>IBaseHttpClient</c> 有短名特判），
/// 用 <c>nameof(...)</c> 只会产出短名 ⇒ HTTPCLIENT014。故此处提供常量而非让调用方拼写。
/// </para>
/// </remarks>
public static class AdsHttpClientNames
{
    /// <summary>业务客户端名称（带 <c>access_token</c> / <c>timestamp</c> / <c>nonce</c> 成组注入）。</summary>
    public const string ClientName = "ads";

    /// <summary>OAuth 客户端名称（<b>不带</b>任何令牌注入）。</summary>
    public const string OAuthClientName = "ads-oauth";

    /// <summary>
    /// 默认接入点（官方主域名，已在 SSRF 白名单内：以 <c>e.qq.com</c> 后缀条目覆盖）。
    /// 值直接取公用层唯一来源 <see cref="WechatApiHosts.AdsBaseUrl"/>，<b>不</b>在本线复制字面量。
    /// </summary>
    /// <remarks>
    /// 报表文件下载走<b>另一台主机</b> <c>dl.e.qq.com</c>（官方 <c>async_report_files/get</c> 的请求地址原文），
    /// 该主机同样被 <c>e.qq.com</c> 后缀覆盖 ⇒ <b>不</b>新增独立白名单条目（守卫 ADS-B4）。
    /// </remarks>
    public const string BaseAddress = WechatApiHosts.AdsBaseUrl;

    /// <summary>报表文件下载主机（<c>async_report_files/get</c> 专用）。</summary>
    public const string DownloadHost = "dl.e.qq.com";

    /// <summary>业务客户端类型的全限定 metadata name，供 <c>[HttpClientApi(HttpClient = ...)]</c> 使用。</summary>
    public const string TypeName = "Mud.Wechat.Ads.Abstractions.Transport.IAdsHttpClient";

    /// <summary>OAuth 客户端类型的全限定 metadata name。</summary>
    public const string OAuthTypeName = "Mud.Wechat.Ads.Abstractions.Transport.IAdsOAuthHttpClient";
}

/// <summary>
/// 广告线业务 HTTP 客户端 —— <c>[HttpClientApi(HttpClient = <see cref="AdsHttpClientNames.TypeName"/>)]</c> 的注入目标。
/// </summary>
/// <remarks>
/// 标记式接口（成员全部来自 <see cref="IEnhancedHttpClient"/>）：它存在的意义是<b>占住一个独立的 DI 类型键</b>，
/// 使广告线拿到的客户端带有 <c>AdsAuthorizationHandler</c>，而不与其它产品线共享默认客户端
/// （<c>IEnhancedHttpClient</c> 的默认实例已被企微线占用，且 BaseAddress 会互相冲突）。
/// </remarks>
public interface IAdsHttpClient : IEnhancedHttpClient;

/// <summary>
/// 广告线 OAuth HTTP 客户端（<c>oauth/token</c> / <c>oauth/refresh_token</c> 专用，<b>不带</b>令牌注入 Handler）。
/// </summary>
public interface IAdsOAuthHttpClient : IEnhancedHttpClient;

/// <summary>
/// 两个广告线客户端的共同实现：命名客户端 + 组件能力面包装（形态照 <c>WechatPayHttpClient</c>）。
/// </summary>
/// <remarks>
/// 不自造传输，而是经组件 <c>AddMudHttpClient</c> 注册命名客户端（由它带上追踪 Handler 与
/// <b>连接期 SSRF 严格模式</b>），再用 <see cref="HttpClientFactoryEnhancedClient"/> 包一层拿到
/// <see cref="IEnhancedHttpClient"/> 能力面。派生类只区别「取哪个命名客户端」。
/// </remarks>
public abstract class AdsEnhancedHttpClient : HttpClientFactoryEnhancedClient
{
    /// <summary>包装指定命名客户端。</summary>
    /// <param name="serviceProvider">服务提供器。</param>
    /// <param name="clientName">命名客户端名称。</param>
    protected AdsEnhancedHttpClient(IServiceProvider serviceProvider, string clientName)
        : base(
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            clientName,
            encryptionProvider: null,
            options: CreateOptions(serviceProvider, clientName))
    {
    }

    private static EnhancedHttpClientOptions CreateOptions(IServiceProvider serviceProvider, string clientName)
    {
        var options = new EnhancedHttpClientOptions
        {
            Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger("Mud.Wechat.Ads." + clientName),
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

/// <summary><see cref="IAdsHttpClient"/> 默认实现（业务客户端）。</summary>
public sealed class AdsHttpClient : AdsEnhancedHttpClient, IAdsHttpClient
{
    /// <summary>创建业务客户端。</summary>
    /// <param name="serviceProvider">服务提供器。</param>
    public AdsHttpClient(IServiceProvider serviceProvider)
        : base(serviceProvider, AdsHttpClientNames.ClientName)
    {
    }
}

/// <summary><see cref="IAdsOAuthHttpClient"/> 默认实现（OAuth 客户端，无令牌 Handler）。</summary>
public sealed class AdsOAuthHttpClient : AdsEnhancedHttpClient, IAdsOAuthHttpClient
{
    /// <summary>创建 OAuth 客户端。</summary>
    /// <param name="serviceProvider">服务提供器。</param>
    public AdsOAuthHttpClient(IServiceProvider serviceProvider)
        : base(serviceProvider, AdsHttpClientNames.OAuthClientName)
    {
    }
}

/// <summary>
/// 广告线凭据取用端口（传输层与业务侧共用的唯一咽喉点）。
/// </summary>
/// <remarks>
/// 由 <see cref="AdsAuthorizationService"/> 实现：负责「缓存命中直接给 / 阈值内提前刷新 /
/// 刷新失败先删库再抛 <c>WechatAdsReauthorizationRequiredException</c>」。
/// 之所以是<b>端口</b>而非让 Handler 直接依赖具体服务：令牌注入面（Handler，Abstractions 注册）
/// 与令牌获取面（需要 DataModels 的 OAuth 应答类型）都在本包内，接口只暴露「给我一个可用 access_token」
/// 这一件事，避免把 <c>AdsAppConfig</c> / 授权状态形态从 Handler 的构造签名里泄漏出去。
/// </remarks>
public interface IAdsAccessTokenProvider
{
    /// <summary>取一个当前可用的 <c>access_token</c>（必要时自动刷新并写穿存储）。</summary>
    /// <param name="appKey">应用键；<c>null</c> / 空白表示取当前应用（作用域优先，回落默认）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>可用的访问令牌。</returns>
    /// <exception cref="Exceptions.WechatAdsReauthorizationRequiredException">无刷新能力或刷新被拒。</exception>
    Task<string> GetAccessTokenAsync(string? appKey, CancellationToken cancellationToken = default);
}
