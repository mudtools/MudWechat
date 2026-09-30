// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调加解密测试：SHA1 验签、AES-256-CBC 解密/加密回环（详细设计 §12）。
/// </summary>
public class WechatCallbackCryptoTests
{
    // 43 位 EncodingAESKey（Base64(32 字节) 去 '='）。
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string Token = "push-token";
    private const string ReceiveId = "ww-corp";

    private static string EncryptBody(string plainXml)
        => WechatCallbackCrypto.Encrypt(AesKey, plainXml, ReceiveId);

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

    [Fact]
    public void Decrypt_ShouldThrow_WhenKeyInvalid()
    {
        var act = () => WechatCallbackCrypto.Decrypt("short-key", "c2FtcGxl");
        act.Should().Throw<InvalidOperationException>().WithMessage("*43*");
    }

    [Fact]
    public void Decrypt_ShouldThrow_WhenCipherCorrupted()
    {
        var encrypted = EncryptBody("<xml/>");
        var corrupted = (encrypted[0] == 'A' ? "B" : "A") + encrypted[1..];

        var act = () => WechatCallbackCrypto.Decrypt(AesKey, corrupted);
        act.Should().Throw<InvalidOperationException>().WithMessage("*解密失败*");
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
    public void ParseSignatureQuery_ShouldTolerate_EmptyAndExtraParams()
    {
        var (signature, timestamp, nonce) = WechatCallbackCrypto.ParseSignatureQuery("a=1&nonce=n1");
        signature.Should().BeNull();
        timestamp.Should().BeNull();
        nonce.Should().Be("n1");

        var empty = WechatCallbackCrypto.ParseSignatureQuery("");
        empty.Signature.Should().BeNull();
    }
}
