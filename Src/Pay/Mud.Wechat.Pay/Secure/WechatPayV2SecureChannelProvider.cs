// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using Mud.Wechat.Pay.V2.Credential;

namespace Mud.Wechat.Pay.V2.Secure;

/// <summary>
/// APIv2 双向证书通道持有者：按<b>商户键</b>缓存一条自带该商户客户端证书的 HTTPS 通道。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么这几支端点不走声明式生成</b>：<c>secapi/pay/*</c> 与两份账单下载要求
/// <b>客户端证书</b>（TLS 握手期呈现，域名为 <c>api2.mch.weixin.qq.com</c>），而
/// <c>IHttpClientFactory</c> 的主 Handler 是<b>按客户端名</b>创建并缓存、连接池亦按名复用 ——
/// 一条共享命名客户端无法承载多商户证书：
/// 若把证书选择塞进 .NET 8 的 <c>SslClientAuthenticationOptionsCallback</c>，
/// 已建立并复用的连接<b>不再触发回调</b>，症状是「商户 A 的请求跑在商户 B 的 TLS 会话上」，
/// 且低 TFM（net6.0）根本没有这个回调可挂。
/// 故安全侧改为<b>每商户一条独立通道</b>（独立 Handler ⇒ 独立连接池 ⇒ 证书与会话一一对应），
/// 端点方法仍集中在一个业务接口上，只是实现不走源生成。
/// </para>
/// <para>
/// <b>缓存键是「商户 + 域名」而不是「商户」</b>：证书族并不全在 <c>api2.mch</c> 上 ——
/// 现金红包（<c>/mmpaymkttransfers/*</c>）、企业付款到零钱与付款到银行卡
/// （<c>/mmpaysptrans/*</c>）官方文档给的是 <c>api.mch</c> 域名却<b>同样要求客户端证书</b>。
/// 若按商户缓存单一通道，红包请求就会被迫发到 <c>api2.mch</c>（该域名下无此路由 ⇒ 404），
/// 或退化到无证书的命名客户端（⇒ 握手期不呈现证书，官方拒）。
/// 故同一商户可以各持一条 <c>api.mch</c>、<c>api2.mch</c>、<c>apihk.mch</c> 与 <c>fraud.mch</c> 通道，互不复用连接。
/// </para>
/// <para>
/// <b>SSRF 口径</b>：通道的 <c>BaseAddress</c> 取自
/// <see cref="WechatPayV2HttpClientNames.BaseAddress"/> / <see cref="WechatPayV2HttpClientNames.SecureBaseAddress"/>
/// / <see cref="WechatPayV2HttpClientNames.GlobalBaseAddress"/> / <see cref="WechatPayV2HttpClientNames.FraudBaseAddress"/>
/// 四个编译期常量（均在 <c>WechatApiHosts</c> 白名单内），
/// 调用方只能四选一、
/// 请求路径全部来自代码里的字面量 ⇒ 不存在宿主可控 URL，绕开组件命名客户端不引入新的白名单豁免口。
/// 服务端证书校验保持默认（<b>不</b>关闭），证书校验失败即请求失败。
/// </para>
/// <para>
/// <b>生命周期</b>：通道一旦建出即随本 Provider 存活（商户级复用），进程退出时由
/// <see cref="Dispose"/> 统一释放。证书材料由
/// <see cref="IWechatPayV2CredentialProvider"/> 取得并交由通道持有，异常消息绝不回显材料内容。
/// </para>
/// </remarks>
internal sealed class WechatPayV2SecureChannelProvider : IDisposable
{
    private readonly IWechatPayMerchantContext _merchantContext;
    private readonly IWechatPayV2CredentialProvider _credentials;
    private readonly ConcurrentDictionary<string, Entry> _channels = new(StringComparer.Ordinal);

