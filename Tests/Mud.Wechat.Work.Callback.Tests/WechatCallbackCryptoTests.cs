// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调加解密测试：SHA1 验签、AES-256-CBC 解密/加密回环（详细设计 §12）、
/// 官方 32 块 PKCS7 填充全边界（P0-1：pad ∈ [1..32]，.NET 内置 PKCS7 的 16 块校验会误拒 pad∈[17..32]）。
/// </summary>
public class WechatCallbackCryptoTests
{
    // 43 位 EncodingAESKey（Base64(32 字节) 去 '='）。
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string Token = "push-token";
    private const string ReceiveId = "ww-corp";

    /// <summary>P0-1 测试矩阵：pad = 1..32 全边界。</summary>
    public static IEnumerable<object[]> PadLengths()
        => Enumerable.Range(1, 32).Select(i => new object[] { i });

    private static string EncryptBody(string plainXml)
        => WechatCallbackCrypto.Encrypt(AesKey, plainXml, ReceiveId);

    /// <summary>按目标 padLen 反推消息长度，使 pad 恰为 padLen（官方规则 padLen = 32 - (20+msg+receiveid)%32）。</summary>
    private static string MessageForPad(int padLen)
    {
        const int receiveLen = 7; // "ww-corp"
        var msgLen = ((((32 - padLen) % 32) - 20 - receiveLen) % 32 + 32) % 32;
        return new string('m', msgLen);
    }

    /// <summary>按官方明文结构构造 rand(16B) + msg_len(4B, 网络序) + msg + receiveid + pad(padLen)，再以 AES-CBC None 加密。</summary>
    private static string EncryptOfficialStyle(int padLen)
    {
        var plain = BuildOfficialPlain(MessageForPad(padLen), ReceiveId, padLen);
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        aes.Key = Convert.FromBase64String(AesKey + "=");
        aes.IV = aes.Key.AsSpan(0, 16).ToArray();
        using var encryptor = aes.CreateEncryptor();
        return Convert.ToBase64String(encryptor.TransformFinalBlock(plain, 0, plain.Length));
    }

    private static byte[] BuildOfficialPlain(string msg, string receiveId, int padLen)
    {
        var msgBytes = Encoding.UTF8.GetBytes(msg);
        var receiveBytes = Encoding.UTF8.GetBytes(receiveId);
        var total = 20 + msgBytes.Length + receiveBytes.Length + padLen;
        var plain = new byte[total];
        RandomNumberGenerator.Fill(plain.AsSpan(0, 16));

        var lenBytes = BitConverter.GetBytes(msgBytes.Length);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(lenBytes);
        }

        lenBytes.CopyTo(plain, 16);
        msgBytes.CopyTo(plain, 20);
        receiveBytes.CopyTo(plain, 20 + msgBytes.Length);
        for (var i = total - padLen; i < total; i++)
        {
            plain[i] = (byte)padLen;
        }

