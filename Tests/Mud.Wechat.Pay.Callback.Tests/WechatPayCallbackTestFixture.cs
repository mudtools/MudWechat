// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.Callback.Tests;

/// <summary>
/// 支付回调三闸测试夹具：真实 RSA / 真实 AES-GCM / 真实抗重放实现 + 最小替身。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么用真密码学而非 Mock</b>：三闸的失败模式全都在<b>密码学与字节边界</b>上
/// （tag 位置、nonce 长度、验签串换行、序列号匹配）。把这些 Mock 掉等于只测「分支被调用」，
/// 而真正会炸的是「字节错位」——本组用例必须跑真算法。
/// </para>
/// <para><b>商户键与序列号</b>：断言与生产常量同形（<c>mchid</c> / 平台证书序列号）。</para>
/// </remarks>
internal sealed class WechatPayCallbackTestFixture : IDisposable
{
    /// <summary>测试商户键（普通商户形态 = <c>mchid</c>）。</summary>
    public const string MerchantKey = "1900000000";

    /// <summary>测试 APIv3 密钥（32 字节 ASCII，仅用于测试）。</summary>
    public const string ApiKeyText = "0123456789abcdef0123456789abcdef";

    /// <summary>非 12 字符的 nonce（用于形状校验用例）。</summary>
    public const string ShortNonce = "abc";

    /// <summary>平台证书序列号（非法定值，测试自签）；<see cref="RotateSigningCertificate"/> 后会切换。</summary>
    public string PlatformSerial { get; private set; }

    /// <summary>测试时钟（可前进以验证时效闸）。</summary>
    public TestClock Clock { get; } = new();

    /// <summary>可切换 APIv3 密钥的凭据替身（验证「解密失败不消耗指纹」）。</summary>
    public FakeCredentialProvider Credentials { get; }

    private RSA _platformKey;
    private readonly X509Certificate2 _platformCertificate;
    private readonly RSA _merchantKey;

    /// <summary>「官方已换新证书、但本机尚未拉取」的那一本（由刷新替身发布）。</summary>
    private X509Certificate2? _pendingRotationCertificate;

    /// <summary>创建夹具（自签平台证书 + 商户密钥）。</summary>
    public WechatPayCallbackTestFixture()
    {
        _platformKey = RSA.Create(2048);
        _merchantKey = RSA.Create(2048);

        var request = new CertificateRequest(
            "CN=MudWechatPayTestPlatform", _platformKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        _platformCertificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(30));
        PlatformSerial = _platformCertificate.SerialNumber;

        Credentials = new FakeCredentialProvider(Encoding.UTF8.GetBytes(ApiKeyText));

        MerchantManager = new FakeMerchantManager(CreateMerchant());
        Certificates = new FakeCertificateStore(PlatformSerial, _platformCertificate);
        SignatureProviders = new FakeSignatureProviderFactory(
            new WechatPaySignatureProvider(CreateMerchant().SerialNumber, _merchantKey, Certificates));

        Options = new WechatPayCallbackOptions();
        OptionsMonitor = new TestOptionsMonitor(Options);

        ReplayGuard = new InMemoryWechatCallbackReplayGuard();
    }

    /// <summary>商户基座替身。</summary>
    public FakeMerchantManager MerchantManager { get; }

    /// <summary>平台证书存储替身。</summary>
    public FakeCertificateStore Certificates { get; }

    /// <summary>签名提供器工厂替身。</summary>
    public FakeSignatureProviderFactory SignatureProviders { get; }

    /// <summary>回调配置（可直接改字段以验证配置面校验）。</summary>
    public WechatPayCallbackOptions Options { get; }

    /// <summary>配置监视器替身。</summary>
    public TestOptionsMonitor OptionsMonitor { get; }

    /// <summary>真实的内存抗重放实现（闸③ 的行为必须真实）。</summary>
    public InMemoryWechatCallbackReplayGuard ReplayGuard { get; }

