// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;

namespace Mud.Wechat.Pay.V2.Security;

/// <summary>
/// 微信支付 APIv2 <b>敏感字段</b>加密器（小微商户资料修改族的 <c>account_number</c>／
/// <c>mobile_phone</c>／<c>email</c> 上传前的密文由此产出）。
/// </summary>
/// <remarks>
/// <para><b>官方算法</b>（<see href="https://pay.weixin.qq.com/doc/v2/partner/4014115349"/>，
/// 2026-10-18 逐页核验）：字段描述「该字段属于敏感字段，需要进行加密处理」+
/// <c>cert_sn</c> 描述「用于对敏感信息进行加密的平台证书序列号」⇒
/// 密钥是<b>微信支付平台证书</b>的公钥（不是商户证书、不是公钥接口那把），
/// 填充为 <b>RSAES-OAEP + SHA-1 + MGF1(SHA-1)</b>（官方 Java 示例
/// <c>RSA/ECB/OAEPWithSHA-1AndMGF1Padding</c>），输出 <b>Base64</b>。</para>
/// <para><b>为什么是 SHA-1 而不是 SHA-256</b>：这是官方示例代码钉死的口径，
/// 与 APIv3 的 <c>RSAES_OAEP_256</c> 签名口径<b>不同源</b> —— 两处混用会得到对方解不开的密文，
/// 且症状是「参数错误」而不是「加密失败」，最难排。故本类只暴露 OAEP-SHA1 一条路，
/// 不提供可选 padding 参数（可选即误用面）。</para>
/// <para><b>密文与 <c>cert_sn</c> 必须同源</b>：调用方传给本类的证书，其序列号就是请求里要填的
/// <c>cert_sn</c>（取获取平台证书接口应答的 <c>serial_no</c>）⇒
/// 用 A 证书加密却填 B 序列号会直接解密失败。证书可从
/// <c>Mud.Wechat.Pay.Abstractions.Credential.IWechatPayPlatformCertificateStore</c>
/// 按序列号取，该端口与 APIv3 回调验签共用同一份缓存。</para>
/// <para><b>不出明文也不出密文</b>（PAY-B7）：本类不打印、不记录任何入参与结果；
/// 异常消息只说明「缺什么」，绝不回显卡号、手机号、邮箱或其密文。</para>
/// <para><b>AOT 说明</b>：只调 <see cref="X509Certificate2.GetRSAPublicKey()"/> 与
/// <see cref="RSA.Encrypt(byte[], RSAEncryptionPadding)"/>，无反射、无序列化，可安全用于 Native AOT 严格模式。</para>
/// </remarks>
public static class WechatPayV2SensitiveFieldEncrypter
{
    /// <summary>
    /// 用微信支付平台证书公钥加密一个敏感字段。
    /// </summary>
    /// <param name="plaintext">字段明文（银行卡号 / 手机号 / 邮箱）。<b>不得进日志。</b></param>
    /// <param name="platformCertificate">微信支付平台证书（须含公钥；其序列号即请求要填的 <c>cert_sn</c>）。</param>
    /// <returns>OAEP-SHA1 密文的 Base64 文本，可直接作为对应字段值上传。</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="plaintext"/> 为 <c>null</c> 或空白 —— 官方语义下空白明文不是合法待加密值。
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// 证书公钥不是 RSA（微信支付平台证书恒为 RSA，出现非 RSA 说明证书材料配错了）。
    /// </exception>
    public static string Encrypt(string plaintext, X509Certificate2 platformCertificate)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
        {
            throw new ArgumentException("待加密的敏感字段不能为空。", nameof(plaintext));
        }

        if (platformCertificate is null) { throw new ArgumentNullException(nameof(platformCertificate)); }

        if (platformCertificate.GetRSAPublicKey() is not RSA publicKey)
        {
            // 不回显证书主题/序列号之外的任何材料，序列号本身不是机密但异常里也没必要带。
            throw new InvalidOperationException(
                "微信支付平台证书公钥不是 RSA 密钥，无法加密敏感字段。" +
                "请确认传入的是「获取平台证书」接口下发的微信支付平台证书，而不是商户 API 证书（apiclient_cert.p12）。");
        }

        using (publicKey)
        {
            // 官方口径：UTF-8 明文 → RSAES-OAEP(SHA-1, MGF1-SHA-1) → Base64。
            var cipher = publicKey.Encrypt(Encoding.UTF8.GetBytes(plaintext), RSAEncryptionPadding.OaepSHA1);
            return Convert.ToBase64String(cipher);
        }
    }

    /// <summary>
    /// 用 <see cref="IWechatPayPlatformCertificateStore"/> 中指定序列号的平台证书加密一个敏感字段
    /// —— 即「密文与 <c>cert_sn</c> 同源」这条官方要求的机械保证：序列号取不到证书即失败，不会静默用错证书。
    /// </summary>
    /// <param name="plaintext">字段明文。<b>不得进日志。</b></param>
    /// <param name="certificateStore">平台证书存取端口。</param>
    /// <param name="serialNumber">要填进请求 <c>cert_sn</c> 的平台证书序列号。</param>
    /// <returns>OAEP-SHA1 密文的 Base64 文本。</returns>
    /// <exception cref="InvalidOperationException">
    /// 端口按该序列号取不到证书（未同步 / 已轮换）。异常消息不回显序列号对应的证书内容。
    /// </exception>
    public static string Encrypt(
        string plaintext, IWechatPayPlatformCertificateStore certificateStore, string? serialNumber)
    {
        if (certificateStore is null) { throw new ArgumentNullException(nameof(certificateStore)); }

        if (!certificateStore.TryGetCertificate(serialNumber, out var certificate) || certificate is null)
        {
            throw new InvalidOperationException(
                "APIv2 敏感字段加密失败：平台证书存储中没有该 cert_sn 对应的微信支付平台证书（fail-closed，不回落到「任意证书」）。" +
                "请先调用「获取平台证书」接口同步证书，或改用当前有效序列号重新加密。");
        }

        return Encrypt(plaintext, certificate);
    }
}
