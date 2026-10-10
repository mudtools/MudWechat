// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// <see cref="IWechatPaySignatureProvider"/> 的 RSA-SHA256 实现（BCL 直调、零反射、AOT 安全）。
/// </summary>
/// <remarks>
/// <para>
/// 算法参数照官方：<c>RSA.SignData</c>/<c>VerifyData</c> + <c>SHA256</c> + <c>RSASignaturePadding.Pkcs1</c>。
/// **不得**改成 <c>Pss</c>——官方只接受 PKCS#1 v1.5，换填充线上验签必失败。
/// </para>
/// <para>
/// 验签侧为 **fail-closed**（PAY-B3）：未知序列号、非法 Base64、证书公钥非 RSA、算法异常，全部收敛为
/// <c>false</c>，不抛出、不回显签名原文。**唯一**允许的异常是参数 <c>null</c>（编程错误，由调用方修）。
/// </para>
/// <para>私钥由调用方持有（经组件 <c>ISecretProvider</c> 取用，见 PAY-B7 / 方案 §2.2 第 5 条），本类不留副本。</para>
/// </remarks>
public sealed class WechatPaySignatureProvider : IWechatPaySignatureProvider
{
    private readonly RSA _merchantPrivateKey;
    private readonly string _merchantSerialNumber;
    private readonly IWechatPayPlatformCertificateStore _platformCertificates;

    /// <summary>初始化签名提供器。</summary>
    /// <param name="merchantSerialNumber">商户 API 证书序列号。</param>
    /// <param name="merchantPrivateKey">商户 RSA 私钥（<b>所有权仍归调用方</b>，本类不释放）。</param>
    /// <param name="platformCertificates">平台证书存取端口（验签用）。</param>
    /// <exception cref="ArgumentNullException">任一参数为 <c>null</c>。</exception>
    /// <exception cref="ArgumentException"><paramref name="merchantSerialNumber"/> 为空白。</exception>
    public WechatPaySignatureProvider(
        string merchantSerialNumber,
        RSA merchantPrivateKey,
        IWechatPayPlatformCertificateStore platformCertificates)
    {
        if (string.IsNullOrWhiteSpace(merchantSerialNumber))
        {
            throw new ArgumentException("商户证书序列号不可为空白。", nameof(merchantSerialNumber));
        }

        _merchantSerialNumber = merchantSerialNumber;
        _merchantPrivateKey = merchantPrivateKey ?? throw new ArgumentNullException(nameof(merchantPrivateKey));
        _platformCertificates = platformCertificates ?? throw new ArgumentNullException(nameof(platformCertificates));
    }

    /// <inheritdoc />
    public WechatPaySignatureResult Sign(string message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        byte[] signature = _merchantPrivateKey.SignData(
            Encoding.UTF8.GetBytes(message),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        return new WechatPaySignatureResult(Convert.ToBase64String(signature), _merchantSerialNumber);
    }

    /// <inheritdoc />
    /// <remarks>
    /// 返回 <c>false</c> 的分支是**安全语义**而非「异常处理」：任何解析失败都代表报文不可信，
    /// 一律拒绝并由调用方记录**不含报文内容**的原因（PAY-B7）。
    /// </remarks>
    public bool Verify(string? serialNumber, string? message, string? signature)
    {
        // 三者任一缺失 ⇒ 直接拒绝（缺失即畸形，不得回落到「跳过校验」）。
        if (string.IsNullOrWhiteSpace(serialNumber)
            || message is null
            || string.IsNullOrEmpty(signature))
        {
            return false;
        }

        // 未知序列号 ⇒ 端口查不到 ⇒ 拒绝（PAY-B3；上层可在刷新证书后重试一次）。
        if (!_platformCertificates.TryGetCertificate(serialNumber, out var platformCertificate)
            || platformCertificate is null)
        {
            return false;
        }

        byte[] signatureBytes;
        try
        {
            signatureBytes = Convert.FromBase64String(signature);
        }
        catch (FormatException)
        {
            // 非法 Base64 ⇒ 拒绝，且不回显原文。
            return false;
        }

        if (platformCertificate.GetRSAPublicKey() is not RSA publicKey)
        {
            return false;
        }

        try
        {
            return publicKey.VerifyData(
                Encoding.UTF8.GetBytes(message),
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch (CryptographicException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            // 签名长度与密钥不匹配（如伪造的短签名）在部分运行时以 ArgumentException 抛出。
            return false;
        }
    }

    /// <summary>把时间戳格式化为不变文化秒级字符串（供组装签名串用）。</summary>
    /// <param name="epochSeconds">秒级 Unix 时间戳。</param>
    /// <returns>十进制字符串。</returns>
    /// <remarks>显式提供而非依赖 <c>ToString()</c>：当前区域性可能插入分组分隔符，会让签名逐字节漂移。</remarks>
    public static string FormatTimestamp(long epochSeconds)
        => epochSeconds.ToString(CultureInfo.InvariantCulture);
}
