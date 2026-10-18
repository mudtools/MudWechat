// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Abstractions.Crypto;

/// <summary>国密算法端口的 fail-fast 默认实现（未注册真实实现时抛出异常）。</summary>
/// <remarks>
/// <para>与既有 <c>InMemoryWechatCallbackReplayGuard</c> / <c>InMemoryWechatTokenStore</c> 同形态：
/// 叶层中立 SPI + 叶层默认实现，但此处语义为「能力缺失」而非「内存兜底」。</para>
/// <para>官方指引：<see href="https://pay.weixin.qq.com/docs/merchant/development/shangmi/guide.html">APIv3 国密指引</see></para>
/// </remarks>
public sealed class NullWechatSmCryptoProvider : IWechatSmCryptoProvider
{
    private const string GuidanceUrl = "https://pay.weixin.qq.com/docs/merchant/development/shangmi/guide.html";
    private const string RegistrationExample = """
        // 在 DI 容器中注册真实实现（以 BouncyCastle 为例）：
        builder.Services.AddSingleton<IWechatSmCryptoProvider, BouncyCastleSmCryptoProvider>();
        // 或引用可选包 Mud.Wechat.Sm.BouncyCastle 后调用：
        builder.AddWechatSmCryptoProvider();
        """;

    private static NotSupportedException CreateException(string operation)
        => new($"国密算法 {operation} 未实现。微信商户需自愿开通国密能力并注册 IWechatSmCryptoProvider 真实实现。\n"
               + $"官方指引：{GuidanceUrl}\n"
               + $"注册示例：\n{RegistrationExample}");

    /// <inheritdoc />
    public byte[] Sm3Digest(byte[] data) => throw CreateException("SM3 摘要");

    /// <inheritdoc />
    public string Sm3DigestHex(byte[] data) => throw CreateException("SM3 摘要（十六进制）");

    /// <inheritdoc />
    public string SignSm2WithSm3(byte[] merchantPrivateKey, string message) => throw CreateException("SM2 签名");

    /// <inheritdoc />
    public bool VerifySm2WithSm3(byte[] platformPublicKey, string message, string signatureBase64)
        => throw CreateException("SM2 验签");

    /// <inheritdoc />
    public string EncryptSm2(byte[] publicKey, string plainText) => throw CreateException("SM2 加密");

    /// <inheritdoc />
    public string DecryptSm2(byte[] merchantPrivateKey, string cipherBase64) => throw CreateException("SM2 解密");

    /// <inheritdoc />
    public byte[] EncryptSm4Gcm(byte[] key, byte[] nonce, byte[] plain, byte[] associatedData, out byte[] tag)
        => throw CreateException("SM4-GCM 加密");

    /// <inheritdoc />
    public bool TryDecryptSm4Gcm(byte[] key, byte[] nonce, byte[] cipher, byte[] associatedData, byte[] tag, out byte[] plain)
        => throw CreateException("SM4-GCM 解密");
}
