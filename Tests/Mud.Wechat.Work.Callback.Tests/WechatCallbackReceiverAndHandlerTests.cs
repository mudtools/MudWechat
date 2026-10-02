// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Configuration;
using Mud.Wechat.Work.Abstractions.Enums;

namespace Mud.Wechat.Work.Callback.Tests;

/// <summary>
/// 回调接收与事件分发测试：抗重放（时效窗口 + 一次性指纹）、指纹闸后移（P1-1）、
/// suite_ticket 入库、cancel_auth/change_auth 的 SuiteId 命中集清理与级联失效（详细设计 §18.4；P0-2 / P0-3）。
/// </summary>
public class WechatCallbackReceiverAndHandlerTests
{
    private const string AesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopq";
    private const string OtherAesKey = "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopR";
    private const string Token = "push-token";
    private const string CorpId = "ww-corp";

    /// <summary>当前时间戳（秒）；回调时间窗为 ±300s，报文必须使用当前时间。</summary>
    private static string Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

    /// <summary>生成唯一 nonce（P0-2 后同一 nonce 的第二次调用会被判为重放）。</summary>
    private static string NewNonce() => "nonce-" + Guid.NewGuid().ToString("N");

    private static WechatCallbackOptions CreateOptions(
        string? aesKey = AesKey, string? token = Token, string? corpId = CorpId)
        => new()
        {
            PushToken = token ?? string.Empty,
            PushEncodingAESKey = aesKey ?? string.Empty,
            CorpId = corpId ?? string.Empty,
        };

    private static (WechatCallbackReceiver Receiver, Func<string, string, string> EncryptedBody) CreateReceiver(
        IWechatCallbackReplayGuard? replayGuard = null, string? aesKey = AesKey)
    {
        var receiver = new WechatCallbackReceiver(CreateOptions(aesKey: aesKey), replayGuard);

        string EncryptAndWrap(string infoType, string extraXml)
        {
            var plainXml = $"<xml><SuiteId>ww-suite</SuiteId><InfoType>{infoType}</InfoType>{extraXml}</xml>";
            var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, CorpId);
            return $"<xml><ToUserName><![CDATA[{CorpId}]]></ToUserName><Encrypt><![CDATA[{encrypt}]]></Encrypt><AgentID><![CDATA[]]></AgentID></xml>";
        }

