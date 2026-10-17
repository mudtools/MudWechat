// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Transport;
using System.Net;
using System.Net.Http.Headers;

namespace Mud.Wechat.Pay.Tests.Transport;

/// <summary>
/// <see cref="WechatPayResponseVerificationHandler"/> 端到端测试：手工 new Handler + HttpClient，
/// 用<b>真实自签证书</b>把应答签名逐字节压过管道。
/// </summary>
/// <remarks>
/// <para>
/// 与 <see cref="WechatPayTransportPipelineTests"/> 的分工：那边走 DI 证明「Handler 真的挂进了管道」，
/// 这边手装证明「Handler 自身的验签与作用域过滤算得对」。
/// </para>
/// <para>
/// 核心锁死两处设计落点：① 非 2xx / 非 JSON 应答<b>原样放行</b>（不得触碰错误体契约与下载通道）；
/// ② 命中范围内的应答四头缺失 / 验签失败<b>一律 fail-closed 抛异常</b>（不得吞异常放行）。
/// </para>
/// </remarks>
public class WechatPayResponseVerificationHandlerTests
{
    private const string MerchantId = "1900000000";
    private const string PlatformSerial = "SERIAL-PLATFORM-1";

    /// <summary>合法签名（serial 与头一致）⇒ 放行且响应体可被下游完整读取。</summary>
    [Fact]
    public async Task SendAsync_ShouldAllow_WhenSignatureValid()
    {
        using var platformKey = RSA.Create(2048);
        using var certificate = CreatePlatformCertificate(platformKey);
        using var store = new WechatPayPlatformCertificateCache();
        store.Set(PlatformSerial, CloneCertificate(certificate));
        using var merchantKey = RSA.Create(2048);

        const string body = """{"code":"SUCCESS","message":"OK"}""";
        var signature = Sign(platformKey, "1728000000", "NONCE123", body);

        using var client = CreateClient(
            store, merchantKey, refresher: null,
            BuildJsonResponse(HttpStatusCode.OK, body, "1728000000", "NONCE123", signature, PlatformSerial));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/NO1");
        using var response = await client.SendAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue();

        // 验签读的应答体必须先缓冲再读出，保证下游还能读到同一份内容。
        (await response.Content.ReadAsStringAsync()).Should().Be(body,
            "because 验签读体后下游仍须能读到同一份原始应答体");
    }

    /// <summary>应答体被篡改（签名对不上）⇒ fail-closed 抛异常。</summary>
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenBodyTampered()
    {
        using var platformKey = RSA.Create(2048);
        using var certificate = CreatePlatformCertificate(platformKey);
        using var store = new WechatPayPlatformCertificateCache();
        store.Set(PlatformSerial, CloneCertificate(certificate));
        using var merchantKey = RSA.Create(2048);

        // 签名覆盖的是原文，但应答体被换成了另一份 ⇒ 验签必失败。
        var signature = Sign(platformKey, "1728000000", "NONCE123", """{"code":"SUCCESS"}""");

        using var client = CreateClient(
            store, merchantKey, refresher: null,
            BuildJsonResponse(HttpStatusCode.OK, """{"code":"TAMPERED"}""", "1728000000", "NONCE123", signature, PlatformSerial));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/NO1");

        var act = async () => await client.SendAsync(request);

        await act.Should().ThrowAsync<WechatPayException>(
            "because 应答体字节与签名不符时必须 fail-closed 拒绝");
    }

    /// <summary>命中范围（2xx JSON）缺任一 <c>Wechatpay-*</c> 验签头 ⇒ fail-closed 抛异常。</summary>
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenVerificationHeaderMissing()
    {
        using var platformKey = RSA.Create(2048);
        using var store = new WechatPayPlatformCertificateCache();
        using var merchantKey = RSA.Create(2048);

        // 缺 Wechatpay-Signature（其余三头齐全）。
        using var client = CreateClient(
            store, merchantKey, refresher: null,
            BuildJsonResponse(HttpStatusCode.OK, """{"code":"SUCCESS"}""", "1728000000", "NONCE123", signature: null, PlatformSerial));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/NO1");

        var act = async () => await client.SendAsync(request);

        await act.Should().ThrowAsync<WechatPayException>(
            "because 命中范围内的应答四头齐全性是前提，缺任一即畸形，不得回落到跳过校验");
    }

