// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Mud.Wechat.Pay.Abstractions.Configuration;
using Mud.Wechat.Pay.Abstractions.Credential;

namespace Mud.Wechat.Pay.Certificates;

/// <summary>
/// <see cref="IWechatPayPlatformCertificateRefresher"/> 默认实现：拉取 <c>GET /v3/certificates</c>、
/// 用商户 APIv3 密钥解密证书密文、写入 <see cref="IWechatPayPlatformCertificateWriter"/>。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012551764"/>（2026-10-09 核验）。
/// 证书以 <c>AEAD_AES_256_GCM</c> 密文返回，解密后为 PEM 明文；官方频率限制 1000 次/s（本实现另加
/// 最小刷新间隔，远低于该上限）。
/// </para>
/// <para><b>三道防放大闸（缺一即成 DoS 面）</b>：</para>
/// <list type="number">
/// <item><b>只处理未知序列号</b>：调用契约要求只在「序列号未知」时调用；本实现首行即查读端口，
/// 已命中直接返回 <c>true</c>（不产生任何网络往返，故即便被误用也只是空转）。</item>
/// <item><b>同商户单飞</b>：同商户并发调用只有一个真正下载，其余等它完成<b>后复查</b> ——
/// 未知序列号风暴（伪造任意 <c>Wechatpay-Serial</c>）不会放大成 N 倍下载。</item>
/// <item><b>最小刷新间隔</b>：<see cref="MinRefreshIntervalSeconds"/> 内的重复请求直接返回
/// <c>false</c>（仍由调用方按未知序列号拒绝）。间隔在<b>下载之前</b>就生效 ⇒ 持续失败也不会被反复打。</item>
/// </list>
/// <para>
/// <b>fail-closed</b>：任何失败（未注册写入端口 / 证书列表为空 / 解密或 PEM 解析失败 / 刷新后仍查不到）
/// <b>一律返回 <c>false</c></b>，由调用方照常拒绝 —— <b>绝不</b>「刷新失败即放行」（§2.6 红线）。
/// 唯一上抛的异常是调用方取消（<see cref="OperationCanceledException"/>）。
/// </para>
/// <para><b>PAY-B7</b>：日志只记商户键、序列号、证书条数；APIv3 密钥与证书密文<b>不入日志</b>。</para>
/// </remarks>
public sealed class WechatPayPlatformCertificateRefresher : IWechatPayPlatformCertificateRefresher
{
    /// <summary>
    /// 同一商户两次真实下载之间的最小间隔（秒）。
    /// </summary>
    /// <remarks>
    /// 取值权衡：平台证书轮换是<b>低频</b>事件（有效期以年计、并行期以天计），而未知序列号可能被
    /// 攻击者高频伪造 ⇒ 间隔必须显著大于「一次合法轮换所需的重试密度」。
    /// 60s 下最坏情况是「轮换后首分钟内的请求各失败一次并告警」，代价可接受；再小则失去防放大意义。
    /// </remarks>
    internal const int MinRefreshIntervalSeconds = 60;

    private readonly IWechatPayCertificatesService _certificates;
    private readonly IWechatPayMerchantCredentialProvider _credentials;
    private readonly IWechatPayPlatformCertificateStore _store;
    private readonly IWechatPayPlatformCertificateWriter? _writer;
    private readonly ILogger<WechatPayPlatformCertificateRefresher>? _logger;

    private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, long> _nextAllowedTicks = new(StringComparer.Ordinal);

