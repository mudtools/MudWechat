// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// URL 验证（echostr，P1-2）与多套件统一注册表（P1-3/D11）测试：
/// 验签→时间窗→解密→receiveid 校验链路、幂等读不消耗指纹（D5）、ToUserName 路由与 fail-closed。
/// </summary>
public class WechatCallbackUrlVerifierAndMultiSuiteTests
{
    private const string AesKeyA = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string AesKeyB = "ZYXWVUTSRQPONMLKJIHGFEDCBAzyxwvutsrqponmlkj";
    private const string TokenA = "token-suite-a";
    private const string TokenB = "token-suite-b";
    private const string SuiteA = "ww-suite-a";
    private const string SuiteB = "ww-suite-b";

    private static string Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    private static string NewNonce() => "nonce-" + Guid.NewGuid().ToString("N");

    /// <summary>经 AddWechatCallbackSuite 登记两个套件并构建服务提供者（公共 DI 面）。</summary>
    private static (ServiceProvider Provider, IWechatCallbackUrlVerifier Verifier, IWechatCallbackReceiver Receiver)
        CreateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddWechatCallbackSuite(o =>
        {
            o.PushToken = TokenA;
            o.PushEncodingAESKey = AesKeyA;
            o.CorpId = SuiteA;
        });
        services.AddWechatCallbackSuite(o =>
        {
            o.PushToken = TokenB;
            o.PushEncodingAESKey = AesKeyB;
            o.CorpId = SuiteB;
        });

