// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Transport;
using System.Net;
using System.Net.Http;

namespace Mud.Wechat.Pay.Tests.Transport;

/// <summary>
/// <see cref="WechatPayAuthorizationHandler"/> 端到端测试（P1-a 基石：真实把 <c>Authorization</c> 头签出来）。
/// </summary>
/// <remarks>
/// 核心用例是「**独立复算**」：从产出的头里取出时间戳/随机串/签名，按官方规范重建签名串，
/// 再用公钥验签 —— 若规范 URL、查询串问号、请求体原文或头组装任何一处错位，这里就会验签失败。
/// </remarks>
public class WechatPayAuthorizationHandlerTests
{
    /// <summary>
    /// 端到端：签名头可用<b>公钥独立复算</b>验证，且字段顺序、随机串长度、商户号取值均照官方。
    /// </summary>
    [Fact]
    public async Task SendAsync_ShouldProduceIndependentlyVerifiableAuthorization_WhenPostingJsonBody()
    {
        using var privateKey = RSA.Create(2048);
        var merchant = CreateOrdinary("1900000000");
        var captured = new CapturingHandler();

        using var request = new HttpRequestMessage(
            HttpMethod.Post, "/v3/pay/transactions/jsapi?out_trade_no=NO1&amount=1")
        {
            Content = new StringContent("""{"mchid":"1900000000"}""", Encoding.UTF8, "application/json"),
        };

        using var response = await SendAsync(request, merchant, privateKey, captured);

        response.IsSuccessStatusCode.Should().BeTrue();

        var header = captured.Request.Headers.GetValues("Authorization").Single();

        // schema + 字段顺序照官方原文：mchid → nonce_str → timestamp → serial_no → signature
        header.Should().StartWith("WECHATPAY2-SHA256-RSA2048 mchid=\"1900000000\",nonce_str=\"",
            "because Authorization 头的 schema 与字段顺序必须与官方抓包一致");

        var parsed = ParseAuthorization(header);

        parsed.MchId.Should().Be("1900000000");
        parsed.SerialNo.Should().Be(merchant.SerialNumber);
        parsed.Nonce.Should().HaveLength(32, "官方随机串为 32 位");
        parsed.Nonce.Should().MatchRegex("^[a-zA-Z0-9]{32}$");
        parsed.Timestamp.Should().MatchRegex("^[0-9]{10}$", "秒级 Unix 时间戳");

        // —— 独立复算：按官方规范重建签名串并用公钥验签 ——
        var canonicalUrl = "/v3/pay/transactions/jsapi?out_trade_no=NO1&amount=1";
        var expectedMessage = string.Concat(
            "POST", "\n",
            canonicalUrl, "\n",
            parsed.Timestamp, "\n",
            parsed.Nonce, "\n",
            """{"mchid":"1900000000"}""", "\n");

        var verified = privateKey.VerifyData(
            Encoding.UTF8.GetBytes(expectedMessage),
            Convert.FromBase64String(parsed.Signature),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        verified.Should().BeTrue(
            "because 规范 URL（含查询串）、请求体原文与头组装必须逐字节对齐，否则线上验签必失败");
    }

    /// <summary>
    /// 查询串问号只允许出现一次：<c>Uri.Query</c> 自带 <c>?</c>，而 <c>BuildCanonicalUrl</c> 会再补一个。
    /// 多一个问号会让全部带查询参数的请求线上验签失败。
    /// </summary>
    [Fact]
    public async Task SendAsync_ShouldNotDuplicateQuestionMark_WhenRequestHasQueryString()
    {
        using var privateKey = RSA.Create(2048);
        var merchant = CreateOrdinary("1900000000");
        var captured = new CapturingHandler();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/pay/transactions/out-trade-no/NO1?serial=S1");

        using var response = await SendAsync(request, merchant, privateKey, captured);

        var parsed = ParseAuthorization(captured.Request.Headers.GetValues("Authorization").Single());

        var message = string.Concat(
            "GET", "\n",
            "/v3/pay/transactions/out-trade-no/NO1?serial=S1", "\n",
            parsed.Timestamp, "\n", parsed.Nonce, "\n", "\n");

        privateKey.VerifyData(
                Encoding.UTF8.GetBytes(message),
                Convert.FromBase64String(parsed.Signature),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1)
            .Should().BeTrue("because 无请求体时签名串以空体收尾，查询串不得出现 '??'");
    }

    /// <summary>已带 <c>Authorization</c> 的请求必须原样放行（重签会换时间戳/随机串，与已取证请求不一致）。</summary>
    [Fact]
    public async Task SendAsync_ShouldLeaveExistingAuthorization_WhenHeaderAlreadyPresent()
    {
        using var privateKey = RSA.Create(2048);
        var merchant = CreateOrdinary("1900000000");
        var captured = new CapturingHandler();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/certificates");
        request.Headers.TryAddWithoutValidation("Authorization", "WECHATPAY2-SHA256-RSA2048 mchid=\"preset\"");

        using var response = await SendAsync(request, merchant, privateKey, captured);

        captured.Request.Headers.GetValues("Authorization").Single()
            .Should().Be("WECHATPAY2-SHA256-RSA2048 mchid=\"preset\"", "because 幂等：已签名请求不得重签");
    }

    /// <summary>多商户宿主：作用域内请求必须用<b>该商户</b>的私钥与序列号签名。</summary>
    [Fact]
    public async Task SendAsync_ShouldUseScopedMerchant_WhenMultipleMerchantsRegistered()
    {
        using var firstKey = RSA.Create(2048);
        using var secondKey = RSA.Create(2048);

        var manager = new WechatPayMerchantManager(new[]
        {
            WithSerial(CreateOrdinary("1900000000"), "SERIAL-A"),
            WithSerial(CreateOrdinary("1900000001"), "SERIAL-B"),
        });

        var context = new WechatPayMerchantContext(manager);
        var credentials = new StubCredentialProvider(
            new Dictionary<string, RSA> { ["1900000000"] = firstKey, ["1900000001"] = secondKey });
        var factory = new WechatPaySignatureProviderFactory(
            credentials, new WechatPayPlatformCertificateCache());

        var captured = new CapturingHandler();
        var handler = new WechatPayAuthorizationHandler(context, factory) { InnerHandler = captured };
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://api.mch.weixin.qq.com") };

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/certificates");

        using (context.UseMerchant("1900000001"))
        {
            using var response = await client.SendAsync(request);
        }

        var parsed = ParseAuthorization(captured.Request.Headers.GetValues("Authorization").Single());

        parsed.MchId.Should().Be("1900000001");
        parsed.SerialNo.Should().Be("SERIAL-B", "because 作用域商户决定用哪套证书");

        // 用第二把公钥验证签名归属（第一把验不过 ⇒ 商户选择真的生效了）
        var message = string.Concat(
            "GET", "\n", "/v3/certificates", "\n",
            parsed.Timestamp, "\n", parsed.Nonce, "\n", "\n");

        secondKey.VerifyData(
                Encoding.UTF8.GetBytes(message),
                Convert.FromBase64String(parsed.Signature),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1)
            .Should().BeTrue("because 作用域商户的私钥必须真的参与签名");

        firstKey.VerifyData(
                Encoding.UTF8.GetBytes(message),
                Convert.FromBase64String(parsed.Signature),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1)
            .Should().BeFalse("because 换错私钥的签名必须验不过，证明选择并非偶然命中");
    }

    /// <summary>服务商形态：<c>Authorization.mchid</c> 取 <c>sp_mchid</c>（子商户无 API 证书，不参与签名）。</summary>
    [Fact]
    public async Task SendAsync_ShouldUseSpMchId_WhenMerchantIsServicePartner()
    {
        using var privateKey = RSA.Create(2048);
        var merchant = new WechatPayMerchantConfig
        {
            SpMchId = "1900000002",
            SubMchId = "1600000001",
            SerialNumber = "SERIAL-SP",
            PrivateKeySecretName = "pay:sp:1900000002:key",
            ApiKeySecretName = "pay:sp:1900000002:apikey",
        };
        var captured = new CapturingHandler();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/v3/certificates");

        using var response = await SendAsync(request, merchant, privateKey, captured);

        var parsed = ParseAuthorization(captured.Request.Headers.GetValues("Authorization").Single());

        parsed.MchId.Should().Be("1900000002",
            "because 签名用的是服务商自己的 API 证书，子商户号只出现在请求体");
    }

    /// <summary>辅助：跑一次发送并捕获最终请求。</summary>
    private static async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        WechatPayMerchantConfig merchant,
        RSA privateKey,
        CapturingHandler captured)
    {
        var context = new WechatPayMerchantContext(
            new WechatPayMerchantManager(new[] { merchant }));

        var factory = new WechatPaySignatureProviderFactory(
            new StubCredentialProvider(new Dictionary<string, RSA> { [merchant.MerchantKey] = privateKey }),
            new WechatPayPlatformCertificateCache());

        var handler = new WechatPayAuthorizationHandler(context, factory) { InnerHandler = captured };

        using var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.mch.weixin.qq.com"),
        };

