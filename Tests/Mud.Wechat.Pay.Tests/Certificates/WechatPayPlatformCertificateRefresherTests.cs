// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Mud.Wechat.Pay.Abstractions.Configuration;
using Mud.Wechat.Pay.Abstractions.Credential;
using Mud.Wechat.Pay.Certificates;
using Mud.Wechat.Pay.DataModels.Certificates;

namespace Mud.Wechat.Pay.Tests.Certificates;

/// <summary>
/// 平台证书按需刷新器用例（设计方案 §2.6 闸①「未知 serial ⇒ 触发一次刷新后仍失败即拒」）。
/// </summary>
/// <remarks>
/// <para>
/// 本组用<b>真实 AES-256-GCM</b>加密真实自签证书的 PEM（官方线格式：nonce 为 12 个 ASCII 字符、
/// ciphertext 为 <c>Base64(密文‖tag)</c>）—— 解密口径一旦错位（例如漏切 tag 或按 Base64 解 nonce），
/// 这里会 100% 失败，而不是「看起来过了」。
/// </para>
/// <para>
/// 另三条用例锁死<b>防放大</b>性质：已命中不下载、间隔内不重复下载、并发单飞 —— 这三条是
/// 「未知序列号可被任意伪造」这一前提下的必要闸门。
/// </para>
/// </remarks>
public class WechatPayPlatformCertificateRefresherTests
{
    private static readonly byte[] ApiKey = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");

    /// <summary>未知序列号 ⇒ 拉取、解密、入库，并返回「已就绪」。</summary>
    [Fact]
    public async Task TryRefreshAsync_ShouldDecryptAndStore_WhenSerialUnknown()
    {
        using var certificate = CreateSelfSignedCertificate();
        var store = new WechatPayPlatformCertificateCache();
        var client = new FakeCertificatesClient(CreateItem(certificate, ApiKey));

        var refresher = new WechatPayPlatformCertificateRefresher(
            client, new FakeCredentialProvider(ApiKey), store, store);

        var refreshed = await refresher.TryRefreshAsync(CreateMerchant(), certificate.SerialNumber);

        refreshed.Should().BeTrue("刷新后目标序列号必须可用");
        store.TryGetCertificate(certificate.SerialNumber, out var stored).Should().BeTrue();
        stored!.Subject.Should().Contain("MudWechatPayRefreshTest");
        client.CallCount.Should().Be(1);
    }

    /// <summary>已命中 ⇒ 幂等短路，<b>零网络往返</b>。</summary>
    [Fact]
    public async Task TryRefreshAsync_ShouldNotDownload_WhenSerialAlreadyKnown()
    {
        using var certificate = CreateSelfSignedCertificate();
        var store = new WechatPayPlatformCertificateCache();
        store.Set(certificate.SerialNumber, X509CertificateLoaderShim(certificate));

        var client = new FakeCertificatesClient(CreateItem(certificate, ApiKey));
        var refresher = new WechatPayPlatformCertificateRefresher(
            client, new FakeCredentialProvider(ApiKey), store, store);

        var refreshed = await refresher.TryRefreshAsync(CreateMerchant(), certificate.SerialNumber);

        refreshed.Should().BeTrue();
        client.CallCount.Should().Be(0, "证书已就绪时不得产生下载 —— 这是「只能按需刷新」的第一道防放大闸");
    }

    /// <summary>宿主只提供只读证书存储（无写端口）⇒ 返回 false 且<b>不抛</b>（fail-closed）。</summary>
    [Fact]
    public async Task TryRefreshAsync_ShouldReturnFalse_WhenWriterMissing()
    {
        using var certificate = CreateSelfSignedCertificate();
        var client = new FakeCertificatesClient(CreateItem(certificate, ApiKey));

        var refresher = new WechatPayPlatformCertificateRefresher(
            client, new FakeCredentialProvider(ApiKey), new ReadOnlyCertificateStore());

        var refreshed = await refresher.TryRefreshAsync(CreateMerchant(), certificate.SerialNumber);

        refreshed.Should().BeFalse("无处写入时不得声称刷新成功（否则会掩盖验签持续失败）");
        client.CallCount.Should().Be(0, "没有写端口时连下载都不必发起");
    }

