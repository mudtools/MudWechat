// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Crypto;

/// <summary>国密算法可插拔端口（SM2 / SM3 / SM4-GCM）。</summary>
/// <remarks>
/// <para><b>为何是端口而非内建实现</b>：.NET BCL 不含国密；叶层零第三方依赖是七线共仓的硬边界。</para>
/// <para><b>未注册即 fail-fast</b>：默认 <see cref="NullWechatSmCryptoProvider"/> 一律抛
/// <see cref="NotSupportedException"/>，绝不静默降级为 RSA 或明文。</para>
/// </remarks>
public interface IWechatSmCryptoProvider
{
    /// <summary>SM3 摘要（32 字节）。</summary>
    byte[] Sm3Digest(byte[] data);

    /// <summary>SM3 摘要（16 进制小写，官方 <c>meta.digest</c> 形态）。</summary>
    string Sm3DigestHex(byte[] data);

    /// <summary>SM2 签名（SM2Sign with SM3，<c>RS_ASN1</c>，ID 固定 <c>1234567812345678</c>；返回 Base64）。</summary>
    string SignSm2WithSm3(byte[] merchantPrivateKey, string message);

    /// <summary>SM2 验签（SM2Sign with SM3，<c>RS_ASN1</c>）。</summary>
    bool VerifySm2WithSm3(byte[] platformPublicKey, string message, string signatureBase64);

    /// <summary>SM2 公钥加密（密文格式 <c>C1C3C2_ASN1</c>，返回 Base64）。</summary>
    string EncryptSm2(byte[] publicKey, string plainText);

    /// <summary>SM2 私钥解密（入参 <c>C1C3C2_ASN1</c> 的 Base64）。</summary>
    string DecryptSm2(byte[] merchantPrivateKey, string cipherBase64);

    /// <summary>SM4-GCM 加密（<c>SM4/GCM/NoPadding</c>，key 16 字节、nonce 12 字节、tag 16 字节）。</summary>
    byte[] EncryptSm4Gcm(byte[] key, byte[] nonce, byte[] plain, byte[] associatedData, out byte[] tag);

    /// <summary>SM4-GCM 解密（校验 tag 失败返回 <c>false</c>，不抛异常）。</summary>
    bool TryDecryptSm4Gcm(byte[] key, byte[] nonce, byte[] cipher, byte[] associatedData, byte[] tag, out byte[] plain);
}
