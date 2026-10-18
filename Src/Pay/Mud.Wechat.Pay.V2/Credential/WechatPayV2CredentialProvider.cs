// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;
using System.Text;
using Mud.HttpUtils;
using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.V2.Credential;

/// <summary>
/// <see cref="IWechatPayV2CredentialProvider"/> 默认实现：经 <c>ISecretProvider</c> 取用 APIv2 密钥与证书。
/// </summary>
public sealed class WechatPayV2CredentialProvider : IWechatPayV2CredentialProvider
{
    /// <summary>APIv2 签名密钥字节长度（官方固定 32，与 APIv3 同长度但<b>不同密钥</b>）。</summary>
    public const int ApiV2KeySizeBytes = 32;

    private readonly ISecretProvider _secrets;

    /// <summary>创建凭据取用器。</summary>
    /// <param name="secrets">组件密钥端口。</param>
    /// <exception cref="ArgumentNullException"><paramref name="secrets"/> 为 <c>null</c> 时抛出。</exception>
    public WechatPayV2CredentialProvider(ISecretProvider secrets)
        => _secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));

    /// <inheritdoc />
    public async Task<string> LoadApiV2KeyAsync(
        WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        var value = await ResolveAsync(config, config.ApiV2KeySecretName, nameof(config.ApiV2KeySecretName))
            .ConfigureAwait(false);

        // 与 APIv3 同一口径：官方说的「32 位」是**字节**，按 UTF-8 编码后判长。
        // 长度不符必须在这里点名 —— 拖到签名阶段症状只是 SIGNERROR，与「密钥装错」毫无关联，无从排查。
        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length != ApiV2KeySizeBytes)
        {
            throw new InvalidOperationException(
                $"{config.MerchantKey}：APIv2 签名密钥长度为 {bytes.Length} 字节，必须为 {ApiV2KeySizeBytes} 字节。" +
                $"请核对 ISecretProvider 中密钥名「{Truncate(config.ApiV2KeySecretName)}」对应的实际值。");
        }

        return value;
    }

    /// <inheritdoc />
    public async Task<X509Certificate2> LoadClientCertificateAsync(
        WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        var material = await ResolveAsync(
            config, config.ApiV2CertificateSecretName, nameof(config.ApiV2CertificateSecretName))
            .ConfigureAwait(false);

        var password = await ResolveCertificatePasswordAsync(config).ConfigureAwait(false);

        var raw = StripPemWrapping(material);
        byte[] pkcs12;
        try
        {
            pkcs12 = Convert.FromBase64String(raw);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                $"{config.MerchantKey}：证书材料不是合法 Base64（{nameof(WechatPayMerchantConfig.ApiV2CertificateSecretName)} " +
                $"「{Truncate(config.ApiV2CertificateSecretName)}」）。应为 apiclient_cert.p12 的 Base64 编码" +
                "（可含 PEM 头尾，本实现会自动剥离）。", ex);
        }

        X509Certificate2 certificate;
        try
        {
#if NET10_0_OR_GREATER
            // net10 起构造器标 SYSLIB0057（密钥会落到用户 profile 目录），官方指定改用 loader。
            certificate = X509CertificateLoader.LoadPkcs12(pkcs12, password, X509KeyStorageFlags.EphemeralKeySet);
#else
            certificate = new X509Certificate2(pkcs12, password, X509KeyStorageFlags.EphemeralKeySet);
#endif
        }
        catch (Exception ex)
        {
            // 口令错误在 Windows/Linux 分别表现为 CryptographicException / 空证书，
            // 消息一律不带材料内容，只给定位线索。
            throw new InvalidOperationException(
                $"{config.MerchantKey}：PKCS#12 证书加载失败（{ex.GetType().Name}）。" +
                "最常见成因是证书口令不对：官方默认口令为商户号，" +
                $"改过口令须经 {nameof(WechatPayMerchantConfig.ApiV2CertificatePasswordSecretName)} 指定密钥名。", ex);
        }

        if (!certificate.HasPrivateKey)
        {
            var disposable = certificate;
            certificate.Dispose();
            throw new InvalidOperationException(
                $"{config.MerchantKey}：证书不含私钥（HasPrivateKey = false），无法用于双向 TLS。" +
                "请确认登记的是 apiclient_cert.p12 而不是导出的公钥证书。");
        }

        return certificate;
    }

    /// <summary>证书口令：配置了口令密钥名则取其密钥值，否则回退官方默认（商户号）。</summary>
    private async Task<string> ResolveCertificatePasswordAsync(WechatPayMerchantConfig config)
    {
        if (string.IsNullOrWhiteSpace(config.ApiV2CertificatePasswordSecretName))
        {
            return config.AuthorizationMchId;
        }

        return await ResolveAsync(
            config, config.ApiV2CertificatePasswordSecretName,
            nameof(config.ApiV2CertificatePasswordSecretName))
            .ConfigureAwait(false);
    }

    /// <summary>经 <c>ISecretProvider</c> 取密钥值，查无即 fail-fast（不静默返回空串）。</summary>
    private async Task<string> ResolveAsync(
        WechatPayMerchantConfig config, string secretName, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(secretName))
        {
            throw new InvalidOperationException(
                $"{config.MerchantKey}：{propertyName} 未配置，该商户不具备对应的 APIv2 能力。");
        }

        var value = await _secrets.GetSecretAsync(secretName).ConfigureAwait(false);

        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException(
                $"{config.MerchantKey}：ISecretProvider 中查无密钥名「{Truncate(secretName)}」（{propertyName}）。" +
                "注意该字段存的是**密钥名**，不是密钥值。");
        }

        return value;
    }

    /// <summary>剥离 PEM 头尾与空白（同一段材料既可登记为纯 Base64，也可登记为 PEM 形态）。</summary>
    private static string StripPemWrapping(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var line in value.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith("-----", StringComparison.Ordinal))
            {
                continue;
            }

            builder.Append(trimmed);
        }

        return builder.ToString();
    }

    /// <summary>诊断文本截断（防止把疑似密钥原文的值整段写进异常消息）。</summary>
    private static string Truncate(string value)
        => value.Length <= 24 ? value : value.Substring(0, 24) + "...";
}
