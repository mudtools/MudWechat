// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// GET URL 验证（EchoAsync）协议测试（v1 方案 §5.1 / §8）：官方 echostr 加密回显流程、
/// 复用 VerifySignature+Decrypt、echo 不消费抗重放指纹（v1.2 D8）、时效窗口闸 fail-closed。
/// </summary>
public class WechatCallbackEchoTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string Token = "push-token";
    private const string CorpId = "ww-corp";
    private const string AppKey = WechatCallbackOptions.WildcardAppKey;

    private static string Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    private static WechatCallbackReceiver CreateReceiver(string receiveId = CorpId)
        => new(new TestOptionsMonitor<WechatCallbackOptions>(new WechatCallbackOptions
        {
            Apps =
            {
                [AppKey] = new WechatAppCallbackOptions
                {
                    PushToken = Token,
                    PushEncodingAESKey = AesKey,
                    CorpId = receiveId,
                },
            },
        }));

    /// <summary>构造加密 echostr 与其合法查询串（官方 90930：echostr 充当 encrypt 参与签名）。</summary>
    private static (string Query, string ExpectedPlain) BuildEchoQuery(string plain)
    {
        var echoStr = WechatCallbackCrypto.Encrypt(AesKey, plain, CorpId);
        var timestamp = Now();
        var nonce = "echo-nonce-" + Guid.NewGuid().ToString("N");
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, echoStr);
        return ($"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}&echostr={Uri.EscapeDataString(echoStr)}", plain);
    }

    [Fact]
    public async Task EchoAsync_ShouldReturnDecryptedPlainEcho()
    {
        var receiver = CreateReceiver();
        var (query, expected) = BuildEchoQuery("1234567890");

        var echo = await receiver.EchoAsync(AppKey, query);

        echo.Should().Be(expected, "回显明文 = echostr 解密结果（Msg 段）");
    }

    [Fact]
    public async Task EchoAsync_ShouldReturnPlain_WithoutBomOrQuotesOrNewline()
    {
        // 协议级约束：回显不加引号、不带 BOM、不带换行（1 秒内交由集成侧覆盖）。
        var receiver = CreateReceiver();
        var (query, _) = BuildEchoQuery("plain-echo");

        var echo = await receiver.EchoAsync(AppKey, query);

        echo.Should().NotStartWith("\"").And.NotEndWith("\"");
        echo.Should().NotContain("\r").And.NotContain("\n");
        var bytes = System.Text.Encoding.UTF8.GetBytes(echo);
        bytes[0].Should().NotBe(0xEF, "UTF-8 BOM 前导字节不得出现");
    }

    [Fact]
    public async Task EchoAsync_ShouldSucceedTwice_WithSameParameters()
    {
        // D8 回归：echo 为幂等验证，不消费抗重放指纹——同一 echostr 二次验证（管理员再点保存）必须成功。
        var receiver = CreateReceiver();
        var (query, _) = BuildEchoQuery("repeat-echo");

        await receiver.EchoAsync(AppKey, query);
        var act = async () => await receiver.EchoAsync(AppKey, query);

        await act.Should().NotThrowAsync("echo 不参与指纹去重（v1.2 D8）");
    }

    [Fact]
    public async Task EchoAsync_ShouldThrow_WhenSignatureMismatch()
    {
        var receiver = CreateReceiver();
        var echoStr = WechatCallbackCrypto.Encrypt(AesKey, "x", CorpId);
        var badQuery = $"msg_signature=0000000000000000000000000000000000000000&timestamp={Now()}&nonce=n&echostr={Uri.EscapeDataString(echoStr)}";

        var act = async () => await receiver.EchoAsync(AppKey, badQuery);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*msg_signature 不匹配*");
    }

    [Fact]
    public async Task EchoAsync_ShouldThrow_WhenEchoStrMissing()
    {
        var receiver = CreateReceiver();
        var act = async () => await receiver.EchoAsync(AppKey, $"msg_signature=abc&timestamp={Now()}&nonce=n");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*echostr*");
    }

    [Fact]
    public async Task EchoAsync_ShouldReject_WhenTimestampExpired()
    {
        var receiver = CreateReceiver();
        var echoStr = WechatCallbackCrypto.Encrypt(AesKey, "x", CorpId);
        var stale = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (WechatCallbackReceiver.ReplayWindowSeconds + 1)).ToString();
        var signature = WechatCallbackCrypto.ComputeSignature(Token, stale, "n", echoStr);

        var act = async () => await receiver.EchoAsync(
            AppKey, $"msg_signature={signature}&timestamp={stale}&nonce=n&echostr={Uri.EscapeDataString(echoStr)}");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*超出时效窗口*");
    }

    [Fact]
    public async Task EchoAsync_ShouldReject_WhenReceiveIdMismatched()
    {
        var receiver = CreateReceiver(receiveId: "ww-other");
        var echoStr = WechatCallbackCrypto.Encrypt(AesKey, "x", CorpId);
        var timestamp = Now();
        var nonce = "n";
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, echoStr);

        var act = async () => await receiver.EchoAsync(
            AppKey, $"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}&echostr={Uri.EscapeDataString(echoStr)}");

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*receiveid*");
    }

    [Fact]
    public async Task EchoAsync_ShouldUsePerAppCredentials()
    {
        // 多应用凭据选择：echo 同样按传入 appKey 解析凭据（精确键优先）。
        const string tokenApp1 = "token-for-app1";
        var options = new WechatCallbackOptions
        {
            Apps =
            {
                ["app1"] = new WechatAppCallbackOptions { PushToken = tokenApp1, PushEncodingAESKey = AesKey, CorpId = "ww-corp1" },
            },
        };
        var receiver = new WechatCallbackReceiver(new TestOptionsMonitor<WechatCallbackOptions>(options));

        var echoStr = WechatCallbackCrypto.Encrypt(AesKey, "echo-app1", "ww-corp1");
        var timestamp = Now();
        var nonce = "n";
        var signature = WechatCallbackCrypto.ComputeSignature(tokenApp1, timestamp, nonce, echoStr);

        var echo = await receiver.EchoAsync(
            "app1", $"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}&echostr={Uri.EscapeDataString(echoStr)}");

        echo.Should().Be("echo-app1");
    }
}
