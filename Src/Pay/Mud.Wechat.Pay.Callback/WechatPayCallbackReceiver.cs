// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mud.Wechat.Pay.Callback;

/// <summary>
/// 支付通知接收器：**三道 fail-closed 闸**（① 平台证书验签 ② 时间戳窗口 ③ 一次性指纹）+ <c>resource</c> 解密。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：通知验签与解密规则见微信支付 APIv3 通知规范；三闸的必要性见设计方案 §2.6
/// （v1「无时效闸 / 无指纹闸」的结论**已被证伪并否决**：只验签不校验时效时，截获一次合法通知即可无限重放）。
/// </para>
/// <para><b>执行顺序（不可调换）</b>：</para>
/// <list type="number">
/// <item>商户键解析（未登记即拒 —— <b>不得</b>回落默认商户）；</item>
/// <item>四头齐全性（缺任一即拒）；</item>
/// <item><b>闸②</b> 时间戳窗口 ±300s（缺失 / 非数字 / 超窗一律拒）；</item>
/// <item><b>闸①</b> 平台证书 RSA-SHA256 验签（序列号未知 / 非法 Base64 / 不匹配一律拒）；</item>
/// <item>报文反序列化（<b>验签之后</b>才允许解析，见 DTO remarks）；</item>
/// <item><c>resource</c> 算法与密文形状校验；</item>
/// <item><b>解密</b> AES-256-GCM（失败即拒，<b>且不消耗指纹</b> —— 官方重试可重入）；</item>
/// <item><b>闸③</b> 一次性指纹（键 = <c>{商户键}:SHA1(ciphertext)</c>，窗口内重复即拒）。</item>
/// </list>
/// <para>
/// <b>为什么指纹闸排在解密之后</b>：若排在之前，一次<b>解密失败</b>（如密钥短暂不可用）就会把指纹消费掉，
/// 官方重试将被判重放而永久丢单。排在解密之后，密码学失败可安全重入，而真正的重放仍被拦住
/// （同一密文解密必然成功 ⇒ 第二次进入即命中指纹）。
/// </para>
/// <para>
/// <b>fail-closed 红线（§2.6）</b>：任一闸都不得降级为 fail-open；反重放存储异常<b>必须上抛</b>
/// （→ 5xx → 官方重试），<b>禁止</b>吞异常放行或静默丢事件。密码学一律走
/// <c>WechatPayAesGcmCodec</c>（<b>禁止</b>复用企微 32 字节块 PKCS7）。
/// </para>
/// <para><b>PAY-B7</b>：日志只记类别与商户键；签名原文、<c>ciphertext</c>、解密明文、APIv3 密钥均不入日志。</para>
/// </remarks>
public sealed class WechatPayCallbackReceiver
{
    /// <summary>公钥模式（<c>Wechatpay-Serial: PUB_KEY_ID_…</c>）的前缀。</summary>
    /// <remarks>
    /// 官方「微信支付公钥」模式与「平台证书」模式并存（方案 §2.5 待决项）。
    /// 本 SDK 的 P1-b <b>裁决采用平台证书模式</b>（即 §2.6 gate ① 所列形态）；
    /// 公钥模式在运行时被<b>显式识别</b>并 fail-closed 拒绝，且给出点名告警 ——
    /// 不留「未知序列号」这种无从排查的形态。
    /// </remarks>
    public const string PublicKeyIdPrefix = "PUB_KEY_ID_";

    private readonly IWechatPayMerchantManager _merchants;
    private readonly IWechatPaySignatureProviderFactory _signatureProviders;
    private readonly IWechatPayMerchantCredentialProvider _credentials;
    private readonly IWechatCallbackReplayGuard _replayGuard;
    private readonly IOptionsMonitor<WechatPayCallbackOptions> _optionsMonitor;
    private readonly IWechatPayCallbackClock _clock;
    private readonly IWechatPayPlatformCertificateRefresher? _refresher;
    private readonly ILogger<WechatPayCallbackReceiver>? _logger;