    /// <summary>最小刷新间隔内的重复请求 ⇒ 不再下载（防未知序列号风暴）。</summary>
    [Fact]
    public async Task TryRefreshAsync_ShouldNotDownloadAgain_WithinMinInterval()
    {
        using var certificate = CreateSelfSignedCertificate();

        // 列表里放一本**别的**证书 ⇒ 目标序列号刷新后仍未知（模拟「攻击者伪造任意 serial」）。
        using var other = CreateSelfSignedCertificate("CN=MudWechatPayOther");
        var store = new WechatPayPlatformCertificateCache();
        var client = new FakeCertificatesClient(CreateItem(other, ApiKey));

        var refresher = new WechatPayPlatformCertificateRefresher(
            client, new FakeCredentialProvider(ApiKey), store, store);
        var merchant = CreateMerchant();

        (await refresher.TryRefreshAsync(merchant, certificate.SerialNumber)).Should().BeFalse();
        (await refresher.TryRefreshAsync(merchant, certificate.SerialNumber)).Should().BeFalse();

        client.CallCount.Should().Be(1,
            $"最小刷新间隔（{WechatPayPlatformCertificateRefresher.MinRefreshIntervalSeconds}s）内不得重复下载");
    }

    /// <summary>并发调用同一未知序列号 ⇒ <b>单飞</b>：只下载一次，且每个调用者都得到「已就绪」。</summary>
    [Fact]
    public async Task TryRefreshAsync_ShouldShareSingleDownload_WhenCalledConcurrently()
    {
        using var certificate = CreateSelfSignedCertificate();
        var store = new WechatPayPlatformCertificateCache();
        var client = new FakeCertificatesClient(CreateItem(certificate, ApiKey));

        var refresher = new WechatPayPlatformCertificateRefresher(
            client, new FakeCredentialProvider(ApiKey), store, store);
        var merchant = CreateMerchant();

        var results = await Task.WhenAll(Enumerable.Range(0, 8)
            .Select(_ => refresher.TryRefreshAsync(merchant, certificate.SerialNumber)));

        results.Should().AllSatisfy(r => r.Should().BeTrue("等待中的调用者应在同批刷新完成后复查到证书"));
        client.CallCount.Should().Be(1, "同商户并发刷新必须单飞，否则并发风暴会被放大成 N 倍下载");
    }

    /// <summary>证书端点为业务失败（抛异常）⇒ 返回 false（fail-closed），不把异常漏给调用方。</summary>
    [Fact]
    public async Task TryRefreshAsync_ShouldReturnFalse_WhenCertificatesClientThrows()
    {
        var client = new FakeCertificatesClient(Array.Empty<PlatformCertificate>())
        {
            ThrowOnCall = new InvalidOperationException("上游 500"),
        };

        var store = new WechatPayPlatformCertificateCache();
        var refresher = new WechatPayPlatformCertificateRefresher(
            client, new FakeCredentialProvider(ApiKey), store, store);

        var refreshed = await refresher.TryRefreshAsync(CreateMerchant(), "DEADBEEF");

        refreshed.Should().BeFalse("刷新失败必须是「仍拒绝」，绝不允许放行（§2.6 红线）");
    }

    /// <summary>空白序列号 ⇒ 直接 false（不构成可刷新语义，也不触发下载）。</summary>
    [Fact]
    public async Task TryRefreshAsync_ShouldReturnFalse_WhenSerialBlank()
    {
        var client = new FakeCertificatesClient(Array.Empty<PlatformCertificate>());
        var store = new WechatPayPlatformCertificateCache();
        var refresher = new WechatPayPlatformCertificateRefresher(
            client, new FakeCredentialProvider(ApiKey), store, store);

        (await refresher.TryRefreshAsync(CreateMerchant(), " ")).Should().BeFalse();
        client.CallCount.Should().Be(0);
    }

    // ---- helpers -------------------------------------------------------------

    private static WechatPayMerchantConfig CreateMerchant() => new()
    {
        MchId = "1900000000",
        SerialNumber = "SERIAL-MERCHANT-0",
        PrivateKeySecretName = "pay:test:key",
        ApiKeySecretName = "pay:test:apikey",
    };