    /// <summary>创建通道持有者。</summary>
    /// <param name="merchantContext">环境商户上下文。</param>
    /// <param name="credentials">APIv2 凭据取用端口。</param>
    public WechatPayV2SecureChannelProvider(
        IWechatPayMerchantContext merchantContext, IWechatPayV2CredentialProvider credentials)
    {
        _merchantContext = merchantContext ?? throw new ArgumentNullException(nameof(merchantContext));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    /// <summary>取当前商户在指定域名上的安全通道（首次访问时装配证书，之后复用）。</summary>
    /// <param name="baseAddress">目标域名（只接受 <see cref="WechatPayV2HttpClientNames"/> 里的四个常量）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>该商户在该域名上的专属通道。</returns>
    /// <exception cref="ArgumentException"><paramref name="baseAddress"/> 不是本线白名单域名。</exception>
    public async Task<WechatPayV2SecureChannel> GetChannelAsync(
        string baseAddress, CancellationToken cancellationToken)
    {
        // 域名白名单在此收口：通道不经 IHttpClientFactory，组件的 UrlValidator 不会替我们把门，
        // 一旦将来有调用方把路径拼成绝对 URL，就会开出一条绕过 SSRF 白名单的通道。
        // 四个常量都是编译期字面量 ⇒ 闭集，商户配置不参与选路。
        if (baseAddress != WechatPayV2HttpClientNames.BaseAddress
            && baseAddress != WechatPayV2HttpClientNames.SecureBaseAddress
            && baseAddress != WechatPayV2HttpClientNames.GlobalBaseAddress
            && baseAddress != WechatPayV2HttpClientNames.FraudBaseAddress)
        {
            throw new ArgumentException(
                $"APIv2 安全通道只允许挂在 {WechatPayV2HttpClientNames.BaseAddress}、" +
                $"{WechatPayV2HttpClientNames.SecureBaseAddress}、" +
                $"{WechatPayV2HttpClientNames.GlobalBaseAddress} 与 " +
                $"{WechatPayV2HttpClientNames.FraudBaseAddress} 四个官方域名上（收到 {baseAddress}）。",
                nameof(baseAddress));
        }

        var merchant = _merchantContext.ResolveCurrent();
        var cacheKey = merchant.MerchantKey + "|" + baseAddress;

        if (_channels.TryGetValue(cacheKey, out var existing))
        {
            return existing.Channel;
        }

        var certificate = await _credentials
            .LoadClientCertificateAsync(merchant, cancellationToken)
            .ConfigureAwait(false);

        var entry = new Entry(new WechatPayV2SecureChannel(merchant.MerchantKey, baseAddress, certificate));

        // 并发下可能有两个线程各自装配：以先到者为准，后来者立即释放多余通道（证书与 HttpClient 都是Disposable）。
        var stored = _channels.GetOrAdd(cacheKey, entry);
        if (!ReferenceEquals(stored, entry))
        {
            entry.Dispose();
        }

        return stored.Channel;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var entry in _channels.Values)
        {
            entry.Dispose();
        }

        _channels.Clear();
    }

    private sealed class Entry : IDisposable
    {
        public Entry(WechatPayV2SecureChannel channel) => Channel = channel;

        public WechatPayV2SecureChannel Channel { get; }

        public void Dispose() => Channel.Dispose();
    }
}

/// <summary>
/// 单个商户的 APIv2 双向证书通道：一个绑定了该商户 <c>apiclient_cert.p12</c> 的 <see cref="HttpClient"/>。
/// </summary>
internal sealed class WechatPayV2SecureChannel : IDisposable
{
    private readonly X509Certificate2 _certificate;

    /// <summary>建通道（证书所有权转入本通道，随通道一并释放）。</summary>
    /// <param name="merchantKey">商户键（仅用于诊断文本）。</param>
    /// <param name="baseAddress">该通道的域名（由 Provider 校验过白名单）。</param>
    /// <param name="certificate">商户 API 证书（须含私钥）。</param>
    public WechatPayV2SecureChannel(
        string merchantKey, string baseAddress, X509Certificate2 certificate)
    {
        MerchantKey = merchantKey;
        BaseAddress = baseAddress;
        _certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));

        var handler = new HttpClientHandler
        {
            // 官方要求 TLS1.2 起；系统默认即可满足，此处不主动降级。
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12
                           | System.Security.Authentication.SslProtocols.Tls13,
        };

        // ClientCertificates 在 .NET Core 上是**只读集合**（非 .NET Framework 的可赋值属性）⇒ 只能 Add。
        handler.ClientCertificates.Add(certificate);

        HttpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseAddress),
            Timeout = TimeSpan.FromSeconds(30),
        };
    }

    /// <summary>商户键（诊断用）。</summary>
    public string MerchantKey { get; }

    /// <summary>该通道绑定的官方域名（诊断用）。</summary>
    public string BaseAddress { get; }

    /// <summary>绑定客户端证书的 HTTP 客户端。</summary>
    public HttpClient HttpClient { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        HttpClient.Dispose();
        _certificate.Dispose();
    }
}