    /// <summary>创建接收器（DI 用）。</summary>
    /// <param name="merchants">多商户基座（商户键 → 配置）。</param>
    /// <param name="signatureProviders">签名提供器工厂（验签）。</param>
    /// <param name="credentials">凭据取用端口（取 APIv3 密钥解密 <c>resource</c>）。</param>
    /// <param name="replayGuard">抗重放端口（一次性指纹）。</param>
    /// <param name="optionsMonitor">回调配置监视器。</param>
    /// <param name="refresher">
    /// 平台证书按需刷新端口（<b>可选</b>）：未知 <c>Wechatpay-Serial</c> 时补齐证书（§2.6 闸①）。
    /// 未注册时未知序列号直接拒绝（仍是 fail-closed，只是少了轮换期自愈）。
    /// </param>
    /// <param name="logger">日志器（可选）。</param>
    /// <exception cref="ArgumentNullException">任一必填参数为 <c>null</c>。</exception>
    public WechatPayCallbackReceiver(
        IWechatPayMerchantManager merchants,
        IWechatPaySignatureProviderFactory signatureProviders,
        IWechatPayMerchantCredentialProvider credentials,
        IWechatCallbackReplayGuard replayGuard,
        IOptionsMonitor<WechatPayCallbackOptions> optionsMonitor,
        IWechatPayPlatformCertificateRefresher? refresher = null,
        ILogger<WechatPayCallbackReceiver>? logger = null)
        : this(merchants, signatureProviders, credentials, replayGuard, optionsMonitor,
            new SystemWechatPayCallbackClock(), refresher, logger)
    {
    }

    /// <summary>创建接收器（测试可注入时钟与刷新端口）。</summary>
    internal WechatPayCallbackReceiver(
        IWechatPayMerchantManager merchants,
        IWechatPaySignatureProviderFactory signatureProviders,
        IWechatPayMerchantCredentialProvider credentials,
        IWechatCallbackReplayGuard replayGuard,
        IOptionsMonitor<WechatPayCallbackOptions> optionsMonitor,
        IWechatPayCallbackClock clock,
        IWechatPayPlatformCertificateRefresher? refresher = null,
        ILogger<WechatPayCallbackReceiver>? logger = null)
    {
        _merchants = merchants ?? throw new ArgumentNullException(nameof(merchants));
        _signatureProviders = signatureProviders ?? throw new ArgumentNullException(nameof(signatureProviders));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _replayGuard = replayGuard ?? throw new ArgumentNullException(nameof(replayGuard));
        _optionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _refresher = refresher;
        _logger = logger;
    }

