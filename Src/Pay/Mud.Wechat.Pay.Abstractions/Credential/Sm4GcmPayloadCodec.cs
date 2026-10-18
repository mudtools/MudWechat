// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;
using Mud.Wechat.Abstractions.Crypto;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>APIv3 对称加解密：<c>SM4/GCM/NoPadding</c>（国密），经 <see cref="IWechatSmCryptoProvider"/> 委托实现。</summary>
/// <remarks>
/// <para><b>官方规则</b>：SM4 密钥 = SM3(APIv3 密钥) 摘要（256bit）取前 128bit（16 字节）。</para>
/// <para><see href="https://pay.weixin.qq.com/docs/merchant/development/shangmi/guide.html">APIv3 国密指引</see></para>
/// </remarks>
public sealed class Sm4GcmPayloadCodec : IWechatPayPayloadCodec
{
    /// <summary>官方算法标识。</summary>
    public const string Algorithm = "SM4_GCM";

    /// <summary>SM4 密钥长度：16 字节（128 位）。</summary>
    private const int Sm4KeySizeBytes = 16;

    /// <summary>GCM nonce 长度：12 字节。</summary>
    private const int NonceSizeBytes = 12;

    /// <summary>GCM 认证标签长度：16 字节。</summary>
    private const int TagSizeBytes = 16;

    private readonly IWechatSmCryptoProvider _smProvider;

    /// <inheritdoc />
    string IWechatPayPayloadCodec.Algorithm => Algorithm;

    /// <summary>创建 SM4-GCM 编解码器。</summary>
    /// <param name="smProvider">国密算法提供者（必选）。</param>
    public Sm4GcmPayloadCodec(IWechatSmCryptoProvider smProvider)
        => _smProvider = smProvider ?? throw new ArgumentNullException(nameof(smProvider));

    /// <inheritdoc />
    /// <remarks>
    /// <b>密钥派生</b>：APIv3 密钥（32 字节）→ SM3 摘要 → 取前 16 字节作为 SM4 密钥。
    /// <b>fail-closed</b>：tag 校验失败返回空数组并抛 <c>CryptographicException</c>。
    /// </remarks>
    public byte[] DecryptOfficialPayload(byte[] apiV3Key, byte[] nonce, byte[] ciphertext, byte[] associatedData, byte[] tag)
    {
        if (apiV3Key == null) throw new ArgumentNullException(nameof(apiV3Key));
        if (nonce == null) throw new ArgumentNullException(nameof(nonce));
        if (ciphertext == null) throw new ArgumentNullException(nameof(ciphertext));
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (associatedData == null) throw new ArgumentNullException(nameof(associatedData));

        // 派生 SM4 密钥：SM3(APIv3 Key) 取前 16 字节
        var sm4Key = WechatSm4KeyDerivation.FromApiV3Key(_smProvider, Encoding.UTF8.GetString(apiV3Key));

        if (nonce.Length != NonceSizeBytes)
        {
            throw new CryptographicException($"Nonce 必须为 {NonceSizeBytes} 字节，实际 {nonce.Length} 字节。");
        }

        if (tag.Length != TagSizeBytes)
        {
            throw new CryptographicException($"Tag 必须为 {TagSizeBytes} 字节，实际 {tag.Length} 字节。");
        }

        // 委托国密 provider 解密
        if (!_smProvider.TryDecryptSm4Gcm(sm4Key, nonce, ciphertext, associatedData, tag, out var plaintext))
        {
            throw new CryptographicException("SM4-GCM 解密失败（tag 校验失败）。");
        }

        return plaintext ?? Array.Empty<byte>();
    }
}
