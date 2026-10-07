// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Globalization;
using System.Xml.Linq;
using Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

namespace Mud.Wechat.OfficialAccount.Callback.Tests;

/// <summary>
/// 被动回复写出用例（V9 已核验的六种 <c>MsgType</c> 形态 + 加密回写 + 官方字段名陷阱）。
/// </summary>
public class MpCallbackReplyWriterTests
{
    private const string Token = "test-token";
    private const string AppId = "wxReplyAppId";
    private const string AesKey43 = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

    private static MpAppCallbackOptions App(MpCallbackSecurityMode mode = MpCallbackSecurityMode.Plain)
        => new() { PushToken = Token, PushEncodingAESKey = AesKey43, AppId = AppId, SecurityMode = mode };

    private static MpCallbackEnvelope Envelope()
        => new()
        {
            ToUserName = AppId,
            FromUserName = "oUser",
            TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
            Nonce = "n1",
        };

    private static XElement WriteXml(MpCallbackReply reply, MpCallbackSecurityMode mode)
    {
        var xml = MpCallbackReplyWriter.WriteReply(App(mode), mode, Envelope(), reply, DateTimeOffset.UtcNow);
        return XElement.Parse(xml);
    }

    /// <summary>公共字段方向：回包 <c>ToUserName = 请求 FromUserName</c>（用户 OpenID），反之亦然（官方口径）。</summary>
    [Fact]
    public void CommonFields_ShouldSwapDirection()
    {
        var root = WriteXml(MpCallbackReply.Text("hi"), MpCallbackSecurityMode.Plain);

        root.Element("ToUserName")!.Value.Should().Be("oUser");
        root.Element("FromUserName")!.Value.Should().Be(AppId);
        root.Element("MsgType")!.Value.Should().Be("text");
        root.Element("Content")!.Value.Should().Be("hi");
        root.Element("CreateTime").Should().NotBeNull();
    }

    /// <summary>文本内容含 XML 元字符时须转义（防回包报文破损）。</summary>
    [Fact]
    public void Text_ShouldEscapeXmlMetacharacters()
    {
        var root = WriteXml(MpCallbackReply.Text("a<b>&\"c\""), MpCallbackSecurityMode.Plain);

        root.Element("Content")!.Value.Should().Be("a<b>&\"c\"");
    }

    /// <summary>
    /// 转客服（I1 补齐的第七型）：基本形态仅 MsgType；指定客服形态附 <c>TransInfo/KfAccount</c>
    /// （官方「将消息转发到客服」页两种 XML 形态）。
    /// </summary>
    [Fact]
    public void TransferToCustomerService_ShouldEmitOptionalTransInfo()
    {
        var basic = WriteXml(MpCallbackReply.TransferToCustomerService(), MpCallbackSecurityMode.Plain);
        basic.Element("MsgType")!.Value.Should().Be("transfer_customer_service");
        basic.Element("TransInfo").Should().BeNull("基本形态无专有字段（官方示例仅 4 公共节点）");

        var assigned = WriteXml(
            MpCallbackReply.TransferToCustomerService("test1@test"), MpCallbackSecurityMode.Plain);
        assigned.Element("MsgType")!.Value.Should().Be("transfer_customer_service");
        assigned.Element("TransInfo")!.Element("KfAccount")!.Value.Should().Be("test1@test");
    }

    /// <summary>图片 / 语音：官方容器 <c>Image</c> / <c>Voice</c> 内为 <c>MediaId</c>。</summary>
    [Fact]
    public void ImageAndVoice_ShouldUseOfficialContainers()
    {
        var image = WriteXml(MpCallbackReply.Image("MEDIA-1"), MpCallbackSecurityMode.Plain);
        image.Element("MsgType")!.Value.Should().Be("image");
        image.Element("Image")!.Element("MediaId")!.Value.Should().Be("MEDIA-1");

        var voice = WriteXml(MpCallbackReply.Voice("MEDIA-2"), MpCallbackSecurityMode.Plain);
        voice.Element("MsgType")!.Value.Should().Be("voice");
        voice.Element("Voice")!.Element("MediaId")!.Value.Should().Be("MEDIA-2");
    }

    /// <summary>视频：<c>Video/MediaId</c> 必填；Title/Description 可选（不传则不出现）。</summary>
    [Fact]
    public void Video_ShouldOmitOptionalFieldsWhenAbsent()
    {
        var minimal = WriteXml(MpCallbackReply.Video("MEDIA-3"), MpCallbackSecurityMode.Plain);
        minimal.Element("Video")!.Element("MediaId")!.Value.Should().Be("MEDIA-3");
        minimal.Element("Video")!.Element("Title").Should().BeNull();

        var full = WriteXml(MpCallbackReply.Video("MEDIA-4", "标题", "描述"), MpCallbackSecurityMode.Plain);
        full.Element("Video")!.Element("Title")!.Value.Should().Be("标题");
        full.Element("Video")!.Element("Description")!.Value.Should().Be("描述");
    }