    /// <summary>构造接收器（注入测试时钟；可选注入平台证书刷新端口）。</summary>
    /// <param name="refresher">平台证书刷新替身；<c>null</c> = 未注册（宿主只装回调的形态）。</param>
    public WechatPayCallbackReceiver CreateReceiver(IWechatPayPlatformCertificateRefresher? refresher = null)
        => new(MerchantManager, SignatureProviders, Credentials, ReplayGuard, OptionsMonitor, Clock, refresher);

    /// <summary>
    /// 模拟官方<b>轮换平台证书</b>：换一把新私钥 + 自签新证书并切换到它签名，
    /// 但<b>不</b>把证书放进本机存储（= 「新序列号先于本机拉取到达」的真实时序）。
    /// </summary>
    /// <remarks>须由刷新替身经 <see cref="PublishPendingRotation"/> 发布后才对本机可见。</remarks>
    public void RotateSigningCertificate()
    {
        _pendingRotationCertificate?.Dispose();
        _platformKey.Dispose();

        _platformKey = RSA.Create(2048);
        var request = new CertificateRequest(
            "CN=MudWechatPayRotatedPlatform", _platformKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        _pendingRotationCertificate = request.CreateSelfSigned(
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(30));

        PlatformSerial = _pendingRotationCertificate.SerialNumber;
    }

    /// <summary>把「待轮换」证书发布到本机存储（刷新成功的等价动作）。</summary>
    /// <returns>是否有待发布的证书。</returns>
    public bool PublishPendingRotation()
    {
        if (_pendingRotationCertificate is null)
        {
            return false;
        }

        Certificates.Add(_pendingRotationCertificate.SerialNumber, _pendingRotationCertificate);
        return true;
    }

    /// <summary>
    /// 构造一条**合法签名**的通知（平台私钥签名 + APIv3 密钥加密）。
    /// </summary>
    /// <param name="resourceJson">明文 resource JSON。</param>
    /// <param name="eventType">事件类型。</param>
    /// <param name="timestamp">时间戳（默认取夹具时钟当前值）。</param>
    /// <param name="nonce">请求随机串。</param>
    /// <param name="serial">平台证书序列号（默认取夹具序列号）。</param>
    /// <param name="associatedData">AAD（默认 <c>transaction</c>）。</param>
    /// <param name="algorithm">算法标识（默认合法值）。</param>
    /// <param name="cipherTextOverride">直接指定 ciphertext（用于形状/篡改用例）。</param>
    /// <returns>请求头与报文体。</returns>
    public (WechatPayCallbackHeaders Headers, string Body) CreateNotification(
        string resourceJson,
        string eventType = "TRANSACTION.SUCCESS",
        string? timestamp = null,
        string nonce = "nonce0123456789abcdefghijklmno",
        string? serial = null,
        string associatedData = "transaction",
        string algorithm = WechatPayAesGcmCodec.Algorithm,
        string? cipherTextOverride = null)
    {
        var cipherText = cipherTextOverride ?? EncryptResource(resourceJson, associatedData);
        var body = BuildBody(eventType, algorithm, cipherText, associatedData);

        return WithSignedBody(body, timestamp, nonce, serial ?? PlatformSerial);
    }

    /// <summary>对<b>任意报文体</b>生成合法签名头（用于「报文缺节点」等形状用例）。</summary>
    /// <param name="body">报文体原文。</param>
    /// <param name="timestamp">时间戳（默认取夹具时钟当前值）。</param>
    /// <param name="nonce">请求随机串。</param>
    /// <param name="serial">序列号。</param>
    /// <returns>请求头与报文体。</returns>
    public (WechatPayCallbackHeaders Headers, string Body) WithSignedBody(
        string body,
        string? timestamp = null,
        string nonce = "nonce0123456789abcdefghijklmno",
        string? serial = null)
    {
        var ts = timestamp ?? Clock.UtcNowEpochSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var message = WechatPaySignatureMessages.BuildVerifyMessage(ts, nonce, body);
        var signature = Convert.ToBase64String(
            _platformKey.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));

        return (new WechatPayCallbackHeaders(ts, nonce, signature, serial ?? PlatformSerial), body);
    }

