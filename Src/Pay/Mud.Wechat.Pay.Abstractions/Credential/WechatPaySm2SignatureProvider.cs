// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text;
using Mud.Wechat.Abstractions.Crypto;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>国密签名提供者（SM2Sign with SM3，官方 <c>WECHATPAY2-SM2-WITH-SM3</c>）。</summary>
/// <remarks>
/// <para><b>官方契约</b>：签名格式 <c>RS_ASN1</c>；签名用 ID = <c>"1234567812345678"</c>（16 字节 ASCII）；签名值 Base64。</para>
/// <para><see href="https://pay.weixin.qq.com/docs/merchant/development/shangmi/guide.html">APIv3 国密指引</see></para>
/// <para><b>前置条件</b>：须注册 <see cref="IWechatSmCryptoProvider"/> 真实实现（如 BouncyCastle）。</para>
/// </remarks>
public sealed class WechatPaySm2SignatureProvider : IWechatPaySignatureProvider
{
    private readonly byte[] _merchantPrivateKey;
    private readonly string _merchantSerialNumber;
    private readonly IWechatPayPlatformCertificateStore _platformCertificates;
    private readonly IWechatSmCryptoProvider _smProvider;

    /// <inheritdoc />
    public string Algorithm => "WECHATPAY2-SM2-WITH-SM3";

    /// <summary>初始化国密签名提供者。</summary>
    /// <param name="merchantSerialNumber">商户 API 证书序列号。</param>
    /// <param name="merchantPrivateKey">商户 SM2 私钥字节（由 <see cref="WechatPayMerchantKey.Sm2PrivateKey"/> 提供）。</param>
    /// <param name="platformCertificates">平台证书存取端口（验签用）。</param>
    /// <param name="smProvider">国密算法提供者。</param>
    public WechatPaySm2SignatureProvider(
        string merchantSerialNumber,
        byte[] merchantPrivateKey,
        IWechatPayPlatformCertificateStore platformCertificates,
        IWechatSmCryptoProvider smProvider)
    {
        if (string.IsNullOrWhiteSpace(merchantSerialNumber))
        {
            throw new ArgumentException("商户证书序列号不可为空白。", nameof(merchantSerialNumber));
        }

        _merchantSerialNumber = merchantSerialNumber;
        _merchantPrivateKey = merchantPrivateKey ?? throw new ArgumentNullException(nameof(merchantPrivateKey));
        _platformCertificates = platformCertificates ?? throw new ArgumentNullException(nameof(platformCertificates));
        _smProvider = smProvider ?? throw new ArgumentNullException(nameof(smProvider));
    }

    /// <inheritdoc />
    public WechatPaySignatureResult Sign(string message)
    {
        if (message is null) throw new ArgumentNullException(nameof(message));

        var signatureBase64 = _smProvider.SignSm2WithSm3(_merchantPrivateKey, message);
        return new WechatPaySignatureResult(signatureBase64, _merchantSerialNumber);
    }

    /// <inheritdoc />
    /// <remarks>
    /// <b>#23 国密验签分派</b>：按 <c>Wechatpay-Signature-Type</c> 头选择本 provider 时，
    /// 从平台证书提取公钥字节后调用 <see cref="IWechatSmCryptoProvider.VerifySm2WithSm3"/>。
    /// fail-closed：未知序列号、非法 Base64、验签失败一律返回 <c>false</c>。
    /// </remarks>
    public bool Verify(string? serialNumber, string? message, string? signature)
    {
        // 三者任一缺失 ⇒ 直接拒绝
        if (string.IsNullOrWhiteSpace(serialNumber) || message is null || string.IsNullOrEmpty(signature))
        {
            return false;
        }

        // 取平台公钥字节（#22 TryGetPublicKey）
        if (!_platformCertificates.TryGetPublicKey(serialNumber, out var publicKey, out var algorithm))
        {
            return false;
        }

        if (publicKey == null || publicKey.Length == 0)
        {
            return false;
        }

        try
        {
            return _smProvider.VerifySm2WithSm3(publicKey, message, signature);
        }
        catch
        {
            // 任何异常 ⇒ 拒绝（fail-closed）
            return false;
        }
    }
}