        var provider = services.BuildServiceProvider();
        return (provider,
            provider.GetRequiredService<IWechatCallbackUrlVerifier>(),
            provider.GetRequiredService<IWechatCallbackReceiver>());
    }

    private static string QueryFor(string token, string encryptOrEchostr, string? timestamp = null, string? nonce = null)
    {
        timestamp ??= Now();
        nonce ??= NewNonce();
        var signature = WechatCallbackCrypto.ComputeSignature(token, timestamp, nonce, encryptOrEchostr);
        return $"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}";
    }

    private static string EncryptFor(string aesKey, string plainXml, string receiveId)
        => WechatCallbackCrypto.Encrypt(aesKey, plainXml, receiveId);

    // ---------------------------------------------------------------- P1-2 URL 验证

    [Fact]
    public async Task VerifyUrlAsync_ShouldReturnDecryptedMessage()
    {
        var (provider, verifier, _) = CreateProvider();
        using var _provider = provider;

        var echostr = EncryptFor(AesKeyA, "echo-plain-123", SuiteA);
        var plain = await verifier.VerifyUrlAsync(SuiteA, QueryFor(TokenA, echostr), echostr);

        plain.Should().Be("echo-plain-123", "URL 验证应返回解密后的明文（宿主原样返回给企业微信）");
    }

    [Fact]
    public async Task VerifyUrlAsync_ShouldRouteByReceiverId_InMultiSuite()
    {
        var (provider, verifier, _) = CreateProvider();
        using var _provider = provider;

        var echoA = EncryptFor(AesKeyA, "echo-a", SuiteA);
        var echoB = EncryptFor(AesKeyB, "echo-b", SuiteB);

        (await verifier.VerifyUrlAsync(SuiteA, QueryFor(TokenA, echoA), echoA)).Should().Be("echo-a");
        (await verifier.VerifyUrlAsync(SuiteB, QueryFor(TokenB, echoB), echoB)).Should().Be("echo-b",
            "多套件下按 receiverId 分发到对应条目（各自 Token/AESKey）");
    }

    [Fact]
    public async Task VerifyUrlAsync_ShouldConsumeNoFingerprint()
    {
        // D5：URL 验证是幂等读，不做指纹去重——同一 echostr 连续两次验证均成功。
        var (provider, verifier, _) = CreateProvider();
        using var _provider = provider;

        var echostr = EncryptFor(AesKeyA, "echo-repeat", SuiteA);

        var act = async () =>
        {
            await verifier.VerifyUrlAsync(SuiteA, QueryFor(TokenA, echostr), echostr);
            await verifier.VerifyUrlAsync(SuiteA, QueryFor(TokenA, echostr), echostr);
        };

        await act.Should().NotThrowAsync("幂等读不消耗指纹（管理端反复保存重试验证不应假失败）");
    }

    [Fact]
    public async Task VerifyUrlAsync_ShouldReject_WhenSignatureMismatch()
    {
        var (provider, verifier, _) = CreateProvider();
        using var _provider = provider;

        var echostr = EncryptFor(AesKeyA, "echo-x", SuiteA);
        var act = async () => await verifier.VerifyUrlAsync(
            SuiteA, $"msg_signature={new string('0', 40)}&timestamp={Now()}&nonce={NewNonce()}", echostr);

        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*msg_signature 不匹配*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    [Fact]
    public async Task VerifyUrlAsync_ShouldReject_WhenTimestampStale()
    {
        var (provider, verifier, _) = CreateProvider();
        using var _provider = provider;

        var echostr = EncryptFor(AesKeyA, "echo-stale", SuiteA);
        var stale = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (WechatCallbackReceiver.ReplayWindowSeconds + 1)).ToString();
        var act = async () => await verifier.VerifyUrlAsync(SuiteA, QueryFor(TokenA, echostr, timestamp: stale), echostr);

        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*超出时效窗口*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.TimestampOutOfRange);
    }

    [Fact]
    public async Task VerifyUrlAsync_ShouldReject_WhenReceiverIdUnknown()
    {
        var (provider, verifier, _) = CreateProvider();
        using var _provider = provider;

        var act = async () => await verifier.VerifyUrlAsync("ww-unknown-suite", "msg_signature=x&timestamp=1&nonce=n", "echo");

        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*接收方 ID*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.UnknownReceiver, "未注册接收方 ID 一律 fail-closed");
    }

    [Fact]
    public async Task VerifyUrlAsync_ShouldReject_WhenReceiveIdMismatch()
    {
        var (provider, verifier, _) = CreateProvider();
        using var _provider = provider;

        // 明文 receiveid 与 receiverId 不一致 → 条目内 receiveid 校验拒绝。
        var echostr = EncryptFor(AesKeyA, "echo-mismatch", "ww-other-receiveid");
        var act = async () => await verifier.VerifyUrlAsync(SuiteA, QueryFor(TokenA, echostr), echostr);

        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*receiveid*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.ReceiveIdMismatch);
    }

    // ---------------------------------------------------------------- P1-3 多套件 POST 路由

    [Fact]
    public async Task Group_ShouldRouteByToUserName()
    {
        var (provider, _, receiver) = CreateProvider();
        using var _provider = provider;

        var evtA = await ReceiveSuiteTicket(receiver, SuiteA, TokenA, AesKeyA, "ticket-A");
        evtA.SuiteId.Should().Be(SuiteA);
        evtA.SuiteTicket.Should().Be("ticket-A", "套件 A 报文应路由到套件 A 的条目（A 的 Token/AESKey）");

        var evtB = await ReceiveSuiteTicket(receiver, SuiteB, TokenB, AesKeyB, "ticket-B");
        evtB.SuiteId.Should().Be(SuiteB);
        evtB.SuiteTicket.Should().Be("ticket-B", "套件 B 报文应路由到套件 B 的条目（B 的 Token/AESKey）");
    }

    [Fact]
    public async Task Group_ShouldReject_WhenToUserNameUnknown()
    {
        var (provider, _, receiver) = CreateProvider();
        using var _provider = provider;

        // ToUserName 未命中注册表：即使密文合法也 fail-closed 拒绝（配置漂移或探测）。
        var encrypt = EncryptFor(AesKeyA, "<xml><InfoType>suite_ticket</InfoType></xml>", SuiteA);
        var body = $"<xml><ToUserName><![CDATA[ww-unknown]]></ToUserName><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

        var act = async () => await receiver.ReceiveAsync(QueryFor(TokenA, encrypt), body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*接收方 ID*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.UnknownReceiver);
    }

    [Fact]
    public async Task Group_ShouldReject_WhenToUserNameMissing()
    {
        var (provider, _, receiver) = CreateProvider();
        using var _provider = provider;

        var encrypt = EncryptFor(AesKeyA, "<xml><InfoType>suite_ticket</InfoType></xml>", SuiteA);
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

        var act = async () => await receiver.ReceiveAsync(QueryFor(TokenA, encrypt), body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*接收方 ID*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.UnknownReceiver);
    }

    [Fact]
    public async Task Group_ShouldReject_WhenEncryptMissing()
    {
        var (provider, _, receiver) = CreateProvider();
        using var _provider = provider;

        var act = async () => await receiver.ReceiveAsync(
            $"msg_signature=abc&timestamp={Now()}&nonce={NewNonce()}", "<xml><ToUserName>x</ToUserName></xml>");
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*Encrypt*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingEncrypt);
    }

    [Fact]
    public async Task Group_ShouldNotLeakFingerprint_AcrossSuites()
    {
        // 指纹含 token 天然跨套件隔离：同一 nonce/密文在两个套件条目下互不影响。
        var (provider, _, receiver) = CreateProvider();
        using var _provider = provider;

        var encrypt = EncryptFor(AesKeyA, "<xml><InfoType>suite_ticket</InfoType></xml>", SuiteA);
        var body = $"<xml><ToUserName><![CDATA[{SuiteA}]]></ToUserName><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";
        var timestamp = Now();
        var nonce = "shared-nonce";

        await receiver.ReceiveAsync(QueryFor(TokenA, encrypt, timestamp, nonce), body);

        // 套件 B 的 token 下同 nonce 重算签名（不同指纹）→ 正常接收（不因 A 已消费而拒绝）。
        var encryptB = EncryptFor(AesKeyB, "<xml><InfoType>suite_ticket</InfoType></xml>", SuiteB);
        var bodyB = $"<xml><ToUserName><![CDATA[{SuiteB}]]></ToUserName><Encrypt><![CDATA[{encryptB}]]></Encrypt></xml>";
        var evtB = await receiver.ReceiveAsync(QueryFor(TokenB, encryptB, timestamp, nonce), bodyB);
        evtB.IsSuiteTicket.Should().BeTrue("指纹含 token，跨套件天然隔离");
    }

    private static async Task<WechatCallbackEvent> ReceiveSuiteTicket(
        IWechatCallbackReceiver receiver, string suiteId, string token, string aesKey, string ticket)
    {
        var plainXml = $"<xml><SuiteId>{suiteId}</SuiteId><InfoType>suite_ticket</InfoType><SuiteTicket>{ticket}</SuiteTicket></xml>";
        var encrypt = EncryptFor(aesKey, plainXml, suiteId);
        var body = $"<xml><ToUserName><![CDATA[{suiteId}]]></ToUserName><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

        return await receiver.ReceiveAsync(QueryFor(token, encrypt), body);
    }
}
