// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.Abstractions.Exceptions;

namespace Mud.Wechat.Pay.Abstractions.Transport;

/// <summary>
/// APIv3 应答验签 Handler：对<b>命中范围</b>的应答用平台证书公钥验签，失败即拒（fail-closed）。
/// </summary>
/// <remarks>
/// <para>
/// <b>验签串与回调同形态</b>（<see cref="WechatPaySignatureMessages.BuildVerifyMessage"/> =
/// <c>时间戳\n随机串\n报文体\n</c>），且报文体取<b>原始应答字符串</b> —— 官方 FAQ 明确的第一大失败原因
/// 就是「反序列化后再重新序列化」导致字节漂移。
/// </para>
/// <para>
/// <b>只验「2xx 且 Content-Type 含 json」的应答</b>，这是本 Handler 的核心作用域，两条排除都是<b>有意为之</b>：
/// </para>
/// <list type="number">
/// <item><b>非 2xx 原样放行</b>：SDK 既有契约是「错误体由调用方按 <c>code</c>/<c>message</c> 判错」
/// （接口带 <c>[AllowAnyStatusCode]</c>），本 Handler <b>不得</b>改变该契约，否则调用方会在验签异常处
/// 拿到一个与官方错误体无关的失败，丢失错误码分类。</item>
/// <item><b>非 JSON 原样放行</b>：文件/二进制/账单/媒体下载通道走同一个 pay client 的
/// <c>SendRawAsync</c>，而官方<b>对下载应答不签名</b> —— 若对这些应答 fail-closed，会直接打断账单下发
/// 与媒体通道。故排除范围限定在 Content-Type 含 <c>json</c> 的应答。</item>
/// </list>
/// <para>
/// <b>可选依赖</b>：<see cref="IWechatPayPlatformCertificateRefresher"/> 只在「验签失败」时用于
/// 「未知序列号的自愈」（与回调接收器同款语义：<b>只</b>补一次、失败即拒）。它只在主包的 <c>AddPayApi</c>
/// 注册，纯 <c>AddPayApp</c> 宿主可能没有 ⇒ 经 <see cref="IServiceProvider.GetService(Type)"/> 显式取
/// 可选依赖（未注册时为 <c>null</c>，此时未知序列号直接拒绝，仍是 fail-closed，只是少了轮换期自愈）。
/// 该端口经 <see cref="Lazy{T}"/> <b>延迟</b>解析：默认刷新端口反向依赖同一个支付命名客户端，
/// 构造期立即解析会造成客户端构建回环（详见构造函数注释）。
/// </para>
/// <para>
/// <b>fail-closed 红线</b>：任何验签失败都收敛为异常，<b>不</b>吞异常放行；<see cref="OperationCanceledException"/>
/// 由框架语义原样上抛。
/// </para>
/// <para>
/// <b>PAY-B7</b>：日志只记商户键、平台证书序列号与失败分类；<c>Wechatpay-Signature</c>、签名值、
/// 应答体原文一律不写入日志或异常消息。
/// </para>
/// </remarks>
internal sealed class WechatPayResponseVerificationHandler : DelegatingHandler
{
    private const string TimestampHeaderName = "Wechatpay-Timestamp";
    private const string NonceHeaderName = "Wechatpay-Nonce";
    private const string SignatureHeaderName = "Wechatpay-Signature";
    private const string SerialHeaderName = "Wechatpay-Serial";

    private readonly IWechatPayMerchantContext _merchantContext;
    private readonly IWechatPaySignatureProviderFactory _signatureProviders;
    private readonly Lazy<IWechatPayPlatformCertificateRefresher?> _refresher;
    private readonly ILogger<WechatPayResponseVerificationHandler>? _logger;