    /// <summary>固定 nonce 字符串（12 字符 ASCII；接收器以 UTF8 还原为 12 字节，故必须可往返）。</summary>
    public const string ResourceNonce = "abcdefghijkl";

    /// <summary>
    /// 用指定 APIv3 密钥加密 resource，并把 GCM tag 拼在密文尾部后整体 Base64（官方形态）。
    /// </summary>
    /// <remarks>
    /// <b>为何直接用 <see cref="AesGcm"/> 而不走 <c>WechatPayAesGcmCodec.Encrypt</c></b>：
    /// 后者<b>每次随机生成 nonce</b>（生产正确行为：GCM 复用 nonce 会泄露明文异或值），
    /// 而通知里的 <c>resource.nonce</c> 必须是 12 个<b>可往返的 ASCII 字符</b>，
    /// 故测试侧需要「指定 nonce」的加密 —— 不能为测试便利给生产 API 开一个可复用 nonce 的口子。
    /// 本测试因此只覆盖生产解密路径（<c>TryDecrypt</c>），加密侧由 P0-b 的往返用例负责。
    /// </remarks>
    public static string EncryptResource(
        string resourceJson, string associatedData = "transaction", byte[]? key = null)
    {
        var nonce = Encoding.UTF8.GetBytes(ResourceNonce);
        var plaintext = Encoding.UTF8.GetBytes(resourceJson);
        var aad = Encoding.UTF8.GetBytes(associatedData);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[WechatPayAesGcmCodec.TagSizeBytes];

        // SYSLIB0053：带 tag 长度的 AesGcm 构造函数仅 net9.0+ 提供，而本测试工程单 TFM 为 net8.0；
        // 生产侧 WechatPayAesGcmCodec 已按 TFM 分流，测试侧此处旧构造的 tag 长度恒为 16（与非过时构造等价）。
#pragma warning disable SYSLIB0053
        using (var aes = new AesGcm(key ?? Encoding.UTF8.GetBytes(ApiKeyText)))
#pragma warning restore SYSLIB0053
        {
            aes.Encrypt(nonce, plaintext, ciphertext, tag, aad);
        }

        var withTag = new byte[ciphertext.Length + tag.Length];
        Array.Copy(ciphertext, withTag, ciphertext.Length);
        Array.Copy(tag, 0, withTag, ciphertext.Length, tag.Length);

        return Convert.ToBase64String(withTag);
    }

    /// <summary>构造通知报文体（显式拼串：同时锁定官方字段名）。</summary>
    public static string BuildBody(
        string eventType, string algorithm, string cipherText, string associatedData)
        => "{\"id\":\"EV-TEST-1\"," +
           "\"create_time\":\"2026-10-09T12:00:00+08:00\"," +
           "\"resource_type\":\"encrypt-resource\"," +
           "\"event_type\":\"" + eventType + "\"," +
           "\"summary\":\"支付成功\"," +
           "\"resource\":{" +
           "\"original_type\":\"transaction\"," +
           "\"algorithm\":\"" + algorithm + "\"," +
           "\"ciphertext\":\"" + cipherText + "\"," +
           "\"associated_data\":\"" + associatedData + "\"," +
           "\"nonce\":\"" + ResourceNonce + "\"}}";

    /// <summary>合法的交易资源明文（字段名照官方原文）。</summary>
    public const string TransactionResourceJson =
        "{\"mchid\":\"1900000000\",\"appid\":\"wx-test\",\"out_trade_no\":\"ORDER-1\"," +
        "\"transaction_id\":\"4200001234\",\"trade_type\":\"JSAPI\",\"trade_state\":\"SUCCESS\"," +
        "\"trade_state_desc\":\"支付成功\",\"bank_type\":\"OTHERS\",\"attach\":\"\"," +
        "\"success_time\":\"2026-10-09T12:00:00+08:00\",\"payer\":{\"openid\":\"o-test\"}," +
        "\"amount\":{\"total\":100,\"payer_total\":100,\"currency\":\"CNY\",\"payer_currency\":\"CNY\"}}";

