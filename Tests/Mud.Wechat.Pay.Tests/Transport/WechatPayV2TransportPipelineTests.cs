// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Mud.Wechat.Pay.Tests.ContractGuards;
using Mud.Wechat.Pay.Tests.Credential;
using Mud.Wechat.Pay.V2.Extensions;
using Mud.Wechat.Pay.V2.Transport;

namespace Mud.Wechat.Pay.Tests.Transport;

/// <summary>
/// 微信支付 APIv2 传输层的 <b>真实 DI 管道</b>测试。
/// </summary>
/// <remarks>
/// <para>
/// 与 <see cref="WechatPayTransportPipelineTests"/> 的分工完全对称，只是载体换成 XML：
/// 那边证明 APIv3 的 <c>Authorization</c> 头出自真私钥，这边走 <c>AddPayApp</c> +
/// <c>AddWechatPayV2Api</c> 装配出的命名客户端，证明<b>签名 Handler 真挂在管道上、
/// 报文里的 <c>sign</c> 能被独立复算</b>。复算一律在本文件内按官方规范重做一遍
/// （排序 → 拼串 → MD5/HMAC-SHA256），<b>不复用生产实现</b>——
/// 否则「实现写错 + 断言跟着错」会一起绿。
/// </para>
/// <para>
/// 真实网络由<b>覆盖主 Handler</b>换成桩（宿主侧后注册者胜出），本组只验管道、不碰外网。
/// </para>
/// </remarks>
public class WechatPayV2TransportPipelineTests
{
    private const string MerchantKeyId = "pay:merchant:1900000000:key";
    private const string ApiKeyId = "pay:merchant:1900000000:apikey";
    private const string ApiV2KeyId = "pay:merchant:1900000000:apiv2key";

    /// <summary>APIv2 签名密钥（官方固定 32 字节；异常消息与报文都不带它）。</summary>
    private static readonly string ApiV2Key = new string('k', 32);

    /// <summary>
    /// 出站报文体必须被管道追加<b>可独立复算</b>的 MD5 <c>sign</c>，且打到官方 <c>api.mch</c> 域名。
    /// </summary>
    [Fact]
    public async Task V2Client_ShouldAppendRecomputableMd5Sign_WhenResolvedFromDiPipeline()
    {
        var capture = new CapturingHandler("<xml><return_code>SUCCESS</return_code></xml>");

        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());