    /// <summary>未知 serial 且<b>无</b>刷新端口 ⇒ fail-closed 抛异常（不得「无刷新即放行」）。</summary>
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenSerialUnknownAndRefresherAbsent()
    {
        using var platformKey = RSA.Create(2048);
        using var certificate = CreatePlatformCertificate(platformKey);
        using var store = new WechatPayPlatformCertificateCache();
        // 刻意不把 serial 放进 store ⇒ 未知序列号。
        using var merchantKey = RSA.Create(2048);

        const string body = """{"code":"SUCCESS"}""";
        var signature = Sign(platformKey, "1728000000", "NONCE123", body);

        using var client = CreateClient(
            store, merchantKey, refresher: null,
            BuildJsonResponse(HttpStatusCode.OK, body, "1728000000", "NONCE123", signature, PlatformSerial));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/NO1");

        var act = async () => await client.SendAsync(request);

        await act.Should().ThrowAsync<WechatPayException>(
            "because 未知序列号且无自愈端口时只能拒绝，绝不因「刷新不可用」而放行");
    }

    /// <summary>未知 serial 但刷新端口注入正确证书后复验通过 ⇒ 放行（一次自愈机会）。</summary>
    [Fact]
    public async Task SendAsync_ShouldAllow_WhenRefresherProvidesCertificateAndReverifyPasses()
    {
        using var platformKey = RSA.Create(2048);
        using var certificate = CreatePlatformCertificate(platformKey);
        using var store = new WechatPayPlatformCertificateCache();
        using var merchantKey = RSA.Create(2048);

        const string body = """{"code":"SUCCESS"}""";
        var signature = Sign(platformKey, "1728000000", "NONCE123", body);

        // 桩刷新器：把正确证书注入 cache 再返回 true —— 模拟官方轮换期「新序列号先出现」的场景。
        var refresher = new StubRefresher(store, CloneCertificate(certificate));

        using var client = CreateClient(
            store, merchantKey, refresher,
            BuildJsonResponse(HttpStatusCode.OK, body, "1728000000", "NONCE123", signature, PlatformSerial));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/NO1");
        using var response = await client.SendAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue(
            "because 刷新补齐证书后复验通过，应放行（一次自愈机会）");
        refresher.CallCount.Should().Be(1, "because 只允许补一次、失败即拒，不得重试到成功");
    }

    /// <summary>刷新端口返回 false（补齐失败）⇒ 仍 fail-closed 抛异常（不得「刷新失败即放行」）。</summary>
    [Fact]
    public async Task SendAsync_ShouldThrow_WhenRefresherReturnsFalse()
    {
        using var platformKey = RSA.Create(2048);
        using var certificate = CreatePlatformCertificate(platformKey);
        using var store = new WechatPayPlatformCertificateCache();
        using var merchantKey = RSA.Create(2048);

        const string body = """{"code":"SUCCESS"}""";
        var signature = Sign(platformKey, "1728000000", "NONCE123", body);

        var refresher = new StubRefresher(store, CloneCertificate(certificate), result: false);

        using var client = CreateClient(
            store, merchantKey, refresher,
            BuildJsonResponse(HttpStatusCode.OK, body, "1728000000", "NONCE123", signature, PlatformSerial));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/NO1");

        var act = async () => await client.SendAsync(request);

        await act.Should().ThrowAsync<WechatPayException>(
            "because 刷新返回 false 只表示「证书没补齐」，绝不放宽验签结论");
    }

