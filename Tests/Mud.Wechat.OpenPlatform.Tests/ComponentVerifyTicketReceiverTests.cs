// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Callback;

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 票据推送接收器的判定顺序与写入语义锁定。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组最重要的一条是「失败路径绝不写存储」</b>：票据是整条凭证链的根，
/// 若伪造报文能覆盖真票据，攻击者等于「一键关停平台的所有第三方平台接口调用」。
/// 故每条失败用例都<b>额外断言</b>存储仍为空/仍为旧值。
/// </para>
/// <para>
/// 报文由<b>官方同一份算法</b>（<see cref="WechatCallbackCrypto"/>）构造 ——
/// 生产代码复用它、用例也复用它，因此测的是「判定顺序与边界」，而不是自造算法自证。
/// </para>
/// </remarks>
public class ComponentVerifyTicketReceiverTests
{
    private const string Token = "push-token";
    private const string AesKey = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFG"; // 43 位
    private const string AppId = "wx-component";

    /// <summary>合法票据推送 ⇒ 接受，票据写入存储，并应回 success。</summary>
    [Fact]
    public void Receive_ShouldAccept_AndStoreTicket()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(TicketPlainXml("ticket-abc"));

        var outcome = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        outcome.Should().Be(ComponentTicketPushOutcome.Accepted);
        ComponentVerifyTicketReceiver.ShouldReturnSuccess(outcome).Should().BeTrue("官方要求回 success");