        return plain;
    }

    [Fact]
    public void Decrypt_ShouldRoundtrip_EncryptedMessage()
    {
        var plain = "<xml><InfoType>suite_ticket</InfoType></xml>";
        var encrypted = EncryptBody(plain);

        var decrypted = WechatCallbackCrypto.Decrypt(AesKey, encrypted);
        decrypted.Should().Be(plain, "解密应还原加密前的明文（随机前缀与 receiveid 被剥离）");
    }

    [Fact]
    public void Decrypt_ShouldExposeReceiveId()
    {
        var encrypted = EncryptBody("<xml/>");

        var decrypted = WechatCallbackCrypto.Decrypt(AesKey, encrypted, out var receiveId);

        decrypted.Should().Be("<xml/>");
        receiveId.Should().Be(ReceiveId,
            "P2-5：明文尾部 receiveid 必须可读（供接收器做明文完整性校验；原实现直接丢弃）");
    }

    [Theory]
    [MemberData(nameof(PadLengths))]
    public void Decrypt_ShouldHandleOfficialPadding_PadLengthMatrix(int padLen)
    {
        // P0-1：官方报文 pad ∈ [1..32]（已对齐补满 32），pad∈[17..32] 在 .NET 内置 PKCS7 下必抛。
        var expectedMsg = MessageForPad(padLen);
        var encrypted = EncryptOfficialStyle(padLen);

        var decrypted = WechatCallbackCrypto.Decrypt(AesKey, encrypted, out var receiveId);

        decrypted.Should().Be(expectedMsg,
            $"官方 32 块填充（padLen={padLen}）应被正确剥离");
        receiveId.Should().Be(ReceiveId, "receiveid 恒不得混入尾部填充字节（§4.1.2 次生缺陷）");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(16)]
    [InlineData(31)]
    [InlineData(32)]
    [InlineData(33)]
    [InlineData(100)]
    public void Encrypt_ShouldPadTo32ByteMultiple(int msgLength)
    {
        var encrypted = WechatCallbackCrypto.Encrypt(AesKey, new string('x', msgLength), ReceiveId);

        var cipherLength = Convert.FromBase64String(encrypted).Length;
        (cipherLength % 32).Should().Be(0, "P0-1 ②：输出密文必须严格为 32 字节的倍数（官方契约）");
        cipherLength.Should().BePositive();
    }

    [Fact]
    public void Decrypt_ShouldHandleEmptyReceiveId()
    {
        // 90968：个人主体第三方回调明文 receiveid 为空串。
        var encrypted = WechatCallbackCrypto.Encrypt(AesKey, "<xml/>", string.Empty);

        var decrypted = WechatCallbackCrypto.Decrypt(AesKey, encrypted, out var receiveId);

        decrypted.Should().Be("<xml/>");
        receiveId.Should().BeEmpty("明文未附带 receiveid 时输出空串");
    }

    [Fact]
    public void Decrypt_ShouldThrowWechatCallbackException_WhenKeyInvalid()
    {
        var act = () => WechatCallbackCrypto.Decrypt("short-key", "c2FtcGxl");
        act.Should().Throw<WechatCallbackException>()
            .WithMessage("*43*")
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);
    }

    [Fact]
    public void Decrypt_ShouldWrapFormatException_WhenKeyInvalidBase64()
    {
        // 43 位但含非法 Base64 字符：FormatException 须被包裹为统一异常面（P0-1 ③）。
        var act = () => WechatCallbackCrypto.Decrypt(new string('?', 43), "c2FtcGxl");
        act.Should().Throw<WechatCallbackException>()
            .WithMessage("*Base64*")
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);
    }

    [Fact]
    public void Decrypt_ShouldFailFast_WhenCipherNotBlockAligned()
    {
        // "abcd" 解码为 3 字节（非 16 倍数）；空密文同样前置拒绝。
        var act = () => WechatCallbackCrypto.Decrypt(AesKey, "abcd");
        act.Should().Throw<WechatCallbackException>()
            .WithMessage("*密文长度非法*")
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);

        var actEmpty = () => WechatCallbackCrypto.Decrypt(AesKey, string.Empty);
        actEmpty.Should().Throw<WechatCallbackException>().WithMessage("*密文长度非法*");
    }

    [Fact]
    public void Decrypt_ShouldThrowWechatCallbackException_WhenCipherCorrupted()
    {
        var encrypted = EncryptBody("<xml/>");
        var corrupted = (encrypted[0] == 'A' ? "B" : "A") + encrypted[1..];

        var act = () => WechatCallbackCrypto.Decrypt(AesKey, corrupted);
        act.Should().Throw<WechatCallbackException>()
            .WithMessage("*解密失败*")
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);
    }

    [Fact]
    public void VerifySignature_ShouldMatch_WechatSha1Protocol()
    {
        var encrypt = EncryptBody("<xml/>");
        var timestamp = "1409659813";
        var nonce = "xxx";

        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, encrypt);
        signature.Should().MatchRegex("^[0-9a-f]{40}$", "SHA1 签名为 40 位十六进制小写");
        WechatCallbackCrypto.VerifySignature(Token, timestamp, nonce, encrypt, signature).Should().BeTrue();
    }

    [Fact]
    public void VerifySignature_ShouldFail_WhenAnyParamTampered()
    {
        var encrypt = EncryptBody("<xml/>");
        var signature = WechatCallbackCrypto.ComputeSignature(Token, "1409659813", "xxx", encrypt);

        WechatCallbackCrypto.VerifySignature(Token, "1409659814", "xxx", encrypt, signature).Should().BeFalse("timestamp 被篡改应验签失败");
        WechatCallbackCrypto.VerifySignature("wrong-token", "1409659813", "xxx", encrypt, signature).Should().BeFalse("token 不匹配应验签失败");
        WechatCallbackCrypto.VerifySignature(Token, "1409659813", "xxx", encrypt + "x", signature).Should().BeFalse("密文被篡改应验签失败");
    }

    [Fact]
    public void ParseSignatureQuery_ShouldExtractThreeParams()
    {
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(
            "msg_signature=abc123&timestamp=1409659813&nonce=xyz");

        signature.Should().Be("abc123");
        timestamp.Should().Be("1409659813");
        nonce.Should().Be("xyz");
    }

    [Fact]
    public void ParseSignatureQuery_ShouldTolerateLeadingQuestionMark_AndExtraParams()
    {
        // 评审补充：HttpRequest.QueryString.Value 含前导 '?'。
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery(
            "?msg_signature=abc123&timestamp=1409659813&nonce=xyz&echostr=ENCRYPT");

        signature.Should().Be("abc123");
        timestamp.Should().Be("1409659813");
        nonce.Should().Be("xyz");

        var (emptySignature, _, emptyNonce) = WechatCallbackCrypto.ParseSignatureQuery("a=1&nonce=n1");
        emptySignature.Should().BeNull();
        emptyNonce.Should().Be("n1");

        var empty = WechatCallbackCrypto.ParseSignatureQuery("");
        empty.Signature.Should().BeNull();
    }
}