    /// <summary>非 2xx ⇒ 不验签、原样返回（保留 SDK「错误体由调用方按 code/message 判错」的契约）。</summary>
    [Fact]
    public async Task SendAsync_ShouldPassThroughWithoutVerifying_WhenStatusNonSuccess()
    {
        using var store = new WechatPayPlatformCertificateCache();
        using var merchantKey = RSA.Create(2048);

        // 即使错误体是 JSON 且毫无验签头，也必须原样放行。
        using var client = CreateClient(
            store, merchantKey, refresher: null,
            BuildJsonResponse(HttpStatusCode.BadRequest, """{"code":"PARAM_ERROR","message":"参数错误"}""",
                timestamp: null, nonce: null, signature: null, serial: null));

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v3/pay/transactions/jsapi");
        using var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest,
            "because 非 2xx 错误体契约由调用方判错，Handler 不得改变该契约");
        (await response.Content.ReadAsStringAsync()).Should().Contain("PARAM_ERROR");
    }

    /// <summary>非 JSON 2xx ⇒ 不验签、原样返回（账单/媒体下载通道官方不签名，不得打断）。</summary>
    [Fact]
    public async Task SendAsync_ShouldPassThroughWithoutVerifying_WhenContentIsNotJson()
    {
        using var store = new WechatPayPlatformCertificateCache();
        using var merchantKey = RSA.Create(2048);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(new byte[] { 0x50, 0x4B, 0x03, 0x04 }),
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        using var client = CreateClient(store, merchantKey, refresher: null, response);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/bill/tradebill");
        using var sent = await client.SendAsync(request);

        sent.IsSuccessStatusCode.Should().BeTrue(
            "because 非 JSON（文件/账单/媒体下载）通道官方不签名，对其 fail-closed 会直接打断下发");
        (await sent.Content.ReadAsByteArrayAsync()).Should().Equal(new byte[] { 0x50, 0x4B, 0x03, 0x04 });
    }

    /// <summary>PAY-B7：验签失败抛出的异常消息里<b>不得</b>出现签名值（也不得出现应答体原文）。</summary>
    [Fact]
    public async Task SendAsync_ShouldNotLeakSignature_WhenVerificationFails()
    {
        using var platformKey = RSA.Create(2048);
        using var certificate = CreatePlatformCertificate(platformKey);
        using var store = new WechatPayPlatformCertificateCache();
        store.Set(PlatformSerial, CloneCertificate(certificate));
        using var merchantKey = RSA.Create(2048);

        const string originalBody = """{"code":"SUCCESS"}""";
        var signature = Sign(platformKey, "1728000000", "NONCE123", originalBody);
        const string tamperedBody = """{"code":"TAMPERED-SECRET"}""";

        using var client = CreateClient(
            store, merchantKey, refresher: null,
            BuildJsonResponse(HttpStatusCode.OK, tamperedBody, "1728000000", "NONCE123", signature, PlatformSerial));

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/NO1");

        var act = async () => await client.SendAsync(request);

        var exception = (await act.Should().ThrowAsync<WechatPayException>()).Which;

        exception.Message.Should().NotContain(signature,
            "because PAY-B7 红线：Wechatpay-Signature 与签名值不得进入异常消息");
        exception.Message.Should().NotContain("TAMPERED-SECRET",
            "because PAY-B7 红线：应答体原文不得进入异常消息");
    }

    /// <summary>辅助：手工装配 Handler + HttpClient，内层桩返回预置应答。</summary>
    private static HttpClient CreateClient(
        WechatPayPlatformCertificateCache store,
        RSA merchantPrivateKey,
        IWechatPayPlatformCertificateRefresher? refresher,
        HttpResponseMessage response)
    {
        var merchant = new WechatPayMerchantConfig
        {
            MchId = MerchantId,
            SerialNumber = "SERIAL-MERCHANT",
            PrivateKeySecretName = "pay:merchant:1900000000:key",
            ApiKeySecretName = "pay:merchant:1900000000:apikey",
        };

        var context = new WechatPayMerchantContext(new WechatPayMerchantManager(new[] { merchant }));
        var factory = new WechatPaySignatureProviderFactory(
            new StubCredentialProvider(new Dictionary<string, RSA> { [merchant.MerchantKey] = merchantPrivateKey }),
            store);

        var services = new ServiceCollection();
        if (refresher is not null)
        {
            services.AddSingleton(refresher);
        }

        var handler = new WechatPayResponseVerificationHandler(
            context, factory, services.BuildServiceProvider())
        {
            InnerHandler = new FixedResponseHandler(response),
        };

        return new HttpClient(handler) { BaseAddress = new Uri("https://api.mch.weixin.qq.com") };
    }

    /// <summary>辅助：构造带预置状态码、应答体与 <c>Wechatpay-*</c> 头的应答（头为 null 时省略该头）。</summary>
    private static HttpResponseMessage BuildJsonResponse(
        HttpStatusCode statusCode,
        string body,
        string? timestamp,
        string? nonce,
        string? signature,
        string? serial)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };

        AddHeader(response, "Wechatpay-Timestamp", timestamp);
        AddHeader(response, "Wechatpay-Nonce", nonce);
        AddHeader(response, "Wechatpay-Signature", signature);
        AddHeader(response, "Wechatpay-Serial", serial);

        return response;
    }

    private static void AddHeader(HttpResponseMessage response, string name, string? value)
    {
        if (value is not null)
        {
            response.Headers.TryAddWithoutValidation(name, value);
        }
    }

    /// <summary>辅助：按官方验签串格式对报文体原文签名（RSA-SHA256 + Pkcs1）。</summary>
    private static string Sign(RSA privateKey, string timestamp, string nonce, string body)
    {
        var message = WechatPaySignatureMessages.BuildVerifyMessage(timestamp, nonce, body);
        var signature = privateKey.SignData(
            Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signature);
    }

    /// <summary>辅助：构造自签平台证书（<b>私钥随 <paramref name="platformKey"/> 独立持有</b>）。</summary>
    private static X509Certificate2 CreatePlatformCertificate(RSA platformKey)
    {
        var request = new CertificateRequest(
            "CN=MudWechatPayResponseVerifyTest",
            platformKey,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
    }

    /// <summary>辅助：复制证书本体（缓存接管所有权后仍可安全独立释放）。</summary>
    private static X509Certificate2 CloneCertificate(X509Certificate2 certificate)
#pragma warning disable SYSLIB0057
        => new(certificate.RawData);
#pragma warning restore SYSLIB0057

    /// <summary>内层桩：返回预置应答（每次同源，用于单次发送用例）。</summary>
    private sealed class FixedResponseHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FixedResponseHandler(HttpResponseMessage response) => _response = response;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    /// <summary>凭据取用器桩：按商户键返回预置 RSA 私钥，不打真实密钥仓库。</summary>
    private sealed class StubCredentialProvider : IWechatPayMerchantCredentialProvider
    {
        private readonly Dictionary<string, RSA> _keysByMerchantKey;

        public StubCredentialProvider(Dictionary<string, RSA> keysByMerchantKey) => _keysByMerchantKey = keysByMerchantKey;

        public Task<RSA> LoadPrivateKeyAsync(WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
        {
            if (!_keysByMerchantKey.TryGetValue(config.MerchantKey, out var key))
            {
                throw new InvalidOperationException("桩未配置商户 " + config.MerchantKey);
            }

            return Task.FromResult(key);
        }

        public Task<byte[]> LoadApiKeyAsync(WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
            => Task.FromResult(new byte[32]);
    }

    /// <summary>刷新端口桩：可注入证书并返回指定结论，统计调用次数。</summary>
    private sealed class StubRefresher : IWechatPayPlatformCertificateRefresher
    {
        private readonly WechatPayPlatformCertificateCache _store;
        private readonly X509Certificate2 _certificate;
        private readonly bool _result;
        private int _callCount;

        public StubRefresher(WechatPayPlatformCertificateCache store, X509Certificate2 certificate, bool result = true)
        {
            _store = store;
            _certificate = certificate;
            _result = result;
        }

        public int CallCount => _callCount;

        public Task<bool> TryRefreshAsync(
            WechatPayMerchantConfig merchant, string serialNumber, CancellationToken cancellationToken = default)
        {
            _callCount++;

            if (_result)
            {
                _store.Set(serialNumber, _certificate);
            }

            return Task.FromResult(_result);
        }
    }
}