    /// <summary>创建刷新器。</summary>
    /// <param name="certificates">平台证书客户端（<c>GET /v3/certificates</c>）。</param>
    /// <param name="credentials">商户凭据取用端口（取 APIv3 密钥解密证书密文）。</param>
    /// <param name="store">平台证书读端口（复查用）。</param>
    /// <param name="writer">平台证书写端口（默认缓存实现；宿主只提供读端口时为 <c>null</c>）。</param>
    /// <param name="logger">日志器（可选）。</param>
    /// <exception cref="ArgumentNullException">任一必填参数为 <c>null</c>。</exception>
    public WechatPayPlatformCertificateRefresher(
        IWechatPayCertificatesService certificates,
        IWechatPayMerchantCredentialProvider credentials,
        IWechatPayPlatformCertificateStore store,
        IWechatPayPlatformCertificateWriter? writer = null,
        ILogger<WechatPayPlatformCertificateRefresher>? logger = null)
    {
        _certificates = certificates ?? throw new ArgumentNullException(nameof(certificates));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _writer = writer;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<bool> TryRefreshAsync(
        WechatPayMerchantConfig merchant,
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        if (merchant is null)
        {
            throw new ArgumentNullException(nameof(merchant));
        }

        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            // 空白序列号不构成「可刷新」的语义（上层验签已把它判为失败）。
            return false;
        }

        // 闸①：已命中 ⇒ 幂等返回，零网络往返。
        if (_store.TryGetCertificate(serialNumber, out _))
        {
            return true;
        }

        var merchantKey = merchant.MerchantKey;
        var gate = _gates.GetOrAdd(merchantKey, static _ => new SemaphoreSlim(1, 1));

        // 闸②：同商户单飞。抢不到 ⇒ 已有刷新在飞，等它完成后复查（不叠加下载）。
        if (!await gate.WaitAsync(0, cancellationToken).ConfigureAwait(false))
        {
            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            gate.Release();
            return _store.TryGetCertificate(serialNumber, out _);
        }

        try
        {
            // 双检：拿到锁时同批刷新可能已把该序列号写入。
            if (_store.TryGetCertificate(serialNumber, out _))
            {
                return true;
            }

            // 闸③：最小刷新间隔（**在下载之前**生效 ⇒ 持续失败也不会被反复打）。
            var nowTicks = DateTimeOffset.UtcNow.UtcTicks;
            if (_nextAllowedTicks.TryGetValue(merchantKey, out var nextAllowed) && nowTicks < nextAllowed)
            {
                _logger?.LogWarning(
                    "平台证书刷新处于最小间隔内（{Interval}s），本次不再下载，仍按未知序列号拒绝。" +
                    "MerchantKey: {MerchantKey}, Serial: {Serial}",
                    MinRefreshIntervalSeconds, merchantKey, serialNumber);
                return false;
            }

            _nextAllowedTicks[merchantKey] = nowTicks + TimeSpan.FromSeconds(MinRefreshIntervalSeconds).Ticks;

            return await DownloadAndStoreAsync(merchant, serialNumber, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // 取消是控制流而非失败：原样上抛（调用方据此结束请求而不写应答）。
            throw;
        }
        catch (Exception ex)
        {
            // fail-closed：刷新本身失败一律返回 false，由调用方按「未知序列号」拒绝（绝不放行）。
            _logger?.LogWarning(ex,
                "平台证书刷新失败，本次仍按未知序列号拒绝。MerchantKey: {MerchantKey}, Serial: {Serial}",
                merchantKey, serialNumber);
            return false;
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>拉取证书列表、解密并写入写端口，最后经<b>读端口</b>复查目标序列号。</summary>
    private async Task<bool> DownloadAndStoreAsync(
        WechatPayMerchantConfig merchant, string serialNumber, CancellationToken cancellationToken)
    {
        var merchantKey = merchant.MerchantKey;

        if (_writer is null)
        {
            // 宿主自定义了只读证书存储 ⇒ SDK 无处写。点名告警一次规划：避免宿主以为「刷新没生效」是偶发。
            _logger?.LogWarning(
                "未注册 IWechatPayPlatformCertificateWriter（宿主自定义了证书存储）⇒ 无法按需刷新平台证书。" +
                "请由宿主的证书存储自行完成轮换，或同时实现写入端口。MerchantKey: {MerchantKey}, Serial: {Serial}",
                merchantKey, serialNumber);
            return false;
        }

        var response = await _certificates
            .GetPlatformCertificatesAsync(cancellationToken)
            .ConfigureAwait(false);

        var items = response?.Data;
        if (items is null || items.Count == 0)
        {
            _logger?.LogWarning(
                "平台证书列表为空（无可用证书）。MerchantKey: {MerchantKey}, Serial: {Serial}",
                merchantKey, serialNumber);
            return false;
        }

        var apiKey = await _credentials
            .LoadApiKeyAsync(merchant, cancellationToken)
            .ConfigureAwait(false);

        try
        {
            var added = 0;
            foreach (var item in items)
            {
                if (string.IsNullOrWhiteSpace(item.SerialNumber))
                {
                    continue;
                }

                // 已有同序列号 ⇒ 跳过（省一次解密；覆盖写入没有语义收益，且会提前释放旧实例）。
                if (_store.TryGetCertificate(item.SerialNumber, out _))
                {
                    continue;
                }

                var cipher = item.EncryptCertificate;
                if (cipher is null
                    || !string.Equals(cipher.Algorithm, WechatPayAesGcmCodec.Algorithm, StringComparison.Ordinal))
                {
                    continue;
                }

                // 官方线格式：nonce 为 12 个 ASCII 字符、ciphertext 为 Base64(密文‖tag)。
                if (!WechatPayAesGcmCodec.TryDecryptOfficialPayload(
                        apiKey, cipher.Nonce, cipher.Ciphertext, cipher.AssociatedData, out var pem)
                    || string.IsNullOrWhiteSpace(pem))
                {
                    continue;
                }

                X509Certificate2 certificate;
                try
                {
                    certificate = X509Certificate2.CreateFromPem(pem!);
                }
                catch (CryptographicException)
                {
                    // 解密成功但非合法 PEM：单条跳过即可（不因一本坏证书放弃整批）。
                    continue;
                }

                _writer.Set(item.SerialNumber!, certificate);
                added++;
            }

            _logger?.LogInformation(
                "平台证书刷新完成：列表 {Total} 本，新增 {Added} 本。MerchantKey: {MerchantKey}, Serial: {Serial}",
                items.Count, added, merchantKey, serialNumber);

            // 复查必须走**读端口**（而非写端口）：宿主可能替换了读端口，此时写进默认缓存
            // 与验签读的存储不是同一份 ⇒ 这里会判 false（fail-closed）而非误报成功。
            if (_store.TryGetCertificate(serialNumber, out _))
            {
                return true;
            }

            _logger?.LogWarning(
                "平台证书刷新后仍找不到序列号 {Serial}（可能该序列号不属于本商户，或宿主的证书存储未实现写入）。" +
                "MerchantKey: {MerchantKey}",
                serialNumber, merchantKey);
            return false;
        }
        finally
        {
            // 密钥材料驻留内存的时间窗收敛到本次刷新。
            CryptographicOperations.ZeroMemory(apiKey);
        }
    }
}
