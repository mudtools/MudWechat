// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Transport;
using Mud.Wechat.Pay.Tests.ContractGuards;
using Mud.Wechat.Pay.Tests.Credential;
using System.Net;
using System.Net.Http;

namespace Mud.Wechat.Pay.Tests.Transport;

/// <summary>
/// 支付传输层的 <b>真实 DI 管道</b>测试（P1-a 基石）。
/// </summary>
/// <remarks>
/// <para>
/// 与 <see cref="WechatPayAuthorizationHandlerTests"/> 的分工：那边<b>手工</b> new Handler + HttpClient，
/// 只证明「Handler 自身算得对」；这边走 <c>AddPayApp</c> 注册 → <c>IHttpClientFactory</c> 取命名客户端，
/// 证明「Handler 真的挂进了管道、BaseAddress 真的生效、跨产品线真的没串味」。
/// 手工装配一旦与 DI 装配脱节（漏挂 / 挂错客户端名），只有这组用例能抓到 ——
/// 而那种缺陷会在**第一笔真实交易**才暴露。
/// </para>
/// <para>
/// 真实网络一律由<b>覆盖主 Handler</b>换成桩（宿主侧后注册者胜出），本组只验管道、不碰外网。
/// </para>
/// </remarks>
public class WechatPayTransportPipelineTests
{
    private const string MerchantKeyId = "pay:merchant:1900000000:key";
    private const string ApiKeyId = "pay:merchant:1900000000:apikey";

    /// <summary>
    /// 端到端：从 <c>AddPayApp</c> 装配出的支付客户端发出的请求，
    /// 必须带上<b>可独立复算</b>的 <c>Authorization</c> 头，且真的打到官方主域名。
    /// </summary>
    [Fact]
    public async Task PayClient_ShouldSignAndTargetOfficialHost_WhenResolvedFromDiPipeline()
    {
        var capture = new CapturingHandler();

        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());

