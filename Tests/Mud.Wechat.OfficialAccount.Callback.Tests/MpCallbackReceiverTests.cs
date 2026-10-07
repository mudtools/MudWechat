// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 公众号回调接收链路用例（三模式 / 双验签 / appid / 抗重放 / 键集合与载荷绑定）。
/// </summary>
/// <remarks>
/// 覆盖方案 §10.2 的用例 1~6、8、10、12 的关键分支（对应守卫 CB-MP-2/3/4 与 CB-INV3）。
/// </remarks>
public class MpCallbackReceiverTests
{
    private const string Token = "test-token";
    private const string AppId = "wxTestCaseAppId";
    private const string AesKey43 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
    private const string AppKey = "mp1";

    /// <summary>构造时间戳（默认取「当前」，避免依赖机器时钟漂移）。</summary>
    private static string Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);

    private static MpCallbackOptions CreateOptions(MpCallbackSecurityMode mode = MpCallbackSecurityMode.Safe)
        => new()
        {
            Apps =
            {
                [AppKey] = new MpAppCallbackOptions
                {
                    PushToken = Token,
                    PushEncodingAESKey = AesKey43,
                    AppId = AppId,
                    SecurityMode = mode,
                },
            },
        };

    private static MpCallbackReceiver CreateReceiver(MpCallbackSecurityMode mode = MpCallbackSecurityMode.Safe)
        => new(
            new TestOptionsMonitor<MpCallbackOptions>(CreateOptions(mode)),
            new InMemoryWechatCallbackReplayGuard(),
            null,
            NullLogger<MpCallbackReceiver>.Instance);

    /// <summary>把消息明文包装为官方密文推送体 <c>&lt;xml&gt;&lt;Encrypt&gt;…</c>。</summary>
    private static (string Body, string Encrypt, string MsgSignature) EncryptMessage(
        string plainXml, string timestamp, string nonce)
    {
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey43, plainXml, AppId);
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, encrypt);
        return ("<xml><ToUserName>" + AppId + "</ToUserName><Encrypt>" + encrypt + "</Encrypt></xml>", encrypt, signature);
    }

    /// <summary>用例 2：安全模式密文推送 → 4 参验签 → 解密 → appid 校验 → 信封字段填充。</summary>
    [Fact]
    public async Task Receive_SafeMode_ShouldDecryptAndFillEnvelope()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var plainXml = "<xml><ToUserName>" + AppId + "</ToUserName><FromUserName>oUser</FromUserName>" +
                       "<CreateTime>1700000000</CreateTime><MsgType>text</MsgType><Content>hello</Content>" +
                       "<MsgId>1234567890</MsgId></xml>";
        var (body, _, msgSignature) = EncryptMessage(plainXml, timestamp, nonce);

        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&msg_signature=" + msgSignature + "&encrypt_type=aes";
        var envelope = await CreateReceiver().ReceiveAsync(AppKey, query, body);

        envelope.AppKey.Should().Be(AppKey);
        envelope.MsgType.Should().Be("text");
        envelope.FromUserName.Should().Be("oUser");
        envelope.MsgId.Should().Be("1234567890");
        envelope.EventTypeKey.Should().Be("text", "事件键 = MsgType（无 Event 时）");
        envelope.IsEncrypted.Should().BeTrue();
        envelope.SecurityMode.Should().Be(MpCallbackSecurityMode.Safe);
    }

    /// <summary>用例 3（关键反例）：<c>signature</c> 正确但 <c>msg_signature</c> 错误 ⇒ 必须拒绝（若误用 signature 验 POST，本用例必红）。</summary>
    [Fact]
    public async Task Receive_ShouldRejectWhenSignatureValidButMsgSignatureWrong()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var plainXml = "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>text</MsgType><Content>hi</Content></xml>";
        var (body, _, _) = EncryptMessage(plainXml, timestamp, nonce);

        var validSignature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce +
                    "&signature=" + validSignature + "&msg_signature=deadbeef";

        var act = async () => await CreateReceiver().ReceiveAsync(AppKey, query, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    /// <summary>用例 4：解密后的 appid 与配置不一致 ⇒ 拒绝（官方要求校验该值是否与自身公众号相符）。</summary>
    [Fact]
    public async Task Receive_ShouldRejectAppIdMismatch()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var plainXml = "<xml><ToUserName>otherAppId</ToUserName><MsgType>text</MsgType></xml>";
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey43, plainXml, "otherAppId");
        var msgSignature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, encrypt);
        var body = "<xml><Encrypt>" + encrypt + "</Encrypt></xml>";
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&msg_signature=" + msgSignature;

        var act = async () => await CreateReceiver().ReceiveAsync(AppKey, query, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.ReceiveIdMismatch);
    }

    /// <summary>用例 6：安全模式收到明文 ⇒ 拒绝（PlainTextRejected）。</summary>
    [Fact]
    public async Task Receive_SafeMode_ShouldRejectPlainText()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var body = "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>text</MsgType><Content>plain</Content></xml>";
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&signature=" + signature;

        var act = async () => await CreateReceiver().ReceiveAsync(AppKey, query, body);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.PlainTextRejected);
    }

    /// <summary>用例 5：明文模式接受明文推送（3 参 signature 验签，无 appid 校验）。</summary>
    [Fact]
    public async Task Receive_PlainMode_ShouldAcceptPlainText()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var body = "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>text</MsgType><Content>plain-ok</Content></xml>";
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&signature=" + signature;

        var envelope = await CreateReceiver(MpCallbackSecurityMode.Plain).ReceiveAsync(AppKey, query, body);

        envelope.IsEncrypted.Should().BeFalse();
        envelope.MsgType.Should().Be("text");
        envelope.FromUserName.Should().BeNull("明文模式无解密环节，字段来自包体解析");
    }

    /// <summary>用例 8：同密文二次进入 ⇒ 指纹闸拒绝（fail-closed）。</summary>
    [Fact]
    public async Task Receive_ShouldRejectReplay()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var plainXml = "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>text</MsgType><Content>x</Content></xml>";
        var (body, _, msgSignature) = EncryptMessage(plainXml, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&msg_signature=" + msgSignature;

        var receiver = CreateReceiver();
        await receiver.ReceiveAsync(AppKey, query, body);

        var act = async () => await receiver.ReceiveAsync(AppKey, query, body);
        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.ReplaySuspected);
    }

    /// <summary>用例 1：GET 回显 —— 3 参验签通过后原样返回 echostr，且不消费指纹（可重复进入）。</summary>
    [Fact]
    public async Task Echo_ShouldReturnEchoStrAndBeIdempotent()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var echoStr = WechatCallbackCrypto.Encrypt(AesKey43, "echo-plain", AppId);
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&signature=" + signature + "&echostr=" + echoStr;

        var receiver = CreateReceiver();
        (await receiver.EchoAsync(AppKey, query)).Should().Be(echoStr);
        (await receiver.EchoAsync(AppKey, query)).Should().Be(echoStr, "GET 回显不消费指纹（平台会反复点提交）");
    }

    /// <summary>用例 1（反例）：GET 验签不匹配 ⇒ 拒绝。</summary>
    [Fact]
    public async Task Echo_ShouldRejectInvalidSignature()
    {
        var query = "timestamp=" + Now() + "&nonce=n1&signature=deadbeef&echostr=abc";

        var act = async () => await CreateReceiver().EchoAsync(AppKey, query);

        (await act.Should().ThrowAsync<WechatCallbackException>())
            .Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    /// <summary>用例 12 相关：键集合下限（7 消息键 + 9 菜单事件键，防静默空跑）。</summary>
    [Fact]
    public void PayloadContracts_ShouldRegisterAllVerifiedKeys()
    {
        var registry = new MpPayloadContractRegistry();
        MpPayloadContracts.RegisterAll(registry);

        registry.RegisteredKeys.Should().HaveCount(20, "本轮可锁定：7 消息键 + 9 菜单事件键 + 4 通用事件键（V1 已核验）");
        registry.RegisteredKeys.Should().Contain(new[]
        {
            MpCallbackEventTypes.Subscribe,
            MpCallbackEventTypes.Unsubscribe,
            MpCallbackEventTypes.Scan,
            MpCallbackEventTypes.Location,
            MpCallbackMessageTypes.Text,
            MpCallbackMessageTypes.Image,
            MpCallbackMessageTypes.Voice,
            MpCallbackMessageTypes.Video,
            MpCallbackMessageTypes.ShortVideo,
            MpCallbackMessageTypes.Location,
            MpCallbackMessageTypes.Link,
            MpCallbackEventTypes.Click,
            MpCallbackEventTypes.View,
            MpCallbackEventTypes.ScanCodePush,
            MpCallbackEventTypes.ScanCodeWaitMsg,
            MpCallbackEventTypes.PicSysPhoto,
            MpCallbackEventTypes.PicPhotoOrAlbum,
            MpCallbackEventTypes.PicWeixin,
            MpCallbackEventTypes.LocationSelect,
            MpCallbackEventTypes.ViewMiniProgram,
        });
    }

    /// <summary>用例 10 相关：文本消息载荷按契约绑定，<c>Content</c> 可读。</summary>
    [Fact]
    public async Task Reader_ShouldBindTextMessageContent()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var plainXml = "<xml><ToUserName>" + AppId + "</ToUserName><FromUserName>oUser</FromUserName>" +
                       "<CreateTime>1700000000</CreateTime><MsgType>text</MsgType><Content>绑定成功</Content>" +
                       "<MsgId>42</MsgId></xml>";
        var (body, _, msgSignature) = EncryptMessage(plainXml, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&msg_signature=" + msgSignature;

        var envelope = await CreateReceiver().ReceiveAsync(AppKey, query, body);

        var registry = new MpPayloadContractRegistry();
        MpPayloadContracts.RegisterAll(registry);
        var reader = new MpCallbackPayloadReader(registry);

        var result = reader.Read<MpTextMessagePayload>(envelope);

        result.Status.Should().Be(MpPayloadReadStatus.Matched);
        result.Payload!.Content.Should().Be("绑定成功");
        result.Payload.EventTypeKey.Should().Be("text");
    }

    /// <summary>用例 10 相关：未登记键降级 <c>GenericCallbackPayload</c> 且不抛。</summary>
    [Fact]
    public async Task Reader_ShouldFallbackForUnregisteredKey()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        // 未登记键用「宿主私有 / 未来官方事件」，避免与已登记键集耦合（键集变动不会让本用例变红）。
        var plainXml = "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>event</MsgType>" +
                       "<Event>custom_private_event</Event><EventKey>some-key</EventKey></xml>";
        var (body, _, msgSignature) = EncryptMessage(plainXml, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&msg_signature=" + msgSignature;

        var envelope = await CreateReceiver().ReceiveAsync(AppKey, query, body);

        var registry = new MpPayloadContractRegistry();
        MpPayloadContracts.RegisterAll(registry);
        var reader = new MpCallbackPayloadReader(registry);

        var result = reader.Read(envelope, typeof(GenericCallbackPayload));

        result.Status.Should().Be(MpPayloadReadStatus.GenericFallback, "未登记契约的事件键按值袋降级（不抛）");
        result.Payload!.Values.Should().ContainKey("EventKey");
    }

    /// <summary>用例 12：XML 安全 —— 含 DTD 的报文不得进入解析（不抛未捕获异常，字段留空）。</summary>
    [Fact]
    public async Task Receive_ShouldTolerateDtdPayload()
    {
        var timestamp = Now();
        var nonce = "nonce-1";
        var plainXml = "<!DOCTYPE xml [<!ENTITY xxe SYSTEM \"file:///etc/passwd\">]>" +
                       "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>text</MsgType></xml>";
        var (body, _, msgSignature) = EncryptMessage(plainXml, timestamp, nonce);
        var query = "timestamp=" + timestamp + "&nonce=" + nonce + "&msg_signature=" + msgSignature;

        var envelope = await CreateReceiver().ReceiveAsync(AppKey, query, body);

        envelope.MsgType.Should().BeNull("DTD 被显式禁用（DtdProcessing.Prohibit），解析失败仅字段留空，不抛异常");
    }
}