    /// <summary>
    /// 音乐：仅 <c>ThumbMediaId</c> 必填；<b>字段名以 XML 报文拼写 <c>MusicUrl</c> 为准</b>
    /// （官方参数表写作 <c>MusicURL</c>，为已知原文不一致）。
    /// </summary>
    [Fact]
    public void Music_ShouldUseXmlSpellingAndRequireThumb()
    {
        var root = WriteXml(
            MpCallbackReply.Music("THUMB-1", "http://m/lo.mp3", "http://m/hq.mp3", "曲名", "描述"),
            MpCallbackSecurityMode.Plain);
        var music = root.Element("Music")!;

        music.Element("ThumbMediaId")!.Value.Should().Be("THUMB-1");
        music.Element("MusicUrl")!.Value.Should().Be("http://m/lo.mp3");
        music.Element("HQMusicUrl")!.Value.Should().Be("http://m/hq.mp3");
        music.Element("MusicURL").Should().BeNull("协议以 XML 报文拼写为准");

        var act = () => MpCallbackReply.Music(string.Empty);
        act.Should().Throw<ArgumentException>("ThumbMediaId 为官方必填");
    }

    /// <summary>
    /// 图文：<c>ArticleCount</c> + <c>Articles/item</c>（**不是** <c>Article</c>）；条数上限 8（官方另一处为 1 条，无法在 SDK 判定）。
    /// </summary>
    [Fact]
    public void News_ShouldUseArticlesItemAndEnforceLimit()
    {
        var root = WriteXml(
            MpCallbackReply.News(new[]
            {
                new MpNewsArticle("t1", "d1", "http://img/1.png", "http://link/1"),
                new MpNewsArticle("t2", "d2", "http://img/2.png", "http://link/2"),
            }),
            MpCallbackSecurityMode.Plain);

        root.Element("MsgType")!.Value.Should().Be("news");
        root.Element("ArticleCount")!.Value.Should().Be("2");
        var items = root.Element("Articles")!.Elements("item").ToList();
        items.Should().HaveCount(2);
        items[0].Element("Title")!.Value.Should().Be("t1");
        items[0].Element("PicUrl")!.Value.Should().Be("http://img/1.png");
        items[0].Element("Url")!.Value.Should().Be("http://link/1");

        var tooMany = Enumerable.Range(0, 9)
            .Select(i => new MpNewsArticle("t", "d", "p", "u")).ToArray();
        ((Action)(() => MpCallbackReply.News(tooMany))).Should().Throw<ArgumentOutOfRangeException>();
        ((Action)(() => MpCallbackReply.News(Array.Empty<MpNewsArticle>()))).Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>安全模式：回包整体加密（外层 <c>Encrypt/MsgSignature/TimeStamp/Nonce</c>），且明文与密文互为可逆。</summary>
    [Fact]
    public void SafeMode_ShouldEncryptWholeReplyWithVerifiableSignature()
    {
        var envelope = Envelope();
        var xml = MpCallbackReplyWriter.WriteReply(
            App(MpCallbackSecurityMode.Safe), MpCallbackSecurityMode.Safe, envelope,
            MpCallbackReply.Text("encrypted-hello"), DateTimeOffset.UtcNow);
        var root = XElement.Parse(xml);

        var encrypt = root.Element("Encrypt")!.Value;
        var decrypted = WechatCallbackCrypto.Decrypt(AesKey43, encrypt, out var receiveId);
        receiveId.Should().Be(AppId);
        XElement.Parse(decrypted).Element("Content")!.Value.Should().Be("encrypted-hello");

        var expected = WechatCallbackCrypto.ComputeSignature(
            Token, envelope.TimeStamp!, envelope.Nonce!, encrypt);
        root.Element("MsgSignature")!.Value.Should().Be(expected,
            "回包签名 = sha1(sort(token, TimeStamp, Nonce, Encrypt))（官方 F10 口径）");
        root.Element("TimeStamp")!.Value.Should().Be(envelope.TimeStamp);
        root.Element("Nonce")!.Value.Should().Be(envelope.Nonce);
    }

    /// <summary>V8：兼容模式下**明文推送不带 <c>encrypt_type</c>** 也必须被接受（分支判定不得依赖该参数）。</summary>
    [Fact]
    public async Task CompatibleMode_ShouldAcceptPlainTextWithoutEncryptType()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
        const string nonce = "n1";
        var body = "<xml><ToUserName>" + AppId + "</ToUserName><MsgType>text</MsgType><Content>x</Content></xml>";
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce);

        var options = new MpCallbackOptions
        {
            Apps =
            {
                ["mp1"] = new MpAppCallbackOptions
                {
                    PushToken = Token,
                    AppId = AppId,
                    SecurityMode = MpCallbackSecurityMode.Compatible,
                },
            },
        };

        var receiver = new MpCallbackReceiver(
            new TestOptionsMonitor<MpCallbackOptions>(options),
            new InMemoryWechatCallbackReplayGuard());

        // 查询串**刻意不含 encrypt_type**（官方未保证兼容模式明文推送携带该参数）。
        var envelope = await receiver.ReceiveAsync(
            "mp1", "timestamp=" + timestamp + "&nonce=" + nonce + "&signature=" + signature, body);

        envelope.IsEncrypted.Should().BeFalse();
        envelope.EncryptType.Should().BeNull();
        envelope.MsgType.Should().Be("text");
    }
}
