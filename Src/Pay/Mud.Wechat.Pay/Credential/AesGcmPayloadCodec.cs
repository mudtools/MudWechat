// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>APIv3 对称加解密：<c>AEAD_AES_256_GCM</c>（PAY-B4），实现 <see cref="IWechatPayPayloadCodec"/> 端口。</summary>
/// <remarks>
/// <para><b>去重裁决（方案 v1 §A.3 #7）</b>：既有 <c>WechatPayAesGcmCodec</c> 的算法体原样迁入此类并实现端口；
/// <c>WechatPayAesGcmCodec.Algorithm</c> 常量改由本类单一提供，全部既有调用点改走端口分派。</para>
/// <para><b>红线</b>：**禁止**把企微 XML 回调的「AES-CBC + 32 字节块 PKCS7」套到这里（方案 v2 §2.6、AGENTS §5.5）。</para>
/// </remarks>
public sealed class AesGcmPayloadCodec : IWechatPayPayloadCodec
{
    /// <summary>官方算法标识（回调 <c>resource.algorithm</c> 的合法取值）。</summary>
    public const string Algorithm = "AEAD_AES_256_GCM";

    /// <summary>APIv3 密钥长度：32 字节（256 位）。</summary>
    private const int KeySizeBytes = 32;

    /// <summary>GCM nonce 长度：12 字节。</summary>
    private const int NonceSizeBytes = 12;

    /// <summary>GCM 认证标签长度：16 字节。</summary>
    private const int TagSizeBytes = 16;

    /// <inheritdoc />
    string IWechatPayPayloadCodec.Algorithm => Algorithm;

    /// <summary>
    /// 构造 AES-GCM 实例：<c>net9.0+</c> 起 <c>AesGcm(byte[])</c> 被标记过时（SYSLIB0053），
    /// 故高版本走带标签长度的构造；<c>net6.0</c>/<c>net8.0</c> 无该重载，回退旧构造；
    /// <c>netstandard2.0</c> 的 BCL 不含该类型，走 <see cref="PayGcmUnavailable"/>（构造即失败）。
    /// </summary>
#if NETSTANDARD2_0
    private static PayGcmUnavailable CreateAesGcm(byte[] key) => new(key);
#elif NET9_0_OR_GREATER
    private static AesGcm CreateAesGcm(byte[] key) => new(key, TagSizeBytes);
#else
    private static AesGcm CreateAesGcm(byte[] key) => new(key);
#endif

    /// <inheritdoc />
    /// <remarks>
    /// <b>fail-closed 且不泄漏</b>（PAY-B4）：认证失败 / 长度非法 / 密钥错误一律返回空数组或抛 <c>CryptographicException</c>，
    /// **绝不静默返回明文**——异常消息里会带上密文字节，等于把 <c>resource.ciphertext</c> 洒进日志与遥测。
    /// </remarks>
    public byte[] DecryptOfficialPayload(byte[] apiV3Key, byte[] nonce, byte[] ciphertext, byte[] associatedData, byte[] tag)
    {
        if (apiV3Key == null) throw new ArgumentNullException(nameof(apiV3Key));
        if (nonce == null) throw new ArgumentNullException(nameof(nonce));
        if (ciphertext == null) throw new ArgumentNullException(nameof(ciphertext));
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (associatedData == null) throw new ArgumentNullException(nameof(associatedData));

        if (apiV3Key.Length != KeySizeBytes)
        {
            throw new CryptographicException($"APIv3 密钥必须为 {KeySizeBytes} 字节，实际 {apiV3Key.Length} 字节。");
        }

        if (nonce.Length != NonceSizeBytes)
        {
            throw new CryptographicException($"Nonce 必须为 {NonceSizeBytes} 字节，实际 {nonce.Length} 字节。");
        }

        if (tag.Length != TagSizeBytes)
        {
            throw new CryptographicException($"Tag 必须为 {TagSizeBytes} 字节，实际 {tag.Length} 字节。");
        }

        // 认证解密：先写入缓冲区，标签校验失败时该缓冲区内容不可信 ⇒ 立即清零后再抛出。
        var buffer = new byte[ciphertext.Length];
        try
        {
            using (var aes = CreateAesGcm(apiV3Key))
            {
                aes.Decrypt(nonce, ciphertext, tag, buffer, associatedData);
            }
        }
        catch (CryptographicException)
        {
            CryptographicOperations.ZeroMemory(buffer);
            throw;
        }
        catch (ArgumentException ex)
        {
            CryptographicOperations.ZeroMemory(buffer);
            throw new CryptographicException("AES-GCM 解密失败（参数非法）。", ex);
        }

        return buffer;
    }
}