        return await client.SendAsync(request);
    }

    private static WechatPayMerchantConfig WithSerial(WechatPayMerchantConfig config, string serial)
    {
        config.SerialNumber = serial;
        return config;
    }

    private static WechatPayMerchantConfig CreateOrdinary(string mchId) => new()
    {
        MchId = mchId,
        SerialNumber = "SERIAL-" + mchId,
        PrivateKeySecretName = "pay:merchant:" + mchId + ":key",
        ApiKeySecretName = "pay:merchant:" + mchId + ":apikey",
    };

    /// <summary>解析 Authorization 头（字段顺序照官方原文）。</summary>
    private static AuthorizationParts ParseAuthorization(string header)
    {
        var match = Regex.Match(
            header,
            "^WECHATPAY2-SHA256-RSA2048 mchid=\"(?<mch>[^\"]*)\",nonce_str=\"(?<nonce>[^\"]*)\"," +
            "timestamp=\"(?<ts>[^\"]*)\",serial_no=\"(?<serial>[^\"]*)\",signature=\"(?<sig>[^\"]*)\"$");

        match.Success.Should().BeTrue("Authorization 头必须与官方 schema 逐字段一致，实际为：" + header);

        return new AuthorizationParts(
            match.Groups["mch"].Value,
            match.Groups["nonce"].Value,
            match.Groups["ts"].Value,
            match.Groups["serial"].Value,
            match.Groups["sig"].Value);
    }

    /// <summary>Authorization 头的五个字段。</summary>
    private sealed class AuthorizationParts
    {
        public AuthorizationParts(string mchId, string nonce, string timestamp, string serialNo, string signature)
        {
            MchId = mchId;
            Nonce = nonce;
            Timestamp = timestamp;
            SerialNo = serialNo;
            Signature = signature;
        }

        public string MchId { get; }
        public string Nonce { get; }
        public string Timestamp { get; }
        public string SerialNo { get; }
        public string Signature { get; }
    }

    /// <summary>只负责记录最终发出的请求，随后回 200。</summary>
    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage Request { get; private set; } = null!;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}"),
            });
        }
    }

    /// <summary>凭据取用器桩：按商户号返回预置的 RSA 私钥，不打真实密钥仓库。</summary>
    private sealed class StubCredentialProvider : IWechatPayMerchantCredentialProvider
    {
        private readonly Dictionary<string, RSA> _keysByMerchantKey;

        public StubCredentialProvider(Dictionary<string, RSA> keysByMerchantKey)
        {
            _keysByMerchantKey = keysByMerchantKey;
        }

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
}
