// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Crypto;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>支付载荷编解码分派器（按 <c>resource.algorithm</c> 选择 AES-GCM / SM4-GCM）。</summary>
/// <remarks>
/// <para><b>唯一咽喉点</b>：全仓仅两处调用此分派器——<c>WechatPayCallbackReceiver</c>（回调通知解密）与
/// <c>WechatPayPlatformCertificateRefresher</c>（平台证书下载 cipher 解密）。</para>
/// <para><b>分派逻辑</b>：<c>AEAD_AES_256_GCM</c> → <see cref="AesGcmPayloadCodec"/>；<c>SM4_GCM</c> → <see cref="Sm4GcmPayloadCodec"/>（经 <see cref="IWechatSmCryptoProvider"/>）。</para>
/// </remarks>
public sealed class WechatPayPayloadCodecResolver
{
    private readonly IWechatPayPayloadCodec _aesCodec;
    private readonly IWechatPayPayloadCodec? _sm4Codec;

    /// <summary>创建分派器。</summary>
    /// <param name="aesCodec">AES-GCM 实现（必选）。</param>
    /// <param name="sm4Codec">SM4-GCM 实现（可选；未注册国密时为 null）。</param>
    public WechatPayPayloadCodecResolver(IWechatPayPayloadCodec aesCodec, IWechatPayPayloadCodec? sm4Codec = null)
    {
        _aesCodec = aesCodec ?? throw new ArgumentNullException(nameof(aesCodec));
        _sm4Codec = sm4Codec;
    }

    /// <summary>
    /// 按算法分派解密。
    /// </summary>
    /// <param name="algorithm">算法标识（<c>AEAD_AES_256_GCM</c> / <c>SM4_GCM</c>）。</param>
    /// <param name="apiV3Key">APIv3 密钥（32 字节；SM4-GCM 时会内部派生为 16 字节）。</param>
    /// <param name="nonce">随机数（12 字节）。</param>
    /// <param name="ciphertext">密文。</param>
    /// <param name="associatedData">附加认证数据。</param>
    /// <param name="tag">认证标签（16 字节）。</param>
    /// <returns>解密后的明文字节。</returns>
    /// <exception cref="NotSupportedException"><paramref name="algorithm"/> 为 <c>SM4_GCM</c> 但未注册 <see cref="IWechatSmCryptoProvider"/> 时抛出。</exception>
    /// <exception cref="CryptographicException">解密失败或 tag 校验失败时抛出。</exception>
    public byte[] TryDecrypt(string algorithm, byte[] apiV3Key, byte[] nonce, byte[] ciphertext, byte[] associatedData, byte[] tag)
    {
        if (algorithm == null) throw new ArgumentNullException(nameof(algorithm));

        return algorithm switch
        {
            "AEAD_AES_256_GCM" => _aesCodec.DecryptOfficialPayload(apiV3Key, nonce, ciphertext, associatedData, tag),
            "SM4_GCM" => _sm4Codec != null
                ? _sm4Codec.DecryptOfficialPayload(apiV3Key, nonce, ciphertext, associatedData, tag)
                : throw new NotSupportedException(
                    $"国密算法 SM4_GCM 未实现。请注册 IWechatSmCryptoProvider 真实实现。\n" +
                    $"官方指引：https://pay.weixin.qq.com/docs/merchant/development/shangmi/guide.html"),
            _ => throw new NotSupportedException($"不支持的载荷算法：{algorithm}。仅支持 AEAD_AES_256_GCM / SM4_GCM。")
        };
    }
}
