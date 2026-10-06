// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 智能机器人媒体解密器测试（官方 100719 / 101463）：AES-256-CBC + PKCS#7 补至 32 字节倍数，
/// IV 取 key 前 16 字节，媒体密文<b>无</b> <c>random(16) + len(4) + receiveid</c> 头。
/// </summary>
/// <remarks>
/// 加密封装由测试侧<b>独立实现</b>（不复用 SDK 的 <c>WechatCallbackCrypto.Encrypt</c>）——
/// 以「外部消费者视角」校验解密器线上字节流形态，避免实现与断言同源而互相掩盖。
/// </remarks>
public class WechatBotMediaDecryptorTests
{
    /// <summary>43 位 EncodingAESKey 形态的密钥（回调地址模式的媒体密钥即回调 AESKey）。</summary>
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";

    /// <summary>另一把合法的 43 位密钥（用于「密钥与密文不匹配」用例）。</summary>
    private const string WrongAesKey = "ZYXWVUTSRQPONMLKJIHGFEDCBAzyxwvutsrqponmlkj";

    [Fact]
    public void DecryptMediaData_ShouldRoundTrip_WhenOfficial32ByteBlockPadding()
    {
        var plain = BuildPlain(size: 100);

        var cipher = EncryptMedia(AesKey, plain, padBlockSize: 32);

        WechatBotMediaDecryptor.DecryptMediaData(AesKey, cipher)
            .Should().Equal(plain, "官方媒体密文为「明文 + PKCS#7 补至 32 字节倍数」，解密须原样还原");
    }

    [Fact]
    public void DecryptMediaData_ShouldRoundTrip_WhenPlainAlreadyBlockAligned()
    {
        // 明文长度恰为 32 倍数：官方补满一整块（padLen = 32），不得被当成「无填充」处理。
        var plain = BuildPlain(size: 64);

        var cipher = EncryptMedia(AesKey, plain, padBlockSize: 32);

        cipher.Length.Should().Be(96, "补满 32 字节块 ⇒ 密文 = 明文长度 + 32");
        WechatBotMediaDecryptor.DecryptMediaData(AesKey, cipher).Should().Equal(plain);
    }

    [Fact]
    public void DecryptMediaData_ShouldAccept16ByteBlockPadding()
    {
        // 剥离校验界为 [1..32]（官方 32 块协议），故 16 块 PKCS#7 密文同样可正确剥离。
        var plain = BuildPlain(size: 50);

        var cipher = EncryptMedia(AesKey, plain, padBlockSize: 16);

        WechatBotMediaDecryptor.DecryptMediaData(AesKey, cipher).Should().Equal(plain);
    }

    [Fact]
    public void DecryptMediaData_ShouldThrow_WhenCipherDataNull()
    {
        var act = () => WechatBotMediaDecryptor.DecryptMediaData(AesKey, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz0123456789ab")]  // 64 位（长连接 hex 形态未支持）
    public void DecryptMediaData_ShouldThrow_WhenKeyIsNot43Chars(string aesKey)
    {
        var cipher = EncryptMedia(AesKey, BuildPlain(size: 32), padBlockSize: 32);

        var act = () => WechatBotMediaDecryptor.DecryptMediaData(aesKey, cipher);

        act.Should().Throw<InvalidOperationException>().WithMessage("*43 位字符*");
    }

    [Fact]
    public void DecryptMediaData_ShouldThrow_WhenCipherLengthNotBlockAligned()
    {
        var cipher = EncryptMedia(AesKey, BuildPlain(size: 32), padBlockSize: 32);
        var truncated = cipher.AsSpan(0, cipher.Length - 1).ToArray();

        var act = () => WechatBotMediaDecryptor.DecryptMediaData(AesKey, truncated);

        act.Should().Throw<InvalidOperationException>().WithMessage("*密文长度非法*");
    }

    [Fact]
    public void DecryptMediaData_ShouldThrow_WhenCipherEmpty()
    {
        var act = () => WechatBotMediaDecryptor.DecryptMediaData(AesKey, Array.Empty<byte>());

        act.Should().Throw<InvalidOperationException>().WithMessage("*密文长度非法*");
    }

    [Fact]
    public void DecryptMediaData_ShouldThrow_WhenKeyDoesNotMatchCipher()
    {
        var cipher = EncryptMedia(AesKey, BuildPlain(size: 100), padBlockSize: 32);

        var act = () => WechatBotMediaDecryptor.DecryptMediaData(WrongAesKey, cipher);

        act.Should().Throw<InvalidOperationException>("fail-closed：密钥不匹配不得静默返回乱码");
    }

    [Fact]
    public void DecryptMediaData_ShouldThrow_WhenKeyIsNotBase64()
    {
        // 43 位但非法 Base64（含 '!'）⇒ 统一异常面（不泄漏密钥材料）。
        var invalidBase64Key = new string('!', 43);
        var cipher = EncryptMedia(AesKey, BuildPlain(size: 32), padBlockSize: 32);

        var act = () => WechatBotMediaDecryptor.DecryptMediaData(invalidBase64Key, cipher);

        act.Should().Throw<InvalidOperationException>().WithMessage("*Base64*");
    }

    /// <summary>构造长度确定、内容可区分的明文字节（避免全零明文掩盖解密缺陷）。</summary>
    private static byte[] BuildPlain(int size)
    {
        var plain = new byte[size];
        for (var i = 0; i < size; i++)
        {
            plain[i] = (byte)(i * 31 + 7);
        }

        return plain;
    }

    /// <summary>
    /// 独立实现的媒体加密（官方 100719：AES-256-CBC + PKCS#7 补至 32 字节倍数，
    /// Key = Base64Decode(EncodingAESKey + "=")，IV = Key 前 16 字节，<b>无</b>明文头）。
    /// </summary>
    private static byte[] EncryptMedia(string aesKey, byte[] plain, int padBlockSize)
    {
        var key = Convert.FromBase64String(aesKey + "=");
        var padLen = padBlockSize - (plain.Length % padBlockSize);
        var padded = new byte[plain.Length + padLen];
        Buffer.BlockCopy(plain, 0, padded, 0, plain.Length);
        for (var i = plain.Length; i < padded.Length; i++)
        {
            padded[i] = (byte)padLen;
        }

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        aes.Key = key;
        aes.IV = key.AsSpan(0, 16).ToArray();

        using var encryptor = aes.CreateEncryptor();
        return encryptor.TransformFinalBlock(padded, 0, padded.Length);
    }
}
