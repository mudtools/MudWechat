// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using Mud.HttpUtils;
using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// <see cref="IWechatPayMerchantCredentialProvider"/> 的默认实现：经组件 <c>ISecretProvider</c> 取用密钥。
/// </summary>
/// <remarks>
/// <b>不注册默认 <c>ISecretProvider</c></b>：该端口属组件 <c>Mud.HttpUtils</c>，本仓不提供实现，
/// 由宿主按自身密钥管理（环境变量 / KeyVault / 配置中心加密段 / HSM 适配）注入。
/// SDK 抢占注册会削弱宿主的密钥治理，且与 AGENTS「SDK 侧不得抢占组件注册」一致。
/// </remarks>
public sealed class WechatPayMerchantCredentialProvider : IWechatPayMerchantCredentialProvider
{
    /// <summary>APIv3 密钥字节长度（官方固定 32 字节）。</summary>
    public const int ApiKeySizeBytes = 32;

    private readonly ISecretProvider _secrets;

    /// <summary>创建凭据取用器。</summary>
    /// <param name="secrets">组件密钥端口。</param>
    /// <exception cref="ArgumentNullException"><paramref name="secrets"/> 为 <c>null</c> 时抛出。</exception>
    public WechatPayMerchantCredentialProvider(ISecretProvider secrets)
        => _secrets = secrets ?? throw new ArgumentNullException(nameof(secrets));

    /// <inheritdoc />
    public async Task<RSA> LoadPrivateKeyAsync(
        WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        var pem = await ResolveSecretAsync(
                config, config.PrivateKeySecretName, nameof(config.PrivateKeySecretName), cancellationToken)
            .ConfigureAwait(false);

        var rsa = RSA.Create();
        try
        {
            rsa.ImportFromPem(pem);
        }
        catch (Exception ex)
        {
            // 异常消息只带商户键与异常类型名，**绝不回显 PEM**（PAY-B7）。
            rsa.Dispose();
            throw new InvalidOperationException(
                $"{config.MerchantKey}：商户私钥 PEM 解析失败（{ex.GetType().Name}）。" +
                "请确认密钥为 PKCS#8 / PKCS#1 / X.509 单段 PEM 且为 RSA-2048。",
                ex);
        }

        return rsa;
    }

    /// <inheritdoc />
    public async Task<byte[]> LoadApiKeyAsync(
        WechatPayMerchantConfig config, CancellationToken cancellationToken = default)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        var value = await ResolveSecretAsync(
                config, config.ApiKeySecretName, nameof(config.ApiKeySecretName), cancellationToken)
            .ConfigureAwait(false);

        // APIv3 密钥是「32 字节」而非「32 个字符」：按 UTF-8 编码后取字节。
        // 官方要求密钥为 32 字节；长度不符时 AES-GCM 侧只能抛 CryptographicException，
        // 那里的错误信息不指名道姓，故在此用配置语境把它说清楚。
        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length != ApiKeySizeBytes)
        {
            throw new InvalidOperationException(
                $"{config.MerchantKey}：APIv3 密钥长度为 {bytes.Length} 字节，必须为 {ApiKeySizeBytes} 字节。" +
                "密钥是 32 字节（不是 32 字符），请核对配置中心中该密钥名对应的实际值。");
        }

        return bytes;
    }

    /// <summary>经 <c>ISecretProvider</c> 取密钥值，查无即 fail-fast（不静默返回空串）。</summary>
    /// <remarks>
    /// 查无密钥名的典型成因是「把密钥值本身填进了 <c>*SecretName</c> 字段」——
    /// 此时 <paramref name="secretName"/> 会被原样拼进异常消息，<b>等于把密钥洒进日志</b>。
    /// 故异常消息一律<b>截断</b>该值（前 24 字符 + 省略号），既够定位又不至于泄密。
    /// </remarks>
    private async Task<string> ResolveSecretAsync(
        WechatPayMerchantConfig config, string secretName, string propertyName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(secretName))
        {
            throw new InvalidOperationException($"{config.MerchantKey}：{propertyName} 不能为空。");
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

    /// <summary>诊断文本截断（防止把疑似密钥原文的值整段写进异常消息）。</summary>
    private static string Truncate(string value)
        => value.Length <= 24 ? value : value.Substring(0, 24) + "...";
}