    /// <summary>
    /// 创建 Handler（由 DI 解析，随命名客户端的 Handler 管道每次发送时执行）。
    /// </summary>
    /// <param name="merchantContext">环境商户上下文（验签需按当前商户取平台证书）。</param>
    /// <param name="signatureProviders">按商户缓存的签名提供者工厂（验签入口）。</param>
    /// <param name="serviceProvider">
    /// 服务提供器：用于解析<b>可选</b>依赖（刷新端口与日志器），
    /// 避免对纯 <c>AddPayApp</c> 宿主强制要求注册 <see cref="IWechatPayPlatformCertificateRefresher"/>。
    /// </param>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c>。</exception>
    public WechatPayResponseVerificationHandler(
        IWechatPayMerchantContext merchantContext,
        IWechatPaySignatureProviderFactory signatureProviders,
        IServiceProvider serviceProvider)
    {
        _merchantContext = merchantContext ?? throw new ArgumentNullException(nameof(merchantContext));
        _signatureProviders = signatureProviders ?? throw new ArgumentNullException(nameof(signatureProviders));

        if (serviceProvider is null)
        {
            throw new ArgumentNullException(nameof(serviceProvider));
        }

        // 显式 GetService：见类注释「可选依赖」。不得用「可选构造参数默认值」走 DI 默认值解析 ——
        // 那会把「未注册」与「注册为 null」混同，且对 ServiceProvider 的默认值解析行为是有意规避的。
        //
        // 刷新端口**延迟**解析（本类由 AddHttpMessageHandler<T> 在构建支付命名客户端管道时解析，
        // 而默认刷新端口经 IWechatPayCertificatesService 反向依赖**同一个**支付命名客户端）：
        // 若在此构造期立即 GetService，会形成「构建支付客户端 → 解析本 Handler → 解析刷新端口 →
        // 解析证书服务 → 再构建同一支付客户端」的循环，DefaultHttpClientFactory 的 Lazy 会抛
        // 「ValueFactory attempted to access the Value property of this instance」。
        // 首次请求时管道已就绪，延迟解析不再回环；未注册时照旧为 null（未知序列号仍 fail-closed 拒绝）。
        _refresher = new Lazy<IWechatPayPlatformCertificateRefresher?>(
            serviceProvider.GetService<IWechatPayPlatformCertificateRefresher>);

        _logger = serviceProvider.GetService<ILogger<WechatPayResponseVerificationHandler>>();
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        // 作用域过滤①：非 2xx 原样放行（见类注释「非 2xx 原样放行」）。
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        // 作用域过滤②：非 JSON 原样放行（见类注释「非 JSON 原样放行」——
        // 官方对文件/账单/媒体下载应答不签名，对其 fail-closed 会打断账单下发与媒体通道）。
        var mediaType = response.Content?.Headers.ContentType?.MediaType;
        if (mediaType is null || !mediaType.Contains("json", StringComparison.OrdinalIgnoreCase))
        {
            return response;
        }

        var merchant = _merchantContext.ResolveCurrent();

        var timestamp = GetHeader(response, TimestampHeaderName);
        var nonce = GetHeader(response, NonceHeaderName);
        var signature = GetHeader(response, SignatureHeaderName);
        var serial = GetHeader(response, SerialHeaderName);

        // 命中范围内的应答必须四头齐全：缺任一即畸形 ⇒ fail-closed（缺失绝不许回落到「跳过校验」）。
        if (timestamp is null || nonce is null || signature is null || serial is null)
        {
            _logger?.LogWarning(
                "微信支付应答验签头缺失：失败分类={FailureKind}，商户={MerchantKey}，平台证书序列号={SerialNumber}。",
                "MissingHeaders",
                merchant.MerchantKey,
                serial);

            throw new WechatPayException(
                null,
                "应答缺少 Wechatpay 验签头（Wechatpay-Timestamp / Wechatpay-Nonce / Wechatpay-Signature / Wechatpay-Serial），拒绝接收。",
                request.RequestUri?.ToString());
        }

        // 验签串取应答体**原文**：先入缓冲区（保证下游仍能读同一份内容），再取字符串。
        // 若先反序列化再序列化会改变字节序列并导致线上验签全量失败（官方 FAQ 第一大失败原因）。
        if (response.Content != null)
        {
            await response.Content.LoadIntoBufferAsync().ConfigureAwait(false);
        }

        var body = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var provider = await _signatureProviders
            .GetOrCreateAsync(merchant, cancellationToken)
            .ConfigureAwait(false);

        var verifyMessage = WechatPaySignatureMessages.BuildVerifyMessage(timestamp, nonce, body);

        if (!provider.Verify(serial, verifyMessage, signature))
        {
            // 「未知序列号可自愈」路径（与回调接收器同款语义）：只补一次、失败即拒。
            // 刷新端口对「已命中本地存储」的序列号直接短路，故**签名不匹配绝不会触发下载**
            // （否则任意伪造应答都能驱动一次证书仓库往返 = 放大型 DoS）。
            if (_refresher.Value is null
                || !await _refresher.Value.TryRefreshAsync(merchant, serial, cancellationToken).ConfigureAwait(false)
                || !provider.Verify(serial, verifyMessage, signature))
            {
                _logger?.LogWarning(
                    "微信支付应答验签未通过：失败分类={FailureKind}，商户={MerchantKey}，平台证书序列号={SerialNumber}。",
                    "InvalidSignature",
                    merchant.MerchantKey,
                    serial);

                // 不区分「序列号未知」与「签名与报文体不匹配」对外提示，避免成为探测 oracle。
                throw new WechatPayException(
                    null,
                    "应答验签未通过（序列号未知或签名与报文体不匹配）。",
                    request.RequestUri?.ToString());
            }
        }

        return response;
    }

    /// <summary>取单个应答头首值；缺失（无该头或无值）返回 <c>null</c>。</summary>
    private static string? GetHeader(HttpResponseMessage response, string name)
        => response.Headers.TryGetValues(name, out var values) ? values.FirstOrDefault() : null;
}