        store.TryGet(out var snapshot).Should().BeTrue();
        snapshot!.Ticket.Should().Be("ticket-abc");
    }

    /// <summary>
    /// <b>签名不匹配 ⇒ 拒绝且<b>不写</b>存储</b>（Token 配错或报文被篡改）。
    /// </summary>
    [Fact]
    public void Receive_ShouldRejectInvalidSignature_WithoutWriting()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        // 用**另一个 Token** 签名（模拟 Token 配置不符/伪造）。
        var push = BuildPush(TicketPlainXml("evil-ticket"), token: "wrong-token");

        var outcome = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        outcome.Should().Be(ComponentTicketPushOutcome.InvalidSignature);
        ComponentVerifyTicketReceiver.ShouldReturnSuccess(outcome).Should().BeFalse(
            "验签失败若回 success，微信不再重试、真票据丢失将静默无感");
        store.TryGet(out _).Should().BeFalse("失败路径绝不写存储（否则伪造报文可覆盖真票据）");
    }

    /// <summary>密文不可解密 ⇒ 拒绝且不写（篡改一个字符即触发）。</summary>
    [Fact]
    public void Receive_ShouldRejectTamperedCipher_WithoutWriting()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(TicketPlainXml("ticket-abc"));

        // 篡改密文（同时改签名使其仍能过验签，以单独验证解密失败这条路径）。
        var tampered = MutateEncrypt(push.Encrypt);
        var signature = WechatCallbackCrypto.ComputeSignature(Token, push.Timestamp, push.Nonce, tampered);
        var body = Envelope(tampered);

        var outcome = sut.Receive(signature, push.Timestamp, push.Nonce, body);

        outcome.Should().Be(ComponentTicketPushOutcome.DecryptFailed);
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>缺少签名 / 时间戳 / 随机串 / 包体 / Encrypt ⇒ 一律 MalformedEnvelope。</summary>
    [Theory]
    [InlineData(null, "1", "n")]
    [InlineData("sig", null, "n")]
    [InlineData("sig", "1", null)]
    [InlineData("", "1", "n")]
    public void Receive_ShouldRejectMalformedEnvelope(string? signature, string? timestamp, string? nonce)
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var outcome = sut.Receive(signature, timestamp, nonce, "<xml><AppId>x</AppId></xml>");

        outcome.Should().Be(ComponentTicketPushOutcome.MalformedEnvelope);
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>包体不是合法 XML ⇒ 同样按信封畸形处理（不得抛出）。</summary>
    [Fact]
    public void Receive_ShouldNotThrow_WhenBodyIsNotXml()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var outcome = sut.Receive("sig", "1", "n", "not-xml-at-all");

        outcome.Should().Be(ComponentTicketPushOutcome.MalformedEnvelope);
    }

    /// <summary>
    /// <b>appid 不一致 ⇒ 拒绝且不写</b>：防「同一条密文被搬到另一个平台的接收 URL」的跨平台重放。
    /// </summary>
    [Fact]
    public void Receive_ShouldReject_WhenReceiveIdMismatches()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        // 用**另一个 appid** 作为 receiveId 加密。
        var push = BuildPush(TicketPlainXml("ticket-abc"), receiveId: "wx-other-component");

        var outcome = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        outcome.Should().Be(ComponentTicketPushOutcome.AppIdMismatch);
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>
    /// 验签解密都通过，但 <c>InfoType</c> 不是票据推送 ⇒ <see cref="ComponentTicketPushOutcome.NotTicketPush"/>，
    /// <b>不写</b>票据、且<b>不</b>自动回 success（本线尚未消费该类事件，自动回 success 会静默丢事件）。
    /// </summary>
    [Fact]
    public void Receive_ShouldNotConsume_NonTicketInfoType()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(
            "<xml><AppId>wx-component</AppId><CreateTime>1730000000</CreateTime>"
            + "<InfoType>authorized</InfoType><AuthorizerAppid>wx-authorizer</AuthorizerAppid></xml>");

        var outcome = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        outcome.Should().Be(ComponentTicketPushOutcome.NotTicketPush);
        ComponentVerifyTicketReceiver.ShouldReturnSuccess(outcome).Should().BeFalse(
            "本线尚未建模授权结果事件 —— 自动回 success 会让这些事件静默丢弃，须由宿主显式决定");
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>类型正确但票据为空 ⇒ MissingTicket（不写空票据）。</summary>
    [Fact]
    public void Receive_ShouldReject_WhenTicketMissing()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(
            "<xml><AppId>wx-component</AppId><CreateTime>1730000000</CreateTime>"
            + "<InfoType>component_verify_ticket</InfoType><ComponentVerifyTicket></ComponentVerifyTicket></xml>");

        var outcome = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        outcome.Should().Be(ComponentTicketPushOutcome.MissingTicket);
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>推送凭据缺失/不合法 ⇒ 构造期 fail-fast（不得推迟到首次推送到达）。</summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("too-short")]
    public void Ctor_ShouldFailFast_WhenPushCredentialsInvalid(string? aesKey)
    {
        var config = new OpenPlatformAppConfig
        {
            ComponentAppId = AppId,
            ComponentAppSecret = "secret",
            Token = Token,
            EncodingAesKey = aesKey,
        };

        var act = () => new ComponentVerifyTicketReceiver(
            new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock()), config);

        act.Should().Throw<InvalidOperationException>().WithMessage("*EncodingAesKey*");
    }

    // ---- helpers -------------------------------------------------------------

    private static ComponentVerifyTicketReceiver CreateSut(IComponentVerifyTicketStore store)
        => new(store, new OpenPlatformAppConfig
        {
            ComponentAppId = AppId,
            ComponentAppSecret = "secret",
            Token = Token,
            EncodingAesKey = AesKey,
        });

    private sealed record Push(string Body, string Encrypt, string Signature, string Timestamp, string Nonce);

    private static Push BuildPush(string plainXml, string? token = null, string? receiveId = null)
    {
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, receiveId ?? AppId);
        const string timestamp = "1730000000";
        const string nonce = "nonce-1";
        var signature = WechatCallbackCrypto.ComputeSignature(token ?? Token, timestamp, nonce, encrypt);

        return new Push(Envelope(encrypt), encrypt, signature, timestamp, nonce);
    }

    private static string Envelope(string encrypt)
        => $"<xml><AppId>{AppId}</AppId><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

    private static string TicketPlainXml(string ticket)
        => "<xml><AppId>wx-component</AppId><CreateTime>1730000000</CreateTime>"
           + $"<InfoType>component_verify_ticket</InfoType><ComponentVerifyTicket>{ticket}</ComponentVerifyTicket></xml>";

    /// <summary>把密文的最后一个字符换掉（保持 Base64 字符集），用于制造解密失败。</summary>
    private static string MutateEncrypt(string encrypt)
    {
        var last = encrypt[encrypt.Length - 1];
        var replacement = last == 'A' ? 'B' : 'A';
        return string.Concat(encrypt.AsSpan(0, encrypt.Length - 1), replacement.ToString());
    }
}