    /// <summary>创建一个合法商户配置（测试用）。</summary>
    public static WechatPayMerchantConfig CreateMerchant() => new()
    {
        MchId = MerchantKey,
        SerialNumber = "SERIAL-MERCHANT-0",
        PrivateKeySecretName = "pay:test:key",
        ApiKeySecretName = "pay:test:apikey",
    };

    /// <inheritdoc />
    public void Dispose()
    {
        _pendingRotationCertificate?.Dispose();
        _platformCertificate.Dispose();
        _platformKey.Dispose();
        _merchantKey.Dispose();
    }

    /// <summary>可前进的测试时钟。</summary>
    internal sealed class TestClock : IWechatPayCallbackClock
    {
        private long _epochSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        /// <inheritdoc />
        public long UtcNowEpochSeconds => _epochSeconds;

        /// <summary>前进指定秒数。</summary>
        /// <param name="seconds">秒数。</param>
        public void Advance(long seconds) => _epochSeconds += seconds;
    }

    /// <summary><see cref="IOptionsMonitor{T}"/> 的最小替身（测试无需热更通知）。</summary>
    internal sealed class TestOptionsMonitor : IOptionsMonitor<WechatPayCallbackOptions>
    {
        /// <summary>创建监视器。</summary>
        /// <param name="value">当前值。</param>
        public TestOptionsMonitor(WechatPayCallbackOptions value) => CurrentValue = value;

        /// <inheritdoc />
        public WechatPayCallbackOptions CurrentValue { get; }

        /// <inheritdoc />
        public WechatPayCallbackOptions Get(string? name) => CurrentValue;

        /// <inheritdoc />
        public IDisposable? OnChange(Action<WechatPayCallbackOptions, string?> listener) => null;
    }

    /// <summary>商户基座替身（只实现回调侧用到的两个成员）。</summary>
    internal sealed class FakeMerchantManager : IWechatPayMerchantManager
    {
        private readonly WechatPayMerchantConfig _merchant;

        /// <summary>创建替身。</summary>
        /// <param name="merchant">唯一商户。</param>
        public FakeMerchantManager(WechatPayMerchantConfig merchant) => _merchant = merchant;

        /// <inheritdoc />
        public IReadOnlyList<WechatPayMerchantConfig> Merchants => new[] { _merchant };

        /// <inheritdoc />
        public bool TryGetMerchant(string merchantKey, out WechatPayMerchantConfig? config)
        {
            if (string.Equals(merchantKey, _merchant.MerchantKey, StringComparison.Ordinal))
            {
                config = _merchant;
                return true;
            }

            config = null;
            return false;
        }

        /// <inheritdoc />
        public WechatPayMerchantConfig GetMerchant(string merchantKey)
            => TryGetMerchant(merchantKey, out var config) ? config! : throw new InvalidOperationException();

        /// <inheritdoc />
        public bool TryGetByOfficialIds(
            string? mchId, string? spMchId, string? subMchId, out WechatPayMerchantConfig? config)
            => TryGetMerchant(mchId ?? string.Empty, out config);
    }

    /// <summary>可切换 APIv3 密钥的凭据替身。</summary>
    internal sealed class FakeCredentialProvider : IWechatPayMerchantCredentialProvider
    {
        /// <summary>当前 APIv3 密钥。</summary>
        public byte[] ApiKey { get; set; }

        /// <summary>创建替身。</summary>
        /// <param name="apiKey">初始 APIv3 密钥（32 字节）。</param>
        public FakeCredentialProvider(byte[] apiKey) => ApiKey = apiKey;

