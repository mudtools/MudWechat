// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Abstractions.Callback;

namespace Mud.Wechat.OpenPlatform.Tests;

/// <summary>
/// 「授权事件接收 URL」推送接收器的判定顺序、分流与写入语义锁定。
/// </summary>
/// <remarks>
/// <para>
/// <b>本组最重要的一条是「失败路径绝不写存储、也绝不交付事件」</b>：
/// 票据是凭证链的根，授权码是换取授权方令牌的钥匙 —— 若伪造报文能覆盖真票据、
/// 或让调用方拿到一个伪造的 <c>authorization_code</c>，攻击者即可冒名接管授权关系。
/// 故每条失败用例都<b>额外断言</b>「存储未被写」与「事件未被交付」。
/// </para>
/// <para>
/// 报文由<b>官方同一份算法</b>（<see cref="WechatCallbackCrypto"/>）构造：
/// 生产代码复用它、用例也复用它 ⇒ 测的是判定顺序与边界，而不是自造算法自证。
/// </para>
/// </remarks>
public class ComponentVerifyTicketReceiverTests
{
    private const string Token = "push-token";
    private const string AesKey = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFG"; // 43 位
    private const string AppId = "wx-component";

    /// <summary>票据推送 ⇒ 接受、写入存储、应回 success。</summary>
    [Fact]
    public void Receive_ShouldAcceptTicket_AndStoreIt()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(TicketXml("ticket-abc"));