            services.AddHttpClient(WechatPayV2HttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => capture);
        });

        var client = provider.GetRequiredService<IWechatPayV2HttpClient>();

        const string Body = "<xml><appid>wx1234567890abcdef</appid><mch_id>1900000000</mch_id>" +
                            "<nonce_str>ABCDEF1234567890</nonce_str><out_trade_no>T202610180001</out_trade_no>" +
                            "<total_fee>1</total_fee><body>测试商品</body></xml>";

        using var request = new HttpRequestMessage(HttpMethod.Post, "/pay/unifiedorder")
        {
            Content = new StringContent(Body, Encoding.UTF8, "text/xml"),
        };

        using var response = await client.SendRawAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var fields = ReadFields(await capture.Request.Content!.ReadAsStringAsync());

        var sign = fields.Single(f => f.Key == "sign").Value;

        // —— 独立复算：官方 stringA = ASCII 升序、排除 sign 与空值，串尾固定 &key=<密钥> ——
        var expected = Md5Hex(Canonical(fields) + "key=" + ApiV2Key);

        sign.Should().Be(expected,
            "because sign 必须由 DI 管道里的签名 Handler 用商户 APIv2 密钥真算出来；不等即说明挂的根本不是这条密钥或排序口径漂移");

        // 原字段必须一个不少：Handler 重写报文体的动作只能「追加 sign」，不能丢字段。
        fields.Should().Contain(f => f.Key == "out_trade_no" && f.Value == "T202610180001");
        fields.Should().Contain(f => f.Key == "body" && f.Value == "测试商品");

        capture.Request.RequestUri!.Host.Should().Be("api.mch.weixin.qq.com",
            "because APIv2 普通端点与 APIv3 同域，但走的是独立命名客户端（不同 Handler 管道）");
        capture.Request.RequestUri.Scheme.Should().Be("https");

        // APIv2 的凭据在报文体内，不在头里 —— 挂了 Authorization 即两条线串味。
        capture.Request.Headers.Contains("Authorization").Should().BeFalse(
            "because APIv2 没有 Authorization 头这一契约，出现即 APIv3 Handler 泄漏进本线管道");
    }

    /// <summary>
    /// <c>sign_type=HMAC-SHA256</c> 时算法必须随报文切换（官方两把算法共用同一密钥）。
    /// </summary>
    [Fact]
    public async Task V2Client_ShouldSwitchToHmacSha256_WhenBodyDeclaresSignType()
    {
        var capture = new CapturingHandler("<xml><return_code>SUCCESS</return_code></xml>");

        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());

            services.AddHttpClient(WechatPayV2HttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => capture);
        });

        var client = provider.GetRequiredService<IWechatPayV2HttpClient>();

        const string Body = "<xml><appid>wx1234567890abcdef</appid><mch_id>1900000000</mch_id>" +
                            "<nonce_str>ABCDEF1234567890</nonce_str><sign_type>HMAC-SHA256</sign_type>" +
                            "<out_trade_no>T202610180002</out_trade_no></xml>";

        using var request = new HttpRequestMessage(HttpMethod.Post, "/pay/orderquery")
        {
            Content = new StringContent(Body, Encoding.UTF8, "text/xml"),
        };

        using var response = await client.SendRawAsync(request);
        response.IsSuccessStatusCode.Should().BeTrue();

        var fields = ReadFields(await capture.Request.Content!.ReadAsStringAsync());
        var sign = fields.Single(f => f.Key == "sign").Value;
        var stringA = Canonical(fields) + "key=" + ApiV2Key;

        sign.Should().Be(HmacSha256Hex(stringA),
            "because 报文声明 sign_type=HMAC-SHA256 时官方按 HMAC 判定，MD5 串必然对不上");
        sign.Should().NotBe(Md5Hex(stringA), "because 默认算法是 MD5，命中即说明 sign_type 被忽略");
    }

    /// <summary>
    /// 已带 <c>sign</c> 的报文<b>不得重签</b>（幂等放行）。
    /// </summary>
    /// <remarks>
    /// 重签的诱惑来自「重试时字段可能被补全」，但一旦字段表口径与首发有差，产出的就是
    /// 「体与 sign 互相矛盾」的自我否定报文 —— 官方只会回 SIGNERROR，且比漏签更难查。
    /// </remarks>
    [Fact]
    public async Task V2Client_ShouldNotResign_WhenBodyAlreadyCarriesSign()
    {
        var capture = new CapturingHandler("<xml><return_code>SUCCESS</return_code></xml>");

        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());

            services.AddHttpClient(WechatPayV2HttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => capture);
        });

        var client = provider.GetRequiredService<IWechatPayV2HttpClient>();

        const string Body = "<xml><appid>wx1234567890abcdef</appid><mch_id>1900000000</mch_id>" +
                            "<nonce_str>ABCDEF1234567890</nonce_str><sign>PLACEHOLDER-NOT-REGENERATED</sign></xml>";

        using var request = new HttpRequestMessage(HttpMethod.Post, "/pay/closeorder")
        {
            Content = new StringContent(Body, Encoding.UTF8, "text/xml"),
        };

        using var response = await client.SendRawAsync(request);
        response.IsSuccessStatusCode.Should().BeTrue();

        var fields = ReadFields(await capture.Request.Content!.ReadAsStringAsync());

        fields.Count(f => f.Key == "sign").Should().Be(1, "because 出现两个 sign 元素会让官方按首个或末个解析，行为不可控");
        fields.Single(f => f.Key == "sign").Value.Should().Be("PLACEHOLDER-NOT-REGENERATED");
    }

    /// <summary>
    /// <b>GET 端点</b>（APIv2 全线只有 <c>h5entrustweb</c> 两支）：签名必须落在<b>查询串</b>上，
    /// 且以待签名的是<b>解码后</b>原值 —— 官方按参数原文排序拼串，拿编码态参与签名必然 SIGNERROR。
    /// </summary>
    [Fact]
    public async Task V2Client_ShouldAppendSignToQuery_WhenRequestHasNoBody()
    {
        var capture = new CapturingHandler(
            "<xml><return_code>SUCCESS</return_code><result_code>SUCCESS</result_code></xml>");

        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());

            services.AddHttpClient(WechatPayV2HttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => capture);
        });

        var client = provider.GetRequiredService<IWechatPayV2HttpClient>();

        // notify_url 带 : / ? & = 等保留字符：正是「编码态 vs 原文」的分界点。
        // 注：contract_display_account 刻意用 ASCII —— 组件层 ValidateRequestAsync 以 Uri.ToString()
        // （显示形态，会把 %XX 还原成字符）判定 well-formed，含非 ASCII 的相对 URL 会在进入本线
        // Handler 之前就被拒，属上游组件行为，不在本线可实现绕开的范围内。
        var query = "appid=wx1234567890abcdef&mch_id=1900000000&plan_id=1" +
                    "&contract_code=C202610180001&request_serial=1" +
                    "&contract_display_account=" + Uri.EscapeDataString("zhangsan@example") +
                    "&notify_url=" + Uri.EscapeDataString("https://example.com/pap/notify?src=h5&tag=1") +
                    "&timestamp=1760000000&clientip=1.2.3.4&version=1.0&sign_type=HMAC-SHA256";

        using var request = new HttpRequestMessage(HttpMethod.Get, "/papay/h5entrustweb?" + query);

        using var response = await client.SendRawAsync(request);
        response.IsSuccessStatusCode.Should().BeTrue();

        var fields = ReadQuery(capture.Request.RequestUri!.Query);

        // 只追加 sign，不得丢参数、不得改写值。
        fields.Should().Contain(f => f.Key == "contract_display_account" && f.Value == "zhangsan@example");
        fields.Should().Contain(f => f.Key == "notify_url"
                                     && f.Value == "https://example.com/pap/notify?src=h5&tag=1");
        fields.Should().ContainSingle(f => f.Key == "sign",
            "because 二次追加会让查询串出现两个 sign，官方取哪个不确定");

        var sign = fields.Single(f => f.Key == "sign").Value;

        // —— 独立复算：以解码后的原值为字段表，sign_type=HMAC-SHA256 ⇒ HMAC ——
        sign.Should().Be(HmacSha256Hex(Canonical(fields) + "key=" + ApiV2Key),
            "because GET 的签名口径与 POST 完全同源，只是载体换成查询串");
        sign.Should().NotBe(Md5Hex(Canonical(fields) + "key=" + ApiV2Key),
            "because 报文里显式带了 sign_type=HMAC-SHA256，命中 MD5 即 Query 分支没读到 sign_type");

        capture.Request.Content.Should().BeNull(
            "because GET 无报文体；若 Handler 误走 body 分支，签名会被写进一个不存在的 XML 信封");
    }

    /// <summary>已带 <c>sign</c> 的查询串不得重签（与体侧同一幂等口径）。</summary>
    [Fact]
    public async Task V2Client_ShouldNotResignQuery_WhenQueryAlreadyCarriesSign()
    {
        var capture = new CapturingHandler(
            "<xml><return_code>SUCCESS</return_code></xml>");

        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());

            services.AddHttpClient(WechatPayV2HttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => capture);
        });

        var client = provider.GetRequiredService<IWechatPayV2HttpClient>();

        using var request = new HttpRequestMessage(
            HttpMethod.Get, "/papay/h5entrustweb?appid=wx1&sign=PLACEHOLDER-NOT-REGENERATED");

        using var response = await client.SendRawAsync(request);
        response.IsSuccessStatusCode.Should().BeTrue();

        var fields = ReadQuery(capture.Request.RequestUri!.Query);
        fields.Single(f => f.Key == "sign").Value.Should().Be("PLACEHOLDER-NOT-REGENERATED");
    }

    /// <summary>
    /// 应答侧验签是<b>双向</b>闸：签名正确的应答放行，签名对不上的应答必须 fail-closed 抛错。
    /// </summary>
    /// <remarks>
    /// 官方应答的 <c>sign</c> 是本 SDK 判定「这份回包真的来自微信支付、没被中间层改写」的唯一凭据。
    /// 放过伪造应答即等于把 <c>SUCCESS</c> 编造权交给链路上的任何人。
    /// </remarks>
    [Fact]
    public async Task V2Client_ShouldVerifyResponseSign_WhenResponseCarriesSign()
    {
        var fields = new List<KeyValuePair<string, string?>>
        {
            new("return_code", "SUCCESS"),
            new("result_code", "SUCCESS"),
            new("appid", "wx1234567890abcdef"),
            new("mch_id", "1900000000"),
            new("nonce_str", "RESPONSENONCE0001"),
            new("transaction_id", "4200000000202610180000000001"),
        };

        var signed = new List<KeyValuePair<string, string?>>(fields)
        {
            new("sign", Md5Hex(Canonical(fields) + "key=" + ApiV2Key)),
        };

        var goodCapture = new CapturingHandler(WriteXml(signed));
        var tamperedCapture = new CapturingHandler(
            WriteXml(fields.Select(f => new KeyValuePair<string, string?>(f.Key, f.Value))
                .Append(new KeyValuePair<string, string?>("sign", "BADSIGN")).ToList()));

        using var goodProvider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());
            services.AddHttpClient(WechatPayV2HttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => goodCapture);
        });
        using var tamperedProvider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());
            services.AddHttpClient(WechatPayV2HttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => tamperedCapture);
        });

        async Task<HttpResponseMessage> Send(IServiceProvider provider, string path)
        {
            var client = provider.GetRequiredService<IWechatPayV2HttpClient>();
            using var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(
                    "<xml><appid>wx1234567890abcdef</appid><mch_id>1900000000</mch_id></xml>",
                    Encoding.UTF8, "text/xml"),
            };
            return await client.SendRawAsync(request);
        }

        using (var ok = await Send(goodProvider, "/pay/orderquery"))
        {
            ok.IsSuccessStatusCode.Should().BeTrue(
                "because 应答 sign 与本地按同一密钥重算一致时必须放行，否则每笔正常交易都会被自己拒掉");
        }

        var tampered = await Assert.ThrowsAsync<InvalidOperationException>(
            () => Send(tamperedProvider, "/pay/orderquery"));

        tampered.Message.Should().Contain("应答验签失败");
        tampered.Message.Should().NotContain(ApiV2Key, "because 异常消息严禁出现密钥原文（PAY-B7 同线红线）");
    }

    /// <summary>
    /// APIv2 客户端必须是<b>跨 scope 单例</b>，且 BaseAddress 落在 APIv2 自己的域名常量上。
    /// </summary>
    [Fact]
    public void V2Client_ShouldBeSingletonWithV2BaseAddress_WhenResolvedAcrossScopes()
    {
        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());
        });

        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        var fromA = scopeA.ServiceProvider.GetRequiredService<IWechatPayV2HttpClient>();
        var fromB = scopeB.ServiceProvider.GetRequiredService<IWechatPayV2HttpClient>();

        fromA.Should().BeSameAs(fromB, "because 客户端是 Singleton，散装实例会让签名 Handler 与连接池各走各的");
        fromA.BaseAddress.Should().Be(new Uri(WechatPayV2HttpClientNames.BaseAddress + "/"));

        // 证书通道服务同样在 AddAllApis() 里随域注册（5 支证书端点的唯一入口）。
        scopeA.ServiceProvider.GetRequiredService<IWechatPayV2SecureService>()
            .Should().BeSameAs(scopeB.ServiceProvider.GetRequiredService<IWechatPayV2SecureService>());
    }

    /// <summary>
    /// 底座顺序：<b>未先</b> <c>AddPayApp</c> 就装配 APIv2 必须注册期 fail-fast，而不是留到首笔交易。
    /// </summary>
    [Fact]
    public void AddWechatPayV2Api_ShouldFailFast_WhenPayAppNotRegistered()
    {
        var services = new ServiceCollection();

        var error = Assert.Throws<InvalidOperationException>(
            () => services.AddWechatPayV2Api(static builder => builder.AddAllApis()));

        error.Message.Should().Contain("AddPayApp");
        error.Message.Should().Contain("IWechatPayMerchantManager");
    }

    /// <summary>
    /// 未配 <c>ApiV2CertificateSecretName</c> 的商户调用证书端点必须<b>点名缺哪个配置项</b>，
    /// 且不得先发起网络请求、更不得把密钥名之外的材料写进消息。
    /// </summary>
    [Fact]
    public async Task SecureService_ShouldFailFastOnMissingCertificate_WhenMerchantHasNoCertSecretName()
    {
        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());
            services.AddWechatPayV2Api(static builder => builder.AddAllApis());
        });

        var secure = provider.GetRequiredService<IWechatPayV2SecureService>();

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => secure.RefundAsync(
            new V2RefundRequest
            {
                AppId = "wx1234567890abcdef",
                MchId = "1900000000",
                OutTradeNo = "T202610180003",
                OutRefundNo = "R202610180003",
                TotalFee = 1,
                RefundFee = 1,
                NonceStr = "ABCDEF1234567890",
            }));

        error.Message.Should().Contain(nameof(WechatPayMerchantConfig.ApiV2CertificateSecretName),
            "because 证书端点的半数故障是「只配了 APIv2 密钥没配证书」，消息必须点名缺的配置项");
        error.Message.Should().NotContain(ApiV2Key);
    }

    /// <summary>辅助：单商户配置（APIv2 密钥名 + APIv3 三件套，Validate 要求后者齐备）。</summary>
    private static List<WechatPayMerchantConfig> CreateSingleMerchant() => new()
    {
        new()
        {
            MchId = "1900000000",
            SerialNumber = WechatPayGoldenVectors.MerchantSerialNumber,
            PrivateKeySecretName = MerchantKeyId,
            ApiKeySecretName = ApiKeyId,
            ApiV2KeySecretName = ApiV2KeyId,
        },
    };

    /// <summary>解析 XML 报文体为扁平字段表（与生产实现无关，独立口径）。</summary>
    private static List<KeyValuePair<string, string?>> ReadFields(string xml)
        => XDocument.Parse(xml).Root!
            .Elements()
            .Select(e => new KeyValuePair<string, string?>(e.Name.LocalName, e.Value))
            .ToList();

    /// <summary>解析查询串为扁平字段表：以<b>解码后</b>的原值为口径（与生产实现无关，独立复算）。</summary>
    private static List<KeyValuePair<string, string?>> ReadQuery(string query)
        => (query ?? string.Empty).TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(part =>
            {
                var index = part.IndexOf('=');
                return index < 0
                    ? new KeyValuePair<string, string?>(Uri.UnescapeDataString(part), null)
                    : new KeyValuePair<string, string?>(
                        Uri.UnescapeDataString(part.Substring(0, index)),
                        Uri.UnescapeDataString(part.Substring(index + 1)));
            })
            .ToList();

    /// <summary>官方待签名串：ASCII 升序、排除 <c>sign</c> 与空值，每节末尾带 <c>&amp;</c>。</summary>
    private static string Canonical(IEnumerable<KeyValuePair<string, string?>> fields)
    {
        var builder = new StringBuilder();
        foreach (var field in fields
                     .Where(f => !string.Equals(f.Key, "sign", StringComparison.OrdinalIgnoreCase))
                     .Where(f => !string.IsNullOrEmpty(f.Value))
                     .OrderBy(f => f.Key, StringComparer.Ordinal))
        {
            builder.Append(field.Key).Append('=').Append(field.Value).Append('&');
        }

        return builder.ToString();
    }

    private static string WriteXml(IEnumerable<KeyValuePair<string, string?>> fields)
    {
        var root = new XElement("xml", fields.Select(f => new XElement(f.Key, f.Value ?? string.Empty)));
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private static string Md5Hex(string value)
        => Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(value)));

    private static string HmacSha256Hex(string value)
        => Convert.ToHexString(HMACSHA256.HashData(Encoding.UTF8.GetBytes(ApiV2Key), Encoding.UTF8.GetBytes(value)));

    /// <summary>辅助：构建根容器（密钥值只进 <c>ISecretProvider</c>，配置面只放密钥名）。</summary>
    private static ServiceProvider BuildProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
            new Dictionary<string, string>
            {
                [MerchantKeyId] = "placeholder",
                [ApiKeyId] = new string('a', 32),
                [ApiV2KeyId] = ApiV2Key,
            }));

        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });
    }

    /// <summary>桩主 Handler：记录最终发出的请求，并回一份可控 XML 应答。</summary>
    private sealed class CapturingHandler : HttpMessageHandler
    {
        private readonly string _responseXml;

        public CapturingHandler(string responseXml) => _responseXml = responseXml;

        public HttpRequestMessage Request { get; private set; } = null!;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseXml, Encoding.UTF8, "text/xml"),
            });
        }
    }
}