            // 宿主侧覆盖主 Handler（后注册者胜出）：把真实网络换成本桩，只留管道。
            services.AddHttpClient(WechatPayHttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => capture);
        });

        var client = provider.GetRequiredService<IWechatPayHttpClient>();

        using var request = new HttpRequestMessage(HttpMethod.Post, "/v3/pay/transactions/jsapi")
        {
            Content = new StringContent(
                """{"mchid":"1900000000"}""", Encoding.UTF8, "application/json"),
        };

        using var response = await client.SendRawAsync(request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var header = capture.Request.Headers.GetValues("Authorization").Single();

        header.Should().StartWith(
            "WECHATPAY2-SHA256-RSA2048 mchid=\"1900000000\",nonce_str=\"",
            "because 签名头的 schema 与字段顺序必须与官方抓包一致");

        header.Should().Contain(
            "serial_no=\"" + WechatPayGoldenVectors.MerchantSerialNumber + "\"",
            "because serial_no 必须来自商户配置（DI 侧取的是真凭据，不是测试桩）");

        // —— 独立复算：按官方规范重建签名串，用证书私钥验签 ——
        // 任何一处错位（规范 URL、查询串问号、请求体原文、头组装）都会让这里验签失败。
        using var verifyKey = RSA.Create();
        verifyKey.ImportFromPem(WechatPayGoldenVectors.MerchantPrivateKeyPem);

        var message = string.Concat(
            "POST", "\n",
            "/v3/pay/transactions/jsapi", "\n",
            Extract(header, "timestamp"), "\n",
            Extract(header, "nonce_str"), "\n",
            """{"mchid":"1900000000"}""", "\n");

        verifyKey.VerifyData(
                Encoding.UTF8.GetBytes(message),
                Convert.FromBase64String(Extract(header, "signature")),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1)
            .Should().BeTrue(
                "because 签名必须真出自 DI 装配出的商户私钥，否则说明管道里挂的根本不是支付 Handler");

        // BaseAddress 必须由命名客户端注入：相对 URI 发出后必须是官方主域名的绝对地址。
        capture.Request.RequestUri.Should().NotBeNull();
        capture.Request.RequestUri.IsAbsoluteUri.Should().BeTrue(
            "because 相对 URI 只有在命名客户端配了 BaseAddress 时才会变成绝对地址");
        capture.Request.RequestUri.Scheme.Should().Be("https");
        capture.Request.RequestUri.Host.Should().Be("api.mch.weixin.qq.com");
    }

    /// <summary>
    /// <see cref="IWechatPayHttpClient"/> 必须是<b>跨 scope 单例</b>（AGENTS §7），
    /// 且其 <c>BaseAddress</c> 指向支付主域名 —— 与企微线的 <c>qyapi.weixin.qq.com</c> 分属两套客户端。
    /// </summary>
    [Fact]
    public void PayClient_ShouldBeSingletonWithPayBaseAddress_WhenResolvedAcrossScopes()
    {
        using var provider = BuildProvider(
            services => services.AddPayApp(CreateSingleMerchant()));

        IWechatPayHttpClient Resolve(IServiceProvider sp) =>
            sp.GetRequiredService<IWechatPayHttpClient>();

        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        var fromA = Resolve(scopeA.ServiceProvider);
        var fromB = Resolve(scopeB.ServiceProvider);

        fromA.Should().BeSameAs(fromB, "because 支付客户端是 Singleton，散装实例会让拦截器与连接池各走各的");
        fromA.BaseAddress.Should().Be(new Uri(WechatPayHttpClientNames.BaseAddress + "/"),
            "because 支付线必须自己占一个 BaseAddress，与企微线互不干扰");
    }

    /// <summary>
    /// 反向断言：**别的**命名客户端不得沾上支付签名头。
    /// 支付线自占客户端类型与客户端名的全部意义就在这里 ——
    /// 一旦 Handler 挂成全局，企微的 <c>qyapi</c> 请求会被套上商户 <c>Authorization</c>。
    /// </summary>
    [Fact]
    public async Task UnrelatedClient_ShouldNotReceivePayAuthorization_WhenDifferentClientName()
    {
        var payCapture = new CapturingHandler();
        var otherCapture = new CapturingHandler();

        using var provider = BuildProvider(services =>
        {
            services.AddPayApp(CreateSingleMerchant());

            services.AddHttpClient(WechatPayHttpClientNames.ClientName)
                .ConfigurePrimaryHttpMessageHandler(() => payCapture);
            services.AddHttpClient("unrelated")
                .ConfigurePrimaryHttpMessageHandler(() => otherCapture);
        });

        var factory = provider.GetRequiredService<IHttpClientFactory>();

        using (var other = factory.CreateClient("unrelated"))
        using (var request = new HttpRequestMessage(HttpMethod.Get, "https://qyapi.weixin.qq.com/cgi-bin/get"))
        {
            using var response = await other.SendAsync(request);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        otherCapture.Request.Headers.Contains("Authorization").Should().BeFalse(
            "because 支付签名 Handler 只能挂在支付命名客户端上，泄漏到其它客户端即跨线污染");

        // 同一容器里支付客户端仍然正常签名 —— 证明上面的「没签名」不是因为整条链路坏了。
        using (var pay = factory.CreateClient(WechatPayHttpClientNames.ClientName))
        using (var request = new HttpRequestMessage(HttpMethod.Get, "/v3/certificates"))
        {
            using var response = await pay.SendAsync(request);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        payCapture.Request.Headers.Contains("Authorization").Should().BeTrue(
            "because 对照组必须签名，否则「未签名」可能只是管道没通");
    }

    /// <summary>
    /// 纯支付宿主（<b>不</b>引用企微/公众号线）必须自己把 SSRF 白名单登记上。
    /// </summary>
    /// <remarks>
    /// 白名单唯一登记点是公用层 <c>AddWechatTokenRecovery</c>，而它只被 <c>AddWechatApp</c>/<c>AddMpApp</c> 调用；
    /// 支付线不走令牌恢复 ⇒ 若 <c>AddPayApp</c> 不登记，纯支付宿主的**每一笔请求**都会被
    /// 组件的连接期 SSRF 严格模式拦下（<c>域名 ... 未通过验证</c>）。
    /// 本用例是该缺陷的回归锁。
    /// </remarks>
    [Fact]
    public void AddPayApp_ShouldRegisterSsrfAllowlist_WhenHostUsesPayLineOnly()
    {
        // 先清成「未登记」形态，模拟纯支付宿主的初始状态（全局静态，需自行还原）。
        // 先拷贝：GetAllowedDomains 返回的是内部视图，ConfigureAllowedDomains 会整体替换该集合。
        var backup = UrlValidator.GetAllowedDomains().ToArray();
        try
        {
            UrlValidator.ConfigureAllowedDomains(Array.Empty<string>());

            var services = new ServiceCollection();
            services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
                new Dictionary<string, string> { [MerchantKeyId] = "x", [ApiKeyId] = "x" }));
            services.AddPayApp(CreateSingleMerchant());

            UrlValidator.GetAllowedDomains().Should().Contain("weixin.qq.com",
                "因为 weixin.qq.com 后缀覆盖 api.mch.weixin.qq.com；未登记即纯支付宿主全线被拦");

            UrlValidator.GetAllowedDomains().Should().NotContain("api.mch.weixin.qq.com",
                "because 支付域名必须由 weixin.qq.com 后缀覆盖，不得新增独立条目（PAY-B8 / AB-G9）");
        }
        finally
        {
            UrlValidator.ConfigureAllowedDomains(backup);
        }
    }

    /// <summary>辅助：单商户（= 默认商户），密钥真放 <c>ISecretProvider</c>，走完整取用链。</summary>
    private static List<WechatPayMerchantConfig> CreateSingleMerchant() => new()
    {
        new()
        {
            MchId = "1900000000",
            SerialNumber = WechatPayGoldenVectors.MerchantSerialNumber,
            PrivateKeySecretName = MerchantKeyId,
            ApiKeySecretName = ApiKeyId,
        },
    };

    /// <summary>辅助：从 Authorization 头取单个字段值。</summary>
    private static string Extract(string header, string field)
    {
        var match = Regex.Match(header, field + "=\"(?<v>[^\"]*)\"");
        match.Success.Should().BeTrue(
            "Authorization 头必须含 " + field + "，实际为：" + header);
        return match.Groups["v"].Value;
    }

    /// <summary>辅助：构建根容器（开 scope 校验；不开 ValidateOnBuild，避免急切解析干扰用例意图）。</summary>
    private static ServiceProvider BuildProvider(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISecretProvider>(new InMemorySecretProvider(
            new Dictionary<string, string>
            {
                // 真 PEM：签名链路要真的能解析出 RSA，占位串会在取密钥时就炸。
                [MerchantKeyId] = WechatPayGoldenVectors.MerchantPrivateKeyPem,
                [ApiKeyId] = new string('k', 32),
            }));

        configure(services);

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false,
        });
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
}