        return (receiver, EncryptAndWrap);
    }

    private static string QueryFor(string encrypt, string timestamp, string nonce)
    {
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, nonce, encrypt);
        return $"msg_signature={signature}&timestamp={timestamp}&nonce={nonce}";
    }

    private static (string Encrypt, string Body) EncryptTicket(string plainXml)
    {
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, plainXml, CorpId);
        return (encrypt, $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldParseSuiteTicketEvent()
    {
        var (receiver, _) = CreateReceiver();
        var (encrypt, body) = EncryptTicket(
            "<xml><SuiteId>ww-suite</SuiteId><InfoType>suite_ticket</InfoType><SuiteTicket>ticket-abc</SuiteTicket></xml>");

        var evt = await receiver.ReceiveAsync(QueryFor(encrypt, Now(), NewNonce()), body);

        evt.IsSuiteTicket.Should().BeTrue();
        evt.SuiteId.Should().Be("ww-suite");
        evt.SuiteTicket.Should().Be("ticket-abc");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenSignatureMismatch()
    {
        var (receiver, _) = CreateReceiver();
        var (_, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");
        var badQuery = $"msg_signature=0000000000000000000000000000000000000000&timestamp={Now()}&nonce={NewNonce()}";

        var act = async () => await receiver.ReceiveAsync(badQuery, body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*验签失败*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.InvalidSignature);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldThrow_WhenEncryptMissing()
    {
        var (receiver, _) = CreateReceiver();
        var act = async () => await receiver.ReceiveAsync(
            $"msg_signature=abc&timestamp={Now()}&nonce={NewNonce()}", "<xml><Nothing/></xml>");
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*Encrypt*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingEncrypt);
    }

    // ---------------------------------------------------------------- P0-2 抗重放

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampExpired()
    {
        var (receiver, _) = CreateReceiver();
        var (encrypt, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");
        var stale = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (WechatCallbackReceiver.ReplayWindowSeconds + 1)).ToString();

        var act = async () => await receiver.ReceiveAsync(QueryFor(encrypt, stale, NewNonce()), body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*超出时效窗口*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.TimestampOutOfRange);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldPass_WhenTimestampWithinWindow()
    {
        var (receiver, _) = CreateReceiver();
        var (encrypt, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");
        var nearBoundary = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (WechatCallbackReceiver.ReplayWindowSeconds - 1)).ToString();

        var act = async () => await receiver.ReceiveAsync(QueryFor(encrypt, nearBoundary, NewNonce()), body);
        await act.Should().NotThrowAsync("±299s 在容差内（边界 -1s 留出执行抖动余量）");
    }

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenTimestampMissing()
    {
        var (receiver, _) = CreateReceiver();
        var (encrypt, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");
        var nonce = NewNonce();
        var signature = WechatCallbackCrypto.ComputeSignature(Token, string.Empty, nonce, encrypt);

        var act = async () => await receiver.ReceiveAsync($"msg_signature={signature}&nonce={nonce}", body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*timestamp 缺失或非数字*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingTimestamp);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenNonceMissing()
    {
        var (receiver, _) = CreateReceiver();
        var (encrypt, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");
        var timestamp = Now();
        var signature = WechatCallbackCrypto.ComputeSignature(Token, timestamp, string.Empty, encrypt);

        var act = async () => await receiver.ReceiveAsync($"msg_signature={signature}&timestamp={timestamp}", body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*nonce 缺失*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.MissingNonce);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenNonceReplayed()
    {
        var (receiver, _) = CreateReceiver();
        var (encrypt, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");
        var query = QueryFor(encrypt, Now(), NewNonce());

        await receiver.ReceiveAsync(query, body);

        var act = async () => await receiver.ReceiveAsync(query, body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*疑似重放*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.ReplaySuspected);
    }

    // ---------------------------------------------------------------- P1-1 指纹闸后移

    [Fact]
    public async Task ReceiveAsync_ShouldNotConsumeFingerprint_WhenDecryptFails()
    {
        // 错 AESKey 的接收器解密失败（指纹未标记）→ 共享同一守卫的正确接收器同报文应成功：
        // 官方重试（96238）不再被指纹闸拒绝（F2 / P1-1 行为修复）。
        var guard = new InMemoryWechatCallbackReplayGuard();
        var (badReceiver, _) = CreateReceiver(replayGuard: guard, aesKey: OtherAesKey);
        var (goodReceiver, _) = CreateReceiver(replayGuard: guard);

        var (encrypt, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");
        var query = QueryFor(encrypt, Now(), NewNonce());

        var act = async () => await badReceiver.ReceiveAsync(query, body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>();
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.DecryptFailed, "错 AESKey 时解密失败（修复前该路径已消耗指纹）");

        var evt = await goodReceiver.ReceiveAsync(query, body);
        evt.IsSuiteTicket.Should().BeTrue("解密失败不消耗指纹，同报文经正确接收器可成功接收");
    }

    // ---------------------------------------------------------------- receiveid 校验

    [Fact]
    public async Task ReceiveAsync_ShouldReject_WhenReceiveIdMismatch()
    {
        // 套件回调场景：接收方 ID 应为 SuiteId；此处填企业 CorpId 模拟配置错误。
        var receiver = new WechatCallbackReceiver(CreateOptions(corpId: "ww-wrong-receiveid"));

        var (encrypt, body) = EncryptTicket("<xml><InfoType>suite_ticket</InfoType></xml>");

        var act = async () => await receiver.ReceiveAsync(QueryFor(encrypt, Now(), NewNonce()), body);
        var assertion = await act.Should().ThrowAsync<WechatCallbackException>().WithMessage("*receiveid*");
        assertion.Which.Kind.Should().Be(WechatCallbackFailureKind.ReceiveIdMismatch);
    }

    [Fact]
    public async Task ReceiveAsync_ShouldSkipReceiveIdCheck_WhenPlaintextReceiveIdEmpty()
    {
        // P3-2：明文未携带 receiveid（官方「个人主体第三方为空串」形态）时跳过校验并一次性告警。
        var encrypt = WechatCallbackCrypto.Encrypt(AesKey, "<xml><InfoType>suite_ticket</InfoType></xml>", string.Empty);
        var body = $"<xml><Encrypt><![CDATA[{encrypt}]]></Encrypt></xml>";
        var receiver = new WechatCallbackReceiver(CreateOptions());

        var act = async () => await receiver.ReceiveAsync(QueryFor(encrypt, Now(), NewNonce()), body);
        await act.Should().NotThrowAsync("明文 receiveid 为空时跳过校验（个人主体第三方兼容，90968）");
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenReceiverIdMissing()
    {
        // P1-3 统一注册表：接收方 ID 必填（注册期 fail-fast；直构造路径由构造期 Validate 兜底）。
        var act = () => new WechatCallbackReceiver(CreateOptions(corpId: string.Empty));
        act.Should().Throw<InvalidOperationException>().WithMessage("*CorpId*");
    }

    // ---------------------------------------------------------------- 事件分发

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

        (await store.GetAsync("ww-suite")).Should().Be("ticket-1", "suite_ticket 事件应写入仓储并驱动 get_suite_token");
    }

    [Fact]
    public async Task Handler_ShouldNotDeleteAnyAuth_OnCancelAuth_WhenSuiteIdMissing()
    {
        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new Abstractions.Authentication.Models.WechatCorpAuthorization
        {
            AppKey = "suite-app",
            AuthCorpId = "corp-X",
            PermanentCode = "pc-X",
        });

        var appManager = CreateAppManager("suite-app", "ww-suite");
        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), corpAuthStore,
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            AuthCorpId = "corp-X",
        });

        (await corpAuthStore.GetAsync("suite-app", "corp-X")).Should().NotBeNull(
            "P0-3：SuiteId 缺失时无法定位归属应用，宁可不清理也不越权删除");
        appManager.Verify(
            m => m.InvalidateTokenAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string[]?>(), It.IsAny<CancellationToken>()),
            Times.Never, "未命中时不得级联失效任何应用的企业令牌");
    }

    [Fact]
    public async Task Handler_ShouldRemoveCorpAuth_AndInvalidateCorpToken_OnCancelAuth()
    {
        var corpAuthStore = new InMemoryWechatCorpAuthStore();
        await corpAuthStore.SetAsync(new Abstractions.Authentication.Models.WechatCorpAuthorization
        {
            AppKey = "suite-app",
            AuthCorpId = "corp-X",
            PermanentCode = "pc-X",
        });

        var appManager = CreateAppManager("suite-app", "ww-suite");
        appManager
            .Setup(m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-X"), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), corpAuthStore,
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "cancel_auth",
            SuiteId = "ww-suite",
            AuthCorpId = "corp-X",
        });

        (await corpAuthStore.GetAsync("suite-app", "corp-X")).Should().BeNull("取消授权后应清理永久授权码");
        appManager.Verify(
            m => m.InvalidateTokenAsync("suite-app", Abstractions.WechatTokenTypes.AccessToken,
                It.Is<string[]>(s => s[0] == "corp-X"), It.IsAny<CancellationToken>()),
            Times.Once, "取消授权应级联失效该企业的授权企业令牌");
    }

    [Fact]
    public async Task Handler_ShouldInvalidateCorpToken_OnChangeAuth()
    {
        var appManager = CreateAppManager("suite-app", "ww-suite");

        var handler = new WechatCallbackHandler(
            new InMemoryWechatSuiteTicketStore(), new InMemoryWechatCorpAuthStore(),
            NullLogger<WechatCallbackHandler>.Instance, appManager.Object);

        await handler.HandleAsync(new WechatCallbackEvent
        {
            InfoType = "change_auth",
            SuiteId = "ww-suite",
            AuthCorpId = "corp-Y",
        });

        appManager.Verify(
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

        (await store.GetAsync("ww-suite")).Should().BeNull("SuiteTicket 为空的事件不应覆盖仓储");
    }

    /// <summary>
    /// 构造仅暴露配置快照的 <see cref="IWechatAppManager"/>（P1-6 后处理器只读
    /// <c>ConfiguredConfigs</c>，不得依赖 <c>TryGetApp</c> 物化上下文）。
    /// </summary>
    private static Mock<IWechatAppManager> CreateAppManager(string appKey, string suiteId)
    {
        var config = new WechatAppConfig
        {
            AppKey = appKey,
            AppType = WechatAppType.ThirdParty,
            CorpId = "ww-provider",
            ProviderSecret = "provider-secret",
            SuiteId = suiteId,
            SuiteSecret = "suite-secret",
        };

        var appManager = new Mock<IWechatAppManager>();
        appManager.Setup(m => m.ConfiguredAppKeys).Returns(new[] { appKey });
        appManager.Setup(m => m.ConfiguredConfigs).Returns(new[] { config });

        WechatAppConfig? outConfig = config;
        appManager.Setup(m => m.TryGetConfig(appKey, out outConfig)).Returns(true);
        return appManager;
    }
}