        /// <inheritdoc />
        public Task<byte[]> LoadApiKeyAsync(WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
            => Task.FromResult((byte[])ApiKey.Clone());

        /// <inheritdoc />
        /// <remarks>回调侧不取私钥（验签用平台证书公钥），故恒抛以暴露误用。</remarks>
        public Task<RSA> LoadPrivateKeyAsync(WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
            => throw new NotSupportedException("回调链路不取商户私钥。");
    }

    /// <summary>平台证书刷新替身（按端口契约：已命中即短路、未知才「下载」）。</summary>
    internal sealed class FakeCertificateRefresher : IWechatPayPlatformCertificateRefresher
    {
        private readonly WechatPayCallbackTestFixture _fixture;
        private readonly bool _canPublish;

        /// <summary>创建替身。</summary>
        /// <param name="fixture">夹具（用于查存储与发布待轮换证书）。</param>
        /// <param name="canPublish"><c>false</c> = 模拟刷新拿不到该序列号（仍返回失败）。</param>
        public FakeCertificateRefresher(WechatPayCallbackTestFixture fixture, bool canPublish = true)
        {
            _fixture = fixture;
            _canPublish = canPublish;
        }

        /// <summary>被调用次数（含短路）。</summary>
        public int CallCount { get; private set; }

        /// <summary>真正发起「下载」的次数（<b>短路时不递增</b>）。</summary>
        public int DownloadCount { get; private set; }

        /// <inheritdoc />
        public Task<bool> TryRefreshAsync(
            WechatPayMerchantConfig merchant, string serialNumber, CancellationToken cancellationToken = default)
        {
            CallCount++;

            // 端口契约第一闸：已命中 ⇒ 幂等短路，零下载。
            if (_fixture.Certificates.TryGetCertificate(serialNumber, out _))
            {
                return Task.FromResult(true);
            }

            DownloadCount++;
            if (_canPublish)
            {
                _fixture.PublishPendingRotation();
            }

            return Task.FromResult(_fixture.Certificates.TryGetCertificate(serialNumber, out _));
        }
    }

    /// <summary>平台证书存储替身（按序列号分槽，支持轮换后追加）。</summary>
    internal sealed class FakeCertificateStore : IWechatPayPlatformCertificateStore
    {
        private readonly Dictionary<string, X509Certificate2> _bySerial = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>创建替身。</summary>
        /// <param name="serial">初始序列号。</param>
        /// <param name="certificate">初始证书。</param>
        public FakeCertificateStore(string serial, X509Certificate2 certificate)
            => _bySerial[serial] = certificate;

        /// <summary>追加（或覆盖）一本证书 —— 模拟刷新成功后的入库。</summary>
        /// <param name="serial">序列号。</param>
        /// <param name="certificate">证书。</param>
        public void Add(string serial, X509Certificate2 certificate) => _bySerial[serial] = certificate;

        /// <summary>当前已知序列号数。</summary>
        public int Count => _bySerial.Count;

        /// <inheritdoc />
        public bool TryGetCertificate(string? serialNumber, out X509Certificate2? certificate)
        {
            if (!string.IsNullOrWhiteSpace(serialNumber) && _bySerial.TryGetValue(serialNumber, out var found))
            {
                certificate = found;
                return true;
            }

            certificate = null;
            return false;
        }
    }

    /// <summary>签名提供器工厂替身（恒返回同一真实提供器）。</summary>
    internal sealed class FakeSignatureProviderFactory : IWechatPaySignatureProviderFactory
    {
        private readonly IWechatPaySignatureProvider _provider;

        /// <summary>创建替身。</summary>
        /// <param name="provider">真实签名提供器。</param>
        public FakeSignatureProviderFactory(IWechatPaySignatureProvider provider) => _provider = provider;

        /// <inheritdoc />
        public Task<IWechatPaySignatureProvider> GetOrCreateAsync(
            WechatPayMerchantConfig merchant, CancellationToken cancellationToken = default)
            => Task.FromResult(_provider);
    }
}