        var result = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.Accepted);
        result.InfoType.Should().Be("component_verify_ticket");
        result.ShouldReturnSuccess.Should().BeTrue("官方要求回 success");

        store.TryGet(out var snapshot).Should().BeTrue();
        snapshot!.Ticket.Should().Be("ticket-abc");
    }

    /// <summary>
    /// <b>授权成功 / 更新授权事件</b>：事件随结果<b>同步交付</b>（含授权码与预授权码），
    /// 应回 success，且<b>不</b>写票据存储。
    /// </summary>
    [Theory]
    [InlineData("authorized")]
    [InlineData("updateauthorized")]
    public void Receive_ShouldDeliverAuthorizerEvent_ForAuthorizedAndUpdated(string infoType)
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(
            $"<xml><AppId>{AppId}</AppId><CreateTime>1730000000</CreateTime><InfoType>{infoType}</InfoType>"
            + "<AuthorizerAppid>wx-authorizer</AuthorizerAppid><AuthorizationCode>code-1</AuthorizationCode>"
            + "<AuthorizationCodeExpiredTime>600</AuthorizationCodeExpiredTime>"
            + "<PreAuthCode>pre-1</PreAuthCode></xml>");

        var result = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.AuthorizerEvent);
        result.InfoType.Should().Be(infoType);
        result.ShouldReturnSuccess.Should().BeTrue(
            "事件已同步交付给调用方 ⇒ 可回 success（建模之前不回，是怕静默丢弃）");

        var e = result.AuthorizerEvent;
        e.Should().NotBeNull();
        e!.AuthorizerAppId.Should().Be("wx-authorizer");
        e.AuthorizationCode.Should().Be("code-1", "授权码是换取授权方令牌的钥匙，必须交付");
        e.PreAuthCode.Should().Be("pre-1");
        e.AuthorizationCodeExpiredTime.Should().Be("600", "该字段只做原样透传（官方未言明是剩余秒数还是绝对时间戳）");
        e.IsUnauthorized.Should().BeFalse();

        store.TryGet(out _).Should().BeFalse("授权事件不写票据存储（两者是不同的载荷）");
    }

    /// <summary>取消授权事件：只有 <c>AuthorizerAppid</c>，无授权码；不应再尝试换取令牌。</summary>
    [Fact]
    public void Receive_ShouldDeliverUnauthorizedEvent_WithoutAuthorizationCode()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(
            $"<xml><AppId>{AppId}</AppId><CreateTime>1730000000</CreateTime><InfoType>unauthorized</InfoType>"
            + "<AuthorizerAppid>wx-authorizer</AuthorizerAppid></xml>");

        var result = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.AuthorizerEvent);
        result.ShouldReturnSuccess.Should().BeTrue();

        result.AuthorizerEvent!.IsUnauthorized.Should().BeTrue();
        result.AuthorizerEvent.AuthorizationCode.Should().BeNull("取消授权没有授权码");
    }

    /// <summary>
    /// <b>签名不匹配 ⇒ 拒绝、不写存储、且<b>不交付事件</b></b>
    /// （若验签失败仍把授权码交出去，攻击者可伪造一次「授权成功」骗取令牌）。
    /// </summary>
    [Fact]
    public void Receive_ShouldRejectInvalidSignature_WithoutWritingOrDelivering()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(
            $"<xml><AppId>{AppId}</AppId><InfoType>authorized</InfoType>"
            + "<AuthorizerAppid>wx-evil</AuthorizerAppid><AuthorizationCode>evil-code</AuthorizationCode></xml>",
            token: "wrong-token");

        var result = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.InvalidSignature);
        result.ShouldReturnSuccess.Should().BeFalse(
            "验签失败若回 success，微信不再重试、真推送丢失将静默无感");
        result.AuthorizerEvent.Should().BeNull("验签失败绝不能交付授权码");
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>密文不可解密（篡改一个字符，同时改签名使其仍过验签）⇒ DecryptFailed 且不写。</summary>
    [Fact]
    public void Receive_ShouldRejectTamperedCipher_WithoutWriting()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(TicketXml("ticket-abc"));

        var tampered = MutateEncrypt(push.Encrypt);
        var signature = WechatCallbackCrypto.ComputeSignature(Token, push.Timestamp, push.Nonce, tampered);

        var result = sut.Receive(signature, push.Timestamp, push.Nonce, Envelope(tampered));

        result.Outcome.Should().Be(ComponentTicketPushOutcome.DecryptFailed);
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>缺少签名 / 时间戳 / 随机串，或包体不是合法 XML ⇒ MalformedEnvelope（不得抛出）。</summary>
    [Theory]
    [InlineData(null, "1", "n", "<xml><AppId>x</AppId></xml>")]
    [InlineData("sig", null, "n", "<xml><AppId>x</AppId></xml>")]
    [InlineData("sig", "1", null, "<xml><AppId>x</AppId></xml>")]
    [InlineData("sig", "1", "n", "not-xml-at-all")]
    [InlineData("sig", "1", "n", "<xml><AppId>x</AppId></xml>")]
    public void Receive_ShouldRejectMalformedEnvelope(string? signature, string? timestamp, string? nonce, string body)
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var result = sut.Receive(signature, timestamp, nonce, body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.MalformedEnvelope);
        result.AuthorizerEvent.Should().BeNull();
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>appid 不一致 ⇒ AppIdMismatch 且不写（防跨平台重放）。</summary>
    [Fact]
    public void Receive_ShouldReject_WhenReceiveIdMismatches()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(TicketXml("ticket-abc"), receiveId: "wx-other-component");

        var result = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.AppIdMismatch);
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>票据类型但票据为空 ⇒ MissingTicket（不写空票据）。</summary>
    [Fact]
    public void Receive_ShouldReject_WhenTicketMissing()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(
            $"<xml><AppId>{AppId}</AppId><InfoType>component_verify_ticket</InfoType>"
            + "<ComponentVerifyTicket></ComponentVerifyTicket></xml>");

        var result = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.MissingTicket);
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>
    /// <b>未知 <c>InfoType</c> ⇒ 不交付、不写，且<b>不</b>自动回 success</b>
    /// （保留可观测性：微信会重试若干次后放弃，宿主的监控能看见）。
    /// </summary>
    [Fact]
    public void Receive_ShouldNotAcknowledge_UnknownInfoType()
    {
        var store = new InMemoryComponentVerifyTicketStore(new SystemOpenPlatformClock());
        var sut = CreateSut(store);

        var push = BuildPush(
            $"<xml><AppId>{AppId}</AppId><InfoType>some_future_event</InfoType></xml>");

        var result = sut.Receive(push.Signature, push.Timestamp, push.Nonce, push.Body);

        result.Outcome.Should().Be(ComponentTicketPushOutcome.UnknownInfoType);
        result.InfoType.Should().Be("some_future_event");
        result.AuthorizerEvent.Should().BeNull();
        result.ShouldReturnSuccess.Should().BeFalse(
            "未知类型若回 success 会静默丢弃；不回则微信重试并在监控上可见");
        store.TryGet(out _).Should().BeFalse();
    }

    /// <summary>推送凭据缺失或不合法 ⇒ 构造期 fail-fast。</summary>
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

    private static string TicketXml(string ticket)
        => $"<xml><AppId>{AppId}</AppId><CreateTime>1730000000</CreateTime>"
           + $"<InfoType>component_verify_ticket</InfoType><ComponentVerifyTicket>{ticket}</ComponentVerifyTicket></xml>";

    /// <summary>把密文最后一个字符换掉（保持 Base64 字符集），用于制造解密失败。</summary>
    private static string MutateEncrypt(string encrypt)
    {
        var last = encrypt[encrypt.Length - 1];
        var replacement = last == 'A' ? 'B' : 'A';
        return string.Concat(encrypt.AsSpan(0, encrypt.Length - 1), replacement.ToString());
    }
}
