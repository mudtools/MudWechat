// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Callback;
using Mud.Wechat.Work.Abstractions.Callback;

namespace Mud.Wechat.Abstractions.Tests.Callback;

/// <summary>
/// 叶层回调协议与安全内核用例（加解密 / 查询解析 / 抗重放 / 中立信封接口）。
/// </summary>
public class WechatCallbackKernelTests
{
    /// <summary>合法 43 位 EncodingAESKey（Base64 可解 ⇒ 32 字节 AES-256 密钥）。</summary>
    private const string AesKey43 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

    /// <summary>签名三参形态必须与「四参 encrypt 为空」结果一致（GET 回显 / POST 密文同源算法）。</summary>
    [Fact]
    public void ComputeSignature_ThreeArg_ShouldEqualFourArgWithNullEncrypt()
    {
        var three = WechatCallbackCrypto.ComputeSignature("token", "1700000000", "nonce");
        var four = WechatCallbackCrypto.ComputeSignature("token", "1700000000", "nonce", null!);

        three.Should().Be(four);
        three.Should().MatchRegex("^[0-9a-f]{40}$", "SHA1 十六进制小写");
    }

    /// <summary>签名算法按字典序排序参与项（token/timestamp/nonce/encrypt 顺序无关）。</summary>
    [Fact]
    public void ComputeSignature_ShouldBeOrderIndependent()
    {
        var a = WechatCallbackCrypto.ComputeSignature("b-token", "2", "1", "enc");
        var b = WechatCallbackCrypto.ComputeSignature("2", "b-token", "enc", "1");

        a.Should().Be(b, "排序后拼接：字典序与入参顺序无关");
    }

    /// <summary>加解密互操作：32 块 PKCS#7 补位/剥离 + receiveid 往返一致。</summary>
    [Theory]
    [InlineData("hello 公众号")]
    [InlineData("")]
    public void EncryptThenDecrypt_ShouldRoundTrip(string plain)
    {
        var cipher = WechatCallbackCrypto.Encrypt(AesKey43, plain, "wxAppId0001");
        var decrypted = WechatCallbackCrypto.Decrypt(AesKey43, cipher, out var receiveId);

        decrypted.Should().Be(plain);
        receiveId.Should().Be("wxAppId0001");
    }

    /// <summary>密钥长度非法一律 fail-closed（异常面为叶层中立类别 DecryptFailed）。</summary>
    [Fact]
    public void Decrypt_ShouldRejectIllegalKeyOrCipher()
    {
        var shortKey = () => WechatCallbackCrypto.Decrypt("too-short", "AAAA");
        shortKey.Should().Throw<WechatCallbackException>()
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);

        var badBase64 = () => WechatCallbackCrypto.Decrypt(AesKey43, "not-base64!!");
        badBase64.Should().Throw<WechatCallbackException>()
            .Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed);
    }

    /// <summary>查询串解析：前导 <c>?</c>、URL 反转义、大小写不敏感、缺失返回 null。</summary>
    [Fact]
    public void Query_ShouldParseTolerantAndCaseInsensitive()
    {
        var dict = WechatCallbackQuery.Parse("?signature=abc%2B1&Msg_Signature=xyz&nonce=");

        dict["signature"].Should().Be("abc+1", "值须 URL 反转义");
        dict["msg_signature"].Should().Be("xyz", "参数名按 OrdinalIgnoreCase 比较");
        dict["NONCE"].Should().BeEmpty();

        WechatCallbackQuery.Get("?echostr=hi", "echostr").Should().Be("hi");
        WechatCallbackQuery.Get("?echostr=", "echostr").Should().BeNull("空值视为缺失");
        WechatCallbackQuery.Get(null, "echostr").Should().BeNull();
    }

    /// <summary>抗重放：同一指纹仅首次消费成功；空键恒 false（fail-closed 由调用方决定）。</summary>
    [Fact]
    public async Task ReplayGuard_ShouldMarkOnceAndRejectEmptyKey()
    {
        var guard = new InMemoryWechatCallbackReplayGuard();
        var window = TimeSpan.FromMinutes(5);

        (await guard.TryMarkAsync("fingerprint-1", window)).Should().BeTrue();
        (await guard.TryMarkAsync("fingerprint-1", window)).Should().BeFalse("窗口内重复即重放");
        (await guard.TryMarkAsync("fingerprint-2", window)).Should().BeTrue();
        (await guard.TryMarkAsync(string.Empty, window)).Should().BeFalse("空键不得互相覆盖放行");
    }

    /// <summary>中立信封接口：产品线信封实现之，且公共字段可经接口视图读取。</summary>
    [Fact]
    public void EnvelopeInterface_ShouldBeImplementedByProductLineEnvelope()
    {
        IWechatCallbackEnvelope envelope = new WechatCallbackEvent
        {
            MsgType = "event",
            Event = "change_contact",
            FromUserName = "sys",
        };

        envelope.EventTypeKey.Should().Be("change_contact");
        envelope.FromUserName.Should().Be("sys");
        envelope.AppKey.Should().BeNull("归属应用键由接收器在请求期填充（接口视图只暴露只读契约）");
    }
}