    /// <summary>
    /// 接收并校验一条通知：通过三道闸后返回已解密上下文。
    /// </summary>
    /// <param name="merchantKey">路由解析出的商户键。</param>
    /// <param name="headers">四个签名相关请求头。</param>
    /// <param name="body">请求体<b>原文</b>（验签输入的原始字节，不得先反序列化再重排）。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>已验签且已解密的通知上下文。</returns>
    /// <exception cref="WechatCallbackException">任一闸失败（<c>Kind</c> 标明类别，消息不含敏感材料）。</exception>
    public async Task<WechatPayCallbackContext> ReceiveAsync(
        string merchantKey,
        WechatPayCallbackHeaders headers,
        string body,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(merchantKey))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.UnknownReceiver, "回调路由缺少商户键。");
        }

        if (body is null)
        {
            throw new ArgumentNullException(nameof(body));
        }

        // ① 商户解析：未登记即拒，绝不回落默认商户（否则会把 A 商户的资金通知派给 B 商户）。
        if (!_merchants.TryGetMerchant(merchantKey, out var merchant) || merchant is null)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.UnknownReceiver,
                $"回调路由命中的商户键未登记：{merchantKey}。");
        }

        var options = _optionsMonitor.CurrentValue;

        // ② 四头齐全性（缺任一即拒 —— 缺失即畸形，不得跳过后续校验）。
        if (string.IsNullOrWhiteSpace(headers.Signature))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "缺少 Wechatpay-Signature 请求头。");
        }

        if (string.IsNullOrWhiteSpace(headers.Timestamp))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingTimestamp, "缺少 Wechatpay-Timestamp 请求头。");
        }

        if (string.IsNullOrWhiteSpace(headers.Nonce))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingNonce, "缺少 Wechatpay-Nonce 请求头。");
        }

        if (string.IsNullOrWhiteSpace(headers.SerialNumber))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingSignature, "缺少 Wechatpay-Serial 请求头。");
        }

        // ③ 公钥模式识别：显式点名（而非落入「未知序列号」）。
        if (headers.SerialNumber!.StartsWith(PublicKeyIdPrefix, StringComparison.Ordinal))
        {
            _logger?.LogWarning(WechatPayCallbackLogEvents.PublicKeyModeUnsupported,
                "通知使用微信支付公钥模式（Wechatpay-Serial 以 {Prefix} 开头）；" +
                "本 SDK 采用平台证书模式（方案 §2.6 gate ①），已 fail-closed 拒绝。MerchantKey: {MerchantKey}",
                PublicKeyIdPrefix, merchantKey);

            throw new WechatCallbackException(
                WechatCallbackFailureKind.InvalidSignature,
                "通知使用微信支付公钥模式（PUB_KEY_ID_），当前采用平台证书模式验签，已拒绝。");
        }

        // ④ 闸②：时间戳窗口（缺失 / 非数字 / 超窗一律拒）。
        var timestampVerdict = WechatPayTimestampGate.Validate(
            headers.Timestamp, _clock.UtcNowEpochSeconds, options.AllowClockSkewSeconds);

        if (timestampVerdict != WechatPayTimestampGate.Verdict.Valid)
        {
            throw new WechatCallbackException(
                timestampVerdict == WechatPayTimestampGate.Verdict.OutOfWindow
                    ? WechatCallbackFailureKind.TimestampOutOfRange
                    : WechatCallbackFailureKind.MissingTimestamp,
                $"Wechatpay-Timestamp 未通过时效闸（{timestampVerdict}，窗口 ±{options.AllowClockSkewSeconds}s）。");
        }

        // ⑤ 闸①：平台证书 RSA-SHA256 验签（验签串 = 时间戳\n随机串\n报文体\n）。
        var signatureProvider = await _signatureProviders
            .GetOrCreateAsync(merchant, cancellationToken)
            .ConfigureAwait(false);

        var verifyMessage = WechatPaySignatureMessages.BuildVerifyMessage(
            headers.Timestamp!, headers.Nonce!, body);

        if (!signatureProvider.Verify(headers.SerialNumber, verifyMessage, headers.Signature))
        {
            // §2.6 闸① 的「未知序列号可自愈」路径：官方在到期前**并行**下发新旧平台证书，
            // 新序列号可能先于本机拉取就出现在通知头里 ⇒ 给一次刷新后复验的机会。
            //
            // **只**补一次、失败即拒（不得重试到成功）；且刷新是否真的下载由端口实现决定 ——
            // 它对「已命中本地存储」的序列号直接短路，故**签名不匹配绝不会触发下载**
            // （否则任意伪造请求都能驱动一次证书仓库往返 = 放大型 DoS）。
            if (_refresher is null
                || !await _refresher.TryRefreshAsync(merchant, headers.SerialNumber!, cancellationToken).ConfigureAwait(false)
                || !signatureProvider.Verify(headers.SerialNumber, verifyMessage, headers.Signature))
            {
                // 不区分「序列号未知」与「签名不匹配」对外提示，避免成为探测 oracle；细节只经日志类别体现。
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.InvalidSignature,
                    "通知验签未通过（序列号未知或签名与报文体不匹配）。");
            }
        }

        // ⑥ 验签通过后才解析报文（原始字节验签是官方 FAQ 的第一要求）。
        WechatPayNotification? notification;
        try
        {
            notification = JsonSerializer.Deserialize(body, WechatPayCallbackJsonContext.Default.WechatPayNotification);
        }
        catch (JsonException ex)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed, "通知报文不是合法 JSON。", ex);
        }

        var resource = notification?.Resource;
        if (notification is null || resource is null)
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "通知报文缺少 resource 节点。");
        }

        if (!string.Equals(resource.Algorithm, WechatPayAesGcmCodec.Algorithm, StringComparison.Ordinal))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.DecryptFailed,
                $"通知 resource 使用了不支持的加密算法（仅支持 {WechatPayAesGcmCodec.Algorithm}）。");
        }

        if (string.IsNullOrEmpty(resource.CipherText) || string.IsNullOrEmpty(resource.Nonce))
        {
            throw new WechatCallbackException(
                WechatCallbackFailureKind.MissingEncrypt, "通知 resource 缺少 ciphertext / nonce。");
        }

        // ⑦ 解密（失败即拒，且**不消耗**指纹 —— 官方重试可重入）。
        var apiKey = await _credentials
            .LoadApiKeyAsync(merchant, cancellationToken)
            .ConfigureAwait(false);

        string resourceJson;
        try
        {
            if (!TryDecryptResource(apiKey, resource, out var plaintext))
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.DecryptFailed,
                    "通知 resource 解密失败（APIv3 密钥不匹配或密文被篡改）。");
            }

            resourceJson = plaintext!;
        }
        finally
        {
            // 密钥材料驻留内存的时间窗收敛到本次解密（PAY-B7 的运行时侧要求）。
            CryptographicOperations.ZeroMemory(apiKey);
        }

        // ⑧ 闸③：一次性指纹（键 = {商户键}:SHA1(ciphertext)，不含密文本身）。
        if (options.RequireReplayGuard)
        {
            var fingerprint = BuildFingerprint(merchantKey, resource.CipherText!);
            var window = TimeSpan.FromSeconds(options.ReplayWindowSeconds);

            // 存储故障**必须上抛**（端口契约）：吞掉会变成「重放放行」或「静默丢事件」。
            var firstConsume = await _replayGuard
                .TryMarkAsync(fingerprint, window, cancellationToken)
                .ConfigureAwait(false);

            if (!firstConsume)
            {
                throw new WechatCallbackException(
                    WechatCallbackFailureKind.ReplaySuspected,
                    $"通知指纹已在 {options.ReplayWindowSeconds}s 保留窗口内消费（疑似重放）。");
            }
        }

        return new WechatPayCallbackContext(
            merchantKey, merchant, notification, resourceJson, DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// 解密 <c>resource</c>：<c>nonce</c> 为 12 个 ASCII 字符、<c>ciphertext</c> 为 Base64（<b>末 16 字节为 GCM tag</b>）。
    /// </summary>
    /// <param name="apiKey">32 字节 APIv3 密钥。</param>
    /// <param name="resource">通知资源节点。</param>
    /// <param name="plaintext">成功时为解密后的 JSON 明文。</param>
    /// <returns>是否成功。</returns>
    /// <remarks>
    /// <b>线格式解析统一收在 <see cref="WechatPayAesGcmCodec.TryDecryptOfficialPayload"/></b>：
    /// 「nonce 为字符串、tag 拼接在密文尾部」是官方对<b>所有</b> <c>AEAD_AES_256_GCM</c> 载荷的统一约定
    /// （回调 <c>resource</c> 与平台证书 <c>encrypt_certificate</c> 完全同形）。曾各自实现一份 ⇒
    /// 任一处漏切 tag 都是「编译期无感、运行期 100% 认证失败」的错位缺陷，故收敛为单一实现。
    /// </remarks>
    private static bool TryDecryptResource(
        byte[] apiKey, WechatPayNotificationResource resource, out string? plaintext)
        => WechatPayAesGcmCodec.TryDecryptOfficialPayload(
            apiKey, resource.Nonce, resource.CipherText, resource.AssociatedData, out plaintext);

    /// <summary>
    /// 组装指纹：<c>{商户键}:{SHA1(ciphertext) 小写十六进制}</c>。
    /// </summary>
    /// <remarks>
    /// <b>不得落盘密文本身</b>（端口契约）：故先单向散列。SHA1 在此<b>不是</b>安全边界
    /// （指纹仅是去重键，且窗口仅 10 分钟），无需抗碰撞性；用 SHA1 是为了与官方示例的指纹口径一致、便于比对。
    /// </remarks>
    private static string BuildFingerprint(string merchantKey, string cipherText)
    {
        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(cipherText));
        return string.Concat(merchantKey, ":", Convert.ToHexString(hash).ToLowerInvariant());
    }
}