    private static X509Certificate2 CreateSelfSignedCertificate(string subject = "CN=MudWechatPayRefreshTest")
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
    }

    /// <summary>构造官方形态的 <c>data[]</c> 条目（PEM 经 AES-256-GCM 加密）。</summary>
    private static PlatformCertificate CreateItem(X509Certificate2 certificate, byte[] apiKey)
    {
        var (nonce, cipherText) = EncryptOfficialPayload(certificate.ExportCertificatePem(), apiKey);

        return new PlatformCertificate
        {
            SerialNumber = certificate.SerialNumber,
            EffectiveTime = DateTimeOffset.UtcNow.AddDays(-1).ToUnixTimeSeconds(),
            ExpireTime = DateTimeOffset.UtcNow.AddDays(30).ToUnixTimeSeconds(),
            EncryptCertificate = new PlatformCertificateCipher
            {
                Algorithm = WechatPayAesGcmCodec.Algorithm,
                Nonce = nonce,
                AssociatedData = "certificate",
                Ciphertext = cipherText,
            },
        };
    }

    /// <summary>按官方线格式加密：nonce 为 12 个 ASCII 字符，ciphertext 为 <c>Base64(密文‖tag)</c>。</summary>
    private static (string Nonce, string CipherText) EncryptOfficialPayload(string plaintext, byte[] apiKey)
    {
        const string nonce = "abcdefghijkl";
        var nonceBytes = Encoding.UTF8.GetBytes(nonce);
        var plain = Encoding.UTF8.GetBytes(plaintext);
        var cipher = new byte[plain.Length];
        var tag = new byte[WechatPayAesGcmCodec.TagSizeBytes];

        // SYSLIB0053：带 tag 长度的构造函数仅 net9.0+ 提供，测试工程单 TFM 为 net8.0（与生产侧分流等价）。
#pragma warning disable SYSLIB0053
        using (var aes = new AesGcm(apiKey))
#pragma warning restore SYSLIB0053
        {
            aes.Encrypt(nonceBytes, plain, cipher, tag, Encoding.UTF8.GetBytes("certificate"));
        }

        var withTag = new byte[cipher.Length + tag.Length];
        Buffer.BlockCopy(cipher, 0, withTag, 0, cipher.Length);
        Buffer.BlockCopy(tag, 0, withTag, cipher.Length, tag.Length);

        return (nonce, Convert.ToBase64String(withTag));
    }

    /// <summary>把证书实例转成「可入库的第二份」（缓存持有所有权，测试自身的实例仍需释放）。</summary>
    /// <remarks>
    /// 用 <c>new X509Certificate2(byte[])</c> 而非 <c>X509CertificateLoader</c>：后者自 net9.0 才有，
    /// 而本测试工程单 TFM 为 net8.0。
    /// </remarks>
    private static X509Certificate2 X509CertificateLoaderShim(X509Certificate2 certificate)
#pragma warning disable SYSLIB0057
        => new(certificate.RawData);
#pragma warning restore SYSLIB0057

    /// <summary>证书客户端替身（可统计调用次数并注入故障）。</summary>
    private sealed class FakeCertificatesClient : IWechatPayCertificatesService
    {
        private readonly IReadOnlyList<PlatformCertificate> _items;
        private int _callCount;

        public FakeCertificatesClient(IReadOnlyList<PlatformCertificate> items) => _items = items;

        public FakeCertificatesClient(PlatformCertificate single) => _items = new[] { single };

        public Exception? ThrowOnCall { get; init; }

        public int CallCount => Volatile.Read(ref _callCount);

        public Task<PlatformCertificatesResponse> GetPlatformCertificatesAsync(
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _callCount);

            if (ThrowOnCall is { } failure)
            {
                throw failure;
            }

            return Task.FromResult(new PlatformCertificatesResponse { Data = _items.ToList() });
        }
    }

    /// <summary>凭据取用替身（只提供 APIv3 密钥）。</summary>
    private sealed class FakeCredentialProvider : IWechatPayMerchantCredentialProvider
    {
        private readonly byte[] _apiKey;

        public FakeCredentialProvider(byte[] apiKey) => _apiKey = apiKey;

        public Task<byte[]> LoadApiKeyAsync(
            WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
            => Task.FromResult((byte[])_apiKey.Clone());

        public Task<RSA> LoadPrivateKeyAsync(
            WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("刷新链路不取商户私钥。");
    }

    /// <summary>只读证书存储替身（不实现写入端口 ⇒ 模拟宿主自定义只读存储）。</summary>
    private sealed class ReadOnlyCertificateStore : IWechatPayPlatformCertificateStore
    {
        public bool TryGetCertificate(string? serialNumber, out X509Certificate2? certificate)
        {
            certificate = null;
            return false;
        }
    }
}
