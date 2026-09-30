// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调接收与事件分发测试：suite_ticket 入库、change_auth/cancel_auth 清理与级联失效（详细设计 §18.4）。
/// </summary>
public class WechatCallbackReceiverAndHandlerTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string Token = "push-token";
    private const string CorpId = "ww-corp";

    private static (WechatCallbackReceiver Receiver, Func<string, string, string> EncryptedBody) CreateReceiver()
    {
        var receiver = new WechatCallbackReceiver(Microsoft.Extensions.Options.Options.Create(new WechatCallbackOptions
        {
            PushToken = Token,
            PushEncodingAESKey = AesKey,
            CorpId = CorpId,
        }));

        string EncryptAndWrap(string infoType, string extraXml)
        {
            var plainXml = $"<xml><SuiteId>ww-suite</SuiteId><InfoType>{infoType}</InfoType>{extraXml}</xml>";
            var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, CorpId);
            var signature = WechatCallbackCrypto.ComputeSignature(Token, "1409659813", "nonce1", encrypt);
            var body = $"<xml><ToUserName><![CDATA[{CorpId}]]></ToUserName><Encrypt><![CDATA[{encrypt}]]></Encrypt><AgentID><![CDATA[]]></AgentID></xml>";
            return body;
        }

        return (receiver, EncryptAndWrap);
    }

    private static string QueryFor(string encrypt, string timestamp = "1409659813", string nonce = "nonce1")
    {
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, encrypt);
        return $"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}";
    }

    [Fact]
    public async Task ReceiveAsync_ShouldParseSuiteTicketEvent()
    {
        var (receiver, wrap) = CreateReceiver();
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey,
            "<xml><SuiteId>ww-suite</SuiteId><InfoType>suite_ticket</InfoType><SuiteTicket>ticket-abc</SuiteTicket></xml>",
            CorpId);
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";

        var evt = await receiver.ReceiveAsync(QueryFor(encrypt), body);

        evt.IsSuiteTicket.Should().BeTrue();
        evt.SuiteId.Should().Be("ww-suite");
        evt.SuiteTicket.Should().Be("ticket-abc");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenSignatureMismatch()
    {
        var (receiver, _) = CreateReceiver();
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, "<xml><InfoType>suite_ticket</InfoType></xml>", CorpId);
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";
        var badQuery = "msg_signature=0000000000000000000000000000000000000000&timestamp=1409659813&nonce=nonce1";

        var act = async () => await receiver.ReceiveAsync(badQuery, body);
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*验签失败*");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenEncryptMissing()
    {
        var (receiver, _) = CreateReceiver();
        var act = async () => await receiver.ReceiveAsync("msg_signature=abc&timestamp=1&nonce=n", "<xml><Nothing/></xml>");
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Encrypt*");
    }

    [Fact]
    public async Task Handler_ShouldWriteSuiteTicketToStore()
    {
        var store = new InMemoryWechatSuiteTicketStore();
        var handler = new WechatCallbackHandler(
            store, new InMemoryWechatCorpAuthStore(), NullLogger<WechatCallbackHandler>.Instance);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "suite_ticket",
            SuiteId = "ww-suite",
            SuiteTicket = "ticket-1",
        });

        (await store.GetAsync()).Should().Be("ticket-1", "suite_ticket 事件应写入仓储并驱动 get_suite_token");
    }

    [Fact]
    public async Task Handler_ShouldRemoveCorpAuth_AndInvalidateCorpToken_OnCancelAuth()
    {
        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new Abstractions.Authentication.TokenManager.CorpAuth("corp-X", "pc-X"));

        var appManagerMock = new Mock<IWechatAppManager>();
        appManagerMock.Setup(m => m.ConfiguredAppKeys).Returns(new[] { "suite-app" });
        appManagerMock
            .Setup(m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-X"), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), corpAuthStore, NullLogger<WechatCallbackHandler>.Instance, appManagerMock.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            AuthCorpId = "corp-X",
        });

        (await corpAuthStore.GetAsync("corp-X")).Should().BeNull("取消授权后应清理永久授权码");
        appManagerMock.Verify(
            m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-X"), It.IsAny<CancellationToken>()),
            Times.Once, "取消授权应级联失效该企业的授权企业令牌");
    }

    [Fact]
    public async Task Handler_ShouldInvalidateCorpToken_OnChangeAuth()
    {
        var appManagerMock = new Mock<IWechatAppManager>();
        appManagerMock.Setup(m => m.ConfiguredAppKeys).Returns(new[] { "suite-app" });

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), new InMemoryWechatCorpAuthStore(),
            NullLogger<WechatCallbackHandler>.Instance, appManagerMock.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "change_auth",
            AuthCorpId = "corp-Y",
        });

        appManagerMock.Verify(
            m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-Y"), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handler_ShouldIgnoreUnknownInfoType()
    {
        var store = new InMemoryWechatSuiteTicketStore();
        var handler = new WechatCallbackHandler(
            store, new InMemoryWechatCorpAuthStore(), NullLogger<WechatCallbackHandler>.Instance);

        var act = async () => await handler.HandleAsync(new WechatCallbackEvent { InfoType = "unknown_event" });
        await act.Should().NotThrowAsync("未知事件仅记录日志，不抛出");
    }

    [Fact]
    public async Task Handler_ShouldSkipEmptySuiteTicket()
    {
        var store = new InMemoryWechatSuiteTicketStore();
        var handler = new WechatCallbackHandler(
            store, new InMemoryWechatCorpAuthStore(), NullLogger<WechatCallbackHandler>.Instance);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "suite_ticket",
            SuiteId = "ww-suite",
        });

        (await store.GetAsync()).Should().BeNull("SuiteTicket 为空的事件不应覆盖仓储");
    }
}
